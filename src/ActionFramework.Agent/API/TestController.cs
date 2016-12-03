using System;
using System.Collections.Generic;
using System.Linq;
using ActionFramework.Agent.App;
using ActionFramework.Agent.Scheduling;
using Microsoft.AspNetCore.Mvc;
using ActionFramework.Scheduling;

namespace ActionFramework.Agent.API
{
    [Route("api/[controller]/[action]")]
    public class TestController : Controller
    {
        [HttpGet]
        public List<ActionFramework.App.App> Get()
        {
            var apps = AppRepository.GetInstalledApps();
            //var asdf = new List<ActionFramework.App.App>();
            //foreach (var app in apps)
            //{
            //    asdf.Add(app.Description);
            //    //foreach
            //}
            return apps;
        }

        [HttpGet]
        public ActionSchedule GetActionSchedule(string appName, string actionName)
        {
            var schedule = ActionScheduleRepository.GetActionSchedule(appName, actionName);
            schedule.Interval = 2;
            schedule.Unit = IntervalUnit.Second;
            schedule.NextRun = DateTime.UtcNow;
            schedule.StopDateTime = DateTime.UtcNow.AddMilliseconds(20000);
            ActionScheduleRepository.SaveActionSchedule(schedule);
            //Todo: whenever 

            return schedule;
        }

        [HttpGet]
        public bool RunAction(string appName, string actionName)
        {
            //var app = AppRepository.GetApp(appName);
            //var success = false;
            //if (app != null)
            //{
            //    var action = app.Actions.FirstOrDefault(a => a.ActionName == actionName);
            //    success = app.RunAction(action);
            //}

            return AppRepository.RunAction(appName, actionName);
        }


        [HttpGet]
        public SystemInformation SystemInformation()
        {
            return new SystemInformation()
            {
                OsDescription = System.Runtime.InteropServices.RuntimeInformation.OSDescription
            };
        }
    }
}
