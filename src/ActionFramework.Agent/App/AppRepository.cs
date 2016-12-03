using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Loader;
using ActionFramework.Agent.Log;
using Microsoft.DotNet.ProjectModel;
using ActionFramework.Log;
using System.Linq;

namespace ActionFramework.Agent.App
{
    public static class AppRepository
    {
        public static List<ActionFramework.App.App> GetInstalledApps()
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

        public static ActionFramework.App.App GetApp(string appName)
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

        public static bool RunAction(string appName, string actionName)
        {
            var success = false;
            var app = GetApp(appName);
            if (app != null)
            {
                var action = app.Actions.FirstOrDefault(a => a.ActionName == actionName);
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

        private static string GetInstalledAppsDirectory()
        {
            //get path to the running application's directory
            var applicationPath = ProjectRootResolver.ResolveRootDirectory(Directory.GetCurrentDirectory());

            const string installedAppsDirectoryName = "InstalledApps";
            var installedAppsPath = applicationPath + "\\" + installedAppsDirectoryName; //todo: check if this works on different OS

            if (!Directory.Exists(installedAppsPath))
            {
                Directory.CreateDirectory(installedAppsPath);
            }

            return installedAppsPath;
        }

        public static string GetAppDirectory(string appName)
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
