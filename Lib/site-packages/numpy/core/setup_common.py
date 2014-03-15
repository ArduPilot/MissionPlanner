# Code shared by distutils and scons builds
import sys
from os.path import join
import warnings

#-------------------
# Versioning support
#-------------------
# How to change C_API_VERSION ?
#   - increase C_API_VERSION value
#   - record the hash for the new C API with the script cversions.py
#   and add the hash to cversions.txt
# The hash values are used to remind developers when the C API number was not
# updated - generates a MismatchCAPIWarning warning which is turned into an
# exception for released version.

# Binary compatibility version number. This number is increased whenever the
# C-API is changed such that binary compatibility is broken, i.e. whenever a
# recompile of extension modules is needed.
C_ABI_VERSION = 0x02000000

# Minor API version.  This number is increased whenever a change is made to the
# C-API -- whether it breaks binary compatibility or not.  Some changes, such
# as adding a function pointer to the end of the function table, can be made
# without breaking binary compatibility.  In this case, only the C_API_VERSION
# (*not* C_ABI_VERSION) would be increased.  Whenever binary compatibility is
# broken, both C_API_VERSION and C_ABI_VERSION should be increased.
C_API_VERSION = 0x00000005

class MismatchCAPIWarning(Warning):
    pass

def is_released(config):
    """Return True if a released version of numpy is detected."""
    from distutils.version import LooseVersion

    v = config.get_version('../version.py')
    if v is None:
        raise ValueError("Could not get version")
    pv = LooseVersion(vstring=v).version
    if len(pv) > 3:
        return False
    return True

def get_api_versions(apiversion, codegen_dir):
    """Return current C API checksum and the recorded checksum for the given
    version of the C API version."""
    api_files = [join(codegen_dir, 'numpy_api_order.txt'),
                 join(codegen_dir, 'ufunc_api_order.txt')]

    # Compute the hash of the current API as defined in the .txt files in
    # code_generators
    sys.path.insert(0, codegen_dir)
    try:
        m = __import__('genapi')
        numpy_api = __import__('numpy_api')
        curapi_hash = m.fullapi_hash(numpy_api.full_api)
        apis_hash = m.get_versions_hash()
    finally:
        del sys.path[0]

    return curapi_hash, apis_hash[apiversion]

def check_api_version(apiversion, codegen_dir):
    """Emits a MismacthCAPIWarning if the C API version needs updating."""
    curapi_hash, api_hash = get_api_versions(apiversion, codegen_dir)

    # If different hash, it means that the api .txt files in
    # codegen_dir have been updated without the API version being
    # updated. Any modification in those .txt files should be reflected
    # in the api and eventually abi versions.
    # To compute the checksum of the current API, use
    # code_generators/cversions.py script
    if not curapi_hash == api_hash:
        msg = (
            "API mismatch detected, the C API version "
            "numbers have to be updated. Current C api version is %d, "
            "with checksum %s, but recorded checksum for C API version %d in "
            "codegen_dir/cversions.txt is %s. If functions were added in the "
            "C API, you have to update C_API_VERSION  in %s.")
        warnings.warn(msg % (apiversion, curapi_hash, apiversion, api_hash,
                             __file__),
                      MismatchCAPIWarning)

def sym2def(symbol):
    define = symbol.replace(' ', '')
    return define.upper()
