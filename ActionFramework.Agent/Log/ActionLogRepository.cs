using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ActionFramework.Core.Log;
using ActionFramework.Agent.App;

namespace ActionFramework.Agent.Log
{
    internal static class ActionLogRepository
    {
        public static bool SaveActionLog(string appName, ActionLog log)
        {
            var successfullySaved = false;
            var actionLogs = GetActionLogs(appName, log.ActionName);
            actionLogs.Insert(0, log);
            actionLogs = actionLogs.OrderByDescending(l => l.StartRunDate).Take(500).ToList(); //do not save more than 500 log entries

            var filePath = GetFilePath(appName, log.ActionName);
            if (filePath != null)
            {
                var json = JsonConvert.SerializeObject(actionLogs, Formatting.Indented);
                File.WriteAllText(filePath, json);
                successfullySaved = true;
            }

            //todo: return false if anything goes wrong
            return successfullySaved;
        }

        private static List<ActionLog> GetActionLogs(string appName, string actionName)
        {
            List<ActionLog> log;
            var filePath = GetFilePath(appName, actionName);
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                log = JsonConvert.DeserializeObject<List<ActionLog>>(json);
            }
            else
            {
                log = new List<ActionLog>();
            }
            return log;
        }

        private static string GetFilePath(string appName, string actionName)
        {
            return GetLogDirectory(appName) + "\\" + actionName + "ActionLog.json";
        }

        private static string GetLogDirectory(string appName)
        {
            var appRepo = new AppRepository(); // ActionFramework.Agent.Agent.GetAppRepository();
            var appDirectory = appRepo.GetAppDirectory(appName);
            if (appDirectory == null)
            {
                return null;
            }

            var logDirectory = appDirectory + "\\Logs";
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            return logDirectory;
        }
    }
}
