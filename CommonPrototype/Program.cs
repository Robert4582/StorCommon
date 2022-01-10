using System;
using Common;
using Common.Extensions;

namespace CommonPrototype
{
    internal class Program
    {
        static void Main(string[] args)
        {
            NetworkFile<string> file;
            switch (Console.ReadLine())
            {
                case "1":
                    MessageQueue Q1 = new MessageQueue("localhost", 5672, QueueInteraction.Bidirectional);
                    Console.WriteLine("Starting RPC Send");

                    Q1.CreateExchange(RabbitMQExchangeTypes.Direct, "TestExchange");
                    Q1.BindServices("TestExchange", Services.GlobalMessage, Services.PrivateMessage);
                    while (true)
                    {
                        file = new NetworkFile<string>() { Service = Services.GlobalMessage, Info = Console.ReadLine() };


                        var response = Q1.SendAsRpc<NetworkFile<string>, NetworkFile<string>>(file);
                        Console.WriteLine($"Received {response.Info}");
                    }
                    break;

                case "2":
                    MessageQueue Q2 = new MessageQueue("localhost", 5672, QueueInteraction.Bidirectional);
                    Console.WriteLine("Starting RPC Receive");
                    Q2.CreateExchange(RabbitMQExchangeTypes.Direct, "TestExchange");
                    Q2.BindServices("TestExchange", Services.GlobalMessage);

                    Q2.AssignOnRecieve((ea) => {
                        var receival = Json.DeserializeFromMemory<NetworkFile<string>>(ea.Body);
                        var toSend = new NetworkFile<string>() { Info = receival.Info,Service = Services.PrivateMessage};
                        Console.WriteLine($"Received {receival.Info} on {receival.CorrelationID}");
                        Q2.RespondToRpc(receival, toSend);
                    });
                    break;

                default:
                    MessageQueue Q3 = new MessageQueue("localhost", 1122, QueueInteraction.Bidirectional);
                    file = new NetworkFile<string>() { Service = Services.GlobalMessage, Info = "Sending Message!" };

                    Q3.CreateExchange(RabbitMQExchangeTypes.Direct, "TestExchange");
                    Q3.BindServices("TestExchange", Services.GlobalMessage);
                    Q3.AssignOnRecieve(() => Console.WriteLine($"Message : \"{file.Info}\", Received!"));

                    Q3.Send(file);
                    break;
            }
            Console.ReadKey();
        }
    }
}
