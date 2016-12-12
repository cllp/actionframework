using System;
using System.Collections.Generic;
using System.Linq;
using ActionFramework.Agent.App;
using Microsoft.AspNetCore.Mvc;
using ActionFramework.Scheduling;
using ActionFrameworkAgent.Scheduling;

namespace ActionFrameworkAgent.API
{
    [Route("api/[controller]/[action]")]
    public class TestController : Controller
    {
        [HttpGet]
        public List<ActionFramework.App.App> Get()
        {
            var appRepo = ActionFramework.Agent.Agent.GetAppRepository();
            var apps = appRepo.GetInstalledApps();
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
            schedule.StopDateTime = DateTime.UtcNow.AddSeconds(180);
            ActionScheduleRepository.SaveActionSchedule(schedule);

            var scheduler = new Scheduler();
            scheduler.ScheduleAction(schedule);

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
            var appRepo = ActionFramework.Agent.Agent.GetAppRepository();

            return appRepo.RunAction(appName, actionName);
        }


        [HttpGet]
        public AgentInformation SystemInformation()
        {
            return new AgentInformation();
        }
    }
}
