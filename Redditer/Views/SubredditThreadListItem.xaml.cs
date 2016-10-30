using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Notifications;
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
using Redditer.ViewModels;

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
            var subredditPage = Tag as SubredditPage;
            if (subredditPage == null)
                return;

            await Launcher.LaunchUriAsync(new Uri(subredditPage.ViewModel.SelectedThread.Url));
        }
    }
}
