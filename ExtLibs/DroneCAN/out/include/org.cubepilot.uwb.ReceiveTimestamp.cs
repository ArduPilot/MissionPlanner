
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
//using org.cubepilot.uwb.Node.cs
        public partial class org_cubepilot_uwb_ReceiveTimestamp: IDroneCANSerialize 
        {
            public const int ORG_CUBEPILOT_UWB_RECEIVETIMESTAMP_MAX_PACK_SIZE = 22;
            public const ulong ORG_CUBEPILOT_UWB_RECEIVETIMESTAMP_DT_SIG = 0xD4A5410C2517AE3D;

            public org_cubepilot_uwb_Node rx_node = new org_cubepilot_uwb_Node();
            public uint64_t rx_timestamp = new uint64_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_org_cubepilot_uwb_ReceiveTimestamp(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_org_cubepilot_uwb_ReceiveTimestamp(transfer, this);
            }

            public static org_cubepilot_uwb_ReceiveTimestamp ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new org_cubepilot_uwb_ReceiveTimestamp();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}