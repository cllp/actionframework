using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using ActionFramework.Log;
using ActionFrameworkAgent.Log;
using Microsoft.DotNet.ProjectModel;

namespace ActionFrameworkAgent.App
{
    public class AppRepository
    {
        //public AppRepository()
        //{
        //    if (!Agent.AgentIsAvailable())
        //    {
        //        throw new Exception("Agent is not available.");
        //    }
        //}

        public List<ActionFramework.App.App> GetInstalledApps()
        {
            var apps = new List<ActionFramework.App.App>();
            var appsPath = GetInstalledAppsDirectory();
            foreach (var appDirectory in Directory.GetDirectories(appsPath))
            {
                var pathSegements = appDirectory.Split('\\');
                var appName = pathSegements[pathSegements.GetUpperBound(0)];
                var filePath = appDirectory + "\\" + appName + ".dll";
                if (File.Exists(filePath))
                {
                    var appAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(filePath);
                    var appType = appAssembly.GetType(appName + "." + appName);
                    var appInstance = Activator.CreateInstance(appType) as ActionFramework.App.App;
                    apps.Add(appInstance);
                }
            }

            return apps;
        }

        public ActionFramework.App.App GetApp(string appName)
        {
            var filePath = GetInstalledAppsDirectory() + "\\" + appName + "\\" + appName + ".dll";
            if (!File.Exists(filePath))
            {
                return null;
            }

            var appAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(filePath);
            var appType = appAssembly.GetType(appName + "." + appName);
            var appInstance = Activator.CreateInstance(appType) as ActionFramework.App.App;
            return appInstance;
        }

        public bool RunAction(string appName, string actionName)
        {
            var success = false;
            var app = GetApp(appName);
            var action = app?.Actions.FirstOrDefault(a => a.ActionName == actionName);
            if (action != null)
            { 
                var actionLog = new ActionLog(action.ActionName);
                actionLog.StartRunDate = DateTime.UtcNow;

                string actionMessage;
                try
                {
                    success = action.Execute(out actionMessage);
                }
                catch (Exception e)
                {
                    actionMessage = e.Message;
                }

                actionLog.Success = success;
                actionLog.EndRunDate = DateTime.UtcNow;
                actionLog.LogMessage = actionMessage;
                ActionLogRepository.SaveActionLog(appName, actionLog);
            }
            return success;
        }

        private string GetInstalledAppsDirectory()
        {
            //get path to the running application's directory
            var applicationPath = ProjectRootResolver.ResolveRootDirectory(Directory.GetCurrentDirectory());

            const string installedAppsDirectoryName = "InstalledApps";
            var installedAppsPath = applicationPath + "\\" + installedAppsDirectoryName; //todo: check if this works on different OS (Linux)

            if (!Directory.Exists(installedAppsPath))
            {
                Directory.CreateDirectory(installedAppsPath);
            }

            return installedAppsPath;
        }

        public string GetAppDirectory(string appName)
        {
            var appDirectory = GetInstalledAppsDirectory() + "\\" + appName;
            if (!Directory.Exists(appDirectory))
            {
                return null;
            }

            return GetInstalledAppsDirectory() + "\\" + appName;
        }

    }
}
