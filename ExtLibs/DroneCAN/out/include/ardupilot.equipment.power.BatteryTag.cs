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
        public partial class ardupilot_equipment_power_BatteryTag: IDroneCANSerialize 
        {
            public const int ARDUPILOT_EQUIPMENT_POWER_BATTERYTAG_MAX_PACK_SIZE = 24;
            public const ulong ARDUPILOT_EQUIPMENT_POWER_BATTERYTAG_DT_SIG = 0x4A5A9B42099F73E1;
            public const int ARDUPILOT_EQUIPMENT_POWER_BATTERYTAG_DT_ID = 20500;

            public uint32_t serial_number = new uint32_t();
            public uint32_t num_cycles = new uint32_t();
            public Single armed_hours = new Single();
            public uint32_t battery_capacity_mAh = new uint32_t();
            public uint32_t first_use_mins = new uint32_t();
            public uint32_t last_arm_time_mins = new uint32_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_ardupilot_equipment_power_BatteryTag(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_ardupilot_equipment_power_BatteryTag(transfer, this, fdcan);
            }

            public static ardupilot_equipment_power_BatteryTag ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new ardupilot_equipment_power_BatteryTag();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}