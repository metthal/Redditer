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
            var comment = Comment.Parse(response.AsObject().Value<JObject>("json").Value<JObject>("data").Value<JArray>("things")[0].Value<JObject>("data"));

            if (CommentedComment != null)
            {
                var index = CommentedThreadPage.ViewModel.Thread.Comments.IndexOf(CommentedComment);
                comment.Depth = CommentedComment.Depth + 1;
                CommentedThreadPage.ViewModel.Thread.Comments.Insert(index + 1, comment);
            }
            else
            {
                comment.Depth = 0;
                CommentedThreadPage.ViewModel.Thread.Comments.Insert(0, comment);
            }
        }

        private string _text;
        private bool _sendingComment;
    }
}
