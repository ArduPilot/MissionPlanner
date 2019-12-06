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
    Computation of nodes and weights for a Gauss quadrature formula

    The algorithm generates the N-point Gauss quadrature formula  with  weight
    function given by coefficients alpha and beta  of  a  recurrence  relation
    which generates a system of orthogonal polynomials:

    P-1(x)   =  0
    P0(x)    =  1
    Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)

    and zeroth moment Mu0

    Mu0 = integral(W(x)dx,a,b)

    INPUT PARAMETERS:
        Alpha   -   array[0..N-1], alpha coefficients
        Beta    -   array[0..N-1], beta coefficients
                    Zero-indexed element is not used and may be arbitrary.
                    Beta[I]>0.
        Mu0     -   zeroth moment of the weight function.
        N       -   number of nodes of the quadrature formula, N>=1

    OUTPUT PARAMETERS:
        Info    -   error code:
                    * -3    internal eigenproblem solver hasn't converged
                    * -2    Beta[i]<=0
                    * -1    incorrect N was passed
                    *  1    OK
        X       -   array[0..N-1] - array of quadrature nodes,
                    in ascending order.
        W       -   array[0..N-1] - array of quadrature weights.

      -- ALGLIB --
         Copyright 2005-2009 by Bochkanov Sergey
    *************************************************************************/
    public static void gqgeneraterec(double[] alpha, double[] beta, double mu0, int n, out int info, out double[] x, out double[] w)
    {
        info = 0;
        x = new double[0];
        w = new double[0];
        gq.gqgeneraterec(alpha, beta, mu0, n, ref info, ref x, ref w, null);
    }
    
    public static void gqgeneraterec(double[] alpha, double[] beta, double mu0, int n, out int info, out double[] x, out double[] w, alglib.xparams _params)
    {
        info = 0;
        x = new double[0];
        w = new double[0];
        gq.gqgeneraterec(alpha, beta, mu0, n, ref info, ref x, ref w, _params);
    }
    
    /*************************************************************************
    Computation of nodes and weights for a Gauss-Lobatto quadrature formula

    The algorithm generates the N-point Gauss-Lobatto quadrature formula  with
    weight function given by coefficients alpha and beta of a recurrence which
    generates a system of orthogonal polynomials.

    P-1(x)   =  0
    P0(x)    =  1
    Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)

    and zeroth moment Mu0

    Mu0 = integral(W(x)dx,a,b)

    INPUT PARAMETERS:
        Alpha   -   array[0..N-2], alpha coefficients
        Beta    -   array[0..N-2], beta coefficients.
                    Zero-indexed element is not used, may be arbitrary.
                    Beta[I]>0
        Mu0     -   zeroth moment of the weighting function.
        A       -   left boundary of the integration interval.
        B       -   right boundary of the integration interval.
        N       -   number of nodes of the quadrature formula, N>=3
                    (including the left and right boundary nodes).

    OUTPUT PARAMETERS:
        Info    -   error code:
                    * -3    internal eigenproblem solver hasn't converged
                    * -2    Beta[i]<=0
                    * -1    incorrect N was passed
                    *  1    OK
        X       -   array[0..N-1] - array of quadrature nodes,
                    in ascending order.
        W       -   array[0..N-1] - array of quadrature weights.

      -- ALGLIB --
         Copyright 2005-2009 by Bochkanov Sergey
    *************************************************************************/
    public static void gqgenerategausslobattorec(double[] alpha, double[] beta, double mu0, double a, double b, int n, out int info, out double[] x, out double[] w)
    {
        info = 0;
        x = new double[0];
        w = new double[0];
        gq.gqgenerategausslobattorec(alpha, beta, mu0, a, b, n, ref info, ref x, ref w, null);
    }
    
    public static void gqgenerategausslobattorec(double[] alpha, double[] beta, double mu0, double a, double b, int n, out int info, out double[] x, out double[] w, alglib.xparams _params)
    {
        info = 0;
        x = new double[0];
        w = new double[0];
        gq.gqgenerategausslobattorec(alpha, beta, mu0, a, b, n, ref info, ref x, ref w, _params);
    }
    
    /*************************************************************************
    Computation of nodes and weights for a Gauss-Radau quadrature formula

    The algorithm generates the N-point Gauss-Radau  quadrature  formula  with
    weight function given by the coefficients alpha and  beta  of a recurrence
    which generates a system of orthogonal polynomials.

    P-1(x)   =  0
    P0(x)    =  1
    Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)

    and zeroth moment Mu0

    Mu0 = integral(W(x)dx,a,b)

    INPUT PARAMETERS:
        Alpha   -   array[0..N-2], alpha coefficients.
        Beta    -   array[0..N-1], beta coefficients
                    Zero-indexed element is not used.
                    Beta[I]>0
        Mu0     -   zeroth moment of the weighting function.
        A       -   left boundary of the integration interval.
        N       -   number of nodes of the quadrature formula, N>=2
                    (including the left boundary node).

    OUTPUT PARAMETERS:
        Info    -   error code:
                    * -3    internal eigenproblem solver hasn't converged
                    * -2    Beta[i]<=0
                    * -1    incorrect N was passed
                    *  1    OK
        X       -   array[0..N-1] - array of quadrature nodes,
                    in ascending order.
        W       -   array[0..N-1] - array of quadrature weights.


      -- ALGLIB --
         Copyright 2005-2009 by Bochkanov Sergey
    *************************************************************************/
    public static void gqgenerategaussradaurec(double[] alpha, double[] beta, double mu0, double a, int n, out int info, out double[] x, out double[] w)
    {
        info = 0;
        x = new double[0];
        w = new double[0];
        gq.gqgenerategaussradaurec(alpha, beta, mu0, a, n, ref info, ref x, ref w, null);
    }
    
    public static void gqgenerategaussradaurec(double[] alpha, double[] beta, double mu0, double a, int n, out int info, out double[] x, out double[] w, alglib.xparams _params)
    {
        info = 0;
        x = new double[0];
        w = new double[0];
        gq.gqgenerategaussradaurec(alpha, beta, mu0, a, n, ref info, ref x, ref w, _params);
    }
    
    /*************************************************************************
    Returns nodes/weights for Gauss-Legendre quadrature on [-1,1] with N
    nodes.

    INPUT PARAMETERS:
        N           -   number of nodes, >=1

    OUTPUT PARAMETERS:
        Info        -   error code:
                        * -4    an  error   was   detected   when  calculating
                                weights/nodes.  N  is  too  large   to  obtain
                                weights/nodes  with  high   enough   accuracy.
                                Try  to   use   multiple   precision  version.
                        * -3    internal eigenproblem solver hasn't  converged
                        * -1    incorrect N was passed
                        * +1    OK
        X           -   array[0..N-1] - array of quadrature nodes,
                        in ascending order.
        W           -   array[0..N-1] - array of quadrature weights.


      -- ALGLIB --
         Copyright 12.05.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void gqgenerategausslegendre(int n, out int info, out double[] x, out double[] w)
    {
        info = 0;
        x = new double[0];
        w = new double[0];
        gq.gqgenerategausslegendre(n, ref info, ref x, ref w, null);
    }
    
    public static void gqgenerategausslegendre(int n, out int info, out double[] x, out double[] w, alglib.xparams _params)
    {
        info = 0;
        x = new double[0];
        w = new double[0];
        gq.gqgenerategausslegendre(n, ref info, ref x, ref w, _params);
    }
    
    /*************************************************************************
    Returns  nodes/weights  for  Gauss-Jacobi quadrature on [-1,1] with weight
    function W(x)=Power(1-x,Alpha)*Power(1+x,Beta).

    INPUT PARAMETERS:
        N           -   number of nodes, >=1
        Alpha       -   power-law coefficient, Alpha>-1
        Beta        -   power-law coefficient, Beta>-1

    OUTPUT PARAMETERS:
        Info        -   error code:
                        * -4    an  error  was   detected   when   calculating
                                weights/nodes. Alpha or  Beta  are  too  close
                                to -1 to obtain weights/nodes with high enough
                                accuracy, or, may be, N is too large.  Try  to
                                use multiple precision version.
                        * -3    internal eigenproblem solver hasn't converged
                        * -1    incorrect N/Alpha/Beta was passed
                        * +1    OK
        X           -   array[0..N-1] - array of quadrature nodes,
                        in ascending order.
        W           -   array[0..N-1] - array of quadrature weights.


      -- ALGLIB --
         Copyright 12.05.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void gqgenerategaussjacobi(int n, double alpha, double beta, out int info, out double[] x, out double[] w)
    {
        info = 0;
        x = new double[0];
        w = new double[0];
        gq.gqgenerategaussjacobi(n, alpha, beta, ref info, ref x, ref w, null);
    }
    
    public static void gqgenerategaussjacobi(int n, double alpha, double beta, out int info, out double[] x, out double[] w, alglib.xparams _params)
    {
        info = 0;
        x = new double[0];
        w = new double[0];
        gq.gqgenerategaussjacobi(n, alpha, beta, ref info, ref x, ref w, _params);
    }
    
    /*************************************************************************
    Returns  nodes/weights  for  Gauss-Laguerre  quadrature  on  [0,+inf) with
    weight function W(x)=Power(x,Alpha)*Exp(-x)

    INPUT PARAMETERS:
        N           -   number of nodes, >=1
        Alpha       -   power-law coefficient, Alpha>-1

    OUTPUT PARAMETERS:
        Info        -   error code:
                        * -4    an  error  was   detected   when   calculating
                                weights/nodes. Alpha is too  close  to  -1  to
                                obtain weights/nodes with high enough accuracy
                                or, may  be,  N  is  too  large.  Try  to  use
                                multiple precision version.
                        * -3    internal eigenproblem solver hasn't converged
                        * -1    incorrect N/Alpha was passed
                        * +1    OK
        X           -   array[0..N-1] - array of quadrature nodes,
                        in ascending order.
        W           -   array[0..N-1] - array of quadrature weights.


      -- ALGLIB --
         Copyright 12.05.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void gqgenerategausslaguerre(int n, double alpha, out int info, out double[] x, out double[] w)
    {
        info = 0;
        x = new double[0];
        w = new double[0];
        gq.gqgenerategausslaguerre(n, alpha, ref info, ref x, ref w, null);
    }
    
    public static void gqgenerategausslaguerre(int n, double alpha, out int info, out double[] x, out double[] w, alglib.xparams _params)
    {
        info = 0;
        x = new double[0];
        w = new double[0];
        gq.gqgenerategausslaguerre(n, alpha, ref info, ref x, ref w, _params);
    }
    
    /*************************************************************************
    Returns  nodes/weights  for  Gauss-Hermite  quadrature on (-inf,+inf) with
    weight function W(x)=Exp(-x*x)

    INPUT PARAMETERS:
        N           -   number of nodes, >=1

    OUTPUT PARAMETERS:
        Info        -   error code:
                        * -4    an  error  was   detected   when   calculating
                                weights/nodes.  May be, N is too large. Try to
                                use multiple precision version.
                        * -3    internal eigenproblem solver hasn't converged
                        * -1    incorrect N/Alpha was passed
                        * +1    OK
        X           -   array[0..N-1] - array of quadrature nodes,
                        in ascending order.
        W           -   array[0..N-1] - array of quadrature weights.


      -- ALGLIB --
         Copyright 12.05.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void gqgenerategausshermite(int n, out int info, out double[] x, out double[] w)
    {
        info = 0;
        x = new double[0];
        w = new double[0];
        gq.gqgenerategausshermite(n, ref info, ref x, ref w, null);
    }
    
    public static void gqgenerategausshermite(int n, out int info, out double[] x, out double[] w, alglib.xparams _params)
    {
        info = 0;
        x = new double[0];
        w = new double[0];
        gq.gqgenerategausshermite(n, ref info, ref x, ref w, _params);
    }

}
public partial class alglib
{

    
    /*************************************************************************
    Computation of nodes and weights of a Gauss-Kronrod quadrature formula

    The algorithm generates the N-point Gauss-Kronrod quadrature formula  with
    weight  function  given  by  coefficients  alpha  and beta of a recurrence
    relation which generates a system of orthogonal polynomials:

        P-1(x)   =  0
        P0(x)    =  1
        Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)

    and zero moment Mu0

        Mu0 = integral(W(x)dx,a,b)


    INPUT PARAMETERS:
        Alpha       -   alpha coefficients, array[0..floor(3*K/2)].
        Beta        -   beta coefficients,  array[0..ceil(3*K/2)].
                        Beta[0] is not used and may be arbitrary.
                        Beta[I]>0.
        Mu0         -   zeroth moment of the weight function.
        N           -   number of nodes of the Gauss-Kronrod quadrature formula,
                        N >= 3,
                        N =  2*K+1.

    OUTPUT PARAMETERS:
        Info        -   error code:
                        * -5    no real and positive Gauss-Kronrod formula can
                                be created for such a weight function  with  a
                                given number of nodes.
                        * -4    N is too large, task may be ill  conditioned -
                                x[i]=x[i+1] found.
                        * -3    internal eigenproblem solver hasn't converged
                        * -2    Beta[i]<=0
                        * -1    incorrect N was passed
                        * +1    OK
        X           -   array[0..N-1] - array of quadrature nodes,
                        in ascending order.
        WKronrod    -   array[0..N-1] - Kronrod weights
        WGauss      -   array[0..N-1] - Gauss weights (interleaved with zeros
                        corresponding to extended Kronrod nodes).

      -- ALGLIB --
         Copyright 08.05.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void gkqgeneraterec(double[] alpha, double[] beta, double mu0, int n, out int info, out double[] x, out double[] wkronrod, out double[] wgauss)
    {
        info = 0;
        x = new double[0];
        wkronrod = new double[0];
        wgauss = new double[0];
        gkq.gkqgeneraterec(alpha, beta, mu0, n, ref info, ref x, ref wkronrod, ref wgauss, null);
    }
    
    public static void gkqgeneraterec(double[] alpha, double[] beta, double mu0, int n, out int info, out double[] x, out double[] wkronrod, out double[] wgauss, alglib.xparams _params)
    {
        info = 0;
        x = new double[0];
        wkronrod = new double[0];
        wgauss = new double[0];
        gkq.gkqgeneraterec(alpha, beta, mu0, n, ref info, ref x, ref wkronrod, ref wgauss, _params);
    }
    
    /*************************************************************************
    Returns   Gauss   and   Gauss-Kronrod   nodes/weights  for  Gauss-Legendre
    quadrature with N points.

    GKQLegendreCalc (calculation) or  GKQLegendreTbl  (precomputed  table)  is
    used depending on machine precision and number of nodes.

    INPUT PARAMETERS:
        N           -   number of Kronrod nodes, must be odd number, >=3.

    OUTPUT PARAMETERS:
        Info        -   error code:
                        * -4    an  error   was   detected   when  calculating
                                weights/nodes.  N  is  too  large   to  obtain
                                weights/nodes  with  high   enough   accuracy.
                                Try  to   use   multiple   precision  version.
                        * -3    internal eigenproblem solver hasn't converged
                        * -1    incorrect N was passed
                        * +1    OK
        X           -   array[0..N-1] - array of quadrature nodes, ordered in
                        ascending order.
        WKronrod    -   array[0..N-1] - Kronrod weights
        WGauss      -   array[0..N-1] - Gauss weights (interleaved with zeros
                        corresponding to extended Kronrod nodes).


      -- ALGLIB --
         Copyright 12.05.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void gkqgenerategausslegendre(int n, out int info, out double[] x, out double[] wkronrod, out double[] wgauss)
    {
        info = 0;
        x = new double[0];
        wkronrod = new double[0];
        wgauss = new double[0];
        gkq.gkqgenerategausslegendre(n, ref info, ref x, ref wkronrod, ref wgauss, null);
    }
    
    public static void gkqgenerategausslegendre(int n, out int info, out double[] x, out double[] wkronrod, out double[] wgauss, alglib.xparams _params)
    {
        info = 0;
        x = new double[0];
        wkronrod = new double[0];
        wgauss = new double[0];
        gkq.gkqgenerategausslegendre(n, ref info, ref x, ref wkronrod, ref wgauss, _params);
    }
    
    /*************************************************************************
    Returns   Gauss   and   Gauss-Kronrod   nodes/weights   for   Gauss-Jacobi
    quadrature on [-1,1] with weight function

        W(x)=Power(1-x,Alpha)*Power(1+x,Beta).

    INPUT PARAMETERS:
        N           -   number of Kronrod nodes, must be odd number, >=3.
        Alpha       -   power-law coefficient, Alpha>-1
        Beta        -   power-law coefficient, Beta>-1

    OUTPUT PARAMETERS:
        Info        -   error code:
                        * -5    no real and positive Gauss-Kronrod formula can
                                be created for such a weight function  with  a
                                given number of nodes.
                        * -4    an  error  was   detected   when   calculating
                                weights/nodes. Alpha or  Beta  are  too  close
                                to -1 to obtain weights/nodes with high enough
                                accuracy, or, may be, N is too large.  Try  to
                                use multiple precision version.
                        * -3    internal eigenproblem solver hasn't converged
                        * -1    incorrect N was passed
                        * +1    OK
                        * +2    OK, but quadrature rule have exterior  nodes,
                                x[0]<-1 or x[n-1]>+1
        X           -   array[0..N-1] - array of quadrature nodes, ordered in
                        ascending order.
        WKronrod    -   array[0..N-1] - Kronrod weights
        WGauss      -   array[0..N-1] - Gauss weights (interleaved with zeros
                        corresponding to extended Kronrod nodes).


      -- ALGLIB --
         Copyright 12.05.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void gkqgenerategaussjacobi(int n, double alpha, double beta, out int info, out double[] x, out double[] wkronrod, out double[] wgauss)
    {
        info = 0;
        x = new double[0];
        wkronrod = new double[0];
        wgauss = new double[0];
        gkq.gkqgenerategaussjacobi(n, alpha, beta, ref info, ref x, ref wkronrod, ref wgauss, null);
    }
    
    public static void gkqgenerategaussjacobi(int n, double alpha, double beta, out int info, out double[] x, out double[] wkronrod, out double[] wgauss, alglib.xparams _params)
    {
        info = 0;
        x = new double[0];
        wkronrod = new double[0];
        wgauss = new double[0];
        gkq.gkqgenerategaussjacobi(n, alpha, beta, ref info, ref x, ref wkronrod, ref wgauss, _params);
    }
    
    /*************************************************************************
    Returns Gauss and Gauss-Kronrod nodes for quadrature with N points.

    Reduction to tridiagonal eigenproblem is used.

    INPUT PARAMETERS:
        N           -   number of Kronrod nodes, must be odd number, >=3.

    OUTPUT PARAMETERS:
        Info        -   error code:
                        * -4    an  error   was   detected   when  calculating
                                weights/nodes.  N  is  too  large   to  obtain
                                weights/nodes  with  high   enough   accuracy.
                                Try  to   use   multiple   precision  version.
                        * -3    internal eigenproblem solver hasn't converged
                        * -1    incorrect N was passed
                        * +1    OK
        X           -   array[0..N-1] - array of quadrature nodes, ordered in
                        ascending order.
        WKronrod    -   array[0..N-1] - Kronrod weights
        WGauss      -   array[0..N-1] - Gauss weights (interleaved with zeros
                        corresponding to extended Kronrod nodes).

      -- ALGLIB --
         Copyright 12.05.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void gkqlegendrecalc(int n, out int info, out double[] x, out double[] wkronrod, out double[] wgauss)
    {
        info = 0;
        x = new double[0];
        wkronrod = new double[0];
        wgauss = new double[0];
        gkq.gkqlegendrecalc(n, ref info, ref x, ref wkronrod, ref wgauss, null);
    }
    
    public static void gkqlegendrecalc(int n, out int info, out double[] x, out double[] wkronrod, out double[] wgauss, alglib.xparams _params)
    {
        info = 0;
        x = new double[0];
        wkronrod = new double[0];
        wgauss = new double[0];
        gkq.gkqlegendrecalc(n, ref info, ref x, ref wkronrod, ref wgauss, _params);
    }
    
    /*************************************************************************
    Returns Gauss and Gauss-Kronrod nodes for quadrature with N  points  using
    pre-calculated table. Nodes/weights were  computed  with  accuracy  up  to
    1.0E-32 (if MPFR version of ALGLIB is used). In standard double  precision
    accuracy reduces to something about 2.0E-16 (depending  on your compiler's
    handling of long floating point constants).

    INPUT PARAMETERS:
        N           -   number of Kronrod nodes.
                        N can be 15, 21, 31, 41, 51, 61.

    OUTPUT PARAMETERS:
        X           -   array[0..N-1] - array of quadrature nodes, ordered in
                        ascending order.
        WKronrod    -   array[0..N-1] - Kronrod weights
        WGauss      -   array[0..N-1] - Gauss weights (interleaved with zeros
                        corresponding to extended Kronrod nodes).


      -- ALGLIB --
         Copyright 12.05.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void gkqlegendretbl(int n, out double[] x, out double[] wkronrod, out double[] wgauss, out double eps)
    {
        x = new double[0];
        wkronrod = new double[0];
        wgauss = new double[0];
        eps = 0;
        gkq.gkqlegendretbl(n, ref x, ref wkronrod, ref wgauss, ref eps, null);
    }
    
    public static void gkqlegendretbl(int n, out double[] x, out double[] wkronrod, out double[] wgauss, out double eps, alglib.xparams _params)
    {
        x = new double[0];
        wkronrod = new double[0];
        wgauss = new double[0];
        eps = 0;
        gkq.gkqlegendretbl(n, ref x, ref wkronrod, ref wgauss, ref eps, _params);
    }

}
public partial class alglib
{


    /*************************************************************************
    Integration report:
    * TerminationType = completetion code:
        * -5    non-convergence of Gauss-Kronrod nodes
                calculation subroutine.
        * -1    incorrect parameters were specified
        *  1    OK
    * Rep.NFEV countains number of function calculations
    * Rep.NIntervals contains number of intervals [a,b]
      was partitioned into.
    *************************************************************************/
    public class autogkreport : alglibobject
    {
        //
        // Public declarations
        //
        public int terminationtype { get { return _innerobj.terminationtype; } set { _innerobj.terminationtype = value; } }
        public int nfev { get { return _innerobj.nfev; } set { _innerobj.nfev = value; } }
        public int nintervals { get { return _innerobj.nintervals; } set { _innerobj.nintervals = value; } }
    
        public autogkreport()
        {
            _innerobj = new autogk.autogkreport();
        }
        
        public override alglib.alglibobject make_copy()
        {
            return new autogkreport((autogk.autogkreport)_innerobj.make_copy());
        }
    
        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private autogk.autogkreport _innerobj;
        public autogk.autogkreport innerobj { get { return _innerobj; } }
        public autogkreport(autogk.autogkreport obj)
        {
            _innerobj = obj;
        }
    }


    /*************************************************************************
    This structure stores state of the integration algorithm.

    Although this class has public fields,  they are not intended for external
    use. You should use ALGLIB functions to work with this class:
    * autogksmooth()/AutoGKSmoothW()/... to create objects
    * autogkintegrate() to begin integration
    * autogkresults() to get results
    *************************************************************************/
    public class autogkstate : alglibobject
    {
        //
        // Public declarations
        //
        public bool needf { get { return _innerobj.needf; } set { _innerobj.needf = value; } }
        public double x { get { return _innerobj.x; } set { _innerobj.x = value; } }
        public double xminusa { get { return _innerobj.xminusa; } set { _innerobj.xminusa = value; } }
        public double bminusx { get { return _innerobj.bminusx; } set { _innerobj.bminusx = value; } }
        public double f { get { return _innerobj.f; } set { _innerobj.f = value; } }
    
        public autogkstate()
        {
            _innerobj = new autogk.autogkstate();
        }
        
        public override alglib.alglibobject make_copy()
        {
            return new autogkstate((autogk.autogkstate)_innerobj.make_copy());
        }
    
        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private autogk.autogkstate _innerobj;
        public autogk.autogkstate innerobj { get { return _innerobj; } }
        public autogkstate(autogk.autogkstate obj)
        {
            _innerobj = obj;
        }
    }
    
    /*************************************************************************
    Integration of a smooth function F(x) on a finite interval [a,b].

    Fast-convergent algorithm based on a Gauss-Kronrod formula is used. Result
    is calculated with accuracy close to the machine precision.

    Algorithm works well only with smooth integrands.  It  may  be  used  with
    continuous non-smooth integrands, but with  less  performance.

    It should never be used with integrands which have integrable singularities
    at lower or upper limits - algorithm may crash. Use AutoGKSingular in such
    cases.

    INPUT PARAMETERS:
        A, B    -   interval boundaries (A<B, A=B or A>B)

    OUTPUT PARAMETERS
        State   -   structure which stores algorithm state

    SEE ALSO
        AutoGKSmoothW, AutoGKSingular, AutoGKResults.


      -- ALGLIB --
         Copyright 06.05.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void autogksmooth(double a, double b, out autogkstate state)
    {
        state = new autogkstate();
        autogk.autogksmooth(a, b, state.innerobj, null);
    }
    
    public static void autogksmooth(double a, double b, out autogkstate state, alglib.xparams _params)
    {
        state = new autogkstate();
        autogk.autogksmooth(a, b, state.innerobj, _params);
    }
    
    /*************************************************************************
    Integration of a smooth function F(x) on a finite interval [a,b].

    This subroutine is same as AutoGKSmooth(), but it guarantees that interval
    [a,b] is partitioned into subintervals which have width at most XWidth.

    Subroutine  can  be  used  when  integrating nearly-constant function with
    narrow "bumps" (about XWidth wide). If "bumps" are too narrow, AutoGKSmooth
    subroutine can overlook them.

    INPUT PARAMETERS:
        A, B    -   interval boundaries (A<B, A=B or A>B)

    OUTPUT PARAMETERS
        State   -   structure which stores algorithm state

    SEE ALSO
        AutoGKSmooth, AutoGKSingular, AutoGKResults.


      -- ALGLIB --
         Copyright 06.05.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void autogksmoothw(double a, double b, double xwidth, out autogkstate state)
    {
        state = new autogkstate();
        autogk.autogksmoothw(a, b, xwidth, state.innerobj, null);
    }
    
    public static void autogksmoothw(double a, double b, double xwidth, out autogkstate state, alglib.xparams _params)
    {
        state = new autogkstate();
        autogk.autogksmoothw(a, b, xwidth, state.innerobj, _params);
    }
    
    /*************************************************************************
    Integration on a finite interval [A,B].
    Integrand have integrable singularities at A/B.

    F(X) must diverge as "(x-A)^alpha" at A, as "(B-x)^beta" at B,  with known
    alpha/beta (alpha>-1, beta>-1).  If alpha/beta  are  not known,  estimates
    from below can be used (but these estimates should be greater than -1 too).

    One  of  alpha/beta variables (or even both alpha/beta) may be equal to 0,
    which means than function F(x) is non-singular at A/B. Anyway (singular at
    bounds or not), function F(x) is supposed to be continuous on (A,B).

    Fast-convergent algorithm based on a Gauss-Kronrod formula is used. Result
    is calculated with accuracy close to the machine precision.

    INPUT PARAMETERS:
        A, B    -   interval boundaries (A<B, A=B or A>B)
        Alpha   -   power-law coefficient of the F(x) at A,
                    Alpha>-1
        Beta    -   power-law coefficient of the F(x) at B,
                    Beta>-1

    OUTPUT PARAMETERS
        State   -   structure which stores algorithm state

    SEE ALSO
        AutoGKSmooth, AutoGKSmoothW, AutoGKResults.


      -- ALGLIB --
         Copyright 06.05.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void autogksingular(double a, double b, double alpha, double beta, out autogkstate state)
    {
        state = new autogkstate();
        autogk.autogksingular(a, b, alpha, beta, state.innerobj, null);
    }
    
    public static void autogksingular(double a, double b, double alpha, double beta, out autogkstate state, alglib.xparams _params)
    {
        state = new autogkstate();
        autogk.autogksingular(a, b, alpha, beta, state.innerobj, _params);
    }
    
    /*************************************************************************
    This function provides reverse communication interface
    Reverse communication interface is not documented or recommended to use.
    See below for functions which provide better documented API
    *************************************************************************/
    public static bool autogkiteration(autogkstate state)
    {
    
        return autogk.autogkiteration(state.innerobj, null);
    }
    
    public static bool autogkiteration(autogkstate state, alglib.xparams _params)
    {
    
        return autogk.autogkiteration(state.innerobj, _params);
    }


    /*************************************************************************
    This function is used to launcn iterations of ODE solver

    It accepts following parameters:
        diff    -   callback which calculates dy/dx for given y and x
        obj     -   optional object which is passed to diff; can be NULL


      -- ALGLIB --
         Copyright 07.05.2009 by Bochkanov Sergey

    *************************************************************************/
    public static void autogkintegrate(autogkstate state, integrator1_func func, object obj)
    {
        autogkintegrate(state, func, obj, null);
    }
        
    public static void autogkintegrate(autogkstate state, integrator1_func func, object obj, alglib.xparams _params)
    {
        if( func==null )
            throw new alglibexception("ALGLIB: error in 'autogkintegrate()' (func is null)");
        while( alglib.autogkiteration(state, _params) )
        {
            if( state.needf )
            {
                func(state.innerobj.x, state.innerobj.xminusa, state.innerobj.bminusx, ref state.innerobj.f, obj);
                continue;
            }
            throw new alglibexception("ALGLIB: unexpected error in 'autogksolve'");
        }
    }
    
    /*************************************************************************
    Adaptive integration results

    Called after AutoGKIteration returned False.

    Input parameters:
        State   -   algorithm state (used by AutoGKIteration).

    Output parameters:
        V       -   integral(f(x)dx,a,b)
        Rep     -   optimization report (see AutoGKReport description)

      -- ALGLIB --
         Copyright 14.11.2007 by Bochkanov Sergey
    *************************************************************************/
    public static void autogkresults(autogkstate state, out double v, out autogkreport rep)
    {
        v = 0;
        rep = new autogkreport();
        autogk.autogkresults(state.innerobj, ref v, rep.innerobj, null);
    }
    
    public static void autogkresults(autogkstate state, out double v, out autogkreport rep, alglib.xparams _params)
    {
        v = 0;
        rep = new autogkreport();
        autogk.autogkresults(state.innerobj, ref v, rep.innerobj, _params);
    }

}
public partial class alglib
{
    public class gq
    {
        /*************************************************************************
        Computation of nodes and weights for a Gauss quadrature formula

        The algorithm generates the N-point Gauss quadrature formula  with  weight
        function given by coefficients alpha and beta  of  a  recurrence  relation
        which generates a system of orthogonal polynomials:

        P-1(x)   =  0
        P0(x)    =  1
        Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)

        and zeroth moment Mu0

        Mu0 = integral(W(x)dx,a,b)

        INPUT PARAMETERS:
            Alpha   -   array[0..N-1], alpha coefficients
            Beta    -   array[0..N-1], beta coefficients
                        Zero-indexed element is not used and may be arbitrary.
                        Beta[I]>0.
            Mu0     -   zeroth moment of the weight function.
            N       -   number of nodes of the quadrature formula, N>=1

        OUTPUT PARAMETERS:
            Info    -   error code:
                        * -3    internal eigenproblem solver hasn't converged
                        * -2    Beta[i]<=0
                        * -1    incorrect N was passed
                        *  1    OK
            X       -   array[0..N-1] - array of quadrature nodes,
                        in ascending order.
            W       -   array[0..N-1] - array of quadrature weights.

          -- ALGLIB --
             Copyright 2005-2009 by Bochkanov Sergey
        *************************************************************************/
        public static void gqgeneraterec(double[] alpha,
            double[] beta,
            double mu0,
            int n,
            ref int info,
            ref double[] x,
            ref double[] w,
            alglib.xparams _params)
        {
            int i = 0;
            double[] d = new double[0];
            double[] e = new double[0];
            double[,] z = new double[0,0];

            info = 0;
            x = new double[0];
            w = new double[0];

            if( n<1 )
            {
                info = -1;
                return;
            }
            info = 1;
            
            //
            // Initialize
            //
            d = new double[n];
            e = new double[n];
            for(i=1; i<=n-1; i++)
            {
                d[i-1] = alpha[i-1];
                if( (double)(beta[i])<=(double)(0) )
                {
                    info = -2;
                    return;
                }
                e[i-1] = Math.Sqrt(beta[i]);
            }
            d[n-1] = alpha[n-1];
            
            //
            // EVD
            //
            if( !evd.smatrixtdevd(ref d, e, n, 3, ref z, _params) )
            {
                info = -3;
                return;
            }
            
            //
            // Generate
            //
            x = new double[n];
            w = new double[n];
            for(i=1; i<=n; i++)
            {
                x[i-1] = d[i-1];
                w[i-1] = mu0*math.sqr(z[0,i-1]);
            }
        }


        /*************************************************************************
        Computation of nodes and weights for a Gauss-Lobatto quadrature formula

        The algorithm generates the N-point Gauss-Lobatto quadrature formula  with
        weight function given by coefficients alpha and beta of a recurrence which
        generates a system of orthogonal polynomials.

        P-1(x)   =  0
        P0(x)    =  1
        Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)

        and zeroth moment Mu0

        Mu0 = integral(W(x)dx,a,b)

        INPUT PARAMETERS:
            Alpha   -   array[0..N-2], alpha coefficients
            Beta    -   array[0..N-2], beta coefficients.
                        Zero-indexed element is not used, may be arbitrary.
                        Beta[I]>0
            Mu0     -   zeroth moment of the weighting function.
            A       -   left boundary of the integration interval.
            B       -   right boundary of the integration interval.
            N       -   number of nodes of the quadrature formula, N>=3
                        (including the left and right boundary nodes).

        OUTPUT PARAMETERS:
            Info    -   error code:
                        * -3    internal eigenproblem solver hasn't converged
                        * -2    Beta[i]<=0
                        * -1    incorrect N was passed
                        *  1    OK
            X       -   array[0..N-1] - array of quadrature nodes,
                        in ascending order.
            W       -   array[0..N-1] - array of quadrature weights.

          -- ALGLIB --
             Copyright 2005-2009 by Bochkanov Sergey
        *************************************************************************/
        public static void gqgenerategausslobattorec(double[] alpha,
            double[] beta,
            double mu0,
            double a,
            double b,
            int n,
            ref int info,
            ref double[] x,
            ref double[] w,
            alglib.xparams _params)
        {
            int i = 0;
            double[] d = new double[0];
            double[] e = new double[0];
            double[,] z = new double[0,0];
            double pim1a = 0;
            double pia = 0;
            double pim1b = 0;
            double pib = 0;
            double t = 0;
            double a11 = 0;
            double a12 = 0;
            double a21 = 0;
            double a22 = 0;
            double b1 = 0;
            double b2 = 0;
            double alph = 0;
            double bet = 0;

            alpha = (double[])alpha.Clone();
            beta = (double[])beta.Clone();
            info = 0;
            x = new double[0];
            w = new double[0];

            if( n<=2 )
            {
                info = -1;
                return;
            }
            info = 1;
            
            //
            // Initialize, D[1:N+1], E[1:N]
            //
            n = n-2;
            d = new double[n+2];
            e = new double[n+1];
            for(i=1; i<=n+1; i++)
            {
                d[i-1] = alpha[i-1];
            }
            for(i=1; i<=n; i++)
            {
                if( (double)(beta[i])<=(double)(0) )
                {
                    info = -2;
                    return;
                }
                e[i-1] = Math.Sqrt(beta[i]);
            }
            
            //
            // Caclulate Pn(a), Pn+1(a), Pn(b), Pn+1(b)
            //
            beta[0] = 0;
            pim1a = 0;
            pia = 1;
            pim1b = 0;
            pib = 1;
            for(i=1; i<=n+1; i++)
            {
                
                //
                // Pi(a)
                //
                t = (a-alpha[i-1])*pia-beta[i-1]*pim1a;
                pim1a = pia;
                pia = t;
                
                //
                // Pi(b)
                //
                t = (b-alpha[i-1])*pib-beta[i-1]*pim1b;
                pim1b = pib;
                pib = t;
            }
            
            //
            // Calculate alpha'(n+1), beta'(n+1)
            //
            a11 = pia;
            a12 = pim1a;
            a21 = pib;
            a22 = pim1b;
            b1 = a*pia;
            b2 = b*pib;
            if( (double)(Math.Abs(a11))>(double)(Math.Abs(a21)) )
            {
                a22 = a22-a12*a21/a11;
                b2 = b2-b1*a21/a11;
                bet = b2/a22;
                alph = (b1-bet*a12)/a11;
            }
            else
            {
                a12 = a12-a22*a11/a21;
                b1 = b1-b2*a11/a21;
                bet = b1/a12;
                alph = (b2-bet*a22)/a21;
            }
            if( (double)(bet)<(double)(0) )
            {
                info = -3;
                return;
            }
            d[n+1] = alph;
            e[n] = Math.Sqrt(bet);
            
            //
            // EVD
            //
            if( !evd.smatrixtdevd(ref d, e, n+2, 3, ref z, _params) )
            {
                info = -3;
                return;
            }
            
            //
            // Generate
            //
            x = new double[n+2];
            w = new double[n+2];
            for(i=1; i<=n+2; i++)
            {
                x[i-1] = d[i-1];
                w[i-1] = mu0*math.sqr(z[0,i-1]);
            }
        }


        /*************************************************************************
        Computation of nodes and weights for a Gauss-Radau quadrature formula

        The algorithm generates the N-point Gauss-Radau  quadrature  formula  with
        weight function given by the coefficients alpha and  beta  of a recurrence
        which generates a system of orthogonal polynomials.

        P-1(x)   =  0
        P0(x)    =  1
        Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)

        and zeroth moment Mu0

        Mu0 = integral(W(x)dx,a,b)

        INPUT PARAMETERS:
            Alpha   -   array[0..N-2], alpha coefficients.
            Beta    -   array[0..N-1], beta coefficients
                        Zero-indexed element is not used.
                        Beta[I]>0
            Mu0     -   zeroth moment of the weighting function.
            A       -   left boundary of the integration interval.
            N       -   number of nodes of the quadrature formula, N>=2
                        (including the left boundary node).

        OUTPUT PARAMETERS:
            Info    -   error code:
                        * -3    internal eigenproblem solver hasn't converged
                        * -2    Beta[i]<=0
                        * -1    incorrect N was passed
                        *  1    OK
            X       -   array[0..N-1] - array of quadrature nodes,
                        in ascending order.
            W       -   array[0..N-1] - array of quadrature weights.


          -- ALGLIB --
             Copyright 2005-2009 by Bochkanov Sergey
        *************************************************************************/
        public static void gqgenerategaussradaurec(double[] alpha,
            double[] beta,
            double mu0,
            double a,
            int n,
            ref int info,
            ref double[] x,
            ref double[] w,
            alglib.xparams _params)
        {
            int i = 0;
            double[] d = new double[0];
            double[] e = new double[0];
            double[,] z = new double[0,0];
            double polim1 = 0;
            double poli = 0;
            double t = 0;

            alpha = (double[])alpha.Clone();
            beta = (double[])beta.Clone();
            info = 0;
            x = new double[0];
            w = new double[0];

            if( n<2 )
            {
                info = -1;
                return;
            }
            info = 1;
            
            //
            // Initialize, D[1:N], E[1:N]
            //
            n = n-1;
            d = new double[n+1];
            e = new double[n];
            for(i=1; i<=n; i++)
            {
                d[i-1] = alpha[i-1];
                if( (double)(beta[i])<=(double)(0) )
                {
                    info = -2;
                    return;
                }
                e[i-1] = Math.Sqrt(beta[i]);
            }
            
            //
            // Caclulate Pn(a), Pn-1(a), and D[N+1]
            //
            beta[0] = 0;
            polim1 = 0;
            poli = 1;
            for(i=1; i<=n; i++)
            {
                t = (a-alpha[i-1])*poli-beta[i-1]*polim1;
                polim1 = poli;
                poli = t;
            }
            d[n] = a-beta[n]*polim1/poli;
            
            //
            // EVD
            //
            if( !evd.smatrixtdevd(ref d, e, n+1, 3, ref z, _params) )
            {
                info = -3;
                return;
            }
            
            //
            // Generate
            //
            x = new double[n+1];
            w = new double[n+1];
            for(i=1; i<=n+1; i++)
            {
                x[i-1] = d[i-1];
                w[i-1] = mu0*math.sqr(z[0,i-1]);
            }
        }


        /*************************************************************************
        Returns nodes/weights for Gauss-Legendre quadrature on [-1,1] with N
        nodes.

        INPUT PARAMETERS:
            N           -   number of nodes, >=1

        OUTPUT PARAMETERS:
            Info        -   error code:
                            * -4    an  error   was   detected   when  calculating
                                    weights/nodes.  N  is  too  large   to  obtain
                                    weights/nodes  with  high   enough   accuracy.
                                    Try  to   use   multiple   precision  version.
                            * -3    internal eigenproblem solver hasn't  converged
                            * -1    incorrect N was passed
                            * +1    OK
            X           -   array[0..N-1] - array of quadrature nodes,
                            in ascending order.
            W           -   array[0..N-1] - array of quadrature weights.


          -- ALGLIB --
             Copyright 12.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void gqgenerategausslegendre(int n,
            ref int info,
            ref double[] x,
            ref double[] w,
            alglib.xparams _params)
        {
            double[] alpha = new double[0];
            double[] beta = new double[0];
            int i = 0;

            info = 0;
            x = new double[0];
            w = new double[0];

            if( n<1 )
            {
                info = -1;
                return;
            }
            alpha = new double[n];
            beta = new double[n];
            for(i=0; i<=n-1; i++)
            {
                alpha[i] = 0;
            }
            beta[0] = 2;
            for(i=1; i<=n-1; i++)
            {
                beta[i] = 1/(4-1/math.sqr(i));
            }
            gqgeneraterec(alpha, beta, beta[0], n, ref info, ref x, ref w, _params);
            
            //
            // test basic properties to detect errors
            //
            if( info>0 )
            {
                if( (double)(x[0])<(double)(-1) || (double)(x[n-1])>(double)(1) )
                {
                    info = -4;
                }
                for(i=0; i<=n-2; i++)
                {
                    if( (double)(x[i])>=(double)(x[i+1]) )
                    {
                        info = -4;
                    }
                }
            }
        }


        /*************************************************************************
        Returns  nodes/weights  for  Gauss-Jacobi quadrature on [-1,1] with weight
        function W(x)=Power(1-x,Alpha)*Power(1+x,Beta).

        INPUT PARAMETERS:
            N           -   number of nodes, >=1
            Alpha       -   power-law coefficient, Alpha>-1
            Beta        -   power-law coefficient, Beta>-1

        OUTPUT PARAMETERS:
            Info        -   error code:
                            * -4    an  error  was   detected   when   calculating
                                    weights/nodes. Alpha or  Beta  are  too  close
                                    to -1 to obtain weights/nodes with high enough
                                    accuracy, or, may be, N is too large.  Try  to
                                    use multiple precision version.
                            * -3    internal eigenproblem solver hasn't converged
                            * -1    incorrect N/Alpha/Beta was passed
                            * +1    OK
            X           -   array[0..N-1] - array of quadrature nodes,
                            in ascending order.
            W           -   array[0..N-1] - array of quadrature weights.


          -- ALGLIB --
             Copyright 12.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void gqgenerategaussjacobi(int n,
            double alpha,
            double beta,
            ref int info,
            ref double[] x,
            ref double[] w,
            alglib.xparams _params)
        {
            double[] a = new double[0];
            double[] b = new double[0];
            double alpha2 = 0;
            double beta2 = 0;
            double apb = 0;
            double t = 0;
            int i = 0;
            double s = 0;

            info = 0;
            x = new double[0];
            w = new double[0];

            if( (n<1 || (double)(alpha)<=(double)(-1)) || (double)(beta)<=(double)(-1) )
            {
                info = -1;
                return;
            }
            a = new double[n];
            b = new double[n];
            apb = alpha+beta;
            a[0] = (beta-alpha)/(apb+2);
            t = (apb+1)*Math.Log(2)+gammafunc.lngamma(alpha+1, ref s, _params)+gammafunc.lngamma(beta+1, ref s, _params)-gammafunc.lngamma(apb+2, ref s, _params);
            if( (double)(t)>(double)(Math.Log(math.maxrealnumber)) )
            {
                info = -4;
                return;
            }
            b[0] = Math.Exp(t);
            if( n>1 )
            {
                alpha2 = math.sqr(alpha);
                beta2 = math.sqr(beta);
                a[1] = (beta2-alpha2)/((apb+2)*(apb+4));
                b[1] = 4*(alpha+1)*(beta+1)/((apb+3)*math.sqr(apb+2));
                for(i=2; i<=n-1; i++)
                {
                    a[i] = 0.25*(beta2-alpha2)/(i*i*(1+0.5*apb/i)*(1+0.5*(apb+2)/i));
                    b[i] = 0.25*(1+alpha/i)*(1+beta/i)*(1+apb/i)/((1+0.5*(apb+1)/i)*(1+0.5*(apb-1)/i)*math.sqr(1+0.5*apb/i));
                }
            }
            gqgeneraterec(a, b, b[0], n, ref info, ref x, ref w, _params);
            
            //
            // test basic properties to detect errors
            //
            if( info>0 )
            {
                if( (double)(x[0])<(double)(-1) || (double)(x[n-1])>(double)(1) )
                {
                    info = -4;
                }
                for(i=0; i<=n-2; i++)
                {
                    if( (double)(x[i])>=(double)(x[i+1]) )
                    {
                        info = -4;
                    }
                }
            }
        }


        /*************************************************************************
        Returns  nodes/weights  for  Gauss-Laguerre  quadrature  on  [0,+inf) with
        weight function W(x)=Power(x,Alpha)*Exp(-x)

        INPUT PARAMETERS:
            N           -   number of nodes, >=1
            Alpha       -   power-law coefficient, Alpha>-1

        OUTPUT PARAMETERS:
            Info        -   error code:
                            * -4    an  error  was   detected   when   calculating
                                    weights/nodes. Alpha is too  close  to  -1  to
                                    obtain weights/nodes with high enough accuracy
                                    or, may  be,  N  is  too  large.  Try  to  use
                                    multiple precision version.
                            * -3    internal eigenproblem solver hasn't converged
                            * -1    incorrect N/Alpha was passed
                            * +1    OK
            X           -   array[0..N-1] - array of quadrature nodes,
                            in ascending order.
            W           -   array[0..N-1] - array of quadrature weights.


          -- ALGLIB --
             Copyright 12.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void gqgenerategausslaguerre(int n,
            double alpha,
            ref int info,
            ref double[] x,
            ref double[] w,
            alglib.xparams _params)
        {
            double[] a = new double[0];
            double[] b = new double[0];
            double t = 0;
            int i = 0;
            double s = 0;

            info = 0;
            x = new double[0];
            w = new double[0];

            if( n<1 || (double)(alpha)<=(double)(-1) )
            {
                info = -1;
                return;
            }
            a = new double[n];
            b = new double[n];
            a[0] = alpha+1;
            t = gammafunc.lngamma(alpha+1, ref s, _params);
            if( (double)(t)>=(double)(Math.Log(math.maxrealnumber)) )
            {
                info = -4;
                return;
            }
            b[0] = Math.Exp(t);
            if( n>1 )
            {
                for(i=1; i<=n-1; i++)
                {
                    a[i] = 2*i+alpha+1;
                    b[i] = i*(i+alpha);
                }
            }
            gqgeneraterec(a, b, b[0], n, ref info, ref x, ref w, _params);
            
            //
            // test basic properties to detect errors
            //
            if( info>0 )
            {
                if( (double)(x[0])<(double)(0) )
                {
                    info = -4;
                }
                for(i=0; i<=n-2; i++)
                {
                    if( (double)(x[i])>=(double)(x[i+1]) )
                    {
                        info = -4;
                    }
                }
            }
        }


        /*************************************************************************
        Returns  nodes/weights  for  Gauss-Hermite  quadrature on (-inf,+inf) with
        weight function W(x)=Exp(-x*x)

        INPUT PARAMETERS:
            N           -   number of nodes, >=1

        OUTPUT PARAMETERS:
            Info        -   error code:
                            * -4    an  error  was   detected   when   calculating
                                    weights/nodes.  May be, N is too large. Try to
                                    use multiple precision version.
                            * -3    internal eigenproblem solver hasn't converged
                            * -1    incorrect N/Alpha was passed
                            * +1    OK
            X           -   array[0..N-1] - array of quadrature nodes,
                            in ascending order.
            W           -   array[0..N-1] - array of quadrature weights.


          -- ALGLIB --
             Copyright 12.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void gqgenerategausshermite(int n,
            ref int info,
            ref double[] x,
            ref double[] w,
            alglib.xparams _params)
        {
            double[] a = new double[0];
            double[] b = new double[0];
            int i = 0;

            info = 0;
            x = new double[0];
            w = new double[0];

            if( n<1 )
            {
                info = -1;
                return;
            }
            a = new double[n];
            b = new double[n];
            for(i=0; i<=n-1; i++)
            {
                a[i] = 0;
            }
            b[0] = Math.Sqrt(4*Math.Atan(1));
            if( n>1 )
            {
                for(i=1; i<=n-1; i++)
                {
                    b[i] = 0.5*i;
                }
            }
            gqgeneraterec(a, b, b[0], n, ref info, ref x, ref w, _params);
            
            //
            // test basic properties to detect errors
            //
            if( info>0 )
            {
                for(i=0; i<=n-2; i++)
                {
                    if( (double)(x[i])>=(double)(x[i+1]) )
                    {
                        info = -4;
                    }
                }
            }
        }


    }
    public class gkq
    {
        /*************************************************************************
        Computation of nodes and weights of a Gauss-Kronrod quadrature formula

        The algorithm generates the N-point Gauss-Kronrod quadrature formula  with
        weight  function  given  by  coefficients  alpha  and beta of a recurrence
        relation which generates a system of orthogonal polynomials:

            P-1(x)   =  0
            P0(x)    =  1
            Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)

        and zero moment Mu0

            Mu0 = integral(W(x)dx,a,b)


        INPUT PARAMETERS:
            Alpha       -   alpha coefficients, array[0..floor(3*K/2)].
            Beta        -   beta coefficients,  array[0..ceil(3*K/2)].
                            Beta[0] is not used and may be arbitrary.
                            Beta[I]>0.
            Mu0         -   zeroth moment of the weight function.
            N           -   number of nodes of the Gauss-Kronrod quadrature formula,
                            N >= 3,
                            N =  2*K+1.

        OUTPUT PARAMETERS:
            Info        -   error code:
                            * -5    no real and positive Gauss-Kronrod formula can
                                    be created for such a weight function  with  a
                                    given number of nodes.
                            * -4    N is too large, task may be ill  conditioned -
                                    x[i]=x[i+1] found.
                            * -3    internal eigenproblem solver hasn't converged
                            * -2    Beta[i]<=0
                            * -1    incorrect N was passed
                            * +1    OK
            X           -   array[0..N-1] - array of quadrature nodes,
                            in ascending order.
            WKronrod    -   array[0..N-1] - Kronrod weights
            WGauss      -   array[0..N-1] - Gauss weights (interleaved with zeros
                            corresponding to extended Kronrod nodes).

          -- ALGLIB --
             Copyright 08.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void gkqgeneraterec(double[] alpha,
            double[] beta,
            double mu0,
            int n,
            ref int info,
            ref double[] x,
            ref double[] wkronrod,
            ref double[] wgauss,
            alglib.xparams _params)
        {
            double[] ta = new double[0];
            int i = 0;
            int j = 0;
            double[] t = new double[0];
            double[] s = new double[0];
            int wlen = 0;
            int woffs = 0;
            double u = 0;
            int m = 0;
            int l = 0;
            int k = 0;
            double[] xgtmp = new double[0];
            double[] wgtmp = new double[0];
            int i_ = 0;

            alpha = (double[])alpha.Clone();
            beta = (double[])beta.Clone();
            info = 0;
            x = new double[0];
            wkronrod = new double[0];
            wgauss = new double[0];

            if( n%2!=1 || n<3 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=(int)Math.Ceiling((double)(3*(n/2))/(double)2); i++)
            {
                if( (double)(beta[i])<=(double)(0) )
                {
                    info = -2;
                    return;
                }
            }
            info = 1;
            
            //
            // from external conventions about N/Beta/Mu0 to internal
            //
            n = n/2;
            beta[0] = mu0;
            
            //
            // Calculate Gauss nodes/weights, save them for later processing
            //
            gq.gqgeneraterec(alpha, beta, mu0, n, ref info, ref xgtmp, ref wgtmp, _params);
            if( info<0 )
            {
                return;
            }
            
            //
            // Resize:
            // * A from 0..floor(3*n/2) to 0..2*n
            // * B from 0..ceil(3*n/2)  to 0..2*n
            //
            ta = new double[(int)Math.Floor((double)(3*n)/(double)2)+1];
            for(i_=0; i_<=(int)Math.Floor((double)(3*n)/(double)2);i_++)
            {
                ta[i_] = alpha[i_];
            }
            alpha = new double[2*n+1];
            for(i_=0; i_<=(int)Math.Floor((double)(3*n)/(double)2);i_++)
            {
                alpha[i_] = ta[i_];
            }
            for(i=(int)Math.Floor((double)(3*n)/(double)2)+1; i<=2*n; i++)
            {
                alpha[i] = 0;
            }
            ta = new double[(int)Math.Ceiling((double)(3*n)/(double)2)+1];
            for(i_=0; i_<=(int)Math.Ceiling((double)(3*n)/(double)2);i_++)
            {
                ta[i_] = beta[i_];
            }
            beta = new double[2*n+1];
            for(i_=0; i_<=(int)Math.Ceiling((double)(3*n)/(double)2);i_++)
            {
                beta[i_] = ta[i_];
            }
            for(i=(int)Math.Ceiling((double)(3*n)/(double)2)+1; i<=2*n; i++)
            {
                beta[i] = 0;
            }
            
            //
            // Initialize T, S
            //
            wlen = 2+n/2;
            t = new double[wlen];
            s = new double[wlen];
            ta = new double[wlen];
            woffs = 1;
            for(i=0; i<=wlen-1; i++)
            {
                t[i] = 0;
                s[i] = 0;
            }
            
            //
            // Algorithm from Dirk P. Laurie, "Calculation of Gauss-Kronrod quadrature rules", 1997.
            //
            t[woffs+0] = beta[n+1];
            for(m=0; m<=n-2; m++)
            {
                u = 0;
                for(k=(m+1)/2; k>=0; k--)
                {
                    l = m-k;
                    u = u+(alpha[k+n+1]-alpha[l])*t[woffs+k]+beta[k+n+1]*s[woffs+k-1]-beta[l]*s[woffs+k];
                    s[woffs+k] = u;
                }
                for(i_=0; i_<=wlen-1;i_++)
                {
                    ta[i_] = t[i_];
                }
                for(i_=0; i_<=wlen-1;i_++)
                {
                    t[i_] = s[i_];
                }
                for(i_=0; i_<=wlen-1;i_++)
                {
                    s[i_] = ta[i_];
                }
            }
            for(j=n/2; j>=0; j--)
            {
                s[woffs+j] = s[woffs+j-1];
            }
            for(m=n-1; m<=2*n-3; m++)
            {
                u = 0;
                for(k=m+1-n; k<=(m-1)/2; k++)
                {
                    l = m-k;
                    j = n-1-l;
                    u = u-(alpha[k+n+1]-alpha[l])*t[woffs+j]-beta[k+n+1]*s[woffs+j]+beta[l]*s[woffs+j+1];
                    s[woffs+j] = u;
                }
                if( m%2==0 )
                {
                    k = m/2;
                    alpha[k+n+1] = alpha[k]+(s[woffs+j]-beta[k+n+1]*s[woffs+j+1])/t[woffs+j+1];
                }
                else
                {
                    k = (m+1)/2;
                    beta[k+n+1] = s[woffs+j]/s[woffs+j+1];
                }
                for(i_=0; i_<=wlen-1;i_++)
                {
                    ta[i_] = t[i_];
                }
                for(i_=0; i_<=wlen-1;i_++)
                {
                    t[i_] = s[i_];
                }
                for(i_=0; i_<=wlen-1;i_++)
                {
                    s[i_] = ta[i_];
                }
            }
            alpha[2*n] = alpha[n-1]-beta[2*n]*s[woffs+0]/t[woffs+0];
            
            //
            // calculation of Kronrod nodes and weights, unpacking of Gauss weights
            //
            gq.gqgeneraterec(alpha, beta, mu0, 2*n+1, ref info, ref x, ref wkronrod, _params);
            if( info==-2 )
            {
                info = -5;
            }
            if( info<0 )
            {
                return;
            }
            for(i=0; i<=2*n-1; i++)
            {
                if( (double)(x[i])>=(double)(x[i+1]) )
                {
                    info = -4;
                }
            }
            if( info<0 )
            {
                return;
            }
            wgauss = new double[2*n+1];
            for(i=0; i<=2*n; i++)
            {
                wgauss[i] = 0;
            }
            for(i=0; i<=n-1; i++)
            {
                wgauss[2*i+1] = wgtmp[i];
            }
        }


        /*************************************************************************
        Returns   Gauss   and   Gauss-Kronrod   nodes/weights  for  Gauss-Legendre
        quadrature with N points.

        GKQLegendreCalc (calculation) or  GKQLegendreTbl  (precomputed  table)  is
        used depending on machine precision and number of nodes.

        INPUT PARAMETERS:
            N           -   number of Kronrod nodes, must be odd number, >=3.

        OUTPUT PARAMETERS:
            Info        -   error code:
                            * -4    an  error   was   detected   when  calculating
                                    weights/nodes.  N  is  too  large   to  obtain
                                    weights/nodes  with  high   enough   accuracy.
                                    Try  to   use   multiple   precision  version.
                            * -3    internal eigenproblem solver hasn't converged
                            * -1    incorrect N was passed
                            * +1    OK
            X           -   array[0..N-1] - array of quadrature nodes, ordered in
                            ascending order.
            WKronrod    -   array[0..N-1] - Kronrod weights
            WGauss      -   array[0..N-1] - Gauss weights (interleaved with zeros
                            corresponding to extended Kronrod nodes).


          -- ALGLIB --
             Copyright 12.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void gkqgenerategausslegendre(int n,
            ref int info,
            ref double[] x,
            ref double[] wkronrod,
            ref double[] wgauss,
            alglib.xparams _params)
        {
            double eps = 0;

            info = 0;
            x = new double[0];
            wkronrod = new double[0];
            wgauss = new double[0];

            if( (double)(math.machineepsilon)>(double)(1.0E-32) && (((((n==15 || n==21) || n==31) || n==41) || n==51) || n==61) )
            {
                info = 1;
                gkqlegendretbl(n, ref x, ref wkronrod, ref wgauss, ref eps, _params);
            }
            else
            {
                gkqlegendrecalc(n, ref info, ref x, ref wkronrod, ref wgauss, _params);
            }
        }


        /*************************************************************************
        Returns   Gauss   and   Gauss-Kronrod   nodes/weights   for   Gauss-Jacobi
        quadrature on [-1,1] with weight function

            W(x)=Power(1-x,Alpha)*Power(1+x,Beta).

        INPUT PARAMETERS:
            N           -   number of Kronrod nodes, must be odd number, >=3.
            Alpha       -   power-law coefficient, Alpha>-1
            Beta        -   power-law coefficient, Beta>-1

        OUTPUT PARAMETERS:
            Info        -   error code:
                            * -5    no real and positive Gauss-Kronrod formula can
                                    be created for such a weight function  with  a
                                    given number of nodes.
                            * -4    an  error  was   detected   when   calculating
                                    weights/nodes. Alpha or  Beta  are  too  close
                                    to -1 to obtain weights/nodes with high enough
                                    accuracy, or, may be, N is too large.  Try  to
                                    use multiple precision version.
                            * -3    internal eigenproblem solver hasn't converged
                            * -1    incorrect N was passed
                            * +1    OK
                            * +2    OK, but quadrature rule have exterior  nodes,
                                    x[0]<-1 or x[n-1]>+1
            X           -   array[0..N-1] - array of quadrature nodes, ordered in
                            ascending order.
            WKronrod    -   array[0..N-1] - Kronrod weights
            WGauss      -   array[0..N-1] - Gauss weights (interleaved with zeros
                            corresponding to extended Kronrod nodes).


          -- ALGLIB --
             Copyright 12.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void gkqgenerategaussjacobi(int n,
            double alpha,
            double beta,
            ref int info,
            ref double[] x,
            ref double[] wkronrod,
            ref double[] wgauss,
            alglib.xparams _params)
        {
            int clen = 0;
            double[] a = new double[0];
            double[] b = new double[0];
            double alpha2 = 0;
            double beta2 = 0;
            double apb = 0;
            double t = 0;
            int i = 0;
            double s = 0;

            info = 0;
            x = new double[0];
            wkronrod = new double[0];
            wgauss = new double[0];

            if( n%2!=1 || n<3 )
            {
                info = -1;
                return;
            }
            if( (double)(alpha)<=(double)(-1) || (double)(beta)<=(double)(-1) )
            {
                info = -1;
                return;
            }
            clen = (int)Math.Ceiling((double)(3*(n/2))/(double)2)+1;
            a = new double[clen];
            b = new double[clen];
            for(i=0; i<=clen-1; i++)
            {
                a[i] = 0;
            }
            apb = alpha+beta;
            a[0] = (beta-alpha)/(apb+2);
            t = (apb+1)*Math.Log(2)+gammafunc.lngamma(alpha+1, ref s, _params)+gammafunc.lngamma(beta+1, ref s, _params)-gammafunc.lngamma(apb+2, ref s, _params);
            if( (double)(t)>(double)(Math.Log(math.maxrealnumber)) )
            {
                info = -4;
                return;
            }
            b[0] = Math.Exp(t);
            if( clen>1 )
            {
                alpha2 = math.sqr(alpha);
                beta2 = math.sqr(beta);
                a[1] = (beta2-alpha2)/((apb+2)*(apb+4));
                b[1] = 4*(alpha+1)*(beta+1)/((apb+3)*math.sqr(apb+2));
                for(i=2; i<=clen-1; i++)
                {
                    a[i] = 0.25*(beta2-alpha2)/(i*i*(1+0.5*apb/i)*(1+0.5*(apb+2)/i));
                    b[i] = 0.25*(1+alpha/i)*(1+beta/i)*(1+apb/i)/((1+0.5*(apb+1)/i)*(1+0.5*(apb-1)/i)*math.sqr(1+0.5*apb/i));
                }
            }
            gkqgeneraterec(a, b, b[0], n, ref info, ref x, ref wkronrod, ref wgauss, _params);
            
            //
            // test basic properties to detect errors
            //
            if( info>0 )
            {
                if( (double)(x[0])<(double)(-1) || (double)(x[n-1])>(double)(1) )
                {
                    info = 2;
                }
                for(i=0; i<=n-2; i++)
                {
                    if( (double)(x[i])>=(double)(x[i+1]) )
                    {
                        info = -4;
                    }
                }
            }
        }


        /*************************************************************************
        Returns Gauss and Gauss-Kronrod nodes for quadrature with N points.

        Reduction to tridiagonal eigenproblem is used.

        INPUT PARAMETERS:
            N           -   number of Kronrod nodes, must be odd number, >=3.

        OUTPUT PARAMETERS:
            Info        -   error code:
                            * -4    an  error   was   detected   when  calculating
                                    weights/nodes.  N  is  too  large   to  obtain
                                    weights/nodes  with  high   enough   accuracy.
                                    Try  to   use   multiple   precision  version.
                            * -3    internal eigenproblem solver hasn't converged
                            * -1    incorrect N was passed
                            * +1    OK
            X           -   array[0..N-1] - array of quadrature nodes, ordered in
                            ascending order.
            WKronrod    -   array[0..N-1] - Kronrod weights
            WGauss      -   array[0..N-1] - Gauss weights (interleaved with zeros
                            corresponding to extended Kronrod nodes).

          -- ALGLIB --
             Copyright 12.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void gkqlegendrecalc(int n,
            ref int info,
            ref double[] x,
            ref double[] wkronrod,
            ref double[] wgauss,
            alglib.xparams _params)
        {
            double[] alpha = new double[0];
            double[] beta = new double[0];
            int alen = 0;
            int blen = 0;
            double mu0 = 0;
            int k = 0;
            int i = 0;

            info = 0;
            x = new double[0];
            wkronrod = new double[0];
            wgauss = new double[0];

            if( n%2!=1 || n<3 )
            {
                info = -1;
                return;
            }
            mu0 = 2;
            alen = (int)Math.Floor((double)(3*(n/2))/(double)2)+1;
            blen = (int)Math.Ceiling((double)(3*(n/2))/(double)2)+1;
            alpha = new double[alen];
            beta = new double[blen];
            for(k=0; k<=alen-1; k++)
            {
                alpha[k] = 0;
            }
            beta[0] = 2;
            for(k=1; k<=blen-1; k++)
            {
                beta[k] = 1/(4-1/math.sqr(k));
            }
            gkqgeneraterec(alpha, beta, mu0, n, ref info, ref x, ref wkronrod, ref wgauss, _params);
            
            //
            // test basic properties to detect errors
            //
            if( info>0 )
            {
                if( (double)(x[0])<(double)(-1) || (double)(x[n-1])>(double)(1) )
                {
                    info = -4;
                }
                for(i=0; i<=n-2; i++)
                {
                    if( (double)(x[i])>=(double)(x[i+1]) )
                    {
                        info = -4;
                    }
                }
            }
        }


        /*************************************************************************
        Returns Gauss and Gauss-Kronrod nodes for quadrature with N  points  using
        pre-calculated table. Nodes/weights were  computed  with  accuracy  up  to
        1.0E-32 (if MPFR version of ALGLIB is used). In standard double  precision
        accuracy reduces to something about 2.0E-16 (depending  on your compiler's
        handling of long floating point constants).

        INPUT PARAMETERS:
            N           -   number of Kronrod nodes.
                            N can be 15, 21, 31, 41, 51, 61.

        OUTPUT PARAMETERS:
            X           -   array[0..N-1] - array of quadrature nodes, ordered in
                            ascending order.
            WKronrod    -   array[0..N-1] - Kronrod weights
            WGauss      -   array[0..N-1] - Gauss weights (interleaved with zeros
                            corresponding to extended Kronrod nodes).


          -- ALGLIB --
             Copyright 12.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void gkqlegendretbl(int n,
            ref double[] x,
            ref double[] wkronrod,
            ref double[] wgauss,
            ref double eps,
            alglib.xparams _params)
        {
            int i = 0;
            int ng = 0;
            int[] p1 = new int[0];
            int[] p2 = new int[0];
            double tmp = 0;

            x = new double[0];
            wkronrod = new double[0];
            wgauss = new double[0];
            eps = 0;

            
            //
            // these initializers are not really necessary,
            // but without them compiler complains about uninitialized locals
            //
            ng = 0;
            
            //
            // Process
            //
            alglib.ap.assert(((((n==15 || n==21) || n==31) || n==41) || n==51) || n==61, "GKQNodesTbl: incorrect N!");
            x = new double[n];
            wkronrod = new double[n];
            wgauss = new double[n];
            for(i=0; i<=n-1; i++)
            {
                x[i] = 0;
                wkronrod[i] = 0;
                wgauss[i] = 0;
            }
            eps = Math.Max(math.machineepsilon, 1.0E-32);
            if( n==15 )
            {
                ng = 4;
                wgauss[0] = 0.129484966168869693270611432679082;
                wgauss[1] = 0.279705391489276667901467771423780;
                wgauss[2] = 0.381830050505118944950369775488975;
                wgauss[3] = 0.417959183673469387755102040816327;
                x[0] = 0.991455371120812639206854697526329;
                x[1] = 0.949107912342758524526189684047851;
                x[2] = 0.864864423359769072789712788640926;
                x[3] = 0.741531185599394439863864773280788;
                x[4] = 0.586087235467691130294144838258730;
                x[5] = 0.405845151377397166906606412076961;
                x[6] = 0.207784955007898467600689403773245;
                x[7] = 0.000000000000000000000000000000000;
                wkronrod[0] = 0.022935322010529224963732008058970;
                wkronrod[1] = 0.063092092629978553290700663189204;
                wkronrod[2] = 0.104790010322250183839876322541518;
                wkronrod[3] = 0.140653259715525918745189590510238;
                wkronrod[4] = 0.169004726639267902826583426598550;
                wkronrod[5] = 0.190350578064785409913256402421014;
                wkronrod[6] = 0.204432940075298892414161999234649;
                wkronrod[7] = 0.209482141084727828012999174891714;
            }
            if( n==21 )
            {
                ng = 5;
                wgauss[0] = 0.066671344308688137593568809893332;
                wgauss[1] = 0.149451349150580593145776339657697;
                wgauss[2] = 0.219086362515982043995534934228163;
                wgauss[3] = 0.269266719309996355091226921569469;
                wgauss[4] = 0.295524224714752870173892994651338;
                x[0] = 0.995657163025808080735527280689003;
                x[1] = 0.973906528517171720077964012084452;
                x[2] = 0.930157491355708226001207180059508;
                x[3] = 0.865063366688984510732096688423493;
                x[4] = 0.780817726586416897063717578345042;
                x[5] = 0.679409568299024406234327365114874;
                x[6] = 0.562757134668604683339000099272694;
                x[7] = 0.433395394129247190799265943165784;
                x[8] = 0.294392862701460198131126603103866;
                x[9] = 0.148874338981631210884826001129720;
                x[10] = 0.000000000000000000000000000000000;
                wkronrod[0] = 0.011694638867371874278064396062192;
                wkronrod[1] = 0.032558162307964727478818972459390;
                wkronrod[2] = 0.054755896574351996031381300244580;
                wkronrod[3] = 0.075039674810919952767043140916190;
                wkronrod[4] = 0.093125454583697605535065465083366;
                wkronrod[5] = 0.109387158802297641899210590325805;
                wkronrod[6] = 0.123491976262065851077958109831074;
                wkronrod[7] = 0.134709217311473325928054001771707;
                wkronrod[8] = 0.142775938577060080797094273138717;
                wkronrod[9] = 0.147739104901338491374841515972068;
                wkronrod[10] = 0.149445554002916905664936468389821;
            }
            if( n==31 )
            {
                ng = 8;
                wgauss[0] = 0.030753241996117268354628393577204;
                wgauss[1] = 0.070366047488108124709267416450667;
                wgauss[2] = 0.107159220467171935011869546685869;
                wgauss[3] = 0.139570677926154314447804794511028;
                wgauss[4] = 0.166269205816993933553200860481209;
                wgauss[5] = 0.186161000015562211026800561866423;
                wgauss[6] = 0.198431485327111576456118326443839;
                wgauss[7] = 0.202578241925561272880620199967519;
                x[0] = 0.998002298693397060285172840152271;
                x[1] = 0.987992518020485428489565718586613;
                x[2] = 0.967739075679139134257347978784337;
                x[3] = 0.937273392400705904307758947710209;
                x[4] = 0.897264532344081900882509656454496;
                x[5] = 0.848206583410427216200648320774217;
                x[6] = 0.790418501442465932967649294817947;
                x[7] = 0.724417731360170047416186054613938;
                x[8] = 0.650996741297416970533735895313275;
                x[9] = 0.570972172608538847537226737253911;
                x[10] = 0.485081863640239680693655740232351;
                x[11] = 0.394151347077563369897207370981045;
                x[12] = 0.299180007153168812166780024266389;
                x[13] = 0.201194093997434522300628303394596;
                x[14] = 0.101142066918717499027074231447392;
                x[15] = 0.000000000000000000000000000000000;
                wkronrod[0] = 0.005377479872923348987792051430128;
                wkronrod[1] = 0.015007947329316122538374763075807;
                wkronrod[2] = 0.025460847326715320186874001019653;
                wkronrod[3] = 0.035346360791375846222037948478360;
                wkronrod[4] = 0.044589751324764876608227299373280;
                wkronrod[5] = 0.053481524690928087265343147239430;
                wkronrod[6] = 0.062009567800670640285139230960803;
                wkronrod[7] = 0.069854121318728258709520077099147;
                wkronrod[8] = 0.076849680757720378894432777482659;
                wkronrod[9] = 0.083080502823133021038289247286104;
                wkronrod[10] = 0.088564443056211770647275443693774;
                wkronrod[11] = 0.093126598170825321225486872747346;
                wkronrod[12] = 0.096642726983623678505179907627589;
                wkronrod[13] = 0.099173598721791959332393173484603;
                wkronrod[14] = 0.100769845523875595044946662617570;
                wkronrod[15] = 0.101330007014791549017374792767493;
            }
            if( n==41 )
            {
                ng = 10;
                wgauss[0] = 0.017614007139152118311861962351853;
                wgauss[1] = 0.040601429800386941331039952274932;
                wgauss[2] = 0.062672048334109063569506535187042;
                wgauss[3] = 0.083276741576704748724758143222046;
                wgauss[4] = 0.101930119817240435036750135480350;
                wgauss[5] = 0.118194531961518417312377377711382;
                wgauss[6] = 0.131688638449176626898494499748163;
                wgauss[7] = 0.142096109318382051329298325067165;
                wgauss[8] = 0.149172986472603746787828737001969;
                wgauss[9] = 0.152753387130725850698084331955098;
                x[0] = 0.998859031588277663838315576545863;
                x[1] = 0.993128599185094924786122388471320;
                x[2] = 0.981507877450250259193342994720217;
                x[3] = 0.963971927277913791267666131197277;
                x[4] = 0.940822633831754753519982722212443;
                x[5] = 0.912234428251325905867752441203298;
                x[6] = 0.878276811252281976077442995113078;
                x[7] = 0.839116971822218823394529061701521;
                x[8] = 0.795041428837551198350638833272788;
                x[9] = 0.746331906460150792614305070355642;
                x[10] = 0.693237656334751384805490711845932;
                x[11] = 0.636053680726515025452836696226286;
                x[12] = 0.575140446819710315342946036586425;
                x[13] = 0.510867001950827098004364050955251;
                x[14] = 0.443593175238725103199992213492640;
                x[15] = 0.373706088715419560672548177024927;
                x[16] = 0.301627868114913004320555356858592;
                x[17] = 0.227785851141645078080496195368575;
                x[18] = 0.152605465240922675505220241022678;
                x[19] = 0.076526521133497333754640409398838;
                x[20] = 0.000000000000000000000000000000000;
                wkronrod[0] = 0.003073583718520531501218293246031;
                wkronrod[1] = 0.008600269855642942198661787950102;
                wkronrod[2] = 0.014626169256971252983787960308868;
                wkronrod[3] = 0.020388373461266523598010231432755;
                wkronrod[4] = 0.025882133604951158834505067096153;
                wkronrod[5] = 0.031287306777032798958543119323801;
                wkronrod[6] = 0.036600169758200798030557240707211;
                wkronrod[7] = 0.041668873327973686263788305936895;
                wkronrod[8] = 0.046434821867497674720231880926108;
                wkronrod[9] = 0.050944573923728691932707670050345;
                wkronrod[10] = 0.055195105348285994744832372419777;
                wkronrod[11] = 0.059111400880639572374967220648594;
                wkronrod[12] = 0.062653237554781168025870122174255;
                wkronrod[13] = 0.065834597133618422111563556969398;
                wkronrod[14] = 0.068648672928521619345623411885368;
                wkronrod[15] = 0.071054423553444068305790361723210;
                wkronrod[16] = 0.073030690332786667495189417658913;
                wkronrod[17] = 0.074582875400499188986581418362488;
                wkronrod[18] = 0.075704497684556674659542775376617;
                wkronrod[19] = 0.076377867672080736705502835038061;
                wkronrod[20] = 0.076600711917999656445049901530102;
            }
            if( n==51 )
            {
                ng = 13;
                wgauss[0] = 0.011393798501026287947902964113235;
                wgauss[1] = 0.026354986615032137261901815295299;
                wgauss[2] = 0.040939156701306312655623487711646;
                wgauss[3] = 0.054904695975835191925936891540473;
                wgauss[4] = 0.068038333812356917207187185656708;
                wgauss[5] = 0.080140700335001018013234959669111;
                wgauss[6] = 0.091028261982963649811497220702892;
                wgauss[7] = 0.100535949067050644202206890392686;
                wgauss[8] = 0.108519624474263653116093957050117;
                wgauss[9] = 0.114858259145711648339325545869556;
                wgauss[10] = 0.119455763535784772228178126512901;
                wgauss[11] = 0.122242442990310041688959518945852;
                wgauss[12] = 0.123176053726715451203902873079050;
                x[0] = 0.999262104992609834193457486540341;
                x[1] = 0.995556969790498097908784946893902;
                x[2] = 0.988035794534077247637331014577406;
                x[3] = 0.976663921459517511498315386479594;
                x[4] = 0.961614986425842512418130033660167;
                x[5] = 0.942974571228974339414011169658471;
                x[6] = 0.920747115281701561746346084546331;
                x[7] = 0.894991997878275368851042006782805;
                x[8] = 0.865847065293275595448996969588340;
                x[9] = 0.833442628760834001421021108693570;
                x[10] = 0.797873797998500059410410904994307;
                x[11] = 0.759259263037357630577282865204361;
                x[12] = 0.717766406813084388186654079773298;
                x[13] = 0.673566368473468364485120633247622;
                x[14] = 0.626810099010317412788122681624518;
                x[15] = 0.577662930241222967723689841612654;
                x[16] = 0.526325284334719182599623778158010;
                x[17] = 0.473002731445714960522182115009192;
                x[18] = 0.417885382193037748851814394594572;
                x[19] = 0.361172305809387837735821730127641;
                x[20] = 0.303089538931107830167478909980339;
                x[21] = 0.243866883720988432045190362797452;
                x[22] = 0.183718939421048892015969888759528;
                x[23] = 0.122864692610710396387359818808037;
                x[24] = 0.061544483005685078886546392366797;
                x[25] = 0.000000000000000000000000000000000;
                wkronrod[0] = 0.001987383892330315926507851882843;
                wkronrod[1] = 0.005561932135356713758040236901066;
                wkronrod[2] = 0.009473973386174151607207710523655;
                wkronrod[3] = 0.013236229195571674813656405846976;
                wkronrod[4] = 0.016847817709128298231516667536336;
                wkronrod[5] = 0.020435371145882835456568292235939;
                wkronrod[6] = 0.024009945606953216220092489164881;
                wkronrod[7] = 0.027475317587851737802948455517811;
                wkronrod[8] = 0.030792300167387488891109020215229;
                wkronrod[9] = 0.034002130274329337836748795229551;
                wkronrod[10] = 0.037116271483415543560330625367620;
                wkronrod[11] = 0.040083825504032382074839284467076;
                wkronrod[12] = 0.042872845020170049476895792439495;
                wkronrod[13] = 0.045502913049921788909870584752660;
                wkronrod[14] = 0.047982537138836713906392255756915;
                wkronrod[15] = 0.050277679080715671963325259433440;
                wkronrod[16] = 0.052362885806407475864366712137873;
                wkronrod[17] = 0.054251129888545490144543370459876;
                wkronrod[18] = 0.055950811220412317308240686382747;
                wkronrod[19] = 0.057437116361567832853582693939506;
                wkronrod[20] = 0.058689680022394207961974175856788;
                wkronrod[21] = 0.059720340324174059979099291932562;
                wkronrod[22] = 0.060539455376045862945360267517565;
                wkronrod[23] = 0.061128509717053048305859030416293;
                wkronrod[24] = 0.061471189871425316661544131965264;
                wkronrod[25] = 0.061580818067832935078759824240055;
            }
            if( n==61 )
            {
                ng = 15;
                wgauss[0] = 0.007968192496166605615465883474674;
                wgauss[1] = 0.018466468311090959142302131912047;
                wgauss[2] = 0.028784707883323369349719179611292;
                wgauss[3] = 0.038799192569627049596801936446348;
                wgauss[4] = 0.048402672830594052902938140422808;
                wgauss[5] = 0.057493156217619066481721689402056;
                wgauss[6] = 0.065974229882180495128128515115962;
                wgauss[7] = 0.073755974737705206268243850022191;
                wgauss[8] = 0.080755895229420215354694938460530;
                wgauss[9] = 0.086899787201082979802387530715126;
                wgauss[10] = 0.092122522237786128717632707087619;
                wgauss[11] = 0.096368737174644259639468626351810;
                wgauss[12] = 0.099593420586795267062780282103569;
                wgauss[13] = 0.101762389748405504596428952168554;
                wgauss[14] = 0.102852652893558840341285636705415;
                x[0] = 0.999484410050490637571325895705811;
                x[1] = 0.996893484074649540271630050918695;
                x[2] = 0.991630996870404594858628366109486;
                x[3] = 0.983668123279747209970032581605663;
                x[4] = 0.973116322501126268374693868423707;
                x[5] = 0.960021864968307512216871025581798;
                x[6] = 0.944374444748559979415831324037439;
                x[7] = 0.926200047429274325879324277080474;
                x[8] = 0.905573307699907798546522558925958;
                x[9] = 0.882560535792052681543116462530226;
                x[10] = 0.857205233546061098958658510658944;
                x[11] = 0.829565762382768397442898119732502;
                x[12] = 0.799727835821839083013668942322683;
                x[13] = 0.767777432104826194917977340974503;
                x[14] = 0.733790062453226804726171131369528;
                x[15] = 0.697850494793315796932292388026640;
                x[16] = 0.660061064126626961370053668149271;
                x[17] = 0.620526182989242861140477556431189;
                x[18] = 0.579345235826361691756024932172540;
                x[19] = 0.536624148142019899264169793311073;
                x[20] = 0.492480467861778574993693061207709;
                x[21] = 0.447033769538089176780609900322854;
                x[22] = 0.400401254830394392535476211542661;
                x[23] = 0.352704725530878113471037207089374;
                x[24] = 0.304073202273625077372677107199257;
                x[25] = 0.254636926167889846439805129817805;
                x[26] = 0.204525116682309891438957671002025;
                x[27] = 0.153869913608583546963794672743256;
                x[28] = 0.102806937966737030147096751318001;
                x[29] = 0.051471842555317695833025213166723;
                x[30] = 0.000000000000000000000000000000000;
                wkronrod[0] = 0.001389013698677007624551591226760;
                wkronrod[1] = 0.003890461127099884051267201844516;
                wkronrod[2] = 0.006630703915931292173319826369750;
                wkronrod[3] = 0.009273279659517763428441146892024;
                wkronrod[4] = 0.011823015253496341742232898853251;
                wkronrod[5] = 0.014369729507045804812451432443580;
                wkronrod[6] = 0.016920889189053272627572289420322;
                wkronrod[7] = 0.019414141193942381173408951050128;
                wkronrod[8] = 0.021828035821609192297167485738339;
                wkronrod[9] = 0.024191162078080601365686370725232;
                wkronrod[10] = 0.026509954882333101610601709335075;
                wkronrod[11] = 0.028754048765041292843978785354334;
                wkronrod[12] = 0.030907257562387762472884252943092;
                wkronrod[13] = 0.032981447057483726031814191016854;
                wkronrod[14] = 0.034979338028060024137499670731468;
                wkronrod[15] = 0.036882364651821229223911065617136;
                wkronrod[16] = 0.038678945624727592950348651532281;
                wkronrod[17] = 0.040374538951535959111995279752468;
                wkronrod[18] = 0.041969810215164246147147541285970;
                wkronrod[19] = 0.043452539701356069316831728117073;
                wkronrod[20] = 0.044814800133162663192355551616723;
                wkronrod[21] = 0.046059238271006988116271735559374;
                wkronrod[22] = 0.047185546569299153945261478181099;
                wkronrod[23] = 0.048185861757087129140779492298305;
                wkronrod[24] = 0.049055434555029778887528165367238;
                wkronrod[25] = 0.049795683427074206357811569379942;
                wkronrod[26] = 0.050405921402782346840893085653585;
                wkronrod[27] = 0.050881795898749606492297473049805;
                wkronrod[28] = 0.051221547849258772170656282604944;
                wkronrod[29] = 0.051426128537459025933862879215781;
                wkronrod[30] = 0.051494729429451567558340433647099;
            }
            
            //
            // copy nodes
            //
            for(i=n-1; i>=n/2; i--)
            {
                x[i] = -x[n-1-i];
            }
            
            //
            // copy Kronrod weights
            //
            for(i=n-1; i>=n/2; i--)
            {
                wkronrod[i] = wkronrod[n-1-i];
            }
            
            //
            // copy Gauss weights
            //
            for(i=ng-1; i>=0; i--)
            {
                wgauss[n-2-2*i] = wgauss[i];
                wgauss[1+2*i] = wgauss[i];
            }
            for(i=0; i<=n/2; i++)
            {
                wgauss[2*i] = 0;
            }
            
            //
            // reorder
            //
            tsort.tagsort(ref x, n, ref p1, ref p2, _params);
            for(i=0; i<=n-1; i++)
            {
                tmp = wkronrod[i];
                wkronrod[i] = wkronrod[p2[i]];
                wkronrod[p2[i]] = tmp;
                tmp = wgauss[i];
                wgauss[i] = wgauss[p2[i]];
                wgauss[p2[i]] = tmp;
            }
        }


    }
    public class autogk
    {
        /*************************************************************************
        Integration report:
        * TerminationType = completetion code:
            * -5    non-convergence of Gauss-Kronrod nodes
                    calculation subroutine.
            * -1    incorrect parameters were specified
            *  1    OK
        * Rep.NFEV countains number of function calculations
        * Rep.NIntervals contains number of intervals [a,b]
          was partitioned into.
        *************************************************************************/
        public class autogkreport : apobject
        {
            public int terminationtype;
            public int nfev;
            public int nintervals;
            public autogkreport()
            {
                init();
            }
            public override void init()
            {
            }
            public override alglib.apobject make_copy()
            {
                autogkreport _result = new autogkreport();
                _result.terminationtype = terminationtype;
                _result.nfev = nfev;
                _result.nintervals = nintervals;
                return _result;
            }
        };


        public class autogkinternalstate : apobject
        {
            public double a;
            public double b;
            public double eps;
            public double xwidth;
            public double x;
            public double f;
            public int info;
            public double r;
            public double[,] heap;
            public int heapsize;
            public int heapwidth;
            public int heapused;
            public double sumerr;
            public double sumabs;
            public double[] qn;
            public double[] wg;
            public double[] wk;
            public double[] wr;
            public int n;
            public rcommstate rstate;
            public autogkinternalstate()
            {
                init();
            }
            public override void init()
            {
                heap = new double[0,0];
                qn = new double[0];
                wg = new double[0];
                wk = new double[0];
                wr = new double[0];
                rstate = new rcommstate();
            }
            public override alglib.apobject make_copy()
            {
                autogkinternalstate _result = new autogkinternalstate();
                _result.a = a;
                _result.b = b;
                _result.eps = eps;
                _result.xwidth = xwidth;
                _result.x = x;
                _result.f = f;
                _result.info = info;
                _result.r = r;
                _result.heap = (double[,])heap.Clone();
                _result.heapsize = heapsize;
                _result.heapwidth = heapwidth;
                _result.heapused = heapused;
                _result.sumerr = sumerr;
                _result.sumabs = sumabs;
                _result.qn = (double[])qn.Clone();
                _result.wg = (double[])wg.Clone();
                _result.wk = (double[])wk.Clone();
                _result.wr = (double[])wr.Clone();
                _result.n = n;
                _result.rstate = (rcommstate)rstate.make_copy();
                return _result;
            }
        };


        /*************************************************************************
        This structure stores state of the integration algorithm.

        Although this class has public fields,  they are not intended for external
        use. You should use ALGLIB functions to work with this class:
        * autogksmooth()/AutoGKSmoothW()/... to create objects
        * autogkintegrate() to begin integration
        * autogkresults() to get results
        *************************************************************************/
        public class autogkstate : apobject
        {
            public double a;
            public double b;
            public double alpha;
            public double beta;
            public double xwidth;
            public double x;
            public double xminusa;
            public double bminusx;
            public bool needf;
            public double f;
            public int wrappermode;
            public autogkinternalstate internalstate;
            public rcommstate rstate;
            public double v;
            public int terminationtype;
            public int nfev;
            public int nintervals;
            public autogkstate()
            {
                init();
            }
            public override void init()
            {
                internalstate = new autogkinternalstate();
                rstate = new rcommstate();
            }
            public override alglib.apobject make_copy()
            {
                autogkstate _result = new autogkstate();
                _result.a = a;
                _result.b = b;
                _result.alpha = alpha;
                _result.beta = beta;
                _result.xwidth = xwidth;
                _result.x = x;
                _result.xminusa = xminusa;
                _result.bminusx = bminusx;
                _result.needf = needf;
                _result.f = f;
                _result.wrappermode = wrappermode;
                _result.internalstate = (autogkinternalstate)internalstate.make_copy();
                _result.rstate = (rcommstate)rstate.make_copy();
                _result.v = v;
                _result.terminationtype = terminationtype;
                _result.nfev = nfev;
                _result.nintervals = nintervals;
                return _result;
            }
        };




        public const int maxsubintervals = 10000;


        /*************************************************************************
        Integration of a smooth function F(x) on a finite interval [a,b].

        Fast-convergent algorithm based on a Gauss-Kronrod formula is used. Result
        is calculated with accuracy close to the machine precision.

        Algorithm works well only with smooth integrands.  It  may  be  used  with
        continuous non-smooth integrands, but with  less  performance.

        It should never be used with integrands which have integrable singularities
        at lower or upper limits - algorithm may crash. Use AutoGKSingular in such
        cases.

        INPUT PARAMETERS:
            A, B    -   interval boundaries (A<B, A=B or A>B)
            
        OUTPUT PARAMETERS
            State   -   structure which stores algorithm state

        SEE ALSO
            AutoGKSmoothW, AutoGKSingular, AutoGKResults.
            

          -- ALGLIB --
             Copyright 06.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void autogksmooth(double a,
            double b,
            autogkstate state,
            alglib.xparams _params)
        {
            alglib.ap.assert(math.isfinite(a), "AutoGKSmooth: A is not finite!");
            alglib.ap.assert(math.isfinite(b), "AutoGKSmooth: B is not finite!");
            autogksmoothw(a, b, 0.0, state, _params);
        }


        /*************************************************************************
        Integration of a smooth function F(x) on a finite interval [a,b].

        This subroutine is same as AutoGKSmooth(), but it guarantees that interval
        [a,b] is partitioned into subintervals which have width at most XWidth.

        Subroutine  can  be  used  when  integrating nearly-constant function with
        narrow "bumps" (about XWidth wide). If "bumps" are too narrow, AutoGKSmooth
        subroutine can overlook them.

        INPUT PARAMETERS:
            A, B    -   interval boundaries (A<B, A=B or A>B)

        OUTPUT PARAMETERS
            State   -   structure which stores algorithm state

        SEE ALSO
            AutoGKSmooth, AutoGKSingular, AutoGKResults.


          -- ALGLIB --
             Copyright 06.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void autogksmoothw(double a,
            double b,
            double xwidth,
            autogkstate state,
            alglib.xparams _params)
        {
            alglib.ap.assert(math.isfinite(a), "AutoGKSmoothW: A is not finite!");
            alglib.ap.assert(math.isfinite(b), "AutoGKSmoothW: B is not finite!");
            alglib.ap.assert(math.isfinite(xwidth), "AutoGKSmoothW: XWidth is not finite!");
            state.wrappermode = 0;
            state.a = a;
            state.b = b;
            state.xwidth = xwidth;
            state.needf = false;
            state.rstate.ra = new double[10+1];
            state.rstate.stage = -1;
        }


        /*************************************************************************
        Integration on a finite interval [A,B].
        Integrand have integrable singularities at A/B.

        F(X) must diverge as "(x-A)^alpha" at A, as "(B-x)^beta" at B,  with known
        alpha/beta (alpha>-1, beta>-1).  If alpha/beta  are  not known,  estimates
        from below can be used (but these estimates should be greater than -1 too).

        One  of  alpha/beta variables (or even both alpha/beta) may be equal to 0,
        which means than function F(x) is non-singular at A/B. Anyway (singular at
        bounds or not), function F(x) is supposed to be continuous on (A,B).

        Fast-convergent algorithm based on a Gauss-Kronrod formula is used. Result
        is calculated with accuracy close to the machine precision.

        INPUT PARAMETERS:
            A, B    -   interval boundaries (A<B, A=B or A>B)
            Alpha   -   power-law coefficient of the F(x) at A,
                        Alpha>-1
            Beta    -   power-law coefficient of the F(x) at B,
                        Beta>-1

        OUTPUT PARAMETERS
            State   -   structure which stores algorithm state

        SEE ALSO
            AutoGKSmooth, AutoGKSmoothW, AutoGKResults.


          -- ALGLIB --
             Copyright 06.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void autogksingular(double a,
            double b,
            double alpha,
            double beta,
            autogkstate state,
            alglib.xparams _params)
        {
            alglib.ap.assert(math.isfinite(a), "AutoGKSingular: A is not finite!");
            alglib.ap.assert(math.isfinite(b), "AutoGKSingular: B is not finite!");
            alglib.ap.assert(math.isfinite(alpha), "AutoGKSingular: Alpha is not finite!");
            alglib.ap.assert(math.isfinite(beta), "AutoGKSingular: Beta is not finite!");
            state.wrappermode = 1;
            state.a = a;
            state.b = b;
            state.alpha = alpha;
            state.beta = beta;
            state.xwidth = 0.0;
            state.needf = false;
            state.rstate.ra = new double[10+1];
            state.rstate.stage = -1;
        }


        /*************************************************************************

          -- ALGLIB --
             Copyright 07.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static bool autogkiteration(autogkstate state,
            alglib.xparams _params)
        {
            bool result = new bool();
            double s = 0;
            double tmp = 0;
            double eps = 0;
            double a = 0;
            double b = 0;
            double x = 0;
            double t = 0;
            double alpha = 0;
            double beta = 0;
            double v1 = 0;
            double v2 = 0;

            
            //
            // Reverse communication preparations
            // I know it looks ugly, but it works the same way
            // anywhere from C++ to Python.
            //
            // This code initializes locals by:
            // * random values determined during code
            //   generation - on first subroutine call
            // * values from previous call - on subsequent calls
            //
            if( state.rstate.stage>=0 )
            {
                s = state.rstate.ra[0];
                tmp = state.rstate.ra[1];
                eps = state.rstate.ra[2];
                a = state.rstate.ra[3];
                b = state.rstate.ra[4];
                x = state.rstate.ra[5];
                t = state.rstate.ra[6];
                alpha = state.rstate.ra[7];
                beta = state.rstate.ra[8];
                v1 = state.rstate.ra[9];
                v2 = state.rstate.ra[10];
            }
            else
            {
                s = 359;
                tmp = -58;
                eps = -919;
                a = -909;
                b = 81;
                x = 255;
                t = 74;
                alpha = -788;
                beta = 809;
                v1 = 205;
                v2 = -838;
            }
            if( state.rstate.stage==0 )
            {
                goto lbl_0;
            }
            if( state.rstate.stage==1 )
            {
                goto lbl_1;
            }
            if( state.rstate.stage==2 )
            {
                goto lbl_2;
            }
            
            //
            // Routine body
            //
            eps = 0;
            a = state.a;
            b = state.b;
            alpha = state.alpha;
            beta = state.beta;
            state.terminationtype = -1;
            state.nfev = 0;
            state.nintervals = 0;
            
            //
            // smooth function  at a finite interval
            //
            if( state.wrappermode!=0 )
            {
                goto lbl_3;
            }
            
            //
            // special case
            //
            if( (double)(a)==(double)(b) )
            {
                state.terminationtype = 1;
                state.v = 0;
                result = false;
                return result;
            }
            
            //
            // general case
            //
            autogkinternalprepare(a, b, eps, state.xwidth, state.internalstate, _params);
        lbl_5:
            if( !autogkinternaliteration(state.internalstate, _params) )
            {
                goto lbl_6;
            }
            x = state.internalstate.x;
            state.x = x;
            state.xminusa = x-a;
            state.bminusx = b-x;
            state.needf = true;
            state.rstate.stage = 0;
            goto lbl_rcomm;
        lbl_0:
            state.needf = false;
            state.nfev = state.nfev+1;
            state.internalstate.f = state.f;
            goto lbl_5;
        lbl_6:
            state.v = state.internalstate.r;
            state.terminationtype = state.internalstate.info;
            state.nintervals = state.internalstate.heapused;
            result = false;
            return result;
        lbl_3:
            
            //
            // function with power-law singularities at the ends of a finite interval
            //
            if( state.wrappermode!=1 )
            {
                goto lbl_7;
            }
            
            //
            // test coefficients
            //
            if( (double)(alpha)<=(double)(-1) || (double)(beta)<=(double)(-1) )
            {
                state.terminationtype = -1;
                state.v = 0;
                result = false;
                return result;
            }
            
            //
            // special cases
            //
            if( (double)(a)==(double)(b) )
            {
                state.terminationtype = 1;
                state.v = 0;
                result = false;
                return result;
            }
            
            //
            // reduction to general form
            //
            if( (double)(a)<(double)(b) )
            {
                s = 1;
            }
            else
            {
                s = -1;
                tmp = a;
                a = b;
                b = tmp;
                tmp = alpha;
                alpha = beta;
                beta = tmp;
            }
            alpha = Math.Min(alpha, 0);
            beta = Math.Min(beta, 0);
            
            //
            // first, integrate left half of [a,b]:
            //     integral(f(x)dx, a, (b+a)/2) =
            //     = 1/(1+alpha) * integral(t^(-alpha/(1+alpha))*f(a+t^(1/(1+alpha)))dt, 0, (0.5*(b-a))^(1+alpha))
            //
            autogkinternalprepare(0, Math.Pow(0.5*(b-a), 1+alpha), eps, state.xwidth, state.internalstate, _params);
        lbl_9:
            if( !autogkinternaliteration(state.internalstate, _params) )
            {
                goto lbl_10;
            }
            
            //
            // Fill State.X, State.XMinusA, State.BMinusX.
            // Latter two are filled correctly even if B<A.
            //
            x = state.internalstate.x;
            t = Math.Pow(x, 1/(1+alpha));
            state.x = a+t;
            if( (double)(s)>(double)(0) )
            {
                state.xminusa = t;
                state.bminusx = b-(a+t);
            }
            else
            {
                state.xminusa = a+t-b;
                state.bminusx = -t;
            }
            state.needf = true;
            state.rstate.stage = 1;
            goto lbl_rcomm;
        lbl_1:
            state.needf = false;
            if( (double)(alpha)!=(double)(0) )
            {
                state.internalstate.f = state.f*Math.Pow(x, -(alpha/(1+alpha)))/(1+alpha);
            }
            else
            {
                state.internalstate.f = state.f;
            }
            state.nfev = state.nfev+1;
            goto lbl_9;
        lbl_10:
            v1 = state.internalstate.r;
            state.nintervals = state.nintervals+state.internalstate.heapused;
            
            //
            // then, integrate right half of [a,b]:
            //     integral(f(x)dx, (b+a)/2, b) =
            //     = 1/(1+beta) * integral(t^(-beta/(1+beta))*f(b-t^(1/(1+beta)))dt, 0, (0.5*(b-a))^(1+beta))
            //
            autogkinternalprepare(0, Math.Pow(0.5*(b-a), 1+beta), eps, state.xwidth, state.internalstate, _params);
        lbl_11:
            if( !autogkinternaliteration(state.internalstate, _params) )
            {
                goto lbl_12;
            }
            
            //
            // Fill State.X, State.XMinusA, State.BMinusX.
            // Latter two are filled correctly (X-A, B-X) even if B<A.
            //
            x = state.internalstate.x;
            t = Math.Pow(x, 1/(1+beta));
            state.x = b-t;
            if( (double)(s)>(double)(0) )
            {
                state.xminusa = b-t-a;
                state.bminusx = t;
            }
            else
            {
                state.xminusa = -t;
                state.bminusx = a-(b-t);
            }
            state.needf = true;
            state.rstate.stage = 2;
            goto lbl_rcomm;
        lbl_2:
            state.needf = false;
            if( (double)(beta)!=(double)(0) )
            {
                state.internalstate.f = state.f*Math.Pow(x, -(beta/(1+beta)))/(1+beta);
            }
            else
            {
                state.internalstate.f = state.f;
            }
            state.nfev = state.nfev+1;
            goto lbl_11;
        lbl_12:
            v2 = state.internalstate.r;
            state.nintervals = state.nintervals+state.internalstate.heapused;
            
            //
            // final result
            //
            state.v = s*(v1+v2);
            state.terminationtype = 1;
            result = false;
            return result;
        lbl_7:
            result = false;
            return result;
            
            //
            // Saving state
            //
        lbl_rcomm:
            result = true;
            state.rstate.ra[0] = s;
            state.rstate.ra[1] = tmp;
            state.rstate.ra[2] = eps;
            state.rstate.ra[3] = a;
            state.rstate.ra[4] = b;
            state.rstate.ra[5] = x;
            state.rstate.ra[6] = t;
            state.rstate.ra[7] = alpha;
            state.rstate.ra[8] = beta;
            state.rstate.ra[9] = v1;
            state.rstate.ra[10] = v2;
            return result;
        }


        /*************************************************************************
        Adaptive integration results

        Called after AutoGKIteration returned False.

        Input parameters:
            State   -   algorithm state (used by AutoGKIteration).

        Output parameters:
            V       -   integral(f(x)dx,a,b)
            Rep     -   optimization report (see AutoGKReport description)

          -- ALGLIB --
             Copyright 14.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void autogkresults(autogkstate state,
            ref double v,
            autogkreport rep,
            alglib.xparams _params)
        {
            v = 0;

            v = state.v;
            rep.terminationtype = state.terminationtype;
            rep.nfev = state.nfev;
            rep.nintervals = state.nintervals;
        }


        /*************************************************************************
        Internal AutoGK subroutine
        eps<0   - error
        eps=0   - automatic eps selection

        width<0 -   error
        width=0 -   no width requirements
        *************************************************************************/
        private static void autogkinternalprepare(double a,
            double b,
            double eps,
            double xwidth,
            autogkinternalstate state,
            alglib.xparams _params)
        {
            
            //
            // Save settings
            //
            state.a = a;
            state.b = b;
            state.eps = eps;
            state.xwidth = xwidth;
            
            //
            // Prepare RComm structure
            //
            state.rstate.ia = new int[3+1];
            state.rstate.ra = new double[8+1];
            state.rstate.stage = -1;
        }


        /*************************************************************************
        Internal AutoGK subroutine
        *************************************************************************/
        private static bool autogkinternaliteration(autogkinternalstate state,
            alglib.xparams _params)
        {
            bool result = new bool();
            double c1 = 0;
            double c2 = 0;
            int i = 0;
            int j = 0;
            double intg = 0;
            double intk = 0;
            double inta = 0;
            double v = 0;
            double ta = 0;
            double tb = 0;
            int ns = 0;
            double qeps = 0;
            int info = 0;

            
            //
            // Reverse communication preparations
            // I know it looks ugly, but it works the same way
            // anywhere from C++ to Python.
            //
            // This code initializes locals by:
            // * random values determined during code
            //   generation - on first subroutine call
            // * values from previous call - on subsequent calls
            //
            if( state.rstate.stage>=0 )
            {
                i = state.rstate.ia[0];
                j = state.rstate.ia[1];
                ns = state.rstate.ia[2];
                info = state.rstate.ia[3];
                c1 = state.rstate.ra[0];
                c2 = state.rstate.ra[1];
                intg = state.rstate.ra[2];
                intk = state.rstate.ra[3];
                inta = state.rstate.ra[4];
                v = state.rstate.ra[5];
                ta = state.rstate.ra[6];
                tb = state.rstate.ra[7];
                qeps = state.rstate.ra[8];
            }
            else
            {
                i = 939;
                j = -526;
                ns = 763;
                info = -541;
                c1 = -698;
                c2 = -900;
                intg = -318;
                intk = -940;
                inta = 1016;
                v = -229;
                ta = -536;
                tb = 487;
                qeps = -115;
            }
            if( state.rstate.stage==0 )
            {
                goto lbl_0;
            }
            if( state.rstate.stage==1 )
            {
                goto lbl_1;
            }
            if( state.rstate.stage==2 )
            {
                goto lbl_2;
            }
            
            //
            // Routine body
            //
            
            //
            // initialize quadratures.
            // use 15-point Gauss-Kronrod formula.
            //
            state.n = 15;
            gkq.gkqgenerategausslegendre(state.n, ref info, ref state.qn, ref state.wk, ref state.wg, _params);
            if( info<0 )
            {
                state.info = -5;
                state.r = 0;
                result = false;
                return result;
            }
            state.wr = new double[state.n];
            for(i=0; i<=state.n-1; i++)
            {
                if( i==0 )
                {
                    state.wr[i] = 0.5*Math.Abs(state.qn[1]-state.qn[0]);
                    continue;
                }
                if( i==state.n-1 )
                {
                    state.wr[state.n-1] = 0.5*Math.Abs(state.qn[state.n-1]-state.qn[state.n-2]);
                    continue;
                }
                state.wr[i] = 0.5*Math.Abs(state.qn[i-1]-state.qn[i+1]);
            }
            
            //
            // special case
            //
            if( (double)(state.a)==(double)(state.b) )
            {
                state.info = 1;
                state.r = 0;
                result = false;
                return result;
            }
            
            //
            // test parameters
            //
            if( (double)(state.eps)<(double)(0) || (double)(state.xwidth)<(double)(0) )
            {
                state.info = -1;
                state.r = 0;
                result = false;
                return result;
            }
            state.info = 1;
            if( (double)(state.eps)==(double)(0) )
            {
                state.eps = 100000*math.machineepsilon;
            }
            
            //
            // First, prepare heap
            // * column 0   -   absolute error
            // * column 1   -   integral of a F(x) (calculated using Kronrod extension nodes)
            // * column 2   -   integral of a |F(x)| (calculated using modified rect. method)
            // * column 3   -   left boundary of a subinterval
            // * column 4   -   right boundary of a subinterval
            //
            if( (double)(state.xwidth)!=(double)(0) )
            {
                goto lbl_3;
            }
            
            //
            // no maximum width requirements
            // start from one big subinterval
            //
            state.heapwidth = 5;
            state.heapsize = 1;
            state.heapused = 1;
            state.heap = new double[state.heapsize, state.heapwidth];
            c1 = 0.5*(state.b-state.a);
            c2 = 0.5*(state.b+state.a);
            intg = 0;
            intk = 0;
            inta = 0;
            i = 0;
        lbl_5:
            if( i>state.n-1 )
            {
                goto lbl_7;
            }
            
            //
            // obtain F
            //
            state.x = c1*state.qn[i]+c2;
            state.rstate.stage = 0;
            goto lbl_rcomm;
        lbl_0:
            v = state.f;
            
            //
            // Gauss-Kronrod formula
            //
            intk = intk+v*state.wk[i];
            if( i%2==1 )
            {
                intg = intg+v*state.wg[i];
            }
            
            //
            // Integral |F(x)|
            // Use rectangles method
            //
            inta = inta+Math.Abs(v)*state.wr[i];
            i = i+1;
            goto lbl_5;
        lbl_7:
            intk = intk*(state.b-state.a)*0.5;
            intg = intg*(state.b-state.a)*0.5;
            inta = inta*(state.b-state.a)*0.5;
            state.heap[0,0] = Math.Abs(intg-intk);
            state.heap[0,1] = intk;
            state.heap[0,2] = inta;
            state.heap[0,3] = state.a;
            state.heap[0,4] = state.b;
            state.sumerr = state.heap[0,0];
            state.sumabs = Math.Abs(inta);
            goto lbl_4;
        lbl_3:
            
            //
            // maximum subinterval should be no more than XWidth.
            // so we create Ceil((B-A)/XWidth)+1 small subintervals
            //
            ns = (int)Math.Ceiling(Math.Abs(state.b-state.a)/state.xwidth)+1;
            state.heapsize = ns;
            state.heapused = ns;
            state.heapwidth = 5;
            state.heap = new double[state.heapsize, state.heapwidth];
            state.sumerr = 0;
            state.sumabs = 0;
            j = 0;
        lbl_8:
            if( j>ns-1 )
            {
                goto lbl_10;
            }
            ta = state.a+j*(state.b-state.a)/ns;
            tb = state.a+(j+1)*(state.b-state.a)/ns;
            c1 = 0.5*(tb-ta);
            c2 = 0.5*(tb+ta);
            intg = 0;
            intk = 0;
            inta = 0;
            i = 0;
        lbl_11:
            if( i>state.n-1 )
            {
                goto lbl_13;
            }
            
            //
            // obtain F
            //
            state.x = c1*state.qn[i]+c2;
            state.rstate.stage = 1;
            goto lbl_rcomm;
        lbl_1:
            v = state.f;
            
            //
            // Gauss-Kronrod formula
            //
            intk = intk+v*state.wk[i];
            if( i%2==1 )
            {
                intg = intg+v*state.wg[i];
            }
            
            //
            // Integral |F(x)|
            // Use rectangles method
            //
            inta = inta+Math.Abs(v)*state.wr[i];
            i = i+1;
            goto lbl_11;
        lbl_13:
            intk = intk*(tb-ta)*0.5;
            intg = intg*(tb-ta)*0.5;
            inta = inta*(tb-ta)*0.5;
            state.heap[j,0] = Math.Abs(intg-intk);
            state.heap[j,1] = intk;
            state.heap[j,2] = inta;
            state.heap[j,3] = ta;
            state.heap[j,4] = tb;
            state.sumerr = state.sumerr+state.heap[j,0];
            state.sumabs = state.sumabs+Math.Abs(inta);
            j = j+1;
            goto lbl_8;
        lbl_10:
        lbl_4:
            
            //
            // method iterations
            //
        lbl_14:
            if( false )
            {
                goto lbl_15;
            }
            
            //
            // additional memory if needed
            //
            if( state.heapused==state.heapsize )
            {
                mheapresize(ref state.heap, ref state.heapsize, 4*state.heapsize, state.heapwidth, _params);
            }
            
            //
            // TODO: every 20 iterations recalculate errors/sums
            //
            if( (double)(state.sumerr)<=(double)(state.eps*state.sumabs) || state.heapused>=maxsubintervals )
            {
                state.r = 0;
                for(j=0; j<=state.heapused-1; j++)
                {
                    state.r = state.r+state.heap[j,1];
                }
                result = false;
                return result;
            }
            
            //
            // Exclude interval with maximum absolute error
            //
            mheappop(ref state.heap, state.heapused, state.heapwidth, _params);
            state.sumerr = state.sumerr-state.heap[state.heapused-1,0];
            state.sumabs = state.sumabs-state.heap[state.heapused-1,2];
            
            //
            // Divide interval, create subintervals
            //
            ta = state.heap[state.heapused-1,3];
            tb = state.heap[state.heapused-1,4];
            state.heap[state.heapused-1,3] = ta;
            state.heap[state.heapused-1,4] = 0.5*(ta+tb);
            state.heap[state.heapused,3] = 0.5*(ta+tb);
            state.heap[state.heapused,4] = tb;
            j = state.heapused-1;
        lbl_16:
            if( j>state.heapused )
            {
                goto lbl_18;
            }
            c1 = 0.5*(state.heap[j,4]-state.heap[j,3]);
            c2 = 0.5*(state.heap[j,4]+state.heap[j,3]);
            intg = 0;
            intk = 0;
            inta = 0;
            i = 0;
        lbl_19:
            if( i>state.n-1 )
            {
                goto lbl_21;
            }
            
            //
            // F(x)
            //
            state.x = c1*state.qn[i]+c2;
            state.rstate.stage = 2;
            goto lbl_rcomm;
        lbl_2:
            v = state.f;
            
            //
            // Gauss-Kronrod formula
            //
            intk = intk+v*state.wk[i];
            if( i%2==1 )
            {
                intg = intg+v*state.wg[i];
            }
            
            //
            // Integral |F(x)|
            // Use rectangles method
            //
            inta = inta+Math.Abs(v)*state.wr[i];
            i = i+1;
            goto lbl_19;
        lbl_21:
            intk = intk*(state.heap[j,4]-state.heap[j,3])*0.5;
            intg = intg*(state.heap[j,4]-state.heap[j,3])*0.5;
            inta = inta*(state.heap[j,4]-state.heap[j,3])*0.5;
            state.heap[j,0] = Math.Abs(intg-intk);
            state.heap[j,1] = intk;
            state.heap[j,2] = inta;
            state.sumerr = state.sumerr+state.heap[j,0];
            state.sumabs = state.sumabs+state.heap[j,2];
            j = j+1;
            goto lbl_16;
        lbl_18:
            mheappush(ref state.heap, state.heapused-1, state.heapwidth, _params);
            mheappush(ref state.heap, state.heapused, state.heapwidth, _params);
            state.heapused = state.heapused+1;
            goto lbl_14;
        lbl_15:
            result = false;
            return result;
            
            //
            // Saving state
            //
        lbl_rcomm:
            result = true;
            state.rstate.ia[0] = i;
            state.rstate.ia[1] = j;
            state.rstate.ia[2] = ns;
            state.rstate.ia[3] = info;
            state.rstate.ra[0] = c1;
            state.rstate.ra[1] = c2;
            state.rstate.ra[2] = intg;
            state.rstate.ra[3] = intk;
            state.rstate.ra[4] = inta;
            state.rstate.ra[5] = v;
            state.rstate.ra[6] = ta;
            state.rstate.ra[7] = tb;
            state.rstate.ra[8] = qeps;
            return result;
        }


        private static void mheappop(ref double[,] heap,
            int heapsize,
            int heapwidth,
            alglib.xparams _params)
        {
            int i = 0;
            int p = 0;
            double t = 0;
            int maxcp = 0;

            if( heapsize==1 )
            {
                return;
            }
            for(i=0; i<=heapwidth-1; i++)
            {
                t = heap[heapsize-1,i];
                heap[heapsize-1,i] = heap[0,i];
                heap[0,i] = t;
            }
            p = 0;
            while( 2*p+1<heapsize-1 )
            {
                maxcp = 2*p+1;
                if( 2*p+2<heapsize-1 )
                {
                    if( (double)(heap[2*p+2,0])>(double)(heap[2*p+1,0]) )
                    {
                        maxcp = 2*p+2;
                    }
                }
                if( (double)(heap[p,0])<(double)(heap[maxcp,0]) )
                {
                    for(i=0; i<=heapwidth-1; i++)
                    {
                        t = heap[p,i];
                        heap[p,i] = heap[maxcp,i];
                        heap[maxcp,i] = t;
                    }
                    p = maxcp;
                }
                else
                {
                    break;
                }
            }
        }


        private static void mheappush(ref double[,] heap,
            int heapsize,
            int heapwidth,
            alglib.xparams _params)
        {
            int i = 0;
            int p = 0;
            double t = 0;
            int parent = 0;

            if( heapsize==0 )
            {
                return;
            }
            p = heapsize;
            while( p!=0 )
            {
                parent = (p-1)/2;
                if( (double)(heap[p,0])>(double)(heap[parent,0]) )
                {
                    for(i=0; i<=heapwidth-1; i++)
                    {
                        t = heap[p,i];
                        heap[p,i] = heap[parent,i];
                        heap[parent,i] = t;
                    }
                    p = parent;
                }
                else
                {
                    break;
                }
            }
        }


        private static void mheapresize(ref double[,] heap,
            ref int heapsize,
            int newheapsize,
            int heapwidth,
            alglib.xparams _params)
        {
            double[,] tmp = new double[0,0];
            int i = 0;
            int i_ = 0;

            tmp = new double[heapsize, heapwidth];
            for(i=0; i<=heapsize-1; i++)
            {
                for(i_=0; i_<=heapwidth-1;i_++)
                {
                    tmp[i,i_] = heap[i,i_];
                }
            }
            heap = new double[newheapsize, heapwidth];
            for(i=0; i<=heapsize-1; i++)
            {
                for(i_=0; i_<=heapwidth-1;i_++)
                {
                    heap[i,i_] = tmp[i,i_];
                }
            }
            heapsize = newheapsize;
        }


    }
}

