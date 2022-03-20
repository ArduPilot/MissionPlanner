
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
        public partial class ardupilot_indication_NotifyState: IDroneCANSerialize 
        {
            public const int ARDUPILOT_INDICATION_NOTIFYSTATE_MAX_PACK_SIZE = 265;
            public const ulong ARDUPILOT_INDICATION_NOTIFYSTATE_DT_SIG = 0x631F2A9C1651FDEC;
            public const int ARDUPILOT_INDICATION_NOTIFYSTATE_DT_ID = 20007;

            public const double ARDUPILOT_INDICATION_NOTIFYSTATE_VEHICLE_STATE_INITIALISING = 0; // saturated uint8
            public const double ARDUPILOT_INDICATION_NOTIFYSTATE_VEHICLE_STATE_ARMED = 1; // saturated uint8
            public const double ARDUPILOT_INDICATION_NOTIFYSTATE_VEHICLE_STATE_FLYING = 2; // saturated uint8
            public const double ARDUPILOT_INDICATION_NOTIFYSTATE_VEHICLE_STATE_PREARM = 3; // saturated uint8
            public const double ARDUPILOT_INDICATION_NOTIFYSTATE_VEHICLE_STATE_PREARM_GPS = 4; // saturated uint8
            public const double ARDUPILOT_INDICATION_NOTIFYSTATE_VEHICLE_STATE_SAVE_TRIM = 5; // saturated uint8
            public const double ARDUPILOT_INDICATION_NOTIFYSTATE_VEHICLE_STATE_ESC_CALIBRATION = 6; // saturated uint8
            public const double ARDUPILOT_INDICATION_NOTIFYSTATE_VEHICLE_STATE_FAILSAFE_RADIO = 7; // saturated uint8
            public const double ARDUPILOT_INDICATION_NOTIFYSTATE_VEHICLE_STATE_FAILSAFE_BATT = 8; // saturated uint8
            public const double ARDUPILOT_INDICATION_NOTIFYSTATE_VEHICLE_STATE_FAILSAFE_GCS = 9; // saturated uint8
            public const double ARDUPILOT_INDICATION_NOTIFYSTATE_VEHICLE_STATE_CHUTE_RELEASED = 10; // saturated uint8
            public const double ARDUPILOT_INDICATION_NOTIFYSTATE_VEHICLE_STATE_EKF_BAD = 11; // saturated uint8
            public const double ARDUPILOT_INDICATION_NOTIFYSTATE_VEHICLE_STATE_FW_UPDATE = 12; // saturated uint8
            public const double ARDUPILOT_INDICATION_NOTIFYSTATE_VEHICLE_STATE_MAGCAL_RUN = 13; // saturated uint8
            public const double ARDUPILOT_INDICATION_NOTIFYSTATE_VEHICLE_STATE_LEAK_DET = 14; // saturated uint8
            public const double ARDUPILOT_INDICATION_NOTIFYSTATE_VEHICLE_STATE_GPS_FUSION = 15; // saturated uint8
            public const double ARDUPILOT_INDICATION_NOTIFYSTATE_VEHICLE_STATE_GPS_GLITCH = 16; // saturated uint8
            public const double ARDUPILOT_INDICATION_NOTIFYSTATE_VEHICLE_STATE_POS_ABS_AVAIL = 17; // saturated uint8
            public const double ARDUPILOT_INDICATION_NOTIFYSTATE_VEHICLE_STATE_LOST = 18; // saturated uint8
            public const double ARDUPILOT_INDICATION_NOTIFYSTATE_VEHICLE_STATE_THROW_READY = 19; // saturated uint8
            public const double ARDUPILOT_INDICATION_NOTIFYSTATE_VEHICLE_STATE_POWERING_OFF = 20; // saturated uint8
            public const double ARDUPILOT_INDICATION_NOTIFYSTATE_VEHICLE_STATE_VIDEO_RECORDING = 21; // saturated uint8
            public const double ARDUPILOT_INDICATION_NOTIFYSTATE_VEHICLE_YAW_EARTH_CENTIDEGREES = 0; // saturated uint8

            public uint8_t aux_data_type = new uint8_t();
            public uint8_t aux_data_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=255)] public uint8_t[] aux_data = Enumerable.Range(1, 255).Select(i => new uint8_t()).ToArray();
            public uint64_t vehicle_state = new uint64_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_ardupilot_indication_NotifyState(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_ardupilot_indication_NotifyState(transfer, this);
            }

            public static ardupilot_indication_NotifyState ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new ardupilot_indication_NotifyState();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}