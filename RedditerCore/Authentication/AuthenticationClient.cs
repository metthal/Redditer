using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RedditerCore.Exceptions;
using RedditerCore.Rest;
using RedditerCore.Utilities;
using HttpMethod = System.Net.Http.HttpMethod;

namespace RedditerCore.Authentication
{
    public class AuthenticationClient : RestClient
    {
        public readonly string AuthBaseUrl = "https://ssl.reddit.com/";
        public readonly string AuthResource = "/api/v1/authorize";
        public readonly string LoginResource = "/post/login";
        public readonly string ClientId = "bHymir0Cz5KU-w";
        public readonly string RedirectUri = "https://github.com/metthal/Redditer";
        public readonly string Scopes = "identity";

        public AuthenticationClient()
        {
            CookieContainer = new CookieContainer();
            FollowRedirects = false;
            BaseUrl = new Uri(AuthBaseUrl);

            ObtainedToken = null;

            _state = null;
            _uh = null;
        }

        public async Task<RestResponse> AuthenticationRequest(HttpMethod method)
        {
            if (method == HttpMethod.Get)
                _state = RandomString.Create(16);

            // First send authentication request
            var request = new RestRequest(method) { Resource = AuthResource };

            AddParameter addParameter = request.AddParameter;
            if (method == HttpMethod.Get)
                addParameter = request.AddQueryParameter;

            addParameter("state", _state);
            addParameter("response_type", "token");
            addParameter("scope", Scopes);
            addParameter("client_id", ClientId);
            addParameter("redirect_uri", RedirectUri);
            if (method == HttpMethod.Post)
            {
                addParameter("uh", _uh);
                addParameter("authorize", "Allow");
            }
            var response = await Execute(request);

            // Follow redirect
            request = new RestRequest(HttpMethod.Get) { Resource = response.Headers.Location.AbsolutePath };
            if (method == HttpMethod.Get)
            {
                request.QueryParameters = response.Headers.Location.Query.Substring(1).SplitToKeyValuePairs('&', '=');
                response = await Execute(request);
            }
            else
            {
                var token = response.Headers.Location.Fragment.Substring(1).SplitToDictionary('&', '=');
                ObtainedToken = new Token(token["token_type"], token["access_token"], Convert.ToUInt32(token["expires_in"]));
            }
            return response;
        }

        public async Task<RestResponse> LogInRequest(User user)
        {
            // First send login request
            var request = new RestRequest(HttpMethod.Post) { Resource = LoginResource };
            request.AddParameter("op", "login");
            request.AddParameter("dest", BuildAuthenticationQuery());
            request.AddParameter("user", user.Username);
            request.AddParameter("passwd", user.Password);
            request.AddParameter("api_type", "json");
            var response = await Execute(request);

            // We are not logged in, wrong username or password
            if (response.Headers.Location == null)
                throw new WrongUsernameOrPasswordException();

            // Follow redirect
            request = new RestRequest(HttpMethod.Get) { Resource = response.Headers.Location.AbsolutePath };
            request.QueryParameters = response.Headers.Location.Query.Substring(1).SplitToKeyValuePairs('&', '=');
            response = await Execute(request);

            // Try to parse 'uh' string from HTML page
            string content = await response.Content();
            var startIndex = content.IndexOf("<input type=\"hidden\" name=\"uh\" value=\"", StringComparison.Ordinal) + "<input type=\"hidden\" name=\"uh\" value=\"".Length;
            var endIndex = content.IndexOf("\"", startIndex, StringComparison.Ordinal);
            _uh = content.Substring(startIndex, endIndex - startIndex);

            return response;
        }

        private string BuildAuthenticationQuery()
        {
            var query = new StringBuilder();
            query.Append(AuthBaseUrl).Append(AuthResource, 1, AuthResource.Length - 1).Append('?');
            query.Append("state=").Append(_state).Append('&');
            query.Append("response_type=").Append("token").Append('&');
            query.Append("scope=").Append(Scopes).Append('&');
            query.Append("client_id=").Append(ClientId).Append('&');
            query.Append("redirect_uri=").Append(RedirectUri);
            return query.ToString();
        }

        public Token ObtainedToken { get; private set; }

        private delegate void AddParameter(string key, string value);

        private string _state;
        private string _uh;
    }
}
