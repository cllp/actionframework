using System;
using Microsoft.AspNetCore.Mvc.Filters;
using ActionFramework.Logger;
using System.Security.Claims;
using System.IO;
using Serilog;

namespace Agent.GenericControllerProvider.Filters
{
    public class ActionLogFilter : ActionFilterAttribute
    {
        private ILogger logger = LogService.Logger;

        public ActionLogFilter()
        {
            
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {

            var user = context.HttpContext.User;
            base.OnActionExecuted(context);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {

            string username = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var host = context.HttpContext.Request.Host;
            var path = context.HttpContext.Request.Path.Value;
            var method = context.HttpContext.Request.Method;
            var protocol = context.HttpContext.Request.Protocol;

            logger.Debug(string.Format("user '{0}' executed controller {1}, with protocol {2} [{3}] on route {4} (ssl: {5})",
                username,
                context.Controller.GetType().FullName,
                protocol, //protocol
                method,
                string.Concat(host, path), //route
                context.HttpContext.Request.IsHttps
                ));

            base.OnActionExecuting(context);
        }
    }

}
