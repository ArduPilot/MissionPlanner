import os
import uavcan
import errno
import em
import math
import copy

def get_empy_env_request(msg):
    assert msg.kind == msg.KIND_SERVICE
    msg_underscored_name = msg.full_name.replace('.','_')+'_req'
    msg_resp_underscored_name = msg.full_name.replace('.','_')+'_res'
    return {
        'msg_underscored_name': msg_underscored_name,
        'msg_header_file_name': msg_header_name_request(msg),
        'msg_c_type': underscored_name_to_ctype(msg_underscored_name),
        'msg_union': msg.request_union,
        'msg_fields': msg.request_fields,
        'msg_constants': msg.request_constants,
        'msg_max_bitlen': msg.get_max_bitlen_request(),
        'msg_dt_sig': msg.get_data_type_signature(),
        'msg_default_dtid': msg.default_dtid,
        'msg_kind': 'request',
        'msg_resp_underscored_name': msg_resp_underscored_name,
        'msg_resp_header_file_name': msg_header_name_response(msg)
    }

def get_empy_env_response(msg):
    assert msg.kind == msg.KIND_SERVICE
    msg_underscored_name = msg.full_name.replace('.','_')+'_res'
    return {
        'msg_underscored_name': msg_underscored_name,
        'msg_header_file_name': msg_header_name_response(msg),
        'msg_c_type': underscored_name_to_ctype(msg_underscored_name),
        'msg_union': msg.response_union,
        'msg_fields': msg.response_fields,
        'msg_constants': msg.response_constants,
        'msg_max_bitlen': msg.get_max_bitlen_response(),
        'msg_dt_sig': msg.get_data_type_signature(),
        'msg_default_dtid': msg.default_dtid,
        'msg_kind': 'response'
    }

def get_empy_env_broadcast(msg):
    assert msg.kind == msg.KIND_MESSAGE
    msg_underscored_name = msg.full_name.replace('.','_')
    return {
        'msg_underscored_name': msg_underscored_name,
        'msg_header_file_name': msg_header_name(msg),
        'msg_c_type': underscored_name_to_ctype(msg_underscored_name),
        'msg_union': msg.union,
        'msg_fields': msg.fields,
        'msg_constants': msg.constants,
        'msg_max_bitlen': msg.get_max_bitlen(),
        'msg_dt_sig': msg.get_data_type_signature(),
        'msg_default_dtid': msg.default_dtid,
        'msg_kind': 'broadcast'
    }

def uavcan_type_is_signed(uavcan_type):
    assert uavcan_type.category == uavcan_type.CATEGORY_PRIMITIVE
    if uavcan_type.kind == uavcan_type.KIND_BOOLEAN:
        return False
    elif uavcan_type.kind == uavcan_type.KIND_UNSIGNED_INT:
        return False
    elif uavcan_type.kind == uavcan_type.KIND_SIGNED_INT:
        return True
    elif uavcan_type.kind == uavcan_type.KIND_FLOAT:
        return True

def union_msg_tag_bitlen_from_num_fields(num_fields):
    return int(math.ceil(math.log(num_fields,2)))

def union_msg_tag_uint_type_from_num_fields(num_fields):
    return c_uint_type_from_bitlen(union_msg_tag_bitlen_from_num_fields(num_fields))

def array_len_field_bitlen(array_type):
    assert array_type.category == array_type.CATEGORY_ARRAY
    return int(math.ceil(math.log(array_type.max_size+1,2)))

def c_int_type_bitlen(bitlen):
    for ret in (8, 16, 32, 64):
        if bitlen <= ret:
            return ret

def c_uint_type_from_bitlen(bitlen):
    return 'uint%u_t' % (c_int_type_bitlen(bitlen),)

def c_int_type_from_bitlen(bitlen):
    return 'int%u_t' % (c_int_type_bitlen(bitlen),)

def underscored_name_to_ctype(name):
    return '%s' % (name)

def uavcan_type_to_ctype(uavcan_type):
    assert uavcan_type.category != uavcan_type.CATEGORY_VOID
    if uavcan_type.category == uavcan_type.CATEGORY_COMPOUND:
        assert uavcan_type.kind == uavcan_type.KIND_MESSAGE
        return '%s' % (underscored_name(uavcan_type))
    elif uavcan_type.category == uavcan_type.CATEGORY_PRIMITIVE:
        if uavcan_type.kind == uavcan_type.KIND_BOOLEAN:
            return 'bool'
        elif uavcan_type.kind == uavcan_type.KIND_UNSIGNED_INT:
            return c_uint_type_from_bitlen(uavcan_type.bitlen)
        elif uavcan_type.kind == uavcan_type.KIND_SIGNED_INT:
            return c_int_type_from_bitlen(uavcan_type.bitlen)
        elif uavcan_type.kind == uavcan_type.KIND_FLOAT:
            if uavcan_type.bitlen <= 16: return 'Single'
            if uavcan_type.bitlen <= 32: return 'Single'
            if uavcan_type.bitlen <= 64: return 'Double'

def field_cdef(field):
    assert field.type.category != field.type.CATEGORY_VOID
    if field.type.category == field.type.CATEGORY_ARRAY:
        if field.type.mode == field.type.MODE_STATIC:
            return '[MarshalAs(UnmanagedType.ByValArray,SizeConst=%u)] public %s[] %s = new %s[%u]' % (field.type.max_size, uavcan_type_to_ctype(field.type.value_type), field.name, uavcan_type_to_ctype(field.type.value_type), field.type.max_size)
        else:
            return 'public uint%u_t %s_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=%u)] public %s[] %s = new %s[%u]' % (c_int_type_bitlen(array_len_field_bitlen(field.type)), field.name, field.type.max_size, uavcan_type_to_ctype(field.type.value_type), field.name, uavcan_type_to_ctype(field.type.value_type), field.type.max_size)
    else:
        return 'public %s %s = new %s()' % (uavcan_type_to_ctype(field.type), field.name, uavcan_type_to_ctype(field.type))

def indent(string, n):
    if string.strip():
        string = '    '*n + string
        string.replace('\n', '\n' + '    '*n)
    return string

def msg_header_name_request(obj):
    if isinstance(obj, uavcan.dsdl.Field):
        obj = obj.type
    assert obj.category == obj.CATEGORY_COMPOUND and obj.kind == obj.KIND_SERVICE
    return '%s_req.cs' % (obj.full_name,)

def msg_header_name_response(obj):
    if isinstance(obj, uavcan.dsdl.Field):
        obj = obj.type
    assert obj.category == obj.CATEGORY_COMPOUND and obj.kind == obj.KIND_SERVICE
    return '%s_res.cs' % (obj.full_name,)

def msg_header_name(obj):
    if isinstance(obj, uavcan.dsdl.Field):
        obj = obj.type
    return '%s.cs' % (obj.full_name,)

def msg_c_file_name_request(obj):
    if isinstance(obj, uavcan.dsdl.Field):
        obj = obj.type
    assert obj.category == obj.CATEGORY_COMPOUND and obj.kind == obj.KIND_SERVICE
    return '%s_req.cs' % (obj.full_name,)

def msg_c_file_name_response(obj):
    if isinstance(obj, uavcan.dsdl.Field):
        obj = obj.type
    assert obj.category == obj.CATEGORY_COMPOUND and obj.kind == obj.KIND_SERVICE
    return '%s_res.cs' % (obj.full_name,)

def msg_c_file_name(obj):
    if isinstance(obj, uavcan.dsdl.Field):
        obj = obj.type
    return '%s.cs' % (obj.full_name,)

def underscored_name(obj):
    return obj.full_name.replace('.','_')

def rel_path(obj):
    return os.path.join(*obj.full_name.split('.')[:-1])

def short_name(obj):
    return obj.full_name.split('.')[-1]

# https://stackoverflow.com/questions/600268/mkdir-p-functionality-in-python
def mkdir_p(path):
    try:
        os.makedirs(path)
    except OSError as exc:  # Python >2.5
        if exc.errno == errno.EEXIST and os.path.isdir(path):
            pass
        else:
            raise
