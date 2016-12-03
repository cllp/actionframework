using System.Collections.Generic;
using System.IO;
using System.Linq;
using ActionFramework.Agent.App;
using Newtonsoft.Json;
using ActionFramework.Scheduling;

namespace ActionFramework.Agent.Scheduling
{
    public static class ActionScheduleRepository
    {
        public static bool SaveActionSchedule(ActionSchedule schedule)
        {
            var successfullySaved = false;
            var schedules = GetActionSchedules(schedule.AppName);
            var existingSchedule = schedules.FirstOrDefault(s => s.ActionName == schedule.ActionName);
            if (existingSchedule != null)
            {
                //delete existing schedule
                schedules.Remove(existingSchedule);
            }
            schedules.Add(schedule);

            var filePath = GetFilePath(schedule.AppName);
            if (filePath != null)
            {
                var json = JsonConvert.SerializeObject(schedules, Formatting.Indented);
                File.WriteAllText(filePath, json);
                successfullySaved = true;
            }

            //todo: return false if anything goes wrong
            return successfullySaved;
        }

        private static List<ActionSchedule> GetActionSchedules(string appName)
        {
            List<ActionSchedule> schedules;
            var filePath = GetFilePath(appName);
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                schedules = JsonConvert.DeserializeObject<List<ActionSchedule>>(json);
            }
            else
            {
                schedules = new List<ActionSchedule>();
            }
            return schedules;
        }

        public static ActionSchedule GetActionSchedule(string appName, string actionName)
        {
            return GetActionSchedules(appName).FirstOrDefault(s => s.ActionName == actionName) ?? new ActionSchedule(appName, actionName);
        }

        private static string GetFilePath(string appName)
        {
            var appDirectory = AppRepository.GetAppDirectory(appName);
            if (appDirectory != null)
            {
                return appDirectory + "\\ActionSchedules.json";
            }
            return null;
        }
    }
}
