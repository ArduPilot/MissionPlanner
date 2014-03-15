import sys
import warnings

import numpy as np
from numpy.testing import *


warnings.filterwarnings('ignore',
             'Casting complex values to real discards the imaginary part')

types = [np.bool_, np.byte, np.ubyte, np.short, np.ushort, np.intc, np.uintc,
         np.int_, np.uint, np.longlong, np.ulonglong,
         np.single, np.double, np.longdouble, np.csingle,
         np.cdouble, np.clongdouble]

alltypes = list( types )
alltypes.append( np.datetime64 )
alltypes.append( np.timedelta64 )

class TestArrayTypes(TestCase):

    def test_argmax( self ):

        x = np.array( [False, False, True, False], dtype=np.bool )
        assert x.argmax() == 2, "Broken array.argmax on np.bool"

        a = np.array( [u'aaa', u'aa', u'bbb'] )
        # u'aaa' > u'aa' and u'bbb' > u'aaa'  Hence, argmax == 2.
        assert a.argmax() == 2, "Broken array.argmax on unicode data."

        a = np.array( [ 'aaa', 'aa', 'bbb'] )
        # 'aaa' > 'aa' and 'bbb' > 'aaa'  Hence, argmax == 2.
        assert a.argmax() == 2, "Broken array.argmax on string data."

    def test_argmax_numeric( self ):

        # Skip the np.bool_ type as it lacks a fill function, hence can't use
        # arange().
        for k,t in enumerate( alltypes[1:] ):

            a = np.arange( 5, dtype=t )
            assert a.argmax() == 4, "Broken array.argmax on type: " + t

    def test_nonzero_numeric_types( self ):

        for k,t in enumerate(alltypes):

            a = np.array( [ t(1) ] )

            assert a, "Broken array.nonzero on type: " + t

    def test_nonzero_string_types( self ):

        a = np.array( [ 'aaa' ] )
        assert a, "Broken array.nonzero on string elements."

        a = np.array( [ u'aaa' ] )
        assert a, "Broken array.nonzero on Unicode elements."

    def test_compare( self ):
        # Light bulb!  argmax doesn't call compare() for numeric/logical
        # types.  It only does that for string types.  Duh.

        pass

    def test_copyswap( self ):

        # Skip np.bool_.
        for k,t in enumerate( types[1:] ):

            x = np.arange( 10, dtype=t )
            # This should exeercise <typoe>_copyswap
            x[::2].fill( t(2) )

            assert_equal( x, [2,1,2,3,2,5,2,7,2,9] )

    def test_copyswap_misc( self ):

        x = np.array( [ u'a', u'b', u'c' ] )
        x[::2].fill( u'd' )
        assert_equal( x, [u'd', u'b', u'd'] )

    def test_copyswapn( self ):

        # bool lacks arange support.
        for k,t in enumerate( alltypes[1:] ):

            x = np.arange( 10, dtype=t )
            y = x.byteswap()
            z = y.byteswap()

            assert_equal( z, x )

    def test_copyswapn_misc( self ):
        x = np.array( [ u'a', u'b', u'c' ] )
        y = x.byteswap()
        z = y.byteswap()

        assert_equal( z, x )

    def test_compare( self ):

        for k,t in enumerate( alltypes[1:] ):

            try:
                a = np.arange( 10, dtype=t )
                keys = a[::2]
                b = a.searchsorted( keys )
                c = a.copy()
                np.insert( c, b, b.astype( t ) )
                c.sort()
                assert_equal( c, a )

            except TypeError, e:
                print "Trouble with type %d:" % k, e

    def test_compare_bool( self ):
        # bool can't handle numpy.arange(), so has to be coded separately.
        a = np.array( [False, True], dtype=np.bool_ )
        keys = a
        b = a.searchsorted( keys )
        c = a.copy()
        np.insert( c, b, keys )
        c.sort()
        assert_equal( c, a )

    def test_dot( self ):

        # Do something to test dot on bool...

        for k,t in enumerate( alltypes[1:] ):
            a = np.arange( 3, dtype=t ) + 1
            assert a.dot(a) == t(14), \
                   "Problem with dot product with array of type %s" % k

    def test_clip( self ):

        for k,t in enumerate( alltypes[1:] ):
            a = np.arange( 5, dtype=t )
            b = a.clip( 2, 3 )
            x = np.array( [2,2,2,3,3], dtype=t )
            assert_equal( b, x )

    def test_clip_bool( self ):
        a = np.array( [False, True], np.bool )
        assert_equal( a.clip(False,False), [False, False] )

    def test_array_casting( self ):

        for k,t in enumerate( alltypes ):

            a = np.array( [ t(1) ] )

            for k2, t2 in enumerate( alltypes ):

                b = a.astype( t2 )

                if k2 < len(types):
                    assert b[0] == 1, \
                           "Busted array type casting: k=%d k2=%d" % (k,k2)

                else:
                    # Casting to datetime64 yields a 1/1/1970+... result,
                    # which isn't so hot for checking against "1".  So, in
                    # these cases, just cast back to the starting time, and
                    # make sure we got back what we started with.
                    c = b.astype( t )
                    assert_equal( c, a )

    def test_take( self ):
        # Test all types, but skipp np.bool_ for now, as it lacks a fill
        # function.  Grrr.
        for k,t in enumerate( alltypes[1:] ):
            a = np.arange( 10, dtype=t )
            idx = np.arange(5) * 2
            c = np.take( a, idx )
            assert_equal( c, a[::2] )

    def test_putmask( self ):

        for k,t in enumerate( alltypes[1:] ):
            a = np.arange( 5, dtype=t )
            mask = np.zeros( 5, dtype=np.bool )
            mask[::2] = True
            np.putmask( a, mask, t(8) )

            x = np.array( [8,1,8,3,8], dtype=t )

            assert_equal( a, x )

    def test_fillwithscalar( self ):

        a = np.empty( 2, dtype=np.datetime64 )
        a.fill( np.datetime64( 3 ) )
        x = np.zeros( 2, dtype=np.datetime64 ) + 3
        assert_equal( a, x )

if __name__ == "__main__":
    run_module_suite()
