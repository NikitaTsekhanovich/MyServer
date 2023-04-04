using System.Collections.Generic;
using NUnit.Framework;
using Server;

namespace Server_Tests
{
    [TestFixture]
    public class DatabaseTests
    {
        public MySqlUserContext DbUserContext;
        public MySqlClientContext DbClientContext;
        public MySqlClientRepository DbClientRepository;
        public MySqlUserRepository DbUserRepository;
        public Client DefaultClient;

        [SetUp]
        public void Setup()
        {
            DbUserContext = new MySqlUserContext();
            DbClientContext = new MySqlClientContext();

            var clientCars = new List<Car>();
            clientCars.Add(new Car { Id = "AA908A", ClientId = "75693475", Model = "Lada 2111", Color = "Red" });

            var clientOperations = new List<Operation>();
            clientOperations.Add(
                new Operation
                {
                    Id = "321",
                    ClientId = "75693475",
                    Name = "Coloring",
                    Price = "1000rub",
                    Status = "Ready",
                    Comment = ""
                }
            );

            DbClientRepository = new MySqlClientRepository();
            DbUserRepository = new MySqlUserRepository();
            DefaultClient = new Client
            {
                Id = "75693475",
                Name = "Ivan",
                Surname = "Ivanov",
                Patronymic = "Ivanovich",
                Registration = "Ekaterinburg city",
                Cars = clientCars,
                Operations = clientOperations
            };
        }

        [Test]
        public void AddDefaultClient()
        {
            DbClientRepository.Add(DefaultClient);
            Assert.True(true);
        }

        [Test]
        public void DeleteDataBase()
        {
            DbClientContext.DeleteData();
            Assert.True(true);
        }

        [Test]
        public void AddDefaultUser()
        {
            var user = new User { Login = "dragon4738", Password = "49038l" };
            DbUserRepository.Add(user);
        }
    }
}