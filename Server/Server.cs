using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Server
{
    class HttpServer
    {
        private bool _running;
        private string _uri;
        private HttpListener _listener;
        private NLog.Logger _logger;
        private ClientController _clientController;
        private UserController _userController;

        public HttpServer(string uri, int port, NLog.Logger logger)
        {
            _uri = $"{uri}:{port}/";
            _running = true;
            _listener = new HttpListener();
            _listener.Prefixes.Add(_uri);
            _logger = logger;
            _clientController = new ClientController(new ClientService());
            _userController = new UserController();
        }

        public void Run()
        {
            _listener.Start();
            var task = StartListeningLoop();
            task.GetAwaiter().GetResult();
            
            _listener.Close();
        }

        private async Task StartListeningLoop()
        {
            _logger.Info("Listeting loop started.");
            while (_running)
            {
                var httpContext = await _listener.GetContextAsync();
                HandleRequest(httpContext);
            }
        }

        private void HandleRequest(HttpListenerContext httpContext)
        {
            var response = httpContext.Response;
            var request = httpContext.Request;

            if (!IsAuthorized(request))
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
            else
            {
                switch (request.HttpMethod)
                {
                    case "GET":
                        HandleGetRequest(response, request);
                        break;
                    case "POST":
                        HandlePostRequest(response, request);
                        break;
                    default:
                        response.StatusCode = (int)HttpStatusCode.NotImplemented;
                        break;
                }
            }

            _logger.Info(
                $"Got request: method: {request.HttpMethod} " +
                $"Path: {request.Url.LocalPath}" +
                $"Client IP: {request.RemoteEndPoint}" 
            );
            response.Close();
        }

        private void HandlePostRequest(HttpListenerResponse response, HttpListenerRequest request)
        {
            var uri = request.Url;
            var localPath = uri.LocalPath;

            switch (localPath)
            {
                case @"/clients":
                    var jsonText = ReadInputData(request);
                    _clientController.Create(JsonSerializer.Deserialize<Client>(jsonText), response);
                    break;
                case @"/users":
                    jsonText = ReadInputData(request);
                    _userController.Create(JsonSerializer.Deserialize<User>(jsonText), response);
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.NotImplemented;
                    break;
            }
        }

        private string ReadInputData(HttpListenerRequest request)
        {
            var text = "";

            using (var reader = new StreamReader(request.InputStream, Encoding.UTF8))
            {
                text = reader.ReadToEnd();
            }

            return text;
        }

        private void HandleGetRequest(HttpListenerResponse response, HttpListenerRequest request)
        {
            
            
            var uri = request.Url;
            var localPath = uri.LocalPath;

            if (uri.LocalPath == "/")
                return;
            
            switch (uri.Segments[1])
            {
                case "clients/":
                    if (uri.Segments.Length > 2)
                    {
                        _clientController.ReadSingleClient(uri.Segments[2], response);
                        _logger.Info($"Tried to read client:{uri.Segments[2]}");
                    }
                    else
                        response.StatusCode = (int)HttpStatusCode.Conflict;
                    break;
            }
        }

        private bool IsAuthorized(HttpListenerRequest request)
        {
            var headers = request.Headers;
            var authPair = headers.Get("Auth");

            if (authPair == null)
                return false;

            var pairArray = authPair.Split('=');
            var login = pairArray[0];
            var password = pairArray[1];

            var user = _userController.ReadUser(login);

            if (user == null)
                return false;

            return user.Password == password;
        }
    }
}