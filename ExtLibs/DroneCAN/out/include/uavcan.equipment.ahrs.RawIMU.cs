
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
//using uavcan.Timestamp.cs
        public partial class uavcan_equipment_ahrs_RawIMU: IDroneCANSerialize 
        {
            public const int UAVCAN_EQUIPMENT_AHRS_RAWIMU_MAX_PACK_SIZE = 120;
            public const ulong UAVCAN_EQUIPMENT_AHRS_RAWIMU_DT_SIG = 0x8280632C40E574B5;
            public const int UAVCAN_EQUIPMENT_AHRS_RAWIMU_DT_ID = 1003;

            public uavcan_Timestamp timestamp = new uavcan_Timestamp();
            public Single integration_interval = new Single();
            [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public Single[] rate_gyro_latest = new Single[3];
            [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public Single[] rate_gyro_integral = new Single[3];
            [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public Single[] accelerometer_latest = new Single[3];
            [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public Single[] accelerometer_integral = new Single[3];
            public uint8_t covariance_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=36)] public Single[] covariance = Enumerable.Range(1, 36).Select(i => new Single()).ToArray();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_uavcan_equipment_ahrs_RawIMU(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_uavcan_equipment_ahrs_RawIMU(transfer, this);
            }

            public static uavcan_equipment_ahrs_RawIMU ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new uavcan_equipment_ahrs_RawIMU();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}