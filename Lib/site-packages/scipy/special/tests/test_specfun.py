# Corresponds to a small subset of scipy.special.specfun
# that has features that were ported manually from fwrap
# to f2py.

# It was assumed that SciPy 0.7.0 shipped with Ubuntu Lucid
# (using f2py) returned the correct values.
#


import numpy as np
from numpy import array, isnan, r_, arange, finfo, pi, sin, cos, tan, exp, log, zeros, \
        sqrt, asarray, inf, nan_to_num, real, arctan, float_

from numpy.testing import assert_equal, assert_almost_equal, assert_array_equal, \
        assert_array_almost_equal, assert_approx_equal, assert_, \
        rand, dec, TestCase, run_module_suite, assert_raises
from scipy.special import specfun
from testutils import assert_tol_equal, with_special_errors

def assert_not_raises(func, *args, **kw):
    # since test failure and test error is not the same
    try:
        func(*args, **kw)
    except:
        assert_(False)

class TestSpecfun(TestCase):
    def test_clqmn(self):
        a, b = specfun.clqmn(2, 2, 1.0+1.0j)
        assert_tol_equal(a,
                         [[(0.40235947810852513-0.5535743588970452j),
                         (-0.044066162994429635-0.15121488078852013j),
                         (-0.040456662363126783-0.016134386225902096j)],
                         [(-0.35157758425414298+0.56886448100578324j),
                         (0.10003085479304373+0.29390281413581559j),
                         (0.12153929047997031+0.044072044775011442j)],
                         [(0.40000000000000002-1.2000000000000004j),
                         (-0.40000000000000008-0.80000000000000004j),
                         (-0.48563228094330374-0.12512005465771367j)]])

        assert_tol_equal(b,
                         [[(0.20000000000000001+0.40000000000000002j),
                         (0.20235947810852509+0.046425641102954752j),
                         (0.067801511016711016-0.053644642365560456j)],
                         [(-0.27100317175264133-0.32471944675364245j),
                         (-0.39937475906618558-0.054954225049664206j),
                         (-0.19116269416515053+0.16816038356718191j)],
                         [(0.88000000000000012+0.16000000000000003j),
                         (1.1200000000000001-0.16000000000000003j),
                         (0.61560302203342199-0.747289284731121j)]])

        # Integer (and real) argument
        assert_tol_equal(specfun.clqmn(1, 1, 3)[0],
                         [[(0.34657359027997264+0j),
                         (0.039720770839917957+0j)],
                         [(-0.35355339059327373+0j),
                         (-0.080402028311274076+0j)]])
        
    def test_clqn(self):
        a, b = specfun.clqn(2, 1.0+2.0j)
        
        assert_tol_equal(a,
                        [(0.17328679513998635-0.3926990816987242j),
                        (-0.041315041462565365-0.046125491418751496j),
                        (-0.010239485507586708+0.0032161793335387366j)])

        assert_tol_equal(b, [(0.125+0.125j),
                         (0.048286795139986335-0.017699081698724153j),
                         (0.0010548756123039084-0.0133764742562545j)])

    def test_clpn(self):
        a, b = specfun.clpn(2, 1+2j)
        assert_tol_equal(a, [(1+0j), (1+2j), (-5+6j)])
        assert_tol_equal(b, [0j, (1+0j), (3+6j)])

    def test_check(self):
        assert_raises(Exception, specfun.lamn, -1, 1.2)
        assert_raises(Exception, specfun.lamn, 0, 1.2)
        assert_not_raises(specfun.lamn, 1, 1.2)

        assert_raises(Exception, specfun.fcszo, 0, 3)
        assert_raises(Exception, specfun.fcszo, 1, 0)
        assert_not_raises(specfun.fcszo, 1, 1)

if __name__ == "__main__":
    run_module_suite()
