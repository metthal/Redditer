using System;
using System.Threading.Tasks;

namespace RedditerCore.Authentication
{
    public interface IUserAuthenticator
    {
        Task<Tuple<string, string>> OnLogInChallenge();
        void OnLogIn(string username, string password);
        Task<bool> OnAppAuthorizeChallenge();
        void OnLogOut(string username);
    }
}
