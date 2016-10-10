using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Redditer.Views
{
    public sealed partial class LoginDialog : ContentDialog
    {
        public LoginDialog()
        {
            this.InitializeComponent();
        }

        private void LoginButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Username = BoxUsername.Text;
            Password = BoxPassword.Password;
        }

        private void CancelButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Username = "";
            Password = "";
        }

        public string Username { get; private set; }
        public string Password { get; private set; }
    }
}
