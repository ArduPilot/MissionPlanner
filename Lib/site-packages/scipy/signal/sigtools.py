import sys

if sys.platform == 'cli':
    import clr
    clr.AddReference("signal")

    from scipy__signal__sigtoolsmod import _linear_filter
    from scipy__signal__sigtoolsmod import _order_filterND, _remez
    from scipy__signal__sigtoolsmod import _correlateND, _medfilt2d
    from scipy__signal__sigtoolsmod import *

