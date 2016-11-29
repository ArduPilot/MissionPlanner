using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.Comms;
using System.Collections;
using MissionPlanner.Utilities;
using MissionPlanner.Controls.BackstageView;

namespace MissionPlanner.Wizard
{
    public partial class _3ConnectAP : MyUserControl, IWizard, IActivate
    {
        List<KeyValuePair<string, string>> fwmap = new List<KeyValuePair<string, string>>();
        ProgressReporterDialogue pdr;
        string comport = "";
        bool fwdone = false;
        private bool usebeta;

        public _3ConnectAP()
        {
            fwmap.Add(new KeyValuePair<string, string>("rover", "ar2"));
            fwmap.Add(new KeyValuePair<string, string>("rover", "APMRover"));

            fwmap.Add(new KeyValuePair<string, string>("plane", "AP-"));
            fwmap.Add(new KeyValuePair<string, string>("plane", "apm1/ArduPlane"));

            fwmap.Add(new KeyValuePair<string, string>("planehil", "APHIL-"));
            fwmap.Add(new KeyValuePair<string, string>("planehil", "apm1-hilsensors/ArduPlane"));

            fwmap.Add(new KeyValuePair<string, string>("quad", "ac2-quad-"));
            fwmap.Add(new KeyValuePair<string, string>("quad", "1-quad/ArduCopter"));

            fwmap.Add(new KeyValuePair<string, string>("trap", "ac2-quad-"));
            fwmap.Add(new KeyValuePair<string, string>("trap", "1-quad/ArduCopter"));

            fwmap.Add(new KeyValuePair<string, string>("tri", "ac2-tri"));
            fwmap.Add(new KeyValuePair<string, string>("tri", "-tri/ArduCopter"));

            fwmap.Add(new KeyValuePair<string, string>("hexa", "ac2-hexa"));
            fwmap.Add(new KeyValuePair<string, string>("hexa", "-hexa/ArduCopter"));

            fwmap.Add(new KeyValuePair<string, string>("y6", "ac2-y6"));
            fwmap.Add(new KeyValuePair<string, string>("y6", "-y6/ArduCopter"));

            fwmap.Add(new KeyValuePair<string, string>("heli", "ac2-heli-"));
            fwmap.Add(new KeyValuePair<string, string>("heli", "-heli/ArduCopter"));

            fwmap.Add(new KeyValuePair<string, string>("helihil", "ac2-helhil"));
            fwmap.Add(new KeyValuePair<string, string>("helihil", "-heli-hil/ArduCopter"));

            fwmap.Add(new KeyValuePair<string, string>("quadhil", "ac2-quadhil"));
            fwmap.Add(new KeyValuePair<string, string>("quadhil", "-quad-hil/ArduCopter"));

            fwmap.Add(new KeyValuePair<string, string>("x8", "ac2-octaquad-"));
            fwmap.Add(new KeyValuePair<string, string>("x8", "-octa-quad/ArduCopter"));

            fwmap.Add(new KeyValuePair<string, string>("octa", "ac2-octa-"));
            fwmap.Add(new KeyValuePair<string, string>("octa", "-octa/ArduCopter"));


            InitializeComponent();

            CMB_port.Items.AddRange(SerialPort.GetPortNames());
        }

        public void Activate()
        {
            foreach (var port in CMB_port.Items)
            {
                if (CMB_port.Text == "")
                {
                    if (SerialPort.GetNiceName((string) port).ToLower().Contains("arduino") ||
                        SerialPort.GetNiceName((string) port).ToLower().Contains("px4"))
                    {
                        CMB_port.Text = port.ToString();
                        break;
                    }
                }
            }
        }

        public int WizardValidate()
        {
            comport = CMB_port.Text;

            if (comport == "")
            {
                CustomMessageBox.Show(Strings.SelectComport, Strings.ERROR);
                return 0;
            }

            if (!fwdone)
            {
                pdr = new ProgressReporterDialogue();

                pdr.DoWork += pdr_DoWork;

                ThemeManager.ApplyThemeTo(pdr);

                pdr.RunBackgroundOperationAsync();

                if (pdr.doWorkArgs.CancelRequested || !string.IsNullOrEmpty(pdr.doWorkArgs.ErrorMessage))
                    return 0;

                pdr.Dispose();
            }

            if (MainV2.comPort.BaseStream.IsOpen)
                MainV2.comPort.BaseStream.Close();

            // setup for over usb
            MainV2.comPort.BaseStream.BaudRate = 115200;
            MainV2.comPort.BaseStream.PortName = comport;


            MainV2.comPort.Open(true);

            // try again
            if (!MainV2.comPort.BaseStream.IsOpen)
            {
                CustomMessageBox.Show("Error connecting. Please unplug, plug back in, wait 10 seconds, and click OK",
                    "Try Again");
                MainV2.comPort.Open(true);
            }

            if (!MainV2.comPort.BaseStream.IsOpen)
                return 0;

            if (string.IsNullOrEmpty(pdr.doWorkArgs.ErrorMessage))
            {
                if (Wizard.config["fwtype"].ToString() == "copter" && Wizard.config["fwframe"].ToString() == "tri")
                    // check if its a tri, and skip the frame type screen
                    return 2;
                if (Wizard.config["fwtype"].ToString() == "copter")
                    // check if its a quad, and show the frame type screen
                    return 1;
                if (Wizard.config["fwtype"].ToString() == "rover")
                    // check if its a rover, and show the compass cal screen - skip accel
                    return 3;
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

        void pdr_DoWork(object sender, Controls.ProgressWorkerEventArgs e, object passdata = null)
        {
            // upload fw

            Utilities.Firmware fw = new Utilities.Firmware();
            fw.Progress += fw_Progress;
            string firmwareurl = "";
            if (usebeta)
                firmwareurl = "https://raw.github.com/diydrones/binary/master/dev/firmware2.xml";

            List<Utilities.Firmware.software> swlist = fw.getFWList(firmwareurl);

            if (swlist.Count == 0)
            {
                e.ErrorMessage = "Error getting Firmware list";
                return;
            }

            switch (Wizard.config["fwtype"].ToString())
            {
                case "copter":
                    // fwframe is already defined for copter
                    break;
                default:
                    // mirror fwtype to fwframe
                    Wizard.config["fwframe"] = Wizard.config["fwtype"].ToString();
                    break;
            }

            string target = Wizard.config["fwframe"].ToString();


            if (e.CancelRequested)
            {
                e.CancelAcknowledged = true;
                return;
            }

            foreach (var sw in swlist)
            {
                foreach (KeyValuePair<string, string> parturl in fwmap)
                {
                    if (target.ToLower() == parturl.Key.ToLower() &&
                        sw.url2560.ToLower().Contains(parturl.Value.ToString().ToLower()))
                    {
                        try
                        {
                            fwdone = fw.update(comport, sw, "");
                            //fwdone = true;
                        }
                        catch
                        {
                        }
                        if (fwdone == false)
                        {
                            e.ErrorMessage = "Error uploading Firmware";
                            return;
                        }
                        break;
                    }
                }
                if (fwdone)
                    break;
            }

            if (e.CancelRequested)
            {
                e.CancelAcknowledged = true;
                return;
            }

            if (!fwdone)
            {
                e.ErrorMessage = "Error with Firmware";
                return;
            }

            return;
        }

        void fw_Progress(int progress, string status)
        {
            pdr.UpdateProgressAndStatus(progress, status);
        }

        private void CMB_port_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            ComboBox combo = sender as ComboBox;
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight),
                    e.Bounds);
            else
                e.Graphics.FillRectangle(new SolidBrush(combo.BackColor),
                    e.Bounds);

            string text = combo.Items[e.Index].ToString();
            if (!MainV2.MONO)
            {
                text = text + " " + SerialPort.GetNiceName(text);
            }

            e.Graphics.DrawString(text, e.Font,
                new SolidBrush(combo.ForeColor),
                new Point(e.Bounds.X, e.Bounds.Y));

            e.DrawFocusRectangle();
        }

        private void CMB_port_Click(object sender, EventArgs e)
        {
            string oldport = CMB_port.Text;

            CMB_port.Items.Clear();
            CMB_port.Items.AddRange(SerialPort.GetPortNames());

            if (CMB_port.Items.Contains(oldport))
                CMB_port.Text = oldport;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (MainV2.comPort.BaseStream.IsOpen)
                MainV2.comPort.BaseStream.Close();

            if (CMB_port.Text == "")
            {
                CustomMessageBox.Show("Please pick a port");
                return;
            }

            MainV2.comPort.BaseStream.PortName = CMB_port.Text;
            MainV2.comPort.BaseStream.BaudRate = 115200;

            if (!MainV2.comPort.BaseStream.IsOpen)
                MainV2.comPort.Open(true);
            Wizard.instance.GoNext(1);
        }

        private void label8_Click(object sender, EventArgs e)
        {
            usebeta = true;
            CustomMessageBox.Show("Using beta FW");
        }
    }
}