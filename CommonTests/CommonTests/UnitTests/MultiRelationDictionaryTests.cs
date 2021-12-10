using NUnit.Framework;
using Common;
using System.Collections.Generic;

namespace CommonTests.UnitTests
{
    public class MultiRelationDictionaryTests
    {
        MultiRelationDictionary<string, Services> bindings;
        
        [SetUp]
        public void Setup()
        {
            //Arrange
            bindings = new MultiRelationDictionary<string,Services>();
        }


        [Test]
        public void CanAddNameToEmpty()
        {
            //Act
            bindings.Add("LoginName");
        }
        [Test]
        public void CanAddServiceToEmpty()
        {
            //Act
            bindings.Add(Services.Login);
        }
        [Test]
        public void CanAddRelationToEmpty()
        {
            //Act
            bindings.AddRelation("LoginName", Services.Login);
        }
        [Test]
        public void CanAddRelationToExistingName()
        {
            //Act
            bindings.Add("LoginName");
            bindings.AddRelation("LoginName", Services.Login);
        }
        [Test]
        public void CanAddRelationToExistingService()
        {
            //Act
            bindings.Add(Services.Login);
            bindings.AddRelation("LoginName", Services.Login);
        }
        [Test]
        public void CanAddMultipleServiceRelationToExistingName()
        {
            //Act
            bindings.Add("LoginName");
            bindings.AddRelations("LoginName", Services.Login, Services.Create);
        }
        [Test]
        public void CanAddMultipleServiceRelationToExistingService()
        {
            //Act
            bindings.Add(Services.Login);
            bindings.AddRelations("LoginName", Services.Login, Services.Create);
        }
        [Test]
        public void CanAddMultipleNameRelationToExistingName()
        {
            //Act
            bindings.Add("LoginName");
            bindings.AddRelations(Services.Login, "LoginName", "DBName");
        }
        [Test]
        public void CanAddMultipleNameRelationToExistingService()
        {
            //Act
            bindings.Add(Services.Login);
            bindings.AddRelations(Services.Login, "LoginName", "DBName");
        }
        [Test]
        public void CanGetEmptyByName()
        {
            //Act
            bindings.Add("LoginName");

            //Assert
            Assert.AreEqual(new List<Services>(), bindings["LoginName"]);
        }

        [Test]
        public void CanGetEmptyByService()
        {
            //Act
            bindings.Add(Services.Login);

            //Assert
            Assert.AreEqual(new List<string>(), bindings[Services.Login]);
        }

        [Test]
        public void CanGetAddedRelationsByName()
        {
            //Act
            bindings.AddRelation("LoginName", Services.Login);
            bindings.AddRelations("LoginName", Services.Login, Services.Create);

            //Assert
            Assert.AreEqual(Services.Login, bindings["LoginName"][0]);
            Assert.AreEqual(Services.Create, bindings["LoginName"][1]);
        }

        [Test]
        public void CanGetAddedRelationsByService()
        {
            //Act
            bindings.AddRelation("LoginName", Services.Login);
            bindings.AddRelations(Services.Login, "LoginName", "DBName");

            //Assert
            Assert.AreEqual("LoginName", bindings[Services.Login][0]);
            Assert.AreEqual("DBName", bindings[Services.Login][1]);
        }

        [Test]
        public void CanRemoveAddedRelation()
        {
            //Act
            bindings.AddRelation("LoginName", Services.Login);
            bindings.AddRelations(Services.Login, "LoginName", "DBName");
            bindings.RemoveRelation(Services.Login, "LoginName");

            //Assert
            Assert.AreEqual("DBName", bindings[Services.Login][0]);
        }
    }
}