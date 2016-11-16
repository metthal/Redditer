using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Redditer.Providers;
using Redditer.ViewModels;
using Redditer.Views;
using RedditerCore.Authentication;

namespace Redditer.Authenticator
{
    public class Authenticator : IUserAuthenticator
    {
        public async Task<Tuple<string, string>> OnLogInChallenge()
        {
            if (Settings.Instance.Data.LastLoggedUser != null)
            {
                var rememberedUser = Accounts.Get(Settings.Instance.Data.LastLoggedUser);
                if (rememberedUser != null)
                    return new Tuple<string, string>(rememberedUser.UserName, rememberedUser.Password);
            }

            var dialog = new LoginDialog();
            var result = await dialog.ShowAsync();
            if (result != ContentDialogResult.Primary)
                return new Tuple<string, string>(null, null);

            Accounts.Save(dialog.Username, dialog.Password);
            return new Tuple<string, string>(dialog.Username, dialog.Password);
        }

        public async Task<bool> OnAppAuthorizeChallenge()
        {
            return true;
            //var dialog = new AuthorizeAppDialog();
            //var result = await dialog.ShowAsync();
            //return result == ContentDialogResult.Primary;
        }

        public SubredditViewModel ViewModel { get; set; }
    }
}
