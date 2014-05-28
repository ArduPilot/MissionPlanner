
import sys

if sys.platform == 'cli':
    import clr
    clr.AddReference('optimize')
    from scipy__optimize___minpack import _lmdif, _chkder, _hybrd, _hybrj, _lmder
    from scipy__optimize___minpack import *


