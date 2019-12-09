using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ActionFramework.Configuration;
using System.Text.Json;
using Serilog;
using ActionFramework.Logger;

namespace ActionFramework
{
    public class App
    {
        public string AppName => assembly.GetName().Name;
        public string AppVersion => this.GetType().GetTypeInfo().Assembly.GetName().Version.ToString();
        private Assembly assembly;
        public ILogger Log => LogService.Logger;


        private IList<ActionConfig> config;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        public App(Assembly assembly)
        {
            this.assembly = assembly;
            config = GetConfiguration();
        }
     
        /// <summary>
        /// Gets the actions.
        /// </summary>
        /// <value>The actions.</value>
        public List<Action> Actions
        {
            get
            {
                try
                {
                    var actions = new List<Action>();
                    var actionType = typeof(Action);

                    var actionTypes = assembly.GetTypes()
                                   .Where(p => p.IsSubclassOf(actionType) && !p.IsAbstract && p.IsClass && p.IsSubclassOf(actionType));

                    foreach (var type in actionTypes)
                    {

                        ActionFramework.Action instance = (ActionFramework.Action)Activator.CreateInstance(type);

                        try
                        {
                            var actionconfig = config.Where(a => a.ActionName.Equals(instance.ActionName)).FirstOrDefault();

                            if (actionconfig == null)
                                throw new Exception($"Action configuration could not be found for action {instance.ActionName}");

                            instance.Configure(actionconfig);

                            instance.Config = actionconfig;

                            actions.Add(instance);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, $"Set action config caused an exception. ActionType '{type.Name}'");
                            throw ex;
                        }
                    }

                    return actions;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Get Actions caused an exception");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionType"></param>
        /// <returns></returns>
        private PropertyInfo[] GetActionProperties(Type actionType)
        {
            return actionType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <returns>The configuration.</returns>
        private IList<ActionConfig> GetConfiguration()
        {
            try
            {
                var environment = ConfigurationManager.Environment; //ConfigurationManager.Settings["AgentSettings:Environment"];
                if (string.IsNullOrEmpty(environment))
                    throw new Exception("Environment is not configured in the agent, verify environment.txt in root directory");

                var rootdir = ConfigurationManager.AppRootDirectory; //TODO change to apps directory to load from the same place as dynamic apps
                //var rootdir = ConfigurationManager.AppsDirectory;//System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string filename = $"{AppName.ToLower()}.config.{environment.ToLower()}.json";

                //var filepath = Path.Combine(rootdir, AppName, filename);
                var filepath = Path.Combine(rootdir, filename); // in order to debug in root directory

                //find file even if there are root directory
                string file = Directory.GetFiles(rootdir, filename, SearchOption.AllDirectories).FirstOrDefault();
                if (string.IsNullOrEmpty(file))
                    throw new Exception($"Could not find file '{filename}' recursively from dir '{rootdir}'");

                var json = File.ReadAllText(file);

                Log.Debug($"App loading configuration '{file}'");

                //TODO: Confirm this works
                var appconfig = JsonSerializer.Deserialize<List<ActionConfig>>(json);
                return appconfig;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"GetConfiguration for app {AppName.ToLower()} in environment {ConfigurationManager.Environment.ToLower()} caused an exception");
                throw ex;
            }   
        }
    }
}
