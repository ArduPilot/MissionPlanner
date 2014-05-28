
import numpy as np
from numpy.core import *
from numpy.testing import *
from unittest import TestCase
import unittest
import random
import tempfile
import os
import warnings

from numpy.compat import asbytes, getexception, strchar

class TestFlags(TestCase):
    def setUp(self):
        self.a = arange(10)

    def test_writeable(self):
        mydict = locals()
        self.a.flags.writeable = False
        self.assertRaises(RuntimeError, runstring, 'self.a[0] = 3', mydict)
        self.a.flags.writeable = True
        self.a[0] = 5
        self.a[0] = 0

    def test_otherflags(self):
        assert_equal(self.a.flags.carray, True)
        assert_equal(self.a.flags.farray, False)
        assert_equal(self.a.flags.behaved, True)
        assert_equal(self.a.flags.fnc, False)
        assert_equal(self.a.flags.forc, True)
        assert_equal(self.a.flags.owndata, True)
        assert_equal(self.a.flags.writeable, True)
        assert_equal(self.a.flags.aligned, True)
        assert_equal(self.a.flags.updateifcopy, False)


class TestAttributes(TestCase):
    def setUp(self):
        self.one = arange(10)
        self.two = arange(20).reshape(4,5)
        self.three = arange(60,dtype=float64).reshape(2,5,6)

    def test_attributes(self):
        assert_equal(self.one.shape, (10,))
        assert_equal(self.two.shape, (4,5))
        assert_equal(self.three.shape, (2,5,6))
        self.three.shape = (10,3,2)
        assert_equal(self.three.shape, (10,3,2))
        self.three.shape = (2,5,6)
        assert_equal(self.one.strides, (self.one.itemsize,))
        num = self.two.itemsize
        assert_equal(self.two.strides, (5*num, num))
        num = self.three.itemsize
        assert_equal(self.three.strides, (30*num, 6*num, num))
        assert_equal(self.one.ndim, 1)
        assert_equal(self.two.ndim, 2)
        assert_equal(self.three.ndim, 3)
        num = self.two.itemsize
        assert_equal(self.two.size, 20)
        assert_equal(self.two.nbytes, 20*num)
        assert_equal(self.two.itemsize, self.two.dtype.itemsize)
        assert_equal(self.two.base, arange(20))

    def test_dtypeattr(self):
        assert_equal(self.one.dtype, dtype(int_))
        assert_equal(self.three.dtype, dtype(float_))
        assert_equal(self.one.dtype.char, 'l')
        assert_equal(self.three.dtype.char, 'd')
        self.assertTrue(self.three.dtype.str[0] in '<>')
        assert_equal(self.one.dtype.str[1], 'i')
        assert_equal(self.three.dtype.str[1], 'f')

    def _test_stridesattr(self):
        # TODO: Fix when we have buffer support
        x = self.one
        def make_array(size, offset, strides):
            return ndarray([size], buffer=x, dtype=int,
                           offset=offset*x.itemsize,
                           strides=strides*x.itemsize)
        assert_equal(make_array(4, 4, -1), array([4, 3, 2, 1]))
        self.assertRaises(ValueError, make_array, 4, 4, -2)
        self.assertRaises(ValueError, make_array, 4, 2, -1)
        self.assertRaises(ValueError, make_array, 8, 3, 1)
        #self.assertRaises(ValueError, make_array, 8, 3, 0)
        #self.assertRaises(ValueError, lambda: ndarray([1], strides=4))


    def _test_set_stridesattr(self):
        # TODO: Fix when we have buffer support
        x = self.one
        def make_array(size, offset, strides):
            try:
                r = ndarray([size], dtype=int, buffer=x, offset=offset*x.itemsize)
            except:
                raise RuntimeError(getexception())
            r.strides = strides=strides*x.itemsize
            return r
        assert_equal(make_array(4, 4, -1), array([4, 3, 2, 1]))
        assert_equal(make_array(7,3,1), array([3, 4, 5, 6, 7, 8, 9]))
        self.assertRaises(ValueError, make_array, 4, 4, -2)
        self.assertRaises(ValueError, make_array, 4, 2, -1)
        self.assertRaises(RuntimeError, make_array, 8, 3, 1)
        #self.assertRaises(ValueError, make_array, 8, 3, 0)

    def test_fill(self):
        for t in "?bhilqpBHILQPfdgFDGO":
            x = empty((3,2,1), t)
            y = empty((3,2,1), t)
            y[...] = 1

        x = array([(0,0.0), (1,1.0)], dtype='i4,f8')
        x.fill(x[0])
        assert_equal(x['f1'][1], x['f1'][0])


class TestDtypedescr(TestCase):
    def test_construction(self):
        d1 = dtype('i4')
        assert_equal(d1, dtype(int32))
        d2 = dtype('f8')
        assert_equal(d2, dtype(float64))

class TestZeroRank(TestCase):
    def setUp(self):
        self.d = array(0), array('x', object)

    def test_ellipsis_subscript(self):
        a,b = self.d
        self.assertEqual(a[...], 0)
        # TODO: We need string comparison for this
        #self.assertEqual(b[...], 'x')
        self.assertTrue(a[...] is a)
        self.assertTrue(b[...] is b)

    def test_empty_subscript(self):
        a,b = self.d
        self.assertEqual(a[()], 0)
        self.assertEqual(b[()], 'x')
        self.assertTrue(type(a[()]) is a.dtype.type)
        self.assertTrue(type(b[()]) is str)

    def test_invalid_subscript(self):
        a,b = self.d
        self.assertRaises(IndexError, lambda x: x[0], a)
        self.assertRaises(IndexError, lambda x: x[0], b)
        self.assertRaises(IndexError, lambda x: x[array([], int)], a)
        self.assertRaises(IndexError, lambda x: x[array([], int)], b)

    def test_ellipsis_subscript_assignment(self):
        a,b = self.d
        a[...] = 42
        self.assertEqual(a, 42)
        b[...] = ''
        self.assertEqual(b.item(), '')

    def test_empty_subscript_assignment(self):
        a,b = self.d
        a[()] = 42
        self.assertEqual(a, 42)
        b[()] = ''
        self.assertEqual(b.item(), '')

    def test_invalid_subscript_assignment(self):
        a,b = self.d
        def assign(x, i, v):
            x[i] = v
        self.assertRaises(IndexError, assign, a, 0, 42)
        self.assertRaises(IndexError, assign, b, 0, '')
        self.assertRaises(ValueError, assign, a, (), '')

    def test_newaxis(self):
        a,b = self.d
        self.assertEqual(a[newaxis].shape, (1,))
        self.assertEqual(a[..., newaxis].shape, (1,))
        self.assertEqual(a[newaxis, ...].shape, (1,))
        self.assertEqual(a[..., newaxis].shape, (1,))
        self.assertEqual(a[newaxis, ..., newaxis].shape, (1,1))
        self.assertEqual(a[..., newaxis, newaxis].shape, (1,1))
        self.assertEqual(a[newaxis, newaxis, ...].shape, (1,1))
        self.assertEqual(a[(newaxis,)*10].shape, (1,)*10)

    def test_invalid_newaxis(self):
        a,b = self.d
        def subscript(x, i): x[i]
        self.assertRaises(IndexError, subscript, a, (newaxis, 0))
        self.assertRaises(IndexError, subscript, a, (newaxis,)*50)

    def test_constructor(self):
        x = ndarray(())
        x[()] = 5
        self.assertEqual(x[()], 5)

    def _test_constructor(self):
        x = ndarray(())
        x[()] = 5
        self.assertEqual(x[()], 5)
        y = ndarray((),buffer=x)
        y[()] = 6
        self.assertEqual(x[()], 6)

    def test_output(self):
        x = array(2)
        self.assertRaises(ValueError, add, x, [1], x)


# this class is only used in TestCreation.test_from_attribute below
class X_array(object):
    def __array__(self, dtype=None):
        pass

class TestCreation(TestCase):
    def test_from_attribute(self):
        self.assertRaises(ValueError, array, X_array())

    def test_from_string(self) :
        types = np.typecodes['AllInteger'] + np.typecodes['Float']
        nstr = ['123','123']
        result = array([123, 123], dtype=int)
        for type in types :
            msg = 'String conversion for %s' % type
            assert_equal(array(nstr, dtype=type), result, err_msg=msg)

class TestBool(TestCase):
    def test_test_interning(self):
        a0 = bool_(0)
        b0 = bool_(False)
        self.assertTrue(a0 is b0)
        a1 = bool_(1)
        b1 = bool_(True)
        self.assertTrue(a1 is b1)
        self.assertTrue(array([True])[0] is a1)
        self.assertTrue(array(True)[()] is a1)


class TestMethods(TestCase):
    def test_test_round(self):
        assert_equal(array([1.2,1.5]).round(), [1,2])
        assert_equal(array(1.5).round(), 2)
        assert_equal(array([12.2,15.5]).round(-1), [10,20])
        assert_equal(array([12.15,15.51]).round(1), [12.2,15.5])

    def test_transpose(self):
        a = array([[1,2],[3,4]])
        assert_equal(a.transpose(), [[1,3],[2,4]])
        self.assertRaises(ValueError, lambda: a.transpose(0))
        self.assertRaises(ValueError, lambda: a.transpose(0,0))
        self.assertRaises(ValueError, lambda: a.transpose(0,1,2))

    # TODO: Without _sortmodule we only support quicksort
    # kinds = ['q', 'm', 'h']
    kinds = ['q']

    def test_sort(self):
        # test ordering for floats and complex containing nans. It is only
        # necessary to check the lessthan comparison, so sorts that
        # only follow the insertion sort path are sufficient. We only
        # test doubles and complex doubles as the logic is the same.

        # check doubles
        msg = "Test real sort order with nans"
        a = np.array([np.nan, 1, 0])
        b = sort(a)
        assert_equal(b, a[::-1], msg)
        # check complex
        msg = "Test complex sort order with nans"
        a = np.zeros(9, dtype=np.complex128)
        a.real += [np.nan, np.nan, np.nan, 1, 0, 1, 1, 0, 0]
        a.imag += [np.nan, 1, 0, np.nan, np.nan, 1, 0, 1, 0]
        b = sort(a)
        assert_equal(b, a[::-1], msg)

        # all c scalar sorts use the same code with different types
        # so it suffices to run a quick check with one type. The number
        # of sorted items must be greater than ~50 to check the actual
        # algorithm because quick and merge sort fall over to insertion
        # sort for small arrays.
        a = np.arange(100)
        b = a[::-1].copy()
        for kind in self.kinds :
            msg = "scalar sort, kind=%s" % kind
            c = a.copy();
            c.sort(kind=kind)
            assert_equal(c, a, msg)
            c = b.copy();
            c.sort(kind=kind)
            assert_equal(c, a, msg)

        # test complex sorts. These use the same code as the scalars
        # but the compare function differs.
        ai = a*1j + 1
        bi = b*1j + 1
        for kind in self.kinds :
            msg = "complex sort, real part == 1, kind=%s" % kind
            c = ai.copy();
            c.sort(kind=kind)
            assert_equal(c, ai, msg)
            c = bi.copy();
            c.sort(kind=kind)
            assert_equal(c, ai, msg)
        ai = a + 1j
        bi = b + 1j
        for kind in self.kinds :
            msg = "complex sort, imag part == 1, kind=%s" % kind
            c = ai.copy();
            c.sort(kind=kind)
            assert_equal(c, ai, msg)
            c = bi.copy();
            c.sort(kind=kind)
            assert_equal(c, ai, msg)

        # test string sorts.
        s = b'aaaaaaaa'
        a = np.array([s + bytes(chr(i)) for i in range(100)])
        b = a[::-1].copy()
        for kind in self.kinds :
            msg = "string sort, kind=%s" % kind
            c = a.copy();
            c.sort(kind=kind)
            assert_equal(c, a, msg)
            c = b.copy();
            c.sort(kind=kind)
            assert_equal(c, a, msg)

        # test unicode sort.
        s = 'aaaaaaaa'
        a = np.array([s + chr(i) for i in range(100)], dtype=np.unicode)
        b = a[::-1].copy()
        for kind in self.kinds :
            msg = "unicode sort, kind=%s" % kind
            c = a.copy();
            c.sort(kind=kind)
            assert_equal(c, a, msg)
            c = b.copy();
            c.sort(kind=kind)
            assert_equal(c, a, msg)

        # todo, check object array sorts.

        # check axis handling. This should be the same for all type
        # specific sorts, so we only check it for one type and one kind
        a = np.array([[3,2],[1,0]])
        b = np.array([[1,0],[3,2]])
        c = np.array([[2,3],[0,1]])
        d = a.copy()
        d.sort(axis=0)
        assert_equal(d, b, "test sort with axis=0")
        d = a.copy()
        d.sort(axis=1)
        assert_equal(d, c, "test sort with axis=1")
        d = a.copy()
        d.sort()
        assert_equal(d, c, "test sort with default axis")
        # using None is known fail at this point
        # d = a.copy()
        # d.sort(axis=None)
        #assert_equal(d, c, "test sort with axis=None")

    def test_sort_bool( self ):
        a = np.array( [ True, False, True, False ] )
        b = a[:-1].copy()
        for k in self.kinds :
            c = a.copy()
            c.sort( kind=k )
            assert_equal( c, [False, False, True, True] )

            c = a.copy()
            # Sort, then index the original (unosrted) array via that
            # resulting arg index array.   The result should be sorted.
            assert_equal( a[c.argsort( kind=k )], [False, False, True, True] )

            # We do it this way rather than asserting the result is a certain
            # integer idex array, because there are multiple index orderings
            # that are valid, and Python sorting doesn't seem to support a
            # stability imperative like certain other languages.

        # Note, comments above indicate that we should be using a much larger
        # array here, in order to guarantee the more interesting sort code is
        # actually executed for the bool case.

    def test_sort_redux( self ):

        types = [ np.byte, np.ubyte,
                  np.short, np.ushort,
                  np.int32, np.uint32,
                  np.int64, np.uint64,
                  np.longlong, np.ulonglong,
                  np.float32, np.complex64,
                  np.double, np.cdouble,
                  np.longdouble, np.clongdouble ]

        for k, t in enumerate( types ):
            a = np.arange( 100, dtype=t )
            b = a[::-1].copy()
            for kind in self.kinds  :
                msg = "scalar sort, kind=%s" % kind
                c = a.copy();
                c.sort(kind=kind)
                assert_equal(c, a, msg)
                c = b.copy();
                c.sort(kind=kind)
                assert_equal(c, a, msg)

                d = b.copy()
                idx = d.argsort( kind=kind )
                assert_equal( d[idx], a, "scalar argsort, kind=%s" % kind )

    def test_sort_other( self ):

        a = np.arange( 5, dtype=np.clongdouble )

        aj = a * 1j

        for k in self.kinds :
            b = a[::-1].copy()
            b.sort( kind=k )
            assert_equal( b, a )

            bj = aj[::-1].copy()
            bj.sort( kind=k )
            assert_equal( bj, aj )

            b = a[::-1].copy()
            bidx = b.argsort( kind=k )
            assert_equal( b[bidx], a )

    def _test_sort_order(self):
        # Test sorting an array with fields
        x1=np.array([21,32,14])
        x2=np.array(['my','first','name'])
        x3=np.array([3.1,4.5,6.2])
        r=np.rec.fromarrays([x1,x2,x3],names='id,word,number')

        r.sort(order=['id'])
        assert_equal(r.id, array([14,21,32]))
        assert_equal(r.word, array(['name','my','first']))
        assert_equal(r.number, array([6.2,3.1,4.5]))

        r.sort(order=['word'])
        assert_equal(r.id, array([32,21,14]))
        assert_equal(r.word, array(['first','my','name']))
        assert_equal(r.number, array([4.5,3.1,6.2]))

        r.sort(order=['number'])
        assert_equal(r.id, array([21,32,14]))
        assert_equal(r.word, array(['my','first','name']))
        assert_equal(r.number, array([3.1,4.5,6.2]))

        if sys.byteorder == 'little':
            strtype = '>i2'
        else:
            strtype = '<i2'
        mydtype = [('name', strchar + '5'),('col2',strtype)]
        r = np.array([('a', 1),('b', 255), ('c', 3), ('d', 258)],
                     dtype= mydtype)
        r.sort(order='col2')
        assert_equal(r['col2'], [1, 3, 255, 258])
        assert_equal(r, np.array([('a', 1), ('c', 3), ('b', 255), ('d', 258)],
                                 dtype=mydtype))


    def test_argsort(self):
        # all c scalar argsorts use the same code with different types
        # so it suffices to run a quick check with one type. The number
        # of sorted items must be greater than ~50 to check the actual
        # algorithm because quick and merge sort fall over to insertion
        # sort for small arrays.
        a = np.arange(100)
        b = a[::-1].copy()
        for kind in self.kinds :
            msg = "scalar argsort, kind=%s" % kind
            assert_equal(a.copy().argsort(kind=kind), a, msg)
            assert_equal(b.copy().argsort(kind=kind), b, msg)

        # test complex argsorts. These use the same code as the scalars
        # but the compare fuction differs.
        ai = a*1j + 1
        bi = b*1j + 1
        for kind in self.kinds :
            msg = "complex argsort, kind=%s" % kind
            assert_equal(ai.copy().argsort(kind=kind), a, msg)
            assert_equal(bi.copy().argsort(kind=kind), b, msg)
        ai = a + 1j
        bi = b + 1j
        for kind in self.kinds :
            msg = "complex argsort, kind=%s" % kind
            assert_equal(ai.copy().argsort(kind=kind), a, msg)
            assert_equal(bi.copy().argsort(kind=kind), b, msg)

        # test string argsorts.
        s = 'aaaaaaaa'
        a = np.array([s + chr(i) for i in range(100)])
        b = a[::-1].copy()
        r = arange(100)
        rr = r[::-1].copy()
        for kind in self.kinds :
            msg = "string argsort, kind=%s" % kind
            assert_equal(a.copy().argsort(kind=kind), r, msg)
            assert_equal(b.copy().argsort(kind=kind), rr, msg)

        # test unicode argsorts.
        s = 'aaaaaaaa'
        a = np.array([s + chr(i) for i in range(100)], dtype=np.unicode)
        b = a[::-1].copy()
        r = arange(100)
        rr = r[::-1].copy()
        for kind in self.kinds :
            msg = "unicode argsort, kind=%s" % kind
            assert_equal(a.copy().argsort(kind=kind), r, msg)
            assert_equal(b.copy().argsort(kind=kind), rr, msg)

        # todo, check object array argsorts.

        # check axis handling. This should be the same for all type
        # specific argsorts, so we only check it for one type and one kind
        a = np.array([[3,2],[1,0]])
        b = np.array([[1,1],[0,0]])
        c = np.array([[1,0],[1,0]])
        assert_equal(a.copy().argsort(axis=0), b)
        assert_equal(a.copy().argsort(axis=1), c)
        assert_equal(a.copy().argsort(), c)
        # using None is known fail at this point
        #assert_equal(a.copy().argsort(axis=None, c)

        if 'm' in self.kinds:
            # check that stable argsorts are stable
            r = np.arange(100)
            # scalars
            a = np.zeros(100)
            assert_equal(a.argsort(kind='m'), r)
            # complex
            a = np.zeros(100, dtype=np.complex)
            assert_equal(a.argsort(kind='m'), r)
            # string
            a = np.array(['aaaaaaaaa' for i in range(100)])
            assert_equal(a.argsort(kind='m'), r)
            # unicode
            a = np.array(['aaaaaaaaa' for i in range(100)], dtype=np.unicode)
            assert_equal(a.argsort(kind='m'), r)

    def test_searchsorted(self):
        # test for floats and complex containing nans. The logic is the
        # same for all float types so only test double types for now.
        # The search sorted routines use the compare functions for the
        # array type, so this checks if that is consistent with the sort
        # order.

        # check double
        a = np.array([np.nan, 1, 0])
        a = np.array([0, 1, np.nan])
        msg = "Test real searchsorted with nans, side='l'"
        b = a.searchsorted(a, side='l')
        assert_equal(b, np.arange(3), msg)
        msg = "Test real searchsorted with nans, side='r'"
        b = a.searchsorted(a, side='r')
        assert_equal(b, np.arange(1,4), msg)
        # check double complex
        a = np.zeros(9, dtype=np.complex128)
        a.real += [0, 0, 1, 1, 0, 1, np.nan, np.nan, np.nan]
        a.imag += [0, 1, 0, 1, np.nan, np.nan, 0, 1, np.nan]
        msg = "Test complex searchsorted with nans, side='l'"
        b = a.searchsorted(a, side='l')
        assert_equal(b, np.arange(9), msg)
        msg = "Test complex searchsorted with nans, side='r'"
        b = a.searchsorted(a, side='r')
        assert_equal(b, np.arange(1,10), msg)


    def test_flatten(self):
        x0 = np.array([[1,2,3],[4,5,6]], np.int32)
        x1 = np.array([[[1,2],[3,4]],[[5,6],[7,8]]], np.int32)
        y0 = np.array([1,2,3,4,5,6], np.int32)
        y0f = np.array([1,4,2,5,3,6], np.int32)
        y1 = np.array([1,2,3,4,5,6,7,8], np.int32)
        y1f = np.array([1,5,3,7,2,6,4,8], np.int32)
        assert_equal(x0.flatten(), y0)
        assert_equal(x0.flatten('F'), y0f)
        assert_equal(x0.flatten('F'), x0.T.flatten())
        assert_equal(x1.flatten(), y1)
        assert_equal(x1.flatten('F'), y1f)
        assert_equal(x1.flatten('F'), x1.T.flatten())

    def test_dot(self):
        a = np.array([[1, 0], [0, 1]])
        b = np.array([[0, 1], [1, 0]])
        c = np.array([[9, 1], [1, -9]])

        assert_equal(np.dot(a, b), a.dot(b))
        assert_equal(np.dot(np.dot(a, b), c), a.dot(b).dot(c))

class TestSubscripting(TestCase):
    def test_test_zero_rank(self):
        x = array([1,2,3])
        self.assertTrue(isinstance(x[0], np.int_))
        # TODO: Fix multiple inheritance
        if sys.version_info[0] < 3 and sys.platform != 'cli':
            self.assertTrue(isinstance(x[0], int))
        self.assertTrue(type(x[0, ...]) is ndarray)

# TODO: Add TestPickling

class TestFancyIndexing(TestCase):
    def test_list(self):
        x = ones((1,1))
        x[:,[0]] = 2.0
        return
        assert_array_equal(x, array([[2.0]]))

        x = ones((1,1,1))
        x[:,:,[0]] = 2.0
        assert_array_equal(x, array([[[2.0]]]))

    def _test_tuple(self):
        x = ones((1,1))
        x[:,(0,)] = 2.0
        assert_array_equal(x, array([[2.0]]))
        x = ones((1,1,1))
        x[:,:,(0,)] = 2.0
        assert_array_equal(x, array([[[2.0]]]))

class TestStringCompare(TestCase):
    def test_string(self):
        g1 = array(["This","is","example"])
        g2 = array(["This","was","example"])
        assert_array_equal(g1 == g2, [g1[i] == g2[i] for i in [0,1,2]])
        assert_array_equal(g1 != g2, [g1[i] != g2[i] for i in [0,1,2]])
        assert_array_equal(g1 <= g2, [g1[i] <= g2[i] for i in [0,1,2]])
        assert_array_equal(g1 >= g2, [g1[i] >= g2[i] for i in [0,1,2]])
        assert_array_equal(g1 < g2, [g1[i] < g2[i] for i in [0,1,2]])
        assert_array_equal(g1 > g2, [g1[i] > g2[i] for i in [0,1,2]])

    def test_mixed(self):
        g1 = array(["spam","spa","spammer","and eggs"])
        g2 = "spam"
        assert_array_equal(g1 == g2, [x == g2 for x in g1])
        assert_array_equal(g1 != g2, [x != g2 for x in g1])
        assert_array_equal(g1 < g2, [x < g2 for x in g1])
        assert_array_equal(g1 > g2, [x > g2 for x in g1])
        assert_array_equal(g1 <= g2, [x <= g2 for x in g1])
        assert_array_equal(g1 >= g2, [x >= g2 for x in g1])


    def test_unicode(self):
        g1 = array([u"This",u"is",u"example"])
        g2 = array([u"This",u"was",u"example"])
        assert_array_equal(g1 == g2, [g1[i] == g2[i] for i in [0,1,2]])
        assert_array_equal(g1 != g2, [g1[i] != g2[i] for i in [0,1,2]])
        assert_array_equal(g1 <= g2, [g1[i] <= g2[i] for i in [0,1,2]])
        assert_array_equal(g1 >= g2, [g1[i] >= g2[i] for i in [0,1,2]])
        assert_array_equal(g1 < g2,  [g1[i] < g2[i] for i in [0,1,2]])
        assert_array_equal(g1 > g2,  [g1[i] > g2[i] for i in [0,1,2]])


class TestArgmax(TestCase):
    def test_all(self):
        a = np.empty((4,5,6,7,8))
        a.flat = [ random.gauss(0, 1) for i in xrange(a.size) ]
        for i in xrange(a.ndim):
            amax = a.max(i)
            aargmax = a.argmax(i)
            axes = range(a.ndim)
            axes.remove(i)
            assert all(amax == aargmax.choose(*a.transpose(i,*axes)))

class TestMinMax(TestCase):
    def test_scalar(self):
        self.assertRaises(ValueError, np.amax, 1, 1)
        self.assertRaises(ValueError, np.amin, 1, 1)

        assert_equal(np.amax(1, axis=0), 1)
        assert_equal(np.amin(1, axis=0), 1)
        assert_equal(np.amax(1, axis=None), 1)
        assert_equal(np.amin(1, axis=None), 1)

    def test_axis(self):
        self.assertRaises(ValueError, np.amax, [1,2,3], 1000)
        assert_equal(np.amax([[1,2,3]], axis=1), 3)


class TestNewaxis(TestCase):
    def test_basic(self):
        sk = array([0,-0.1,0.1])
        res = 250*sk[:,newaxis]
        assert_almost_equal(res.ravel(),250*sk)

class TestClip(TestCase):
    def _check_range(self,x,cmin,cmax):
        assert np.all(x >= cmin)
        assert np.all(x <= cmax)

    def _clip_type(self,type_group,array_max,
                   clip_min,clip_max,inplace=False,
                   expected_min=None,expected_max=None):
        if expected_min is None:
            expected_min = clip_min
        if expected_max is None:
            expected_max = clip_max

        for T in np.sctypes[type_group]:
            if sys.byteorder == 'little':
                byte_orders = ['=','>']
            else:
                byte_orders = ['<','=']

            for byteorder in byte_orders:
                dtype = np.dtype(T).newbyteorder(byteorder)

                #x = (np.random.random(1000) * array_max).astype(dtype)
                x = (np.linspace(0.0, 1.0, 1000) * array_max).astype(dtype)
                if inplace:
                    x.clip(clip_min,clip_max,x)
                else:
                    x = x.clip(clip_min,clip_max)
                    byteorder = '='

                if x.dtype.byteorder == '|': byteorder = '|'
                assert_equal(x.dtype.byteorder,byteorder)
                self._check_range(x,expected_min,expected_max)
        return x

    def test_basic(self):
        for inplace in [False, True]:
            self._clip_type('float',1024,-12.8,100.2, inplace=inplace)
            self._clip_type('float',1024,0,0, inplace=inplace)
            self._clip_type('complex',1024,0,0, inplace=inplace)

            self._clip_type('int',1024,-120,100.5, inplace=inplace)
            self._clip_type('int',1024,0,0, inplace=inplace)

            x = self._clip_type('uint',1024,-120,100,expected_min=0, inplace=inplace)
            x = self._clip_type('uint',1024,0,0, inplace=inplace)

    def test_record_array(self):
        rec = np.array([(-5, 2.0, 3.0), (5.0, 4.0, 3.0)],
                      dtype=[('x', '<f8'), ('y', '<f8'), ('z', '<f8')])
        y = rec['x'].clip(-0.3,0.5)
        self._check_range(y,-0.3,0.5)

    def test_max_or_min(self):
        val = np.array([0,1,2,3,4,5,6,7])
        x = val.clip(3)
        assert np.all(x >= 3)
        x = val.clip(min=3)
        assert np.all(x >= 3)
        x = val.clip(max=4)
        assert np.all(x <= 4)


class TestPutmask(TestCase):
    def tst_basic(self,x,T,mask,val):
        np.putmask(x,mask,val)
        assert np.all(x[mask] == T(val))
        assert x.dtype == T

    def test_ip_types(self):
        unchecked_types = [str, unicode, np.void, object]

        x = np.random.random(1000)*100
        mask = x < 40

        for val in [-100,0,15]:
            for types in np.sctypes.itervalues():
                for T in types:
                    if T not in unchecked_types:
                        yield self.tst_basic,x.copy().astype(T),T,mask,val

    def test_mask_size(self):
        self.assertRaises(ValueError, np.putmask,
                              np.array([1,2,3]), [True], 5)

    def tst_byteorder(self,dtype):
        x = np.array([1,2,3],dtype)
        np.putmask(x,[True,False,True],-1)
        assert_array_equal(x,[-1,2,-1])

    def test_ip_byteorder(self):
        for dtype in ('>i4','<i4'):
            yield self.tst_byteorder,dtype

    def test_record_array(self):
        # Note mixed byteorder.
        rec = np.array([(-5, 2.0, 3.0), (5.0, 4.0, 3.0)],
                      dtype=[('x', '<f8'), ('y', '>f8'), ('z', '<f8')])
        np.putmask(rec['x'],[True,False],10)
        assert_array_equal(rec['x'],[10,5])
        np.putmask(rec['y'],[True,False],10)
        assert_array_equal(rec['y'],[10,4])

    def test_masked_array(self):
        ## x = np.array([1,2,3])
        ## z = np.ma.array(x,mask=[True,False,False])
        ## np.putmask(z,[True,True,True],3)
        pass


class TestTake(TestCase):
    def tst_basic(self,x):
        ind = range(x.shape[0])
        assert_array_equal(x.take(ind, axis=0), x)

    def test_ip_types(self):
        unchecked_types = [str, unicode, np.void, object]

        x = np.array([100*random.random() for i in range(24)])
        #x = np.random.random(24)*100
        x.shape = 2,3,4
        for types in np.sctypes.itervalues():
            for T in types:
                if T not in unchecked_types:
                    yield self.tst_basic,x.copy().astype(T)

    def test_raise(self):
        x = np.array([100*random.random() for i in range(24)])
        #x = np.random.random(24)*100
        x.shape = 2,3,4
        self.assertRaises(IndexError, x.take, [0,1,2], axis=0)
        self.assertRaises(IndexError, x.take, [-3], axis=0)
        assert_array_equal(x.take([-1], axis=0)[0], x[1])

    def test_clip(self):
        x = np.array([100*random.random() for i in range(24)])
        #x = np.random.random(24)*100
        x.shape = 2,3,4
        assert_array_equal(x.take([-1], axis=0, mode='clip')[0], x[0])
        assert_array_equal(x.take([2], axis=0, mode='clip')[0], x[1])

    def test_wrap(self):
        x = np.array([100*random.random() for i in range(24)])
        #x = np.random.random(24)*100
        x.shape = 2,3,4
        assert_array_equal(x.take([-1], axis=0, mode='wrap')[0], x[1])
        assert_array_equal(x.take([2], axis=0, mode='wrap')[0], x[0])
        assert_array_equal(x.take([3], axis=0, mode='wrap')[0], x[1])

    def tst_byteorder(self,dtype):
        x = np.array([1,2,3],dtype)
        assert_array_equal(x.take([0,2,1]),[1,3,2])

    def test_ip_byteorder(self):
        for dtype in ('>i4','<i4'):
            yield self.tst_byteorder,dtype

    def test_record_array(self):
        # Note mixed byteorder.
        rec = np.array([(-5, 2.0, 3.0), (5.0, 4.0, 3.0)],
                      dtype=[('x', '<f8'), ('y', '>f8'), ('z', '<f8')])
        rec1 = rec.take([1])
        assert rec1['x'] == 5.0 and rec1['y'] == 4.0


#TODO: Add TestLexsort (needs mergesort from _sortmodule.c

class TestIO(TestCase):
    """Test tofile, fromfile, tostring, and fromstring"""

    def setUp(self):
        shape = (2,4,3)
        self.x = np.empty(shape, dtype=np.complex)
        self.x.flat = [ random.random() + random.random()*1j for i in xrange(self.x.size)]
        self.x[0,:,1] = [nan, inf, -inf, nan]
        self.dtype = self.x.dtype
        self.filename = tempfile.mktemp()

    def tearDown(self):
        if os.path.isfile(self.filename):
            os.unlink(self.filename)
            #tmp_file.close()

    def test_empty_files_binary(self):
        f = open(self.filename, 'w')
        f.close()
        y = fromfile(self.filename)
        assert_(y.size == 0, "Array not empty")

    def test_empty_files_text(self):
        f = open(self.filename, 'w')
        f.close()
        y = fromfile(self.filename, sep=" ")
        assert_(y.size == 0, "Array not empty")

    # TODO: We can't take files as input.
    def _test_roundtrip_file(self):
        f = open(self.filename, 'wb')
        self.x.tofile(f)
        f.close()
        # NB. doesn't work with flush+seek, due to use of C stdio
        f = open(self.filename, 'rb')
        y = np.fromfile(f, dtype=self.dtype)
        f.close()
        assert_array_equal(y, self.x.flat)
        os.unlink(self.filename)

    def test_roundtrip_filename(self):
        self.x.tofile(self.filename)
        y = np.fromfile(self.filename, dtype=self.dtype)
        assert_array_equal(y, self.x.flat)

    def test_roundtrip_binary_str(self):
        s = self.x.tostring()
        y = np.fromstring(s, dtype=self.dtype)
        assert_array_equal(y, self.x.flat)

        s = self.x.tostring('F')
        y = np.fromstring(s, dtype=self.dtype)
        assert_array_equal(y, self.x.flatten('F'))

    def test_roundtrip_str(self):
        x = self.x.real.ravel()
        s = "@".join(map(str, x))
        y = np.fromstring(s, sep="@")
        # NB. str imbues less precision
        nan_mask = ~np.isfinite(x)
        assert_array_equal(x[nan_mask], y[nan_mask])
        assert_array_almost_equal(x[~nan_mask], y[~nan_mask], decimal=5)

    def test_roundtrip_repr(self):
        x = self.x.real.ravel()
        s = "@".join(map(repr, x))
        y = np.fromstring(s, sep="@")
        assert_array_equal(x, y)

    def _check_from(self, s, value, **kw):
        y = np.fromstring(asbytes(s), **kw)
        assert_array_equal(y, value)

        f = open(self.filename, 'wb')
        f.write(asbytes(s))
        f.close()
        y = np.fromfile(self.filename, **kw)
        assert_array_equal(y, value)

    def test_nan(self):
        self._check_from("nan +nan -nan NaN nan(foo) +NaN(BAR) -NAN(q_u_u_x_)",
                         [nan, nan, nan, nan, nan, nan, nan],
                         sep=' ')

    def test_inf(self):
        self._check_from("inf +inf -inf infinity -Infinity iNfInItY -inF",
                         [inf, inf, -inf, inf, -inf, inf, -inf], sep=' ')

    def test_numbers(self):
        self._check_from("1.234 -1.234 .3 .3e55 -123133.1231e+133",
                         [1.234, -1.234, .3, .3e55, -123133.1231e+133], sep=' ')

    def test_binary(self):
        self._check_from(b'\x00\x00\x80?\x00\x00\x00@\x00\x00@@\x00\x00\x80@',
                         array([1,2,3,4]),
                         dtype='<f4')

    def test_string(self):
        self._check_from('1,2,3,4', [1., 2., 3., 4.], sep=',')

    def test_counted_string(self):
        self._check_from('1,2,3,4', [1., 2., 3., 4.], count=4, sep=',')
        self._check_from('1,2,3,4', [1., 2., 3.], count=3, sep=',')
        self._check_from('1,2,3,4', [1., 2., 3., 4.], count=-1, sep=',')

    def test_string_with_ws(self):
        self._check_from('1 2  3     4   ', [1, 2, 3, 4], dtype=int, sep=' ')

    def test_counted_string_with_ws(self):
        self._check_from('1 2  3     4   ', [1,2,3], count=3, dtype=int,
                         sep=' ')

    def test_ascii(self):
        self._check_from('1 , 2 , 3 , 4', [1.,2.,3.,4.], sep=',')
        self._check_from('1,2,3,4', [1.,2.,3.,4.], dtype=float, sep=',')

    def test_malformed(self):
        self._check_from('1.234 1,234', [1.234, 1.], sep=' ')

    def test_long_sep(self):
        self._check_from('1_x_3_x_4_x_5', [1,3,4,5], sep='_x_')

    def test_dtype(self):
        v = np.array([1,2,3,4], dtype=np.int_)
        self._check_from('1,2,3,4', v, sep=',', dtype=np.int_)

    def test_tofile_sep(self):
        x = np.array([1.51, 2, 3.51, 4], dtype=float)
        f = open(self.filename, 'w')
        x.tofile(f, sep=',')
        f.close()
        f = open(self.filename, 'r')
        s = f.read()
        f.close()
        assert_equal(s, '1.51,2.0,3.51,4.0')
        os.unlink(self.filename)

    def test_tofile_format(self):
        x = np.array([1.51, 2, 3.51, 4], dtype=float)
        f = open(self.filename, 'w')
        x.tofile(f, sep=',', format='%.2f')
        f.close()
        f = open(self.filename, 'r')
        s = f.read()
        f.close()
        assert_equal(s, '1.51,2.00,3.51,4.00')

#TODO: Add TestFromBuffer

class TestResize(TestCase):
    def test_basic(self):
        x = np.array([[1, 0, 0], [0, 1, 0], [0, 0, 1]])
        x.resize((5,5))
        assert_array_equal(x.flat[:9],np.array([[1, 0, 0], [0, 1, 0], [0, 0, 1]]).flat)
        assert_array_equal(x[9:].flat,0)

    # TODO: This isn't going to work since we don't have .NET refcounts
    def _test_check_reference(self):
        x = np.array([[1, 0, 0], [0, 1, 0], [0, 0, 1]])
        y = x
        self.assertRaises(ValueError,x.resize,(5,1))

    def test_int_shape(self):
        x = np.eye(3)
        x.resize(3)
        assert_array_equal(x, np.eye(3)[0,:])

    def test_none_shape(self):
        x = np.eye(3)
        x.resize(None)
        assert_array_equal(x, np.eye(3))
        x.resize()
        assert_array_equal(x, np.eye(3))

    def test_invalid_arguements(self):
        # TODO: I had to change the error types to ValueError
        self.assertRaises(ValueError, np.eye(3).resize, 'hi')
        self.assertRaises(ValueError, np.eye(3).resize, -1)
        self.assertRaises(ValueError, np.eye(3).resize, order=1)
        self.assertRaises(ValueError, np.eye(3).resize, refcheck='hi')

    def test_freeform_shape(self):
        x = np.eye(3)
        x.resize(3,2,1)
        assert_(x.shape == (3,2,1))

    def test_zeros_appended(self):
        x = np.eye(3)
        x.resize(2,3,3)
        assert_array_equal(x[0], np.eye(3))
        assert_array_equal(x[1], np.zeros((3,3)))



class TestRecord(TestCase):
    def test_field_rename(self):
        dt = np.dtype([('f',float),('i',int)])
        dt.names = ['p','q']
        assert_equal(dt.names,['p','q'])

    if sys.version_info[0] >= 3:
        def test_bytes_fields(self):
            # Bytes are not allowed in field names and not recognized in titles
            # on Py3
            self.assertRaises(TypeError, np.dtype, [(asbytes('a'), int)])
            self.assertRaises(TypeError, np.dtype, [(('b', asbytes('a')), int)])

            dt = np.dtype([((asbytes('a'), 'b'), int)])
            self.assertRaises(ValueError, dt.__getitem__, asbytes('a'))

            x = np.array([(1,), (2,), (3,)], dtype=dt)
            self.assertRaises(ValueError, x.__getitem__, asbytes('a'))

            y = x[0]
            self.assertRaises(IndexError, y.__getitem__, asbytes('a'))
    elif sys.platform != 'cli':
        def test_unicode_field_titles(self):
            # Unicode field titles are added to field dict on Py2
            title = unicode('b')
            dt = np.dtype([((title, 'a'), int)])
            dt[title]
            dt['a']
            x = np.array([(1,), (2,), (3,)], dtype=dt)
            x[title]
            x['a']
            y = x[0]
            y[title]
            y['a']

        def test_unicode_field_names(self):
            # Unicode field names are not allowed on Py2
            title = unicode('b')
            self.assertRaises(TypeError, np.dtype, [(title, int)])
            self.assertRaises(TypeError, np.dtype, [(('a', title), int)])


class TestView(TestCase):
    def test_basic(self):
        x = np.array([(1,2,3,4),(5,6,7,8)],dtype=[('r',np.int8),('g',np.int8),
                                                  ('b',np.int8),('a',np.int8)])
        # We must be specific about the endianness here:
        y = x.view(dtype='<i4')
        # ... and again without the keyword.
        z = x.view('<i4')
        assert_array_equal(y, z)
        assert_array_equal(y, [67305985, 134678021])


class TestStats(TestCase):
    def test_subclass(self):
        class TestArray(np.ndarray):
            def __new__(cls, data, info):
                result = np.array(data)
                result = result.view(cls)
                result.info = info
                return result
            def __array_finalize__(self, obj):
                self.info = getattr(obj, "info", '')
        dat = TestArray([[1,2,3,4],[5,6,7,8]], 'jubba')
        res = dat.mean(1)
        assert res.info == dat.info
        res = dat.std(1)
        assert res.info == dat.info
        res = dat.var(1)
        assert res.info == dat.info

# TODO: Add TestSummarization. Does string compares on reprs

class TestChoose(TestCase):
    def setUp(self):
        self.x = 2*ones((3,),dtype=int)
        self.y = 3*ones((3,),dtype=int)
        self.x2 = 2*ones((2,3), dtype=int)
        self.y2 = 3*ones((2,3), dtype=int)
        self.ind = [0,0,1]

    def test_basic(self):
        A = np.choose(self.ind, (self.x, self.y))
        assert_equal(A, [2,2,3])

    def test_broadcast1(self):
        A = np.choose(self.ind, (self.x2, self.y2))
        assert_equal(A, [[2,2,3],[2,2,3]])

    def test_broadcast2(self):
        A = np.choose(self.ind, (self.x, self.y2))
        assert_equal(A, [[2,2,3],[2,2,3]])

class TestWarnings(TestCase):
    def setUp(self):
        warnings.resetwarnings()

    def test_complex_warning(self):
        x = np.array([1,2])
        y = np.array([1-2j,1+2j])

        warnings.simplefilter("error", np.ComplexWarning)
        self.assertRaises(np.ComplexWarning, x.__setitem__, slice(None), y)
        warnings.simplefilter("default", np.ComplexWarning)

from numpy.core._internal import _dtype_from_pep3118

class TestPEP3118Dtype(TestCase):
    def _check(self, spec, wanted):
        dt = np.dtype(wanted)
        if isinstance(wanted, list) and isinstance(wanted[-1], tuple):
            if wanted[-1][0] == '':
                names = list(dt.names)
                names[-1] = ''
                dt.names = tuple(names)
        assert_equal(_dtype_from_pep3118(spec), dt,
                     err_msg="spec %r != dtype %r" % (spec, wanted))

    def test_native_padding(self):
        align = np.dtype('i').alignment
        for j in xrange(8):
            if j == 0:
                s = 'bi'
            else:
                s = 'b%dxi' % j
            self._check('@'+s, {'f0': ('i1', 0),
                                'f1': ('i', align*(1 + j//align))})
            self._check('='+s, {'f0': ('i1', 0),
                                'f1': ('i', 1+j)})

    def test_native_padding_2(self):
        # Native padding should work also for structs and sub-arrays
        self._check('x3T{xi}', {'f0': (({'f0': ('i', 4)}, (3,)), 4)})
        self._check('^x3T{xi}', {'f0': (({'f0': ('i', 1)}, (3,)), 1)})

    def test_trailing_padding(self):
        # Trailing padding should be included, *and*, the item size
        # should match the alignment if in aligned mode
        align = np.dtype('i').alignment
        def VV(n):
            return 'V%d' % (align*(1 + (n-1)//align))

        self._check('ix', [('f0', 'i'), ('', VV(1))])
        self._check('ixx', [('f0', 'i'), ('', VV(2))])
        self._check('ixxx', [('f0', 'i'), ('', VV(3))])
        self._check('ixxxx', [('f0', 'i'), ('', VV(4))])
        self._check('i7x', [('f0', 'i'), ('', VV(7))])

        self._check('^ix', [('f0', 'i'), ('', 'V1')])
        self._check('^ixx', [('f0', 'i'), ('', 'V2')])
        self._check('^ixxx', [('f0', 'i'), ('', 'V3')])
        self._check('^ixxxx', [('f0', 'i'), ('', 'V4')])
        self._check('^i7x', [('f0', 'i'), ('', 'V7')])

    def test_byteorder_inside_struct(self):
        # The byte order after @T{=i} should be '=', not '@'.
        # Check this by noting the absence of native alignment.
        self._check('@T{^i}xi', {'f0': ({'f0': ('i', 0)}, 0),
                                 'f1': ('i', 5)})

    def test_intra_padding(self):
        # Natively aligned sub-arrays may require some internal padding
        align = np.dtype('i').alignment
        def VV(n):
            return 'V%d' % (align*(1 + (n-1)//align))

        self._check('(3)T{ix}', ({'f0': ('i', 0), '': (VV(1), 4)}, (3,)))

# TODO: Add TestNewBufferProtocol


if __name__ == '__main__':
    unittest.main()
