@{from canard_dsdlc_helpers import *}@

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

@{
dep_headers = set()
for field in msg_fields:
    if field.type.category == field.type.CATEGORY_COMPOUND:
        dep_headers.add(msg_header_name(field.type))
    if field.type.category == field.type.CATEGORY_ARRAY and field.type.value_type.category == field.type.value_type.CATEGORY_COMPOUND:
        dep_headers.add(msg_header_name(field.type.value_type))
}@
@[  for header in dep_headers]@
//using @(header)
@[  end for]@

public const int @(msg_underscored_name.upper())_MAX_PACK_SIZE = @((msg_max_bitlen+7)/8);
public const ulong @(msg_underscored_name.upper())_DT_SIG = @('0x%08X' % (msg_dt_sig,));
@[  if msg_default_dtid is not None]@
public const int @(msg_underscored_name.upper())_DT_ID = @(msg_default_dtid);
@[  end if]@


@[  if msg_constants]
@[    for constant in msg_constants]@
public const double @(msg_underscored_name.upper())_@(constant.name.upper()) = @(constant.value); // @(constant.type)
@[    end for]@
@[  end if]@
@[  if msg_union]
public enum @(msg_underscored_name)_type_t {
@[    for field in msg_fields]@
    @(msg_underscored_name.upper())_TYPE_@(field.name.upper()),
@[    end for]@
};
@[  end if]@

public class @(msg_c_type): IUAVCANSerialize {
@[  if msg_union]@
    public @(msg_underscored_name)_type_t @(msg_underscored_name)_type;
	//[StructLayout(LayoutKind.Explicit, Pack = 1)] 
    public class unions {
@[    for field in msg_fields]@
@[      if field.type.category != field.type.CATEGORY_VOID]@
		//[FieldOffset(0)]
        @(field_cdef(field));
@[      end if]@
@[    end for]@
    };
	public unions union = new unions();
@[  else]@
@[    for field in msg_fields]@
@[      if field.type.category != field.type.CATEGORY_VOID]@
    @(field_cdef(field));
@[      end if]@
@[    end for]@
@[  end if]@

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_@(msg_c_type)(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_@(msg_c_type)(transfer, this);
}

};

}
}