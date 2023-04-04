using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;
using Newtonsoft.Json;
using NUnit.Framework;
using Server;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Server_Tests
{
    [TestFixture]
    public class ConnectionTests
    {
        public string Url;
        public string PostUrl;
        public string ClientGetUrl;
        private HttpMessageHandler _handler;

        [SetUp]
        public void Setup()
        {
            ClientGetUrl = "http://127.0.0.1:8080/clients/75693475";
            Url = "http://127.0.0.1:8080";
            PostUrl = "http://127.0.0.1:8080/clients";
            _handler = new HttpClientHandler();
        }

        [Test]
        public void TestConnectionSetting()
        {
            Assert.AreEqual(200, SetConnection().GetAwaiter().GetResult());
        }

        private async Task<int> SetConnection()
        {
            var client = new HttpClient(_handler, false);
            var res = await client.GetAsync(Url); 
            client.Dispose();

            return (int)res.StatusCode;
        }

        [Test]
        public void TestMakeMultibleRequests()
        {
            var times = 10;
            var tasks = new Task<int>[times];
            
            for (var i = 0; i < times; i++)
                tasks[i] = SetConnection();

            Task.WaitAll(tasks);

            foreach (var task in tasks)
                Assert.AreEqual(200, task.Result);
        }

        [Test]
        public void GetSomeInfo()
        {
            var task = MakeGetRequest();
            var (response, text) = task.GetAwaiter().GetResult();

            Console.WriteLine(
                $"Get response: {response.Content}\n" +
                $"Get text: {text}"
            );
            
            Assert.True(true);
        }
        
        private async Task<(HttpResponseMessage, string)> MakeGetRequest()
        {
            var client = new HttpClient(_handler, false);
            var response = await client.GetAsync(ClientGetUrl);

            var text = await response.Content.ReadAsStringAsync();
            client.Dispose();

            return (response, text);
        }

        [Test]
        public void PostSomeInfo()
        {
            var operations = new List<Operation>();
            operations.Add(
                new Operation
                {
                    Id = "432", 
                    Name = "Oil exchange", 
                    Price = "600rub",
                    Status = "In Process",
                    Comment = "Yep."
                });
            operations.Add(
                new Operation
                {
                    Id = "44232", 
                    Name = "Wheels", 
                    Price = "60000rub",
                    Status = "In Process",
                    Comment = "Yep. Yep. Yep."
                });

            var cars = new List<Car>();
            cars.Add(new Car
            {
                Id = "K894JJ", 
                ClientId = "43278", 
                Color = "Blue", 
                Model = "Land Cruiser    Lada"
            });


            var clienTwo = new Client
            {
                Id = "43278",
                Name = "Andrey",
                Surname = "Lyatipov",
                Patronymic = "Igorevich",

                Registration = "Revda",
                Operations = operations,
                Cars = cars
            };
            
            var client = new HttpClient(_handler, false);
            var httpContent = new StringContent(JsonSerializer.Serialize(clienTwo));
            var postResp = client.PostAsync(PostUrl, httpContent).GetAwaiter().GetResult();
            client.Dispose();
            
            Assert.AreEqual(201, (int)postResp.StatusCode);
        }
    }
}