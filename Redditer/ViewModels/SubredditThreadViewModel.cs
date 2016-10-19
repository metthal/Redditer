using System;
using System.Collections.Generic;
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
        }

        private SubredditThread _thread;
    }
}
