using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using NLog;

namespace Server
{
    public class MySqlUserContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public MySqlUserContext()
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
                @"server=localhost;database=avtovek_users;uid=root;password=0000;",
                new MySqlServerVersion(new Version(8, 0, 30))
            );
        }
    }
    
    public class MySqlUserRepository 
    {
        private Logger _logger;

        public MySqlUserRepository()
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
        
        public void Add(User user)
        {
            using (var context = new MySqlUserContext())
            {
                context.Users.Add(user);
                context.SaveChanges();
                _logger.Info($"Added user: {user.Login}");
            }
        }

        public User Read(string login)
        {
            User client;
            
            using (var context = new MySqlUserContext())
            {
                var clients = context.Users;

                client = clients.FirstOrDefault(usr => usr.Login == login);
                _logger.Info($"Read user: {login}");
            }

            return client;
        }

        public bool ExistById(string login)
        {
            User user;
            
            using (var context = new MySqlUserContext())
            {
                user = context.Users.First(usr => usr.Login == login);
                _logger.Info($"Check user: {user.Login}");
            }

            return user != null;
        }

        public bool DeleteByLogin(string login)
        {
            using (var context = new MySqlUserContext())
            {
                var user = context.Users.First(usr => usr.Login == login);
                context.Users.Remove(user);
                _logger.Info($"Delete user: {user.Login}");
            }

            return true;
        }
    }
}