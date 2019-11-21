using System;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace ActionFramework.Logger
{
    public class LogService
    {
        private static ILogger logger = null;

        public LogService()
        {
            EnsureInitialized();
        }

        private static void Configure()
        {
            //var configuration = ConfigurationManager.Settings.GetSection("Serilog");
            
            var configuration = new ConfigurationBuilder()
              .AddJsonFile("LogSettings.json")
              .Build();
            //Log.Logger = new LoggerConfiguration()
            logger = new LoggerConfiguration()
           //.Enrich.FromLogContext()
           //.WriteTo.Console()
           .ReadFrom.Configuration(configuration)

           //.WriteTo.Console(new RenderedCompactJsonFormatter())
           //.WriteTo.File(new RenderedCompactJsonFormatter(), @"c:\temp\log.json")
           //.WriteTo.File(@"c:\temp\log.json")

           /*
           .WriteTo.Graylog(new GraylogSinkOptions()
           {
               HostnameOrAddress = ConfigurationManager.Settings["AgentSettings:GraylogUrl"], //"3lkjk6.stackhero-network.com", //3lkjk6.stackhero-network.com
               Port = 12201,
               MinimumLogEventLevel = Serilog.Events.LogEventLevel.Verbose,
               Facility = ConfigurationManager.Settings["AgentSettings:GraylogFacility"]//"Development"
           })
           */

           //.WriteTo.File(new RenderedCompactJsonFormatter(), "/Logs/log.ndjson")
           //.WriteTo.Console(new RenderedCompactJsonFormatter())
           .CreateLogger();

            Serilog.Debugging.SelfLog.Enable(Console.Error);

            //return Log.Logger;
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
