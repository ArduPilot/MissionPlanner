﻿using System;
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
    public partial class _4FrameType : MyUserControl, IWizard
    {
        bool selected = false;

        public _4FrameType()
        {
            InitializeComponent();
        }

        public int WizardValidate()
        {
            if (selected)
                return 1;

            return 0;
        }

        public bool WizardBusy()
        {
            return false;
        }

        void setframeType(object sender)
        {
            string option = (sender as PictureBoxMouseOver).Tag.ToString();

            selected = true;

            switch (option) {
                case "x":
                    MainV2.comPort.setParam("FRAME", 1);
                    break;
                case "+":
                    MainV2.comPort.setParam("FRAME", 0);
                    break;
                case "trap":
                    MainV2.comPort.setParam("FRAME", 2);
                    break;
                case "h":
                    MainV2.comPort.setParam("FRAME", 3);
                    break;
                case "y6b":
                    MainV2.comPort.setParam("FRAME", 10);
                    break;
            }
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            DeselectAll();
            (sender as PictureBoxMouseOver).selected = true;
            setframeType(sender);
        }

        void DeselectAll()
        {
            foreach (var ctl in this.panel1.Controls)
            {
                if (ctl.GetType() == typeof(PictureBoxMouseOver))
                {
                    (ctl as PictureBoxMouseOver).selected = false;
                }
            }
        }
    }
}
