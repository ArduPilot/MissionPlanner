using System;
using System.Collections.Generic;
using System.Text;
using float32 = System.Single;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UAVCAN
{
    public static class Extension
    {
        public static T ByteArrayToUAVCANMsg<T>(this byte[] transfer, int startoffset) where T : new()
        {
            var ans = ((IUAVCANSerialize) new T());
            ans.decode(new uavcan.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
            return (T) ans;
        }

        public static IEnumerable<Tuple<T, T>> NowNextBy2<T>(this IEnumerable<T> list)
        {
            T now = default(T);
            T next = default(T);

            int a = -1;
            foreach (var item in list)
            {
                a++;
                now = next;
                next = item;
                if (a % 2 == 0)
                    continue;
                yield return new Tuple<T, T>(now, next);
            }
        }

        public static object GetValue(this uavcan.uavcan_protocol_param_Value value)
        {
            switch (value.uavcan_protocol_param_Value_type)
            {
                case uavcan.uavcan_protocol_param_Value_type_t.UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_BOOLEAN_VALUE:
                    return value.union.boolean_value;
                    break;
                case uavcan.uavcan_protocol_param_Value_type_t.UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_EMPTY:
                    return "Empty";
                    break;
                case uavcan.uavcan_protocol_param_Value_type_t.UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_INTEGER_VALUE:
                    return value.union.integer_value;
                    break;
                case uavcan.uavcan_protocol_param_Value_type_t.UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_REAL_VALUE:
                    return value.union.real_value;
                    break;
                case uavcan.uavcan_protocol_param_Value_type_t.UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_STRING_VALUE:
                    return ASCIIEncoding.ASCII.GetString(value.union.string_value, 0, value.union.string_value_len);
                    break;
            }

            return null;
        }

        public static object GetValue(this uavcan.uavcan_protocol_param_NumericValue value)
        {
            switch (value.uavcan_protocol_param_NumericValue_type)
            {
                case uavcan.uavcan_protocol_param_NumericValue_type_t.UAVCAN_PROTOCOL_PARAM_NUMERICVALUE_TYPE_EMPTY:
                    return value.union.empty;
                    break;
                case uavcan.uavcan_protocol_param_NumericValue_type_t.UAVCAN_PROTOCOL_PARAM_NUMERICVALUE_TYPE_INTEGER_VALUE:
                    return value.union.integer_value;
                    break;
                case uavcan.uavcan_protocol_param_NumericValue_type_t.UAVCAN_PROTOCOL_PARAM_NUMERICVALUE_TYPE_REAL_VALUE:
                    return value.union.real_value;
                    break;
            }

            return null;
        }
    }
}