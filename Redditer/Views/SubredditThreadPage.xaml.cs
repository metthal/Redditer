﻿using System;
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
            ViewModel.Thread = e.Parameter as SubredditThread;
            ViewModel.LoadComments();

            base.OnNavigatedTo(e);
        }

        private void CommentsScrolling(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var s = (ScrollViewer)sender;
            if (Math.Abs(s.VerticalOffset - s.ScrollableHeight) < 0.01)
                System.Diagnostics.Debug.WriteLine("{0}/{1}", s.VerticalOffset, s.ScrollableHeight);
        }
    }
}
