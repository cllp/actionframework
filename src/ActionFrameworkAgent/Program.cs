using System;
using ActionFramework.Agent;
using ActionFrameworkAgent.Scheduling;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace ActionFrameworkAgent
{
    public class Program
    {
        public static IConfigurationRoot Configuration { get; set; }

        public static void Main(string[] args)
        {
            //SetupConfiguration();
            //Console.WriteLine("Configuration[\"agentguid\"]: " + Configuration["agentguid"]);

            Console.WriteLine("Setting up action timers...");
            var scheduler = new Scheduler();
            scheduler.ScheduleAllActions();
            Console.WriteLine($"{Scheduler.ActiveTimers.Count} action(s) are scheduled.");

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://localhost:7406") //todo: add to config?
                .UseStartup<Startup>()
                .Build();

            host.Run();
            
        }

        private static void SetupConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            //TODO: store api key in a more secure way(?) .AddUserSecrets()

            Configuration = builder.Build();
        }
    }
}
