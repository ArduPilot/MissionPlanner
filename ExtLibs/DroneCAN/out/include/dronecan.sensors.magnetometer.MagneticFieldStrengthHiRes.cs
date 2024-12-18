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
        public partial class dronecan_sensors_magnetometer_MagneticFieldStrengthHiRes: IDroneCANSerialize 
        {
            public const int DRONECAN_SENSORS_MAGNETOMETER_MAGNETICFIELDSTRENGTHHIRES_MAX_PACK_SIZE = 13;
            public const ulong DRONECAN_SENSORS_MAGNETOMETER_MAGNETICFIELDSTRENGTHHIRES_DT_SIG = 0x3053EBE3D750286F;
            public const int DRONECAN_SENSORS_MAGNETOMETER_MAGNETICFIELDSTRENGTHHIRES_DT_ID = 1043;

            public uint8_t sensor_id = new uint8_t();
            [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public Single[] magnetic_field_ga = new Single[3];

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_dronecan_sensors_magnetometer_MagneticFieldStrengthHiRes(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_dronecan_sensors_magnetometer_MagneticFieldStrengthHiRes(transfer, this, fdcan);
            }

            public static dronecan_sensors_magnetometer_MagneticFieldStrengthHiRes ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new dronecan_sensors_magnetometer_MagneticFieldStrengthHiRes();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}