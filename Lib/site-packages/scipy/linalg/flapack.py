import sys

if sys.platform == 'cli':
    import clr
    clr.AddReference("linalg")

    from scipy__linalg__flapack import *
