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
        public partial class ardupilot_equipment_power_BatteryContinuous: IDroneCANSerialize 
        {
            public const int ARDUPILOT_EQUIPMENT_POWER_BATTERYCONTINUOUS_MAX_PACK_SIZE = 25;
            public const ulong ARDUPILOT_EQUIPMENT_POWER_BATTERYCONTINUOUS_DT_SIG = 0x756B561340D5E4AE;
            public const int ARDUPILOT_EQUIPMENT_POWER_BATTERYCONTINUOUS_DT_ID = 20010;

            public const double ARDUPILOT_EQUIPMENT_POWER_BATTERYCONTINUOUS_STATUS_FLAG_READY_TO_USE = 1; // saturated uint32
            public const double ARDUPILOT_EQUIPMENT_POWER_BATTERYCONTINUOUS_STATUS_FLAG_CHARGING = 2; // saturated uint32
            public const double ARDUPILOT_EQUIPMENT_POWER_BATTERYCONTINUOUS_STATUS_FLAG_CELL_BALANCING = 4; // saturated uint32
            public const double ARDUPILOT_EQUIPMENT_POWER_BATTERYCONTINUOUS_STATUS_FLAG_FAULT_CELL_IMBALANCE = 8; // saturated uint32
            public const double ARDUPILOT_EQUIPMENT_POWER_BATTERYCONTINUOUS_STATUS_FLAG_AUTO_DISCHARGING = 16; // saturated uint32
            public const double ARDUPILOT_EQUIPMENT_POWER_BATTERYCONTINUOUS_STATUS_FLAG_REQUIRES_SERVICE = 32; // saturated uint32
            public const double ARDUPILOT_EQUIPMENT_POWER_BATTERYCONTINUOUS_STATUS_FLAG_BAD_BATTERY = 64; // saturated uint32
            public const double ARDUPILOT_EQUIPMENT_POWER_BATTERYCONTINUOUS_STATUS_FLAG_PROTECTIONS_ENABLED = 128; // saturated uint32
            public const double ARDUPILOT_EQUIPMENT_POWER_BATTERYCONTINUOUS_STATUS_FLAG_FAULT_PROTECTION_SYSTEM = 256; // saturated uint32
            public const double ARDUPILOT_EQUIPMENT_POWER_BATTERYCONTINUOUS_STATUS_FLAG_FAULT_OVER_VOLT = 512; // saturated uint32
            public const double ARDUPILOT_EQUIPMENT_POWER_BATTERYCONTINUOUS_STATUS_FLAG_FAULT_UNDER_VOLT = 1024; // saturated uint32
            public const double ARDUPILOT_EQUIPMENT_POWER_BATTERYCONTINUOUS_STATUS_FLAG_FAULT_OVER_TEMP = 2048; // saturated uint32
            public const double ARDUPILOT_EQUIPMENT_POWER_BATTERYCONTINUOUS_STATUS_FLAG_FAULT_UNDER_TEMP = 4096; // saturated uint32
            public const double ARDUPILOT_EQUIPMENT_POWER_BATTERYCONTINUOUS_STATUS_FLAG_FAULT_OVER_CURRENT = 8192; // saturated uint32
            public const double ARDUPILOT_EQUIPMENT_POWER_BATTERYCONTINUOUS_STATUS_FLAG_FAULT_SHORT_CIRCUIT = 16384; // saturated uint32
            public const double ARDUPILOT_EQUIPMENT_POWER_BATTERYCONTINUOUS_STATUS_FLAG_FAULT_INCOMPATIBLE_VOLTAGE = 32768; // saturated uint32
            public const double ARDUPILOT_EQUIPMENT_POWER_BATTERYCONTINUOUS_STATUS_FLAG_FAULT_INCOMPATIBLE_FIRMWARE = 65536; // saturated uint32
            public const double ARDUPILOT_EQUIPMENT_POWER_BATTERYCONTINUOUS_STATUS_FLAG_FAULT_INCOMPATIBLE_CELLS_CONFIGURATION = 131072; // saturated uint32
            public const double ARDUPILOT_EQUIPMENT_POWER_BATTERYCONTINUOUS_STATUS_FLAG_CAPACITY_RELATIVE_TO_FULL = 262144; // saturated uint32

            public Single temperature_cells = new Single();
            public Single temperature_pcb = new Single();
            public Single temperature_other = new Single();
            public Single current = new Single();
            public Single voltage = new Single();
            public Single state_of_charge = new Single();
            public uint8_t slot_id = new uint8_t();
            public Single capacity_consumed = new Single();
            public uint32_t status_flags = new uint32_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_ardupilot_equipment_power_BatteryContinuous(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_ardupilot_equipment_power_BatteryContinuous(transfer, this, fdcan);
            }

            public static ardupilot_equipment_power_BatteryContinuous ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new ardupilot_equipment_power_BatteryContinuous();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}