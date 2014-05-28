import sys

if sys.platform == 'cli':
    import clr
    clr.AddReference("_fftpack")

    from scipy__fftpack___fftpack import *
