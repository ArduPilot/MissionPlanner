import numpy as np
from numpy.testing import *

class TestBuiltin(TestCase):
    def test_run(self):
        """Only test hash runs at all."""
        for t in [np.int, np.float, np.complex, np.int32, np.str, np.object,
                np.unicode]:
            dt = np.dtype(t)
            hash(dt)

class TestRecord(TestCase):
    def test_equivalent_record(self):
        """Test whether equivalent record dtypes hash the same."""
        a = np.dtype([('yo', np.int)])
        b = np.dtype([('yo', np.int)])
        self.assertTrue(hash(a) == hash(b), 
                "two equivalent types do not hash to the same value !")

    def test_different_names(self):
        # In theory, they may hash the same (collision) ?
        a = np.dtype([('yo', np.int)])
        b = np.dtype([('ye', np.int)])
        self.assertTrue(hash(a) != hash(b),
                "%s and %s hash the same !" % (a, b))

    def test_different_titles(self):
        # In theory, they may hash the same (collision) ?
        a = np.dtype({'names': ['r','b'], 'formats': ['u1', 'u1'],
            'titles': ['Red pixel', 'Blue pixel']})
        b = np.dtype({'names': ['r','b'], 'formats': ['u1', 'u1'],
            'titles': ['RRed pixel', 'Blue pixel']})
        self.assertTrue(hash(a) != hash(b),
                "%s and %s hash the same !" % (a, b))

    def test_not_lists(self):
        """Test if an appropriate exception is raised when passing bad values to
        the dtype constructor.
        """
        self.assertRaises(TypeError, np.dtype,
            dict(names=set(['A', 'B']), formats=['f8', 'i4']))
        self.assertRaises(TypeError, np.dtype,
            dict(names=['A', 'B'], formats=set(['f8', 'i4'])))

class TestSubarray(TestCase):
    def test_single_subarray(self):
        a = np.dtype((np.int, (2)))
        b = np.dtype((np.int, (2,)))
        self.assertTrue(hash(a) == hash(b), 
                "two equivalent types do not hash to the same value !")

    def test_equivalent_record(self):
        """Test whether equivalent subarray dtypes hash the same."""
        a = np.dtype((np.int, (2, 3)))
        b = np.dtype((np.int, (2, 3)))
        self.assertTrue(hash(a) == hash(b), 
                "two equivalent types do not hash to the same value !")

    def test_nonequivalent_record(self):
        """Test whether different subarray dtypes hash differently."""
        a = np.dtype((np.int, (2, 3)))
        b = np.dtype((np.int, (3, 2)))
        self.assertTrue(hash(a) != hash(b), 
                "%s and %s hash the same !" % (a, b))

        a = np.dtype((np.int, (2, 3)))
        b = np.dtype((np.int, (2, 2)))
        self.assertTrue(hash(a) != hash(b), 
                "%s and %s hash the same !" % (a, b))

        a = np.dtype((np.int, (1, 2, 3)))
        b = np.dtype((np.int, (1, 2)))
        self.assertTrue(hash(a) != hash(b), 
                "%s and %s hash the same !" % (a, b))

class TestMonsterType(TestCase):
    """Test deeply nested subtypes."""
    def test1(self):
        simple1 = np.dtype({'names': ['r','b'], 'formats': ['u1', 'u1'],
            'titles': ['Red pixel', 'Blue pixel']})
        a = np.dtype([('yo', np.int), ('ye', simple1),
            ('yi', np.dtype((np.int, (3, 2))))])
        b = np.dtype([('yo', np.int), ('ye', simple1),
            ('yi', np.dtype((np.int, (3, 2))))])
        self.assertTrue(hash(a) == hash(b))

        c = np.dtype([('yo', np.int), ('ye', simple1),
            ('yi', np.dtype((a, (3, 2))))])
        d = np.dtype([('yo', np.int), ('ye', simple1),
            ('yi', np.dtype((a, (3, 2))))])
        self.assertTrue(hash(c) == hash(d))

class TestBasicFunctions(TestCase):
    def test_compare(self):
        a = np.dtype('i')
        b = np.dtype('i')
        self.assertTrue(a == b)

        a = np.dtype([('one', np.dtype('d')), ('two', np.dtype('i'))])
        b = np.dtype([('one', np.dtype('d')), ('two', np.dtype('i'))])
        c = np.dtype([('two', np.dtype('i')), ('one', np.dtype('d'))])
        self.assertTrue(a == a)
        self.assertTrue(a == b)
        self.assertFalse(b == c)

        self.assertFalse(a != a)
        self.assertFalse(a != b)
        self.assertTrue(b != c)
        
        # Try using the repeat operation and make sure the base is correct.
        c = b * 3
        self.assertFalse(c == b)
        self.assertTrue(c.base == b)

    def test_seq(self):
        a = np.dtype([('one', np.dtype('d')), ('two', np.dtype('i'))])
        self.assertTrue(a[0] == np.dtype('d'))
        self.assertTrue(a['two'] == np.dtype('i'))
        self.assertFalse(a['two'] == np.dtype('d'))
        try:
            x = a[2]
            self.assertTrue(False, "Failed to catch index out of range exception.")
        except:
            pass

        try:
            x = a['foo']
            self.assertTrue(False, 'Failed to catch incorrect field name exception.')
        except:
            pass

        # Make sure scalar int values work as index values.
        arr = np.arange(4)
        self.assertTrue(a[arr[0]] == np.dtype('d'))
        self.assertTrue(a[arr[1]] == np.dtype('i'))
        try:
            x = a[arr[2]]
            self.assertTrue(False, 'Failed to catch index out of range exception using ScalarInt index value.')
        except:
            pass


if __name__ == '__main__':
    import unittest
    unittest.main()
