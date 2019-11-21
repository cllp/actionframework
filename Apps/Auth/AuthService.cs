using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace LLT.API.Auth
{
    public class AuthService
    {
        private List<UserModel> _users;

        public AuthService()
        {
            _users = new List<UserModel>() {
                new UserModel(){
                     Id = 2,
                     Username = "idun",
                     Password = "sqbAW3D97nnfPgcx"
                },
                 new UserModel(){
                     Id = 1,
                     Username = "admin",
                     Password = "visby1234"
                },
                 new UserModel(){
                     Id = 1,
                     Username = "test",
                     Password = "test"
                }
            };
        }

        public UserModel Authenticate(string username, string password)
        {
            var user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);

            //get the secret from Agents configuration
            var secret = ActionFramework.Configuration.ConfigurationManager.Settings["AgentSettings:Secret"];

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Issuer = "llt.sense",
                Subject = new ClaimsIdentity(new Claim[]
                {
                    //todo: need more claims?
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
