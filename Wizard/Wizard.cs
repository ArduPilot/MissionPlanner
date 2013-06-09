using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArdupilotMega.Controls.Wizard
{
    public partial class Wizard : Form
    {
        MainSwitcher wiz_main = null;



        public Wizard()
        {
            InitializeComponent();

            wiz_main = new MainSwitcher(this);
        }

        private void BUT_Back_Click(object sender, EventArgs e)
        {
            wiz_main.ShowScreen("test");
        }

        private void BUT_Next_Click(object sender, EventArgs e)
        {
            wiz_main.ShowScreen("test");
        }
    }
}
