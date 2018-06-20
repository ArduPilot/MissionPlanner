using System;
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
            txt_FlightPlanName.Text = AltitudeAngel.service.FlightPlanName;
            chk_FlightPlanCommercial.Checked = AltitudeAngel.service.FlightPlanCommercial;
            txt_FlightPlanDuration.Text =
                ((int) AltitudeAngel.service.FlightPlanTimeSpan.TotalMinutes).ToString();

            but_enable.Enabled = !AltitudeAngel.service.IsSignedIn;
            but_disable.Enabled = AltitudeAngel.service.IsSignedIn;

            foreach (var item in AltitudeAngelWings.ApiClient.Client.Extensions.FiltersSeen)
            {
                if (AltitudeAngel.service.FilteredOut.Contains(item))
                    chklb_layers.Items.Add(item, false);
                else
                    chklb_layers.Items.Add(item, true);
            }
        }

        private void but_enable_Click(object sender, EventArgs e)
        {
            try
            {
                if (AltitudeAngel.service.IsSignedIn)
                {
                    CustomMessageBox.Show("You are already signed in", "AltitudeAngel");
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

        private void txt_FlightPlanName_TextChanged(object sender, EventArgs e)
        {
            AltitudeAngel.service.FlightPlanName = txt_FlightPlanName.Text;
        }

        private void txt_FlightPlanDuration_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txt_FlightPlanDuration.Text, out var minutes))
            {
                AltitudeAngel.service.FlightPlanTimeSpan = TimeSpan.FromMinutes(minutes);
            }
        }

        private void chk_FlightPlanCommercial_CheckedChanged(object sender, EventArgs e)
        {
            AltitudeAngel.service.FlightPlanCommercial = chk_FlightPlanCommercial.Checked;
        }
    }
}
