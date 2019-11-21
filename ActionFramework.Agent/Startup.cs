using System.Text;
using Agent.Configuration;
using Agent.Controllers.Generic;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Agent
{
    public class Startup
    {
        private IWebHostEnvironment hostingEnvironment;
        public static AgentSettings AgentSettings;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            hostingEnvironment = environment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            //adding caching to the application
            services.AddMemoryCache();

            // configure strongly typed settings objects
            var agentSettingsSection = ActionFramework.Configuration.ConfigurationManager.GetSection("AgentSettings");
            services.Configure<AgentSettings>(agentSettingsSection);
            AgentSettings = agentSettingsSection.Get<AgentSettings>();

            //configure authentication
            var key = Encoding.ASCII.GetBytes(AgentSettings.Secret);

            //add jwt authentication
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            //set compatibility to dotnet core 3.0
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddMvc(option => option.EnableEndpointRouting = false);

            services.AddMvc(o => o.Conventions.Add(new GenericControllerRouteConvention())).
                ConfigureApplicationPartManager(m => m.FeatureProviders.Add(new GenericTypeControllerFeatureProvider())).
                ConfigureApplicationPartManager(m => m.FeatureProviders.Add(new RemoteControllerFeatureProvider(hostingEnvironment)));

            if(AgentSettings.UseHangfireServer)
                services.AddHangfire(x => x.UseSqlServerStorage(AgentSettings.AgentConnectionString));
            //services.AddHangfireServer();

            if (AgentSettings.UseHangfireDashboard)
            {
                /*
                var storage1 = new SqlServerStorage("Connection1");
                var storage2 = new SqlServerStorage("Connection2");

                app.UseHangfireDashboard("/hangfire1", new DashboardOptions(), storage1);
                app.UseHangfireDashboard("/hangfire2", new DashboardOptions(), storage2);
                 */
                services.AddHangfire(config =>
                {
                    config.UseSqlServerStorage(AgentSettings.AgentConnectionString);
                });
            }

            //the generic controller for storage
            //services.AddSingleton(typeof(Storage<>));
            //services.AddSingleton(typeof(LogService));

            // Explicitly register the settings object by delegating to the IOptions object
            services.AddSingleton(resolver =>
                resolver.GetRequiredService<IOptions<AgentSettings>>().Value);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();

            //Hangfire server
            if (AgentSettings.UseHangfireServer)
            {
                BackgroundJobServerOptions backgroundJobServerOptions = new BackgroundJobServerOptions();
                backgroundJobServerOptions.ServerName = AgentSettings.AgentName;
                app.UseHangfireServer(backgroundJobServerOptions);

                //StartUpLogger.FileLog.Information("init hangfire server using server agent name: ", AgentSettings.AgentName);
            }

            //Hangfire Dashboard
            if (AgentSettings.UseHangfireDashboard)
            {
                DashboardOptions dashboardOptions = new DashboardOptions()
                {
                    DisplayStorageConnectionString = true,
                    Authorization = new[]
                    {
                        new HangfireAuthFilter()
                    }
                };

                app.UseHangfireDashboard("", dashboardOptions);
            }
        }

    }

    public class HangfireAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
            /*
            return context.GetHttpContext().User != null && context.GetHttpContext().User.Identity != null
                && context.GetHttpContext().User.Identity.IsAuthenticated
                && context.GetHttpContext().User.Identity.Name == "something";
                */
        }
    }
}

    

 
