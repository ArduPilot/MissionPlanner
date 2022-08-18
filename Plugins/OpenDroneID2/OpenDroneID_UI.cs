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
using MissionPlanner.ArduPilot;
using static MissionPlanner.Utilities.LTM;


namespace MissionPlanner.Controls
{
    public partial class OpenDroneID_UI : UserControl
    {
        static OpenDroneID_UI Instance;

        static MAVLink.mavlink_open_drone_id_arm_status_t odid_arm_status;
        private bool hasODID = false;
        
        private bool gpsHasSBAS = false;
        private double wgs84_alt;
        private Stopwatch last_odid_msg = new Stopwatch();
        PointLatLngAlt gotolocation = new PointLatLngAlt();

        private bool _odid_arm_msg, _uas_id, _gcs_gps, _odid_arm_status; 

        private const int ODID_ARM_MESSAGE_TIMEOUT = 5000;
        public OpenDroneID myDID = new OpenDroneID();

        private Plugin.PluginHost _host = null;

        private int _mySYS = 0; 

        private int _last_odid_error_agg;

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

            timer1.Start();
            timer2.Start();
        }

        public void setHost(Plugin.PluginHost host)
        {
            _host = host;
        }

        private void start_sub(bool force = false)
        {

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
                    myDID.Start(_host.comPort, arg.sysid, arg.compid);
                }
                catch
                {
                    Console.WriteLine("Error Initializing ODID Message Handler");
                }

            }
            
            hasODID = true;

            




            return true; ;
        }

 
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!hasODID) return;

            checkODIDMsgs();

            checkUID();

            checkODID_OK();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            

            checkGCSGPS();

            start_sub();
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

        private void checkODID_OK()
        {

            if (_gcs_gps == false)
            {

                myODID_Status.setStatusAlert("GCS GPS Invalid");

            }
            else if (_odid_arm_msg == false)
            {
                myODID_Status.setStatusAlert("Remote ID Msg Timeout");

            }
            else if (_odid_arm_status == false)
            {
                myODID_Status.setStatusAlert("Remote ID ARM Error");

            }
            else if (_uas_id == false)
            {
                myODID_Status.setStatusAlert("Need to input UAS ID");
            }
            else
            {
                myODID_Status.setStatusOK();

            }
            
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
                
                
                // Sanity Check
                if (nmea_GPS_1.Lat != 0.0 && nmea_GPS_1.Lng != 0.0)
                {

                    gotolocation.Lat = nmea_GPS_1.Lat; 
                    gotolocation.Lng = nmea_GPS_1.Lng;
                    gotolocation.Alt = nmea_GPS_1.Alt; 

                    if (_host != null)
                        _host.comPort.MAV.cs.MovingBase = gotolocation;

                    myDID.operator_latitude = nmea_GPS_1.Lat;
                    myDID.operator_longitude = nmea_GPS_1.Lng;
                    myDID.operator_altitude_geo = (float)wgs84_alt;
                    myDID.operator_location_type = MAVLink.MAV_ODID_OPERATOR_LOCATION_TYPE.LIVE_GNSS;


                }
            } catch
            {
                Console.WriteLine("Error Setting NMEA GPS Data");
            }

            // Check GCS GPS
            if (nmea_GPS_1.last_gps_msg.ElapsedMilliseconds > 5000)
            {
                //TODO Fix
                //LBL_GCS_GPS_Invalid.Text = "GCS Data Timeout.";
                _gcs_gps = false;
            }
            else if (gotolocation.Lat == 0.0 || gotolocation.Lng == 0.0)
            {
                //LBL_GCS_GPS_Invalid.Text = "GCS GPS Lock Invalid.";
                LED_gps_valid.Color = Color.Orange;
                _gcs_gps = false;
            }
            else if (gpsHasSBAS == false)
            {
                LED_gps_valid.Color = Color.Yellow;
                //LBL_GCS_GPS_Invalid.Text = "GCS No DGPS Corr.";
                _gcs_gps = true;  // NOTE: This may need to be changed in the future to enforce SBAS only solutions
            }
            else
            {
                LED_gps_valid.Color = Color.Green;
                //LBL_GCS_GPS_Invalid.Text = "";
                _gcs_gps = true;
            }
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

    }

}
