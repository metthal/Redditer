using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Redditer.Models;
using Redditer.Providers;
using Redditer.Utilities;

namespace Redditer.ViewModels
{
    public class SubredditViewModel : BaseViewModel
    {
        public SubredditViewModel()
        {
            SortType = new ObservableCollection<string>{ "hot", "new", "top", "controversial", "gilded" };
            QueriedSubreddits = new ObservableCollection<string>();

            CurrentSubreddit = new Subreddit("/r/all", new ObservableCollection<SubredditThread>());
            SelectedThread = null;
            SelectedQueriedSubreddit = null;

            _nextListing = null;
        }

        public async void LoadSubreddit(string subreddit, string sortType)
        {
            if (!subreddit.StartsWith("/r/"))
                subreddit = subreddit.Insert(0, "/r/");

            string dummy;
            var response = await Reddit.Instance.ListThreads(subreddit, sortType);
            var threadListings = response.ParseListings(out _nextListing, out dummy);
            if (threadListings == null)
                return;

            var newThreads = new ObservableCollection<SubredditThread>();
            foreach (var jthread in threadListings)
            {
                newThreads.Add(ParseThread(jthread.Value<JObject>("data")));
            }

            CurrentSubreddit = new Subreddit(subreddit, newThreads);
            QueriedSubreddits.Clear();
        }

        public async Task NextThreads(string sortType)
        {
            string dummy;
            var response = await Reddit.Instance.ListThreads(CurrentSubreddit.Name, sortType, _nextListing, CurrentSubreddit.Threads.Count);
            var threadListings = response.ParseListings(out _nextListing, out dummy);
            if (threadListings == null)
                return;

            foreach (var jthread in threadListings)
            {
                CurrentSubreddit.Threads.Add(ParseThread(jthread.Value<JObject>("data")));
            }
        }

        public async void QuerySubreddits(string prefix)
        {
            var response = await Reddit.Instance.QuerySubreddits(prefix);
            var queryResult = response.AsObject();
            if (queryResult == null)
                return;

            var subreddits = queryResult.Value<JArray>("names");

            var queriedSubreddits = new ObservableCollection<string>();
            foreach (var subreddit in subreddits)
            {
                queriedSubreddits.Add(subreddit.ToString());
            }

            QueriedSubreddits = queriedSubreddits;
        }

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

        public ObservableCollection<string> QueriedSubreddits
        {
            get { return _queriedSubreddits; }
            private set
            {
                _queriedSubreddits = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> SortType { get; }

        public string SelectedQueriedSubreddit { get; set; }

        private SubredditThread ParseThread(JObject jobject)
        {
            return new SubredditThread
                {
                    Link = jobject.Value<string>("permalink"),
                    Title = WebUtility.HtmlDecode(jobject.Value<string>("title")),
                    Author = jobject.Value<string>("author"),
                    Score = jobject.Value<int>("score"),
                    Subreddit = jobject.Value<string>("subreddit"),
                    Nsfw = jobject.Value<bool>("over_18"),
                    Sticky = jobject.Value<bool>("stickied"),
                    Flairs = WebUtility.HtmlDecode(jobject.Value<string>("link_flair_text")),
                    NumberOfComments = jobject.Value<int>("num_comments"),
                    Created = DateTimeHelper.FromTimestamp(jobject.Value<ulong>("created_utc")),
                    Edited = jobject["edited"].Type == JTokenType.Boolean
                        ? Maybe<DateTime>.Nothing()
                        : Maybe<DateTime>.Just(DateTimeHelper.FromTimestamp(jobject.Value<ulong>("edited"))),
                    Thumbnail = jobject.Value<string>("thumbnail"),
                    Domain = jobject.Value<string>("domain"),
                    Selfpost = jobject.Value<bool>("is_self"),
                    Url = jobject.Value<string>("url")
                };
        }

        private Subreddit _currentSubreddit;
        private SubredditThread _selectedThread;
        private string _nextListing;
        private ObservableCollection<string> _queriedSubreddits;
    }
}
