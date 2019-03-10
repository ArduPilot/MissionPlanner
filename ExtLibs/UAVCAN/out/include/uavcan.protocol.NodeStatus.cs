

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




public const int UAVCAN_PROTOCOL_NODESTATUS_MAX_PACK_SIZE = 7;
public const ulong UAVCAN_PROTOCOL_NODESTATUS_DT_SIG = 0xF0868D0C1A7C6F1;

public const int UAVCAN_PROTOCOL_NODESTATUS_DT_ID = 341;





public const double UAVCAN_PROTOCOL_NODESTATUS_MAX_BROADCASTING_PERIOD_MS = 1000; // saturated uint16

public const double UAVCAN_PROTOCOL_NODESTATUS_MIN_BROADCASTING_PERIOD_MS = 2; // saturated uint16

public const double UAVCAN_PROTOCOL_NODESTATUS_OFFLINE_TIMEOUT_MS = 3000; // saturated uint16

public const double UAVCAN_PROTOCOL_NODESTATUS_HEALTH_OK = 0; // saturated uint2

public const double UAVCAN_PROTOCOL_NODESTATUS_HEALTH_WARNING = 1; // saturated uint2

public const double UAVCAN_PROTOCOL_NODESTATUS_HEALTH_ERROR = 2; // saturated uint2

public const double UAVCAN_PROTOCOL_NODESTATUS_HEALTH_CRITICAL = 3; // saturated uint2

public const double UAVCAN_PROTOCOL_NODESTATUS_MODE_OPERATIONAL = 0; // saturated uint3

public const double UAVCAN_PROTOCOL_NODESTATUS_MODE_INITIALIZATION = 1; // saturated uint3

public const double UAVCAN_PROTOCOL_NODESTATUS_MODE_MAINTENANCE = 2; // saturated uint3

public const double UAVCAN_PROTOCOL_NODESTATUS_MODE_SOFTWARE_UPDATE = 3; // saturated uint3

public const double UAVCAN_PROTOCOL_NODESTATUS_MODE_OFFLINE = 7; // saturated uint3




public class uavcan_protocol_NodeStatus: IUAVCANSerialize {



    public uint32_t uptime_sec = new uint32_t();



    public uint8_t health = new uint8_t();



    public uint8_t mode = new uint8_t();



    public uint8_t sub_mode = new uint8_t();



    public uint16_t vendor_specific_status_code = new uint16_t();




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_protocol_NodeStatus(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_protocol_NodeStatus(transfer, this);
}

};

}
}