using System;
using Common;

namespace CommonPrototype
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MessageQueue Q1 = new MessageQueue("localhost");
            var file = new NetworkFile() { Service = Services.GlobalMessage };

            Q1.CreateExchange(RabbitMQExchangeTypes.Direct, "TestExchange");

            Q1.BindServices("TestExchange", Services.GlobalMessage);

            Q1.AssignOnRecieve(() => Console.WriteLine("Message Received!"));

            Q1.Send(file);
            Console.ReadKey();
        }
    }
}
