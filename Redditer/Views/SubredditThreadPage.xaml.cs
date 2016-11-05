using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Redditer.Models;
using Redditer.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Redditer.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SubredditThreadPage : Page
    {
        public SubredditThreadPage()
        {
            this.InitializeComponent();

            ViewModel = new SubredditThreadViewModel();

            DataContext = ViewModel;
        }

        public SubredditThreadViewModel ViewModel { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.ParentViewModel = e.Parameter as SubredditViewModel;
            ViewModel.Thread = ViewModel.ParentViewModel.SelectedThread;
            ViewModel.LoadComments();

            base.OnNavigatedTo(e);
        }

        private void SubredditTextBoxKeydown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                if (subredditTextBox.Text.Length == 0)
                    return;

                subredditTextBox.IsEnabled = false; // Hides the keyboard
                ViewModel.ParentViewModel.LoadSubreddit(subredditTextBox.Text, ViewModel.ParentViewModel.SortType[0]);
                Frame.GoBack();

                subredditTextBox.IsEnabled = true;
                subredditTextBox.Text = "";
                splitView.IsPaneOpen = false;
            }
            else if (VirtualKey.A <= e.Key && e.Key <= VirtualKey.Z)
            {
                ViewModel.ParentViewModel.QuerySubreddits(subredditTextBox.Text + e.Key);
            }
            else if (VirtualKey.Number0 <= e.Key && e.Key <= VirtualKey.Number9)
            {
                var c = e.Key.ToString()[6];
                ViewModel.ParentViewModel.QuerySubreddits(subredditTextBox.Text + c);
            }
            else
            {
                ViewModel.ParentViewModel.QuerySubreddits(subredditTextBox.Text);
            }
        }

        private void ChooseQueriedSubreddit(object sender, SelectionChangedEventArgs e)
        {
            // Do not execute this if deselection fires this
            if (!splitView.IsPaneOpen || e.AddedItems.Count == 0)
                return;

            ViewModel.ParentViewModel.LoadSubreddit(ViewModel.ParentViewModel.SelectedQueriedSubreddit, ViewModel.ParentViewModel.SortType[0]);
            Frame.GoBack();

            subredditTextBox.Text = "";
            splitView.IsPaneOpen = false;

            //ScrollToTop();
        }

        private void CommentsScrolling(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var s = (ScrollViewer)sender;
            if (Math.Abs(s.VerticalOffset - s.ScrollableHeight) < 0.01)
                System.Diagnostics.Debug.WriteLine("{0}/{1}", s.VerticalOffset, s.ScrollableHeight);
        }

        private void CommentClicked(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            var comment = e.AddedItems[0] as Comment;
            if (comment.LoadMoreComments)
            {
                ViewModel.LoadMoreComments(comment);
            }
        }
    }
}
