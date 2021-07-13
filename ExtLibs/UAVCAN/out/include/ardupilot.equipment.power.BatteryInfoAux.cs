
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

namespace UAVCAN
{
    public partial class uavcan {

//using uavcan.Timestamp.cs

        public const int ARDUPILOT_EQUIPMENT_POWER_BATTERYINFOAUX_MAX_PACK_SIZE = 527;
        public const ulong ARDUPILOT_EQUIPMENT_POWER_BATTERYINFOAUX_DT_SIG = 0x12A50AF430686DAA;
        public const int ARDUPILOT_EQUIPMENT_POWER_BATTERYINFOAUX_DT_ID = 20004;



        public partial class ardupilot_equipment_power_BatteryInfoAux: IUAVCANSerialize {
            public uavcan_Timestamp timestamp = new uavcan_Timestamp();
            public uint8_t voltage_cell_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=255)] public Single[] voltage_cell = Enumerable.Range(1, 255).Select(i => new Single()).ToArray();
            public uint16_t cycle_count = new uint16_t();
            public uint16_t over_discharge_count = new uint16_t();
            public Single max_current = new Single();
            public Single nominal_voltage = new Single();
            public bool is_powering_off = new bool();

            public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_ardupilot_equipment_power_BatteryInfoAux(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_ardupilot_equipment_power_BatteryInfoAux(transfer, this);
            }
        }
    }
}