using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using SkiaSharp;

namespace SvgNet.SvgGdi
{
}

namespace System.Windows.Forms
{
    public class WebBrowser : Control
    {
        public bool CanGoBack { get; internal set; }

        public object Url { get; set; }

        public bool CanGoForward { get; set; }

        public string DocumentText { get; set; }

        public HtmlDocument Document { get; set; }

        public event EventHandler<WebBrowserNavigatingEventArgs> Navigating;
        public event EventHandler<WebBrowserNavigatedEventArgs> Navigated;

        public void Navigate(Uri authorizeUri)
        {
        }

        public void GoBack()
        {
        }

        public void GoForward()
        {
        }

        public void Navigate(string authorizeUri)
        {
        }
    }

    public class HtmlDocument
    {
        public void InvokeScript(string script)
        {
        }
    }

    public class WebBrowserNavigatedEventArgs : EventArgs
    {
        public Uri Url;
    }

    public class WebBrowserNavigatingEventArgs : EventArgs
    {
        public bool Cancel;
        public Uri Url;
    }
}
