using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;

using int8_t = System.SByte;
using int16_t = System.Int16;
using int32_t = System.Int32;
using int64_t = System.Int64;

using float32 = System.Single;

using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace DroneCAN
{
    public partial class DroneCAN 
    {
        public partial class com_hex_equipment_flow_Measurement: IDroneCANSerialize 
        {
            public const int COM_HEX_EQUIPMENT_FLOW_MEASUREMENT_MAX_PACK_SIZE = 21;
            public const ulong COM_HEX_EQUIPMENT_FLOW_MEASUREMENT_DT_SIG = 0x6A908866BCB49C18;
            public const int COM_HEX_EQUIPMENT_FLOW_MEASUREMENT_DT_ID = 20200;

            public Single integration_interval = new Single();
            [MarshalAs(UnmanagedType.ByValArray,SizeConst=2)] public Single[] rate_gyro_integral = new Single[2];
            [MarshalAs(UnmanagedType.ByValArray,SizeConst=2)] public Single[] flow_integral = new Single[2];
            public uint8_t quality = new uint8_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_com_hex_equipment_flow_Measurement(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_com_hex_equipment_flow_Measurement(transfer, this, fdcan);
            }

            public static com_hex_equipment_flow_Measurement ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new com_hex_equipment_flow_Measurement();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}