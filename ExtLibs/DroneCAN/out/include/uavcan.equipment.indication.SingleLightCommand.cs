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
//using uavcan.equipment.indication.RGB565.cs
        public partial class uavcan_equipment_indication_SingleLightCommand: IDroneCANSerialize 
        {
            public const int UAVCAN_EQUIPMENT_INDICATION_SINGLELIGHTCOMMAND_MAX_PACK_SIZE = 3;
            public const ulong UAVCAN_EQUIPMENT_INDICATION_SINGLELIGHTCOMMAND_DT_SIG = 0xE894B8B589807007;

            public const double UAVCAN_EQUIPMENT_INDICATION_SINGLELIGHTCOMMAND_LIGHT_ID_ANTI_COLLISION = 246; // saturated uint8
            public const double UAVCAN_EQUIPMENT_INDICATION_SINGLELIGHTCOMMAND_LIGHT_ID_RIGHT_OF_WAY = 247; // saturated uint8
            public const double UAVCAN_EQUIPMENT_INDICATION_SINGLELIGHTCOMMAND_LIGHT_ID_STROBE = 248; // saturated uint8
            public const double UAVCAN_EQUIPMENT_INDICATION_SINGLELIGHTCOMMAND_LIGHT_ID_WING = 249; // saturated uint8
            public const double UAVCAN_EQUIPMENT_INDICATION_SINGLELIGHTCOMMAND_LIGHT_ID_LOGO = 250; // saturated uint8
            public const double UAVCAN_EQUIPMENT_INDICATION_SINGLELIGHTCOMMAND_LIGHT_ID_TAXI = 251; // saturated uint8
            public const double UAVCAN_EQUIPMENT_INDICATION_SINGLELIGHTCOMMAND_LIGHT_ID_TURN_OFF = 252; // saturated uint8
            public const double UAVCAN_EQUIPMENT_INDICATION_SINGLELIGHTCOMMAND_LIGHT_ID_TAKE_OFF = 253; // saturated uint8
            public const double UAVCAN_EQUIPMENT_INDICATION_SINGLELIGHTCOMMAND_LIGHT_ID_LANDING = 254; // saturated uint8
            public const double UAVCAN_EQUIPMENT_INDICATION_SINGLELIGHTCOMMAND_LIGHT_ID_FORMATION = 255; // saturated uint8

            public uint8_t light_id = new uint8_t();
            public uavcan_equipment_indication_RGB565 color = new uavcan_equipment_indication_RGB565();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_uavcan_equipment_indication_SingleLightCommand(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_uavcan_equipment_indication_SingleLightCommand(transfer, this, fdcan);
            }

            public static uavcan_equipment_indication_SingleLightCommand ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new uavcan_equipment_indication_SingleLightCommand();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}