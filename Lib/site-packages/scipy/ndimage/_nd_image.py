
import sys

if sys.platform == 'cli':
    import clr
    clr.AddReference('ndimage')
    from scipy__ndimage___nd_image import *
