using ActionFramework.Scheduling;
using System;
using System.Collections.Generic;
using ActionFramework.Agent.App;

namespace ActionFramework.Agent.Scheduling
{
    public class Scheduler
    {
        public List<ActionTimer> ScheduleAllActions()
        {
            var timers = new List<ActionTimer>();

            var apps = AppRepository.GetInstalledApps();
            foreach (var app in apps)
            {
                foreach (var action in app.Actions)
                {
                    var schedule = ActionScheduleRepository.GetActionSchedule(app.AppName, action.ActionName);
                    if (ShouldActionBeScheduled(schedule))
                    {
                        var testTimer = new ActionTimer(schedule);
                        timers.Add(testTimer);
                    }
                }
            }

            return timers;
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
