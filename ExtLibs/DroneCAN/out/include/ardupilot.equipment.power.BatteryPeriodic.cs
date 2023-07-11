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
        public partial class ardupilot_equipment_power_BatteryPeriodic: IDroneCANSerialize 
        {
            public const int ARDUPILOT_EQUIPMENT_POWER_BATTERYPERIODIC_MAX_PACK_SIZE = 125;
            public const ulong ARDUPILOT_EQUIPMENT_POWER_BATTERYPERIODIC_DT_SIG = 0xF012494E97358D2;
            public const int ARDUPILOT_EQUIPMENT_POWER_BATTERYPERIODIC_DT_ID = 20011;

            public uint8_t name_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=50)] public uint8_t[] name = Enumerable.Range(1, 50).Select(i => new uint8_t()).ToArray();
            public uint8_t serial_number_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=32)] public uint8_t[] serial_number = Enumerable.Range(1, 32).Select(i => new uint8_t()).ToArray();
            public uint8_t manufacture_date_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=9)] public uint8_t[] manufacture_date = Enumerable.Range(1, 9).Select(i => new uint8_t()).ToArray();
            public Single design_capacity = new Single();
            public uint8_t cells_in_series = new uint8_t();
            public Single nominal_voltage = new Single();
            public Single discharge_minimum_voltage = new Single();
            public Single charging_minimum_voltage = new Single();
            public Single charging_maximum_voltage = new Single();
            public Single charging_maximum_current = new Single();
            public Single discharge_maximum_current = new Single();
            public Single discharge_maximum_burst_current = new Single();
            public Single full_charge_capacity = new Single();
            public uint16_t cycle_count = new uint16_t();
            public uint8_t state_of_health = new uint8_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_ardupilot_equipment_power_BatteryPeriodic(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_ardupilot_equipment_power_BatteryPeriodic(transfer, this, fdcan);
            }

            public static ardupilot_equipment_power_BatteryPeriodic ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new ardupilot_equipment_power_BatteryPeriodic();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}