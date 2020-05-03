
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


public const int UAVCAN_OLLIW_UC4H_DISTANCESENSORPROPERTIES_MAX_PACK_SIZE = 8;
public const ulong UAVCAN_OLLIW_UC4H_DISTANCESENSORPROPERTIES_DT_SIG = 0x7FC8D57D4F2EF1D1;



public class uavcan_olliw_uc4h_DistanceSensorProperties: IUAVCANSerialize {
    public Single range_min = new Single();
    public Single range_max = new Single();
    public Single vertical_field_of_view = new Single();
    public Single horizontal_field_of_view = new Single();

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_olliw_uc4h_DistanceSensorProperties(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_olliw_uc4h_DistanceSensorProperties(transfer, this);
}

};

}
}