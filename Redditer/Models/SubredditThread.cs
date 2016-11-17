using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Redditer.Providers;
using Redditer.Utilities;
using RedditerCore.Reddit;

namespace Redditer.Models
{
    public class SubredditThread
    {
        public static SubredditThread Parse(JObject data)
        {
            return new SubredditThread
            {
                Name = data.Value<string>("name"),
                Link = data.Value<string>("permalink"),
                Title = WebUtility.HtmlDecode(data.Value<string>("title")),
                Author = data.Value<string>("author"),
                Score = data.Value<int>("score"),
                Subreddit = data.Value<string>("subreddit"),
                Nsfw = data.Value<bool>("over_18"),
                Sticky = data.Value<bool>("stickied"),
                Flairs = WebUtility.HtmlDecode(data.Value<string>("link_flair_text")),
                NumberOfComments = data.Value<int>("num_comments"),
                Created = DateTimeHelper.FromTimestamp(data.Value<ulong>("created_utc")),
                Edited = data["edited"].Type == JTokenType.Boolean
                    ? Maybe<DateTime>.Nothing()
                    : Maybe<DateTime>.Just(DateTimeHelper.FromTimestamp(data.Value<ulong>("edited"))),
                Thumbnail = data.Value<string>("thumbnail"),
                Domain = data.Value<string>("domain"),
                Selfpost = data.Value<bool>("is_self"),
                Url = data.Value<string>("url"),
                Likes = data["likes"].Type == JTokenType.Boolean
                    ? Maybe<bool>.Just(data.Value<bool>("likes"))
                    : Maybe<bool>.Nothing()
            };
        }

        public string Name { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int Score { get; set; }
        public string Subreddit
        {
            get { return _subreddit; }
            set
            {
                _subreddit = value;
                if (!_subreddit.StartsWith("/r/"))
                    _subreddit = _subreddit.Insert(0, "/r/");
            }
        }
        public bool Nsfw { get; set; }
        public bool Sticky { get; set; }
        public string Flairs { get; set; }
        public int NumberOfComments { get; set; }
        public DateTime Created { get; set; }
        public Maybe<DateTime> Edited { get; set; }
        public string Thumbnail { get; set; }
        public bool HasThumbnail => Thumbnail.StartsWith("http");
        public string Domain { get; set; }
        public bool Selfpost { get; set; }
        public bool Linkpost => !Selfpost;
        public string Url { get; set; }
        public string Selftext { get; set; }
        public Maybe<bool> Likes { get; set; }
        public bool Upvoted => Reddit.Instance.User.Authenticated && Likes.Defined && Likes.Value;
        public bool Downvoted => Reddit.Instance.User.Authenticated && Likes.Defined && !Likes.Value;
        public bool Voted => Upvoted || Downvoted;
        public ObservableCollection<Comment> Comments { get; set; }

        private string _subreddit;
    }
}
