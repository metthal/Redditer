using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

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
        public HttpResponseHeaders Headers => Message.Headers;
        public HttpStatusCode StatusCode => Message.StatusCode;
        public bool Success => Message.IsSuccessStatusCode;
        public string ReasonPhrase => Message.ReasonPhrase;
    }
}
