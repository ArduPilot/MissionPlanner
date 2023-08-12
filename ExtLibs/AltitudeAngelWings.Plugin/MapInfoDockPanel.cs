using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AltitudeAngelWings.Clients.Api;
using AltitudeAngelWings.Clients.Api.Model;
using AltitudeAngelWings.Plugin.Properties;
using WebBrowser = System.Windows.Forms.WebBrowser;

namespace AltitudeAngelWings.Plugin
{
    public class MapInfoDockPanel : IMapInfoDockPanel
    {
        private const string MapInfoDockPanelName = "###MapInfoDockPanel###";
        private readonly Control _parent;
        private readonly IUiThreadInvoke _uiThreadInvoke;
        private readonly Lazy<IApiClient> _apiClient;

        public MapInfoDockPanel(Control parent, IUiThreadInvoke uiThreadInvoke, Lazy<IApiClient> apiClient)
        {
            _parent = parent;
            _uiThreadInvoke = uiThreadInvoke;
            _apiClient = apiClient;
        }

        public void Show(FeatureProperties[] featureProperties)
        {
            if (featureProperties.Length == 0)
            {
                Hide();
                return;
            }

            _uiThreadInvoke.Invoke(() =>
            {
                _parent.SuspendLayout();

                var browser = GetOrAddWebBrowser();
                browser.BringToFront();
                var rateCards = featureProperties
                    .Where(f => f.UtmStatus?.RateTypes != null && f.UtmStatus.RateTypes.Keys.Count > 0)
                    .SelectMany(f => f.UtmStatus.RateTypes.Values
                        .SelectMany(c => c.Select(r => r.Id))
                        .Distinct())
                    .ToList();
                IDictionary<string, RateCardDetail> rateCardLookup = new Dictionary<string, RateCardDetail>();
                if (rateCards.Count > 0)
                {
                    browser.DocumentText = string.Empty;
                    browser.Visible = true;
                    rateCardLookup = UiTask.ShowWaitPanel(browser, async token =>
                        {
                            var rateCardDetails = await Task.WhenAll(rateCards.Select(i => _apiClient.Value.GetRateCard(i, token)));
                            return rateCardDetails.ToDictionary(d => d.Id);
                        },
                        "Getting rate cards");
                }

                var builder = new StringBuilder();
                builder.Append("<div class=\"panel\">");
                foreach (var featureProperty in featureProperties)
                {
                    builder.Append(featureProperty.FormatAsHtml(rateCardLookup));
                }
                builder.Append("</div>");

                browser.DocumentText = Resources.MapInfoDockPanel.Replace(
                    MapInfoDockPanelName,
                    builder.ToString());
                browser.Visible = true;
                _parent.ResumeLayout();
            });
        }

        public void Hide()
        {
            _uiThreadInvoke.Invoke(() =>
            {
                _parent.SuspendLayout();
                GetOrAddWebBrowser().Visible = false;
                _parent.ResumeLayout();
            });
        }

        private WebBrowser GetOrAddWebBrowser()
        {
            if (_parent.Controls.ContainsKey(MapInfoDockPanelName))
            {
                var existing = (WebBrowser)_parent.Controls[MapInfoDockPanelName];
                SetPanelLocation(existing);
                return existing;
            }

            var browser = new WebBrowser
            {
                Name = MapInfoDockPanelName,
                WebBrowserShortcutsEnabled = false,
                AllowWebBrowserDrop = false,
                IsWebBrowserContextMenuEnabled = false,
                ScriptErrorsSuppressed = true,
                AllowNavigation = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Visible = false
            };
            SetPanelLocation(browser);
            browser.NewWindow += (sender, args) => args.Cancel = true;
            ((SHDocVw.WebBrowser)browser.ActiveXInstance).NewWindow3 +=
                (ref object o, ref bool b, uint u, string s, string url) =>
                {
                    Process.Start(url);
                };
            _parent.Controls.Add(browser);
            _parent.Resize += (sender, args) => SetPanelLocation(browser);
            return browser;
        }

        private void SetPanelLocation(Control browser)
        {
            const int width = 350;
            const int topOffset = 30;
            const int bottomOffset = 10;
            var trackBar = _parent.Controls.OfType<TrackBar>().FirstOrDefault();
            var zoomOffset = trackBar?.Width + bottomOffset ?? 60;
            browser.Width = width;
            browser.Height = _parent.Height - topOffset - bottomOffset;
            browser.Location = new Point(_parent.Width - width - zoomOffset, topOffset);
        }
    }
}