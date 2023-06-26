using System.Text;
using System.Windows.Forms;
using AltitudeAngelWings.ApiClient.Client;
using AltitudeAngelWings.Plugin.Properties;
using AltitudeAngelWings.Service;

namespace AltitudeAngelWings.Plugin
{
    public class MapInfoDockPanel : IMapInfoDockPanel
    {
        private const string MapInfoDockPanelName = "###MapInfoDockPanel###";
        private readonly Control _parent;
        private readonly IUiThreadInvoke _uiThreadInvoke;

        public MapInfoDockPanel(Control parent, IUiThreadInvoke uiThreadInvoke)
        {
            _parent = parent;
            _uiThreadInvoke = uiThreadInvoke;
        }

        public void Show(FeatureProperties[] featureProperties)
        {
            if (featureProperties.Length == 0)
            {
                Hide();
                return;
            }

            var builder = new StringBuilder();
            foreach (var featureProperty in featureProperties)
            {
                builder.Append(featureProperty.FormatAsHtml());
            }
            
            _uiThreadInvoke.FireAndForget(() =>
            {
                _parent.SuspendLayout();
                var browser = GetOrAddWebBrowser();
                browser.BringToFront();
                browser.DocumentText = Resources.MapInfoDockPanel.Replace(
                    MapInfoDockPanelName,
                    builder.ToString());
                browser.Visible = true;
                _parent.ResumeLayout();
            });
        }

        public void Hide()
        {
            _uiThreadInvoke.FireAndForget(() =>
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
                return (WebBrowser)_parent.Controls[MapInfoDockPanelName];
            }

            var browser = new WebBrowser
            {
                Name = MapInfoDockPanelName,
                Dock = DockStyle.Right,
                WebBrowserShortcutsEnabled = false,
                AllowWebBrowserDrop = false,
                IsWebBrowserContextMenuEnabled = false,
                ScriptErrorsSuppressed = true,
                Width = 350,
                Visible = false
            };
            _parent.Controls.Add(browser);
            return browser;
        }
    }
}