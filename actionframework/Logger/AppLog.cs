using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActionFramework.Logger
{
    public class AppLog
    {
        private static ILoggerFactory loggerFactory = null;

        public static ILoggerFactory Factory
        {
            get
            {
                if (loggerFactory == null)
                {
                    loggerFactory = LoggerFactory.Create(builder =>
                    {
                        builder
                            //.AddFilter("Microsoft", LogLevel.Warning)
                            //.AddFilter("System", LogLevel.Warning)
                            //.AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                            //.AddDebug()
                            .SetMinimumLevel(LogLevel.Information);
                            //.AddConsole()
                            //.AddDebug();
                        //.AddEventLog();
                    });

                }
                return loggerFactory;
            }
            set { loggerFactory = value; }
        }
    }
}
