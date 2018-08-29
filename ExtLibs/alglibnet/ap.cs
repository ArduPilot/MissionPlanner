/**************************************************************************
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
**************************************************************************/
using System;
public partial class alglib
{
    /********************************************************************
    Callback definitions for optimizers/fitters/solvers.
    
    Callbacks for unparameterized (general) functions:
    * ndimensional_func         calculates f(arg), stores result to func
    * ndimensional_grad         calculates func = f(arg), 
                                grad[i] = df(arg)/d(arg[i])
    * ndimensional_hess         calculates func = f(arg),
                                grad[i] = df(arg)/d(arg[i]),
                                hess[i,j] = d2f(arg)/(d(arg[i])*d(arg[j]))
    
    Callbacks for systems of functions:
    * ndimensional_fvec         calculates vector function f(arg),
                                stores result to fi
    * ndimensional_jac          calculates f[i] = fi(arg)
                                jac[i,j] = df[i](arg)/d(arg[j])
                                
    Callbacks for  parameterized  functions,  i.e.  for  functions  which 
    depend on two vectors: P and Q.  Gradient  and Hessian are calculated 
    with respect to P only.
    * ndimensional_pfunc        calculates f(p,q),
                                stores result to func
    * ndimensional_pgrad        calculates func = f(p,q),
                                grad[i] = df(p,q)/d(p[i])
    * ndimensional_phess        calculates func = f(p,q),
                                grad[i] = df(p,q)/d(p[i]),
                                hess[i,j] = d2f(p,q)/(d(p[i])*d(p[j]))

    Callbacks for progress reports:
    * ndimensional_rep          reports current position of optimization algo    
    
    Callbacks for ODE solvers:
    * ndimensional_ode_rp       calculates dy/dx for given y[] and x
    
    Callbacks for integrators:
    * integrator1_func          calculates f(x) for given x
                                (additional parameters xminusa and bminusx
                                contain x-a and b-x)
    ********************************************************************/
    public delegate void ndimensional_func (double[] arg, ref double func, object obj);
    public delegate void ndimensional_grad (double[] arg, ref double func, double[] grad, object obj);
    public delegate void ndimensional_hess (double[] arg, ref double func, double[] grad, double[,] hess, object obj);
    
    public delegate void ndimensional_fvec (double[] arg, double[] fi, object obj);
    public delegate void ndimensional_jac  (double[] arg, double[] fi, double[,] jac, object obj);
    
    public delegate void ndimensional_pfunc(double[] p, double[] q, ref double func, object obj);
    public delegate void ndimensional_pgrad(double[] p, double[] q, ref double func, double[] grad, object obj);
    public delegate void ndimensional_phess(double[] p, double[] q, ref double func, double[] grad, double[,] hess, object obj);
    
    public delegate void ndimensional_rep(double[] arg, double func, object obj);

    public delegate void ndimensional_ode_rp (double[] y, double x, double[] dy, object obj);

    public delegate void integrator1_func (double x, double xminusa, double bminusx, ref double f, object obj);

    /********************************************************************
    Class defining a complex number with double precision.
    ********************************************************************/
    public struct complex
    {
        public double x;
        public double y;

        public complex(double _x)
        {
            x = _x;
            y = 0;
        }
        public complex(double _x, double _y)
        {
            x = _x;
            y = _y;
        }
        public static implicit operator complex(double _x)
        {
            return new complex(_x);
        }
        public static bool operator==(complex lhs, complex rhs)
        {
            return ((double)lhs.x==(double)rhs.x) & ((double)lhs.y==(double)rhs.y);
        }
        public static bool operator!=(complex lhs, complex rhs)
        {
            return ((double)lhs.x!=(double)rhs.x) | ((double)lhs.y!=(double)rhs.y);
        }
        public static complex operator+(complex lhs)
        {
            return lhs;
        }
        public static complex operator-(complex lhs)
        {
            return new complex(-lhs.x,-lhs.y);
        }
        public static complex operator+(complex lhs, complex rhs)
        {
            return new complex(lhs.x+rhs.x,lhs.y+rhs.y);
        }
        public static complex operator-(complex lhs, complex rhs)
        {
            return new complex(lhs.x-rhs.x,lhs.y-rhs.y);
        }
        public static complex operator*(complex lhs, complex rhs)
        { 
            return new complex(lhs.x*rhs.x-lhs.y*rhs.y, lhs.x*rhs.y+lhs.y*rhs.x);
        }
        public static complex operator/(complex lhs, complex rhs)
        {
            complex result;
            double e;
            double f;
            if( System.Math.Abs(rhs.y)<System.Math.Abs(rhs.x) )
            {
                e = rhs.y/rhs.x;
                f = rhs.x+rhs.y*e;
                result.x = (lhs.x+lhs.y*e)/f;
                result.y = (lhs.y-lhs.x*e)/f;
            }
            else
            {
                e = rhs.x/rhs.y;
                f = rhs.y+rhs.x*e;
                result.x = (lhs.y+lhs.x*e)/f;
                result.y = (-lhs.x+lhs.y*e)/f;
            }
            return result;
        }
        public override int GetHashCode() 
        { 
            return x.GetHashCode() ^ y.GetHashCode(); 
        }
        public override bool Equals(object obj) 
        { 
            if( obj is byte)
                return Equals(new complex((byte)obj));
            if( obj is sbyte)
                return Equals(new complex((sbyte)obj));
            if( obj is short)
                return Equals(new complex((short)obj));
            if( obj is ushort)
                return Equals(new complex((ushort)obj));
            if( obj is int)
                return Equals(new complex((int)obj));
            if( obj is uint)
                return Equals(new complex((uint)obj));
            if( obj is long)
                return Equals(new complex((long)obj));
            if( obj is ulong)
                return Equals(new complex((ulong)obj));
            if( obj is float)
                return Equals(new complex((float)obj));
            if( obj is double)
                return Equals(new complex((double)obj));
            if( obj is decimal)
                return Equals(new complex((double)(decimal)obj));
            return base.Equals(obj); 
        }    
    }    
    
    /********************************************************************
    Class defining an ALGLIB exception
    ********************************************************************/
    public class alglibexception : System.Exception
    {
        public string msg;
        public alglibexception(string s)
        {
            msg = s;
        }
    }
    
    /********************************************************************
    Critical failure, resilts in immediate termination of entire program.
    ********************************************************************/
    public static void AE_CRITICAL_ASSERT(bool x)
    {
        if( !x )
            System.Environment.FailFast("ALGLIB: critical error");
    }
    
    /********************************************************************
    ALGLIB object, parent  class  for  all  internal  AlgoPascal  objects
    managed by ALGLIB.
    
    Any internal AlgoPascal object inherits from this class.
    
    User-visible objects inherit from alglibobject (see below).
    ********************************************************************/
    public abstract class apobject
    {
        public abstract void init();
        public abstract apobject make_copy();
    }
    
    /********************************************************************
    ALGLIB object, parent class for all user-visible objects  managed  by
    ALGLIB.
    
    Methods:
        _deallocate()       deallocation:
                            * in managed ALGLIB it does nothing
                            * in native ALGLIB it clears  dynamic  memory
                              being  hold  by  object  and  sets internal
                              reference to null.
        make_copy()         creates deep copy of the object.
                            Works in both managed and native versions  of
                            ALGLIB.
    ********************************************************************/
    public abstract class alglibobject : IDisposable
    {
        public virtual void _deallocate() {}
        public abstract alglibobject make_copy();
        public void Dispose()
        {
            _deallocate();
        }
    }
    
    /********************************************************************
    xparams object, used to pass additional parameters like multithreading
    settings, and several predefined values
    ********************************************************************/
    public class xparams
    {
        public ulong flags;
        public xparams(ulong v)
        {
            flags = v;
        }
    }
    private static ulong FLG_THREADING_MASK          = 0x7;
    private static   int FLG_THREADING_SHIFT         = 0;
    private static ulong FLG_THREADING_USE_GLOBAL    = 0x0;
    private static ulong FLG_THREADING_SERIAL        = 0x1;
    private static ulong FLG_THREADING_PARALLEL      = 0x2;
    public static xparams serial   = new xparams(FLG_THREADING_SERIAL);
    public static xparams parallel = new xparams(FLG_THREADING_PARALLEL);

    /********************************************************************
    Global flags, split into several char-sized variables in order
    to avoid problem with non-atomic reads/writes (single-byte ops
    are atomic on all modern architectures);
    
    Following variables are included:
    * threading-related settings
    ********************************************************************/
    public static byte global_threading_flags = (byte)(FLG_THREADING_SERIAL>>FLG_THREADING_SHIFT);
    
    public static void setglobalthreading(xparams p)
    {
        AE_CRITICAL_ASSERT(p!=null);
        ae_set_global_threading(p.flags);
    }
    
    public static void ae_set_global_threading(ulong flg_value)
    {
        flg_value = flg_value&FLG_THREADING_MASK;
        AE_CRITICAL_ASSERT(flg_value==FLG_THREADING_SERIAL || flg_value==FLG_THREADING_PARALLEL);
        global_threading_flags = (byte)(flg_value>>FLG_THREADING_SHIFT);
    }
    
    public static ulong ae_get_global_threading()
    {
        return ((ulong)global_threading_flags)<<FLG_THREADING_SHIFT;
    }
    
    static ulong ae_get_effective_threading(xparams p)
    {
        if( p==null || (p.flags&FLG_THREADING_MASK)==FLG_THREADING_USE_GLOBAL )
            return ((ulong)global_threading_flags)<<FLG_THREADING_SHIFT;
        return p.flags&FLG_THREADING_MASK;
    }
    
    /********************************************************************
    Deallocation of ALGLIB object:
    * in managed ALGLIB this method just sets refence to null
    * in native ALGLIB call of this method:
      1) clears dynamic memory being hold by  object  and  sets  internal
         reference to null.
      2) sets to null variable being passed to this method
    
    IMPORTANT (1): in  native  edition  of  ALGLIB,  obj becomes unusable
                   after this call!!!  It  is  possible  to  save  a copy
                   of reference in another variable (original variable is
                   set to null), but any attempt to work with this object
                   will crash your program.
    
    IMPORTANT (2): memory owned by object will be recycled by GC  in  any
                   case. This method just enforces IMMEDIATE deallocation.
    ********************************************************************/
    public static void deallocateimmediately<T>(ref T obj) where T : alglib.alglibobject
    {
        obj._deallocate();
        obj = null;
    }

    /********************************************************************
    Allocation counter:
    * in managed ALGLIB it always returns 0 (dummy code)
    * in native ALGLIB it returns current value of the allocation counter
      (if it was activated)
    ********************************************************************/
    public static long alloc_counter()
    {
        return 0;
    }
    
    /********************************************************************
    Activization of the allocation counter:
    * in managed ALGLIB it does nothing (dummy code)
    * in native ALGLIB it turns on allocation counting.
    ********************************************************************/
    public static void alloc_counter_activate()
    {
    }
    
    /********************************************************************
    This function allows to set one of the debug flags.
    In managed ALGLIB does nothing (dummy).
    ********************************************************************/
    public static void set_dbg_flag(long flag_id, long flag_value)
    {
    }
    
    /********************************************************************
    This function allows to get one of the debug counters.
    In managed ALGLIB does nothing (dummy).
    ********************************************************************/
    public static long get_dbg_value(long id)
    {
        return 0;
    }
    
    /********************************************************************
    Activization of the allocation counter:
    * in managed ALGLIB it does nothing (dummy code)
    * in native ALGLIB it turns on allocation counting.
    ********************************************************************/
    public static void free_disposed_items()
    {
    }
    
    /************************************************************************
    This function maps nworkers  number  (which  can  be  positive,  zero  or
    negative with 0 meaning "all cores", -1 meaning "all cores -1" and so on)
    to "effective", strictly positive workers count.

    This  function  is  intended  to  be used by debugging/testing code which
    tests different number of worker threads. It is NOT aligned  in  any  way
    with ALGLIB multithreading framework (i.e. it can return  non-zero worker
    count even for single-threaded GPLed ALGLIB).
    ************************************************************************/
    public static int get_effective_workers(int nworkers)
    {
        int ncores = System.Environment.ProcessorCount;
        if( nworkers>=1 )
            return nworkers>ncores ? ncores : nworkers;
        return ncores+nworkers>=1 ? ncores+nworkers : 1;
    }
    
    /********************************************************************
    reverse communication structure
    ********************************************************************/
    public class rcommstate : apobject
    {
        public rcommstate()
        {
            init();
        }
        public override void init()
        {
            stage = -1;
            ia = new int[0];
            ba = new bool[0];
            ra = new double[0];
            ca = new alglib.complex[0];
        }
        public override apobject make_copy()
        {
            rcommstate result = new rcommstate();
            result.stage = stage;
            result.ia = (int[])ia.Clone();
            result.ba = (bool[])ba.Clone();
            result.ra = (double[])ra.Clone();
            result.ca = (alglib.complex[])ca.Clone();
            return result;
        }
        public int stage;
        public int[] ia;
        public bool[] ba;
        public double[] ra;
        public alglib.complex[] ca;
    };

    /********************************************************************
    internal functions
    ********************************************************************/
    public class ap
    {
        public static int len<T>(T[] a)
        { return a.Length; }
        public static int rows<T>(T[,] a)
        { return a.GetLength(0); }
        public static int cols<T>(T[,] a)
        { return a.GetLength(1); }
        public static void swap<T>(ref T a, ref T b)
        {
            T t = a;
            a = b;
            b = t;
        }
        
        public static void assert(bool cond, string s)
        {
            if( !cond )
                throw new alglibexception(s);
        }
        
        public static void assert(bool cond)
        {
            assert(cond, "ALGLIB: assertion failed");
        }
        
        /****************************************************************
        Error tracking for unit testing purposes; utility functions.
        ****************************************************************/
        public static string sef_xdesc = "";
        
        public static void seterrorflag(ref bool flag, bool cond, string xdesc)
        {
            if( cond )
            {
                flag = true;
                sef_xdesc = xdesc;
            }
        }
        
        /****************************************************************
        returns dps (digits-of-precision) value corresponding to threshold.
        dps(0.9)  = dps(0.5)  = dps(0.1) = 0
        dps(0.09) = dps(0.05) = dps(0.01) = 1
        and so on
        ****************************************************************/
        public static int threshold2dps(double threshold)
        {
            int result = 0;
            double t;
            for (result = 0, t = 1; t / 10 > threshold*(1+1E-10); result++, t /= 10) ;
            return result;
        }

        /****************************************************************
        prints formatted complex
        ****************************************************************/
        public static string format(complex a, int _dps)
        {
            int dps = Math.Abs(_dps);
            string fmt = _dps>=0 ? "F" : "E";
            string fmtx = String.Format("{{0:"+fmt+"{0}}}", dps);
            string fmty = String.Format("{{0:"+fmt+"{0}}}", dps);
            string result = String.Format(fmtx, a.x) + (a.y >= 0 ? "+" : "-") + String.Format(fmty, Math.Abs(a.y)) + "i";
            result = result.Replace(',', '.');
            return result;
        }

        /****************************************************************
        prints formatted array
        ****************************************************************/
        public static string format(bool[] a)
        {
            string[] result = new string[len(a)];
            int i;
            for(i=0; i<len(a); i++)
                if( a[i] )
                    result[i] = "true";
                else
                    result[i] = "false";
            return "{"+String.Join(",",result)+"}";
        }
        
        /****************************************************************
        prints formatted array
        ****************************************************************/
        public static string format(int[] a)
        {
            string[] result = new string[len(a)];
            int i;
            for (i = 0; i < len(a); i++)
                result[i] = a[i].ToString();
            return "{" + String.Join(",", result) + "}";
        }

        /****************************************************************
        prints formatted array
        ****************************************************************/
        public static string format(double[] a, int _dps)
        {
            int dps = Math.Abs(_dps);
            string sfmt = _dps >= 0 ? "F" : "E";
            string fmt = String.Format("{{0:" + sfmt + "{0}}}", dps);
            string[] result = new string[len(a)];
            int i;
            for (i = 0; i < len(a); i++)
            {
                result[i] = String.Format(fmt, a[i]);
                result[i] = result[i].Replace(',', '.');
            }
            return "{" + String.Join(",", result) + "}";
        }

        /****************************************************************
        prints formatted array
        ****************************************************************/
        public static string format(complex[] a, int _dps)
        {
            int dps = Math.Abs(_dps);
            string fmt = _dps >= 0 ? "F" : "E";
            string fmtx = String.Format("{{0:"+fmt+"{0}}}", dps);
            string fmty = String.Format("{{0:"+fmt+"{0}}}", dps);
            string[] result = new string[len(a)];
            int i;
            for (i = 0; i < len(a); i++)
            {
                result[i] = String.Format(fmtx, a[i].x) + (a[i].y >= 0 ? "+" : "-") + String.Format(fmty, Math.Abs(a[i].y)) + "i";
                result[i] = result[i].Replace(',', '.');
            }
            return "{" + String.Join(",", result) + "}";
        }

        /****************************************************************
        prints formatted matrix
        ****************************************************************/
        public static string format(bool[,] a)
        {
            int i, j, m, n;
            n = cols(a);
            m = rows(a);
            bool[] line = new bool[n];
            string[] result = new string[m];
            for (i = 0; i < m; i++)
            {
                for (j = 0; j < n; j++)
                    line[j] = a[i, j];
                result[i] = format(line);
            }
            return "{" + String.Join(",", result) + "}";
        }

        /****************************************************************
        prints formatted matrix
        ****************************************************************/
        public static string format(int[,] a)
        {
            int i, j, m, n;
            n = cols(a);
            m = rows(a);
            int[] line = new int[n];
            string[] result = new string[m];
            for (i = 0; i < m; i++)
            {
                for (j = 0; j < n; j++)
                    line[j] = a[i, j];
                result[i] = format(line);
            }
            return "{" + String.Join(",", result) + "}";
        }

        /****************************************************************
        prints formatted matrix
        ****************************************************************/
        public static string format(double[,] a, int dps)
        {
            int i, j, m, n;
            n = cols(a);
            m = rows(a);
            double[] line = new double[n];
            string[] result = new string[m];
            for (i = 0; i < m; i++)
            {
                for (j = 0; j < n; j++)
                    line[j] = a[i, j];
                result[i] = format(line, dps);
            }
            return "{" + String.Join(",", result) + "}";
        }

        /****************************************************************
        prints formatted matrix
        ****************************************************************/
        public static string format(complex[,] a, int dps)
        {
            int i, j, m, n;
            n = cols(a);
            m = rows(a);
            complex[] line = new complex[n];
            string[] result = new string[m];
            for (i = 0; i < m; i++)
            {
                for (j = 0; j < n; j++)
                    line[j] = a[i, j];
                result[i] = format(line, dps);
            }
            return "{" + String.Join(",", result) + "}";
        }

        /****************************************************************
        checks that matrix is symmetric.
        max|A-A^T| is calculated; if it is within 1.0E-14 of max|A|,
        matrix is considered symmetric
        ****************************************************************/
        public static bool issymmetric(double[,] a)
        {
            int i, j, n;
            double err, mx, v1, v2;
            if( rows(a)!=cols(a) )
                return false;
            n = rows(a);
            if( n==0 )
                return true;
            mx = 0;
            err = 0;
            for( i=0; i<n; i++)
            {
                for(j=i+1; j<n; j++)
                {
                    v1 = a[i,j];
                    v2 = a[j,i];
                    if( !math.isfinite(v1) )
                        return false;
                    if( !math.isfinite(v2) )
                        return false;
                    err = Math.Max(err, Math.Abs(v1-v2));
                    mx  = Math.Max(mx,  Math.Abs(v1));
                    mx  = Math.Max(mx,  Math.Abs(v2));
                }
                v1 = a[i,i];
                if( !math.isfinite(v1) )
                    return false;
                mx = Math.Max(mx, Math.Abs(v1));
            }
            if( mx==0 )
                return true;
            return err/mx<=1.0E-14;
        }
        
        /****************************************************************
        checks that matrix is Hermitian.
        max|A-A^H| is calculated; if it is within 1.0E-14 of max|A|,
        matrix is considered Hermitian
        ****************************************************************/
        public static bool ishermitian(complex[,] a)
        {
            int i, j, n;
            double err, mx;
            complex v1, v2, vt;
            if( rows(a)!=cols(a) )
                return false;
            n = rows(a);
            if( n==0 )
                return true;
            mx = 0;
            err = 0;
            for( i=0; i<n; i++)
            {
                for(j=i+1; j<n; j++)
                {
                    v1 = a[i,j];
                    v2 = a[j,i];
                    if( !math.isfinite(v1.x) )
                        return false;
                    if( !math.isfinite(v1.y) )
                        return false;
                    if( !math.isfinite(v2.x) )
                        return false;
                    if( !math.isfinite(v2.y) )
                        return false;
                    vt.x = v1.x-v2.x;
                    vt.y = v1.y+v2.y;
                    err = Math.Max(err, math.abscomplex(vt));
                    mx  = Math.Max(mx,  math.abscomplex(v1));
                    mx  = Math.Max(mx,  math.abscomplex(v2));
                }
                v1 = a[i,i];
                if( !math.isfinite(v1.x) )
                    return false;
                if( !math.isfinite(v1.y) )
                    return false;
                err = Math.Max(err, Math.Abs(v1.y));
                mx = Math.Max(mx, math.abscomplex(v1));
            }
            if( mx==0 )
                return true;
            return err/mx<=1.0E-14;
        }
        
        
        /****************************************************************
        Forces symmetricity by copying upper half of A to the lower one
        ****************************************************************/
        public static bool forcesymmetric(double[,] a)
        {
            int i, j, n;
            if( rows(a)!=cols(a) )
                return false;
            n = rows(a);
            if( n==0 )
                return true;
            for( i=0; i<n; i++)
                for(j=i+1; j<n; j++)
                    a[i,j] = a[j,i];
            return true;
        }
        
        /****************************************************************
        Forces Hermiticity by copying upper half of A to the lower one
        ****************************************************************/
        public static bool forcehermitian(complex[,] a)
        {
            int i, j, n;
            complex v;
            if( rows(a)!=cols(a) )
                return false;
            n = rows(a);
            if( n==0 )
                return true;
            for( i=0; i<n; i++)
                for(j=i+1; j<n; j++)
                {
                    v = a[j,i];
                    a[i,j].x = v.x;
                    a[i,j].y = -v.y;
                }
            return true;
        }
    };
    
    /********************************************************************
    math functions
    ********************************************************************/
    public class math
    {
        //public static System.Random RndObject = new System.Random(System.DateTime.Now.Millisecond);
        public static System.Random rndobject = new System.Random(System.DateTime.Now.Millisecond + 1000*System.DateTime.Now.Second + 60*1000*System.DateTime.Now.Minute);

        public const double machineepsilon = 5E-16;
        public const double maxrealnumber = 1E300;
        public const double minrealnumber = 1E-300;
        
        public static bool isfinite(double d)
        {
            return !System.Double.IsNaN(d) && !System.Double.IsInfinity(d);
        }
        
        public static double randomreal()
        {
            double r = 0;
            lock(rndobject){ r = rndobject.NextDouble(); }
            return r;
        }
        public static int randominteger(int N)
        {
            int r = 0;
            lock(rndobject){ r = rndobject.Next(N); }
            return r;
        }
        public static double sqr(double X)
        {
            return X*X;
        }        
        public static double abscomplex(complex z)
        {
            double w;
            double xabs;
            double yabs;
            double v;
    
            xabs = System.Math.Abs(z.x);
            yabs = System.Math.Abs(z.y);
            w = xabs>yabs ? xabs : yabs;
            v = xabs<yabs ? xabs : yabs; 
            if( v==0 )
                return w;
            else
            {
                double t = v/w;
                return w*System.Math.Sqrt(1+t*t);
            }
        }
        public static complex conj(complex z)
        {
            return new complex(z.x, -z.y); 
        }    
        public static complex csqr(complex z)
        {
            return new complex(z.x*z.x-z.y*z.y, 2*z.x*z.y); 
        }

    }

    /*
     * CSV functionality
     */
     
    public static int CSV_DEFAULT      = 0x0;
    public static int CSV_SKIP_HEADERS = 0x1;
    
    /*
     * CSV operations: reading CSV file to real matrix.
     * 
     * This function reads CSV  file  and  stores  its  contents  to  double
     * precision 2D array. Format of the data file must conform to RFC  4180
     * specification, with additional notes:
     * - file size should be less than 2GB
     * - ASCI encoding, UTF-8 without BOM (in header names) are supported
     * - any character (comma/tab/space) may be used as field separator,  as
     *   long as it is distinct from one used for decimal point
     * - multiple subsequent field separators (say, two  spaces) are treated
     *   as MULTIPLE separators, not one big separator
     * - both comma and full stop may be used as decimal point. Parser  will
     *   automatically determine specific character being used.  Both  fixed
     *   and exponential number formats are  allowed.   Thousand  separators
     *   are NOT allowed.
     * - line may end with \n (Unix style) or \r\n (Windows  style),  parser
     *   will automatically adapt to chosen convention
     * - escaped fields (ones in double quotes) are not supported
     * 
     * INPUT PARAMETERS:
     *     filename        relative/absolute path
     *     separator       character used to separate fields.  May  be  ' ',
     *                     ',', '\t'. Other separators are possible too.
     *     flags           several values combined with bitwise OR:
     *                     * alglib::CSV_SKIP_HEADERS -  if present, first row
     *                       contains headers  and  will  be  skipped.   Its
     *                       contents is used to determine fields count, and
     *                       that's all.
     *                     If no flags are specified, default value 0x0  (or
     *                     alglib::CSV_DEFAULT, which is same) should be used.
     *                     
     * OUTPUT PARAMETERS:
     *     out             2D matrix, CSV file parsed with atof()
     *     
     * HANDLING OF SPECIAL CASES:
     * - file does not exist - alglib::ap_error exception is thrown
     * - empty file - empty array is returned (no exception)
     * - skip_first_row=true, only one row in file - empty array is returned
     * - field contents is not recognized by atof() - field value is replaced
     *   by 0.0
     */
    public static void read_csv(string filename, char separator, int flags, out double[,] matrix)
    {
        //
        // Parameters
        //
        bool skip_first_row = (flags&CSV_SKIP_HEADERS)!=0;
        
        //
        // Prepare empty output array
        //
        matrix = new double[0,0];
        
        //
        // Read file, normalize file contents:
        // * replace 0x0 by spaces
        // * remove trailing spaces and newlines
        // * append trailing '\n' and '\0' characters
        // Return if file contains only spaces/newlines.
        //
        byte b_space = System.Convert.ToByte(' ');
        byte b_tab   = System.Convert.ToByte('\t');
        byte b_lf    = System.Convert.ToByte('\n');
        byte b_cr    = System.Convert.ToByte('\r');
        byte b_comma = System.Convert.ToByte(',');
        byte b_fullstop= System.Convert.ToByte('.');
        byte[] v0 = System.IO.File.ReadAllBytes(filename);
        if( v0.Length==0 )
            return;
        byte[] v1 = new byte[v0.Length+2];
        int filesize = v0.Length;
        for(int i=0; i<filesize; i++)
            v1[i] = v0[i]==0 ? b_space : v0[i];
        for(; filesize>0; )
        {
            byte c = v1[filesize-1];
            if( c==b_space || c==b_tab || c==b_cr || c==b_lf )
            {
                filesize--;
                continue;
            }
            break;
        }
        if( filesize==0 )
            return;
        v1[filesize+0] = b_lf;
        v1[filesize+1] = 0x0;
        filesize+=2;
        
        
        //
        // Scan dataset.
        //
        int rows_count, cols_count, max_length = 0;
        cols_count = 1;
        for(int idx=0; idx<filesize; idx++)
        {
            if( v1[idx]==separator )
                cols_count++;
            if( v1[idx]==b_lf )
                break;
        }
        rows_count = 0;
        for(int idx=0; idx<filesize; idx++)
            if( v1[idx]==b_lf )
                rows_count++;
        if( rows_count==1 && skip_first_row ) // empty output, return
            return;
        int[] offsets = new int[rows_count*cols_count];
        int[] lengths = new int[rows_count*cols_count];
        int cur_row_idx = 0;
        for(int row_start=0; v1[row_start]!=0x0; )
        {
            // determine row length
            int row_length;
            for(row_length=0; v1[row_start+row_length]!=b_lf; row_length++);
            
            // determine cols count, perform integrity check
            int cur_cols_cnt=1;
            for(int idx=0; idx<row_length; idx++)
                if( v1[row_start+idx]==separator )
                    cur_cols_cnt++;
            if( cols_count!=cur_cols_cnt )
                throw new alglib.alglibexception("read_csv: non-rectangular contents, rows have different sizes");
            
            // store offsets and lengths of the fields
            int cur_offs = 0;
            int cur_col_idx = 0;
            for(int idx=0; idx<row_length+1; idx++)
                if( v1[row_start+idx]==separator || v1[row_start+idx]==b_lf )
                {
                    offsets[cur_row_idx*cols_count+cur_col_idx] = row_start+cur_offs;
                    lengths[cur_row_idx*cols_count+cur_col_idx] = idx-cur_offs;
                    max_length = idx-cur_offs>max_length ? idx-cur_offs : max_length;
                    cur_offs = idx+1;
                    cur_col_idx++;
                }
            
            // advance row start
            cur_row_idx++;
            row_start = row_start+row_length+1;
        }
        
        //
        // Convert
        //
        int row0 = skip_first_row ? 1 : 0;
        int row1 = rows_count;
        System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CreateSpecificCulture(""); // invariant culture
        matrix = new double[row1-row0, cols_count];
        alglib.AE_CRITICAL_ASSERT(culture.NumberFormat.NumberDecimalSeparator==".");
        for(int ridx=row0; ridx<row1; ridx++)
            for(int cidx=0; cidx<cols_count; cidx++)
            {
                int field_len  = lengths[ridx*cols_count+cidx];
                int field_offs = offsets[ridx*cols_count+cidx];
                
                // replace , by full stop
                for(int idx=0; idx<field_len; idx++)
                    if( v1[field_offs+idx]==b_comma )
                        v1[field_offs+idx] = b_fullstop;
                
                // convert
                string s_val = System.Text.Encoding.ASCII.GetString(v1, field_offs, field_len);
                double d_val;
                Double.TryParse(s_val, System.Globalization.NumberStyles.Float, culture, out d_val);
                matrix[ridx-row0,cidx] = d_val;
            }
    }
    
    
    /********************************************************************
    serializer object (should not be used directly)
    ********************************************************************/
    public class serializer
    {
        enum SMODE { DEFAULT, ALLOC, TO_STRING, FROM_STRING, TO_STREAM, FROM_STREAM };
        private const int SER_ENTRIES_PER_ROW = 5;
        private const int SER_ENTRY_LENGTH    = 11;
        
        private SMODE mode;
        private int entries_needed;
        private int entries_saved;
        private int bytes_asked;
        private int bytes_written;
        private int bytes_read;
        private char[] out_str;
        private char[] in_str;
        private System.IO.Stream io_stream;
        
        // local temporaries
        private char[] entry_buf_char;
        private byte[] entry_buf_byte; 
        
        public serializer()
        {
            mode = SMODE.DEFAULT;
            entries_needed = 0;
            bytes_asked = 0;
            entry_buf_byte = new byte[SER_ENTRY_LENGTH+2];
            entry_buf_char = new char[SER_ENTRY_LENGTH+2];
        }

        public void clear_buffers()
        {
            out_str = null;
            in_str = null;
            io_stream = null;
        }

        public void alloc_start()
        {
            entries_needed = 0;
            bytes_asked = 0;
            mode = SMODE.ALLOC;
        }

        public void alloc_entry()
        {
            if( mode!=SMODE.ALLOC )
                throw new alglib.alglibexception("ALGLIB: internal error during (un)serialization");
            entries_needed++;
        }

        private int get_alloc_size()
        {
            int rows, lastrowsize, result;
            
            // check and change mode
            if( mode!=SMODE.ALLOC )
                throw new alglib.alglibexception("ALGLIB: internal error during (un)serialization");
            
            // if no entries needes (degenerate case)
            if( entries_needed==0 )
            {
                bytes_asked = 4; /* a pair of chars for \r\n, one for space, one for dot */ 
                return bytes_asked;
            }
            
            // non-degenerate case
            rows = entries_needed/SER_ENTRIES_PER_ROW;
            lastrowsize = SER_ENTRIES_PER_ROW;
            if( entries_needed%SER_ENTRIES_PER_ROW!=0 )
            {
                lastrowsize = entries_needed%SER_ENTRIES_PER_ROW;
                rows++;
            }
            
            // calculate result size
            result  = ((rows-1)*SER_ENTRIES_PER_ROW+lastrowsize)*SER_ENTRY_LENGTH;  /* data size */
            result +=  (rows-1)*(SER_ENTRIES_PER_ROW-1)+(lastrowsize-1);            /* space symbols */
            result += rows*2;                                                       /* newline symbols */
            result += 1;                                                            /* trailing dot */
            bytes_asked = result;
            return result;
        }

        public void sstart_str()
        {
            int allocsize = get_alloc_size();
            
            // clear input/output buffers which may hold pointers to unneeded memory
            // NOTE: it also helps us to avoid errors when data are written to incorrect location
            clear_buffers();
            
            // check and change mode
            if( mode!=SMODE.ALLOC )
                throw new alglib.alglibexception("ALGLIB: internal error during (un)serialization");
            mode = SMODE.TO_STRING;
            
            // other preparations
            out_str = new char[allocsize];
            entries_saved = 0;
            bytes_written = 0;
        }

        public void sstart_stream(System.IO.Stream o_stream)
        {   
            // clear input/output buffers which may hold pointers to unneeded memory
            // NOTE: it also helps us to avoid errors when data are written to incorrect location
            clear_buffers();
            
            // check and change mode
            if( mode!=SMODE.ALLOC )
                throw new alglib.alglibexception("ALGLIB: internal error during (un)serialization");
            mode = SMODE.TO_STREAM;
            io_stream = o_stream;
        }

        public void ustart_str(string s)
        {
            // clear input/output buffers which may hold pointers to unneeded memory
            // NOTE: it also helps us to avoid errors when data are written to incorrect location
            clear_buffers();
            
            // check and change mode
            if( mode!=SMODE.DEFAULT )
                throw new alglib.alglibexception("ALGLIB: internal error during (un)serialization");
            mode = SMODE.FROM_STRING;
            
            in_str = s.ToCharArray();
            bytes_read = 0;
        }

        public void ustart_stream(System.IO.Stream i_stream)
        {
            // clear input/output buffers which may hold pointers to unneeded memory
            // NOTE: it also helps us to avoid errors when data are written to incorrect location
            clear_buffers();
            
            // check and change mode
            if( mode!=SMODE.DEFAULT )
                throw new alglib.alglibexception("ALGLIB: internal error during (un)serialization");
            mode = SMODE.FROM_STREAM;
            io_stream = i_stream;
        }

        private void serialize_value(bool v0, int v1, double v2, int val_idx)
        {
            // prepare serialization
            char[] arr_out = null;
            int cnt_out = 0;
            if( mode==SMODE.TO_STRING )
            {
                arr_out = out_str;
                cnt_out = bytes_written;
            }
            else if( mode==SMODE.TO_STREAM )
            {
                arr_out = entry_buf_char;
                cnt_out = 0;
            }
            else
                throw new alglib.alglibexception("ALGLIB: internal error during serialization");
            
            // serialize
            if( val_idx==0 )
                bool2str(  v0, arr_out, ref cnt_out);
            else if( val_idx==1 )
                int2str(   v1, arr_out, ref cnt_out);
            else if( val_idx==2 )
                double2str(v2, arr_out, ref cnt_out);
            else
                throw new alglib.alglibexception("ALGLIB: internal error during serialization");
            entries_saved++;
            if( entries_saved%SER_ENTRIES_PER_ROW!=0 )
            {
                arr_out[cnt_out] = ' ';
                cnt_out++;
            }
            else
            {
                arr_out[cnt_out+0] = '\r';
                arr_out[cnt_out+1] = '\n';
                cnt_out+=2;
            }
            
            // post-process
            if( mode==SMODE.TO_STRING )
            {
                bytes_written = cnt_out;
                return;
            }
            else if( mode==SMODE.TO_STREAM )
            {
                for(int k=0; k<cnt_out; k++)
                    entry_buf_byte[k] = (byte)entry_buf_char[k];
                io_stream.Write(entry_buf_byte, 0, cnt_out);
                return;
            }
            else
                throw new alglib.alglibexception("ALGLIB: internal error during serialization");
        }

        private void unstream_entry_char()
        {
            if( mode!=SMODE.FROM_STREAM )
                throw new alglib.alglibexception("ALGLIB: internal error during unserialization");
            int c;
            for(;;)
            {
                c = io_stream.ReadByte();
                if( c<0 )
                    throw new alglib.alglibexception("ALGLIB: internal error during unserialization");
                if( c!=' ' && c!='\t' && c!='\n' && c!='\r' )
                    break;
            }
            entry_buf_char[0] = (char)c;
            for(int k=1; k<SER_ENTRY_LENGTH; k++)
            {
                c = io_stream.ReadByte();
                entry_buf_char[k] = (char)c;
                if( c<0 || c==' ' || c=='\t' || c=='\n' || c=='\r' )
                    throw new alglib.alglibexception("ALGLIB: internal error during unserialization");
            }
            entry_buf_char[SER_ENTRY_LENGTH] = (char)0;
        }

        public void serialize_bool(bool v)
        {
            serialize_value(v, 0, 0, 0);
        }

        public void serialize_int(int v)
        {
            serialize_value(false, v, 0, 1);
        }

        public void serialize_double(double v)
        {
            serialize_value(false, 0, v, 2);
        }

        public bool unserialize_bool()
        {
            if( mode==SMODE.FROM_STRING )
                return str2bool(in_str, ref bytes_read);
            if( mode==SMODE.FROM_STREAM )
            {
                unstream_entry_char();
                int dummy = 0;
                return str2bool(entry_buf_char, ref dummy);
            }
            throw new alglib.alglibexception("ALGLIB: internal error during (un)serialization");
        }

        public int unserialize_int()
        {
            if( mode==SMODE.FROM_STRING )
                return str2int(in_str, ref bytes_read);
            if( mode==SMODE.FROM_STREAM )
            {
                unstream_entry_char();
                int dummy = 0;
                return str2int(entry_buf_char, ref dummy);
            }
            throw new alglib.alglibexception("ALGLIB: internal error during (un)serialization");
        }

        public double unserialize_double()
        {
            if( mode==SMODE.FROM_STRING )
                return str2double(in_str, ref bytes_read);
            if( mode==SMODE.FROM_STREAM )
            {
                unstream_entry_char();
                int dummy = 0;
                return str2double(entry_buf_char, ref dummy);
            }
            throw new alglib.alglibexception("ALGLIB: internal error during (un)serialization");
        }

        public void stop()
        {
            if( mode==SMODE.TO_STRING )
            {
                out_str[bytes_written] = '.';
                bytes_written++;
                return;
            }
            if( mode==SMODE.FROM_STRING )
            {
                //
                // because input string may be from pre-3.11 serializer,
                // which does not include trailing dot, we do not test
                // string for presence of "." symbol. Anyway, because string
                // is not stream, we do not have to read ALL trailing symbols.
                //
                return;
            }
            if( mode==SMODE.TO_STREAM )
            {
                io_stream.WriteByte((byte)'.');
                return;
            }
            if( mode==SMODE.FROM_STREAM )
            {
                for(;;)
                {
                    int c = io_stream.ReadByte();
                    if( c==' ' || c=='\t' || c=='\n' || c=='\r' )
                        continue;
                    if( c=='.' )
                        break;
                    throw new alglib.alglibexception("ALGLIB: internal error during unserialization");
                }
                return;
            }
            throw new alglib.alglibexception("ALGLIB: internal error during unserialization");
        }

        public string get_string()
        {
            if( mode!=SMODE.TO_STRING )
                throw new alglib.alglibexception("ALGLIB: internal error during (un)serialization");
             return new string(out_str, 0, bytes_written);
        }


        /************************************************************************
        This function converts six-bit value (from 0 to 63)  to  character  (only
        digits, lowercase and uppercase letters, minus and underscore are used).

        If v is negative or greater than 63, this function returns '?'.
        ************************************************************************/
        private static char[] _sixbits2char_tbl = new char[64]{ 
                '0', '1', '2', '3', '4', '5', '6', '7',
                '8', '9', 'A', 'B', 'C', 'D', 'E', 'F',
                'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',
                'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V',
                'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 
                'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 
                'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 
                'u', 'v', 'w', 'x', 'y', 'z', '-', '_' };
        private static char sixbits2char(int v)
        {
            if( v<0 || v>63 )
                return '?';
            return _sixbits2char_tbl[v];
        }
        
        /************************************************************************
        This function converts character to six-bit value (from 0 to 63).

        This function is inverse of ae_sixbits2char()
        If c is not correct character, this function returns -1.
        ************************************************************************/
        private static int[] _char2sixbits_tbl = new int[128] {
            -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, 62, -1, -1,
             0,  1,  2,  3,  4,  5,  6,  7,
             8,  9, -1, -1, -1, -1, -1, -1,
            -1, 10, 11, 12, 13, 14, 15, 16,
            17, 18, 19, 20, 21, 22, 23, 24,
            25, 26, 27, 28, 29, 30, 31, 32,
            33, 34, 35, -1, -1, -1, -1, 63,
            -1, 36, 37, 38, 39, 40, 41, 42,
            43, 44, 45, 46, 47, 48, 49, 50,
            51, 52, 53, 54, 55, 56, 57, 58,
            59, 60, 61, -1, -1, -1, -1, -1 };
        private static int char2sixbits(char c)
        {
            return (c>=0 && c<127) ? _char2sixbits_tbl[c] : -1;
        }
        
        /************************************************************************
        This function converts three bytes (24 bits) to four six-bit values 
        (24 bits again).

        src         array
        src_offs    offset of three-bytes chunk
        dst         array for ints
        dst_offs    offset of four-ints chunk
        ************************************************************************/
        private static void threebytes2foursixbits(byte[] src, int src_offs, int[] dst, int dst_offs)
        {
            dst[dst_offs+0] =  src[src_offs+0] & 0x3F;
            dst[dst_offs+1] = (src[src_offs+0]>>6) | ((src[src_offs+1]&0x0F)<<2);
            dst[dst_offs+2] = (src[src_offs+1]>>4) | ((src[src_offs+2]&0x03)<<4);
            dst[dst_offs+3] =  src[src_offs+2]>>2;
        }

        /************************************************************************
        This function converts four six-bit values (24 bits) to three bytes
        (24 bits again).

        src         pointer to four ints
        src_offs    offset of the chunk
        dst         pointer to three bytes
        dst_offs    offset of the chunk
        ************************************************************************/
        private static void foursixbits2threebytes(int[] src, int src_offs, byte[] dst, int dst_offs)
        {
            dst[dst_offs+0] =      (byte)(src[src_offs+0] | ((src[src_offs+1]&0x03)<<6));
            dst[dst_offs+1] = (byte)((src[src_offs+1]>>2) | ((src[src_offs+2]&0x0F)<<4));
            dst[dst_offs+2] = (byte)((src[src_offs+2]>>4) |  (src[src_offs+3]<<2));
        }

        /************************************************************************
        This function serializes boolean value into buffer

        v           boolean value to be serialized
        buf         buffer, at least 11 characters wide
        offs        offset in the buffer
        
        after return from this function, offs points to the char's past the value
        being read.
        ************************************************************************/
        private static void bool2str(bool v, char[] buf, ref int offs)
        {
            char c = v ? '1' : '0';
            int i;
            for(i=0; i<SER_ENTRY_LENGTH; i++)
                buf[offs+i] = c;
            offs += SER_ENTRY_LENGTH;
        }

        /************************************************************************
        This function unserializes boolean value from buffer

        buf         buffer which contains value; leading spaces/tabs/newlines are 
                    ignored, traling spaces/tabs/newlines are treated as  end  of
                    the boolean value.
        offs        offset in the buffer
        
        after return from this function, offs points to the char's past the value
        being read.

        This function raises an error in case unexpected symbol is found
        ************************************************************************/
        private static bool str2bool(char[] buf, ref int offs)
        {
            bool was0, was1;
            string emsg = "ALGLIB: unable to read boolean value from stream";
            
            was0 = false;
            was1 = false;
            while( buf[offs]==' ' || buf[offs]=='\t' || buf[offs]=='\n' || buf[offs]=='\r' )
                offs++;
            while( buf[offs]!=' ' && buf[offs]!='\t' && buf[offs]!='\n' && buf[offs]!='\r' && buf[offs]!=0 )
            {
                if( buf[offs]=='0' )
                {
                    was0 = true;
                    offs++;
                    continue;
                }
                if( buf[offs]=='1' )
                {
                    was1 = true;
                    offs++;
                    continue;
                }
                throw new alglib.alglibexception(emsg);
            }
            if( (!was0) && (!was1) )
                throw new alglib.alglibexception(emsg);
            if( was0 && was1 )
                throw new alglib.alglibexception(emsg);
            return was1 ? true : false;
        }

        /************************************************************************
        This function serializes integer value into buffer

        v           integer value to be serialized
        buf         buffer, at least 11 characters wide 
        offs        offset in the buffer
        
        after return from this function, offs points to the char's past the value
        being read.

        This function raises an error in case unexpected symbol is found
        ************************************************************************/
        private static void int2str(int v, char[] buf, ref int offs)
        {
            int i;
            byte[] _bytes = System.BitConverter.GetBytes((int)v);
            byte[]  bytes = new byte[9];
            int[] sixbits = new int[12];
            byte c;
            
            //
            // copy v to array of bytes, sign extending it and 
            // converting to little endian order. Additionally, 
            // we set 9th byte to zero in order to simplify 
            // conversion to six-bit representation
            //
            if( !System.BitConverter.IsLittleEndian )
                System.Array.Reverse(_bytes);
            c = v<0 ? (byte)0xFF : (byte)0x00;
            for(i=0; i<sizeof(int); i++)
                bytes[i] = _bytes[i];
            for(i=sizeof(int); i<8; i++)
                bytes[i] = c;
            bytes[8] = 0;
            
            //
            // convert to six-bit representation, output
            //
            // NOTE: last 12th element of sixbits is always zero, we do not output it
            //
            threebytes2foursixbits(bytes, 0, sixbits, 0);
            threebytes2foursixbits(bytes, 3, sixbits, 4);
            threebytes2foursixbits(bytes, 6, sixbits, 8);        
            for(i=0; i<SER_ENTRY_LENGTH; i++)
                buf[offs+i] = sixbits2char(sixbits[i]);
            offs += SER_ENTRY_LENGTH;
        }

        /************************************************************************
        This function unserializes integer value from string

        buf         buffer which contains value; leading spaces/tabs/newlines are 
                    ignored, traling spaces/tabs/newlines are treated as  end  of
                    the integer value.
        offs        offset in the buffer
        
        after return from this function, offs points to the char's past the value
        being read.

        This function raises an error in case unexpected symbol is found
        ************************************************************************/
        private static int str2int(char[] buf, ref int offs)
        {
            string emsg =       "ALGLIB: unable to read integer value from stream";
            string emsg3264 =   "ALGLIB: unable to read integer value from stream (value does not fit into 32 bits)";
            int[] sixbits = new int[12];
            byte[] bytes = new byte[9];
            byte[] _bytes = new byte[sizeof(int)];
            int sixbitsread, i;
            byte c;
            
            // 
            // 1. skip leading spaces
            // 2. read and decode six-bit digits
            // 3. set trailing digits to zeros
            // 4. convert to little endian 64-bit integer representation
            // 5. check that we fit into int
            // 6. convert to big endian representation, if needed
            //
            sixbitsread = 0;
            while( buf[offs]==' ' || buf[offs]=='\t' || buf[offs]=='\n' || buf[offs]=='\r' )
                offs++;
            while( buf[offs]!=' ' && buf[offs]!='\t' && buf[offs]!='\n' && buf[offs]!='\r' && buf[offs]!=0 )
            {
                int d;
                d = char2sixbits(buf[offs]);
                if( d<0 || sixbitsread>=SER_ENTRY_LENGTH )
                    throw new alglib.alglibexception(emsg);
                sixbits[sixbitsread] = d;
                sixbitsread++;
                offs++;
            }
            if( sixbitsread==0 )
                throw new alglib.alglibexception(emsg);
            for(i=sixbitsread; i<12; i++)
                sixbits[i] = 0;
            foursixbits2threebytes(sixbits, 0, bytes, 0);
            foursixbits2threebytes(sixbits, 4, bytes, 3);
            foursixbits2threebytes(sixbits, 8, bytes, 6);
            c = (bytes[sizeof(int)-1] & 0x80)!=0 ? (byte)0xFF : (byte)0x00;
            for(i=sizeof(int); i<8; i++)
                if( bytes[i]!=c )
                    throw new alglib.alglibexception(emsg3264);
            for(i=0; i<sizeof(int); i++)
                _bytes[i] = bytes[i];        
            if( !System.BitConverter.IsLittleEndian )
                System.Array.Reverse(_bytes);
            return System.BitConverter.ToInt32(_bytes,0);
        }    
        
        
        /************************************************************************
        This function serializes double value into buffer

        v           double value to be serialized
        buf         buffer, at least 11 characters wide 
        offs        offset in the buffer
        
        after return from this function, offs points to the char's past the value
        being read.
        ************************************************************************/
        private static void double2str(double v, char[] buf, ref int offs)
        {
            int i;
            int[] sixbits = new int[12];
            byte[] bytes = new byte[9];

            //
            // handle special quantities
            //
            if( System.Double.IsNaN(v) )
            {
                buf[offs+0] = '.';
                buf[offs+1] = 'n';
                buf[offs+2] = 'a';
                buf[offs+3] = 'n';
                buf[offs+4] = '_';
                buf[offs+5] = '_';
                buf[offs+6] = '_';
                buf[offs+7] = '_';
                buf[offs+8] = '_';
                buf[offs+9] = '_';
                buf[offs+10] = '_';
                offs += SER_ENTRY_LENGTH;
                return;
            }
            if( System.Double.IsPositiveInfinity(v) )
            {
                buf[offs+0] = '.';
                buf[offs+1] = 'p';
                buf[offs+2] = 'o';
                buf[offs+3] = 's';
                buf[offs+4] = 'i';
                buf[offs+5] = 'n';
                buf[offs+6] = 'f';
                buf[offs+7] = '_';
                buf[offs+8] = '_';
                buf[offs+9] = '_';
                buf[offs+10] = '_';
                offs += SER_ENTRY_LENGTH;
                return;
            }
            if( System.Double.IsNegativeInfinity(v) )
            {
                buf[offs+0] = '.';
                buf[offs+1] = 'n';
                buf[offs+2] = 'e';
                buf[offs+3] = 'g';
                buf[offs+4] = 'i';
                buf[offs+5] = 'n';
                buf[offs+6] = 'f';
                buf[offs+7] = '_';
                buf[offs+8] = '_';
                buf[offs+9] = '_';
                buf[offs+10] = '_';
                offs += SER_ENTRY_LENGTH;
                return;
            }
            
            //
            // process general case:
            // 1. copy v to array of chars
            // 2. set 9th byte to zero in order to simplify conversion to six-bit representation
            // 3. convert to little endian (if needed)
            // 4. convert to six-bit representation
            //    (last 12th element of sixbits is always zero, we do not output it)
            //
            byte[] _bytes = System.BitConverter.GetBytes((double)v);
            if( !System.BitConverter.IsLittleEndian )
                System.Array.Reverse(_bytes);
            for(i=0; i<sizeof(double); i++)
                bytes[i] = _bytes[i];
            for(i=sizeof(double); i<9; i++)
                bytes[i] = 0;
            threebytes2foursixbits(bytes, 0, sixbits, 0);
            threebytes2foursixbits(bytes, 3, sixbits, 4);
            threebytes2foursixbits(bytes, 6, sixbits, 8);
            for(i=0; i<SER_ENTRY_LENGTH; i++)
                buf[offs+i] = sixbits2char(sixbits[i]);
            offs += SER_ENTRY_LENGTH;
        }

        /************************************************************************
        This function unserializes double value from string

        buf         buffer which contains value; leading spaces/tabs/newlines are 
                    ignored, traling spaces/tabs/newlines are treated as  end  of
                    the double value.
        offs        offset in the buffer
        
        after return from this function, offs points to the char's past the value
        being read.

        This function raises an error in case unexpected symbol is found
        ************************************************************************/
        private static double str2double(char[] buf, ref int offs)
        {
            string emsg = "ALGLIB: unable to read double value from stream";
            int[] sixbits = new int[12];
            byte[]  bytes = new byte[9];
            byte[] _bytes = new byte[sizeof(double)];
            int sixbitsread, i;
            
            
            // 
            // skip leading spaces
            //
            while( buf[offs]==' ' || buf[offs]=='\t' || buf[offs]=='\n' || buf[offs]=='\r' )
                offs++;
            
              
            //
            // Handle special cases
            //
            if( buf[offs]=='.' )
            {
                string s = new string(buf, offs, SER_ENTRY_LENGTH);
                if( s==".nan_______" )
                {
                    offs += SER_ENTRY_LENGTH;
                    return System.Double.NaN;
                }
                if( s==".posinf____" )
                {
                    offs += SER_ENTRY_LENGTH;
                    return System.Double.PositiveInfinity;
                }
                if( s==".neginf____" )
                {
                    offs += SER_ENTRY_LENGTH;
                    return System.Double.NegativeInfinity;
                }
                throw new alglib.alglibexception(emsg);
            }
            
            // 
            // General case:
            // 1. read and decode six-bit digits
            // 2. check that all 11 digits were read
            // 3. set last 12th digit to zero (needed for simplicity of conversion)
            // 4. convert to 8 bytes
            // 5. convert to big endian representation, if needed
            //
            sixbitsread = 0;
            while( buf[offs]!=' ' && buf[offs]!='\t' && buf[offs]!='\n' && buf[offs]!='\r' && buf[offs]!=0 )
            {
                int d;
                d = char2sixbits(buf[offs]);
                if( d<0 || sixbitsread>=SER_ENTRY_LENGTH )
                    throw new alglib.alglibexception(emsg);
                sixbits[sixbitsread] = d;
                sixbitsread++;
                offs++;
            }
            if( sixbitsread!=SER_ENTRY_LENGTH )
                throw new alglib.alglibexception(emsg);
            sixbits[SER_ENTRY_LENGTH] = 0;
            foursixbits2threebytes(sixbits, 0, bytes, 0);
            foursixbits2threebytes(sixbits, 4, bytes, 3);
            foursixbits2threebytes(sixbits, 8, bytes, 6);
            for(i=0; i<sizeof(double); i++)
                _bytes[i] = bytes[i];        
            if( !System.BitConverter.IsLittleEndian )
                System.Array.Reverse(_bytes);        
            return System.BitConverter.ToDouble(_bytes,0);
        }
    }
    
    /*
     * Parts of alglib.smp class which are shared with GPL version of ALGLIB
     */
    public partial class smp
    {
        #pragma warning disable 420
        public const int AE_LOCK_CYCLES = 512;
        public const int AE_LOCK_TESTS_BEFORE_YIELD = 16;
        
        /*
         * This variable is used to perform spin-wait loops in a platform-independent manner
         * (loops which should work same way on Mono and Microsoft NET). You SHOULD NEVER
         * change this field - it must be zero during all program life.
         */
        public static volatile int never_change_it = 0;
        
        /*************************************************************************
        Lock.

        This class provides lightweight spin lock
        *************************************************************************/
        public class ae_lock
        {
            public volatile int is_locked;
        }

        /********************************************************************
        Shared pool: data structure used to provide thread-safe access to pool
        of temporary variables.
        ********************************************************************/
        public class sharedpoolentry
        {
            public apobject obj;
            public sharedpoolentry next_entry;
        }
        public class shared_pool : apobject
        {
            /* lock object which protects pool */
            public ae_lock pool_lock;
    
            /* seed object (used to create new instances of temporaries) */
            public volatile apobject seed_object;
            
            /*
             * list of recycled OBJECTS:
             * 1. entries in this list store pointers to recycled objects
             * 2. every time we retrieve object, we retrieve first entry from this list,
             *    move it to recycled_entries and return its obj field to caller/
             */
            public volatile sharedpoolentry recycled_objects;
            
            /* 
             * list of recycled ENTRIES:
             * 1. this list holds entries which are not used to store recycled objects;
             *    every time recycled object is retrieved, its entry is moved to this list.
             * 2. every time object is recycled, we try to fetch entry for him from this list
             *    before allocating it with malloc()
             */
            public volatile sharedpoolentry recycled_entries;
            
            /* enumeration pointer, points to current recycled object*/
            public volatile sharedpoolentry enumeration_counter;
            
            /* constructor */
            public shared_pool()
            {
                ae_init_lock(ref pool_lock);
            }
            
            /* initializer - creation of empty pool */
            public override void init()
            {
                seed_object = null;
                recycled_objects = null;
                recycled_entries = null;
                enumeration_counter = null;
            }
            
            /* copy constructor (it is NOT thread-safe) */
            public override apobject make_copy()
            {
                sharedpoolentry ptr, buf;
                shared_pool result = new shared_pool();
                
                /* create lock */
                ae_init_lock(ref result.pool_lock);
    
                /* copy seed object */
                if( seed_object!=null )
                    result.seed_object = seed_object.make_copy();
                
                /*
                 * copy recycled objects:
                 * 1. copy to temporary list (objects are inserted to beginning, order is reversed)
                 * 2. copy temporary list to output list (order is restored back to normal)
                 */
                buf = null;
                for(ptr=recycled_objects; ptr!=null; ptr=ptr.next_entry)
                {
                    sharedpoolentry tmp = new sharedpoolentry();
                    tmp.obj =  ptr.obj.make_copy();
                    tmp.next_entry = buf;
                    buf = tmp;
                }
                result.recycled_objects = null;
                for(ptr=buf; ptr!=null;)
                {
                    sharedpoolentry next_ptr = ptr.next_entry;
                    ptr.next_entry = result.recycled_objects;
                    result.recycled_objects = ptr;
                    ptr = next_ptr;
                }
    
                /* recycled entries are not copied because they do not store any information */
                result.recycled_entries = null;
    
                /* enumeration counter is reset on copying */
                result.enumeration_counter = null;
    
                return result;
            }
        }
        

        /************************************************************************
        This function performs given number of spin-wait iterations
        ************************************************************************/
        public static void ae_spin_wait(int cnt)
        {
            /*
             * these strange operations with ae_never_change_it are necessary to
             * prevent compiler optimization of the loop.
             */
            int i;
            
            /* very unlikely because no one will wait for such amount of cycles */
            if( cnt>0x12345678 )
                never_change_it = cnt%10;
            
            /* spin wait, test condition which will never be true */
            for(i=0; i<cnt; i++)
                if( never_change_it>0 )
                    never_change_it--;
        }


        /************************************************************************
        This function causes the calling thread to relinquish the CPU. The thread
        is moved to the end of the queue and some other thread gets to run.
        ************************************************************************/
        public static void ae_yield()
        {
            System.Threading.Thread.Sleep(0);
        }

        /************************************************************************
        This function initializes ae_lock structure and sets lock in a free mode.
        ************************************************************************/
        public static void ae_init_lock(ref ae_lock obj)
        {
            obj = new ae_lock();
            obj.is_locked = 0;
        }


        /************************************************************************
        This function acquires lock. In case lock is busy, we perform several
        iterations inside tight loop before trying again.
        ************************************************************************/
        public static void ae_acquire_lock(ae_lock obj)
        {
            int cnt = 0;
            for(;;)
            {
                if( System.Threading.Interlocked.CompareExchange(ref obj.is_locked, 1, 0)==0 )
                    return;
                ae_spin_wait(AE_LOCK_CYCLES);
                cnt++;
                if( cnt%AE_LOCK_TESTS_BEFORE_YIELD==0 )
                    ae_yield();
            }
        }


        /************************************************************************
        This function releases lock.
        ************************************************************************/
        public static void ae_release_lock(ae_lock obj)
        {
            System.Threading.Interlocked.Exchange(ref obj.is_locked, 0);
        }


        /************************************************************************
        This function frees ae_lock structure.
        ************************************************************************/
        public static void ae_free_lock(ref ae_lock obj)
        {
            obj = null;
        }
        
        
        /************************************************************************
        This function returns True, if internal seed object was set.  It  returns
        False for un-seeded pool.

        dst                 destination pool (initialized by constructor function)

        NOTE: this function is NOT thread-safe. It does not acquire pool lock, so
              you should NOT call it when lock can be used by another thread.
        ************************************************************************/
        public static bool ae_shared_pool_is_initialized(shared_pool dst)
        {
            return dst.seed_object!=null;
        }


        /************************************************************************
        This function sets internal seed object. All objects owned by the pool
        (current seed object, recycled objects) are automatically freed.

        dst                 destination pool (initialized by constructor function)
        seed_object         new seed object

        NOTE: this function is NOT thread-safe. It does not acquire pool lock, so
              you should NOT call it when lock can be used by another thread.
        ************************************************************************/
        public static void ae_shared_pool_set_seed(shared_pool dst, alglib.apobject seed_object)
        {
            dst.seed_object = seed_object.make_copy();
            dst.recycled_objects = null;
            dst.enumeration_counter = null;
        }


        /************************************************************************
        This  function  retrieves  a  copy  of  the seed object from the pool and
        stores it to target variable.

        pool                pool
        obj                 target variable
        
        NOTE: this function IS thread-safe.  It  acquires  pool  lock  during its
              operation and can be used simultaneously from several threads.
        ************************************************************************/
        public static void ae_shared_pool_retrieve<T>(shared_pool pool, ref T obj) where T : alglib.apobject
        {
            alglib.apobject new_obj;
            
            /* assert that pool was seeded */
            alglib.ap.assert(pool.seed_object!=null, "ALGLIB: shared pool is not seeded, PoolRetrieve() failed");
            
            /* acquire lock */
            ae_acquire_lock(pool.pool_lock);
            
            /* try to reuse recycled objects */
            if( pool.recycled_objects!=null )
            {
                /* retrieve entry/object from list of recycled objects */
                sharedpoolentry result = pool.recycled_objects;
                pool.recycled_objects = pool.recycled_objects.next_entry;
                new_obj = result.obj;
                result.obj = null;
                
                /* move entry to list of recycled entries */
                result.next_entry = pool.recycled_entries;
                pool.recycled_entries = result;
                
                /* release lock */
                ae_release_lock(pool.pool_lock);
                
                /* assign object to smart pointer */
                obj = (T)new_obj;
                
                return;
            }
                
            /*
             * release lock; we do not need it anymore because
             * copy constructor does not modify source variable.
             */
            ae_release_lock(pool.pool_lock);
            
            /* create new object from seed */
            new_obj = pool.seed_object.make_copy();
                
            /* assign object to pointer and return */
            obj = (T)new_obj;
        }


        /************************************************************************
        This  function  recycles object owned by the source variable by moving it
        to internal storage of the shared pool.

        Source  variable  must  own  the  object,  i.e.  be  the only place where
        reference  to  object  is  stored.  After  call  to  this function source
        variable becomes NULL.

        pool                pool
        obj                 source variable

        NOTE: this function IS thread-safe.  It  acquires  pool  lock  during its
              operation and can be used simultaneously from several threads.
        ************************************************************************/
        public static void ae_shared_pool_recycle<T>(shared_pool pool, ref T obj) where T : alglib.apobject
        {
            sharedpoolentry new_entry;
            
            /* assert that pool was seeded */
            alglib.ap.assert(pool.seed_object!=null, "ALGLIB: shared pool is not seeded, PoolRecycle() failed");
            
            /* assert that pointer non-null */
            alglib.ap.assert(obj!=null, "ALGLIB: obj in ae_shared_pool_recycle() is NULL");
            
            /* acquire lock */
            ae_acquire_lock(pool.pool_lock);
            
            /* acquire shared pool entry (reuse one from recycled_entries or malloc new one) */
            if( pool.recycled_entries!=null )
            {
                /* reuse previously allocated entry */
                new_entry = pool.recycled_entries;
                pool.recycled_entries = new_entry.next_entry;
            }
            else
            {
                /*
                 * Allocate memory for new entry.
                 *
                 * NOTE: we release pool lock during allocation because new() may raise
                 *       exception and we do not want our pool to be left in the locked state.
                 */
                ae_release_lock(pool.pool_lock);
                new_entry = new sharedpoolentry();
                ae_acquire_lock(pool.pool_lock);
            }
            
            /* add object to the list of recycled objects */
            new_entry.obj = obj;
            new_entry.next_entry = pool.recycled_objects;
            pool.recycled_objects = new_entry;
            
            /* release lock object */
            ae_release_lock(pool.pool_lock);
            
            /* release source pointer */
            obj = null;
        }


        /************************************************************************
        This function clears internal list of  recycled  objects,  but  does  not
        change seed object managed by the pool.

        pool                pool

        NOTE: this function is NOT thread-safe. It does not acquire pool lock, so
              you should NOT call it when lock can be used by another thread.
        ************************************************************************/
        public static void ae_shared_pool_clear_recycled(shared_pool pool)
        {
            pool.recycled_objects = null;
        }


        /************************************************************************
        This function allows to enumerate recycled elements of the  shared  pool.
        It stores reference to the first recycled object in the smart pointer.

        IMPORTANT:
        * in case target variable owns non-NULL value, it is rewritten
        * recycled object IS NOT removed from pool
        * target variable DOES NOT become owner of the new value; you can use
          reference to recycled object, but you do not own it.
        * this function IS NOT thread-safe
        * you SHOULD NOT modify shared pool during enumeration (although you  can
          modify state of the objects retrieved from pool)
        * in case there is no recycled objects in the pool, NULL is stored to obj
        * in case pool is not seeded, NULL is stored to obj

        pool                pool
        obj                 reference
        ************************************************************************/
        public static void ae_shared_pool_first_recycled<T>(shared_pool pool, ref T obj) where T : alglib.apobject
        {   
            /* modify internal enumeration counter */
            pool.enumeration_counter = pool.recycled_objects;
            
            /* exit on empty list */
            if( pool.enumeration_counter==null )
            {
                obj = null;
                return;
            }
            
            /* assign object to smart pointer */
            obj = (T)pool.enumeration_counter.obj;
        }


        /************************************************************************
        This function allows to enumerate recycled elements of the  shared  pool.
        It stores pointer to the next recycled object in the smart pointer.

        IMPORTANT:
        * in case target variable owns non-NULL value, it is rewritten
        * recycled object IS NOT removed from pool
        * target pointer DOES NOT become owner of the new value
        * this function IS NOT thread-safe
        * you SHOULD NOT modify shared pool during enumeration (although you  can
          modify state of the objects retrieved from pool)
        * in case there is no recycled objects left in the pool, NULL is stored.
        * in case pool is not seeded, NULL is stored.

        pool                pool
        obj                 target variable
        ************************************************************************/
        public static void ae_shared_pool_next_recycled<T>(shared_pool pool, ref T obj) where T : alglib.apobject
        {   
            /* exit on end of list */
            if( pool.enumeration_counter==null )
            {
                obj = null;
                return;
            }
            
            /* modify internal enumeration counter */
            pool.enumeration_counter = pool.enumeration_counter.next_entry;
            
            /* exit on empty list */
            if( pool.enumeration_counter==null )
            {
                obj = null;
                return;
            }
            
            /* assign object to smart pointer */
            obj = (T)pool.enumeration_counter.obj;
        }


        /************************************************************************
        This function clears internal list of recycled objects and  seed  object.
        However, pool still can be used (after initialization with another seed).

        pool                pool
        state               ALGLIB environment state

        NOTE: this function is NOT thread-safe. It does not acquire pool lock, so
              you should NOT call it when lock can be used by another thread.
        ************************************************************************/
        public static void ae_shared_pool_reset(shared_pool pool)
        {   
            pool.seed_object = null;
            pool.recycled_objects = null;
            pool.enumeration_counter = null;
        }
    }
}
public partial class alglib
{
    public partial class smp
    {
        public static int cores_count = 1;
        public static volatile int cores_to_use = 1;
        public static bool isparallelcontext()
        {
            return false;
        }
    }
    public class smpselftests
    {
        public static bool runtests()
        {
            return true;
        }
    }
    public static void setnworkers(int nworkers)
    {
        alglib.smp.cores_to_use = nworkers;
    }
    public static int getnworkers()
    {
        return alglib.smp.cores_to_use;
    }
}
