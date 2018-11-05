using System;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using log4net;
using MissionPlanner.Controls;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigHWESP8266 : MyUserControl, IActivate
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ConfigHWESP8266()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            if (MainV2.comPort.BaseStream.IsOpen)
            {
                MainV2.comPort.sendPacket(new MAVLink.mavlink_param_request_list_t()
                {
                    target_system = (byte) 0,
                    target_component = (byte) MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE
                }, MainV2.comPort.sysidcurrent, (byte) MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE);
            }

            if (MainV2.comPort.BaseStream.IsOpen
            ) // && MainV2.comPort.MAV.compid == (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE)
            {
                byte sysid = MainV2.comPort.MAV.sysid;

                var mav = MainV2.comPort.MAVlist[sysid, (byte) MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE];

                if (mav.param.ContainsKey("WIFI_SSID1"))
                {
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

                    var ssidsta = (ASCIIEncoding.ASCII.GetString(mav.param["WIFI_SSIDSTA1"].data) +
                                   ASCIIEncoding.ASCII.GetString(mav.param["WIFI_SSIDSTA2"].data) +
                                   ASCIIEncoding.ASCII.GetString(mav.param["WIFI_SSIDSTA3"].data) +
                                   ASCIIEncoding.ASCII.GetString(mav.param["WIFI_SSIDSTA4"].data)).TrimEnd('\0');

                    var pwdsta = (ASCIIEncoding.ASCII.GetString(mav.param["WIFI_PWDSTA1"].data) +
                                  ASCIIEncoding.ASCII.GetString(mav.param["WIFI_PWDSTA2"].data) +
                                  ASCIIEncoding.ASCII.GetString(mav.param["WIFI_PWDSTA3"].data) +
                                  ASCIIEncoding.ASCII.GetString(mav.param["WIFI_PWDSTA4"].data)).TrimEnd('\0');

                    var DEBUG_ENABLED = mav.param["DEBUG_ENABLED"].ToString();
                    var WIFI_MODE = mav.param["WIFI_MODE"].ToString();
                    var WIFI_IPADDRESS =
                        new IPAddress(BitConverter.GetBytes((int) mav.param["WIFI_IPADDRESS"])).ToString();
                    var WIFI_UDP_HPORT = mav.param["WIFI_UDP_HPORT"].ToString();
                    var WIFI_UDP_CPORT = mav.param["WIFI_UDP_CPORT"].ToString();

                    var WIFI_IPSTA = new IPAddress(BitConverter.GetBytes((int) mav.param["WIFI_IPSTA"])).ToString();
                    var WIFI_GATEWAYSTA =
                        new IPAddress(BitConverter.GetBytes((int) mav.param["WIFI_GATEWAYSTA"])).ToString();
                    var WIFI_SUBNET_STA =
                        new IPAddress(BitConverter.GetBytes((int) mav.param["WIFI_SUBNET_STA"])).ToString();

                    txt_ip.Text = WIFI_IPSTA;
                    txt_gateway.Text = WIFI_GATEWAYSTA;
                    txt_subnet.Text = WIFI_SUBNET_STA;

                    chk_mode.Checked = (WIFI_MODE == "0") ? false : true;

                    label5.Text = String.Format("DEBUG_ENABLED {0},\n" +
                                                "WIFI_MODE {1},\n" +
                                                "WIFI_IPADDRESS {2},\n" +
                                                "WIFI_UDP_HPORT {3},\n" +
                                                "WIFI_UDP_CPORT {4},\n" +
                                                "WIFI_IPSTA {5},\n" +
                                                "WIFI_GATEWAYSTA {6},\n" +
                                                "WIFI_SUBNET_STA {7}\n",
                        DEBUG_ENABLED, WIFI_MODE,WIFI_IPADDRESS,
                        WIFI_UDP_HPORT,
                        WIFI_UDP_CPORT, WIFI_IPSTA, WIFI_GATEWAYSTA, WIFI_SUBNET_STA);

                    Enabled = true;
                    return;
                }
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
            MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE,"WIFI_CHANNEL", int.Parse(cmb_channel.Text));
            MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE, "UART_BAUDRATE", int.Parse(cmb_baud.Text));

            MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE, "WIFI_SSID1", BitConverter.ToUInt32(stringTobytearray(txt_ssid.Text, 0, 4),0));
            MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE, "WIFI_SSID2",BitConverter.ToUInt32( stringTobytearray(txt_ssid.Text, 4, 4),0));
            MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE, "WIFI_SSID3", BitConverter.ToUInt32(stringTobytearray(txt_ssid.Text, 8, 4),0));
            MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE, "WIFI_SSID4", BitConverter.ToUInt32(stringTobytearray(txt_ssid.Text, 12, 4), 0));

            MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE, "WIFI_PASSWORD1", BitConverter.ToUInt32(stringTobytearray(txt_password.Text, 0, 4), 0));
            MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE, "WIFI_PASSWORD2", BitConverter.ToUInt32(stringTobytearray(txt_password.Text, 4, 4), 0));
            MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE, "WIFI_PASSWORD3", BitConverter.ToUInt32(stringTobytearray(txt_password.Text, 8, 4), 0));
            MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE, "WIFI_PASSWORD4", BitConverter.ToUInt32(stringTobytearray(txt_password.Text, 12, 4), 0));

            MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE, "WIFI_SSIDSTA1", BitConverter.ToUInt32(stringTobytearray(txt_ssid.Text, 0, 4), 0));
            MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE, "WIFI_SSIDSTA2", BitConverter.ToUInt32(stringTobytearray(txt_ssid.Text, 4, 4), 0));
            MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE, "WIFI_SSIDSTA3", BitConverter.ToUInt32(stringTobytearray(txt_ssid.Text, 8, 4), 0));
            MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE, "WIFI_SSIDSTA4", BitConverter.ToUInt32(stringTobytearray(txt_ssid.Text, 12, 4), 0));

            MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE, "WIFI_PWDSTA1", BitConverter.ToUInt32(stringTobytearray(txt_password.Text, 0, 4), 0));
            MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE, "WIFI_PWDSTA2", BitConverter.ToUInt32(stringTobytearray(txt_password.Text, 4, 4), 0));
            MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE, "WIFI_PWDSTA3", BitConverter.ToUInt32(stringTobytearray(txt_password.Text, 8, 4), 0));
            MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE, "WIFI_PWDSTA4", BitConverter.ToUInt32(stringTobytearray(txt_password.Text, 12, 4), 0));

            MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE, "WIFI_IPSTA", BitConverter.ToUInt32(IPAddress.Parse(txt_ip.Text).GetAddressBytes(), 0));
            MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE, "WIFI_GATEWAYSTA", BitConverter.ToUInt32(IPAddress.Parse(txt_gateway.Text).GetAddressBytes(), 0));
            MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE, "WIFI_SUBNET_STA", BitConverter.ToUInt32(IPAddress.Parse(txt_subnet.Text).GetAddressBytes(), 0));

            MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE, "WIFI_MODE", chk_mode.Checked ? 1 : 0);

            // save to eeprom
            bool pass = MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE, MAVLink.MAV_CMD.PREFLIGHT_STORAGE, 1, 0, 0, 0, 0, 0, 0);

            // reboot
            pass =  pass & MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE, MAVLink.MAV_CMD.PREFLIGHT_REBOOT_SHUTDOWN, 0, 1, 0, 0, 0, 0, 0);

            if (!pass)
                CustomMessageBox.Show(Strings.ErrorSettingParameter, Strings.ERROR);
            else
                CustomMessageBox.Show(Strings.ProgrammedOK);
        }

        private void but_resetdefault_Click(object sender, EventArgs e)
        {
            // reset to defaults
            if (MainV2.comPort.doCommand((byte) MainV2.comPort.sysidcurrent,
                (byte) MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE, MAVLink.MAV_CMD.PREFLIGHT_STORAGE, 2, 0, 0, 0, 0,
                0, 0))
            {
                // save the defaults
                if (MainV2.comPort.doCommand((byte) MainV2.comPort.sysidcurrent,
                    (byte) MAVLink.MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE, MAVLink.MAV_CMD.PREFLIGHT_STORAGE, 1, 0, 0, 0,
                    0, 0, 0))
                {
                    // pass control back to the user
                    Activate();

                    Activate();

                    CustomMessageBox.Show(Strings.ProgrammedOK);
                }
                else
                {
                    CustomMessageBox.Show(Strings.ErrorSettingParameter, Strings.ERROR);
                }
            }
            else
            {
                CustomMessageBox.Show(Strings.ErrorSettingParameter, Strings.ERROR);
            }
        }

        private void chk_mode_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_mode.Checked)
            {
                groupBoxsta.Enabled = true;
            }
            else
            {
                groupBoxsta.Enabled = false;
            }
        }
    }
}