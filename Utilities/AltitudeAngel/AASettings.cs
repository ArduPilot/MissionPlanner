using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MissionPlanner.Utilities.AltitudeAngel
{
    public partial class AASettings : Form
    {
        public AASettings()
        {
            InitializeComponent();

            ThemeManager.ApplyThemeTo(this);

            // load settings
            chk_grounddata.Checked = AltitudeAngel.service.GroundDataDisplay;
            chk_airdata.Checked = AltitudeAngel.service.AirDataDisplay;
            chk_FlightReportEnable.Checked = AltitudeAngel.service.FlightReportEnable;
            txt_FlightReportName.Text = AltitudeAngel.service.FlightReportName;
            chk_FlightReportCommercial.Checked = AltitudeAngel.service.FlightReportCommercial;
            txt_FlightReportDuration.Text =
                ((int) AltitudeAngel.service.FlightReportTimeSpan.TotalMinutes).ToString();

            but_enable.Enabled = !AltitudeAngel.service.IsSignedIn;
            but_disable.Enabled = AltitudeAngel.service.IsSignedIn;

            foreach (var item in AltitudeAngelWings.ApiClient.Client.Extensions.FiltersSeen)
            {
                if (AltitudeAngel.service.FilteredOut.Contains(item))
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
                if (AltitudeAngel.service.IsSignedIn)
                {
                    CustomMessageBox.Show("You are already signed in", "Altitude Angel");
                    return;
                }

                AltitudeAngel.service.SignInAsync();
            }
            catch (TypeInitializationException)
            {
                CustomMessageBox.Show("Please update your dotnet version, you cannot use the feature without this.", Strings.ERROR);
            }
        }

        private void but_disable_Click(object sender, EventArgs e)
        {
            AltitudeAngel.service.DisconnectAsync();

            AltitudeAngel.service.ProcessAllFromCache(AltitudeAngel.MP.FlightDataMap);
        }

        private void chk_airdata_CheckedChanged(object sender, EventArgs e)
        {
            AltitudeAngel.service.AirDataDisplay = chk_airdata.Checked;

            AltitudeAngel.service.ProcessAllFromCache(AltitudeAngel.MP.FlightDataMap);
        }

        private void chk_grounddata_CheckedChanged(object sender, EventArgs e)
        {
            AltitudeAngel.service.GroundDataDisplay = chk_grounddata.Checked;

            AltitudeAngel.service.ProcessAllFromCache(AltitudeAngel.MP.FlightDataMap);
        }

        private void chklb_layers_SelectedIndexChanged(object sender, EventArgs e)
        {
            AltitudeAngel.service.FilteredOut.Clear();

            foreach (string item in chklb_layers.Items)
            {
                if (!chklb_layers.CheckedItems.Contains(item))
                {
                    AltitudeAngel.service.FilteredOut.Add(item);
                }
            }

            // reset state on change
            AltitudeAngel.service.ProcessAllFromCache(AltitudeAngel.MP.FlightDataMap);
        }

        private void txt_FlightReportName_TextChanged(object sender, EventArgs e)
        {
            AltitudeAngel.service.FlightReportName = txt_FlightReportName.Text;
        }

        private void txt_FlightReportDuration_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txt_FlightReportDuration.Text, out var minutes))
            {
                AltitudeAngel.service.FlightReportTimeSpan = TimeSpan.FromMinutes(minutes);
            }
        }

        private void chk_FlightReportCommercial_CheckedChanged(object sender, EventArgs e)
        {
            AltitudeAngel.service.FlightReportCommercial = chk_FlightReportCommercial.Checked;
        }

        private void chk_FlightReportEnable_CheckedChanged(object sender, EventArgs e)
        {
            AltitudeAngel.service.FlightReportEnable = chk_FlightReportEnable.Checked;
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
            Process.Start("https://www.altitudeangel.com/");
        }
    }
}
