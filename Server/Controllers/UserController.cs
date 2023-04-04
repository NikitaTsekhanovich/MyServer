using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Server
{
    public class UserController
    {
        private MySqlUserRepository _repositoryContext { get; }

        public UserController()
        {
            _repositoryContext = new MySqlUserRepository();
        }

        public void Create(User user, HttpListenerResponse response)
        {
            _repositoryContext.Add(user);
            response.StatusCode = (int)HttpStatusCode.Created;
        }
        
        public User ReadUser(string login)
        {
            return _repositoryContext.Read(login);
        }

        public void Delete(string login, HttpListenerResponse response)
        {
            var deleted = _repositoryContext.DeleteByLogin(login);

            if (deleted)
                response.StatusCode = (int)HttpStatusCode.OK;
            else
                response.StatusCode = (int)HttpStatusCode.NotModified;
        }
    }
}