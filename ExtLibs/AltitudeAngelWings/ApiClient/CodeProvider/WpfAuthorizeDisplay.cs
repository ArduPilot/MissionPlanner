using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using AltitudeAngelWings.ApiClient.Client;

namespace AltitudeAngelWings.ApiClient.CodeProvider
{
    /// <summary>
    ///     Provides auth code URIs from an auth URI.
    /// </summary>
    public class WpfAuthorizeDisplay : Window, IAuthorizeCodeProvider
    {
        /// <summary>
        ///     Constructor. Ensure this is called from the UI thread.
        /// </summary>
        public WpfAuthorizeDisplay()
        {
            Width = 800;
            Height = 600;

            _webBrowser = new WebBrowser();
            _webBrowser.Navigating += WebBrowserOnNavigating;
            Content = new Grid { Children = { _webBrowser } };
        }

        public Task<Uri> GetCodeUri(Uri authorizeUri, Uri redirectUri)
        {
            _redirectUri = redirectUri;

            _webBrowser.Navigate(authorizeUri);

            bool? result = ShowDialog();
            if (result == true)
            {
            }


            return _tcs.Task;
        }

        private void WebBrowserOnNavigating(object sender, NavigatingCancelEventArgs navigatingCancelEventArgs)
        {
            if (navigatingCancelEventArgs.Uri.ToString().StartsWith(_redirectUri.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                navigatingCancelEventArgs.Cancel = true;
                _tcs.SetResult(navigatingCancelEventArgs.Uri);
                Close();
            }
        }

        private readonly TaskCompletionSource<Uri> _tcs = new TaskCompletionSource<Uri>();
        private Uri _redirectUri;
        private readonly WebBrowser _webBrowser;
    }
}
