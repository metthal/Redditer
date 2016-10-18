using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redditer.Models
{
    public class Subreddit
    {
        public Subreddit(string name, IEnumerable<SubredditThread> threads)
        {
            Name = name;
            Threads = new ObservableCollection<SubredditThread>(threads);
        }

        public string Name { get; set; }
        public ObservableCollection<SubredditThread> Threads { get; set; }
    }
}
