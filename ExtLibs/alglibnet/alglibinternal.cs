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



}
public partial class alglib
{
    public class scodes
    {
        public static int getrdfserializationcode(alglib.xparams _params)
        {
            int result = 0;

            result = 1;
            return result;
        }


        public static int getkdtreeserializationcode(alglib.xparams _params)
        {
            int result = 0;

            result = 2;
            return result;
        }


        public static int getmlpserializationcode(alglib.xparams _params)
        {
            int result = 0;

            result = 3;
            return result;
        }


        public static int getmlpeserializationcode(alglib.xparams _params)
        {
            int result = 0;

            result = 4;
            return result;
        }


        public static int getrbfserializationcode(alglib.xparams _params)
        {
            int result = 0;

            result = 5;
            return result;
        }


        public static int getspline2dserializationcode(alglib.xparams _params)
        {
            int result = 0;

            result = 6;
            return result;
        }


    }
    public class apserv
    {
        /*************************************************************************
        Buffers for internal functions which need buffers:
        * check for size of the buffer you want to use.
        * if buffer is too small, resize it; leave unchanged, if it is larger than
          needed.
        * use it.

        We can pass this structure to multiple functions;  after first run through
        functions buffer sizes will be finally determined,  and  on  a next run no
        allocation will be required.
        *************************************************************************/
        public class apbuffers : apobject
        {
            public bool[] ba0;
            public int[] ia0;
            public int[] ia1;
            public int[] ia2;
            public int[] ia3;
            public double[] ra0;
            public double[] ra1;
            public double[] ra2;
            public double[] ra3;
            public double[,] rm0;
            public double[,] rm1;
            public apbuffers()
            {
                init();
            }
            public override void init()
            {
                ba0 = new bool[0];
                ia0 = new int[0];
                ia1 = new int[0];
                ia2 = new int[0];
                ia3 = new int[0];
                ra0 = new double[0];
                ra1 = new double[0];
                ra2 = new double[0];
                ra3 = new double[0];
                rm0 = new double[0,0];
                rm1 = new double[0,0];
            }
            public override alglib.apobject make_copy()
            {
                apbuffers _result = new apbuffers();
                _result.ba0 = (bool[])ba0.Clone();
                _result.ia0 = (int[])ia0.Clone();
                _result.ia1 = (int[])ia1.Clone();
                _result.ia2 = (int[])ia2.Clone();
                _result.ia3 = (int[])ia3.Clone();
                _result.ra0 = (double[])ra0.Clone();
                _result.ra1 = (double[])ra1.Clone();
                _result.ra2 = (double[])ra2.Clone();
                _result.ra3 = (double[])ra3.Clone();
                _result.rm0 = (double[,])rm0.Clone();
                _result.rm1 = (double[,])rm1.Clone();
                return _result;
            }
        };


        /*************************************************************************
        Structure which is used to workaround limitations of ALGLIB parallellization
        environment.

          -- ALGLIB --
             Copyright 12.04.2009 by Bochkanov Sergey
        *************************************************************************/
        public class sboolean : apobject
        {
            public bool val;
            public sboolean()
            {
                init();
            }
            public override void init()
            {
            }
            public override alglib.apobject make_copy()
            {
                sboolean _result = new sboolean();
                _result.val = val;
                return _result;
            }
        };


        /*************************************************************************
        Structure which is used to workaround limitations of ALGLIB parallellization
        environment.

          -- ALGLIB --
             Copyright 12.04.2009 by Bochkanov Sergey
        *************************************************************************/
        public class sbooleanarray : apobject
        {
            public bool[] val;
            public sbooleanarray()
            {
                init();
            }
            public override void init()
            {
                val = new bool[0];
            }
            public override alglib.apobject make_copy()
            {
                sbooleanarray _result = new sbooleanarray();
                _result.val = (bool[])val.Clone();
                return _result;
            }
        };


        /*************************************************************************
        Structure which is used to workaround limitations of ALGLIB parallellization
        environment.

          -- ALGLIB --
             Copyright 12.04.2009 by Bochkanov Sergey
        *************************************************************************/
        public class sinteger : apobject
        {
            public int val;
            public sinteger()
            {
                init();
            }
            public override void init()
            {
            }
            public override alglib.apobject make_copy()
            {
                sinteger _result = new sinteger();
                _result.val = val;
                return _result;
            }
        };


        /*************************************************************************
        Structure which is used to workaround limitations of ALGLIB parallellization
        environment.

          -- ALGLIB --
             Copyright 12.04.2009 by Bochkanov Sergey
        *************************************************************************/
        public class sintegerarray : apobject
        {
            public int[] val;
            public sintegerarray()
            {
                init();
            }
            public override void init()
            {
                val = new int[0];
            }
            public override alglib.apobject make_copy()
            {
                sintegerarray _result = new sintegerarray();
                _result.val = (int[])val.Clone();
                return _result;
            }
        };


        /*************************************************************************
        Structure which is used to workaround limitations of ALGLIB parallellization
        environment.

          -- ALGLIB --
             Copyright 12.04.2009 by Bochkanov Sergey
        *************************************************************************/
        public class sreal : apobject
        {
            public double val;
            public sreal()
            {
                init();
            }
            public override void init()
            {
            }
            public override alglib.apobject make_copy()
            {
                sreal _result = new sreal();
                _result.val = val;
                return _result;
            }
        };


        /*************************************************************************
        Structure which is used to workaround limitations of ALGLIB parallellization
        environment.

          -- ALGLIB --
             Copyright 12.04.2009 by Bochkanov Sergey
        *************************************************************************/
        public class srealarray : apobject
        {
            public double[] val;
            public srealarray()
            {
                init();
            }
            public override void init()
            {
                val = new double[0];
            }
            public override alglib.apobject make_copy()
            {
                srealarray _result = new srealarray();
                _result.val = (double[])val.Clone();
                return _result;
            }
        };


        /*************************************************************************
        Structure which is used to workaround limitations of ALGLIB parallellization
        environment.

          -- ALGLIB --
             Copyright 12.04.2009 by Bochkanov Sergey
        *************************************************************************/
        public class scomplex : apobject
        {
            public complex val;
            public scomplex()
            {
                init();
            }
            public override void init()
            {
            }
            public override alglib.apobject make_copy()
            {
                scomplex _result = new scomplex();
                _result.val = val;
                return _result;
            }
        };


        /*************************************************************************
        Structure which is used to workaround limitations of ALGLIB parallellization
        environment.

          -- ALGLIB --
             Copyright 12.04.2009 by Bochkanov Sergey
        *************************************************************************/
        public class scomplexarray : apobject
        {
            public complex[] val;
            public scomplexarray()
            {
                init();
            }
            public override void init()
            {
                val = new complex[0];
            }
            public override alglib.apobject make_copy()
            {
                scomplexarray _result = new scomplexarray();
                _result.val = (alglib.complex[])val.Clone();
                return _result;
            }
        };




        /*************************************************************************
        Internally calls SetErrorFlag() with condition:

            Abs(Val-RefVal)>Tol*Max(Abs(RefVal),S)
            
        This function is used to test relative error in Val against  RefVal,  with
        relative error being replaced by absolute when scale  of  RefVal  is  less
        than S.

        This function returns value of COND.
        *************************************************************************/
        public static void seterrorflagdiff(ref bool flag,
            double val,
            double refval,
            double tol,
            double s,
            alglib.xparams _params)
        {
            alglib.ap.seterrorflag(ref flag, (double)(Math.Abs(val-refval))>(double)(tol*Math.Max(Math.Abs(refval), s)), "apserv.ap:162");
        }


        /*************************************************************************
        The function always returns False.
        It may be used sometimes to prevent spurious warnings.

          -- ALGLIB --
             Copyright 17.09.2012 by Bochkanov Sergey
        *************************************************************************/
        public static bool alwaysfalse(alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        The function "touches" integer - it is used  to  avoid  compiler  messages
        about unused variables (in rare cases when we do NOT want to remove  these
        variables).

          -- ALGLIB --
             Copyright 17.09.2012 by Bochkanov Sergey
        *************************************************************************/
        public static void touchint(ref int a,
            alglib.xparams _params)
        {
        }


        /*************************************************************************
        The function "touches" real   -  it is used  to  avoid  compiler  messages
        about unused variables (in rare cases when we do NOT want to remove  these
        variables).

          -- ALGLIB --
             Copyright 17.09.2012 by Bochkanov Sergey
        *************************************************************************/
        public static void touchreal(ref double a,
            alglib.xparams _params)
        {
        }


        /*************************************************************************
        The function performs zero-coalescing on real value.

        NOTE: no check is performed for B<>0

          -- ALGLIB --
             Copyright 18.05.2015 by Bochkanov Sergey
        *************************************************************************/
        public static double coalesce(double a,
            double b,
            alglib.xparams _params)
        {
            double result = 0;

            result = a;
            if( (double)(a)==(double)(0.0) )
            {
                result = b;
            }
            return result;
        }


        /*************************************************************************
        The function performs zero-coalescing on integer value.

        NOTE: no check is performed for B<>0

          -- ALGLIB --
             Copyright 18.05.2015 by Bochkanov Sergey
        *************************************************************************/
        public static int coalescei(int a,
            int b,
            alglib.xparams _params)
        {
            int result = 0;

            result = a;
            if( a==0 )
            {
                result = b;
            }
            return result;
        }


        /*************************************************************************
        The function convert integer value to real value.

          -- ALGLIB --
             Copyright 17.09.2012 by Bochkanov Sergey
        *************************************************************************/
        public static double inttoreal(int a,
            alglib.xparams _params)
        {
            double result = 0;

            result = a;
            return result;
        }


        /*************************************************************************
        The function calculates binary logarithm.

        NOTE: it costs twice as much as Ln(x)

          -- ALGLIB --
             Copyright 17.09.2012 by Bochkanov Sergey
        *************************************************************************/
        public static double logbase2(double x,
            alglib.xparams _params)
        {
            double result = 0;

            result = Math.Log(x)/Math.Log(2);
            return result;
        }


        /*************************************************************************
        This function compares two numbers for approximate equality, with tolerance
        to errors as large as tol.


          -- ALGLIB --
             Copyright 02.12.2009 by Bochkanov Sergey
        *************************************************************************/
        public static bool approxequal(double a,
            double b,
            double tol,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = (double)(Math.Abs(a-b))<=(double)(tol);
            return result;
        }


        /*************************************************************************
        This function compares two numbers for approximate equality, with tolerance
        to errors as large as max(|a|,|b|)*tol.


          -- ALGLIB --
             Copyright 02.12.2009 by Bochkanov Sergey
        *************************************************************************/
        public static bool approxequalrel(double a,
            double b,
            double tol,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = (double)(Math.Abs(a-b))<=(double)(Math.Max(Math.Abs(a), Math.Abs(b))*tol);
            return result;
        }


        /*************************************************************************
        This  function  generates  1-dimensional  general  interpolation task with
        moderate Lipshitz constant (close to 1.0)

        If N=1 then suborutine generates only one point at the middle of [A,B]

          -- ALGLIB --
             Copyright 02.12.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void taskgenint1d(double a,
            double b,
            int n,
            ref double[] x,
            ref double[] y,
            alglib.xparams _params)
        {
            int i = 0;
            double h = 0;

            x = new double[0];
            y = new double[0];

            alglib.ap.assert(n>=1, "TaskGenInterpolationEqdist1D: N<1!");
            x = new double[n];
            y = new double[n];
            if( n>1 )
            {
                x[0] = a;
                y[0] = 2*math.randomreal()-1;
                h = (b-a)/(n-1);
                for(i=1; i<=n-1; i++)
                {
                    if( i!=n-1 )
                    {
                        x[i] = a+(i+0.2*(2*math.randomreal()-1))*h;
                    }
                    else
                    {
                        x[i] = b;
                    }
                    y[i] = y[i-1]+(2*math.randomreal()-1)*(x[i]-x[i-1]);
                }
            }
            else
            {
                x[0] = 0.5*(a+b);
                y[0] = 2*math.randomreal()-1;
            }
        }


        /*************************************************************************
        This function generates  1-dimensional equidistant interpolation task with
        moderate Lipshitz constant (close to 1.0)

        If N=1 then suborutine generates only one point at the middle of [A,B]

          -- ALGLIB --
             Copyright 02.12.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void taskgenint1dequidist(double a,
            double b,
            int n,
            ref double[] x,
            ref double[] y,
            alglib.xparams _params)
        {
            int i = 0;
            double h = 0;

            x = new double[0];
            y = new double[0];

            alglib.ap.assert(n>=1, "TaskGenInterpolationEqdist1D: N<1!");
            x = new double[n];
            y = new double[n];
            if( n>1 )
            {
                x[0] = a;
                y[0] = 2*math.randomreal()-1;
                h = (b-a)/(n-1);
                for(i=1; i<=n-1; i++)
                {
                    x[i] = a+i*h;
                    y[i] = y[i-1]+(2*math.randomreal()-1)*h;
                }
            }
            else
            {
                x[0] = 0.5*(a+b);
                y[0] = 2*math.randomreal()-1;
            }
        }


        /*************************************************************************
        This function generates  1-dimensional Chebyshev-1 interpolation task with
        moderate Lipshitz constant (close to 1.0)

        If N=1 then suborutine generates only one point at the middle of [A,B]

          -- ALGLIB --
             Copyright 02.12.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void taskgenint1dcheb1(double a,
            double b,
            int n,
            ref double[] x,
            ref double[] y,
            alglib.xparams _params)
        {
            int i = 0;

            x = new double[0];
            y = new double[0];

            alglib.ap.assert(n>=1, "TaskGenInterpolation1DCheb1: N<1!");
            x = new double[n];
            y = new double[n];
            if( n>1 )
            {
                for(i=0; i<=n-1; i++)
                {
                    x[i] = 0.5*(b+a)+0.5*(b-a)*Math.Cos(Math.PI*(2*i+1)/(2*n));
                    if( i==0 )
                    {
                        y[i] = 2*math.randomreal()-1;
                    }
                    else
                    {
                        y[i] = y[i-1]+(2*math.randomreal()-1)*(x[i]-x[i-1]);
                    }
                }
            }
            else
            {
                x[0] = 0.5*(a+b);
                y[0] = 2*math.randomreal()-1;
            }
        }


        /*************************************************************************
        This function generates  1-dimensional Chebyshev-2 interpolation task with
        moderate Lipshitz constant (close to 1.0)

        If N=1 then suborutine generates only one point at the middle of [A,B]

          -- ALGLIB --
             Copyright 02.12.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void taskgenint1dcheb2(double a,
            double b,
            int n,
            ref double[] x,
            ref double[] y,
            alglib.xparams _params)
        {
            int i = 0;

            x = new double[0];
            y = new double[0];

            alglib.ap.assert(n>=1, "TaskGenInterpolation1DCheb2: N<1!");
            x = new double[n];
            y = new double[n];
            if( n>1 )
            {
                for(i=0; i<=n-1; i++)
                {
                    x[i] = 0.5*(b+a)+0.5*(b-a)*Math.Cos(Math.PI*i/(n-1));
                    if( i==0 )
                    {
                        y[i] = 2*math.randomreal()-1;
                    }
                    else
                    {
                        y[i] = y[i-1]+(2*math.randomreal()-1)*(x[i]-x[i-1]);
                    }
                }
            }
            else
            {
                x[0] = 0.5*(a+b);
                y[0] = 2*math.randomreal()-1;
            }
        }


        /*************************************************************************
        This function checks that all values from X[] are distinct. It does more
        than just usual floating point comparison:
        * first, it calculates max(X) and min(X)
        * second, it maps X[] from [min,max] to [1,2]
        * only at this stage actual comparison is done

        The meaning of such check is to ensure that all values are "distinct enough"
        and will not cause interpolation subroutine to fail.

        NOTE:
            X[] must be sorted by ascending (subroutine ASSERT's it)

          -- ALGLIB --
             Copyright 02.12.2009 by Bochkanov Sergey
        *************************************************************************/
        public static bool aredistinct(double[] x,
            int n,
            alglib.xparams _params)
        {
            bool result = new bool();
            double a = 0;
            double b = 0;
            int i = 0;
            bool nonsorted = new bool();

            alglib.ap.assert(n>=1, "APSERVAreDistinct: internal error (N<1)");
            if( n==1 )
            {
                
                //
                // everything is alright, it is up to caller to decide whether it
                // can interpolate something with just one point
                //
                result = true;
                return result;
            }
            a = x[0];
            b = x[0];
            nonsorted = false;
            for(i=1; i<=n-1; i++)
            {
                a = Math.Min(a, x[i]);
                b = Math.Max(b, x[i]);
                nonsorted = nonsorted || (double)(x[i-1])>=(double)(x[i]);
            }
            alglib.ap.assert(!nonsorted, "APSERVAreDistinct: internal error (not sorted)");
            for(i=1; i<=n-1; i++)
            {
                if( (double)((x[i]-a)/(b-a)+1)==(double)((x[i-1]-a)/(b-a)+1) )
                {
                    result = false;
                    return result;
                }
            }
            result = true;
            return result;
        }


        /*************************************************************************
        This function checks that two boolean values are the same (both  are  True 
        or both are False).

          -- ALGLIB --
             Copyright 02.12.2009 by Bochkanov Sergey
        *************************************************************************/
        public static bool aresameboolean(bool v1,
            bool v2,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = (v1 && v2) || (!v1 && !v2);
            return result;
        }


        /*************************************************************************
        If Length(X)<N, resizes X

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void bvectorsetlengthatleast(ref bool[] x,
            int n,
            alglib.xparams _params)
        {
            if( alglib.ap.len(x)<n )
            {
                x = new bool[n];
            }
        }


        /*************************************************************************
        If Length(X)<N, resizes X

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void ivectorsetlengthatleast(ref int[] x,
            int n,
            alglib.xparams _params)
        {
            if( alglib.ap.len(x)<n )
            {
                x = new int[n];
            }
        }


        /*************************************************************************
        If Length(X)<N, resizes X

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void rvectorsetlengthatleast(ref double[] x,
            int n,
            alglib.xparams _params)
        {
            if( alglib.ap.len(x)<n )
            {
                x = new double[n];
            }
        }


        /*************************************************************************
        If Cols(X)<N or Rows(X)<M, resizes X

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixsetlengthatleast(ref double[,] x,
            int m,
            int n,
            alglib.xparams _params)
        {
            if( m>0 && n>0 )
            {
                if( alglib.ap.rows(x)<m || alglib.ap.cols(x)<n )
                {
                    x = new double[m, n];
                }
            }
        }


        /*************************************************************************
        Grows X, i.e. changes its size in such a way that:
        a) contents is preserved
        b) new size is at least N
        c) new size can be larger than N, so subsequent grow() calls can return
           without reallocation

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void ivectorgrowto(ref int[] x,
            int n,
            alglib.xparams _params)
        {
            int[] oldx = new int[0];
            int i = 0;
            int n2 = 0;

            
            //
            // Enough place
            //
            if( alglib.ap.len(x)>=n )
            {
                return;
            }
            
            //
            // Choose new size
            //
            n = Math.Max(n, (int)Math.Round(1.8*alglib.ap.len(x)+1));
            
            //
            // Grow
            //
            n2 = alglib.ap.len(x);
            alglib.ap.swap(ref x, ref oldx);
            x = new int[n];
            for(i=0; i<=n-1; i++)
            {
                if( i<n2 )
                {
                    x[i] = oldx[i];
                }
                else
                {
                    x[i] = 0;
                }
            }
        }


        /*************************************************************************
        Grows X, i.e. appends rows in such a way that:
        a) contents is preserved
        b) new row count is at least N
        c) new row count can be larger than N, so subsequent grow() calls can return
           without reallocation
        d) new matrix has at least MinCols columns (if less than specified amount
           of columns is present, new columns are added with undefined contents);
           MinCols can be 0 or negative value = ignored

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixgrowrowsto(ref double[,] a,
            int n,
            int mincols,
            alglib.xparams _params)
        {
            double[,] olda = new double[0,0];
            int i = 0;
            int j = 0;
            int n2 = 0;
            int m = 0;

            
            //
            // Enough place?
            //
            if( alglib.ap.rows(a)>=n && alglib.ap.cols(a)>=mincols )
            {
                return;
            }
            
            //
            // Sizes and metrics
            //
            if( alglib.ap.rows(a)<n )
            {
                n = Math.Max(n, (int)Math.Round(1.8*alglib.ap.rows(a)+1));
            }
            n2 = Math.Min(alglib.ap.rows(a), n);
            m = alglib.ap.cols(a);
            
            //
            // Grow
            //
            alglib.ap.swap(ref a, ref olda);
            a = new double[n, Math.Max(m, mincols)];
            for(i=0; i<=n2-1; i++)
            {
                for(j=0; j<=m-1; j++)
                {
                    a[i,j] = olda[i,j];
                }
            }
        }


        /*************************************************************************
        Grows X, i.e. changes its size in such a way that:
        a) contents is preserved
        b) new size is at least N
        c) new size can be larger than N, so subsequent grow() calls can return
           without reallocation

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void rvectorgrowto(ref double[] x,
            int n,
            alglib.xparams _params)
        {
            double[] oldx = new double[0];
            int i = 0;
            int n2 = 0;

            
            //
            // Enough place
            //
            if( alglib.ap.len(x)>=n )
            {
                return;
            }
            
            //
            // Choose new size
            //
            n = Math.Max(n, (int)Math.Round(1.8*alglib.ap.len(x)+1));
            
            //
            // Grow
            //
            n2 = alglib.ap.len(x);
            alglib.ap.swap(ref x, ref oldx);
            x = new double[n];
            for(i=0; i<=n-1; i++)
            {
                if( i<n2 )
                {
                    x[i] = oldx[i];
                }
                else
                {
                    x[i] = 0;
                }
            }
        }


        /*************************************************************************
        Resizes X and:
        * preserves old contents of X
        * fills new elements by zeros

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void ivectorresize(ref int[] x,
            int n,
            alglib.xparams _params)
        {
            int[] oldx = new int[0];
            int i = 0;
            int n2 = 0;

            n2 = alglib.ap.len(x);
            alglib.ap.swap(ref x, ref oldx);
            x = new int[n];
            for(i=0; i<=n-1; i++)
            {
                if( i<n2 )
                {
                    x[i] = oldx[i];
                }
                else
                {
                    x[i] = 0;
                }
            }
        }


        /*************************************************************************
        Resizes X and:
        * preserves old contents of X
        * fills new elements by zeros

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void rvectorresize(ref double[] x,
            int n,
            alglib.xparams _params)
        {
            double[] oldx = new double[0];
            int i = 0;
            int n2 = 0;

            n2 = alglib.ap.len(x);
            alglib.ap.swap(ref x, ref oldx);
            x = new double[n];
            for(i=0; i<=n-1; i++)
            {
                if( i<n2 )
                {
                    x[i] = oldx[i];
                }
                else
                {
                    x[i] = 0;
                }
            }
        }


        /*************************************************************************
        Resizes X and:
        * preserves old contents of X
        * fills new elements by zeros

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixresize(ref double[,] x,
            int m,
            int n,
            alglib.xparams _params)
        {
            double[,] oldx = new double[0,0];
            int i = 0;
            int j = 0;
            int m2 = 0;
            int n2 = 0;

            m2 = alglib.ap.rows(x);
            n2 = alglib.ap.cols(x);
            alglib.ap.swap(ref x, ref oldx);
            x = new double[m, n];
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( i<m2 && j<n2 )
                    {
                        x[i,j] = oldx[i,j];
                    }
                    else
                    {
                        x[i,j] = 0.0;
                    }
                }
            }
        }


        /*************************************************************************
        Resizes X and:
        * preserves old contents of X
        * fills new elements by zeros

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void imatrixresize(ref int[,] x,
            int m,
            int n,
            alglib.xparams _params)
        {
            int[,] oldx = new int[0,0];
            int i = 0;
            int j = 0;
            int m2 = 0;
            int n2 = 0;

            m2 = alglib.ap.rows(x);
            n2 = alglib.ap.cols(x);
            alglib.ap.swap(ref x, ref oldx);
            x = new int[m, n];
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( i<m2 && j<n2 )
                    {
                        x[i,j] = oldx[i,j];
                    }
                    else
                    {
                        x[i,j] = 0;
                    }
                }
            }
        }


        /*************************************************************************
        appends element to X

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void ivectorappend(ref int[] x,
            int v,
            alglib.xparams _params)
        {
            int[] oldx = new int[0];
            int i = 0;
            int n = 0;

            n = alglib.ap.len(x);
            alglib.ap.swap(ref x, ref oldx);
            x = new int[n+1];
            for(i=0; i<=n-1; i++)
            {
                x[i] = oldx[i];
            }
            x[n] = v;
        }


        /*************************************************************************
        This function checks that length(X) is at least N and first N values  from
        X[] are finite

          -- ALGLIB --
             Copyright 18.06.2010 by Bochkanov Sergey
        *************************************************************************/
        public static bool isfinitevector(double[] x,
            int n,
            alglib.xparams _params)
        {
            bool result = new bool();
            int i = 0;

            alglib.ap.assert(n>=0, "APSERVIsFiniteVector: internal error (N<0)");
            if( n==0 )
            {
                result = true;
                return result;
            }
            if( alglib.ap.len(x)<n )
            {
                result = false;
                return result;
            }
            for(i=0; i<=n-1; i++)
            {
                if( !math.isfinite(x[i]) )
                {
                    result = false;
                    return result;
                }
            }
            result = true;
            return result;
        }


        /*************************************************************************
        This function checks that first N values from X[] are finite

          -- ALGLIB --
             Copyright 18.06.2010 by Bochkanov Sergey
        *************************************************************************/
        public static bool isfinitecvector(complex[] z,
            int n,
            alglib.xparams _params)
        {
            bool result = new bool();
            int i = 0;

            alglib.ap.assert(n>=0, "APSERVIsFiniteCVector: internal error (N<0)");
            for(i=0; i<=n-1; i++)
            {
                if( !math.isfinite(z[i].x) || !math.isfinite(z[i].y) )
                {
                    result = false;
                    return result;
                }
            }
            result = true;
            return result;
        }


        /*************************************************************************
        This function checks that size of X is at least MxN and values from
        X[0..M-1,0..N-1] are finite.

          -- ALGLIB --
             Copyright 18.06.2010 by Bochkanov Sergey
        *************************************************************************/
        public static bool apservisfinitematrix(double[,] x,
            int m,
            int n,
            alglib.xparams _params)
        {
            bool result = new bool();
            int i = 0;
            int j = 0;

            alglib.ap.assert(n>=0, "APSERVIsFiniteMatrix: internal error (N<0)");
            alglib.ap.assert(m>=0, "APSERVIsFiniteMatrix: internal error (M<0)");
            if( m==0 || n==0 )
            {
                result = true;
                return result;
            }
            if( alglib.ap.rows(x)<m || alglib.ap.cols(x)<n )
            {
                result = false;
                return result;
            }
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( !math.isfinite(x[i,j]) )
                    {
                        result = false;
                        return result;
                    }
                }
            }
            result = true;
            return result;
        }


        /*************************************************************************
        This function checks that all values from X[0..M-1,0..N-1] are finite

          -- ALGLIB --
             Copyright 18.06.2010 by Bochkanov Sergey
        *************************************************************************/
        public static bool apservisfinitecmatrix(complex[,] x,
            int m,
            int n,
            alglib.xparams _params)
        {
            bool result = new bool();
            int i = 0;
            int j = 0;

            alglib.ap.assert(n>=0, "APSERVIsFiniteCMatrix: internal error (N<0)");
            alglib.ap.assert(m>=0, "APSERVIsFiniteCMatrix: internal error (M<0)");
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( !math.isfinite(x[i,j].x) || !math.isfinite(x[i,j].y) )
                    {
                        result = false;
                        return result;
                    }
                }
            }
            result = true;
            return result;
        }


        /*************************************************************************
        This function checks that size of X is at least NxN and all values from
        upper/lower triangle of X[0..N-1,0..N-1] are finite

          -- ALGLIB --
             Copyright 18.06.2010 by Bochkanov Sergey
        *************************************************************************/
        public static bool isfinitertrmatrix(double[,] x,
            int n,
            bool isupper,
            alglib.xparams _params)
        {
            bool result = new bool();
            int i = 0;
            int j1 = 0;
            int j2 = 0;
            int j = 0;

            alglib.ap.assert(n>=0, "APSERVIsFiniteRTRMatrix: internal error (N<0)");
            if( n==0 )
            {
                result = true;
                return result;
            }
            if( alglib.ap.rows(x)<n || alglib.ap.cols(x)<n )
            {
                result = false;
                return result;
            }
            for(i=0; i<=n-1; i++)
            {
                if( isupper )
                {
                    j1 = i;
                    j2 = n-1;
                }
                else
                {
                    j1 = 0;
                    j2 = i;
                }
                for(j=j1; j<=j2; j++)
                {
                    if( !math.isfinite(x[i,j]) )
                    {
                        result = false;
                        return result;
                    }
                }
            }
            result = true;
            return result;
        }


        /*************************************************************************
        This function checks that all values from upper/lower triangle of
        X[0..N-1,0..N-1] are finite

          -- ALGLIB --
             Copyright 18.06.2010 by Bochkanov Sergey
        *************************************************************************/
        public static bool apservisfinitectrmatrix(complex[,] x,
            int n,
            bool isupper,
            alglib.xparams _params)
        {
            bool result = new bool();
            int i = 0;
            int j1 = 0;
            int j2 = 0;
            int j = 0;

            alglib.ap.assert(n>=0, "APSERVIsFiniteCTRMatrix: internal error (N<0)");
            for(i=0; i<=n-1; i++)
            {
                if( isupper )
                {
                    j1 = i;
                    j2 = n-1;
                }
                else
                {
                    j1 = 0;
                    j2 = i;
                }
                for(j=j1; j<=j2; j++)
                {
                    if( !math.isfinite(x[i,j].x) || !math.isfinite(x[i,j].y) )
                    {
                        result = false;
                        return result;
                    }
                }
            }
            result = true;
            return result;
        }


        /*************************************************************************
        This function checks that all values from X[0..M-1,0..N-1] are  finite  or
        NaN's.

          -- ALGLIB --
             Copyright 18.06.2010 by Bochkanov Sergey
        *************************************************************************/
        public static bool apservisfiniteornanmatrix(double[,] x,
            int m,
            int n,
            alglib.xparams _params)
        {
            bool result = new bool();
            int i = 0;
            int j = 0;

            alglib.ap.assert(n>=0, "APSERVIsFiniteOrNaNMatrix: internal error (N<0)");
            alglib.ap.assert(m>=0, "APSERVIsFiniteOrNaNMatrix: internal error (M<0)");
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( !(math.isfinite(x[i,j]) || Double.IsNaN(x[i,j])) )
                    {
                        result = false;
                        return result;
                    }
                }
            }
            result = true;
            return result;
        }


        /*************************************************************************
        Safe sqrt(x^2+y^2)

          -- ALGLIB --
             Copyright by Bochkanov Sergey
        *************************************************************************/
        public static double safepythag2(double x,
            double y,
            alglib.xparams _params)
        {
            double result = 0;
            double w = 0;
            double xabs = 0;
            double yabs = 0;
            double z = 0;

            xabs = Math.Abs(x);
            yabs = Math.Abs(y);
            w = Math.Max(xabs, yabs);
            z = Math.Min(xabs, yabs);
            if( (double)(z)==(double)(0) )
            {
                result = w;
            }
            else
            {
                result = w*Math.Sqrt(1+math.sqr(z/w));
            }
            return result;
        }


        /*************************************************************************
        Safe sqrt(x^2+y^2)

          -- ALGLIB --
             Copyright by Bochkanov Sergey
        *************************************************************************/
        public static double safepythag3(double x,
            double y,
            double z,
            alglib.xparams _params)
        {
            double result = 0;
            double w = 0;

            w = Math.Max(Math.Abs(x), Math.Max(Math.Abs(y), Math.Abs(z)));
            if( (double)(w)==(double)(0) )
            {
                result = 0;
                return result;
            }
            x = x/w;
            y = y/w;
            z = z/w;
            result = w*Math.Sqrt(math.sqr(x)+math.sqr(y)+math.sqr(z));
            return result;
        }


        /*************************************************************************
        Safe division.

        This function attempts to calculate R=X/Y without overflow.

        It returns:
        * +1, if abs(X/Y)>=MaxRealNumber or undefined - overflow-like situation
              (no overlfow is generated, R is either NAN, PosINF, NegINF)
        *  0, if MinRealNumber<abs(X/Y)<MaxRealNumber or X=0, Y<>0
              (R contains result, may be zero)
        * -1, if 0<abs(X/Y)<MinRealNumber - underflow-like situation
              (R contains zero; it corresponds to underflow)

        No overflow is generated in any case.

          -- ALGLIB --
             Copyright by Bochkanov Sergey
        *************************************************************************/
        public static int saferdiv(double x,
            double y,
            ref double r,
            alglib.xparams _params)
        {
            int result = 0;

            r = 0;

            
            //
            // Two special cases:
            // * Y=0
            // * X=0 and Y<>0
            //
            if( (double)(y)==(double)(0) )
            {
                result = 1;
                if( (double)(x)==(double)(0) )
                {
                    r = Double.NaN;
                }
                if( (double)(x)>(double)(0) )
                {
                    r = Double.PositiveInfinity;
                }
                if( (double)(x)<(double)(0) )
                {
                    r = Double.NegativeInfinity;
                }
                return result;
            }
            if( (double)(x)==(double)(0) )
            {
                r = 0;
                result = 0;
                return result;
            }
            
            //
            // make Y>0
            //
            if( (double)(y)<(double)(0) )
            {
                x = -x;
                y = -y;
            }
            
            //
            //
            //
            if( (double)(y)>=(double)(1) )
            {
                r = x/y;
                if( (double)(Math.Abs(r))<=(double)(math.minrealnumber) )
                {
                    result = -1;
                    r = 0;
                }
                else
                {
                    result = 0;
                }
            }
            else
            {
                if( (double)(Math.Abs(x))>=(double)(math.maxrealnumber*y) )
                {
                    if( (double)(x)>(double)(0) )
                    {
                        r = Double.PositiveInfinity;
                    }
                    else
                    {
                        r = Double.NegativeInfinity;
                    }
                    result = 1;
                }
                else
                {
                    r = x/y;
                    result = 0;
                }
            }
            return result;
        }


        /*************************************************************************
        This function calculates "safe" min(X/Y,V) for positive finite X, Y, V.
        No overflow is generated in any case.

          -- ALGLIB --
             Copyright by Bochkanov Sergey
        *************************************************************************/
        public static double safeminposrv(double x,
            double y,
            double v,
            alglib.xparams _params)
        {
            double result = 0;
            double r = 0;

            if( (double)(y)>=(double)(1) )
            {
                
                //
                // Y>=1, we can safely divide by Y
                //
                r = x/y;
                result = v;
                if( (double)(v)>(double)(r) )
                {
                    result = r;
                }
                else
                {
                    result = v;
                }
            }
            else
            {
                
                //
                // Y<1, we can safely multiply by Y
                //
                if( (double)(x)<(double)(v*y) )
                {
                    result = x/y;
                }
                else
                {
                    result = v;
                }
            }
            return result;
        }


        /*************************************************************************
        This function makes periodic mapping of X to [A,B].

        It accepts X, A, B (A>B). It returns T which lies in  [A,B] and integer K,
        such that X = T + K*(B-A).

        NOTES:
        * K is represented as real value, although actually it is integer
        * T is guaranteed to be in [A,B]
        * T replaces X

          -- ALGLIB --
             Copyright by Bochkanov Sergey
        *************************************************************************/
        public static void apperiodicmap(ref double x,
            double a,
            double b,
            ref double k,
            alglib.xparams _params)
        {
            k = 0;

            alglib.ap.assert((double)(a)<(double)(b), "APPeriodicMap: internal error!");
            k = (int)Math.Floor((x-a)/(b-a));
            x = x-k*(b-a);
            while( (double)(x)<(double)(a) )
            {
                x = x+(b-a);
                k = k-1;
            }
            while( (double)(x)>(double)(b) )
            {
                x = x-(b-a);
                k = k+1;
            }
            x = Math.Max(x, a);
            x = Math.Min(x, b);
        }


        /*************************************************************************
        Returns random normal number using low-quality system-provided generator

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static double randomnormal(alglib.xparams _params)
        {
            double result = 0;
            double u = 0;
            double v = 0;
            double s = 0;

            while( true )
            {
                u = 2*math.randomreal()-1;
                v = 2*math.randomreal()-1;
                s = math.sqr(u)+math.sqr(v);
                if( (double)(s)>(double)(0) && (double)(s)<(double)(1) )
                {
                    
                    //
                    // two Sqrt's instead of one to
                    // avoid overflow when S is too small
                    //
                    s = Math.Sqrt(-(2*Math.Log(s)))/Math.Sqrt(s);
                    result = u*s;
                    break;
                }
            }
            return result;
        }


        /*************************************************************************
        Generates random unit vector using low-quality system-provided generator.
        Reallocates array if its size is too short.

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void randomunit(int n,
            ref double[] x,
            alglib.xparams _params)
        {
            int i = 0;
            double v = 0;
            double vv = 0;

            alglib.ap.assert(n>0, "RandomUnit: N<=0");
            if( alglib.ap.len(x)<n )
            {
                x = new double[n];
            }
            do
            {
                v = 0.0;
                for(i=0; i<=n-1; i++)
                {
                    vv = randomnormal(_params);
                    x[i] = vv;
                    v = v+vv*vv;
                }
            }
            while( (double)(v)<=(double)(0) );
            v = 1/Math.Sqrt(v);
            for(i=0; i<=n-1; i++)
            {
                x[i] = x[i]*v;
            }
        }


        /*************************************************************************
        This function is used to swap two integer values
        *************************************************************************/
        public static void swapi(ref int v0,
            ref int v1,
            alglib.xparams _params)
        {
            int v = 0;

            v = v0;
            v0 = v1;
            v1 = v;
        }


        /*************************************************************************
        This function is used to swap two real values
        *************************************************************************/
        public static void swapr(ref double v0,
            ref double v1,
            alglib.xparams _params)
        {
            double v = 0;

            v = v0;
            v0 = v1;
            v1 = v;
        }


        /*************************************************************************
        This function is used to swap two rows of the matrix; if NCols<0, automatically
        determined from the matrix size.
        *************************************************************************/
        public static void swaprows(double[,] a,
            int i0,
            int i1,
            int ncols,
            alglib.xparams _params)
        {
            int j = 0;
            double v = 0;

            if( i0==i1 )
            {
                return;
            }
            if( ncols<0 )
            {
                ncols = alglib.ap.cols(a);
            }
            for(j=0; j<=ncols-1; j++)
            {
                v = a[i0,j];
                a[i0,j] = a[i1,j];
                a[i1,j] = v;
            }
        }


        /*************************************************************************
        This function is used to swap two "entries" in 1-dimensional array composed
        from D-element entries
        *************************************************************************/
        public static void swapentries(double[] a,
            int i0,
            int i1,
            int entrywidth,
            alglib.xparams _params)
        {
            int offs0 = 0;
            int offs1 = 0;
            int j = 0;
            double v = 0;

            if( i0==i1 )
            {
                return;
            }
            offs0 = i0*entrywidth;
            offs1 = i1*entrywidth;
            for(j=0; j<=entrywidth-1; j++)
            {
                v = a[offs0+j];
                a[offs0+j] = a[offs1+j];
                a[offs1+j] = v;
            }
        }


        /*************************************************************************
        This function is used to swap two elements of the vector
        *************************************************************************/
        public static void swapelements(double[] a,
            int i0,
            int i1,
            alglib.xparams _params)
        {
            double v = 0;

            if( i0==i1 )
            {
                return;
            }
            v = a[i0];
            a[i0] = a[i1];
            a[i1] = v;
        }


        /*************************************************************************
        This function is used to swap two elements of the vector
        *************************************************************************/
        public static void swapelementsi(int[] a,
            int i0,
            int i1,
            alglib.xparams _params)
        {
            int v = 0;

            if( i0==i1 )
            {
                return;
            }
            v = a[i0];
            a[i0] = a[i1];
            a[i1] = v;
        }


        /*************************************************************************
        This function is used to return maximum of three real values
        *************************************************************************/
        public static double maxreal3(double v0,
            double v1,
            double v2,
            alglib.xparams _params)
        {
            double result = 0;

            result = v0;
            if( (double)(result)<(double)(v1) )
            {
                result = v1;
            }
            if( (double)(result)<(double)(v2) )
            {
                result = v2;
            }
            return result;
        }


        /*************************************************************************
        This function is used to increment value of integer variable
        *************************************************************************/
        public static void inc(ref int v,
            alglib.xparams _params)
        {
            v = v+1;
        }


        /*************************************************************************
        This function is used to decrement value of integer variable
        *************************************************************************/
        public static void dec(ref int v,
            alglib.xparams _params)
        {
            v = v-1;
        }


        /*************************************************************************
        This function is used to increment value of integer variable; name of  the
        function suggests that increment is done in multithreaded setting  in  the
        thread-unsafe manner (optional progress reports which do not need guaranteed
        correctness)
        *************************************************************************/
        public static void threadunsafeinc(ref int v,
            alglib.xparams _params)
        {
            v = v+1;
        }


        /*************************************************************************
        This function performs two operations:
        1. decrements value of integer variable, if it is positive
        2. explicitly sets variable to zero if it is non-positive
        It is used by some algorithms to decrease value of internal counters.
        *************************************************************************/
        public static void countdown(ref int v,
            alglib.xparams _params)
        {
            if( v>0 )
            {
                v = v-1;
            }
            else
            {
                v = 0;
            }
        }


        /*************************************************************************
        This function returns product of two real numbers. It is convenient when
        you have to perform typecast-and-product of two INTEGERS.
        *************************************************************************/
        public static double rmul2(double v0,
            double v1,
            alglib.xparams _params)
        {
            double result = 0;

            result = v0*v1;
            return result;
        }


        /*************************************************************************
        This function returns product of three real numbers. It is convenient when
        you have to perform typecast-and-product of two INTEGERS.
        *************************************************************************/
        public static double rmul3(double v0,
            double v1,
            double v2,
            alglib.xparams _params)
        {
            double result = 0;

            result = v0*v1*v2;
            return result;
        }


        /*************************************************************************
        This function returns (A div B) rounded up; it expects that A>0, B>0, but
        does not check it.
        *************************************************************************/
        public static int idivup(int a,
            int b,
            alglib.xparams _params)
        {
            int result = 0;

            result = a/b;
            if( a%b>0 )
            {
                result = result+1;
            }
            return result;
        }


        /*************************************************************************
        This function returns min(i0,i1)
        *************************************************************************/
        public static int imin2(int i0,
            int i1,
            alglib.xparams _params)
        {
            int result = 0;

            result = i0;
            if( i1<result )
            {
                result = i1;
            }
            return result;
        }


        /*************************************************************************
        This function returns min(i0,i1,i2)
        *************************************************************************/
        public static int imin3(int i0,
            int i1,
            int i2,
            alglib.xparams _params)
        {
            int result = 0;

            result = i0;
            if( i1<result )
            {
                result = i1;
            }
            if( i2<result )
            {
                result = i2;
            }
            return result;
        }


        /*************************************************************************
        This function returns max(i0,i1)
        *************************************************************************/
        public static int imax2(int i0,
            int i1,
            alglib.xparams _params)
        {
            int result = 0;

            result = i0;
            if( i1>result )
            {
                result = i1;
            }
            return result;
        }


        /*************************************************************************
        This function returns max(i0,i1,i2)
        *************************************************************************/
        public static int imax3(int i0,
            int i1,
            int i2,
            alglib.xparams _params)
        {
            int result = 0;

            result = i0;
            if( i1>result )
            {
                result = i1;
            }
            if( i2>result )
            {
                result = i2;
            }
            return result;
        }


        /*************************************************************************
        This function returns max(r0,r1,r2)
        *************************************************************************/
        public static double rmax3(double r0,
            double r1,
            double r2,
            alglib.xparams _params)
        {
            double result = 0;

            result = r0;
            if( (double)(r1)>(double)(result) )
            {
                result = r1;
            }
            if( (double)(r2)>(double)(result) )
            {
                result = r2;
            }
            return result;
        }


        /*************************************************************************
        This function returns max(|r0|,|r1|,|r2|)
        *************************************************************************/
        public static double rmaxabs3(double r0,
            double r1,
            double r2,
            alglib.xparams _params)
        {
            double result = 0;

            r0 = Math.Abs(r0);
            r1 = Math.Abs(r1);
            r2 = Math.Abs(r2);
            result = r0;
            if( (double)(r1)>(double)(result) )
            {
                result = r1;
            }
            if( (double)(r2)>(double)(result) )
            {
                result = r2;
            }
            return result;
        }


        /*************************************************************************
        'bounds' value: maps X to [B1,B2]

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static double boundval(double x,
            double b1,
            double b2,
            alglib.xparams _params)
        {
            double result = 0;

            if( (double)(x)<=(double)(b1) )
            {
                result = b1;
                return result;
            }
            if( (double)(x)>=(double)(b2) )
            {
                result = b2;
                return result;
            }
            result = x;
            return result;
        }


        /*************************************************************************
        'bounds' value: maps X to [B1,B2]

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static int iboundval(int x,
            int b1,
            int b2,
            alglib.xparams _params)
        {
            int result = 0;

            if( x<=b1 )
            {
                result = b1;
                return result;
            }
            if( x>=b2 )
            {
                result = b2;
                return result;
            }
            result = x;
            return result;
        }


        /*************************************************************************
        'bounds' value: maps X to [B1,B2]

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static double rboundval(double x,
            double b1,
            double b2,
            alglib.xparams _params)
        {
            double result = 0;

            if( (double)(x)<=(double)(b1) )
            {
                result = b1;
                return result;
            }
            if( (double)(x)>=(double)(b2) )
            {
                result = b2;
                return result;
            }
            result = x;
            return result;
        }


        /*************************************************************************
        Allocation of serializer: complex value
        *************************************************************************/
        public static void alloccomplex(alglib.serializer s,
            complex v,
            alglib.xparams _params)
        {
            s.alloc_entry();
            s.alloc_entry();
        }


        /*************************************************************************
        Serialization: complex value
        *************************************************************************/
        public static void serializecomplex(alglib.serializer s,
            complex v,
            alglib.xparams _params)
        {
            s.serialize_double(v.x);
            s.serialize_double(v.y);
        }


        /*************************************************************************
        Unserialization: complex value
        *************************************************************************/
        public static complex unserializecomplex(alglib.serializer s,
            alglib.xparams _params)
        {
            complex result = 0;

            result.x = s.unserialize_double();
            result.y = s.unserialize_double();
            return result;
        }


        /*************************************************************************
        Allocation of serializer: real array
        *************************************************************************/
        public static void allocrealarray(alglib.serializer s,
            double[] v,
            int n,
            alglib.xparams _params)
        {
            int i = 0;

            if( n<0 )
            {
                n = alglib.ap.len(v);
            }
            s.alloc_entry();
            for(i=0; i<=n-1; i++)
            {
                s.alloc_entry();
            }
        }


        /*************************************************************************
        Serialization: complex value
        *************************************************************************/
        public static void serializerealarray(alglib.serializer s,
            double[] v,
            int n,
            alglib.xparams _params)
        {
            int i = 0;

            if( n<0 )
            {
                n = alglib.ap.len(v);
            }
            s.serialize_int(n);
            for(i=0; i<=n-1; i++)
            {
                s.serialize_double(v[i]);
            }
        }


        /*************************************************************************
        Unserialization: complex value
        *************************************************************************/
        public static void unserializerealarray(alglib.serializer s,
            ref double[] v,
            alglib.xparams _params)
        {
            int n = 0;
            int i = 0;
            double t = 0;

            v = new double[0];

            n = s.unserialize_int();
            if( n==0 )
            {
                return;
            }
            v = new double[n];
            for(i=0; i<=n-1; i++)
            {
                t = s.unserialize_double();
                v[i] = t;
            }
        }


        /*************************************************************************
        Allocation of serializer: Integer array
        *************************************************************************/
        public static void allocintegerarray(alglib.serializer s,
            int[] v,
            int n,
            alglib.xparams _params)
        {
            int i = 0;

            if( n<0 )
            {
                n = alglib.ap.len(v);
            }
            s.alloc_entry();
            for(i=0; i<=n-1; i++)
            {
                s.alloc_entry();
            }
        }


        /*************************************************************************
        Serialization: Integer array
        *************************************************************************/
        public static void serializeintegerarray(alglib.serializer s,
            int[] v,
            int n,
            alglib.xparams _params)
        {
            int i = 0;

            if( n<0 )
            {
                n = alglib.ap.len(v);
            }
            s.serialize_int(n);
            for(i=0; i<=n-1; i++)
            {
                s.serialize_int(v[i]);
            }
        }


        /*************************************************************************
        Unserialization: complex value
        *************************************************************************/
        public static void unserializeintegerarray(alglib.serializer s,
            ref int[] v,
            alglib.xparams _params)
        {
            int n = 0;
            int i = 0;
            int t = 0;

            v = new int[0];

            n = s.unserialize_int();
            if( n==0 )
            {
                return;
            }
            v = new int[n];
            for(i=0; i<=n-1; i++)
            {
                t = s.unserialize_int();
                v[i] = t;
            }
        }


        /*************************************************************************
        Allocation of serializer: real matrix
        *************************************************************************/
        public static void allocrealmatrix(alglib.serializer s,
            double[,] v,
            int n0,
            int n1,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;

            if( n0<0 )
            {
                n0 = alglib.ap.rows(v);
            }
            if( n1<0 )
            {
                n1 = alglib.ap.cols(v);
            }
            s.alloc_entry();
            s.alloc_entry();
            for(i=0; i<=n0-1; i++)
            {
                for(j=0; j<=n1-1; j++)
                {
                    s.alloc_entry();
                }
            }
        }


        /*************************************************************************
        Serialization: complex value
        *************************************************************************/
        public static void serializerealmatrix(alglib.serializer s,
            double[,] v,
            int n0,
            int n1,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;

            if( n0<0 )
            {
                n0 = alglib.ap.rows(v);
            }
            if( n1<0 )
            {
                n1 = alglib.ap.cols(v);
            }
            s.serialize_int(n0);
            s.serialize_int(n1);
            for(i=0; i<=n0-1; i++)
            {
                for(j=0; j<=n1-1; j++)
                {
                    s.serialize_double(v[i,j]);
                }
            }
        }


        /*************************************************************************
        Unserialization: complex value
        *************************************************************************/
        public static void unserializerealmatrix(alglib.serializer s,
            ref double[,] v,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;
            int n0 = 0;
            int n1 = 0;
            double t = 0;

            v = new double[0,0];

            n0 = s.unserialize_int();
            n1 = s.unserialize_int();
            if( n0==0 || n1==0 )
            {
                return;
            }
            v = new double[n0, n1];
            for(i=0; i<=n0-1; i++)
            {
                for(j=0; j<=n1-1; j++)
                {
                    t = s.unserialize_double();
                    v[i,j] = t;
                }
            }
        }


        /*************************************************************************
        Copy integer array
        *************************************************************************/
        public static void copyintegerarray(int[] src,
            ref int[] dst,
            alglib.xparams _params)
        {
            int i = 0;

            dst = new int[0];

            if( alglib.ap.len(src)>0 )
            {
                dst = new int[alglib.ap.len(src)];
                for(i=0; i<=alglib.ap.len(src)-1; i++)
                {
                    dst[i] = src[i];
                }
            }
        }


        /*************************************************************************
        Copy real array
        *************************************************************************/
        public static void copyrealarray(double[] src,
            ref double[] dst,
            alglib.xparams _params)
        {
            int i = 0;

            dst = new double[0];

            if( alglib.ap.len(src)>0 )
            {
                dst = new double[alglib.ap.len(src)];
                for(i=0; i<=alglib.ap.len(src)-1; i++)
                {
                    dst[i] = src[i];
                }
            }
        }


        /*************************************************************************
        Copy real matrix
        *************************************************************************/
        public static void copyrealmatrix(double[,] src,
            ref double[,] dst,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;

            dst = new double[0,0];

            if( alglib.ap.rows(src)>0 && alglib.ap.cols(src)>0 )
            {
                dst = new double[alglib.ap.rows(src), alglib.ap.cols(src)];
                for(i=0; i<=alglib.ap.rows(src)-1; i++)
                {
                    for(j=0; j<=alglib.ap.cols(src)-1; j++)
                    {
                        dst[i,j] = src[i,j];
                    }
                }
            }
        }


        /*************************************************************************
        Clears integer array
        *************************************************************************/
        public static void unsetintegerarray(ref int[] a,
            alglib.xparams _params)
        {
            a = new int[0];

        }


        /*************************************************************************
        Clears real array
        *************************************************************************/
        public static void unsetrealarray(ref double[] a,
            alglib.xparams _params)
        {
            a = new double[0];

        }


        /*************************************************************************
        Clears real matrix
        *************************************************************************/
        public static void unsetrealmatrix(ref double[,] a,
            alglib.xparams _params)
        {
            a = new double[0,0];

        }


        /*************************************************************************
        This function is used in parallel functions for recurrent division of large
        task into two smaller tasks.

        It has following properties:
        * it works only for TaskSize>=2 and TaskSize>TileSize (assertion is thrown otherwise)
        * Task0+Task1=TaskSize, Task0>0, Task1>0
        * Task0 and Task1 are close to each other
        * Task0>=Task1
        * Task0 is always divisible by TileSize

          -- ALGLIB --
             Copyright 07.04.2013 by Bochkanov Sergey
        *************************************************************************/
        public static void tiledsplit(int tasksize,
            int tilesize,
            ref int task0,
            ref int task1,
            alglib.xparams _params)
        {
            int cc = 0;

            task0 = 0;
            task1 = 0;

            alglib.ap.assert(tasksize>=2, "TiledSplit: TaskSize<2");
            alglib.ap.assert(tasksize>tilesize, "TiledSplit: TaskSize<=TileSize");
            cc = chunkscount(tasksize, tilesize, _params);
            alglib.ap.assert(cc>=2, "TiledSplit: integrity check failed");
            task0 = idivup(cc, 2, _params)*tilesize;
            task1 = tasksize-task0;
            alglib.ap.assert(task0>=1, "TiledSplit: internal error");
            alglib.ap.assert(task1>=1, "TiledSplit: internal error");
            alglib.ap.assert(task0%tilesize==0, "TiledSplit: internal error");
            alglib.ap.assert(task0>=task1, "TiledSplit: internal error");
        }


        /*************************************************************************
        This function searches integer array. Elements in this array are actually
        records, each NRec elements wide. Each record has unique header - NHeader
        integer values, which identify it. Records are lexicographically sorted by
        header.

        Records are identified by their index, not offset (offset = NRec*index).

        This function searches A (records with indices [I0,I1)) for a record with
        header B. It returns index of this record (not offset!), or -1 on failure.

          -- ALGLIB --
             Copyright 28.03.2011 by Bochkanov Sergey
        *************************************************************************/
        public static int recsearch(ref int[] a,
            int nrec,
            int nheader,
            int i0,
            int i1,
            int[] b,
            alglib.xparams _params)
        {
            int result = 0;
            int mididx = 0;
            int cflag = 0;
            int k = 0;
            int offs = 0;

            result = -1;
            while( true )
            {
                if( i0>=i1 )
                {
                    break;
                }
                mididx = (i0+i1)/2;
                offs = nrec*mididx;
                cflag = 0;
                for(k=0; k<=nheader-1; k++)
                {
                    if( a[offs+k]<b[k] )
                    {
                        cflag = -1;
                        break;
                    }
                    if( a[offs+k]>b[k] )
                    {
                        cflag = 1;
                        break;
                    }
                }
                if( cflag==0 )
                {
                    result = mididx;
                    return result;
                }
                if( cflag<0 )
                {
                    i0 = mididx+1;
                }
                else
                {
                    i1 = mididx;
                }
            }
            return result;
        }


        /*************************************************************************
        This function is used in parallel functions for recurrent division of large
        task into two smaller tasks.

        It has following properties:
        * it works only for TaskSize>=2 (assertion is thrown otherwise)
        * for TaskSize=2, it returns Task0=1, Task1=1
        * in case TaskSize is odd,  Task0=TaskSize-1, Task1=1
        * in case TaskSize is even, Task0 and Task1 are approximately TaskSize/2
          and both Task0 and Task1 are even, Task0>=Task1

          -- ALGLIB --
             Copyright 07.04.2013 by Bochkanov Sergey
        *************************************************************************/
        public static void splitlengtheven(int tasksize,
            ref int task0,
            ref int task1,
            alglib.xparams _params)
        {
            task0 = 0;
            task1 = 0;

            alglib.ap.assert(tasksize>=2, "SplitLengthEven: TaskSize<2");
            if( tasksize==2 )
            {
                task0 = 1;
                task1 = 1;
                return;
            }
            if( tasksize%2==0 )
            {
                
                //
                // Even division
                //
                task0 = tasksize/2;
                task1 = tasksize/2;
                if( task0%2!=0 )
                {
                    task0 = task0+1;
                    task1 = task1-1;
                }
            }
            else
            {
                
                //
                // Odd task size, split trailing odd part from it.
                //
                task0 = tasksize-1;
                task1 = 1;
            }
            alglib.ap.assert(task0>=1, "SplitLengthEven: internal error");
            alglib.ap.assert(task1>=1, "SplitLengthEven: internal error");
        }


        /*************************************************************************
        This function is used to calculate number of chunks (including partial,
        non-complete chunks) in some set. It expects that ChunkSize>=1, TaskSize>=0.
        Assertion is thrown otherwise.

        Function result is equivalent to Ceil(TaskSize/ChunkSize), but with guarantees
        that rounding errors won't ruin results.

          -- ALGLIB --
             Copyright 21.01.2015 by Bochkanov Sergey
        *************************************************************************/
        public static int chunkscount(int tasksize,
            int chunksize,
            alglib.xparams _params)
        {
            int result = 0;

            alglib.ap.assert(tasksize>=0, "ChunksCount: TaskSize<0");
            alglib.ap.assert(chunksize>=1, "ChunksCount: ChunkSize<1");
            result = tasksize/chunksize;
            if( tasksize%chunksize!=0 )
            {
                result = result+1;
            }
            return result;
        }


        /*************************************************************************
        Returns A-tile size for a matrix.

        A-tiles are smallest tiles (32x32), suitable for processing by ALGLIB  own
        implementation of Level 3 linear algebra.

          -- ALGLIB routine --
             10.01.2019
             Bochkanov Sergey
        *************************************************************************/
        public static int matrixtilesizea(alglib.xparams _params)
        {
            int result = 0;

            result = 32;
            return result;
        }


        /*************************************************************************
        Returns B-tile size for a matrix.

        B-tiles are larger  tiles (64x64), suitable for parallel execution or for
        processing by vendor's implementation of Level 3 linear algebra.

          -- ALGLIB routine --
             10.01.2019
             Bochkanov Sergey
        *************************************************************************/
        public static int matrixtilesizeb(alglib.xparams _params)
        {
            int result = 0;

            result = 64;
            return result;
        }


        /*************************************************************************
        This function returns minimum cost of task which is feasible for
        multithreaded processing. It returns real number in order to avoid overflow
        problems.

          -- ALGLIB --
             Copyright 10.01.2018 by Bochkanov Sergey
        *************************************************************************/
        public static double smpactivationlevel(alglib.xparams _params)
        {
            double result = 0;
            double nn = 0;

            nn = 2*matrixtilesizeb(_params);
            result = Math.Max(0.95*2*nn*nn*nn, 1.0E7);
            return result;
        }


        /*************************************************************************
        This function returns minimum cost of task which is feasible for
        spawn (given that multithreading is active).

        It returns real number in order to avoid overflow problems.

          -- ALGLIB --
             Copyright 10.01.2018 by Bochkanov Sergey
        *************************************************************************/
        public static double spawnlevel(alglib.xparams _params)
        {
            double result = 0;
            double nn = 0;

            nn = 2*matrixtilesizea(_params);
            result = 0.95*2*nn*nn*nn;
            return result;
        }


        /*************************************************************************
        --- OBSOLETE FUNCTION, USE TILED SPLIT INSTEAD --- 

        This function is used in parallel functions for recurrent division of large
        task into two smaller tasks.

        It has following properties:
        * it works only for TaskSize>=2 and ChunkSize>=2
          (assertion is thrown otherwise)
        * Task0+Task1=TaskSize, Task0>0, Task1>0
        * Task0 and Task1 are close to each other
        * in case TaskSize>ChunkSize, Task0 is always divisible by ChunkSize

          -- ALGLIB --
             Copyright 07.04.2013 by Bochkanov Sergey
        *************************************************************************/
        public static void splitlength(int tasksize,
            int chunksize,
            ref int task0,
            ref int task1,
            alglib.xparams _params)
        {
            task0 = 0;
            task1 = 0;

            alglib.ap.assert(chunksize>=2, "SplitLength: ChunkSize<2");
            alglib.ap.assert(tasksize>=2, "SplitLength: TaskSize<2");
            task0 = tasksize/2;
            if( task0>chunksize && task0%chunksize!=0 )
            {
                task0 = task0-task0%chunksize;
            }
            task1 = tasksize-task0;
            alglib.ap.assert(task0>=1, "SplitLength: internal error");
            alglib.ap.assert(task1>=1, "SplitLength: internal error");
        }


    }
    public class tsort
    {
        /*************************************************************************
        This function sorts array of real keys by ascending.

        Its results are:
        * sorted array A
        * permutation tables P1, P2

        Algorithm outputs permutation tables using two formats:
        * as usual permutation of [0..N-1]. If P1[i]=j, then sorted A[i]  contains
          value which was moved there from J-th position.
        * as a sequence of pairwise permutations. Sorted A[] may  be  obtained  by
          swaping A[i] and A[P2[i]] for all i from 0 to N-1.
          
        INPUT PARAMETERS:
            A       -   unsorted array
            N       -   array size

        OUPUT PARAMETERS:
            A       -   sorted array
            P1, P2  -   permutation tables, array[N]
            
        NOTES:
            this function assumes that A[] is finite; it doesn't checks that
            condition. All other conditions (size of input arrays, etc.) are not
            checked too.

          -- ALGLIB --
             Copyright 14.05.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void tagsort(ref double[] a,
            int n,
            ref int[] p1,
            ref int[] p2,
            alglib.xparams _params)
        {
            apserv.apbuffers buf = new apserv.apbuffers();

            p1 = new int[0];
            p2 = new int[0];

            tagsortbuf(ref a, n, ref p1, ref p2, buf, _params);
        }


        /*************************************************************************
        Buffered variant of TagSort, which accepts preallocated output arrays as
        well as special structure for buffered allocations. If arrays are too
        short, they are reallocated. If they are large enough, no memory
        allocation is done.

        It is intended to be used in the performance-critical parts of code, where
        additional allocations can lead to severe performance degradation

          -- ALGLIB --
             Copyright 14.05.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void tagsortbuf(ref double[] a,
            int n,
            ref int[] p1,
            ref int[] p2,
            apserv.apbuffers buf,
            alglib.xparams _params)
        {
            int i = 0;
            int lv = 0;
            int lp = 0;
            int rv = 0;
            int rp = 0;

            
            //
            // Special cases
            //
            if( n<=0 )
            {
                return;
            }
            if( n==1 )
            {
                apserv.ivectorsetlengthatleast(ref p1, 1, _params);
                apserv.ivectorsetlengthatleast(ref p2, 1, _params);
                p1[0] = 0;
                p2[0] = 0;
                return;
            }
            
            //
            // General case, N>1: prepare permutations table P1
            //
            apserv.ivectorsetlengthatleast(ref p1, n, _params);
            for(i=0; i<=n-1; i++)
            {
                p1[i] = i;
            }
            
            //
            // General case, N>1: sort, update P1
            //
            apserv.rvectorsetlengthatleast(ref buf.ra0, n, _params);
            apserv.ivectorsetlengthatleast(ref buf.ia0, n, _params);
            tagsortfasti(ref a, ref p1, ref buf.ra0, ref buf.ia0, n, _params);
            
            //
            // General case, N>1: fill permutations table P2
            //
            // To fill P2 we maintain two arrays:
            // * PV (Buf.IA0), Position(Value). PV[i] contains position of I-th key at the moment
            // * VP (Buf.IA1), Value(Position). VP[i] contains key which has position I at the moment
            //
            // At each step we making permutation of two items:
            //   Left, which is given by position/value pair LP/LV
            //   and Right, which is given by RP/RV
            // and updating PV[] and VP[] correspondingly.
            //
            apserv.ivectorsetlengthatleast(ref buf.ia0, n, _params);
            apserv.ivectorsetlengthatleast(ref buf.ia1, n, _params);
            apserv.ivectorsetlengthatleast(ref p2, n, _params);
            for(i=0; i<=n-1; i++)
            {
                buf.ia0[i] = i;
                buf.ia1[i] = i;
            }
            for(i=0; i<=n-1; i++)
            {
                
                //
                // calculate LP, LV, RP, RV
                //
                lp = i;
                lv = buf.ia1[lp];
                rv = p1[i];
                rp = buf.ia0[rv];
                
                //
                // Fill P2
                //
                p2[i] = rp;
                
                //
                // update PV and VP
                //
                buf.ia1[lp] = rv;
                buf.ia1[rp] = lv;
                buf.ia0[lv] = rp;
                buf.ia0[rv] = lp;
            }
        }


        /*************************************************************************
        Same as TagSort, but optimized for real keys and integer labels.

        A is sorted, and same permutations are applied to B.

        NOTES:
        1.  this function assumes that A[] is finite; it doesn't checks that
            condition. All other conditions (size of input arrays, etc.) are not
            checked too.
        2.  this function uses two buffers, BufA and BufB, each is N elements large.
            They may be preallocated (which will save some time) or not, in which
            case function will automatically allocate memory.

          -- ALGLIB --
             Copyright 11.12.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void tagsortfasti(ref double[] a,
            ref int[] b,
            ref double[] bufa,
            ref int[] bufb,
            int n,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;
            bool isascending = new bool();
            bool isdescending = new bool();
            double tmpr = 0;
            int tmpi = 0;

            
            //
            // Special case
            //
            if( n<=1 )
            {
                return;
            }
            
            //
            // Test for already sorted set
            //
            isascending = true;
            isdescending = true;
            for(i=1; i<=n-1; i++)
            {
                isascending = isascending && a[i]>=a[i-1];
                isdescending = isdescending && a[i]<=a[i-1];
            }
            if( isascending )
            {
                return;
            }
            if( isdescending )
            {
                for(i=0; i<=n-1; i++)
                {
                    j = n-1-i;
                    if( j<=i )
                    {
                        break;
                    }
                    tmpr = a[i];
                    a[i] = a[j];
                    a[j] = tmpr;
                    tmpi = b[i];
                    b[i] = b[j];
                    b[j] = tmpi;
                }
                return;
            }
            
            //
            // General case
            //
            if( alglib.ap.len(bufa)<n )
            {
                bufa = new double[n];
            }
            if( alglib.ap.len(bufb)<n )
            {
                bufb = new int[n];
            }
            tagsortfastirec(ref a, ref b, ref bufa, ref bufb, 0, n-1, _params);
        }


        /*************************************************************************
        Same as TagSort, but optimized for real keys and real labels.

        A is sorted, and same permutations are applied to B.

        NOTES:
        1.  this function assumes that A[] is finite; it doesn't checks that
            condition. All other conditions (size of input arrays, etc.) are not
            checked too.
        2.  this function uses two buffers, BufA and BufB, each is N elements large.
            They may be preallocated (which will save some time) or not, in which
            case function will automatically allocate memory.

          -- ALGLIB --
             Copyright 11.12.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void tagsortfastr(ref double[] a,
            ref double[] b,
            ref double[] bufa,
            ref double[] bufb,
            int n,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;
            bool isascending = new bool();
            bool isdescending = new bool();
            double tmpr = 0;

            
            //
            // Special case
            //
            if( n<=1 )
            {
                return;
            }
            
            //
            // Test for already sorted set
            //
            isascending = true;
            isdescending = true;
            for(i=1; i<=n-1; i++)
            {
                isascending = isascending && a[i]>=a[i-1];
                isdescending = isdescending && a[i]<=a[i-1];
            }
            if( isascending )
            {
                return;
            }
            if( isdescending )
            {
                for(i=0; i<=n-1; i++)
                {
                    j = n-1-i;
                    if( j<=i )
                    {
                        break;
                    }
                    tmpr = a[i];
                    a[i] = a[j];
                    a[j] = tmpr;
                    tmpr = b[i];
                    b[i] = b[j];
                    b[j] = tmpr;
                }
                return;
            }
            
            //
            // General case
            //
            if( alglib.ap.len(bufa)<n )
            {
                bufa = new double[n];
            }
            if( alglib.ap.len(bufb)<n )
            {
                bufb = new double[n];
            }
            tagsortfastrrec(ref a, ref b, ref bufa, ref bufb, 0, n-1, _params);
        }


        /*************************************************************************
        Same as TagSort, but optimized for real keys without labels.

        A is sorted, and that's all.

        NOTES:
        1.  this function assumes that A[] is finite; it doesn't checks that
            condition. All other conditions (size of input arrays, etc.) are not
            checked too.
        2.  this function uses buffer, BufA, which is N elements large. It may be
            preallocated (which will save some time) or not, in which case
            function will automatically allocate memory.

          -- ALGLIB --
             Copyright 11.12.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void tagsortfast(ref double[] a,
            ref double[] bufa,
            int n,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;
            bool isascending = new bool();
            bool isdescending = new bool();
            double tmpr = 0;

            
            //
            // Special case
            //
            if( n<=1 )
            {
                return;
            }
            
            //
            // Test for already sorted set
            //
            isascending = true;
            isdescending = true;
            for(i=1; i<=n-1; i++)
            {
                isascending = isascending && a[i]>=a[i-1];
                isdescending = isdescending && a[i]<=a[i-1];
            }
            if( isascending )
            {
                return;
            }
            if( isdescending )
            {
                for(i=0; i<=n-1; i++)
                {
                    j = n-1-i;
                    if( j<=i )
                    {
                        break;
                    }
                    tmpr = a[i];
                    a[i] = a[j];
                    a[j] = tmpr;
                }
                return;
            }
            
            //
            // General case
            //
            if( alglib.ap.len(bufa)<n )
            {
                bufa = new double[n];
            }
            tagsortfastrec(ref a, ref bufa, 0, n-1, _params);
        }


        /*************************************************************************
        Sorting function optimized for integer keys and real labels, can be used
        to sort middle of the array

        A is sorted, and same permutations are applied to B.

        NOTES:
            this function assumes that A[] is finite; it doesn't checks that
            condition. All other conditions (size of input arrays, etc.) are not
            checked too.

          -- ALGLIB --
             Copyright 11.12.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void tagsortmiddleir(ref int[] a,
            ref double[] b,
            int offset,
            int n,
            alglib.xparams _params)
        {
            int i = 0;
            int k = 0;
            int t = 0;
            int tmp = 0;
            double tmpr = 0;

            
            //
            // Special cases
            //
            if( n<=1 )
            {
                return;
            }
            
            //
            // General case, N>1: sort, update B
            //
            i = 2;
            do
            {
                t = i;
                while( t!=1 )
                {
                    k = t/2;
                    if( a[offset+k-1]>=a[offset+t-1] )
                    {
                        t = 1;
                    }
                    else
                    {
                        tmp = a[offset+k-1];
                        a[offset+k-1] = a[offset+t-1];
                        a[offset+t-1] = tmp;
                        tmpr = b[offset+k-1];
                        b[offset+k-1] = b[offset+t-1];
                        b[offset+t-1] = tmpr;
                        t = k;
                    }
                }
                i = i+1;
            }
            while( i<=n );
            i = n-1;
            do
            {
                tmp = a[offset+i];
                a[offset+i] = a[offset+0];
                a[offset+0] = tmp;
                tmpr = b[offset+i];
                b[offset+i] = b[offset+0];
                b[offset+0] = tmpr;
                t = 1;
                while( t!=0 )
                {
                    k = 2*t;
                    if( k>i )
                    {
                        t = 0;
                    }
                    else
                    {
                        if( k<i )
                        {
                            if( a[offset+k]>a[offset+k-1] )
                            {
                                k = k+1;
                            }
                        }
                        if( a[offset+t-1]>=a[offset+k-1] )
                        {
                            t = 0;
                        }
                        else
                        {
                            tmp = a[offset+k-1];
                            a[offset+k-1] = a[offset+t-1];
                            a[offset+t-1] = tmp;
                            tmpr = b[offset+k-1];
                            b[offset+k-1] = b[offset+t-1];
                            b[offset+t-1] = tmpr;
                            t = k;
                        }
                    }
                }
                i = i-1;
            }
            while( i>=1 );
        }


        /*************************************************************************
        Heap operations: adds element to the heap

        PARAMETERS:
            A       -   heap itself, must be at least array[0..N]
            B       -   array of integer tags, which are updated according to
                        permutations in the heap
            N       -   size of the heap (without new element).
                        updated on output
            VA      -   value of the element being added
            VB      -   value of the tag

          -- ALGLIB --
             Copyright 28.02.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void tagheappushi(ref double[] a,
            ref int[] b,
            ref int n,
            double va,
            int vb,
            alglib.xparams _params)
        {
            int j = 0;
            int k = 0;
            double v = 0;

            if( n<0 )
            {
                return;
            }
            
            //
            // N=0 is a special case
            //
            if( n==0 )
            {
                a[0] = va;
                b[0] = vb;
                n = n+1;
                return;
            }
            
            //
            // add current point to the heap
            // (add to the bottom, then move up)
            //
            // we don't write point to the heap
            // until its final position is determined
            // (it allow us to reduce number of array access operations)
            //
            j = n;
            n = n+1;
            while( j>0 )
            {
                k = (j-1)/2;
                v = a[k];
                if( (double)(v)<(double)(va) )
                {
                    
                    //
                    // swap with higher element
                    //
                    a[j] = v;
                    b[j] = b[k];
                    j = k;
                }
                else
                {
                    
                    //
                    // element in its place. terminate.
                    //
                    break;
                }
            }
            a[j] = va;
            b[j] = vb;
        }


        /*************************************************************************
        Heap operations: replaces top element with new element
        (which is moved down)

        PARAMETERS:
            A       -   heap itself, must be at least array[0..N-1]
            B       -   array of integer tags, which are updated according to
                        permutations in the heap
            N       -   size of the heap
            VA      -   value of the element which replaces top element
            VB      -   value of the tag

          -- ALGLIB --
             Copyright 28.02.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void tagheapreplacetopi(ref double[] a,
            ref int[] b,
            int n,
            double va,
            int vb,
            alglib.xparams _params)
        {
            int j = 0;
            int k1 = 0;
            int k2 = 0;
            double v = 0;
            double v1 = 0;
            double v2 = 0;

            if( n<1 )
            {
                return;
            }
            
            //
            // N=1 is a special case
            //
            if( n==1 )
            {
                a[0] = va;
                b[0] = vb;
                return;
            }
            
            //
            // move down through heap:
            // * J  -   current element
            // * K1 -   first child (always exists)
            // * K2 -   second child (may not exists)
            //
            // we don't write point to the heap
            // until its final position is determined
            // (it allow us to reduce number of array access operations)
            //
            j = 0;
            k1 = 1;
            k2 = 2;
            while( k1<n )
            {
                if( k2>=n )
                {
                    
                    //
                    // only one child.
                    //
                    // swap and terminate (because this child
                    // have no siblings due to heap structure)
                    //
                    v = a[k1];
                    if( (double)(v)>(double)(va) )
                    {
                        a[j] = v;
                        b[j] = b[k1];
                        j = k1;
                    }
                    break;
                }
                else
                {
                    
                    //
                    // two childs
                    //
                    v1 = a[k1];
                    v2 = a[k2];
                    if( (double)(v1)>(double)(v2) )
                    {
                        if( (double)(va)<(double)(v1) )
                        {
                            a[j] = v1;
                            b[j] = b[k1];
                            j = k1;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        if( (double)(va)<(double)(v2) )
                        {
                            a[j] = v2;
                            b[j] = b[k2];
                            j = k2;
                        }
                        else
                        {
                            break;
                        }
                    }
                    k1 = 2*j+1;
                    k2 = 2*j+2;
                }
            }
            a[j] = va;
            b[j] = vb;
        }


        /*************************************************************************
        Heap operations: pops top element from the heap

        PARAMETERS:
            A       -   heap itself, must be at least array[0..N-1]
            B       -   array of integer tags, which are updated according to
                        permutations in the heap
            N       -   size of the heap, N>=1

        On output top element is moved to A[N-1], B[N-1], heap is reordered, N is
        decreased by 1.

          -- ALGLIB --
             Copyright 28.02.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void tagheappopi(ref double[] a,
            ref int[] b,
            ref int n,
            alglib.xparams _params)
        {
            double va = 0;
            int vb = 0;

            if( n<1 )
            {
                return;
            }
            
            //
            // N=1 is a special case
            //
            if( n==1 )
            {
                n = 0;
                return;
            }
            
            //
            // swap top element and last element,
            // then reorder heap
            //
            va = a[n-1];
            vb = b[n-1];
            a[n-1] = a[0];
            b[n-1] = b[0];
            n = n-1;
            tagheapreplacetopi(ref a, ref b, n, va, vb, _params);
        }


        /*************************************************************************
        Search first element less than T in sorted array.

        PARAMETERS:
            A - sorted array by ascending from 0 to N-1
            N - number of elements in array
            T - the desired element

        RESULT:
            The very first element's index, which isn't less than T.
        In the case when there aren't such elements, returns N.
        *************************************************************************/
        public static int lowerbound(double[] a,
            int n,
            double t,
            alglib.xparams _params)
        {
            int result = 0;
            int l = 0;
            int half = 0;
            int first = 0;
            int middle = 0;

            l = n;
            first = 0;
            while( l>0 )
            {
                half = l/2;
                middle = first+half;
                if( (double)(a[middle])<(double)(t) )
                {
                    first = middle+1;
                    l = l-half-1;
                }
                else
                {
                    l = half;
                }
            }
            result = first;
            return result;
        }


        /*************************************************************************
        Search first element more than T in sorted array.

        PARAMETERS:
            A - sorted array by ascending from 0 to N-1
            N - number of elements in array
            T - the desired element

            RESULT:
            The very first element's index, which more than T.
        In the case when there aren't such elements, returns N.
        *************************************************************************/
        public static int upperbound(double[] a,
            int n,
            double t,
            alglib.xparams _params)
        {
            int result = 0;
            int l = 0;
            int half = 0;
            int first = 0;
            int middle = 0;

            l = n;
            first = 0;
            while( l>0 )
            {
                half = l/2;
                middle = first+half;
                if( (double)(t)<(double)(a[middle]) )
                {
                    l = half;
                }
                else
                {
                    first = middle+1;
                    l = l-half-1;
                }
            }
            result = first;
            return result;
        }


        /*************************************************************************
        Internal TagSortFastI: sorts A[I1...I2] (both bounds are included),
        applies same permutations to B.

          -- ALGLIB --
             Copyright 06.09.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void tagsortfastirec(ref double[] a,
            ref int[] b,
            ref double[] bufa,
            ref int[] bufb,
            int i1,
            int i2,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int cntless = 0;
            int cnteq = 0;
            int cntgreater = 0;
            double tmpr = 0;
            int tmpi = 0;
            double v0 = 0;
            double v1 = 0;
            double v2 = 0;
            double vp = 0;

            
            //
            // Fast exit
            //
            if( i2<=i1 )
            {
                return;
            }
            
            //
            // Non-recursive sort for small arrays
            //
            if( i2-i1<=16 )
            {
                for(j=i1+1; j<=i2; j++)
                {
                    
                    //
                    // Search elements [I1..J-1] for place to insert Jth element.
                    //
                    // This code stops immediately if we can leave A[J] at J-th position
                    // (all elements have same value of A[J] larger than any of them)
                    //
                    tmpr = a[j];
                    tmpi = j;
                    for(k=j-1; k>=i1; k--)
                    {
                        if( a[k]<=tmpr )
                        {
                            break;
                        }
                        tmpi = k;
                    }
                    k = tmpi;
                    
                    //
                    // Insert Jth element into Kth position
                    //
                    if( k!=j )
                    {
                        tmpr = a[j];
                        tmpi = b[j];
                        for(i=j-1; i>=k; i--)
                        {
                            a[i+1] = a[i];
                            b[i+1] = b[i];
                        }
                        a[k] = tmpr;
                        b[k] = tmpi;
                    }
                }
                return;
            }
            
            //
            // Quicksort: choose pivot
            // Here we assume that I2-I1>=2
            //
            v0 = a[i1];
            v1 = a[i1+(i2-i1)/2];
            v2 = a[i2];
            if( v0>v1 )
            {
                tmpr = v1;
                v1 = v0;
                v0 = tmpr;
            }
            if( v1>v2 )
            {
                tmpr = v2;
                v2 = v1;
                v1 = tmpr;
            }
            if( v0>v1 )
            {
                tmpr = v1;
                v1 = v0;
                v0 = tmpr;
            }
            vp = v1;
            
            //
            // now pass through A/B and:
            // * move elements that are LESS than VP to the left of A/B
            // * move elements that are EQUAL to VP to the right of BufA/BufB (in the reverse order)
            // * move elements that are GREATER than VP to the left of BufA/BufB (in the normal order
            // * move elements from the tail of BufA/BufB to the middle of A/B (restoring normal order)
            // * move elements from the left of BufA/BufB to the end of A/B
            //
            cntless = 0;
            cnteq = 0;
            cntgreater = 0;
            for(i=i1; i<=i2; i++)
            {
                v0 = a[i];
                if( v0<vp )
                {
                    
                    //
                    // LESS
                    //
                    k = i1+cntless;
                    if( i!=k )
                    {
                        a[k] = v0;
                        b[k] = b[i];
                    }
                    cntless = cntless+1;
                    continue;
                }
                if( v0==vp )
                {
                    
                    //
                    // EQUAL
                    //
                    k = i2-cnteq;
                    bufa[k] = v0;
                    bufb[k] = b[i];
                    cnteq = cnteq+1;
                    continue;
                }
                
                //
                // GREATER
                //
                k = i1+cntgreater;
                bufa[k] = v0;
                bufb[k] = b[i];
                cntgreater = cntgreater+1;
            }
            for(i=0; i<=cnteq-1; i++)
            {
                j = i1+cntless+cnteq-1-i;
                k = i2+i-(cnteq-1);
                a[j] = bufa[k];
                b[j] = bufb[k];
            }
            for(i=0; i<=cntgreater-1; i++)
            {
                j = i1+cntless+cnteq+i;
                k = i1+i;
                a[j] = bufa[k];
                b[j] = bufb[k];
            }
            
            //
            // Sort left and right parts of the array (ignoring middle part)
            //
            tagsortfastirec(ref a, ref b, ref bufa, ref bufb, i1, i1+cntless-1, _params);
            tagsortfastirec(ref a, ref b, ref bufa, ref bufb, i1+cntless+cnteq, i2, _params);
        }


        /*************************************************************************
        Internal TagSortFastR: sorts A[I1...I2] (both bounds are included),
        applies same permutations to B.

          -- ALGLIB --
             Copyright 06.09.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void tagsortfastrrec(ref double[] a,
            ref double[] b,
            ref double[] bufa,
            ref double[] bufb,
            int i1,
            int i2,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            double tmpr = 0;
            double tmpr2 = 0;
            int tmpi = 0;
            int cntless = 0;
            int cnteq = 0;
            int cntgreater = 0;
            double v0 = 0;
            double v1 = 0;
            double v2 = 0;
            double vp = 0;

            
            //
            // Fast exit
            //
            if( i2<=i1 )
            {
                return;
            }
            
            //
            // Non-recursive sort for small arrays
            //
            if( i2-i1<=16 )
            {
                for(j=i1+1; j<=i2; j++)
                {
                    
                    //
                    // Search elements [I1..J-1] for place to insert Jth element.
                    //
                    // This code stops immediatly if we can leave A[J] at J-th position
                    // (all elements have same value of A[J] larger than any of them)
                    //
                    tmpr = a[j];
                    tmpi = j;
                    for(k=j-1; k>=i1; k--)
                    {
                        if( a[k]<=tmpr )
                        {
                            break;
                        }
                        tmpi = k;
                    }
                    k = tmpi;
                    
                    //
                    // Insert Jth element into Kth position
                    //
                    if( k!=j )
                    {
                        tmpr = a[j];
                        tmpr2 = b[j];
                        for(i=j-1; i>=k; i--)
                        {
                            a[i+1] = a[i];
                            b[i+1] = b[i];
                        }
                        a[k] = tmpr;
                        b[k] = tmpr2;
                    }
                }
                return;
            }
            
            //
            // Quicksort: choose pivot
            // Here we assume that I2-I1>=16
            //
            v0 = a[i1];
            v1 = a[i1+(i2-i1)/2];
            v2 = a[i2];
            if( v0>v1 )
            {
                tmpr = v1;
                v1 = v0;
                v0 = tmpr;
            }
            if( v1>v2 )
            {
                tmpr = v2;
                v2 = v1;
                v1 = tmpr;
            }
            if( v0>v1 )
            {
                tmpr = v1;
                v1 = v0;
                v0 = tmpr;
            }
            vp = v1;
            
            //
            // now pass through A/B and:
            // * move elements that are LESS than VP to the left of A/B
            // * move elements that are EQUAL to VP to the right of BufA/BufB (in the reverse order)
            // * move elements that are GREATER than VP to the left of BufA/BufB (in the normal order
            // * move elements from the tail of BufA/BufB to the middle of A/B (restoring normal order)
            // * move elements from the left of BufA/BufB to the end of A/B
            //
            cntless = 0;
            cnteq = 0;
            cntgreater = 0;
            for(i=i1; i<=i2; i++)
            {
                v0 = a[i];
                if( v0<vp )
                {
                    
                    //
                    // LESS
                    //
                    k = i1+cntless;
                    if( i!=k )
                    {
                        a[k] = v0;
                        b[k] = b[i];
                    }
                    cntless = cntless+1;
                    continue;
                }
                if( v0==vp )
                {
                    
                    //
                    // EQUAL
                    //
                    k = i2-cnteq;
                    bufa[k] = v0;
                    bufb[k] = b[i];
                    cnteq = cnteq+1;
                    continue;
                }
                
                //
                // GREATER
                //
                k = i1+cntgreater;
                bufa[k] = v0;
                bufb[k] = b[i];
                cntgreater = cntgreater+1;
            }
            for(i=0; i<=cnteq-1; i++)
            {
                j = i1+cntless+cnteq-1-i;
                k = i2+i-(cnteq-1);
                a[j] = bufa[k];
                b[j] = bufb[k];
            }
            for(i=0; i<=cntgreater-1; i++)
            {
                j = i1+cntless+cnteq+i;
                k = i1+i;
                a[j] = bufa[k];
                b[j] = bufb[k];
            }
            
            //
            // Sort left and right parts of the array (ignoring middle part)
            //
            tagsortfastrrec(ref a, ref b, ref bufa, ref bufb, i1, i1+cntless-1, _params);
            tagsortfastrrec(ref a, ref b, ref bufa, ref bufb, i1+cntless+cnteq, i2, _params);
        }


        /*************************************************************************
        Internal TagSortFastI: sorts A[I1...I2] (both bounds are included),
        applies same permutations to B.

          -- ALGLIB --
             Copyright 06.09.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void tagsortfastrec(ref double[] a,
            ref double[] bufa,
            int i1,
            int i2,
            alglib.xparams _params)
        {
            int cntless = 0;
            int cnteq = 0;
            int cntgreater = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            double tmpr = 0;
            int tmpi = 0;
            double v0 = 0;
            double v1 = 0;
            double v2 = 0;
            double vp = 0;

            
            //
            // Fast exit
            //
            if( i2<=i1 )
            {
                return;
            }
            
            //
            // Non-recursive sort for small arrays
            //
            if( i2-i1<=16 )
            {
                for(j=i1+1; j<=i2; j++)
                {
                    
                    //
                    // Search elements [I1..J-1] for place to insert Jth element.
                    //
                    // This code stops immediatly if we can leave A[J] at J-th position
                    // (all elements have same value of A[J] larger than any of them)
                    //
                    tmpr = a[j];
                    tmpi = j;
                    for(k=j-1; k>=i1; k--)
                    {
                        if( a[k]<=tmpr )
                        {
                            break;
                        }
                        tmpi = k;
                    }
                    k = tmpi;
                    
                    //
                    // Insert Jth element into Kth position
                    //
                    if( k!=j )
                    {
                        tmpr = a[j];
                        for(i=j-1; i>=k; i--)
                        {
                            a[i+1] = a[i];
                        }
                        a[k] = tmpr;
                    }
                }
                return;
            }
            
            //
            // Quicksort: choose pivot
            // Here we assume that I2-I1>=16
            //
            v0 = a[i1];
            v1 = a[i1+(i2-i1)/2];
            v2 = a[i2];
            if( v0>v1 )
            {
                tmpr = v1;
                v1 = v0;
                v0 = tmpr;
            }
            if( v1>v2 )
            {
                tmpr = v2;
                v2 = v1;
                v1 = tmpr;
            }
            if( v0>v1 )
            {
                tmpr = v1;
                v1 = v0;
                v0 = tmpr;
            }
            vp = v1;
            
            //
            // now pass through A/B and:
            // * move elements that are LESS than VP to the left of A/B
            // * move elements that are EQUAL to VP to the right of BufA/BufB (in the reverse order)
            // * move elements that are GREATER than VP to the left of BufA/BufB (in the normal order
            // * move elements from the tail of BufA/BufB to the middle of A/B (restoring normal order)
            // * move elements from the left of BufA/BufB to the end of A/B
            //
            cntless = 0;
            cnteq = 0;
            cntgreater = 0;
            for(i=i1; i<=i2; i++)
            {
                v0 = a[i];
                if( v0<vp )
                {
                    
                    //
                    // LESS
                    //
                    k = i1+cntless;
                    if( i!=k )
                    {
                        a[k] = v0;
                    }
                    cntless = cntless+1;
                    continue;
                }
                if( v0==vp )
                {
                    
                    //
                    // EQUAL
                    //
                    k = i2-cnteq;
                    bufa[k] = v0;
                    cnteq = cnteq+1;
                    continue;
                }
                
                //
                // GREATER
                //
                k = i1+cntgreater;
                bufa[k] = v0;
                cntgreater = cntgreater+1;
            }
            for(i=0; i<=cnteq-1; i++)
            {
                j = i1+cntless+cnteq-1-i;
                k = i2+i-(cnteq-1);
                a[j] = bufa[k];
            }
            for(i=0; i<=cntgreater-1; i++)
            {
                j = i1+cntless+cnteq+i;
                k = i1+i;
                a[j] = bufa[k];
            }
            
            //
            // Sort left and right parts of the array (ignoring middle part)
            //
            tagsortfastrec(ref a, ref bufa, i1, i1+cntless-1, _params);
            tagsortfastrec(ref a, ref bufa, i1+cntless+cnteq, i2, _params);
        }


    }
    public class ablasmkl
    {
        /*************************************************************************
        MKL-based kernel

          -- ALGLIB routine --
             12.10.2017
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixgermkl(int m,
            int n,
            double[,] a,
            int ia,
            int ja,
            double alpha,
            double[] u,
            int iu,
            double[] v,
            int iv,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel

          -- ALGLIB routine --
             12.10.2017
             Bochkanov Sergey
        *************************************************************************/
        public static bool cmatrixrank1mkl(int m,
            int n,
            ref complex[,] a,
            int ia,
            int ja,
            ref complex[] u,
            int iu,
            ref complex[] v,
            int iv,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel

          -- ALGLIB routine --
             12.10.2017
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixrank1mkl(int m,
            int n,
            double[,] a,
            int ia,
            int ja,
            double[] u,
            int iu,
            double[] v,
            int iv,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel

          -- ALGLIB routine --
             12.10.2017
             Bochkanov Sergey
        *************************************************************************/
        public static bool cmatrixmvmkl(int m,
            int n,
            complex[,] a,
            int ia,
            int ja,
            int opa,
            complex[] x,
            int ix,
            ref complex[] y,
            int iy,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel

          -- ALGLIB routine --
             12.10.2017
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixmvmkl(int m,
            int n,
            double[,] a,
            int ia,
            int ja,
            int opa,
            double[] x,
            int ix,
            double[] y,
            int iy,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel

          -- ALGLIB routine --
             12.10.2017
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixgemvmkl(int m,
            int n,
            double alpha,
            double[,] a,
            int ia,
            int ja,
            int opa,
            double[] x,
            int ix,
            double beta,
            double[] y,
            int iy,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel

          -- ALGLIB routine --
             12.10.2017
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixtrsvmkl(int n,
            double[,] a,
            int ia,
            int ja,
            bool isupper,
            bool isunit,
            int optype,
            double[] x,
            int ix,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel

          -- ALGLIB routine --
             01.10.2013
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixsyrkmkl(int n,
            int k,
            double alpha,
            double[,] a,
            int ia,
            int ja,
            int optypea,
            double beta,
            double[,] c,
            int ic,
            int jc,
            bool isupper,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel

          -- ALGLIB routine --
             01.10.2013
             Bochkanov Sergey
        *************************************************************************/
        public static bool cmatrixherkmkl(int n,
            int k,
            double alpha,
            complex[,] a,
            int ia,
            int ja,
            int optypea,
            double beta,
            complex[,] c,
            int ic,
            int jc,
            bool isupper,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel

          -- ALGLIB routine --
             01.10.2013
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixgemmmkl(int m,
            int n,
            int k,
            double alpha,
            double[,] a,
            int ia,
            int ja,
            int optypea,
            double[,] b,
            int ib,
            int jb,
            int optypeb,
            double beta,
            double[,] c,
            int ic,
            int jc,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel

          -- ALGLIB routine --
             01.10.2017
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixsymvmkl(int n,
            double alpha,
            double[,] a,
            int ia,
            int ja,
            bool isupper,
            double[] x,
            int ix,
            double beta,
            double[] y,
            int iy,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel

          -- ALGLIB routine --
             16.10.2014
             Bochkanov Sergey
        *************************************************************************/
        public static bool cmatrixgemmmkl(int m,
            int n,
            int k,
            complex alpha,
            complex[,] a,
            int ia,
            int ja,
            int optypea,
            complex[,] b,
            int ib,
            int jb,
            int optypeb,
            complex beta,
            complex[,] c,
            int ic,
            int jc,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel

          -- ALGLIB routine --
             16.10.2014
             Bochkanov Sergey
        *************************************************************************/
        public static bool cmatrixlefttrsmmkl(int m,
            int n,
            complex[,] a,
            int i1,
            int j1,
            bool isupper,
            bool isunit,
            int optype,
            complex[,] x,
            int i2,
            int j2,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel

          -- ALGLIB routine --
             16.10.2014
             Bochkanov Sergey
        *************************************************************************/
        public static bool cmatrixrighttrsmmkl(int m,
            int n,
            complex[,] a,
            int i1,
            int j1,
            bool isupper,
            bool isunit,
            int optype,
            complex[,] x,
            int i2,
            int j2,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel

          -- ALGLIB routine --
             16.10.2014
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixlefttrsmmkl(int m,
            int n,
            double[,] a,
            int i1,
            int j1,
            bool isupper,
            bool isunit,
            int optype,
            double[,] x,
            int i2,
            int j2,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel

          -- ALGLIB routine --
             16.10.2014
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixrighttrsmmkl(int m,
            int n,
            double[,] a,
            int i1,
            int j1,
            bool isupper,
            bool isunit,
            int optype,
            double[,] x,
            int i2,
            int j2,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel.

        NOTE:

        if function returned False, CholResult is NOT modified. Not ever referenced!
        if function returned True, CholResult is set to status of Cholesky decomposition
        (True on succeess).

          -- ALGLIB routine --
             16.10.2014
             Bochkanov Sergey
        *************************************************************************/
        public static bool spdmatrixcholeskymkl(double[,] a,
            int offs,
            int n,
            bool isupper,
            ref bool cholresult,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel.

          -- ALGLIB routine --
             20.10.2014
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixplumkl(ref double[,] a,
            int offs,
            int m,
            int n,
            ref int[] pivots,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel.

        NOTE: this function needs preallocated output/temporary arrays.
              D and E must be at least max(M,N)-wide.

          -- ALGLIB routine --
             20.10.2014
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixbdmkl(double[,] a,
            int m,
            int n,
            double[] d,
            double[] e,
            double[] tauq,
            double[] taup,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel.

        If ByQ is True,  TauP is not used (can be empty array).
        If ByQ is False, TauQ is not used (can be empty array).

          -- ALGLIB routine --
             20.10.2014
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixbdmultiplybymkl(double[,] qp,
            int m,
            int n,
            double[] tauq,
            double[] taup,
            double[,] z,
            int zrows,
            int zcolumns,
            bool byq,
            bool fromtheright,
            bool dotranspose,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel.

        NOTE: Tau must be preallocated array with at least N-1 elements.

          -- ALGLIB routine --
             20.10.2014
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixhessenbergmkl(double[,] a,
            int n,
            double[] tau,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel.

        NOTE: Q must be preallocated N*N array

          -- ALGLIB routine --
             20.10.2014
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixhessenbergunpackqmkl(double[,] a,
            int n,
            double[] tau,
            double[,] q,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel.

        NOTE: Tau, D, E must be preallocated arrays;
              length(E)=length(Tau)=N-1 (or larger)
              length(D)=N (or larger)

          -- ALGLIB routine --
             20.10.2014
             Bochkanov Sergey
        *************************************************************************/
        public static bool smatrixtdmkl(double[,] a,
            int n,
            bool isupper,
            double[] tau,
            double[] d,
            double[] e,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel.

        NOTE: Q must be preallocated N*N array

          -- ALGLIB routine --
             20.10.2014
             Bochkanov Sergey
        *************************************************************************/
        public static bool smatrixtdunpackqmkl(double[,] a,
            int n,
            bool isupper,
            double[] tau,
            double[,] q,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel.

        NOTE: Tau, D, E must be preallocated arrays;
              length(E)=length(Tau)=N-1 (or larger)
              length(D)=N (or larger)

          -- ALGLIB routine --
             20.10.2014
             Bochkanov Sergey
        *************************************************************************/
        public static bool hmatrixtdmkl(complex[,] a,
            int n,
            bool isupper,
            complex[] tau,
            double[] d,
            double[] e,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel.

        NOTE: Q must be preallocated N*N array

          -- ALGLIB routine --
             20.10.2014
             Bochkanov Sergey
        *************************************************************************/
        public static bool hmatrixtdunpackqmkl(complex[,] a,
            int n,
            bool isupper,
            complex[] tau,
            complex[,] q,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel.

        Returns True if MKL was present and handled request (MKL  completion  code
        is returned as separate output parameter).

        D and E are pre-allocated arrays with length N (both of them!). On output,
        D constraints singular values, and E is destroyed.

        SVDResult is modified if and only if MKL is present.

          -- ALGLIB routine --
             20.10.2014
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixbdsvdmkl(double[] d,
            double[] e,
            int n,
            bool isupper,
            double[,] u,
            int nru,
            double[,] c,
            int ncc,
            double[,] vt,
            int ncvt,
            ref bool svdresult,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based DHSEQR kernel.

        Returns True if MKL was present and handled request.

        WR and WI are pre-allocated arrays with length N.
        Z is pre-allocated array[N,N].

          -- ALGLIB routine --
             20.10.2014
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixinternalschurdecompositionmkl(double[,] h,
            int n,
            int tneeded,
            int zneeded,
            double[] wr,
            double[] wi,
            double[,] z,
            ref int info,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based DTREVC kernel.

        Returns True if MKL was present and handled request.

        NOTE: this function does NOT support HOWMNY=3!!!!

        VL and VR are pre-allocated arrays with length N*N, if required. If particalar
        variables is not required, it can be dummy (empty) array.

          -- ALGLIB routine --
             20.10.2014
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixinternaltrevcmkl(double[,] t,
            int n,
            int side,
            int howmny,
            double[,] vl,
            double[,] vr,
            ref int m,
            ref int info,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel.

        Returns True if MKL was present and handled request (MKL  completion  code
        is returned as separate output parameter).

        D and E are pre-allocated arrays with length N (both of them!). On output,
        D constraints eigenvalues, and E is destroyed.

        Z is preallocated array[N,N] for ZNeeded<>0; ignored for ZNeeded=0.

        EVDResult is modified if and only if MKL is present.

          -- ALGLIB routine --
             20.10.2014
             Bochkanov Sergey
        *************************************************************************/
        public static bool smatrixtdevdmkl(double[] d,
            double[] e,
            int n,
            int zneeded,
            double[,] z,
            ref bool evdresult,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        MKL-based kernel.

        Returns True if MKL was present and handled request (MKL  completion  code
        is returned as separate output parameter).

        D and E are pre-allocated arrays with length N (both of them!). On output,
        D constraints eigenvalues, and E is destroyed.

        Z is preallocated array[N,N] for ZNeeded<>0; ignored for ZNeeded=0.

        EVDResult is modified if and only if MKL is present.

          -- ALGLIB routine --
             20.10.2014
             Bochkanov Sergey
        *************************************************************************/
        public static bool sparsegemvcrsmkl(int opa,
            int arows,
            int acols,
            double alpha,
            double[] vals,
            int[] cidx,
            int[] ridx,
            double[] x,
            int ix,
            double beta,
            double[] y,
            int iy,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


    }
    public class ablasf
    {
        /*************************************************************************
        Fast kernel

          -- ALGLIB routine --
             19.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixgerf(int m,
            int n,
            double[,] a,
            int ia,
            int ja,
            double ralpha,
            double[] u,
            int iu,
            double[] v,
            int iv,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        Fast kernel

          -- ALGLIB routine --
             19.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool cmatrixrank1f(int m,
            int n,
            ref complex[,] a,
            int ia,
            int ja,
            ref complex[] u,
            int iu,
            ref complex[] v,
            int iv,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        Fast kernel

          -- ALGLIB routine --
             19.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixrank1f(int m,
            int n,
            ref double[,] a,
            int ia,
            int ja,
            ref double[] u,
            int iu,
            ref double[] v,
            int iv,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        Fast kernel

          -- ALGLIB routine --
             19.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool cmatrixrighttrsmf(int m,
            int n,
            complex[,] a,
            int i1,
            int j1,
            bool isupper,
            bool isunit,
            int optype,
            complex[,] x,
            int i2,
            int j2,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        Fast kernel

          -- ALGLIB routine --
             19.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool cmatrixlefttrsmf(int m,
            int n,
            complex[,] a,
            int i1,
            int j1,
            bool isupper,
            bool isunit,
            int optype,
            complex[,] x,
            int i2,
            int j2,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        Fast kernel

          -- ALGLIB routine --
             19.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixrighttrsmf(int m,
            int n,
            double[,] a,
            int i1,
            int j1,
            bool isupper,
            bool isunit,
            int optype,
            double[,] x,
            int i2,
            int j2,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        Fast kernel

          -- ALGLIB routine --
             19.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixlefttrsmf(int m,
            int n,
            double[,] a,
            int i1,
            int j1,
            bool isupper,
            bool isunit,
            int optype,
            double[,] x,
            int i2,
            int j2,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        Fast kernel

          -- ALGLIB routine --
             19.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool cmatrixherkf(int n,
            int k,
            double alpha,
            complex[,] a,
            int ia,
            int ja,
            int optypea,
            double beta,
            complex[,] c,
            int ic,
            int jc,
            bool isupper,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        Fast kernel

          -- ALGLIB routine --
             19.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixsyrkf(int n,
            int k,
            double alpha,
            double[,] a,
            int ia,
            int ja,
            int optypea,
            double beta,
            double[,] c,
            int ic,
            int jc,
            bool isupper,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        Fast kernel

          -- ALGLIB routine --
             19.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixgemmf(int m,
            int n,
            int k,
            double alpha,
            double[,] a,
            int ia,
            int ja,
            int optypea,
            double[,] b,
            int ib,
            int jb,
            int optypeb,
            double beta,
            double[,] c,
            int ic,
            int jc,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        Fast kernel

          -- ALGLIB routine --
             19.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool cmatrixgemmf(int m,
            int n,
            int k,
            complex alpha,
            complex[,] a,
            int ia,
            int ja,
            int optypea,
            complex[,] b,
            int ib,
            int jb,
            int optypeb,
            complex beta,
            complex[,] c,
            int ic,
            int jc,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        CMatrixGEMM kernel, basecase code for CMatrixGEMM.

        This subroutine calculates C = alpha*op1(A)*op2(B) +beta*C where:
        * C is MxN general matrix
        * op1(A) is MxK matrix
        * op2(B) is KxN matrix
        * "op" may be identity transformation, transposition, conjugate transposition

        Additional info:
        * multiplication result replaces C. If Beta=0, C elements are not used in
          calculations (not multiplied by zero - just not referenced)
        * if Alpha=0, A is not used (not multiplied by zero - just not referenced)
        * if both Beta and Alpha are zero, C is filled by zeros.

        IMPORTANT:

        This function does NOT preallocate output matrix C, it MUST be preallocated
        by caller prior to calling this function. In case C does not have  enough
        space to store result, exception will be generated.

        INPUT PARAMETERS
            M       -   matrix size, M>0
            N       -   matrix size, N>0
            K       -   matrix size, K>0
            Alpha   -   coefficient
            A       -   matrix
            IA      -   submatrix offset
            JA      -   submatrix offset
            OpTypeA -   transformation type:
                        * 0 - no transformation
                        * 1 - transposition
                        * 2 - conjugate transposition
            B       -   matrix
            IB      -   submatrix offset
            JB      -   submatrix offset
            OpTypeB -   transformation type:
                        * 0 - no transformation
                        * 1 - transposition
                        * 2 - conjugate transposition
            Beta    -   coefficient
            C       -   PREALLOCATED output matrix
            IC      -   submatrix offset
            JC      -   submatrix offset

          -- ALGLIB routine --
             27.03.2013
             Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixgemmk(int m,
            int n,
            int k,
            complex alpha,
            complex[,] a,
            int ia,
            int ja,
            int optypea,
            complex[,] b,
            int ib,
            int jb,
            int optypeb,
            complex beta,
            complex[,] c,
            int ic,
            int jc,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;
            complex v = 0;
            complex v00 = 0;
            complex v01 = 0;
            complex v10 = 0;
            complex v11 = 0;
            double v00x = 0;
            double v00y = 0;
            double v01x = 0;
            double v01y = 0;
            double v10x = 0;
            double v10y = 0;
            double v11x = 0;
            double v11y = 0;
            double a0x = 0;
            double a0y = 0;
            double a1x = 0;
            double a1y = 0;
            double b0x = 0;
            double b0y = 0;
            double b1x = 0;
            double b1y = 0;
            int idxa0 = 0;
            int idxa1 = 0;
            int idxb0 = 0;
            int idxb1 = 0;
            int i0 = 0;
            int i1 = 0;
            int ik = 0;
            int j0 = 0;
            int j1 = 0;
            int jk = 0;
            int t = 0;
            int offsa = 0;
            int offsb = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // if matrix size is zero
            //
            if( m==0 || n==0 )
            {
                return;
            }
            
            //
            // Try optimized code
            //
            if( cmatrixgemmf(m, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc, _params) )
            {
                return;
            }
            
            //
            // if K=0 or Alpha=0, then C=Beta*C
            //
            if( k==0 || alpha==0 )
            {
                if( beta!=1 )
                {
                    if( beta!=0 )
                    {
                        for(i=0; i<=m-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                c[ic+i,jc+j] = beta*c[ic+i,jc+j];
                            }
                        }
                    }
                    else
                    {
                        for(i=0; i<=m-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                c[ic+i,jc+j] = 0;
                            }
                        }
                    }
                }
                return;
            }
            
            //
            // This phase is not really necessary, but compiler complains
            // about "possibly uninitialized variables"
            //
            a0x = 0;
            a0y = 0;
            a1x = 0;
            a1y = 0;
            b0x = 0;
            b0y = 0;
            b1x = 0;
            b1y = 0;
            
            //
            // General case
            //
            i = 0;
            while( i<m )
            {
                j = 0;
                while( j<n )
                {
                    
                    //
                    // Choose between specialized 4x4 code and general code
                    //
                    if( i+2<=m && j+2<=n )
                    {
                        
                        //
                        // Specialized 4x4 code for [I..I+3]x[J..J+3] submatrix of C.
                        //
                        // This submatrix is calculated as sum of K rank-1 products,
                        // with operands cached in local variables in order to speed
                        // up operations with arrays.
                        //
                        v00x = 0.0;
                        v00y = 0.0;
                        v01x = 0.0;
                        v01y = 0.0;
                        v10x = 0.0;
                        v10y = 0.0;
                        v11x = 0.0;
                        v11y = 0.0;
                        if( optypea==0 )
                        {
                            idxa0 = ia+i+0;
                            idxa1 = ia+i+1;
                            offsa = ja;
                        }
                        else
                        {
                            idxa0 = ja+i+0;
                            idxa1 = ja+i+1;
                            offsa = ia;
                        }
                        if( optypeb==0 )
                        {
                            idxb0 = jb+j+0;
                            idxb1 = jb+j+1;
                            offsb = ib;
                        }
                        else
                        {
                            idxb0 = ib+j+0;
                            idxb1 = ib+j+1;
                            offsb = jb;
                        }
                        for(t=0; t<=k-1; t++)
                        {
                            if( optypea==0 )
                            {
                                a0x = a[idxa0,offsa].x;
                                a0y = a[idxa0,offsa].y;
                                a1x = a[idxa1,offsa].x;
                                a1y = a[idxa1,offsa].y;
                            }
                            if( optypea==1 )
                            {
                                a0x = a[offsa,idxa0].x;
                                a0y = a[offsa,idxa0].y;
                                a1x = a[offsa,idxa1].x;
                                a1y = a[offsa,idxa1].y;
                            }
                            if( optypea==2 )
                            {
                                a0x = a[offsa,idxa0].x;
                                a0y = -a[offsa,idxa0].y;
                                a1x = a[offsa,idxa1].x;
                                a1y = -a[offsa,idxa1].y;
                            }
                            if( optypeb==0 )
                            {
                                b0x = b[offsb,idxb0].x;
                                b0y = b[offsb,idxb0].y;
                                b1x = b[offsb,idxb1].x;
                                b1y = b[offsb,idxb1].y;
                            }
                            if( optypeb==1 )
                            {
                                b0x = b[idxb0,offsb].x;
                                b0y = b[idxb0,offsb].y;
                                b1x = b[idxb1,offsb].x;
                                b1y = b[idxb1,offsb].y;
                            }
                            if( optypeb==2 )
                            {
                                b0x = b[idxb0,offsb].x;
                                b0y = -b[idxb0,offsb].y;
                                b1x = b[idxb1,offsb].x;
                                b1y = -b[idxb1,offsb].y;
                            }
                            v00x = v00x+a0x*b0x-a0y*b0y;
                            v00y = v00y+a0x*b0y+a0y*b0x;
                            v01x = v01x+a0x*b1x-a0y*b1y;
                            v01y = v01y+a0x*b1y+a0y*b1x;
                            v10x = v10x+a1x*b0x-a1y*b0y;
                            v10y = v10y+a1x*b0y+a1y*b0x;
                            v11x = v11x+a1x*b1x-a1y*b1y;
                            v11y = v11y+a1x*b1y+a1y*b1x;
                            offsa = offsa+1;
                            offsb = offsb+1;
                        }
                        v00.x = v00x;
                        v00.y = v00y;
                        v10.x = v10x;
                        v10.y = v10y;
                        v01.x = v01x;
                        v01.y = v01y;
                        v11.x = v11x;
                        v11.y = v11y;
                        if( beta==0 )
                        {
                            c[ic+i+0,jc+j+0] = alpha*v00;
                            c[ic+i+0,jc+j+1] = alpha*v01;
                            c[ic+i+1,jc+j+0] = alpha*v10;
                            c[ic+i+1,jc+j+1] = alpha*v11;
                        }
                        else
                        {
                            c[ic+i+0,jc+j+0] = beta*c[ic+i+0,jc+j+0]+alpha*v00;
                            c[ic+i+0,jc+j+1] = beta*c[ic+i+0,jc+j+1]+alpha*v01;
                            c[ic+i+1,jc+j+0] = beta*c[ic+i+1,jc+j+0]+alpha*v10;
                            c[ic+i+1,jc+j+1] = beta*c[ic+i+1,jc+j+1]+alpha*v11;
                        }
                    }
                    else
                    {
                        
                        //
                        // Determine submatrix [I0..I1]x[J0..J1] to process
                        //
                        i0 = i;
                        i1 = Math.Min(i+1, m-1);
                        j0 = j;
                        j1 = Math.Min(j+1, n-1);
                        
                        //
                        // Process submatrix
                        //
                        for(ik=i0; ik<=i1; ik++)
                        {
                            for(jk=j0; jk<=j1; jk++)
                            {
                                if( k==0 || alpha==0 )
                                {
                                    v = 0;
                                }
                                else
                                {
                                    v = 0.0;
                                    if( optypea==0 && optypeb==0 )
                                    {
                                        i1_ = (ib)-(ja);
                                        v = 0.0;
                                        for(i_=ja; i_<=ja+k-1;i_++)
                                        {
                                            v += a[ia+ik,i_]*b[i_+i1_,jb+jk];
                                        }
                                    }
                                    if( optypea==0 && optypeb==1 )
                                    {
                                        i1_ = (jb)-(ja);
                                        v = 0.0;
                                        for(i_=ja; i_<=ja+k-1;i_++)
                                        {
                                            v += a[ia+ik,i_]*b[ib+jk,i_+i1_];
                                        }
                                    }
                                    if( optypea==0 && optypeb==2 )
                                    {
                                        i1_ = (jb)-(ja);
                                        v = 0.0;
                                        for(i_=ja; i_<=ja+k-1;i_++)
                                        {
                                            v += a[ia+ik,i_]*math.conj(b[ib+jk,i_+i1_]);
                                        }
                                    }
                                    if( optypea==1 && optypeb==0 )
                                    {
                                        i1_ = (ib)-(ia);
                                        v = 0.0;
                                        for(i_=ia; i_<=ia+k-1;i_++)
                                        {
                                            v += a[i_,ja+ik]*b[i_+i1_,jb+jk];
                                        }
                                    }
                                    if( optypea==1 && optypeb==1 )
                                    {
                                        i1_ = (jb)-(ia);
                                        v = 0.0;
                                        for(i_=ia; i_<=ia+k-1;i_++)
                                        {
                                            v += a[i_,ja+ik]*b[ib+jk,i_+i1_];
                                        }
                                    }
                                    if( optypea==1 && optypeb==2 )
                                    {
                                        i1_ = (jb)-(ia);
                                        v = 0.0;
                                        for(i_=ia; i_<=ia+k-1;i_++)
                                        {
                                            v += a[i_,ja+ik]*math.conj(b[ib+jk,i_+i1_]);
                                        }
                                    }
                                    if( optypea==2 && optypeb==0 )
                                    {
                                        i1_ = (ib)-(ia);
                                        v = 0.0;
                                        for(i_=ia; i_<=ia+k-1;i_++)
                                        {
                                            v += math.conj(a[i_,ja+ik])*b[i_+i1_,jb+jk];
                                        }
                                    }
                                    if( optypea==2 && optypeb==1 )
                                    {
                                        i1_ = (jb)-(ia);
                                        v = 0.0;
                                        for(i_=ia; i_<=ia+k-1;i_++)
                                        {
                                            v += math.conj(a[i_,ja+ik])*b[ib+jk,i_+i1_];
                                        }
                                    }
                                    if( optypea==2 && optypeb==2 )
                                    {
                                        i1_ = (jb)-(ia);
                                        v = 0.0;
                                        for(i_=ia; i_<=ia+k-1;i_++)
                                        {
                                            v += math.conj(a[i_,ja+ik])*math.conj(b[ib+jk,i_+i1_]);
                                        }
                                    }
                                }
                                if( beta==0 )
                                {
                                    c[ic+ik,jc+jk] = alpha*v;
                                }
                                else
                                {
                                    c[ic+ik,jc+jk] = beta*c[ic+ik,jc+jk]+alpha*v;
                                }
                            }
                        }
                    }
                    j = j+2;
                }
                i = i+2;
            }
        }


        /*************************************************************************
        RMatrixGEMM kernel, basecase code for RMatrixGEMM.

        This subroutine calculates C = alpha*op1(A)*op2(B) +beta*C where:
        * C is MxN general matrix
        * op1(A) is MxK matrix
        * op2(B) is KxN matrix
        * "op" may be identity transformation, transposition

        Additional info:
        * multiplication result replaces C. If Beta=0, C elements are not used in
          calculations (not multiplied by zero - just not referenced)
        * if Alpha=0, A is not used (not multiplied by zero - just not referenced)
        * if both Beta and Alpha are zero, C is filled by zeros.

        IMPORTANT:

        This function does NOT preallocate output matrix C, it MUST be preallocated
        by caller prior to calling this function. In case C does not have  enough
        space to store result, exception will be generated.

        INPUT PARAMETERS
            M       -   matrix size, M>0
            N       -   matrix size, N>0
            K       -   matrix size, K>0
            Alpha   -   coefficient
            A       -   matrix
            IA      -   submatrix offset
            JA      -   submatrix offset
            OpTypeA -   transformation type:
                        * 0 - no transformation
                        * 1 - transposition
            B       -   matrix
            IB      -   submatrix offset
            JB      -   submatrix offset
            OpTypeB -   transformation type:
                        * 0 - no transformation
                        * 1 - transposition
            Beta    -   coefficient
            C       -   PREALLOCATED output matrix
            IC      -   submatrix offset
            JC      -   submatrix offset

          -- ALGLIB routine --
             27.03.2013
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixgemmk(int m,
            int n,
            int k,
            double alpha,
            double[,] a,
            int ia,
            int ja,
            int optypea,
            double[,] b,
            int ib,
            int jb,
            int optypeb,
            double beta,
            double[,] c,
            int ic,
            int jc,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;

            
            //
            // if matrix size is zero
            //
            if( m==0 || n==0 )
            {
                return;
            }
            
            //
            // Try optimized code
            //
            if( rmatrixgemmf(m, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc, _params) )
            {
                return;
            }
            
            //
            // if K=0 or Alpha=0, then C=Beta*C
            //
            if( k==0 || (double)(alpha)==(double)(0) )
            {
                if( (double)(beta)!=(double)(1) )
                {
                    if( (double)(beta)!=(double)(0) )
                    {
                        for(i=0; i<=m-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                c[ic+i,jc+j] = beta*c[ic+i,jc+j];
                            }
                        }
                    }
                    else
                    {
                        for(i=0; i<=m-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                c[ic+i,jc+j] = 0;
                            }
                        }
                    }
                }
                return;
            }
            
            //
            // Call specialized code.
            //
            // NOTE: specialized code was moved to separate function because of strange
            //       issues with instructions cache on some systems; Having too long
            //       functions significantly slows down internal loop of the algorithm.
            //
            if( optypea==0 && optypeb==0 )
            {
                rmatrixgemmk44v00(m, n, k, alpha, a, ia, ja, b, ib, jb, beta, c, ic, jc, _params);
            }
            if( optypea==0 && optypeb!=0 )
            {
                rmatrixgemmk44v01(m, n, k, alpha, a, ia, ja, b, ib, jb, beta, c, ic, jc, _params);
            }
            if( optypea!=0 && optypeb==0 )
            {
                rmatrixgemmk44v10(m, n, k, alpha, a, ia, ja, b, ib, jb, beta, c, ic, jc, _params);
            }
            if( optypea!=0 && optypeb!=0 )
            {
                rmatrixgemmk44v11(m, n, k, alpha, a, ia, ja, b, ib, jb, beta, c, ic, jc, _params);
            }
        }


        /*************************************************************************
        RMatrixGEMM kernel, basecase code for RMatrixGEMM, specialized for sitation
        with OpTypeA=0 and OpTypeB=0.

        Additional info:
        * this function requires that Alpha<>0 (assertion is thrown otherwise)

        INPUT PARAMETERS
            M       -   matrix size, M>0
            N       -   matrix size, N>0
            K       -   matrix size, K>0
            Alpha   -   coefficient
            A       -   matrix
            IA      -   submatrix offset
            JA      -   submatrix offset
            B       -   matrix
            IB      -   submatrix offset
            JB      -   submatrix offset
            Beta    -   coefficient
            C       -   PREALLOCATED output matrix
            IC      -   submatrix offset
            JC      -   submatrix offset

          -- ALGLIB routine --
             27.03.2013
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixgemmk44v00(int m,
            int n,
            int k,
            double alpha,
            double[,] a,
            int ia,
            int ja,
            double[,] b,
            int ib,
            int jb,
            double beta,
            double[,] c,
            int ic,
            int jc,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;
            double v = 0;
            double v00 = 0;
            double v01 = 0;
            double v02 = 0;
            double v03 = 0;
            double v10 = 0;
            double v11 = 0;
            double v12 = 0;
            double v13 = 0;
            double v20 = 0;
            double v21 = 0;
            double v22 = 0;
            double v23 = 0;
            double v30 = 0;
            double v31 = 0;
            double v32 = 0;
            double v33 = 0;
            double a0 = 0;
            double a1 = 0;
            double a2 = 0;
            double a3 = 0;
            double b0 = 0;
            double b1 = 0;
            double b2 = 0;
            double b3 = 0;
            int idxa0 = 0;
            int idxa1 = 0;
            int idxa2 = 0;
            int idxa3 = 0;
            int idxb0 = 0;
            int idxb1 = 0;
            int idxb2 = 0;
            int idxb3 = 0;
            int i0 = 0;
            int i1 = 0;
            int ik = 0;
            int j0 = 0;
            int j1 = 0;
            int jk = 0;
            int t = 0;
            int offsa = 0;
            int offsb = 0;
            int i_ = 0;
            int i1_ = 0;

            alglib.ap.assert((double)(alpha)!=(double)(0), "RMatrixGEMMK44V00: internal error (Alpha=0)");
            
            //
            // if matrix size is zero
            //
            if( m==0 || n==0 )
            {
                return;
            }
            
            //
            // A*B
            //
            i = 0;
            while( i<m )
            {
                j = 0;
                while( j<n )
                {
                    
                    //
                    // Choose between specialized 4x4 code and general code
                    //
                    if( i+4<=m && j+4<=n )
                    {
                        
                        //
                        // Specialized 4x4 code for [I..I+3]x[J..J+3] submatrix of C.
                        //
                        // This submatrix is calculated as sum of K rank-1 products,
                        // with operands cached in local variables in order to speed
                        // up operations with arrays.
                        //
                        idxa0 = ia+i+0;
                        idxa1 = ia+i+1;
                        idxa2 = ia+i+2;
                        idxa3 = ia+i+3;
                        offsa = ja;
                        idxb0 = jb+j+0;
                        idxb1 = jb+j+1;
                        idxb2 = jb+j+2;
                        idxb3 = jb+j+3;
                        offsb = ib;
                        v00 = 0.0;
                        v01 = 0.0;
                        v02 = 0.0;
                        v03 = 0.0;
                        v10 = 0.0;
                        v11 = 0.0;
                        v12 = 0.0;
                        v13 = 0.0;
                        v20 = 0.0;
                        v21 = 0.0;
                        v22 = 0.0;
                        v23 = 0.0;
                        v30 = 0.0;
                        v31 = 0.0;
                        v32 = 0.0;
                        v33 = 0.0;
                        
                        //
                        // Different variants of internal loop
                        //
                        for(t=0; t<=k-1; t++)
                        {
                            a0 = a[idxa0,offsa];
                            a1 = a[idxa1,offsa];
                            b0 = b[offsb,idxb0];
                            b1 = b[offsb,idxb1];
                            v00 = v00+a0*b0;
                            v01 = v01+a0*b1;
                            v10 = v10+a1*b0;
                            v11 = v11+a1*b1;
                            a2 = a[idxa2,offsa];
                            a3 = a[idxa3,offsa];
                            v20 = v20+a2*b0;
                            v21 = v21+a2*b1;
                            v30 = v30+a3*b0;
                            v31 = v31+a3*b1;
                            b2 = b[offsb,idxb2];
                            b3 = b[offsb,idxb3];
                            v22 = v22+a2*b2;
                            v23 = v23+a2*b3;
                            v32 = v32+a3*b2;
                            v33 = v33+a3*b3;
                            v02 = v02+a0*b2;
                            v03 = v03+a0*b3;
                            v12 = v12+a1*b2;
                            v13 = v13+a1*b3;
                            offsa = offsa+1;
                            offsb = offsb+1;
                        }
                        if( (double)(beta)==(double)(0) )
                        {
                            c[ic+i+0,jc+j+0] = alpha*v00;
                            c[ic+i+0,jc+j+1] = alpha*v01;
                            c[ic+i+0,jc+j+2] = alpha*v02;
                            c[ic+i+0,jc+j+3] = alpha*v03;
                            c[ic+i+1,jc+j+0] = alpha*v10;
                            c[ic+i+1,jc+j+1] = alpha*v11;
                            c[ic+i+1,jc+j+2] = alpha*v12;
                            c[ic+i+1,jc+j+3] = alpha*v13;
                            c[ic+i+2,jc+j+0] = alpha*v20;
                            c[ic+i+2,jc+j+1] = alpha*v21;
                            c[ic+i+2,jc+j+2] = alpha*v22;
                            c[ic+i+2,jc+j+3] = alpha*v23;
                            c[ic+i+3,jc+j+0] = alpha*v30;
                            c[ic+i+3,jc+j+1] = alpha*v31;
                            c[ic+i+3,jc+j+2] = alpha*v32;
                            c[ic+i+3,jc+j+3] = alpha*v33;
                        }
                        else
                        {
                            c[ic+i+0,jc+j+0] = beta*c[ic+i+0,jc+j+0]+alpha*v00;
                            c[ic+i+0,jc+j+1] = beta*c[ic+i+0,jc+j+1]+alpha*v01;
                            c[ic+i+0,jc+j+2] = beta*c[ic+i+0,jc+j+2]+alpha*v02;
                            c[ic+i+0,jc+j+3] = beta*c[ic+i+0,jc+j+3]+alpha*v03;
                            c[ic+i+1,jc+j+0] = beta*c[ic+i+1,jc+j+0]+alpha*v10;
                            c[ic+i+1,jc+j+1] = beta*c[ic+i+1,jc+j+1]+alpha*v11;
                            c[ic+i+1,jc+j+2] = beta*c[ic+i+1,jc+j+2]+alpha*v12;
                            c[ic+i+1,jc+j+3] = beta*c[ic+i+1,jc+j+3]+alpha*v13;
                            c[ic+i+2,jc+j+0] = beta*c[ic+i+2,jc+j+0]+alpha*v20;
                            c[ic+i+2,jc+j+1] = beta*c[ic+i+2,jc+j+1]+alpha*v21;
                            c[ic+i+2,jc+j+2] = beta*c[ic+i+2,jc+j+2]+alpha*v22;
                            c[ic+i+2,jc+j+3] = beta*c[ic+i+2,jc+j+3]+alpha*v23;
                            c[ic+i+3,jc+j+0] = beta*c[ic+i+3,jc+j+0]+alpha*v30;
                            c[ic+i+3,jc+j+1] = beta*c[ic+i+3,jc+j+1]+alpha*v31;
                            c[ic+i+3,jc+j+2] = beta*c[ic+i+3,jc+j+2]+alpha*v32;
                            c[ic+i+3,jc+j+3] = beta*c[ic+i+3,jc+j+3]+alpha*v33;
                        }
                    }
                    else
                    {
                        
                        //
                        // Determine submatrix [I0..I1]x[J0..J1] to process
                        //
                        i0 = i;
                        i1 = Math.Min(i+3, m-1);
                        j0 = j;
                        j1 = Math.Min(j+3, n-1);
                        
                        //
                        // Process submatrix
                        //
                        for(ik=i0; ik<=i1; ik++)
                        {
                            for(jk=j0; jk<=j1; jk++)
                            {
                                if( k==0 || (double)(alpha)==(double)(0) )
                                {
                                    v = 0;
                                }
                                else
                                {
                                    i1_ = (ib)-(ja);
                                    v = 0.0;
                                    for(i_=ja; i_<=ja+k-1;i_++)
                                    {
                                        v += a[ia+ik,i_]*b[i_+i1_,jb+jk];
                                    }
                                }
                                if( (double)(beta)==(double)(0) )
                                {
                                    c[ic+ik,jc+jk] = alpha*v;
                                }
                                else
                                {
                                    c[ic+ik,jc+jk] = beta*c[ic+ik,jc+jk]+alpha*v;
                                }
                            }
                        }
                    }
                    j = j+4;
                }
                i = i+4;
            }
        }


        /*************************************************************************
        RMatrixGEMM kernel, basecase code for RMatrixGEMM, specialized for sitation
        with OpTypeA=0 and OpTypeB=1.

        Additional info:
        * this function requires that Alpha<>0 (assertion is thrown otherwise)

        INPUT PARAMETERS
            M       -   matrix size, M>0
            N       -   matrix size, N>0
            K       -   matrix size, K>0
            Alpha   -   coefficient
            A       -   matrix
            IA      -   submatrix offset
            JA      -   submatrix offset
            B       -   matrix
            IB      -   submatrix offset
            JB      -   submatrix offset
            Beta    -   coefficient
            C       -   PREALLOCATED output matrix
            IC      -   submatrix offset
            JC      -   submatrix offset

          -- ALGLIB routine --
             27.03.2013
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixgemmk44v01(int m,
            int n,
            int k,
            double alpha,
            double[,] a,
            int ia,
            int ja,
            double[,] b,
            int ib,
            int jb,
            double beta,
            double[,] c,
            int ic,
            int jc,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;
            double v = 0;
            double v00 = 0;
            double v01 = 0;
            double v02 = 0;
            double v03 = 0;
            double v10 = 0;
            double v11 = 0;
            double v12 = 0;
            double v13 = 0;
            double v20 = 0;
            double v21 = 0;
            double v22 = 0;
            double v23 = 0;
            double v30 = 0;
            double v31 = 0;
            double v32 = 0;
            double v33 = 0;
            double a0 = 0;
            double a1 = 0;
            double a2 = 0;
            double a3 = 0;
            double b0 = 0;
            double b1 = 0;
            double b2 = 0;
            double b3 = 0;
            int idxa0 = 0;
            int idxa1 = 0;
            int idxa2 = 0;
            int idxa3 = 0;
            int idxb0 = 0;
            int idxb1 = 0;
            int idxb2 = 0;
            int idxb3 = 0;
            int i0 = 0;
            int i1 = 0;
            int ik = 0;
            int j0 = 0;
            int j1 = 0;
            int jk = 0;
            int t = 0;
            int offsa = 0;
            int offsb = 0;
            int i_ = 0;
            int i1_ = 0;

            alglib.ap.assert((double)(alpha)!=(double)(0), "RMatrixGEMMK44V00: internal error (Alpha=0)");
            
            //
            // if matrix size is zero
            //
            if( m==0 || n==0 )
            {
                return;
            }
            
            //
            // A*B'
            //
            i = 0;
            while( i<m )
            {
                j = 0;
                while( j<n )
                {
                    
                    //
                    // Choose between specialized 4x4 code and general code
                    //
                    if( i+4<=m && j+4<=n )
                    {
                        
                        //
                        // Specialized 4x4 code for [I..I+3]x[J..J+3] submatrix of C.
                        //
                        // This submatrix is calculated as sum of K rank-1 products,
                        // with operands cached in local variables in order to speed
                        // up operations with arrays.
                        //
                        idxa0 = ia+i+0;
                        idxa1 = ia+i+1;
                        idxa2 = ia+i+2;
                        idxa3 = ia+i+3;
                        offsa = ja;
                        idxb0 = ib+j+0;
                        idxb1 = ib+j+1;
                        idxb2 = ib+j+2;
                        idxb3 = ib+j+3;
                        offsb = jb;
                        v00 = 0.0;
                        v01 = 0.0;
                        v02 = 0.0;
                        v03 = 0.0;
                        v10 = 0.0;
                        v11 = 0.0;
                        v12 = 0.0;
                        v13 = 0.0;
                        v20 = 0.0;
                        v21 = 0.0;
                        v22 = 0.0;
                        v23 = 0.0;
                        v30 = 0.0;
                        v31 = 0.0;
                        v32 = 0.0;
                        v33 = 0.0;
                        for(t=0; t<=k-1; t++)
                        {
                            a0 = a[idxa0,offsa];
                            a1 = a[idxa1,offsa];
                            b0 = b[idxb0,offsb];
                            b1 = b[idxb1,offsb];
                            v00 = v00+a0*b0;
                            v01 = v01+a0*b1;
                            v10 = v10+a1*b0;
                            v11 = v11+a1*b1;
                            a2 = a[idxa2,offsa];
                            a3 = a[idxa3,offsa];
                            v20 = v20+a2*b0;
                            v21 = v21+a2*b1;
                            v30 = v30+a3*b0;
                            v31 = v31+a3*b1;
                            b2 = b[idxb2,offsb];
                            b3 = b[idxb3,offsb];
                            v22 = v22+a2*b2;
                            v23 = v23+a2*b3;
                            v32 = v32+a3*b2;
                            v33 = v33+a3*b3;
                            v02 = v02+a0*b2;
                            v03 = v03+a0*b3;
                            v12 = v12+a1*b2;
                            v13 = v13+a1*b3;
                            offsa = offsa+1;
                            offsb = offsb+1;
                        }
                        if( (double)(beta)==(double)(0) )
                        {
                            c[ic+i+0,jc+j+0] = alpha*v00;
                            c[ic+i+0,jc+j+1] = alpha*v01;
                            c[ic+i+0,jc+j+2] = alpha*v02;
                            c[ic+i+0,jc+j+3] = alpha*v03;
                            c[ic+i+1,jc+j+0] = alpha*v10;
                            c[ic+i+1,jc+j+1] = alpha*v11;
                            c[ic+i+1,jc+j+2] = alpha*v12;
                            c[ic+i+1,jc+j+3] = alpha*v13;
                            c[ic+i+2,jc+j+0] = alpha*v20;
                            c[ic+i+2,jc+j+1] = alpha*v21;
                            c[ic+i+2,jc+j+2] = alpha*v22;
                            c[ic+i+2,jc+j+3] = alpha*v23;
                            c[ic+i+3,jc+j+0] = alpha*v30;
                            c[ic+i+3,jc+j+1] = alpha*v31;
                            c[ic+i+3,jc+j+2] = alpha*v32;
                            c[ic+i+3,jc+j+3] = alpha*v33;
                        }
                        else
                        {
                            c[ic+i+0,jc+j+0] = beta*c[ic+i+0,jc+j+0]+alpha*v00;
                            c[ic+i+0,jc+j+1] = beta*c[ic+i+0,jc+j+1]+alpha*v01;
                            c[ic+i+0,jc+j+2] = beta*c[ic+i+0,jc+j+2]+alpha*v02;
                            c[ic+i+0,jc+j+3] = beta*c[ic+i+0,jc+j+3]+alpha*v03;
                            c[ic+i+1,jc+j+0] = beta*c[ic+i+1,jc+j+0]+alpha*v10;
                            c[ic+i+1,jc+j+1] = beta*c[ic+i+1,jc+j+1]+alpha*v11;
                            c[ic+i+1,jc+j+2] = beta*c[ic+i+1,jc+j+2]+alpha*v12;
                            c[ic+i+1,jc+j+3] = beta*c[ic+i+1,jc+j+3]+alpha*v13;
                            c[ic+i+2,jc+j+0] = beta*c[ic+i+2,jc+j+0]+alpha*v20;
                            c[ic+i+2,jc+j+1] = beta*c[ic+i+2,jc+j+1]+alpha*v21;
                            c[ic+i+2,jc+j+2] = beta*c[ic+i+2,jc+j+2]+alpha*v22;
                            c[ic+i+2,jc+j+3] = beta*c[ic+i+2,jc+j+3]+alpha*v23;
                            c[ic+i+3,jc+j+0] = beta*c[ic+i+3,jc+j+0]+alpha*v30;
                            c[ic+i+3,jc+j+1] = beta*c[ic+i+3,jc+j+1]+alpha*v31;
                            c[ic+i+3,jc+j+2] = beta*c[ic+i+3,jc+j+2]+alpha*v32;
                            c[ic+i+3,jc+j+3] = beta*c[ic+i+3,jc+j+3]+alpha*v33;
                        }
                    }
                    else
                    {
                        
                        //
                        // Determine submatrix [I0..I1]x[J0..J1] to process
                        //
                        i0 = i;
                        i1 = Math.Min(i+3, m-1);
                        j0 = j;
                        j1 = Math.Min(j+3, n-1);
                        
                        //
                        // Process submatrix
                        //
                        for(ik=i0; ik<=i1; ik++)
                        {
                            for(jk=j0; jk<=j1; jk++)
                            {
                                if( k==0 || (double)(alpha)==(double)(0) )
                                {
                                    v = 0;
                                }
                                else
                                {
                                    i1_ = (jb)-(ja);
                                    v = 0.0;
                                    for(i_=ja; i_<=ja+k-1;i_++)
                                    {
                                        v += a[ia+ik,i_]*b[ib+jk,i_+i1_];
                                    }
                                }
                                if( (double)(beta)==(double)(0) )
                                {
                                    c[ic+ik,jc+jk] = alpha*v;
                                }
                                else
                                {
                                    c[ic+ik,jc+jk] = beta*c[ic+ik,jc+jk]+alpha*v;
                                }
                            }
                        }
                    }
                    j = j+4;
                }
                i = i+4;
            }
        }


        /*************************************************************************
        RMatrixGEMM kernel, basecase code for RMatrixGEMM, specialized for sitation
        with OpTypeA=1 and OpTypeB=0.

        Additional info:
        * this function requires that Alpha<>0 (assertion is thrown otherwise)

        INPUT PARAMETERS
            M       -   matrix size, M>0
            N       -   matrix size, N>0
            K       -   matrix size, K>0
            Alpha   -   coefficient
            A       -   matrix
            IA      -   submatrix offset
            JA      -   submatrix offset
            B       -   matrix
            IB      -   submatrix offset
            JB      -   submatrix offset
            Beta    -   coefficient
            C       -   PREALLOCATED output matrix
            IC      -   submatrix offset
            JC      -   submatrix offset

          -- ALGLIB routine --
             27.03.2013
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixgemmk44v10(int m,
            int n,
            int k,
            double alpha,
            double[,] a,
            int ia,
            int ja,
            double[,] b,
            int ib,
            int jb,
            double beta,
            double[,] c,
            int ic,
            int jc,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;
            double v = 0;
            double v00 = 0;
            double v01 = 0;
            double v02 = 0;
            double v03 = 0;
            double v10 = 0;
            double v11 = 0;
            double v12 = 0;
            double v13 = 0;
            double v20 = 0;
            double v21 = 0;
            double v22 = 0;
            double v23 = 0;
            double v30 = 0;
            double v31 = 0;
            double v32 = 0;
            double v33 = 0;
            double a0 = 0;
            double a1 = 0;
            double a2 = 0;
            double a3 = 0;
            double b0 = 0;
            double b1 = 0;
            double b2 = 0;
            double b3 = 0;
            int idxa0 = 0;
            int idxa1 = 0;
            int idxa2 = 0;
            int idxa3 = 0;
            int idxb0 = 0;
            int idxb1 = 0;
            int idxb2 = 0;
            int idxb3 = 0;
            int i0 = 0;
            int i1 = 0;
            int ik = 0;
            int j0 = 0;
            int j1 = 0;
            int jk = 0;
            int t = 0;
            int offsa = 0;
            int offsb = 0;
            int i_ = 0;
            int i1_ = 0;

            alglib.ap.assert((double)(alpha)!=(double)(0), "RMatrixGEMMK44V00: internal error (Alpha=0)");
            
            //
            // if matrix size is zero
            //
            if( m==0 || n==0 )
            {
                return;
            }
            
            //
            // A'*B
            //
            i = 0;
            while( i<m )
            {
                j = 0;
                while( j<n )
                {
                    
                    //
                    // Choose between specialized 4x4 code and general code
                    //
                    if( i+4<=m && j+4<=n )
                    {
                        
                        //
                        // Specialized 4x4 code for [I..I+3]x[J..J+3] submatrix of C.
                        //
                        // This submatrix is calculated as sum of K rank-1 products,
                        // with operands cached in local variables in order to speed
                        // up operations with arrays.
                        //
                        idxa0 = ja+i+0;
                        idxa1 = ja+i+1;
                        idxa2 = ja+i+2;
                        idxa3 = ja+i+3;
                        offsa = ia;
                        idxb0 = jb+j+0;
                        idxb1 = jb+j+1;
                        idxb2 = jb+j+2;
                        idxb3 = jb+j+3;
                        offsb = ib;
                        v00 = 0.0;
                        v01 = 0.0;
                        v02 = 0.0;
                        v03 = 0.0;
                        v10 = 0.0;
                        v11 = 0.0;
                        v12 = 0.0;
                        v13 = 0.0;
                        v20 = 0.0;
                        v21 = 0.0;
                        v22 = 0.0;
                        v23 = 0.0;
                        v30 = 0.0;
                        v31 = 0.0;
                        v32 = 0.0;
                        v33 = 0.0;
                        for(t=0; t<=k-1; t++)
                        {
                            a0 = a[offsa,idxa0];
                            a1 = a[offsa,idxa1];
                            b0 = b[offsb,idxb0];
                            b1 = b[offsb,idxb1];
                            v00 = v00+a0*b0;
                            v01 = v01+a0*b1;
                            v10 = v10+a1*b0;
                            v11 = v11+a1*b1;
                            a2 = a[offsa,idxa2];
                            a3 = a[offsa,idxa3];
                            v20 = v20+a2*b0;
                            v21 = v21+a2*b1;
                            v30 = v30+a3*b0;
                            v31 = v31+a3*b1;
                            b2 = b[offsb,idxb2];
                            b3 = b[offsb,idxb3];
                            v22 = v22+a2*b2;
                            v23 = v23+a2*b3;
                            v32 = v32+a3*b2;
                            v33 = v33+a3*b3;
                            v02 = v02+a0*b2;
                            v03 = v03+a0*b3;
                            v12 = v12+a1*b2;
                            v13 = v13+a1*b3;
                            offsa = offsa+1;
                            offsb = offsb+1;
                        }
                        if( (double)(beta)==(double)(0) )
                        {
                            c[ic+i+0,jc+j+0] = alpha*v00;
                            c[ic+i+0,jc+j+1] = alpha*v01;
                            c[ic+i+0,jc+j+2] = alpha*v02;
                            c[ic+i+0,jc+j+3] = alpha*v03;
                            c[ic+i+1,jc+j+0] = alpha*v10;
                            c[ic+i+1,jc+j+1] = alpha*v11;
                            c[ic+i+1,jc+j+2] = alpha*v12;
                            c[ic+i+1,jc+j+3] = alpha*v13;
                            c[ic+i+2,jc+j+0] = alpha*v20;
                            c[ic+i+2,jc+j+1] = alpha*v21;
                            c[ic+i+2,jc+j+2] = alpha*v22;
                            c[ic+i+2,jc+j+3] = alpha*v23;
                            c[ic+i+3,jc+j+0] = alpha*v30;
                            c[ic+i+3,jc+j+1] = alpha*v31;
                            c[ic+i+3,jc+j+2] = alpha*v32;
                            c[ic+i+3,jc+j+3] = alpha*v33;
                        }
                        else
                        {
                            c[ic+i+0,jc+j+0] = beta*c[ic+i+0,jc+j+0]+alpha*v00;
                            c[ic+i+0,jc+j+1] = beta*c[ic+i+0,jc+j+1]+alpha*v01;
                            c[ic+i+0,jc+j+2] = beta*c[ic+i+0,jc+j+2]+alpha*v02;
                            c[ic+i+0,jc+j+3] = beta*c[ic+i+0,jc+j+3]+alpha*v03;
                            c[ic+i+1,jc+j+0] = beta*c[ic+i+1,jc+j+0]+alpha*v10;
                            c[ic+i+1,jc+j+1] = beta*c[ic+i+1,jc+j+1]+alpha*v11;
                            c[ic+i+1,jc+j+2] = beta*c[ic+i+1,jc+j+2]+alpha*v12;
                            c[ic+i+1,jc+j+3] = beta*c[ic+i+1,jc+j+3]+alpha*v13;
                            c[ic+i+2,jc+j+0] = beta*c[ic+i+2,jc+j+0]+alpha*v20;
                            c[ic+i+2,jc+j+1] = beta*c[ic+i+2,jc+j+1]+alpha*v21;
                            c[ic+i+2,jc+j+2] = beta*c[ic+i+2,jc+j+2]+alpha*v22;
                            c[ic+i+2,jc+j+3] = beta*c[ic+i+2,jc+j+3]+alpha*v23;
                            c[ic+i+3,jc+j+0] = beta*c[ic+i+3,jc+j+0]+alpha*v30;
                            c[ic+i+3,jc+j+1] = beta*c[ic+i+3,jc+j+1]+alpha*v31;
                            c[ic+i+3,jc+j+2] = beta*c[ic+i+3,jc+j+2]+alpha*v32;
                            c[ic+i+3,jc+j+3] = beta*c[ic+i+3,jc+j+3]+alpha*v33;
                        }
                    }
                    else
                    {
                        
                        //
                        // Determine submatrix [I0..I1]x[J0..J1] to process
                        //
                        i0 = i;
                        i1 = Math.Min(i+3, m-1);
                        j0 = j;
                        j1 = Math.Min(j+3, n-1);
                        
                        //
                        // Process submatrix
                        //
                        for(ik=i0; ik<=i1; ik++)
                        {
                            for(jk=j0; jk<=j1; jk++)
                            {
                                if( k==0 || (double)(alpha)==(double)(0) )
                                {
                                    v = 0;
                                }
                                else
                                {
                                    v = 0.0;
                                    i1_ = (ib)-(ia);
                                    v = 0.0;
                                    for(i_=ia; i_<=ia+k-1;i_++)
                                    {
                                        v += a[i_,ja+ik]*b[i_+i1_,jb+jk];
                                    }
                                }
                                if( (double)(beta)==(double)(0) )
                                {
                                    c[ic+ik,jc+jk] = alpha*v;
                                }
                                else
                                {
                                    c[ic+ik,jc+jk] = beta*c[ic+ik,jc+jk]+alpha*v;
                                }
                            }
                        }
                    }
                    j = j+4;
                }
                i = i+4;
            }
        }


        /*************************************************************************
        RMatrixGEMM kernel, basecase code for RMatrixGEMM, specialized for sitation
        with OpTypeA=1 and OpTypeB=1.

        Additional info:
        * this function requires that Alpha<>0 (assertion is thrown otherwise)

        INPUT PARAMETERS
            M       -   matrix size, M>0
            N       -   matrix size, N>0
            K       -   matrix size, K>0
            Alpha   -   coefficient
            A       -   matrix
            IA      -   submatrix offset
            JA      -   submatrix offset
            B       -   matrix
            IB      -   submatrix offset
            JB      -   submatrix offset
            Beta    -   coefficient
            C       -   PREALLOCATED output matrix
            IC      -   submatrix offset
            JC      -   submatrix offset

          -- ALGLIB routine --
             27.03.2013
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixgemmk44v11(int m,
            int n,
            int k,
            double alpha,
            double[,] a,
            int ia,
            int ja,
            double[,] b,
            int ib,
            int jb,
            double beta,
            double[,] c,
            int ic,
            int jc,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;
            double v = 0;
            double v00 = 0;
            double v01 = 0;
            double v02 = 0;
            double v03 = 0;
            double v10 = 0;
            double v11 = 0;
            double v12 = 0;
            double v13 = 0;
            double v20 = 0;
            double v21 = 0;
            double v22 = 0;
            double v23 = 0;
            double v30 = 0;
            double v31 = 0;
            double v32 = 0;
            double v33 = 0;
            double a0 = 0;
            double a1 = 0;
            double a2 = 0;
            double a3 = 0;
            double b0 = 0;
            double b1 = 0;
            double b2 = 0;
            double b3 = 0;
            int idxa0 = 0;
            int idxa1 = 0;
            int idxa2 = 0;
            int idxa3 = 0;
            int idxb0 = 0;
            int idxb1 = 0;
            int idxb2 = 0;
            int idxb3 = 0;
            int i0 = 0;
            int i1 = 0;
            int ik = 0;
            int j0 = 0;
            int j1 = 0;
            int jk = 0;
            int t = 0;
            int offsa = 0;
            int offsb = 0;
            int i_ = 0;
            int i1_ = 0;

            alglib.ap.assert((double)(alpha)!=(double)(0), "RMatrixGEMMK44V00: internal error (Alpha=0)");
            
            //
            // if matrix size is zero
            //
            if( m==0 || n==0 )
            {
                return;
            }
            
            //
            // A'*B'
            //
            i = 0;
            while( i<m )
            {
                j = 0;
                while( j<n )
                {
                    
                    //
                    // Choose between specialized 4x4 code and general code
                    //
                    if( i+4<=m && j+4<=n )
                    {
                        
                        //
                        // Specialized 4x4 code for [I..I+3]x[J..J+3] submatrix of C.
                        //
                        // This submatrix is calculated as sum of K rank-1 products,
                        // with operands cached in local variables in order to speed
                        // up operations with arrays.
                        //
                        idxa0 = ja+i+0;
                        idxa1 = ja+i+1;
                        idxa2 = ja+i+2;
                        idxa3 = ja+i+3;
                        offsa = ia;
                        idxb0 = ib+j+0;
                        idxb1 = ib+j+1;
                        idxb2 = ib+j+2;
                        idxb3 = ib+j+3;
                        offsb = jb;
                        v00 = 0.0;
                        v01 = 0.0;
                        v02 = 0.0;
                        v03 = 0.0;
                        v10 = 0.0;
                        v11 = 0.0;
                        v12 = 0.0;
                        v13 = 0.0;
                        v20 = 0.0;
                        v21 = 0.0;
                        v22 = 0.0;
                        v23 = 0.0;
                        v30 = 0.0;
                        v31 = 0.0;
                        v32 = 0.0;
                        v33 = 0.0;
                        for(t=0; t<=k-1; t++)
                        {
                            a0 = a[offsa,idxa0];
                            a1 = a[offsa,idxa1];
                            b0 = b[idxb0,offsb];
                            b1 = b[idxb1,offsb];
                            v00 = v00+a0*b0;
                            v01 = v01+a0*b1;
                            v10 = v10+a1*b0;
                            v11 = v11+a1*b1;
                            a2 = a[offsa,idxa2];
                            a3 = a[offsa,idxa3];
                            v20 = v20+a2*b0;
                            v21 = v21+a2*b1;
                            v30 = v30+a3*b0;
                            v31 = v31+a3*b1;
                            b2 = b[idxb2,offsb];
                            b3 = b[idxb3,offsb];
                            v22 = v22+a2*b2;
                            v23 = v23+a2*b3;
                            v32 = v32+a3*b2;
                            v33 = v33+a3*b3;
                            v02 = v02+a0*b2;
                            v03 = v03+a0*b3;
                            v12 = v12+a1*b2;
                            v13 = v13+a1*b3;
                            offsa = offsa+1;
                            offsb = offsb+1;
                        }
                        if( (double)(beta)==(double)(0) )
                        {
                            c[ic+i+0,jc+j+0] = alpha*v00;
                            c[ic+i+0,jc+j+1] = alpha*v01;
                            c[ic+i+0,jc+j+2] = alpha*v02;
                            c[ic+i+0,jc+j+3] = alpha*v03;
                            c[ic+i+1,jc+j+0] = alpha*v10;
                            c[ic+i+1,jc+j+1] = alpha*v11;
                            c[ic+i+1,jc+j+2] = alpha*v12;
                            c[ic+i+1,jc+j+3] = alpha*v13;
                            c[ic+i+2,jc+j+0] = alpha*v20;
                            c[ic+i+2,jc+j+1] = alpha*v21;
                            c[ic+i+2,jc+j+2] = alpha*v22;
                            c[ic+i+2,jc+j+3] = alpha*v23;
                            c[ic+i+3,jc+j+0] = alpha*v30;
                            c[ic+i+3,jc+j+1] = alpha*v31;
                            c[ic+i+3,jc+j+2] = alpha*v32;
                            c[ic+i+3,jc+j+3] = alpha*v33;
                        }
                        else
                        {
                            c[ic+i+0,jc+j+0] = beta*c[ic+i+0,jc+j+0]+alpha*v00;
                            c[ic+i+0,jc+j+1] = beta*c[ic+i+0,jc+j+1]+alpha*v01;
                            c[ic+i+0,jc+j+2] = beta*c[ic+i+0,jc+j+2]+alpha*v02;
                            c[ic+i+0,jc+j+3] = beta*c[ic+i+0,jc+j+3]+alpha*v03;
                            c[ic+i+1,jc+j+0] = beta*c[ic+i+1,jc+j+0]+alpha*v10;
                            c[ic+i+1,jc+j+1] = beta*c[ic+i+1,jc+j+1]+alpha*v11;
                            c[ic+i+1,jc+j+2] = beta*c[ic+i+1,jc+j+2]+alpha*v12;
                            c[ic+i+1,jc+j+3] = beta*c[ic+i+1,jc+j+3]+alpha*v13;
                            c[ic+i+2,jc+j+0] = beta*c[ic+i+2,jc+j+0]+alpha*v20;
                            c[ic+i+2,jc+j+1] = beta*c[ic+i+2,jc+j+1]+alpha*v21;
                            c[ic+i+2,jc+j+2] = beta*c[ic+i+2,jc+j+2]+alpha*v22;
                            c[ic+i+2,jc+j+3] = beta*c[ic+i+2,jc+j+3]+alpha*v23;
                            c[ic+i+3,jc+j+0] = beta*c[ic+i+3,jc+j+0]+alpha*v30;
                            c[ic+i+3,jc+j+1] = beta*c[ic+i+3,jc+j+1]+alpha*v31;
                            c[ic+i+3,jc+j+2] = beta*c[ic+i+3,jc+j+2]+alpha*v32;
                            c[ic+i+3,jc+j+3] = beta*c[ic+i+3,jc+j+3]+alpha*v33;
                        }
                    }
                    else
                    {
                        
                        //
                        // Determine submatrix [I0..I1]x[J0..J1] to process
                        //
                        i0 = i;
                        i1 = Math.Min(i+3, m-1);
                        j0 = j;
                        j1 = Math.Min(j+3, n-1);
                        
                        //
                        // Process submatrix
                        //
                        for(ik=i0; ik<=i1; ik++)
                        {
                            for(jk=j0; jk<=j1; jk++)
                            {
                                if( k==0 || (double)(alpha)==(double)(0) )
                                {
                                    v = 0;
                                }
                                else
                                {
                                    v = 0.0;
                                    i1_ = (jb)-(ia);
                                    v = 0.0;
                                    for(i_=ia; i_<=ia+k-1;i_++)
                                    {
                                        v += a[i_,ja+ik]*b[ib+jk,i_+i1_];
                                    }
                                }
                                if( (double)(beta)==(double)(0) )
                                {
                                    c[ic+ik,jc+jk] = alpha*v;
                                }
                                else
                                {
                                    c[ic+ik,jc+jk] = beta*c[ic+ik,jc+jk]+alpha*v;
                                }
                            }
                        }
                    }
                    j = j+4;
                }
                i = i+4;
            }
        }


    }
    public class creflections
    {
        /*************************************************************************
        Generation of an elementary complex reflection transformation

        The subroutine generates elementary complex reflection H of  order  N,  so
        that, for a given X, the following equality holds true:

             ( X(1) )   ( Beta )
        H' * (  ..  ) = (  0   ),   H'*H = I,   Beta is a real number
             ( X(n) )   (  0   )

        where

                      ( V(1) )
        H = 1 - Tau * (  ..  ) * ( conj(V(1)), ..., conj(V(n)) )
                      ( V(n) )

        where the first component of vector V equals 1.

        Input parameters:
            X   -   vector. Array with elements [1..N].
            N   -   reflection order.

        Output parameters:
            X   -   components from 2 to N are replaced by vector V.
                    The first component is replaced with parameter Beta.
            Tau -   scalar value Tau.

        This subroutine is the modification of CLARFG subroutines  from the LAPACK
        library. It has similar functionality except for the fact that it  doesn't
        handle errors when intermediate results cause an overflow.

          -- LAPACK auxiliary routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             September 30, 1994
        *************************************************************************/
        public static void complexgeneratereflection(ref complex[] x,
            int n,
            ref complex tau,
            alglib.xparams _params)
        {
            int j = 0;
            complex alpha = 0;
            double alphi = 0;
            double alphr = 0;
            double beta = 0;
            double xnorm = 0;
            double mx = 0;
            complex t = 0;
            double s = 0;
            complex v = 0;
            int i_ = 0;

            tau = 0;

            if( n<=0 )
            {
                tau = 0;
                return;
            }
            
            //
            // Scale if needed (to avoid overflow/underflow during intermediate
            // calculations).
            //
            mx = 0;
            for(j=1; j<=n; j++)
            {
                mx = Math.Max(math.abscomplex(x[j]), mx);
            }
            s = 1;
            if( (double)(mx)!=(double)(0) )
            {
                if( (double)(mx)<(double)(1) )
                {
                    s = Math.Sqrt(math.minrealnumber);
                    v = 1/s;
                    for(i_=1; i_<=n;i_++)
                    {
                        x[i_] = v*x[i_];
                    }
                }
                else
                {
                    s = Math.Sqrt(math.maxrealnumber);
                    v = 1/s;
                    for(i_=1; i_<=n;i_++)
                    {
                        x[i_] = v*x[i_];
                    }
                }
            }
            
            //
            // calculate
            //
            alpha = x[1];
            mx = 0;
            for(j=2; j<=n; j++)
            {
                mx = Math.Max(math.abscomplex(x[j]), mx);
            }
            xnorm = 0;
            if( (double)(mx)!=(double)(0) )
            {
                for(j=2; j<=n; j++)
                {
                    t = x[j]/mx;
                    xnorm = xnorm+(t*math.conj(t)).x;
                }
                xnorm = Math.Sqrt(xnorm)*mx;
            }
            alphr = alpha.x;
            alphi = alpha.y;
            if( (double)(xnorm)==(double)(0) && (double)(alphi)==(double)(0) )
            {
                tau = 0;
                x[1] = x[1]*s;
                return;
            }
            mx = Math.Max(Math.Abs(alphr), Math.Abs(alphi));
            mx = Math.Max(mx, Math.Abs(xnorm));
            beta = -(mx*Math.Sqrt(math.sqr(alphr/mx)+math.sqr(alphi/mx)+math.sqr(xnorm/mx)));
            if( (double)(alphr)<(double)(0) )
            {
                beta = -beta;
            }
            tau.x = (beta-alphr)/beta;
            tau.y = -(alphi/beta);
            alpha = 1/(alpha-beta);
            if( n>1 )
            {
                for(i_=2; i_<=n;i_++)
                {
                    x[i_] = alpha*x[i_];
                }
            }
            alpha = beta;
            x[1] = alpha;
            
            //
            // Scale back
            //
            x[1] = x[1]*s;
        }


        /*************************************************************************
        Application of an elementary reflection to a rectangular matrix of size MxN

        The  algorithm  pre-multiplies  the  matrix  by  an  elementary reflection
        transformation  which  is  given  by  column  V  and  scalar  Tau (see the
        description of the GenerateReflection). Not the whole matrix  but  only  a
        part of it is transformed (rows from M1 to M2, columns from N1 to N2). Only
        the elements of this submatrix are changed.

        Note: the matrix is multiplied by H, not by H'.   If  it  is  required  to
        multiply the matrix by H', it is necessary to pass Conj(Tau) instead of Tau.

        Input parameters:
            C       -   matrix to be transformed.
            Tau     -   scalar defining transformation.
            V       -   column defining transformation.
                        Array whose index ranges within [1..M2-M1+1]
            M1, M2  -   range of rows to be transformed.
            N1, N2  -   range of columns to be transformed.
            WORK    -   working array whose index goes from N1 to N2.

        Output parameters:
            C       -   the result of multiplying the input matrix C by the
                        transformation matrix which is given by Tau and V.
                        If N1>N2 or M1>M2, C is not modified.

          -- LAPACK auxiliary routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             September 30, 1994
        *************************************************************************/
        public static void complexapplyreflectionfromtheleft(ref complex[,] c,
            complex tau,
            complex[] v,
            int m1,
            int m2,
            int n1,
            int n2,
            ref complex[] work,
            alglib.xparams _params)
        {
            complex t = 0;
            int i = 0;
            int i_ = 0;

            if( (tau==0 || n1>n2) || m1>m2 )
            {
                return;
            }
            
            //
            // w := C^T * conj(v)
            //
            for(i=n1; i<=n2; i++)
            {
                work[i] = 0;
            }
            for(i=m1; i<=m2; i++)
            {
                t = math.conj(v[i+1-m1]);
                for(i_=n1; i_<=n2;i_++)
                {
                    work[i_] = work[i_] + t*c[i,i_];
                }
            }
            
            //
            // C := C - tau * v * w^T
            //
            for(i=m1; i<=m2; i++)
            {
                t = v[i-m1+1]*tau;
                for(i_=n1; i_<=n2;i_++)
                {
                    c[i,i_] = c[i,i_] - t*work[i_];
                }
            }
        }


        /*************************************************************************
        Application of an elementary reflection to a rectangular matrix of size MxN

        The  algorithm  post-multiplies  the  matrix  by  an elementary reflection
        transformation  which  is  given  by  column  V  and  scalar  Tau (see the
        description  of  the  GenerateReflection). Not the whole matrix but only a
        part  of  it  is  transformed (rows from M1 to M2, columns from N1 to N2).
        Only the elements of this submatrix are changed.

        Input parameters:
            C       -   matrix to be transformed.
            Tau     -   scalar defining transformation.
            V       -   column defining transformation.
                        Array whose index ranges within [1..N2-N1+1]
            M1, M2  -   range of rows to be transformed.
            N1, N2  -   range of columns to be transformed.
            WORK    -   working array whose index goes from M1 to M2.

        Output parameters:
            C       -   the result of multiplying the input matrix C by the
                        transformation matrix which is given by Tau and V.
                        If N1>N2 or M1>M2, C is not modified.

          -- LAPACK auxiliary routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             September 30, 1994
        *************************************************************************/
        public static void complexapplyreflectionfromtheright(ref complex[,] c,
            complex tau,
            ref complex[] v,
            int m1,
            int m2,
            int n1,
            int n2,
            ref complex[] work,
            alglib.xparams _params)
        {
            complex t = 0;
            int i = 0;
            int vm = 0;
            int i_ = 0;
            int i1_ = 0;

            if( (tau==0 || n1>n2) || m1>m2 )
            {
                return;
            }
            
            //
            // w := C * v
            //
            vm = n2-n1+1;
            for(i=m1; i<=m2; i++)
            {
                i1_ = (1)-(n1);
                t = 0.0;
                for(i_=n1; i_<=n2;i_++)
                {
                    t += c[i,i_]*v[i_+i1_];
                }
                work[i] = t;
            }
            
            //
            // C := C - w * conj(v^T)
            //
            for(i_=1; i_<=vm;i_++)
            {
                v[i_] = math.conj(v[i_]);
            }
            for(i=m1; i<=m2; i++)
            {
                t = work[i]*tau;
                i1_ = (1) - (n1);
                for(i_=n1; i_<=n2;i_++)
                {
                    c[i,i_] = c[i,i_] - t*v[i_+i1_];
                }
            }
            for(i_=1; i_<=vm;i_++)
            {
                v[i_] = math.conj(v[i_]);
            }
        }


    }
    public class rotations
    {
        /*************************************************************************
        Application of a sequence of  elementary rotations to a matrix

        The algorithm pre-multiplies the matrix by a sequence of rotation
        transformations which is given by arrays C and S. Depending on the value
        of the IsForward parameter either 1 and 2, 3 and 4 and so on (if IsForward=true)
        rows are rotated, or the rows N and N-1, N-2 and N-3 and so on, are rotated.

        Not the whole matrix but only a part of it is transformed (rows from M1 to
        M2, columns from N1 to N2). Only the elements of this submatrix are changed.

        Input parameters:
            IsForward   -   the sequence of the rotation application.
            M1,M2       -   the range of rows to be transformed.
            N1, N2      -   the range of columns to be transformed.
            C,S         -   transformation coefficients.
                            Array whose index ranges within [1..M2-M1].
            A           -   processed matrix.
            WORK        -   working array whose index ranges within [N1..N2].

        Output parameters:
            A           -   transformed matrix.

        Utility subroutine.
        *************************************************************************/
        public static void applyrotationsfromtheleft(bool isforward,
            int m1,
            int m2,
            int n1,
            int n2,
            double[] c,
            double[] s,
            double[,] a,
            double[] work,
            alglib.xparams _params)
        {
            int j = 0;
            int jp1 = 0;
            double ctemp = 0;
            double stemp = 0;
            double temp = 0;
            int i_ = 0;

            if( m1>m2 || n1>n2 )
            {
                return;
            }
            
            //
            // Form  P * A
            //
            if( isforward )
            {
                if( n1!=n2 )
                {
                    
                    //
                    // Common case: N1<>N2
                    //
                    for(j=m1; j<=m2-1; j++)
                    {
                        ctemp = c[j-m1+1];
                        stemp = s[j-m1+1];
                        if( (double)(ctemp)!=(double)(1) || (double)(stemp)!=(double)(0) )
                        {
                            jp1 = j+1;
                            for(i_=n1; i_<=n2;i_++)
                            {
                                work[i_] = ctemp*a[jp1,i_];
                            }
                            for(i_=n1; i_<=n2;i_++)
                            {
                                work[i_] = work[i_] - stemp*a[j,i_];
                            }
                            for(i_=n1; i_<=n2;i_++)
                            {
                                a[j,i_] = ctemp*a[j,i_];
                            }
                            for(i_=n1; i_<=n2;i_++)
                            {
                                a[j,i_] = a[j,i_] + stemp*a[jp1,i_];
                            }
                            for(i_=n1; i_<=n2;i_++)
                            {
                                a[jp1,i_] = work[i_];
                            }
                        }
                    }
                }
                else
                {
                    
                    //
                    // Special case: N1=N2
                    //
                    for(j=m1; j<=m2-1; j++)
                    {
                        ctemp = c[j-m1+1];
                        stemp = s[j-m1+1];
                        if( (double)(ctemp)!=(double)(1) || (double)(stemp)!=(double)(0) )
                        {
                            temp = a[j+1,n1];
                            a[j+1,n1] = ctemp*temp-stemp*a[j,n1];
                            a[j,n1] = stemp*temp+ctemp*a[j,n1];
                        }
                    }
                }
            }
            else
            {
                if( n1!=n2 )
                {
                    
                    //
                    // Common case: N1<>N2
                    //
                    for(j=m2-1; j>=m1; j--)
                    {
                        ctemp = c[j-m1+1];
                        stemp = s[j-m1+1];
                        if( (double)(ctemp)!=(double)(1) || (double)(stemp)!=(double)(0) )
                        {
                            jp1 = j+1;
                            for(i_=n1; i_<=n2;i_++)
                            {
                                work[i_] = ctemp*a[jp1,i_];
                            }
                            for(i_=n1; i_<=n2;i_++)
                            {
                                work[i_] = work[i_] - stemp*a[j,i_];
                            }
                            for(i_=n1; i_<=n2;i_++)
                            {
                                a[j,i_] = ctemp*a[j,i_];
                            }
                            for(i_=n1; i_<=n2;i_++)
                            {
                                a[j,i_] = a[j,i_] + stemp*a[jp1,i_];
                            }
                            for(i_=n1; i_<=n2;i_++)
                            {
                                a[jp1,i_] = work[i_];
                            }
                        }
                    }
                }
                else
                {
                    
                    //
                    // Special case: N1=N2
                    //
                    for(j=m2-1; j>=m1; j--)
                    {
                        ctemp = c[j-m1+1];
                        stemp = s[j-m1+1];
                        if( (double)(ctemp)!=(double)(1) || (double)(stemp)!=(double)(0) )
                        {
                            temp = a[j+1,n1];
                            a[j+1,n1] = ctemp*temp-stemp*a[j,n1];
                            a[j,n1] = stemp*temp+ctemp*a[j,n1];
                        }
                    }
                }
            }
        }


        /*************************************************************************
        Application of a sequence of  elementary rotations to a matrix

        The algorithm post-multiplies the matrix by a sequence of rotation
        transformations which is given by arrays C and S. Depending on the value
        of the IsForward parameter either 1 and 2, 3 and 4 and so on (if IsForward=true)
        rows are rotated, or the rows N and N-1, N-2 and N-3 and so on are rotated.

        Not the whole matrix but only a part of it is transformed (rows from M1
        to M2, columns from N1 to N2). Only the elements of this submatrix are changed.

        Input parameters:
            IsForward   -   the sequence of the rotation application.
            M1,M2       -   the range of rows to be transformed.
            N1, N2      -   the range of columns to be transformed.
            C,S         -   transformation coefficients.
                            Array whose index ranges within [1..N2-N1].
            A           -   processed matrix.
            WORK        -   working array whose index ranges within [M1..M2].

        Output parameters:
            A           -   transformed matrix.

        Utility subroutine.
        *************************************************************************/
        public static void applyrotationsfromtheright(bool isforward,
            int m1,
            int m2,
            int n1,
            int n2,
            double[] c,
            double[] s,
            double[,] a,
            double[] work,
            alglib.xparams _params)
        {
            int j = 0;
            int jp1 = 0;
            double ctemp = 0;
            double stemp = 0;
            double temp = 0;
            int i_ = 0;

            
            //
            // Form A * P'
            //
            if( isforward )
            {
                if( m1!=m2 )
                {
                    
                    //
                    // Common case: M1<>M2
                    //
                    for(j=n1; j<=n2-1; j++)
                    {
                        ctemp = c[j-n1+1];
                        stemp = s[j-n1+1];
                        if( (double)(ctemp)!=(double)(1) || (double)(stemp)!=(double)(0) )
                        {
                            jp1 = j+1;
                            for(i_=m1; i_<=m2;i_++)
                            {
                                work[i_] = ctemp*a[i_,jp1];
                            }
                            for(i_=m1; i_<=m2;i_++)
                            {
                                work[i_] = work[i_] - stemp*a[i_,j];
                            }
                            for(i_=m1; i_<=m2;i_++)
                            {
                                a[i_,j] = ctemp*a[i_,j];
                            }
                            for(i_=m1; i_<=m2;i_++)
                            {
                                a[i_,j] = a[i_,j] + stemp*a[i_,jp1];
                            }
                            for(i_=m1; i_<=m2;i_++)
                            {
                                a[i_,jp1] = work[i_];
                            }
                        }
                    }
                }
                else
                {
                    
                    //
                    // Special case: M1=M2
                    //
                    for(j=n1; j<=n2-1; j++)
                    {
                        ctemp = c[j-n1+1];
                        stemp = s[j-n1+1];
                        if( (double)(ctemp)!=(double)(1) || (double)(stemp)!=(double)(0) )
                        {
                            temp = a[m1,j+1];
                            a[m1,j+1] = ctemp*temp-stemp*a[m1,j];
                            a[m1,j] = stemp*temp+ctemp*a[m1,j];
                        }
                    }
                }
            }
            else
            {
                if( m1!=m2 )
                {
                    
                    //
                    // Common case: M1<>M2
                    //
                    for(j=n2-1; j>=n1; j--)
                    {
                        ctemp = c[j-n1+1];
                        stemp = s[j-n1+1];
                        if( (double)(ctemp)!=(double)(1) || (double)(stemp)!=(double)(0) )
                        {
                            jp1 = j+1;
                            for(i_=m1; i_<=m2;i_++)
                            {
                                work[i_] = ctemp*a[i_,jp1];
                            }
                            for(i_=m1; i_<=m2;i_++)
                            {
                                work[i_] = work[i_] - stemp*a[i_,j];
                            }
                            for(i_=m1; i_<=m2;i_++)
                            {
                                a[i_,j] = ctemp*a[i_,j];
                            }
                            for(i_=m1; i_<=m2;i_++)
                            {
                                a[i_,j] = a[i_,j] + stemp*a[i_,jp1];
                            }
                            for(i_=m1; i_<=m2;i_++)
                            {
                                a[i_,jp1] = work[i_];
                            }
                        }
                    }
                }
                else
                {
                    
                    //
                    // Special case: M1=M2
                    //
                    for(j=n2-1; j>=n1; j--)
                    {
                        ctemp = c[j-n1+1];
                        stemp = s[j-n1+1];
                        if( (double)(ctemp)!=(double)(1) || (double)(stemp)!=(double)(0) )
                        {
                            temp = a[m1,j+1];
                            a[m1,j+1] = ctemp*temp-stemp*a[m1,j];
                            a[m1,j] = stemp*temp+ctemp*a[m1,j];
                        }
                    }
                }
            }
        }


        /*************************************************************************
        The subroutine generates the elementary rotation, so that:

        [  CS  SN  ]  .  [ F ]  =  [ R ]
        [ -SN  CS  ]     [ G ]     [ 0 ]

        CS**2 + SN**2 = 1
        *************************************************************************/
        public static void generaterotation(double f,
            double g,
            ref double cs,
            ref double sn,
            ref double r,
            alglib.xparams _params)
        {
            double f1 = 0;
            double g1 = 0;

            cs = 0;
            sn = 0;
            r = 0;

            if( (double)(g)==(double)(0) )
            {
                cs = 1;
                sn = 0;
                r = f;
            }
            else
            {
                if( (double)(f)==(double)(0) )
                {
                    cs = 0;
                    sn = 1;
                    r = g;
                }
                else
                {
                    f1 = f;
                    g1 = g;
                    if( (double)(Math.Abs(f1))>(double)(Math.Abs(g1)) )
                    {
                        r = Math.Abs(f1)*Math.Sqrt(1+math.sqr(g1/f1));
                    }
                    else
                    {
                        r = Math.Abs(g1)*Math.Sqrt(1+math.sqr(f1/g1));
                    }
                    cs = f1/r;
                    sn = g1/r;
                    if( (double)(Math.Abs(f))>(double)(Math.Abs(g)) && (double)(cs)<(double)(0) )
                    {
                        cs = -cs;
                        sn = -sn;
                        r = -r;
                    }
                }
            }
        }


    }
    public class trlinsolve
    {
        /*************************************************************************
        Utility subroutine performing the "safe" solution of system of linear
        equations with triangular coefficient matrices.

        The subroutine uses scaling and solves the scaled system A*x=s*b (where  s
        is  a  scalar  value)  instead  of  A*x=b,  choosing  s  so  that x can be
        represented by a floating-point number. The closer the system  gets  to  a
        singular, the less s is. If the system is singular, s=0 and x contains the
        non-trivial solution of equation A*x=0.

        The feature of an algorithm is that it could not cause an  overflow  or  a
        division by zero regardless of the matrix used as the input.

        The algorithm can solve systems of equations with  upper/lower  triangular
        matrices,  with/without unit diagonal, and systems of type A*x=b or A'*x=b
        (where A' is a transposed matrix A).

        Input parameters:
            A       -   system matrix. Array whose indexes range within [0..N-1, 0..N-1].
            N       -   size of matrix A.
            X       -   right-hand member of a system.
                        Array whose index ranges within [0..N-1].
            IsUpper -   matrix type. If it is True, the system matrix is the upper
                        triangular and is located in  the  corresponding  part  of
                        matrix A.
            Trans   -   problem type. If it is True, the problem to be  solved  is
                        A'*x=b, otherwise it is A*x=b.
            Isunit  -   matrix type. If it is True, the system matrix has  a  unit
                        diagonal (the elements on the main diagonal are  not  used
                        in the calculation process), otherwise the matrix is considered
                        to be a general triangular matrix.

        Output parameters:
            X       -   solution. Array whose index ranges within [0..N-1].
            S       -   scaling factor.

          -- LAPACK auxiliary routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             June 30, 1992
        *************************************************************************/
        public static void rmatrixtrsafesolve(double[,] a,
            int n,
            ref double[] x,
            ref double s,
            bool isupper,
            bool istrans,
            bool isunit,
            alglib.xparams _params)
        {
            bool normin = new bool();
            double[] cnorm = new double[0];
            double[,] a1 = new double[0,0];
            double[] x1 = new double[0];
            int i = 0;
            int i_ = 0;
            int i1_ = 0;

            s = 0;

            
            //
            // From 0-based to 1-based
            //
            normin = false;
            a1 = new double[n+1, n+1];
            x1 = new double[n+1];
            for(i=1; i<=n; i++)
            {
                i1_ = (0) - (1);
                for(i_=1; i_<=n;i_++)
                {
                    a1[i,i_] = a[i-1,i_+i1_];
                }
            }
            i1_ = (0) - (1);
            for(i_=1; i_<=n;i_++)
            {
                x1[i_] = x[i_+i1_];
            }
            
            //
            // Solve 1-based
            //
            safesolvetriangular(a1, n, ref x1, ref s, isupper, istrans, isunit, normin, ref cnorm, _params);
            
            //
            // From 1-based to 0-based
            //
            i1_ = (1) - (0);
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = x1[i_+i1_];
            }
        }


        /*************************************************************************
        Obsolete 1-based subroutine.
        See RMatrixTRSafeSolve for 0-based replacement.
        *************************************************************************/
        public static void safesolvetriangular(double[,] a,
            int n,
            ref double[] x,
            ref double s,
            bool isupper,
            bool istrans,
            bool isunit,
            bool normin,
            ref double[] cnorm,
            alglib.xparams _params)
        {
            int i = 0;
            int imax = 0;
            int j = 0;
            int jfirst = 0;
            int jinc = 0;
            int jlast = 0;
            int jm1 = 0;
            int jp1 = 0;
            int ip1 = 0;
            int im1 = 0;
            int k = 0;
            int flg = 0;
            double v = 0;
            double vd = 0;
            double bignum = 0;
            double grow = 0;
            double rec = 0;
            double smlnum = 0;
            double sumj = 0;
            double tjj = 0;
            double tjjs = 0;
            double tmax = 0;
            double tscal = 0;
            double uscal = 0;
            double xbnd = 0;
            double xj = 0;
            double xmax = 0;
            bool notran = new bool();
            bool upper = new bool();
            bool nounit = new bool();
            int i_ = 0;

            s = 0;

            upper = isupper;
            notran = !istrans;
            nounit = !isunit;
            
            //
            // these initializers are not really necessary,
            // but without them compiler complains about uninitialized locals
            //
            tjjs = 0;
            
            //
            // Quick return if possible
            //
            if( n==0 )
            {
                return;
            }
            
            //
            // Determine machine dependent parameters to control overflow.
            //
            smlnum = math.minrealnumber/(math.machineepsilon*2);
            bignum = 1/smlnum;
            s = 1;
            if( !normin )
            {
                cnorm = new double[n+1];
                
                //
                // Compute the 1-norm of each column, not including the diagonal.
                //
                if( upper )
                {
                    
                    //
                    // A is upper triangular.
                    //
                    for(j=1; j<=n; j++)
                    {
                        v = 0;
                        for(k=1; k<=j-1; k++)
                        {
                            v = v+Math.Abs(a[k,j]);
                        }
                        cnorm[j] = v;
                    }
                }
                else
                {
                    
                    //
                    // A is lower triangular.
                    //
                    for(j=1; j<=n-1; j++)
                    {
                        v = 0;
                        for(k=j+1; k<=n; k++)
                        {
                            v = v+Math.Abs(a[k,j]);
                        }
                        cnorm[j] = v;
                    }
                    cnorm[n] = 0;
                }
            }
            
            //
            // Scale the column norms by TSCAL if the maximum element in CNORM is
            // greater than BIGNUM.
            //
            imax = 1;
            for(k=2; k<=n; k++)
            {
                if( (double)(cnorm[k])>(double)(cnorm[imax]) )
                {
                    imax = k;
                }
            }
            tmax = cnorm[imax];
            if( (double)(tmax)<=(double)(bignum) )
            {
                tscal = 1;
            }
            else
            {
                tscal = 1/(smlnum*tmax);
                for(i_=1; i_<=n;i_++)
                {
                    cnorm[i_] = tscal*cnorm[i_];
                }
            }
            
            //
            // Compute a bound on the computed solution vector to see if the
            // Level 2 BLAS routine DTRSV can be used.
            //
            j = 1;
            for(k=2; k<=n; k++)
            {
                if( (double)(Math.Abs(x[k]))>(double)(Math.Abs(x[j])) )
                {
                    j = k;
                }
            }
            xmax = Math.Abs(x[j]);
            xbnd = xmax;
            if( notran )
            {
                
                //
                // Compute the growth in A * x = b.
                //
                if( upper )
                {
                    jfirst = n;
                    jlast = 1;
                    jinc = -1;
                }
                else
                {
                    jfirst = 1;
                    jlast = n;
                    jinc = 1;
                }
                if( (double)(tscal)!=(double)(1) )
                {
                    grow = 0;
                }
                else
                {
                    if( nounit )
                    {
                        
                        //
                        // A is non-unit triangular.
                        //
                        // Compute GROW = 1/G(j) and XBND = 1/M(j).
                        // Initially, G(0) = max{x(i), i=1,...,n}.
                        //
                        grow = 1/Math.Max(xbnd, smlnum);
                        xbnd = grow;
                        j = jfirst;
                        while( (jinc>0 && j<=jlast) || (jinc<0 && j>=jlast) )
                        {
                            
                            //
                            // Exit the loop if the growth factor is too small.
                            //
                            if( (double)(grow)<=(double)(smlnum) )
                            {
                                break;
                            }
                            
                            //
                            // M(j) = G(j-1) / abs(A(j,j))
                            //
                            tjj = Math.Abs(a[j,j]);
                            xbnd = Math.Min(xbnd, Math.Min(1, tjj)*grow);
                            if( (double)(tjj+cnorm[j])>=(double)(smlnum) )
                            {
                                
                                //
                                // G(j) = G(j-1)*( 1 + CNORM(j) / abs(A(j,j)) )
                                //
                                grow = grow*(tjj/(tjj+cnorm[j]));
                            }
                            else
                            {
                                
                                //
                                // G(j) could overflow, set GROW to 0.
                                //
                                grow = 0;
                            }
                            if( j==jlast )
                            {
                                grow = xbnd;
                            }
                            j = j+jinc;
                        }
                    }
                    else
                    {
                        
                        //
                        // A is unit triangular.
                        //
                        // Compute GROW = 1/G(j), where G(0) = max{x(i), i=1,...,n}.
                        //
                        grow = Math.Min(1, 1/Math.Max(xbnd, smlnum));
                        j = jfirst;
                        while( (jinc>0 && j<=jlast) || (jinc<0 && j>=jlast) )
                        {
                            
                            //
                            // Exit the loop if the growth factor is too small.
                            //
                            if( (double)(grow)<=(double)(smlnum) )
                            {
                                break;
                            }
                            
                            //
                            // G(j) = G(j-1)*( 1 + CNORM(j) )
                            //
                            grow = grow*(1/(1+cnorm[j]));
                            j = j+jinc;
                        }
                    }
                }
            }
            else
            {
                
                //
                // Compute the growth in A' * x = b.
                //
                if( upper )
                {
                    jfirst = 1;
                    jlast = n;
                    jinc = 1;
                }
                else
                {
                    jfirst = n;
                    jlast = 1;
                    jinc = -1;
                }
                if( (double)(tscal)!=(double)(1) )
                {
                    grow = 0;
                }
                else
                {
                    if( nounit )
                    {
                        
                        //
                        // A is non-unit triangular.
                        //
                        // Compute GROW = 1/G(j) and XBND = 1/M(j).
                        // Initially, M(0) = max{x(i), i=1,...,n}.
                        //
                        grow = 1/Math.Max(xbnd, smlnum);
                        xbnd = grow;
                        j = jfirst;
                        while( (jinc>0 && j<=jlast) || (jinc<0 && j>=jlast) )
                        {
                            
                            //
                            // Exit the loop if the growth factor is too small.
                            //
                            if( (double)(grow)<=(double)(smlnum) )
                            {
                                break;
                            }
                            
                            //
                            // G(j) = max( G(j-1), M(j-1)*( 1 + CNORM(j) ) )
                            //
                            xj = 1+cnorm[j];
                            grow = Math.Min(grow, xbnd/xj);
                            
                            //
                            // M(j) = M(j-1)*( 1 + CNORM(j) ) / abs(A(j,j))
                            //
                            tjj = Math.Abs(a[j,j]);
                            if( (double)(xj)>(double)(tjj) )
                            {
                                xbnd = xbnd*(tjj/xj);
                            }
                            if( j==jlast )
                            {
                                grow = Math.Min(grow, xbnd);
                            }
                            j = j+jinc;
                        }
                    }
                    else
                    {
                        
                        //
                        // A is unit triangular.
                        //
                        // Compute GROW = 1/G(j), where G(0) = max{x(i), i=1,...,n}.
                        //
                        grow = Math.Min(1, 1/Math.Max(xbnd, smlnum));
                        j = jfirst;
                        while( (jinc>0 && j<=jlast) || (jinc<0 && j>=jlast) )
                        {
                            
                            //
                            // Exit the loop if the growth factor is too small.
                            //
                            if( (double)(grow)<=(double)(smlnum) )
                            {
                                break;
                            }
                            
                            //
                            // G(j) = ( 1 + CNORM(j) )*G(j-1)
                            //
                            xj = 1+cnorm[j];
                            grow = grow/xj;
                            j = j+jinc;
                        }
                    }
                }
            }
            if( (double)(grow*tscal)>(double)(smlnum) )
            {
                
                //
                // Use the Level 2 BLAS solve if the reciprocal of the bound on
                // elements of X is not too small.
                //
                if( (upper && notran) || (!upper && !notran) )
                {
                    if( nounit )
                    {
                        vd = a[n,n];
                    }
                    else
                    {
                        vd = 1;
                    }
                    x[n] = x[n]/vd;
                    for(i=n-1; i>=1; i--)
                    {
                        ip1 = i+1;
                        if( upper )
                        {
                            v = 0.0;
                            for(i_=ip1; i_<=n;i_++)
                            {
                                v += a[i,i_]*x[i_];
                            }
                        }
                        else
                        {
                            v = 0.0;
                            for(i_=ip1; i_<=n;i_++)
                            {
                                v += a[i_,i]*x[i_];
                            }
                        }
                        if( nounit )
                        {
                            vd = a[i,i];
                        }
                        else
                        {
                            vd = 1;
                        }
                        x[i] = (x[i]-v)/vd;
                    }
                }
                else
                {
                    if( nounit )
                    {
                        vd = a[1,1];
                    }
                    else
                    {
                        vd = 1;
                    }
                    x[1] = x[1]/vd;
                    for(i=2; i<=n; i++)
                    {
                        im1 = i-1;
                        if( upper )
                        {
                            v = 0.0;
                            for(i_=1; i_<=im1;i_++)
                            {
                                v += a[i_,i]*x[i_];
                            }
                        }
                        else
                        {
                            v = 0.0;
                            for(i_=1; i_<=im1;i_++)
                            {
                                v += a[i,i_]*x[i_];
                            }
                        }
                        if( nounit )
                        {
                            vd = a[i,i];
                        }
                        else
                        {
                            vd = 1;
                        }
                        x[i] = (x[i]-v)/vd;
                    }
                }
            }
            else
            {
                
                //
                // Use a Level 1 BLAS solve, scaling intermediate results.
                //
                if( (double)(xmax)>(double)(bignum) )
                {
                    
                    //
                    // Scale X so that its components are less than or equal to
                    // BIGNUM in absolute value.
                    //
                    s = bignum/xmax;
                    for(i_=1; i_<=n;i_++)
                    {
                        x[i_] = s*x[i_];
                    }
                    xmax = bignum;
                }
                if( notran )
                {
                    
                    //
                    // Solve A * x = b
                    //
                    j = jfirst;
                    while( (jinc>0 && j<=jlast) || (jinc<0 && j>=jlast) )
                    {
                        
                        //
                        // Compute x(j) = b(j) / A(j,j), scaling x if necessary.
                        //
                        xj = Math.Abs(x[j]);
                        flg = 0;
                        if( nounit )
                        {
                            tjjs = a[j,j]*tscal;
                        }
                        else
                        {
                            tjjs = tscal;
                            if( (double)(tscal)==(double)(1) )
                            {
                                flg = 100;
                            }
                        }
                        if( flg!=100 )
                        {
                            tjj = Math.Abs(tjjs);
                            if( (double)(tjj)>(double)(smlnum) )
                            {
                                
                                //
                                // abs(A(j,j)) > SMLNUM:
                                //
                                if( (double)(tjj)<(double)(1) )
                                {
                                    if( (double)(xj)>(double)(tjj*bignum) )
                                    {
                                        
                                        //
                                        // Scale x by 1/b(j).
                                        //
                                        rec = 1/xj;
                                        for(i_=1; i_<=n;i_++)
                                        {
                                            x[i_] = rec*x[i_];
                                        }
                                        s = s*rec;
                                        xmax = xmax*rec;
                                    }
                                }
                                x[j] = x[j]/tjjs;
                                xj = Math.Abs(x[j]);
                            }
                            else
                            {
                                if( (double)(tjj)>(double)(0) )
                                {
                                    
                                    //
                                    // 0 < abs(A(j,j)) <= SMLNUM:
                                    //
                                    if( (double)(xj)>(double)(tjj*bignum) )
                                    {
                                        
                                        //
                                        // Scale x by (1/abs(x(j)))*abs(A(j,j))*BIGNUM
                                        // to avoid overflow when dividing by A(j,j).
                                        //
                                        rec = tjj*bignum/xj;
                                        if( (double)(cnorm[j])>(double)(1) )
                                        {
                                            
                                            //
                                            // Scale by 1/CNORM(j) to avoid overflow when
                                            // multiplying x(j) times column j.
                                            //
                                            rec = rec/cnorm[j];
                                        }
                                        for(i_=1; i_<=n;i_++)
                                        {
                                            x[i_] = rec*x[i_];
                                        }
                                        s = s*rec;
                                        xmax = xmax*rec;
                                    }
                                    x[j] = x[j]/tjjs;
                                    xj = Math.Abs(x[j]);
                                }
                                else
                                {
                                    
                                    //
                                    // A(j,j) = 0:  Set x(1:n) = 0, x(j) = 1, and
                                    // scale = 0, and compute a solution to A*x = 0.
                                    //
                                    for(i=1; i<=n; i++)
                                    {
                                        x[i] = 0;
                                    }
                                    x[j] = 1;
                                    xj = 1;
                                    s = 0;
                                    xmax = 0;
                                }
                            }
                        }
                        
                        //
                        // Scale x if necessary to avoid overflow when adding a
                        // multiple of column j of A.
                        //
                        if( (double)(xj)>(double)(1) )
                        {
                            rec = 1/xj;
                            if( (double)(cnorm[j])>(double)((bignum-xmax)*rec) )
                            {
                                
                                //
                                // Scale x by 1/(2*abs(x(j))).
                                //
                                rec = rec*0.5;
                                for(i_=1; i_<=n;i_++)
                                {
                                    x[i_] = rec*x[i_];
                                }
                                s = s*rec;
                            }
                        }
                        else
                        {
                            if( (double)(xj*cnorm[j])>(double)(bignum-xmax) )
                            {
                                
                                //
                                // Scale x by 1/2.
                                //
                                for(i_=1; i_<=n;i_++)
                                {
                                    x[i_] = 0.5*x[i_];
                                }
                                s = s*0.5;
                            }
                        }
                        if( upper )
                        {
                            if( j>1 )
                            {
                                
                                //
                                // Compute the update
                                // x(1:j-1) := x(1:j-1) - x(j) * A(1:j-1,j)
                                //
                                v = x[j]*tscal;
                                jm1 = j-1;
                                for(i_=1; i_<=jm1;i_++)
                                {
                                    x[i_] = x[i_] - v*a[i_,j];
                                }
                                i = 1;
                                for(k=2; k<=j-1; k++)
                                {
                                    if( (double)(Math.Abs(x[k]))>(double)(Math.Abs(x[i])) )
                                    {
                                        i = k;
                                    }
                                }
                                xmax = Math.Abs(x[i]);
                            }
                        }
                        else
                        {
                            if( j<n )
                            {
                                
                                //
                                // Compute the update
                                // x(j+1:n) := x(j+1:n) - x(j) * A(j+1:n,j)
                                //
                                jp1 = j+1;
                                v = x[j]*tscal;
                                for(i_=jp1; i_<=n;i_++)
                                {
                                    x[i_] = x[i_] - v*a[i_,j];
                                }
                                i = j+1;
                                for(k=j+2; k<=n; k++)
                                {
                                    if( (double)(Math.Abs(x[k]))>(double)(Math.Abs(x[i])) )
                                    {
                                        i = k;
                                    }
                                }
                                xmax = Math.Abs(x[i]);
                            }
                        }
                        j = j+jinc;
                    }
                }
                else
                {
                    
                    //
                    // Solve A' * x = b
                    //
                    j = jfirst;
                    while( (jinc>0 && j<=jlast) || (jinc<0 && j>=jlast) )
                    {
                        
                        //
                        // Compute x(j) = b(j) - sum A(k,j)*x(k).
                        //   k<>j
                        //
                        xj = Math.Abs(x[j]);
                        uscal = tscal;
                        rec = 1/Math.Max(xmax, 1);
                        if( (double)(cnorm[j])>(double)((bignum-xj)*rec) )
                        {
                            
                            //
                            // If x(j) could overflow, scale x by 1/(2*XMAX).
                            //
                            rec = rec*0.5;
                            if( nounit )
                            {
                                tjjs = a[j,j]*tscal;
                            }
                            else
                            {
                                tjjs = tscal;
                            }
                            tjj = Math.Abs(tjjs);
                            if( (double)(tjj)>(double)(1) )
                            {
                                
                                //
                                // Divide by A(j,j) when scaling x if A(j,j) > 1.
                                //
                                rec = Math.Min(1, rec*tjj);
                                uscal = uscal/tjjs;
                            }
                            if( (double)(rec)<(double)(1) )
                            {
                                for(i_=1; i_<=n;i_++)
                                {
                                    x[i_] = rec*x[i_];
                                }
                                s = s*rec;
                                xmax = xmax*rec;
                            }
                        }
                        sumj = 0;
                        if( (double)(uscal)==(double)(1) )
                        {
                            
                            //
                            // If the scaling needed for A in the dot product is 1,
                            // call DDOT to perform the dot product.
                            //
                            if( upper )
                            {
                                if( j>1 )
                                {
                                    jm1 = j-1;
                                    sumj = 0.0;
                                    for(i_=1; i_<=jm1;i_++)
                                    {
                                        sumj += a[i_,j]*x[i_];
                                    }
                                }
                                else
                                {
                                    sumj = 0;
                                }
                            }
                            else
                            {
                                if( j<n )
                                {
                                    jp1 = j+1;
                                    sumj = 0.0;
                                    for(i_=jp1; i_<=n;i_++)
                                    {
                                        sumj += a[i_,j]*x[i_];
                                    }
                                }
                            }
                        }
                        else
                        {
                            
                            //
                            // Otherwise, use in-line code for the dot product.
                            //
                            if( upper )
                            {
                                for(i=1; i<=j-1; i++)
                                {
                                    v = a[i,j]*uscal;
                                    sumj = sumj+v*x[i];
                                }
                            }
                            else
                            {
                                if( j<n )
                                {
                                    for(i=j+1; i<=n; i++)
                                    {
                                        v = a[i,j]*uscal;
                                        sumj = sumj+v*x[i];
                                    }
                                }
                            }
                        }
                        if( (double)(uscal)==(double)(tscal) )
                        {
                            
                            //
                            // Compute x(j) := ( x(j) - sumj ) / A(j,j) if 1/A(j,j)
                            // was not used to scale the dotproduct.
                            //
                            x[j] = x[j]-sumj;
                            xj = Math.Abs(x[j]);
                            flg = 0;
                            if( nounit )
                            {
                                tjjs = a[j,j]*tscal;
                            }
                            else
                            {
                                tjjs = tscal;
                                if( (double)(tscal)==(double)(1) )
                                {
                                    flg = 150;
                                }
                            }
                            
                            //
                            // Compute x(j) = x(j) / A(j,j), scaling if necessary.
                            //
                            if( flg!=150 )
                            {
                                tjj = Math.Abs(tjjs);
                                if( (double)(tjj)>(double)(smlnum) )
                                {
                                    
                                    //
                                    // abs(A(j,j)) > SMLNUM:
                                    //
                                    if( (double)(tjj)<(double)(1) )
                                    {
                                        if( (double)(xj)>(double)(tjj*bignum) )
                                        {
                                            
                                            //
                                            // Scale X by 1/abs(x(j)).
                                            //
                                            rec = 1/xj;
                                            for(i_=1; i_<=n;i_++)
                                            {
                                                x[i_] = rec*x[i_];
                                            }
                                            s = s*rec;
                                            xmax = xmax*rec;
                                        }
                                    }
                                    x[j] = x[j]/tjjs;
                                }
                                else
                                {
                                    if( (double)(tjj)>(double)(0) )
                                    {
                                        
                                        //
                                        // 0 < abs(A(j,j)) <= SMLNUM:
                                        //
                                        if( (double)(xj)>(double)(tjj*bignum) )
                                        {
                                            
                                            //
                                            // Scale x by (1/abs(x(j)))*abs(A(j,j))*BIGNUM.
                                            //
                                            rec = tjj*bignum/xj;
                                            for(i_=1; i_<=n;i_++)
                                            {
                                                x[i_] = rec*x[i_];
                                            }
                                            s = s*rec;
                                            xmax = xmax*rec;
                                        }
                                        x[j] = x[j]/tjjs;
                                    }
                                    else
                                    {
                                        
                                        //
                                        // A(j,j) = 0:  Set x(1:n) = 0, x(j) = 1, and
                                        // scale = 0, and compute a solution to A'*x = 0.
                                        //
                                        for(i=1; i<=n; i++)
                                        {
                                            x[i] = 0;
                                        }
                                        x[j] = 1;
                                        s = 0;
                                        xmax = 0;
                                    }
                                }
                            }
                        }
                        else
                        {
                            
                            //
                            // Compute x(j) := x(j) / A(j,j)  - sumj if the dot
                            // product has already been divided by 1/A(j,j).
                            //
                            x[j] = x[j]/tjjs-sumj;
                        }
                        xmax = Math.Max(xmax, Math.Abs(x[j]));
                        j = j+jinc;
                    }
                }
                s = s/tscal;
            }
            
            //
            // Scale the column norms by 1/TSCAL for return.
            //
            if( (double)(tscal)!=(double)(1) )
            {
                v = 1/tscal;
                for(i_=1; i_<=n;i_++)
                {
                    cnorm[i_] = v*cnorm[i_];
                }
            }
        }


    }
    public class safesolve
    {
        /*************************************************************************
        Real implementation of CMatrixScaledTRSafeSolve

          -- ALGLIB routine --
             21.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixscaledtrsafesolve(double[,] a,
            double sa,
            int n,
            ref double[] x,
            bool isupper,
            int trans,
            bool isunit,
            double maxgrowth,
            alglib.xparams _params)
        {
            bool result = new bool();
            double lnmax = 0;
            double nrmb = 0;
            double nrmx = 0;
            int i = 0;
            complex alpha = 0;
            complex beta = 0;
            double vr = 0;
            complex cx = 0;
            double[] tmp = new double[0];
            int i_ = 0;

            alglib.ap.assert(n>0, "RMatrixTRSafeSolve: incorrect N!");
            alglib.ap.assert(trans==0 || trans==1, "RMatrixTRSafeSolve: incorrect Trans!");
            result = true;
            lnmax = Math.Log(math.maxrealnumber);
            
            //
            // Quick return if possible
            //
            if( n<=0 )
            {
                return result;
            }
            
            //
            // Load norms: right part and X
            //
            nrmb = 0;
            for(i=0; i<=n-1; i++)
            {
                nrmb = Math.Max(nrmb, Math.Abs(x[i]));
            }
            nrmx = 0;
            
            //
            // Solve
            //
            tmp = new double[n];
            result = true;
            if( isupper && trans==0 )
            {
                
                //
                // U*x = b
                //
                for(i=n-1; i>=0; i--)
                {
                    
                    //
                    // Task is reduced to alpha*x[i] = beta
                    //
                    if( isunit )
                    {
                        alpha = sa;
                    }
                    else
                    {
                        alpha = a[i,i]*sa;
                    }
                    if( i<n-1 )
                    {
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            tmp[i_] = sa*a[i,i_];
                        }
                        vr = 0.0;
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            vr += tmp[i_]*x[i_];
                        }
                        beta = x[i]-vr;
                    }
                    else
                    {
                        beta = x[i];
                    }
                    
                    //
                    // solve alpha*x[i] = beta
                    //
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, ref nrmx, ref cx, _params);
                    if( !result )
                    {
                        return result;
                    }
                    x[i] = cx.x;
                }
                return result;
            }
            if( !isupper && trans==0 )
            {
                
                //
                // L*x = b
                //
                for(i=0; i<=n-1; i++)
                {
                    
                    //
                    // Task is reduced to alpha*x[i] = beta
                    //
                    if( isunit )
                    {
                        alpha = sa;
                    }
                    else
                    {
                        alpha = a[i,i]*sa;
                    }
                    if( i>0 )
                    {
                        for(i_=0; i_<=i-1;i_++)
                        {
                            tmp[i_] = sa*a[i,i_];
                        }
                        vr = 0.0;
                        for(i_=0; i_<=i-1;i_++)
                        {
                            vr += tmp[i_]*x[i_];
                        }
                        beta = x[i]-vr;
                    }
                    else
                    {
                        beta = x[i];
                    }
                    
                    //
                    // solve alpha*x[i] = beta
                    //
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, ref nrmx, ref cx, _params);
                    if( !result )
                    {
                        return result;
                    }
                    x[i] = cx.x;
                }
                return result;
            }
            if( isupper && trans==1 )
            {
                
                //
                // U^T*x = b
                //
                for(i=0; i<=n-1; i++)
                {
                    
                    //
                    // Task is reduced to alpha*x[i] = beta
                    //
                    if( isunit )
                    {
                        alpha = sa;
                    }
                    else
                    {
                        alpha = a[i,i]*sa;
                    }
                    beta = x[i];
                    
                    //
                    // solve alpha*x[i] = beta
                    //
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, ref nrmx, ref cx, _params);
                    if( !result )
                    {
                        return result;
                    }
                    x[i] = cx.x;
                    
                    //
                    // update the rest of right part
                    //
                    if( i<n-1 )
                    {
                        vr = cx.x;
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            tmp[i_] = sa*a[i,i_];
                        }
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            x[i_] = x[i_] - vr*tmp[i_];
                        }
                    }
                }
                return result;
            }
            if( !isupper && trans==1 )
            {
                
                //
                // L^T*x = b
                //
                for(i=n-1; i>=0; i--)
                {
                    
                    //
                    // Task is reduced to alpha*x[i] = beta
                    //
                    if( isunit )
                    {
                        alpha = sa;
                    }
                    else
                    {
                        alpha = a[i,i]*sa;
                    }
                    beta = x[i];
                    
                    //
                    // solve alpha*x[i] = beta
                    //
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, ref nrmx, ref cx, _params);
                    if( !result )
                    {
                        return result;
                    }
                    x[i] = cx.x;
                    
                    //
                    // update the rest of right part
                    //
                    if( i>0 )
                    {
                        vr = cx.x;
                        for(i_=0; i_<=i-1;i_++)
                        {
                            tmp[i_] = sa*a[i,i_];
                        }
                        for(i_=0; i_<=i-1;i_++)
                        {
                            x[i_] = x[i_] - vr*tmp[i_];
                        }
                    }
                }
                return result;
            }
            result = false;
            return result;
        }


        /*************************************************************************
        Internal subroutine for safe solution of

            SA*op(A)=b
            
        where  A  is  NxN  upper/lower  triangular/unitriangular  matrix, op(A) is
        either identity transform, transposition or Hermitian transposition, SA is
        a scaling factor such that max(|SA*A[i,j]|) is close to 1.0 in magnutude.

        This subroutine  limits  relative  growth  of  solution  (in inf-norm)  by
        MaxGrowth,  returning  False  if  growth  exceeds MaxGrowth. Degenerate or
        near-degenerate matrices are handled correctly (False is returned) as long
        as MaxGrowth is significantly less than MaxRealNumber/norm(b).

          -- ALGLIB routine --
             21.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool cmatrixscaledtrsafesolve(complex[,] a,
            double sa,
            int n,
            ref complex[] x,
            bool isupper,
            int trans,
            bool isunit,
            double maxgrowth,
            alglib.xparams _params)
        {
            bool result = new bool();
            double lnmax = 0;
            double nrmb = 0;
            double nrmx = 0;
            int i = 0;
            complex alpha = 0;
            complex beta = 0;
            complex vc = 0;
            complex[] tmp = new complex[0];
            int i_ = 0;

            alglib.ap.assert(n>0, "CMatrixTRSafeSolve: incorrect N!");
            alglib.ap.assert((trans==0 || trans==1) || trans==2, "CMatrixTRSafeSolve: incorrect Trans!");
            result = true;
            lnmax = Math.Log(math.maxrealnumber);
            
            //
            // Quick return if possible
            //
            if( n<=0 )
            {
                return result;
            }
            
            //
            // Load norms: right part and X
            //
            nrmb = 0;
            for(i=0; i<=n-1; i++)
            {
                nrmb = Math.Max(nrmb, math.abscomplex(x[i]));
            }
            nrmx = 0;
            
            //
            // Solve
            //
            tmp = new complex[n];
            result = true;
            if( isupper && trans==0 )
            {
                
                //
                // U*x = b
                //
                for(i=n-1; i>=0; i--)
                {
                    
                    //
                    // Task is reduced to alpha*x[i] = beta
                    //
                    if( isunit )
                    {
                        alpha = sa;
                    }
                    else
                    {
                        alpha = a[i,i]*sa;
                    }
                    if( i<n-1 )
                    {
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            tmp[i_] = sa*a[i,i_];
                        }
                        vc = 0.0;
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            vc += tmp[i_]*x[i_];
                        }
                        beta = x[i]-vc;
                    }
                    else
                    {
                        beta = x[i];
                    }
                    
                    //
                    // solve alpha*x[i] = beta
                    //
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, ref nrmx, ref vc, _params);
                    if( !result )
                    {
                        return result;
                    }
                    x[i] = vc;
                }
                return result;
            }
            if( !isupper && trans==0 )
            {
                
                //
                // L*x = b
                //
                for(i=0; i<=n-1; i++)
                {
                    
                    //
                    // Task is reduced to alpha*x[i] = beta
                    //
                    if( isunit )
                    {
                        alpha = sa;
                    }
                    else
                    {
                        alpha = a[i,i]*sa;
                    }
                    if( i>0 )
                    {
                        for(i_=0; i_<=i-1;i_++)
                        {
                            tmp[i_] = sa*a[i,i_];
                        }
                        vc = 0.0;
                        for(i_=0; i_<=i-1;i_++)
                        {
                            vc += tmp[i_]*x[i_];
                        }
                        beta = x[i]-vc;
                    }
                    else
                    {
                        beta = x[i];
                    }
                    
                    //
                    // solve alpha*x[i] = beta
                    //
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, ref nrmx, ref vc, _params);
                    if( !result )
                    {
                        return result;
                    }
                    x[i] = vc;
                }
                return result;
            }
            if( isupper && trans==1 )
            {
                
                //
                // U^T*x = b
                //
                for(i=0; i<=n-1; i++)
                {
                    
                    //
                    // Task is reduced to alpha*x[i] = beta
                    //
                    if( isunit )
                    {
                        alpha = sa;
                    }
                    else
                    {
                        alpha = a[i,i]*sa;
                    }
                    beta = x[i];
                    
                    //
                    // solve alpha*x[i] = beta
                    //
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, ref nrmx, ref vc, _params);
                    if( !result )
                    {
                        return result;
                    }
                    x[i] = vc;
                    
                    //
                    // update the rest of right part
                    //
                    if( i<n-1 )
                    {
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            tmp[i_] = sa*a[i,i_];
                        }
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            x[i_] = x[i_] - vc*tmp[i_];
                        }
                    }
                }
                return result;
            }
            if( !isupper && trans==1 )
            {
                
                //
                // L^T*x = b
                //
                for(i=n-1; i>=0; i--)
                {
                    
                    //
                    // Task is reduced to alpha*x[i] = beta
                    //
                    if( isunit )
                    {
                        alpha = sa;
                    }
                    else
                    {
                        alpha = a[i,i]*sa;
                    }
                    beta = x[i];
                    
                    //
                    // solve alpha*x[i] = beta
                    //
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, ref nrmx, ref vc, _params);
                    if( !result )
                    {
                        return result;
                    }
                    x[i] = vc;
                    
                    //
                    // update the rest of right part
                    //
                    if( i>0 )
                    {
                        for(i_=0; i_<=i-1;i_++)
                        {
                            tmp[i_] = sa*a[i,i_];
                        }
                        for(i_=0; i_<=i-1;i_++)
                        {
                            x[i_] = x[i_] - vc*tmp[i_];
                        }
                    }
                }
                return result;
            }
            if( isupper && trans==2 )
            {
                
                //
                // U^H*x = b
                //
                for(i=0; i<=n-1; i++)
                {
                    
                    //
                    // Task is reduced to alpha*x[i] = beta
                    //
                    if( isunit )
                    {
                        alpha = sa;
                    }
                    else
                    {
                        alpha = math.conj(a[i,i])*sa;
                    }
                    beta = x[i];
                    
                    //
                    // solve alpha*x[i] = beta
                    //
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, ref nrmx, ref vc, _params);
                    if( !result )
                    {
                        return result;
                    }
                    x[i] = vc;
                    
                    //
                    // update the rest of right part
                    //
                    if( i<n-1 )
                    {
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            tmp[i_] = sa*math.conj(a[i,i_]);
                        }
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            x[i_] = x[i_] - vc*tmp[i_];
                        }
                    }
                }
                return result;
            }
            if( !isupper && trans==2 )
            {
                
                //
                // L^T*x = b
                //
                for(i=n-1; i>=0; i--)
                {
                    
                    //
                    // Task is reduced to alpha*x[i] = beta
                    //
                    if( isunit )
                    {
                        alpha = sa;
                    }
                    else
                    {
                        alpha = math.conj(a[i,i])*sa;
                    }
                    beta = x[i];
                    
                    //
                    // solve alpha*x[i] = beta
                    //
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, ref nrmx, ref vc, _params);
                    if( !result )
                    {
                        return result;
                    }
                    x[i] = vc;
                    
                    //
                    // update the rest of right part
                    //
                    if( i>0 )
                    {
                        for(i_=0; i_<=i-1;i_++)
                        {
                            tmp[i_] = sa*math.conj(a[i,i_]);
                        }
                        for(i_=0; i_<=i-1;i_++)
                        {
                            x[i_] = x[i_] - vc*tmp[i_];
                        }
                    }
                }
                return result;
            }
            result = false;
            return result;
        }


        /*************************************************************************
        complex basic solver-updater for reduced linear system

            alpha*x[i] = beta

        solves this equation and updates it in overlfow-safe manner (keeping track
        of relative growth of solution).

        Parameters:
            Alpha   -   alpha
            Beta    -   beta
            LnMax   -   precomputed Ln(MaxRealNumber)
            BNorm   -   inf-norm of b (right part of original system)
            MaxGrowth-  maximum growth of norm(x) relative to norm(b)
            XNorm   -   inf-norm of other components of X (which are already processed)
                        it is updated by CBasicSolveAndUpdate.
            X       -   solution

          -- ALGLIB routine --
             26.01.2009
             Bochkanov Sergey
        *************************************************************************/
        private static bool cbasicsolveandupdate(complex alpha,
            complex beta,
            double lnmax,
            double bnorm,
            double maxgrowth,
            ref double xnorm,
            ref complex x,
            alglib.xparams _params)
        {
            bool result = new bool();
            double v = 0;

            x = 0;

            result = false;
            if( alpha==0 )
            {
                return result;
            }
            if( beta!=0 )
            {
                
                //
                // alpha*x[i]=beta
                //
                v = Math.Log(math.abscomplex(beta))-Math.Log(math.abscomplex(alpha));
                if( (double)(v)>(double)(lnmax) )
                {
                    return result;
                }
                x = beta/alpha;
            }
            else
            {
                
                //
                // alpha*x[i]=0
                //
                x = 0;
            }
            
            //
            // update NrmX, test growth limit
            //
            xnorm = Math.Max(xnorm, math.abscomplex(x));
            if( (double)(xnorm)>(double)(maxgrowth*bnorm) )
            {
                return result;
            }
            result = true;
            return result;
        }


    }
    public class hblas
    {
        public static void hermitianmatrixvectormultiply(complex[,] a,
            bool isupper,
            int i1,
            int i2,
            complex[] x,
            complex alpha,
            ref complex[] y,
            alglib.xparams _params)
        {
            int i = 0;
            int ba1 = 0;
            int by1 = 0;
            int by2 = 0;
            int bx1 = 0;
            int bx2 = 0;
            int n = 0;
            complex v = 0;
            int i_ = 0;
            int i1_ = 0;

            n = i2-i1+1;
            if( n<=0 )
            {
                return;
            }
            
            //
            // Let A = L + D + U, where
            //  L is strictly lower triangular (main diagonal is zero)
            //  D is diagonal
            //  U is strictly upper triangular (main diagonal is zero)
            //
            // A*x = L*x + D*x + U*x
            //
            // Calculate D*x first
            //
            for(i=i1; i<=i2; i++)
            {
                y[i-i1+1] = a[i,i]*x[i-i1+1];
            }
            
            //
            // Add L*x + U*x
            //
            if( isupper )
            {
                for(i=i1; i<=i2-1; i++)
                {
                    
                    //
                    // Add L*x to the result
                    //
                    v = x[i-i1+1];
                    by1 = i-i1+2;
                    by2 = n;
                    ba1 = i+1;
                    i1_ = (ba1) - (by1);
                    for(i_=by1; i_<=by2;i_++)
                    {
                        y[i_] = y[i_] + v*math.conj(a[i,i_+i1_]);
                    }
                    
                    //
                    // Add U*x to the result
                    //
                    bx1 = i-i1+2;
                    bx2 = n;
                    ba1 = i+1;
                    i1_ = (ba1)-(bx1);
                    v = 0.0;
                    for(i_=bx1; i_<=bx2;i_++)
                    {
                        v += x[i_]*a[i,i_+i1_];
                    }
                    y[i-i1+1] = y[i-i1+1]+v;
                }
            }
            else
            {
                for(i=i1+1; i<=i2; i++)
                {
                    
                    //
                    // Add L*x to the result
                    //
                    bx1 = 1;
                    bx2 = i-i1;
                    ba1 = i1;
                    i1_ = (ba1)-(bx1);
                    v = 0.0;
                    for(i_=bx1; i_<=bx2;i_++)
                    {
                        v += x[i_]*a[i,i_+i1_];
                    }
                    y[i-i1+1] = y[i-i1+1]+v;
                    
                    //
                    // Add U*x to the result
                    //
                    v = x[i-i1+1];
                    by1 = 1;
                    by2 = i-i1;
                    ba1 = i1;
                    i1_ = (ba1) - (by1);
                    for(i_=by1; i_<=by2;i_++)
                    {
                        y[i_] = y[i_] + v*math.conj(a[i,i_+i1_]);
                    }
                }
            }
            for(i_=1; i_<=n;i_++)
            {
                y[i_] = alpha*y[i_];
            }
        }


        public static void hermitianrank2update(ref complex[,] a,
            bool isupper,
            int i1,
            int i2,
            complex[] x,
            complex[] y,
            ref complex[] t,
            complex alpha,
            alglib.xparams _params)
        {
            int i = 0;
            int tp1 = 0;
            int tp2 = 0;
            complex v = 0;
            int i_ = 0;
            int i1_ = 0;

            if( isupper )
            {
                for(i=i1; i<=i2; i++)
                {
                    tp1 = i+1-i1;
                    tp2 = i2-i1+1;
                    v = alpha*x[i+1-i1];
                    for(i_=tp1; i_<=tp2;i_++)
                    {
                        t[i_] = v*math.conj(y[i_]);
                    }
                    v = math.conj(alpha)*y[i+1-i1];
                    for(i_=tp1; i_<=tp2;i_++)
                    {
                        t[i_] = t[i_] + v*math.conj(x[i_]);
                    }
                    i1_ = (tp1) - (i);
                    for(i_=i; i_<=i2;i_++)
                    {
                        a[i,i_] = a[i,i_] + t[i_+i1_];
                    }
                }
            }
            else
            {
                for(i=i1; i<=i2; i++)
                {
                    tp1 = 1;
                    tp2 = i+1-i1;
                    v = alpha*x[i+1-i1];
                    for(i_=tp1; i_<=tp2;i_++)
                    {
                        t[i_] = v*math.conj(y[i_]);
                    }
                    v = math.conj(alpha)*y[i+1-i1];
                    for(i_=tp1; i_<=tp2;i_++)
                    {
                        t[i_] = t[i_] + v*math.conj(x[i_]);
                    }
                    i1_ = (tp1) - (i1);
                    for(i_=i1; i_<=i;i_++)
                    {
                        a[i,i_] = a[i,i_] + t[i_+i1_];
                    }
                }
            }
        }


    }
    public class sblas
    {
        public static void symmetricmatrixvectormultiply(double[,] a,
            bool isupper,
            int i1,
            int i2,
            double[] x,
            double alpha,
            ref double[] y,
            alglib.xparams _params)
        {
            int i = 0;
            int ba1 = 0;
            int ba2 = 0;
            int by1 = 0;
            int by2 = 0;
            int bx1 = 0;
            int bx2 = 0;
            int n = 0;
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            n = i2-i1+1;
            if( n<=0 )
            {
                return;
            }
            
            //
            // Let A = L + D + U, where
            //  L is strictly lower triangular (main diagonal is zero)
            //  D is diagonal
            //  U is strictly upper triangular (main diagonal is zero)
            //
            // A*x = L*x + D*x + U*x
            //
            // Calculate D*x first
            //
            for(i=i1; i<=i2; i++)
            {
                y[i-i1+1] = a[i,i]*x[i-i1+1];
            }
            
            //
            // Add L*x + U*x
            //
            if( isupper )
            {
                for(i=i1; i<=i2-1; i++)
                {
                    
                    //
                    // Add L*x to the result
                    //
                    v = x[i-i1+1];
                    by1 = i-i1+2;
                    by2 = n;
                    ba1 = i+1;
                    ba2 = i2;
                    i1_ = (ba1) - (by1);
                    for(i_=by1; i_<=by2;i_++)
                    {
                        y[i_] = y[i_] + v*a[i,i_+i1_];
                    }
                    
                    //
                    // Add U*x to the result
                    //
                    bx1 = i-i1+2;
                    bx2 = n;
                    ba1 = i+1;
                    ba2 = i2;
                    i1_ = (ba1)-(bx1);
                    v = 0.0;
                    for(i_=bx1; i_<=bx2;i_++)
                    {
                        v += x[i_]*a[i,i_+i1_];
                    }
                    y[i-i1+1] = y[i-i1+1]+v;
                }
            }
            else
            {
                for(i=i1+1; i<=i2; i++)
                {
                    
                    //
                    // Add L*x to the result
                    //
                    bx1 = 1;
                    bx2 = i-i1;
                    ba1 = i1;
                    ba2 = i-1;
                    i1_ = (ba1)-(bx1);
                    v = 0.0;
                    for(i_=bx1; i_<=bx2;i_++)
                    {
                        v += x[i_]*a[i,i_+i1_];
                    }
                    y[i-i1+1] = y[i-i1+1]+v;
                    
                    //
                    // Add U*x to the result
                    //
                    v = x[i-i1+1];
                    by1 = 1;
                    by2 = i-i1;
                    ba1 = i1;
                    ba2 = i-1;
                    i1_ = (ba1) - (by1);
                    for(i_=by1; i_<=by2;i_++)
                    {
                        y[i_] = y[i_] + v*a[i,i_+i1_];
                    }
                }
            }
            for(i_=1; i_<=n;i_++)
            {
                y[i_] = alpha*y[i_];
            }
            apserv.touchint(ref ba2, _params);
        }


        public static void symmetricrank2update(ref double[,] a,
            bool isupper,
            int i1,
            int i2,
            double[] x,
            double[] y,
            ref double[] t,
            double alpha,
            alglib.xparams _params)
        {
            int i = 0;
            int tp1 = 0;
            int tp2 = 0;
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            if( isupper )
            {
                for(i=i1; i<=i2; i++)
                {
                    tp1 = i+1-i1;
                    tp2 = i2-i1+1;
                    v = x[i+1-i1];
                    for(i_=tp1; i_<=tp2;i_++)
                    {
                        t[i_] = v*y[i_];
                    }
                    v = y[i+1-i1];
                    for(i_=tp1; i_<=tp2;i_++)
                    {
                        t[i_] = t[i_] + v*x[i_];
                    }
                    for(i_=tp1; i_<=tp2;i_++)
                    {
                        t[i_] = alpha*t[i_];
                    }
                    i1_ = (tp1) - (i);
                    for(i_=i; i_<=i2;i_++)
                    {
                        a[i,i_] = a[i,i_] + t[i_+i1_];
                    }
                }
            }
            else
            {
                for(i=i1; i<=i2; i++)
                {
                    tp1 = 1;
                    tp2 = i+1-i1;
                    v = x[i+1-i1];
                    for(i_=tp1; i_<=tp2;i_++)
                    {
                        t[i_] = v*y[i_];
                    }
                    v = y[i+1-i1];
                    for(i_=tp1; i_<=tp2;i_++)
                    {
                        t[i_] = t[i_] + v*x[i_];
                    }
                    for(i_=tp1; i_<=tp2;i_++)
                    {
                        t[i_] = alpha*t[i_];
                    }
                    i1_ = (tp1) - (i1);
                    for(i_=i1; i_<=i;i_++)
                    {
                        a[i,i_] = a[i,i_] + t[i_+i1_];
                    }
                }
            }
        }


    }
    public class blas
    {
        public static double vectornorm2(double[] x,
            int i1,
            int i2,
            alglib.xparams _params)
        {
            double result = 0;
            int n = 0;
            int ix = 0;
            double absxi = 0;
            double scl = 0;
            double ssq = 0;

            n = i2-i1+1;
            if( n<1 )
            {
                result = 0;
                return result;
            }
            if( n==1 )
            {
                result = Math.Abs(x[i1]);
                return result;
            }
            scl = 0;
            ssq = 1;
            for(ix=i1; ix<=i2; ix++)
            {
                if( (double)(x[ix])!=(double)(0) )
                {
                    absxi = Math.Abs(x[ix]);
                    if( (double)(scl)<(double)(absxi) )
                    {
                        ssq = 1+ssq*math.sqr(scl/absxi);
                        scl = absxi;
                    }
                    else
                    {
                        ssq = ssq+math.sqr(absxi/scl);
                    }
                }
            }
            result = scl*Math.Sqrt(ssq);
            return result;
        }


        public static int vectoridxabsmax(double[] x,
            int i1,
            int i2,
            alglib.xparams _params)
        {
            int result = 0;
            int i = 0;

            result = i1;
            for(i=i1+1; i<=i2; i++)
            {
                if( (double)(Math.Abs(x[i]))>(double)(Math.Abs(x[result])) )
                {
                    result = i;
                }
            }
            return result;
        }


        public static int columnidxabsmax(double[,] x,
            int i1,
            int i2,
            int j,
            alglib.xparams _params)
        {
            int result = 0;
            int i = 0;

            result = i1;
            for(i=i1+1; i<=i2; i++)
            {
                if( (double)(Math.Abs(x[i,j]))>(double)(Math.Abs(x[result,j])) )
                {
                    result = i;
                }
            }
            return result;
        }


        public static int rowidxabsmax(double[,] x,
            int j1,
            int j2,
            int i,
            alglib.xparams _params)
        {
            int result = 0;
            int j = 0;

            result = j1;
            for(j=j1+1; j<=j2; j++)
            {
                if( (double)(Math.Abs(x[i,j]))>(double)(Math.Abs(x[i,result])) )
                {
                    result = j;
                }
            }
            return result;
        }


        public static double upperhessenberg1norm(double[,] a,
            int i1,
            int i2,
            int j1,
            int j2,
            ref double[] work,
            alglib.xparams _params)
        {
            double result = 0;
            int i = 0;
            int j = 0;

            alglib.ap.assert(i2-i1==j2-j1, "UpperHessenberg1Norm: I2-I1<>J2-J1!");
            for(j=j1; j<=j2; j++)
            {
                work[j] = 0;
            }
            for(i=i1; i<=i2; i++)
            {
                for(j=Math.Max(j1, j1+i-i1-1); j<=j2; j++)
                {
                    work[j] = work[j]+Math.Abs(a[i,j]);
                }
            }
            result = 0;
            for(j=j1; j<=j2; j++)
            {
                result = Math.Max(result, work[j]);
            }
            return result;
        }


        public static void copymatrix(double[,] a,
            int is1,
            int is2,
            int js1,
            int js2,
            ref double[,] b,
            int id1,
            int id2,
            int jd1,
            int jd2,
            alglib.xparams _params)
        {
            int isrc = 0;
            int idst = 0;
            int i_ = 0;
            int i1_ = 0;

            if( is1>is2 || js1>js2 )
            {
                return;
            }
            alglib.ap.assert(is2-is1==id2-id1, "CopyMatrix: different sizes!");
            alglib.ap.assert(js2-js1==jd2-jd1, "CopyMatrix: different sizes!");
            for(isrc=is1; isrc<=is2; isrc++)
            {
                idst = isrc-is1+id1;
                i1_ = (js1) - (jd1);
                for(i_=jd1; i_<=jd2;i_++)
                {
                    b[idst,i_] = a[isrc,i_+i1_];
                }
            }
        }


        public static void inplacetranspose(ref double[,] a,
            int i1,
            int i2,
            int j1,
            int j2,
            ref double[] work,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;
            int ips = 0;
            int jps = 0;
            int l = 0;
            int i_ = 0;
            int i1_ = 0;

            if( i1>i2 || j1>j2 )
            {
                return;
            }
            alglib.ap.assert(i1-i2==j1-j2, "InplaceTranspose error: incorrect array size!");
            for(i=i1; i<=i2-1; i++)
            {
                j = j1+i-i1;
                ips = i+1;
                jps = j1+ips-i1;
                l = i2-i;
                i1_ = (ips) - (1);
                for(i_=1; i_<=l;i_++)
                {
                    work[i_] = a[i_+i1_,j];
                }
                i1_ = (jps) - (ips);
                for(i_=ips; i_<=i2;i_++)
                {
                    a[i_,j] = a[i,i_+i1_];
                }
                i1_ = (1) - (jps);
                for(i_=jps; i_<=j2;i_++)
                {
                    a[i,i_] = work[i_+i1_];
                }
            }
        }


        public static void copyandtranspose(double[,] a,
            int is1,
            int is2,
            int js1,
            int js2,
            ref double[,] b,
            int id1,
            int id2,
            int jd1,
            int jd2,
            alglib.xparams _params)
        {
            int isrc = 0;
            int jdst = 0;
            int i_ = 0;
            int i1_ = 0;

            if( is1>is2 || js1>js2 )
            {
                return;
            }
            alglib.ap.assert(is2-is1==jd2-jd1, "CopyAndTranspose: different sizes!");
            alglib.ap.assert(js2-js1==id2-id1, "CopyAndTranspose: different sizes!");
            for(isrc=is1; isrc<=is2; isrc++)
            {
                jdst = isrc-is1+jd1;
                i1_ = (js1) - (id1);
                for(i_=id1; i_<=id2;i_++)
                {
                    b[i_,jdst] = a[isrc,i_+i1_];
                }
            }
        }


        public static void matrixvectormultiply(double[,] a,
            int i1,
            int i2,
            int j1,
            int j2,
            bool trans,
            double[] x,
            int ix1,
            int ix2,
            double alpha,
            ref double[] y,
            int iy1,
            int iy2,
            double beta,
            alglib.xparams _params)
        {
            int i = 0;
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            if( !trans )
            {
                
                //
                // y := alpha*A*x + beta*y;
                //
                if( i1>i2 || j1>j2 )
                {
                    return;
                }
                alglib.ap.assert(j2-j1==ix2-ix1, "MatrixVectorMultiply: A and X dont match!");
                alglib.ap.assert(i2-i1==iy2-iy1, "MatrixVectorMultiply: A and Y dont match!");
                
                //
                // beta*y
                //
                if( (double)(beta)==(double)(0) )
                {
                    for(i=iy1; i<=iy2; i++)
                    {
                        y[i] = 0;
                    }
                }
                else
                {
                    for(i_=iy1; i_<=iy2;i_++)
                    {
                        y[i_] = beta*y[i_];
                    }
                }
                
                //
                // alpha*A*x
                //
                for(i=i1; i<=i2; i++)
                {
                    i1_ = (ix1)-(j1);
                    v = 0.0;
                    for(i_=j1; i_<=j2;i_++)
                    {
                        v += a[i,i_]*x[i_+i1_];
                    }
                    y[iy1+i-i1] = y[iy1+i-i1]+alpha*v;
                }
            }
            else
            {
                
                //
                // y := alpha*A'*x + beta*y;
                //
                if( i1>i2 || j1>j2 )
                {
                    return;
                }
                alglib.ap.assert(i2-i1==ix2-ix1, "MatrixVectorMultiply: A and X dont match!");
                alglib.ap.assert(j2-j1==iy2-iy1, "MatrixVectorMultiply: A and Y dont match!");
                
                //
                // beta*y
                //
                if( (double)(beta)==(double)(0) )
                {
                    for(i=iy1; i<=iy2; i++)
                    {
                        y[i] = 0;
                    }
                }
                else
                {
                    for(i_=iy1; i_<=iy2;i_++)
                    {
                        y[i_] = beta*y[i_];
                    }
                }
                
                //
                // alpha*A'*x
                //
                for(i=i1; i<=i2; i++)
                {
                    v = alpha*x[ix1+i-i1];
                    i1_ = (j1) - (iy1);
                    for(i_=iy1; i_<=iy2;i_++)
                    {
                        y[i_] = y[i_] + v*a[i,i_+i1_];
                    }
                }
            }
        }


        public static double pythag2(double x,
            double y,
            alglib.xparams _params)
        {
            double result = 0;
            double w = 0;
            double xabs = 0;
            double yabs = 0;
            double z = 0;

            xabs = Math.Abs(x);
            yabs = Math.Abs(y);
            w = Math.Max(xabs, yabs);
            z = Math.Min(xabs, yabs);
            if( (double)(z)==(double)(0) )
            {
                result = w;
            }
            else
            {
                result = w*Math.Sqrt(1+math.sqr(z/w));
            }
            return result;
        }


        public static void matrixmatrixmultiply(double[,] a,
            int ai1,
            int ai2,
            int aj1,
            int aj2,
            bool transa,
            double[,] b,
            int bi1,
            int bi2,
            int bj1,
            int bj2,
            bool transb,
            double alpha,
            ref double[,] c,
            int ci1,
            int ci2,
            int cj1,
            int cj2,
            double beta,
            ref double[] work,
            alglib.xparams _params)
        {
            int arows = 0;
            int acols = 0;
            int brows = 0;
            int bcols = 0;
            int crows = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int l = 0;
            int r = 0;
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Setup
            //
            if( !transa )
            {
                arows = ai2-ai1+1;
                acols = aj2-aj1+1;
            }
            else
            {
                arows = aj2-aj1+1;
                acols = ai2-ai1+1;
            }
            if( !transb )
            {
                brows = bi2-bi1+1;
                bcols = bj2-bj1+1;
            }
            else
            {
                brows = bj2-bj1+1;
                bcols = bi2-bi1+1;
            }
            alglib.ap.assert(acols==brows, "MatrixMatrixMultiply: incorrect matrix sizes!");
            if( ((arows<=0 || acols<=0) || brows<=0) || bcols<=0 )
            {
                return;
            }
            crows = arows;
            
            //
            // Test WORK
            //
            i = Math.Max(arows, acols);
            i = Math.Max(brows, i);
            i = Math.Max(i, bcols);
            work[1] = 0;
            work[i] = 0;
            
            //
            // Prepare C
            //
            if( (double)(beta)==(double)(0) )
            {
                for(i=ci1; i<=ci2; i++)
                {
                    for(j=cj1; j<=cj2; j++)
                    {
                        c[i,j] = 0;
                    }
                }
            }
            else
            {
                for(i=ci1; i<=ci2; i++)
                {
                    for(i_=cj1; i_<=cj2;i_++)
                    {
                        c[i,i_] = beta*c[i,i_];
                    }
                }
            }
            
            //
            // A*B
            //
            if( !transa && !transb )
            {
                for(l=ai1; l<=ai2; l++)
                {
                    for(r=bi1; r<=bi2; r++)
                    {
                        v = alpha*a[l,aj1+r-bi1];
                        k = ci1+l-ai1;
                        i1_ = (bj1) - (cj1);
                        for(i_=cj1; i_<=cj2;i_++)
                        {
                            c[k,i_] = c[k,i_] + v*b[r,i_+i1_];
                        }
                    }
                }
                return;
            }
            
            //
            // A*B'
            //
            if( !transa && transb )
            {
                if( arows*acols<brows*bcols )
                {
                    for(r=bi1; r<=bi2; r++)
                    {
                        for(l=ai1; l<=ai2; l++)
                        {
                            i1_ = (bj1)-(aj1);
                            v = 0.0;
                            for(i_=aj1; i_<=aj2;i_++)
                            {
                                v += a[l,i_]*b[r,i_+i1_];
                            }
                            c[ci1+l-ai1,cj1+r-bi1] = c[ci1+l-ai1,cj1+r-bi1]+alpha*v;
                        }
                    }
                    return;
                }
                else
                {
                    for(l=ai1; l<=ai2; l++)
                    {
                        for(r=bi1; r<=bi2; r++)
                        {
                            i1_ = (bj1)-(aj1);
                            v = 0.0;
                            for(i_=aj1; i_<=aj2;i_++)
                            {
                                v += a[l,i_]*b[r,i_+i1_];
                            }
                            c[ci1+l-ai1,cj1+r-bi1] = c[ci1+l-ai1,cj1+r-bi1]+alpha*v;
                        }
                    }
                    return;
                }
            }
            
            //
            // A'*B
            //
            if( transa && !transb )
            {
                for(l=aj1; l<=aj2; l++)
                {
                    for(r=bi1; r<=bi2; r++)
                    {
                        v = alpha*a[ai1+r-bi1,l];
                        k = ci1+l-aj1;
                        i1_ = (bj1) - (cj1);
                        for(i_=cj1; i_<=cj2;i_++)
                        {
                            c[k,i_] = c[k,i_] + v*b[r,i_+i1_];
                        }
                    }
                }
                return;
            }
            
            //
            // A'*B'
            //
            if( transa && transb )
            {
                if( arows*acols<brows*bcols )
                {
                    for(r=bi1; r<=bi2; r++)
                    {
                        k = cj1+r-bi1;
                        for(i=1; i<=crows; i++)
                        {
                            work[i] = 0.0;
                        }
                        for(l=ai1; l<=ai2; l++)
                        {
                            v = alpha*b[r,bj1+l-ai1];
                            i1_ = (aj1) - (1);
                            for(i_=1; i_<=crows;i_++)
                            {
                                work[i_] = work[i_] + v*a[l,i_+i1_];
                            }
                        }
                        i1_ = (1) - (ci1);
                        for(i_=ci1; i_<=ci2;i_++)
                        {
                            c[i_,k] = c[i_,k] + work[i_+i1_];
                        }
                    }
                    return;
                }
                else
                {
                    for(l=aj1; l<=aj2; l++)
                    {
                        k = ai2-ai1+1;
                        i1_ = (ai1) - (1);
                        for(i_=1; i_<=k;i_++)
                        {
                            work[i_] = a[i_+i1_,l];
                        }
                        for(r=bi1; r<=bi2; r++)
                        {
                            i1_ = (bj1)-(1);
                            v = 0.0;
                            for(i_=1; i_<=k;i_++)
                            {
                                v += work[i_]*b[r,i_+i1_];
                            }
                            c[ci1+l-aj1,cj1+r-bi1] = c[ci1+l-aj1,cj1+r-bi1]+alpha*v;
                        }
                    }
                    return;
                }
            }
        }


    }
    public class linmin
    {
        public class linminstate : apobject
        {
            public bool brackt;
            public bool stage1;
            public int infoc;
            public double dg;
            public double dgm;
            public double dginit;
            public double dgtest;
            public double dgx;
            public double dgxm;
            public double dgy;
            public double dgym;
            public double finit;
            public double ftest1;
            public double fm;
            public double fx;
            public double fxm;
            public double fy;
            public double fym;
            public double stx;
            public double sty;
            public double stmin;
            public double stmax;
            public double width;
            public double width1;
            public double xtrapf;
            public linminstate()
            {
                init();
            }
            public override void init()
            {
            }
            public override alglib.apobject make_copy()
            {
                linminstate _result = new linminstate();
                _result.brackt = brackt;
                _result.stage1 = stage1;
                _result.infoc = infoc;
                _result.dg = dg;
                _result.dgm = dgm;
                _result.dginit = dginit;
                _result.dgtest = dgtest;
                _result.dgx = dgx;
                _result.dgxm = dgxm;
                _result.dgy = dgy;
                _result.dgym = dgym;
                _result.finit = finit;
                _result.ftest1 = ftest1;
                _result.fm = fm;
                _result.fx = fx;
                _result.fxm = fxm;
                _result.fy = fy;
                _result.fym = fym;
                _result.stx = stx;
                _result.sty = sty;
                _result.stmin = stmin;
                _result.stmax = stmax;
                _result.width = width;
                _result.width1 = width1;
                _result.xtrapf = xtrapf;
                return _result;
            }
        };


        public class armijostate : apobject
        {
            public bool needf;
            public double[] x;
            public double f;
            public int n;
            public double[] xbase;
            public double[] s;
            public double stplen;
            public double fcur;
            public double stpmax;
            public int fmax;
            public int nfev;
            public int info;
            public rcommstate rstate;
            public armijostate()
            {
                init();
            }
            public override void init()
            {
                x = new double[0];
                xbase = new double[0];
                s = new double[0];
                rstate = new rcommstate();
            }
            public override alglib.apobject make_copy()
            {
                armijostate _result = new armijostate();
                _result.needf = needf;
                _result.x = (double[])x.Clone();
                _result.f = f;
                _result.n = n;
                _result.xbase = (double[])xbase.Clone();
                _result.s = (double[])s.Clone();
                _result.stplen = stplen;
                _result.fcur = fcur;
                _result.stpmax = stpmax;
                _result.fmax = fmax;
                _result.nfev = nfev;
                _result.info = info;
                _result.rstate = (rcommstate)rstate.make_copy();
                return _result;
            }
        };




        public const double ftol = 0.001;
        public const double xtol = 100*math.machineepsilon;
        public const int maxfev = 20;
        public const double stpmin = 1.0E-50;
        public const double defstpmax = 1.0E+50;
        public const double armijofactor = 1.3;


        /*************************************************************************
        Normalizes direction/step pair: makes |D|=1, scales Stp.
        If |D|=0, it returns, leavind D/Stp unchanged.

          -- ALGLIB --
             Copyright 01.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void linminnormalized(ref double[] d,
            ref double stp,
            int n,
            alglib.xparams _params)
        {
            double mx = 0;
            double s = 0;
            int i = 0;
            int i_ = 0;

            
            //
            // first, scale D to avoid underflow/overflow durng squaring
            //
            mx = 0;
            for(i=0; i<=n-1; i++)
            {
                mx = Math.Max(mx, Math.Abs(d[i]));
            }
            if( (double)(mx)==(double)(0) )
            {
                return;
            }
            s = 1/mx;
            for(i_=0; i_<=n-1;i_++)
            {
                d[i_] = s*d[i_];
            }
            stp = stp/s;
            
            //
            // normalize D
            //
            s = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                s += d[i_]*d[i_];
            }
            s = 1/Math.Sqrt(s);
            for(i_=0; i_<=n-1;i_++)
            {
                d[i_] = s*d[i_];
            }
            stp = stp/s;
        }


        /*************************************************************************
        THE  PURPOSE  OF  MCSRCH  IS  TO  FIND A STEP WHICH SATISFIES A SUFFICIENT
        DECREASE CONDITION AND A CURVATURE CONDITION.

        AT EACH STAGE THE SUBROUTINE  UPDATES  AN  INTERVAL  OF  UNCERTAINTY  WITH
        ENDPOINTS  STX  AND  STY.  THE INTERVAL OF UNCERTAINTY IS INITIALLY CHOSEN
        SO THAT IT CONTAINS A MINIMIZER OF THE MODIFIED FUNCTION

            F(X+STP*S) - F(X) - FTOL*STP*(GRADF(X)'S).

        IF  A STEP  IS OBTAINED FOR  WHICH THE MODIFIED FUNCTION HAS A NONPOSITIVE
        FUNCTION  VALUE  AND  NONNEGATIVE  DERIVATIVE,   THEN   THE   INTERVAL  OF
        UNCERTAINTY IS CHOSEN SO THAT IT CONTAINS A MINIMIZER OF F(X+STP*S).

        THE  ALGORITHM  IS  DESIGNED TO FIND A STEP WHICH SATISFIES THE SUFFICIENT
        DECREASE CONDITION

            F(X+STP*S) .LE. F(X) + FTOL*STP*(GRADF(X)'S),

        AND THE CURVATURE CONDITION

            ABS(GRADF(X+STP*S)'S)) .LE. GTOL*ABS(GRADF(X)'S).

        IF  FTOL  IS  LESS  THAN GTOL AND IF, FOR EXAMPLE, THE FUNCTION IS BOUNDED
        BELOW,  THEN  THERE  IS  ALWAYS  A  STEP  WHICH SATISFIES BOTH CONDITIONS.
        IF  NO  STEP  CAN BE FOUND  WHICH  SATISFIES  BOTH  CONDITIONS,  THEN  THE
        ALGORITHM  USUALLY STOPS  WHEN  ROUNDING ERRORS  PREVENT FURTHER PROGRESS.
        IN THIS CASE STP ONLY SATISFIES THE SUFFICIENT DECREASE CONDITION.


        :::::::::::::IMPORTANT NOTES:::::::::::::

        NOTE 1:

        This routine  guarantees that it will stop at the last point where function
        value was calculated. It won't make several additional function evaluations
        after finding good point. So if you store function evaluations requested by
        this routine, you can be sure that last one is the point where we've stopped.

        NOTE 2:

        when 0<StpMax<StpMin, algorithm will terminate with INFO=5 and Stp=StpMax

        NOTE 3:

        this algorithm guarantees that, if MCINFO=1 or MCINFO=5, then:
        * F(final_point)<F(initial_point) - strict inequality
        * final_point<>initial_point - after rounding to machine precision

        NOTE 4:

        when non-descent direction is specified, algorithm stops with MCINFO=0,
        Stp=0 and initial point at X[].
        :::::::::::::::::::::::::::::::::::::::::


        PARAMETERS DESCRIPRION

        STAGE IS ZERO ON FIRST CALL, ZERO ON FINAL EXIT

        N IS A POSITIVE INTEGER INPUT VARIABLE SET TO THE NUMBER OF VARIABLES.

        X IS  AN  ARRAY  OF  LENGTH N. ON INPUT IT MUST CONTAIN THE BASE POINT FOR
        THE LINE SEARCH. ON OUTPUT IT CONTAINS X+STP*S.

        F IS  A  VARIABLE. ON INPUT IT MUST CONTAIN THE VALUE OF F AT X. ON OUTPUT
        IT CONTAINS THE VALUE OF F AT X + STP*S.

        G IS AN ARRAY OF LENGTH N. ON INPUT IT MUST CONTAIN THE GRADIENT OF F AT X.
        ON OUTPUT IT CONTAINS THE GRADIENT OF F AT X + STP*S.

        S IS AN INPUT ARRAY OF LENGTH N WHICH SPECIFIES THE SEARCH DIRECTION.

        STP  IS  A NONNEGATIVE VARIABLE. ON INPUT STP CONTAINS AN INITIAL ESTIMATE
        OF A SATISFACTORY STEP. ON OUTPUT STP CONTAINS THE FINAL ESTIMATE.

        FTOL AND GTOL ARE NONNEGATIVE INPUT VARIABLES. TERMINATION OCCURS WHEN THE
        SUFFICIENT DECREASE CONDITION AND THE DIRECTIONAL DERIVATIVE CONDITION ARE
        SATISFIED.

        XTOL IS A NONNEGATIVE INPUT VARIABLE. TERMINATION OCCURS WHEN THE RELATIVE
        WIDTH OF THE INTERVAL OF UNCERTAINTY IS AT MOST XTOL.

        STPMIN AND STPMAX ARE NONNEGATIVE INPUT VARIABLES WHICH SPECIFY LOWER  AND
        UPPER BOUNDS FOR THE STEP.

        MAXFEV IS A POSITIVE INTEGER INPUT VARIABLE. TERMINATION OCCURS WHEN THE
        NUMBER OF CALLS TO FCN IS AT LEAST MAXFEV BY THE END OF AN ITERATION.

        INFO IS AN INTEGER OUTPUT VARIABLE SET AS FOLLOWS:
            INFO = 0  IMPROPER INPUT PARAMETERS.

            INFO = 1  THE SUFFICIENT DECREASE CONDITION AND THE
                      DIRECTIONAL DERIVATIVE CONDITION HOLD.

            INFO = 2  RELATIVE WIDTH OF THE INTERVAL OF UNCERTAINTY
                      IS AT MOST XTOL.

            INFO = 3  NUMBER OF CALLS TO FCN HAS REACHED MAXFEV.

            INFO = 4  THE STEP IS AT THE LOWER BOUND STPMIN.

            INFO = 5  THE STEP IS AT THE UPPER BOUND STPMAX.

            INFO = 6  ROUNDING ERRORS PREVENT FURTHER PROGRESS.
                      THERE MAY NOT BE A STEP WHICH SATISFIES THE
                      SUFFICIENT DECREASE AND CURVATURE CONDITIONS.
                      TOLERANCES MAY BE TOO SMALL.

        NFEV IS AN INTEGER OUTPUT VARIABLE SET TO THE NUMBER OF CALLS TO FCN.

        WA IS A WORK ARRAY OF LENGTH N.

        ARGONNE NATIONAL LABORATORY. MINPACK PROJECT. JUNE 1983
        JORGE J. MORE', DAVID J. THUENTE
        *************************************************************************/
        public static void mcsrch(int n,
            ref double[] x,
            ref double f,
            ref double[] g,
            double[] s,
            ref double stp,
            double stpmax,
            double gtol,
            ref int info,
            ref int nfev,
            ref double[] wa,
            linminstate state,
            ref int stage,
            alglib.xparams _params)
        {
            int i = 0;
            double v = 0;
            double p5 = 0;
            double p66 = 0;
            double zero = 0;
            int i_ = 0;

            
            //
            // init
            //
            p5 = 0.5;
            p66 = 0.66;
            state.xtrapf = 4.0;
            zero = 0;
            if( (double)(stpmax)==(double)(0) )
            {
                stpmax = defstpmax;
            }
            if( (double)(stp)<(double)(stpmin) )
            {
                stp = stpmin;
            }
            if( (double)(stp)>(double)(stpmax) )
            {
                stp = stpmax;
            }
            
            //
            // Main cycle
            //
            while( true )
            {
                if( stage==0 )
                {
                    
                    //
                    // NEXT
                    //
                    stage = 2;
                    continue;
                }
                if( stage==2 )
                {
                    state.infoc = 1;
                    info = 0;
                    
                    //
                    //     CHECK THE INPUT PARAMETERS FOR ERRORS.
                    //
                    if( (double)(stpmax)<(double)(stpmin) && (double)(stpmax)>(double)(0) )
                    {
                        info = 5;
                        stp = stpmax;
                        stage = 0;
                        return;
                    }
                    if( ((((((n<=0 || (double)(stp)<=(double)(0)) || (double)(ftol)<(double)(0)) || (double)(gtol)<(double)(zero)) || (double)(xtol)<(double)(zero)) || (double)(stpmin)<(double)(zero)) || (double)(stpmax)<(double)(stpmin)) || maxfev<=0 )
                    {
                        stage = 0;
                        return;
                    }
                    
                    //
                    //     COMPUTE THE INITIAL GRADIENT IN THE SEARCH DIRECTION
                    //     AND CHECK THAT S IS A DESCENT DIRECTION.
                    //
                    v = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        v += g[i_]*s[i_];
                    }
                    state.dginit = v;
                    if( (double)(state.dginit)>=(double)(0) )
                    {
                        stage = 0;
                        stp = 0;
                        return;
                    }
                    
                    //
                    //     INITIALIZE LOCAL VARIABLES.
                    //
                    state.brackt = false;
                    state.stage1 = true;
                    nfev = 0;
                    state.finit = f;
                    state.dgtest = ftol*state.dginit;
                    state.width = stpmax-stpmin;
                    state.width1 = state.width/p5;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        wa[i_] = x[i_];
                    }
                    
                    //
                    //     THE VARIABLES STX, FX, DGX CONTAIN THE VALUES OF THE STEP,
                    //     FUNCTION, AND DIRECTIONAL DERIVATIVE AT THE BEST STEP.
                    //     THE VARIABLES STY, FY, DGY CONTAIN THE VALUE OF THE STEP,
                    //     FUNCTION, AND DERIVATIVE AT THE OTHER ENDPOINT OF
                    //     THE INTERVAL OF UNCERTAINTY.
                    //     THE VARIABLES STP, F, DG CONTAIN THE VALUES OF THE STEP,
                    //     FUNCTION, AND DERIVATIVE AT THE CURRENT STEP.
                    //
                    state.stx = 0;
                    state.fx = state.finit;
                    state.dgx = state.dginit;
                    state.sty = 0;
                    state.fy = state.finit;
                    state.dgy = state.dginit;
                    
                    //
                    // NEXT
                    //
                    stage = 3;
                    continue;
                }
                if( stage==3 )
                {
                    
                    //
                    //     START OF ITERATION.
                    //
                    //     SET THE MINIMUM AND MAXIMUM STEPS TO CORRESPOND
                    //     TO THE PRESENT INTERVAL OF UNCERTAINTY.
                    //
                    if( state.brackt )
                    {
                        if( (double)(state.stx)<(double)(state.sty) )
                        {
                            state.stmin = state.stx;
                            state.stmax = state.sty;
                        }
                        else
                        {
                            state.stmin = state.sty;
                            state.stmax = state.stx;
                        }
                    }
                    else
                    {
                        state.stmin = state.stx;
                        state.stmax = stp+state.xtrapf*(stp-state.stx);
                    }
                    
                    //
                    //        FORCE THE STEP TO BE WITHIN THE BOUNDS STPMAX AND STPMIN.
                    //
                    if( (double)(stp)>(double)(stpmax) )
                    {
                        stp = stpmax;
                    }
                    if( (double)(stp)<(double)(stpmin) )
                    {
                        stp = stpmin;
                    }
                    
                    //
                    //        IF AN UNUSUAL TERMINATION IS TO OCCUR THEN LET
                    //        STP BE THE LOWEST POINT OBTAINED SO FAR.
                    //
                    if( (((state.brackt && ((double)(stp)<=(double)(state.stmin) || (double)(stp)>=(double)(state.stmax))) || nfev>=maxfev-1) || state.infoc==0) || (state.brackt && (double)(state.stmax-state.stmin)<=(double)(xtol*state.stmax)) )
                    {
                        stp = state.stx;
                    }
                    
                    //
                    //        EVALUATE THE FUNCTION AND GRADIENT AT STP
                    //        AND COMPUTE THE DIRECTIONAL DERIVATIVE.
                    //
                    for(i_=0; i_<=n-1;i_++)
                    {
                        x[i_] = wa[i_];
                    }
                    for(i_=0; i_<=n-1;i_++)
                    {
                        x[i_] = x[i_] + stp*s[i_];
                    }
                    
                    //
                    // NEXT
                    //
                    stage = 4;
                    return;
                }
                if( stage==4 )
                {
                    info = 0;
                    nfev = nfev+1;
                    v = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        v += g[i_]*s[i_];
                    }
                    state.dg = v;
                    state.ftest1 = state.finit+stp*state.dgtest;
                    
                    //
                    //        TEST FOR CONVERGENCE.
                    //
                    if( (state.brackt && ((double)(stp)<=(double)(state.stmin) || (double)(stp)>=(double)(state.stmax))) || state.infoc==0 )
                    {
                        info = 6;
                    }
                    if( (((double)(stp)==(double)(stpmax) && (double)(f)<(double)(state.finit)) && (double)(f)<=(double)(state.ftest1)) && (double)(state.dg)<=(double)(state.dgtest) )
                    {
                        info = 5;
                    }
                    if( (double)(stp)==(double)(stpmin) && (((double)(f)>=(double)(state.finit) || (double)(f)>(double)(state.ftest1)) || (double)(state.dg)>=(double)(state.dgtest)) )
                    {
                        info = 4;
                    }
                    if( nfev>=maxfev )
                    {
                        info = 3;
                    }
                    if( state.brackt && (double)(state.stmax-state.stmin)<=(double)(xtol*state.stmax) )
                    {
                        info = 2;
                    }
                    if( ((double)(f)<(double)(state.finit) && (double)(f)<=(double)(state.ftest1)) && (double)(Math.Abs(state.dg))<=(double)(-(gtol*state.dginit)) )
                    {
                        info = 1;
                    }
                    
                    //
                    //        CHECK FOR TERMINATION.
                    //
                    if( info!=0 )
                    {
                        
                        //
                        // Check guarantees provided by the function for INFO=1 or INFO=5
                        //
                        if( info==1 || info==5 )
                        {
                            v = 0.0;
                            for(i=0; i<=n-1; i++)
                            {
                                v = v+(wa[i]-x[i])*(wa[i]-x[i]);
                            }
                            if( (double)(f)>=(double)(state.finit) || (double)(v)==(double)(0.0) )
                            {
                                info = 6;
                            }
                        }
                        stage = 0;
                        return;
                    }
                    
                    //
                    //        IN THE FIRST STAGE WE SEEK A STEP FOR WHICH THE MODIFIED
                    //        FUNCTION HAS A NONPOSITIVE VALUE AND NONNEGATIVE DERIVATIVE.
                    //
                    if( (state.stage1 && (double)(f)<=(double)(state.ftest1)) && (double)(state.dg)>=(double)(Math.Min(ftol, gtol)*state.dginit) )
                    {
                        state.stage1 = false;
                    }
                    
                    //
                    //        A MODIFIED FUNCTION IS USED TO PREDICT THE STEP ONLY IF
                    //        WE HAVE NOT OBTAINED A STEP FOR WHICH THE MODIFIED
                    //        FUNCTION HAS A NONPOSITIVE FUNCTION VALUE AND NONNEGATIVE
                    //        DERIVATIVE, AND IF A LOWER FUNCTION VALUE HAS BEEN
                    //        OBTAINED BUT THE DECREASE IS NOT SUFFICIENT.
                    //
                    if( (state.stage1 && (double)(f)<=(double)(state.fx)) && (double)(f)>(double)(state.ftest1) )
                    {
                        
                        //
                        //           DEFINE THE MODIFIED FUNCTION AND DERIVATIVE VALUES.
                        //
                        state.fm = f-stp*state.dgtest;
                        state.fxm = state.fx-state.stx*state.dgtest;
                        state.fym = state.fy-state.sty*state.dgtest;
                        state.dgm = state.dg-state.dgtest;
                        state.dgxm = state.dgx-state.dgtest;
                        state.dgym = state.dgy-state.dgtest;
                        
                        //
                        //           CALL CSTEP TO UPDATE THE INTERVAL OF UNCERTAINTY
                        //           AND TO COMPUTE THE NEW STEP.
                        //
                        mcstep(ref state.stx, ref state.fxm, ref state.dgxm, ref state.sty, ref state.fym, ref state.dgym, ref stp, state.fm, state.dgm, ref state.brackt, state.stmin, state.stmax, ref state.infoc, _params);
                        
                        //
                        //           RESET THE FUNCTION AND GRADIENT VALUES FOR F.
                        //
                        state.fx = state.fxm+state.stx*state.dgtest;
                        state.fy = state.fym+state.sty*state.dgtest;
                        state.dgx = state.dgxm+state.dgtest;
                        state.dgy = state.dgym+state.dgtest;
                    }
                    else
                    {
                        
                        //
                        //           CALL MCSTEP TO UPDATE THE INTERVAL OF UNCERTAINTY
                        //           AND TO COMPUTE THE NEW STEP.
                        //
                        mcstep(ref state.stx, ref state.fx, ref state.dgx, ref state.sty, ref state.fy, ref state.dgy, ref stp, f, state.dg, ref state.brackt, state.stmin, state.stmax, ref state.infoc, _params);
                    }
                    
                    //
                    //        FORCE A SUFFICIENT DECREASE IN THE SIZE OF THE
                    //        INTERVAL OF UNCERTAINTY.
                    //
                    if( state.brackt )
                    {
                        if( (double)(Math.Abs(state.sty-state.stx))>=(double)(p66*state.width1) )
                        {
                            stp = state.stx+p5*(state.sty-state.stx);
                        }
                        state.width1 = state.width;
                        state.width = Math.Abs(state.sty-state.stx);
                    }
                    
                    //
                    //  NEXT.
                    //
                    stage = 3;
                    continue;
                }
            }
        }


        /*************************************************************************
        These functions perform Armijo line search using  at  most  FMAX  function
        evaluations.  It  doesn't  enforce  some  kind  of  " sufficient decrease"
        criterion - it just tries different Armijo steps and returns optimum found
        so far.

        Optimization is done using F-rcomm interface:
        * ArmijoCreate initializes State structure
          (reusing previously allocated buffers)
        * ArmijoIteration is subsequently called
        * ArmijoResults returns results

        INPUT PARAMETERS:
            N       -   problem size
            X       -   array[N], starting point
            F       -   F(X+S*STP)
            S       -   step direction, S>0
            STP     -   step length
            STPMAX  -   maximum value for STP or zero (if no limit is imposed)
            FMAX    -   maximum number of function evaluations
            State   -   optimization state

          -- ALGLIB --
             Copyright 05.10.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void armijocreate(int n,
            double[] x,
            double f,
            double[] s,
            double stp,
            double stpmax,
            int fmax,
            armijostate state,
            alglib.xparams _params)
        {
            int i_ = 0;

            if( alglib.ap.len(state.x)<n )
            {
                state.x = new double[n];
            }
            if( alglib.ap.len(state.xbase)<n )
            {
                state.xbase = new double[n];
            }
            if( alglib.ap.len(state.s)<n )
            {
                state.s = new double[n];
            }
            state.stpmax = stpmax;
            state.fmax = fmax;
            state.stplen = stp;
            state.fcur = f;
            state.n = n;
            for(i_=0; i_<=n-1;i_++)
            {
                state.xbase[i_] = x[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.s[i_] = s[i_];
            }
            state.rstate.ia = new int[0+1];
            state.rstate.ra = new double[0+1];
            state.rstate.stage = -1;
        }


        /*************************************************************************
        This is rcomm-based search function

          -- ALGLIB --
             Copyright 05.10.2010 by Bochkanov Sergey
        *************************************************************************/
        public static bool armijoiteration(armijostate state,
            alglib.xparams _params)
        {
            bool result = new bool();
            double v = 0;
            int n = 0;
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
                v = state.rstate.ra[0];
            }
            else
            {
                n = 359;
                v = -58;
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
            if( state.rstate.stage==3 )
            {
                goto lbl_3;
            }
            
            //
            // Routine body
            //
            if( ((double)(state.stplen)<=(double)(0) || (double)(state.stpmax)<(double)(0)) || state.fmax<2 )
            {
                state.info = 0;
                result = false;
                return result;
            }
            if( (double)(state.stplen)<=(double)(stpmin) )
            {
                state.info = 4;
                result = false;
                return result;
            }
            n = state.n;
            state.nfev = 0;
            
            //
            // We always need F
            //
            state.needf = true;
            
            //
            // Bound StpLen
            //
            if( (double)(state.stplen)>(double)(state.stpmax) && (double)(state.stpmax)!=(double)(0) )
            {
                state.stplen = state.stpmax;
            }
            
            //
            // Increase length
            //
            v = state.stplen*armijofactor;
            if( (double)(v)>(double)(state.stpmax) && (double)(state.stpmax)!=(double)(0) )
            {
                v = state.stpmax;
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.x[i_] + v*state.s[i_];
            }
            state.rstate.stage = 0;
            goto lbl_rcomm;
        lbl_0:
            state.nfev = state.nfev+1;
            if( (double)(state.f)>=(double)(state.fcur) )
            {
                goto lbl_4;
            }
            state.stplen = v;
            state.fcur = state.f;
        lbl_6:
            if( false )
            {
                goto lbl_7;
            }
            
            //
            // test stopping conditions
            //
            if( state.nfev>=state.fmax )
            {
                state.info = 3;
                result = false;
                return result;
            }
            if( (double)(state.stplen)>=(double)(state.stpmax) )
            {
                state.info = 5;
                result = false;
                return result;
            }
            
            //
            // evaluate F
            //
            v = state.stplen*armijofactor;
            if( (double)(v)>(double)(state.stpmax) && (double)(state.stpmax)!=(double)(0) )
            {
                v = state.stpmax;
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.x[i_] + v*state.s[i_];
            }
            state.rstate.stage = 1;
            goto lbl_rcomm;
        lbl_1:
            state.nfev = state.nfev+1;
            
            //
            // make decision
            //
            if( (double)(state.f)<(double)(state.fcur) )
            {
                state.stplen = v;
                state.fcur = state.f;
            }
            else
            {
                state.info = 1;
                result = false;
                return result;
            }
            goto lbl_6;
        lbl_7:
        lbl_4:
            
            //
            // Decrease length
            //
            v = state.stplen/armijofactor;
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.x[i_] + v*state.s[i_];
            }
            state.rstate.stage = 2;
            goto lbl_rcomm;
        lbl_2:
            state.nfev = state.nfev+1;
            if( (double)(state.f)>=(double)(state.fcur) )
            {
                goto lbl_8;
            }
            state.stplen = state.stplen/armijofactor;
            state.fcur = state.f;
        lbl_10:
            if( false )
            {
                goto lbl_11;
            }
            
            //
            // test stopping conditions
            //
            if( state.nfev>=state.fmax )
            {
                state.info = 3;
                result = false;
                return result;
            }
            if( (double)(state.stplen)<=(double)(stpmin) )
            {
                state.info = 4;
                result = false;
                return result;
            }
            
            //
            // evaluate F
            //
            v = state.stplen/armijofactor;
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.x[i_] + v*state.s[i_];
            }
            state.rstate.stage = 3;
            goto lbl_rcomm;
        lbl_3:
            state.nfev = state.nfev+1;
            
            //
            // make decision
            //
            if( (double)(state.f)<(double)(state.fcur) )
            {
                state.stplen = state.stplen/armijofactor;
                state.fcur = state.f;
            }
            else
            {
                state.info = 1;
                result = false;
                return result;
            }
            goto lbl_10;
        lbl_11:
        lbl_8:
            
            //
            // Nothing to be done
            //
            state.info = 1;
            result = false;
            return result;
            
            //
            // Saving state
            //
        lbl_rcomm:
            result = true;
            state.rstate.ia[0] = n;
            state.rstate.ra[0] = v;
            return result;
        }


        /*************************************************************************
        Results of Armijo search

        OUTPUT PARAMETERS:
            INFO    -   on output it is set to one of the return codes:
                        * 0     improper input params
                        * 1     optimum step is found with at most FMAX evaluations
                        * 3     FMAX evaluations were used,
                                X contains optimum found so far
                        * 4     step is at lower bound STPMIN
                        * 5     step is at upper bound
            STP     -   step length (in case of failure it is still returned)
            F       -   function value (in case of failure it is still returned)

          -- ALGLIB --
             Copyright 05.10.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void armijoresults(armijostate state,
            ref int info,
            ref double stp,
            ref double f,
            alglib.xparams _params)
        {
            info = state.info;
            stp = state.stplen;
            f = state.fcur;
        }


        private static void mcstep(ref double stx,
            ref double fx,
            ref double dx,
            ref double sty,
            ref double fy,
            ref double dy,
            ref double stp,
            double fp,
            double dp,
            ref bool brackt,
            double stmin,
            double stmax,
            ref int info,
            alglib.xparams _params)
        {
            bool bound = new bool();
            double gamma = 0;
            double p = 0;
            double q = 0;
            double r = 0;
            double s = 0;
            double sgnd = 0;
            double stpc = 0;
            double stpf = 0;
            double stpq = 0;
            double theta = 0;

            info = 0;
            
            //
            //     CHECK THE INPUT PARAMETERS FOR ERRORS.
            //
            if( ((brackt && ((double)(stp)<=(double)(Math.Min(stx, sty)) || (double)(stp)>=(double)(Math.Max(stx, sty)))) || (double)(dx*(stp-stx))>=(double)(0)) || (double)(stmax)<(double)(stmin) )
            {
                return;
            }
            
            //
            //     DETERMINE IF THE DERIVATIVES HAVE OPPOSITE SIGN.
            //
            sgnd = dp*(dx/Math.Abs(dx));
            
            //
            //     FIRST CASE. A HIGHER FUNCTION VALUE.
            //     THE MINIMUM IS BRACKETED. IF THE CUBIC STEP IS CLOSER
            //     TO STX THAN THE QUADRATIC STEP, THE CUBIC STEP IS TAKEN,
            //     ELSE THE AVERAGE OF THE CUBIC AND QUADRATIC STEPS IS TAKEN.
            //
            if( (double)(fp)>(double)(fx) )
            {
                info = 1;
                bound = true;
                theta = 3*(fx-fp)/(stp-stx)+dx+dp;
                s = Math.Max(Math.Abs(theta), Math.Max(Math.Abs(dx), Math.Abs(dp)));
                gamma = s*Math.Sqrt(math.sqr(theta/s)-dx/s*(dp/s));
                if( (double)(stp)<(double)(stx) )
                {
                    gamma = -gamma;
                }
                p = gamma-dx+theta;
                q = gamma-dx+gamma+dp;
                r = p/q;
                stpc = stx+r*(stp-stx);
                stpq = stx+dx/((fx-fp)/(stp-stx)+dx)/2*(stp-stx);
                if( (double)(Math.Abs(stpc-stx))<(double)(Math.Abs(stpq-stx)) )
                {
                    stpf = stpc;
                }
                else
                {
                    stpf = stpc+(stpq-stpc)/2;
                }
                brackt = true;
            }
            else
            {
                if( (double)(sgnd)<(double)(0) )
                {
                    
                    //
                    //     SECOND CASE. A LOWER FUNCTION VALUE AND DERIVATIVES OF
                    //     OPPOSITE SIGN. THE MINIMUM IS BRACKETED. IF THE CUBIC
                    //     STEP IS CLOSER TO STX THAN THE QUADRATIC (SECANT) STEP,
                    //     THE CUBIC STEP IS TAKEN, ELSE THE QUADRATIC STEP IS TAKEN.
                    //
                    info = 2;
                    bound = false;
                    theta = 3*(fx-fp)/(stp-stx)+dx+dp;
                    s = Math.Max(Math.Abs(theta), Math.Max(Math.Abs(dx), Math.Abs(dp)));
                    gamma = s*Math.Sqrt(math.sqr(theta/s)-dx/s*(dp/s));
                    if( (double)(stp)>(double)(stx) )
                    {
                        gamma = -gamma;
                    }
                    p = gamma-dp+theta;
                    q = gamma-dp+gamma+dx;
                    r = p/q;
                    stpc = stp+r*(stx-stp);
                    stpq = stp+dp/(dp-dx)*(stx-stp);
                    if( (double)(Math.Abs(stpc-stp))>(double)(Math.Abs(stpq-stp)) )
                    {
                        stpf = stpc;
                    }
                    else
                    {
                        stpf = stpq;
                    }
                    brackt = true;
                }
                else
                {
                    if( (double)(Math.Abs(dp))<(double)(Math.Abs(dx)) )
                    {
                        
                        //
                        //     THIRD CASE. A LOWER FUNCTION VALUE, DERIVATIVES OF THE
                        //     SAME SIGN, AND THE MAGNITUDE OF THE DERIVATIVE DECREASES.
                        //     THE CUBIC STEP IS ONLY USED IF THE CUBIC TENDS TO INFINITY
                        //     IN THE DIRECTION OF THE STEP OR IF THE MINIMUM OF THE CUBIC
                        //     IS BEYOND STP. OTHERWISE THE CUBIC STEP IS DEFINED TO BE
                        //     EITHER STPMIN OR STPMAX. THE QUADRATIC (SECANT) STEP IS ALSO
                        //     COMPUTED AND IF THE MINIMUM IS BRACKETED THEN THE THE STEP
                        //     CLOSEST TO STX IS TAKEN, ELSE THE STEP FARTHEST AWAY IS TAKEN.
                        //
                        info = 3;
                        bound = true;
                        theta = 3*(fx-fp)/(stp-stx)+dx+dp;
                        s = Math.Max(Math.Abs(theta), Math.Max(Math.Abs(dx), Math.Abs(dp)));
                        
                        //
                        //        THE CASE GAMMA = 0 ONLY ARISES IF THE CUBIC DOES NOT TEND
                        //        TO INFINITY IN THE DIRECTION OF THE STEP.
                        //
                        gamma = s*Math.Sqrt(Math.Max(0, math.sqr(theta/s)-dx/s*(dp/s)));
                        if( (double)(stp)>(double)(stx) )
                        {
                            gamma = -gamma;
                        }
                        p = gamma-dp+theta;
                        q = gamma+(dx-dp)+gamma;
                        r = p/q;
                        if( (double)(r)<(double)(0) && (double)(gamma)!=(double)(0) )
                        {
                            stpc = stp+r*(stx-stp);
                        }
                        else
                        {
                            if( (double)(stp)>(double)(stx) )
                            {
                                stpc = stmax;
                            }
                            else
                            {
                                stpc = stmin;
                            }
                        }
                        stpq = stp+dp/(dp-dx)*(stx-stp);
                        if( brackt )
                        {
                            if( (double)(Math.Abs(stp-stpc))<(double)(Math.Abs(stp-stpq)) )
                            {
                                stpf = stpc;
                            }
                            else
                            {
                                stpf = stpq;
                            }
                        }
                        else
                        {
                            if( (double)(Math.Abs(stp-stpc))>(double)(Math.Abs(stp-stpq)) )
                            {
                                stpf = stpc;
                            }
                            else
                            {
                                stpf = stpq;
                            }
                        }
                    }
                    else
                    {
                        
                        //
                        //     FOURTH CASE. A LOWER FUNCTION VALUE, DERIVATIVES OF THE
                        //     SAME SIGN, AND THE MAGNITUDE OF THE DERIVATIVE DOES
                        //     NOT DECREASE. IF THE MINIMUM IS NOT BRACKETED, THE STEP
                        //     IS EITHER STPMIN OR STPMAX, ELSE THE CUBIC STEP IS TAKEN.
                        //
                        info = 4;
                        bound = false;
                        if( brackt )
                        {
                            theta = 3*(fp-fy)/(sty-stp)+dy+dp;
                            s = Math.Max(Math.Abs(theta), Math.Max(Math.Abs(dy), Math.Abs(dp)));
                            gamma = s*Math.Sqrt(math.sqr(theta/s)-dy/s*(dp/s));
                            if( (double)(stp)>(double)(sty) )
                            {
                                gamma = -gamma;
                            }
                            p = gamma-dp+theta;
                            q = gamma-dp+gamma+dy;
                            r = p/q;
                            stpc = stp+r*(sty-stp);
                            stpf = stpc;
                        }
                        else
                        {
                            if( (double)(stp)>(double)(stx) )
                            {
                                stpf = stmax;
                            }
                            else
                            {
                                stpf = stmin;
                            }
                        }
                    }
                }
            }
            
            //
            //     UPDATE THE INTERVAL OF UNCERTAINTY. THIS UPDATE DOES NOT
            //     DEPEND ON THE NEW STEP OR THE CASE ANALYSIS ABOVE.
            //
            if( (double)(fp)>(double)(fx) )
            {
                sty = stp;
                fy = fp;
                dy = dp;
            }
            else
            {
                if( (double)(sgnd)<(double)(0.0) )
                {
                    sty = stx;
                    fy = fx;
                    dy = dx;
                }
                stx = stp;
                fx = fp;
                dx = dp;
            }
            
            //
            //     COMPUTE THE NEW STEP AND SAFEGUARD IT.
            //
            stpf = Math.Min(stmax, stpf);
            stpf = Math.Max(stmin, stpf);
            stp = stpf;
            if( brackt && bound )
            {
                if( (double)(sty)>(double)(stx) )
                {
                    stp = Math.Min(stx+0.66*(sty-stx), stp);
                }
                else
                {
                    stp = Math.Max(stx+0.66*(sty-stx), stp);
                }
            }
        }


    }
    public class xblas
    {
        /*************************************************************************
        More precise dot-product. Absolute error of  subroutine  result  is  about
        1 ulp of max(MX,V), where:
            MX = max( |a[i]*b[i]| )
            V  = |(a,b)|

        INPUT PARAMETERS
            A       -   array[0..N-1], vector 1
            B       -   array[0..N-1], vector 2
            N       -   vectors length, N<2^29.
            Temp    -   array[0..N-1], pre-allocated temporary storage

        OUTPUT PARAMETERS
            R       -   (A,B)
            RErr    -   estimate of error. This estimate accounts for both  errors
                        during  calculation  of  (A,B)  and  errors  introduced by
                        rounding of A and B to fit in double (about 1 ulp).

          -- ALGLIB --
             Copyright 24.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void xdot(double[] a,
            double[] b,
            int n,
            ref double[] temp,
            ref double r,
            ref double rerr,
            alglib.xparams _params)
        {
            int i = 0;
            double mx = 0;
            double v = 0;

            r = 0;
            rerr = 0;

            
            //
            // special cases:
            // * N=0
            //
            if( n==0 )
            {
                r = 0;
                rerr = 0;
                return;
            }
            mx = 0;
            for(i=0; i<=n-1; i++)
            {
                v = a[i]*b[i];
                temp[i] = v;
                mx = Math.Max(mx, Math.Abs(v));
            }
            if( (double)(mx)==(double)(0) )
            {
                r = 0;
                rerr = 0;
                return;
            }
            xsum(ref temp, mx, n, ref r, ref rerr, _params);
        }


        /*************************************************************************
        More precise complex dot-product. Absolute error of  subroutine  result is
        about 1 ulp of max(MX,V), where:
            MX = max( |a[i]*b[i]| )
            V  = |(a,b)|

        INPUT PARAMETERS
            A       -   array[0..N-1], vector 1
            B       -   array[0..N-1], vector 2
            N       -   vectors length, N<2^29.
            Temp    -   array[0..2*N-1], pre-allocated temporary storage

        OUTPUT PARAMETERS
            R       -   (A,B)
            RErr    -   estimate of error. This estimate accounts for both  errors
                        during  calculation  of  (A,B)  and  errors  introduced by
                        rounding of A and B to fit in double (about 1 ulp).

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void xcdot(complex[] a,
            complex[] b,
            int n,
            ref double[] temp,
            ref complex r,
            ref double rerr,
            alglib.xparams _params)
        {
            int i = 0;
            double mx = 0;
            double v = 0;
            double rerrx = 0;
            double rerry = 0;

            r = 0;
            rerr = 0;

            
            //
            // special cases:
            // * N=0
            //
            if( n==0 )
            {
                r = 0;
                rerr = 0;
                return;
            }
            
            //
            // calculate real part
            //
            mx = 0;
            for(i=0; i<=n-1; i++)
            {
                v = a[i].x*b[i].x;
                temp[2*i+0] = v;
                mx = Math.Max(mx, Math.Abs(v));
                v = -(a[i].y*b[i].y);
                temp[2*i+1] = v;
                mx = Math.Max(mx, Math.Abs(v));
            }
            if( (double)(mx)==(double)(0) )
            {
                r.x = 0;
                rerrx = 0;
            }
            else
            {
                xsum(ref temp, mx, 2*n, ref r.x, ref rerrx, _params);
            }
            
            //
            // calculate imaginary part
            //
            mx = 0;
            for(i=0; i<=n-1; i++)
            {
                v = a[i].x*b[i].y;
                temp[2*i+0] = v;
                mx = Math.Max(mx, Math.Abs(v));
                v = a[i].y*b[i].x;
                temp[2*i+1] = v;
                mx = Math.Max(mx, Math.Abs(v));
            }
            if( (double)(mx)==(double)(0) )
            {
                r.y = 0;
                rerry = 0;
            }
            else
            {
                xsum(ref temp, mx, 2*n, ref r.y, ref rerry, _params);
            }
            
            //
            // total error
            //
            if( (double)(rerrx)==(double)(0) && (double)(rerry)==(double)(0) )
            {
                rerr = 0;
            }
            else
            {
                rerr = Math.Max(rerrx, rerry)*Math.Sqrt(1+math.sqr(Math.Min(rerrx, rerry)/Math.Max(rerrx, rerry)));
            }
        }


        /*************************************************************************
        Internal subroutine for extra-precise calculation of SUM(w[i]).

        INPUT PARAMETERS:
            W   -   array[0..N-1], values to be added
                    W is modified during calculations.
            MX  -   max(W[i])
            N   -   array size
            
        OUTPUT PARAMETERS:
            R   -   SUM(w[i])
            RErr-   error estimate for R

          -- ALGLIB --
             Copyright 24.08.2009 by Bochkanov Sergey
        *************************************************************************/
        private static void xsum(ref double[] w,
            double mx,
            int n,
            ref double r,
            ref double rerr,
            alglib.xparams _params)
        {
            int i = 0;
            int k = 0;
            int ks = 0;
            double v = 0;
            double s = 0;
            double ln2 = 0;
            double chunk = 0;
            double invchunk = 0;
            bool allzeros = new bool();
            int i_ = 0;

            r = 0;
            rerr = 0;

            
            //
            // special cases:
            // * N=0
            // * N is too large to use integer arithmetics
            //
            if( n==0 )
            {
                r = 0;
                rerr = 0;
                return;
            }
            if( (double)(mx)==(double)(0) )
            {
                r = 0;
                rerr = 0;
                return;
            }
            alglib.ap.assert(n<536870912, "XDot: N is too large!");
            
            //
            // Prepare
            //
            ln2 = Math.Log(2);
            rerr = mx*math.machineepsilon;
            
            //
            // 1. find S such that 0.5<=S*MX<1
            // 2. multiply W by S, so task is normalized in some sense
            // 3. S:=1/S so we can obtain original vector multiplying by S
            //
            k = (int)Math.Round(Math.Log(mx)/ln2);
            s = xfastpow(2, -k, _params);
            if( !math.isfinite(s) )
            {
                
                //
                // Overflow or underflow during evaluation of S; fallback low-precision code
                //
                r = 0;
                rerr = mx*math.machineepsilon;
                for(i=0; i<=n-1; i++)
                {
                    r = r+w[i];
                }
                return;
            }
            while( (double)(s*mx)>=(double)(1) )
            {
                s = 0.5*s;
            }
            while( (double)(s*mx)<(double)(0.5) )
            {
                s = 2*s;
            }
            for(i_=0; i_<=n-1;i_++)
            {
                w[i_] = s*w[i_];
            }
            s = 1/s;
            
            //
            // find Chunk=2^M such that N*Chunk<2^29
            //
            // we have chosen upper limit (2^29) with enough space left
            // to tolerate possible problems with rounding and N's close
            // to the limit, so we don't want to be very strict here.
            //
            k = (int)(Math.Log((double)536870912/(double)n)/ln2);
            chunk = xfastpow(2, k, _params);
            if( (double)(chunk)<(double)(2) )
            {
                chunk = 2;
            }
            invchunk = 1/chunk;
            
            //
            // calculate result
            //
            r = 0;
            for(i_=0; i_<=n-1;i_++)
            {
                w[i_] = chunk*w[i_];
            }
            while( true )
            {
                s = s*invchunk;
                allzeros = true;
                ks = 0;
                for(i=0; i<=n-1; i++)
                {
                    v = w[i];
                    k = (int)(v);
                    if( (double)(v)!=(double)(k) )
                    {
                        allzeros = false;
                    }
                    w[i] = chunk*(v-k);
                    ks = ks+k;
                }
                r = r+s*ks;
                v = Math.Abs(r);
                if( allzeros || (double)(s*n+mx)==(double)(mx) )
                {
                    break;
                }
            }
            
            //
            // correct error
            //
            rerr = Math.Max(rerr, Math.Abs(r)*math.machineepsilon);
        }


        /*************************************************************************
        Fast Pow

          -- ALGLIB --
             Copyright 24.08.2009 by Bochkanov Sergey
        *************************************************************************/
        private static double xfastpow(double r,
            int n,
            alglib.xparams _params)
        {
            double result = 0;

            result = 0;
            if( n>0 )
            {
                if( n%2==0 )
                {
                    result = math.sqr(xfastpow(r, n/2, _params));
                }
                else
                {
                    result = r*xfastpow(r, n-1, _params);
                }
                return result;
            }
            if( n==0 )
            {
                result = 1;
            }
            if( n<0 )
            {
                result = xfastpow(1/r, -n, _params);
            }
            return result;
        }


    }
    public class basicstatops
    {
        /*************************************************************************
        Internal tied ranking subroutine.

        INPUT PARAMETERS:
            X       -   array to rank
            N       -   array size
            IsCentered- whether ranks are centered or not:
                        * True      -   ranks are centered in such way that  their
                                        sum is zero
                        * False     -   ranks are not centered
            Buf     -   temporary buffers
            
        NOTE: when IsCentered is True and all X[] are equal, this  function  fills
              X by zeros (exact zeros are used, not sum which is only approximately
              equal to zero).
        *************************************************************************/
        public static void rankx(double[] x,
            int n,
            bool iscentered,
            apserv.apbuffers buf,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            double tmp = 0;
            double voffs = 0;

            
            //
            // Prepare
            //
            if( n<1 )
            {
                return;
            }
            if( n==1 )
            {
                x[0] = 0;
                return;
            }
            if( alglib.ap.len(buf.ra1)<n )
            {
                buf.ra1 = new double[n];
            }
            if( alglib.ap.len(buf.ia1)<n )
            {
                buf.ia1 = new int[n];
            }
            for(i=0; i<=n-1; i++)
            {
                buf.ra1[i] = x[i];
                buf.ia1[i] = i;
            }
            tsort.tagsortfasti(ref buf.ra1, ref buf.ia1, ref buf.ra2, ref buf.ia2, n, _params);
            
            //
            // Special test for all values being equal
            //
            if( (double)(buf.ra1[0])==(double)(buf.ra1[n-1]) )
            {
                if( iscentered )
                {
                    tmp = 0.0;
                }
                else
                {
                    tmp = (double)(n-1)/(double)2;
                }
                for(i=0; i<=n-1; i++)
                {
                    x[i] = tmp;
                }
                return;
            }
            
            //
            // compute tied ranks
            //
            i = 0;
            while( i<=n-1 )
            {
                j = i+1;
                while( j<=n-1 )
                {
                    if( (double)(buf.ra1[j])!=(double)(buf.ra1[i]) )
                    {
                        break;
                    }
                    j = j+1;
                }
                for(k=i; k<=j-1; k++)
                {
                    buf.ra1[k] = (double)(i+j-1)/(double)2;
                }
                i = j;
            }
            
            //
            // back to x
            //
            if( iscentered )
            {
                voffs = (double)(n-1)/(double)2;
            }
            else
            {
                voffs = 0.0;
            }
            for(i=0; i<=n-1; i++)
            {
                x[buf.ia1[i]] = buf.ra1[i]-voffs;
            }
        }


        /*************************************************************************
        Internal untied ranking subroutine.

        INPUT PARAMETERS:
            X       -   array to rank
            N       -   array size
            Buf     -   temporary buffers

        Returns untied ranks (in case of a tie ranks are resolved arbitrarily).
        *************************************************************************/
        public static void rankxuntied(double[] x,
            int n,
            apserv.apbuffers buf,
            alglib.xparams _params)
        {
            int i = 0;

            
            //
            // Prepare
            //
            if( n<1 )
            {
                return;
            }
            if( n==1 )
            {
                x[0] = 0;
                return;
            }
            if( alglib.ap.len(buf.ra1)<n )
            {
                buf.ra1 = new double[n];
            }
            if( alglib.ap.len(buf.ia1)<n )
            {
                buf.ia1 = new int[n];
            }
            for(i=0; i<=n-1; i++)
            {
                buf.ra1[i] = x[i];
                buf.ia1[i] = i;
            }
            tsort.tagsortfasti(ref buf.ra1, ref buf.ia1, ref buf.ra2, ref buf.ia2, n, _params);
            for(i=0; i<=n-1; i++)
            {
                x[buf.ia1[i]] = i;
            }
        }


    }
    public class hpccores
    {
        /*************************************************************************
        This structure stores  temporary  buffers  used  by  gradient  calculation
        functions for neural networks.
        *************************************************************************/
        public class mlpbuffers : apobject
        {
            public int chunksize;
            public int ntotal;
            public int nin;
            public int nout;
            public int wcount;
            public double[] batch4buf;
            public double[] hpcbuf;
            public double[,] xy;
            public double[,] xy2;
            public double[] xyrow;
            public double[] x;
            public double[] y;
            public double[] desiredy;
            public double e;
            public double[] g;
            public double[] tmp0;
            public mlpbuffers()
            {
                init();
            }
            public override void init()
            {
                batch4buf = new double[0];
                hpcbuf = new double[0];
                xy = new double[0,0];
                xy2 = new double[0,0];
                xyrow = new double[0];
                x = new double[0];
                y = new double[0];
                desiredy = new double[0];
                g = new double[0];
                tmp0 = new double[0];
            }
            public override alglib.apobject make_copy()
            {
                mlpbuffers _result = new mlpbuffers();
                _result.chunksize = chunksize;
                _result.ntotal = ntotal;
                _result.nin = nin;
                _result.nout = nout;
                _result.wcount = wcount;
                _result.batch4buf = (double[])batch4buf.Clone();
                _result.hpcbuf = (double[])hpcbuf.Clone();
                _result.xy = (double[,])xy.Clone();
                _result.xy2 = (double[,])xy2.Clone();
                _result.xyrow = (double[])xyrow.Clone();
                _result.x = (double[])x.Clone();
                _result.y = (double[])y.Clone();
                _result.desiredy = (double[])desiredy.Clone();
                _result.e = e;
                _result.g = (double[])g.Clone();
                _result.tmp0 = (double[])tmp0.Clone();
                return _result;
            }
        };




        /*************************************************************************
        Prepares HPC compuations  of  chunked  gradient with HPCChunkedGradient().
        You  have to call this function  before  calling  HPCChunkedGradient() for
        a new set of weights. You have to call it only once, see example below:

        HOW TO PROCESS DATASET WITH THIS FUNCTION:
            Grad:=0
            HPCPrepareChunkedGradient(Weights, WCount, NTotal, NOut, Buf)
            foreach chunk-of-dataset do
                HPCChunkedGradient(...)
            HPCFinalizeChunkedGradient(Buf, Grad)

        *************************************************************************/
        public static void hpcpreparechunkedgradient(double[] weights,
            int wcount,
            int ntotal,
            int nin,
            int nout,
            mlpbuffers buf,
            alglib.xparams _params)
        {
            int i = 0;
            int batch4size = 0;
            int chunksize = 0;

            chunksize = 4;
            batch4size = 3*chunksize*ntotal+chunksize*(2*nout+1);
            if( alglib.ap.rows(buf.xy)<chunksize || alglib.ap.cols(buf.xy)<nin+nout )
            {
                buf.xy = new double[chunksize, nin+nout];
            }
            if( alglib.ap.rows(buf.xy2)<chunksize || alglib.ap.cols(buf.xy2)<nin+nout )
            {
                buf.xy2 = new double[chunksize, nin+nout];
            }
            if( alglib.ap.len(buf.xyrow)<nin+nout )
            {
                buf.xyrow = new double[nin+nout];
            }
            if( alglib.ap.len(buf.x)<nin )
            {
                buf.x = new double[nin];
            }
            if( alglib.ap.len(buf.y)<nout )
            {
                buf.y = new double[nout];
            }
            if( alglib.ap.len(buf.desiredy)<nout )
            {
                buf.desiredy = new double[nout];
            }
            if( alglib.ap.len(buf.batch4buf)<batch4size )
            {
                buf.batch4buf = new double[batch4size];
            }
            if( alglib.ap.len(buf.hpcbuf)<wcount )
            {
                buf.hpcbuf = new double[wcount];
            }
            if( alglib.ap.len(buf.g)<wcount )
            {
                buf.g = new double[wcount];
            }
            if( !hpcpreparechunkedgradientx(weights, wcount, buf.hpcbuf, _params) )
            {
                for(i=0; i<=wcount-1; i++)
                {
                    buf.hpcbuf[i] = 0.0;
                }
            }
            buf.wcount = wcount;
            buf.ntotal = ntotal;
            buf.nin = nin;
            buf.nout = nout;
            buf.chunksize = chunksize;
        }


        /*************************************************************************
        Finalizes HPC compuations  of  chunked gradient with HPCChunkedGradient().
        You  have to call this function  after  calling  HPCChunkedGradient()  for
        a new set of weights. You have to call it only once, see example below:

        HOW TO PROCESS DATASET WITH THIS FUNCTION:
            Grad:=0
            HPCPrepareChunkedGradient(Weights, WCount, NTotal, NOut, Buf)
            foreach chunk-of-dataset do
                HPCChunkedGradient(...)
            HPCFinalizeChunkedGradient(Buf, Grad)

        *************************************************************************/
        public static void hpcfinalizechunkedgradient(mlpbuffers buf,
            double[] grad,
            alglib.xparams _params)
        {
            int i = 0;

            if( !hpcfinalizechunkedgradientx(buf.hpcbuf, buf.wcount, grad, _params) )
            {
                for(i=0; i<=buf.wcount-1; i++)
                {
                    grad[i] = grad[i]+buf.hpcbuf[i];
                }
            }
        }


        /*************************************************************************
        Fast kernel for chunked gradient.

        *************************************************************************/
        public static bool hpcchunkedgradient(double[] weights,
            int[] structinfo,
            double[] columnmeans,
            double[] columnsigmas,
            double[,] xy,
            int cstart,
            int csize,
            double[] batch4buf,
            double[] hpcbuf,
            ref double e,
            bool naturalerrorfunc,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        Fast kernel for chunked processing.

        *************************************************************************/
        public static bool hpcchunkedprocess(double[] weights,
            int[] structinfo,
            double[] columnmeans,
            double[] columnsigmas,
            double[,] xy,
            int cstart,
            int csize,
            double[] batch4buf,
            double[] hpcbuf,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        Stub function.

          -- ALGLIB routine --
             14.06.2013
             Bochkanov Sergey
        *************************************************************************/
        private static bool hpcpreparechunkedgradientx(double[] weights,
            int wcount,
            double[] hpcbuf,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        Stub function.

          -- ALGLIB routine --
             14.06.2013
             Bochkanov Sergey
        *************************************************************************/
        private static bool hpcfinalizechunkedgradientx(double[] buf,
            int wcount,
            double[] grad,
            alglib.xparams _params)
        {
            bool result = new bool();

            result = false;
            return result;
        }


    }
    public class ntheory
    {
        public static void findprimitiverootandinverse(int n,
            ref int proot,
            ref int invproot,
            alglib.xparams _params)
        {
            int candroot = 0;
            int phin = 0;
            int q = 0;
            int f = 0;
            bool allnonone = new bool();
            int x = 0;
            int lastx = 0;
            int y = 0;
            int lasty = 0;
            int a = 0;
            int b = 0;
            int t = 0;
            int n2 = 0;

            proot = 0;
            invproot = 0;

            alglib.ap.assert(n>=3, "FindPrimitiveRootAndInverse: N<3");
            proot = 0;
            invproot = 0;
            
            //
            // check that N is prime
            //
            alglib.ap.assert(isprime(n, _params), "FindPrimitiveRoot: N is not prime");
            
            //
            // Because N is prime, Euler totient function is equal to N-1
            //
            phin = n-1;
            
            //
            // Test different values of PRoot - from 2 to N-1.
            // One of these values MUST be primitive root.
            //
            // For testing we use algorithm from Wiki (Primitive root modulo n):
            // * compute phi(N)
            // * determine the different prime factors of phi(N), say p1, ..., pk
            // * for every element m of Zn*, compute m^(phi(N)/pi) mod N for i=1..k
            //   using a fast algorithm for modular exponentiation.
            // * a number m for which these k results are all different from 1 is a
            //   primitive root.
            //
            for(candroot=2; candroot<=n-1; candroot++)
            {
                
                //
                // We have current candidate root in CandRoot.
                //
                // Scan different prime factors of PhiN. Here:
                // * F is a current candidate factor
                // * Q is a current quotient - amount which was left after dividing PhiN
                //   by all previous factors
                //
                // For each factor, perform test mentioned above.
                //
                q = phin;
                f = 2;
                allnonone = true;
                while( q>1 )
                {
                    if( q%f==0 )
                    {
                        t = modexp(candroot, phin/f, n, _params);
                        if( t==1 )
                        {
                            allnonone = false;
                            break;
                        }
                        while( q%f==0 )
                        {
                            q = q/f;
                        }
                    }
                    f = f+1;
                }
                if( allnonone )
                {
                    proot = candroot;
                    break;
                }
            }
            alglib.ap.assert(proot>=2, "FindPrimitiveRoot: internal error (root not found)");
            
            //
            // Use extended Euclidean algorithm to find multiplicative inverse of primitive root
            //
            x = 0;
            lastx = 1;
            y = 1;
            lasty = 0;
            a = proot;
            b = n;
            while( b!=0 )
            {
                q = a/b;
                t = a%b;
                a = b;
                b = t;
                t = lastx-q*x;
                lastx = x;
                x = t;
                t = lasty-q*y;
                lasty = y;
                y = t;
            }
            while( lastx<0 )
            {
                lastx = lastx+n;
            }
            invproot = lastx;
            
            //
            // Check that it is safe to perform multiplication modulo N.
            // Check results for consistency.
            //
            n2 = (n-1)*(n-1);
            alglib.ap.assert(n2/(n-1)==n-1, "FindPrimitiveRoot: internal error");
            alglib.ap.assert(proot*invproot/proot==invproot, "FindPrimitiveRoot: internal error");
            alglib.ap.assert(proot*invproot/invproot==proot, "FindPrimitiveRoot: internal error");
            alglib.ap.assert(proot*invproot%n==1, "FindPrimitiveRoot: internal error");
        }


        private static bool isprime(int n,
            alglib.xparams _params)
        {
            bool result = new bool();
            int p = 0;

            result = false;
            p = 2;
            while( p*p<=n )
            {
                if( n%p==0 )
                {
                    return result;
                }
                p = p+1;
            }
            result = true;
            return result;
        }


        private static int modmul(int a,
            int b,
            int n,
            alglib.xparams _params)
        {
            int result = 0;
            int t = 0;
            double ra = 0;
            double rb = 0;

            alglib.ap.assert(a>=0 && a<n, "ModMul: A<0 or A>=N");
            alglib.ap.assert(b>=0 && b<n, "ModMul: B<0 or B>=N");
            
            //
            // Base cases
            //
            ra = a;
            rb = b;
            if( b==0 || a==0 )
            {
                result = 0;
                return result;
            }
            if( b==1 || a==1 )
            {
                result = a*b;
                return result;
            }
            if( (double)(ra*rb)==(double)(a*b) )
            {
                result = a*b%n;
                return result;
            }
            
            //
            // Non-base cases
            //
            if( b%2==0 )
            {
                
                //
                // A*B = (A*(B/2)) * 2
                //
                // Product T=A*(B/2) is calculated recursively, product T*2 is
                // calculated as follows:
                // * result:=T-N
                // * result:=result+T
                // * if result<0 then result:=result+N
                //
                // In case integer result overflows, we generate exception
                //
                t = modmul(a, b/2, n, _params);
                result = t-n;
                result = result+t;
                if( result<0 )
                {
                    result = result+n;
                }
            }
            else
            {
                
                //
                // A*B = (A*(B div 2)) * 2 + A
                //
                // Product T=A*(B/2) is calculated recursively, product T*2 is
                // calculated as follows:
                // * result:=T-N
                // * result:=result+T
                // * if result<0 then result:=result+N
                //
                // In case integer result overflows, we generate exception
                //
                t = modmul(a, b/2, n, _params);
                result = t-n;
                result = result+t;
                if( result<0 )
                {
                    result = result+n;
                }
                result = result-n;
                result = result+a;
                if( result<0 )
                {
                    result = result+n;
                }
            }
            return result;
        }


        private static int modexp(int a,
            int b,
            int n,
            alglib.xparams _params)
        {
            int result = 0;
            int t = 0;

            alglib.ap.assert(a>=0 && a<n, "ModExp: A<0 or A>=N");
            alglib.ap.assert(b>=0, "ModExp: B<0");
            
            //
            // Base cases
            //
            if( b==0 )
            {
                result = 1;
                return result;
            }
            if( b==1 )
            {
                result = a;
                return result;
            }
            
            //
            // Non-base cases
            //
            if( b%2==0 )
            {
                t = modmul(a, a, n, _params);
                result = modexp(t, b/2, n, _params);
            }
            else
            {
                t = modmul(a, a, n, _params);
                result = modexp(t, b/2, n, _params);
                result = modmul(result, a, n, _params);
            }
            return result;
        }


    }
    public class ftbase
    {
        /*************************************************************************
        This record stores execution plan for the fast transformation  along  with
        preallocated temporary buffers and precalculated values.

        FIELDS:
            Entries         -   plan entries, one row = one entry (see below for
                                description).
            Buf0,Buf1,Buf2  -   global temporary buffers; some of them are allocated,
                                some of them are not (as decided by plan generation
                                subroutine).
            Buffer          -   global buffer whose size is equal to plan size.
                                There is one-to-one correspondence between elements
                                of global buffer and elements of array transformed.
                                Because of it global buffer can be used as temporary
                                thread-safe storage WITHOUT ACQUIRING LOCK - each
                                worker thread works with its part of input array,
                                and each part of input array corresponds to distinct
                                part of buffer.
            
        FORMAT OF THE ENTRIES TABLE:

        Entries table is 2D array which stores one entry per row. Row format is:
            row[0]      operation type:
                        *  0 for "end of plan/subplan"
                        * +1 for "reference O(N^2) complex FFT"
                        * -1 for complex transposition
                        * -2 for multiplication by twiddle factors of complex FFT
                        * -3 for "start of plan/subplan"
            row[1]      repetition count, >=1
            row[2]      base operand size (number of microvectors), >=1
            row[3]      microvector size (measured in real numbers), >=1
            row[4]      parameter0, meaning depends on row[0]
            row[5]      parameter1, meaning depends on row[0]

        FORMAT OF THE DATA:

        Transformation plan works with row[1]*row[2]*row[3]  real  numbers,  which
        are (in most cases) interpreted as sequence of complex numbers. These data
        are grouped as follows:
        * we have row[1] contiguous OPERANDS, which can be treated separately
        * each operand includes row[2] contiguous MICROVECTORS
        * each microvector includes row[3] COMPONENTS, which can be treated separately
        * pair of components form complex number, so in most cases row[3] will be even

        Say, if you want to perform complex FFT of length 3, then:
        * you have 1 operand: row[1]=1
        * operand consists of 3 microvectors:   row[2]=3
        * each microvector has two components:  row[3]=2
        * a pair of subsequent components is treated as complex number

        if you want to perform TWO simultaneous complex FFT's of length 3, then you
        can choose between two representations:
        * 1 operand, 3 microvectors, 4 components; storage format is given below:
          [ A0X A0Y B0X B0Y A1X A1Y B1X B1Y ... ]
          (here A denotes first sequence, B - second one).
        * 2 operands, 3 microvectors, 2 components; storage format is given below:
          [ A0X A0Y A1X A2Y ... B0X B0Y B1X B1Y ... ]
        Most FFT operations are supported only for the second format, but you
        should remember that first format sometimes can be used too.

        SUPPORTED OPERATIONS:

        row[0]=0:
        * "end of plan/subplan"
        * in case we meet entry with such type,  FFT  transformation  is  finished
          (or we return from recursive FFT subplan, in case it was subplan).

        row[0]=+1:
        * "reference 1D complex FFT"
        * we perform reference O(N^2) complex FFT on input data, which are treated
          as row[1] arrays, each of row[2] complex numbers, and row[3] must be
          equal to 2
        * transformation is performed using temporary buffer

        row[0]=opBluesteinsFFT:
        * input array is handled with Bluestein's algorithm (by zero-padding to
          Param0 complex numbers).
        * this plan calls Param0-point subplan which is located at offset Param1
          (offset is measured with respect to location of the calling entry)
        * this plan uses precomputed quantities stored in Plan.PrecR at
          offset Param2.
        * transformation is performed using 4 temporary buffers, which are
          retrieved from Plan.BluesteinPool.

        row[0]=+3:
        * "optimized 1D complex FFT"
        * this function supports only several operand sizes: from 1 to 5.
          These transforms are hard-coded and performed very efficiently

        row[0]=opRadersFFT:
        * input array is handled with Rader's algorithm (permutation and
          reduction to N-1-point FFT)
        * this plan calls N-1-point subplan which is located at offset Param0
          (offset is measured with respect to location of the calling entry)
        * this plan uses precomputed primitive root and its inverse (modulo N)
          which are stored in Param1 and Param2.
        * Param3 stores offset of the precomputed data for the plan
        * plan length must be prime, (N-1)*(N-1) must fit into integer variable

        row[0]=-1
        * "complex transposition"
        * input data are treated as row[1] independent arrays, which are processed
          separately
        * each of operands is treated as matrix with row[4] rows and row[2]/row[4]
          columns. Each element of the matrix is microvector with row[3] components.
        * transposition is performed using temporary buffer

        row[0]=-2
        * "multiplication by twiddle factors of complex FFT"
        * input data are treated as row[1] independent arrays, which are processed
          separately
        * row[4] contains N1 - length of the "first FFT"  in  a  Cooley-Tukey  FFT
          algorithm
        * this function does not require temporary buffers

        row[0]=-3
        * "start of the plan"
        * each subplan must start from this entry
        * param0 is ignored
        * param1 stores approximate (optimistic) estimate of KFLOPs required to
          transform one operand of the plan. Total cost of the plan is approximately
          equal to row[1]*param1 KFLOPs.
        * this function does not require temporary buffers

        row[0]=-4
        * "jump"
        * param0 stores relative offset of the jump site
          (+1 corresponds to the next entry)

        row[0]=-5
        * "parallel call"
        * input data are treated as row[1] independent arrays
        * child subplan is applied independently for each of arrays - row[1] times
        * subplan length must be equal to row[2]*row[3]
        * param0 stores relative offset of the child subplan site
          (+1 corresponds to the next entry)
        * param1 stores approximate total cost of plan, measured in UNITS
          (1 UNIT = 100 KFLOPs). Plan cost must be rounded DOWN to nearest integer.


            
        TODO 
             2. from KFLOPs to UNITs, 1 UNIT = 100 000 FLOP!!!!!!!!!!!

             3. from IsRoot to TaskType = {0, -1, +1}; or maybe, add IsSeparatePlan
                to distinguish root of child subplan from global root which uses
                separate buffer
                
             4. child subplans in parallel call must NOT use buffer provided by parent plan;
                they must allocate their own local buffer
        *************************************************************************/
        public class fasttransformplan : apobject
        {
            public int[,] entries;
            public double[] buffer;
            public double[] precr;
            public double[] preci;
            public alglib.smp.shared_pool bluesteinpool;
            public fasttransformplan()
            {
                init();
            }
            public override void init()
            {
                entries = new int[0,0];
                buffer = new double[0];
                precr = new double[0];
                preci = new double[0];
                bluesteinpool = new alglib.smp.shared_pool();
            }
            public override alglib.apobject make_copy()
            {
                fasttransformplan _result = new fasttransformplan();
                _result.entries = (int[,])entries.Clone();
                _result.buffer = (double[])buffer.Clone();
                _result.precr = (double[])precr.Clone();
                _result.preci = (double[])preci.Clone();
                _result.bluesteinpool = (alglib.smp.shared_pool)bluesteinpool.make_copy();
                return _result;
            }
        };




        public const int coltype = 0;
        public const int coloperandscnt = 1;
        public const int coloperandsize = 2;
        public const int colmicrovectorsize = 3;
        public const int colparam0 = 4;
        public const int colparam1 = 5;
        public const int colparam2 = 6;
        public const int colparam3 = 7;
        public const int colscnt = 8;
        public const int opend = 0;
        public const int opcomplexreffft = 1;
        public const int opbluesteinsfft = 2;
        public const int opcomplexcodeletfft = 3;
        public const int opcomplexcodelettwfft = 4;
        public const int opradersfft = 5;
        public const int opcomplextranspose = -1;
        public const int opcomplexfftfactors = -2;
        public const int opstart = -3;
        public const int opjmp = -4;
        public const int opparallelcall = -5;
        public const int maxradix = 6;
        public const int updatetw = 16;
        public const int recursivethreshold = 1024;
        public const int raderthreshold = 19;
        public const int ftbasecodeletrecommended = 5;
        public const double ftbaseinefficiencyfactor = 1.3;
        public const int ftbasemaxsmoothfactor = 5;


        /*************************************************************************
        This subroutine generates FFT plan for K complex FFT's with length N each.

        INPUT PARAMETERS:
            N           -   FFT length (in complex numbers), N>=1
            K           -   number of repetitions, K>=1
            
        OUTPUT PARAMETERS:
            Plan        -   plan

          -- ALGLIB --
             Copyright 05.04.2013 by Bochkanov Sergey
        *************************************************************************/
        public static void ftcomplexfftplan(int n,
            int k,
            fasttransformplan plan,
            alglib.xparams _params)
        {
            apserv.srealarray bluesteinbuf = new apserv.srealarray();
            int rowptr = 0;
            int bluesteinsize = 0;
            int precrptr = 0;
            int preciptr = 0;
            int precrsize = 0;
            int precisize = 0;

            
            //
            // Initial check for parameters
            //
            alglib.ap.assert(n>0, "FTComplexFFTPlan: N<=0");
            alglib.ap.assert(k>0, "FTComplexFFTPlan: K<=0");
            
            //
            // Determine required sizes of precomputed real and integer
            // buffers. This stage of code is highly dependent on internals
            // of FTComplexFFTPlanRec() and must be kept synchronized with
            // possible changes in internals of plan generation function.
            //
            // Buffer size is determined as follows:
            // * N is factorized
            // * we factor out anything which is less or equal to MaxRadix
            // * prime factor F>RaderThreshold requires 4*FTBaseFindSmooth(2*F-1)
            //   real entries to store precomputed Quantities for Bluestein's
            //   transformation
            // * prime factor F<=RaderThreshold does NOT require
            //   precomputed storage
            //
            precrsize = 0;
            precisize = 0;
            ftdeterminespacerequirements(n, ref precrsize, ref precisize, _params);
            if( precrsize>0 )
            {
                plan.precr = new double[precrsize];
            }
            if( precisize>0 )
            {
                plan.preci = new double[precisize];
            }
            
            //
            // Generate plan
            //
            rowptr = 0;
            precrptr = 0;
            preciptr = 0;
            bluesteinsize = 1;
            plan.buffer = new double[2*n*k];
            ftcomplexfftplanrec(n, k, true, true, ref rowptr, ref bluesteinsize, ref precrptr, ref preciptr, plan, _params);
            bluesteinbuf.val = new double[bluesteinsize];
            alglib.smp.ae_shared_pool_set_seed(plan.bluesteinpool, bluesteinbuf);
            
            //
            // Check that actual amount of precomputed space used by transformation
            // plan is EXACTLY equal to amount of space allocated by us.
            //
            alglib.ap.assert(precrptr==precrsize, "FTComplexFFTPlan: internal error (PrecRPtr<>PrecRSize)");
            alglib.ap.assert(preciptr==precisize, "FTComplexFFTPlan: internal error (PrecRPtr<>PrecRSize)");
        }


        /*************************************************************************
        This subroutine applies transformation plan to input/output array A.

        INPUT PARAMETERS:
            Plan        -   transformation plan
            A           -   array, must be large enough for plan to work
            OffsA       -   offset of the subarray to process
            RepCnt      -   repetition count (transformation is repeatedly applied
                            to subsequent subarrays)
            
        OUTPUT PARAMETERS:
            Plan        -   plan (temporary buffers can be modified, plan itself
                            is unchanged and can be reused)
            A           -   transformed array

          -- ALGLIB --
             Copyright 05.04.2013 by Bochkanov Sergey
        *************************************************************************/
        public static void ftapplyplan(fasttransformplan plan,
            double[] a,
            int offsa,
            int repcnt,
            alglib.xparams _params)
        {
            int plansize = 0;
            int i = 0;

            plansize = plan.entries[0,coloperandscnt]*plan.entries[0,coloperandsize]*plan.entries[0,colmicrovectorsize];
            for(i=0; i<=repcnt-1; i++)
            {
                ftapplysubplan(plan, 0, a, offsa+plansize*i, 0, plan.buffer, 1, _params);
            }
        }


        /*************************************************************************
        Returns good factorization N=N1*N2.

        Usually N1<=N2 (but not always - small N's may be exception).
        if N1<>1 then N2<>1.

        Factorization is chosen depending on task type and codelets we have.

          -- ALGLIB --
             Copyright 01.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void ftbasefactorize(int n,
            int tasktype,
            ref int n1,
            ref int n2,
            alglib.xparams _params)
        {
            int j = 0;

            n1 = 0;
            n2 = 0;

            n1 = 0;
            n2 = 0;
            
            //
            // try to find good codelet
            //
            if( n1*n2!=n )
            {
                for(j=ftbasecodeletrecommended; j>=2; j--)
                {
                    if( n%j==0 )
                    {
                        n1 = j;
                        n2 = n/j;
                        break;
                    }
                }
            }
            
            //
            // try to factorize N
            //
            if( n1*n2!=n )
            {
                for(j=ftbasecodeletrecommended+1; j<=n-1; j++)
                {
                    if( n%j==0 )
                    {
                        n1 = j;
                        n2 = n/j;
                        break;
                    }
                }
            }
            
            //
            // looks like N is prime :(
            //
            if( n1*n2!=n )
            {
                n1 = 1;
                n2 = n;
            }
            
            //
            // normalize
            //
            if( n2==1 && n1!=1 )
            {
                n2 = n1;
                n1 = 1;
            }
        }


        /*************************************************************************
        Is number smooth?

          -- ALGLIB --
             Copyright 01.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static bool ftbaseissmooth(int n,
            alglib.xparams _params)
        {
            bool result = new bool();
            int i = 0;

            for(i=2; i<=ftbasemaxsmoothfactor; i++)
            {
                while( n%i==0 )
                {
                    n = n/i;
                }
            }
            result = n==1;
            return result;
        }


        /*************************************************************************
        Returns smallest smooth (divisible only by 2, 3, 5) number that is greater
        than or equal to max(N,2)

          -- ALGLIB --
             Copyright 01.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static int ftbasefindsmooth(int n,
            alglib.xparams _params)
        {
            int result = 0;
            int best = 0;

            best = 2;
            while( best<n )
            {
                best = 2*best;
            }
            ftbasefindsmoothrec(n, 1, 2, ref best, _params);
            result = best;
            return result;
        }


        /*************************************************************************
        Returns  smallest  smooth  (divisible only by 2, 3, 5) even number that is
        greater than or equal to max(N,2)

          -- ALGLIB --
             Copyright 01.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static int ftbasefindsmootheven(int n,
            alglib.xparams _params)
        {
            int result = 0;
            int best = 0;

            best = 2;
            while( best<n )
            {
                best = 2*best;
            }
            ftbasefindsmoothrec(n, 2, 2, ref best, _params);
            result = best;
            return result;
        }


        /*************************************************************************
        Returns estimate of FLOP count for the FFT.

        It is only an estimate based on operations count for the PERFECT FFT
        and relative inefficiency of the algorithm actually used.

        N should be power of 2, estimates are badly wrong for non-power-of-2 N's.

          -- ALGLIB --
             Copyright 01.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static double ftbasegetflopestimate(int n,
            alglib.xparams _params)
        {
            double result = 0;

            result = ftbaseinefficiencyfactor*(4*n*Math.Log(n)/Math.Log(2)-6*n+8);
            return result;
        }


        /*************************************************************************
        This function returns EXACT estimate of the space requirements for N-point
        FFT. Internals of this function are highly dependent on details of different
        FFTs employed by this unit, so every time algorithm is changed this function
        has to be rewritten.

        INPUT PARAMETERS:
            N           -   transform length
            PrecRSize   -   must be set to zero
            PrecISize   -   must be set to zero
            
        OUTPUT PARAMETERS:
            PrecRSize   -   number of real temporaries required for transformation
            PrecISize   -   number of integer temporaries required for transformation

            
          -- ALGLIB --
             Copyright 05.04.2013 by Bochkanov Sergey
        *************************************************************************/
        private static void ftdeterminespacerequirements(int n,
            ref int precrsize,
            ref int precisize,
            alglib.xparams _params)
        {
            int ncur = 0;
            int f = 0;
            int i = 0;

            
            //
            // Determine required sizes of precomputed real and integer
            // buffers. This stage of code is highly dependent on internals
            // of FTComplexFFTPlanRec() and must be kept synchronized with
            // possible changes in internals of plan generation function.
            //
            // Buffer size is determined as follows:
            // * N is factorized
            // * we factor out anything which is less or equal to MaxRadix
            // * prime factor F>RaderThreshold requires 4*FTBaseFindSmooth(2*F-1)
            //   real entries to store precomputed Quantities for Bluestein's
            //   transformation
            // * prime factor F<=RaderThreshold requires 2*(F-1)+ESTIMATE(F-1)
            //   precomputed storage
            //
            ncur = n;
            for(i=2; i<=maxradix; i++)
            {
                while( ncur%i==0 )
                {
                    ncur = ncur/i;
                }
            }
            f = 2;
            while( f<=ncur )
            {
                while( ncur%f==0 )
                {
                    if( f>raderthreshold )
                    {
                        precrsize = precrsize+4*ftbasefindsmooth(2*f-1, _params);
                    }
                    else
                    {
                        precrsize = precrsize+2*(f-1);
                        ftdeterminespacerequirements(f-1, ref precrsize, ref precisize, _params);
                    }
                    ncur = ncur/f;
                }
                f = f+1;
            }
        }


        /*************************************************************************
        Recurrent function called by FTComplexFFTPlan() and other functions. It
        recursively builds transformation plan

        INPUT PARAMETERS:
            N           -   FFT length (in complex numbers), N>=1
            K           -   number of repetitions, K>=1
            ChildPlan   -   if True, plan generator inserts OpStart/opEnd in the
                            plan header/footer.
            TopmostPlan -   if True, plan generator assumes that it is topmost plan:
                            * it may use global buffer for transpositions
                            and there is no other plan which executes in parallel
            RowPtr      -   index which points to past-the-last entry generated so far
            BluesteinSize-  amount of storage (in real numbers) required for Bluestein buffer
            PrecRPtr    -   pointer to unused part of precomputed real buffer (Plan.PrecR):
                            * when this function stores some data to precomputed buffer,
                              it advances pointer.
                            * it is responsibility of the function to assert that
                              Plan.PrecR has enough space to store data before actually
                              writing to buffer.
                            * it is responsibility of the caller to allocate enough
                              space before calling this function
            PrecIPtr    -   pointer to unused part of precomputed integer buffer (Plan.PrecI):
                            * when this function stores some data to precomputed buffer,
                              it advances pointer.
                            * it is responsibility of the function to assert that
                              Plan.PrecR has enough space to store data before actually
                              writing to buffer.
                            * it is responsibility of the caller to allocate enough
                              space before calling this function
            Plan        -   plan (generated so far)
            
        OUTPUT PARAMETERS:
            RowPtr      -   updated pointer (advanced by number of entries generated
                            by function)
            BluesteinSize-  updated amount
                            (may be increased, but may never be decreased)
                
        NOTE: in case TopmostPlan is True, ChildPlan is also must be True.
            
          -- ALGLIB --
             Copyright 05.04.2013 by Bochkanov Sergey
        *************************************************************************/
        private static void ftcomplexfftplanrec(int n,
            int k,
            bool childplan,
            bool topmostplan,
            ref int rowptr,
            ref int bluesteinsize,
            ref int precrptr,
            ref int preciptr,
            fasttransformplan plan,
            alglib.xparams _params)
        {
            apserv.srealarray localbuf = new apserv.srealarray();
            int m = 0;
            int n1 = 0;
            int n2 = 0;
            int gq = 0;
            int giq = 0;
            int row0 = 0;
            int row1 = 0;
            int row2 = 0;
            int row3 = 0;

            alglib.ap.assert(n>0, "FTComplexFFTPlan: N<=0");
            alglib.ap.assert(k>0, "FTComplexFFTPlan: K<=0");
            alglib.ap.assert(!topmostplan || childplan, "FTComplexFFTPlan: ChildPlan is inconsistent with TopmostPlan");
            
            //
            // Try to generate "topmost" plan
            //
            if( topmostplan && n>recursivethreshold )
            {
                ftfactorize(n, false, ref n1, ref n2, _params);
                if( n1*n2==0 )
                {
                    
                    //
                    // Handle prime-factor FFT with Bluestein's FFT.
                    // Determine size of Bluestein's buffer.
                    //
                    m = ftbasefindsmooth(2*n-1, _params);
                    bluesteinsize = Math.Max(2*m, bluesteinsize);
                    
                    //
                    // Generate plan
                    //
                    ftpushentry2(plan, ref rowptr, opstart, k, n, 2, -1, ftoptimisticestimate(n, _params), _params);
                    ftpushentry4(plan, ref rowptr, opbluesteinsfft, k, n, 2, m, 2, precrptr, 0, _params);
                    row0 = rowptr;
                    ftpushentry(plan, ref rowptr, opjmp, 0, 0, 0, 0, _params);
                    ftcomplexfftplanrec(m, 1, true, true, ref rowptr, ref bluesteinsize, ref precrptr, ref preciptr, plan, _params);
                    row1 = rowptr;
                    plan.entries[row0,colparam0] = row1-row0;
                    ftpushentry(plan, ref rowptr, opend, k, n, 2, 0, _params);
                    
                    //
                    // Fill precomputed buffer
                    //
                    ftprecomputebluesteinsfft(n, m, plan.precr, precrptr, _params);
                    
                    //
                    // Update pointer to the precomputed area
                    //
                    precrptr = precrptr+4*m;
                }
                else
                {
                    
                    //
                    // Handle composite FFT with recursive Cooley-Tukey which
                    // uses global buffer instead of local one.
                    //
                    ftpushentry2(plan, ref rowptr, opstart, k, n, 2, -1, ftoptimisticestimate(n, _params), _params);
                    ftpushentry(plan, ref rowptr, opcomplextranspose, k, n, 2, n1, _params);
                    row0 = rowptr;
                    ftpushentry2(plan, ref rowptr, opparallelcall, k*n2, n1, 2, 0, ftoptimisticestimate(n, _params), _params);
                    ftpushentry(plan, ref rowptr, opcomplexfftfactors, k, n, 2, n1, _params);
                    ftpushentry(plan, ref rowptr, opcomplextranspose, k, n, 2, n2, _params);
                    row2 = rowptr;
                    ftpushentry2(plan, ref rowptr, opparallelcall, k*n1, n2, 2, 0, ftoptimisticestimate(n, _params), _params);
                    ftpushentry(plan, ref rowptr, opcomplextranspose, k, n, 2, n1, _params);
                    ftpushentry(plan, ref rowptr, opend, k, n, 2, 0, _params);
                    row1 = rowptr;
                    ftcomplexfftplanrec(n1, 1, true, false, ref rowptr, ref bluesteinsize, ref precrptr, ref preciptr, plan, _params);
                    plan.entries[row0,colparam0] = row1-row0;
                    row3 = rowptr;
                    ftcomplexfftplanrec(n2, 1, true, false, ref rowptr, ref bluesteinsize, ref precrptr, ref preciptr, plan, _params);
                    plan.entries[row2,colparam0] = row3-row2;
                }
                return;
            }
            
            //
            // Prepare "non-topmost" plan:
            // * calculate factorization
            // * use local (shared) buffer
            // * update buffer size - ANY plan will need at least
            //   2*N temporaries, additional requirements can be
            //   applied later
            //
            ftfactorize(n, false, ref n1, ref n2, _params);
            
            //
            // Handle FFT's with N1*N2=0: either small-N or prime-factor
            //
            if( n1*n2==0 )
            {
                if( n<=maxradix )
                {
                    
                    //
                    // Small-N FFT
                    //
                    if( childplan )
                    {
                        ftpushentry2(plan, ref rowptr, opstart, k, n, 2, -1, ftoptimisticestimate(n, _params), _params);
                    }
                    ftpushentry(plan, ref rowptr, opcomplexcodeletfft, k, n, 2, 0, _params);
                    if( childplan )
                    {
                        ftpushentry(plan, ref rowptr, opend, k, n, 2, 0, _params);
                    }
                    return;
                }
                if( n<=raderthreshold )
                {
                    
                    //
                    // Handle prime-factor FFT's with Rader's FFT
                    //
                    m = n-1;
                    if( childplan )
                    {
                        ftpushentry2(plan, ref rowptr, opstart, k, n, 2, -1, ftoptimisticestimate(n, _params), _params);
                    }
                    ntheory.findprimitiverootandinverse(n, ref gq, ref giq, _params);
                    ftpushentry4(plan, ref rowptr, opradersfft, k, n, 2, 2, gq, giq, precrptr, _params);
                    ftprecomputeradersfft(n, gq, giq, plan.precr, precrptr, _params);
                    precrptr = precrptr+2*(n-1);
                    row0 = rowptr;
                    ftpushentry(plan, ref rowptr, opjmp, 0, 0, 0, 0, _params);
                    ftcomplexfftplanrec(m, 1, true, false, ref rowptr, ref bluesteinsize, ref precrptr, ref preciptr, plan, _params);
                    row1 = rowptr;
                    plan.entries[row0,colparam0] = row1-row0;
                    if( childplan )
                    {
                        ftpushentry(plan, ref rowptr, opend, k, n, 2, 0, _params);
                    }
                }
                else
                {
                    
                    //
                    // Handle prime-factor FFT's with Bluestein's FFT
                    //
                    m = ftbasefindsmooth(2*n-1, _params);
                    bluesteinsize = Math.Max(2*m, bluesteinsize);
                    if( childplan )
                    {
                        ftpushentry2(plan, ref rowptr, opstart, k, n, 2, -1, ftoptimisticestimate(n, _params), _params);
                    }
                    ftpushentry4(plan, ref rowptr, opbluesteinsfft, k, n, 2, m, 2, precrptr, 0, _params);
                    ftprecomputebluesteinsfft(n, m, plan.precr, precrptr, _params);
                    precrptr = precrptr+4*m;
                    row0 = rowptr;
                    ftpushentry(plan, ref rowptr, opjmp, 0, 0, 0, 0, _params);
                    ftcomplexfftplanrec(m, 1, true, false, ref rowptr, ref bluesteinsize, ref precrptr, ref preciptr, plan, _params);
                    row1 = rowptr;
                    plan.entries[row0,colparam0] = row1-row0;
                    if( childplan )
                    {
                        ftpushentry(plan, ref rowptr, opend, k, n, 2, 0, _params);
                    }
                }
                return;
            }
            
            //
            // Handle Cooley-Tukey FFT with small N1
            //
            if( n1<=maxradix )
            {
                
                //
                // Specialized transformation for small N1:
                // * N2 short inplace FFT's, each N1-point, with integrated twiddle factors
                // * N1 long FFT's
                // * final transposition
                //
                if( childplan )
                {
                    ftpushentry2(plan, ref rowptr, opstart, k, n, 2, -1, ftoptimisticestimate(n, _params), _params);
                }
                ftpushentry(plan, ref rowptr, opcomplexcodelettwfft, k, n1, 2*n2, 0, _params);
                ftcomplexfftplanrec(n2, k*n1, false, false, ref rowptr, ref bluesteinsize, ref precrptr, ref preciptr, plan, _params);
                ftpushentry(plan, ref rowptr, opcomplextranspose, k, n, 2, n1, _params);
                if( childplan )
                {
                    ftpushentry(plan, ref rowptr, opend, k, n, 2, 0, _params);
                }
                return;
            }
            
            //
            // Handle general Cooley-Tukey FFT, either "flat" or "recursive"
            //
            if( n<=recursivethreshold )
            {
                
                //
                // General code for large N1/N2, "flat" version without explicit recurrence
                // (nested subplans are inserted directly into the body of the plan)
                //
                if( childplan )
                {
                    ftpushentry2(plan, ref rowptr, opstart, k, n, 2, -1, ftoptimisticestimate(n, _params), _params);
                }
                ftpushentry(plan, ref rowptr, opcomplextranspose, k, n, 2, n1, _params);
                ftcomplexfftplanrec(n1, k*n2, false, false, ref rowptr, ref bluesteinsize, ref precrptr, ref preciptr, plan, _params);
                ftpushentry(plan, ref rowptr, opcomplexfftfactors, k, n, 2, n1, _params);
                ftpushentry(plan, ref rowptr, opcomplextranspose, k, n, 2, n2, _params);
                ftcomplexfftplanrec(n2, k*n1, false, false, ref rowptr, ref bluesteinsize, ref precrptr, ref preciptr, plan, _params);
                ftpushentry(plan, ref rowptr, opcomplextranspose, k, n, 2, n1, _params);
                if( childplan )
                {
                    ftpushentry(plan, ref rowptr, opend, k, n, 2, 0, _params);
                }
            }
            else
            {
                
                //
                // General code for large N1/N2, "recursive" version - nested subplans
                // are separated from the plan body.
                //
                // Generate parent plan.
                //
                if( childplan )
                {
                    ftpushentry2(plan, ref rowptr, opstart, k, n, 2, -1, ftoptimisticestimate(n, _params), _params);
                }
                ftpushentry(plan, ref rowptr, opcomplextranspose, k, n, 2, n1, _params);
                row0 = rowptr;
                ftpushentry2(plan, ref rowptr, opparallelcall, k*n2, n1, 2, 0, ftoptimisticestimate(n, _params), _params);
                ftpushentry(plan, ref rowptr, opcomplexfftfactors, k, n, 2, n1, _params);
                ftpushentry(plan, ref rowptr, opcomplextranspose, k, n, 2, n2, _params);
                row2 = rowptr;
                ftpushentry2(plan, ref rowptr, opparallelcall, k*n1, n2, 2, 0, ftoptimisticestimate(n, _params), _params);
                ftpushentry(plan, ref rowptr, opcomplextranspose, k, n, 2, n1, _params);
                if( childplan )
                {
                    ftpushentry(plan, ref rowptr, opend, k, n, 2, 0, _params);
                }
                
                //
                // Generate child subplans, insert refence to parent plans
                //
                row1 = rowptr;
                ftcomplexfftplanrec(n1, 1, true, false, ref rowptr, ref bluesteinsize, ref precrptr, ref preciptr, plan, _params);
                plan.entries[row0,colparam0] = row1-row0;
                row3 = rowptr;
                ftcomplexfftplanrec(n2, 1, true, false, ref rowptr, ref bluesteinsize, ref precrptr, ref preciptr, plan, _params);
                plan.entries[row2,colparam0] = row3-row2;
            }
        }


        /*************************************************************************
        This function pushes one more entry to the plan. It resizes Entries matrix
        if needed.

        INPUT PARAMETERS:
            Plan        -   plan (generated so far)
            RowPtr      -   index which points to past-the-last entry generated so far
            EType       -   entry type
            EOpCnt      -   operands count
            EOpSize     -   operand size
            EMcvSize    -   microvector size
            EParam0     -   parameter 0
            
        OUTPUT PARAMETERS:
            Plan        -   updated plan    
            RowPtr      -   updated pointer

        NOTE: Param1 is set to -1.
            
          -- ALGLIB --
             Copyright 05.04.2013 by Bochkanov Sergey
        *************************************************************************/
        private static void ftpushentry(fasttransformplan plan,
            ref int rowptr,
            int etype,
            int eopcnt,
            int eopsize,
            int emcvsize,
            int eparam0,
            alglib.xparams _params)
        {
            ftpushentry2(plan, ref rowptr, etype, eopcnt, eopsize, emcvsize, eparam0, -1, _params);
        }


        /*************************************************************************
        Same as FTPushEntry(), but sets Param0 AND Param1.
        This function pushes one more entry to the plan. It resized Entries matrix
        if needed.

        INPUT PARAMETERS:
            Plan        -   plan (generated so far)
            RowPtr      -   index which points to past-the-last entry generated so far
            EType       -   entry type
            EOpCnt      -   operands count
            EOpSize     -   operand size
            EMcvSize    -   microvector size
            EParam0     -   parameter 0
            EParam1     -   parameter 1
            
        OUTPUT PARAMETERS:
            Plan        -   updated plan    
            RowPtr      -   updated pointer

          -- ALGLIB --
             Copyright 05.04.2013 by Bochkanov Sergey
        *************************************************************************/
        private static void ftpushentry2(fasttransformplan plan,
            ref int rowptr,
            int etype,
            int eopcnt,
            int eopsize,
            int emcvsize,
            int eparam0,
            int eparam1,
            alglib.xparams _params)
        {
            if( rowptr>=alglib.ap.rows(plan.entries) )
            {
                apserv.imatrixresize(ref plan.entries, Math.Max(2*alglib.ap.rows(plan.entries), 1), colscnt, _params);
            }
            plan.entries[rowptr,coltype] = etype;
            plan.entries[rowptr,coloperandscnt] = eopcnt;
            plan.entries[rowptr,coloperandsize] = eopsize;
            plan.entries[rowptr,colmicrovectorsize] = emcvsize;
            plan.entries[rowptr,colparam0] = eparam0;
            plan.entries[rowptr,colparam1] = eparam1;
            plan.entries[rowptr,colparam2] = 0;
            plan.entries[rowptr,colparam3] = 0;
            rowptr = rowptr+1;
        }


        /*************************************************************************
        Same as FTPushEntry(), but sets Param0, Param1, Param2 and Param3.
        This function pushes one more entry to the plan. It resized Entries matrix
        if needed.

        INPUT PARAMETERS:
            Plan        -   plan (generated so far)
            RowPtr      -   index which points to past-the-last entry generated so far
            EType       -   entry type
            EOpCnt      -   operands count
            EOpSize     -   operand size
            EMcvSize    -   microvector size
            EParam0     -   parameter 0
            EParam1     -   parameter 1
            EParam2     -   parameter 2
            EParam3     -   parameter 3
            
        OUTPUT PARAMETERS:
            Plan        -   updated plan    
            RowPtr      -   updated pointer

          -- ALGLIB --
             Copyright 05.04.2013 by Bochkanov Sergey
        *************************************************************************/
        private static void ftpushentry4(fasttransformplan plan,
            ref int rowptr,
            int etype,
            int eopcnt,
            int eopsize,
            int emcvsize,
            int eparam0,
            int eparam1,
            int eparam2,
            int eparam3,
            alglib.xparams _params)
        {
            if( rowptr>=alglib.ap.rows(plan.entries) )
            {
                apserv.imatrixresize(ref plan.entries, Math.Max(2*alglib.ap.rows(plan.entries), 1), colscnt, _params);
            }
            plan.entries[rowptr,coltype] = etype;
            plan.entries[rowptr,coloperandscnt] = eopcnt;
            plan.entries[rowptr,coloperandsize] = eopsize;
            plan.entries[rowptr,colmicrovectorsize] = emcvsize;
            plan.entries[rowptr,colparam0] = eparam0;
            plan.entries[rowptr,colparam1] = eparam1;
            plan.entries[rowptr,colparam2] = eparam2;
            plan.entries[rowptr,colparam3] = eparam3;
            rowptr = rowptr+1;
        }


        /*************************************************************************
        This subroutine applies subplan to input/output array A.

        INPUT PARAMETERS:
            Plan        -   transformation plan
            SubPlan     -   subplan index
            A           -   array, must be large enough for plan to work
            ABase       -   base offset in array A, this value points to start of
                            subarray whose length is equal to length of the plan
            AOffset     -   offset with respect to ABase, 0<=AOffset<PlanLength.
                            This is an offset within large PlanLength-subarray of
                            the chunk to process.
            Buf         -   temporary buffer whose length is equal to plan length
                            (without taking into account RepCnt) or larger.
            OffsBuf     -   offset in the buffer array
            RepCnt      -   repetition count (transformation is repeatedly applied
                            to subsequent subarrays)
            
        OUTPUT PARAMETERS:
            Plan        -   plan (temporary buffers can be modified, plan itself
                            is unchanged and can be reused)
            A           -   transformed array

          -- ALGLIB --
             Copyright 05.04.2013 by Bochkanov Sergey
        *************************************************************************/
        private static void ftapplysubplan(fasttransformplan plan,
            int subplan,
            double[] a,
            int abase,
            int aoffset,
            double[] buf,
            int repcnt,
            alglib.xparams _params)
        {
            int rowidx = 0;
            int i = 0;
            int n1 = 0;
            int n2 = 0;
            int operation = 0;
            int operandscnt = 0;
            int operandsize = 0;
            int microvectorsize = 0;
            int param0 = 0;
            int param1 = 0;
            int parentsize = 0;
            int childsize = 0;
            int chunksize = 0;
            int lastchunksize = 0;
            apserv.srealarray bufa = null;
            apserv.srealarray bufb = null;
            apserv.srealarray bufc = null;
            apserv.srealarray bufd = null;

            alglib.ap.assert(plan.entries[subplan,coltype]==opstart, "FTApplySubPlan: incorrect subplan header");
            rowidx = subplan+1;
            while( plan.entries[rowidx,coltype]!=opend )
            {
                operation = plan.entries[rowidx,coltype];
                operandscnt = repcnt*plan.entries[rowidx,coloperandscnt];
                operandsize = plan.entries[rowidx,coloperandsize];
                microvectorsize = plan.entries[rowidx,colmicrovectorsize];
                param0 = plan.entries[rowidx,colparam0];
                param1 = plan.entries[rowidx,colparam1];
                apserv.touchint(ref param1, _params);
                
                //
                // Process "jump" operation
                //
                if( operation==opjmp )
                {
                    rowidx = rowidx+plan.entries[rowidx,colparam0];
                    continue;
                }
                
                //
                // Process "parallel call" operation:
                // * we perform initial check for consistency between parent and child plans
                // * we call FTSplitAndApplyParallelPlan(), which splits parallel plan into
                //   several parallel tasks
                //
                if( operation==opparallelcall )
                {
                    parentsize = operandsize*microvectorsize;
                    childsize = plan.entries[rowidx+param0,coloperandscnt]*plan.entries[rowidx+param0,coloperandsize]*plan.entries[rowidx+param0,colmicrovectorsize];
                    alglib.ap.assert(plan.entries[rowidx+param0,coltype]==opstart, "FTApplySubPlan: incorrect child subplan header");
                    alglib.ap.assert(parentsize==childsize, "FTApplySubPlan: incorrect child subplan header");
                    chunksize = Math.Max(recursivethreshold/childsize, 1);
                    lastchunksize = operandscnt%chunksize;
                    if( lastchunksize==0 )
                    {
                        lastchunksize = chunksize;
                    }
                    i = 0;
                    while( i<operandscnt )
                    {
                        chunksize = Math.Min(chunksize, operandscnt-i);
                        ftapplysubplan(plan, rowidx+param0, a, abase, aoffset+i*childsize, buf, chunksize, _params);
                        i = i+chunksize;
                    }
                    rowidx = rowidx+1;
                    continue;
                }
                
                //
                // Process "reference complex FFT" operation
                //
                if( operation==opcomplexreffft )
                {
                    ftapplycomplexreffft(a, abase+aoffset, operandscnt, operandsize, microvectorsize, buf, _params);
                    rowidx = rowidx+1;
                    continue;
                }
                
                //
                // Process "codelet FFT" operation
                //
                if( operation==opcomplexcodeletfft )
                {
                    ftapplycomplexcodeletfft(a, abase+aoffset, operandscnt, operandsize, microvectorsize, _params);
                    rowidx = rowidx+1;
                    continue;
                }
                
                //
                // Process "integrated codelet FFT" operation
                //
                if( operation==opcomplexcodelettwfft )
                {
                    ftapplycomplexcodelettwfft(a, abase+aoffset, operandscnt, operandsize, microvectorsize, _params);
                    rowidx = rowidx+1;
                    continue;
                }
                
                //
                // Process Bluestein's FFT operation
                //
                if( operation==opbluesteinsfft )
                {
                    alglib.ap.assert(microvectorsize==2, "FTApplySubPlan: microvectorsize!=2 for Bluesteins FFT");
                    alglib.smp.ae_shared_pool_retrieve(plan.bluesteinpool, ref bufa);
                    alglib.smp.ae_shared_pool_retrieve(plan.bluesteinpool, ref bufb);
                    alglib.smp.ae_shared_pool_retrieve(plan.bluesteinpool, ref bufc);
                    alglib.smp.ae_shared_pool_retrieve(plan.bluesteinpool, ref bufd);
                    ftbluesteinsfft(plan, a, abase, aoffset, operandscnt, operandsize, plan.entries[rowidx,colparam0], plan.entries[rowidx,colparam2], rowidx+plan.entries[rowidx,colparam1], bufa.val, bufb.val, bufc.val, bufd.val, _params);
                    alglib.smp.ae_shared_pool_recycle(plan.bluesteinpool, ref bufa);
                    alglib.smp.ae_shared_pool_recycle(plan.bluesteinpool, ref bufb);
                    alglib.smp.ae_shared_pool_recycle(plan.bluesteinpool, ref bufc);
                    alglib.smp.ae_shared_pool_recycle(plan.bluesteinpool, ref bufd);
                    rowidx = rowidx+1;
                    continue;
                }
                
                //
                // Process Rader's FFT
                //
                if( operation==opradersfft )
                {
                    ftradersfft(plan, a, abase, aoffset, operandscnt, operandsize, rowidx+plan.entries[rowidx,colparam0], plan.entries[rowidx,colparam1], plan.entries[rowidx,colparam2], plan.entries[rowidx,colparam3], buf, _params);
                    rowidx = rowidx+1;
                    continue;
                }
                
                //
                // Process "complex twiddle factors" operation
                //
                if( operation==opcomplexfftfactors )
                {
                    alglib.ap.assert(microvectorsize==2, "FTApplySubPlan: MicrovectorSize<>1");
                    n1 = plan.entries[rowidx,colparam0];
                    n2 = operandsize/n1;
                    for(i=0; i<=operandscnt-1; i++)
                    {
                        ffttwcalc(a, abase+aoffset+i*operandsize*2, n1, n2, _params);
                    }
                    rowidx = rowidx+1;
                    continue;
                }
                
                //
                // Process "complex transposition" operation
                //
                if( operation==opcomplextranspose )
                {
                    alglib.ap.assert(microvectorsize==2, "FTApplySubPlan: MicrovectorSize<>1");
                    n1 = plan.entries[rowidx,colparam0];
                    n2 = operandsize/n1;
                    for(i=0; i<=operandscnt-1; i++)
                    {
                        internalcomplexlintranspose(a, n1, n2, abase+aoffset+i*operandsize*2, buf, _params);
                    }
                    rowidx = rowidx+1;
                    continue;
                }
                
                //
                // Error
                //
                alglib.ap.assert(false, "FTApplySubPlan: unexpected plan type");
            }
        }


        /*************************************************************************
        This subroutine applies complex reference FFT to input/output array A.

        VERY SLOW OPERATION, do not use it in real life plans :)

        INPUT PARAMETERS:
            A           -   array, must be large enough for plan to work
            Offs        -   offset of the subarray to process
            OperandsCnt -   operands count (see description of FastTransformPlan)
            OperandSize -   operand size (see description of FastTransformPlan)
            MicrovectorSize-microvector size (see description of FastTransformPlan)
            Buf         -   temporary array, must be at least OperandsCnt*OperandSize*MicrovectorSize
            
        OUTPUT PARAMETERS:
            A           -   transformed array

          -- ALGLIB --
             Copyright 05.04.2013 by Bochkanov Sergey
        *************************************************************************/
        private static void ftapplycomplexreffft(double[] a,
            int offs,
            int operandscnt,
            int operandsize,
            int microvectorsize,
            double[] buf,
            alglib.xparams _params)
        {
            int opidx = 0;
            int i = 0;
            int k = 0;
            double hre = 0;
            double him = 0;
            double c = 0;
            double s = 0;
            double re = 0;
            double im = 0;
            int n = 0;

            alglib.ap.assert(operandscnt>=1, "FTApplyComplexRefFFT: OperandsCnt<1");
            alglib.ap.assert(operandsize>=1, "FTApplyComplexRefFFT: OperandSize<1");
            alglib.ap.assert(microvectorsize==2, "FTApplyComplexRefFFT: MicrovectorSize<>2");
            n = operandsize;
            for(opidx=0; opidx<=operandscnt-1; opidx++)
            {
                for(i=0; i<=n-1; i++)
                {
                    hre = 0;
                    him = 0;
                    for(k=0; k<=n-1; k++)
                    {
                        re = a[offs+opidx*operandsize*2+2*k+0];
                        im = a[offs+opidx*operandsize*2+2*k+1];
                        c = Math.Cos(-(2*Math.PI*k*i/n));
                        s = Math.Sin(-(2*Math.PI*k*i/n));
                        hre = hre+c*re-s*im;
                        him = him+c*im+s*re;
                    }
                    buf[2*i+0] = hre;
                    buf[2*i+1] = him;
                }
                for(i=0; i<=operandsize*2-1; i++)
                {
                    a[offs+opidx*operandsize*2+i] = buf[i];
                }
            }
        }


        /*************************************************************************
        This subroutine applies complex codelet FFT to input/output array A.

        INPUT PARAMETERS:
            A           -   array, must be large enough for plan to work
            Offs        -   offset of the subarray to process
            OperandsCnt -   operands count (see description of FastTransformPlan)
            OperandSize -   operand size (see description of FastTransformPlan)
            MicrovectorSize-microvector size, must be 2
            
        OUTPUT PARAMETERS:
            A           -   transformed array

          -- ALGLIB --
             Copyright 05.04.2013 by Bochkanov Sergey
        *************************************************************************/
        private static void ftapplycomplexcodeletfft(double[] a,
            int offs,
            int operandscnt,
            int operandsize,
            int microvectorsize,
            alglib.xparams _params)
        {
            int opidx = 0;
            int n = 0;
            int aoffset = 0;
            double a0x = 0;
            double a0y = 0;
            double a1x = 0;
            double a1y = 0;
            double a2x = 0;
            double a2y = 0;
            double a3x = 0;
            double a3y = 0;
            double a4x = 0;
            double a4y = 0;
            double a5x = 0;
            double a5y = 0;
            double v0 = 0;
            double v1 = 0;
            double v2 = 0;
            double v3 = 0;
            double t1x = 0;
            double t1y = 0;
            double t2x = 0;
            double t2y = 0;
            double t3x = 0;
            double t3y = 0;
            double t4x = 0;
            double t4y = 0;
            double t5x = 0;
            double t5y = 0;
            double m1x = 0;
            double m1y = 0;
            double m2x = 0;
            double m2y = 0;
            double m3x = 0;
            double m3y = 0;
            double m4x = 0;
            double m4y = 0;
            double m5x = 0;
            double m5y = 0;
            double s1x = 0;
            double s1y = 0;
            double s2x = 0;
            double s2y = 0;
            double s3x = 0;
            double s3y = 0;
            double s4x = 0;
            double s4y = 0;
            double s5x = 0;
            double s5y = 0;
            double c1 = 0;
            double c2 = 0;
            double c3 = 0;
            double c4 = 0;
            double c5 = 0;
            double v = 0;

            alglib.ap.assert(operandscnt>=1, "FTApplyComplexCodeletFFT: OperandsCnt<1");
            alglib.ap.assert(operandsize>=1, "FTApplyComplexCodeletFFT: OperandSize<1");
            alglib.ap.assert(microvectorsize==2, "FTApplyComplexCodeletFFT: MicrovectorSize<>2");
            n = operandsize;
            
            //
            // Hard-coded transforms for different N's
            //
            alglib.ap.assert(n<=maxradix, "FTApplyComplexCodeletFFT: N>MaxRadix");
            if( n==2 )
            {
                for(opidx=0; opidx<=operandscnt-1; opidx++)
                {
                    aoffset = offs+opidx*operandsize*2;
                    a0x = a[aoffset+0];
                    a0y = a[aoffset+1];
                    a1x = a[aoffset+2];
                    a1y = a[aoffset+3];
                    v0 = a0x+a1x;
                    v1 = a0y+a1y;
                    v2 = a0x-a1x;
                    v3 = a0y-a1y;
                    a[aoffset+0] = v0;
                    a[aoffset+1] = v1;
                    a[aoffset+2] = v2;
                    a[aoffset+3] = v3;
                }
                return;
            }
            if( n==3 )
            {
                c1 = Math.Cos(2*Math.PI/3)-1;
                c2 = Math.Sin(2*Math.PI/3);
                for(opidx=0; opidx<=operandscnt-1; opidx++)
                {
                    aoffset = offs+opidx*operandsize*2;
                    a0x = a[aoffset+0];
                    a0y = a[aoffset+1];
                    a1x = a[aoffset+2];
                    a1y = a[aoffset+3];
                    a2x = a[aoffset+4];
                    a2y = a[aoffset+5];
                    t1x = a1x+a2x;
                    t1y = a1y+a2y;
                    a0x = a0x+t1x;
                    a0y = a0y+t1y;
                    m1x = c1*t1x;
                    m1y = c1*t1y;
                    m2x = c2*(a1y-a2y);
                    m2y = c2*(a2x-a1x);
                    s1x = a0x+m1x;
                    s1y = a0y+m1y;
                    a1x = s1x+m2x;
                    a1y = s1y+m2y;
                    a2x = s1x-m2x;
                    a2y = s1y-m2y;
                    a[aoffset+0] = a0x;
                    a[aoffset+1] = a0y;
                    a[aoffset+2] = a1x;
                    a[aoffset+3] = a1y;
                    a[aoffset+4] = a2x;
                    a[aoffset+5] = a2y;
                }
                return;
            }
            if( n==4 )
            {
                for(opidx=0; opidx<=operandscnt-1; opidx++)
                {
                    aoffset = offs+opidx*operandsize*2;
                    a0x = a[aoffset+0];
                    a0y = a[aoffset+1];
                    a1x = a[aoffset+2];
                    a1y = a[aoffset+3];
                    a2x = a[aoffset+4];
                    a2y = a[aoffset+5];
                    a3x = a[aoffset+6];
                    a3y = a[aoffset+7];
                    t1x = a0x+a2x;
                    t1y = a0y+a2y;
                    t2x = a1x+a3x;
                    t2y = a1y+a3y;
                    m2x = a0x-a2x;
                    m2y = a0y-a2y;
                    m3x = a1y-a3y;
                    m3y = a3x-a1x;
                    a[aoffset+0] = t1x+t2x;
                    a[aoffset+1] = t1y+t2y;
                    a[aoffset+4] = t1x-t2x;
                    a[aoffset+5] = t1y-t2y;
                    a[aoffset+2] = m2x+m3x;
                    a[aoffset+3] = m2y+m3y;
                    a[aoffset+6] = m2x-m3x;
                    a[aoffset+7] = m2y-m3y;
                }
                return;
            }
            if( n==5 )
            {
                v = 2*Math.PI/5;
                c1 = (Math.Cos(v)+Math.Cos(2*v))/2-1;
                c2 = (Math.Cos(v)-Math.Cos(2*v))/2;
                c3 = -Math.Sin(v);
                c4 = -(Math.Sin(v)+Math.Sin(2*v));
                c5 = Math.Sin(v)-Math.Sin(2*v);
                for(opidx=0; opidx<=operandscnt-1; opidx++)
                {
                    aoffset = offs+opidx*operandsize*2;
                    t1x = a[aoffset+2]+a[aoffset+8];
                    t1y = a[aoffset+3]+a[aoffset+9];
                    t2x = a[aoffset+4]+a[aoffset+6];
                    t2y = a[aoffset+5]+a[aoffset+7];
                    t3x = a[aoffset+2]-a[aoffset+8];
                    t3y = a[aoffset+3]-a[aoffset+9];
                    t4x = a[aoffset+6]-a[aoffset+4];
                    t4y = a[aoffset+7]-a[aoffset+5];
                    t5x = t1x+t2x;
                    t5y = t1y+t2y;
                    a[aoffset+0] = a[aoffset+0]+t5x;
                    a[aoffset+1] = a[aoffset+1]+t5y;
                    m1x = c1*t5x;
                    m1y = c1*t5y;
                    m2x = c2*(t1x-t2x);
                    m2y = c2*(t1y-t2y);
                    m3x = -(c3*(t3y+t4y));
                    m3y = c3*(t3x+t4x);
                    m4x = -(c4*t4y);
                    m4y = c4*t4x;
                    m5x = -(c5*t3y);
                    m5y = c5*t3x;
                    s3x = m3x-m4x;
                    s3y = m3y-m4y;
                    s5x = m3x+m5x;
                    s5y = m3y+m5y;
                    s1x = a[aoffset+0]+m1x;
                    s1y = a[aoffset+1]+m1y;
                    s2x = s1x+m2x;
                    s2y = s1y+m2y;
                    s4x = s1x-m2x;
                    s4y = s1y-m2y;
                    a[aoffset+2] = s2x+s3x;
                    a[aoffset+3] = s2y+s3y;
                    a[aoffset+4] = s4x+s5x;
                    a[aoffset+5] = s4y+s5y;
                    a[aoffset+6] = s4x-s5x;
                    a[aoffset+7] = s4y-s5y;
                    a[aoffset+8] = s2x-s3x;
                    a[aoffset+9] = s2y-s3y;
                }
                return;
            }
            if( n==6 )
            {
                c1 = Math.Cos(2*Math.PI/3)-1;
                c2 = Math.Sin(2*Math.PI/3);
                c3 = Math.Cos(-(Math.PI/3));
                c4 = Math.Sin(-(Math.PI/3));
                for(opidx=0; opidx<=operandscnt-1; opidx++)
                {
                    aoffset = offs+opidx*operandsize*2;
                    a0x = a[aoffset+0];
                    a0y = a[aoffset+1];
                    a1x = a[aoffset+2];
                    a1y = a[aoffset+3];
                    a2x = a[aoffset+4];
                    a2y = a[aoffset+5];
                    a3x = a[aoffset+6];
                    a3y = a[aoffset+7];
                    a4x = a[aoffset+8];
                    a4y = a[aoffset+9];
                    a5x = a[aoffset+10];
                    a5y = a[aoffset+11];
                    v0 = a0x;
                    v1 = a0y;
                    a0x = a0x+a3x;
                    a0y = a0y+a3y;
                    a3x = v0-a3x;
                    a3y = v1-a3y;
                    v0 = a1x;
                    v1 = a1y;
                    a1x = a1x+a4x;
                    a1y = a1y+a4y;
                    a4x = v0-a4x;
                    a4y = v1-a4y;
                    v0 = a2x;
                    v1 = a2y;
                    a2x = a2x+a5x;
                    a2y = a2y+a5y;
                    a5x = v0-a5x;
                    a5y = v1-a5y;
                    t4x = a4x*c3-a4y*c4;
                    t4y = a4x*c4+a4y*c3;
                    a4x = t4x;
                    a4y = t4y;
                    t5x = -(a5x*c3)-a5y*c4;
                    t5y = a5x*c4-a5y*c3;
                    a5x = t5x;
                    a5y = t5y;
                    t1x = a1x+a2x;
                    t1y = a1y+a2y;
                    a0x = a0x+t1x;
                    a0y = a0y+t1y;
                    m1x = c1*t1x;
                    m1y = c1*t1y;
                    m2x = c2*(a1y-a2y);
                    m2y = c2*(a2x-a1x);
                    s1x = a0x+m1x;
                    s1y = a0y+m1y;
                    a1x = s1x+m2x;
                    a1y = s1y+m2y;
                    a2x = s1x-m2x;
                    a2y = s1y-m2y;
                    t1x = a4x+a5x;
                    t1y = a4y+a5y;
                    a3x = a3x+t1x;
                    a3y = a3y+t1y;
                    m1x = c1*t1x;
                    m1y = c1*t1y;
                    m2x = c2*(a4y-a5y);
                    m2y = c2*(a5x-a4x);
                    s1x = a3x+m1x;
                    s1y = a3y+m1y;
                    a4x = s1x+m2x;
                    a4y = s1y+m2y;
                    a5x = s1x-m2x;
                    a5y = s1y-m2y;
                    a[aoffset+0] = a0x;
                    a[aoffset+1] = a0y;
                    a[aoffset+2] = a3x;
                    a[aoffset+3] = a3y;
                    a[aoffset+4] = a1x;
                    a[aoffset+5] = a1y;
                    a[aoffset+6] = a4x;
                    a[aoffset+7] = a4y;
                    a[aoffset+8] = a2x;
                    a[aoffset+9] = a2y;
                    a[aoffset+10] = a5x;
                    a[aoffset+11] = a5y;
                }
                return;
            }
        }


        /*************************************************************************
        This subroutine applies complex "integrated" codelet FFT  to  input/output
        array A. "Integrated" codelet differs from "normal" one in following ways:
        * it can work with MicrovectorSize>1
        * hence, it can be used in Cooley-Tukey FFT without transpositions
        * it performs inlined multiplication by twiddle factors of Cooley-Tukey
          FFT with N2=MicrovectorSize/2.

        INPUT PARAMETERS:
            A           -   array, must be large enough for plan to work
            Offs        -   offset of the subarray to process
            OperandsCnt -   operands count (see description of FastTransformPlan)
            OperandSize -   operand size (see description of FastTransformPlan)
            MicrovectorSize-microvector size, must be 1
            
        OUTPUT PARAMETERS:
            A           -   transformed array

          -- ALGLIB --
             Copyright 05.04.2013 by Bochkanov Sergey
        *************************************************************************/
        private static void ftapplycomplexcodelettwfft(double[] a,
            int offs,
            int operandscnt,
            int operandsize,
            int microvectorsize,
            alglib.xparams _params)
        {
            int opidx = 0;
            int mvidx = 0;
            int n = 0;
            int m = 0;
            int aoffset0 = 0;
            int aoffset2 = 0;
            int aoffset4 = 0;
            int aoffset6 = 0;
            int aoffset8 = 0;
            int aoffset10 = 0;
            double a0x = 0;
            double a0y = 0;
            double a1x = 0;
            double a1y = 0;
            double a2x = 0;
            double a2y = 0;
            double a3x = 0;
            double a3y = 0;
            double a4x = 0;
            double a4y = 0;
            double a5x = 0;
            double a5y = 0;
            double v0 = 0;
            double v1 = 0;
            double v2 = 0;
            double v3 = 0;
            double q0x = 0;
            double q0y = 0;
            double t1x = 0;
            double t1y = 0;
            double t2x = 0;
            double t2y = 0;
            double t3x = 0;
            double t3y = 0;
            double t4x = 0;
            double t4y = 0;
            double t5x = 0;
            double t5y = 0;
            double m1x = 0;
            double m1y = 0;
            double m2x = 0;
            double m2y = 0;
            double m3x = 0;
            double m3y = 0;
            double m4x = 0;
            double m4y = 0;
            double m5x = 0;
            double m5y = 0;
            double s1x = 0;
            double s1y = 0;
            double s2x = 0;
            double s2y = 0;
            double s3x = 0;
            double s3y = 0;
            double s4x = 0;
            double s4y = 0;
            double s5x = 0;
            double s5y = 0;
            double c1 = 0;
            double c2 = 0;
            double c3 = 0;
            double c4 = 0;
            double c5 = 0;
            double v = 0;
            double tw0 = 0;
            double tw1 = 0;
            double twx = 0;
            double twxm1 = 0;
            double twy = 0;
            double tw2x = 0;
            double tw2y = 0;
            double tw3x = 0;
            double tw3y = 0;
            double tw4x = 0;
            double tw4y = 0;
            double tw5x = 0;
            double tw5y = 0;

            alglib.ap.assert(operandscnt>=1, "FTApplyComplexCodeletFFT: OperandsCnt<1");
            alglib.ap.assert(operandsize>=1, "FTApplyComplexCodeletFFT: OperandSize<1");
            alglib.ap.assert(microvectorsize>=1, "FTApplyComplexCodeletFFT: MicrovectorSize<>1");
            alglib.ap.assert(microvectorsize%2==0, "FTApplyComplexCodeletFFT: MicrovectorSize is not even");
            n = operandsize;
            m = microvectorsize/2;
            
            //
            // Hard-coded transforms for different N's
            //
            alglib.ap.assert(n<=maxradix, "FTApplyComplexCodeletTwFFT: N>MaxRadix");
            if( n==2 )
            {
                v = -(2*Math.PI/(n*m));
                tw0 = -(2*math.sqr(Math.Sin(0.5*v)));
                tw1 = Math.Sin(v);
                for(opidx=0; opidx<=operandscnt-1; opidx++)
                {
                    aoffset0 = offs+opidx*operandsize*microvectorsize;
                    aoffset2 = aoffset0+microvectorsize;
                    twxm1 = 0.0;
                    twy = 0.0;
                    for(mvidx=0; mvidx<=m-1; mvidx++)
                    {
                        a0x = a[aoffset0];
                        a0y = a[aoffset0+1];
                        a1x = a[aoffset2];
                        a1y = a[aoffset2+1];
                        v0 = a0x+a1x;
                        v1 = a0y+a1y;
                        v2 = a0x-a1x;
                        v3 = a0y-a1y;
                        a[aoffset0] = v0;
                        a[aoffset0+1] = v1;
                        a[aoffset2] = v2*(1+twxm1)-v3*twy;
                        a[aoffset2+1] = v3*(1+twxm1)+v2*twy;
                        aoffset0 = aoffset0+2;
                        aoffset2 = aoffset2+2;
                        if( (mvidx+1)%updatetw==0 )
                        {
                            v = -(2*Math.PI*(mvidx+1)/(n*m));
                            twxm1 = Math.Sin(0.5*v);
                            twxm1 = -(2*twxm1*twxm1);
                            twy = Math.Sin(v);
                        }
                        else
                        {
                            v = twxm1+tw0+twxm1*tw0-twy*tw1;
                            twy = twy+tw1+twxm1*tw1+twy*tw0;
                            twxm1 = v;
                        }
                    }
                }
                return;
            }
            if( n==3 )
            {
                v = -(2*Math.PI/(n*m));
                tw0 = -(2*math.sqr(Math.Sin(0.5*v)));
                tw1 = Math.Sin(v);
                c1 = Math.Cos(2*Math.PI/3)-1;
                c2 = Math.Sin(2*Math.PI/3);
                for(opidx=0; opidx<=operandscnt-1; opidx++)
                {
                    aoffset0 = offs+opidx*operandsize*microvectorsize;
                    aoffset2 = aoffset0+microvectorsize;
                    aoffset4 = aoffset2+microvectorsize;
                    twx = 1.0;
                    twxm1 = 0.0;
                    twy = 0.0;
                    for(mvidx=0; mvidx<=m-1; mvidx++)
                    {
                        a0x = a[aoffset0];
                        a0y = a[aoffset0+1];
                        a1x = a[aoffset2];
                        a1y = a[aoffset2+1];
                        a2x = a[aoffset4];
                        a2y = a[aoffset4+1];
                        t1x = a1x+a2x;
                        t1y = a1y+a2y;
                        a0x = a0x+t1x;
                        a0y = a0y+t1y;
                        m1x = c1*t1x;
                        m1y = c1*t1y;
                        m2x = c2*(a1y-a2y);
                        m2y = c2*(a2x-a1x);
                        s1x = a0x+m1x;
                        s1y = a0y+m1y;
                        a1x = s1x+m2x;
                        a1y = s1y+m2y;
                        a2x = s1x-m2x;
                        a2y = s1y-m2y;
                        tw2x = twx*twx-twy*twy;
                        tw2y = 2*twx*twy;
                        a[aoffset0] = a0x;
                        a[aoffset0+1] = a0y;
                        a[aoffset2] = a1x*twx-a1y*twy;
                        a[aoffset2+1] = a1y*twx+a1x*twy;
                        a[aoffset4] = a2x*tw2x-a2y*tw2y;
                        a[aoffset4+1] = a2y*tw2x+a2x*tw2y;
                        aoffset0 = aoffset0+2;
                        aoffset2 = aoffset2+2;
                        aoffset4 = aoffset4+2;
                        if( (mvidx+1)%updatetw==0 )
                        {
                            v = -(2*Math.PI*(mvidx+1)/(n*m));
                            twxm1 = Math.Sin(0.5*v);
                            twxm1 = -(2*twxm1*twxm1);
                            twy = Math.Sin(v);
                            twx = twxm1+1;
                        }
                        else
                        {
                            v = twxm1+tw0+twxm1*tw0-twy*tw1;
                            twy = twy+tw1+twxm1*tw1+twy*tw0;
                            twxm1 = v;
                            twx = v+1;
                        }
                    }
                }
                return;
            }
            if( n==4 )
            {
                v = -(2*Math.PI/(n*m));
                tw0 = -(2*math.sqr(Math.Sin(0.5*v)));
                tw1 = Math.Sin(v);
                for(opidx=0; opidx<=operandscnt-1; opidx++)
                {
                    aoffset0 = offs+opidx*operandsize*microvectorsize;
                    aoffset2 = aoffset0+microvectorsize;
                    aoffset4 = aoffset2+microvectorsize;
                    aoffset6 = aoffset4+microvectorsize;
                    twx = 1.0;
                    twxm1 = 0.0;
                    twy = 0.0;
                    for(mvidx=0; mvidx<=m-1; mvidx++)
                    {
                        a0x = a[aoffset0];
                        a0y = a[aoffset0+1];
                        a1x = a[aoffset2];
                        a1y = a[aoffset2+1];
                        a2x = a[aoffset4];
                        a2y = a[aoffset4+1];
                        a3x = a[aoffset6];
                        a3y = a[aoffset6+1];
                        t1x = a0x+a2x;
                        t1y = a0y+a2y;
                        t2x = a1x+a3x;
                        t2y = a1y+a3y;
                        m2x = a0x-a2x;
                        m2y = a0y-a2y;
                        m3x = a1y-a3y;
                        m3y = a3x-a1x;
                        tw2x = twx*twx-twy*twy;
                        tw2y = 2*twx*twy;
                        tw3x = twx*tw2x-twy*tw2y;
                        tw3y = twx*tw2y+twy*tw2x;
                        a1x = m2x+m3x;
                        a1y = m2y+m3y;
                        a2x = t1x-t2x;
                        a2y = t1y-t2y;
                        a3x = m2x-m3x;
                        a3y = m2y-m3y;
                        a[aoffset0] = t1x+t2x;
                        a[aoffset0+1] = t1y+t2y;
                        a[aoffset2] = a1x*twx-a1y*twy;
                        a[aoffset2+1] = a1y*twx+a1x*twy;
                        a[aoffset4] = a2x*tw2x-a2y*tw2y;
                        a[aoffset4+1] = a2y*tw2x+a2x*tw2y;
                        a[aoffset6] = a3x*tw3x-a3y*tw3y;
                        a[aoffset6+1] = a3y*tw3x+a3x*tw3y;
                        aoffset0 = aoffset0+2;
                        aoffset2 = aoffset2+2;
                        aoffset4 = aoffset4+2;
                        aoffset6 = aoffset6+2;
                        if( (mvidx+1)%updatetw==0 )
                        {
                            v = -(2*Math.PI*(mvidx+1)/(n*m));
                            twxm1 = Math.Sin(0.5*v);
                            twxm1 = -(2*twxm1*twxm1);
                            twy = Math.Sin(v);
                            twx = twxm1+1;
                        }
                        else
                        {
                            v = twxm1+tw0+twxm1*tw0-twy*tw1;
                            twy = twy+tw1+twxm1*tw1+twy*tw0;
                            twxm1 = v;
                            twx = v+1;
                        }
                    }
                }
                return;
            }
            if( n==5 )
            {
                v = -(2*Math.PI/(n*m));
                tw0 = -(2*math.sqr(Math.Sin(0.5*v)));
                tw1 = Math.Sin(v);
                v = 2*Math.PI/5;
                c1 = (Math.Cos(v)+Math.Cos(2*v))/2-1;
                c2 = (Math.Cos(v)-Math.Cos(2*v))/2;
                c3 = -Math.Sin(v);
                c4 = -(Math.Sin(v)+Math.Sin(2*v));
                c5 = Math.Sin(v)-Math.Sin(2*v);
                for(opidx=0; opidx<=operandscnt-1; opidx++)
                {
                    aoffset0 = offs+opidx*operandsize*microvectorsize;
                    aoffset2 = aoffset0+microvectorsize;
                    aoffset4 = aoffset2+microvectorsize;
                    aoffset6 = aoffset4+microvectorsize;
                    aoffset8 = aoffset6+microvectorsize;
                    twx = 1.0;
                    twxm1 = 0.0;
                    twy = 0.0;
                    for(mvidx=0; mvidx<=m-1; mvidx++)
                    {
                        a0x = a[aoffset0];
                        a0y = a[aoffset0+1];
                        a1x = a[aoffset2];
                        a1y = a[aoffset2+1];
                        a2x = a[aoffset4];
                        a2y = a[aoffset4+1];
                        a3x = a[aoffset6];
                        a3y = a[aoffset6+1];
                        a4x = a[aoffset8];
                        a4y = a[aoffset8+1];
                        t1x = a1x+a4x;
                        t1y = a1y+a4y;
                        t2x = a2x+a3x;
                        t2y = a2y+a3y;
                        t3x = a1x-a4x;
                        t3y = a1y-a4y;
                        t4x = a3x-a2x;
                        t4y = a3y-a2y;
                        t5x = t1x+t2x;
                        t5y = t1y+t2y;
                        q0x = a0x+t5x;
                        q0y = a0y+t5y;
                        m1x = c1*t5x;
                        m1y = c1*t5y;
                        m2x = c2*(t1x-t2x);
                        m2y = c2*(t1y-t2y);
                        m3x = -(c3*(t3y+t4y));
                        m3y = c3*(t3x+t4x);
                        m4x = -(c4*t4y);
                        m4y = c4*t4x;
                        m5x = -(c5*t3y);
                        m5y = c5*t3x;
                        s3x = m3x-m4x;
                        s3y = m3y-m4y;
                        s5x = m3x+m5x;
                        s5y = m3y+m5y;
                        s1x = q0x+m1x;
                        s1y = q0y+m1y;
                        s2x = s1x+m2x;
                        s2y = s1y+m2y;
                        s4x = s1x-m2x;
                        s4y = s1y-m2y;
                        tw2x = twx*twx-twy*twy;
                        tw2y = 2*twx*twy;
                        tw3x = twx*tw2x-twy*tw2y;
                        tw3y = twx*tw2y+twy*tw2x;
                        tw4x = tw2x*tw2x-tw2y*tw2y;
                        tw4y = tw2x*tw2y+tw2y*tw2x;
                        a1x = s2x+s3x;
                        a1y = s2y+s3y;
                        a2x = s4x+s5x;
                        a2y = s4y+s5y;
                        a3x = s4x-s5x;
                        a3y = s4y-s5y;
                        a4x = s2x-s3x;
                        a4y = s2y-s3y;
                        a[aoffset0] = q0x;
                        a[aoffset0+1] = q0y;
                        a[aoffset2] = a1x*twx-a1y*twy;
                        a[aoffset2+1] = a1x*twy+a1y*twx;
                        a[aoffset4] = a2x*tw2x-a2y*tw2y;
                        a[aoffset4+1] = a2x*tw2y+a2y*tw2x;
                        a[aoffset6] = a3x*tw3x-a3y*tw3y;
                        a[aoffset6+1] = a3x*tw3y+a3y*tw3x;
                        a[aoffset8] = a4x*tw4x-a4y*tw4y;
                        a[aoffset8+1] = a4x*tw4y+a4y*tw4x;
                        aoffset0 = aoffset0+2;
                        aoffset2 = aoffset2+2;
                        aoffset4 = aoffset4+2;
                        aoffset6 = aoffset6+2;
                        aoffset8 = aoffset8+2;
                        if( (mvidx+1)%updatetw==0 )
                        {
                            v = -(2*Math.PI*(mvidx+1)/(n*m));
                            twxm1 = Math.Sin(0.5*v);
                            twxm1 = -(2*twxm1*twxm1);
                            twy = Math.Sin(v);
                            twx = twxm1+1;
                        }
                        else
                        {
                            v = twxm1+tw0+twxm1*tw0-twy*tw1;
                            twy = twy+tw1+twxm1*tw1+twy*tw0;
                            twxm1 = v;
                            twx = v+1;
                        }
                    }
                }
                return;
            }
            if( n==6 )
            {
                c1 = Math.Cos(2*Math.PI/3)-1;
                c2 = Math.Sin(2*Math.PI/3);
                c3 = Math.Cos(-(Math.PI/3));
                c4 = Math.Sin(-(Math.PI/3));
                v = -(2*Math.PI/(n*m));
                tw0 = -(2*math.sqr(Math.Sin(0.5*v)));
                tw1 = Math.Sin(v);
                for(opidx=0; opidx<=operandscnt-1; opidx++)
                {
                    aoffset0 = offs+opidx*operandsize*microvectorsize;
                    aoffset2 = aoffset0+microvectorsize;
                    aoffset4 = aoffset2+microvectorsize;
                    aoffset6 = aoffset4+microvectorsize;
                    aoffset8 = aoffset6+microvectorsize;
                    aoffset10 = aoffset8+microvectorsize;
                    twx = 1.0;
                    twxm1 = 0.0;
                    twy = 0.0;
                    for(mvidx=0; mvidx<=m-1; mvidx++)
                    {
                        a0x = a[aoffset0+0];
                        a0y = a[aoffset0+1];
                        a1x = a[aoffset2+0];
                        a1y = a[aoffset2+1];
                        a2x = a[aoffset4+0];
                        a2y = a[aoffset4+1];
                        a3x = a[aoffset6+0];
                        a3y = a[aoffset6+1];
                        a4x = a[aoffset8+0];
                        a4y = a[aoffset8+1];
                        a5x = a[aoffset10+0];
                        a5y = a[aoffset10+1];
                        v0 = a0x;
                        v1 = a0y;
                        a0x = a0x+a3x;
                        a0y = a0y+a3y;
                        a3x = v0-a3x;
                        a3y = v1-a3y;
                        v0 = a1x;
                        v1 = a1y;
                        a1x = a1x+a4x;
                        a1y = a1y+a4y;
                        a4x = v0-a4x;
                        a4y = v1-a4y;
                        v0 = a2x;
                        v1 = a2y;
                        a2x = a2x+a5x;
                        a2y = a2y+a5y;
                        a5x = v0-a5x;
                        a5y = v1-a5y;
                        t4x = a4x*c3-a4y*c4;
                        t4y = a4x*c4+a4y*c3;
                        a4x = t4x;
                        a4y = t4y;
                        t5x = -(a5x*c3)-a5y*c4;
                        t5y = a5x*c4-a5y*c3;
                        a5x = t5x;
                        a5y = t5y;
                        t1x = a1x+a2x;
                        t1y = a1y+a2y;
                        a0x = a0x+t1x;
                        a0y = a0y+t1y;
                        m1x = c1*t1x;
                        m1y = c1*t1y;
                        m2x = c2*(a1y-a2y);
                        m2y = c2*(a2x-a1x);
                        s1x = a0x+m1x;
                        s1y = a0y+m1y;
                        a1x = s1x+m2x;
                        a1y = s1y+m2y;
                        a2x = s1x-m2x;
                        a2y = s1y-m2y;
                        t1x = a4x+a5x;
                        t1y = a4y+a5y;
                        a3x = a3x+t1x;
                        a3y = a3y+t1y;
                        m1x = c1*t1x;
                        m1y = c1*t1y;
                        m2x = c2*(a4y-a5y);
                        m2y = c2*(a5x-a4x);
                        s1x = a3x+m1x;
                        s1y = a3y+m1y;
                        a4x = s1x+m2x;
                        a4y = s1y+m2y;
                        a5x = s1x-m2x;
                        a5y = s1y-m2y;
                        tw2x = twx*twx-twy*twy;
                        tw2y = 2*twx*twy;
                        tw3x = twx*tw2x-twy*tw2y;
                        tw3y = twx*tw2y+twy*tw2x;
                        tw4x = tw2x*tw2x-tw2y*tw2y;
                        tw4y = 2*tw2x*tw2y;
                        tw5x = tw3x*tw2x-tw3y*tw2y;
                        tw5y = tw3x*tw2y+tw3y*tw2x;
                        a[aoffset0+0] = a0x;
                        a[aoffset0+1] = a0y;
                        a[aoffset2+0] = a3x*twx-a3y*twy;
                        a[aoffset2+1] = a3y*twx+a3x*twy;
                        a[aoffset4+0] = a1x*tw2x-a1y*tw2y;
                        a[aoffset4+1] = a1y*tw2x+a1x*tw2y;
                        a[aoffset6+0] = a4x*tw3x-a4y*tw3y;
                        a[aoffset6+1] = a4y*tw3x+a4x*tw3y;
                        a[aoffset8+0] = a2x*tw4x-a2y*tw4y;
                        a[aoffset8+1] = a2y*tw4x+a2x*tw4y;
                        a[aoffset10+0] = a5x*tw5x-a5y*tw5y;
                        a[aoffset10+1] = a5y*tw5x+a5x*tw5y;
                        aoffset0 = aoffset0+2;
                        aoffset2 = aoffset2+2;
                        aoffset4 = aoffset4+2;
                        aoffset6 = aoffset6+2;
                        aoffset8 = aoffset8+2;
                        aoffset10 = aoffset10+2;
                        if( (mvidx+1)%updatetw==0 )
                        {
                            v = -(2*Math.PI*(mvidx+1)/(n*m));
                            twxm1 = Math.Sin(0.5*v);
                            twxm1 = -(2*twxm1*twxm1);
                            twy = Math.Sin(v);
                            twx = twxm1+1;
                        }
                        else
                        {
                            v = twxm1+tw0+twxm1*tw0-twy*tw1;
                            twy = twy+tw1+twxm1*tw1+twy*tw0;
                            twxm1 = v;
                            twx = v+1;
                        }
                    }
                }
                return;
            }
        }


        /*************************************************************************
        This subroutine precomputes data for complex Bluestein's  FFT  and  writes
        them to array PrecR[] at specified offset. It  is  responsibility  of  the
        caller to make sure that PrecR[] is large enough.

        INPUT PARAMETERS:
            N           -   original size of the transform
            M           -   size of the "padded" Bluestein's transform
            PrecR       -   preallocated array
            Offs        -   offset
            
        OUTPUT PARAMETERS:
            PrecR       -   data at Offs:Offs+4*M-1 are modified:
                            * PrecR[Offs:Offs+2*M-1] stores Z[k]=exp(i*pi*k^2/N)
                            * PrecR[Offs+2*M:Offs+4*M-1] stores FFT of the Z
                            Other parts of PrecR are unchanged.
                            
        NOTE: this function performs internal M-point FFT. It allocates temporary
              plan which is destroyed after leaving this function.

          -- ALGLIB --
             Copyright 08.05.2013 by Bochkanov Sergey
        *************************************************************************/
        private static void ftprecomputebluesteinsfft(int n,
            int m,
            double[] precr,
            int offs,
            alglib.xparams _params)
        {
            int i = 0;
            double bx = 0;
            double by = 0;
            fasttransformplan plan = new fasttransformplan();

            
            //
            // Fill first half of PrecR with b[k] = exp(i*pi*k^2/N)
            //
            for(i=0; i<=2*m-1; i++)
            {
                precr[offs+i] = 0;
            }
            for(i=0; i<=n-1; i++)
            {
                bx = Math.Cos(Math.PI/n*i*i);
                by = Math.Sin(Math.PI/n*i*i);
                precr[offs+2*i+0] = bx;
                precr[offs+2*i+1] = by;
                precr[offs+2*((m-i)%m)+0] = bx;
                precr[offs+2*((m-i)%m)+1] = by;
            }
            
            //
            // Precomputed FFT
            //
            ftcomplexfftplan(m, 1, plan, _params);
            for(i=0; i<=2*m-1; i++)
            {
                precr[offs+2*m+i] = precr[offs+i];
            }
            ftapplysubplan(plan, 0, precr, offs+2*m, 0, plan.buffer, 1, _params);
        }


        /*************************************************************************
        This subroutine applies complex Bluestein's FFT to input/output array A.

        INPUT PARAMETERS:
            Plan        -   transformation plan
            A           -   array, must be large enough for plan to work
            ABase       -   base offset in array A, this value points to start of
                            subarray whose length is equal to length of the plan
            AOffset     -   offset with respect to ABase, 0<=AOffset<PlanLength.
                            This is an offset within large PlanLength-subarray of
                            the chunk to process.
            OperandsCnt -   number of repeated operands (length N each)
            N           -   original data length (measured in complex numbers)
            M           -   padded data length (measured in complex numbers)
            PrecOffs    -   offset of the precomputed data for the plan
            SubPlan     -   position of the length-M FFT subplan which is used by
                            transformation
            BufA        -   temporary buffer, at least 2*M elements
            BufB        -   temporary buffer, at least 2*M elements
            BufC        -   temporary buffer, at least 2*M elements
            BufD        -   temporary buffer, at least 2*M elements
            
        OUTPUT PARAMETERS:
            A           -   transformed array

          -- ALGLIB --
             Copyright 05.04.2013 by Bochkanov Sergey
        *************************************************************************/
        private static void ftbluesteinsfft(fasttransformplan plan,
            double[] a,
            int abase,
            int aoffset,
            int operandscnt,
            int n,
            int m,
            int precoffs,
            int subplan,
            double[] bufa,
            double[] bufb,
            double[] bufc,
            double[] bufd,
            alglib.xparams _params)
        {
            int op = 0;
            int i = 0;
            double x = 0;
            double y = 0;
            double bx = 0;
            double by = 0;
            double ax = 0;
            double ay = 0;
            double rx = 0;
            double ry = 0;
            int p0 = 0;
            int p1 = 0;
            int p2 = 0;

            for(op=0; op<=operandscnt-1; op++)
            {
                
                //
                // Multiply A by conj(Z), store to buffer.
                // Pad A by zeros.
                //
                // NOTE: Z[k]=exp(i*pi*k^2/N)
                //
                p0 = abase+aoffset+op*2*n;
                p1 = precoffs;
                for(i=0; i<=n-1; i++)
                {
                    x = a[p0+0];
                    y = a[p0+1];
                    bx = plan.precr[p1+0];
                    by = -plan.precr[p1+1];
                    bufa[2*i+0] = x*bx-y*by;
                    bufa[2*i+1] = x*by+y*bx;
                    p0 = p0+2;
                    p1 = p1+2;
                }
                for(i=2*n; i<=2*m-1; i++)
                {
                    bufa[i] = 0;
                }
                
                //
                // Perform convolution of A and Z (using precomputed
                // FFT of Z stored in Plan structure).
                //
                ftapplysubplan(plan, subplan, bufa, 0, 0, bufc, 1, _params);
                p0 = 0;
                p1 = precoffs+2*m;
                for(i=0; i<=m-1; i++)
                {
                    ax = bufa[p0+0];
                    ay = bufa[p0+1];
                    bx = plan.precr[p1+0];
                    by = plan.precr[p1+1];
                    bufa[p0+0] = ax*bx-ay*by;
                    bufa[p0+1] = -(ax*by+ay*bx);
                    p0 = p0+2;
                    p1 = p1+2;
                }
                ftapplysubplan(plan, subplan, bufa, 0, 0, bufc, 1, _params);
                
                //
                // Post processing:
                //     A:=conj(Z)*conj(A)/M
                // Here conj(A)/M corresponds to last stage of inverse DFT,
                // and conj(Z) comes from Bluestein's FFT algorithm.
                //
                p0 = precoffs;
                p1 = 0;
                p2 = abase+aoffset+op*2*n;
                for(i=0; i<=n-1; i++)
                {
                    bx = plan.precr[p0+0];
                    by = plan.precr[p0+1];
                    rx = bufa[p1+0]/m;
                    ry = -(bufa[p1+1]/m);
                    a[p2+0] = rx*bx-ry*-by;
                    a[p2+1] = rx*-by+ry*bx;
                    p0 = p0+2;
                    p1 = p1+2;
                    p2 = p2+2;
                }
            }
        }


        /*************************************************************************
        This subroutine precomputes data for complex Rader's FFT and  writes  them
        to array PrecR[] at specified offset. It  is  responsibility of the caller
        to make sure that PrecR[] is large enough.

        INPUT PARAMETERS:
            N           -   original size of the transform (before reduction to N-1)
            RQ          -   primitive root modulo N
            RIQ         -   inverse of primitive root modulo N
            PrecR       -   preallocated array
            Offs        -   offset
            
        OUTPUT PARAMETERS:
            PrecR       -   data at Offs:Offs+2*(N-1)-1 store FFT of Rader's factors,
                            other parts of PrecR are unchanged.
                            
        NOTE: this function performs internal (N-1)-point FFT. It allocates temporary
              plan which is destroyed after leaving this function.

          -- ALGLIB --
             Copyright 08.05.2013 by Bochkanov Sergey
        *************************************************************************/
        private static void ftprecomputeradersfft(int n,
            int rq,
            int riq,
            double[] precr,
            int offs,
            alglib.xparams _params)
        {
            int q = 0;
            fasttransformplan plan = new fasttransformplan();
            int kiq = 0;
            double v = 0;

            
            //
            // Fill PrecR with Rader factors, perform FFT
            //
            kiq = 1;
            for(q=0; q<=n-2; q++)
            {
                v = -(2*Math.PI*kiq/n);
                precr[offs+2*q+0] = Math.Cos(v);
                precr[offs+2*q+1] = Math.Sin(v);
                kiq = kiq*riq%n;
            }
            ftcomplexfftplan(n-1, 1, plan, _params);
            ftapplysubplan(plan, 0, precr, offs, 0, plan.buffer, 1, _params);
        }


        /*************************************************************************
        This subroutine applies complex Rader's FFT to input/output array A.

        INPUT PARAMETERS:
            A           -   array, must be large enough for plan to work
            ABase       -   base offset in array A, this value points to start of
                            subarray whose length is equal to length of the plan
            AOffset     -   offset with respect to ABase, 0<=AOffset<PlanLength.
                            This is an offset within large PlanLength-subarray of
                            the chunk to process.
            OperandsCnt -   number of repeated operands (length N each)
            N           -   original data length (measured in complex numbers)
            SubPlan     -   position of the (N-1)-point FFT subplan which is used
                            by transformation
            RQ          -   primitive root modulo N
            RIQ         -   inverse of primitive root modulo N
            PrecOffs    -   offset of the precomputed data for the plan
            Buf         -   temporary array
            
        OUTPUT PARAMETERS:
            A           -   transformed array

          -- ALGLIB --
             Copyright 05.04.2013 by Bochkanov Sergey
        *************************************************************************/
        private static void ftradersfft(fasttransformplan plan,
            double[] a,
            int abase,
            int aoffset,
            int operandscnt,
            int n,
            int subplan,
            int rq,
            int riq,
            int precoffs,
            double[] buf,
            alglib.xparams _params)
        {
            int opidx = 0;
            int i = 0;
            int q = 0;
            int kq = 0;
            int kiq = 0;
            double x0 = 0;
            double y0 = 0;
            int p0 = 0;
            int p1 = 0;
            double ax = 0;
            double ay = 0;
            double bx = 0;
            double by = 0;
            double rx = 0;
            double ry = 0;

            alglib.ap.assert(operandscnt>=1, "FTApplyComplexRefFFT: OperandsCnt<1");
            
            //
            // Process operands
            //
            for(opidx=0; opidx<=operandscnt-1; opidx++)
            {
                
                //
                // fill QA
                //
                kq = 1;
                p0 = abase+aoffset+opidx*n*2;
                p1 = aoffset+opidx*n*2;
                rx = a[p0+0];
                ry = a[p0+1];
                x0 = rx;
                y0 = ry;
                for(q=0; q<=n-2; q++)
                {
                    ax = a[p0+2*kq+0];
                    ay = a[p0+2*kq+1];
                    buf[p1+0] = ax;
                    buf[p1+1] = ay;
                    rx = rx+ax;
                    ry = ry+ay;
                    kq = kq*rq%n;
                    p1 = p1+2;
                }
                p0 = abase+aoffset+opidx*n*2;
                p1 = aoffset+opidx*n*2;
                for(q=0; q<=n-2; q++)
                {
                    a[p0] = buf[p1];
                    a[p0+1] = buf[p1+1];
                    p0 = p0+2;
                    p1 = p1+2;
                }
                
                //
                // Convolution
                //
                ftapplysubplan(plan, subplan, a, abase, aoffset+opidx*n*2, buf, 1, _params);
                p0 = abase+aoffset+opidx*n*2;
                p1 = precoffs;
                for(i=0; i<=n-2; i++)
                {
                    ax = a[p0+0];
                    ay = a[p0+1];
                    bx = plan.precr[p1+0];
                    by = plan.precr[p1+1];
                    a[p0+0] = ax*bx-ay*by;
                    a[p0+1] = -(ax*by+ay*bx);
                    p0 = p0+2;
                    p1 = p1+2;
                }
                ftapplysubplan(plan, subplan, a, abase, aoffset+opidx*n*2, buf, 1, _params);
                p0 = abase+aoffset+opidx*n*2;
                for(i=0; i<=n-2; i++)
                {
                    a[p0+0] = a[p0+0]/(n-1);
                    a[p0+1] = -(a[p0+1]/(n-1));
                    p0 = p0+2;
                }
                
                //
                // Result
                //
                buf[aoffset+opidx*n*2+0] = rx;
                buf[aoffset+opidx*n*2+1] = ry;
                kiq = 1;
                p0 = aoffset+opidx*n*2;
                p1 = abase+aoffset+opidx*n*2;
                for(q=0; q<=n-2; q++)
                {
                    buf[p0+2*kiq+0] = x0+a[p1+0];
                    buf[p0+2*kiq+1] = y0+a[p1+1];
                    kiq = kiq*riq%n;
                    p1 = p1+2;
                }
                p0 = abase+aoffset+opidx*n*2;
                p1 = aoffset+opidx*n*2;
                for(q=0; q<=n-1; q++)
                {
                    a[p0] = buf[p1];
                    a[p0+1] = buf[p1+1];
                    p0 = p0+2;
                    p1 = p1+2;
                }
            }
        }


        /*************************************************************************
        Factorizes task size N into product of two smaller sizes N1 and N2

        INPUT PARAMETERS:
            N       -   task size, N>0
            IsRoot  -   whether taks is root task (first one in a sequence)
            
        OUTPUT PARAMETERS:
            N1, N2  -   such numbers that:
                        * for prime N:                  N1=N2=0
                        * for composite N<=MaxRadix:    N1=N2=0
                        * for composite N>MaxRadix:     1<=N1<=N2, N1*N2=N

          -- ALGLIB --
             Copyright 08.04.2013 by Bochkanov Sergey
        *************************************************************************/
        private static void ftfactorize(int n,
            bool isroot,
            ref int n1,
            ref int n2,
            alglib.xparams _params)
        {
            int j = 0;
            int k = 0;

            n1 = 0;
            n2 = 0;

            alglib.ap.assert(n>0, "FTFactorize: N<=0");
            n1 = 0;
            n2 = 0;
            
            //
            // Small N
            //
            if( n<=maxradix )
            {
                return;
            }
            
            //
            // Large N, recursive split
            //
            if( n>recursivethreshold )
            {
                k = (int)Math.Ceiling(Math.Sqrt(n))+1;
                alglib.ap.assert(k*k>=n, "FTFactorize: internal error during recursive factorization");
                for(j=k; j>=2; j--)
                {
                    if( n%j==0 )
                    {
                        n1 = Math.Min(n/j, j);
                        n2 = Math.Max(n/j, j);
                        return;
                    }
                }
            }
            
            //
            // N>MaxRadix, try to find good codelet
            //
            for(j=maxradix; j>=2; j--)
            {
                if( n%j==0 )
                {
                    n1 = j;
                    n2 = n/j;
                    break;
                }
            }
            
            //
            // In case no good codelet was found,
            // try to factorize N into product of ANY primes.
            //
            if( n1*n2!=n )
            {
                for(j=2; j<=n-1; j++)
                {
                    if( n%j==0 )
                    {
                        n1 = j;
                        n2 = n/j;
                        break;
                    }
                    if( j*j>n )
                    {
                        break;
                    }
                }
            }
            
            //
            // normalize
            //
            if( n1>n2 )
            {
                j = n1;
                n1 = n2;
                n2 = j;
            }
        }


        /*************************************************************************
        Returns optimistic estimate of the FFT cost, in UNITs (1 UNIT = 100 KFLOPs)

        INPUT PARAMETERS:
            N       -   task size, N>0
            
        RESULU:
            cost in UNITs, rounded down to nearest integer

        NOTE: If FFT cost is less than 1 UNIT, it will return 0 as result.

          -- ALGLIB --
             Copyright 08.04.2013 by Bochkanov Sergey
        *************************************************************************/
        private static int ftoptimisticestimate(int n,
            alglib.xparams _params)
        {
            int result = 0;

            alglib.ap.assert(n>0, "FTOptimisticEstimate: N<=0");
            result = (int)Math.Floor(1.0E-5*5*n*Math.Log(n)/Math.Log(2));
            return result;
        }


        /*************************************************************************
        Twiddle factors calculation

          -- ALGLIB --
             Copyright 01.05.2009 by Bochkanov Sergey
        *************************************************************************/
        private static void ffttwcalc(double[] a,
            int aoffset,
            int n1,
            int n2,
            alglib.xparams _params)
        {
            int i = 0;
            int j2 = 0;
            int n = 0;
            int halfn1 = 0;
            int offs = 0;
            double x = 0;
            double y = 0;
            double twxm1 = 0;
            double twy = 0;
            double twbasexm1 = 0;
            double twbasey = 0;
            double twrowxm1 = 0;
            double twrowy = 0;
            double tmpx = 0;
            double tmpy = 0;
            double v = 0;
            int updatetw2 = 0;

            
            //
            // Multiplication by twiddle factors for complex Cooley-Tukey FFT
            // with N factorized as N1*N2.
            //
            // Naive solution to this problem is given below:
            //
            //     > for K:=1 to N2-1 do
            //     >     for J:=1 to N1-1 do
            //     >     begin
            //     >         Idx:=K*N1+J;
            //     >         X:=A[AOffset+2*Idx+0];
            //     >         Y:=A[AOffset+2*Idx+1];
            //     >         TwX:=Cos(-2*Pi()*K*J/(N1*N2));
            //     >         TwY:=Sin(-2*Pi()*K*J/(N1*N2));
            //     >         A[AOffset+2*Idx+0]:=X*TwX-Y*TwY;
            //     >         A[AOffset+2*Idx+1]:=X*TwY+Y*TwX;
            //     >     end;
            //
            // However, there are exist more efficient solutions.
            //
            // Each pass of the inner cycle corresponds to multiplication of one
            // entry of A by W[k,j]=exp(-I*2*pi*k*j/N). This factor can be rewritten
            // as exp(-I*2*pi*k/N)^j. So we can replace costly exponentiation by
            // repeated multiplication: W[k,j+1]=W[k,j]*exp(-I*2*pi*k/N), with
            // second factor being computed once in the beginning of the iteration.
            //
            // Also, exp(-I*2*pi*k/N) can be represented as exp(-I*2*pi/N)^k, i.e.
            // we have W[K+1,1]=W[K,1]*W[1,1].
            //
            // In our loop we use following variables:
            // * [TwBaseXM1,TwBaseY] =   [cos(2*pi/N)-1,     sin(2*pi/N)]
            // * [TwRowXM1, TwRowY]  =   [cos(2*pi*I/N)-1,   sin(2*pi*I/N)]
            // * [TwXM1,    TwY]     =   [cos(2*pi*I*J/N)-1, sin(2*pi*I*J/N)]
            //
            // Meaning of the variables:
            // * [TwXM1,TwY] is current twiddle factor W[I,J]
            // * [TwRowXM1, TwRowY] is W[I,1]
            // * [TwBaseXM1,TwBaseY] is W[1,1]
            //
            // During inner loop we multiply current twiddle factor by W[I,1],
            // during outer loop we update W[I,1].
            //
            //
            alglib.ap.assert(updatetw>=2, "FFTTwCalc: internal error - UpdateTw<2");
            updatetw2 = updatetw/2;
            halfn1 = n1/2;
            n = n1*n2;
            v = -(2*Math.PI/n);
            twbasexm1 = -(2*math.sqr(Math.Sin(0.5*v)));
            twbasey = Math.Sin(v);
            twrowxm1 = 0;
            twrowy = 0;
            offs = aoffset;
            for(i=0; i<=n2-1; i++)
            {
                
                //
                // Initialize twiddle factor for current row
                //
                twxm1 = 0;
                twy = 0;
                
                //
                // N1-point block is separated into 2-point chunks and residual 1-point chunk
                // (in case N1 is odd). Unrolled loop is several times faster.
                //
                for(j2=0; j2<=halfn1-1; j2++)
                {
                    
                    //
                    // Processing:
                    // * process first element in a chunk.
                    // * update twiddle factor (unconditional update)
                    // * process second element
                    // * conditional update of the twiddle factor
                    //
                    x = a[offs+0];
                    y = a[offs+1];
                    tmpx = x*(1+twxm1)-y*twy;
                    tmpy = x*twy+y*(1+twxm1);
                    a[offs+0] = tmpx;
                    a[offs+1] = tmpy;
                    tmpx = (1+twxm1)*twrowxm1-twy*twrowy;
                    twy = twy+(1+twxm1)*twrowy+twy*twrowxm1;
                    twxm1 = twxm1+tmpx;
                    x = a[offs+2];
                    y = a[offs+3];
                    tmpx = x*(1+twxm1)-y*twy;
                    tmpy = x*twy+y*(1+twxm1);
                    a[offs+2] = tmpx;
                    a[offs+3] = tmpy;
                    offs = offs+4;
                    if( (j2+1)%updatetw2==0 && j2<halfn1-1 )
                    {
                        
                        //
                        // Recalculate twiddle factor
                        //
                        v = -(2*Math.PI*i*2*(j2+1)/n);
                        twxm1 = Math.Sin(0.5*v);
                        twxm1 = -(2*twxm1*twxm1);
                        twy = Math.Sin(v);
                    }
                    else
                    {
                        
                        //
                        // Update twiddle factor
                        //
                        tmpx = (1+twxm1)*twrowxm1-twy*twrowy;
                        twy = twy+(1+twxm1)*twrowy+twy*twrowxm1;
                        twxm1 = twxm1+tmpx;
                    }
                }
                if( n1%2==1 )
                {
                    
                    //
                    // Handle residual chunk
                    //
                    x = a[offs+0];
                    y = a[offs+1];
                    tmpx = x*(1+twxm1)-y*twy;
                    tmpy = x*twy+y*(1+twxm1);
                    a[offs+0] = tmpx;
                    a[offs+1] = tmpy;
                    offs = offs+2;
                }
                
                //
                // update TwRow: TwRow(new) = TwRow(old)*TwBase
                //
                if( i<n2-1 )
                {
                    if( (i+1)%updatetw==0 )
                    {
                        v = -(2*Math.PI*(i+1)/n);
                        twrowxm1 = Math.Sin(0.5*v);
                        twrowxm1 = -(2*twrowxm1*twrowxm1);
                        twrowy = Math.Sin(v);
                    }
                    else
                    {
                        tmpx = twbasexm1+twrowxm1*twbasexm1-twrowy*twbasey;
                        tmpy = twbasey+twrowxm1*twbasey+twrowy*twbasexm1;
                        twrowxm1 = twrowxm1+tmpx;
                        twrowy = twrowy+tmpy;
                    }
                }
            }
        }


        /*************************************************************************
        Linear transpose: transpose complex matrix stored in 1-dimensional array

          -- ALGLIB --
             Copyright 01.05.2009 by Bochkanov Sergey
        *************************************************************************/
        private static void internalcomplexlintranspose(double[] a,
            int m,
            int n,
            int astart,
            double[] buf,
            alglib.xparams _params)
        {
            int i_ = 0;
            int i1_ = 0;

            ffticltrec(a, astart, n, buf, 0, m, m, n, _params);
            i1_ = (0) - (astart);
            for(i_=astart; i_<=astart+2*m*n-1;i_++)
            {
                a[i_] = buf[i_+i1_];
            }
        }


        /*************************************************************************
        Recurrent subroutine for a InternalComplexLinTranspose

        Write A^T to B, where:
        * A is m*n complex matrix stored in array A as pairs of real/image values,
          beginning from AStart position, with AStride stride
        * B is n*m complex matrix stored in array B as pairs of real/image values,
          beginning from BStart position, with BStride stride
        stride is measured in complex numbers, i.e. in real/image pairs.

          -- ALGLIB --
             Copyright 01.05.2009 by Bochkanov Sergey
        *************************************************************************/
        private static void ffticltrec(double[] a,
            int astart,
            int astride,
            double[] b,
            int bstart,
            int bstride,
            int m,
            int n,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;
            int idx1 = 0;
            int idx2 = 0;
            int m2 = 0;
            int m1 = 0;
            int n1 = 0;

            if( m==0 || n==0 )
            {
                return;
            }
            if( Math.Max(m, n)<=8 )
            {
                m2 = 2*bstride;
                for(i=0; i<=m-1; i++)
                {
                    idx1 = bstart+2*i;
                    idx2 = astart+2*i*astride;
                    for(j=0; j<=n-1; j++)
                    {
                        b[idx1+0] = a[idx2+0];
                        b[idx1+1] = a[idx2+1];
                        idx1 = idx1+m2;
                        idx2 = idx2+2;
                    }
                }
                return;
            }
            if( n>m )
            {
                
                //
                // New partition:
                //
                // "A^T -> B" becomes "(A1 A2)^T -> ( B1 )
                //                                  ( B2 )
                //
                n1 = n/2;
                if( n-n1>=8 && n1%8!=0 )
                {
                    n1 = n1+(8-n1%8);
                }
                alglib.ap.assert(n-n1>0);
                ffticltrec(a, astart, astride, b, bstart, bstride, m, n1, _params);
                ffticltrec(a, astart+2*n1, astride, b, bstart+2*n1*bstride, bstride, m, n-n1, _params);
            }
            else
            {
                
                //
                // New partition:
                //
                // "A^T -> B" becomes "( A1 )^T -> ( B1 B2 )
                //                     ( A2 )
                //
                m1 = m/2;
                if( m-m1>=8 && m1%8!=0 )
                {
                    m1 = m1+(8-m1%8);
                }
                alglib.ap.assert(m-m1>0);
                ffticltrec(a, astart, astride, b, bstart, bstride, m1, n, _params);
                ffticltrec(a, astart+2*m1*astride, astride, b, bstart+2*m1, bstride, m-m1, n, _params);
            }
        }


        /*************************************************************************
        Recurrent subroutine for a InternalRealLinTranspose


          -- ALGLIB --
             Copyright 01.05.2009 by Bochkanov Sergey
        *************************************************************************/
        private static void fftirltrec(ref double[] a,
            int astart,
            int astride,
            ref double[] b,
            int bstart,
            int bstride,
            int m,
            int n,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;
            int idx1 = 0;
            int idx2 = 0;
            int m1 = 0;
            int n1 = 0;

            if( m==0 || n==0 )
            {
                return;
            }
            if( Math.Max(m, n)<=8 )
            {
                for(i=0; i<=m-1; i++)
                {
                    idx1 = bstart+i;
                    idx2 = astart+i*astride;
                    for(j=0; j<=n-1; j++)
                    {
                        b[idx1] = a[idx2];
                        idx1 = idx1+bstride;
                        idx2 = idx2+1;
                    }
                }
                return;
            }
            if( n>m )
            {
                
                //
                // New partition:
                //
                // "A^T -> B" becomes "(A1 A2)^T -> ( B1 )
                //                                  ( B2 )
                //
                n1 = n/2;
                if( n-n1>=8 && n1%8!=0 )
                {
                    n1 = n1+(8-n1%8);
                }
                alglib.ap.assert(n-n1>0);
                fftirltrec(ref a, astart, astride, ref b, bstart, bstride, m, n1, _params);
                fftirltrec(ref a, astart+n1, astride, ref b, bstart+n1*bstride, bstride, m, n-n1, _params);
            }
            else
            {
                
                //
                // New partition:
                //
                // "A^T -> B" becomes "( A1 )^T -> ( B1 B2 )
                //                     ( A2 )
                //
                m1 = m/2;
                if( m-m1>=8 && m1%8!=0 )
                {
                    m1 = m1+(8-m1%8);
                }
                alglib.ap.assert(m-m1>0);
                fftirltrec(ref a, astart, astride, ref b, bstart, bstride, m1, n, _params);
                fftirltrec(ref a, astart+m1*astride, astride, ref b, bstart+m1, bstride, m-m1, n, _params);
            }
        }


        /*************************************************************************
        recurrent subroutine for FFTFindSmoothRec

          -- ALGLIB --
             Copyright 01.05.2009 by Bochkanov Sergey
        *************************************************************************/
        private static void ftbasefindsmoothrec(int n,
            int seed,
            int leastfactor,
            ref int best,
            alglib.xparams _params)
        {
            alglib.ap.assert(ftbasemaxsmoothfactor<=5, "FTBaseFindSmoothRec: internal error!");
            if( seed>=n )
            {
                best = Math.Min(best, seed);
                return;
            }
            if( leastfactor<=2 )
            {
                ftbasefindsmoothrec(n, seed*2, 2, ref best, _params);
            }
            if( leastfactor<=3 )
            {
                ftbasefindsmoothrec(n, seed*3, 3, ref best, _params);
            }
            if( leastfactor<=5 )
            {
                ftbasefindsmoothrec(n, seed*5, 5, ref best, _params);
            }
        }


    }
    public class nearunityunit
    {
        public static double nulog1p(double x,
            alglib.xparams _params)
        {
            double result = 0;
            double z = 0;
            double lp = 0;
            double lq = 0;

            z = 1.0+x;
            if( (double)(z)<(double)(0.70710678118654752440) || (double)(z)>(double)(1.41421356237309504880) )
            {
                result = Math.Log(z);
                return result;
            }
            z = x*x;
            lp = 4.5270000862445199635215E-5;
            lp = lp*x+4.9854102823193375972212E-1;
            lp = lp*x+6.5787325942061044846969E0;
            lp = lp*x+2.9911919328553073277375E1;
            lp = lp*x+6.0949667980987787057556E1;
            lp = lp*x+5.7112963590585538103336E1;
            lp = lp*x+2.0039553499201281259648E1;
            lq = 1.0000000000000000000000E0;
            lq = lq*x+1.5062909083469192043167E1;
            lq = lq*x+8.3047565967967209469434E1;
            lq = lq*x+2.2176239823732856465394E2;
            lq = lq*x+3.0909872225312059774938E2;
            lq = lq*x+2.1642788614495947685003E2;
            lq = lq*x+6.0118660497603843919306E1;
            z = -(0.5*z)+x*(z*lp/lq);
            result = x+z;
            return result;
        }


        public static double nuexpm1(double x,
            alglib.xparams _params)
        {
            double result = 0;
            double r = 0;
            double xx = 0;
            double ep = 0;
            double eq = 0;

            if( (double)(x)<(double)(-0.5) || (double)(x)>(double)(0.5) )
            {
                result = Math.Exp(x)-1.0;
                return result;
            }
            xx = x*x;
            ep = 1.2617719307481059087798E-4;
            ep = ep*xx+3.0299440770744196129956E-2;
            ep = ep*xx+9.9999999999999999991025E-1;
            eq = 3.0019850513866445504159E-6;
            eq = eq*xx+2.5244834034968410419224E-3;
            eq = eq*xx+2.2726554820815502876593E-1;
            eq = eq*xx+2.0000000000000000000897E0;
            r = x*ep;
            r = r/(eq-r);
            result = r+r;
            return result;
        }


        public static double nucosm1(double x,
            alglib.xparams _params)
        {
            double result = 0;
            double xx = 0;
            double c = 0;

            if( (double)(x)<(double)(-(0.25*Math.PI)) || (double)(x)>(double)(0.25*Math.PI) )
            {
                result = Math.Cos(x)-1;
                return result;
            }
            xx = x*x;
            c = 4.7377507964246204691685E-14;
            c = c*xx-1.1470284843425359765671E-11;
            c = c*xx+2.0876754287081521758361E-9;
            c = c*xx-2.7557319214999787979814E-7;
            c = c*xx+2.4801587301570552304991E-5;
            c = c*xx-1.3888888888888872993737E-3;
            c = c*xx+4.1666666666666666609054E-2;
            result = -(0.5*xx)+xx*xx*c;
            return result;
        }


    }
    public class alglibbasics
    {


    }
}

