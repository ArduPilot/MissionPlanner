import sys
from numpy.testing import *
import numpy as np

class TestMethods(TestCase):

    def test_itemset( self ):

        # array method itemset is implemented via array_setscalar

        x = np.arange(3)
        x.itemset( 1, 99 )

        assert_array_equal( x, [0, 99, 2] )

if __name__ == "__main__":
    run_module_suite()
