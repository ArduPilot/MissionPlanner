using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using log4net;
using MissionPlanner.Controls;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigHWESP8266 : UserControl, IActivate
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ConfigHWESP8266()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            if (MainV2.comPort.BaseStream.IsOpen && MainV2.comPort.MAV.compid == (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE)
            {
                byte sysid = MainV2.comPort.MAV.sysid;

                var mav = MainV2.comPort.MAVlist[sysid, (byte) MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE];

                txt_ssid.Text = (ASCIIEncoding.ASCII.GetString(mav.param["WIFI_SSID1"].data) +
                                 ASCIIEncoding.ASCII.GetString(mav.param["WIFI_SSID2"].data) +
                                 ASCIIEncoding.ASCII.GetString(mav.param["WIFI_SSID3"].data) +
                                 ASCIIEncoding.ASCII.GetString(mav.param["WIFI_SSID4"].data)).TrimEnd('\0');

                cmb_baud.Text = mav.param["UART_BAUDRATE"].ToString();

                txt_password.Text = (ASCIIEncoding.ASCII.GetString(mav.param["WIFI_PASSWORD1"].data) +
                                     ASCIIEncoding.ASCII.GetString(mav.param["WIFI_PASSWORD2"].data) +
                                     ASCIIEncoding.ASCII.GetString(mav.param["WIFI_PASSWORD3"].data) +
                                     ASCIIEncoding.ASCII.GetString(mav.param["WIFI_PASSWORD4"].data)).TrimEnd('\0');

                cmb_channel.Text = mav.param["WIFI_CHANNEL"].ToString();

                Enabled = true;
                return;
            }
            Enabled = false;
        }

        byte[] stringTobytearray(string input, int start, int length)
        {
            var ans = ASCIIEncoding.ASCII.GetBytes(input);

            Array.Resize(ref ans, start + length);

            byte[] dst = new byte[length];

            Array.ConstrainedCopy(ans,start,dst,0,length);

            return dst;
        }

        private void BUT_ESPsettings_Click(object sender, EventArgs e)
        {
            MainV2.comPort.setParam("WIFI_CHANNEL", int.Parse(cmb_channel.Text));
            MainV2.comPort.setParam("UART_BAUDRATE", int.Parse(cmb_baud.Text));

            MainV2.comPort.setParam("WIFI_SSID1", BitConverter.ToUInt32(stringTobytearray(txt_ssid.Text, 0, 4),0));
            MainV2.comPort.setParam("WIFI_SSID2",BitConverter.ToUInt32( stringTobytearray(txt_ssid.Text, 4, 4),0));
            MainV2.comPort.setParam("WIFI_SSID3", BitConverter.ToUInt32(stringTobytearray(txt_ssid.Text, 8, 4),0));
            MainV2.comPort.setParam("WIFI_SSID4", BitConverter.ToUInt32(stringTobytearray(txt_ssid.Text, 12, 4), 0));

            MainV2.comPort.setParam("WIFI_PASSWORD1", BitConverter.ToUInt32(stringTobytearray(txt_password.Text, 0, 4), 0));
            MainV2.comPort.setParam("WIFI_PASSWORD2", BitConverter.ToUInt32(stringTobytearray(txt_password.Text, 4, 4), 0));
            MainV2.comPort.setParam("WIFI_PASSWORD3", BitConverter.ToUInt32(stringTobytearray(txt_password.Text, 8, 4), 0));
            MainV2.comPort.setParam("WIFI_PASSWORD4", BitConverter.ToUInt32(stringTobytearray(txt_password.Text, 12, 4), 0));
            
            // save to eeprom
            bool pass = MainV2.comPort.doCommand(MAVLink.MAV_CMD.PREFLIGHT_STORAGE, 1, 0, 0, 0, 0, 0, 0);

            // reboot
            pass =  pass & MainV2.comPort.doCommand(MAVLink.MAV_CMD.PREFLIGHT_REBOOT_SHUTDOWN, 0, 1, 0, 0, 0, 0, 0);

            if (!pass)
                CustomMessageBox.Show(Strings.ErrorSettingParameter, Strings.ERROR);
            else
                CustomMessageBox.Show(Strings.ProgrammedOK);
        }
    }
}