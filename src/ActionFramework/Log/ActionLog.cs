using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActionFramework.Log
{
    public class ActionLog
    {
        public string ActionName { get; set; }
        public DateTime StartRunDate { get; set; }
        public DateTime EndRunDate { get; set; }
        public bool Success { get; set; }

        private string _logMessage;
        public string LogMessage
        {
            get { return _logMessage; }
            set
            {
                const int maxLen = 500;
                _logMessage = value.Length > maxLen ? value.Substring(0, maxLen) : value;
            }
        }

        public ActionLog(string actionName)
        {
            ActionName = actionName;
        }
    }
}
