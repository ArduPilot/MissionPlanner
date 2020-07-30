using AltitudeAngelWings.ApiClient.Client;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace AltitudeAngelWings.ApiClient.CodeProvider
{
    /// <summary>
    ///     Provides auth code URIs from an auth URI.
    /// </summary>
    public class WpfAuthorizeDisplay : Form, IAuthorizeCodeProvider
    {
        private readonly TaskCompletionSource<Uri> _tcs = new TaskCompletionSource<Uri>();
        private Uri _redirectUri;
        private readonly WebBrowser _webBrowser;
        private bool seturl = false;

        /// <summary>
        ///     Constructor. Ensure this is called from the UI thread.
        /// </summary>
        public WpfAuthorizeDisplay()
        {
            Width = 800;
            Height = 600;

            try
            {
                _webBrowser = new WebBrowser();
                _webBrowser.Navigating += WebBrowserOnNavigating;
                _webBrowser.Navigated += WebBrowserOnNavigated;
                _webBrowser.Dock = DockStyle.Fill;
                Controls.Add(_webBrowser);
            }
            catch { }
        }

        private void WebBrowserOnNavigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (!e.Url.ToString()
                .StartsWith(_redirectUri.ToString(), StringComparison.OrdinalIgnoreCase)) return;
            seturl = true;
            _tcs.SetResult(e.Url);
            Close();
        }

        public Task<Uri> GetCodeUri(Uri authorizeUri, Uri redirectUri)
        {
            _redirectUri = redirectUri;

            if (_webBrowser != null)
            {
                _webBrowser.Navigate(authorizeUri);
                ShowDialog();
            }

            if(!seturl)
                _tcs.SetResult(_redirectUri);

            return _tcs.Task;
        }

        private void WebBrowserOnNavigating(object sender, WebBrowserNavigatingEventArgs navigatingCancelEventArgs)
        {
            if (!navigatingCancelEventArgs.Url.ToString()
                .StartsWith(_redirectUri.ToString(), StringComparison.OrdinalIgnoreCase)) return;
            navigatingCancelEventArgs.Cancel = true;
            seturl = true;
            _tcs.SetResult(navigatingCancelEventArgs.Url);
            Close();
        }
    }
}
