using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using Common.Extensions;

namespace Common
{
    public class MessageQueue : IDisposable
    {
        protected ConnectionFactory factory;
        protected IConnection connection;
        public IModel channel;

        public QueueInteraction queueInteraction;

        public MultiRelationDictionary<string, Services> ServiceExchangeBindings { get; protected set; }

        public EventingBasicConsumer consumer;

        public MessageQueue(string hostname = null, int port = 5672, QueueInteraction interaction = QueueInteraction.Bidirectional)
        {
            hostname = hostname == null ? Constants.Hostname : hostname;
            factory = new ConnectionFactory() { HostName = hostname, Port = port };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            consumer = new EventingBasicConsumer(channel);
            queueInteraction = interaction;

            ServiceExchangeBindings = new MultiRelationDictionary<string, Services>();
        }

        bool ShouldHaveReceive
        {
            get { return queueInteraction != QueueInteraction.Broadcaster; }
        }
        bool ShouldHaveSend
        {
            get { return queueInteraction != QueueInteraction.Listener; }
        }

        public virtual void CreateExchange(RabbitMQExchangeTypes exchangeType, string exchangeName)
        {
            channel.ExchangeDeclare(exchange: exchangeName,
                                    type: exchangeType.ToLower());
            ServiceExchangeBindings.Add(exchangeName);
        }

        public virtual void CreateExchanges(params (RabbitMQExchangeTypes,string)[] exchangeInfo)
        {
            foreach (var info in exchangeInfo)
            {
                CreateExchange(info.Item1, info.Item2);
            }

        }

        public virtual void BindServices(string exchangeName, params Services[] Services)
        {
            string queueName = channel.QueueDeclare().QueueName;

            foreach (var item in Services)
            {
                channel.QueueBind(queue: queueName,
                                  exchange: exchangeName,
                                  routingKey: item.ToString());

                ServiceExchangeBindings.AddRelation(exchangeName, item);
            }
            if (queueInteraction != QueueInteraction.Broadcaster)
            {
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);
            }
        }

        public virtual void Send<T>(T data) where T : NetworkFile
        {
            if (ShouldHaveSend)
            {
                var messageBytes = Json.SerializeToBytes(data);
                foreach (var exchangeName in ServiceExchangeBindings[data.Service])
                {
                    channel.BasicPublish(
                        exchange: exchangeName,
                        routingKey: data.Service.ToString(),
                        body: messageBytes);
                }
            }
            else
            {
                throw new Exception("Queue is listener and should not broadcast.");
            }
            
        }
        public virtual void Send<T>(T data, IBasicProperties props) where T : NetworkFile
        {
            if (ShouldHaveSend)
            {
                var messageBytes = Json.SerializeToBytes(data);
                foreach (var exchangeName in ServiceExchangeBindings[data.Service])
                {
                    channel.BasicPublish(
                        exchange: exchangeName,
                        routingKey: data.Service.ToString(),
                        body: messageBytes);
                }
            }
            else
            {
                throw new Exception("Queue is listener and should not broadcast.");
            }

        }
        public void UnassignOnRecieve(Action<object, BasicDeliverEventArgs> method)
        {
            consumer.Received -= (model, ea) => method(model, ea);
        }

        public void AssignOnRecieve(Action<object, BasicDeliverEventArgs> methodToCall)
        {
            if (ShouldHaveReceive)
            {
                consumer.Received += (model, ea) => methodToCall(model, ea);
            }
            else
            {
                throw new Exception("Queue is broadcaster and should not listen.");
            }
        }

        public void AssignOnRecieve(Action<BasicDeliverEventArgs> methodToCall)
        {
            AssignOnRecieve((model, ea) => methodToCall(ea));
        }

        public void AssignOnRecieve(Action methodToCall)
        {
            AssignOnRecieve((model, ea) => methodToCall());
        }

        public void Dispose()
        {
            connection.Dispose();
            channel.Dispose();
        }
    }
}
