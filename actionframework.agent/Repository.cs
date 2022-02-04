
using Action = ActionFramework.Action;
using App = ActionFramework.App;
using Hangfire;
using Agent.Configuration;
using Serilog;
using ActionFramework.Logger;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agent
{
    /// <summary>
    /// App repository.
    /// </summary>
    public class Repository
    {
        //Singleton
        private static Lazy<Repository> repository = new Lazy<Repository>(() => new Repository());
        public static Repository Current => repository.Value;
        private AgentSettings agentSettings = Startup.AgentSettings;
        private readonly ILogger logger = LogService.Logger;

        public Repository()
        {
        }

        public bool ScheduleAction(Action action, string schedule)
        {
            var msg = string.Format("RecurringJob '{0}' for action: '{1}'.", schedule, action.ActionName);

            try
            {
                RecurringJob.AddOrUpdate(string.Format("{0}", action.ToString().ToLower()), () => RunAction(action.ActionName.ToLower()), schedule);
                return true;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Gets the apps.
        /// </summary>
        /// <value>The apps.</value>
        public List<ActionFramework.App> Apps
        { 
            get
            {
                return ActionFramework.AppContext.Current;
            }
        }

        public T Cast<T>(object o)
        {
            return (T)o;
        }
        
        public async Task<object> RunAction(string actionname)
        {
            logger.Debug(string.Format("Executing action async {0}", actionname));
            if (string.IsNullOrEmpty(actionname))
                throw new Exception("Action name is null or empty");

            var action = ActionFramework.AppContext.Action(actionname);

            if (action == null)
                throw new Exception($"Action {actionname} is null");

            var result = await RunAction(action, "");
            return result;
        }

        public async Task<object> RunAction(string actionname, dynamic data)
        {
            logger.Debug(string.Format("Executing action async with data {0}", actionname));
            var action = ActionFramework.AppContext.Action(actionname);
            var result = await RunAction(action, data);
            return result;
        }

        public async Task<object> RunAction(ActionFramework.Action action, dynamic data)
        {
            //find the app and action instance
            logger.Debug(string.Format("Executing action in sync with data {0}", action.ActionName));
            ActionFramework.App app = ActionFramework.AppContext.AppFromAction(action.ActionName);

            if (app == null)
                throw new Exception($"App from action {action.ActionName} could not be found");

            try
            {
                //var result = await action.Run(data); //TODO: Check this!
                var result = action.Run(data);

                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"RunAction {action.ActionName} caused an exception. Input {System.Text.Json.JsonSerializer.Serialize(data)}");
                throw ex;
            }
        }
    }
}
