using System;
using Common;
using Common.Extensions;

namespace CommonPrototype
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MessageQueue Q1 = new MessageQueue("localhost");
            NetworkFile<string> file;
            switch (Console.ReadLine())
            {
                case "1":
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
                    Console.WriteLine("Starting RPC Receive");
                    Q1.CreateExchange(RabbitMQExchangeTypes.Direct, "TestExchange");
                    Q1.BindServices("TestExchange", Services.GlobalMessage);

                    Q1.AssignOnRecieve((ea) => {
                        var receival = Json.DeserializeFromMemory<NetworkFile<string>>(ea.Body);
                        var toSend = new NetworkFile<string>() { Info = receival.Info,Service = Services.PrivateMessage};
                        Console.WriteLine($"Received {receival.Info} on {receival.CorrelationID}");
                        Q1.RespondToRpc(receival, toSend);
                    });
                    break;

                default:
                     file = new NetworkFile<string>() { Service = Services.GlobalMessage, Info = "Sending Message!" };

                    Q1.CreateExchange(RabbitMQExchangeTypes.Direct, "TestExchange");
                    Q1.BindServices("TestExchange", Services.GlobalMessage);
                    Q1.AssignOnRecieve(() => Console.WriteLine($"Message : \"{file.Info}\", Received!"));

                    Q1.Send(file);
                    break;
            }
            Console.ReadKey();
        }
    }
}
