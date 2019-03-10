

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



//using org.cubepilot.uwb.Node.cs

//using org.cubepilot.uwb.ReceiveTimestamp.cs


public const int ORG_CUBEPILOT_UWB_OBSERVATION_MAX_PACK_SIZE = 250;
public const ulong ORG_CUBEPILOT_UWB_OBSERVATION_DT_SIG = 0x817EABC2996B0D62;

public const int ORG_CUBEPILOT_UWB_OBSERVATION_DT_ID = 20759;






public class org_cubepilot_uwb_Observation: IUAVCANSerialize {



    public uint64_t timestamp_us = new uint64_t();



    public org_cubepilot_uwb_Node tx_node = new org_cubepilot_uwb_Node();



    public uint64_t tx_timestamp = new uint64_t();



    public uint8_t rx_timestamps_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=10)] public org_cubepilot_uwb_ReceiveTimestamp[] rx_timestamps = new org_cubepilot_uwb_ReceiveTimestamp[10];




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_org_cubepilot_uwb_Observation(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_org_cubepilot_uwb_Observation(transfer, this);
}

};

}
}