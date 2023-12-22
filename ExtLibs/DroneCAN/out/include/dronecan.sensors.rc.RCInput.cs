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
        public partial class dronecan_sensors_rc_RCInput: IDroneCANSerialize 
        {
            public const int DRONECAN_SENSORS_RC_RCINPUT_MAX_PACK_SIZE = 53;
            public const ulong DRONECAN_SENSORS_RC_RCINPUT_DT_SIG = 0x771555E596AAB4CF;
            public const int DRONECAN_SENSORS_RC_RCINPUT_DT_ID = 1140;

            public const double DRONECAN_SENSORS_RC_RCINPUT_STATUS_QUALITY_VALID = 1; // saturated uint8
            public const double DRONECAN_SENSORS_RC_RCINPUT_STATUS_FAILSAFE = 2; // saturated uint8

            public uint16_t status = new uint16_t();
            public uint8_t quality = new uint8_t();
            public uint8_t id = new uint8_t();
            public uint8_t rcin_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=32)] public uint16_t[] rcin = Enumerable.Range(1, 32).Select(i => new uint16_t()).ToArray();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_dronecan_sensors_rc_RCInput(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_dronecan_sensors_rc_RCInput(transfer, this, fdcan);
            }

            public static dronecan_sensors_rc_RCInput ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new dronecan_sensors_rc_RCInput();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}