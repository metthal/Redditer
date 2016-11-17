using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Popups;
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

        private void RefreshTapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.LoadComments();
        }

        private void MenuTapped(object sender, TappedRoutedEventArgs e)
        {
            var button = sender as Button;
            menuFlyout.ShowAt(button);
        }

        private async void MenuLoginTapped(object sender, TappedRoutedEventArgs e)
        {
            var result = await ViewModel.Login();
            if (result)
            {
                ViewModel.LoadComments();
                return;
            }

            var dialog = new MessageDialog("Failed to login");
            await dialog.ShowAsync();
        }

        private void MenuLogoutTapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.Logout();
            ViewModel.LoadComments();
        }

        private void CommentClicked(object sender, ItemClickEventArgs e)
        {

            var commentList = (ListView)e.OriginalSource;
            var clickedItem = (ListViewItem)commentList.ContainerFromItem(e.ClickedItem);
            var commentItem = (CommentListItem)clickedItem.ContentTemplateRoot;

            // Extract selected item and selected thread
            ListViewItem selectedItem = null;
            CommentListItem selectedCommentItem = null;
            if (commentList.SelectedItem != null)
            {
                // Container is virtualized, so if comment is not visible it may not have any list item associated
                selectedItem = (ListViewItem)commentList.ContainerFromItem(commentList.SelectedItem);
                if (selectedItem != null)
                    selectedCommentItem = (CommentListItem)selectedItem.ContentTemplateRoot;
            }

            var comment = e.ClickedItem as Comment;
            if (comment.LoadMoreComments)
            {
                ViewModel.LoadMoreComments(comment);
                // Disable selection mode, deselect item and hide extended menu
                commentList.SelectionMode = ListViewSelectionMode.None;
                commentList.SelectedItem = null;
                if (selectedCommentItem != null)
                    selectedCommentItem.ExtendedMenu = false;
                return;
            }

            // If there is no thread selected
            if (selectedItem == null)
            {
                // Enable selection mode, select the clicked item and show extended menu
                commentList.SelectionMode = ListViewSelectionMode.Single;
                commentList.SelectedItem = e.ClickedItem;
                commentItem.ExtendedMenu = true;
            }
            // If we click on the same item as currently selected item
            else if (commentList.SelectedItem == e.ClickedItem)
            {
                // Disable selection mode, deselect item and hide extended menu
                commentList.SelectionMode = ListViewSelectionMode.None;
                commentList.SelectedItem = null;
                selectedCommentItem.ExtendedMenu = false;
            }
            // If we clicked on another thread while other thread was selected
            else
            {
                // If the previously selected thread was visible, hide extended menu
                // If the previously selected thread is not visible, this is handled in CommentListScrolling event
                // Show extended menu on the newly selected item
                if (selectedCommentItem != null)
                    selectedCommentItem.ExtendedMenu = false;
                commentItem.ExtendedMenu = true;
            }
        }

        private void CommentListUpdating(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            // This piece of code makes sure that if we select some comment, then scroll away so we no longer see selected comment and select another one,
            // we no longer see the old comment as selected when we scroll back to original posisition.
            var commentItem = (CommentListItem)args.ItemContainer.ContentTemplateRoot;
            if (commentItem == null)
                return;

            // If there is some other comment selected and it is not this comment and extended menu is shown, then hide the extended menu
            if (ViewModel.SelectedComment != null && ViewModel.SelectedComment != (Comment)args.Item && commentItem.ExtendedMenu)
                commentItem.ExtendedMenu = false;
        }

        private async void AddComment(object sender, TappedRoutedEventArgs e)
        {
            if (ViewModel.IsNotLoggedIn)
            {
                var dialog = new MessageDialog("You need to be logged in to post comments.");
                await dialog.ShowAsync();
                return;
            }

            NavigateToAddCommentPage();
        }

        public void NavigateToAddCommentPage(Comment comment = null)
        {
            var args = new Tuple<SubredditThreadPage, Comment>(this, comment);
            Frame.Navigate(typeof(AddCommentPage), args);
        }
    }
}
