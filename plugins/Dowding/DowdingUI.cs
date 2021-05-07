using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace Dowding
{
    public partial class DowdingUI : MyUserControl, IActivate
    {
        public DowdingUI()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            txt_username.Text = Settings.Instance["Dowding_username"];
            txt_password.Text = Settings.Instance["Dowding_password"];
            cmb_server.Text = Settings.Instance["Dowding_server"];
            chk_enable.Checked = Settings.Instance.GetBoolean("Dowding_enabled", false);
        }

        private async void but_verify_Click(object sender, EventArgs e)
        {
            try
            {
                await new MissionPlanner.WebAPIs.Dowding().Auth(txt_username.Text, txt_password.Text, cmb_server.Text);

                Settings.Instance["Dowding_username"] = txt_username.Text;
                Settings.Instance["Dowding_password"] = txt_password.Text;
                Settings.Instance["Dowding_server"] = cmb_server.Text;
                Settings.Instance.Save();

                CustomMessageBox.Show("Verified!");
            }
            catch
            {
                CustomMessageBox.Show("Username or password invalid");
            }
        }

        private void chk_enable_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance["Dowding_enabled"] = chk_enable.Checked.ToString();
        }

        private void but_token_Click(object sender, EventArgs e)
        {
            var token = "";
            if (InputBox.Show("Token", "Enter your token", ref token) == DialogResult.OK)
            {
                Settings.Instance["Dowding_token"] = token;
            }
        }

        private void but_start_Click(object sender, EventArgs e)
        {
            DowdingPlugin.Start();
        }
    }
}
