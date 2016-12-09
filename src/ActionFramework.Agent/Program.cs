using System;
using ActionFramework.Agent;
using ActionFrameworkAgent.Scheduling;
using Microsoft.AspNetCore.Hosting;

namespace ActionFrameworkAgent
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
    }
}
