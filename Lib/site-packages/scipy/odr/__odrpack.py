
import sys

if sys.platform == 'cli':
    import clr
    clr.AddReference('odr')
    from scipy__odr____odrpack import *


