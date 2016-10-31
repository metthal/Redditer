using System;
using System.Collections.ObjectModel;
using System.Net;
using Newtonsoft.Json.Linq;
using Redditer.Models;
using Redditer.Providers;
using Redditer.Utilities;
using RedditerCore.Reddit;

namespace Redditer.ViewModels
{
    public class SubredditThreadViewModel : BaseViewModel
    {
        public SubredditThread Thread
        {
            get { return _thread; }
            set
            {
                _thread = value;
                OnPropertyChanged();
            }
        }

        public async void LoadComments()
        {
            var response = await Reddit.Instance.ListComments(Thread.Link);
            var commentArray = response.AsArray();
            if (commentArray == null)
                return;

            string dummy1, dummy2;
            var opCommentListings = new RedditResponse(commentArray[0].ToString()).ParseListings(out dummy1, out dummy2);
            var userCommentsListings = new RedditResponse(commentArray[1].ToString()).ParseListings(out dummy1, out dummy2);

            Thread.Selftext = WebUtility.HtmlDecode(opCommentListings[0].Value<JObject>("data").Value<string>("selftext"));
            OnPropertyChanged("Thread");

            var comments = new ObservableCollection<Comment>();
            LinearizeComments(comments, userCommentsListings, 0);
            Thread.Comments = comments;
            OnPropertyChanged("Thread");
        }

        public void LinearizeComments(ObservableCollection<Comment> comments, JArray listing, int depth)
        {
            foreach (var jcomment in listing)
            {
                if (jcomment.Value<string>("kind") != "t1")
                    continue;

                var data = jcomment.Value<JObject>("data");
                var comment = new Comment
                {
                    Depth = depth,
                    Author = data.Value<string>("author"),
                    Text = WebUtility.HtmlDecode(data.Value<string>("body")),
                    Score = data.Value<int>("score"),
                    Flair = data.Value<string>("author_flair_text"),
                    Created = DateTimeHelper.FromTimestamp(data.Value<ulong>("created_utc")),
                    Edited = data["edited"].Type == JTokenType.Boolean
                        ? Maybe<DateTime>.Nothing()
                        : Maybe<DateTime>.Just(DateTimeHelper.FromTimestamp(data.Value<ulong>("edited"))),
                    Gilded = data.Value<int>("gilded")
                };
                comments.Add(comment);

                JToken replies;
                if (data.TryGetValue("replies", out replies) && replies.ToString() != "")
                {
                    string dummy1, dummy2;
                    var repliesResponse = new RedditResponse(replies.ToString());
                    var repliesListings = repliesResponse.ParseListings(out dummy1, out dummy2);
                    LinearizeComments(comments, repliesListings, depth + 1);
                }
            }
        }

        private SubredditThread _thread;
    }
}
