from numpy.testing import *
from numpy import random
import numpy as np

class TestRegression(TestCase):

    def test_VonMises_range(self):
        """Make sure generated random variables are in [-pi, pi].

        Regression test for ticket #986.
        """
        for mu in np.linspace(-7., 7., 5):
            r = random.mtrand.vonmises(mu,1,50)
            assert np.all(r > -np.pi) and np.all(r <= np.pi)

    def test_hypergeometric_range(self) :
        """Test for ticket #921"""
        assert_(np.all(np.random.hypergeometric(3, 18, 11, size=10) < 4))
        assert_(np.all(np.random.hypergeometric(18, 3, 11, size=10) > 0))

    def test_logseries_convergence(self) :
        """Test for ticket #923"""
        N = 1000
        np.random.seed(0)
        rvsn = np.random.logseries(0.8, size=N)
        # these two frequency counts should be close to theoretical
        # numbers with this large sample
        # theoretical large N result is 0.49706795
        freq = np.sum(rvsn == 1) / float(N)
        msg = "Frequency was %f, should be > 0.45" % freq
        assert_(freq > 0.45, msg)
        # theoretical large N result is 0.19882718
        freq = np.sum(rvsn == 2) / float(N)
        msg = "Frequency was %f, should be < 0.23" % freq
        assert_(freq < 0.23, msg)

    def test_permutation_longs(self):
        np.random.seed(1234)
        a = np.random.permutation(12)
        np.random.seed(1234)
        b = np.random.permutation(12L)
        assert_array_equal(a, b)

class TestMultinomial(TestCase):
    def test_basic(self):
        random.multinomial(100, [0.2, 0.8])

    def test_zero_probability(self):
        random.multinomial(100, [0.2, 0.8, 0.0, 0.0, 0.0])

    def test_int_negative_interval(self):
        assert -5 <= random.randint(-5,-1) < -1
        x = random.randint(-5,-1,5)
        assert np.all(-5 <= x)
        assert np.all(x < -1)



class TestSetState(TestCase):
    def setUp(self):
        self.seed = 1234567890
        self.prng = random.RandomState(self.seed)
        self.state = self.prng.get_state()

    def test_basic(self):
        old = self.prng.tomaxint(16)
        self.prng.set_state(self.state)
        new = self.prng.tomaxint(16)
        assert np.all(old == new)

    def test_gaussian_reset(self):
        """ Make sure the cached every-other-Gaussian is reset.
        """
        old = self.prng.standard_normal(size=3)
        self.prng.set_state(self.state)
        new = self.prng.standard_normal(size=3)
        assert np.all(old == new)

    def test_gaussian_reset_in_media_res(self):
        """ When the state is saved with a cached Gaussian, make sure the cached
        Gaussian is restored.
        """
        self.prng.standard_normal()
        state = self.prng.get_state()
        old = self.prng.standard_normal(size=3)
        self.prng.set_state(state)
        new = self.prng.standard_normal(size=3)
        assert np.all(old == new)

    def test_backwards_compatibility(self):
        """ Make sure we can accept old state tuples that do not have the cached
        Gaussian value.
        """
        old_state = self.state[:-2]
        x1 = self.prng.standard_normal(size=16)
        self.prng.set_state(old_state)
        x2 = self.prng.standard_normal(size=16)
        self.prng.set_state(self.state)
        x3 = self.prng.standard_normal(size=16)
        assert np.all(x1 == x2)
        assert np.all(x1 == x3)

    def test_negative_binomial(self):
        """ Ensure that the negative binomial results take floating point
        arguments without truncation.
        """
        self.prng.negative_binomial(0.5, 0.5)


class Test_MTRand(TestCase):

    def test_binomial(self):
        n, p, N = 10, 0.5, 1000
        sample = random.binomial(n, p, N)
        avg = 1.0 * sum(sample) / N / n
        self.assert_(0.45 < avg < 0.55)
        self.assertRaises(ValueError, random.binomial, -1, 0.5)
        self.assertRaises(ValueError, random.binomial, 10, -0.1)
        self.assertRaises(ValueError, random.binomial, 10, 1.1)

    def test_binomial_array(self):
        # See if first argument can also be an array
        n = np.array([10, 20, 5])
        p = np.array([0.4, 0.7, 0.2])

        b = random.binomial(n, 0.4)
        self.assert_(b.shape, (3,))

        b = random.binomial(5, p)
        self.assert_(b.shape, (3,))

        b = random.binomial(n, p)
        self.assert_(b.shape, (3,))

        self.assertRaises(ValueError, random.binomial, n, -0.1)
        n = np.array([10, -20, 5])
        self.assertRaises(ValueError, random.binomial, n, 0.4)

    def test_randn(self):
        random.seed(0)
        a = random.randn(3)
        self.assertEqual(repr(a),
                         'array([ 1.76405235,  0.40015721,  0.97873798])')

    def test_bytes(self):
        random.seed(101)
        self.assertEqual(random.bytes(10), '_\xb32\x84\x0bFs\x8dQE')

    def test_uniform(self):
        a = random.uniform(size=1000)
        self.assertEqual(a.shape, (1000,))
        self.assert_(450 < sum(a) < 550)
        a = random.uniform(high=[1, 10])
        self.assertEqual(a.shape, (2,))
        self.assertRaises(ValueError, random.uniform, 0, [1, 10], (25,))
        high = [1, 5, 10]
        a = random.uniform(high=high, size=(1000, len(high)))
        for i, h in enumerate(high):
            self.assert_(450 * h < sum(a[..., i]) < 550 * h)
        a = random.uniform(high=[[1, 10], [2, 20]])
        self.assertEqual(a.shape, (2, 2))

    def test_randint(self):
        self.assertEqual(random.randint(1), 0)
        for dum in xrange(10):
            self.assert_(0 <= random.randint(5) < 5)
        a = random.randint(5, size=100)
        self.assert_(np.all(np.less(a, 5)))
        self.assert_(np.all(np.greater_equal(a, 0)))
        self.assertEqual(min(a), 0)
        self.assertEqual(max(a), 4)
        shape = 3, 4, 2
        a = random.randint(5, size=shape)
        self.assertEqual(a.shape, shape)

    def test_random_integers(self):
        self.assertEqual(random.random_integers(1), 1)
        a = random.random_integers(10, size=(4, 5))
        self.assertEqual(a.shape, (4, 5))

    def test_multivariate_normal(self):
        mean = [0, 0]
        cov = [[1, 0], [0, 100]]
        a = np.random.multivariate_normal(mean, cov, 100)
        self.assertEqual(a.shape, (100, 2))


if __name__ == "__main__":
    run_module_suite()
