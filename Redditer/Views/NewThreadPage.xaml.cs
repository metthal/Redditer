using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Redditer.Models;
using Redditer.Providers;
using Redditer.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Redditer.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewThreadPage : Page
    {
        public NewThreadPage()
        {
            this.InitializeComponent();

            ViewModel = new NewThreadViewModel();

            DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Subreddit = e.Parameter as Subreddit;

            base.OnNavigatedTo(e);
        }

        private async void SubmitNewThread(object sender, TappedRoutedEventArgs e)
        {
            if (pivotView.SelectedIndex == 0)
                await ViewModel.SubmitNewTextThread();
            else if (pivotView.SelectedIndex == 1)
                await ViewModel.SubmitNewLinkThread();

            var rootFrame = Window.Current.Content as Frame;
            rootFrame.GoBack();
        }

        public NewThreadViewModel ViewModel { get; set; }
    }
}
