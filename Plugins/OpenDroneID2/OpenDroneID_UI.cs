using System;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;
using MissionPlanner.Comms;
using MissionPlanner.Utilities;
using System.Drawing;
using System.Diagnostics;
using MissionPlanner.ArduPilot;

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

        public OpenDroneID_UI()
        {
            Instance = this;

            InitializeComponent();

            start_sub(true);
        }

        private void start_sub(bool force = false)
        {
            if (!force && (MainV2.comPort.BaseStream == null || !MainV2.comPort.BaseStream.IsOpen))
            {
                // pass
            }
            else
            {
                Console.WriteLine("\n\n\n[DRONE ID] Subscribing to OPEN_DRONE_ID_ARM_STATUS for SysId: " + MainV2.comPort.sysidcurrent);
                //MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.OPEN_DRONE_ID_ARM_STATUS, handleODIDArmMSg2, (byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent);
                MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.OPEN_DRONE_ID_ARM_STATUS, handleODIDArmMSg2, 0, 0);

            }
        }

        private bool handleODIDArmMSg2(MAVLink.MAVLinkMessage arg)
        {
            Console.WriteLine("Got ODID Message!");
            //MAVLink.mavlink_open_drone_id_arm_status_t odid_arm_status;
            odid_arm_status = arg.ToStructure<MAVLink.mavlink_open_drone_id_arm_status_t>();

            // TODO: Check timestamp of ODID message and indicate error
            if (hasODID == true)
                last_odid_msg.Restart();
            else
            {
                Console.WriteLine("[DRONE_ID] Detected and Starting on System ID: " + MainV2.comPort.MAV.sysid);
                addStatusMessage("Detected and Starting on System ID: " + MainV2.comPort.MAV.sysid);
                last_odid_msg.Start();
                myDID.Start(MainV2.comPort, arg.sysid, arg.compid);
            }
            
            hasODID = true;

            return true; ;
        }

        //TO-DO - we may want to move this to a more centralized spot, or make this 
        //the primary thread for Moving Base GPS read. 
        private void timer1_Tick(object sender, EventArgs e)
        {


            checkGCSGPS();

            checkODIDMsgs();

            checkODID_OK();

            checkUID();
        }

        private void checkODIDMsgs()
        {
            if (hasODID == false) return;

            // Check Requirements
            _odid_arm_msg = (last_odid_msg.ElapsedMilliseconds < 5000);
            LED_RemoteID_Messages.Color = (_odid_arm_msg==false) ? Color.Red : Color.Green;

            LED_ArmedError.Color = ((odid_arm_status.status > 0) ? Color.Red : Color.Green);

            TXT_ODID_Status.Text = ((odid_arm_status.status > 0) ? "Ready" : System.Text.Encoding.UTF8.GetString(odid_arm_status.error));
                        
        }

        private void checkODID_OK()
        {
            
            if (_gcs_gps == false)
            {
                
                myODID_Status.setStatusAlert("GCS GPS Invalid");
                
            } else if (_odid_arm_msg == false)
            {
                myODID_Status.setStatusAlert("Remote ID Msg Timeout");
                
            } else if (_odid_arm_status == false)
            {
                myODID_Status.setStatusAlert("Remote ID ARM Error");
                
            } else
            {
                myODID_Status.setStatusOK();
                
            }
            
        }

        private void checkUID()
        {

            _uas_id = !String.IsNullOrEmpty(TXT_UAS_ID.Text);
            LED_UAS_ID.Color = _uas_id ? Color.Green : Color.Red;
            
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


                    MainV2.comPort.MAV.cs.MovingBase = gotolocation;

                    myDID.operator_latitude = nmea_GPS_1.Lat;
                    myDID.operator_longitude = nmea_GPS_1.Lng;
                    myDID.operator_altitude_geo = (float)wgs84_alt;
                    myDID.operator_location_type = MAVLink.MAV_ODID_OPERATOR_LOCATION_TYPE.LIVE_GNSS;


                }
            } catch
            {

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
                _gcs_gps = false;
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
