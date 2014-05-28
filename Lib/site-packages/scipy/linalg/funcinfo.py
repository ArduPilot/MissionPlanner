

class LinalgFunctionInfo(object):
    def __init__(self, function, module_name, prefix, typecode, dtype):
        self.function = function
        self.module_name = module_name
        self.prefix = prefix
        self.typecode = typecode
        self.dtype = dtype

_functions = {}

def register_func_info(function, module_name, prefix, typecode, dtype):
    info = LinalgFunctionInfo(function, module_name, prefix, typecode, dtype)
    _functions[function] = info

def get_func_info(function):
    return _functions[function]

