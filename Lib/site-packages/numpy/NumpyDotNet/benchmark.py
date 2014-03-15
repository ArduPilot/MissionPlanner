

import sys
import time
from numpy import *


UsingIronPython = False
if sys.subversion[0] == 'IronPython':
    import System
    UsingIronPython = True


#import numbers
#from random import random

class Complex(object):
    def __init__(self, r, i):
        self.__r = r
        self.__i = i

    def __eq__(self, other):
        return (self.__r == other.__r) and (self.__i == other.__i)

    def __ne__(self, other):
        return not (self == other)

    def __add__(self, other):
        return Complex(self.__r + other.__r, self.__i + other.__i)

    def __sub__(self, other):
        return Complex(self.__r - other.__r, self.__i - other.__i)

    def __mul__(self, other):
        return Complex(self.__r * other.__r - self.__i * other.__i, self.__r * other.__i + self.__i * other.__r)

    def __str__(self):
        return "(%f, %f)" % (self.__r, self.__i)




sizes = (10, 100, 1000, 10000, 100000, 1000000)

print "sizes,", ",".join([str(s) for s in sizes])


def random():
    # Resulting distribution isn't very random, could be better.
    return 0.25

def creationTest(iters):
    for size in sizes:
        t0 = time.clock()
        a = 0
        for i in xrange(iters):
            a = ndarray(size)
            a.flat = i
        t1 = time.clock()
        times[size] = t1-t0

    print "creation,", ",".join([str(t) for (s, t) in sorted(times.items())])

def viewCreationTest(iters):
    for size in sizes:
        t0 = time.clock()
        a = ndarray(size)
        for i in xrange(iters):
            b = a[1:-1]
            #b.Dispose()
        t1 = time.clock()
        times[size] = t1-t0

    print "view creation,", ",".join([str(t) for (s, t) in sorted(times.items())])

def basicData():
    tens = []
    twenties = []
    results = []
    for size in sizes:
        a = ndarray(size)
        a[:] = 10
        if a[0] != 10 or a[size-1] != 10:
            print "Error: 'tens' array not initialized correctly (%s, %s)." % (a[0], a[size-1])
        tens.append(a)
        a = ndarray(size)
        a[:] = 20
        twenties.append(a)
        a = ndarray(size)
        results.append(a)
    return tens, twenties, results

    
def multiplyTest(iters):
    tens, twenties, results = basicData()

    for i, size in enumerate(sizes):
        a = tens[i]
        b = twenties[i]
        c = results[i]
        t0 = time.clock()
        for j in xrange(longIter):
            multiply(a, b, c)
        t1 = time.clock()
        times[size] = t1-t0
        if c[0] != 200:
            print "Error: multiply produced incorrect value for c[0] (%s, expected 200)." % c[0]
        if c[size-1] != 200:
            print "Error: multiply produced incorect value for c[-1] (%s, expected 200)." % c[size-1]

    print "multiply,", ",".join([str(t) for (s, t) in sorted(times.items())])


def addTest(iters):
    tens, twenties, results = basicData()

    for i, size in enumerate(sizes):
        a = tens[i]
        b = twenties[i]
        c = results[i]
        t0 = time.clock()
        for j in xrange(iters):
            for k in xrange(size):
                c[k] = a[k] + b[k]
        t1 = time.clock();
        times[size] = t1-t0
        if c[0] != 30 or c[size-1] != 30:
            print "Error: add produced incorrect values for c[0], c[-1]: %s, %s expected 30, 30" % (c[0], c[size-1])
    
    print "add,", ",".join([str(t) for (s, t) in sorted(times.items())])


def derivativeTest(iters):
    # Derivative test
    for size in sizes:
       a = ndarray(size)
       a[0] = 1.0
       for j in range(1, size):
           a[j] = a[j-1] + random() - 0.5            # Simple random walk
       dt = 1.0
	    
       # Time the derivative calc.
       tmp = ndarray(size-1)
       t0 = time.clock()
       dx = ndarray(size-1)
       for j in range(iters):
           try:
                subtract(a[1:], a[:-1], tmp)
                #dx = divide(tmp, dt, dx)
                dx = tmp / dt
           except Exception as e:
               print "j = %s, tmp = %d\ndt = %s" % (j, len(tmp), dt)
               raise e
       t1 = time.clock()
       times[size] = t1-t0
    
       a = 0
       dx = 0
    print "derivative," , ",".join([str(t) for (s, t) in sorted(times.items())])

def convolutionTest(iters):
    # Convolution.
    for i in range(1, len(sizes)):
        size = sizes[i]
        size2 = sizes[0]
        a = array(size)
        b = array(size2)
        a[0] = 1.0
        b[0] = 1.0
        for j in range(1, size):
            a[j] = 1.0 #a[i] + random() - 0.5            # Simple random walk
        for j in range(1, size2):
            b[j] = 1.0 #b[j] + random() - 0.5

        P, Q, N = len(a), len(b), len(a)+len(b)-1
        r1 = range(iters)
        r2 = range(N-1)        
        t0 = time.clock()
        z = ndarray(N, 1)

        #aa = a[0:9]
        #bb = b[0:9]
        tmp = ndarray(9, 1)
        for j in r1:
            for k in r2:
                lower, upper = max(0, k-(Q-1)), min(P-1, k)
                if lower <> upper and upper-lower == 9:
                    aa = a[lower:upper]
                    bb = b[k-upper:k-lower]
                    #tmp = aa * bb
                    multiply(aa, bb, tmp)
                    z[k] = tmp[0]
                    #z[k] = (a[lower:upper] * b[k-upper:k-lower])[0]
                    #tmp.Dispose()
                    #bb.Dispose()
                    #aa.Dispose()

        t1 = time.clock()
        times[size] = t1-t0

    print "convolution,", ",".join([str(t) for (s, t) in sorted(times.items())])


def objectArrayTest(iters):
    # Test again with object types.
    for i, size in enumerate(sizes[:-1]):
        with ndarray(size, 0) as a:
            with ndarray(size, 0) as b:
                with ndarray(size, 0) as c:
                    bValue = Complex(5.2, -3.2)
                    for p in xrange(size):
                        a[p] = Complex(14.2*i, 1.2*i+5.0)
                        b[p] = bValue
                    
                    t0 = time.clock()
                    for j in xrange(iters):
                        subtract(a, b, c)
                    t1 = time.clock()
                    times[size] = t1-t0

    print "object subtract,", ",".join([str(t) for (s, t) in sorted(times.items())])


def collect():
    if UsingIronPython:
        t0 = time.clock()
        System.GC.Collect()
        System.GC.WaitForPendingFinalizers()
        t1 = time.clock()
        print "Garbage collection time: %s" % (t1-t0)

longIter = 100

for k in range(3):
    times = {}

    #time.sleep(10);
    #print "Starting creation test."
    creationTest(longIter)
    collect()
    viewCreationTest(10000)
    collect()

    #print "Multiply test"
    multiplyTest(longIter)
    collect()
        
    #print "Add test"
    addTest(5)
    collect()
        
    #print "Derivative test"
    derivativeTest(2000)
    collect()
    
    #convolutionTest(5)   
    collect()
    
    if 0 and UsingIronPython:
        objectArrayTest(20)
        collect()
        
    
	    
