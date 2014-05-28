import sys

if sys.platform == 'cli':
    import clr
    clr.AddReference("fftpack_lite")

    from numpy__fft__fftpack_cython import *
