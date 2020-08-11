
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
using System.Linq;
using System.Runtime.InteropServices;

namespace UAVCAN
{
public partial class uavcan {


public const int COM_HEX_EQUIPMENT_GNSS_SIGNAL_MAX_PACK_SIZE = 9;
public const ulong COM_HEX_EQUIPMENT_GNSS_SIGNAL_DT_SIG = 0x9E7881D1C8450DD;



public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_GNSS_ID_GPS = 0; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_GNSS_ID_GALILEO = 2; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_GNSS_ID_BEIDOU = 3; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_GNSS_ID_QZSS = 5; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_GNSS_ID_GLONASS = 6; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_SIG_ID_L1 = 0; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_SIG_ID_E1B = 1; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_SIG_ID_L2_OF = 2; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_SIG_ID_L2_CL = 3; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_SIG_ID_L2_CM = 4; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_SIG_ID_E5_BI = 5; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_SIG_ID_E5_BQ = 6; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_QUALITY_IND_NO_SIGNAL = 0; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_QUALITY_IND_SEARCHING = 1; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_QUALITY_IND_ACQUIRED = 2; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_QUALITY_IND_DETECTED_UNUSABLE = 3; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_QUALITY_IND_CODE_LOCKED_TIME_SYNCED = 4; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_QUALITY_IND_CODE_AND_CARRIER_LOCKED_TIME_SYNCED1 = 5; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_QUALITY_IND_CODE_AND_CARRIER_LOCKED_TIME_SYNCED2 = 6; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_QUALITY_IND_CODE_AND_CARRIER_LOCKED_TIME_SYNCED3 = 7; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_CORR_SOURCE_NONE = 0; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_CORR_SOURCE_SBAS = 1; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_CORR_SOURCE_BEIDOU = 2; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_CORR_SOURCE_RTCM2 = 3; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_CORR_SOURCE_RTCM3_OCR = 4; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_CORR_SOURCE_RTCM3_SSR = 5; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_CORR_SOURCE_QZSS_SLAS = 6; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_IONO_MODEL_NONE = 0; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_IONO_MODEL_GPS_KLOBUCHAR = 1; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_IONO_MODEL_SBAS = 2; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_IONO_MODEL_BEIDOU_KLOBUCHAR = 3; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_IONO_MODEL_DUAL_FREQ = 8; // saturated uint4
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_HEALTH_UNKNOWN = 0; // saturated uint2
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_HEALTH_HEALTHY = 1; // saturated uint2
public const double COM_HEX_EQUIPMENT_GNSS_SIGNAL_HEALTH_UNHEALTHY = 2; // saturated uint2

public class com_hex_equipment_gnss_Signal: IUAVCANSerialize {
    public uint8_t sv_id = new uint8_t();
    public uint8_t gnss_id = new uint8_t();
    public uint8_t sig_id = new uint8_t();
    public uint8_t freq_id = new uint8_t();
    public uint8_t quality_ind = new uint8_t();
    public int16_t pr_res = new int16_t();
    public uint8_t cno = new uint8_t();
    public uint8_t corr_source = new uint8_t();
    public uint8_t iono_model = new uint8_t();
    public uint8_t health = new uint8_t();
    public bool pr_smoothed = new bool();
    public bool pr_used = new bool();
    public bool cr_used = new bool();
    public bool do_used = new bool();
    public bool pr_corr_used = new bool();
    public bool cr_corr_used = new bool();
    public bool do_corr_used = new bool();

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_com_hex_equipment_gnss_Signal(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_com_hex_equipment_gnss_Signal(transfer, this);
}

};

}
}