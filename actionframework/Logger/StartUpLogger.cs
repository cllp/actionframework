/*
using System;
using System.IO;
using System.Text.Json;
using ActionFramework.Configuration;
using Serilog;

namespace ActionFramework.Logger
{
    public class StartUpLogger
    {
        private Serilog.ILogger serilog;

        private StartUpLogger()
        {
            serilog = new LoggerConfiguration().WriteTo.File($"Logs/startup.{DateTime.UtcNow.ToString("yyyy-MM-dd.HHmm")}.log", fileSizeLimitBytes: 1024 * 1024).CreateLogger();
                //.File(string.Format("Logs/{0}.action.log", DateTime.UtcNow.ToString("yyyy-MM-dd"))).CreateLogger();
        }

        public bool Information(string message)
        {
            serilog.Information(message);
            return true;
        }

        public bool Information(string message, object log)
        {
            serilog.Information(JsonSerializer.Serialize(new { Message = message, Log = log }));
            return true;
        }

        public bool Warning(Exception ex, string message)
        {
            serilog.Warning(ex, message);
            return true;
        }

        public bool Warning(string message)
        {
            serilog.Warning(message);
            return true;
        }

        public bool Error(Exception ex, string message)
        {
            serilog.Error(ex, message);
            return true;
        }

        private static Lazy<StartUpLogger> fileLog = new Lazy<StartUpLogger>(() => new StartUpLogger());

        public static StartUpLogger FileLog => fileLog.Value;
    }
}
*/