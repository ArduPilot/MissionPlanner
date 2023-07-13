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
        public partial class ardupilot_indication_SafetyState: IDroneCANSerialize 
        {
            public const int ARDUPILOT_INDICATION_SAFETYSTATE_MAX_PACK_SIZE = 1;
            public const ulong ARDUPILOT_INDICATION_SAFETYSTATE_DT_SIG = 0xE965701A95A1A6A1;
            public const int ARDUPILOT_INDICATION_SAFETYSTATE_DT_ID = 20000;

            public const double ARDUPILOT_INDICATION_SAFETYSTATE_STATUS_SAFETY_ON = 0; // saturated uint8
            public const double ARDUPILOT_INDICATION_SAFETYSTATE_STATUS_SAFETY_OFF = 255; // saturated uint8

            public uint8_t status = new uint8_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_ardupilot_indication_SafetyState(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_ardupilot_indication_SafetyState(transfer, this, fdcan);
            }

            public static ardupilot_indication_SafetyState ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new ardupilot_indication_SafetyState();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}