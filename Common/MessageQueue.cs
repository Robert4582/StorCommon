using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class MessageQueue : IDisposable
    {
        protected ConnectionFactory factory;
        protected IConnection connection;
        protected IModel channel;

        public QueueInteraction queueInteraction;

        public MultiRelationDictionary<string, Services> ServiceExchangeBindings { get; protected set; }

        public EventingBasicConsumer consumer;

        public MessageQueue(string hostname = null, QueueInteraction interaction = QueueInteraction.Bidirectional)
        {
            hostname = hostname == null ? Constants.Hostname : hostname;
            factory = new ConnectionFactory() { HostName = hostname };
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

        public virtual void Send(NetworkFile data)
        {
            if (queueInteraction != QueueInteraction.Listener)
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

        public void AssignOnRecieve(Action methodToCall)
        {
            if (ShouldHaveReceive)
            {
                consumer.Received += (model, ea) => methodToCall();
            }
            else
            {
                throw new Exception("Queue is broadcaster and should not listen.");
            }
        }

        public void AssignOnRecieve(Action<BasicDeliverEventArgs> methodToCall)
        {
            if (ShouldHaveReceive)
            {
                consumer.Received += (model, ea) => methodToCall(ea);
            }
            else
            {
                throw new Exception("Queue is broadcaster and should not listen.");
            }
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

        public void Dispose()
        {
            connection.Dispose();
            channel.Dispose();
        }
    }
}
