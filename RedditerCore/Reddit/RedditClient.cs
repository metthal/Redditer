﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedditerCore.Authentication;
using RedditerCore.Rest;

namespace RedditerCore.Reddit
{
    public class RedditClient : RestClient
    {
        public readonly string RedditBaseUrl = "https://ssl.reddit.com/";
        public readonly string RedditBaseOAuthUrl = "https://oauth.reddit.com/";

        public RedditClient()
        {
            BaseUrl = new Uri(RedditBaseUrl);
            User = new User();

            _authClient = new AuthenticationClient();
        }

        public RedditClient(User user)
        {
            BaseUrl = new Uri(RedditBaseOAuthUrl);
            User = user;

            _authClient = new AuthenticationClient();
        }

        public async Task<JObject> Call(HttpMethod method, string apiMethod, params string[] args)
        {
            // Odd number of parameters
            if ((args.Length & 1) == 1)
                throw new ArgumentException();

            if (User.Authenticated && User.AccessToken.Expired)
                await RenewToken();

            var request = new RestRequest(method) { Resource = apiMethod };
            for (var i = 0; i < args.Length; i += 2)
            {
                var key = args[i];
                var value = args[i + 1];
                if (method == HttpMethod.Get)
                    request.AddQueryParameter(key, value);
                else
                    request.AddParameter(key, value);
            }

            var response = await Execute(request);
            var content = await response.Content();

            return JObject.Parse(content);
        }

        public async Task<Token> LogIn(IUserAuthenticator authenticator)
        {
            await _authClient.AuthenticationRequest(HttpMethod.Get);

            if (User.Username == null && User.Password == null)
            {
                var credentials = await authenticator.OnLogInChallenge();
                User.Username = credentials.Item1;
                User.Password = credentials.Item2;
            }

            // User requested to cancel login process
            if (User.Username == null && User.Password == null)
                return null;

            await _authClient.LogInRequest(User);

            if (await authenticator.OnAppAuthorizeChallenge())
            {
                await _authClient.AuthenticationRequest(HttpMethod.Post);
                UpdateAuthenticationInfo(_authClient.ObtainedToken);
            }

            return User.AccessToken;
        }

        public async Task<Token> RenewToken()
        {
            await _authClient.AuthenticationRequest(HttpMethod.Post);
            User.AccessToken = _authClient.ObtainedToken;
            return User.AccessToken;
        }

        public async Task<JObject> AboutMe()
        {
            return await Call(HttpMethod.Get, "/api/v1/me");
        }

        public async Task<JObject> ListThreads(string subreddit)
        {
            if (subreddit == "")
                subreddit = "/r/all";

            return await Call(HttpMethod.Get, subreddit + "/hot/.json");
        }

        protected void UpdateAuthenticationInfo(Token token)
        {
            User.AccessToken = token;
            Authorization = token != null ? token.ToString() : "";
            BaseUrl = new Uri(User.Authenticated ? RedditBaseOAuthUrl : RedditBaseUrl);
        }

        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                UpdateAuthenticationInfo(_user.AccessToken);
            }
        }

        private User _user;

        private readonly AuthenticationClient _authClient;
    }
}
