using System;
using System.IO;
using ActionFramework.Agent;
using ActionFrameworkAgent.Configuration;
using ActionFrameworkAgent.Scheduling;
using Microsoft.AspNetCore.Hosting;
using Microsoft.DotNet.ProjectModel;
using Microsoft.Extensions.Configuration;

namespace ActionFrameworkAgent
{
    public class Program
    {
        public static IConfigurationRoot Configuration { get; set; }

        public static void Main(string[] args)
        {
            SetupConfiguration();
            Console.WriteLine("Configuration[\"agentguid\"]: " + Configuration["agentguid"]);

            Console.WriteLine("Setting up action timers...");
            var scheduler = new Scheduler();
            scheduler.ScheduleAllActions();
            Console.WriteLine($"{Scheduler.ActiveTimers.Count} action(s) are scheduled.");

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://127.0.0.1:" + Configuration["PortNumber"])
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }

        private static void SetupConfiguration()
        {
            var configManager = new ConfigManager();
            configManager.InitConfiguration();

            var builder = new ConfigurationBuilder()
                .AddJsonFile(configManager.SettingsFilePath, optional: false, reloadOnChange: true);

            //TODO: store api key in a more secure way(?) .AddUserSecrets()

            Configuration = builder.Build();
        }
    }
}
