using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Agent.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace Agent.Auth
{
    
    public class AuthService : IAuthService
    {
        private List<UserModel> _users;
        private static readonly AgentSettings agentSettings = Startup.AgentSettings; //ConfigurationManager.Settings["AppSettings:Secret"];//"Microsoft.IdentityModel.Tokens.X509SecurityKey cannot be smaller than 1024 bits";
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications

        //private readonly AppSettings _appSettings;

        public AuthService()
        {
            _users = new List<UserModel>() {
                new UserModel(){
                     Id = 2,
                     Username = agentSettings.Username,
                     Password = agentSettings.Password
                },
                 new UserModel(){
                     Id = 1,
                     Username = "admin",
                     Password = "visby1234"
                },
            };
        }

        public UserModel Authenticate(string username, string password)
        {
            var user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(agentSettings.Secret); //_appSettings.Secret
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Username.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            user.Password = "***";

            return user;
        }

        public IEnumerable<UserModel> GetAll()
        {
            // return users without passwords
            return _users.Select(x => {
                x.Password = "***";
                return x;
            });
        }
    }
}