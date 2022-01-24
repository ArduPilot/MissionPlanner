using System.Text;
using System.Windows.Forms;
using AltitudeAngelWings;
using AltitudeAngelWings.ApiClient.Client;
using AltitudeAngelWings.Service;

namespace MissionPlanner.Utilities.AltitudeAngel
{
    public class MapInfoDockPanel : IMapInfoDockPanel
    {
        private readonly Control _parent;
        private readonly IUiThreadInvoke _uiThreadInvoke;
        private WebBrowser _browser;

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
                if (_browser == null)
                {
                    _browser = new WebBrowser
                    {
                        Dock = DockStyle.Right,
                        WebBrowserShortcutsEnabled = false,
                        AllowWebBrowserDrop = false,
                        IsWebBrowserContextMenuEnabled = false,
                        ScriptErrorsSuppressed = true,
                        Width = 350,
                        Visible = false
                    };
                    _parent.Controls.Add(_browser);
                }
                _browser.BringToFront();
                _browser.DocumentText = AltitudeAngelPlugin.Resources.MapInfoDockPanel.Replace(
                    AltitudeAngelPlugin.Resources.MapInfoDockPanelReplace,
                    builder.ToString());
                _browser.Visible = true;
            });
        }

        public void Hide()
        {
            _uiThreadInvoke.FireAndForget(() =>
            {
                if (_browser != null)
                {
                    _browser.Visible = false;
                }
            });
        }
    }
}