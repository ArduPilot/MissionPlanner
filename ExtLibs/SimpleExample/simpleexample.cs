using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleExample
{
    public partial class simpleexample : Form
    {
        MAVLink.MavlinkParse mavlink = new MAVLink.MavlinkParse();
        bool armed = false;

        public simpleexample()
        {
            InitializeComponent();
        }

        private void but_connect_Click(object sender, EventArgs e)
        {
            // if the port is open close it
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                return;
            }

            // set the comport options
            serialPort1.PortName = CMB_comport.Text;
            serialPort1.BaudRate = int.Parse(cmb_baudrate.Text);

            // open the comport
            serialPort1.Open();

            // set timeout to 2 seconds
            serialPort1.ReadTimeout = 2000;

            // request streams - asume target is at 1,1
            mavlink.GenerateMAVLinkPacket(MAVLink.MAVLINK_MSG_ID.REQUEST_DATA_STREAM,
                new MAVLink.mavlink_request_data_stream_t()
                {
                    req_message_rate = 2,
                    req_stream_id = (byte)MAVLink.MAV_DATA_STREAM.ALL, 
                    start_stop = 1,
                    target_component = 1,
                    target_system = 1
                });

            while (serialPort1.IsOpen)
            {
                try
                {
                    // try read a hb packet from the comport
                    var hb = readsomedata<MAVLink.mavlink_heartbeat_t>();

                    var att = readsomedata<MAVLink.mavlink_attitude_t>();

                    Console.WriteLine(att.pitch*57.2958 + " " + att.roll*57.2958);
                }
                catch
                {
                }

                System.Threading.Thread.Sleep(1);
                Application.DoEvents();

            }
        }

        T readsomedata<T>(int timeout = 2000)
        {
            DateTime deadline = DateTime.Now.AddMilliseconds(timeout);

            // read the current buffered bytes
            while (DateTime.Now < deadline)
            {
                var packet = mavlink.ReadPacketObj(serialPort1.BaseStream);

                if (packet == null)
                    continue;

                Console.WriteLine(packet);

                if (packet.GetType() == typeof(T))
                {
                    return (T)packet;
                }
            }

            throw new Exception("No packet match found");
        }

        private void but_armdisarm_Click(object sender, EventArgs e)
        {
            MAVLink.mavlink_command_long_t req = new MAVLink.mavlink_command_long_t();

            req.target_system = 1;
            req.target_component = 1;

            req.command = (ushort)MAVLink.MAV_CMD.COMPONENT_ARM_DISARM;

            req.param1 = armed ? 0 : 1;
            armed = !armed;
            /*
            req.param2 = p2;
            req.param3 = p3;
            req.param4 = p4;
            req.param5 = p5;
            req.param6 = p6;
            req.param7 = p7;
            */

            byte[] packet = mavlink.GenerateMAVLinkPacket(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, req);

            serialPort1.Write(packet, 0, packet.Length);

            try
            {
                var ack = readsomedata<MAVLink.mavlink_command_ack_t>();
                if (ack.result == (byte)MAVLink.MAV_RESULT.ACCEPTED) 
                {

                }
            }
            catch 
            { 
            }
        }

        private void CMB_comport_Click(object sender, EventArgs e)
        {
            CMB_comport.DataSource = SerialPort.GetPortNames();
        }
    }
}
