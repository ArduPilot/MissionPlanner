using System;
using System.Linq;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;
using MissionPlanner.Comms;
using MissionPlanner.Utilities;
using System.Drawing;
using System.Diagnostics;
using static MissionPlanner.Utilities.LTM;
using System.Runtime.InteropServices;


namespace MissionPlanner.Controls
{
    public partial class OpenDroneID_UI : UserControl
    {
        static OpenDroneID_UI Instance;

        static MAVLink.mavlink_open_drone_id_arm_status_t odid_arm_status;
        private bool hasODID = false;
        
        private bool gpsHasSBAS = false;

        private Stopwatch last_odid_msg = new Stopwatch();
        PointLatLngAlt gotolocation = new PointLatLngAlt();

        private bool _odid_arm_msg, _uas_id, _gcs_gps, _odid_arm_status; 

        private const int ODID_ARM_MESSAGE_TIMEOUT = 5000;
        private OpenDroneID_Backend myDID = new OpenDroneID_Backend();
        private OpenDroneID_Map_Status host_map_status = new OpenDroneID_Map_Status();

        private Plugin.PluginHost _host = null;

        private int _mySYS = 0; 

        private int _last_odid_error_agg;

        System.Threading.Thread _thread_odid;
        static bool threadrun = false;
        DateTime _last_time_1 = DateTime.Now;
        float _update_rate_hz_1 = 10.0f; // 10 hz
        DateTime _last_time_2 = DateTime.Now;
        float _update_rate_hz_2 = 1.0f; // 1 hz

        bool dev_mode_rm = false;

        public OpenDroneID_UI()
        {
            Instance = this;
            
            

            InitializeComponent();

            CMB_op_id_type.DisplayMember = "Value";
            CMB_op_id_type.ValueMember = "Key";
            CMB_op_id_type.DataSource = System.Enum.GetValues(typeof(MAVLink.MAV_ODID_OPERATOR_ID_TYPE));

            CMB_uas_id_type.DisplayMember = "Value";
            CMB_uas_id_type.ValueMember = "Key";
            CMB_uas_id_type.DataSource = System.Enum.GetValues(typeof(MAVLink.MAV_ODID_ID_TYPE));

            CMB_uas_type.DisplayMember = "Value";
            CMB_uas_type.ValueMember = "Key";
            CMB_uas_type.DataSource = System.Enum.GetValues(typeof(MAVLink.MAV_ODID_UA_TYPE));

            CMB_self_id_type.DisplayMember = "Value";
            CMB_self_id_type.ValueMember = "Key";
            CMB_self_id_type.DataSource = System.Enum.GetValues(typeof(MAVLink.MAV_ODID_DESC_TYPE));

            myODID_Status._parent_ODID = this;

            start();
        }

        public void start()
        {
            //Console.WriteLine();
            _thread_odid = new System.Threading.Thread(new System.Threading.ThreadStart(mainloop))
            {
                IsBackground = true,
                Name = "ODID_Thread"
            };
            _thread_odid.Start();
        }

        public void setVer(String msg)
        {
            LBL_version.Text = msg;
        }
        public void setHost(Plugin.PluginHost host)
        {
            _host = host;
        }

        private void start_sub(bool force = false)
        {
            if (_host == null) return;

            if (!force && (_host.comPort.BaseStream == null || !_host.comPort.BaseStream.IsOpen))
            {
                // pass
            }
            else if (_host.comPort.sysidcurrent != _mySYS && _host.comPort.sysidcurrent > 0)
            {
                addStatusMessage("Sub. to ODID ARM_STATUS for SysId: " + _host.comPort.sysidcurrent);
                
                _host.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.OPEN_DRONE_ID_ARM_STATUS, handleODIDArmMSg2, (byte) _host.comPort.sysidcurrent, (byte) MAVLink.MAV_COMPONENT.MAV_COMP_ID_ODID_TXRX_1);
                _host.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.OPEN_DRONE_ID_ARM_STATUS, handleODIDArmMSg2, (byte)_host.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_ODID_TXRX_2);
                _host.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.OPEN_DRONE_ID_ARM_STATUS, handleODIDArmMSg2, (byte)_host.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_ODID_TXRX_3);
                _host.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.OPEN_DRONE_ID_ARM_STATUS, handleODIDArmMSg2, (byte)_host.comPort.sysidcurrent, (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_AUTOPILOT1);
                _mySYS =  _host.comPort.sysidcurrent;
                hasODID = false;
                last_odid_msg.Stop();

                //myDID.Stop();
            }
        }

        private bool handleODIDArmMSg2(MAVLink.MAVLinkMessage arg)
        {
            
            odid_arm_status = arg.ToStructure<MAVLink.mavlink_open_drone_id_arm_status_t>();


            // TODO: Check timestamp of ODID message and indicate error
            if (hasODID == true)
            {
                last_odid_msg.Restart();

                int _this_agg = (odid_arm_status.status == 0) ? 0 : odid_arm_status.error.Aggregate(0, (a, b) => a + b);

                if (_this_agg != _last_odid_error_agg)
                {
                    if (odid_arm_status.status != 0) {
                        string s = System.Text.Encoding.ASCII.GetString(odid_arm_status.error);
                        addStatusMessage("Arm Error: " + s.Substring(0, s.IndexOf((char)0)));
                    }
                    else
                        addStatusMessage("Arm Stats: Ready");
                    _last_odid_error_agg = _this_agg;
                }
            }
            else
            {
                try
                {
                    Console.WriteLine("[DRONE_ID] Detected and Starting on System ID: " + _host.comPort.MAV.sysid);
                    addStatusMessage("Detected and Starting on System ID: " + _host.comPort.MAV.sysid);

                    last_odid_msg.Start();
                    if (dev_mode_rm == false)
                        myDID.Start(_host.comPort, arg.sysid, arg.compid);
                    host_map_status.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                    
                    _host.MainForm.Invoke((Action)(() =>
                    {
                        _host.FDGMapControl.Controls.Add(host_map_status);
                        host_map_status.Location = new System.Drawing.Point(_host.FDGMapControl.Width-host_map_status.Width-10, 25);
                        host_map_status._parent_ODID = this;
                    }));
                    host_map_status.Visible = true;

                    addStatusMessage("Double Click Primary status indicator to declare Emergency over ODID.");

                }
                catch
                {
                    Console.WriteLine("Error Initializing ODID Message Handler");
                }

            }
            
            hasODID = true;

            




            return true; ;
        }

 
 


        private void checkODIDMsgs()
        {
            if (hasODID == false) return;

            // Check Requirements
            _odid_arm_msg = (last_odid_msg.ElapsedMilliseconds < 5000);

            if (_odid_arm_msg == false)
            {
                _odid_arm_status = false;  // can't be valid if it's timed out. 
            } 

            LED_RemoteID_Messages.Color = (_odid_arm_msg==false) ? Color.Red : Color.Green;

            _odid_arm_status = (odid_arm_status.status == 0);
            LED_ArmedError.Color = (_odid_arm_status==false ? Color.Red : Color.Green);


            if (_odid_arm_msg == false)
                TXT_RID_Status_Msg.Text = "Timeout.";
            else
                TXT_RID_Status_Msg.Text = ((odid_arm_status.status == 0) ? "Ready" : System.Text.Encoding.UTF8.GetString(odid_arm_status.error));
                        
        }

        public void setEmergencyFromMap()
        {
            Console.WriteLine("ODID - Pilot Declared an ODID Emergency");
            TXT_self_id_TXT.Text = "Pilot Emergency Status Declared";
            CMB_self_id_type.SelectedIndex = (int)MAVLink.MAV_ODID_DESC_TYPE.EMERGENCY;
            addStatusMessage("Pilot Emergency Status");
        }

        private void checkODID_OK()
        {
            string msg = "";

            if (CMB_self_id_type.SelectedIndex == (int)MAVLink.MAV_ODID_DESC_TYPE.EMERGENCY)
            {
                myODID_Status.setStatusEmergency(TXT_self_id_TXT.Text);
                host_map_status.setStatusEmergency(TXT_self_id_TXT.Text);
                
                return;
            }

            if (_gcs_gps == false)
            {
                msg = "GCS GPS Invalid";

            }
            else if (_odid_arm_msg == false)
            {
                msg = "Remote ID Msg Timeout";

            }
            else if (_odid_arm_status == false)
            {
                msg = "Remote ID ARM Error";

            }
            else if (_uas_id == false)
            {
                msg = "Need to input UAS ID";
            }
            else
            {
                myODID_Status.setStatusOK();
                host_map_status.setStatusOK();
                return;
            }

            myODID_Status.setStatusAlert(msg);
            host_map_status.setStatusAlert(msg);


        }

        private void checkUID()
        {

            _uas_id = (!String.IsNullOrEmpty(TXT_UAS_ID.Text) && CMB_uas_id_type.SelectedIndex > 0 && CMB_uas_type.SelectedIndex > 0);
            LED_UAS_ID.Color = _uas_id ? Color.Green : Color.Red;

            // Note - this needs to be updated later to accomondate a Standard Remote ID Configuratoin
            if (_uas_id && CMB_uas_id_type.SelectedIndex > 0 && CMB_uas_type.SelectedIndex > 0)
            {
                myDID.UAS_ID = TXT_UAS_ID.Text;
                
                myDID.UA_type = (MAVLink.MAV_ODID_ID_TYPE) CMB_uas_id_type.SelectedIndex;
                myDID.UAS_ID_type = (MAVLink.MAV_ODID_UA_TYPE) CMB_uas_type.SelectedIndex;

            }

            if (TXT_self_id_TXT.Text.Length > 0)
            {
                // Send Self ID Info
                myDID.description = TXT_self_id_TXT.Text;
                myDID.description_type = (MAVLink.MAV_ODID_DESC_TYPE) CMB_self_id_type.SelectedIndex;
            }

            if (txt_UserID.Text.Length > 0)
            {
                myDID.operator_id = txt_UserID.Text;
                myDID.operator_id_type = (MAVLink.MAV_ODID_OPERATOR_ID_TYPE)CMB_op_id_type.SelectedIndex;
            }

            
            
        }

        private void checkGCSGPS()
        {
            try 
            {
                // Check NMEA GPS information
                NMEA_GPS_Connection.PointNMEA _gps_data = nmea_GPS_1.getPointNMEA();

                

                // Sanity Check
                if (_gps_data.Lat != 0.0 && _gps_data.Lng != 0.0)
                {

                    gotolocation.Lat = _gps_data.Lat; 
                    gotolocation.Lng = _gps_data.Lng;
                    gotolocation.Alt = _gps_data.Alt; 

                    if (_host != null)
                        _host.comPort.MAV.cs.MovingBase = gotolocation;

                    myDID.operator_latitude = _gps_data.Lat;
                    myDID.operator_longitude = _gps_data.Lng;
                    myDID.operator_altitude_geo = (float)_gps_data.Alt_WGS84;
                    myDID.operator_location_type = MAVLink.MAV_ODID_OPERATOR_LOCATION_TYPE.LIVE_GNSS;

                    myDID.since_last_msg_ms = nmea_GPS_1.last_gps_msg.ElapsedMilliseconds; 

                }
            } catch
            {
                Console.WriteLine("Error Setting NMEA GPS Data");
            }

            // Check GCS GPS
            if (nmea_GPS_1.last_gps_msg.ElapsedMilliseconds > 5000)
            {
                //TODO Fix
                _gcs_gps = false;
            }
            else if (gotolocation.Lat == 0.0 || gotolocation.Lng == 0.0)
            {
                LED_gps_valid.Color = Color.Orange;
                _gcs_gps = false;
            }
            else if (gpsHasSBAS == false)
            {
                LED_gps_valid.Color = Color.Yellow;
                _gcs_gps = true;  // NOTE: This may need to be changed in the future to enforce SBAS only solutions
            }
            else
            {
                LED_gps_valid.Color = Color.Green;
                _gcs_gps = true;
            }
        }

        private void LBL_version_DoubleClick(object sender, EventArgs e)
        {
            // Note: this function is for development only and should be removed for production enviroments. 

            if (CustomMessageBox.Show("Are you sure you want to disable outgoing Remote ID?", "RID Developer Mode?", CustomMessageBox.MessageBoxButtons.YesNo) == CustomMessageBox.DialogResult.No)
            {
                return;
            }
            Console.WriteLine("----------------- REMOTE ID Outgoing Messages have been disabled ------------------------------------");
            addStatusMessage("REMOTE ID outgoing messages have been DIABLED");
            dev_mode_rm = true;
            myDID.Stop();
            _thread_odid.Abort();
        }

        private void addStatusMessage(String msg)
        {
            TXT_ODID_Status.Text = DateTime.Now.ToString("HH:mm:ss") + " - " + msg + Environment.NewLine + TXT_ODID_Status.Text; 

            // TODO: Add a cleanup step here. 
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Settings.Instance["ODID_UAS_ID"] = TXT_UAS_ID.Text; 
        }

        private void mainloop()
        {
            threadrun = true;
            while (threadrun)
            {
                DateTime _now = DateTime.Now;
                try
                {
                    if (_now > _last_time_1.AddSeconds(1.0 / _update_rate_hz_1))
                    {
                        // Check GPS
                        if (hasODID) {
                            checkODIDMsgs();

                            checkUID();

                            checkODID_OK();
                        }
                        _last_time_1 = DateTime.Now;
                    }
                }
                catch
                {
                    System.Threading.Thread.Sleep((int)(1000 / _update_rate_hz_1));
                }

                try
                {
                    if (_now > _last_time_2.AddSeconds(1.0 / _update_rate_hz_2))
                    {

                        checkGCSGPS();

                        start_sub();
                        _last_time_2 = DateTime.Now;
                    }
                }
                catch
                {
                    System.Threading.Thread.Sleep((int)(1000 / _update_rate_hz_2));
                }


                System.Threading.Thread.Sleep((int)25);
            }
        }

    }

}
