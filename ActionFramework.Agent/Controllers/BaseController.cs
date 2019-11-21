using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Agent.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ActionFramework.Logger;
using Serilog;

namespace Agent.Controllers.Generic
{
    [Route("api/[controller]")]
    public class BaseController<T> : Controller where T : class
    {
        //private Storage<T> _storage;
        private readonly AgentSettings _agentSettings;
        private readonly IConfiguration _configuration;
        private Repository repository = Repository.Current;
        private ILogger logger = LogService.Logger;

        //public BaseController(Storage<T> storage, AgentSettings agentSettings, IConfiguration configuration)
        public BaseController(AgentSettings agentSettings, IConfiguration configuration)
        {
            _agentSettings = agentSettings;
            _configuration = configuration;
        }

        /// <summary>
        /// Dynamic Get, single data string for input to an action
        /// </summary>
        [HttpGet("{data}")]
        public async Task<IActionResult> Get(string data)
        {
            try
            {
                var route = Request.Path.Value;
                var username = Request.HttpContext.User.Identity.Name;
                logger.Debug($"BaseController Post {route}. Username {username}. Data {System.Text.Json.JsonSerializer.Serialize(data)}");


                var action = ActionFramework.AppContext.Action<T>();

                var result = await repository.RunAction(action, data);

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"async Task<IActionResult> Get for path: '{Request.Path.Value}' caused an exception. Data: '{data}' ");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Dynamic Post. Post the body for input to an action
        /// </summary>
        [HttpPost]
        //[ActionLogFilter]
        public async Task<IActionResult> Post([FromBody]dynamic data)
        {

            var action = ActionFramework.AppContext.Action<T>();

            try
            {
                var route = Request.Path.Value;
                var username = Request.HttpContext.User.Identity.Name;

                logger.Debug($"BaseController Post {route}. Username {username}. Data {System.Text.Json.JsonSerializer.Serialize(data)}");

                var result = await repository.RunAction(action, data);

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $" An exception occured Data: '{System.Text.Json.JsonSerializer.Serialize(data)}'");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }

        /// <summary>
        /// Dynamic Get. Stores all Query key/values in a dictionary for input to an action
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[ActionLogFilter]
        public async Task<IActionResult> Get()
        {
            try
            {
                var data = new Dictionary<string, object>();

                foreach (var key in Request.Query.Keys)
                {
                    Microsoft.Extensions.Primitives.StringValues queryVal;
                    bool success = Request.Query.TryGetValue(key, out queryVal);
                    data.Add(key, queryVal);
                }

                var route = Request.Path.Value;
                var username = Request.HttpContext.User.Identity.Name;

                logger.Debug($"BaseController Get {route}. Username {username}. QueryStrings {System.Text.Json.JsonSerializer.Serialize(data)}");

                var action = ActionFramework.AppContext.Action<T>();

                var result = await repository.RunAction(action, data);

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"async Task<IActionResult> Get() for path: '{Request.Path.Value}' caused an exception");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private string GetActionNameFromRoute(string requestPath)
        {
            return Path.GetFileName(requestPath);
        }

        /*
        [HttpPost("{id}")]
        public void Post(Guid id, [FromBody]T value)
        {
            _storage.Add(id, value);
        }
        */
    }
}
