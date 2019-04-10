
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


public const int UAVCAN_EQUIPMENT_GNSS_RTCMSTREAM_MAX_PACK_SIZE = 130;
public const ulong UAVCAN_EQUIPMENT_GNSS_RTCMSTREAM_DT_SIG = 0x1F56030ECB171501;
public const int UAVCAN_EQUIPMENT_GNSS_RTCMSTREAM_DT_ID = 1062;



public const double UAVCAN_EQUIPMENT_GNSS_RTCMSTREAM_PROTOCOL_ID_UNKNOWN = 0; // saturated uint8
public const double UAVCAN_EQUIPMENT_GNSS_RTCMSTREAM_PROTOCOL_ID_RTCM2 = 2; // saturated uint8
public const double UAVCAN_EQUIPMENT_GNSS_RTCMSTREAM_PROTOCOL_ID_RTCM3 = 3; // saturated uint8

public class uavcan_equipment_gnss_RTCMStream: IUAVCANSerialize {
    public uint8_t protocol_id = new uint8_t();
    public uint8_t data_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=128)] public uint8_t[] data = new uint8_t[128];

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_equipment_gnss_RTCMStream(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_equipment_gnss_RTCMStream(transfer, this);
}

};

}
}