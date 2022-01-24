using AltitudeAngelWings.ApiClient.Client;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AltitudeAngelWings;

namespace MissionPlanner.Utilities.AltitudeAngel
{
    /// <summary>
    ///     Provides auth code URIs from an auth URI.
    /// </summary>
    public class WpfAuthorizeDisplay : IAuthorizeCodeProvider
    {
        private static Task<Uri> _existingTask;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);
        private readonly IUiThreadInvoke _invoke;
        private readonly int _width;
        private readonly int _height;
        private Uri _result;
        private Action _close = () => { };

        public WpfAuthorizeDisplay(IUiThreadInvoke invoke, int width = 800, int height = 600)
        {
            _invoke = invoke;
            _width = width;
            _height = height;
        }

        public async Task<Uri> GetCodeUri(Uri authorizeUri, Uri redirectUri)
        {
            try
            {
                await _lock.WaitAsync();
                if (_existingTask == null)
                {
                    _existingTask = _invoke.Invoke(() =>
                    {
                        using (var form = new Form())
                        {
                            _result = redirectUri;
                            // ReSharper disable once AccessToDisposedClosure
                            _close = () => form.Close();
                            form.Width = _width;
                            form.Height = _height;
                            var webBrowser = new WebBrowser();
                            webBrowser.Navigating += WebBrowserOnNavigating;
                            webBrowser.Navigated += WebBrowserOnNavigated;
                            webBrowser.Dock = DockStyle.Fill;
                            form.Controls.Add(webBrowser);
                            webBrowser.Navigate(authorizeUri);
                            form.ShowDialog();
                            _existingTask = null;
                            return _result;
                        }
                    });
                }
            }
            finally
            {
                _lock.Release();
            }
            return await _existingTask;
        }

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
