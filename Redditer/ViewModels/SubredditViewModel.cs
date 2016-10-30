using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using RedditerCore.Reddit;
using Newtonsoft.Json.Linq;
using Redditer.Models;
using Redditer.Providers;
using Redditer.Utilities;
using Redditer.Views;

namespace Redditer.ViewModels
{
    public class SubredditViewModel : BaseViewModel
    {
        public SubredditViewModel()
        {
            SortType = new ObservableCollection<string>{ "hot", "new", "top", "controversial", "gilded" };

            CurrentSubreddit = new Subreddit("/r/all", new ObservableCollection<SubredditThread>());
            SelectedThread = null;
        }

        public async void LoadSubreddit(string subreddit, string sortType)
        {
            if (!subreddit.StartsWith("/r/"))
                subreddit = subreddit.Insert(0, "/r/");

            var threadListings = await Reddit.Instance.ListThreads(subreddit, sortType);

            var newThreads = new ObservableCollection<SubredditThread>();
            foreach (var jthread in threadListings.Data)
            {
                var jthreadData = jthread.Value<JObject>("data");
                newThreads.Add(new SubredditThread
                {
                    Link = jthreadData.Value<string>("permalink"),
                    Title = WebUtility.HtmlDecode(jthreadData.Value<string>("title")),
                    Author = jthreadData.Value<string>("author"),
                    Score = jthreadData.Value<int>("score"),
                    Subreddit = jthreadData.Value<string>("subreddit"),
                    Nsfw = jthreadData.Value<bool>("over_18"),
                    Sticky = jthreadData.Value<bool>("stickied"),
                    Flairs = WebUtility.HtmlDecode(jthreadData.Value<string>("link_flair_text")),
                    NumberOfComments = jthreadData.Value<int>("num_comments"),
                    Created = DateTimeHelper.FromTimestamp(jthreadData.Value<ulong>("created_utc")),
                    Edited = jthreadData["edited"].Type == JTokenType.Boolean
                        ? Maybe<DateTime>.Nothing()
                        : Maybe<DateTime>.Just(DateTimeHelper.FromTimestamp(jthreadData.Value<ulong>("edited"))),
                    Thumbnail = jthreadData.Value<string>("thumbnail"),
                    Domain = jthreadData.Value<string>("domain")
                });
            }

            CurrentSubreddit = new Subreddit(subreddit, newThreads);
        }

        public ObservableCollection<string> SortType { get; }
        public Subreddit CurrentSubreddit
        {
            get { return _currentSubreddit; }
            set
            {
                _currentSubreddit = value;
                OnPropertyChanged();
            }
        }
        public SubredditThread SelectedThread
        {
            get { return _selectedThread; }
            set
            {
                _selectedThread = value;
                OnPropertyChanged();
            }
        }

        private Subreddit _currentSubreddit;
        private SubredditThread _selectedThread;
    }
}
