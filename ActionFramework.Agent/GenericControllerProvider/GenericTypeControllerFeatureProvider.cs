using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ActionFramework;
using Agent;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Agent.Controllers.Generic
{
    public class GenericTypeControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public GenericTypeControllerFeatureProvider()
        {
            //agentlog = new LoggerConfiguration().WriteTo.File(string.Format("Logs/{0}.{1}.agent.log", DateTime.UtcNow.ToString("yyyy-MM-dd"), _agentSettings.AgentName)).CreateLogger();
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            var apps = AppContext.Current;

            foreach(var app in apps)
            {
                foreach(var action in app.Actions.Where(c => c.Config.ApiSettings.Active))
                {
                    switch (action.Config.ApiSettings.Method.ToLower())
                    {
                        case "get":
                            {
                                break;
                            };
                        case "post":
                            {
                                break;
                            };
                        default:
                            {
                                throw new System.Exception($"No api method set in ApiSettings for action {action.ActionName}");
                            }
                    }

                    //adding a new controller of the action type 
                    feature.Controllers.Add(typeof(BaseController<>).MakeGenericType(action.GetType()).GetTypeInfo());

                    //test to add same controller again
                    //if one action is configured for two controllers
                    //if(action.ActionName.Equals("GetDeviceInfo"))
                        //feature.Controllers.Add(typeof(BaseController<>).MakeGenericType(action.GetType()).GetTypeInfo());

                }
            }
            

            /*
            foreach(var app in apps)
            {
                foreach(var controller in app.Controllers)
                {
                    if(controller.GetType().Name.Contains("Post"))
                        feature.Controllers.Add(typeof(GenericPostController<>).MakeGenericType(controller.GetType()).GetTypeInfo());
                    else
                        feature.Controllers.Add(typeof(BaseController<>).MakeGenericType(controller.GetType()).GetTypeInfo());
                }
            }           
            */

            /*
            var currentAssembly = typeof(GenericTypeControllerFeatureProvider).Assembly;
            var candidates = currentAssembly.GetExportedTypes().Where(x => x.GetCustomAttributes<GeneratedControllerAttribute>().Any());

            foreach (var candidate in candidates)
            {
                feature.Controllers.Add(typeof(BaseController<>).MakeGenericType(candidate).GetTypeInfo());
            }
            */
        }
    }
}
