using System.Collections.Generic;

namespace Server
{
    public interface IRepository
    {
        void Add(Client client);
        List<Client> FindAll();
        Client Read(string id);
        bool ExistById(string id);
        void DeleteById(string id);
        void ReWriteById(string id, Client client);
    }
}