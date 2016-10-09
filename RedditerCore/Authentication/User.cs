using System.Net.Http;
using System.Threading.Tasks;

namespace RedditerCore.Authentication
{
    public class User
    {
        public User()
        {
            Username = null;
            Password = null;
            AccessToken = null;

            _client = new AuthenticationClient();
        }

        public User(string username, string password) : this()
        {
            Username = username;
            Password = password;
        }

        public async Task<Token> LogIn(IUserAuthenticator authenticator)
        {
            await _client.AuthenticationRequest(HttpMethod.Get);

            if (Username == null && Password == null)
            {
                var credentials = authenticator.OnLogInChallenge();
                Username = credentials.Item1;
                Password = credentials.Item2;
            }
            await _client.LogInRequest(Username, Password);

            if (authenticator.OnAppAuthorizeChallenge())
            {
                await _client.AuthenticationRequest(HttpMethod.Post);
                AccessToken = _client.ObtainedToken;
            }

            return AccessToken;
        }

        public void LogOut()
        {
        }

        public bool Authenticated => AccessToken != null;
        public string Username { get; set; }
        public string Password { get; set; }
        public Token AccessToken { get; private set; }

        private readonly AuthenticationClient _client;
    }
}
