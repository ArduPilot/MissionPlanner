using MissionPlanner.Utilities;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class RelayOptions : UserControl
    {
        // start at 0 increment each instance
        static int relay = 0;
        Color couleur = Color.LightGray;

        public int thisrelay { get; set; }

        public RelayOptions()
        {
            InitializeComponent();

            thisrelay = relay;

            TXT_rcchannel.Text = "E";//"Relay " + thisrelay.ToString();

            loadSettings();

            relay++;

            TXT_rcchannel.BackColor = Color.LightGray;

            #region tout change de couleur
            BUT_High.BackColor = couleur;
            BUT_High.BGGradBot = couleur;
            BUT_High.BGGradTop = couleur;
            BUT_Low.BackColor = couleur;
            BUT_Low.BGGradBot = couleur;
            BUT_Low.BGGradTop = couleur; 
            BackColor = couleur;
            #endregion
        }

        void loadSettings()
        {
            string desc = Settings.Instance["Relay" + thisrelay + "_desc"];

            string highdesc = Settings.Instance["Relay" + thisrelay + "_highdesc"];
            string lowdesc = Settings.Instance["Relay" + thisrelay + "_lowdesc"];

            if (!string.IsNullOrEmpty(desc))
            {
                TXT_rcchannel.Text = desc;
            }

            if (!string.IsNullOrEmpty(highdesc))
            {
                BUT_High.Text = "Haut";//highdesc;
            }

            if (!string.IsNullOrEmpty(lowdesc))
            {
                BUT_Low.Text = "Bas";//lowdesc;
            }

            #region tout change de couleur
            BUT_High.BackColor = couleur;
            BUT_High.BGGradBot = couleur;
            BUT_High.BGGradTop = couleur;
            BUT_Low.BackColor = couleur;
            BUT_Low.BGGradBot = couleur;
            BUT_Low.BGGradTop = couleur;
            BackColor = couleur;
            #endregion
        }

        private void BUT_Low_Click(object sender, EventArgs e)
        {
            try
            {
                thisrelay = 1;      //Alex
                if (MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.MAV_CMD.DO_SET_RELAY, thisrelay, 0, 0, 0, 0, 0, 0))
                {
                    couleur = Color.FromArgb(255, 120, 120); 
                    #region tout change de couleur
                    BUT_High.BackColor = couleur;
                    BUT_High.BGGradBot = couleur;
                    BUT_High.BGGradTop = couleur;
                    BUT_Low.BackColor = couleur;
                    BUT_Low.BGGradBot = couleur;
                    BUT_Low.BGGradTop = couleur;
                    BackColor = couleur;
                    #endregion
                }
                else
                {
                    CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.CommandFailed + ex.ToString(), Strings.ERROR);
            }
        }

        private void BUT_High_Click(object sender, EventArgs e)
        {
            try
            {
                thisrelay = 1;      //Alex
                if (MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.MAV_CMD.DO_SET_RELAY, thisrelay, 1, 0, 0,
                    0, 0, 0))
                {
                    couleur = Color.PaleGreen;
                    #region tout change de couleur
                    BUT_High.BackColor = couleur;
                    BUT_High.BGGradBot = couleur;
                    BUT_High.BGGradTop = couleur;
                    BUT_Low.BackColor = couleur;
                    BUT_Low.BGGradBot = couleur;
                    BUT_Low.BGGradTop = couleur;
                    BackColor = couleur;
                    #endregion
                }
                else
                {
                    CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.CommandFailed + ex.ToString(), Strings.ERROR);
            }
        }

        public void BUT_Repeat_Click(object sender, EventArgs e)
        {
            try
            {
                thisrelay = 0;
                if (MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.MAV_CMD.DO_SET_RELAY, thisrelay, 0, 0, 0,
                    0, 0, 0))
                {
                    TXT_rcchannel.BackColor = Color.Red;
                }

                Application.DoEvents();
                System.Threading.Thread.Sleep(200);

                if (MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.MAV_CMD.DO_SET_RELAY, thisrelay, 1, 0, 0,
                    0, 0, 0))
                {
                    TXT_rcchannel.BackColor = Color.Green;
                }

                Application.DoEvents();
                System.Threading.Thread.Sleep(200);

                if (MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.MAV_CMD.DO_SET_RELAY, thisrelay, 0, 0, 0,
                    0, 0, 0))
                {
                    TXT_rcchannel.BackColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.CommandFailed + ex.ToString(), Strings.ERROR);
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control sourcectl = ((ContextMenuStrip)renameToolStripMenuItem.Owner).SourceControl;

            string desc = sourcectl.Text;
            MissionPlanner.Controls.InputBox.Show("Description", "Enter new Description", ref desc);
            sourcectl.Text = desc;

            if (sourcectl == BUT_High)
            {
                Settings.Instance["Relay" + thisrelay + "_highdesc"] = desc;
            }
            else if (sourcectl == BUT_Low)
            {
                Settings.Instance["Relay" + thisrelay + "_lowdesc"] = desc;
            }
            else if (sourcectl == TXT_rcchannel)
            {
                Settings.Instance["Relay" + thisrelay + "_desc"] = desc;
            }
        }
    }
}