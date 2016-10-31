using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RedditerCore.Reddit
{
    public class RedditResponse
    {
        public RedditResponse(string content)
        {
            Content = content;
        }

        public JObject AsObject()
        {
            try
            {
                return JObject.Parse(Content);
            }
            catch (JsonReaderException)
            {
                return null;
            }
        }

        public JArray AsArray()
        {
            try
            {
                return JArray.Parse(Content);
            }
            catch (JsonReaderException)
            {
                return null;
            }
        }

        public JArray ParseListings(out string after, out string before)
        {
            after = null;
            before = null;

            var obj = AsObject();
            if (obj == null)
                return null;

            JToken kind;
            if (obj.TryGetValue("kind", out kind))
            {
                if (kind.ToString() != "Listing")
                    return null;
            }

            var data = obj.Value<JObject>("data");
            var children = data.Value<JArray>("children");
            after = data.Value<string>("after");
            before = data.Value<string>("before");
            return children;
        }

        public bool IsError()
        {
            var obj = AsObject();
            if (obj == null)
                return false;

            JToken error;
            if (obj.TryGetValue("error", out error))
                return error.ToObject<int>() != 0;

            return false;
        }

        public string Content { get; }
    }
}
