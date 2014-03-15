
import sys

if sys.platform == 'cli':
    import clr
    clr.AddReference('special')
    from scipy__special__lambertw import *

