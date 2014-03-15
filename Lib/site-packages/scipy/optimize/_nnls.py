
import sys

if sys.platform == 'cli':
    import clr
    clr.AddReference('optimize')
    from scipy__optimize___nnls import *


