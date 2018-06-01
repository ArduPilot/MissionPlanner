using MissionPlanner.Attributes;

namespace MissionPlanner.ArduPilot
{
    public enum ap_product
    {
        [DisplayText("HIL")]
        AP_PRODUCT_ID_NONE = 0x00, // Hardware in the loop
        [DisplayText("APM1 1280")]
        AP_PRODUCT_ID_APM1_1280 = 0x01, // APM1 with 1280 CPUs
        [DisplayText("APM1 2560")]
        AP_PRODUCT_ID_APM1_2560 = 0x02, // APM1 with 2560 CPUs
        [DisplayText("SITL")]
        AP_PRODUCT_ID_SITL = 0x03, // Software in the loop
        [DisplayText("PX4")]
        AP_PRODUCT_ID_PX4 = 0x04, // PX4 on NuttX
        [DisplayText("PX4 FMU 2")]
        AP_PRODUCT_ID_PX4_V2 = 0x05, // PX4 FMU2 on NuttX
        [DisplayText("APM2 ES C4")]
        AP_PRODUCT_ID_APM2ES_REV_C4 = 0x14, // APM2 with MPU6000ES_REV_C4
        [DisplayText("APM2 ES C5")]
        AP_PRODUCT_ID_APM2ES_REV_C5 = 0x15, // APM2 with MPU6000ES_REV_C5
        [DisplayText("APM2 ES D6")]
        AP_PRODUCT_ID_APM2ES_REV_D6 = 0x16, // APM2 with MPU6000ES_REV_D6
        [DisplayText("APM2 ES D7")]
        AP_PRODUCT_ID_APM2ES_REV_D7 = 0x17, // APM2 with MPU6000ES_REV_D7
        [DisplayText("APM2 ES D8")]
        AP_PRODUCT_ID_APM2ES_REV_D8 = 0x18, // APM2 with MPU6000ES_REV_D8	
        [DisplayText("APM2 C4")]
        AP_PRODUCT_ID_APM2_REV_C4 = 0x54, // APM2 with MPU6000_REV_C4 	
        [DisplayText("APM2 C5")]
        AP_PRODUCT_ID_APM2_REV_C5 = 0x55, // APM2 with MPU6000_REV_C5 	
        [DisplayText("APM2 D6")]
        AP_PRODUCT_ID_APM2_REV_D6 = 0x56, // APM2 with MPU6000_REV_D6 		
        [DisplayText("APM2 D7")]
        AP_PRODUCT_ID_APM2_REV_D7 = 0x57, // APM2 with MPU6000_REV_D7 	
        [DisplayText("APM2 D8")]
        AP_PRODUCT_ID_APM2_REV_D8 = 0x58, // APM2 with MPU6000_REV_D8 	
        [DisplayText("APM2 D9")]
        AP_PRODUCT_ID_APM2_REV_D9 = 0x59, // APM2 with MPU6000_REV_D9 
        [DisplayText("FlyMaple")]
        AP_PRODUCT_ID_FLYMAPLE = 0x100, // Flymaple with ITG3205, ADXL345, HMC5883, BMP085
        [DisplayText("Linux")]
        AP_PRODUCT_ID_L3G4200D = 0x101, // Linux with L3G4200D and ADXL345
    }
}