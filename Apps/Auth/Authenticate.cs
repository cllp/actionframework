using System;
using Action = ActionFramework.Action;
using Newtonsoft.Json;
using ActionFramework.Logger;

namespace LLT.API.Auth
{
    public class Authenticate : Action
    {
       
        public override object Run(dynamic jsonData)
        {
            try
            {
                dynamic obj = JsonConvert.DeserializeObject(jsonData.ToString());
                string username = obj.username;
                string password = obj.password;

                var _authService = new AuthService();
                var user = _authService.Authenticate(username, password);

                if (user == null)
                {
                    LogFactory.File.Warning($"Could not authenticate '{username}'. Username or password is incorrect");
                    return "Username or password is incorrect";
                }

                LogFactory.File.Warning($"Successfully authenticated user: '{username}'");
                return user;

            }
            catch (Exception ex)
            {
                LogFactory.File.Error(ex, $"LLT Sense Authenticate caused an exception");
                throw ex;
            }
        }

    }
}
