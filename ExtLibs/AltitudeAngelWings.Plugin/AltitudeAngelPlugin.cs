using System;
using System.Windows.Forms;
using AltitudeAngelWings.Plugin.Properties;
using AltitudeAngelWings.Service;

namespace AltitudeAngelWings.Plugin
{
    public class AltitudeAngelPlugin : MissionPlanner.Plugin.Plugin
    {
        private const string SettingsMenuItemName = "altitudeAngelSettings";

        public override string Name => Resources.PluginName;

        public override string Version => GetType().Assembly.GetName().Version.ToString();

        public override string Author => Resources.PluginAuthor;

        private bool _enabled;

        static internal AltitudeAngelPlugin Instance;

        public override bool Init()
        {
            Instance = this;
            Host.MainForm.Invoke((Action)(() => 
            {
                Host.FDMenuMap.Items.Add(CreateSettingsMenuItem());
                Host.FPMenuMap.Items.Add(CreateSettingsMenuItem());
            }));
            return true;
        }

        public override bool Loaded()
        {
            ServiceLocator.Clear();
            if (Host.config.ContainsKey("AA_CheckEnableAltitudeAngel"))
            {
                _enabled = Host.config.GetBoolean("AA_CheckEnableAltitudeAngel");
            }
            else
            {
                AskToEnableAltitudeAngel();
            }
            if (_enabled)
            {
                EnableAltitudeAngel();
            }
            return true;
        }

        public override bool Exit()
        {
            ServiceLocator.Clear();
            return true;
        }

        private void AskToEnableAltitudeAngel(bool explicitClick = false)
        {
            var text = Resources.AskToEnableAltitudeAngel;
            if (explicitClick)
            {
                text = Resources.ExplicitClickPrefix + text;
            }
            _enabled = CustomMessageBox.Show(
                text,
                Resources.AskToEnableCaption,
                CustomMessageBox.MessageBoxButtons.YesNo) == CustomMessageBox.DialogResult.Yes;
            Host.config["AA_CheckEnableAltitudeAngel"] = (_enabled).ToString();
            Host.config.Save();
        }

        private void EnableAltitudeAngel()
        {
            ConfigureServiceLocator();
            var service = ServiceLocator.GetService<IAltitudeAngelService>();
            if (!service.IsSignedIn)
            {
                service.SignInAsync();
            }
        }

        private ToolStripMenuItem CreateSettingsMenuItem()
        {
            var menuItem = new ToolStripMenuItem
            {
                Name = SettingsMenuItemName,
                Text = Resources.SettingsMenuItemText,
                Enabled = true,
                Visible = true
            };
            menuItem.Click += OnSettingsClick;
            return menuItem;
        }

        private void OnSettingsClick(object sender, EventArgs e)
        {
            if (!_enabled)
            {
                AskToEnableAltitudeAngel(true);
            }
            if (!_enabled) return;
            EnableAltitudeAngel();
            new AASettings().Show(Host.MainForm);
        }

        private void ConfigureServiceLocator()
        {
            ServiceLocator.Clear();
            ServiceLocator.Register(l => Host);
            ServiceLocator.Configure();
        }
    }
}