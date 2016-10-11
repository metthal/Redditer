using Windows.UI.Xaml;
using Redditer.ViewModels;
using Windows.UI.Xaml.Controls;

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
            ViewModel.LoadSubreddit("/r/all");
        }
    }
}
