
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
        public partial class cuav_equipment_power_CBAT: IDroneCANSerialize 
        {
            public const int CUAV_EQUIPMENT_POWER_CBAT_MAX_PACK_SIZE = 124;
            public const ulong CUAV_EQUIPMENT_POWER_CBAT_DT_SIG = 0xB4DACE3A38E09A74;
            public const int CUAV_EQUIPMENT_POWER_CBAT_DT_ID = 20300;

            public const double CUAV_EQUIPMENT_POWER_CBAT_STATUS_FLAG_IN_USE = 1; // saturated uint11
            public const double CUAV_EQUIPMENT_POWER_CBAT_STATUS_FLAG_CHARGING = 2; // saturated uint11
            public const double CUAV_EQUIPMENT_POWER_CBAT_STATUS_FLAG_CHARGED = 4; // saturated uint11
            public const double CUAV_EQUIPMENT_POWER_CBAT_STATUS_FLAG_TEMP_HOT = 8; // saturated uint11
            public const double CUAV_EQUIPMENT_POWER_CBAT_STATUS_FLAG_TEMP_COLD = 16; // saturated uint11
            public const double CUAV_EQUIPMENT_POWER_CBAT_STATUS_FLAG_OVERLOAD = 32; // saturated uint11
            public const double CUAV_EQUIPMENT_POWER_CBAT_STATUS_FLAG_BAD_BATTERY = 64; // saturated uint11
            public const double CUAV_EQUIPMENT_POWER_CBAT_STATUS_FLAG_NEED_SERVICE = 128; // saturated uint11
            public const double CUAV_EQUIPMENT_POWER_CBAT_STATUS_FLAG_BMS_ERROR = 256; // saturated uint11
            public const double CUAV_EQUIPMENT_POWER_CBAT_STATUS_FLAG_RESERVED_A = 512; // saturated uint11
            public const double CUAV_EQUIPMENT_POWER_CBAT_STATUS_FLAG_RESERVED_B = 1024; // saturated uint11

            public Single temperature = new Single();
            public Single voltage = new Single();
            public uint8_t voltage_cell_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=15)] public Single[] voltage_cell = Enumerable.Range(1, 15).Select(i => new Single()).ToArray();
            public uint8_t cell_count = new uint8_t();
            public Single current = new Single();
            public Single average_current = new Single();
            public Single average_power = new Single();
            public Single available_energy = new Single();
            public Single remaining_capacity = new Single();
            public Single full_charge_capacity = new Single();
            public Single design_capacity = new Single();
            public uint16_t average_time_to_empty = new uint16_t();
            public uint16_t average_time_to_full = new uint16_t();
            public uint8_t state_of_health = new uint8_t();
            public uint8_t state_of_charge = new uint8_t();
            public uint8_t max_error = new uint8_t();
            public uint16_t serial_number = new uint16_t();
            public uint16_t manufacture_date = new uint16_t();
            public uint16_t cycle_count = new uint16_t();
            public uint16_t over_discharge_count = new uint16_t();
            public Single passed_charge = new Single();
            public Single nominal_voltage = new Single();
            public bool is_powering_off = new bool();
            public uint16_t interface_error = new uint16_t();
            public uint16_t status_flags = new uint16_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_cuav_equipment_power_CBAT(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_cuav_equipment_power_CBAT(transfer, this);
            }

            public static cuav_equipment_power_CBAT ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new cuav_equipment_power_CBAT();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}