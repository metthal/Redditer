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

            _reddit = new RedditClient();
        }

        public async void LoadSubreddit(string subreddit)
        {
            var threadListings = await _reddit.ListThreads(subreddit);

            var newThreads = new ObservableCollection<string>();
            foreach (var jthread in threadListings.Data)
            {
                newThreads.Add(jthread.Value<JObject>("data").Value<string>("title"));
            }

            Threads = newThreads;
        }

        public ObservableCollection<string> SortType { get; }
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
        private ObservableCollection<string> _threads;
    }
}
