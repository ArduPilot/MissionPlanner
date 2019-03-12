
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



static void encode_uavcan_protocol_param_Value(uavcan_protocol_param_Value msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_protocol_param_Value(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_protocol_param_Value(CanardRxTransfer transfer, uavcan_protocol_param_Value msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_protocol_param_Value(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_protocol_param_Value(uint8_t[] buffer, uavcan_protocol_param_Value msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {

    uint8_t uavcan_protocol_param_Value_type = (uint8_t)msg.uavcan_protocol_param_Value_type;
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 3, uavcan_protocol_param_Value_type);
    chunk_cb(buffer, 3, ctx);

    switch(msg.uavcan_protocol_param_Value_type) {
        case uavcan_protocol_param_Value_type_t.UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_EMPTY: {
            _encode_uavcan_protocol_param_Empty(buffer, msg.union.empty, chunk_cb, ctx, false);
            break;
        }
        case uavcan_protocol_param_Value_type_t.UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_INTEGER_VALUE: {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 64, msg.union.integer_value);
            chunk_cb(buffer, 64, ctx);
            break;
        }
        case uavcan_protocol_param_Value_type_t.UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_REAL_VALUE: {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 32, msg.union.real_value);
            chunk_cb(buffer, 32, ctx);
            break;
        }
        case uavcan_protocol_param_Value_type_t.UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_BOOLEAN_VALUE: {
            memset(buffer,0,8);
            canardEncodeScalar(buffer, 0, 8, msg.union.boolean_value);
            chunk_cb(buffer, 8, ctx);
            break;
        }
        case uavcan_protocol_param_Value_type_t.UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_STRING_VALUE: {
            if (!tao) {
                memset(buffer,0,8);
                canardEncodeScalar(buffer, 0, 8, msg.union.string_value_len);
                chunk_cb(buffer, 8, ctx);
            }
            for (int i=0; i < msg.union.string_value_len; i++) {
                    memset(buffer,0,8);
                    canardEncodeScalar(buffer, 0, 8, msg.union.string_value[i]);
                    chunk_cb(buffer, 8, ctx);
            }
            break;
        }
    }
}

static void _decode_uavcan_protocol_param_Value(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_protocol_param_Value msg, bool tao) {

    uint8_t uavcan_protocol_param_Value_type = 0;
    canardDecodeScalar(transfer, bit_ofs, 3, false, ref uavcan_protocol_param_Value_type);
    msg.uavcan_protocol_param_Value_type = (uavcan_protocol_param_Value_type_t)uavcan_protocol_param_Value_type;
    bit_ofs += 3;

    switch(msg.uavcan_protocol_param_Value_type) {
        case uavcan_protocol_param_Value_type_t.UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_EMPTY: {
            _decode_uavcan_protocol_param_Empty(transfer, ref bit_ofs, msg.union.empty, false);
            break;
        }

        case uavcan_protocol_param_Value_type_t.UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_INTEGER_VALUE: {
            canardDecodeScalar(transfer, bit_ofs, 64, true, ref msg.union.integer_value);
            bit_ofs += 64;
            break;
        }

        case uavcan_protocol_param_Value_type_t.UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_REAL_VALUE: {
            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.union.real_value);
            bit_ofs += 32;
            break;
        }

        case uavcan_protocol_param_Value_type_t.UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_BOOLEAN_VALUE: {
            canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.union.boolean_value);
            bit_ofs += 8;
            break;
        }

        case uavcan_protocol_param_Value_type_t.UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_STRING_VALUE: {
            if (!tao) {
                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.union.string_value_len);
                bit_ofs += 8;
            } else {
                msg.union.string_value_len = (uint8_t)(((transfer.payload_len*8)-bit_ofs)/8);
            }

            for (int i=0; i < msg.union.string_value_len; i++) {
                canardDecodeScalar(transfer, bit_ofs, 8, false, ref msg.union.string_value[i]);
                bit_ofs += 8;
            }
            break;
        }

    }
}

}
}