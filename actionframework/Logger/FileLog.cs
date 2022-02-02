/*
using System;
using System.Text.Json;
using Serilog;

namespace ActionFramework.Logger
{
    internal class FileLog : IActionLogger
    {
        private Serilog.ILogger serilog;

        public FileLog()
        {
            serilog = new LoggerConfiguration().WriteTo.File($"Logs/{DateTime.UtcNow.ToString("yyyy-MM-dd.HH")}00.log", fileSizeLimitBytes: 1024 * 1024).CreateLogger();
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
            //serilog.Information(JsonConvert.SerializeObject(new { Message = message, Log = log }));
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

        public bool Debug(string message)
        {
            serilog.Debug(message);
            return true;
        }

        public bool Error(Exception ex, string message)
        {
            serilog.Error(ex, message);
            return true;
        }
    }

   
}
*/