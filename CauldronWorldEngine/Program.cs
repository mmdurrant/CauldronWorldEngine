using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorldMessengerLib;

namespace CauldronWorldEngine
{
    class Program
    {
        private static WorldEngine WorldEngine;

        static void Main(string[] args)
        {
            WorldEngine = new WorldEngine();
            while (ReadUserInput())
            {
                Thread.Sleep(100);
            }
        }

        private static bool ReadUserInput()
        {
            var input = Console.ReadLine().Split(' ');
            var command = input[0].ToLower();
            switch (command)
            {
                case "start":
                    var engine = new Thread(() =>
                    {
                        WorldEngine.Start();
                    });
                    engine.Start();
                    return true;
                case "end":
                    return false;
                case "save":
                    if (input.Length >= 2)
                    {
                        var savePath = input[1];
                        WorldEngine.SaveData(savePath);
                    }
                    else
                    {
                        WorldEngine.SaveData(WorldEngine.DefaultSavePath);
                    }
                    return true;
                case "load":
                    if (input.Length >= 2)
                    {
                        var loadPath = input[1];
                        WorldEngine.LoadSettings(loadPath);
                        WorldEngine.LoadData(loadPath);
                    }
                    else
                    {
                        WorldEngine.LoadSettings(WorldEngine.DefaultSavePath);
                        WorldEngine.LoadData(WorldEngine.DefaultSavePath);
                    }
                    return true;
                default:
                    return true;
            }

        }


    }
}
