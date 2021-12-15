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
    }
}