using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ActionFramework.Agent
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            //app.Run(context =>
            //{
            //    return context.Response.WriteAsync("Hello from ASP.NET Core!");
            //});

            app.UseMvc();
        }
    }
}
