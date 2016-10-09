using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using RedditerCore.Utilities;
using HttpClient = System.Net.Http.HttpClient;
using HttpRequestMessage = System.Net.Http.HttpRequestMessage;

namespace RedditerCore.Rest
{
    public class RestClient
    {
        public RestClient()
        {
            _handler = new HttpClientHandler();
            _client = new HttpClient(_handler);
            UserAgent = "Redditer";
            Authorization = "";
        }

        public async Task<RestResponse> Execute(RestRequest request)
        {
            UriBuilder requestUri = new UriBuilder(BaseUrl);
            requestUri.Path = request.Resource;
            if (request.QueryParameters.Count > 0)
                requestUri.Query = request.QueryParameters.AsQueryString();

            HttpRequestMessage message = new HttpRequestMessage();
            if (UserAgent.Length > 0)
                message.Headers.Add("User-Agent", UserAgent);
            if (Authorization.Length > 0)
                message.Headers.Add("Authorization", Authorization);
            message.Headers.Add("Accept", "application/json, application/xml, text/json, text/x-json, text/javascript, text/xml");
            //message.Headers.Add("Accept-Encoding", "gzip, deflate");
            message.RequestUri = requestUri.Uri;
            message.Method = request.Method;
            if (request.Parameters.Count > 0)
                message.Content = new FormUrlEncodedContent(request.Parameters);

            var response = await _client.SendAsync(message);
            return new RestResponse(response);
        }

        public CookieContainer CookieContainer
        {
            get { return _handler.CookieContainer; }
            set { _handler.CookieContainer = value; }
        }

        public bool FollowRedirects
        {
            get { return _handler.AllowAutoRedirect; }
            set { _handler.AllowAutoRedirect = value; }
        }

        public Uri BaseUrl
        {
            get { return _client.BaseAddress; }
            set { _client.BaseAddress = value; }
        }

        public string UserAgent { get; set; }
        public string Authorization { get; set; }

        private HttpClientHandler _handler;
        private HttpClient _client;
    }
}
