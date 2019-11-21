using System;
using System.Collections.Generic;

namespace Agent.Auth
{
    public interface IAuthService
    {
        UserModel Authenticate(string username, string password);
        IEnumerable<UserModel> GetAll();
    }
}
