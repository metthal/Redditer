using System;
using Windows.System;
using Windows.UI.Xaml;
using Redditer.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Redditer.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Redditer.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SubredditPage : Page
    {
        public SubredditPage()
        {
            this.InitializeComponent();

            ViewModel = new SubredditViewModel();

            DataContext = ViewModel;
        }

        public void OpenSelectedThreadComments()
        {
            Frame.Navigate(typeof(SubredditThreadPage), ViewModel.SelectedThread);
        }

        private void SubredditTextBoxKeydown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                ViewModel.LoadSubreddit(subredditTextBox.Text, ViewModel.SortType[pivotView.SelectedIndex]);

                subredditTextBox.Text = "";
                splitView.IsPaneOpen = false;

                ScrollToTop();
            }
            else if (VirtualKey.A <= e.Key && e.Key <= VirtualKey.Z)
            {
                System.Diagnostics.Debug.WriteLine(subredditTextBox.Text + e.Key);
                ViewModel.QuerySubreddits(subredditTextBox.Text + e.Key);
            }
            else if (VirtualKey.Number0 <= e.Key && e.Key <= VirtualKey.Number9)
            {
                var c = e.Key.ToString()[6];
                System.Diagnostics.Debug.WriteLine(subredditTextBox.Text + c);
                ViewModel.QuerySubreddits(subredditTextBox.Text + c);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(subredditTextBox.Text);
                ViewModel.QuerySubreddits(subredditTextBox.Text);
            }
        }

        private void ChangeSortType(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.LoadSubreddit(ViewModel.CurrentSubreddit.Name, ViewModel.SortType[pivotView.SelectedIndex]);

            ScrollToTop();
        }

        private void ThreadClicked(object sender, ItemClickEventArgs e)
        {
            var threadList = (ListView)e.OriginalSource;
            var clickedItem = (ListViewItem)threadList.ContainerFromItem(e.ClickedItem);
            var clickedSubredditItem = (SubredditThreadListItem)clickedItem.ContentTemplateRoot;

            // Extract selected item and selected thread
            ListViewItem selectedItem = null;
            SubredditThreadListItem selectedSubredditItem = null;
            if (threadList.SelectedItem != null)
            {
                // Container is virtualized, so if thread is not visible it may not have any list item associated
                selectedItem = (ListViewItem)threadList.ContainerFromItem(threadList.SelectedItem);
                if (selectedItem != null)
                    selectedSubredditItem = (SubredditThreadListItem)selectedItem.ContentTemplateRoot;
            }

            // If there is no thread selected
            if (selectedItem == null)
            {
                // Enable selection mode, select the clicked item and show extended menu
                threadList.SelectionMode = ListViewSelectionMode.Single;
                threadList.SelectedItem = e.ClickedItem;
                clickedSubredditItem.ExtendedMenu = true;
            }
            // If we click on the same item as currently selected item
            else if (threadList.SelectedItem == e.ClickedItem)
            {
                // Disable selection mode, deselect item and hide extended menu
                threadList.SelectionMode = ListViewSelectionMode.None;
                threadList.SelectedItem = null;
                selectedSubredditItem.ExtendedMenu = false;
            }
            // If we clicked on another thread while other thread was selected
            else
            {
                // If the previously selected thread was visible, hide extended menu
                // If the previously selected thread is not visible, this is handled in ThreadListScrolling event
                // Show extended menu on the newly selected item
                if (selectedSubredditItem != null)
                    selectedSubredditItem.ExtendedMenu = false;
                clickedSubredditItem.ExtendedMenu = true;
            }
        }
        private void ThreadListUpdating(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            // This piece of code makes sure that if we select some thread, then scroll away so we no longer see selected thread and select another one,
            // we no longer see the old thread as selected when we scroll back to original posisition.
            var subredditThreadItem = (SubredditThreadListItem)args.ItemContainer.ContentTemplateRoot;
            if (subredditThreadItem == null)
                return;

            // If there is some other thread selected and it is not this thread and extended menu is shown, then hide the extended menu
            if (ViewModel.SelectedThread != null && ViewModel.SelectedThread != (SubredditThread)args.Item && subredditThreadItem.ExtendedMenu)
                subredditThreadItem.ExtendedMenu = false;
        }

        private async void ThreadListScrolling(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var s = (ScrollViewer)sender;
            if (Math.Abs(s.VerticalOffset - s.ScrollableHeight) < 0.01)
                await ViewModel.NextThreads(ViewModel.SortType[pivotView.SelectedIndex]);
        }

        private void ChooseQueriedSubreddit(object sender, SelectionChangedEventArgs e)
        {
            // Do not execute this if deselection fires this
            if (e.AddedItems.Count == 0)
                return;

            ViewModel.LoadSubreddit(ViewModel.SelectedQueriedSubreddit, ViewModel.SortType[pivotView.SelectedIndex]);

            subredditTextBox.Text = "";
            splitView.IsPaneOpen = false;

            ScrollToTop();
        }

        private void ScrollToTop()
        {
            if (pivotView?.Items == null || pivotView.Items.Count == 0)
                return;

            if (pivotView.Items[pivotView.SelectedIndex] == null)
                return;

            var pivotItem = (PivotItem) pivotView.ContainerFromItem(pivotView.Items[pivotView.SelectedIndex]);
            if (pivotItem == null)
                return;

            var scrollViewer = (ScrollViewer) pivotItem.ContentTemplateRoot;
            scrollViewer?.ChangeView(0, 0, 1);
        }

        public SubredditViewModel ViewModel { get; set; }
    }
}
