using NUnit.Framework;
using Common;
using System.Collections.Generic;
using System.Threading;

namespace CommonTests.IntegrationTests
{
    public class MessageQueueTests
    {
        MessageQueue Q1;
        MessageQueue Q2;

        [SetUp]
        public void Setup()
        {
            //Arrange
            Q1 = new MessageQueue("host.docker.internal:5672");
            Q2 = new MessageQueue("host.docker.internal:5672");

        }
        [TearDown]
        public void TearDown()
        {
            //Annihilate
            Q1.Dispose();
            Q2.Dispose();
            
        }


        [Test]
        public void CanReceiveMessage()
        {
            //Arrange
            var file = new NetworkFile() { Service = Services.GlobalMessage };
            bool HasRecieved = false;


            //Act
            Q1.CreateExchange(RabbitMQExchangeTypes.Direct, "TestExchange");
            Q2.CreateExchange(RabbitMQExchangeTypes.Direct, "TestExchange");

            Q1.BindServices("TestExchange", Services.GlobalMessage);
            Q2.BindServices("TestExchange", Services.GlobalMessage);


            Q2.AssignOnRecieve(() => HasRecieved = true);

            Q1.Send(file);
            Thread.Sleep(20);

            //Assert
            Assert.IsTrue(HasRecieved);
        }

        [Test]
        public void DoesNotInterceptOwnMessage()
        {
            //Arrange
            Q1.queueInteraction = QueueInteraction.Broadcaster;
            var file = new NetworkFile() { Service = Services.GlobalMessage };
            bool HasSelfRecieved = false;


            //Act
            Q1.CreateExchange(RabbitMQExchangeTypes.Direct, "TestExchange");
            Q2.CreateExchange(RabbitMQExchangeTypes.Direct, "TestExchange");

            Q1.BindServices("TestExchange", Services.GlobalMessage);
            Q2.BindServices("TestExchange", Services.GlobalMessage);

            Q1.AssignOnRecieve(() => HasSelfRecieved = true);

            Q1.Send(file);
            Thread.Sleep(20);

            //Assert
            Assert.IsFalse(HasSelfRecieved);
        }
    }
}