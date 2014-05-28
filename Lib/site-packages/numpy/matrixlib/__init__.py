"""Sub-package containing the matrix class and related functions."""
from defmatrix import *

__all__ = defmatrix.__all__

from numpy.testing import Tester
test = Tester(__file__).test
bench = Tester(__file__).bench
