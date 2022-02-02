using System;
using Microsoft.AspNetCore.Mvc;
using Agent.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Agent.Auth;
using ActionFramework.Logger;
using Serilog;
using Microsoft.AspNetCore.Http;

namespace Agent.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AgentController : Controller
    {
        private readonly AgentSettings _agentSettings;
        private readonly IConfiguration _configuration;
        private IAuthService _authService;
        private ILogger logger = LogService.Logger;

        public AgentController(AgentSettings agentSettings, IConfiguration configuration)
        {
            _agentSettings = agentSettings;
            _configuration = configuration;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Schedule(string actionname, string cron)
        {
            string schedule = string.Empty;

            var repository = Repository.Current;

            var action = ActionFramework.AppContext.Action(actionname);

            //if cron is null 
            if (string.IsNullOrEmpty(cron))
                schedule = action.Config.Cron;
            else
                schedule = cron;

            var result = repository.ScheduleAction(action, schedule);

            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        //[AllowAnonymous]
        public IActionResult Apps()
        {
            try
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(ActionFramework.AppContext.Current);
                return new ContentResult { Content = json, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + ex.StackTrace);
            }
        }

        [HttpGet]
        //[AllowAnonymous]
        [Authorize]
        public IActionResult InstallPackage([FromQuery]string package)
        {
            try
            {
                //ActionFramework.PackageManager.Helper packageManager = new ActionFramework.PackageManager.Helper();
                //packageManager.Install();
                return Ok("Installed");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + ex.StackTrace);
            }
        }

        [HttpGet]
        [Authorize]
        //[AllowAnonymous]
        public IActionResult Settings()
        {
            return Ok(_agentSettings);
        }

        [HttpGet]
        [Authorize]
        //[AllowAnonymous]
        public IActionResult Initialize()
        {
            ActionFramework.AppContext.Initialize();
            return Ok(_agentSettings);
        }

        [HttpGet]
        [Authorize]
        //[AllowAnonymous]
        public IActionResult Configuration()
        {
            return Ok(_configuration);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Status()
        {

            try
            {
                logger.Information("Logging the serilog factory with singleton");
                return Ok($"{_agentSettings.AgentName} running in {ActionFramework.Configuration.ConfigurationManager.Environment} environment");
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"async Task<IActionResult> Get for path: '{Request.Path.Value}' caused an exception.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost, HttpGet]
        public IActionResult Authenticate([FromBody]UserModel userParam)
        {
            try
            {
                logger.Information(string.Format("Authenticating {0}", userParam.Username));
                _authService = new AuthService();
                var user = _authService.Authenticate(userParam.Username, userParam.Password);

                if (user == null)
                    return BadRequest(new { message = "Username or password is incorrect" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + ex.StackTrace);
            }
        }
    }
}
