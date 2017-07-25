using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using RabbitMQ.Client;
using WorldMessengerLib.WorldMessages;
using Debug = UnityEngine.Debug;

namespace WorldMessengerLib
{
    
    public class Sender
    {
        private string _channel { get; }
        private string _topic { get; }
        private string _exchangeType { get; }
        private IConnection _connection { get; set; }
        private IModel _model  { get; set; }

        public Sender(string channel)
        {
            _channel = channel;
            _topic = StaticStrings.AllTopics;
            _exchangeType = StaticStrings.Fanout;
        }

        public Sender(string channel, string topic)
        {
            _channel = channel;
            _topic = topic;
            _exchangeType = StaticStrings.Topic;
        }

        public void Connect()
        {
            var factory = new ConnectionFactory{HostName = "localhost"};
            _connection = factory.CreateConnection();
            _model = _connection.CreateModel();
            _model.ExchangeDeclare(_channel, _exchangeType);
        }

        public void Disconnect()
        {
            _model.Close();
            _connection.Close();
        }

        public bool SendMessage(IWorldMessage message)
        {
            try
            {
                var body = WorldMessage.ToByteArray(new WorldMessage(message));
                _model.BasicPublish(_channel, _topic, null, body);
                return true;
            }
            catch (Exception e)
            {
                if (_channel == StaticChannelNames.WorldChannel)
                {
                    Debug.Log(string.Format("Exception during Sender.SendMessage: {0}", e));
                }
                else
                {
                    Console.WriteLine($"Exception during Sender.SendMessage: {e}");
                }
                return false;
            }

        }

       
    }

    
}
