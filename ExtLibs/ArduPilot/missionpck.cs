using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using log4net;
using MissionPlanner.Utilities;
using uint16_t = System.UInt16;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;
using System.IO;

namespace MissionPlanner.ArduPilot
{
    public static class missionpck
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static readonly uint16_t mission_magic = 0x763d;

        internal struct header
        {
            internal uint16_t magic;// = mission_magic;
            internal uint16_t data_type; // MAV_MISSION_TYPE_*
            internal uint16_t options; // optional features
            internal uint16_t start; // first WP num, 0 for full upload
            internal uint16_t num_items;
        };

        /// <summary>
        /// https://github.com/ArduPilot/ardupilot/blob/master/libraries/AP_Filesystem/AP_Filesystem_Mission.cpp
        /// mission.dat
        /// fence.dat
        /// rally.dat
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static (List<MAVLink.mavlink_mission_item_int_t> wps, MAVLink.MAV_MISSION_TYPE type, ushort start) unpack(byte[] data)
        {
            int dataPointer = 0;

            var header = new header();

            header.magic = BitConverter.ToUInt16(data, dataPointer);
            header.data_type = BitConverter.ToUInt16(data, dataPointer += 2);
            header.options = BitConverter.ToUInt16(data, dataPointer += 2);
            header.start = BitConverter.ToUInt16(data, dataPointer += 2);
            header.num_items = BitConverter.ToUInt16(data, dataPointer += 2);
            dataPointer += 2;

            if (header.magic != mission_magic)
                throw new Exception("Invalid Magic " + header.magic);

            MAVLink.message_info msginfo = MAVLink.MAVLINK_MESSAGE_INFOS.GetMessageInfo((int)MAVLink.MAVLINK_MSG_ID.MISSION_ITEM_INT);

            var msgs = new ReadOnlySpan<byte>(data, dataPointer, data.Length - dataPointer);

            uint8_t item_size = (byte)msginfo.length;
            ushort msgcount = (ushort)(msgs.Length / item_size);

            if (msgcount != header.num_items)
                throw new Exception("Bad Item count " + header.num_items + " vs " + msgcount);

            List<MAVLink.mavlink_mission_item_int_t> list = new List<MAVLink.mavlink_mission_item_int_t>(header.num_items);

            for (int a = 0; a < msgcount; a++)
            {
                var msg = msgs.Slice(a * item_size, item_size);
                var missionitem = MavlinkUtil.ByteArrayToStructure<MAVLink.mavlink_mission_item_int_t>(msg.ToArray(), 0);
                list.Add(missionitem);
            }

            return (list, (MAVLink.MAV_MISSION_TYPE)header.data_type, header.start);
        }

        public static byte[] pack(List<MAVLink.mavlink_mission_item_int_t> wps, MAVLink.MAV_MISSION_TYPE type, ushort start = 0)
        {
            header hdr = new header() { data_type = (ushort)type, magic = mission_magic, num_items = (ushort)wps.Count, start = start };

            var ans = new MemoryStream();
            var hdrbyte = MavlinkUtil.StructureToByteArray(hdr);
            ans.Write(hdrbyte, 0, hdrbyte.Length);

            foreach (var wp in wps)
            {
                var wpbyte = MavlinkUtil.StructureToByteArray(wp);
                ans.Write(wpbyte, 0, wpbyte.Length);
            }

            return ans.ToArray();
        }
    }
}