using System;
using System.ComponentModel;
using System.Windows.Forms;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.WinForms
{
	public class WebViewRenderer : ViewRenderer<WebView, WebBrowser>, IWebViewDelegate
	{
		bool _ignoreSourceChanges;
		WebNavigationEvent _lastBackForwardEvent;
		WebNavigationEvent _lastEvent;

		IWebViewController WebViewController => Element;

		void IWebViewDelegate.LoadHtml(string html, string baseUrl)
		{
			try
			{
				if (string.IsNullOrEmpty(html))
				{
					var urlWebViewSource = Element.Source as HtmlWebViewSource;

					if (urlWebViewSource != null)
					{
						html = urlWebViewSource.Html;
					}
				}

				if (Control != null)
				{
					Control.DocumentText = html;
					Control.Update();
				}
			}
			catch (Exception ex)
			{
				Log.Warning("WebView load string", $"WebView load string failed: {ex}");
			}
		}

		void IWebViewDelegate.LoadUrl(string url)
		{
			try
			{
				if (string.IsNullOrEmpty(url))
				{
					var urlWebViewSource = Element.Source as UrlWebViewSource;

					if (urlWebViewSource != null)
					{
						url = urlWebViewSource.Url;
					}
				}

				if (Control != null)
				{
					Control.Navigate(url);
				}
			}
			catch (Exception ex)
			{
				Log.Warning("WebView load url", $"WebView load url failed: {ex}");
			}
		}

		protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new WebBrowser());

					WebViewController.EvalRequested += OnEvalRequested;
					WebViewController.GoBackRequested += OnGoBackRequested;
					WebViewController.GoForwardRequested += OnGoForwardRequested;
				}
			}

			Load();
		}

		protected override void OnNativeElementChanged(NativeElementChangedEventArgs<WebBrowser> e)
		{
			base.OnNativeElementChanged(e);
			if (e.OldControl != null)
			{
				e.OldControl.Navigating -= OnLoadStarted;
				e.OldControl.Navigated -= OnLoadFinished;
			}

			if (e.NewControl != null)
			{
				e.NewControl.Navigating += OnLoadStarted;
				e.NewControl.Navigated += OnLoadFinished;
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == WebView.SourceProperty.PropertyName)
				Load();
		}

		void Load()
		{
			if (_ignoreSourceChanges)
				return;

			Element?.Source?.Load(this);

			UpdateCanGoBackForward();
		}

		void UpdateCanGoBackForward()
		{
			if (Element == null)
				return;

			if (Control != null)
			{
				WebViewController.CanGoBack = Control.CanGoBack;
				WebViewController.CanGoForward = Control.CanGoForward;
			}
		}

		void OnLoadStarted(object sender, EventArgs e)
		{
			var uri = Control.Url?.ToString();

			if (!string.IsNullOrEmpty(uri))
			{
				var args = new WebNavigatingEventArgs(_lastEvent, new UrlWebViewSource { Url = uri }, uri);

				Element.SendNavigating(args);

				if (args.Cancel)
					_lastEvent = WebNavigationEvent.NewPage;
			}
		}

		void OnLoadFinished(object o, EventArgs args)
		{
			if (Control == null)
			{
				return;
			}

			_ignoreSourceChanges = true;

			((IElementController)(Element))?.SetValueFromRenderer(WebView.SourceProperty,
				new UrlWebViewSource { Url = Control.Url.ToString() });

			_ignoreSourceChanges = false;

			_lastEvent = _lastBackForwardEvent;
			WebViewController?.SendNavigated(new WebNavigatedEventArgs(
				_lastEvent,
				Element?.Source,
				Control.Url.ToString(),
				WebNavigationResult.Success));

			UpdateCanGoBackForward();
		}

		void OnEvalRequested(object sender, EvalRequested eventArg)
		{
			if (Control != null)
			{
				var script = eventArg?.Script;
				Control.DocumentText = script;
				Control.Document.InvokeScript(script);
			}
		}

		void OnGoBackRequested(object sender, EventArgs eventArgs)
		{
			if (Control == null)
			{
				return;
			}

			if (Control.CanGoBack)
			{
				_lastBackForwardEvent = WebNavigationEvent.Back;
				Control.GoBack();
			}

			UpdateCanGoBackForward();
		}

		void OnGoForwardRequested(object sender, EventArgs eventArgs)
		{
			if (Control == null)
			{
				return;
			}

			if (Control.CanGoForward)
			{
				_lastBackForwardEvent = WebNavigationEvent.Forward;
				Control.GoForward();
			}

			UpdateCanGoBackForward();
		}
	}
}