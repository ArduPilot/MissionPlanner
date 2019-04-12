

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



//using uavcan.protocol.HardwareVersion.cs

//using uavcan.protocol.NodeStatus.cs

//using uavcan.protocol.SoftwareVersion.cs


public const int UAVCAN_PROTOCOL_GETNODEINFO_RES_MAX_PACK_SIZE = 377;
public const ulong UAVCAN_PROTOCOL_GETNODEINFO_RES_DT_SIG = 0xEE468A8121C46A9E;

public const int UAVCAN_PROTOCOL_GETNODEINFO_RES_DT_ID = 1;






public class uavcan_protocol_GetNodeInfo_res: IUAVCANSerialize {



    public uavcan_protocol_NodeStatus status = new uavcan_protocol_NodeStatus();



    public uavcan_protocol_SoftwareVersion software_version = new uavcan_protocol_SoftwareVersion();



    public uavcan_protocol_HardwareVersion hardware_version = new uavcan_protocol_HardwareVersion();



    public uint8_t name_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=80)] public uint8_t[] name = new uint8_t[80];




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_protocol_GetNodeInfo_res(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_protocol_GetNodeInfo_res(transfer, this);
}

};

}
}