


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




static void encode_uavcan_protocol_param_NumericValue(uavcan_protocol_param_NumericValue msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx) {
    uint8_t[] buffer = new uint8_t[8];
    _encode_uavcan_protocol_param_NumericValue(buffer, msg, chunk_cb, ctx, true);
}

static uint32_t decode_uavcan_protocol_param_NumericValue(CanardRxTransfer transfer, uavcan_protocol_param_NumericValue msg) {
    uint32_t bit_ofs = 0;
    _decode_uavcan_protocol_param_NumericValue(transfer, ref bit_ofs, msg, true);
    return (bit_ofs+7)/8;
}

static void _encode_uavcan_protocol_param_NumericValue(uint8_t[] buffer, uavcan_protocol_param_NumericValue msg, uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx, bool tao) {



    uint8_t uavcan_protocol_param_NumericValue_type = (uint8_t)msg.uavcan_protocol_param_NumericValue_type;
    memset(buffer,0,8);
    canardEncodeScalar(buffer, 0, 2, uavcan_protocol_param_NumericValue_type);
    chunk_cb(buffer, 2, ctx);

    switch(msg.uavcan_protocol_param_NumericValue_type) {




        case uavcan_protocol_param_NumericValue_type_t.UAVCAN_PROTOCOL_PARAM_NUMERICVALUE_TYPE_EMPTY: {



            _encode_uavcan_protocol_param_Empty(buffer, msg.union.empty, chunk_cb, ctx, false);


            break;

        }



        case uavcan_protocol_param_NumericValue_type_t.UAVCAN_PROTOCOL_PARAM_NUMERICVALUE_TYPE_INTEGER_VALUE: {



            memset(buffer,0,8);

            canardEncodeScalar(buffer, 0, 64, msg.union.integer_value);

            chunk_cb(buffer, 64, ctx);


            break;

        }



        case uavcan_protocol_param_NumericValue_type_t.UAVCAN_PROTOCOL_PARAM_NUMERICVALUE_TYPE_REAL_VALUE: {



            memset(buffer,0,8);

            canardEncodeScalar(buffer, 0, 32, msg.union.real_value);

            chunk_cb(buffer, 32, ctx);


            break;

        }




    }


}

static void _decode_uavcan_protocol_param_NumericValue(CanardRxTransfer transfer,ref uint32_t bit_ofs, uavcan_protocol_param_NumericValue msg, bool tao) {



    uint8_t uavcan_protocol_param_NumericValue_type = 0;
    canardDecodeScalar(transfer, bit_ofs, 2, false, ref uavcan_protocol_param_NumericValue_type);
    msg.uavcan_protocol_param_NumericValue_type = (uavcan_protocol_param_NumericValue_type_t)uavcan_protocol_param_NumericValue_type;
    bit_ofs += 2;

    switch(msg.uavcan_protocol_param_NumericValue_type) {




        case uavcan_protocol_param_NumericValue_type_t.UAVCAN_PROTOCOL_PARAM_NUMERICVALUE_TYPE_EMPTY: {



            _decode_uavcan_protocol_param_Empty(transfer, ref bit_ofs, msg.union.empty, false);


            break;

        }




        case uavcan_protocol_param_NumericValue_type_t.UAVCAN_PROTOCOL_PARAM_NUMERICVALUE_TYPE_INTEGER_VALUE: {





            canardDecodeScalar(transfer, bit_ofs, 64, true, ref msg.union.integer_value);


            bit_ofs += 64;


            break;

        }




        case uavcan_protocol_param_NumericValue_type_t.UAVCAN_PROTOCOL_PARAM_NUMERICVALUE_TYPE_REAL_VALUE: {





            canardDecodeScalar(transfer, bit_ofs, 32, true, ref msg.union.real_value);


            bit_ofs += 32;


            break;

        }





    }

}

}
}