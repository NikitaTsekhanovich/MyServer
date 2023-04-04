using System.Collections.Generic;

namespace Server
{
    public interface IClientService
    {
        void Create(Client client);
        List<Client> ReadAll();
        Client Read(string id);
        bool Update(Client client, string id);
        bool Delete(string id);
    }
}