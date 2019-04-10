
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


public const int UAVCAN_PROTOCOL_SOFTWAREVERSION_MAX_PACK_SIZE = 15;
public const ulong UAVCAN_PROTOCOL_SOFTWAREVERSION_DT_SIG = 0xDD46FD376527FEA1;



public const double UAVCAN_PROTOCOL_SOFTWAREVERSION_OPTIONAL_FIELD_FLAG_VCS_COMMIT = 1; // saturated uint8
public const double UAVCAN_PROTOCOL_SOFTWAREVERSION_OPTIONAL_FIELD_FLAG_IMAGE_CRC = 2; // saturated uint8

public class uavcan_protocol_SoftwareVersion: IUAVCANSerialize {
    public uint8_t major = new uint8_t();
    public uint8_t minor = new uint8_t();
    public uint8_t optional_field_flags = new uint8_t();
    public uint32_t vcs_commit = new uint32_t();
    public uint64_t image_crc = new uint64_t();

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_protocol_SoftwareVersion(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_protocol_SoftwareVersion(transfer, this);
}

};

}
}