using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Server
{
    public class ClientController
    {
        private IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        public void Create(Client client, HttpListenerResponse response)
        {
            _clientService.Create(client);
            response.StatusCode = (int)HttpStatusCode.Created;
        }

        public async void ReadAll(HttpListenerResponse response)
        {
            var clients = _clientService.ReadAll();
            if (clients != null && clients.Count != 0)
            {
                var content = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(clients));
                response.StatusCode = (int)HttpStatusCode.OK;
                await response.OutputStream.WriteAsync(content, 0, content.Length);
            }
            else
                response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        public async void ReadSingleClient(string id, HttpListenerResponse response)
        {
            var client = _clientService.Read(id);

            if (client != null)
            {
                var content = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(client));
                response.StatusCode = (int)HttpStatusCode.OK;
                await response.OutputStream.WriteAsync(content, 0, content.Length);
            }
            else
                response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        public void Update(string id, Client client, HttpListenerResponse response)
        {
            var updated = _clientService.Update(client, id);

            if (updated)
                response.StatusCode = (int)HttpStatusCode.OK;
            else
                response.StatusCode = (int)HttpStatusCode.NotModified;
        }

        public void Delete(string id, HttpListenerResponse response)
        {
            var deleted = _clientService.Delete(id);

            if (deleted)
                response.StatusCode = (int)HttpStatusCode.OK;
            else
                response.StatusCode = (int)HttpStatusCode.NotModified;
        }
    }
}