using Newtonsoft.Json.Linq;

namespace RedditerCore.Reddit
{
    public class RedditListings
    {
        public RedditListings(JObject obj)
        {
            _object = obj;
        }

        public JArray Data => _object.Value<JObject>("data").Value<JArray>("children");
        public string Next => _object.Value<JObject>("data").Value<string>("after");
        public string Previous => _object.Value<JObject>("data").Value<string>("before");

        private readonly JObject _object;
    }
}
