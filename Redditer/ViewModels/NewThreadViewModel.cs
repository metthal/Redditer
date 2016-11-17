using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Redditer.Models;
using Redditer.Providers;
using RedditerCore.Reddit;

namespace Redditer.ViewModels
{
    public class NewThreadViewModel : BaseViewModel
    {
        public NewThreadViewModel()
        {
            Subreddit = null;
            Title = "";
            Text = "";
            Link = "";
            SendRepliesToInbox = true;
        }

        public Subreddit Subreddit
        {
            get { return _subreddit; }
            set
            {
                _subreddit = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        public string Link
        {
            get { return _link; }
            set
            {
                _link = value;
                OnPropertyChanged();
            }
        }

        public bool SendRepliesToInbox
        {
            get { return _sendRepliesToInbox; }
            set
            {
                _sendRepliesToInbox = value;
                OnPropertyChanged();
            }
        }

        public async Task SubmitNewTextThread()
        {
            var response = await Reddit.Instance.Submit(RedditClient.SubmitType.Text, Subreddit.Shortname, Title, Text, SendRepliesToInbox);
        }

        public async Task SubmitNewLinkThread()
        {
            var response = await Reddit.Instance.Submit(RedditClient.SubmitType.Link, Subreddit.Shortname, Title, Link, SendRepliesToInbox);
        }

        private Subreddit _subreddit;
        private string _title;
        private string _text;
        private string _link;
        private bool _sendRepliesToInbox;
    }
}
