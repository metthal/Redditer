using System.Collections.Generic;
using System.Net.Http;

namespace RedditerCore.Rest
{
    public class RestRequest
    {
        public RestRequest()
        {
            QueryParameters = new List<KeyValuePair<string, string>>();
            Parameters = new List<KeyValuePair<string, string>>();
        }

        public RestRequest(HttpMethod method) : this()
        {
            Method = method;
        }

        public void AddQueryParameter(string key, string value)
        {
            QueryParameters.Add(new KeyValuePair<string, string>(key, value));
        }

        public void AddParameter(string key, string value)
        {
            Parameters.Add(new KeyValuePair<string, string>(key, value));
        }

        public HttpMethod Method { get; set; }
        public string Resource { get; set; }
        public List<KeyValuePair<string, string>> QueryParameters { get; set; }
        public List<KeyValuePair<string, string>> Parameters { get; set; }
    }
}
