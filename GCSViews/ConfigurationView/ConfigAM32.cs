using MissionPlanner.Controls;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigAM32 : MyUserControl, IActivate
    {
        private WebView2 webView;
        private bool _hasNavigated = false;

        public ConfigAM32()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.webView = new WebView2();
            this.SuspendLayout();
            //
            // webView
            //
            this.webView.Dock = DockStyle.Fill;
            this.webView.Location = new System.Drawing.Point(0, 0);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(800, 600);
            this.webView.TabIndex = 0;
            //
            // ConfigAM32
            //
            this.Controls.Add(this.webView);
            this.Name = "ConfigAM32";
            this.Size = new System.Drawing.Size(800, 600);
            this.ResumeLayout(false);
        }

        public async void Activate()
        {
            if (!_hasNavigated)
            {
                _hasNavigated = true;
                try
                {
                    await webView.EnsureCoreWebView2Async(null);
                    webView.CoreWebView2.Navigate("https://am32.ca/configurator");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("WebView2 error: " + ex.Message);
                }
            }
        }
    }
}
