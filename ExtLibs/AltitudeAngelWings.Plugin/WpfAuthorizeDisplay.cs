using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using AltitudeAngelWings.ApiClient.Client;

namespace AltitudeAngelWings.Plugin
{
    /// <summary>
    ///     Provides auth code URIs from an auth URI.
    /// </summary>
    public class WpfAuthorizeDisplay : IAuthorizeCodeProvider
    {
        private readonly IUiThreadInvoke _invoke;
        private readonly IWin32Window _owner;
        private readonly int _width;
        private readonly int _height;
        private Uri _result;
        private Action _close = () => { };

        public WpfAuthorizeDisplay(IUiThreadInvoke invoke, IWin32Window owner = null, int width = 800, int height = 600)
        {
            _invoke = invoke;
            _owner = owner;
            _width = width;
            _height = height;
        }

        public Task<Uri> GetCodeUri(Uri authorizeUri, Uri redirectUri)
            => _invoke.Invoke<Uri>(() =>
            {
                using (var form = new Form())
                {
                    _result = redirectUri;
                    // ReSharper disable once AccessToDisposedClosure
                    _close = () => form.Close();
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.Width = _width;
                    form.Height = _height;
                    var webBrowser = new WebBrowser();
                    webBrowser.Navigating += WebBrowserOnNavigating;
                    webBrowser.Navigated += WebBrowserOnNavigated;
                    webBrowser.Dock = DockStyle.Fill;
                    form.Controls.Add(webBrowser);
                    webBrowser.Navigate(authorizeUri);
                    form.ShowDialog(_owner);
                    return _result;
                }
            });

        private void WebBrowserOnNavigating(object sender, WebBrowserNavigatingEventArgs navigatingEventArgs)
        {
            if (!IsRedirectUrl(navigatingEventArgs.Url)) return;
            navigatingEventArgs.Cancel = true;
            _result = navigatingEventArgs.Url;
            _close();
        }

        private void WebBrowserOnNavigated(object sender, WebBrowserNavigatedEventArgs navigatedEventArgs)
        {
            if (!IsRedirectUrl(navigatedEventArgs.Url)) return;
            _result = navigatedEventArgs.Url;
            _close();
        }

        private bool IsRedirectUrl(Uri uri)
            => uri.ToString().StartsWith(_result.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}
