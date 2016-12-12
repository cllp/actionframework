using ActionFramework.Agent.App;
using ActionFramework.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ActionFrameworkAgent.Scheduling
{
    public class Scheduler
    {
        public static List<ActionTimer> ActiveTimers { get; private set; } //static field to be able to access the active timers.

        public Scheduler()
        {
            if (ActiveTimers == null)
            {
                ActiveTimers = new List<ActionTimer>();
            }
        }

        public void ScheduleAllActions()
        {
            var appRepo = ActionFramework.Agent.Agent.GetAppRepository();
            var apps = appRepo.GetInstalledApps();
            foreach (var app in apps)
            {
                foreach (var action in app.Actions)
                {
                    var schedule = ActionScheduleRepository.GetActionSchedule(app.AppName, action.ActionName);
                     ScheduleAction(schedule);
                }
            }
        }

        public void ScheduleAction(ActionSchedule schedule)
        {
            //remove any existing timer for this action
            var existingTimer = ActiveTimers.FirstOrDefault(t => t.AppName == schedule.AppName && t.ActionName == schedule.ActionName);
            if (existingTimer != null)
            {
                Console.WriteLine("disposing timer..."); //todo: remove this debugmessage
                existingTimer.Timer.Dispose();
                existingTimer.Timer = null;
                ActiveTimers.Remove(existingTimer);
            }

            if (ShouldActionBeScheduled(schedule))
            {
                ActiveTimers.Add(new ActionTimer(schedule));
            }
        }

        private bool ShouldActionBeScheduled(ActionSchedule actionSchedule)
        {
            if (actionSchedule.NextRun == DateTime.MinValue)
            {
                return false;
            }
            if (actionSchedule.StopDateTime == DateTime.MinValue)
            {
                return true;
            }
            if (actionSchedule.StopDateTime > DateTime.UtcNow)
            {
                return true;
            }

            return false;
        }
    }
}
