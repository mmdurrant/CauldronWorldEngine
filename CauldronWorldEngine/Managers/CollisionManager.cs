using System.Collections.Generic;
using System.Threading;
using CollisionEngineLib;
using CollisionEngineLib.Objects;
using Microsoft.Xna.Framework;
using WorldMessengerLib;
using WorldMessengerLib.WorldMessages;

namespace CauldronWorldEngine.Managers
{
    public class CollisionManager
    {
        public int DefaultMaxTimes { get; set; } = 16;
        private Dictionary<string, CollisionEngine> WorldTiles { get; set; } = new Dictionary<string, CollisionEngine>();
        private Receiver PositionReceiver { get; set; } = new Receiver(StaticChannelNames.CollisionChannel);
        private readonly object _collisionMessageLock = new object();
        private bool _readingMessages;
        private bool _started;

        public CollisionManager()
        {

        }

        public void Start()
        {
            if (_started) return;
            _started = true;
            PositionReceiver.Connect();
            var thread = new Thread(() =>
            {
                while (_started)
                {
                    ReadCollisionMessages();
                    Thread.Sleep(100);
                }
            });
            thread.Start();
        }

        public void Stop()
        {
            _started = false;
        }

        public bool AddWorldTile(string worldName, Vector2 size)
        {
            if (WorldTiles.ContainsKey(worldName)) return false;
            WorldTiles.Add(worldName, new CollisionEngine(new QuadTree(size, DefaultMaxTimes), worldName));
            var thread = new Thread(() =>
            {
                WorldTiles[worldName].Start();
                WorldTiles[worldName].Update();
            });
            thread.Start();
            return true;
        }



        public bool RemoveWorldTile(string worldName)
        {
            WorldTiles[worldName].Stop();
            WorldTiles[worldName] = null;
            return WorldTiles.Remove(worldName);
        }

        public bool SetWorldTileSize(string worldName, Vector2 size)
        {
            WorldTiles[worldName].Stop();
            WorldTiles[worldName].ResizeWorld(size);
            WorldTiles[worldName].Start();
            return true;
        }

        public bool MoveObject(string obj, string worldTile, Vector2 position)
        {
            if (WorldTiles.ContainsKey(worldTile) && WorldTiles[worldTile].DoesItemExist(obj))
            {
                WorldTiles[worldTile].Move(obj, position);
                return true;
            }
            return false;
        }

        
        public bool AddObject(Collidable obj, Vector2 size, Vector2 position, string worldTile)
        {
            if (WorldTiles.ContainsKey(worldTile) && !WorldTiles[worldTile].DoesItemExist(obj.ColliderId))
            {
                WorldTiles[worldTile].Add(new QuadTreePositionItem(obj, position, size));
                return true;
            }
            return false;
        }

        public bool RemoveObject(string obj, string worldTile)
        {
            return WorldTiles.ContainsKey(worldTile) && WorldTiles[worldTile].Remove(obj);
        }

        private void ReadCollisionMessages()
        {
            if (!_readingMessages)
            {
                _readingMessages = true;
                lock (_collisionMessageLock)
                {
                    var thread = new Thread(() =>
                    {
                        while (PositionReceiver.Messages.Count > 0)
                        {
                            var msg = PositionReceiver.Messages.Dequeue();
                            switch (msg.MessageType)
                            {
                                case WorldMessageType.Position:
                                    var pos = msg.ReadMessage<ObjectPositionMessage>();
                                    if (WorldTiles.ContainsKey(pos.WorldTile)) 
                                    {
                                        if (WorldTiles[pos.WorldTile].DoesItemExist(pos.ColliderId))
                                        {
                                            WorldTiles[pos.WorldTile].Move(pos.ColliderId, StaticConversionMethods.ToMicrosoftVector2(pos.Position));
                                        }
                                    }
                                    break;
                                case WorldMessageType.AddCollisionEngine:
                                    var add = msg.ReadMessage<AddCollisionEngineMessage>();
                                    AddWorldTile(add.Name, StaticConversionMethods.ToMicrosoftVector2(add.Size));
                                    break;
                                case WorldMessageType.RemoveCollisionEngine:
                                    var remove = msg.ReadMessage<RemoveCollisionEngineMessage>();
                                    RemoveWorldTile(remove.Name);
                                    break; 
                            }
                        }
                    });
                    thread.Start();
                }
            }
        }
        
    }
}