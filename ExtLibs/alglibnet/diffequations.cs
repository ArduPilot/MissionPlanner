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

    *************************************************************************/
    public class odesolverstate : alglibobject
    {
        //
        // Public declarations
        //
        public bool needdy { get { return _innerobj.needdy; } set { _innerobj.needdy = value; } }
        public double[] y { get { return _innerobj.y; } }
        public double[] dy { get { return _innerobj.dy; } }
        public double x { get { return _innerobj.x; } set { _innerobj.x = value; } }
    
        public odesolverstate()
        {
            _innerobj = new odesolver.odesolverstate();
        }
        
        public override alglib.alglibobject make_copy()
        {
            return new odesolverstate((odesolver.odesolverstate)_innerobj.make_copy());
        }
    
        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private odesolver.odesolverstate _innerobj;
        public odesolver.odesolverstate innerobj { get { return _innerobj; } }
        public odesolverstate(odesolver.odesolverstate obj)
        {
            _innerobj = obj;
        }
    }


    /*************************************************************************

    *************************************************************************/
    public class odesolverreport : alglibobject
    {
        //
        // Public declarations
        //
        public int nfev { get { return _innerobj.nfev; } set { _innerobj.nfev = value; } }
        public int terminationtype { get { return _innerobj.terminationtype; } set { _innerobj.terminationtype = value; } }
    
        public odesolverreport()
        {
            _innerobj = new odesolver.odesolverreport();
        }
        
        public override alglib.alglibobject make_copy()
        {
            return new odesolverreport((odesolver.odesolverreport)_innerobj.make_copy());
        }
    
        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private odesolver.odesolverreport _innerobj;
        public odesolver.odesolverreport innerobj { get { return _innerobj; } }
        public odesolverreport(odesolver.odesolverreport obj)
        {
            _innerobj = obj;
        }
    }
    
    /*************************************************************************
    Cash-Karp adaptive ODE solver.

    This subroutine solves ODE  Y'=f(Y,x)  with  initial  conditions  Y(xs)=Ys
    (here Y may be single variable or vector of N variables).

    INPUT PARAMETERS:
        Y       -   initial conditions, array[0..N-1].
                    contains values of Y[] at X[0]
        N       -   system size
        X       -   points at which Y should be tabulated, array[0..M-1]
                    integrations starts at X[0], ends at X[M-1],  intermediate
                    values at X[i] are returned too.
                    SHOULD BE ORDERED BY ASCENDING OR BY DESCENDING!
        M       -   number of intermediate points + first point + last point:
                    * M>2 means that you need both Y(X[M-1]) and M-2 values at
                      intermediate points
                    * M=2 means that you want just to integrate from  X[0]  to
                      X[1] and don't interested in intermediate values.
                    * M=1 means that you don't want to integrate :)
                      it is degenerate case, but it will be handled correctly.
                    * M<1 means error
        Eps     -   tolerance (absolute/relative error on each  step  will  be
                    less than Eps). When passing:
                    * Eps>0, it means desired ABSOLUTE error
                    * Eps<0, it means desired RELATIVE error.  Relative errors
                      are calculated with respect to maximum values of  Y seen
                      so far. Be careful to use this criterion  when  starting
                      from Y[] that are close to zero.
        H       -   initial  step  lenth,  it  will  be adjusted automatically
                    after the first  step.  If  H=0,  step  will  be  selected
                    automatically  (usualy  it  will  be  equal  to  0.001  of
                    min(x[i]-x[j])).

    OUTPUT PARAMETERS
        State   -   structure which stores algorithm state between  subsequent
                    calls of OdeSolverIteration. Used for reverse communication.
                    This structure should be passed  to the OdeSolverIteration
                    subroutine.

    SEE ALSO
        AutoGKSmoothW, AutoGKSingular, AutoGKIteration, AutoGKResults.


      -- ALGLIB --
         Copyright 01.09.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void odesolverrkck(double[] y, int n, double[] x, int m, double eps, double h, out odesolverstate state)
    {
        state = new odesolverstate();
        odesolver.odesolverrkck(y, n, x, m, eps, h, state.innerobj, null);
    }
    
    public static void odesolverrkck(double[] y, int n, double[] x, int m, double eps, double h, out odesolverstate state, alglib.xparams _params)
    {
        state = new odesolverstate();
        odesolver.odesolverrkck(y, n, x, m, eps, h, state.innerobj, _params);
    }
            
    public static void odesolverrkck(double[] y, double[] x, double eps, double h, out odesolverstate state)
    {
        int n;
        int m;
    
        state = new odesolverstate();
        n = ap.len(y);
        m = ap.len(x);
        odesolver.odesolverrkck(y, n, x, m, eps, h, state.innerobj, null);
    
        return;
    }
            
    public static void odesolverrkck(double[] y, double[] x, double eps, double h, out odesolverstate state, alglib.xparams _params)
    {
        int n;
        int m;
    
        state = new odesolverstate();
        n = ap.len(y);
        m = ap.len(x);
        odesolver.odesolverrkck(y, n, x, m, eps, h, state.innerobj, _params);
    
        return;
    }
    
    /*************************************************************************
    This function provides reverse communication interface
    Reverse communication interface is not documented or recommended to use.
    See below for functions which provide better documented API
    *************************************************************************/
    public static bool odesolveriteration(odesolverstate state)
    {
    
        return odesolver.odesolveriteration(state.innerobj, null);
    }
    
    public static bool odesolveriteration(odesolverstate state, alglib.xparams _params)
    {
    
        return odesolver.odesolveriteration(state.innerobj, _params);
    }
    /*************************************************************************
    This function is used to launcn iterations of ODE solver

    It accepts following parameters:
        diff    -   callback which calculates dy/dx for given y and x
        obj     -   optional object which is passed to diff; can be NULL


      -- ALGLIB --
         Copyright 01.09.2009 by Bochkanov Sergey

    *************************************************************************/
    public static void odesolversolve(odesolverstate state, ndimensional_ode_rp diff, object obj)
    {
        odesolversolve(state, diff, obj, null);
    }
    
    public static void odesolversolve(odesolverstate state, ndimensional_ode_rp diff, object obj, alglib.xparams _params)
    {
        if( diff==null )
            throw new alglibexception("ALGLIB: error in 'odesolversolve()' (diff is null)");
        while( alglib.odesolveriteration(state, _params) )
        {
            if( state.needdy )
            {
                diff(state.innerobj.y, state.innerobj.x, state.innerobj.dy, obj);
                continue;
            }
            throw new alglibexception("ALGLIB: unexpected error in 'odesolversolve'");
        }
    }


    
    /*************************************************************************
    ODE solver results

    Called after OdeSolverIteration returned False.

    INPUT PARAMETERS:
        State   -   algorithm state (used by OdeSolverIteration).

    OUTPUT PARAMETERS:
        M       -   number of tabulated values, M>=1
        XTbl    -   array[0..M-1], values of X
        YTbl    -   array[0..M-1,0..N-1], values of Y in X[i]
        Rep     -   solver report:
                    * Rep.TerminationType completetion code:
                        * -2    X is not ordered  by  ascending/descending  or
                                there are non-distinct X[],  i.e.  X[i]=X[i+1]
                        * -1    incorrect parameters were specified
                        *  1    task has been solved
                    * Rep.NFEV contains number of function calculations

      -- ALGLIB --
         Copyright 01.09.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void odesolverresults(odesolverstate state, out int m, out double[] xtbl, out double[,] ytbl, out odesolverreport rep)
    {
        m = 0;
        xtbl = new double[0];
        ytbl = new double[0,0];
        rep = new odesolverreport();
        odesolver.odesolverresults(state.innerobj, ref m, ref xtbl, ref ytbl, rep.innerobj, null);
    }
    
    public static void odesolverresults(odesolverstate state, out int m, out double[] xtbl, out double[,] ytbl, out odesolverreport rep, alglib.xparams _params)
    {
        m = 0;
        xtbl = new double[0];
        ytbl = new double[0,0];
        rep = new odesolverreport();
        odesolver.odesolverresults(state.innerobj, ref m, ref xtbl, ref ytbl, rep.innerobj, _params);
    }

}
public partial class alglib
{
    public class odesolver
    {
        public class odesolverstate : apobject
        {
            public int n;
            public int m;
            public double xscale;
            public double h;
            public double eps;
            public bool fraceps;
            public double[] yc;
            public double[] escale;
            public double[] xg;
            public int solvertype;
            public bool needdy;
            public double x;
            public double[] y;
            public double[] dy;
            public double[,] ytbl;
            public int repterminationtype;
            public int repnfev;
            public double[] yn;
            public double[] yns;
            public double[] rka;
            public double[] rkc;
            public double[] rkcs;
            public double[,] rkb;
            public double[,] rkk;
            public rcommstate rstate;
            public odesolverstate()
            {
                init();
            }
            public override void init()
            {
                yc = new double[0];
                escale = new double[0];
                xg = new double[0];
                y = new double[0];
                dy = new double[0];
                ytbl = new double[0,0];
                yn = new double[0];
                yns = new double[0];
                rka = new double[0];
                rkc = new double[0];
                rkcs = new double[0];
                rkb = new double[0,0];
                rkk = new double[0,0];
                rstate = new rcommstate();
            }
            public override alglib.apobject make_copy()
            {
                odesolverstate _result = new odesolverstate();
                _result.n = n;
                _result.m = m;
                _result.xscale = xscale;
                _result.h = h;
                _result.eps = eps;
                _result.fraceps = fraceps;
                _result.yc = (double[])yc.Clone();
                _result.escale = (double[])escale.Clone();
                _result.xg = (double[])xg.Clone();
                _result.solvertype = solvertype;
                _result.needdy = needdy;
                _result.x = x;
                _result.y = (double[])y.Clone();
                _result.dy = (double[])dy.Clone();
                _result.ytbl = (double[,])ytbl.Clone();
                _result.repterminationtype = repterminationtype;
                _result.repnfev = repnfev;
                _result.yn = (double[])yn.Clone();
                _result.yns = (double[])yns.Clone();
                _result.rka = (double[])rka.Clone();
                _result.rkc = (double[])rkc.Clone();
                _result.rkcs = (double[])rkcs.Clone();
                _result.rkb = (double[,])rkb.Clone();
                _result.rkk = (double[,])rkk.Clone();
                _result.rstate = (rcommstate)rstate.make_copy();
                return _result;
            }
        };


        public class odesolverreport : apobject
        {
            public int nfev;
            public int terminationtype;
            public odesolverreport()
            {
                init();
            }
            public override void init()
            {
            }
            public override alglib.apobject make_copy()
            {
                odesolverreport _result = new odesolverreport();
                _result.nfev = nfev;
                _result.terminationtype = terminationtype;
                return _result;
            }
        };




        public const double odesolvermaxgrow = 3.0;
        public const double odesolvermaxshrink = 10.0;


        /*************************************************************************
        Cash-Karp adaptive ODE solver.

        This subroutine solves ODE  Y'=f(Y,x)  with  initial  conditions  Y(xs)=Ys
        (here Y may be single variable or vector of N variables).

        INPUT PARAMETERS:
            Y       -   initial conditions, array[0..N-1].
                        contains values of Y[] at X[0]
            N       -   system size
            X       -   points at which Y should be tabulated, array[0..M-1]
                        integrations starts at X[0], ends at X[M-1],  intermediate
                        values at X[i] are returned too.
                        SHOULD BE ORDERED BY ASCENDING OR BY DESCENDING!
            M       -   number of intermediate points + first point + last point:
                        * M>2 means that you need both Y(X[M-1]) and M-2 values at
                          intermediate points
                        * M=2 means that you want just to integrate from  X[0]  to
                          X[1] and don't interested in intermediate values.
                        * M=1 means that you don't want to integrate :)
                          it is degenerate case, but it will be handled correctly.
                        * M<1 means error
            Eps     -   tolerance (absolute/relative error on each  step  will  be
                        less than Eps). When passing:
                        * Eps>0, it means desired ABSOLUTE error
                        * Eps<0, it means desired RELATIVE error.  Relative errors
                          are calculated with respect to maximum values of  Y seen
                          so far. Be careful to use this criterion  when  starting
                          from Y[] that are close to zero.
            H       -   initial  step  lenth,  it  will  be adjusted automatically
                        after the first  step.  If  H=0,  step  will  be  selected
                        automatically  (usualy  it  will  be  equal  to  0.001  of
                        min(x[i]-x[j])).

        OUTPUT PARAMETERS
            State   -   structure which stores algorithm state between  subsequent
                        calls of OdeSolverIteration. Used for reverse communication.
                        This structure should be passed  to the OdeSolverIteration
                        subroutine.

        SEE ALSO
            AutoGKSmoothW, AutoGKSingular, AutoGKIteration, AutoGKResults.


          -- ALGLIB --
             Copyright 01.09.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void odesolverrkck(double[] y,
            int n,
            double[] x,
            int m,
            double eps,
            double h,
            odesolverstate state,
            alglib.xparams _params)
        {
            alglib.ap.assert(n>=1, "ODESolverRKCK: N<1!");
            alglib.ap.assert(m>=1, "ODESolverRKCK: M<1!");
            alglib.ap.assert(alglib.ap.len(y)>=n, "ODESolverRKCK: Length(Y)<N!");
            alglib.ap.assert(alglib.ap.len(x)>=m, "ODESolverRKCK: Length(X)<M!");
            alglib.ap.assert(apserv.isfinitevector(y, n, _params), "ODESolverRKCK: Y contains infinite or NaN values!");
            alglib.ap.assert(apserv.isfinitevector(x, m, _params), "ODESolverRKCK: Y contains infinite or NaN values!");
            alglib.ap.assert(math.isfinite(eps), "ODESolverRKCK: Eps is not finite!");
            alglib.ap.assert((double)(eps)!=(double)(0), "ODESolverRKCK: Eps is zero!");
            alglib.ap.assert(math.isfinite(h), "ODESolverRKCK: H is not finite!");
            odesolverinit(0, y, n, x, m, eps, h, state, _params);
        }


        /*************************************************************************

          -- ALGLIB --
             Copyright 01.09.2009 by Bochkanov Sergey
        *************************************************************************/
        public static bool odesolveriteration(odesolverstate state,
            alglib.xparams _params)
        {
            bool result = new bool();
            int n = 0;
            int m = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            double xc = 0;
            double v = 0;
            double h = 0;
            double h2 = 0;
            bool gridpoint = new bool();
            double err = 0;
            double maxgrowpow = 0;
            int klimit = 0;
            int i_ = 0;

            
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
                n = state.rstate.ia[0];
                m = state.rstate.ia[1];
                i = state.rstate.ia[2];
                j = state.rstate.ia[3];
                k = state.rstate.ia[4];
                klimit = state.rstate.ia[5];
                gridpoint = state.rstate.ba[0];
                xc = state.rstate.ra[0];
                v = state.rstate.ra[1];
                h = state.rstate.ra[2];
                h2 = state.rstate.ra[3];
                err = state.rstate.ra[4];
                maxgrowpow = state.rstate.ra[5];
            }
            else
            {
                n = 359;
                m = -58;
                i = -919;
                j = -909;
                k = 81;
                klimit = 255;
                gridpoint = false;
                xc = -788;
                v = 809;
                h = 205;
                h2 = -838;
                err = 939;
                maxgrowpow = -526;
            }
            if( state.rstate.stage==0 )
            {
                goto lbl_0;
            }
            
            //
            // Routine body
            //
            
            //
            // prepare
            //
            if( state.repterminationtype!=0 )
            {
                result = false;
                return result;
            }
            n = state.n;
            m = state.m;
            h = state.h;
            maxgrowpow = Math.Pow(odesolvermaxgrow, 5);
            state.repnfev = 0;
            
            //
            // some preliminary checks for internal errors
            // after this we assume that H>0 and M>1
            //
            alglib.ap.assert((double)(state.h)>(double)(0), "ODESolver: internal error");
            alglib.ap.assert(m>1, "ODESolverIteration: internal error");
            
            //
            // choose solver
            //
            if( state.solvertype!=0 )
            {
                goto lbl_1;
            }
            
            //
            // Cask-Karp solver
            // Prepare coefficients table.
            // Check it for errors
            //
            state.rka = new double[6];
            state.rka[0] = 0;
            state.rka[1] = (double)1/(double)5;
            state.rka[2] = (double)3/(double)10;
            state.rka[3] = (double)3/(double)5;
            state.rka[4] = 1;
            state.rka[5] = (double)7/(double)8;
            state.rkb = new double[6, 5];
            state.rkb[1,0] = (double)1/(double)5;
            state.rkb[2,0] = (double)3/(double)40;
            state.rkb[2,1] = (double)9/(double)40;
            state.rkb[3,0] = (double)3/(double)10;
            state.rkb[3,1] = -((double)9/(double)10);
            state.rkb[3,2] = (double)6/(double)5;
            state.rkb[4,0] = -((double)11/(double)54);
            state.rkb[4,1] = (double)5/(double)2;
            state.rkb[4,2] = -((double)70/(double)27);
            state.rkb[4,3] = (double)35/(double)27;
            state.rkb[5,0] = (double)1631/(double)55296;
            state.rkb[5,1] = (double)175/(double)512;
            state.rkb[5,2] = (double)575/(double)13824;
            state.rkb[5,3] = (double)44275/(double)110592;
            state.rkb[5,4] = (double)253/(double)4096;
            state.rkc = new double[6];
            state.rkc[0] = (double)37/(double)378;
            state.rkc[1] = 0;
            state.rkc[2] = (double)250/(double)621;
            state.rkc[3] = (double)125/(double)594;
            state.rkc[4] = 0;
            state.rkc[5] = (double)512/(double)1771;
            state.rkcs = new double[6];
            state.rkcs[0] = (double)2825/(double)27648;
            state.rkcs[1] = 0;
            state.rkcs[2] = (double)18575/(double)48384;
            state.rkcs[3] = (double)13525/(double)55296;
            state.rkcs[4] = (double)277/(double)14336;
            state.rkcs[5] = (double)1/(double)4;
            state.rkk = new double[6, n];
            
            //
            // Main cycle consists of two iterations:
            // * outer where we travel from X[i-1] to X[i]
            // * inner where we travel inside [X[i-1],X[i]]
            //
            state.ytbl = new double[m, n];
            state.escale = new double[n];
            state.yn = new double[n];
            state.yns = new double[n];
            xc = state.xg[0];
            for(i_=0; i_<=n-1;i_++)
            {
                state.ytbl[0,i_] = state.yc[i_];
            }
            for(j=0; j<=n-1; j++)
            {
                state.escale[j] = 0;
            }
            i = 1;
        lbl_3:
            if( i>m-1 )
            {
                goto lbl_5;
            }
            
            //
            // begin inner iteration
            //
        lbl_6:
            if( false )
            {
                goto lbl_7;
            }
            
            //
            // truncate step if needed (beyond right boundary).
            // determine should we store X or not
            //
            if( (double)(xc+h)>=(double)(state.xg[i]) )
            {
                h = state.xg[i]-xc;
                gridpoint = true;
            }
            else
            {
                gridpoint = false;
            }
            
            //
            // Update error scale maximums
            //
            // These maximums are initialized by zeros,
            // then updated every iterations.
            //
            for(j=0; j<=n-1; j++)
            {
                state.escale[j] = Math.Max(state.escale[j], Math.Abs(state.yc[j]));
            }
            
            //
            // make one step:
            // 1. calculate all info needed to do step
            // 2. update errors scale maximums using values/derivatives
            //    obtained during (1)
            //
            // Take into account that we use scaling of X to reduce task
            // to the form where x[0] < x[1] < ... < x[n-1]. So X is
            // replaced by x=xscale*t, and dy/dx=f(y,x) is replaced
            // by dy/dt=xscale*f(y,xscale*t).
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.yn[i_] = state.yc[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.yns[i_] = state.yc[i_];
            }
            k = 0;
        lbl_8:
            if( k>5 )
            {
                goto lbl_10;
            }
            
            //
            // prepare data for the next update of YN/YNS
            //
            state.x = state.xscale*(xc+state.rka[k]*h);
            for(i_=0; i_<=n-1;i_++)
            {
                state.y[i_] = state.yc[i_];
            }
            for(j=0; j<=k-1; j++)
            {
                v = state.rkb[k,j];
                for(i_=0; i_<=n-1;i_++)
                {
                    state.y[i_] = state.y[i_] + v*state.rkk[j,i_];
                }
            }
            state.needdy = true;
            state.rstate.stage = 0;
            goto lbl_rcomm;
        lbl_0:
            state.needdy = false;
            state.repnfev = state.repnfev+1;
            v = h*state.xscale;
            for(i_=0; i_<=n-1;i_++)
            {
                state.rkk[k,i_] = v*state.dy[i_];
            }
            
            //
            // update YN/YNS
            //
            v = state.rkc[k];
            for(i_=0; i_<=n-1;i_++)
            {
                state.yn[i_] = state.yn[i_] + v*state.rkk[k,i_];
            }
            v = state.rkcs[k];
            for(i_=0; i_<=n-1;i_++)
            {
                state.yns[i_] = state.yns[i_] + v*state.rkk[k,i_];
            }
            k = k+1;
            goto lbl_8;
        lbl_10:
            
            //
            // estimate error
            //
            err = 0;
            for(j=0; j<=n-1; j++)
            {
                if( !state.fraceps )
                {
                    
                    //
                    // absolute error is estimated
                    //
                    err = Math.Max(err, Math.Abs(state.yn[j]-state.yns[j]));
                }
                else
                {
                    
                    //
                    // Relative error is estimated
                    //
                    v = state.escale[j];
                    if( (double)(v)==(double)(0) )
                    {
                        v = 1;
                    }
                    err = Math.Max(err, Math.Abs(state.yn[j]-state.yns[j])/v);
                }
            }
            
            //
            // calculate new step, restart if necessary
            //
            if( (double)(maxgrowpow*err)<=(double)(state.eps) )
            {
                h2 = odesolvermaxgrow*h;
            }
            else
            {
                h2 = h*Math.Pow(state.eps/err, 0.2);
            }
            if( (double)(h2)<(double)(h/odesolvermaxshrink) )
            {
                h2 = h/odesolvermaxshrink;
            }
            if( (double)(err)>(double)(state.eps) )
            {
                h = h2;
                goto lbl_6;
            }
            
            //
            // advance position
            //
            xc = xc+h;
            for(i_=0; i_<=n-1;i_++)
            {
                state.yc[i_] = state.yn[i_];
            }
            
            //
            // update H
            //
            h = h2;
            
            //
            // break on grid point
            //
            if( gridpoint )
            {
                goto lbl_7;
            }
            goto lbl_6;
        lbl_7:
            
            //
            // save result
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.ytbl[i,i_] = state.yc[i_];
            }
            i = i+1;
            goto lbl_3;
        lbl_5:
            state.repterminationtype = 1;
            result = false;
            return result;
        lbl_1:
            result = false;
            return result;
            
            //
            // Saving state
            //
        lbl_rcomm:
            result = true;
            state.rstate.ia[0] = n;
            state.rstate.ia[1] = m;
            state.rstate.ia[2] = i;
            state.rstate.ia[3] = j;
            state.rstate.ia[4] = k;
            state.rstate.ia[5] = klimit;
            state.rstate.ba[0] = gridpoint;
            state.rstate.ra[0] = xc;
            state.rstate.ra[1] = v;
            state.rstate.ra[2] = h;
            state.rstate.ra[3] = h2;
            state.rstate.ra[4] = err;
            state.rstate.ra[5] = maxgrowpow;
            return result;
        }


        /*************************************************************************
        ODE solver results

        Called after OdeSolverIteration returned False.

        INPUT PARAMETERS:
            State   -   algorithm state (used by OdeSolverIteration).

        OUTPUT PARAMETERS:
            M       -   number of tabulated values, M>=1
            XTbl    -   array[0..M-1], values of X
            YTbl    -   array[0..M-1,0..N-1], values of Y in X[i]
            Rep     -   solver report:
                        * Rep.TerminationType completetion code:
                            * -2    X is not ordered  by  ascending/descending  or
                                    there are non-distinct X[],  i.e.  X[i]=X[i+1]
                            * -1    incorrect parameters were specified
                            *  1    task has been solved
                        * Rep.NFEV contains number of function calculations

          -- ALGLIB --
             Copyright 01.09.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void odesolverresults(odesolverstate state,
            ref int m,
            ref double[] xtbl,
            ref double[,] ytbl,
            odesolverreport rep,
            alglib.xparams _params)
        {
            double v = 0;
            int i = 0;
            int i_ = 0;

            m = 0;
            xtbl = new double[0];
            ytbl = new double[0,0];

            rep.terminationtype = state.repterminationtype;
            if( rep.terminationtype>0 )
            {
                m = state.m;
                rep.nfev = state.repnfev;
                xtbl = new double[state.m];
                v = state.xscale;
                for(i_=0; i_<=state.m-1;i_++)
                {
                    xtbl[i_] = v*state.xg[i_];
                }
                ytbl = new double[state.m, state.n];
                for(i=0; i<=state.m-1; i++)
                {
                    for(i_=0; i_<=state.n-1;i_++)
                    {
                        ytbl[i,i_] = state.ytbl[i,i_];
                    }
                }
            }
            else
            {
                rep.nfev = 0;
            }
        }


        /*************************************************************************
        Internal initialization subroutine
        *************************************************************************/
        private static void odesolverinit(int solvertype,
            double[] y,
            int n,
            double[] x,
            int m,
            double eps,
            double h,
            odesolverstate state,
            alglib.xparams _params)
        {
            int i = 0;
            double v = 0;
            int i_ = 0;

            
            //
            // Prepare RComm
            //
            state.rstate.ia = new int[5+1];
            state.rstate.ba = new bool[0+1];
            state.rstate.ra = new double[5+1];
            state.rstate.stage = -1;
            state.needdy = false;
            
            //
            // check parameters.
            //
            if( (n<=0 || m<1) || (double)(eps)==(double)(0) )
            {
                state.repterminationtype = -1;
                return;
            }
            if( (double)(h)<(double)(0) )
            {
                h = -h;
            }
            
            //
            // quick exit if necessary.
            // after this block we assume that M>1
            //
            if( m==1 )
            {
                state.repnfev = 0;
                state.repterminationtype = 1;
                state.ytbl = new double[1, n];
                for(i_=0; i_<=n-1;i_++)
                {
                    state.ytbl[0,i_] = y[i_];
                }
                state.xg = new double[m];
                for(i_=0; i_<=m-1;i_++)
                {
                    state.xg[i_] = x[i_];
                }
                return;
            }
            
            //
            // check again: correct order of X[]
            //
            if( (double)(x[1])==(double)(x[0]) )
            {
                state.repterminationtype = -2;
                return;
            }
            for(i=1; i<=m-1; i++)
            {
                if( ((double)(x[1])>(double)(x[0]) && (double)(x[i])<=(double)(x[i-1])) || ((double)(x[1])<(double)(x[0]) && (double)(x[i])>=(double)(x[i-1])) )
                {
                    state.repterminationtype = -2;
                    return;
                }
            }
            
            //
            // auto-select H if necessary
            //
            if( (double)(h)==(double)(0) )
            {
                v = Math.Abs(x[1]-x[0]);
                for(i=2; i<=m-1; i++)
                {
                    v = Math.Min(v, Math.Abs(x[i]-x[i-1]));
                }
                h = 0.001*v;
            }
            
            //
            // store parameters
            //
            state.n = n;
            state.m = m;
            state.h = h;
            state.eps = Math.Abs(eps);
            state.fraceps = (double)(eps)<(double)(0);
            state.xg = new double[m];
            for(i_=0; i_<=m-1;i_++)
            {
                state.xg[i_] = x[i_];
            }
            if( (double)(x[1])>(double)(x[0]) )
            {
                state.xscale = 1;
            }
            else
            {
                state.xscale = -1;
                for(i_=0; i_<=m-1;i_++)
                {
                    state.xg[i_] = -1*state.xg[i_];
                }
            }
            state.yc = new double[n];
            for(i_=0; i_<=n-1;i_++)
            {
                state.yc[i_] = y[i_];
            }
            state.solvertype = solvertype;
            state.repterminationtype = 0;
            
            //
            // Allocate arrays
            //
            state.y = new double[n];
            state.dy = new double[n];
        }


    }
}

