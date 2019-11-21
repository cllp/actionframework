using System;
using Microsoft.AspNetCore.Mvc.Filters;
using ActionFramework.Logger;
using System.Security.Claims;
using System.IO;

namespace Agent.GenericControllerProvider.Filters
{
    public class ActionLogFilter : ActionFilterAttribute
    {
        public ActionLogFilter()
        {
            
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //context.HttpContext.Authentication//Microsoft.AspNetCore.Authentication

            var user = context.HttpContext.User;
            base.OnActionExecuted(context);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {

            //var test = context.RouteData.Values;
            //string username = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //var path = context.HttpContext.Request.Path;
            //var body = RequestBody(context.HttpContext.Request.Body);

            //log.Information(context.Controller.GetType().Name, "COntroller axecuting context");

            base.OnActionExecuting(context);
        }
    }

}
