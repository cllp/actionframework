using System;
using System.IO;
using ActionFramework.Logger;
using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Configuration.Json;

namespace ActionFramework.Configuration
{
    /// <summary>
    /// Configuration manager.
    /// {
    ///     "Grandfather": {
    ///         "Father": {
    ///             "Child": "myvalue"
    ///         }
    /// }
    /// Use like this: ConfigurationManager.AppSetting["Grandfather:Father:Child"]
    /// </summary>

    public static class ConfigurationManager
    {
        public static IConfiguration Settings { get; }
        //public static IHostingEnvironment env { get; }

        static ConfigurationManager()
        {
            Settings = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile($"appsettings.{Environment}.json", false, true)
                    //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                    //.AddEnvironmentVariables()
                    .Build();
        }

        public static IConfigurationSection GetSection(string sectionname)
        {
            var section = Settings.GetSection(sectionname);
            return section; //agentSettingsSection.Get<T>();
        }

        public static string Environment
        {
            get
            {
                var rootdir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string filename = $"environment.txt";
                var filepath = Path.Combine(rootdir, filename);
                try
                {
                    var env = File.ReadAllText(filepath);
                    return env;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error loading required file environment.txt in root directory. Message: '{ex.Message}'");
                }
            }
        }

        public static string AppsDirectory
        {
            get
            {
                try
                {
                    var rootdir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    var appsdir = Path.Combine(rootdir, Settings["AgentSettings:AppsDir"]); //get from config

                    if (string.IsNullOrEmpty(appsdir))
                        throw new Exception($"Apps directory '{appsdir}' is null or empty");

                    return appsdir;
                    //LogFactory.File.Information($"Current apps directory '{rootdir}'");
                    //return Path.Combine(rootdir, ""); //get from root
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occured resolving AppsDirectory. Message. '{ex.Message}'");
                }
            }
        }

        public static string AppRootDirectory
        {
            get
            {
                try
                {
                    var rootdir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    return Path.Combine(rootdir, "");   
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occured resolving AppsRootDirectory. Message. '{ex.Message}'");
                }
            }

        }
}
    
}
