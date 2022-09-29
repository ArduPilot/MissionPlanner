using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MissionPlanner.Utilities;
using DateTime = System.DateTime;
using MissionPlanner;


namespace MissionPlanner
{
    /// <summary>
    /// https://mavlink.io/en/services/opendroneid.html
    /// </summary>
    public class OpenDroneID_Backend
    {
        private MAVLinkInterface _mav;
        private byte target_system;
        private byte target_component;

        private  float rate_hz  = 0.1f;
        private float system_updaterate_hz  = 1.0f;

        private long _gps_timeout_ms = 5000;

        public MAVLink.MAV_ODID_UA_TYPE UAS_ID_type { get; set; } = 0;
        public string UAS_ID { get; set; } = "";
        public MAVLink.MAV_ODID_ID_TYPE UA_type { get; set; } = 0;
        public MAVLink.MAV_ODID_DESC_TYPE description_type { get; set; } = 0;
        public string description { get; set; } = "";
        public byte area_count { get; set; } = 1;
        public byte area_radius { get; set; } = 0;
        public int area_ceiling { get; set; } = -1000;
        public int area_floor { get; set; } = -1000;
        public MAVLink.MAV_ODID_CATEGORY_EU category_eu { get; set; } = 0;
        public MAVLink.MAV_ODID_CLASS_EU class_eu { get; set; } = 0;
        public MAVLink.MAV_ODID_CLASSIFICATION_TYPE classification_type { get; set; } = 0;
        public MAVLink.MAV_ODID_OPERATOR_LOCATION_TYPE operator_location_type { get; set; } = 0;
        public MAVLink.MAV_ODID_OPERATOR_ID_TYPE operator_id_type { get; set; } = 0;
        public string operator_id { get; set; } = "";

        public double operator_latitude { get; set; } = 0;
        public double operator_longitude { get; set; } = 0;
        public float operator_altitude_geo { get; set; } = 0;

        public long since_last_msg_ms { get; set; }

        int count;
        private DateTime last_ext_send_s = DateTime.MinValue;
        private DateTime last_update_send_s = DateTime.MinValue;
        private Timer timer;
        private bool running;

        public void Start(MAVLinkInterface mav, byte sysid, byte compid)
        {
            _mav = mav;
            target_system = sysid;
            target_component = (byte) MAVLink.MAV_COMPONENT.MAV_COMP_ID_ALL; // compid;

            running = true;
            timer = new Timer(Send, this, TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100));
        }

        public void Stop()
        {
            if (timer != null) timer.Stop();
            running = false;
        }

        public bool isRunning()
        {
            return running;
        }

        internal void Send(object self)
        {
            running = true;
            if (!Send_Update())
            {
                Send_Extended();
            }
            
        }

        public void Send_Extended()
        {
            var now = DateTime.Now;
            if ((now - last_ext_send_s).TotalSeconds > (1.0 / (rate_hz*4)) )
            {
                last_ext_send_s = now;

                switch (count++ % 4)
                {
                    case 0:
                    {
                        send_basic_id();
                        break;
                    }

                    case 1:
                    {
                        send_id_system();
                        break;
                    }

                    case 2:
                    {
                        send_self_id();
                        break;
                    }

                    case 3:
                    {
                        send_operator_id();
                        break;
                    }
                }
            }
        }

        public bool Send_Update()
        {
            var now = DateTime.Now;
            
            // system update doesn't send if gps times out

            if ((since_last_msg_ms < _gps_timeout_ms) && (now - last_update_send_s).TotalSeconds > (1.0 / system_updaterate_hz))
            {
                last_update_send_s = now;
                send_system_update();
                return true;
            }
                return false;
        }

        public void send_basic_id()
        {
            var basic_id =
                MAVLink.mavlink_open_drone_id_basic_id_t.PopulateXMLOrder(target_system,
                    target_component,
                    id_or_mac(),
                    (byte)UAS_ID_type,
                    (byte)UA_type,
                    UAS_ID.MakeBytesSize(20));

            _mav.sendPacket(basic_id, target_system, target_component);
        }

        public void send_id_system()
        {
            // To meet compliance, sends 0's for lat/lng/alt if timeout

            var id_system = MAVLink.mavlink_open_drone_id_system_t.PopulateXMLOrder(target_system,
                target_component,
                id_or_mac(),
                (byte)operator_location_type,
                (byte)classification_type,
                (since_last_msg_ms < _gps_timeout_ms) ? (int) (operator_latitude * 1.0e7): 0,
                (since_last_msg_ms < _gps_timeout_ms) ? (int) (operator_longitude * 1.0e7): 0,
                area_count,
                area_radius,
                area_ceiling,
                area_floor,
                (byte)category_eu,
                (byte)class_eu,
                (since_last_msg_ms < _gps_timeout_ms) ? operator_altitude_geo:0.0f,
                timestamp_2019());
            _mav.sendPacket(id_system, target_system, target_component);
        }

        public void send_system_update()
        {
            
            var id_system = MAVLink.mavlink_open_drone_id_system_update_t.PopulateXMLOrder(target_system,
                target_component,
                (int)(operator_latitude * 1.0e7),
                (int)(operator_longitude * 1.0e7),
                operator_altitude_geo,
                timestamp_2019());
            _mav.sendPacket(id_system, target_system, target_component);
        }

        public void send_self_id()
        {
            var self_id = MAVLink.mavlink_open_drone_id_self_id_t.PopulateXMLOrder(target_system,
                target_component,
                id_or_mac(),
                (byte)description_type,
                to_string(description, 23));
            _mav.sendPacket(self_id, target_system, target_component);
        }

        private byte[] to_string(string s, int i)
        {
            return s.MakeBytesSize(i);
        }

        public void send_operator_id()
        {
            var operator_id_pkt = MAVLink.mavlink_open_drone_id_operator_id_t.PopulateXMLOrder(target_system,
                target_component,
                id_or_mac(),
                (byte)operator_id_type,
                to_string(operator_id, 20));
            _mav.sendPacket(operator_id_pkt, target_system, target_component);
        }

        public byte[] id_or_mac()
        {
            return "".PadRight(20).MakeBytes();
        }

        public uint timestamp_2019()
        {
            var jan_1_2019_s = 1546300800;
            return (uint) (DateTime.UtcNow.toUnixTimeDouble() - jan_1_2019_s);
        }
    }
}
