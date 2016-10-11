using System.Collections.ObjectModel;
using RedditerCore.Reddit;
using Newtonsoft.Json.Linq;

namespace Redditer.ViewModels
{
    public class SubredditViewModel
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

            foreach (var jthread in threadListings.Data)
            {
                Threads.Add(jthread.Value<JObject>("data").Value<string>("title"));
            }
        }

        public ObservableCollection<string> SortType { get; }
        public ObservableCollection<string> Threads { get; }

        private readonly RedditClient _reddit;
    }
}
