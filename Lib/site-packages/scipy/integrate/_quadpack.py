
import sys

if sys.platform == 'cli':
    import clr
    clr.AddReference('integrate')
    from scipy__integrate___quadpack import _qagie, _qagpe, _qawoe, _qawfe, _qawce, _qagse, _qawse
    from scipy__integrate___quadpack import *


