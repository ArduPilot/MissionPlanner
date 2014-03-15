
import sys

if sys.platform == 'cli':
    import clr
    clr.AddReference('optimize')
    from scipy__optimize___zeros import _brentq, _ridder, _brenth, _bisect
    from scipy__optimize___zeros import *


