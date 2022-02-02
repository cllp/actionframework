using ActionFramework;
using ActionFramework.Api;
using Agent.GenericControllerProvider.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.Reflection;

namespace Agent.Controllers.Generic
{
    public class GenericControllerRouteConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.IsGenericType)
            {
                //TODO: do exception controlling in this class

                 

                var genericType = controller.ControllerType.GenericTypeArguments[0];
                var action = AppContext.Action(genericType.Name);

                if (action == null)
                    throw new System.Exception($"Action is null. Generic type {controller.ControllerType} does not inherit Action base class.");

                var customNameAttribute = genericType.GetCustomAttribute<GeneratedControllerAttribute>();

                //test to add same controller again
                //if (action.ActionName.Equals("GetDeviceInfo"))
                    //string test = "asd"; //

                //Add logging filter to the controller
                controller.Filters.Add(new ActionLogFilter());

                //TODO: add policy from string
                //set authoriation filter to the controller
                if(action.Config.ApiSettings.Authorize)
                    controller.Filters.Add(new AuthorizeFilter());

                //configure the route
                if (!string.IsNullOrEmpty(action.Config.ApiSettings.Route)) //check if we have a custom route from config
                {
                    controller.Selectors.Add(new SelectorModel
                    {
                        AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(action.Config.ApiSettings.Route)),
                    });
                }
                else if (customNameAttribute?.Route != null) //check if there is a custom route attribute on the class
                {
                    controller.Selectors.Add(new SelectorModel
                    {
                        AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(customNameAttribute.Route)),
                    });
                }
                else //add the route from the controller name
                {
                    controller.ControllerName = genericType.Name;
                }
            }
        }
    }
}
