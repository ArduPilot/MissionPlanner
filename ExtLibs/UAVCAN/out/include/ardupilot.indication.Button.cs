
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


public const int ARDUPILOT_INDICATION_BUTTON_MAX_PACK_SIZE = 2;
public const ulong ARDUPILOT_INDICATION_BUTTON_DT_SIG = 0x645A46EFBA7466E;
public const int ARDUPILOT_INDICATION_BUTTON_DT_ID = 20001;



public const double ARDUPILOT_INDICATION_BUTTON_BUTTON_SAFETY = 1; // saturated uint8

public class ardupilot_indication_Button: IUAVCANSerialize {
    public uint8_t button = new uint8_t();
    public uint8_t press_time = new uint8_t();

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_ardupilot_indication_Button(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_ardupilot_indication_Button(transfer, this);
}

};

}
}