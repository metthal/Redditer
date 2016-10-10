using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Redditer.Views
{
    public sealed partial class AuthorizeAppDialog : ContentDialog
    {
        public AuthorizeAppDialog()
        {
            this.InitializeComponent();
        }

        private void AllowButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Allow = true;
        }

        private void DenyButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Allow = false;
        }

        public bool Allow { get; private set; }
    }
}
