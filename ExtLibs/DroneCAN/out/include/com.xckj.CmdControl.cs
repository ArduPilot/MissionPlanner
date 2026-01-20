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
        public partial class com_xckj_esc_CmdControl: IDroneCANSerialize
        {
            public const int COM_XCKJ_ESC_CMDCONTROL_MAX_PACK_SIZE = 4;
            public const ulong COM_XCKJ_ESC_CMDCONTROL_DT_SIG = 0x4fdf12a8708a2030;
            public const int COM_XCKJ_ESC_CMDCONTROL_DT_ID = 20110;

            public const double COM_XCKJ_ESC_CMDCONTROL_NODECMD_DIS_ALL_UP = 0;
            public const double COM_XCKJ_ESC_CMDCONTROL_NODECMD_DIS_UP = 1;
            public const double COM_XCKJ_ESC_CMDCONTROL_NODECMD_TRIG_HB = 10;
            public const double COM_XCKJ_ESC_CMDCONTROL_NODECMD_EN_ALLUP = 100;
            public const double COM_XCKJ_ESC_CMDCONTROL_NODECMD_RST = 0xFEU;

            public uint8_t NodeCmd = new uint8_t();
            public uint8_t CmdNodeId = new uint8_t();
            public uint8_t reserved = new uint8_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_com_xckj_esc_CmdControl(this, chunk_cb, ctx, fdcan);
            } 
            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_com_xckj_esc_CmdControl(transfer, this, fdcan);
            }
            public static com_xckj_esc_CmdControl ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new com_xckj_esc_CmdControl();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }

}
