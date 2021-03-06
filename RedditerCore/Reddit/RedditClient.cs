﻿using System;
using System.Threading.Tasks;
using Windows.Web.Http;
using RedditerCore.Authentication;
using RedditerCore.Rest;

namespace RedditerCore.Reddit
{
    public class RedditClient : RestClient
    {
        public readonly string RedditBaseUrl = "https://ssl.reddit.com/";
        public readonly string RedditBaseOAuthUrl = "https://oauth.reddit.com/";

        public enum VoteType
        {
            Downvote = -1,
            Unvote = 0,
            Upvote = 1
        }

        public enum SubmitType
        {
            Text,
            Link
        }

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
            {
                User.Username = null;
                User.Password = null;
                return null;
            }

            if (await _authClient.LogInRequest(User) == null)
            {
                User.Username = null;
                User.Password = null;
                return null;
            }

            if (await authenticator.OnAppAuthorizeChallenge())
            {
                await _authClient.AuthenticationRequest(HttpMethod.Post);
                UpdateAuthenticationInfo(_authClient.ObtainedToken);
            }
            else
            {
                User.Username = null;
                User.Password = null;
                return null;
            }

            authenticator.OnLogIn(User.Username, User.Password);
            return User.AccessToken;
        }

        public void LogOut(IUserAuthenticator authenticator)
        {
            authenticator.OnLogOut(User.Username);
            RemoveCookie("reddit_session");
            User = new User();
        }

        public async Task<RedditResponse> AboutMe()
        {
            var response = await Call(HttpMethod.Get, "/api/v1/me");
            return new RedditResponse(response);
        }

        public async Task<RedditResponse> ListThreads(string subreddit, string sortType, string nextListing = null, int count = 0)
        {
            if (subreddit == "")
                subreddit = "/r/all";

            string response;
            if (nextListing == null)
                response = await Call(HttpMethod.Get, subreddit + "/" + sortType + "/.json");
            else
                response = await Call(HttpMethod.Get, subreddit + "/" + sortType + "/.json",
                    "after", nextListing,
                    "count", Convert.ToString(count));

            return new RedditResponse(response);
        }

        public async Task<RedditResponse> ListComments(string threadLink)
        {
            var response = await Call(HttpMethod.Get, threadLink + "/.json");
            return new RedditResponse(response);
        }

        public async Task<RedditResponse> ListComments(string threadLink, string commentLink)
        {
            var response = await Call(HttpMethod.Get, threadLink + commentLink + "/.json");
            return new RedditResponse(response);
        }

        public async Task<RedditResponse> QuerySubreddits(string prefix)
        {
            var response = await Call(HttpMethod.Post, "/api/search_reddit_names/.json",
                "query", prefix,
                "exact", Convert.ToString(false),
                "include_over_18", Convert.ToString(true),
                "hide_unadvertisable", Convert.ToString(false));
            return new RedditResponse(response);
        }

        public async Task<RedditResponse> Vote(string fullname, VoteType voteType)
        {
            var response = await Call(HttpMethod.Post, "/api/vote",
                "id", fullname,
                "dir", Convert.ToString((int)voteType),
                "rank", "2");
            return new RedditResponse(response);
        }

        public async Task<RedditResponse> Submit(SubmitType type, string subreddit, string title, string text, bool sendReplies)
        {
            var contentKind = type == SubmitType.Link ? "link" : "self";
            var contentAttribute = type == SubmitType.Link ? "url" : "text";

            var response = await Call(HttpMethod.Post, "/api/submit",
                "api_type", "json",
                "sr", subreddit,
                "resubmit", Convert.ToString(true),
                "kind", contentKind,
                "title", title,
                contentAttribute, text,
                "sendreplies", Convert.ToString(sendReplies));
            return new RedditResponse(response);
        }

        public async Task<RedditResponse> Comment(string parentFullname, string text)
        {
            var response = await Call(HttpMethod.Post, "/api/comment",
                "api_type", "json",
                "thing_id", parentFullname,
                "text", text);
            return new RedditResponse(response);
        }

        protected async Task<string> Call(HttpMethod method, string apiMethod, params string[] args)
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

            return content;
        }

        protected async Task<Token> RenewToken()
        {
            await _authClient.AuthenticationRequest(HttpMethod.Post);
            User.AccessToken = _authClient.ObtainedToken;
            return User.AccessToken;
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
