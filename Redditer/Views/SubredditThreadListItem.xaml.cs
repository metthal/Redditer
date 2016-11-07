using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.Notifications;
using Redditer.Models;
using Redditer.Providers;
using Redditer.Utilities;
using Redditer.ViewModels;
using RedditerCore.Reddit;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Redditer.Views
{
    public sealed partial class SubredditThreadListItem : UserControl
    {
        public SubredditThreadListItem()
        {
            this.InitializeComponent();
        }

        public bool ExtendedMenu
        {
            get { return (bool)GetValue(ExtendedMenuProperty); }
            set { SetValue(ExtendedMenuProperty, value); }
        }

        public static readonly DependencyProperty ExtendedMenuProperty =
            DependencyProperty.Register("ExtendedMenu", typeof(bool), typeof(SubredditThreadListItem), new PropertyMetadata(false));

        private void ShowAuthorProfile(object sender, RoutedEventArgs e)
        {
            var toastContent = new ToastContent
            {
                Visual = new ToastVisual
                {
                    BindingGeneric = new ToastBindingGeneric
                    {
                        Children =
                        {
                            new AdaptiveText {Text = "Redditer"},
                            new AdaptiveText {Text = "Not Implemented Yet"}
                        }
                    }
                }
            };
            var toast = new ToastNotification(toastContent.GetXml()) { ExpirationTime = DateTimeOffset.Now.AddSeconds(4) };

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private void ShowComments(object sender, RoutedEventArgs e)
        {
            var subredditPage = Tag as SubredditPage;
            subredditPage?.OpenSelectedThreadComments();
        }

        private void ReportThread(object sender, RoutedEventArgs e)
        {
            var toastContent = new ToastContent
            {
                Visual = new ToastVisual
                {
                    BindingGeneric = new ToastBindingGeneric
                    {
                        Children =
                        {
                            new AdaptiveText {Text = "Redditer"},
                            new AdaptiveText {Text = "Not Implemented Yet"}
                        }
                    }
                }
            };
            var toast = new ToastNotification(toastContent.GetXml()) { ExpirationTime = DateTimeOffset.Now.AddSeconds(4) };

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private async void FollowLink(object sender, RoutedEventArgs e)
        {
            var subredditPage = Tag as SubredditPage;
            if (subredditPage == null)
                return;

            if (subredditPage.ViewModel.SelectedThread.Selfpost)
                subredditPage.OpenSelectedThreadComments();
            else
                await Launcher.LaunchUriAsync(new Uri(subredditPage.ViewModel.SelectedThread.Url));
        }

        private async void ThumbnailTapped(object sender, TappedRoutedEventArgs e)
        {
            var subredditThread = DataContext as SubredditThread;
            if (subredditThread == null)
                return;

            await Launcher.LaunchUriAsync(new Uri(subredditThread.Url));
        }

        private async void UpvoteTapped(object sender, TappedRoutedEventArgs e)
        {
            var subredditThread = DataContext as SubredditThread;
            if (subredditThread == null)
                return;

            var subredditPage = Tag as SubredditPage;
            if (subredditPage == null)
                return;

            if (!subredditPage.ViewModel.IsLoggedIn)
            {
                var dialog = new MessageDialog("You need to be logged in to vote");
                await dialog.ShowAsync();
                return;
            }

            if (subredditThread.Upvoted)
            {
                upvoteButton.Foreground = new SolidColorBrush(Colors.White);
                scoreText.Foreground = new SolidColorBrush(Colors.White);
                scoreText.Text = Convert.ToString(Convert.ToInt32(scoreText.Text) - 1);
                subredditThread.Likes = Maybe<bool>.Nothing();
                await Reddit.Instance.Vote(subredditThread.Name, RedditClient.VoteType.Unvote);
            }
            else
            {
                upvoteButton.Foreground = new SolidColorBrush(Colors.DodgerBlue);
                scoreText.Foreground = new SolidColorBrush(Colors.DodgerBlue);
                downvoteButton.Foreground = new SolidColorBrush(Colors.White);
                scoreText.Text = Convert.ToString(Convert.ToInt32(scoreText.Text) + (subredditThread.Downvoted ? 2 : 1));
                subredditThread.Likes = Maybe<bool>.Just(true);
                await Reddit.Instance.Vote(subredditThread.Name, RedditClient.VoteType.Upvote);
            }
        }

        private async void DownvoteTapped(object sender, TappedRoutedEventArgs e)
        {
            var subredditThread = DataContext as SubredditThread;
            if (subredditThread == null)
                return;

            var subredditPage = Tag as SubredditPage;
            if (subredditPage == null)
                return;

            if (!subredditPage.ViewModel.IsLoggedIn)
            {
                var dialog = new MessageDialog("You need to be logged in to vote.");
                await dialog.ShowAsync();
                return;
            }

            if (subredditThread.Downvoted)
            {
                downvoteButton.Foreground = new SolidColorBrush(Colors.White);
                scoreText.Foreground = new SolidColorBrush(Colors.White);
                scoreText.Text = Convert.ToString(Convert.ToInt32(scoreText.Text) + 1);
                subredditThread.Likes = Maybe<bool>.Nothing();
                await Reddit.Instance.Vote(subredditThread.Name, RedditClient.VoteType.Unvote);
            }
            else
            {
                upvoteButton.Foreground = new SolidColorBrush(Colors.White);
                scoreText.Foreground = new SolidColorBrush(Colors.DodgerBlue);
                downvoteButton.Foreground = new SolidColorBrush(Colors.DodgerBlue);
                scoreText.Text = Convert.ToString(Convert.ToInt32(scoreText.Text) - (subredditThread.Upvoted ? 2 : 1));
                subredditThread.Likes = Maybe<bool>.Just(false);
                await Reddit.Instance.Vote(subredditThread.Name, RedditClient.VoteType.Downvote);
            }
        }
    }
}
