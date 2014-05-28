import sys

if sys.platform == 'cli':
    import clr
    clr.AddReference("signal")

    from scipy__signal__lfilter import *
