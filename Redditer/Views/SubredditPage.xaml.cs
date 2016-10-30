using Windows.System;
using Windows.UI.Xaml;
using Redditer.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
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
            ViewModel.LoadSubreddit(ViewModel.CurrentSubreddit.Name, ViewModel.SortType[pivotView.SelectedIndex]);
        }

        private void VisitSubreddit(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter)
                return;

            ViewModel.LoadSubreddit(subredditTextBox.Text, ViewModel.SortType[pivotView.SelectedIndex]);

            subredditTextBox.Text = "";
            splitView.IsPaneOpen = false;
        }

        private void ChangeSortType(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.LoadSubreddit(ViewModel.CurrentSubreddit.Name, ViewModel.SortType[pivotView.SelectedIndex]);
        }

        private void ThreadClicked(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            Frame.Navigate(typeof(SubredditThreadPage), ViewModel.SelectedThread);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.SelectedThread = null;
            base.OnNavigatedTo(e);
        }

        public SubredditViewModel ViewModel { get; set; }
    }
}
