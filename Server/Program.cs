using System;
using System.Net;
using System.Threading.Tasks;
using NLog;

namespace Server
{
    class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static void Main(string[] args)
        {
            ConfigureLogger();
            new HttpServer("http://*", 8080, Logger).Run();
            NLog.LogManager.Shutdown();
        }

        private static void ConfigureLogger()
        {
            var config = new NLog.Config.LoggingConfiguration();
            
            var logfile = new NLog.Targets.FileTarget("logfile")
            {
                FileName = "logs.txt", 
                DeleteOldFileOnStartup = true
            };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            
        
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            
        
            LogManager.Configuration = config;
        }
        
        static void ParseIPAddress(string addressString)
        {
            try
            {
                var address = IPAddress.Parse(addressString);
                Console.WriteLine($"The address is {address} and is of the family {address.AddressFamily}.");

                if (IPAddress.IsLoopback(address))
                {
                    Console.WriteLine("Address is loopback");
                }
                else
                    Console.WriteLine("Address is not loopback");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Failure parsing " + addressString + ". " + ex.ToString());
            }
        }
    }
}