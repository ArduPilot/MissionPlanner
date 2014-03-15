import sys

if sys.platform == 'cli':
    import clr
    clr.AddReference("linalg")

    from scipy__linalg___flinalg import *
