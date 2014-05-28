# To get sub-modules
from info import __doc__

from fftpack import *
from helper import *

from numpy.testing import Tester
test = Tester(__file__).test
bench = Tester(__file__).bench
