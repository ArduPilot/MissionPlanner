
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
        public partial class mppt_Stream: IDroneCANSerialize 
        {
            public const int MPPT_STREAM_MAX_PACK_SIZE = 14;
            public const ulong MPPT_STREAM_DT_SIG = 0xDD7096B255FB6358;
            public const int MPPT_STREAM_DT_ID = 20020;

            public const double MPPT_STREAM_OV_FAULT = 1; // saturated uint8
            public const double MPPT_STREAM_UV_FAULT = 2; // saturated uint8
            public const double MPPT_STREAM_OC_FAULT = 4; // saturated uint8
            public const double MPPT_STREAM_OT_FAULT = 8; // saturated uint8

            public uint8_t fault_flags = new uint8_t();
            public int8_t temperature = new int8_t();
            public Single input_voltage = new Single();
            public Single input_current = new Single();
            public Single input_power = new Single();
            public Single output_voltage = new Single();
            public Single output_current = new Single();
            public Single output_power = new Single();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_mppt_Stream(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_mppt_Stream(transfer, this);
            }

            public static mppt_Stream ByteArrayToDroneCANMsg(byte[] transfer, int startoffset)
            {
                var ans = new mppt_Stream();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
                return ans;
            }
        }
    }
}