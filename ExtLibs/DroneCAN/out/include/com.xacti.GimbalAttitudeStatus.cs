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
        public partial class com_xacti_GimbalAttitudeStatus: IDroneCANSerialize 
        {
            public const int COM_XACTI_GIMBALATTITUDESTATUS_MAX_PACK_SIZE = 12;
            public const ulong COM_XACTI_GIMBALATTITUDESTATUS_DT_SIG = 0xEB428B6C25832692;
            public const int COM_XACTI_GIMBALATTITUDESTATUS_DT_ID = 20402;

            public int16_t gimbal_roll = new int16_t();
            public int16_t gimbal_pitch = new int16_t();
            public int16_t gimbal_yaw = new int16_t();
            public int16_t magneticencoder_roll = new int16_t();
            public int16_t magneticencoder_pitch = new int16_t();
            public int16_t magneticencoder_yaw = new int16_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_com_xacti_GimbalAttitudeStatus(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_com_xacti_GimbalAttitudeStatus(transfer, this, fdcan);
            }

            public static com_xacti_GimbalAttitudeStatus ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new com_xacti_GimbalAttitudeStatus();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}