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

namespace DroneCAN
{
    public partial class DroneCAN 
    {
        public partial class com_himark_servo_ServoInfo: IDroneCANSerialize 
        {
            public const int COM_HIMARK_SERVO_SERVOINFO_MAX_PACK_SIZE = 12;
            public const ulong COM_HIMARK_SERVO_SERVOINFO_DT_SIG = 0xCA8F4B8F97D23B57;
            public const int COM_HIMARK_SERVO_SERVOINFO_DT_ID = 2019;

            public const double COM_HIMARK_SERVO_SERVOINFO_ERROR_STATUS_NO_ERROR = 0; // saturated uint5
            public const double COM_HIMARK_SERVO_SERVOINFO_ERROR_STATUS_DATA_ERROR = 1; // saturated uint5

            public uint8_t servo_id = new uint8_t();
            public uint16_t pwm_input = new uint16_t();
            public int16_t pos_cmd = new int16_t();
            public int16_t pos_sensor = new int16_t();
            public uint16_t voltage = new uint16_t();
            public uint16_t current = new uint16_t();
            public uint16_t pcb_temp = new uint16_t();
            public uint16_t motor_temp = new uint16_t();
            public uint8_t error_status = new uint8_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_com_himark_servo_ServoInfo(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_com_himark_servo_ServoInfo(transfer, this, fdcan);
            }

            public static com_himark_servo_ServoInfo ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new com_himark_servo_ServoInfo();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}