using System.Collections.ObjectModel;
using System.ComponentModel;
using RedditerCore.Reddit;
using Newtonsoft.Json.Linq;
using Redditer.Models;
using Redditer.Views;

namespace Redditer.ViewModels
{
    public class SubredditViewModel : BaseViewModel
    {
        public SubredditViewModel()
        {
            SortType = new ObservableCollection<string>{ "hot", "new", "top", "controversial", "gilded" };

            _currentSubreddit = new Subreddit("/r/all", new ObservableCollection<SubredditThread>());
            _reddit = new RedditClient();
        }

        public async void LoadSubreddit(string subreddit, string sortType)
        {
            var threadListings = await _reddit.ListThreads(subreddit, sortType);

            var newThreads = new ObservableCollection<SubredditThread>();
            foreach (var jthread in threadListings.Data)
            {
                var jthreadData = jthread.Value<JObject>("data");
                newThreads.Add(new SubredditThread
                {
                    Link = jthreadData.Value<string>("permalink"),
                    Title = jthreadData.Value<string>("title")
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

        private readonly RedditClient _reddit;
        private Subreddit _currentSubreddit;
    }
}
