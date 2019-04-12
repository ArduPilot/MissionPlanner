

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
using System.Runtime.InteropServices;

namespace UAVCAN
{
public partial class uavcan {




public const int UAVCAN_PROTOCOL_CANIFACESTATS_MAX_PACK_SIZE = 18;
public const ulong UAVCAN_PROTOCOL_CANIFACESTATS_DT_SIG = 0x13B106F0C44CA350;






public class uavcan_protocol_CANIfaceStats: IUAVCANSerialize {



    public uint64_t frames_tx = new uint64_t();



    public uint64_t frames_rx = new uint64_t();



    public uint64_t errors = new uint64_t();




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_protocol_CANIfaceStats(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_protocol_CANIfaceStats(transfer, this);
}

};

}
}