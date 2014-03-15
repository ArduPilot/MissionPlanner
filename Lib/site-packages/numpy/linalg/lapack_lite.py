import sys

if sys.platform == 'cli':
    import clr
    clr.AddReference("lapack_lite")

    from numpy__linalg__lapack_lite import *
