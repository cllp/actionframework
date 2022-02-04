using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace ActionFramework.Logger
{
    public class LogService
    {
        private static ILogger logger = null;
        private readonly IConfiguration _configuration;

        public LogService(IConfiguration configuration)
        {
            _configuration = configuration;
            //EnsureInitialized();
        }

        private static void Configure()
        {
            var configuration = new ConfigurationBuilder()

            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile(string.Format("appsettings.{0}.json", ActionFramework.Configuration.ConfigurationManager.Environment), true)
            .Build();

            logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentUserName()
            //.Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            //.Serilog.Enrichers.AspnetcoreHttpcontext()
            //.Enrich.WithCorrelationId()
            .CreateLogger();
            
            Serilog.Debugging.SelfLog.Enable(Console.Error);
        }

        private static void EnsureInitialized()
        {
            if (!IsInitialized)
            {
                Configure();
            }
        }

        public static ILogger Logger
        {
            get
            {
                EnsureInitialized();
                return logger;
            }
        }

        public static bool IsInitialized
        {
            get { return logger != null; }
        }
    }
}
