using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WorldMessengerLib.WorldMessages;

namespace WorldMessengerLib
{
    
    public class Receiver
    {
        public Queue<WorldMessage> Messages { get; private set; }

        private string _channel { get; }
        private string _topic { get; }
        private string _exchangeType { get; }
        private IConnection _connection { get; set; }
        private IModel _model { get; set; }
        private EventingBasicConsumer _consumer { get; set; }

        public Receiver(string channel)
        {
            _channel = channel;
            _exchangeType = StaticStrings.Fanout;
            _topic = StaticStrings.AllTopics;
            Messages = new Queue<WorldMessage>();
        }

        public Receiver(string channel, string topic)
        {
            _channel = channel;
            _exchangeType = StaticStrings.Topic;
            _topic = topic;
            Messages = new Queue<WorldMessage>();
        }

        public void Connect()
        {
            var factory = new ConnectionFactory {HostName = "localhost"};
            _connection = factory.CreateConnection();
            _model = _connection.CreateModel();
            _model.ExchangeDeclare(_channel, _exchangeType);
            var queueName = _model.QueueDeclare().QueueName;
            _model.QueueBind(queueName, _channel,_topic, null);
            _consumer = new EventingBasicConsumer();
            _consumer.Received += (sender, args) =>
            {
                Messages.Enqueue(WorldMessage.Deserialize<WorldMessage>(args.Body));
            };
            _model.BasicConsume(queueName, true, _consumer);
        }
    }
}