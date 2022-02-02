using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Linq;
using ActionFramework.Logger;
using ActionFramework.Configuration;
using Microsoft.Extensions.DependencyModel;
using Serilog;

namespace ActionFramework
{
    public static class AppContext
    {
        private static List<ActionFramework.App> apps;
        public static ILogger Log => LogService.Logger;

        public static void AddApp(Assembly assembly)
        {
            EnsureInitialized();

            if (IsApp(assembly))
            {
                apps.Add(new ActionFramework.App(assembly));
            }
        }

        public static void Initialize()
        {
            try
            {
                apps = new List<ActionFramework.App> ();

                var appdirs = new string[] { ConfigurationManager.AppRootDirectory, ConfigurationManager.AppsDirectory };

                foreach (var dir in appdirs)
                { 
                    if (!Directory.Exists(dir))
                        throw new Exception(string.Format("Apps directory does not exists '{0}' ", dir));

                    var files = Array.FindAll(
                        System.IO.Directory.GetFiles(dir, "*.dll", System.IO.SearchOption.AllDirectories), IncludeDll);

                    //loop through each dll 
                    foreach (var dll in files)
                    {
                        //load all assemblies to check the types
                        var assemblyContext = AssemblyLoadContext.Default;

                        Assembly assembly = null;

                        try
                        {
                            //TODO: check why SNI not working
                            if (!dll.EndsWith("sni.dll", StringComparison.CurrentCultureIgnoreCase))
                                assembly = assemblyContext.LoadFromAssemblyPath(dll);
                        }
                        catch (Exception ex)
                        {

                            Log.Error(ex, $"Could not load app assembly: '{dll}'. Message: '{ex.Message}'");
                            throw ex;
                        }

                        if (assembly != null)
                        { 
                            if (IsApp(assembly))
                            {
                                //the the assembly (app) name
                                string filename = Path.GetFileNameWithoutExtension(dll);

                                Log.Debug($"Loading assembly app {filename}");

                                //check if the apps already contains an app with the same name
                                if (apps.Find(a => a.AppName.Equals(filename, StringComparison.InvariantCultureIgnoreCase)) == null)
                                {
                                    apps.Add(new ActionFramework.App(assembly));
                                }
                                else
                                {
                                    Log.Warning($"App '{filename}' is already loaded, skipping dublicate '{dll}'. Check directory '{dir}' recursively for dublicates");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occured when loading assemblies");
                throw ex;
            }
        }

        private static bool IncludeDll(string s)
        {
            var filename = Path.GetFileName(s);

            var excludeDll = new[] {
                        "sni.dll",
                        "ActionFramework.Agent.dll",
                        "Microsoft",
                        "Serilog",
                        "System.",
                        "Dapper",
                        "Hangfire",
                        "Newtonsoft",
                        "RestSharp",
                        "SendGrid",
                        "Elastic"
                    };

            //filename.StartsWith();
            if (excludeDll.Any(filename.Contains))
                return false;
            else
                return true;
        }

private static Assembly CustomResolving(AssemblyLoadContext arg1, AssemblyName arg2)
        {
            return arg1.LoadFromAssemblyPath(@"C:\Addons\" + arg2.Name + ".dll");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ass"></param>
        /// <returns></returns>
        private static List<AssemblyName> GetAssemblyReferences(Assembly ass)
        {
            var context = DependencyContext.Default;
            List<AssemblyName> references = new List<AssemblyName>();

            if (!context.CompileLibraries.Any())
                Console.WriteLine("Compilation libraries empty");

            foreach (var compilationLibrary in context.CompileLibraries)
            {
                foreach (var resolvedPath in compilationLibrary
                                              .ResolveReferencePaths())
                {
                    Console.WriteLine($"Compilation {compilationLibrary.Name}:{Path.GetFileName(resolvedPath)}");
                    if (!File.Exists(resolvedPath))
                        Console.WriteLine($"Compilation library resolved to non existent path {resolvedPath}");
                }
            }

            foreach (var runtimeLibrary in context.RuntimeLibraries)
            {
                foreach (var assembly in runtimeLibrary.GetDefaultAssemblyNames(context))
                    references.Add(assembly);//Console.WriteLine($"Runtime {runtimeLibrary.Name}:{assembly.Name}");
            }

            return references;
        }

        /// <summary>
        /// Returns a agent configuration context that is applicable for this context.
        /// </summary>
        public static List<ActionFramework.App> Current
        {
            get
            {
                EnsureInitialized();
                return apps;
            }
        }

        public static bool IsInitialized
        {
            get { return apps != null; }
        }

        private static void EnsureInitialized()
        {
            if (!IsInitialized)
            {
                Initialize();
            }
        }

        public static bool IsApp(Assembly assembly)
        {
            try
            {
                if (assembly.GetName().Name.Equals("Agent")) //TODO: we could load the agent (dynamic) actions here by the Agent app
                    return false;

                var actionType = typeof(ActionFramework.Action);

                var actionTypes = assembly.GetTypes()
                               .Where(p => p.IsSubclassOf(actionType) && !p.IsAbstract && p.IsClass && p.IsSubclassOf(actionType));

                if (actionTypes.Count() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                //StartUpLogger.FileLog.Error(ex.InnerException, $"Check if assembly '{assembly}' is an app caused an exception");
                throw ex;
            }
        }

        public static Action Action(string actionname)
        {
            EnsureInitialized();

            try
            { 
                foreach (var app in apps)
                {
                    var action = app.Actions.Find(
                        a => a.ActionName.Equals(actionname, StringComparison.CurrentCultureIgnoreCase));

                    if (action != null)
                        return action;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Find action by name '{actionname}' caused an exception. Message: '{ex.Message}'");
            }
        }

        public static App App(string appname)
        {
            EnsureInitialized();

            try
            {
                return apps.Find(a => a.AppName.Equals(appname, StringComparison.CurrentCultureIgnoreCase));
            }
            catch (Exception ex)
            {
                throw new Exception($"Find app by name '{appname}' caused an exception. Message: '{ex.Message}'");
            }
        }

        public static Action Action<T>()
        {
            EnsureInitialized();

            try
            {
                foreach (var app in apps)
                {
                    var action = app.Actions.Find(a => a.GetType().Equals(typeof(T)));

                    if (action != null)
                        return action;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Find action by type '{typeof(T).Name}' caused an exception. MEssage: '{ex.Message}'");
            }
        }

        public static App AppFromAction(string actionname)
        {
            EnsureInitialized();

            try
            {
                foreach (var app in apps)
                {
                    var action = app.Actions.Find(a => a.ActionName.Equals(actionname, StringComparison.CurrentCultureIgnoreCase));

                    if (action != null)
                        return app;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Find app by actionname '{actionname}' caused an exception. MEssage: '{ex.Message}'");
            }
        }
    }

}
