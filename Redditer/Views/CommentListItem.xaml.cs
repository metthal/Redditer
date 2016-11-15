using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Redditer.Models;
using Redditer.Providers;
using Redditer.Utilities;
using RedditerCore.Reddit;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Redditer.Views
{
    public sealed partial class CommentListItem : UserControl
    {
        public CommentListItem()
        {
            this.InitializeComponent();
        }

        private async void UpvoteTapped(object sender, TappedRoutedEventArgs e)
        {
            var comment = DataContext as Comment;
            if (comment == null)
                return;

            var subredditThreadPage = Tag as SubredditThreadPage;
            if (subredditThreadPage == null)
                return;

            if (!subredditThreadPage.ViewModel.IsLoggedIn)
            {
                var dialog = new MessageDialog("You need to be logged in to vote");
                await dialog.ShowAsync();
                return;
            }

            if (comment.Upvoted)
            {
                upvoteButton.Foreground = new SolidColorBrush(Colors.White);
                scoreText.Foreground = new SolidColorBrush(Colors.White);
                scoreText.Text = Convert.ToString(Convert.ToInt32(scoreText.Text) - 1);
                comment.Likes = Maybe<bool>.Nothing();
                await Reddit.Instance.Vote(comment.Name, RedditClient.VoteType.Unvote);
            }
            else
            {
                upvoteButton.Foreground = new SolidColorBrush(Colors.DodgerBlue);
                scoreText.Foreground = new SolidColorBrush(Colors.DodgerBlue);
                downvoteButton.Foreground = new SolidColorBrush(Colors.White);
                scoreText.Text = Convert.ToString(Convert.ToInt32(scoreText.Text) + (comment.Downvoted ? 2 : 1));
                comment.Likes = Maybe<bool>.Just(true);
                await Reddit.Instance.Vote(comment.Name, RedditClient.VoteType.Upvote);
            }
        }

        private async void DownvoteTapped(object sender, TappedRoutedEventArgs e)
        {
            var comment = DataContext as Comment;
            if (comment == null)
                return;

            var subredditThreadPage = Tag as SubredditThreadPage;
            if (subredditThreadPage == null)
                return;

            if (!subredditThreadPage.ViewModel.IsLoggedIn)
            {
                var dialog = new MessageDialog("You need to be logged in to vote.");
                await dialog.ShowAsync();
                return;
            }

            if (comment.Downvoted)
            {
                downvoteButton.Foreground = new SolidColorBrush(Colors.White);
                scoreText.Foreground = new SolidColorBrush(Colors.White);
                scoreText.Text = Convert.ToString(Convert.ToInt32(scoreText.Text) + 1);
                comment.Likes = Maybe<bool>.Nothing();
                await Reddit.Instance.Vote(comment.Name, RedditClient.VoteType.Unvote);
            }
            else
            {
                upvoteButton.Foreground = new SolidColorBrush(Colors.White);
                scoreText.Foreground = new SolidColorBrush(Colors.DodgerBlue);
                downvoteButton.Foreground = new SolidColorBrush(Colors.DodgerBlue);
                scoreText.Text = Convert.ToString(Convert.ToInt32(scoreText.Text) - (comment.Upvoted ? 2 : 1));
                comment.Likes = Maybe<bool>.Just(false);
                await Reddit.Instance.Vote(comment.Name, RedditClient.VoteType.Downvote);
            }
        }
    }
}
