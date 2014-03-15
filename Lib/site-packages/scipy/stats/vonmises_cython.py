
import sys

if sys.platform == 'cli':
    import clr
    clr.AddReference('stats')
    from scipy__stats__vonmises_cython import *



