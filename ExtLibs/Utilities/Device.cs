using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MissionPlanner.Utilities
{
    public class Device
    {
        // from ap_hal\device.h
        public enum BusType
        {
            BUS_TYPE_UNKNOWN = 0,
            BUS_TYPE_I2C = 1,
            BUS_TYPE_SPI = 2,
            BUS_TYPE_UAVCAN = 3,
            BUS_TYPE_SITL = 4
        }

        public enum Speed
        {
            SPEED_HIGH,
            SPEED_LOW,
        }

        //nuttx\include\nuttx\spi.h
        public enum spi_dev_e
        {
            SPIDEV_NONE = 0,    /* Not a valid value */
            SPIDEV_MMCSD,       /* Select SPI MMC/SD device */
            SPIDEV_FLASH,       /* Select SPI FLASH device */
            SPIDEV_ETHERNET,    /* Select SPI ethernet device */
            SPIDEV_DISPLAY,     /* Select SPI LCD/OLED display device */
            SPIDEV_WIRELESS,    /* Select SPI Wireless device */
            SPIDEV_TOUCHSCREEN, /* Select SPI touchscreen device */
            SPIDEV_EXPANDER,    /* Select SPI I/O expander device */
            SPIDEV_MUX,         /* Select SPI multiplexer device */
            SPIDEV_AUDIO_DATA,  /* Select SPI audio codec device data port */
            SPIDEV_AUDIO_CTRL,  /* Select SPI audio codec device control port */
        };

        // float = 1 sign, 8 exponents, 23 significand

        public struct DeviceStructure
        {
            // the data
            public UInt32 devid;

            // accessors
            public BusType bus_type { get { return (BusType) (devid & 0x7); } } // : 3;
            public byte bus { get { return (byte)((devid >> 3) & 0x1f); } } //: 5;    // which instance of the bus type
            public byte address { get { return (byte) ((devid >> 8) & 0xff); } } // address on the bus (eg. I2C address)
            public DevTypes devtype { get { return (DevTypes)((devid >> 16) & 0xff); } } // device class specific device type

            public imu_types devtypeimu { get { return (imu_types)((devid >> 16) & 0xff); } }

            public DeviceStructure(UInt32 id)
            {
                devid = id;

                Console.WriteLine(ToString());
            }

            public override string ToString()
            {
                return string.Format("devid {5} bus type {0} bus {1} address {2} devtype {3} or {4} ", bus_type,
                    bus, address, devtype, devtypeimu, devid);
            }

            // from AP_Compass_Backend.h
            public enum DevTypes
            {
                DEVTYPE_HMC5883_OLD = 0x01,
                DEVTYPE_HMC5883 = 0x07,
                DEVTYPE_LSM303D = 0x02,
                DEVTYPE_AK8963 = 0x04,
                DEVTYPE_BMM150 = 0x05,
                DEVTYPE_LSM9DS1 = 0x06,
                DEVTYPE_LIS3MDL = 0x08,
                DEVTYPE_AK09916 = 0x09,
                DEVTYPE_IST8310 = 0x0A,
                DEVTYPE_ICM20948 = 0x0B,
                DEVTYPE_MMC3416 = 0x0C,
                DEVTYPE_QMC5883L = 0x0D,
                DEVTYPE_MAG3110 = 0x0E,
                DEVTYPE_SITL = 0x0F,
                DEVTYPE_IST8308 = 0x10,
            }

            //AP_InertialSensor_Backend.h
            public enum imu_types
            {
                DEVTYPE_BMI160 = 0x09,
                DEVTYPE_L3G4200D = 0x10,
                DEVTYPE_ACC_LSM303D = 0x11,
                DEVTYPE_ACC_BMA180 = 0x12,
                DEVTYPE_ACC_MPU6000 = 0x13,
                DEVTYPE_ACC_MPU9250 = 0x16,
                DEVTYPE_ACC_IIS328DQ = 0x17,
                DEVTYPE_ACC_LSM9DS1 = 0x18,
                DEVTYPE_GYR_MPU6000 = 0x21,
                DEVTYPE_GYR_L3GD20 = 0x22,
                DEVTYPE_GYR_MPU9250 = 0x24,
                DEVTYPE_GYR_I3G4250D = 0x25,
                DEVTYPE_GYR_LSM9DS1 = 0x26,
                DEVTYPE_INS_ICM20789 = 0x27,
                DEVTYPE_INS_ICM20689 = 0x28,
                DEVTYPE_INS_BMI055 = 0x29,
                DEVTYPE_SITL = 0x2A,
            };

        public enum px4_i2c_bus
            {
                PX4_I2C_BUS_ONBOARD=0,
                PX4_I2C_BUS_EXPANSION=1
            }

            // from PX4Firmware\src\drivers\boards\px4fmu-v2\board_config.h
            public enum px4_spi_bus 
            {
 PX4_SPI_BUS_SENSORS=	1,
 PX4_SPI_BUS_RAMTRON=	2,
 PX4_SPI_BUS_EXT	=	4,
            }
        }

        //DEV_ID
        //DEV_ID2
        //DEV_ID3

        //ap_hal - device.h
        //set_device_type
        //get_bus_id

        /**
* make a bus id given bus type, bus number, bus address and
* device type This is for use by devices that do not use one of
* the standard HAL Device types, such as UAVCAN devices
*/
        /*
           static uint32_t make_bus_id(enum BusType bus_type, uint8_t bus, uint8_t address, uint8_t devtype) {
           union DeviceId d;
           d.devid_s.bus_type = bus_type;
           d.devid_s.bus = bus;
           d.devid_s.address = address;
           d.devid_s.devtype = devtype;
           return d.devid;
           */

        // devid is a union of bus type, bus, address and devtype
    }
}
