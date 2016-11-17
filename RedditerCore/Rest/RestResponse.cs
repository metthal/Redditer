using System;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using HttpStatusCode = Windows.Web.Http.HttpStatusCode;

namespace RedditerCore.Rest
{
    public class RestResponse
    {
        public RestResponse(HttpResponseMessage message)
        {
            Message = message;
        }

        public async Task<string> Content()
        {
            return await Message.Content.ReadAsStringAsync();
        }

        public HttpResponseMessage Message { get; }
        public HttpResponseHeaderCollection Headers => Message.Headers;
        public HttpStatusCode StatusCode => Message.StatusCode;
        public bool Success => Message.IsSuccessStatusCode;
        public string ReasonPhrase => Message.ReasonPhrase;
    }
}
