using System;
using System.Diagnostics;
using System.Windows.Forms;
using AltitudeAngelWings;
using AltitudeAngelWings.Extra;
using AltitudeAngelWings.Service;

namespace MissionPlanner.Utilities.AltitudeAngel
{
    internal partial class AASettings : Form
    {
        private readonly ISettings _settings;
        private readonly IAltitudeAngelService _altitudeAngelService;
        private readonly IMissionPlanner _missionPlanner;

        public AASettings()
            : this(ServiceLocator.GetService<ISettings>(), ServiceLocator.GetService<IAltitudeAngelService>(), ServiceLocator.GetService<IMissionPlanner>())
        {
        }

        public AASettings(ISettings settings, IAltitudeAngelService altitudeAngelService, IMissionPlanner missionPlanner)
        {
            _settings = settings;
            _altitudeAngelService = altitudeAngelService;
            _missionPlanner = missionPlanner;
            InitializeComponent();

            ThemeManager.ApplyThemeTo(this);

            // load settings
            chk_grounddata.Checked = _settings.GroundDataDisplay;
            chk_airdata.Checked = _settings.AirDataDisplay;
            chk_FlightReportEnable.Checked = _settings.FlightReportEnable;
            txt_FlightReportName.Text = _settings.FlightReportName;
            chk_FlightReportCommercial.Checked = _settings.FlightReportCommercial;
            txt_FlightReportDuration.Text =
                ((int) _settings.FlightReportTimeSpan.TotalMinutes).ToString();

            but_enable.Enabled = !_altitudeAngelService.IsSignedIn;
            but_disable.Enabled = _altitudeAngelService.IsSignedIn;

            foreach (var item in AltitudeAngelWings.ApiClient.Client.Extensions.FiltersSeen)
            {
                if (_altitudeAngelService.FilteredOut.Contains(item))
                    chklb_layers.Items.Add(item, false);
                else
                    chklb_layers.Items.Add(item, true);
            }

            RefreshControlStates();
        }

        private void but_enable_Click(object sender, EventArgs e)
        {
            try
            {
                if (_altitudeAngelService.IsSignedIn)
                {
                    CustomMessageBox.Show("You are already signed in", "Altitude Angel");
                    return;
                }

                _altitudeAngelService.SignInAsync();
            }
            catch (TypeInitializationException)
            {
                CustomMessageBox.Show("Please update your dotnet version, you cannot use the feature without this.", Strings.ERROR);
            }
        }

        private void but_disable_Click(object sender, EventArgs e)
        {
            _altitudeAngelService.DisconnectAsync();

            _altitudeAngelService.ProcessAllFromCache(_missionPlanner.FlightDataMap);
        }

        private void chk_airdata_CheckedChanged(object sender, EventArgs e)
        {
            _settings.AirDataDisplay = chk_airdata.Checked;

            _altitudeAngelService.ProcessAllFromCache(_missionPlanner.FlightDataMap);
        }

        private void chk_grounddata_CheckedChanged(object sender, EventArgs e)
        {
            _settings.GroundDataDisplay = chk_grounddata.Checked;

            _altitudeAngelService.ProcessAllFromCache(_missionPlanner.FlightDataMap);
        }

        private void chklb_layers_SelectedIndexChanged(object sender, EventArgs e)
        {
            _altitudeAngelService.FilteredOut.Clear();

            foreach (string item in chklb_layers.Items)
            {
                if (!chklb_layers.CheckedItems.Contains(item))
                {
                    _altitudeAngelService.FilteredOut.Add(item);
                }
            }

            // reset state on change
            _altitudeAngelService.ProcessAllFromCache(_missionPlanner.FlightDataMap);
        }

        private void txt_FlightReportName_TextChanged(object sender, EventArgs e)
        {
            _settings.FlightReportName = txt_FlightReportName.Text;
        }

        private void txt_FlightReportDuration_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txt_FlightReportDuration.Text, out var minutes))
            {
                _settings.FlightReportTimeSpan = TimeSpan.FromMinutes(minutes);
            }
        }

        private void chk_FlightReportCommercial_CheckedChanged(object sender, EventArgs e)
        {
            _settings.FlightReportCommercial = chk_FlightReportCommercial.Checked;
        }

        private void chk_FlightReportEnable_CheckedChanged(object sender, EventArgs e)
        {
            _settings.FlightReportEnable = chk_FlightReportEnable.Checked;
            RefreshControlStates();
        }

        private void RefreshControlStates()
        {
            lbl_FlightReportName.Enabled = chk_FlightReportEnable.Checked;
            txt_FlightReportName.Enabled = chk_FlightReportEnable.Checked;
            lbl_FlightReportDuration.Enabled = chk_FlightReportEnable.Checked;
            txt_FlightReportDuration.Enabled = chk_FlightReportEnable.Checked;
            chk_FlightReportCommercial.Enabled = chk_FlightReportEnable.Checked;
        }

        private void lbl_FlightReportWhat_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://bit.ly/aamissionplanner1");
        }
    }
}
