
import sys


if sys.platform == 'cli':
    import clr
    import math
    clr.AddReference("NumpyDotNet")
    import NumpyDotNet
    NumpyDotNet.umath.__init__()
    pi = math.pi
    e = math.e
    from NumpyDotNet.umath import *

