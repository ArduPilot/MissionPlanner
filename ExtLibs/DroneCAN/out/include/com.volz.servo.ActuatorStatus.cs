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
        public partial class com_volz_servo_ActuatorStatus: IDroneCANSerialize 
        {
            public const int COM_VOLZ_SERVO_ACTUATORSTATUS_MAX_PACK_SIZE = 7;
            public const ulong COM_VOLZ_SERVO_ACTUATORSTATUS_DT_SIG = 0x29BF0D53B4060263;
            public const int COM_VOLZ_SERVO_ACTUATORSTATUS_DT_ID = 20020;

            public uint8_t actuator_id = new uint8_t();
            public Single actual_position = new Single();
            public uint8_t current = new uint8_t();
            public uint8_t voltage = new uint8_t();
            public uint8_t motor_pwm = new uint8_t();
            public uint8_t motor_temperature = new uint8_t();

            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool fdcan = false)
            {
                encode_com_volz_servo_ActuatorStatus(this, chunk_cb, ctx, fdcan);
            }

            public void decode(CanardRxTransfer transfer, bool fdcan = false)
            {
                decode_com_volz_servo_ActuatorStatus(transfer, this, fdcan);
            }

            public static com_volz_servo_ActuatorStatus ByteArrayToDroneCANMsg(byte[] transfer, int startoffset, bool fdcan = false)
            {
                var ans = new com_volz_servo_ActuatorStatus();
                ans.decode(new DroneCAN.CanardRxTransfer(transfer.Skip(startoffset).ToArray()), fdcan);
                return ans;
            }
        }
    }
}