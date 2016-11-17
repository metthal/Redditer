using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Redditer.Models;
using Redditer.Providers;
using Redditer.Views;

namespace Redditer.ViewModels
{
    public class AddCommentViewModel : BaseViewModel
    {
        public AddCommentViewModel()
        {
            CommentedThreadPage = null;
            CommentedComment = null;
            Text = "";
            SendingComment = false;
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

        public bool SendingComment
        {
            get { return _sendingComment; }
            set
            {
                _sendingComment = value;
                OnPropertyChanged();
                OnPropertyChanged("NotSendingComment");
            }
        }

        public bool NotSendingComment
        {
            get { return !_sendingComment; }
            set
            {
                _sendingComment = !value;
                OnPropertyChanged();
                OnPropertyChanged("SendingComment");
            }
        }

        public SubredditThreadPage CommentedThreadPage { get; set; }
        public Comment CommentedComment { get; set; }

        public async Task SubmitComment()
        {
            var response = await Reddit.Instance.Comment(CommentedComment != null ? CommentedComment.Name : CommentedThreadPage.ViewModel.Thread.Name, Text);
        }

        private string _text;
        private bool _sendingComment;
    }
}
