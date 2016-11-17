using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
using Redditer.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Redditer.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddCommentPage : Page
    {
        public AddCommentPage()
        {
            this.InitializeComponent();

            ViewModel = new AddCommentViewModel();

            DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var args = e.Parameter as Tuple<SubredditThreadPage, Comment>;
            ViewModel.CommentedThreadPage = args.Item1;
            ViewModel.CommentedComment = args.Item2;

            base.OnNavigatedTo(e);
        }

        private async void SubmitComment(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.SendingComment = true;
            await ViewModel.SubmitComment();
            ViewModel.SendingComment = false;

            var rootFrame = Window.Current.Content as Frame;
            rootFrame.GoBack();
        }

        public AddCommentViewModel ViewModel { get; set; }
    }
}
