using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner.Utilities;

namespace MissionPlanner.ArduPilot
{
    public static class parampck
    {/*
  packed format:
    uint8_t type:4;         // AP_Param type NONE=0, INT8=1, INT16=2, INT32=3, FLOAT=4
    uint8_t type_len:4;     // number of bytes in type
    uint8_t common_len:4;   // number of name bytes in common with previous entry, 0..15
    uint8_t name_len:4;     // non-common length of param name -1 (0..15)
    uint8_t name[name_len]; // name
    uint8_t data[];         // value, may be truncated by record_length
 */
        public static MAVLink.MAVLinkParamList unpack(byte[] data)
        {
            MAVLink.MAVLinkParamList list = new MAVLink.MAVLinkParamList();

            var last_name = "";
            while (true)
            {
                while (len(data) > 0 && ord(data[0]) == 0)
                    data = data.Skip(1).ToArray();
                if (len(data) == 0)
                    break;
                var ptype = data[0];
                var plen = data[1];
                var type_len = (ptype >> 4) & 0x0F;
                ptype &= 0x0F;
                var name_len = ((plen >> 4) & 0x0F) + 1;
                var common_len = (plen & 0x0F);
                var name = new StringBuilder().Append(last_name.Take(common_len).ToArray())
                    .Append(data.Skip(2).Take(name_len).Select(a=>(char)a).ToArray()).ToString();
                var vdata = data.Skip(2 + name_len).Take(type_len);
                last_name = name;
                data = data.Skip(2 + name_len + type_len).ToArray();
                var v = decode_value(ptype, vdata);
                Console.WriteLine("{0,-16} {1}", name, v);
                //print("%-16s %f" % (name, float (v)))

                list.Add(new MAVLink.MAVLinkParam(name, vdata.ToArray().MakeSize(4),
                    (ptype == 1 ? MAVLink.MAV_PARAM_TYPE.INT8 :
                        ptype == 2 ? MAVLink.MAV_PARAM_TYPE.INT16 :
                        ptype == 3 ? MAVLink.MAV_PARAM_TYPE.INT32 :
                        ptype == 4 ? MAVLink.MAV_PARAM_TYPE.REAL32 : (MAVLink.MAV_PARAM_TYPE) 0),
                    (ptype == 1 ? MAVLink.MAV_PARAM_TYPE.INT8 :
                        ptype == 2 ? MAVLink.MAV_PARAM_TYPE.INT16 :
                        ptype == 3 ? MAVLink.MAV_PARAM_TYPE.INT32 :
                        ptype == 4 ? MAVLink.MAV_PARAM_TYPE.REAL32 : (MAVLink.MAV_PARAM_TYPE) 0)));
            }

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