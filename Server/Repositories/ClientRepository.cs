using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using NLog;

namespace Server
{
    public class MySqlClientContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }

        public MySqlClientContext()
        {
            Database.EnsureCreated();
        }

        public void DeleteData()
        {
            Database.EnsureDeleted();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder opBuilder)
        {
            opBuilder.UseMySql(
                @"server=localhost;database=avtovek;uid=root;password=0000;",
                new MySqlServerVersion(new Version(8, 0, 30))
            );
        }
    }

    public class MySqlClientRepository : IRepository
    {
        private Logger _logger;

        public MySqlClientRepository()
        {
            _logger = LogManager.GetCurrentClassLogger();
            ConfigureLogger();
        }
        
        private void ConfigureLogger()
        {
            var config = new NLog.Config.LoggingConfiguration();
            
            var logfile = new NLog.Targets.FileTarget("logfile")
            {
                FileName = "database_logs.txt", 
                DeleteOldFileOnStartup = true
            };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            
        
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            
        
            LogManager.Configuration = config;
        }
        
        public void Add(Client client)
        {
            using (var context = new MySqlClientContext())
            {
                context.Clients.Add(client);
                context.SaveChanges();
                _logger.Info($"Added client: {client.Id}");
            }
        }
        
        public List<Client> FindAll()
        {
            List<Client> clients;
            
            using (var context = new MySqlClientContext())
            {
                clients = context.Clients.ToList();
                _logger.Info($"Return clients");
            }

            return clients;
        }

        public Client Read(string id)
        {
            Client client;
            
            using (var context = new MySqlClientContext())
            {
                var clients = context.Clients.Include(cl => cl.Cars).Include(cl => cl.Operations);
                client = clients.FirstOrDefault(cl => cl.Id == id);
                _logger.Info($"Read client: {id}");
            }

            return client;
        }

        public bool ExistById(string id)
        {
            Client client;
            
            using (var context = new MySqlClientContext())
            {
                client = context.Clients.First(cl => cl.Id == id);
                _logger.Info($"Check client: {client.Id}");
            }

            return client != null;
        }

        public void DeleteById(string id)
        {
            using (var context = new MySqlClientContext())
            {
                var client = context.Clients.First(cl => cl.Id == id);
                context.Clients.Remove(client);
                _logger.Info($"Delete client: {client.Id}");
            }
        }

        public void ReWriteById(string id, Client client)
        {
            using (var context = new MySqlClientContext())
            {
                var oldClient = context.Clients.First(cl => cl.Id == id);
                context.Clients.Remove(oldClient);
                context.Clients.Add(client);
                _logger.Info($"Update client: {client.Id}");
            }
        }
    }
}