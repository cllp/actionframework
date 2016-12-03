using Microsoft.AspNetCore.Hosting;
using System;

namespace ActionFramework.Agent
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Setting up action timers...");
            var scheduler = new Scheduling.Scheduler();
            var timers = scheduler.ScheduleAllActions();
            Console.WriteLine($"{timers.Count} action(s) where scheduled.");

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://localhost:7406") //todo: add to config?
                .UseStartup<Startup>()
                .Build();

            host.Run();
            
        }
    }
}
