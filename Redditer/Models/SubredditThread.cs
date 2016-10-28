using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditerCore.Reddit;

namespace Redditer.Models
{
    public class SubredditThread
    {
        public string Link { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int Score { get; set; }
        public string Selftext { get; set; }
        public ObservableCollection<Comment> Comments { get; set; }
    }
}
