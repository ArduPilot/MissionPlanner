

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



//using uavcan.protocol.param.NumericValue.cs

//using uavcan.protocol.param.Value.cs


public const int UAVCAN_PROTOCOL_PARAM_GETSET_RES_MAX_PACK_SIZE = 371;
public const ulong UAVCAN_PROTOCOL_PARAM_GETSET_RES_DT_SIG = 0xA7B622F939D1A4D5;

public const int UAVCAN_PROTOCOL_PARAM_GETSET_RES_DT_ID = 11;






public class uavcan_protocol_param_GetSet_res: IUAVCANSerialize {





    public uavcan_protocol_param_Value value = new uavcan_protocol_param_Value();





    public uavcan_protocol_param_Value default_value = new uavcan_protocol_param_Value();





    public uavcan_protocol_param_NumericValue max_value = new uavcan_protocol_param_NumericValue();





    public uavcan_protocol_param_NumericValue min_value = new uavcan_protocol_param_NumericValue();



    public uint8_t name_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=92)] public uint8_t[] name = new uint8_t[92];




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_protocol_param_GetSet_res(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_protocol_param_GetSet_res(transfer, this);
}

};

}
}