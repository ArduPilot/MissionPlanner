using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using log4net;
using MissionPlanner.Utilities;

namespace MissionPlanner.ArduPilot
{
    public static class parampck
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static readonly int magic = 0x671b;
        /*
          packed format:
            file header:
              uint16_t magic = 0x671b
              uint16_t num_params
              uint16_t total_params
            per-parameter:
            uint8_t type:4;         // AP_Param type NONE=0, INT8=1, INT16=2, INT32=3, FLOAT=4
            uint8_t flags:4;        // for future use
            uint8_t common_len:4;   // number of name bytes in common with previous entry, 0..15
            uint8_t name_len:4;     // non-common length of param name -1 (0..15)
            uint8_t name[name_len]; // name
            uint8_t data[];         // value, length given by variable type
            Any leading zero bytes after the header should be discarded as pad
            bytes. Pad bytes are used to ensure that a parameter data[] field
            does not cross a read packet boundary
        */
        static Dictionary<int, (int size, char type)> map = new Dictionary<int, (int size, char type)>()
        {
            { 1, (1, 'b') },
            { 2, (2, 'h') },
            {3, (4, 'i')},
            {4, (4, 'f')},
        };

        public static MAVLink.MAVLinkParamList unpack(byte[] data)
        {
            MAVLink.MAVLinkParamList list = new MAVLink.MAVLinkParamList();

            if (data.Length < 6)
                return null;

            var magic2 = BitConverter.ToUInt16(data.Take(6).ToArray(), 0);
            var num_params = BitConverter.ToUInt16(data.Take(6).ToArray(), 2);
            var total_params = BitConverter.ToUInt16(data.Take(6).ToArray(), 4);

            if (magic2 != magic)
                return null;

            data = data.Skip(6).ToArray();

            byte pad_byte = 0;
            int count = 0;
            var last_name = "";
            while (true)
            {
                while (len(data) > 0 && ord(data[0]) == pad_byte)
                    data = data.Skip(1).ToArray();

                if (len(data) == 0)
                    break;

                var ptype = data[0];
                var plen = data[1];
                var flags = (ptype >> 4) & 0x0F;
                ptype &= 0x0F;

                if (!map.ContainsKey(ptype))
                    return null;

                var (type_len, type_format) = map[ptype];

                var name_len = ((plen >> 4) & 0x0F) + 1;
                var common_len = (plen & 0x0F);
                var name = new StringBuilder().Append(last_name.Take(common_len).ToArray())
                    .Append(data.Skip(2).Take(name_len).Select(a => (char)a).ToArray()).ToString();
                var vdata = data.Skip(2 + name_len).Take(type_len);
                last_name = name;
                data = data.Skip(2 + name_len + type_len).ToArray();
                var v = decode_value(ptype, vdata);
                count += 1;
                log.DebugFormat("{0,-16} {1,-16} {2,-16} {3,-16}", name, v, type_len, type_format);
                //print("%-16s %f" % (name, float (v)))

                list.Add(new MAVLink.MAVLinkParam(name, vdata.ToArray().MakeSize(4),
                    (ptype == 1 ? MAVLink.MAV_PARAM_TYPE.INT8 :
                        ptype == 2 ? MAVLink.MAV_PARAM_TYPE.INT16 :
                        ptype == 3 ? MAVLink.MAV_PARAM_TYPE.INT32 :
                        ptype == 4 ? MAVLink.MAV_PARAM_TYPE.REAL32 : (MAVLink.MAV_PARAM_TYPE)0),
                    (ptype == 1 ? MAVLink.MAV_PARAM_TYPE.INT8 :
                        ptype == 2 ? MAVLink.MAV_PARAM_TYPE.INT16 :
                        ptype == 3 ? MAVLink.MAV_PARAM_TYPE.INT32 :
                        ptype == 4 ? MAVLink.MAV_PARAM_TYPE.REAL32 : (MAVLink.MAV_PARAM_TYPE)0)));
            }

            if (count != num_params || count > total_params)
                return null;

            return list;
        }

        private static object decode_value(byte ptype, IEnumerable<byte> vdata)
        {
            if (ptype == 1)
            {
                vdata = pad_data(vdata, 1);
                return vdata.First();
            }

            if (ptype == 2)
            {
                vdata = pad_data(vdata, 2);
                return BitConverter.ToUInt16(vdata.ToArray(), 0);
            }

            if (ptype == 3)
            {
                vdata = pad_data(vdata, 4);
                return BitConverter.ToUInt32(vdata.ToArray(), 0);
            }

            if (ptype == 4)
            {
                vdata = pad_data(vdata, 4);
                return BitConverter.ToSingle(vdata.ToArray(), 0);
            }

            //print("bad ptype %u" % ptype)
            return 0;
        }

        private static byte[] pad_data(IEnumerable<byte> vdata, int vlen)
        {
            var ans = vdata.ToList();
            while (len(ans) < vlen)
                ans.Add(0);
            return ans.ToArray();
        }

        private static int ord(byte b)
        {
            return b;
        }

        private static int len(IEnumerable<byte> data)
        {
            return data.Count();
        }
    }
}