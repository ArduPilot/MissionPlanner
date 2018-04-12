using System;
using System.Diagnostics;
using System.Windows.Forms;
using MissionPlanner;
using MissionPlanner.Comms;
using MissionPlanner.Radio;
using MissionPlanner.Utilities;
using Microsoft.VisualBasic;

namespace SikRadio
{
    public partial class Config : Form
    {
        public Config()
        {
            InitializeComponent();

            loadSettings();

            CMB_SerialPort.Items.AddRange(SerialPort.GetPortNames());
            CMB_SerialPort.Items.Add("TCP");

            if (CMB_SerialPort.Items.Count > 0)
                CMB_SerialPort.SelectedIndex = 0;

            // default
            CMB_Baudrate.SelectedIndex = CMB_Baudrate.Items.IndexOf("57600");

            MissionPlanner.Comms.CommsBase.InputBoxShow += CommsBaseOnInputBoxShow;
        }

        /// <summary>
        /// Shows a dialog box in which to enter comms information.
        /// </summary>
        /// <param name="title">The title of the dialog box.</param>
        /// <param name="prompttext">The text to display in the dialog box.</param>
        /// <param name="text">The text to return.</param>
        /// <returns></returns>
        public static inputboxreturn CommsBaseOnInputBoxShow(string title, string prompttext, ref string text)
        {
            text = Interaction.InputBox(prompttext, title, "");

            return inputboxreturn.OK;
        }

        private void loadSettings()
        {
            Terminal.threadrun = false;

            panel1.Controls.Clear();

            var form = new Sikradio();

            panel1.Controls.Add(form);

            ThemeManager.SetTheme(ThemeManager.Themes.None);

            ThemeManager.ApplyThemeTo(this);
        }

        private void loadTerminal()
        {
            panel1.Controls.Clear();

            var form = new Terminal();

            form.Dock = DockStyle.Fill;

            panel1.Controls.Add(form);

            ThemeManager.SetTheme(ThemeManager.Themes.None);

            ThemeManager.ApplyThemeTo(this);
        }

        private void loadRssi()
        {
            Terminal.threadrun = false;

            panel1.Controls.Clear();

            var form = new Rssi();

            form.Dock = DockStyle.Fill;

            panel1.Controls.Add(form);

            ThemeManager.SetTheme(ThemeManager.Themes.None);

            ThemeManager.ApplyThemeTo(this);
        }

        private void CMB_SerialPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainV2.comPort.BaseStream.PortName = CMB_SerialPort.Text;
            MainV2.comPortName = CMB_SerialPort.Text;
        }

        private void CMB_Baudrate_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainV2.comPort.BaseStream.BaudRate = int.Parse(CMB_Baudrate.Text);
            MainV2.comPortBaud = int.Parse(CMB_Baudrate.Text);
        }

        private void CMB_SerialPort_Click(object sender, EventArgs e)
        {
            CMB_SerialPort.Items.Clear();
            CMB_SerialPort.Items.AddRange(SerialPort.GetPortNames());
            CMB_SerialPort.Items.Add("TCP");
       
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://code.google.com/p/ardupilot-mega/wiki/3DRadio");
        }

        private void projectPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/tridge/SiK");
        }

        private void terminalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadTerminal();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadSettings();
        }

        private void rssiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadRssi();
        }
    }
}