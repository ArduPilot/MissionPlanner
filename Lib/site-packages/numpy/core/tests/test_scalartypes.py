import sys
from numpy.testing import *
import numpy as np

class TestIndex(TestCase):

    def test_index( self ):
        a = np.arange(5)

        itypes = [ np.byte, np.ubyte,
                   np.short, np.ushort,
                   np.int32, np.uint32,
                   np.int, np.uint,
                   # np.long, np.ulong, ???  no "ulong" in numpy?
                   np.longlong, np.ulonglong ]

        for k,t in enumerate( itypes ):
            x = t( 1 )
            y = t( 2 )
            assert_equal( a[x:y], [1] )

    def test_index_bool( self ):
        a = np.arange(5)

        x = np.bool_(False)
        y = np.bool_(True)

        z = a[x:y]

        assert_equal( z, [0] )

if __name__ == "__main__":
    run_module_suite()
