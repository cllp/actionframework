using ActionFramework.Configuration;
//using Microsoft.Extensions.Logging;
using Serilog.Configuration;
using Serilog;
using Serilog.Sinks.Graylog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

namespace ActionFramework.Logger
{
    public sealed class LogService<T>
    {
        //public readonly ILogger<T> Log;

        private LogService()
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettingsDev.json", optional: false, reloadOnChange: true)
            .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .CreateLogger();


            .WriteTo.Graylog(new GraylogSinkOptions()
            {
                HostnameOrAddress = ConfigurationManager.Settings["AgentSettings:GraylogUrl"], //"3lkjk6.stackhero-network.com", //3lkjk6.stackhero-network.com
                Port = 12201,
                MinimumLogEventLevel = Serilog.Events.LogEventLevel.Verbose,
                Facility = ConfigurationManager.Settings["AgentSettings:GraylogFacility"]//"Development"
            });

            
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                    .AddConsole()
                    //.AddConfiguration(loggerConfiguration)
                    //.AddConfiguration(ConfigurationManager.GetSection("Serilog"))
                    /*
                    .AddGelf(options =>
                    {
                        options.Host = "graylog-hostname";
                        options.LogSource = "application-name";
                    });
                    */
                ;
                    
                //.AddEventLog();
            });
                

            

            //Log = loggerFactory.CreateLogger<T>();
            //Log.LogInformation("LogService Initiated");
        }

        public static LogService<T> Instance { get { return Nested.instance; } }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly LogService<T> instance = new LogService<T>();
        }
    }

   
}
