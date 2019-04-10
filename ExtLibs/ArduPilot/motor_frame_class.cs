namespace MissionPlanner.ArduPilot
{
    public enum motor_frame_class
    {
        MOTOR_FRAME_UNDEFINED = 0,
        MOTOR_FRAME_QUAD = 1,
        MOTOR_FRAME_HEXA = 2,
        MOTOR_FRAME_OCTA = 3,
        MOTOR_FRAME_OCTAQUAD = 4,
        MOTOR_FRAME_Y6 = 5,
        MOTOR_FRAME_HELI = 6,
        MOTOR_FRAME_TRI = 7,
        MOTOR_FRAME_SINGLE = 8,
        MOTOR_FRAME_COAX = 9,
        MOTOR_FRAME_TAILSITTER = 10,
        MOTOR_FRAME_HELI_DUAL = 11,
        MOTOR_FRAME_DODECAHEXA = 12,
        MOTOR_FRAME_HELI_QUAD = 13,
    };

    //from ap_hal\boards\chibios.h
    public enum hal_ins_spi
    {
        ms5611,
        ms5611_int,
        ms5611_ext,
        lps22h,
        bmp280,
        mpu6000,
        mpu6000_ext,
        lsm9ds0_g,
        lsm9ds0_am,
        lsm9ds0_ext_g,
        lsm9ds0_ext_am,
        mpu9250,
        mpu9250_ext,
        mpu6500,
        icm20608,
        //icm20608-am,
        icm20608_ext,
        hmc5843,
        lis3mdl,
    }

    /// <summary>
    /// define.*I2C_ADD
    /// </summary>
    public enum hal_ins_i2c
    {
        ADS1115_I2C_ADDR = 0x48,
        DLVR_I2C_ADDR = 0x28,
        MS4525D0_I2C_ADDR = 0x28,
        MS5525D0_I2C_ADDR_1 = 0x76,
        MS5525D0_I2C_ADDR_2 = 0x77,
        SDP3XD0_I2C_ADDR = 0x21,
        SDP3XD1_I2C_ADDR = 0x22,
        SDP3XD2_I2C_ADDR = 0x23,
        HAL_BARO_BMP085_I2C_ADDR = 0x77,
        HAL_BARO_BMP280_I2C_ADDR = 0x76,
        HAL_BARO_BMP280_I2C_ADDR2 = 0x77,
        HAL_BARO_DPS280_I2C_ADDR = 0x76,
        HAL_BARO_DPS280_I2C_ADDR2 = 0x77,
        HAL_BARO_FBM320_I2C_ADDR = 0x6C,
        HAL_BARO_FBM320_I2C_ADDR2 = 0x6D,
        HAL_BARO_ICM20789_I2C_ADDR = 0x63,
        HAL_BARO_KELLERLD_I2C_ADDR = 0x40,
        HAL_BARO_LPS25H_I2C_ADDR = 0x5D,
        HAL_BARO_MS5611_I2C_ADDR = 0x77,
        HAL_BARO_MS5611_I2C_ADDR2 = 0x76,
        HAL_BARO_MS5607_I2C_ADDR = 0x77,
        HAL_BARO_MS5837_I2C_ADDR = 0x76,
        HAL_BARO_MS5637_I2C_ADDR = 0x76,
        AP_BATTMONITOR_SMBUS_I2C_ADDR = 0x0B,
        HAL_COMPASS_AK09916_I2C_ADDR = 0x0C,
        HAL_COMPASS_ICM20948_I2C_ADDR = 0x69,
        AK8963_I2C_ADDR = 0x0c,
        HAL_COMPASS_IST8308_I2C_ADDR = 0x0C,
        HAL_COMPASS_IST8310_I2C_ADDR = 0x0E,
        HAL_COMPASS_LIS3MDL_I2C_ADDR = 0x1c,
        HAL_COMPASS_LIS3MDL_I2C_ADDR2 = 0x1e,
        HAL_MAG3110_I2C_ADDR = 0x0E,
        HAL_COMPASS_MMC3416_I2C_ADDR = 0x30,
        HAL_COMPASS_QMC5883L_I2C_ADDR = 0x0D,
        HAL_COMPASS_RM3100_I2C_ADDR = 0x20,
        HAL_COMPASS_HMC5843_I2C_ADDR = 0x1E,

        HAL_INS_MPU60x0_I2C_ADDR = 0x68,
        HAL_COMPASS_AK8963_I2C_ADDR = 0x0d,

        HAL_RCOUT_BEBOP_BLDC_I2C_ADDR = 0x08,

        HAL_RCOUT_DISCO_BLDC_I2C_ADDR = 0x08,

        HAL_INS_MPU9250_I2C_ADDR = 0x68,

        HAL_COMPASS_BMM150_I2C_ADDR = 0x12,

        HAL_OPTFLOW_PX4FLOW_I2C_ADDRESS = 0x42,

        HAL_BARO_LPS25H_I2C_IMU_ADDR = 0x69,

        HAL_BARO_20789_I2C_ADDR_PRESS = 0x63,
        HAL_BARO_20789_I2C_ADDR_ICM = 0x68,

        BEBOP_BLDC_I2C_ADDR = 0x08,
        INS_INVENSENSE_20789_I2C_ADDR = 0x68,
        L3G4200D_I2C_ADDRESS = 0x69,
        IRLOCK_I2C_ADDRESS = 0x54,
        NOTIFY_DISPLAY_I2C_ADDR = 0x3C,
        NCP5623_LED_I2C_ADDR = 0x38,// default I2C bus address,
        NCP5623_C_LED_I2C_ADDR = 0x39, // default I2C bus address for the NCP5623C,
        OREOLED_BASE_I2C_ADDR = 0x68,
        TOSHIBA_LED_I2C_ADDR = 0x55, // default I2C bus address,
        PX4FLOW_BASE_I2C_ADDR = 0x42,

    }
}