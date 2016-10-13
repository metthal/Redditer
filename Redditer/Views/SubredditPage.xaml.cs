using Windows.System;
using Windows.UI.Xaml;
using Redditer.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

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

        public SubredditViewModel ViewModel { get; set; }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadSubreddit("/r/all", ViewModel.SortType[0]);
        }

        private void VisitSubreddit(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter)
                return;

            var subreddit = subredditTextBox.Text;
            if (!subreddit.StartsWith("/r/"))
                subreddit = subreddit.Insert(0, "/r/");

            subredditTextBox.Text = "";
            splitView.IsPaneOpen = false;

            ViewModel.LoadSubreddit(subreddit, ViewModel.SortType[pivotView.SelectedIndex]);
        }

        private void ChangeSortType(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.LoadSubreddit(ViewModel.CurrentSubreddit, ViewModel.SortType[pivotView.SelectedIndex]);
        }
    }
}
