using System;
using System.Threading.Tasks;

namespace RedditerCore.Authentication
{
    public interface IUserAuthenticator
    {
        Task<Tuple<string, string>> OnLogInChallenge();
        Task<bool> OnAppAuthorizeChallenge();
    }
}
