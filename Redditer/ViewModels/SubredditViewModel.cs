using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp;
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
            SortType = new ObservableCollection<string>{ "hot", "new", "top", "controversial" };
            QueriedSubreddits = new ObservableCollection<string>();

            CurrentSubreddit = new Subreddit("/r/all", new ObservableCollection<SubredditThread>());
            SelectedThread = null;
            SelectedQueriedSubreddit = null;
            IsSubredditLoading = true;
            IsEndlessLoading = false;
            IsNotConnectedToInternet = false;

            _nextListing = null;
        }

        public async Task<bool> Login()
        {
            var auth = new Authenticator.Authenticator();
            await Reddit.Instance.LogIn(auth);

            OnPropertyChanged("IsLoggedIn");
            OnPropertyChanged("IsNotLoggedIn");
            OnPropertyChanged("User");
            return IsLoggedIn;
        }

        public void Logout()
        {
            var auth = new Authenticator.Authenticator();
            Reddit.Instance.LogOut(auth);

            OnPropertyChanged("IsLoggedIn");
            OnPropertyChanged("IsNotLoggedIn");
            OnPropertyChanged("User");
        }

        public async void LoadSubreddit(string subreddit, string sortType)
        {
            IsSubredditLoading = true;

            if (!ConnectionHelper.IsInternetAvailable)
            {
                IsNotConnectedToInternet = true;
                IsSubredditLoading = false;
                return;
            }

            IsNotConnectedToInternet = false;
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
                newThreads.Add(SubredditThread.Parse(jthread.Value<JObject>("data")));
            }

            CurrentSubreddit = new Subreddit(subreddit, newThreads);
            QueriedSubreddits = Favorites;
            OnPropertyChanged("InFavorites");
            OnPropertyChanged("NotInFavorites");
            IsSubredditLoading = false;
        }

        public async Task NextThreads(string sortType)
        {
            if (IsEndlessLoading)
                return;
            IsEndlessLoading = true;

            string dummy;
            var response = await Reddit.Instance.ListThreads(CurrentSubreddit.Name, sortType, _nextListing, CurrentSubreddit.Threads.Count);
            var threadListings = response.ParseListings(out _nextListing, out dummy);
            if (threadListings == null)
                return;

            IsEndlessLoading = false;

            foreach (var jthread in threadListings)
            {
                CurrentSubreddit.Threads.Add(SubredditThread.Parse(jthread.Value<JObject>("data")));
            }
        }

        public async void QuerySubreddits(string prefix)
        {
            if (prefix == "")
            {
                QueriedSubreddits = Favorites;
                return;
            }

            if (!ConnectionHelper.IsInternetAvailable)
                return;

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

        public bool IsSubredditLoading
        {
            get { return _isSubredditLoading; }
            set
            {
                _isSubredditLoading = value;
                OnPropertyChanged();
                OnPropertyChanged("IsSubredditLoaded");
            }
        }

        public bool IsSubredditLoaded
        {
            get { return !_isSubredditLoading; }
            set
            {
                _isSubredditLoading = !value;
                OnPropertyChanged();
                OnPropertyChanged("IsSubredditLoading");
            }
        }

        public bool IsEndlessLoading
        {
            get { return _isEndlessLoading; }
            set
            {
                _isEndlessLoading = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Favorites => new ObservableCollection<string>(Settings.Instance.Favorites);
        public bool InFavorites
        {
            get { return Settings.Instance.Favorites.Contains(CurrentSubreddit.Shortname); }
            set
            {
                if (value)
                {
                    if (!Settings.Instance.Favorites.Contains(CurrentSubreddit.Shortname))
                        Settings.Instance.Favorites.Add(CurrentSubreddit.Shortname);
                }
                else
                    Settings.Instance.Favorites.Remove(CurrentSubreddit.Shortname);

                QueriedSubreddits = Favorites;
                OnPropertyChanged();
                OnPropertyChanged("NotInFavorites");
                OnPropertyChanged("Favorites");
            }
        }
        public bool NotInFavorites => !InFavorites;

        public bool IsLoggedIn => Reddit.Instance.User.Authenticated;
        public bool IsNotLoggedIn => !IsLoggedIn;
        public string User => Reddit.Instance.User.Username;

        public bool IsNotConnectedToInternet
        {
            get { return _isNotConnectedToInternet; }
            set
            {
                _isNotConnectedToInternet = value;
                OnPropertyChanged();
            }
        }

        private Subreddit _currentSubreddit;
        private SubredditThread _selectedThread;
        private string _nextListing;
        private ObservableCollection<string> _queriedSubreddits;
        private bool _isSubredditLoading;
        private bool _isEndlessLoading;
        private bool _isNotConnectedToInternet;
    }
}
