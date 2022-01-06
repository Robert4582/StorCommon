using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Common.Extensions
{
    public static class EnumExtensions
    {
        public static T[] All<T>(this T value) where T : Enum
        {
            return (T[])Enum.GetValues(value.GetType());
        }
        public static string ToLower<T>(this T source) where T : Enum//enum
        {

            return source.ToString().ToLower();
        }
    }
    public static class QueueExtensions
    {
        public static Y SendAsRpc<Y,T>(this MessageQueue queue, T fileToSend) where T : NetworkFile where Y: NetworkFile
        {
            BlockingCollection<Y> respQueue = new BlockingCollection<Y>();

            IBasicProperties props = queue.channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();

            props.CorrelationId = correlationId;
            props.ReplyTo = queue.channel.QueueDeclare().QueueName;
            props.CorrelationId = correlationId;
            fileToSend.CorrelationID = correlationId;

            string queueName = queue.channel.BasicConsume(
                consumer: queue.consumer,
                queue: props.ReplyTo,
                autoAck: true);

            static void AddToQueue(object model, BasicDeliverEventArgs ea, string correlationId, BlockingCollection<Y> respQueue)
            {
                var body = ea.Body.ToArray();
                var response = Json.DeserializeFromBytes<Y>(body);
                
                if (response.CorrelationID == correlationId)
                {
                    respQueue.Add(response);
                }
            }

            queue.AssignOnRecieve((x, y) => {
                AddToQueue(x, y, correlationId, respQueue);
                queue.channel.BasicCancel(queueName);
            });

            queue.Send(fileToSend, props);
            return respQueue.Take();
        }


        public static void RespondToRpc<T, Y>(this MessageQueue queue, T fileReceived, Y fileToSend) where T : NetworkFile where Y : NetworkFile
        {
            IBasicProperties props = queue.channel.CreateBasicProperties();

            props.CorrelationId = fileReceived.CorrelationID;
            fileToSend.CorrelationID = fileReceived.CorrelationID;

            queue.Send(fileToSend, props);
        }
    }
}
