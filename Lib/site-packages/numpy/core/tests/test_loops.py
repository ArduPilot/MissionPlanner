# This is to test things in numpy/core/src/umath/loops.c

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

alltypes = types + [ np.datetime64, np.timedelta64 ]

int_types = [ np.byte, np.ubyte, np.short, np.ushort, np.intc, np.uintc,
              np.int_, np.uint, np.longlong, np.ulonglong ]

rc_types = [ np.single,  np.double,  np.longdouble,
             np.csingle, np.cdouble, np.clongdouble ]

class TestLoops(TestCase):

    def test_conjugate_reals( self ):

        rtypes = [ np.byte, np.ubyte,
                   np.short, np.ushort,
                   np.intc, np.uintc,
                   np.int_, np.uint,
                   np.longlong, np.ulonglong,
                   np.single, np.double, np.longdouble ]

        for t in rtypes:
            self.exercise_conjugate_real_t( t )

    def exercise_conjugate_real_t( self, tp ):

        a = np.arange( 5, dtype=tp )
        b = np.conjugate( a )
        # conjugate has no effect on a pure real number.
        assert_equal( b, a )

    # Still need to test conjugate on cfloat, cdouble and clongdouble

    def test_conjugate_complex( self ):

        x = np.arange( 5, dtype=np.clongdouble )
        x *= 0+1j
        y = np.conjugate( x )
        yeqx = y == x
        # conj(0) == 0
        assert_equal( yeqx, [True, False, False, False, False] )

        z = np.conjugate( y )
        assert_equal( z, x )

    def test_logical_ops( self ):

        for t in alltypes[1:]:
            self.exercise_greater_t( t )
            self.exercise_greater_equal_t( t )
            self.exercise_less_t( t )
            self.exercise_less_equal_t( t )
            self.exercise_logical_and_t( t )
            self.exercise_logical_or_t( t )
            self.exercise_logical_xor_t( t )
            self.exercise_logical_not_t( t )
            self.exercise_logical_not_equal_t( t )

    def exercise_greater_t( self, tp ):

        a = np.arange( 5, dtype=tp )
        b = a > 2
        assert_equal( b, [False, False, False, True, True] )

    def exercise_greater_equal_t( self, tp ):

        a = np.arange( 5, dtype=tp )
        b = a >= 2
        assert_equal( b, [False, False, True, True, True] )

    def exercise_less_t( self, tp ):

        a = np.arange( 5, dtype=tp )
        b = a < 2
        assert_equal( b, [True, True, False, False, False] )

    def exercise_less_equal_t( self, tp ):

        a = np.arange( 5, dtype=tp )
        b = a <= 2
        assert_equal( b, [True, True, True, False, False] )

    def exercise_logical_and_t( self, tp ):

        a = np.arange( 5, dtype=tp )
        b = np.ones( 5, dtype=tp )
        assert_equal( np.logical_and(a,b),
                      [False, True, True, True, True] )

    def exercise_logical_or_t( self, tp ):

        a = np.arange( 5, dtype=tp )
        b = np.ones( 5, dtype=tp )
        assert_equal( np.logical_or(a,b),
                      [True, True, True, True, True] )

    def exercise_logical_xor_t( self, tp ):

        a = np.arange( 5, dtype=tp )
        b = a.copy()
        assert_equal( np.logical_xor(a,b),
                      [False, False, False, False, False] )

    def exercise_logical_not_t( self, tp ):

        a = np.arange( 5, dtype=tp )
        b = np.logical_not( a )
        assert_equal( b, [True, False, False, False, False] )

    def exercise_logical_not_equal_t( self, tp ):

        a = np.arange( 5, dtype=tp )
        b = a[::-1]
        assert_equal( np.not_equal(a,b), [True, True, False, True, True] )

    def test_reciprocal( self ):

        for t in int_types:
            self.exercise_reciprocal_int_t( t )

        for t in rc_types:
            self.exercise_reciprocal_rc_t( t )

    def exercise_reciprocal_int_t( self, tp ):

        a = np.arange( 1, 5, dtype=tp )
        b = np.reciprocal( a )
        assert_equal( b, [1, 0, 0, 0] )

    def exercise_reciprocal_rc_t( self, tp ):

        a = np.arange( 1, 5, dtype=tp )
        b = np.reciprocal( a )
        c = np.reciprocal( b )
        assert_equal( c, a )

    def test_BOOL_ops( self ):

        a = np.array( [False, True] )

        b = a >= True
        assert_equal( b, [False, True] )

        b = a > False
        assert_equal( b, [False, True] )

        b = a <= False
        assert_equal( b, [True, False] )

        b = a < True
        assert_equal( b, [True, False] )

        # This runs the BOOL_to_UNICODE function, but we can't convert back
        # to check that the transformation is reversible.
        a.astype( np.unicode )

        b = a != False
        assert_equal( b, [False, True] )

        x = np.array( [True, False] )
        c = np.maximum( a, x )
        d = np.minimum( a, x )

        assert_equal( c, [True, True] )
        assert_equal( d, [False, False] )

        z = np.ones_like( x )
        assert z.shape == x.shape, "BOOL_ones_like botched the shape."

        # Casting a bool array to np.void throws an exception.  Is that
        # expected?

        # BOOL_scan requires writing a file I/O test.  Deferred for now.

    def test_invert( self ):

        for t in int_types:
            a = np.arange( 5, dtype=t )
            assert_equal( np.invert( np.invert( a ) ), a )

    def test_math_ops( self ):

        for t in int_types:
            self.exercise_fmod_t( t )

        for t in types[1:]:
            self.exercise_floor_divide_t( t )
            self.exercise_fmin_t( t )
            self.exercise_square_t( t )

        for t in alltypes[1:]:
            self.exercise_maximum_t( t )
            self.exercise_minimum_t( t )

        signed_types = [ np.byte, np.short, np.intc, np.int,
                         np.long, np.longlong,
                         np.single, np.double, np.longdouble,
                         np.csingle, np.cdouble, np.clongdouble,
                         np.datetime64, np.timedelta64 ]

        unsigned_types = [ np.ubyte, np.ushort, np.uintc,
                           np.uint, np.ulonglong ]

        for t in signed_types:
            self.exercise_sign_t( t )

        for t in unsigned_types:
            # based on exercise_sign_t(), but noting that unsigned types
            # never have negative values, so the sign is never -1.
            a = np.arange( 5, dtype=t ) - 2
            b = np.sign(a).astype( np.int )
            assert_equal( b, [1,1,0,1,1] )

        for t in [ np.single, np.double, np.longdouble ]:
            self.exercise_frexp_t( t )

    def exercise_fmod_t( self, tp ):

        a = np.arange( 5, dtype=tp )
        b = np.array( [2,2,2,2,2], dtype=tp )
        c = np.fmod( a, b )
        assert_equal( c, np.array( [0,1,0,1,0], dtype=tp ) )

    def exercise_floor_divide_t( self, tp ):

        a = np.arange( 5, dtype=tp )
        assert_equal( np.floor_divide( a, 2 ), [0, 0, 1, 1, 2] )

    def exercise_fmin_t( self, tp ):

        a = np.arange( 5, dtype=tp )
        b = a[::-1]
        c = np.fmin( a, b )
        assert_equal( c, np.array( [0,1,2,1,0], dtype=tp ) )

    def exercise_square_t( self, tp ):

        a = np.arange( 5, dtype=tp )
        assert_equal( np.square(a), a*a )

    def exercise_maximum_t( self, tp ):

        a = np.arange( 5, dtype=tp )
        b = np.empty( 5, dtype=tp )
        b.fill( tp(2) )
        c = np.maximum( a, b )

        assert_equal( c, np.array( [2,2,2,3,4], dtype=tp ) )

    def exercise_minimum_t( self, tp ):

        a = np.arange( 5, dtype=tp )
        b = np.empty( 5, dtype=tp )
        b.fill( tp(2) )
        c = np.minimum( a, b )

        assert_equal( c, np.array( [0,1,2,2,2], dtype=tp ) )

    def exercise_sign_t( self, tp ):

        a = np.arange( 5, dtype=tp ) - 2
        b = np.sign(a).astype( np.int )
        assert_equal( b, [-1,-1,0,1,1] )

    def exercise_frexp_t( self, tp ):

        a = np.arange( 5, dtype=tp )

        b = np.frexp( a )
        coeff = b[0]
        exp   = b[1]

        for i in range( len(coeff) ):
            x = coeff[i] * pow( 2, exp[i] )
            assert x == a[i]

    def test_ones_like( self ):

        for t in alltypes[1:]:
            a = np.arange( 5, dtype=t )
            b = np.ones_like( a )
            assert a.shape == b.shape, "ones_like garbles shape"
            assert_equal( b, np.array( [1,1,1,1,1], dtype=t ) )

    def test_fmax( self ):

        for t in [ np.single, np.double, np.longdouble,
                   np.csingle, np.cdouble, np.clongdouble ]:

            a = np.arange( 5, dtype=t )
            b = a[::-1]
            c = np.fmax( a, b )
            assert_equal( c, np.array( [4,3,2,3,4], dtype=t ) )

    def test_object_loops( self ):

        a = np.arange( 5, dtype='O' )
        b = a[::-1]

        c = a >= b
        assert_equal( c, [False, False, True, True, True] )

        c = a > b
        assert_equal( c, [False, False, False, True, True] )

        c = a < b
        assert_equal( c, [True, True, False, False, False] )

        c = a <= b
        assert_equal( c, [True, True, True, False, False] )

        # Check casting to numeric types
        for t in [ np.byte, np.ubyte, np.short, np.ushort,
                   np.intc, np.uintc, np.int, np.uint,
                   np.longlong, np.ulonglong,
                   np.single, np.double, np.longdouble,
                   np.csingle, np.cdouble, np.clongdouble,
                   np.unicode ]:
            b = a.astype( t )

            # We have to use array of sequence conversion rather than
            # arange() here because np.unicode lacks fill support, hence
            # won't work with arange().
            assert_equal( b, np.array( [0,1,2,3,4], dtype=t ) )

            c = b.astype( 'O' )

            # Object arrays don't compare as equal, so we have to cast again
            # to the target type to verify equivalence.
            assert_equal( b, c.astype( t ) )

        # Timedelta and datetime when converted to object, are
        # datetime/timedelta objects, not the integers that they started off
        # as in the a array above.  So we convert again, and test for
        # equality between datetime/timedelta objects, which were converted
        # through the datetime/timedelta scalars.  This seems reasonable.
        for t in [ np.datetime64, np.timedelta64 ]:
            b = a.astype( t )
            assert_equal( b, np.array( [0,1,2,3,4], dtype=t ) )

            c = b.astype( 'O' )
            d = c.astype( t )

            # Object arrays don't compare as equal, so we cast them to
            # arrays of integers and do the comparison that way.
            assert_equal( b.astype( np.int ), d.astype( np.int ) )

        # Curiously, this doesn't exercise OBJECT_to_OBJECT, hence adding
        # that to the suppression list.
        b = a.astype( 'O' )
        assert_equal( b, np.array( [0,1,2,3,4], dtype='O' ) )

        # Unclear how to check OBJECT_to_VOID
        try:
            va = a.astype( np.void )
            assert_equal( va, np.array( [0,1,2,3,4], dtype=np.void ) )
        except:
            pass

        # Now check OBJECT_sign
        a = a - 2
        b = np.sign( a )
        assert_equal( b, np.array( [-1,-1,0,1,1] ) )

    def test_string_conversions( self ):

        target_types = [ np.byte, np.ubyte, np.short, np.ushort,
                         np.intc, np.uintc,
                         np.int, np.uint, np.longlong, np.ulonglong,
                         np.single, np.double, np.longdouble,
                         np.csingle, np.cdouble, np.clongdouble,
                         np.unicode ]

        for t in target_types:
            a = np.array( [0,1,2,3,4], dtype=t )
            b = a.astype( 'S10' )
            c = b.astype( t )
            assert_equal( a, c )

    @dec.knownfailureif(True, "datetime converions fail.")
    def test_string_to_datetimedelta( self ):
        # Handle these separately, as they each currently fail.
        for t in [np.datetime64, np.timedelta64]:
            a = np.array( [0,1,2,3,4], dtype=t )
            b = a.astype( 'S50' )
            c = b.astype( t )
            assert_equal( a, c )

    @dec.knownfailureif(True, "string to bool converions fail.")
    def test_string_to_bool( self ):

        a = np.array( [False, True] )
        b = a.astype( 'S10' )
        # This also fails with np.bool_
        c = b.astype( np.bool )
        assert_equal( c, a )

    def test_unicode_conversions( self ):
        target_types = [ np.byte, np.ubyte, np.short, np.ushort,
                         np.intc, np.uintc,
                         np.int, np.uint, np.longlong, np.ulonglong,
                         np.single, np.double, np.longdouble,
                         np.csingle, np.cdouble, np.clongdouble,
                         ]

        for t in target_types:
            a = np.array( [0,1,2,3,4], dtype=t )
            b = a.astype( 'U10' )
            c = b.astype( t )
            assert_equal( a, c )

    @dec.knownfailureif(True, "datetime converions fail.")
    def test_unicode_to_datetimedelta( self ):
        # Handle these separately, as they each currently fail.
        for t in [np.datetime64, np.timedelta64]:
            a = np.array( [0,1,2,3,4], dtype=t )
            b = a.astype( 'U50' )
            c = b.astype( t )
            assert_equal( a, c )

    def test_copysign( self ):

        for t in [np.single, np.double, np.longdouble]:
            a = np.arange( 5, dtype=t ) -2
            b = np.arange( 5, dtype=t )
            c = np.copysign( b, a )
            assert_equal( c, np.array( [0, -1, 2, 3, 4], dtype=t ) )

if __name__ == "__main__":
    run_module_suite()
