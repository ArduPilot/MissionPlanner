
import sys

if sys.platform == 'cli':
    import clr
    clr.AddReference("NumpyDotNet");
    from NumpyDotNet import *
    ModuleMethods.__init__()
    from NumpyDotNet.ModuleMethods import *
    import NumpyDotNet.ModuleMethods as NDNMM
    typeinfo = NDNMM.typeinfo
    _flagdict = NDNMM._flagdict

