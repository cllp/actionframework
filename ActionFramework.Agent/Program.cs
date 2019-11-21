using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ActionFramework.Configuration;
using ActionFramework.Logger;
using Agent.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace Agent
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //enable the serilog from ActionFramework
            Log.Logger = LogService.Logger;
            
            try
            {
                Log.Information("ActionFramework Agent Starting up");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "ActionFramework Agent start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }            
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .CaptureStartupErrors(true) // the default
                .UseSetting("detailedErrors", "true")
                .UseStartup<Startup>();
    }
}
