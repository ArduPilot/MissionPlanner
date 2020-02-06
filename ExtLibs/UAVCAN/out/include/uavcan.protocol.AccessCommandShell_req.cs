
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


public const int UAVCAN_PROTOCOL_ACCESSCOMMANDSHELL_REQ_MAX_PACK_SIZE = 130;
public const ulong UAVCAN_PROTOCOL_ACCESSCOMMANDSHELL_REQ_DT_SIG = 0x59276B5921C9246E;
public const int UAVCAN_PROTOCOL_ACCESSCOMMANDSHELL_REQ_DT_ID = 6;



public const double UAVCAN_PROTOCOL_ACCESSCOMMANDSHELL_REQ_NEWLINE = 10; // saturated uint8
public const double UAVCAN_PROTOCOL_ACCESSCOMMANDSHELL_REQ_MIN_OUTPUT_LIFETIME_SEC = 10; // saturated uint8
public const double UAVCAN_PROTOCOL_ACCESSCOMMANDSHELL_REQ_FLAG_RESET_SHELL = 1; // saturated uint8
public const double UAVCAN_PROTOCOL_ACCESSCOMMANDSHELL_REQ_FLAG_CLEAR_OUTPUT_BUFFERS = 2; // saturated uint8
public const double UAVCAN_PROTOCOL_ACCESSCOMMANDSHELL_REQ_FLAG_READ_STDOUT = 64; // saturated uint8
public const double UAVCAN_PROTOCOL_ACCESSCOMMANDSHELL_REQ_FLAG_READ_STDERR = 128; // saturated uint8

public class uavcan_protocol_AccessCommandShell_req: IUAVCANSerialize {
    public uint8_t flags = new uint8_t();
    public uint8_t input_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=128)] public uint8_t[] input = new uint8_t[128];

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_protocol_AccessCommandShell_req(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_protocol_AccessCommandShell_req(transfer, this);
}

};

}
}