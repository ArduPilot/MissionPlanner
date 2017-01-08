using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MissionPlanner.Utilities
{
    public class Device
    {
//ph2
//COMPASS_DEV_ID	73225
//COMPASS_DEV_ID2	262434
//COMPASS_DEV_ID3	131874

        // from ap_hal\device.h
        public enum BusType
        {
            BUS_TYPE_UNKNOWN = 0,
            BUS_TYPE_I2C = 1,
            BUS_TYPE_SPI = 2,
            BUS_TYPE_UAVCAN = 3
        }

        public enum Speed
        {
            SPEED_HIGH,
            SPEED_LOW,
        }

        // float = 1 sign, 8 exponents, 23 significand

        public struct DeviceStructure
        {
            // the data
            public UInt32 devid;

            // accessors
            public BusType bus_type { get { return (BusType) (devid & 0x3); } } // : 3;
            public byte bus { get { return (byte)((devid >> 2) & 0x1f); } } //: 5;    // which instance of the bus type
            public byte address { get { return (byte) ((devid >> 8) & 0xff); } } // address on the bus (eg. I2C address)
            public DevTypes devtype { get { return (DevTypes)((devid >> 16) & 0xff); } } // device class specific device type

            public DeviceStructure(UInt32 id)
            {
                devid = id;

                Console.WriteLine("bus type {0} bus {1} address (i2c addr or spi CS) {2} devtype {3} ", bus_type, bus, address, devtype);
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
