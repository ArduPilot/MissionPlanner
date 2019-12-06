/*************************************************************************
ALGLIB 3.14.0 (source code generated 2018-06-16)
Copyright (c) Sergey Bochkanov (ALGLIB project).

>>> SOURCE LICENSE >>>
This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation (www.fsf.org); either version 2 of the 
License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

A copy of the GNU General Public License is available at
http://www.fsf.org/licensing/licenses
>>> END OF LICENSE >>>
*************************************************************************/
#pragma warning disable 162
#pragma warning disable 164
#pragma warning disable 219
using System;

public partial class alglib
{

    
    /*************************************************************************
    1-dimensional complex FFT.

    Array size N may be arbitrary number (composite or prime).  Composite  N's
    are handled with cache-oblivious variation of  a  Cooley-Tukey  algorithm.
    Small prime-factors are transformed using hard coded  codelets (similar to
    FFTW codelets, but without low-level  optimization),  large  prime-factors
    are handled with Bluestein's algorithm.

    Fastests transforms are for smooth N's (prime factors are 2, 3,  5  only),
    most fast for powers of 2. When N have prime factors  larger  than  these,
    but orders of magnitude smaller than N, computations will be about 4 times
    slower than for nearby highly composite N's. When N itself is prime, speed
    will be 6 times lower.

    Algorithm has O(N*logN) complexity for any N (composite or prime).

    INPUT PARAMETERS
        A   -   array[0..N-1] - complex function to be transformed
        N   -   problem size

    OUTPUT PARAMETERS
        A   -   DFT of a input array, array[0..N-1]
                A_out[j] = SUM(A_in[k]*exp(-2*pi*sqrt(-1)*j*k/N), k = 0..N-1)


      -- ALGLIB --
         Copyright 29.05.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void fftc1d(ref complex[] a, int n)
    {
    
        fft.fftc1d(ref a, n, null);
    }
    
    public static void fftc1d(ref complex[] a, int n, alglib.xparams _params)
    {
    
        fft.fftc1d(ref a, n, _params);
    }
            
    public static void fftc1d(ref complex[] a)
    {
        int n;
    
    
        n = ap.len(a);
        fft.fftc1d(ref a, n, null);
    
        return;
    }
            
    public static void fftc1d(ref complex[] a, alglib.xparams _params)
    {
        int n;
    
    
        n = ap.len(a);
        fft.fftc1d(ref a, n, _params);
    
        return;
    }
    
    /*************************************************************************
    1-dimensional complex inverse FFT.

    Array size N may be arbitrary number (composite or prime).  Algorithm  has
    O(N*logN) complexity for any N (composite or prime).

    See FFTC1D() description for more information about algorithm performance.

    INPUT PARAMETERS
        A   -   array[0..N-1] - complex array to be transformed
        N   -   problem size

    OUTPUT PARAMETERS
        A   -   inverse DFT of a input array, array[0..N-1]
                A_out[j] = SUM(A_in[k]/N*exp(+2*pi*sqrt(-1)*j*k/N), k = 0..N-1)


      -- ALGLIB --
         Copyright 29.05.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void fftc1dinv(ref complex[] a, int n)
    {
    
        fft.fftc1dinv(ref a, n, null);
    }
    
    public static void fftc1dinv(ref complex[] a, int n, alglib.xparams _params)
    {
    
        fft.fftc1dinv(ref a, n, _params);
    }
            
    public static void fftc1dinv(ref complex[] a)
    {
        int n;
    
    
        n = ap.len(a);
        fft.fftc1dinv(ref a, n, null);
    
        return;
    }
            
    public static void fftc1dinv(ref complex[] a, alglib.xparams _params)
    {
        int n;
    
    
        n = ap.len(a);
        fft.fftc1dinv(ref a, n, _params);
    
        return;
    }
    
    /*************************************************************************
    1-dimensional real FFT.

    Algorithm has O(N*logN) complexity for any N (composite or prime).

    INPUT PARAMETERS
        A   -   array[0..N-1] - real function to be transformed
        N   -   problem size

    OUTPUT PARAMETERS
        F   -   DFT of a input array, array[0..N-1]
                F[j] = SUM(A[k]*exp(-2*pi*sqrt(-1)*j*k/N), k = 0..N-1)

    NOTE:
        F[] satisfies symmetry property F[k] = conj(F[N-k]),  so just one half
    of  array  is  usually needed. But for convinience subroutine returns full
    complex array (with frequencies above N/2), so its result may be  used  by
    other FFT-related subroutines.


      -- ALGLIB --
         Copyright 01.06.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void fftr1d(double[] a, int n, out complex[] f)
    {
        f = new complex[0];
        fft.fftr1d(a, n, ref f, null);
    }
    
    public static void fftr1d(double[] a, int n, out complex[] f, alglib.xparams _params)
    {
        f = new complex[0];
        fft.fftr1d(a, n, ref f, _params);
    }
            
    public static void fftr1d(double[] a, out complex[] f)
    {
        int n;
    
        f = new complex[0];
        n = ap.len(a);
        fft.fftr1d(a, n, ref f, null);
    
        return;
    }
            
    public static void fftr1d(double[] a, out complex[] f, alglib.xparams _params)
    {
        int n;
    
        f = new complex[0];
        n = ap.len(a);
        fft.fftr1d(a, n, ref f, _params);
    
        return;
    }
    
    /*************************************************************************
    1-dimensional real inverse FFT.

    Algorithm has O(N*logN) complexity for any N (composite or prime).

    INPUT PARAMETERS
        F   -   array[0..floor(N/2)] - frequencies from forward real FFT
        N   -   problem size

    OUTPUT PARAMETERS
        A   -   inverse DFT of a input array, array[0..N-1]

    NOTE:
        F[] should satisfy symmetry property F[k] = conj(F[N-k]), so just  one
    half of frequencies array is needed - elements from 0 to floor(N/2).  F[0]
    is ALWAYS real. If N is even F[floor(N/2)] is real too. If N is odd,  then
    F[floor(N/2)] has no special properties.

    Relying on properties noted above, FFTR1DInv subroutine uses only elements
    from 0th to floor(N/2)-th. It ignores imaginary part of F[0],  and in case
    N is even it ignores imaginary part of F[floor(N/2)] too.

    When you call this function using full arguments list - "FFTR1DInv(F,N,A)"
    - you can pass either either frequencies array with N elements or  reduced
    array with roughly N/2 elements - subroutine will  successfully  transform
    both.

    If you call this function using reduced arguments list -  "FFTR1DInv(F,A)"
    - you must pass FULL array with N elements (although higher  N/2 are still
    not used) because array size is used to automatically determine FFT length


      -- ALGLIB --
         Copyright 01.06.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void fftr1dinv(complex[] f, int n, out double[] a)
    {
        a = new double[0];
        fft.fftr1dinv(f, n, ref a, null);
    }
    
    public static void fftr1dinv(complex[] f, int n, out double[] a, alglib.xparams _params)
    {
        a = new double[0];
        fft.fftr1dinv(f, n, ref a, _params);
    }
            
    public static void fftr1dinv(complex[] f, out double[] a)
    {
        int n;
    
        a = new double[0];
        n = ap.len(f);
        fft.fftr1dinv(f, n, ref a, null);
    
        return;
    }
            
    public static void fftr1dinv(complex[] f, out double[] a, alglib.xparams _params)
    {
        int n;
    
        a = new double[0];
        n = ap.len(f);
        fft.fftr1dinv(f, n, ref a, _params);
    
        return;
    }

}
public partial class alglib
{

    
    /*************************************************************************
    1-dimensional Fast Hartley Transform.

    Algorithm has O(N*logN) complexity for any N (composite or prime).

    INPUT PARAMETERS
        A   -   array[0..N-1] - real function to be transformed
        N   -   problem size

    OUTPUT PARAMETERS
        A   -   FHT of a input array, array[0..N-1],
                A_out[k] = sum(A_in[j]*(cos(2*pi*j*k/N)+sin(2*pi*j*k/N)), j=0..N-1)


      -- ALGLIB --
         Copyright 04.06.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void fhtr1d(ref double[] a, int n)
    {
    
        fht.fhtr1d(ref a, n, null);
    }
    
    public static void fhtr1d(ref double[] a, int n, alglib.xparams _params)
    {
    
        fht.fhtr1d(ref a, n, _params);
    }
    
    /*************************************************************************
    1-dimensional inverse FHT.

    Algorithm has O(N*logN) complexity for any N (composite or prime).

    INPUT PARAMETERS
        A   -   array[0..N-1] - complex array to be transformed
        N   -   problem size

    OUTPUT PARAMETERS
        A   -   inverse FHT of a input array, array[0..N-1]


      -- ALGLIB --
         Copyright 29.05.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void fhtr1dinv(ref double[] a, int n)
    {
    
        fht.fhtr1dinv(ref a, n, null);
    }
    
    public static void fhtr1dinv(ref double[] a, int n, alglib.xparams _params)
    {
    
        fht.fhtr1dinv(ref a, n, _params);
    }

}
public partial class alglib
{

    
    /*************************************************************************
    1-dimensional complex convolution.

    For given A/B returns conv(A,B) (non-circular). Subroutine can automatically
    choose between three implementations: straightforward O(M*N)  formula  for
    very small N (or M), overlap-add algorithm for  cases  where  max(M,N)  is
    significantly larger than min(M,N), but O(M*N) algorithm is too slow,  and
    general FFT-based formula for cases where two previois algorithms are  too
    slow.

    Algorithm has max(M,N)*log(max(M,N)) complexity for any M/N.

    INPUT PARAMETERS
        A   -   array[0..M-1] - complex function to be transformed
        M   -   problem size
        B   -   array[0..N-1] - complex function to be transformed
        N   -   problem size

    OUTPUT PARAMETERS
        R   -   convolution: A*B. array[0..N+M-2].

    NOTE:
        It is assumed that A is zero at T<0, B is zero too.  If  one  or  both
    functions have non-zero values at negative T's, you  can  still  use  this
    subroutine - just shift its result correspondingly.

      -- ALGLIB --
         Copyright 21.07.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void convc1d(complex[] a, int m, complex[] b, int n, out complex[] r)
    {
        r = new complex[0];
        conv.convc1d(a, m, b, n, ref r, null);
    }
    
    public static void convc1d(complex[] a, int m, complex[] b, int n, out complex[] r, alglib.xparams _params)
    {
        r = new complex[0];
        conv.convc1d(a, m, b, n, ref r, _params);
    }
    
    /*************************************************************************
    1-dimensional complex non-circular deconvolution (inverse of ConvC1D()).

    Algorithm has M*log(M)) complexity for any M (composite or prime).

    INPUT PARAMETERS
        A   -   array[0..M-1] - convolved signal, A = conv(R, B)
        M   -   convolved signal length
        B   -   array[0..N-1] - response
        N   -   response length, N<=M

    OUTPUT PARAMETERS
        R   -   deconvolved signal. array[0..M-N].

    NOTE:
        deconvolution is unstable process and may result in division  by  zero
    (if your response function is degenerate, i.e. has zero Fourier coefficient).

    NOTE:
        It is assumed that A is zero at T<0, B is zero too.  If  one  or  both
    functions have non-zero values at negative T's, you  can  still  use  this
    subroutine - just shift its result correspondingly.

      -- ALGLIB --
         Copyright 21.07.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void convc1dinv(complex[] a, int m, complex[] b, int n, out complex[] r)
    {
        r = new complex[0];
        conv.convc1dinv(a, m, b, n, ref r, null);
    }
    
    public static void convc1dinv(complex[] a, int m, complex[] b, int n, out complex[] r, alglib.xparams _params)
    {
        r = new complex[0];
        conv.convc1dinv(a, m, b, n, ref r, _params);
    }
    
    /*************************************************************************
    1-dimensional circular complex convolution.

    For given S/R returns conv(S,R) (circular). Algorithm has linearithmic
    complexity for any M/N.

    IMPORTANT:  normal convolution is commutative,  i.e.   it  is symmetric  -
    conv(A,B)=conv(B,A).  Cyclic convolution IS NOT.  One function - S - is  a
    signal,  periodic function, and another - R - is a response,  non-periodic
    function with limited length.

    INPUT PARAMETERS
        S   -   array[0..M-1] - complex periodic signal
        M   -   problem size
        B   -   array[0..N-1] - complex non-periodic response
        N   -   problem size

    OUTPUT PARAMETERS
        R   -   convolution: A*B. array[0..M-1].

    NOTE:
        It is assumed that B is zero at T<0. If  it  has  non-zero  values  at
    negative T's, you can still use this subroutine - just  shift  its  result
    correspondingly.

      -- ALGLIB --
         Copyright 21.07.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void convc1dcircular(complex[] s, int m, complex[] r, int n, out complex[] c)
    {
        c = new complex[0];
        conv.convc1dcircular(s, m, r, n, ref c, null);
    }
    
    public static void convc1dcircular(complex[] s, int m, complex[] r, int n, out complex[] c, alglib.xparams _params)
    {
        c = new complex[0];
        conv.convc1dcircular(s, m, r, n, ref c, _params);
    }
    
    /*************************************************************************
    1-dimensional circular complex deconvolution (inverse of ConvC1DCircular()).

    Algorithm has M*log(M)) complexity for any M (composite or prime).

    INPUT PARAMETERS
        A   -   array[0..M-1] - convolved periodic signal, A = conv(R, B)
        M   -   convolved signal length
        B   -   array[0..N-1] - non-periodic response
        N   -   response length

    OUTPUT PARAMETERS
        R   -   deconvolved signal. array[0..M-1].

    NOTE:
        deconvolution is unstable process and may result in division  by  zero
    (if your response function is degenerate, i.e. has zero Fourier coefficient).

    NOTE:
        It is assumed that B is zero at T<0. If  it  has  non-zero  values  at
    negative T's, you can still use this subroutine - just  shift  its  result
    correspondingly.

      -- ALGLIB --
         Copyright 21.07.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void convc1dcircularinv(complex[] a, int m, complex[] b, int n, out complex[] r)
    {
        r = new complex[0];
        conv.convc1dcircularinv(a, m, b, n, ref r, null);
    }
    
    public static void convc1dcircularinv(complex[] a, int m, complex[] b, int n, out complex[] r, alglib.xparams _params)
    {
        r = new complex[0];
        conv.convc1dcircularinv(a, m, b, n, ref r, _params);
    }
    
    /*************************************************************************
    1-dimensional real convolution.

    Analogous to ConvC1D(), see ConvC1D() comments for more details.

    INPUT PARAMETERS
        A   -   array[0..M-1] - real function to be transformed
        M   -   problem size
        B   -   array[0..N-1] - real function to be transformed
        N   -   problem size

    OUTPUT PARAMETERS
        R   -   convolution: A*B. array[0..N+M-2].

    NOTE:
        It is assumed that A is zero at T<0, B is zero too.  If  one  or  both
    functions have non-zero values at negative T's, you  can  still  use  this
    subroutine - just shift its result correspondingly.

      -- ALGLIB --
         Copyright 21.07.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void convr1d(double[] a, int m, double[] b, int n, out double[] r)
    {
        r = new double[0];
        conv.convr1d(a, m, b, n, ref r, null);
    }
    
    public static void convr1d(double[] a, int m, double[] b, int n, out double[] r, alglib.xparams _params)
    {
        r = new double[0];
        conv.convr1d(a, m, b, n, ref r, _params);
    }
    
    /*************************************************************************
    1-dimensional real deconvolution (inverse of ConvC1D()).

    Algorithm has M*log(M)) complexity for any M (composite or prime).

    INPUT PARAMETERS
        A   -   array[0..M-1] - convolved signal, A = conv(R, B)
        M   -   convolved signal length
        B   -   array[0..N-1] - response
        N   -   response length, N<=M

    OUTPUT PARAMETERS
        R   -   deconvolved signal. array[0..M-N].

    NOTE:
        deconvolution is unstable process and may result in division  by  zero
    (if your response function is degenerate, i.e. has zero Fourier coefficient).

    NOTE:
        It is assumed that A is zero at T<0, B is zero too.  If  one  or  both
    functions have non-zero values at negative T's, you  can  still  use  this
    subroutine - just shift its result correspondingly.

      -- ALGLIB --
         Copyright 21.07.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void convr1dinv(double[] a, int m, double[] b, int n, out double[] r)
    {
        r = new double[0];
        conv.convr1dinv(a, m, b, n, ref r, null);
    }
    
    public static void convr1dinv(double[] a, int m, double[] b, int n, out double[] r, alglib.xparams _params)
    {
        r = new double[0];
        conv.convr1dinv(a, m, b, n, ref r, _params);
    }
    
    /*************************************************************************
    1-dimensional circular real convolution.

    Analogous to ConvC1DCircular(), see ConvC1DCircular() comments for more details.

    INPUT PARAMETERS
        S   -   array[0..M-1] - real signal
        M   -   problem size
        B   -   array[0..N-1] - real response
        N   -   problem size

    OUTPUT PARAMETERS
        R   -   convolution: A*B. array[0..M-1].

    NOTE:
        It is assumed that B is zero at T<0. If  it  has  non-zero  values  at
    negative T's, you can still use this subroutine - just  shift  its  result
    correspondingly.

      -- ALGLIB --
         Copyright 21.07.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void convr1dcircular(double[] s, int m, double[] r, int n, out double[] c)
    {
        c = new double[0];
        conv.convr1dcircular(s, m, r, n, ref c, null);
    }
    
    public static void convr1dcircular(double[] s, int m, double[] r, int n, out double[] c, alglib.xparams _params)
    {
        c = new double[0];
        conv.convr1dcircular(s, m, r, n, ref c, _params);
    }
    
    /*************************************************************************
    1-dimensional complex deconvolution (inverse of ConvC1D()).

    Algorithm has M*log(M)) complexity for any M (composite or prime).

    INPUT PARAMETERS
        A   -   array[0..M-1] - convolved signal, A = conv(R, B)
        M   -   convolved signal length
        B   -   array[0..N-1] - response
        N   -   response length

    OUTPUT PARAMETERS
        R   -   deconvolved signal. array[0..M-N].

    NOTE:
        deconvolution is unstable process and may result in division  by  zero
    (if your response function is degenerate, i.e. has zero Fourier coefficient).

    NOTE:
        It is assumed that B is zero at T<0. If  it  has  non-zero  values  at
    negative T's, you can still use this subroutine - just  shift  its  result
    correspondingly.

      -- ALGLIB --
         Copyright 21.07.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void convr1dcircularinv(double[] a, int m, double[] b, int n, out double[] r)
    {
        r = new double[0];
        conv.convr1dcircularinv(a, m, b, n, ref r, null);
    }
    
    public static void convr1dcircularinv(double[] a, int m, double[] b, int n, out double[] r, alglib.xparams _params)
    {
        r = new double[0];
        conv.convr1dcircularinv(a, m, b, n, ref r, _params);
    }

}
public partial class alglib
{

    
    /*************************************************************************
    1-dimensional complex cross-correlation.

    For given Pattern/Signal returns corr(Pattern,Signal) (non-circular).

    Correlation is calculated using reduction to  convolution.  Algorithm with
    max(N,N)*log(max(N,N)) complexity is used (see  ConvC1D()  for  more  info
    about performance).

    IMPORTANT:
        for  historical reasons subroutine accepts its parameters in  reversed
        order: CorrC1D(Signal, Pattern) = Pattern x Signal (using  traditional
        definition of cross-correlation, denoting cross-correlation as "x").

    INPUT PARAMETERS
        Signal  -   array[0..N-1] - complex function to be transformed,
                    signal containing pattern
        N       -   problem size
        Pattern -   array[0..M-1] - complex function to be transformed,
                    pattern to search withing signal
        M       -   problem size

    OUTPUT PARAMETERS
        R       -   cross-correlation, array[0..N+M-2]:
                    * positive lags are stored in R[0..N-1],
                      R[i] = sum(conj(pattern[j])*signal[i+j]
                    * negative lags are stored in R[N..N+M-2],
                      R[N+M-1-i] = sum(conj(pattern[j])*signal[-i+j]

    NOTE:
        It is assumed that pattern domain is [0..M-1].  If Pattern is non-zero
    on [-K..M-1],  you can still use this subroutine, just shift result by K.

      -- ALGLIB --
         Copyright 21.07.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void corrc1d(complex[] signal, int n, complex[] pattern, int m, out complex[] r)
    {
        r = new complex[0];
        corr.corrc1d(signal, n, pattern, m, ref r, null);
    }
    
    public static void corrc1d(complex[] signal, int n, complex[] pattern, int m, out complex[] r, alglib.xparams _params)
    {
        r = new complex[0];
        corr.corrc1d(signal, n, pattern, m, ref r, _params);
    }
    
    /*************************************************************************
    1-dimensional circular complex cross-correlation.

    For given Pattern/Signal returns corr(Pattern,Signal) (circular).
    Algorithm has linearithmic complexity for any M/N.

    IMPORTANT:
        for  historical reasons subroutine accepts its parameters in  reversed
        order:   CorrC1DCircular(Signal, Pattern) = Pattern x Signal    (using
        traditional definition of cross-correlation, denoting cross-correlation
        as "x").

    INPUT PARAMETERS
        Signal  -   array[0..N-1] - complex function to be transformed,
                    periodic signal containing pattern
        N       -   problem size
        Pattern -   array[0..M-1] - complex function to be transformed,
                    non-periodic pattern to search withing signal
        M       -   problem size

    OUTPUT PARAMETERS
        R   -   convolution: A*B. array[0..M-1].


      -- ALGLIB --
         Copyright 21.07.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void corrc1dcircular(complex[] signal, int m, complex[] pattern, int n, out complex[] c)
    {
        c = new complex[0];
        corr.corrc1dcircular(signal, m, pattern, n, ref c, null);
    }
    
    public static void corrc1dcircular(complex[] signal, int m, complex[] pattern, int n, out complex[] c, alglib.xparams _params)
    {
        c = new complex[0];
        corr.corrc1dcircular(signal, m, pattern, n, ref c, _params);
    }
    
    /*************************************************************************
    1-dimensional real cross-correlation.

    For given Pattern/Signal returns corr(Pattern,Signal) (non-circular).

    Correlation is calculated using reduction to  convolution.  Algorithm with
    max(N,N)*log(max(N,N)) complexity is used (see  ConvC1D()  for  more  info
    about performance).

    IMPORTANT:
        for  historical reasons subroutine accepts its parameters in  reversed
        order: CorrR1D(Signal, Pattern) = Pattern x Signal (using  traditional
        definition of cross-correlation, denoting cross-correlation as "x").

    INPUT PARAMETERS
        Signal  -   array[0..N-1] - real function to be transformed,
                    signal containing pattern
        N       -   problem size
        Pattern -   array[0..M-1] - real function to be transformed,
                    pattern to search withing signal
        M       -   problem size

    OUTPUT PARAMETERS
        R       -   cross-correlation, array[0..N+M-2]:
                    * positive lags are stored in R[0..N-1],
                      R[i] = sum(pattern[j]*signal[i+j]
                    * negative lags are stored in R[N..N+M-2],
                      R[N+M-1-i] = sum(pattern[j]*signal[-i+j]

    NOTE:
        It is assumed that pattern domain is [0..M-1].  If Pattern is non-zero
    on [-K..M-1],  you can still use this subroutine, just shift result by K.

      -- ALGLIB --
         Copyright 21.07.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void corrr1d(double[] signal, int n, double[] pattern, int m, out double[] r)
    {
        r = new double[0];
        corr.corrr1d(signal, n, pattern, m, ref r, null);
    }
    
    public static void corrr1d(double[] signal, int n, double[] pattern, int m, out double[] r, alglib.xparams _params)
    {
        r = new double[0];
        corr.corrr1d(signal, n, pattern, m, ref r, _params);
    }
    
    /*************************************************************************
    1-dimensional circular real cross-correlation.

    For given Pattern/Signal returns corr(Pattern,Signal) (circular).
    Algorithm has linearithmic complexity for any M/N.

    IMPORTANT:
        for  historical reasons subroutine accepts its parameters in  reversed
        order:   CorrR1DCircular(Signal, Pattern) = Pattern x Signal    (using
        traditional definition of cross-correlation, denoting cross-correlation
        as "x").

    INPUT PARAMETERS
        Signal  -   array[0..N-1] - real function to be transformed,
                    periodic signal containing pattern
        N       -   problem size
        Pattern -   array[0..M-1] - real function to be transformed,
                    non-periodic pattern to search withing signal
        M       -   problem size

    OUTPUT PARAMETERS
        R   -   convolution: A*B. array[0..M-1].


      -- ALGLIB --
         Copyright 21.07.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void corrr1dcircular(double[] signal, int m, double[] pattern, int n, out double[] c)
    {
        c = new double[0];
        corr.corrr1dcircular(signal, m, pattern, n, ref c, null);
    }
    
    public static void corrr1dcircular(double[] signal, int m, double[] pattern, int n, out double[] c, alglib.xparams _params)
    {
        c = new double[0];
        corr.corrr1dcircular(signal, m, pattern, n, ref c, _params);
    }

}
public partial class alglib
{
    public class fft
    {
        /*************************************************************************
        1-dimensional complex FFT.

        Array size N may be arbitrary number (composite or prime).  Composite  N's
        are handled with cache-oblivious variation of  a  Cooley-Tukey  algorithm.
        Small prime-factors are transformed using hard coded  codelets (similar to
        FFTW codelets, but without low-level  optimization),  large  prime-factors
        are handled with Bluestein's algorithm.

        Fastests transforms are for smooth N's (prime factors are 2, 3,  5  only),
        most fast for powers of 2. When N have prime factors  larger  than  these,
        but orders of magnitude smaller than N, computations will be about 4 times
        slower than for nearby highly composite N's. When N itself is prime, speed
        will be 6 times lower.

        Algorithm has O(N*logN) complexity for any N (composite or prime).

        INPUT PARAMETERS
            A   -   array[0..N-1] - complex function to be transformed
            N   -   problem size
            
        OUTPUT PARAMETERS
            A   -   DFT of a input array, array[0..N-1]
                    A_out[j] = SUM(A_in[k]*exp(-2*pi*sqrt(-1)*j*k/N), k = 0..N-1)


          -- ALGLIB --
             Copyright 29.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void fftc1d(ref complex[] a,
            int n,
            alglib.xparams _params)
        {
            ftbase.fasttransformplan plan = new ftbase.fasttransformplan();
            int i = 0;
            double[] buf = new double[0];

            alglib.ap.assert(n>0, "FFTC1D: incorrect N!");
            alglib.ap.assert(alglib.ap.len(a)>=n, "FFTC1D: Length(A)<N!");
            alglib.ap.assert(apserv.isfinitecvector(a, n, _params), "FFTC1D: A contains infinite or NAN values!");
            
            //
            // Special case: N=1, FFT is just identity transform.
            // After this block we assume that N is strictly greater than 1.
            //
            if( n==1 )
            {
                return;
            }
            
            //
            // convert input array to the more convinient format
            //
            buf = new double[2*n];
            for(i=0; i<=n-1; i++)
            {
                buf[2*i+0] = a[i].x;
                buf[2*i+1] = a[i].y;
            }
            
            //
            // Generate plan and execute it.
            //
            // Plan is a combination of a successive factorizations of N and
            // precomputed data. It is much like a FFTW plan, but is not stored
            // between subroutine calls and is much simpler.
            //
            ftbase.ftcomplexfftplan(n, 1, plan, _params);
            ftbase.ftapplyplan(plan, buf, 0, 1, _params);
            
            //
            // result
            //
            for(i=0; i<=n-1; i++)
            {
                a[i].x = buf[2*i+0];
                a[i].y = buf[2*i+1];
            }
        }


        /*************************************************************************
        1-dimensional complex inverse FFT.

        Array size N may be arbitrary number (composite or prime).  Algorithm  has
        O(N*logN) complexity for any N (composite or prime).

        See FFTC1D() description for more information about algorithm performance.

        INPUT PARAMETERS
            A   -   array[0..N-1] - complex array to be transformed
            N   -   problem size

        OUTPUT PARAMETERS
            A   -   inverse DFT of a input array, array[0..N-1]
                    A_out[j] = SUM(A_in[k]/N*exp(+2*pi*sqrt(-1)*j*k/N), k = 0..N-1)


          -- ALGLIB --
             Copyright 29.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void fftc1dinv(ref complex[] a,
            int n,
            alglib.xparams _params)
        {
            int i = 0;

            alglib.ap.assert(n>0, "FFTC1DInv: incorrect N!");
            alglib.ap.assert(alglib.ap.len(a)>=n, "FFTC1DInv: Length(A)<N!");
            alglib.ap.assert(apserv.isfinitecvector(a, n, _params), "FFTC1DInv: A contains infinite or NAN values!");
            
            //
            // Inverse DFT can be expressed in terms of the DFT as
            //
            //     invfft(x) = fft(x')'/N
            //
            // here x' means conj(x).
            //
            for(i=0; i<=n-1; i++)
            {
                a[i].y = -a[i].y;
            }
            fftc1d(ref a, n, _params);
            for(i=0; i<=n-1; i++)
            {
                a[i].x = a[i].x/n;
                a[i].y = -(a[i].y/n);
            }
        }


        /*************************************************************************
        1-dimensional real FFT.

        Algorithm has O(N*logN) complexity for any N (composite or prime).

        INPUT PARAMETERS
            A   -   array[0..N-1] - real function to be transformed
            N   -   problem size

        OUTPUT PARAMETERS
            F   -   DFT of a input array, array[0..N-1]
                    F[j] = SUM(A[k]*exp(-2*pi*sqrt(-1)*j*k/N), k = 0..N-1)

        NOTE:
            F[] satisfies symmetry property F[k] = conj(F[N-k]),  so just one half
        of  array  is  usually needed. But for convinience subroutine returns full
        complex array (with frequencies above N/2), so its result may be  used  by
        other FFT-related subroutines.


          -- ALGLIB --
             Copyright 01.06.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void fftr1d(double[] a,
            int n,
            ref complex[] f,
            alglib.xparams _params)
        {
            int i = 0;
            int n2 = 0;
            int idx = 0;
            complex hn = 0;
            complex hmnc = 0;
            complex v = 0;
            double[] buf = new double[0];
            ftbase.fasttransformplan plan = new ftbase.fasttransformplan();
            int i_ = 0;

            f = new complex[0];

            alglib.ap.assert(n>0, "FFTR1D: incorrect N!");
            alglib.ap.assert(alglib.ap.len(a)>=n, "FFTR1D: Length(A)<N!");
            alglib.ap.assert(apserv.isfinitevector(a, n, _params), "FFTR1D: A contains infinite or NAN values!");
            
            //
            // Special cases:
            // * N=1, FFT is just identity transform.
            // * N=2, FFT is simple too
            //
            // After this block we assume that N is strictly greater than 2
            //
            if( n==1 )
            {
                f = new complex[1];
                f[0] = a[0];
                return;
            }
            if( n==2 )
            {
                f = new complex[2];
                f[0].x = a[0]+a[1];
                f[0].y = 0;
                f[1].x = a[0]-a[1];
                f[1].y = 0;
                return;
            }
            
            //
            // Choose between odd-size and even-size FFTs
            //
            if( n%2==0 )
            {
                
                //
                // even-size real FFT, use reduction to the complex task
                //
                n2 = n/2;
                buf = new double[n];
                for(i_=0; i_<=n-1;i_++)
                {
                    buf[i_] = a[i_];
                }
                ftbase.ftcomplexfftplan(n2, 1, plan, _params);
                ftbase.ftapplyplan(plan, buf, 0, 1, _params);
                f = new complex[n];
                for(i=0; i<=n2; i++)
                {
                    idx = 2*(i%n2);
                    hn.x = buf[idx+0];
                    hn.y = buf[idx+1];
                    idx = 2*((n2-i)%n2);
                    hmnc.x = buf[idx+0];
                    hmnc.y = -buf[idx+1];
                    v.x = -Math.Sin(-(2*Math.PI*i/n));
                    v.y = Math.Cos(-(2*Math.PI*i/n));
                    f[i] = hn+hmnc-v*(hn-hmnc);
                    f[i].x = 0.5*f[i].x;
                    f[i].y = 0.5*f[i].y;
                }
                for(i=n2+1; i<=n-1; i++)
                {
                    f[i] = math.conj(f[n-i]);
                }
            }
            else
            {
                
                //
                // use complex FFT
                //
                f = new complex[n];
                for(i=0; i<=n-1; i++)
                {
                    f[i] = a[i];
                }
                fftc1d(ref f, n, _params);
            }
        }


        /*************************************************************************
        1-dimensional real inverse FFT.

        Algorithm has O(N*logN) complexity for any N (composite or prime).

        INPUT PARAMETERS
            F   -   array[0..floor(N/2)] - frequencies from forward real FFT
            N   -   problem size

        OUTPUT PARAMETERS
            A   -   inverse DFT of a input array, array[0..N-1]

        NOTE:
            F[] should satisfy symmetry property F[k] = conj(F[N-k]), so just  one
        half of frequencies array is needed - elements from 0 to floor(N/2).  F[0]
        is ALWAYS real. If N is even F[floor(N/2)] is real too. If N is odd,  then
        F[floor(N/2)] has no special properties.

        Relying on properties noted above, FFTR1DInv subroutine uses only elements
        from 0th to floor(N/2)-th. It ignores imaginary part of F[0],  and in case
        N is even it ignores imaginary part of F[floor(N/2)] too.

        When you call this function using full arguments list - "FFTR1DInv(F,N,A)"
        - you can pass either either frequencies array with N elements or  reduced
        array with roughly N/2 elements - subroutine will  successfully  transform
        both.

        If you call this function using reduced arguments list -  "FFTR1DInv(F,A)"
        - you must pass FULL array with N elements (although higher  N/2 are still
        not used) because array size is used to automatically determine FFT length


          -- ALGLIB --
             Copyright 01.06.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void fftr1dinv(complex[] f,
            int n,
            ref double[] a,
            alglib.xparams _params)
        {
            int i = 0;
            double[] h = new double[0];
            complex[] fh = new complex[0];

            a = new double[0];

            alglib.ap.assert(n>0, "FFTR1DInv: incorrect N!");
            alglib.ap.assert(alglib.ap.len(f)>=(int)Math.Floor((double)n/(double)2)+1, "FFTR1DInv: Length(F)<Floor(N/2)+1!");
            alglib.ap.assert(math.isfinite(f[0].x), "FFTR1DInv: F contains infinite or NAN values!");
            for(i=1; i<=(int)Math.Floor((double)n/(double)2)-1; i++)
            {
                alglib.ap.assert(math.isfinite(f[i].x) && math.isfinite(f[i].y), "FFTR1DInv: F contains infinite or NAN values!");
            }
            alglib.ap.assert(math.isfinite(f[(int)Math.Floor((double)n/(double)2)].x), "FFTR1DInv: F contains infinite or NAN values!");
            if( n%2!=0 )
            {
                alglib.ap.assert(math.isfinite(f[(int)Math.Floor((double)n/(double)2)].y), "FFTR1DInv: F contains infinite or NAN values!");
            }
            
            //
            // Special case: N=1, FFT is just identity transform.
            // After this block we assume that N is strictly greater than 1.
            //
            if( n==1 )
            {
                a = new double[1];
                a[0] = f[0].x;
                return;
            }
            
            //
            // inverse real FFT is reduced to the inverse real FHT,
            // which is reduced to the forward real FHT,
            // which is reduced to the forward real FFT.
            //
            // Don't worry, it is really compact and efficient reduction :)
            //
            h = new double[n];
            a = new double[n];
            h[0] = f[0].x;
            for(i=1; i<=(int)Math.Floor((double)n/(double)2)-1; i++)
            {
                h[i] = f[i].x-f[i].y;
                h[n-i] = f[i].x+f[i].y;
            }
            if( n%2==0 )
            {
                h[(int)Math.Floor((double)n/(double)2)] = f[(int)Math.Floor((double)n/(double)2)].x;
            }
            else
            {
                h[(int)Math.Floor((double)n/(double)2)] = f[(int)Math.Floor((double)n/(double)2)].x-f[(int)Math.Floor((double)n/(double)2)].y;
                h[(int)Math.Floor((double)n/(double)2)+1] = f[(int)Math.Floor((double)n/(double)2)].x+f[(int)Math.Floor((double)n/(double)2)].y;
            }
            fftr1d(h, n, ref fh, _params);
            for(i=0; i<=n-1; i++)
            {
                a[i] = (fh[i].x-fh[i].y)/n;
            }
        }


        /*************************************************************************
        Internal subroutine. Never call it directly!


          -- ALGLIB --
             Copyright 01.06.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void fftr1dinternaleven(ref double[] a,
            int n,
            ref double[] buf,
            ftbase.fasttransformplan plan,
            alglib.xparams _params)
        {
            double x = 0;
            double y = 0;
            int i = 0;
            int n2 = 0;
            int idx = 0;
            complex hn = 0;
            complex hmnc = 0;
            complex v = 0;
            int i_ = 0;

            alglib.ap.assert(n>0 && n%2==0, "FFTR1DEvenInplace: incorrect N!");
            
            //
            // Special cases:
            // * N=2
            //
            // After this block we assume that N is strictly greater than 2
            //
            if( n==2 )
            {
                x = a[0]+a[1];
                y = a[0]-a[1];
                a[0] = x;
                a[1] = y;
                return;
            }
            
            //
            // even-size real FFT, use reduction to the complex task
            //
            n2 = n/2;
            for(i_=0; i_<=n-1;i_++)
            {
                buf[i_] = a[i_];
            }
            ftbase.ftapplyplan(plan, buf, 0, 1, _params);
            a[0] = buf[0]+buf[1];
            for(i=1; i<=n2-1; i++)
            {
                idx = 2*(i%n2);
                hn.x = buf[idx+0];
                hn.y = buf[idx+1];
                idx = 2*(n2-i);
                hmnc.x = buf[idx+0];
                hmnc.y = -buf[idx+1];
                v.x = -Math.Sin(-(2*Math.PI*i/n));
                v.y = Math.Cos(-(2*Math.PI*i/n));
                v = hn+hmnc-v*(hn-hmnc);
                a[2*i+0] = 0.5*v.x;
                a[2*i+1] = 0.5*v.y;
            }
            a[1] = buf[0]-buf[1];
        }


        /*************************************************************************
        Internal subroutine. Never call it directly!


          -- ALGLIB --
             Copyright 01.06.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void fftr1dinvinternaleven(ref double[] a,
            int n,
            ref double[] buf,
            ftbase.fasttransformplan plan,
            alglib.xparams _params)
        {
            double x = 0;
            double y = 0;
            double t = 0;
            int i = 0;
            int n2 = 0;

            alglib.ap.assert(n>0 && n%2==0, "FFTR1DInvInternalEven: incorrect N!");
            
            //
            // Special cases:
            // * N=2
            //
            // After this block we assume that N is strictly greater than 2
            //
            if( n==2 )
            {
                x = 0.5*(a[0]+a[1]);
                y = 0.5*(a[0]-a[1]);
                a[0] = x;
                a[1] = y;
                return;
            }
            
            //
            // inverse real FFT is reduced to the inverse real FHT,
            // which is reduced to the forward real FHT,
            // which is reduced to the forward real FFT.
            //
            // Don't worry, it is really compact and efficient reduction :)
            //
            n2 = n/2;
            buf[0] = a[0];
            for(i=1; i<=n2-1; i++)
            {
                x = a[2*i+0];
                y = a[2*i+1];
                buf[i] = x-y;
                buf[n-i] = x+y;
            }
            buf[n2] = a[1];
            fftr1dinternaleven(ref buf, n, ref a, plan, _params);
            a[0] = buf[0]/n;
            t = (double)1/(double)n;
            for(i=1; i<=n2-1; i++)
            {
                x = buf[2*i+0];
                y = buf[2*i+1];
                a[i] = t*(x-y);
                a[n-i] = t*(x+y);
            }
            a[n2] = buf[1]/n;
        }


    }
    public class fht
    {
        /*************************************************************************
        1-dimensional Fast Hartley Transform.

        Algorithm has O(N*logN) complexity for any N (composite or prime).

        INPUT PARAMETERS
            A   -   array[0..N-1] - real function to be transformed
            N   -   problem size
            
        OUTPUT PARAMETERS
            A   -   FHT of a input array, array[0..N-1],
                    A_out[k] = sum(A_in[j]*(cos(2*pi*j*k/N)+sin(2*pi*j*k/N)), j=0..N-1)


          -- ALGLIB --
             Copyright 04.06.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void fhtr1d(ref double[] a,
            int n,
            alglib.xparams _params)
        {
            int i = 0;
            complex[] fa = new complex[0];

            alglib.ap.assert(n>0, "FHTR1D: incorrect N!");
            
            //
            // Special case: N=1, FHT is just identity transform.
            // After this block we assume that N is strictly greater than 1.
            //
            if( n==1 )
            {
                return;
            }
            
            //
            // Reduce FHt to real FFT
            //
            fft.fftr1d(a, n, ref fa, _params);
            for(i=0; i<=n-1; i++)
            {
                a[i] = fa[i].x-fa[i].y;
            }
        }


        /*************************************************************************
        1-dimensional inverse FHT.

        Algorithm has O(N*logN) complexity for any N (composite or prime).

        INPUT PARAMETERS
            A   -   array[0..N-1] - complex array to be transformed
            N   -   problem size

        OUTPUT PARAMETERS
            A   -   inverse FHT of a input array, array[0..N-1]


          -- ALGLIB --
             Copyright 29.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void fhtr1dinv(ref double[] a,
            int n,
            alglib.xparams _params)
        {
            int i = 0;

            alglib.ap.assert(n>0, "FHTR1DInv: incorrect N!");
            
            //
            // Special case: N=1, iFHT is just identity transform.
            // After this block we assume that N is strictly greater than 1.
            //
            if( n==1 )
            {
                return;
            }
            
            //
            // Inverse FHT can be expressed in terms of the FHT as
            //
            //     invfht(x) = fht(x)/N
            //
            fhtr1d(ref a, n, _params);
            for(i=0; i<=n-1; i++)
            {
                a[i] = a[i]/n;
            }
        }


    }
    public class conv
    {
        /*************************************************************************
        1-dimensional complex convolution.

        For given A/B returns conv(A,B) (non-circular). Subroutine can automatically
        choose between three implementations: straightforward O(M*N)  formula  for
        very small N (or M), overlap-add algorithm for  cases  where  max(M,N)  is
        significantly larger than min(M,N), but O(M*N) algorithm is too slow,  and
        general FFT-based formula for cases where two previois algorithms are  too
        slow.

        Algorithm has max(M,N)*log(max(M,N)) complexity for any M/N.

        INPUT PARAMETERS
            A   -   array[0..M-1] - complex function to be transformed
            M   -   problem size
            B   -   array[0..N-1] - complex function to be transformed
            N   -   problem size

        OUTPUT PARAMETERS
            R   -   convolution: A*B. array[0..N+M-2].

        NOTE:
            It is assumed that A is zero at T<0, B is zero too.  If  one  or  both
        functions have non-zero values at negative T's, you  can  still  use  this
        subroutine - just shift its result correspondingly.

          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void convc1d(complex[] a,
            int m,
            complex[] b,
            int n,
            ref complex[] r,
            alglib.xparams _params)
        {
            r = new complex[0];

            alglib.ap.assert(n>0 && m>0, "ConvC1D: incorrect N or M!");
            
            //
            // normalize task: make M>=N,
            // so A will be longer that B.
            //
            if( m<n )
            {
                convc1d(b, n, a, m, ref r, _params);
                return;
            }
            convc1dx(a, m, b, n, false, -1, 0, ref r, _params);
        }


        /*************************************************************************
        1-dimensional complex non-circular deconvolution (inverse of ConvC1D()).

        Algorithm has M*log(M)) complexity for any M (composite or prime).

        INPUT PARAMETERS
            A   -   array[0..M-1] - convolved signal, A = conv(R, B)
            M   -   convolved signal length
            B   -   array[0..N-1] - response
            N   -   response length, N<=M

        OUTPUT PARAMETERS
            R   -   deconvolved signal. array[0..M-N].

        NOTE:
            deconvolution is unstable process and may result in division  by  zero
        (if your response function is degenerate, i.e. has zero Fourier coefficient).

        NOTE:
            It is assumed that A is zero at T<0, B is zero too.  If  one  or  both
        functions have non-zero values at negative T's, you  can  still  use  this
        subroutine - just shift its result correspondingly.

          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void convc1dinv(complex[] a,
            int m,
            complex[] b,
            int n,
            ref complex[] r,
            alglib.xparams _params)
        {
            int i = 0;
            int p = 0;
            double[] buf = new double[0];
            double[] buf2 = new double[0];
            ftbase.fasttransformplan plan = new ftbase.fasttransformplan();
            complex c1 = 0;
            complex c2 = 0;
            complex c3 = 0;
            double t = 0;

            r = new complex[0];

            alglib.ap.assert((n>0 && m>0) && n<=m, "ConvC1DInv: incorrect N or M!");
            p = ftbase.ftbasefindsmooth(m, _params);
            ftbase.ftcomplexfftplan(p, 1, plan, _params);
            buf = new double[2*p];
            for(i=0; i<=m-1; i++)
            {
                buf[2*i+0] = a[i].x;
                buf[2*i+1] = a[i].y;
            }
            for(i=m; i<=p-1; i++)
            {
                buf[2*i+0] = 0;
                buf[2*i+1] = 0;
            }
            buf2 = new double[2*p];
            for(i=0; i<=n-1; i++)
            {
                buf2[2*i+0] = b[i].x;
                buf2[2*i+1] = b[i].y;
            }
            for(i=n; i<=p-1; i++)
            {
                buf2[2*i+0] = 0;
                buf2[2*i+1] = 0;
            }
            ftbase.ftapplyplan(plan, buf, 0, 1, _params);
            ftbase.ftapplyplan(plan, buf2, 0, 1, _params);
            for(i=0; i<=p-1; i++)
            {
                c1.x = buf[2*i+0];
                c1.y = buf[2*i+1];
                c2.x = buf2[2*i+0];
                c2.y = buf2[2*i+1];
                c3 = c1/c2;
                buf[2*i+0] = c3.x;
                buf[2*i+1] = -c3.y;
            }
            ftbase.ftapplyplan(plan, buf, 0, 1, _params);
            t = (double)1/(double)p;
            r = new complex[m-n+1];
            for(i=0; i<=m-n; i++)
            {
                r[i].x = t*buf[2*i+0];
                r[i].y = -(t*buf[2*i+1]);
            }
        }


        /*************************************************************************
        1-dimensional circular complex convolution.

        For given S/R returns conv(S,R) (circular). Algorithm has linearithmic
        complexity for any M/N.

        IMPORTANT:  normal convolution is commutative,  i.e.   it  is symmetric  -
        conv(A,B)=conv(B,A).  Cyclic convolution IS NOT.  One function - S - is  a
        signal,  periodic function, and another - R - is a response,  non-periodic
        function with limited length.

        INPUT PARAMETERS
            S   -   array[0..M-1] - complex periodic signal
            M   -   problem size
            B   -   array[0..N-1] - complex non-periodic response
            N   -   problem size

        OUTPUT PARAMETERS
            R   -   convolution: A*B. array[0..M-1].

        NOTE:
            It is assumed that B is zero at T<0. If  it  has  non-zero  values  at
        negative T's, you can still use this subroutine - just  shift  its  result
        correspondingly.

          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void convc1dcircular(complex[] s,
            int m,
            complex[] r,
            int n,
            ref complex[] c,
            alglib.xparams _params)
        {
            complex[] buf = new complex[0];
            int i1 = 0;
            int i2 = 0;
            int j2 = 0;
            int i_ = 0;
            int i1_ = 0;

            c = new complex[0];

            alglib.ap.assert(n>0 && m>0, "ConvC1DCircular: incorrect N or M!");
            
            //
            // normalize task: make M>=N,
            // so A will be longer (at least - not shorter) that B.
            //
            if( m<n )
            {
                buf = new complex[m];
                for(i1=0; i1<=m-1; i1++)
                {
                    buf[i1] = 0;
                }
                i1 = 0;
                while( i1<n )
                {
                    i2 = Math.Min(i1+m-1, n-1);
                    j2 = i2-i1;
                    i1_ = (i1) - (0);
                    for(i_=0; i_<=j2;i_++)
                    {
                        buf[i_] = buf[i_] + r[i_+i1_];
                    }
                    i1 = i1+m;
                }
                convc1dcircular(s, m, buf, m, ref c, _params);
                return;
            }
            convc1dx(s, m, r, n, true, -1, 0, ref c, _params);
        }


        /*************************************************************************
        1-dimensional circular complex deconvolution (inverse of ConvC1DCircular()).

        Algorithm has M*log(M)) complexity for any M (composite or prime).

        INPUT PARAMETERS
            A   -   array[0..M-1] - convolved periodic signal, A = conv(R, B)
            M   -   convolved signal length
            B   -   array[0..N-1] - non-periodic response
            N   -   response length

        OUTPUT PARAMETERS
            R   -   deconvolved signal. array[0..M-1].

        NOTE:
            deconvolution is unstable process and may result in division  by  zero
        (if your response function is degenerate, i.e. has zero Fourier coefficient).

        NOTE:
            It is assumed that B is zero at T<0. If  it  has  non-zero  values  at
        negative T's, you can still use this subroutine - just  shift  its  result
        correspondingly.

          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void convc1dcircularinv(complex[] a,
            int m,
            complex[] b,
            int n,
            ref complex[] r,
            alglib.xparams _params)
        {
            int i = 0;
            int i1 = 0;
            int i2 = 0;
            int j2 = 0;
            double[] buf = new double[0];
            double[] buf2 = new double[0];
            complex[] cbuf = new complex[0];
            ftbase.fasttransformplan plan = new ftbase.fasttransformplan();
            complex c1 = 0;
            complex c2 = 0;
            complex c3 = 0;
            double t = 0;
            int i_ = 0;
            int i1_ = 0;

            r = new complex[0];

            alglib.ap.assert(n>0 && m>0, "ConvC1DCircularInv: incorrect N or M!");
            
            //
            // normalize task: make M>=N,
            // so A will be longer (at least - not shorter) that B.
            //
            if( m<n )
            {
                cbuf = new complex[m];
                for(i=0; i<=m-1; i++)
                {
                    cbuf[i] = 0;
                }
                i1 = 0;
                while( i1<n )
                {
                    i2 = Math.Min(i1+m-1, n-1);
                    j2 = i2-i1;
                    i1_ = (i1) - (0);
                    for(i_=0; i_<=j2;i_++)
                    {
                        cbuf[i_] = cbuf[i_] + b[i_+i1_];
                    }
                    i1 = i1+m;
                }
                convc1dcircularinv(a, m, cbuf, m, ref r, _params);
                return;
            }
            
            //
            // Task is normalized
            //
            ftbase.ftcomplexfftplan(m, 1, plan, _params);
            buf = new double[2*m];
            for(i=0; i<=m-1; i++)
            {
                buf[2*i+0] = a[i].x;
                buf[2*i+1] = a[i].y;
            }
            buf2 = new double[2*m];
            for(i=0; i<=n-1; i++)
            {
                buf2[2*i+0] = b[i].x;
                buf2[2*i+1] = b[i].y;
            }
            for(i=n; i<=m-1; i++)
            {
                buf2[2*i+0] = 0;
                buf2[2*i+1] = 0;
            }
            ftbase.ftapplyplan(plan, buf, 0, 1, _params);
            ftbase.ftapplyplan(plan, buf2, 0, 1, _params);
            for(i=0; i<=m-1; i++)
            {
                c1.x = buf[2*i+0];
                c1.y = buf[2*i+1];
                c2.x = buf2[2*i+0];
                c2.y = buf2[2*i+1];
                c3 = c1/c2;
                buf[2*i+0] = c3.x;
                buf[2*i+1] = -c3.y;
            }
            ftbase.ftapplyplan(plan, buf, 0, 1, _params);
            t = (double)1/(double)m;
            r = new complex[m];
            for(i=0; i<=m-1; i++)
            {
                r[i].x = t*buf[2*i+0];
                r[i].y = -(t*buf[2*i+1]);
            }
        }


        /*************************************************************************
        1-dimensional real convolution.

        Analogous to ConvC1D(), see ConvC1D() comments for more details.

        INPUT PARAMETERS
            A   -   array[0..M-1] - real function to be transformed
            M   -   problem size
            B   -   array[0..N-1] - real function to be transformed
            N   -   problem size

        OUTPUT PARAMETERS
            R   -   convolution: A*B. array[0..N+M-2].

        NOTE:
            It is assumed that A is zero at T<0, B is zero too.  If  one  or  both
        functions have non-zero values at negative T's, you  can  still  use  this
        subroutine - just shift its result correspondingly.

          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void convr1d(double[] a,
            int m,
            double[] b,
            int n,
            ref double[] r,
            alglib.xparams _params)
        {
            r = new double[0];

            alglib.ap.assert(n>0 && m>0, "ConvR1D: incorrect N or M!");
            
            //
            // normalize task: make M>=N,
            // so A will be longer that B.
            //
            if( m<n )
            {
                convr1d(b, n, a, m, ref r, _params);
                return;
            }
            convr1dx(a, m, b, n, false, -1, 0, ref r, _params);
        }


        /*************************************************************************
        1-dimensional real deconvolution (inverse of ConvC1D()).

        Algorithm has M*log(M)) complexity for any M (composite or prime).

        INPUT PARAMETERS
            A   -   array[0..M-1] - convolved signal, A = conv(R, B)
            M   -   convolved signal length
            B   -   array[0..N-1] - response
            N   -   response length, N<=M

        OUTPUT PARAMETERS
            R   -   deconvolved signal. array[0..M-N].

        NOTE:
            deconvolution is unstable process and may result in division  by  zero
        (if your response function is degenerate, i.e. has zero Fourier coefficient).

        NOTE:
            It is assumed that A is zero at T<0, B is zero too.  If  one  or  both
        functions have non-zero values at negative T's, you  can  still  use  this
        subroutine - just shift its result correspondingly.

          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void convr1dinv(double[] a,
            int m,
            double[] b,
            int n,
            ref double[] r,
            alglib.xparams _params)
        {
            int i = 0;
            int p = 0;
            double[] buf = new double[0];
            double[] buf2 = new double[0];
            double[] buf3 = new double[0];
            ftbase.fasttransformplan plan = new ftbase.fasttransformplan();
            complex c1 = 0;
            complex c2 = 0;
            complex c3 = 0;
            int i_ = 0;

            r = new double[0];

            alglib.ap.assert((n>0 && m>0) && n<=m, "ConvR1DInv: incorrect N or M!");
            p = ftbase.ftbasefindsmootheven(m, _params);
            buf = new double[p];
            for(i_=0; i_<=m-1;i_++)
            {
                buf[i_] = a[i_];
            }
            for(i=m; i<=p-1; i++)
            {
                buf[i] = 0;
            }
            buf2 = new double[p];
            for(i_=0; i_<=n-1;i_++)
            {
                buf2[i_] = b[i_];
            }
            for(i=n; i<=p-1; i++)
            {
                buf2[i] = 0;
            }
            buf3 = new double[p];
            ftbase.ftcomplexfftplan(p/2, 1, plan, _params);
            fft.fftr1dinternaleven(ref buf, p, ref buf3, plan, _params);
            fft.fftr1dinternaleven(ref buf2, p, ref buf3, plan, _params);
            buf[0] = buf[0]/buf2[0];
            buf[1] = buf[1]/buf2[1];
            for(i=1; i<=p/2-1; i++)
            {
                c1.x = buf[2*i+0];
                c1.y = buf[2*i+1];
                c2.x = buf2[2*i+0];
                c2.y = buf2[2*i+1];
                c3 = c1/c2;
                buf[2*i+0] = c3.x;
                buf[2*i+1] = c3.y;
            }
            fft.fftr1dinvinternaleven(ref buf, p, ref buf3, plan, _params);
            r = new double[m-n+1];
            for(i_=0; i_<=m-n;i_++)
            {
                r[i_] = buf[i_];
            }
        }


        /*************************************************************************
        1-dimensional circular real convolution.

        Analogous to ConvC1DCircular(), see ConvC1DCircular() comments for more details.

        INPUT PARAMETERS
            S   -   array[0..M-1] - real signal
            M   -   problem size
            B   -   array[0..N-1] - real response
            N   -   problem size

        OUTPUT PARAMETERS
            R   -   convolution: A*B. array[0..M-1].

        NOTE:
            It is assumed that B is zero at T<0. If  it  has  non-zero  values  at
        negative T's, you can still use this subroutine - just  shift  its  result
        correspondingly.

          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void convr1dcircular(double[] s,
            int m,
            double[] r,
            int n,
            ref double[] c,
            alglib.xparams _params)
        {
            double[] buf = new double[0];
            int i1 = 0;
            int i2 = 0;
            int j2 = 0;
            int i_ = 0;
            int i1_ = 0;

            c = new double[0];

            alglib.ap.assert(n>0 && m>0, "ConvC1DCircular: incorrect N or M!");
            
            //
            // normalize task: make M>=N,
            // so A will be longer (at least - not shorter) that B.
            //
            if( m<n )
            {
                buf = new double[m];
                for(i1=0; i1<=m-1; i1++)
                {
                    buf[i1] = 0;
                }
                i1 = 0;
                while( i1<n )
                {
                    i2 = Math.Min(i1+m-1, n-1);
                    j2 = i2-i1;
                    i1_ = (i1) - (0);
                    for(i_=0; i_<=j2;i_++)
                    {
                        buf[i_] = buf[i_] + r[i_+i1_];
                    }
                    i1 = i1+m;
                }
                convr1dcircular(s, m, buf, m, ref c, _params);
                return;
            }
            
            //
            // reduce to usual convolution
            //
            convr1dx(s, m, r, n, true, -1, 0, ref c, _params);
        }


        /*************************************************************************
        1-dimensional complex deconvolution (inverse of ConvC1D()).

        Algorithm has M*log(M)) complexity for any M (composite or prime).

        INPUT PARAMETERS
            A   -   array[0..M-1] - convolved signal, A = conv(R, B)
            M   -   convolved signal length
            B   -   array[0..N-1] - response
            N   -   response length

        OUTPUT PARAMETERS
            R   -   deconvolved signal. array[0..M-N].

        NOTE:
            deconvolution is unstable process and may result in division  by  zero
        (if your response function is degenerate, i.e. has zero Fourier coefficient).

        NOTE:
            It is assumed that B is zero at T<0. If  it  has  non-zero  values  at
        negative T's, you can still use this subroutine - just  shift  its  result
        correspondingly.

          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void convr1dcircularinv(double[] a,
            int m,
            double[] b,
            int n,
            ref double[] r,
            alglib.xparams _params)
        {
            int i = 0;
            int i1 = 0;
            int i2 = 0;
            int j2 = 0;
            double[] buf = new double[0];
            double[] buf2 = new double[0];
            double[] buf3 = new double[0];
            complex[] cbuf = new complex[0];
            complex[] cbuf2 = new complex[0];
            ftbase.fasttransformplan plan = new ftbase.fasttransformplan();
            complex c1 = 0;
            complex c2 = 0;
            complex c3 = 0;
            int i_ = 0;
            int i1_ = 0;

            r = new double[0];

            alglib.ap.assert(n>0 && m>0, "ConvR1DCircularInv: incorrect N or M!");
            
            //
            // normalize task: make M>=N,
            // so A will be longer (at least - not shorter) that B.
            //
            if( m<n )
            {
                buf = new double[m];
                for(i=0; i<=m-1; i++)
                {
                    buf[i] = 0;
                }
                i1 = 0;
                while( i1<n )
                {
                    i2 = Math.Min(i1+m-1, n-1);
                    j2 = i2-i1;
                    i1_ = (i1) - (0);
                    for(i_=0; i_<=j2;i_++)
                    {
                        buf[i_] = buf[i_] + b[i_+i1_];
                    }
                    i1 = i1+m;
                }
                convr1dcircularinv(a, m, buf, m, ref r, _params);
                return;
            }
            
            //
            // Task is normalized
            //
            if( m%2==0 )
            {
                
                //
                // size is even, use fast even-size FFT
                //
                buf = new double[m];
                for(i_=0; i_<=m-1;i_++)
                {
                    buf[i_] = a[i_];
                }
                buf2 = new double[m];
                for(i_=0; i_<=n-1;i_++)
                {
                    buf2[i_] = b[i_];
                }
                for(i=n; i<=m-1; i++)
                {
                    buf2[i] = 0;
                }
                buf3 = new double[m];
                ftbase.ftcomplexfftplan(m/2, 1, plan, _params);
                fft.fftr1dinternaleven(ref buf, m, ref buf3, plan, _params);
                fft.fftr1dinternaleven(ref buf2, m, ref buf3, plan, _params);
                buf[0] = buf[0]/buf2[0];
                buf[1] = buf[1]/buf2[1];
                for(i=1; i<=m/2-1; i++)
                {
                    c1.x = buf[2*i+0];
                    c1.y = buf[2*i+1];
                    c2.x = buf2[2*i+0];
                    c2.y = buf2[2*i+1];
                    c3 = c1/c2;
                    buf[2*i+0] = c3.x;
                    buf[2*i+1] = c3.y;
                }
                fft.fftr1dinvinternaleven(ref buf, m, ref buf3, plan, _params);
                r = new double[m];
                for(i_=0; i_<=m-1;i_++)
                {
                    r[i_] = buf[i_];
                }
            }
            else
            {
                
                //
                // odd-size, use general real FFT
                //
                fft.fftr1d(a, m, ref cbuf, _params);
                buf2 = new double[m];
                for(i_=0; i_<=n-1;i_++)
                {
                    buf2[i_] = b[i_];
                }
                for(i=n; i<=m-1; i++)
                {
                    buf2[i] = 0;
                }
                fft.fftr1d(buf2, m, ref cbuf2, _params);
                for(i=0; i<=(int)Math.Floor((double)m/(double)2); i++)
                {
                    cbuf[i] = cbuf[i]/cbuf2[i];
                }
                fft.fftr1dinv(cbuf, m, ref r, _params);
            }
        }


        /*************************************************************************
        1-dimensional complex convolution.

        Extended subroutine which allows to choose convolution algorithm.
        Intended for internal use, ALGLIB users should call ConvC1D()/ConvC1DCircular().

        INPUT PARAMETERS
            A   -   array[0..M-1] - complex function to be transformed
            M   -   problem size
            B   -   array[0..N-1] - complex function to be transformed
            N   -   problem size, N<=M
            Alg -   algorithm type:
                    *-2     auto-select Q for overlap-add
                    *-1     auto-select algorithm and parameters
                    * 0     straightforward formula for small N's
                    * 1     general FFT-based code
                    * 2     overlap-add with length Q
            Q   -   length for overlap-add

        OUTPUT PARAMETERS
            R   -   convolution: A*B. array[0..N+M-1].

          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void convc1dx(complex[] a,
            int m,
            complex[] b,
            int n,
            bool circular,
            int alg,
            int q,
            ref complex[] r,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;
            int p = 0;
            int ptotal = 0;
            int i1 = 0;
            int i2 = 0;
            int j1 = 0;
            int j2 = 0;
            complex[] bbuf = new complex[0];
            complex v = 0;
            double ax = 0;
            double ay = 0;
            double bx = 0;
            double by = 0;
            double t = 0;
            double tx = 0;
            double ty = 0;
            double flopcand = 0;
            double flopbest = 0;
            int algbest = 0;
            ftbase.fasttransformplan plan = new ftbase.fasttransformplan();
            double[] buf = new double[0];
            double[] buf2 = new double[0];
            int i_ = 0;
            int i1_ = 0;

            r = new complex[0];

            alglib.ap.assert(n>0 && m>0, "ConvC1DX: incorrect N or M!");
            alglib.ap.assert(n<=m, "ConvC1DX: N<M assumption is false!");
            
            //
            // Auto-select
            //
            if( alg==-1 || alg==-2 )
            {
                
                //
                // Initial candidate: straightforward implementation.
                //
                // If we want to use auto-fitted overlap-add,
                // flop count is initialized by large real number - to force
                // another algorithm selection
                //
                algbest = 0;
                if( alg==-1 )
                {
                    flopbest = 2*m*n;
                }
                else
                {
                    flopbest = math.maxrealnumber;
                }
                
                //
                // Another candidate - generic FFT code
                //
                if( alg==-1 )
                {
                    if( circular && ftbase.ftbaseissmooth(m, _params) )
                    {
                        
                        //
                        // special code for circular convolution of a sequence with a smooth length
                        //
                        flopcand = 3*ftbase.ftbasegetflopestimate(m, _params)+6*m;
                        if( (double)(flopcand)<(double)(flopbest) )
                        {
                            algbest = 1;
                            flopbest = flopcand;
                        }
                    }
                    else
                    {
                        
                        //
                        // general cyclic/non-cyclic convolution
                        //
                        p = ftbase.ftbasefindsmooth(m+n-1, _params);
                        flopcand = 3*ftbase.ftbasegetflopestimate(p, _params)+6*p;
                        if( (double)(flopcand)<(double)(flopbest) )
                        {
                            algbest = 1;
                            flopbest = flopcand;
                        }
                    }
                }
                
                //
                // Another candidate - overlap-add
                //
                q = 1;
                ptotal = 1;
                while( ptotal<n )
                {
                    ptotal = ptotal*2;
                }
                while( ptotal<=m+n-1 )
                {
                    p = ptotal-n+1;
                    flopcand = (int)Math.Ceiling((double)m/(double)p)*(2*ftbase.ftbasegetflopestimate(ptotal, _params)+8*ptotal);
                    if( (double)(flopcand)<(double)(flopbest) )
                    {
                        flopbest = flopcand;
                        algbest = 2;
                        q = p;
                    }
                    ptotal = ptotal*2;
                }
                alg = algbest;
                convc1dx(a, m, b, n, circular, alg, q, ref r, _params);
                return;
            }
            
            //
            // straightforward formula for
            // circular and non-circular convolutions.
            //
            // Very simple code, no further comments needed.
            //
            if( alg==0 )
            {
                
                //
                // Special case: N=1
                //
                if( n==1 )
                {
                    r = new complex[m];
                    v = b[0];
                    for(i_=0; i_<=m-1;i_++)
                    {
                        r[i_] = v*a[i_];
                    }
                    return;
                }
                
                //
                // use straightforward formula
                //
                if( circular )
                {
                    
                    //
                    // circular convolution
                    //
                    r = new complex[m];
                    v = b[0];
                    for(i_=0; i_<=m-1;i_++)
                    {
                        r[i_] = v*a[i_];
                    }
                    for(i=1; i<=n-1; i++)
                    {
                        v = b[i];
                        i1 = 0;
                        i2 = i-1;
                        j1 = m-i;
                        j2 = m-1;
                        i1_ = (j1) - (i1);
                        for(i_=i1; i_<=i2;i_++)
                        {
                            r[i_] = r[i_] + v*a[i_+i1_];
                        }
                        i1 = i;
                        i2 = m-1;
                        j1 = 0;
                        j2 = m-i-1;
                        i1_ = (j1) - (i1);
                        for(i_=i1; i_<=i2;i_++)
                        {
                            r[i_] = r[i_] + v*a[i_+i1_];
                        }
                    }
                }
                else
                {
                    
                    //
                    // non-circular convolution
                    //
                    r = new complex[m+n-1];
                    for(i=0; i<=m+n-2; i++)
                    {
                        r[i] = 0;
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        v = b[i];
                        i1_ = (0) - (i);
                        for(i_=i; i_<=i+m-1;i_++)
                        {
                            r[i_] = r[i_] + v*a[i_+i1_];
                        }
                    }
                }
                return;
            }
            
            //
            // general FFT-based code for
            // circular and non-circular convolutions.
            //
            // First, if convolution is circular, we test whether M is smooth or not.
            // If it is smooth, we just use M-length FFT to calculate convolution.
            // If it is not, we calculate non-circular convolution and wrap it arount.
            //
            // IF convolution is non-circular, we use zero-padding + FFT.
            //
            if( alg==1 )
            {
                if( circular && ftbase.ftbaseissmooth(m, _params) )
                {
                    
                    //
                    // special code for circular convolution with smooth M
                    //
                    ftbase.ftcomplexfftplan(m, 1, plan, _params);
                    buf = new double[2*m];
                    for(i=0; i<=m-1; i++)
                    {
                        buf[2*i+0] = a[i].x;
                        buf[2*i+1] = a[i].y;
                    }
                    buf2 = new double[2*m];
                    for(i=0; i<=n-1; i++)
                    {
                        buf2[2*i+0] = b[i].x;
                        buf2[2*i+1] = b[i].y;
                    }
                    for(i=n; i<=m-1; i++)
                    {
                        buf2[2*i+0] = 0;
                        buf2[2*i+1] = 0;
                    }
                    ftbase.ftapplyplan(plan, buf, 0, 1, _params);
                    ftbase.ftapplyplan(plan, buf2, 0, 1, _params);
                    for(i=0; i<=m-1; i++)
                    {
                        ax = buf[2*i+0];
                        ay = buf[2*i+1];
                        bx = buf2[2*i+0];
                        by = buf2[2*i+1];
                        tx = ax*bx-ay*by;
                        ty = ax*by+ay*bx;
                        buf[2*i+0] = tx;
                        buf[2*i+1] = -ty;
                    }
                    ftbase.ftapplyplan(plan, buf, 0, 1, _params);
                    t = (double)1/(double)m;
                    r = new complex[m];
                    for(i=0; i<=m-1; i++)
                    {
                        r[i].x = t*buf[2*i+0];
                        r[i].y = -(t*buf[2*i+1]);
                    }
                }
                else
                {
                    
                    //
                    // M is non-smooth, general code (circular/non-circular):
                    // * first part is the same for circular and non-circular
                    //   convolutions. zero padding, FFTs, inverse FFTs
                    // * second part differs:
                    //   * for non-circular convolution we just copy array
                    //   * for circular convolution we add array tail to its head
                    //
                    p = ftbase.ftbasefindsmooth(m+n-1, _params);
                    ftbase.ftcomplexfftplan(p, 1, plan, _params);
                    buf = new double[2*p];
                    for(i=0; i<=m-1; i++)
                    {
                        buf[2*i+0] = a[i].x;
                        buf[2*i+1] = a[i].y;
                    }
                    for(i=m; i<=p-1; i++)
                    {
                        buf[2*i+0] = 0;
                        buf[2*i+1] = 0;
                    }
                    buf2 = new double[2*p];
                    for(i=0; i<=n-1; i++)
                    {
                        buf2[2*i+0] = b[i].x;
                        buf2[2*i+1] = b[i].y;
                    }
                    for(i=n; i<=p-1; i++)
                    {
                        buf2[2*i+0] = 0;
                        buf2[2*i+1] = 0;
                    }
                    ftbase.ftapplyplan(plan, buf, 0, 1, _params);
                    ftbase.ftapplyplan(plan, buf2, 0, 1, _params);
                    for(i=0; i<=p-1; i++)
                    {
                        ax = buf[2*i+0];
                        ay = buf[2*i+1];
                        bx = buf2[2*i+0];
                        by = buf2[2*i+1];
                        tx = ax*bx-ay*by;
                        ty = ax*by+ay*bx;
                        buf[2*i+0] = tx;
                        buf[2*i+1] = -ty;
                    }
                    ftbase.ftapplyplan(plan, buf, 0, 1, _params);
                    t = (double)1/(double)p;
                    if( circular )
                    {
                        
                        //
                        // circular, add tail to head
                        //
                        r = new complex[m];
                        for(i=0; i<=m-1; i++)
                        {
                            r[i].x = t*buf[2*i+0];
                            r[i].y = -(t*buf[2*i+1]);
                        }
                        for(i=m; i<=m+n-2; i++)
                        {
                            r[i-m].x = r[i-m].x+t*buf[2*i+0];
                            r[i-m].y = r[i-m].y-t*buf[2*i+1];
                        }
                    }
                    else
                    {
                        
                        //
                        // non-circular, just copy
                        //
                        r = new complex[m+n-1];
                        for(i=0; i<=m+n-2; i++)
                        {
                            r[i].x = t*buf[2*i+0];
                            r[i].y = -(t*buf[2*i+1]);
                        }
                    }
                }
                return;
            }
            
            //
            // overlap-add method for
            // circular and non-circular convolutions.
            //
            // First part of code (separate FFTs of input blocks) is the same
            // for all types of convolution. Second part (overlapping outputs)
            // differs for different types of convolution. We just copy output
            // when convolution is non-circular. We wrap it around, if it is
            // circular.
            //
            if( alg==2 )
            {
                buf = new double[2*(q+n-1)];
                
                //
                // prepare R
                //
                if( circular )
                {
                    r = new complex[m];
                    for(i=0; i<=m-1; i++)
                    {
                        r[i] = 0;
                    }
                }
                else
                {
                    r = new complex[m+n-1];
                    for(i=0; i<=m+n-2; i++)
                    {
                        r[i] = 0;
                    }
                }
                
                //
                // pre-calculated FFT(B)
                //
                bbuf = new complex[q+n-1];
                for(i_=0; i_<=n-1;i_++)
                {
                    bbuf[i_] = b[i_];
                }
                for(j=n; j<=q+n-2; j++)
                {
                    bbuf[j] = 0;
                }
                fft.fftc1d(ref bbuf, q+n-1, _params);
                
                //
                // prepare FFT plan for chunks of A
                //
                ftbase.ftcomplexfftplan(q+n-1, 1, plan, _params);
                
                //
                // main overlap-add cycle
                //
                i = 0;
                while( i<=m-1 )
                {
                    p = Math.Min(q, m-i);
                    for(j=0; j<=p-1; j++)
                    {
                        buf[2*j+0] = a[i+j].x;
                        buf[2*j+1] = a[i+j].y;
                    }
                    for(j=p; j<=q+n-2; j++)
                    {
                        buf[2*j+0] = 0;
                        buf[2*j+1] = 0;
                    }
                    ftbase.ftapplyplan(plan, buf, 0, 1, _params);
                    for(j=0; j<=q+n-2; j++)
                    {
                        ax = buf[2*j+0];
                        ay = buf[2*j+1];
                        bx = bbuf[j].x;
                        by = bbuf[j].y;
                        tx = ax*bx-ay*by;
                        ty = ax*by+ay*bx;
                        buf[2*j+0] = tx;
                        buf[2*j+1] = -ty;
                    }
                    ftbase.ftapplyplan(plan, buf, 0, 1, _params);
                    t = (double)1/(double)(q+n-1);
                    if( circular )
                    {
                        j1 = Math.Min(i+p+n-2, m-1)-i;
                        j2 = j1+1;
                    }
                    else
                    {
                        j1 = p+n-2;
                        j2 = j1+1;
                    }
                    for(j=0; j<=j1; j++)
                    {
                        r[i+j].x = r[i+j].x+buf[2*j+0]*t;
                        r[i+j].y = r[i+j].y-buf[2*j+1]*t;
                    }
                    for(j=j2; j<=p+n-2; j++)
                    {
                        r[j-j2].x = r[j-j2].x+buf[2*j+0]*t;
                        r[j-j2].y = r[j-j2].y-buf[2*j+1]*t;
                    }
                    i = i+p;
                }
                return;
            }
        }


        /*************************************************************************
        1-dimensional real convolution.

        Extended subroutine which allows to choose convolution algorithm.
        Intended for internal use, ALGLIB users should call ConvR1D().

        INPUT PARAMETERS
            A   -   array[0..M-1] - complex function to be transformed
            M   -   problem size
            B   -   array[0..N-1] - complex function to be transformed
            N   -   problem size, N<=M
            Alg -   algorithm type:
                    *-2     auto-select Q for overlap-add
                    *-1     auto-select algorithm and parameters
                    * 0     straightforward formula for small N's
                    * 1     general FFT-based code
                    * 2     overlap-add with length Q
            Q   -   length for overlap-add

        OUTPUT PARAMETERS
            R   -   convolution: A*B. array[0..N+M-1].

          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void convr1dx(double[] a,
            int m,
            double[] b,
            int n,
            bool circular,
            int alg,
            int q,
            ref double[] r,
            alglib.xparams _params)
        {
            double v = 0;
            int i = 0;
            int j = 0;
            int p = 0;
            int ptotal = 0;
            int i1 = 0;
            int i2 = 0;
            int j1 = 0;
            int j2 = 0;
            double ax = 0;
            double ay = 0;
            double bx = 0;
            double by = 0;
            double tx = 0;
            double ty = 0;
            double flopcand = 0;
            double flopbest = 0;
            int algbest = 0;
            ftbase.fasttransformplan plan = new ftbase.fasttransformplan();
            double[] buf = new double[0];
            double[] buf2 = new double[0];
            double[] buf3 = new double[0];
            int i_ = 0;
            int i1_ = 0;

            r = new double[0];

            alglib.ap.assert(n>0 && m>0, "ConvC1DX: incorrect N or M!");
            alglib.ap.assert(n<=m, "ConvC1DX: N<M assumption is false!");
            
            //
            // handle special cases
            //
            if( Math.Min(m, n)<=2 )
            {
                alg = 0;
            }
            
            //
            // Auto-select
            //
            if( alg<0 )
            {
                
                //
                // Initial candidate: straightforward implementation.
                //
                // If we want to use auto-fitted overlap-add,
                // flop count is initialized by large real number - to force
                // another algorithm selection
                //
                algbest = 0;
                if( alg==-1 )
                {
                    flopbest = 0.15*m*n;
                }
                else
                {
                    flopbest = math.maxrealnumber;
                }
                
                //
                // Another candidate - generic FFT code
                //
                if( alg==-1 )
                {
                    if( (circular && ftbase.ftbaseissmooth(m, _params)) && m%2==0 )
                    {
                        
                        //
                        // special code for circular convolution of a sequence with a smooth length
                        //
                        flopcand = 3*ftbase.ftbasegetflopestimate(m/2, _params)+(double)(6*m)/(double)2;
                        if( (double)(flopcand)<(double)(flopbest) )
                        {
                            algbest = 1;
                            flopbest = flopcand;
                        }
                    }
                    else
                    {
                        
                        //
                        // general cyclic/non-cyclic convolution
                        //
                        p = ftbase.ftbasefindsmootheven(m+n-1, _params);
                        flopcand = 3*ftbase.ftbasegetflopestimate(p/2, _params)+(double)(6*p)/(double)2;
                        if( (double)(flopcand)<(double)(flopbest) )
                        {
                            algbest = 1;
                            flopbest = flopcand;
                        }
                    }
                }
                
                //
                // Another candidate - overlap-add
                //
                q = 1;
                ptotal = 1;
                while( ptotal<n )
                {
                    ptotal = ptotal*2;
                }
                while( ptotal<=m+n-1 )
                {
                    p = ptotal-n+1;
                    flopcand = (int)Math.Ceiling((double)m/(double)p)*(2*ftbase.ftbasegetflopestimate(ptotal/2, _params)+1*(ptotal/2));
                    if( (double)(flopcand)<(double)(flopbest) )
                    {
                        flopbest = flopcand;
                        algbest = 2;
                        q = p;
                    }
                    ptotal = ptotal*2;
                }
                alg = algbest;
                convr1dx(a, m, b, n, circular, alg, q, ref r, _params);
                return;
            }
            
            //
            // straightforward formula for
            // circular and non-circular convolutions.
            //
            // Very simple code, no further comments needed.
            //
            if( alg==0 )
            {
                
                //
                // Special case: N=1
                //
                if( n==1 )
                {
                    r = new double[m];
                    v = b[0];
                    for(i_=0; i_<=m-1;i_++)
                    {
                        r[i_] = v*a[i_];
                    }
                    return;
                }
                
                //
                // use straightforward formula
                //
                if( circular )
                {
                    
                    //
                    // circular convolution
                    //
                    r = new double[m];
                    v = b[0];
                    for(i_=0; i_<=m-1;i_++)
                    {
                        r[i_] = v*a[i_];
                    }
                    for(i=1; i<=n-1; i++)
                    {
                        v = b[i];
                        i1 = 0;
                        i2 = i-1;
                        j1 = m-i;
                        j2 = m-1;
                        i1_ = (j1) - (i1);
                        for(i_=i1; i_<=i2;i_++)
                        {
                            r[i_] = r[i_] + v*a[i_+i1_];
                        }
                        i1 = i;
                        i2 = m-1;
                        j1 = 0;
                        j2 = m-i-1;
                        i1_ = (j1) - (i1);
                        for(i_=i1; i_<=i2;i_++)
                        {
                            r[i_] = r[i_] + v*a[i_+i1_];
                        }
                    }
                }
                else
                {
                    
                    //
                    // non-circular convolution
                    //
                    r = new double[m+n-1];
                    for(i=0; i<=m+n-2; i++)
                    {
                        r[i] = 0;
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        v = b[i];
                        i1_ = (0) - (i);
                        for(i_=i; i_<=i+m-1;i_++)
                        {
                            r[i_] = r[i_] + v*a[i_+i1_];
                        }
                    }
                }
                return;
            }
            
            //
            // general FFT-based code for
            // circular and non-circular convolutions.
            //
            // First, if convolution is circular, we test whether M is smooth or not.
            // If it is smooth, we just use M-length FFT to calculate convolution.
            // If it is not, we calculate non-circular convolution and wrap it arount.
            //
            // If convolution is non-circular, we use zero-padding + FFT.
            //
            // We assume that M+N-1>2 - we should call small case code otherwise
            //
            if( alg==1 )
            {
                alglib.ap.assert(m+n-1>2, "ConvR1DX: internal error!");
                if( (circular && ftbase.ftbaseissmooth(m, _params)) && m%2==0 )
                {
                    
                    //
                    // special code for circular convolution with smooth even M
                    //
                    buf = new double[m];
                    for(i_=0; i_<=m-1;i_++)
                    {
                        buf[i_] = a[i_];
                    }
                    buf2 = new double[m];
                    for(i_=0; i_<=n-1;i_++)
                    {
                        buf2[i_] = b[i_];
                    }
                    for(i=n; i<=m-1; i++)
                    {
                        buf2[i] = 0;
                    }
                    buf3 = new double[m];
                    ftbase.ftcomplexfftplan(m/2, 1, plan, _params);
                    fft.fftr1dinternaleven(ref buf, m, ref buf3, plan, _params);
                    fft.fftr1dinternaleven(ref buf2, m, ref buf3, plan, _params);
                    buf[0] = buf[0]*buf2[0];
                    buf[1] = buf[1]*buf2[1];
                    for(i=1; i<=m/2-1; i++)
                    {
                        ax = buf[2*i+0];
                        ay = buf[2*i+1];
                        bx = buf2[2*i+0];
                        by = buf2[2*i+1];
                        tx = ax*bx-ay*by;
                        ty = ax*by+ay*bx;
                        buf[2*i+0] = tx;
                        buf[2*i+1] = ty;
                    }
                    fft.fftr1dinvinternaleven(ref buf, m, ref buf3, plan, _params);
                    r = new double[m];
                    for(i_=0; i_<=m-1;i_++)
                    {
                        r[i_] = buf[i_];
                    }
                }
                else
                {
                    
                    //
                    // M is non-smooth or non-even, general code (circular/non-circular):
                    // * first part is the same for circular and non-circular
                    //   convolutions. zero padding, FFTs, inverse FFTs
                    // * second part differs:
                    //   * for non-circular convolution we just copy array
                    //   * for circular convolution we add array tail to its head
                    //
                    p = ftbase.ftbasefindsmootheven(m+n-1, _params);
                    buf = new double[p];
                    for(i_=0; i_<=m-1;i_++)
                    {
                        buf[i_] = a[i_];
                    }
                    for(i=m; i<=p-1; i++)
                    {
                        buf[i] = 0;
                    }
                    buf2 = new double[p];
                    for(i_=0; i_<=n-1;i_++)
                    {
                        buf2[i_] = b[i_];
                    }
                    for(i=n; i<=p-1; i++)
                    {
                        buf2[i] = 0;
                    }
                    buf3 = new double[p];
                    ftbase.ftcomplexfftplan(p/2, 1, plan, _params);
                    fft.fftr1dinternaleven(ref buf, p, ref buf3, plan, _params);
                    fft.fftr1dinternaleven(ref buf2, p, ref buf3, plan, _params);
                    buf[0] = buf[0]*buf2[0];
                    buf[1] = buf[1]*buf2[1];
                    for(i=1; i<=p/2-1; i++)
                    {
                        ax = buf[2*i+0];
                        ay = buf[2*i+1];
                        bx = buf2[2*i+0];
                        by = buf2[2*i+1];
                        tx = ax*bx-ay*by;
                        ty = ax*by+ay*bx;
                        buf[2*i+0] = tx;
                        buf[2*i+1] = ty;
                    }
                    fft.fftr1dinvinternaleven(ref buf, p, ref buf3, plan, _params);
                    if( circular )
                    {
                        
                        //
                        // circular, add tail to head
                        //
                        r = new double[m];
                        for(i_=0; i_<=m-1;i_++)
                        {
                            r[i_] = buf[i_];
                        }
                        if( n>=2 )
                        {
                            i1_ = (m) - (0);
                            for(i_=0; i_<=n-2;i_++)
                            {
                                r[i_] = r[i_] + buf[i_+i1_];
                            }
                        }
                    }
                    else
                    {
                        
                        //
                        // non-circular, just copy
                        //
                        r = new double[m+n-1];
                        for(i_=0; i_<=m+n-2;i_++)
                        {
                            r[i_] = buf[i_];
                        }
                    }
                }
                return;
            }
            
            //
            // overlap-add method
            //
            if( alg==2 )
            {
                alglib.ap.assert((q+n-1)%2==0, "ConvR1DX: internal error!");
                buf = new double[q+n-1];
                buf2 = new double[q+n-1];
                buf3 = new double[q+n-1];
                ftbase.ftcomplexfftplan((q+n-1)/2, 1, plan, _params);
                
                //
                // prepare R
                //
                if( circular )
                {
                    r = new double[m];
                    for(i=0; i<=m-1; i++)
                    {
                        r[i] = 0;
                    }
                }
                else
                {
                    r = new double[m+n-1];
                    for(i=0; i<=m+n-2; i++)
                    {
                        r[i] = 0;
                    }
                }
                
                //
                // pre-calculated FFT(B)
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    buf2[i_] = b[i_];
                }
                for(j=n; j<=q+n-2; j++)
                {
                    buf2[j] = 0;
                }
                fft.fftr1dinternaleven(ref buf2, q+n-1, ref buf3, plan, _params);
                
                //
                // main overlap-add cycle
                //
                i = 0;
                while( i<=m-1 )
                {
                    p = Math.Min(q, m-i);
                    i1_ = (i) - (0);
                    for(i_=0; i_<=p-1;i_++)
                    {
                        buf[i_] = a[i_+i1_];
                    }
                    for(j=p; j<=q+n-2; j++)
                    {
                        buf[j] = 0;
                    }
                    fft.fftr1dinternaleven(ref buf, q+n-1, ref buf3, plan, _params);
                    buf[0] = buf[0]*buf2[0];
                    buf[1] = buf[1]*buf2[1];
                    for(j=1; j<=(q+n-1)/2-1; j++)
                    {
                        ax = buf[2*j+0];
                        ay = buf[2*j+1];
                        bx = buf2[2*j+0];
                        by = buf2[2*j+1];
                        tx = ax*bx-ay*by;
                        ty = ax*by+ay*bx;
                        buf[2*j+0] = tx;
                        buf[2*j+1] = ty;
                    }
                    fft.fftr1dinvinternaleven(ref buf, q+n-1, ref buf3, plan, _params);
                    if( circular )
                    {
                        j1 = Math.Min(i+p+n-2, m-1)-i;
                        j2 = j1+1;
                    }
                    else
                    {
                        j1 = p+n-2;
                        j2 = j1+1;
                    }
                    i1_ = (0) - (i);
                    for(i_=i; i_<=i+j1;i_++)
                    {
                        r[i_] = r[i_] + buf[i_+i1_];
                    }
                    if( p+n-2>=j2 )
                    {
                        i1_ = (j2) - (0);
                        for(i_=0; i_<=p+n-2-j2;i_++)
                        {
                            r[i_] = r[i_] + buf[i_+i1_];
                        }
                    }
                    i = i+p;
                }
                return;
            }
        }


    }
    public class corr
    {
        /*************************************************************************
        1-dimensional complex cross-correlation.

        For given Pattern/Signal returns corr(Pattern,Signal) (non-circular).

        Correlation is calculated using reduction to  convolution.  Algorithm with
        max(N,N)*log(max(N,N)) complexity is used (see  ConvC1D()  for  more  info
        about performance).

        IMPORTANT:
            for  historical reasons subroutine accepts its parameters in  reversed
            order: CorrC1D(Signal, Pattern) = Pattern x Signal (using  traditional
            definition of cross-correlation, denoting cross-correlation as "x").

        INPUT PARAMETERS
            Signal  -   array[0..N-1] - complex function to be transformed,
                        signal containing pattern
            N       -   problem size
            Pattern -   array[0..M-1] - complex function to be transformed,
                        pattern to search withing signal
            M       -   problem size

        OUTPUT PARAMETERS
            R       -   cross-correlation, array[0..N+M-2]:
                        * positive lags are stored in R[0..N-1],
                          R[i] = sum(conj(pattern[j])*signal[i+j]
                        * negative lags are stored in R[N..N+M-2],
                          R[N+M-1-i] = sum(conj(pattern[j])*signal[-i+j]

        NOTE:
            It is assumed that pattern domain is [0..M-1].  If Pattern is non-zero
        on [-K..M-1],  you can still use this subroutine, just shift result by K.

          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void corrc1d(complex[] signal,
            int n,
            complex[] pattern,
            int m,
            ref complex[] r,
            alglib.xparams _params)
        {
            complex[] p = new complex[0];
            complex[] b = new complex[0];
            int i = 0;
            int i_ = 0;
            int i1_ = 0;

            r = new complex[0];

            alglib.ap.assert(n>0 && m>0, "CorrC1D: incorrect N or M!");
            p = new complex[m];
            for(i=0; i<=m-1; i++)
            {
                p[m-1-i] = math.conj(pattern[i]);
            }
            conv.convc1d(p, m, signal, n, ref b, _params);
            r = new complex[m+n-1];
            i1_ = (m-1) - (0);
            for(i_=0; i_<=n-1;i_++)
            {
                r[i_] = b[i_+i1_];
            }
            if( m+n-2>=n )
            {
                i1_ = (0) - (n);
                for(i_=n; i_<=m+n-2;i_++)
                {
                    r[i_] = b[i_+i1_];
                }
            }
        }


        /*************************************************************************
        1-dimensional circular complex cross-correlation.

        For given Pattern/Signal returns corr(Pattern,Signal) (circular).
        Algorithm has linearithmic complexity for any M/N.

        IMPORTANT:
            for  historical reasons subroutine accepts its parameters in  reversed
            order:   CorrC1DCircular(Signal, Pattern) = Pattern x Signal    (using
            traditional definition of cross-correlation, denoting cross-correlation
            as "x").

        INPUT PARAMETERS
            Signal  -   array[0..N-1] - complex function to be transformed,
                        periodic signal containing pattern
            N       -   problem size
            Pattern -   array[0..M-1] - complex function to be transformed,
                        non-periodic pattern to search withing signal
            M       -   problem size

        OUTPUT PARAMETERS
            R   -   convolution: A*B. array[0..M-1].


          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void corrc1dcircular(complex[] signal,
            int m,
            complex[] pattern,
            int n,
            ref complex[] c,
            alglib.xparams _params)
        {
            complex[] p = new complex[0];
            complex[] b = new complex[0];
            int i1 = 0;
            int i2 = 0;
            int i = 0;
            int j2 = 0;
            int i_ = 0;
            int i1_ = 0;

            c = new complex[0];

            alglib.ap.assert(n>0 && m>0, "ConvC1DCircular: incorrect N or M!");
            
            //
            // normalize task: make M>=N,
            // so A will be longer (at least - not shorter) that B.
            //
            if( m<n )
            {
                b = new complex[m];
                for(i1=0; i1<=m-1; i1++)
                {
                    b[i1] = 0;
                }
                i1 = 0;
                while( i1<n )
                {
                    i2 = Math.Min(i1+m-1, n-1);
                    j2 = i2-i1;
                    i1_ = (i1) - (0);
                    for(i_=0; i_<=j2;i_++)
                    {
                        b[i_] = b[i_] + pattern[i_+i1_];
                    }
                    i1 = i1+m;
                }
                corrc1dcircular(signal, m, b, m, ref c, _params);
                return;
            }
            
            //
            // Task is normalized
            //
            p = new complex[n];
            for(i=0; i<=n-1; i++)
            {
                p[n-1-i] = math.conj(pattern[i]);
            }
            conv.convc1dcircular(signal, m, p, n, ref b, _params);
            c = new complex[m];
            i1_ = (n-1) - (0);
            for(i_=0; i_<=m-n;i_++)
            {
                c[i_] = b[i_+i1_];
            }
            if( m-n+1<=m-1 )
            {
                i1_ = (0) - (m-n+1);
                for(i_=m-n+1; i_<=m-1;i_++)
                {
                    c[i_] = b[i_+i1_];
                }
            }
        }


        /*************************************************************************
        1-dimensional real cross-correlation.

        For given Pattern/Signal returns corr(Pattern,Signal) (non-circular).

        Correlation is calculated using reduction to  convolution.  Algorithm with
        max(N,N)*log(max(N,N)) complexity is used (see  ConvC1D()  for  more  info
        about performance).

        IMPORTANT:
            for  historical reasons subroutine accepts its parameters in  reversed
            order: CorrR1D(Signal, Pattern) = Pattern x Signal (using  traditional
            definition of cross-correlation, denoting cross-correlation as "x").

        INPUT PARAMETERS
            Signal  -   array[0..N-1] - real function to be transformed,
                        signal containing pattern
            N       -   problem size
            Pattern -   array[0..M-1] - real function to be transformed,
                        pattern to search withing signal
            M       -   problem size

        OUTPUT PARAMETERS
            R       -   cross-correlation, array[0..N+M-2]:
                        * positive lags are stored in R[0..N-1],
                          R[i] = sum(pattern[j]*signal[i+j]
                        * negative lags are stored in R[N..N+M-2],
                          R[N+M-1-i] = sum(pattern[j]*signal[-i+j]

        NOTE:
            It is assumed that pattern domain is [0..M-1].  If Pattern is non-zero
        on [-K..M-1],  you can still use this subroutine, just shift result by K.

          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void corrr1d(double[] signal,
            int n,
            double[] pattern,
            int m,
            ref double[] r,
            alglib.xparams _params)
        {
            double[] p = new double[0];
            double[] b = new double[0];
            int i = 0;
            int i_ = 0;
            int i1_ = 0;

            r = new double[0];

            alglib.ap.assert(n>0 && m>0, "CorrR1D: incorrect N or M!");
            p = new double[m];
            for(i=0; i<=m-1; i++)
            {
                p[m-1-i] = pattern[i];
            }
            conv.convr1d(p, m, signal, n, ref b, _params);
            r = new double[m+n-1];
            i1_ = (m-1) - (0);
            for(i_=0; i_<=n-1;i_++)
            {
                r[i_] = b[i_+i1_];
            }
            if( m+n-2>=n )
            {
                i1_ = (0) - (n);
                for(i_=n; i_<=m+n-2;i_++)
                {
                    r[i_] = b[i_+i1_];
                }
            }
        }


        /*************************************************************************
        1-dimensional circular real cross-correlation.

        For given Pattern/Signal returns corr(Pattern,Signal) (circular).
        Algorithm has linearithmic complexity for any M/N.

        IMPORTANT:
            for  historical reasons subroutine accepts its parameters in  reversed
            order:   CorrR1DCircular(Signal, Pattern) = Pattern x Signal    (using
            traditional definition of cross-correlation, denoting cross-correlation
            as "x").

        INPUT PARAMETERS
            Signal  -   array[0..N-1] - real function to be transformed,
                        periodic signal containing pattern
            N       -   problem size
            Pattern -   array[0..M-1] - real function to be transformed,
                        non-periodic pattern to search withing signal
            M       -   problem size

        OUTPUT PARAMETERS
            R   -   convolution: A*B. array[0..M-1].


          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void corrr1dcircular(double[] signal,
            int m,
            double[] pattern,
            int n,
            ref double[] c,
            alglib.xparams _params)
        {
            double[] p = new double[0];
            double[] b = new double[0];
            int i1 = 0;
            int i2 = 0;
            int i = 0;
            int j2 = 0;
            int i_ = 0;
            int i1_ = 0;

            c = new double[0];

            alglib.ap.assert(n>0 && m>0, "ConvC1DCircular: incorrect N or M!");
            
            //
            // normalize task: make M>=N,
            // so A will be longer (at least - not shorter) that B.
            //
            if( m<n )
            {
                b = new double[m];
                for(i1=0; i1<=m-1; i1++)
                {
                    b[i1] = 0;
                }
                i1 = 0;
                while( i1<n )
                {
                    i2 = Math.Min(i1+m-1, n-1);
                    j2 = i2-i1;
                    i1_ = (i1) - (0);
                    for(i_=0; i_<=j2;i_++)
                    {
                        b[i_] = b[i_] + pattern[i_+i1_];
                    }
                    i1 = i1+m;
                }
                corrr1dcircular(signal, m, b, m, ref c, _params);
                return;
            }
            
            //
            // Task is normalized
            //
            p = new double[n];
            for(i=0; i<=n-1; i++)
            {
                p[n-1-i] = pattern[i];
            }
            conv.convr1dcircular(signal, m, p, n, ref b, _params);
            c = new double[m];
            i1_ = (n-1) - (0);
            for(i_=0; i_<=m-n;i_++)
            {
                c[i_] = b[i_+i1_];
            }
            if( m-n+1<=m-1 )
            {
                i1_ = (0) - (m-n+1);
                for(i_=m-n+1; i_<=m-1;i_++)
                {
                    c[i_] = b[i_+i1_];
                }
            }
        }


    }
}

