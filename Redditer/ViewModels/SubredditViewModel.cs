using System.Collections.ObjectModel;
using System.ComponentModel;
using RedditerCore.Reddit;
using Newtonsoft.Json.Linq;

namespace Redditer.ViewModels
{
    public class SubredditViewModel : BaseViewModel
    {
        public SubredditViewModel()
        {
            SortType = new ObservableCollection<string>{ "hot", "new", "top", "controversial", "gilded" };
            Threads = new ObservableCollection<string>();

            _currentSubreddit = "/r/all";

            _reddit = new RedditClient();
        }

        public async void LoadSubreddit(string subreddit, string sortType)
        {
            Threads.Clear();

            var threadListings = await _reddit.ListThreads(subreddit, sortType);

            var newThreads = new ObservableCollection<string>();
            foreach (var jthread in threadListings.Data)
            {
                newThreads.Add(jthread.Value<JObject>("data").Value<string>("title"));
            }

            Threads = newThreads;
            CurrentSubreddit = subreddit;
        }

        public ObservableCollection<string> SortType { get; }
        public string CurrentSubreddit
        {
            get { return _currentSubreddit; }
            set
            {
                _currentSubreddit = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> Threads
        {
            get { return _threads; }
            set
            {
                _threads = value;
                OnPropertyChanged();
            }
        }

        private readonly RedditClient _reddit;
        private string _currentSubreddit;
        private ObservableCollection<string> _threads;
    }
}
