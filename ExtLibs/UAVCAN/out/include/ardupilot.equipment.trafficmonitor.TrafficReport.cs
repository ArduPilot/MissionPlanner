
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

//using uavcan.Timestamp.cs

public const int ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_MAX_PACK_SIZE = 47;
public const ulong ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_DT_SIG = 0x68E45DB60B6981F8;
public const int ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_DT_ID = 20790;



public const double ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_SOURCE_ADSB = 0; // saturated uint3
public const double ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_SOURCE_ADSB_UAT = 1; // saturated uint3
public const double ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_SOURCE_FLARM = 2; // saturated uint3
public const double ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_TRAFFIC_TYPE_UNKNOWN = 0; // saturated uint5
public const double ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_TRAFFIC_TYPE_LIGHT = 1; // saturated uint5
public const double ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_TRAFFIC_TYPE_SMALL = 2; // saturated uint5
public const double ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_TRAFFIC_TYPE_LARGE = 3; // saturated uint5
public const double ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_TRAFFIC_TYPE_HIGH_VORTEX_LARGE = 4; // saturated uint5
public const double ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_TRAFFIC_TYPE_HEAVY = 5; // saturated uint5
public const double ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_TRAFFIC_TYPE_HIGHLY_MANUV = 6; // saturated uint5
public const double ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_TRAFFIC_TYPE_ROTOCRAFT = 7; // saturated uint5
public const double ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_TRAFFIC_TYPE_GLIDER = 9; // saturated uint5
public const double ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_TRAFFIC_TYPE_LIGHTER_THAN_AIR = 10; // saturated uint5
public const double ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_TRAFFIC_TYPE_PARACHUTE = 11; // saturated uint5
public const double ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_TRAFFIC_TYPE_ULTRA_LIGHT = 12; // saturated uint5
public const double ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_TRAFFIC_TYPE_UAV = 14; // saturated uint5
public const double ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_TRAFFIC_TYPE_SPACE = 15; // saturated uint5
public const double ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_TRAFFIC_TYPE_EMERGENCY_SURFACE = 17; // saturated uint5
public const double ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_TRAFFIC_TYPE_SERVICE_SURFACE = 18; // saturated uint5
public const double ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_TRAFFIC_TYPE_POINT_OBSTACLE = 19; // saturated uint5
public const double ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_ALT_TYPE_ALT_UNKNOWN = 0; // saturated uint7
public const double ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_ALT_TYPE_PRESSURE_AMSL = 1; // saturated uint7
public const double ARDUPILOT_EQUIPMENT_TRAFFICMONITOR_TRAFFICREPORT_ALT_TYPE_WGS84 = 2; // saturated uint7

public class ardupilot_equipment_trafficmonitor_TrafficReport: IUAVCANSerialize {
    public uavcan_Timestamp timestamp = new uavcan_Timestamp();
    public uint32_t icao_address = new uint32_t();
    public uint16_t tslc = new uint16_t();
    public int32_t latitude_deg_1e7 = new int32_t();
    public int32_t longitude_deg_1e7 = new int32_t();
    public Single alt_m = new Single();
    public Single heading = new Single();
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public Single[] velocity = new Single[3];
    public uint16_t squawk = new uint16_t();
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=9)] public uint8_t[] callsign = new uint8_t[9];
    public uint8_t source = new uint8_t();
    public uint8_t traffic_type = new uint8_t();
    public uint8_t alt_type = new uint8_t();
    public bool lat_lon_valid = new bool();
    public bool heading_valid = new bool();
    public bool velocity_valid = new bool();
    public bool callsign_valid = new bool();
    public bool ident_valid = new bool();
    public bool simulated_report = new bool();
    public bool vertical_velocity_valid = new bool();
    public bool baro_valid = new bool();

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_ardupilot_equipment_trafficmonitor_TrafficReport(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_ardupilot_equipment_trafficmonitor_TrafficReport(transfer, this);
}

};

}
}