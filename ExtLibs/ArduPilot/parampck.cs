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
        static Dictionary<int, (int type_len, char type_format)> map = new Dictionary<int, (int type_len, char type_format)>()
        {
            { 1, (1, 'b') },
            { 2, (2, 'h') },
            { 3, (4, 'i') },
            { 4, (4, 'f') },
        };

        public static MAVLink.MAVLinkParamList unpack(byte[] data)
        {
            MAVLink.MAVLinkParamList list = new MAVLink.MAVLinkParamList();

            if (data.Length < 6)
                return null;

            int dataPointer = 0;

            var magic2 = BitConverter.ToUInt16(data, dataPointer);
            var num_params = BitConverter.ToUInt16(data, dataPointer+=2);
            var total_params = BitConverter.ToUInt16(data, dataPointer+=2);
            dataPointer+=2;

            if (magic2 != magic)
                return null;

            byte pad_byte = 0;
            int count = 0;
            var last_name = "";
            while (true)
            {
                while (data.Length - dataPointer > 0 && data[dataPointer] == pad_byte)
                    dataPointer++;

                if (data.Length - dataPointer == 0)
                    break;

                var ptype = data[dataPointer++];
                var plen = data[dataPointer++];
                // var flags = (ptype >> 4) & 0x0F;
                ptype &= 0x0F;

                if (!map.TryGetValue(ptype, out (int type_len, char type_format) mapped))
                    return null;

                var name_len = ((plen >> 4) & 0x0F) + 1;
                var common_len = plen & 0x0F;

                var nameBuilder = new StringBuilder().Append(last_name.Substring(0, common_len));

                for (int i = dataPointer; i < dataPointer + name_len; i++)
                    nameBuilder.Append((char)data[i]);

                dataPointer += name_len;

                var name = nameBuilder.ToString();
                
                last_name = name;
                var v = decode_value(mapped.type_format, data, dataPointer);
                
                count += 1;
                log.DebugFormat("{0,-16} {1,-16} {2,-16} {3,-16}", name, v, mapped.type_len, mapped.type_format);
                //print("%-16s %f" % (name, float (v)))

                var vdata = new byte[4];
                Array.Copy(data, dataPointer, vdata, 0, mapped.type_len);
                dataPointer += mapped.type_len;

                list.Add(new MAVLink.MAVLinkParam(name, vdata,
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

        private static object decode_value(char type_format, byte[] data, int startIndex)
        {
            switch (type_format)
            {
                case 'b': return data[startIndex];
                case 'h': return BitConverter.ToUInt16(data, startIndex);
                case 'i': return BitConverter.ToUInt32(data, startIndex);
                case 'f': return BitConverter.ToSingle(data, startIndex);

                default:
                    throw new Exception($"Unexpected type_format: {type_format}");
            }
        }
    }
}