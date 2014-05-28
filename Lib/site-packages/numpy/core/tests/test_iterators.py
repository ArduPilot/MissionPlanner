
import numpy as np
from numpy.testing import *


class TestBroadcast(TestCase):
    """ Tests np.broadcast. """

    def _broadcast_add(self, args, dtype):
        """ Uses a multiter to do the equiv of a ufunc add. """
        b = np.broadcast(*args)
        result = np.empty(b.size, dtype=dtype)
        flat_arrays = [ a.ravel() for a in args ]
        for i, index in enumerate(b):
            val = 0
            for j, array in zip(index, flat_arrays):
                val += array[j]
            result[i] = val

        return result.reshape(b.shape)

    def test_broadcast_add(self):
        a = np.arange(10)
        b = np.arange(100).reshape(10,10)

        assert_array_equal(a+b, self._broadcast_add((a,b), a.dtype))


if __name__ == "__main__":
    run_module_suite()

