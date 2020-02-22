
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


public const int UAVCAN_PROTOCOL_GLOBALTIMESYNC_MAX_PACK_SIZE = 7;
public const ulong UAVCAN_PROTOCOL_GLOBALTIMESYNC_DT_SIG = 0x20271116A793C2DB;
public const int UAVCAN_PROTOCOL_GLOBALTIMESYNC_DT_ID = 4;



public const double UAVCAN_PROTOCOL_GLOBALTIMESYNC_MAX_BROADCASTING_PERIOD_MS = 1100; // saturated uint16
public const double UAVCAN_PROTOCOL_GLOBALTIMESYNC_MIN_BROADCASTING_PERIOD_MS = 40; // saturated uint16
public const double UAVCAN_PROTOCOL_GLOBALTIMESYNC_RECOMMENDED_BROADCASTER_TIMEOUT_MS = 2200; // saturated uint16

public class uavcan_protocol_GlobalTimeSync: IUAVCANSerialize {
    public uint64_t previous_transmission_timestamp_usec = new uint64_t();

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_protocol_GlobalTimeSync(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_protocol_GlobalTimeSync(transfer, this);
}

};

}
}