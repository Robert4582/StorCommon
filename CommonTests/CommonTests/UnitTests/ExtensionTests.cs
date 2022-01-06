using NUnit.Framework;
using Common;
using Common.Extensions;

namespace CommonTests.UnitTests
{
    public class ExtensionTests
    {
        [Test]
        public void CanGetLowerEnumName()
        {
            //Arrange
            Services service = Services.Create;
            string expected = "create";

            //Act
            string actual = service.ToLower();

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CanGetAllEnums()
        {
            //Arrange
            RabbitMQExchangeTypes[] expected = new RabbitMQExchangeTypes[] {
                RabbitMQExchangeTypes.Direct,
                RabbitMQExchangeTypes.Fanout,
                RabbitMQExchangeTypes.Headers,
                RabbitMQExchangeTypes.Topic};

            //Act
            var actual = RabbitMQExchangeTypes.Direct.All();

            //Assert
            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}