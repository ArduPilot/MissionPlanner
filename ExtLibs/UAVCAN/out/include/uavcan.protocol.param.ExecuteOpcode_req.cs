
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


public const int UAVCAN_PROTOCOL_PARAM_EXECUTEOPCODE_REQ_MAX_PACK_SIZE = 7;
public const ulong UAVCAN_PROTOCOL_PARAM_EXECUTEOPCODE_REQ_DT_SIG = 0x3B131AC5EB69D2CD;
public const int UAVCAN_PROTOCOL_PARAM_EXECUTEOPCODE_REQ_DT_ID = 10;



public const double UAVCAN_PROTOCOL_PARAM_EXECUTEOPCODE_REQ_OPCODE_SAVE = 0; // saturated uint8
public const double UAVCAN_PROTOCOL_PARAM_EXECUTEOPCODE_REQ_OPCODE_ERASE = 1; // saturated uint8

public class uavcan_protocol_param_ExecuteOpcode_req: IUAVCANSerialize {
    public uint8_t opcode = new uint8_t();
    public int64_t argument = new int64_t();

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_protocol_param_ExecuteOpcode_req(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_protocol_param_ExecuteOpcode_req(transfer, this);
}

};

}
}