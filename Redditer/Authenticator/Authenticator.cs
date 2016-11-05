using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Redditer.ViewModels;
using Redditer.Views;
using RedditerCore.Authentication;

namespace Redditer.Authenticator
{
    public class Authenticator : IUserAuthenticator
    {
        public async Task<Tuple<string, string>> OnLogInChallenge()
        {
            var dialog = new LoginDialog();
            var result = await dialog.ShowAsync();
            return result != ContentDialogResult.Primary ? new Tuple<string, string>(null, null) : new Tuple<string, string>(dialog.Username, dialog.Password);
        }

        public async Task<bool> OnAppAuthorizeChallenge()
        {
            var dialog = new AuthorizeAppDialog();
            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }

        public SubredditViewModel ViewModel { get; set; }
    }
}
