using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using RedditerCore.Utilities;
using HttpClient = Windows.Web.Http.HttpClient;
using HttpRequestMessage = Windows.Web.Http.HttpRequestMessage;

namespace RedditerCore.Rest
{
    public class RestClient
    {
        public RestClient()
        {
            _filter = new HttpBaseProtocolFilter
            {
                AutomaticDecompression = true
            };
            _client = new HttpClient(_filter);
            UserAgent = "Redditer";
            Authorization = "";
        }

        public async Task<RestResponse> Execute(RestRequest request)
        {
            var requestUri = new UriBuilder(BaseUrl) { Path = request.Resource };
            if (request.QueryParameters.Count > 0)
                requestUri.Query = request.QueryParameters.AsQueryString();

            HttpRequestMessage message = new HttpRequestMessage();
            if (UserAgent.Length > 0)
                message.Headers.Add("User-Agent", UserAgent);
            if (Authorization.Length > 0)
                message.Headers.Add("Authorization", Authorization);
            message.Headers.Add("Accept", "application/json, application/xml, text/json, text/x-json, text/javascript, text/xml");
            message.Headers.Add("Accept-Encoding", "gzip, deflate");
            message.RequestUri = requestUri.Uri;
            message.Method = request.Method;
            if (request.Parameters.Count > 0)
                message.Content = new HttpFormUrlEncodedContent(request.Parameters);

            var response = await _client.SendRequestAsync(message);
            return new RestResponse(response);
        }

        public void RemoveCookie(string cookieName)
        {
            var cookies = _filter.CookieManager.GetCookies(BaseUrl);
            var cookie = cookies.Single(httpCookie => httpCookie.Name == cookieName);
            if (cookie == null)
                return;

            _filter.CookieManager.DeleteCookie(cookie);
        }

        public bool FollowRedirects
        {
            get { return _filter.AllowAutoRedirect; }
            set { _filter.AllowAutoRedirect = value; }
        }

        public Uri BaseUrl { get; set; }
        public string UserAgent { get; set; }
        public string Authorization { get; set; }

        private readonly HttpClient _client;
        private readonly HttpBaseProtocolFilter _filter;
    }
}
