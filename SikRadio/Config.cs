﻿using System;
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
        bool _Connected = false;
        ISikRadioForm _CurrentForm;
        static ICommsSerial _comPort;

        public Config()
        {
            InitializeComponent();


            CMB_SerialPort.Items.AddRange(SerialPort.GetPortNames());
            CMB_SerialPort.Items.Add("TCP");

            if (CMB_SerialPort.Items.Count > 0)
                CMB_SerialPort.SelectedIndex = 0;

            // default
            CMB_Baudrate.SelectedIndex = CMB_Baudrate.Items.IndexOf("57600");

            MissionPlanner.Comms.CommsBase.InputBoxShow += CommsBaseOnInputBoxShow;

            settingsToolStripMenuItem_Click(null, null);
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

        public static ICommsSerial comPort
        {
            get
            {
                return _comPort;
            }
        }

        private ISikRadioForm loadSettings()
        {
            //Terminal.threadrun = false;

            //panel1.Controls.Clear();

            var form = new Sikradio();
            form.Enabled = false;
            form.GetCommsSerialAlt = () => SikRadio.Config.comPort;

            panel1.Controls.Add(form);

            ThemeManager.SetTheme(ThemeManager.Themes.None);

            ThemeManager.ApplyThemeTo(this);

            return form;
        }

        private ISikRadioForm loadTerminal()
        {
            //panel1.Controls.Clear();

            var form = new Terminal();
            form.Enabled = false;

            form.Dock = DockStyle.Fill;

            panel1.Controls.Add(form);

            ThemeManager.SetTheme(ThemeManager.Themes.None);

            ThemeManager.ApplyThemeTo(this);

            return form;
        }

        private ISikRadioForm loadRssi()
        {
            //Terminal.threadrun = false;

            //panel1.Controls.Clear();

            var form = new Rssi();
            form.Enabled = false;

            form.Dock = DockStyle.Fill;

            panel1.Controls.Add(form);

            ThemeManager.SetTheme(ThemeManager.Themes.None);

            ThemeManager.ApplyThemeTo(this);

            return form;
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

        void ShowForm(Func<ISikRadioForm> Constructor)
        {
            if (_CurrentForm != null)
            {
                _CurrentForm.Disconnect();
                _CurrentForm.Dispose();
            }
            _CurrentForm = Constructor();
            _CurrentForm.Enabled = _Connected;
            _CurrentForm.Show();
            if (_Connected)
            {
                _CurrentForm.Connect();
            }
        }

        private void terminalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(loadTerminal);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(loadSettings);
        }

        private void rssiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(loadRssi);
        }

        void getTelemPortWithRadio(ref ICommsSerial comPort)
        {
            // try telem1

            comPort = new MAVLinkSerialPort(MainV2.comPort, (int)MAVLink.SERIAL_CONTROL_DEV.TELEM1);

            comPort.ReadTimeout = 4000;

            comPort.Open();
        }

        bool Connect()
        {
            try
            {
                if (MainV2.comPort.BaseStream.PortName.Contains("TCP"))
                {
                    _comPort = new TcpSerial();
                    _comPort.BaudRate = MainV2.comPort.BaseStream.BaudRate;
                    _comPort.ReadTimeout = 4000;
                    _comPort.Open();
                }
                else
                {
                    _comPort = new SerialPort();

                    if (MainV2.comPort.BaseStream.IsOpen)
                    {
                        getTelemPortWithRadio(ref _comPort);
                    }
                    else
                    {
                        _comPort.PortName = MainV2.comPort.BaseStream.PortName;
                        _comPort.BaudRate = MainV2.comPort.BaseStream.BaudRate;
                    }

                    _comPort.ReadTimeout = 4000;

                    _comPort.Open();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        bool Disconnect()
        {
            _comPort.Close();
            _comPort = null;
            return true;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (_Connected)
            {
                if (_CurrentForm != null)
                {
                    _CurrentForm.Disconnect();
                }
                Disconnect();
                _Connected = false;
                btnConnect.Text = "Connect";
                if (_CurrentForm != null)
                {
                    _CurrentForm.Enabled = false;
                }
                CMB_Baudrate.Enabled = true;
                CMB_SerialPort.Enabled = true;
            }
            else
            {
                if (Connect())
                {
                    if (_CurrentForm != null)
                    {
                        _CurrentForm.Connect();
                    }
                    _Connected = true;
                    btnConnect.Text = "Disconnect";
                    if (_CurrentForm != null)
                    {
                        _CurrentForm.Enabled = true;
                    }
                    CMB_Baudrate.Enabled = false;
                    CMB_SerialPort.Enabled = false;
                }
            }
        }

        private void Config_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_CurrentForm != null)
            {
                _CurrentForm.Disconnect();
            }
        }
    }
}