using System;
using System.Collections.Generic;
using System.Text;

namespace ActionFramework.Logger
{
    public enum LogType
    {
        Information,
        Warning,
        Error,
        Debug,
        Custom,
        Trace
    }

    public class Log
    {
        public LogType LogType { get; set; }
        public string[] Tags { get; set; }
        public Exception Ex { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
