using System;

namespace RedditerCore.Authentication
{
    public interface IUserAuthenticator
    {
        Tuple<string, string> OnLogInChallenge();
        bool OnAppAuthorizeChallenge();
    }
}
