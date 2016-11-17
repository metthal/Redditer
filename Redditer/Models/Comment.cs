using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Redditer.Providers;
using Redditer.Utilities;

namespace Redditer.Models
{
    public class Comment
    {
        public Comment()
        {
            LoadMoreCommentsLink = Maybe<string>.Nothing();
        }

        public static Comment Parse(JObject data)
        {
            return new Comment
            {
                Id = data.Value<string>("id"),
                Name = data.Value<string>("name"),
                Author = data.Value<string>("author"),
                Text = WebUtility.HtmlDecode(data.Value<string>("body")),
                Score = data.Value<int>("score"),
                Flair = data.Value<string>("author_flair_text"),
                Created = DateTimeHelper.FromTimestamp(data.Value<ulong>("created_utc")),
                Edited = data["edited"].Type == JTokenType.Boolean
                    ? Maybe<DateTime>.Nothing()
                    : Maybe<DateTime>.Just(DateTimeHelper.FromTimestamp(data.Value<ulong>("edited"))),
                Gilded = data.Value<int>("gilded"),
                Likes = data["likes"].Type == JTokenType.Boolean
                    ? Maybe<bool>.Just(data.Value<bool>("likes"))
                    : Maybe<bool>.Nothing()
            };
        }

        public int Depth { get; set; }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string Author { get; set; }
        public int Score { get; set; }
        public string Flair { get; set; }
        public DateTime Created { get; set; }
        public Maybe<DateTime> Edited { get; set; }
        public int Gilded { get; set; }
        public bool IsGilded => Gilded > 0;
        public bool IsMultigilded => Gilded > 1;
        public Maybe<bool> Likes { get; set; }
        public bool Upvoted => Reddit.Instance.User.Authenticated && Likes != null && Likes.Defined && Likes.Value;
        public bool Downvoted => Reddit.Instance.User.Authenticated && Likes != null && Likes.Defined && !Likes.Value;
        public bool Voted => Upvoted || Downvoted;
        public bool RegularComment => !LoadMoreComments;

        public Maybe<string> LoadMoreCommentsLink { get; set; }
        public bool LoadMoreComments => LoadMoreCommentsLink.Defined;
        public int LoadMoreCommentsCount { get; set; }
        public List<string> LoadMoreCommentsChildren { get; set; }
    }
}
