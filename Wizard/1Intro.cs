using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls;

namespace MissionPlanner.Wizard
{
    public partial class _1Intro : MyUserControl, IWizard
    {
        public _1Intro()
        {
            InitializeComponent();
        }

        public int WizardValidate()
        {
            //check if we have a valid selection 
            if (Wizard.config.ContainsKey("fwtype"))
            {
                if (Wizard.config["fwtype"].ToString() == "copter")
                    // check if its a quad, and show the frame type screen
                    return 1;
                else
                // skip the frame type screen as its not valid for anythine else
                    return 2;
            }

            return 0;
        }

        public bool WizardBusy()
        {
            return false;
        }

        void setfwtype(object sender)
        {
            // only remembers the last selected item
            Wizard.config["fwtype"] = ((Control) sender).Tag.ToString();
        }

        private void pictureBoxplane_Click(object sender, EventArgs e)
        {
            DeselectAll();
            (sender as PictureBoxMouseOver).selected = true;
            setfwtype(sender);
        }

        private void pictureBoxrover_Click(object sender, EventArgs e)
        {
            DeselectAll();
            (sender as PictureBoxMouseOver).selected = true;
            setfwtype(sender);
        }

        private void pictureBoxquad_Click(object sender, EventArgs e)
        {
            DeselectAll();
            (sender as PictureBoxMouseOver).selected = true;
            setfwtype(sender);
        }

        private void pictureBoxheli_Click(object sender, EventArgs e)
        {
            DeselectAll();
            (sender as PictureBoxMouseOver).selected = true;
            setfwtype(sender);
        }

        void DeselectAll()
        {
            pictureBoxplane.selected = false;
            pictureBoxheli.selected = false;
            pictureBoxquad.selected = false;
            pictureBoxrover.selected = false;
        }
    }
}