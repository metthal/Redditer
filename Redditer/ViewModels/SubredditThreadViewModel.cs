using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Redditer.Models;
using Redditer.Providers;
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
            var opComment = new RedditListings(JObject.Parse(response[0].ToString()));

            Thread.Selftext = opComment.Data[0].Value<JObject>("data").Value<string>("selftext");
            OnPropertyChanged("Thread");

            var comments = new ObservableCollection<Comment>();
            LinearizeComments(comments, JObject.Parse(response[1].ToString()), 0);
            Thread.Comments = comments;
            OnPropertyChanged("Thread");
        }

        public void LinearizeComments(ObservableCollection<Comment> comments, JObject listing, int depth)
        {
            if (listing.HasValues && listing.Value<string>("kind") != "Listing")
                return;

            var children = listing.Value<JObject>("data").Value<JArray>("children");
            foreach (var jcomment in children)
            {
                if (jcomment.Value<string>("kind") != "t1")
                    continue;

                var data = jcomment.Value<JObject>("data");
                var comment = new Comment
                {
                    Author = data.Value<string>("author"),
                    Depth = depth,
                    Text = data.Value<string>("body")
                };
                comments.Add(comment);

                JToken replies;
                if (data.TryGetValue("replies", out replies) && replies.ToString() != "")
                    LinearizeComments(comments, data.Value<JObject>("replies"), depth + 1);
            }
        }

        private SubredditThread _thread;
    }
}
