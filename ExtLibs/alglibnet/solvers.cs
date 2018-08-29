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
    public class densesolverreport : alglibobject
    {
        //
        // Public declarations
        //
        public double r1 { get { return _innerobj.r1; } set { _innerobj.r1 = value; } }
        public double rinf { get { return _innerobj.rinf; } set { _innerobj.rinf = value; } }
    
        public densesolverreport()
        {
            _innerobj = new directdensesolvers.densesolverreport();
        }
        
        public override alglib.alglibobject make_copy()
        {
            return new densesolverreport((directdensesolvers.densesolverreport)_innerobj.make_copy());
        }
    
        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private directdensesolvers.densesolverreport _innerobj;
        public directdensesolvers.densesolverreport innerobj { get { return _innerobj; } }
        public densesolverreport(directdensesolvers.densesolverreport obj)
        {
            _innerobj = obj;
        }
    }


    /*************************************************************************

    *************************************************************************/
    public class densesolverlsreport : alglibobject
    {
        //
        // Public declarations
        //
        public double r2 { get { return _innerobj.r2; } set { _innerobj.r2 = value; } }
        public double[,] cx { get { return _innerobj.cx; } set { _innerobj.cx = value; } }
        public int n { get { return _innerobj.n; } set { _innerobj.n = value; } }
        public int k { get { return _innerobj.k; } set { _innerobj.k = value; } }
    
        public densesolverlsreport()
        {
            _innerobj = new directdensesolvers.densesolverlsreport();
        }
        
        public override alglib.alglibobject make_copy()
        {
            return new densesolverlsreport((directdensesolvers.densesolverlsreport)_innerobj.make_copy());
        }
    
        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private directdensesolvers.densesolverlsreport _innerobj;
        public directdensesolvers.densesolverlsreport innerobj { get { return _innerobj; } }
        public densesolverlsreport(directdensesolvers.densesolverlsreport obj)
        {
            _innerobj = obj;
        }
    }
    
    /*************************************************************************
    Dense solver for A*x=b with N*N real matrix A and N*1 real vectorx  x  and
    b. This is "slow-but-feature rich" version of the  linear  solver.  Faster
    version is RMatrixSolveFast() function.

    Algorithm features:
    * automatic detection of degenerate cases
    * condition number estimation
    * iterative refinement
    * O(N^3) complexity

    IMPORTANT: ! this function is NOT the most efficient linear solver provided
               ! by ALGLIB. It estimates condition  number  of  linear  system
               ! and  performs  iterative   refinement,   which   results   in
               ! significant performance penalty  when  compared  with  "fast"
               ! version  which  just  performs  LU  decomposition  and  calls
               ! triangular solver.
               !
               ! This  performance  penalty  is  especially  visible  in   the
               ! multithreaded mode, because both condition number  estimation
               ! and   iterative    refinement   are   inherently   sequential
               ! calculations. It is also very significant on small matrices.
               !
               ! Thus, if you need high performance and if you are pretty sure
               ! that your system is well conditioned, we  strongly  recommend
               ! you to use faster solver, RMatrixSolveFast() function.

      ! COMMERCIAL EDITION OF ALGLIB:
      !
      ! Commercial Edition of ALGLIB includes following important improvements
      ! of this function:
      ! * high-performance native backend with same C# interface (C# version)
      ! * multithreading support (C++ and C# versions)
      ! * hardware vendor (Intel) implementations of linear algebra primitives
      !   (C++ and C# versions, x86/x64 platform)
      !
      ! We recommend you to read 'Working with commercial version' section  of
      ! ALGLIB Reference Manual in order to find out how to  use  performance-
      ! related features provided by commercial edition of ALGLIB.

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        N       -   size of A
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    matrix is very badly conditioned or exactly singular.
                    * -1    N<=0 was passed
                    *  1    task is solved (but matrix A may be ill-conditioned,
                            check R1/RInf parameters for condition numbers).
        Rep     -   additional report, following fields are set:
                    * rep.r1    condition number in 1-norm
                    * rep.rinf  condition number in inf-norm
        X       -   array[N], it contains:
                    * info>0    =>  solution
                    * info=-3   =>  filled by zeros

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void rmatrixsolve(double[,] a, int n, double[] b, out int info, out densesolverreport rep, out double[] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0];
        directdensesolvers.rmatrixsolve(a, n, b, ref info, rep.innerobj, ref x, null);
    }
    
    public static void rmatrixsolve(double[,] a, int n, double[] b, out int info, out densesolverreport rep, out double[] x, alglib.xparams _params)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0];
        directdensesolvers.rmatrixsolve(a, n, b, ref info, rep.innerobj, ref x, _params);
    }
    
    /*************************************************************************
    Dense solver.

    This  subroutine  solves  a  system  A*x=b,  where A is NxN non-denegerate
    real matrix, x  and  b  are  vectors.  This is a "fast" version of  linear
    solver which does NOT provide  any  additional  functions  like  condition
    number estimation or iterative refinement.

    Algorithm features:
    * efficient algorithm O(N^3) complexity
    * no performance overhead from additional functionality

    If you need condition number estimation or iterative refinement, use  more
    feature-rich version - RMatrixSolve().

      ! COMMERCIAL EDITION OF ALGLIB:
      !
      ! Commercial Edition of ALGLIB includes following important improvements
      ! of this function:
      ! * high-performance native backend with same C# interface (C# version)
      ! * multithreading support (C++ and C# versions)
      ! * hardware vendor (Intel) implementations of linear algebra primitives
      !   (C++ and C# versions, x86/x64 platform)
      !
      ! We recommend you to read 'Working with commercial version' section  of
      ! ALGLIB Reference Manual in order to find out how to  use  performance-
      ! related features provided by commercial edition of ALGLIB.

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        N       -   size of A
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    matrix is exactly singular (ill conditioned matrices
                            are not recognized).
                    * -1    N<=0 was passed
                    *  1    task is solved
        B       -   array[N]:
                    * info>0    =>  overwritten by solution
                    * info=-3   =>  filled by zeros

      -- ALGLIB --
         Copyright 16.03.2015 by Bochkanov Sergey
    *************************************************************************/
    public static void rmatrixsolvefast(double[,] a, int n, ref double[] b, out int info)
    {
        info = 0;
        directdensesolvers.rmatrixsolvefast(a, n, b, ref info, null);
    }
    
    public static void rmatrixsolvefast(double[,] a, int n, ref double[] b, out int info, alglib.xparams _params)
    {
        info = 0;
        directdensesolvers.rmatrixsolvefast(a, n, b, ref info, _params);
    }
    
    /*************************************************************************
    Dense solver.

    Similar to RMatrixSolve() but solves task with multiple right parts (where
    b and x are NxM matrices). This is  "slow-but-robust"  version  of  linear
    solver with additional functionality  like  condition  number  estimation.
    There also exists faster version - RMatrixSolveMFast().

    Algorithm features:
    * automatic detection of degenerate cases
    * condition number estimation
    * optional iterative refinement
    * O(N^3+M*N^2) complexity

    IMPORTANT: ! this function is NOT the most efficient linear solver provided
               ! by ALGLIB. It estimates condition  number  of  linear  system
               ! and  performs  iterative   refinement,   which   results   in
               ! significant performance penalty  when  compared  with  "fast"
               ! version  which  just  performs  LU  decomposition  and  calls
               ! triangular solver.
               !
               ! This  performance  penalty  is  especially  visible  in   the
               ! multithreaded mode, because both condition number  estimation
               ! and   iterative    refinement   are   inherently   sequential
               ! calculations. It also very significant on small matrices.
               !
               ! Thus, if you need high performance and if you are pretty sure
               ! that your system is well conditioned, we  strongly  recommend
               ! you to use faster solver, RMatrixSolveMFast() function.

      ! COMMERCIAL EDITION OF ALGLIB:
      !
      ! Commercial Edition of ALGLIB includes following important improvements
      ! of this function:
      ! * high-performance native backend with same C# interface (C# version)
      ! * multithreading support (C++ and C# versions)
      ! * hardware vendor (Intel) implementations of linear algebra primitives
      !   (C++ and C# versions, x86/x64 platform)
      !
      ! We recommend you to read 'Working with commercial version' section  of
      ! ALGLIB Reference Manual in order to find out how to  use  performance-
      ! related features provided by commercial edition of ALGLIB.

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        N       -   size of A
        B       -   array[0..N-1,0..M-1], right part
        M       -   right part size
        RFS     -   iterative refinement switch:
                    * True - refinement is used.
                      Less performance, more precision.
                    * False - refinement is not used.
                      More performance, less precision.

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    A is ill conditioned or singular.
                            X is filled by zeros in such cases.
                    * -1    N<=0 was passed
                    *  1    task is solved (but matrix A may be ill-conditioned,
                            check R1/RInf parameters for condition numbers).
        Rep     -   additional report, following fields are set:
                    * rep.r1    condition number in 1-norm
                    * rep.rinf  condition number in inf-norm
        X       -   array[N], it contains:
                    * info>0    =>  solution
                    * info=-3   =>  filled by zeros


      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void rmatrixsolvem(double[,] a, int n, double[,] b, int m, bool rfs, out int info, out densesolverreport rep, out double[,] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0,0];
        directdensesolvers.rmatrixsolvem(a, n, b, m, rfs, ref info, rep.innerobj, ref x, null);
    }
    
    public static void rmatrixsolvem(double[,] a, int n, double[,] b, int m, bool rfs, out int info, out densesolverreport rep, out double[,] x, alglib.xparams _params)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0,0];
        directdensesolvers.rmatrixsolvem(a, n, b, m, rfs, ref info, rep.innerobj, ref x, _params);
    }
    
    /*************************************************************************
    Dense solver.

    Similar to RMatrixSolve() but solves task with multiple right parts (where
    b and x are NxM matrices). This is "fast" version of linear  solver  which
    does NOT offer additional functions like condition  number  estimation  or
    iterative refinement.

    Algorithm features:
    * O(N^3+M*N^2) complexity
    * no additional functionality, highest performance

      ! COMMERCIAL EDITION OF ALGLIB:
      !
      ! Commercial Edition of ALGLIB includes following important improvements
      ! of this function:
      ! * high-performance native backend with same C# interface (C# version)
      ! * multithreading support (C++ and C# versions)
      ! * hardware vendor (Intel) implementations of linear algebra primitives
      !   (C++ and C# versions, x86/x64 platform)
      !
      ! We recommend you to read 'Working with commercial version' section  of
      ! ALGLIB Reference Manual in order to find out how to  use  performance-
      ! related features provided by commercial edition of ALGLIB.

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        N       -   size of A
        B       -   array[0..N-1,0..M-1], right part
        M       -   right part size
        RFS     -   iterative refinement switch:
                    * True - refinement is used.
                      Less performance, more precision.
                    * False - refinement is not used.
                      More performance, less precision.

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    matrix is exactly singular (ill conditioned matrices
                            are not recognized).
                            X is filled by zeros in such cases.
                    * -1    N<=0 was passed
                    *  1    task is solved
        Rep     -   additional report, following fields are set:
                    * rep.r1    condition number in 1-norm
                    * rep.rinf  condition number in inf-norm
        B       -   array[N]:
                    * info>0    =>  overwritten by solution
                    * info=-3   =>  filled by zeros


      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void rmatrixsolvemfast(double[,] a, int n, ref double[,] b, int m, out int info)
    {
        info = 0;
        directdensesolvers.rmatrixsolvemfast(a, n, b, m, ref info, null);
    }
    
    public static void rmatrixsolvemfast(double[,] a, int n, ref double[,] b, int m, out int info, alglib.xparams _params)
    {
        info = 0;
        directdensesolvers.rmatrixsolvemfast(a, n, b, m, ref info, _params);
    }
    
    /*************************************************************************
    Dense solver.

    This  subroutine  solves  a  system  A*x=b,  where A is NxN non-denegerate
    real matrix given by its LU decomposition, x and b are real vectors.  This
    is "slow-but-robust" version of the linear LU-based solver. Faster version
    is RMatrixLUSolveFast() function.

    Algorithm features:
    * automatic detection of degenerate cases
    * O(N^2) complexity
    * condition number estimation

    No iterative refinement  is provided because exact form of original matrix
    is not known to subroutine. Use RMatrixSolve or RMatrixMixedSolve  if  you
    need iterative refinement.

    IMPORTANT: ! this function is NOT the most efficient linear solver provided
               ! by ALGLIB. It estimates condition  number  of  linear system,
               ! which results in 10-15x  performance  penalty  when  compared
               ! with "fast" version which just calls triangular solver.
               !
               ! This performance penalty is insignificant  when compared with
               ! cost of large LU decomposition.  However,  if you  call  this
               ! function many times for the same  left  side,  this  overhead
               ! BECOMES significant. It  also  becomes significant for small-
               ! scale problems.
               !
               ! In such cases we strongly recommend you to use faster solver,
               ! RMatrixLUSolveFast() function.

    INPUT PARAMETERS
        LUA     -   array[N,N], LU decomposition, RMatrixLU result
        P       -   array[N], pivots array, RMatrixLU result
        N       -   size of A
        B       -   array[N], right part

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    matrix is very badly conditioned or exactly singular.
                    * -1    N<=0 was passed
                    *  1    task is solved (but matrix A may be ill-conditioned,
                            check R1/RInf parameters for condition numbers).
        Rep     -   additional report, following fields are set:
                    * rep.r1    condition number in 1-norm
                    * rep.rinf  condition number in inf-norm
        X       -   array[N], it contains:
                    * info>0    =>  solution
                    * info=-3   =>  filled by zeros


      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void rmatrixlusolve(double[,] lua, int[] p, int n, double[] b, out int info, out densesolverreport rep, out double[] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0];
        directdensesolvers.rmatrixlusolve(lua, p, n, b, ref info, rep.innerobj, ref x, null);
    }
    
    public static void rmatrixlusolve(double[,] lua, int[] p, int n, double[] b, out int info, out densesolverreport rep, out double[] x, alglib.xparams _params)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0];
        directdensesolvers.rmatrixlusolve(lua, p, n, b, ref info, rep.innerobj, ref x, _params);
    }
    
    /*************************************************************************
    Dense solver.

    This  subroutine  solves  a  system  A*x=b,  where A is NxN non-denegerate
    real matrix given by its LU decomposition, x and b are real vectors.  This
    is "fast-without-any-checks" version of the linear LU-based solver. Slower
    but more robust version is RMatrixLUSolve() function.

    Algorithm features:
    * O(N^2) complexity
    * fast algorithm without ANY additional checks, just triangular solver

    INPUT PARAMETERS
        LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
        P       -   array[0..N-1], pivots array, RMatrixLU result
        N       -   size of A
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    matrix is exactly singular (ill conditioned matrices
                            are not recognized).
                            X is filled by zeros in such cases.
                    * -1    N<=0 was passed
                    *  1    task is solved
        B       -   array[N]:
                    * info>0    =>  overwritten by solution
                    * info=-3   =>  filled by zeros

      -- ALGLIB --
         Copyright 18.03.2015 by Bochkanov Sergey
    *************************************************************************/
    public static void rmatrixlusolvefast(double[,] lua, int[] p, int n, ref double[] b, out int info)
    {
        info = 0;
        directdensesolvers.rmatrixlusolvefast(lua, p, n, b, ref info, null);
    }
    
    public static void rmatrixlusolvefast(double[,] lua, int[] p, int n, ref double[] b, out int info, alglib.xparams _params)
    {
        info = 0;
        directdensesolvers.rmatrixlusolvefast(lua, p, n, b, ref info, _params);
    }
    
    /*************************************************************************
    Dense solver.

    Similar to RMatrixLUSolve() but solves  task  with  multiple  right  parts
    (where b and x are NxM matrices). This  is  "robust-but-slow"  version  of
    LU-based solver which performs additional  checks  for  non-degeneracy  of
    inputs (condition number estimation). If you need  best  performance,  use
    "fast-without-any-checks" version, RMatrixLUSolveMFast().

    Algorithm features:
    * automatic detection of degenerate cases
    * O(M*N^2) complexity
    * condition number estimation

    No iterative refinement  is provided because exact form of original matrix
    is not known to subroutine. Use RMatrixSolve or RMatrixMixedSolve  if  you
    need iterative refinement.

    IMPORTANT: ! this function is NOT the most efficient linear solver provided
               ! by ALGLIB. It estimates condition  number  of  linear system,
               ! which  results  in  significant  performance   penalty   when
               ! compared with "fast"  version  which  just  calls  triangular
               ! solver.
               !
               ! This performance penalty is especially apparent when you  use
               ! ALGLIB parallel capabilities (condition number estimation  is
               ! inherently  sequential).  It   also   becomes significant for
               ! small-scale problems.
               !
               ! In such cases we strongly recommend you to use faster solver,
               ! RMatrixLUSolveMFast() function.

      ! COMMERCIAL EDITION OF ALGLIB:
      !
      ! Commercial Edition of ALGLIB includes following important improvements
      ! of this function:
      ! * high-performance native backend with same C# interface (C# version)
      ! * multithreading support (C++ and C# versions)
      ! * hardware vendor (Intel) implementations of linear algebra primitives
      !   (C++ and C# versions, x86/x64 platform)
      !
      ! We recommend you to read 'Working with commercial version' section  of
      ! ALGLIB Reference Manual in order to find out how to  use  performance-
      ! related features provided by commercial edition of ALGLIB.

    INPUT PARAMETERS
        LUA     -   array[N,N], LU decomposition, RMatrixLU result
        P       -   array[N], pivots array, RMatrixLU result
        N       -   size of A
        B       -   array[0..N-1,0..M-1], right part
        M       -   right part size

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    matrix is very badly conditioned or exactly singular.
                            X is filled by zeros in such cases.
                    * -1    N<=0 was passed
                    *  1    task is solved (but matrix A may be ill-conditioned,
                            check R1/RInf parameters for condition numbers).
        Rep     -   additional report, following fields are set:
                    * rep.r1    condition number in 1-norm
                    * rep.rinf  condition number in inf-norm
        X       -   array[N,M], it contains:
                    * info>0    =>  solution
                    * info=-3   =>  filled by zeros


      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void rmatrixlusolvem(double[,] lua, int[] p, int n, double[,] b, int m, out int info, out densesolverreport rep, out double[,] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0,0];
        directdensesolvers.rmatrixlusolvem(lua, p, n, b, m, ref info, rep.innerobj, ref x, null);
    }
    
    public static void rmatrixlusolvem(double[,] lua, int[] p, int n, double[,] b, int m, out int info, out densesolverreport rep, out double[,] x, alglib.xparams _params)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0,0];
        directdensesolvers.rmatrixlusolvem(lua, p, n, b, m, ref info, rep.innerobj, ref x, _params);
    }
    
    /*************************************************************************
    Dense solver.

    Similar to RMatrixLUSolve() but solves  task  with  multiple  right parts,
    where b and x are NxM matrices.  This is "fast-without-any-checks" version
    of LU-based solver. It does not estimate  condition number  of  a  system,
    so it is extremely fast. If you need better detection  of  near-degenerate
    cases, use RMatrixLUSolveM() function.

    Algorithm features:
    * O(M*N^2) complexity
    * fast algorithm without ANY additional checks, just triangular solver

      ! COMMERCIAL EDITION OF ALGLIB:
      !
      ! Commercial Edition of ALGLIB includes following important improvements
      ! of this function:
      ! * high-performance native backend with same C# interface (C# version)
      ! * multithreading support (C++ and C# versions)
      ! * hardware vendor (Intel) implementations of linear algebra primitives
      !   (C++ and C# versions, x86/x64 platform)
      !
      ! We recommend you to read 'Working with commercial version' section  of
      ! ALGLIB Reference Manual in order to find out how to  use  performance-
      ! related features provided by commercial edition of ALGLIB.

    INPUT PARAMETERS:
        LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
        P       -   array[0..N-1], pivots array, RMatrixLU result
        N       -   size of A
        B       -   array[0..N-1,0..M-1], right part
        M       -   right part size

    OUTPUT PARAMETERS:
        Info    -   return code:
                    * -3    matrix is exactly singular (ill conditioned matrices
                            are not recognized).
                    * -1    N<=0 was passed
                    *  1    task is solved
        B       -   array[N,M]:
                    * info>0    =>  overwritten by solution
                    * info=-3   =>  filled by zeros

      -- ALGLIB --
         Copyright 18.03.2015 by Bochkanov Sergey
    *************************************************************************/
    public static void rmatrixlusolvemfast(double[,] lua, int[] p, int n, ref double[,] b, int m, out int info)
    {
        info = 0;
        directdensesolvers.rmatrixlusolvemfast(lua, p, n, b, m, ref info, null);
    }
    
    public static void rmatrixlusolvemfast(double[,] lua, int[] p, int n, ref double[,] b, int m, out int info, alglib.xparams _params)
    {
        info = 0;
        directdensesolvers.rmatrixlusolvemfast(lua, p, n, b, m, ref info, _params);
    }
    
    /*************************************************************************
    Dense solver.

    This  subroutine  solves  a  system  A*x=b,  where BOTH ORIGINAL A AND ITS
    LU DECOMPOSITION ARE KNOWN. You can use it if for some  reasons  you  have
    both A and its LU decomposition.

    Algorithm features:
    * automatic detection of degenerate cases
    * condition number estimation
    * iterative refinement
    * O(N^2) complexity

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
        P       -   array[0..N-1], pivots array, RMatrixLU result
        N       -   size of A
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    matrix is very badly conditioned or exactly singular.
                    * -1    N<=0 was passed
                    *  1    task is solved (but matrix A may be ill-conditioned,
                            check R1/RInf parameters for condition numbers).
        Rep     -   additional report, following fields are set:
                    * rep.r1    condition number in 1-norm
                    * rep.rinf  condition number in inf-norm
        X       -   array[N], it contains:
                    * info>0    =>  solution
                    * info=-3   =>  filled by zeros

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void rmatrixmixedsolve(double[,] a, double[,] lua, int[] p, int n, double[] b, out int info, out densesolverreport rep, out double[] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0];
        directdensesolvers.rmatrixmixedsolve(a, lua, p, n, b, ref info, rep.innerobj, ref x, null);
    }
    
    public static void rmatrixmixedsolve(double[,] a, double[,] lua, int[] p, int n, double[] b, out int info, out densesolverreport rep, out double[] x, alglib.xparams _params)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0];
        directdensesolvers.rmatrixmixedsolve(a, lua, p, n, b, ref info, rep.innerobj, ref x, _params);
    }
    
    /*************************************************************************
    Dense solver.

    Similar to RMatrixMixedSolve() but  solves task with multiple right  parts
    (where b and x are NxM matrices).

    Algorithm features:
    * automatic detection of degenerate cases
    * condition number estimation
    * iterative refinement
    * O(M*N^2) complexity

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
        P       -   array[0..N-1], pivots array, RMatrixLU result
        N       -   size of A
        B       -   array[0..N-1,0..M-1], right part
        M       -   right part size

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    matrix is very badly conditioned or exactly singular.
                    * -1    N<=0 was passed
                    *  1    task is solved (but matrix A may be ill-conditioned,
                            check R1/RInf parameters for condition numbers).
        Rep     -   additional report, following fields are set:
                    * rep.r1    condition number in 1-norm
                    * rep.rinf  condition number in inf-norm
        X       -   array[N,M], it contains:
                    * info>0    =>  solution
                    * info=-3   =>  filled by zeros

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void rmatrixmixedsolvem(double[,] a, double[,] lua, int[] p, int n, double[,] b, int m, out int info, out densesolverreport rep, out double[,] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0,0];
        directdensesolvers.rmatrixmixedsolvem(a, lua, p, n, b, m, ref info, rep.innerobj, ref x, null);
    }
    
    public static void rmatrixmixedsolvem(double[,] a, double[,] lua, int[] p, int n, double[,] b, int m, out int info, out densesolverreport rep, out double[,] x, alglib.xparams _params)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0,0];
        directdensesolvers.rmatrixmixedsolvem(a, lua, p, n, b, m, ref info, rep.innerobj, ref x, _params);
    }
    
    /*************************************************************************
    Complex dense solver for A*X=B with N*N  complex  matrix  A,  N*M  complex
    matrices  X  and  B.  "Slow-but-feature-rich"   version   which   provides
    additional functions, at the cost of slower  performance.  Faster  version
    may be invoked with CMatrixSolveMFast() function.

    Algorithm features:
    * automatic detection of degenerate cases
    * condition number estimation
    * iterative refinement
    * O(N^3+M*N^2) complexity

    IMPORTANT: ! this function is NOT the most efficient linear solver provided
               ! by ALGLIB. It estimates condition  number  of  linear  system
               ! and  performs  iterative   refinement,   which   results   in
               ! significant performance penalty  when  compared  with  "fast"
               ! version  which  just  performs  LU  decomposition  and  calls
               ! triangular solver.
               !
               ! This  performance  penalty  is  especially  visible  in   the
               ! multithreaded mode, because both condition number  estimation
               ! and   iterative    refinement   are   inherently   sequential
               ! calculations.
               !
               ! Thus, if you need high performance and if you are pretty sure
               ! that your system is well conditioned, we  strongly  recommend
               ! you to use faster solver, CMatrixSolveMFast() function.

      ! COMMERCIAL EDITION OF ALGLIB:
      !
      ! Commercial Edition of ALGLIB includes following important improvements
      ! of this function:
      ! * high-performance native backend with same C# interface (C# version)
      ! * multithreading support (C++ and C# versions)
      ! * hardware vendor (Intel) implementations of linear algebra primitives
      !   (C++ and C# versions, x86/x64 platform)
      !
      ! We recommend you to read 'Working with commercial version' section  of
      ! ALGLIB Reference Manual in order to find out how to  use  performance-
      ! related features provided by commercial edition of ALGLIB.

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        N       -   size of A
        B       -   array[0..N-1,0..M-1], right part
        M       -   right part size
        RFS     -   iterative refinement switch:
                    * True - refinement is used.
                      Less performance, more precision.
                    * False - refinement is not used.
                      More performance, less precision.

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    matrix is very badly conditioned or exactly singular.
                            X is filled by zeros in such cases.
                    * -1    N<=0 was passed
                    *  1    task is solved (but matrix A may be ill-conditioned,
                            check R1/RInf parameters for condition numbers).
        Rep     -   additional report, following fields are set:
                    * rep.r1    condition number in 1-norm
                    * rep.rinf  condition number in inf-norm
        X       -   array[N,M], it contains:
                    * info>0    =>  solution
                    * info=-3   =>  filled by zeros

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void cmatrixsolvem(complex[,] a, int n, complex[,] b, int m, bool rfs, out int info, out densesolverreport rep, out complex[,] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0,0];
        directdensesolvers.cmatrixsolvem(a, n, b, m, rfs, ref info, rep.innerobj, ref x, null);
    }
    
    public static void cmatrixsolvem(complex[,] a, int n, complex[,] b, int m, bool rfs, out int info, out densesolverreport rep, out complex[,] x, alglib.xparams _params)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0,0];
        directdensesolvers.cmatrixsolvem(a, n, b, m, rfs, ref info, rep.innerobj, ref x, _params);
    }
    
    /*************************************************************************
    Complex dense solver for A*X=B with N*N  complex  matrix  A,  N*M  complex
    matrices  X  and  B.  "Fast-but-lightweight" version which  provides  just
    triangular solver - and no additional functions like iterative  refinement
    or condition number estimation.

    Algorithm features:
    * O(N^3+M*N^2) complexity
    * no additional time consuming functions

      ! COMMERCIAL EDITION OF ALGLIB:
      !
      ! Commercial Edition of ALGLIB includes following important improvements
      ! of this function:
      ! * high-performance native backend with same C# interface (C# version)
      ! * multithreading support (C++ and C# versions)
      ! * hardware vendor (Intel) implementations of linear algebra primitives
      !   (C++ and C# versions, x86/x64 platform)
      !
      ! We recommend you to read 'Working with commercial version' section  of
      ! ALGLIB Reference Manual in order to find out how to  use  performance-
      ! related features provided by commercial edition of ALGLIB.

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        N       -   size of A
        B       -   array[0..N-1,0..M-1], right part
        M       -   right part size

    OUTPUT PARAMETERS:
        Info    -   return code:
                    * -3    matrix is exactly singular (ill conditioned matrices
                            are not recognized).
                    * -1    N<=0 was passed
                    *  1    task is solved
        B       -   array[N,M]:
                    * info>0    =>  overwritten by solution
                    * info=-3   =>  filled by zeros

      -- ALGLIB --
         Copyright 16.03.2015 by Bochkanov Sergey
    *************************************************************************/
    public static void cmatrixsolvemfast(complex[,] a, int n, ref complex[,] b, int m, out int info)
    {
        info = 0;
        directdensesolvers.cmatrixsolvemfast(a, n, b, m, ref info, null);
    }
    
    public static void cmatrixsolvemfast(complex[,] a, int n, ref complex[,] b, int m, out int info, alglib.xparams _params)
    {
        info = 0;
        directdensesolvers.cmatrixsolvemfast(a, n, b, m, ref info, _params);
    }
    
    /*************************************************************************
    Complex dense solver for A*x=B with N*N complex matrix A and  N*1  complex
    vectors x and b. "Slow-but-feature-rich" version of the solver.

    Algorithm features:
    * automatic detection of degenerate cases
    * condition number estimation
    * iterative refinement
    * O(N^3) complexity

    IMPORTANT: ! this function is NOT the most efficient linear solver provided
               ! by ALGLIB. It estimates condition  number  of  linear  system
               ! and  performs  iterative   refinement,   which   results   in
               ! significant performance penalty  when  compared  with  "fast"
               ! version  which  just  performs  LU  decomposition  and  calls
               ! triangular solver.
               !
               ! This  performance  penalty  is  especially  visible  in   the
               ! multithreaded mode, because both condition number  estimation
               ! and   iterative    refinement   are   inherently   sequential
               ! calculations.
               !
               ! Thus, if you need high performance and if you are pretty sure
               ! that your system is well conditioned, we  strongly  recommend
               ! you to use faster solver, CMatrixSolveFast() function.

      ! COMMERCIAL EDITION OF ALGLIB:
      !
      ! Commercial Edition of ALGLIB includes following important improvements
      ! of this function:
      ! * high-performance native backend with same C# interface (C# version)
      ! * multithreading support (C++ and C# versions)
      ! * hardware vendor (Intel) implementations of linear algebra primitives
      !   (C++ and C# versions, x86/x64 platform)
      !
      ! We recommend you to read 'Working with commercial version' section  of
      ! ALGLIB Reference Manual in order to find out how to  use  performance-
      ! related features provided by commercial edition of ALGLIB.

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        N       -   size of A
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    matrix is very badly conditioned or exactly singular.
                    * -1    N<=0 was passed
                    *  1    task is solved (but matrix A may be ill-conditioned,
                            check R1/RInf parameters for condition numbers).
        Rep     -   additional report, following fields are set:
                    * rep.r1    condition number in 1-norm
                    * rep.rinf  condition number in inf-norm
        X       -   array[N], it contains:
                    * info>0    =>  solution
                    * info=-3   =>  filled by zeros

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void cmatrixsolve(complex[,] a, int n, complex[] b, out int info, out densesolverreport rep, out complex[] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0];
        directdensesolvers.cmatrixsolve(a, n, b, ref info, rep.innerobj, ref x, null);
    }
    
    public static void cmatrixsolve(complex[,] a, int n, complex[] b, out int info, out densesolverreport rep, out complex[] x, alglib.xparams _params)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0];
        directdensesolvers.cmatrixsolve(a, n, b, ref info, rep.innerobj, ref x, _params);
    }
    
    /*************************************************************************
    Complex dense solver for A*x=B with N*N complex matrix A and  N*1  complex
    vectors x and b. "Fast-but-lightweight" version of the solver.

    Algorithm features:
    * O(N^3) complexity
    * no additional time consuming features, just triangular solver

      ! COMMERCIAL EDITION OF ALGLIB:
      !
      ! Commercial Edition of ALGLIB includes following important improvements
      ! of this function:
      ! * high-performance native backend with same C# interface (C# version)
      ! * multithreading support (C++ and C# versions)
      ! * hardware vendor (Intel) implementations of linear algebra primitives
      !   (C++ and C# versions, x86/x64 platform)
      !
      ! We recommend you to read 'Working with commercial version' section  of
      ! ALGLIB Reference Manual in order to find out how to  use  performance-
      ! related features provided by commercial edition of ALGLIB.

    INPUT PARAMETERS:
        A       -   array[0..N-1,0..N-1], system matrix
        N       -   size of A
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS:
        Info    -   return code:
                    * -3    matrix is exactly singular (ill conditioned matrices
                            are not recognized).
                    * -1    N<=0 was passed
                    *  1    task is solved
        B       -   array[N]:
                    * info>0    =>  overwritten by solution
                    * info=-3   =>  filled by zeros

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void cmatrixsolvefast(complex[,] a, int n, ref complex[] b, out int info)
    {
        info = 0;
        directdensesolvers.cmatrixsolvefast(a, n, b, ref info, null);
    }
    
    public static void cmatrixsolvefast(complex[,] a, int n, ref complex[] b, out int info, alglib.xparams _params)
    {
        info = 0;
        directdensesolvers.cmatrixsolvefast(a, n, b, ref info, _params);
    }
    
    /*************************************************************************
    Dense solver for A*X=B with N*N complex A given by its  LU  decomposition,
    and N*M matrices X and B (multiple right sides).   "Slow-but-feature-rich"
    version of the solver.

    Algorithm features:
    * automatic detection of degenerate cases
    * O(M*N^2) complexity
    * condition number estimation

    No iterative refinement  is provided because exact form of original matrix
    is not known to subroutine. Use CMatrixSolve or CMatrixMixedSolve  if  you
    need iterative refinement.

    IMPORTANT: ! this function is NOT the most efficient linear solver provided
               ! by ALGLIB. It estimates condition  number  of  linear system,
               ! which  results  in  significant  performance   penalty   when
               ! compared with "fast"  version  which  just  calls  triangular
               ! solver.
               !
               ! This performance penalty is especially apparent when you  use
               ! ALGLIB parallel capabilities (condition number estimation  is
               ! inherently  sequential).  It   also   becomes significant for
               ! small-scale problems.
               !
               ! In such cases we strongly recommend you to use faster solver,
               ! CMatrixLUSolveMFast() function.

      ! COMMERCIAL EDITION OF ALGLIB:
      !
      ! Commercial Edition of ALGLIB includes following important improvements
      ! of this function:
      ! * high-performance native backend with same C# interface (C# version)
      ! * multithreading support (C++ and C# versions)
      ! * hardware vendor (Intel) implementations of linear algebra primitives
      !   (C++ and C# versions, x86/x64 platform)
      !
      ! We recommend you to read 'Working with commercial version' section  of
      ! ALGLIB Reference Manual in order to find out how to  use  performance-
      ! related features provided by commercial edition of ALGLIB.

    INPUT PARAMETERS
        LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
        P       -   array[0..N-1], pivots array, RMatrixLU result
        N       -   size of A
        B       -   array[0..N-1,0..M-1], right part
        M       -   right part size

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    matrix is very badly conditioned or exactly singular.
                    * -1    N<=0 was passed
                    *  1    task is solved (but matrix A may be ill-conditioned,
                            check R1/RInf parameters for condition numbers).
        Rep     -   additional report, following fields are set:
                    * rep.r1    condition number in 1-norm
                    * rep.rinf  condition number in inf-norm
        X       -   array[N,M], it contains:
                    * info>0    =>  solution
                    * info=-3   =>  filled by zeros

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void cmatrixlusolvem(complex[,] lua, int[] p, int n, complex[,] b, int m, out int info, out densesolverreport rep, out complex[,] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0,0];
        directdensesolvers.cmatrixlusolvem(lua, p, n, b, m, ref info, rep.innerobj, ref x, null);
    }
    
    public static void cmatrixlusolvem(complex[,] lua, int[] p, int n, complex[,] b, int m, out int info, out densesolverreport rep, out complex[,] x, alglib.xparams _params)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0,0];
        directdensesolvers.cmatrixlusolvem(lua, p, n, b, m, ref info, rep.innerobj, ref x, _params);
    }
    
    /*************************************************************************
    Dense solver for A*X=B with N*N complex A given by its  LU  decomposition,
    and N*M matrices X and B (multiple  right  sides).  "Fast-but-lightweight"
    version of the solver.

    Algorithm features:
    * O(M*N^2) complexity
    * no additional time-consuming features

      ! COMMERCIAL EDITION OF ALGLIB:
      !
      ! Commercial Edition of ALGLIB includes following important improvements
      ! of this function:
      ! * high-performance native backend with same C# interface (C# version)
      ! * multithreading support (C++ and C# versions)
      ! * hardware vendor (Intel) implementations of linear algebra primitives
      !   (C++ and C# versions, x86/x64 platform)
      !
      ! We recommend you to read 'Working with commercial version' section  of
      ! ALGLIB Reference Manual in order to find out how to  use  performance-
      ! related features provided by commercial edition of ALGLIB.

    INPUT PARAMETERS
        LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
        P       -   array[0..N-1], pivots array, RMatrixLU result
        N       -   size of A
        B       -   array[0..N-1,0..M-1], right part
        M       -   right part size

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    matrix is exactly singular (ill conditioned matrices
                            are not recognized).
                    * -1    N<=0 was passed
                    *  1    task is solved
        B       -   array[N,M]:
                    * info>0    =>  overwritten by solution
                    * info=-3   =>  filled by zeros


      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void cmatrixlusolvemfast(complex[,] lua, int[] p, int n, ref complex[,] b, int m, out int info)
    {
        info = 0;
        directdensesolvers.cmatrixlusolvemfast(lua, p, n, b, m, ref info, null);
    }
    
    public static void cmatrixlusolvemfast(complex[,] lua, int[] p, int n, ref complex[,] b, int m, out int info, alglib.xparams _params)
    {
        info = 0;
        directdensesolvers.cmatrixlusolvemfast(lua, p, n, b, m, ref info, _params);
    }
    
    /*************************************************************************
    Complex dense linear solver for A*x=b with complex N*N A  given  by its LU
    decomposition and N*1 vectors x and b. This is  "slow-but-robust"  version
    of  the  complex  linear  solver  with  additional  features   which   add
    significant performance overhead. Faster version  is  CMatrixLUSolveFast()
    function.

    Algorithm features:
    * automatic detection of degenerate cases
    * O(N^2) complexity
    * condition number estimation

    No iterative refinement is provided because exact form of original matrix
    is not known to subroutine. Use CMatrixSolve or CMatrixMixedSolve  if  you
    need iterative refinement.

    IMPORTANT: ! this function is NOT the most efficient linear solver provided
               ! by ALGLIB. It estimates condition  number  of  linear system,
               ! which results in 10-15x  performance  penalty  when  compared
               ! with "fast" version which just calls triangular solver.
               !
               ! This performance penalty is insignificant  when compared with
               ! cost of large LU decomposition.  However,  if you  call  this
               ! function many times for the same  left  side,  this  overhead
               ! BECOMES significant. It  also  becomes significant for small-
               ! scale problems.
               !
               ! In such cases we strongly recommend you to use faster solver,
               ! CMatrixLUSolveFast() function.

    INPUT PARAMETERS
        LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
        P       -   array[0..N-1], pivots array, CMatrixLU result
        N       -   size of A
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    matrix is very badly conditioned or exactly singular.
                    * -1    N<=0 was passed
                    *  1    task is solved (but matrix A may be ill-conditioned,
                            check R1/RInf parameters for condition numbers).
        Rep     -   additional report, following fields are set:
                    * rep.r1    condition number in 1-norm
                    * rep.rinf  condition number in inf-norm
        X       -   array[N], it contains:
                    * info>0    =>  solution
                    * info=-3   =>  filled by zeros

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void cmatrixlusolve(complex[,] lua, int[] p, int n, complex[] b, out int info, out densesolverreport rep, out complex[] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0];
        directdensesolvers.cmatrixlusolve(lua, p, n, b, ref info, rep.innerobj, ref x, null);
    }
    
    public static void cmatrixlusolve(complex[,] lua, int[] p, int n, complex[] b, out int info, out densesolverreport rep, out complex[] x, alglib.xparams _params)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0];
        directdensesolvers.cmatrixlusolve(lua, p, n, b, ref info, rep.innerobj, ref x, _params);
    }
    
    /*************************************************************************
    Complex dense linear solver for A*x=b with N*N complex A given by  its  LU
    decomposition and N*1 vectors x and b. This is  fast  lightweight  version
    of solver, which is significantly faster than CMatrixLUSolve(),  but  does
    not provide additional information (like condition numbers).

    Algorithm features:
    * O(N^2) complexity
    * no additional time-consuming features, just triangular solver

    INPUT PARAMETERS
        LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
        P       -   array[0..N-1], pivots array, CMatrixLU result
        N       -   size of A
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    matrix is exactly singular (ill conditioned matrices
                            are not recognized).
                    * -1    N<=0 was passed
                    *  1    task is solved
        B       -   array[N]:
                    * info>0    =>  overwritten by solution
                    * info=-3   =>  filled by zeros

    NOTE: unlike  CMatrixLUSolve(),  this   function   does   NOT   check  for
          near-degeneracy of input matrix. It  checks  for  EXACT  degeneracy,
          because this check is easy to do. However,  very  badly  conditioned
          matrices may went unnoticed.


      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void cmatrixlusolvefast(complex[,] lua, int[] p, int n, ref complex[] b, out int info)
    {
        info = 0;
        directdensesolvers.cmatrixlusolvefast(lua, p, n, b, ref info, null);
    }
    
    public static void cmatrixlusolvefast(complex[,] lua, int[] p, int n, ref complex[] b, out int info, alglib.xparams _params)
    {
        info = 0;
        directdensesolvers.cmatrixlusolvefast(lua, p, n, b, ref info, _params);
    }
    
    /*************************************************************************
    Dense solver. Same as RMatrixMixedSolveM(), but for complex matrices.

    Algorithm features:
    * automatic detection of degenerate cases
    * condition number estimation
    * iterative refinement
    * O(M*N^2) complexity

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
        P       -   array[0..N-1], pivots array, CMatrixLU result
        N       -   size of A
        B       -   array[0..N-1,0..M-1], right part
        M       -   right part size

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    matrix is very badly conditioned or exactly singular.
                    * -1    N<=0 was passed
                    *  1    task is solved (but matrix A may be ill-conditioned,
                            check R1/RInf parameters for condition numbers).
        Rep     -   additional report, following fields are set:
                    * rep.r1    condition number in 1-norm
                    * rep.rinf  condition number in inf-norm
        X       -   array[N,M], it contains:
                    * info>0    =>  solution
                    * info=-3   =>  filled by zeros

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void cmatrixmixedsolvem(complex[,] a, complex[,] lua, int[] p, int n, complex[,] b, int m, out int info, out densesolverreport rep, out complex[,] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0,0];
        directdensesolvers.cmatrixmixedsolvem(a, lua, p, n, b, m, ref info, rep.innerobj, ref x, null);
    }
    
    public static void cmatrixmixedsolvem(complex[,] a, complex[,] lua, int[] p, int n, complex[,] b, int m, out int info, out densesolverreport rep, out complex[,] x, alglib.xparams _params)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0,0];
        directdensesolvers.cmatrixmixedsolvem(a, lua, p, n, b, m, ref info, rep.innerobj, ref x, _params);
    }
    
    /*************************************************************************
    Dense solver. Same as RMatrixMixedSolve(), but for complex matrices.

    Algorithm features:
    * automatic detection of degenerate cases
    * condition number estimation
    * iterative refinement
    * O(N^2) complexity

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
        P       -   array[0..N-1], pivots array, CMatrixLU result
        N       -   size of A
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    matrix is very badly conditioned or exactly singular.
                    * -1    N<=0 was passed
                    *  1    task is solved (but matrix A may be ill-conditioned,
                            check R1/RInf parameters for condition numbers).
        Rep     -   additional report, following fields are set:
                    * rep.r1    condition number in 1-norm
                    * rep.rinf  condition number in inf-norm
        X       -   array[N], it contains:
                    * info>0    =>  solution
                    * info=-3   =>  filled by zeros

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void cmatrixmixedsolve(complex[,] a, complex[,] lua, int[] p, int n, complex[] b, out int info, out densesolverreport rep, out complex[] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0];
        directdensesolvers.cmatrixmixedsolve(a, lua, p, n, b, ref info, rep.innerobj, ref x, null);
    }
    
    public static void cmatrixmixedsolve(complex[,] a, complex[,] lua, int[] p, int n, complex[] b, out int info, out densesolverreport rep, out complex[] x, alglib.xparams _params)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0];
        directdensesolvers.cmatrixmixedsolve(a, lua, p, n, b, ref info, rep.innerobj, ref x, _params);
    }
    
    /*************************************************************************
    Dense solver for A*X=B with N*N symmetric positive definite matrix A,  and
    N*M vectors X and B. It is "slow-but-feature-rich" version of the solver.

    Algorithm features:
    * automatic detection of degenerate cases
    * condition number estimation
    * O(N^3+M*N^2) complexity
    * matrix is represented by its upper or lower triangle

    No iterative refinement is provided because such partial representation of
    matrix does not allow efficient calculation of extra-precise  matrix-vector
    products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
    need iterative refinement.

    IMPORTANT: ! this function is NOT the most efficient linear solver provided
               ! by ALGLIB. It estimates condition  number  of  linear system,
               ! which  results  in  significant   performance   penalty  when
               ! compared with "fast" version  which  just  performs  Cholesky
               ! decomposition and calls triangular solver.
               !
               ! This  performance  penalty  is  especially  visible  in   the
               ! multithreaded mode, because both condition number  estimation
               ! and   iterative    refinement   are   inherently   sequential
               ! calculations.
               !
               ! Thus, if you need high performance and if you are pretty sure
               ! that your system is well conditioned, we  strongly  recommend
               ! you to use faster solver, SPDMatrixSolveMFast() function.

      ! COMMERCIAL EDITION OF ALGLIB:
      !
      ! Commercial Edition of ALGLIB includes following important improvements
      ! of this function:
      ! * high-performance native backend with same C# interface (C# version)
      ! * multithreading support (C++ and C# versions)
      ! * hardware vendor (Intel) implementations of linear algebra primitives
      !   (C++ and C# versions, x86/x64 platform)
      !
      ! We recommend you to read 'Working with commercial version' section  of
      ! ALGLIB Reference Manual in order to find out how to  use  performance-
      ! related features provided by commercial edition of ALGLIB.

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        N       -   size of A
        IsUpper -   what half of A is provided
        B       -   array[0..N-1,0..M-1], right part
        M       -   right part size

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    matrix is very badly conditioned or non-SPD.
                    * -1    N<=0 was passed
                    *  1    task is solved (but matrix A may be ill-conditioned,
                            check R1/RInf parameters for condition numbers).
        Rep     -   additional report, following fields are set:
                    * rep.r1    condition number in 1-norm
                    * rep.rinf  condition number in inf-norm
        X       -   array[N,M], it contains:
                    * info>0    =>  solution
                    * info=-3   =>  filled by zeros

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void spdmatrixsolvem(double[,] a, int n, bool isupper, double[,] b, int m, out int info, out densesolverreport rep, out double[,] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0,0];
        directdensesolvers.spdmatrixsolvem(a, n, isupper, b, m, ref info, rep.innerobj, ref x, null);
    }
    
    public static void spdmatrixsolvem(double[,] a, int n, bool isupper, double[,] b, int m, out int info, out densesolverreport rep, out double[,] x, alglib.xparams _params)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0,0];
        directdensesolvers.spdmatrixsolvem(a, n, isupper, b, m, ref info, rep.innerobj, ref x, _params);
    }
    
    /*************************************************************************
    Dense solver for A*X=B with N*N symmetric positive definite matrix A,  and
    N*M vectors X and B. It is "fast-but-lightweight" version of the solver.

    Algorithm features:
    * O(N^3+M*N^2) complexity
    * matrix is represented by its upper or lower triangle
    * no additional time consuming features

      ! COMMERCIAL EDITION OF ALGLIB:
      !
      ! Commercial Edition of ALGLIB includes following important improvements
      ! of this function:
      ! * high-performance native backend with same C# interface (C# version)
      ! * multithreading support (C++ and C# versions)
      ! * hardware vendor (Intel) implementations of linear algebra primitives
      !   (C++ and C# versions, x86/x64 platform)
      !
      ! We recommend you to read 'Working with commercial version' section  of
      ! ALGLIB Reference Manual in order to find out how to  use  performance-
      ! related features provided by commercial edition of ALGLIB.

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        N       -   size of A
        IsUpper -   what half of A is provided
        B       -   array[0..N-1,0..M-1], right part
        M       -   right part size

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    A is is exactly singular
                    * -1    N<=0 was passed
                    *  1    task was solved
        B       -   array[N,M], it contains:
                    * info>0    =>  solution
                    * info=-3   =>  filled by zeros

      -- ALGLIB --
         Copyright 17.03.2015 by Bochkanov Sergey
    *************************************************************************/
    public static void spdmatrixsolvemfast(double[,] a, int n, bool isupper, ref double[,] b, int m, out int info)
    {
        info = 0;
        directdensesolvers.spdmatrixsolvemfast(a, n, isupper, b, m, ref info, null);
    }
    
    public static void spdmatrixsolvemfast(double[,] a, int n, bool isupper, ref double[,] b, int m, out int info, alglib.xparams _params)
    {
        info = 0;
        directdensesolvers.spdmatrixsolvemfast(a, n, isupper, b, m, ref info, _params);
    }
    
    /*************************************************************************
    Dense linear solver for A*x=b with N*N real  symmetric  positive  definite
    matrix A,  N*1 vectors x and b.  "Slow-but-feature-rich"  version  of  the
    solver.

    Algorithm features:
    * automatic detection of degenerate cases
    * condition number estimation
    * O(N^3) complexity
    * matrix is represented by its upper or lower triangle

    No iterative refinement is provided because such partial representation of
    matrix does not allow efficient calculation of extra-precise  matrix-vector
    products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
    need iterative refinement.

    IMPORTANT: ! this function is NOT the most efficient linear solver provided
               ! by ALGLIB. It estimates condition  number  of  linear system,
               ! which  results  in  significant   performance   penalty  when
               ! compared with "fast" version  which  just  performs  Cholesky
               ! decomposition and calls triangular solver.
               !
               ! This  performance  penalty  is  especially  visible  in   the
               ! multithreaded mode, because both condition number  estimation
               ! and   iterative    refinement   are   inherently   sequential
               ! calculations.
               !
               ! Thus, if you need high performance and if you are pretty sure
               ! that your system is well conditioned, we  strongly  recommend
               ! you to use faster solver, SPDMatrixSolveFast() function.

      ! COMMERCIAL EDITION OF ALGLIB:
      !
      ! Commercial Edition of ALGLIB includes following important improvements
      ! of this function:
      ! * high-performance native backend with same C# interface (C# version)
      ! * multithreading support (C++ and C# versions)
      ! * hardware vendor (Intel) implementations of linear algebra primitives
      !   (C++ and C# versions, x86/x64 platform)
      !
      ! We recommend you to read 'Working with commercial version' section  of
      ! ALGLIB Reference Manual in order to find out how to  use  performance-
      ! related features provided by commercial edition of ALGLIB.

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        N       -   size of A
        IsUpper -   what half of A is provided
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    matrix is very badly conditioned or non-SPD.
                    * -1    N<=0 was passed
                    *  1    task is solved (but matrix A may be ill-conditioned,
                            check R1/RInf parameters for condition numbers).
        Rep     -   additional report, following fields are set:
                    * rep.r1    condition number in 1-norm
                    * rep.rinf  condition number in inf-norm
        X       -   array[N], it contains:
                    * info>0    =>  solution
                    * info=-3   =>  filled by zeros

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void spdmatrixsolve(double[,] a, int n, bool isupper, double[] b, out int info, out densesolverreport rep, out double[] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0];
        directdensesolvers.spdmatrixsolve(a, n, isupper, b, ref info, rep.innerobj, ref x, null);
    }
    
    public static void spdmatrixsolve(double[,] a, int n, bool isupper, double[] b, out int info, out densesolverreport rep, out double[] x, alglib.xparams _params)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0];
        directdensesolvers.spdmatrixsolve(a, n, isupper, b, ref info, rep.innerobj, ref x, _params);
    }
    
    /*************************************************************************
    Dense linear solver for A*x=b with N*N real  symmetric  positive  definite
    matrix A,  N*1 vectors x and  b.  "Fast-but-lightweight"  version  of  the
    solver.

    Algorithm features:
    * O(N^3) complexity
    * matrix is represented by its upper or lower triangle
    * no additional time consuming features like condition number estimation

      ! COMMERCIAL EDITION OF ALGLIB:
      !
      ! Commercial Edition of ALGLIB includes following important improvements
      ! of this function:
      ! * high-performance native backend with same C# interface (C# version)
      ! * multithreading support (C++ and C# versions)
      ! * hardware vendor (Intel) implementations of linear algebra primitives
      !   (C++ and C# versions, x86/x64 platform)
      !
      ! We recommend you to read 'Working with commercial version' section  of
      ! ALGLIB Reference Manual in order to find out how to  use  performance-
      ! related features provided by commercial edition of ALGLIB.

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        N       -   size of A
        IsUpper -   what half of A is provided
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    A is is exactly singular or non-SPD
                    * -1    N<=0 was passed
                    *  1    task was solved
        B       -   array[N], it contains:
                    * info>0    =>  solution
                    * info=-3   =>  filled by zeros

      -- ALGLIB --
         Copyright 17.03.2015 by Bochkanov Sergey
    *************************************************************************/
    public static void spdmatrixsolvefast(double[,] a, int n, bool isupper, ref double[] b, out int info)
    {
        info = 0;
        directdensesolvers.spdmatrixsolvefast(a, n, isupper, b, ref info, null);
    }
    
    public static void spdmatrixsolvefast(double[,] a, int n, bool isupper, ref double[] b, out int info, alglib.xparams _params)
    {
        info = 0;
        directdensesolvers.spdmatrixsolvefast(a, n, isupper, b, ref info, _params);
    }
    
    /*************************************************************************
    Dense solver for A*X=B with N*N symmetric positive definite matrix A given
    by its Cholesky decomposition, and N*M vectors X and B. It  is  "slow-but-
    feature-rich" version of the solver which estimates  condition  number  of
    the system.

    Algorithm features:
    * automatic detection of degenerate cases
    * O(M*N^2) complexity
    * condition number estimation
    * matrix is represented by its upper or lower triangle

    No iterative refinement is provided because such partial representation of
    matrix does not allow efficient calculation of extra-precise  matrix-vector
    products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
    need iterative refinement.

    IMPORTANT: ! this function is NOT the most efficient linear solver provided
               ! by ALGLIB. It estimates condition  number  of  linear system,
               ! which  results  in  significant  performance   penalty   when
               ! compared with "fast"  version  which  just  calls  triangular
               ! solver. Amount of  overhead  introduced  depends  on  M  (the
               ! larger - the more efficient).
               !
               ! This performance penalty is insignificant  when compared with
               ! cost of large LU decomposition.  However,  if you  call  this
               ! function many times for the same  left  side,  this  overhead
               ! BECOMES significant. It  also  becomes significant for small-
               ! scale problems (N<50).
               !
               ! In such cases we strongly recommend you to use faster solver,
               ! SPDMatrixCholeskySolveMFast() function.

    INPUT PARAMETERS
        CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
                    SPDMatrixCholesky result
        N       -   size of CHA
        IsUpper -   what half of CHA is provided
        B       -   array[0..N-1,0..M-1], right part
        M       -   right part size

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    A is is exactly singular or badly conditioned
                            X is filled by zeros in such cases.
                    * -1    N<=0 was passed
                    *  1    task was solved
        Rep     -   additional report, following fields are set:
                    * rep.r1    condition number in 1-norm
                    * rep.rinf  condition number in inf-norm
        X       -   array[N]:
                    * for info>0 contains solution
                    * for info=-3 filled by zeros

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void spdmatrixcholeskysolvem(double[,] cha, int n, bool isupper, double[,] b, int m, out int info, out densesolverreport rep, out double[,] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0,0];
        directdensesolvers.spdmatrixcholeskysolvem(cha, n, isupper, b, m, ref info, rep.innerobj, ref x, null);
    }
    
    public static void spdmatrixcholeskysolvem(double[,] cha, int n, bool isupper, double[,] b, int m, out int info, out densesolverreport rep, out double[,] x, alglib.xparams _params)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0,0];
        directdensesolvers.spdmatrixcholeskysolvem(cha, n, isupper, b, m, ref info, rep.innerobj, ref x, _params);
    }
    
    /*************************************************************************
    Dense solver for A*X=B with N*N symmetric positive definite matrix A given
    by its Cholesky decomposition, and N*M vectors X and B. It  is  "fast-but-
    lightweight" version of  the  solver  which  just  solves  linear  system,
    without any additional functions.

    Algorithm features:
    * O(M*N^2) complexity
    * matrix is represented by its upper or lower triangle
    * no additional functionality

    INPUT PARAMETERS
        CHA     -   array[N,N], Cholesky decomposition,
                    SPDMatrixCholesky result
        N       -   size of CHA
        IsUpper -   what half of CHA is provided
        B       -   array[N,M], right part
        M       -   right part size

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    A is is exactly singular or badly conditioned
                            X is filled by zeros in such cases.
                    * -1    N<=0 was passed
                    *  1    task was solved
        B       -   array[N]:
                    * for info>0 overwritten by solution
                    * for info=-3 filled by zeros

      -- ALGLIB --
         Copyright 18.03.2015 by Bochkanov Sergey
    *************************************************************************/
    public static void spdmatrixcholeskysolvemfast(double[,] cha, int n, bool isupper, ref double[,] b, int m, out int info)
    {
        info = 0;
        directdensesolvers.spdmatrixcholeskysolvemfast(cha, n, isupper, b, m, ref info, null);
    }
    
    public static void spdmatrixcholeskysolvemfast(double[,] cha, int n, bool isupper, ref double[,] b, int m, out int info, alglib.xparams _params)
    {
        info = 0;
        directdensesolvers.spdmatrixcholeskysolvemfast(cha, n, isupper, b, m, ref info, _params);
    }
    
    /*************************************************************************
    Dense solver for A*x=b with N*N symmetric positive definite matrix A given
    by its Cholesky decomposition, and N*1 real vectors x and b. This is "slow-
    but-feature-rich"  version  of  the  solver  which,  in  addition  to  the
    solution, performs condition number estimation.

    Algorithm features:
    * automatic detection of degenerate cases
    * O(N^2) complexity
    * condition number estimation
    * matrix is represented by its upper or lower triangle

    No iterative refinement is provided because such partial representation of
    matrix does not allow efficient calculation of extra-precise  matrix-vector
    products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
    need iterative refinement.

    IMPORTANT: ! this function is NOT the most efficient linear solver provided
               ! by ALGLIB. It estimates condition  number  of  linear system,
               ! which results in 10-15x  performance  penalty  when  compared
               ! with "fast" version which just calls triangular solver.
               !
               ! This performance penalty is insignificant  when compared with
               ! cost of large LU decomposition.  However,  if you  call  this
               ! function many times for the same  left  side,  this  overhead
               ! BECOMES significant. It  also  becomes significant for small-
               ! scale problems (N<50).
               !
               ! In such cases we strongly recommend you to use faster solver,
               ! SPDMatrixCholeskySolveFast() function.

    INPUT PARAMETERS
        CHA     -   array[N,N], Cholesky decomposition,
                    SPDMatrixCholesky result
        N       -   size of A
        IsUpper -   what half of CHA is provided
        B       -   array[N], right part

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    A is is exactly singular or ill conditioned
                            X is filled by zeros in such cases.
                    * -1    N<=0 was passed
                    *  1    task is solved
        Rep     -   additional report, following fields are set:
                    * rep.r1    condition number in 1-norm
                    * rep.rinf  condition number in inf-norm
        X       -   array[N]:
                    * for info>0  - solution
                    * for info=-3 - filled by zeros

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void spdmatrixcholeskysolve(double[,] cha, int n, bool isupper, double[] b, out int info, out densesolverreport rep, out double[] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0];
        directdensesolvers.spdmatrixcholeskysolve(cha, n, isupper, b, ref info, rep.innerobj, ref x, null);
    }
    
    public static void spdmatrixcholeskysolve(double[,] cha, int n, bool isupper, double[] b, out int info, out densesolverreport rep, out double[] x, alglib.xparams _params)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0];
        directdensesolvers.spdmatrixcholeskysolve(cha, n, isupper, b, ref info, rep.innerobj, ref x, _params);
    }
    
    /*************************************************************************
    Dense solver for A*x=b with N*N symmetric positive definite matrix A given
    by its Cholesky decomposition, and N*1 real vectors x and b. This is "fast-
    but-lightweight" version of the solver.

    Algorithm features:
    * O(N^2) complexity
    * matrix is represented by its upper or lower triangle
    * no additional features

    INPUT PARAMETERS
        CHA     -   array[N,N], Cholesky decomposition,
                    SPDMatrixCholesky result
        N       -   size of A
        IsUpper -   what half of CHA is provided
        B       -   array[N], right part

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    A is is exactly singular or ill conditioned
                            X is filled by zeros in such cases.
                    * -1    N<=0 was passed
                    *  1    task is solved
        B       -   array[N]:
                    * for info>0  - overwritten by solution
                    * for info=-3 - filled by zeros

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void spdmatrixcholeskysolvefast(double[,] cha, int n, bool isupper, ref double[] b, out int info)
    {
        info = 0;
        directdensesolvers.spdmatrixcholeskysolvefast(cha, n, isupper, b, ref info, null);
    }
    
    public static void spdmatrixcholeskysolvefast(double[,] cha, int n, bool isupper, ref double[] b, out int info, alglib.xparams _params)
    {
        info = 0;
        directdensesolvers.spdmatrixcholeskysolvefast(cha, n, isupper, b, ref info, _params);
    }
    
    /*************************************************************************
    Dense solver for A*X=B, with N*N Hermitian positive definite matrix A  and
    N*M  complex  matrices  X  and  B.  "Slow-but-feature-rich" version of the
    solver.

    Algorithm features:
    * automatic detection of degenerate cases
    * condition number estimation
    * O(N^3+M*N^2) complexity
    * matrix is represented by its upper or lower triangle

    No iterative refinement is provided because such partial representation of
    matrix does not allow efficient calculation of extra-precise  matrix-vector
    products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
    need iterative refinement.

    IMPORTANT: ! this function is NOT the most efficient linear solver provided
               ! by ALGLIB. It estimates condition  number  of  linear system,
               ! which  results  in  significant  performance   penalty   when
               ! compared with "fast"  version  which  just  calls  triangular
               ! solver.
               !
               ! This performance penalty is especially apparent when you  use
               ! ALGLIB parallel capabilities (condition number estimation  is
               ! inherently  sequential).  It   also   becomes significant for
               ! small-scale problems (N<100).
               !
               ! In such cases we strongly recommend you to use faster solver,
               ! HPDMatrixSolveMFast() function.

      ! COMMERCIAL EDITION OF ALGLIB:
      !
      ! Commercial Edition of ALGLIB includes following important improvements
      ! of this function:
      ! * high-performance native backend with same C# interface (C# version)
      ! * multithreading support (C++ and C# versions)
      ! * hardware vendor (Intel) implementations of linear algebra primitives
      !   (C++ and C# versions, x86/x64 platform)
      !
      ! We recommend you to read 'Working with commercial version' section  of
      ! ALGLIB Reference Manual in order to find out how to  use  performance-
      ! related features provided by commercial edition of ALGLIB.

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        N       -   size of A
        IsUpper -   what half of A is provided
        B       -   array[0..N-1,0..M-1], right part
        M       -   right part size

    OUTPUT PARAMETERS
        Info    -   same as in RMatrixSolve.
                    Returns -3 for non-HPD matrices.
        Rep     -   same as in RMatrixSolve
        X       -   same as in RMatrixSolve

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void hpdmatrixsolvem(complex[,] a, int n, bool isupper, complex[,] b, int m, out int info, out densesolverreport rep, out complex[,] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0,0];
        directdensesolvers.hpdmatrixsolvem(a, n, isupper, b, m, ref info, rep.innerobj, ref x, null);
    }
    
    public static void hpdmatrixsolvem(complex[,] a, int n, bool isupper, complex[,] b, int m, out int info, out densesolverreport rep, out complex[,] x, alglib.xparams _params)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0,0];
        directdensesolvers.hpdmatrixsolvem(a, n, isupper, b, m, ref info, rep.innerobj, ref x, _params);
    }
    
    /*************************************************************************
    Dense solver for A*X=B, with N*N Hermitian positive definite matrix A  and
    N*M complex matrices X and B. "Fast-but-lightweight" version of the solver.

    Algorithm features:
    * O(N^3+M*N^2) complexity
    * matrix is represented by its upper or lower triangle
    * no additional time consuming features like condition number estimation

      ! COMMERCIAL EDITION OF ALGLIB:
      !
      ! Commercial Edition of ALGLIB includes following important improvements
      ! of this function:
      ! * high-performance native backend with same C# interface (C# version)
      ! * multithreading support (C++ and C# versions)
      ! * hardware vendor (Intel) implementations of linear algebra primitives
      !   (C++ and C# versions, x86/x64 platform)
      !
      ! We recommend you to read 'Working with commercial version' section  of
      ! ALGLIB Reference Manual in order to find out how to  use  performance-
      ! related features provided by commercial edition of ALGLIB.

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        N       -   size of A
        IsUpper -   what half of A is provided
        B       -   array[0..N-1,0..M-1], right part
        M       -   right part size

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    A is is exactly  singular or is not positive definite.
                            B is filled by zeros in such cases.
                    * -1    N<=0 was passed
                    *  1    task is solved
        B       -   array[0..N-1]:
                    * overwritten by solution
                    * zeros, if problem was not solved

      -- ALGLIB --
         Copyright 17.03.2015 by Bochkanov Sergey
    *************************************************************************/
    public static void hpdmatrixsolvemfast(complex[,] a, int n, bool isupper, ref complex[,] b, int m, out int info)
    {
        info = 0;
        directdensesolvers.hpdmatrixsolvemfast(a, n, isupper, b, m, ref info, null);
    }
    
    public static void hpdmatrixsolvemfast(complex[,] a, int n, bool isupper, ref complex[,] b, int m, out int info, alglib.xparams _params)
    {
        info = 0;
        directdensesolvers.hpdmatrixsolvemfast(a, n, isupper, b, m, ref info, _params);
    }
    
    /*************************************************************************
    Dense solver for A*x=b, with N*N Hermitian positive definite matrix A, and
    N*1 complex vectors  x  and  b.  "Slow-but-feature-rich"  version  of  the
    solver.

    Algorithm features:
    * automatic detection of degenerate cases
    * condition number estimation
    * O(N^3) complexity
    * matrix is represented by its upper or lower triangle

    No iterative refinement is provided because such partial representation of
    matrix does not allow efficient calculation of extra-precise  matrix-vector
    products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
    need iterative refinement.

    IMPORTANT: ! this function is NOT the most efficient linear solver provided
               ! by ALGLIB. It estimates condition  number  of  linear system,
               ! which  results  in  significant   performance   penalty  when
               ! compared with "fast" version  which  just  performs  Cholesky
               ! decomposition and calls triangular solver.
               !
               ! This  performance  penalty  is  especially  visible  in   the
               ! multithreaded mode, because both condition number  estimation
               ! and   iterative    refinement   are   inherently   sequential
               ! calculations.
               !
               ! Thus, if you need high performance and if you are pretty sure
               ! that your system is well conditioned, we  strongly  recommend
               ! you to use faster solver, HPDMatrixSolveFast() function.

      ! COMMERCIAL EDITION OF ALGLIB:
      !
      ! Commercial Edition of ALGLIB includes following important improvements
      ! of this function:
      ! * high-performance native backend with same C# interface (C# version)
      ! * multithreading support (C++ and C# versions)
      ! * hardware vendor (Intel) implementations of linear algebra primitives
      !   (C++ and C# versions, x86/x64 platform)
      !
      ! We recommend you to read 'Working with commercial version' section  of
      ! ALGLIB Reference Manual in order to find out how to  use  performance-
      ! related features provided by commercial edition of ALGLIB.

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        N       -   size of A
        IsUpper -   what half of A is provided
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS
        Info    -   same as in RMatrixSolve
                    Returns -3 for non-HPD matrices.
        Rep     -   same as in RMatrixSolve
        X       -   same as in RMatrixSolve

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void hpdmatrixsolve(complex[,] a, int n, bool isupper, complex[] b, out int info, out densesolverreport rep, out complex[] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0];
        directdensesolvers.hpdmatrixsolve(a, n, isupper, b, ref info, rep.innerobj, ref x, null);
    }
    
    public static void hpdmatrixsolve(complex[,] a, int n, bool isupper, complex[] b, out int info, out densesolverreport rep, out complex[] x, alglib.xparams _params)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0];
        directdensesolvers.hpdmatrixsolve(a, n, isupper, b, ref info, rep.innerobj, ref x, _params);
    }
    
    /*************************************************************************
    Dense solver for A*x=b, with N*N Hermitian positive definite matrix A, and
    N*1 complex vectors  x  and  b.  "Fast-but-lightweight"  version  of   the
    solver without additional functions.

    Algorithm features:
    * O(N^3) complexity
    * matrix is represented by its upper or lower triangle
    * no additional time consuming functions

      ! COMMERCIAL EDITION OF ALGLIB:
      !
      ! Commercial Edition of ALGLIB includes following important improvements
      ! of this function:
      ! * high-performance native backend with same C# interface (C# version)
      ! * multithreading support (C++ and C# versions)
      ! * hardware vendor (Intel) implementations of linear algebra primitives
      !   (C++ and C# versions, x86/x64 platform)
      !
      ! We recommend you to read 'Working with commercial version' section  of
      ! ALGLIB Reference Manual in order to find out how to  use  performance-
      ! related features provided by commercial edition of ALGLIB.

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        N       -   size of A
        IsUpper -   what half of A is provided
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    A is is exactly singular or not positive definite
                            X is filled by zeros in such cases.
                    * -1    N<=0 was passed
                    *  1    task was solved
        B       -   array[0..N-1]:
                    * overwritten by solution
                    * zeros, if A is exactly singular (diagonal of its LU
                      decomposition has exact zeros).

      -- ALGLIB --
         Copyright 17.03.2015 by Bochkanov Sergey
    *************************************************************************/
    public static void hpdmatrixsolvefast(complex[,] a, int n, bool isupper, ref complex[] b, out int info)
    {
        info = 0;
        directdensesolvers.hpdmatrixsolvefast(a, n, isupper, b, ref info, null);
    }
    
    public static void hpdmatrixsolvefast(complex[,] a, int n, bool isupper, ref complex[] b, out int info, alglib.xparams _params)
    {
        info = 0;
        directdensesolvers.hpdmatrixsolvefast(a, n, isupper, b, ref info, _params);
    }
    
    /*************************************************************************
    Dense solver for A*X=B with N*N Hermitian positive definite matrix A given
    by its Cholesky decomposition and N*M complex matrices X  and  B.  This is
    "slow-but-feature-rich" version of the solver which, in  addition  to  the
    solution, estimates condition number of the system.

    Algorithm features:
    * automatic detection of degenerate cases
    * O(M*N^2) complexity
    * condition number estimation
    * matrix is represented by its upper or lower triangle

    No iterative refinement is provided because such partial representation of
    matrix does not allow efficient calculation of extra-precise  matrix-vector
    products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
    need iterative refinement.

    IMPORTANT: ! this function is NOT the most efficient linear solver provided
               ! by ALGLIB. It estimates condition  number  of  linear system,
               ! which  results  in  significant  performance   penalty   when
               ! compared with "fast"  version  which  just  calls  triangular
               ! solver. Amount of  overhead  introduced  depends  on  M  (the
               ! larger - the more efficient).
               !
               ! This performance penalty is insignificant  when compared with
               ! cost of large Cholesky decomposition.  However,  if  you call
               ! this  function  many  times  for  the same  left  side,  this
               ! overhead BECOMES significant. It  also   becomes  significant
               ! for small-scale problems (N<50).
               !
               ! In such cases we strongly recommend you to use faster solver,
               ! HPDMatrixCholeskySolveMFast() function.


    INPUT PARAMETERS
        CHA     -   array[N,N], Cholesky decomposition,
                    HPDMatrixCholesky result
        N       -   size of CHA
        IsUpper -   what half of CHA is provided
        B       -   array[N,M], right part
        M       -   right part size

    OUTPUT PARAMETERS:
        Info    -   return code:
                    * -3    A is singular, or VERY close to singular.
                            X is filled by zeros in such cases.
                    * -1    N<=0 was passed
                    *  1    task was solved
        Rep     -   additional report, following fields are set:
                    * rep.r1    condition number in 1-norm
                    * rep.rinf  condition number in inf-norm
        X       -   array[N]:
                    * for info>0 contains solution
                    * for info=-3 filled by zeros

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void hpdmatrixcholeskysolvem(complex[,] cha, int n, bool isupper, complex[,] b, int m, out int info, out densesolverreport rep, out complex[,] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0,0];
        directdensesolvers.hpdmatrixcholeskysolvem(cha, n, isupper, b, m, ref info, rep.innerobj, ref x, null);
    }
    
    public static void hpdmatrixcholeskysolvem(complex[,] cha, int n, bool isupper, complex[,] b, int m, out int info, out densesolverreport rep, out complex[,] x, alglib.xparams _params)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0,0];
        directdensesolvers.hpdmatrixcholeskysolvem(cha, n, isupper, b, m, ref info, rep.innerobj, ref x, _params);
    }
    
    /*************************************************************************
    Dense solver for A*X=B with N*N Hermitian positive definite matrix A given
    by its Cholesky decomposition and N*M complex matrices X  and  B.  This is
    "fast-but-lightweight" version of the solver.

    Algorithm features:
    * O(M*N^2) complexity
    * matrix is represented by its upper or lower triangle
    * no additional time-consuming features

    INPUT PARAMETERS
        CHA     -   array[N,N], Cholesky decomposition,
                    HPDMatrixCholesky result
        N       -   size of CHA
        IsUpper -   what half of CHA is provided
        B       -   array[N,M], right part
        M       -   right part size

    OUTPUT PARAMETERS:
        Info    -   return code:
                    * -3    A is singular, or VERY close to singular.
                            X is filled by zeros in such cases.
                    * -1    N<=0 was passed
                    *  1    task was solved
        B       -   array[N]:
                    * for info>0 overwritten by solution
                    * for info=-3 filled by zeros

      -- ALGLIB --
         Copyright 18.03.2015 by Bochkanov Sergey
    *************************************************************************/
    public static void hpdmatrixcholeskysolvemfast(complex[,] cha, int n, bool isupper, ref complex[,] b, int m, out int info)
    {
        info = 0;
        directdensesolvers.hpdmatrixcholeskysolvemfast(cha, n, isupper, b, m, ref info, null);
    }
    
    public static void hpdmatrixcholeskysolvemfast(complex[,] cha, int n, bool isupper, ref complex[,] b, int m, out int info, alglib.xparams _params)
    {
        info = 0;
        directdensesolvers.hpdmatrixcholeskysolvemfast(cha, n, isupper, b, m, ref info, _params);
    }
    
    /*************************************************************************
    Dense solver for A*x=b with N*N Hermitian positive definite matrix A given
    by its Cholesky decomposition, and N*1 complex vectors x and  b.  This  is
    "slow-but-feature-rich" version of the solver  which  estimates  condition
    number of the system.

    Algorithm features:
    * automatic detection of degenerate cases
    * O(N^2) complexity
    * condition number estimation
    * matrix is represented by its upper or lower triangle

    No iterative refinement is provided because such partial representation of
    matrix does not allow efficient calculation of extra-precise  matrix-vector
    products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
    need iterative refinement.

    IMPORTANT: ! this function is NOT the most efficient linear solver provided
               ! by ALGLIB. It estimates condition  number  of  linear system,
               ! which results in 10-15x  performance  penalty  when  compared
               ! with "fast" version which just calls triangular solver.
               !
               ! This performance penalty is insignificant  when compared with
               ! cost of large LU decomposition.  However,  if you  call  this
               ! function many times for the same  left  side,  this  overhead
               ! BECOMES significant. It  also  becomes significant for small-
               ! scale problems (N<50).
               !
               ! In such cases we strongly recommend you to use faster solver,
               ! HPDMatrixCholeskySolveFast() function.

    INPUT PARAMETERS
        CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
                    SPDMatrixCholesky result
        N       -   size of A
        IsUpper -   what half of CHA is provided
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    A is is exactly singular or ill conditioned
                            X is filled by zeros in such cases.
                    * -1    N<=0 was passed
                    *  1    task is solved
        Rep     -   additional report, following fields are set:
                    * rep.r1    condition number in 1-norm
                    * rep.rinf  condition number in inf-norm
        X       -   array[N]:
                    * for info>0  - solution
                    * for info=-3 - filled by zeros

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void hpdmatrixcholeskysolve(complex[,] cha, int n, bool isupper, complex[] b, out int info, out densesolverreport rep, out complex[] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0];
        directdensesolvers.hpdmatrixcholeskysolve(cha, n, isupper, b, ref info, rep.innerobj, ref x, null);
    }
    
    public static void hpdmatrixcholeskysolve(complex[,] cha, int n, bool isupper, complex[] b, out int info, out densesolverreport rep, out complex[] x, alglib.xparams _params)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0];
        directdensesolvers.hpdmatrixcholeskysolve(cha, n, isupper, b, ref info, rep.innerobj, ref x, _params);
    }
    
    /*************************************************************************
    Dense solver for A*x=b with N*N Hermitian positive definite matrix A given
    by its Cholesky decomposition, and N*1 complex vectors x and  b.  This  is
    "fast-but-lightweight" version of the solver.

    Algorithm features:
    * O(N^2) complexity
    * matrix is represented by its upper or lower triangle
    * no additional time-consuming features

    INPUT PARAMETERS
        CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
                    SPDMatrixCholesky result
        N       -   size of A
        IsUpper -   what half of CHA is provided
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    A is is exactly singular or ill conditioned
                            B is filled by zeros in such cases.
                    * -1    N<=0 was passed
                    *  1    task is solved
        B       -   array[N]:
                    * for info>0  - overwritten by solution
                    * for info=-3 - filled by zeros

      -- ALGLIB --
         Copyright 18.03.2015 by Bochkanov Sergey
    *************************************************************************/
    public static void hpdmatrixcholeskysolvefast(complex[,] cha, int n, bool isupper, ref complex[] b, out int info)
    {
        info = 0;
        directdensesolvers.hpdmatrixcholeskysolvefast(cha, n, isupper, b, ref info, null);
    }
    
    public static void hpdmatrixcholeskysolvefast(complex[,] cha, int n, bool isupper, ref complex[] b, out int info, alglib.xparams _params)
    {
        info = 0;
        directdensesolvers.hpdmatrixcholeskysolvefast(cha, n, isupper, b, ref info, _params);
    }
    
    /*************************************************************************
    Dense solver.

    This subroutine finds solution of the linear system A*X=B with non-square,
    possibly degenerate A.  System  is  solved in the least squares sense, and
    general least squares solution  X = X0 + CX*y  which  minimizes |A*X-B| is
    returned. If A is non-degenerate, solution in the usual sense is returned.

    Algorithm features:
    * automatic detection (and correct handling!) of degenerate cases
    * iterative refinement
    * O(N^3) complexity

      ! COMMERCIAL EDITION OF ALGLIB:
      !
      ! Commercial Edition of ALGLIB includes following important improvements
      ! of this function:
      ! * high-performance native backend with same C# interface (C# version)
      ! * multithreading support (C++ and C# versions)
      ! * hardware vendor (Intel) implementations of linear algebra primitives
      !   (C++ and C# versions, x86/x64 platform)
      !
      ! We recommend you to read 'Working with commercial version' section  of
      ! ALGLIB Reference Manual in order to find out how to  use  performance-
      ! related features provided by commercial edition of ALGLIB.

    INPUT PARAMETERS
        A       -   array[0..NRows-1,0..NCols-1], system matrix
        NRows   -   vertical size of A
        NCols   -   horizontal size of A
        B       -   array[0..NCols-1], right part
        Threshold-  a number in [0,1]. Singular values  beyond  Threshold  are
                    considered  zero.  Set  it to 0.0, if you don't understand
                    what it means, so the solver will choose good value on its
                    own.

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -4    SVD subroutine failed
                    * -1    if NRows<=0 or NCols<=0 or Threshold<0 was passed
                    *  1    if task is solved
        Rep     -   solver report, see below for more info
        X       -   array[0..N-1,0..M-1], it contains:
                    * solution of A*X=B (even for singular A)
                    * zeros, if SVD subroutine failed

    SOLVER REPORT

    Subroutine sets following fields of the Rep structure:
    * R2        reciprocal of condition number: 1/cond(A), 2-norm.
    * N         = NCols
    * K         dim(Null(A))
    * CX        array[0..N-1,0..K-1], kernel of A.
                Columns of CX store such vectors that A*CX[i]=0.

      -- ALGLIB --
         Copyright 24.08.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void rmatrixsolvels(double[,] a, int nrows, int ncols, double[] b, double threshold, out int info, out densesolverlsreport rep, out double[] x)
    {
        info = 0;
        rep = new densesolverlsreport();
        x = new double[0];
        directdensesolvers.rmatrixsolvels(a, nrows, ncols, b, threshold, ref info, rep.innerobj, ref x, null);
    }
    
    public static void rmatrixsolvels(double[,] a, int nrows, int ncols, double[] b, double threshold, out int info, out densesolverlsreport rep, out double[] x, alglib.xparams _params)
    {
        info = 0;
        rep = new densesolverlsreport();
        x = new double[0];
        directdensesolvers.rmatrixsolvels(a, nrows, ncols, b, threshold, ref info, rep.innerobj, ref x, _params);
    }

}
public partial class alglib
{


    /*************************************************************************
    This object stores state of the LinLSQR method.

    You should use ALGLIB functions to work with this object.
    *************************************************************************/
    public class linlsqrstate : alglibobject
    {
        //
        // Public declarations
        //
    
        public linlsqrstate()
        {
            _innerobj = new linlsqr.linlsqrstate();
        }
        
        public override alglib.alglibobject make_copy()
        {
            return new linlsqrstate((linlsqr.linlsqrstate)_innerobj.make_copy());
        }
    
        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private linlsqr.linlsqrstate _innerobj;
        public linlsqr.linlsqrstate innerobj { get { return _innerobj; } }
        public linlsqrstate(linlsqr.linlsqrstate obj)
        {
            _innerobj = obj;
        }
    }


    /*************************************************************************

    *************************************************************************/
    public class linlsqrreport : alglibobject
    {
        //
        // Public declarations
        //
        public int iterationscount { get { return _innerobj.iterationscount; } set { _innerobj.iterationscount = value; } }
        public int nmv { get { return _innerobj.nmv; } set { _innerobj.nmv = value; } }
        public int terminationtype { get { return _innerobj.terminationtype; } set { _innerobj.terminationtype = value; } }
    
        public linlsqrreport()
        {
            _innerobj = new linlsqr.linlsqrreport();
        }
        
        public override alglib.alglibobject make_copy()
        {
            return new linlsqrreport((linlsqr.linlsqrreport)_innerobj.make_copy());
        }
    
        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private linlsqr.linlsqrreport _innerobj;
        public linlsqr.linlsqrreport innerobj { get { return _innerobj; } }
        public linlsqrreport(linlsqr.linlsqrreport obj)
        {
            _innerobj = obj;
        }
    }
    
    /*************************************************************************
    This function initializes linear LSQR Solver. This solver is used to solve
    non-symmetric (and, possibly, non-square) problems. Least squares solution
    is returned for non-compatible systems.

    USAGE:
    1. User initializes algorithm state with LinLSQRCreate() call
    2. User tunes solver parameters with  LinLSQRSetCond() and other functions
    3. User  calls  LinLSQRSolveSparse()  function which takes algorithm state
       and SparseMatrix object.
    4. User calls LinLSQRResults() to get solution
    5. Optionally, user may call LinLSQRSolveSparse() again to  solve  another
       problem  with different matrix and/or right part without reinitializing
       LinLSQRState structure.

    INPUT PARAMETERS:
        M       -   number of rows in A
        N       -   number of variables, N>0

    OUTPUT PARAMETERS:
        State   -   structure which stores algorithm state

    NOTE: see also linlsqrcreatebuf()  for  version  which  reuses  previously
          allocated place as much as possible.

      -- ALGLIB --
         Copyright 30.11.2011 by Bochkanov Sergey
    *************************************************************************/
    public static void linlsqrcreate(int m, int n, out linlsqrstate state)
    {
        state = new linlsqrstate();
        linlsqr.linlsqrcreate(m, n, state.innerobj, null);
    }
    
    public static void linlsqrcreate(int m, int n, out linlsqrstate state, alglib.xparams _params)
    {
        state = new linlsqrstate();
        linlsqr.linlsqrcreate(m, n, state.innerobj, _params);
    }
    
    /*************************************************************************
    This function initializes linear LSQR Solver.  It  provides  exactly  same
    functionality as linlsqrcreate(), but reuses  previously  allocated  space
    as much as possible.

    INPUT PARAMETERS:
        M       -   number of rows in A
        N       -   number of variables, N>0

    OUTPUT PARAMETERS:
        State   -   structure which stores algorithm state

      -- ALGLIB --
         Copyright 14.11.2018 by Bochkanov Sergey
    *************************************************************************/
    public static void linlsqrcreatebuf(int m, int n, linlsqrstate state)
    {
    
        linlsqr.linlsqrcreatebuf(m, n, state.innerobj, null);
    }
    
    public static void linlsqrcreatebuf(int m, int n, linlsqrstate state, alglib.xparams _params)
    {
    
        linlsqr.linlsqrcreatebuf(m, n, state.innerobj, _params);
    }
    
    /*************************************************************************
    This  function  changes  preconditioning  settings of LinLSQQSolveSparse()
    function. By default, SolveSparse() uses diagonal preconditioner,  but  if
    you want to use solver without preconditioning, you can call this function
    which forces solver to use unit matrix for preconditioning.

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state

      -- ALGLIB --
         Copyright 19.11.2012 by Bochkanov Sergey
    *************************************************************************/
    public static void linlsqrsetprecunit(linlsqrstate state)
    {
    
        linlsqr.linlsqrsetprecunit(state.innerobj, null);
    }
    
    public static void linlsqrsetprecunit(linlsqrstate state, alglib.xparams _params)
    {
    
        linlsqr.linlsqrsetprecunit(state.innerobj, _params);
    }
    
    /*************************************************************************
    This  function  changes  preconditioning  settings  of  LinCGSolveSparse()
    function.  LinCGSolveSparse() will use diagonal of the  system  matrix  as
    preconditioner. This preconditioning mode is active by default.

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state

      -- ALGLIB --
         Copyright 19.11.2012 by Bochkanov Sergey
    *************************************************************************/
    public static void linlsqrsetprecdiag(linlsqrstate state)
    {
    
        linlsqr.linlsqrsetprecdiag(state.innerobj, null);
    }
    
    public static void linlsqrsetprecdiag(linlsqrstate state, alglib.xparams _params)
    {
    
        linlsqr.linlsqrsetprecdiag(state.innerobj, _params);
    }
    
    /*************************************************************************
    This function sets optional Tikhonov regularization coefficient.
    It is zero by default.

    INPUT PARAMETERS:
        LambdaI -   regularization factor, LambdaI>=0

    OUTPUT PARAMETERS:
        State   -   structure which stores algorithm state

      -- ALGLIB --
         Copyright 30.11.2011 by Bochkanov Sergey
    *************************************************************************/
    public static void linlsqrsetlambdai(linlsqrstate state, double lambdai)
    {
    
        linlsqr.linlsqrsetlambdai(state.innerobj, lambdai, null);
    }
    
    public static void linlsqrsetlambdai(linlsqrstate state, double lambdai, alglib.xparams _params)
    {
    
        linlsqr.linlsqrsetlambdai(state.innerobj, lambdai, _params);
    }
    
    /*************************************************************************
    Procedure for solution of A*x=b with sparse A.

    INPUT PARAMETERS:
        State   -   algorithm state
        A       -   sparse M*N matrix in the CRS format (you MUST contvert  it
                    to CRS format  by  calling  SparseConvertToCRS()  function
                    BEFORE you pass it to this function).
        B       -   right part, array[M]

    RESULT:
        This function returns no result.
        You can get solution by calling LinCGResults()

    NOTE: this function uses lightweight preconditioning -  multiplication  by
          inverse of diag(A). If you want, you can turn preconditioning off by
          calling LinLSQRSetPrecUnit(). However, preconditioning cost is   low
          and preconditioner is very important for solution  of  badly  scaled
          problems.

      -- ALGLIB --
         Copyright 30.11.2011 by Bochkanov Sergey
    *************************************************************************/
    public static void linlsqrsolvesparse(linlsqrstate state, sparsematrix a, double[] b)
    {
    
        linlsqr.linlsqrsolvesparse(state.innerobj, a.innerobj, b, null);
    }
    
    public static void linlsqrsolvesparse(linlsqrstate state, sparsematrix a, double[] b, alglib.xparams _params)
    {
    
        linlsqr.linlsqrsolvesparse(state.innerobj, a.innerobj, b, _params);
    }
    
    /*************************************************************************
    This function sets stopping criteria.

    INPUT PARAMETERS:
        EpsA    -   algorithm will be stopped if ||A^T*Rk||/(||A||*||Rk||)<=EpsA.
        EpsB    -   algorithm will be stopped if ||Rk||<=EpsB*||B||
        MaxIts  -   algorithm will be stopped if number of iterations
                    more than MaxIts.

    OUTPUT PARAMETERS:
        State   -   structure which stores algorithm state

    NOTE: if EpsA,EpsB,EpsC and MaxIts are zero then these variables will
    be setted as default values.

      -- ALGLIB --
         Copyright 30.11.2011 by Bochkanov Sergey
    *************************************************************************/
    public static void linlsqrsetcond(linlsqrstate state, double epsa, double epsb, int maxits)
    {
    
        linlsqr.linlsqrsetcond(state.innerobj, epsa, epsb, maxits, null);
    }
    
    public static void linlsqrsetcond(linlsqrstate state, double epsa, double epsb, int maxits, alglib.xparams _params)
    {
    
        linlsqr.linlsqrsetcond(state.innerobj, epsa, epsb, maxits, _params);
    }
    
    /*************************************************************************
    LSQR solver: results.

    This function must be called after LinLSQRSolve

    INPUT PARAMETERS:
        State   -   algorithm state

    OUTPUT PARAMETERS:
        X       -   array[N], solution
        Rep     -   optimization report:
                    * Rep.TerminationType completetion code:
                        *  1    ||Rk||<=EpsB*||B||
                        *  4    ||A^T*Rk||/(||A||*||Rk||)<=EpsA
                        *  5    MaxIts steps was taken
                        *  7    rounding errors prevent further progress,
                                X contains best point found so far.
                                (sometimes returned on singular systems)
                    * Rep.IterationsCount contains iterations count
                    * NMV countains number of matrix-vector calculations

      -- ALGLIB --
         Copyright 30.11.2011 by Bochkanov Sergey
    *************************************************************************/
    public static void linlsqrresults(linlsqrstate state, out double[] x, out linlsqrreport rep)
    {
        x = new double[0];
        rep = new linlsqrreport();
        linlsqr.linlsqrresults(state.innerobj, ref x, rep.innerobj, null);
    }
    
    public static void linlsqrresults(linlsqrstate state, out double[] x, out linlsqrreport rep, alglib.xparams _params)
    {
        x = new double[0];
        rep = new linlsqrreport();
        linlsqr.linlsqrresults(state.innerobj, ref x, rep.innerobj, _params);
    }
    
    /*************************************************************************
    This function turns on/off reporting.

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state
        NeedXRep-   whether iteration reports are needed or not

    If NeedXRep is True, algorithm will call rep() callback function if  it is
    provided to MinCGOptimize().

      -- ALGLIB --
         Copyright 30.11.2011 by Bochkanov Sergey
    *************************************************************************/
    public static void linlsqrsetxrep(linlsqrstate state, bool needxrep)
    {
    
        linlsqr.linlsqrsetxrep(state.innerobj, needxrep, null);
    }
    
    public static void linlsqrsetxrep(linlsqrstate state, bool needxrep, alglib.xparams _params)
    {
    
        linlsqr.linlsqrsetxrep(state.innerobj, needxrep, _params);
    }

}
public partial class alglib
{


    /*************************************************************************

    *************************************************************************/
    public class polynomialsolverreport : alglibobject
    {
        //
        // Public declarations
        //
        public double maxerr { get { return _innerobj.maxerr; } set { _innerobj.maxerr = value; } }
    
        public polynomialsolverreport()
        {
            _innerobj = new polynomialsolver.polynomialsolverreport();
        }
        
        public override alglib.alglibobject make_copy()
        {
            return new polynomialsolverreport((polynomialsolver.polynomialsolverreport)_innerobj.make_copy());
        }
    
        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private polynomialsolver.polynomialsolverreport _innerobj;
        public polynomialsolver.polynomialsolverreport innerobj { get { return _innerobj; } }
        public polynomialsolverreport(polynomialsolver.polynomialsolverreport obj)
        {
            _innerobj = obj;
        }
    }
    
    /*************************************************************************
    Polynomial root finding.

    This function returns all roots of the polynomial
        P(x) = a0 + a1*x + a2*x^2 + ... + an*x^n
    Both real and complex roots are returned (see below).

    INPUT PARAMETERS:
        A       -   array[N+1], polynomial coefficients:
                    * A[0] is constant term
                    * A[N] is a coefficient of X^N
        N       -   polynomial degree

    OUTPUT PARAMETERS:
        X       -   array of complex roots:
                    * for isolated real root, X[I] is strictly real: IMAGE(X[I])=0
                    * complex roots are always returned in pairs - roots occupy
                      positions I and I+1, with:
                      * X[I+1]=Conj(X[I])
                      * IMAGE(X[I]) > 0
                      * IMAGE(X[I+1]) = -IMAGE(X[I]) < 0
                    * multiple real roots may have non-zero imaginary part due
                      to roundoff errors. There is no reliable way to distinguish
                      real root of multiplicity 2 from two  complex  roots  in
                      the presence of roundoff errors.
        Rep     -   report, additional information, following fields are set:
                    * Rep.MaxErr - max( |P(xi)| )  for  i=0..N-1.  This  field
                      allows to quickly estimate "quality" of the roots  being
                      returned.

    NOTE:   this function uses companion matrix method to find roots. In  case
            internal EVD  solver  fails  do  find  eigenvalues,  exception  is
            generated.

    NOTE:   roots are not "polished" and  no  matrix  balancing  is  performed
            for them.

      -- ALGLIB --
         Copyright 24.02.2014 by Bochkanov Sergey
    *************************************************************************/
    public static void polynomialsolve(double[] a, int n, out complex[] x, out polynomialsolverreport rep)
    {
        x = new complex[0];
        rep = new polynomialsolverreport();
        polynomialsolver.polynomialsolve(a, n, ref x, rep.innerobj, null);
    }
    
    public static void polynomialsolve(double[] a, int n, out complex[] x, out polynomialsolverreport rep, alglib.xparams _params)
    {
        x = new complex[0];
        rep = new polynomialsolverreport();
        polynomialsolver.polynomialsolve(a, n, ref x, rep.innerobj, _params);
    }

}
public partial class alglib
{


    /*************************************************************************

    *************************************************************************/
    public class nleqstate : alglibobject
    {
        //
        // Public declarations
        //
        public bool needf { get { return _innerobj.needf; } set { _innerobj.needf = value; } }
        public bool needfij { get { return _innerobj.needfij; } set { _innerobj.needfij = value; } }
        public bool xupdated { get { return _innerobj.xupdated; } set { _innerobj.xupdated = value; } }
        public double f { get { return _innerobj.f; } set { _innerobj.f = value; } }
        public double[] fi { get { return _innerobj.fi; } }
        public double[,] j { get { return _innerobj.j; } }
        public double[] x { get { return _innerobj.x; } }
    
        public nleqstate()
        {
            _innerobj = new nleq.nleqstate();
        }
        
        public override alglib.alglibobject make_copy()
        {
            return new nleqstate((nleq.nleqstate)_innerobj.make_copy());
        }
    
        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private nleq.nleqstate _innerobj;
        public nleq.nleqstate innerobj { get { return _innerobj; } }
        public nleqstate(nleq.nleqstate obj)
        {
            _innerobj = obj;
        }
    }


    /*************************************************************************

    *************************************************************************/
    public class nleqreport : alglibobject
    {
        //
        // Public declarations
        //
        public int iterationscount { get { return _innerobj.iterationscount; } set { _innerobj.iterationscount = value; } }
        public int nfunc { get { return _innerobj.nfunc; } set { _innerobj.nfunc = value; } }
        public int njac { get { return _innerobj.njac; } set { _innerobj.njac = value; } }
        public int terminationtype { get { return _innerobj.terminationtype; } set { _innerobj.terminationtype = value; } }
    
        public nleqreport()
        {
            _innerobj = new nleq.nleqreport();
        }
        
        public override alglib.alglibobject make_copy()
        {
            return new nleqreport((nleq.nleqreport)_innerobj.make_copy());
        }
    
        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private nleq.nleqreport _innerobj;
        public nleq.nleqreport innerobj { get { return _innerobj; } }
        public nleqreport(nleq.nleqreport obj)
        {
            _innerobj = obj;
        }
    }
    
    /*************************************************************************
                    LEVENBERG-MARQUARDT-LIKE NONLINEAR SOLVER

    DESCRIPTION:
    This algorithm solves system of nonlinear equations
        F[0](x[0], ..., x[n-1])   = 0
        F[1](x[0], ..., x[n-1])   = 0
        ...
        F[M-1](x[0], ..., x[n-1]) = 0
    with M/N do not necessarily coincide.  Algorithm  converges  quadratically
    under following conditions:
        * the solution set XS is nonempty
        * for some xs in XS there exist such neighbourhood N(xs) that:
          * vector function F(x) and its Jacobian J(x) are continuously
            differentiable on N
          * ||F(x)|| provides local error bound on N, i.e. there  exists  such
            c1, that ||F(x)||>c1*distance(x,XS)
    Note that these conditions are much more weaker than usual non-singularity
    conditions. For example, algorithm will converge for any  affine  function
    F (whether its Jacobian singular or not).


    REQUIREMENTS:
    Algorithm will request following information during its operation:
    * function vector F[] and Jacobian matrix at given point X
    * value of merit function f(x)=F[0]^2(x)+...+F[M-1]^2(x) at given point X


    USAGE:
    1. User initializes algorithm state with NLEQCreateLM() call
    2. User tunes solver parameters with  NLEQSetCond(),  NLEQSetStpMax()  and
       other functions
    3. User  calls  NLEQSolve()  function  which  takes  algorithm  state  and
       pointers (delegates, etc.) to callback functions which calculate  merit
       function value and Jacobian.
    4. User calls NLEQResults() to get solution
    5. Optionally, user may call NLEQRestartFrom() to  solve  another  problem
       with same parameters (N/M) but another starting  point  and/or  another
       function vector. NLEQRestartFrom() allows to reuse already  initialized
       structure.


    INPUT PARAMETERS:
        N       -   space dimension, N>1:
                    * if provided, only leading N elements of X are used
                    * if not provided, determined automatically from size of X
        M       -   system size
        X       -   starting point


    OUTPUT PARAMETERS:
        State   -   structure which stores algorithm state


    NOTES:
    1. you may tune stopping conditions with NLEQSetCond() function
    2. if target function contains exp() or other fast growing functions,  and
       optimization algorithm makes too large steps which leads  to  overflow,
       use NLEQSetStpMax() function to bound algorithm's steps.
    3. this  algorithm  is  a  slightly  modified implementation of the method
       described  in  'Levenberg-Marquardt  method  for constrained  nonlinear
       equations with strong local convergence properties' by Christian Kanzow
       Nobuo Yamashita and Masao Fukushima and further  developed  in  'On the
       convergence of a New Levenberg-Marquardt Method'  by  Jin-yan  Fan  and
       Ya-Xiang Yuan.


      -- ALGLIB --
         Copyright 20.08.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void nleqcreatelm(int n, int m, double[] x, out nleqstate state)
    {
        state = new nleqstate();
        nleq.nleqcreatelm(n, m, x, state.innerobj, null);
    }
    
    public static void nleqcreatelm(int n, int m, double[] x, out nleqstate state, alglib.xparams _params)
    {
        state = new nleqstate();
        nleq.nleqcreatelm(n, m, x, state.innerobj, _params);
    }
            
    public static void nleqcreatelm(int m, double[] x, out nleqstate state)
    {
        int n;
    
        state = new nleqstate();
        n = ap.len(x);
        nleq.nleqcreatelm(n, m, x, state.innerobj, null);
    
        return;
    }
            
    public static void nleqcreatelm(int m, double[] x, out nleqstate state, alglib.xparams _params)
    {
        int n;
    
        state = new nleqstate();
        n = ap.len(x);
        nleq.nleqcreatelm(n, m, x, state.innerobj, _params);
    
        return;
    }
    
    /*************************************************************************
    This function sets stopping conditions for the nonlinear solver

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state
        EpsF    -   >=0
                    The subroutine finishes  its work if on k+1-th iteration
                    the condition ||F||<=EpsF is satisfied
        MaxIts  -   maximum number of iterations. If MaxIts=0, the  number  of
                    iterations is unlimited.

    Passing EpsF=0 and MaxIts=0 simultaneously will lead to  automatic
    stopping criterion selection (small EpsF).

    NOTES:

      -- ALGLIB --
         Copyright 20.08.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void nleqsetcond(nleqstate state, double epsf, int maxits)
    {
    
        nleq.nleqsetcond(state.innerobj, epsf, maxits, null);
    }
    
    public static void nleqsetcond(nleqstate state, double epsf, int maxits, alglib.xparams _params)
    {
    
        nleq.nleqsetcond(state.innerobj, epsf, maxits, _params);
    }
    
    /*************************************************************************
    This function turns on/off reporting.

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state
        NeedXRep-   whether iteration reports are needed or not

    If NeedXRep is True, algorithm will call rep() callback function if  it is
    provided to NLEQSolve().

      -- ALGLIB --
         Copyright 20.08.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void nleqsetxrep(nleqstate state, bool needxrep)
    {
    
        nleq.nleqsetxrep(state.innerobj, needxrep, null);
    }
    
    public static void nleqsetxrep(nleqstate state, bool needxrep, alglib.xparams _params)
    {
    
        nleq.nleqsetxrep(state.innerobj, needxrep, _params);
    }
    
    /*************************************************************************
    This function sets maximum step length

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state
        StpMax  -   maximum step length, >=0. Set StpMax to 0.0,  if you don't
                    want to limit step length.

    Use this subroutine when target function  contains  exp()  or  other  fast
    growing functions, and algorithm makes  too  large  steps  which  lead  to
    overflow. This function allows us to reject steps that are too large  (and
    therefore expose us to the possible overflow) without actually calculating
    function value at the x+stp*d.

      -- ALGLIB --
         Copyright 20.08.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void nleqsetstpmax(nleqstate state, double stpmax)
    {
    
        nleq.nleqsetstpmax(state.innerobj, stpmax, null);
    }
    
    public static void nleqsetstpmax(nleqstate state, double stpmax, alglib.xparams _params)
    {
    
        nleq.nleqsetstpmax(state.innerobj, stpmax, _params);
    }
    
    /*************************************************************************
    This function provides reverse communication interface
    Reverse communication interface is not documented or recommended to use.
    See below for functions which provide better documented API
    *************************************************************************/
    public static bool nleqiteration(nleqstate state)
    {
    
        return nleq.nleqiteration(state.innerobj, null);
    }
    
    public static bool nleqiteration(nleqstate state, alglib.xparams _params)
    {
    
        return nleq.nleqiteration(state.innerobj, _params);
    }
    /*************************************************************************
    This family of functions is used to launcn iterations of nonlinear solver

    These functions accept following parameters:
        func    -   callback which calculates function (or merit function)
                    value func at given point x
        jac     -   callback which calculates function vector fi[]
                    and Jacobian jac at given point x
        rep     -   optional callback which is called after each iteration
                    can be null
        obj     -   optional object which is passed to func/grad/hess/jac/rep
                    can be null


      -- ALGLIB --
         Copyright 20.03.2009 by Bochkanov Sergey

    *************************************************************************/
    public static void nleqsolve(nleqstate state, ndimensional_func func, ndimensional_jac  jac, ndimensional_rep rep, object obj)
    {
        nleqsolve(state, func, jac, rep, obj, null);
    }
    
    public static void nleqsolve(nleqstate state, ndimensional_func func, ndimensional_jac  jac, ndimensional_rep rep, object obj, alglib.xparams _params)
    {
        if( func==null )
            throw new alglibexception("ALGLIB: error in 'nleqsolve()' (func is null)");
        if( jac==null )
            throw new alglibexception("ALGLIB: error in 'nleqsolve()' (jac is null)");
        while( alglib.nleqiteration(state, _params) )
        {
            if( state.needf )
            {
                func(state.x, ref state.innerobj.f, obj);
                continue;
            }
            if( state.needfij )
            {
                jac(state.x, state.innerobj.fi, state.innerobj.j, obj);
                continue;
            }
            if( state.innerobj.xupdated )
            {
                if( rep!=null )
                    rep(state.innerobj.x, state.innerobj.f, obj);
                continue;
            }
            throw new alglibexception("ALGLIB: error in 'nleqsolve' (some derivatives were not provided?)");
        }
    }


    
    /*************************************************************************
    NLEQ solver results

    INPUT PARAMETERS:
        State   -   algorithm state.

    OUTPUT PARAMETERS:
        X       -   array[0..N-1], solution
        Rep     -   optimization report:
                    * Rep.TerminationType completetion code:
                        * -4    ERROR:  algorithm   has   converged   to   the
                                stationary point Xf which is local minimum  of
                                f=F[0]^2+...+F[m-1]^2, but is not solution  of
                                nonlinear system.
                        *  1    sqrt(f)<=EpsF.
                        *  5    MaxIts steps was taken
                        *  7    stopping conditions are too stringent,
                                further improvement is impossible
                    * Rep.IterationsCount contains iterations count
                    * NFEV countains number of function calculations
                    * ActiveConstraints contains number of active constraints

      -- ALGLIB --
         Copyright 20.08.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void nleqresults(nleqstate state, out double[] x, out nleqreport rep)
    {
        x = new double[0];
        rep = new nleqreport();
        nleq.nleqresults(state.innerobj, ref x, rep.innerobj, null);
    }
    
    public static void nleqresults(nleqstate state, out double[] x, out nleqreport rep, alglib.xparams _params)
    {
        x = new double[0];
        rep = new nleqreport();
        nleq.nleqresults(state.innerobj, ref x, rep.innerobj, _params);
    }
    
    /*************************************************************************
    NLEQ solver results

    Buffered implementation of NLEQResults(), which uses pre-allocated  buffer
    to store X[]. If buffer size is  too  small,  it  resizes  buffer.  It  is
    intended to be used in the inner cycles of performance critical algorithms
    where array reallocation penalty is too large to be ignored.

      -- ALGLIB --
         Copyright 20.08.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void nleqresultsbuf(nleqstate state, ref double[] x, nleqreport rep)
    {
    
        nleq.nleqresultsbuf(state.innerobj, ref x, rep.innerobj, null);
    }
    
    public static void nleqresultsbuf(nleqstate state, ref double[] x, nleqreport rep, alglib.xparams _params)
    {
    
        nleq.nleqresultsbuf(state.innerobj, ref x, rep.innerobj, _params);
    }
    
    /*************************************************************************
    This  subroutine  restarts  CG  algorithm from new point. All optimization
    parameters are left unchanged.

    This  function  allows  to  solve multiple  optimization  problems  (which
    must have same number of dimensions) without object reallocation penalty.

    INPUT PARAMETERS:
        State   -   structure used for reverse communication previously
                    allocated with MinCGCreate call.
        X       -   new starting point.
        BndL    -   new lower bounds
        BndU    -   new upper bounds

      -- ALGLIB --
         Copyright 30.07.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void nleqrestartfrom(nleqstate state, double[] x)
    {
    
        nleq.nleqrestartfrom(state.innerobj, x, null);
    }
    
    public static void nleqrestartfrom(nleqstate state, double[] x, alglib.xparams _params)
    {
    
        nleq.nleqrestartfrom(state.innerobj, x, _params);
    }

}
public partial class alglib
{


    /*************************************************************************
    This structure is a sparse solver report.

    Following fields can be accessed by users:
    *************************************************************************/
    public class sparsesolverreport : alglibobject
    {
        //
        // Public declarations
        //
        public int terminationtype { get { return _innerobj.terminationtype; } set { _innerobj.terminationtype = value; } }
    
        public sparsesolverreport()
        {
            _innerobj = new directsparsesolvers.sparsesolverreport();
        }
        
        public override alglib.alglibobject make_copy()
        {
            return new sparsesolverreport((directsparsesolvers.sparsesolverreport)_innerobj.make_copy());
        }
    
        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private directsparsesolvers.sparsesolverreport _innerobj;
        public directsparsesolvers.sparsesolverreport innerobj { get { return _innerobj; } }
        public sparsesolverreport(directsparsesolvers.sparsesolverreport obj)
        {
            _innerobj = obj;
        }
    }
    
    /*************************************************************************
    Sparse linear solver for A*x=b with N*N  sparse  real  symmetric  positive
    definite matrix A, N*1 vectors x and b.

    This solver  converts  input  matrix  to  SKS  format,  performs  Cholesky
    factorization using  SKS  Cholesky  subroutine  (works  well  for  limited
    bandwidth matrices) and uses sparse triangular solvers to get solution  of
    the original system.

    INPUT PARAMETERS
        A       -   sparse matrix, must be NxN exactly
        N       -   size of A, N>0
        IsUpper -   which half of A is provided (another half is ignored)
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS
        Rep     -   solver report, following fields are set:
                    * rep.terminationtype - solver status; >0 for success,
                      set to -3 on failure (degenerate or non-SPD system).
        X       -   array[N], it contains:
                    * rep.terminationtype>0    =>  solution
                    * rep.terminationtype=-3   =>  filled by zeros

      -- ALGLIB --
         Copyright 26.12.2017 by Bochkanov Sergey
    *************************************************************************/
    public static void sparsesolvesks(sparsematrix a, int n, bool isupper, double[] b, out sparsesolverreport rep, out double[] x)
    {
        rep = new sparsesolverreport();
        x = new double[0];
        directsparsesolvers.sparsesolvesks(a.innerobj, n, isupper, b, rep.innerobj, ref x, null);
    }
    
    public static void sparsesolvesks(sparsematrix a, int n, bool isupper, double[] b, out sparsesolverreport rep, out double[] x, alglib.xparams _params)
    {
        rep = new sparsesolverreport();
        x = new double[0];
        directsparsesolvers.sparsesolvesks(a.innerobj, n, isupper, b, rep.innerobj, ref x, _params);
    }
    
    /*************************************************************************
    Sparse linear solver for A*x=b with N*N real  symmetric  positive definite
    matrix A given by its Cholesky decomposition, and N*1 vectors x and b.

    IMPORTANT: this solver requires input matrix to be in  the  SKS  (Skyline)
               sparse storage format. An exception will be  generated  if  you
               pass matrix in some other format (HASH or CRS).

    INPUT PARAMETERS
        A       -   sparse NxN matrix stored in SKS format, must be NxN exactly
        N       -   size of A, N>0
        IsUpper -   which half of A is provided (another half is ignored)
        B       -   array[N], right part

    OUTPUT PARAMETERS
        Rep     -   solver report, following fields are set:
                    * rep.terminationtype - solver status; >0 for success,
                      set to -3 on failure (degenerate or non-SPD system).
        X       -   array[N], it contains:
                    * rep.terminationtype>0    =>  solution
                    * rep.terminationtype=-3   =>  filled by zeros

      -- ALGLIB --
         Copyright 26.12.2017 by Bochkanov Sergey
    *************************************************************************/
    public static void sparsecholeskysolvesks(sparsematrix a, int n, bool isupper, double[] b, out sparsesolverreport rep, out double[] x)
    {
        rep = new sparsesolverreport();
        x = new double[0];
        directsparsesolvers.sparsecholeskysolvesks(a.innerobj, n, isupper, b, rep.innerobj, ref x, null);
    }
    
    public static void sparsecholeskysolvesks(sparsematrix a, int n, bool isupper, double[] b, out sparsesolverreport rep, out double[] x, alglib.xparams _params)
    {
        rep = new sparsesolverreport();
        x = new double[0];
        directsparsesolvers.sparsecholeskysolvesks(a.innerobj, n, isupper, b, rep.innerobj, ref x, _params);
    }

}
public partial class alglib
{


    /*************************************************************************
    This object stores state of the linear CG method.

    You should use ALGLIB functions to work with this object.
    Never try to access its fields directly!
    *************************************************************************/
    public class lincgstate : alglibobject
    {
        //
        // Public declarations
        //
    
        public lincgstate()
        {
            _innerobj = new lincg.lincgstate();
        }
        
        public override alglib.alglibobject make_copy()
        {
            return new lincgstate((lincg.lincgstate)_innerobj.make_copy());
        }
    
        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private lincg.lincgstate _innerobj;
        public lincg.lincgstate innerobj { get { return _innerobj; } }
        public lincgstate(lincg.lincgstate obj)
        {
            _innerobj = obj;
        }
    }


    /*************************************************************************

    *************************************************************************/
    public class lincgreport : alglibobject
    {
        //
        // Public declarations
        //
        public int iterationscount { get { return _innerobj.iterationscount; } set { _innerobj.iterationscount = value; } }
        public int nmv { get { return _innerobj.nmv; } set { _innerobj.nmv = value; } }
        public int terminationtype { get { return _innerobj.terminationtype; } set { _innerobj.terminationtype = value; } }
        public double r2 { get { return _innerobj.r2; } set { _innerobj.r2 = value; } }
    
        public lincgreport()
        {
            _innerobj = new lincg.lincgreport();
        }
        
        public override alglib.alglibobject make_copy()
        {
            return new lincgreport((lincg.lincgreport)_innerobj.make_copy());
        }
    
        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private lincg.lincgreport _innerobj;
        public lincg.lincgreport innerobj { get { return _innerobj; } }
        public lincgreport(lincg.lincgreport obj)
        {
            _innerobj = obj;
        }
    }
    
    /*************************************************************************
    This function initializes linear CG Solver. This solver is used  to  solve
    symmetric positive definite problems. If you want  to  solve  nonsymmetric
    (or non-positive definite) problem you may use LinLSQR solver provided  by
    ALGLIB.

    USAGE:
    1. User initializes algorithm state with LinCGCreate() call
    2. User tunes solver parameters with  LinCGSetCond() and other functions
    3. Optionally, user sets starting point with LinCGSetStartingPoint()
    4. User  calls LinCGSolveSparse() function which takes algorithm state and
       SparseMatrix object.
    5. User calls LinCGResults() to get solution
    6. Optionally, user may call LinCGSolveSparse()  again  to  solve  another
       problem  with different matrix and/or right part without reinitializing
       LinCGState structure.

    INPUT PARAMETERS:
        N       -   problem dimension, N>0

    OUTPUT PARAMETERS:
        State   -   structure which stores algorithm state

      -- ALGLIB --
         Copyright 14.11.2011 by Bochkanov Sergey
    *************************************************************************/
    public static void lincgcreate(int n, out lincgstate state)
    {
        state = new lincgstate();
        lincg.lincgcreate(n, state.innerobj, null);
    }
    
    public static void lincgcreate(int n, out lincgstate state, alglib.xparams _params)
    {
        state = new lincgstate();
        lincg.lincgcreate(n, state.innerobj, _params);
    }
    
    /*************************************************************************
    This function sets starting point.
    By default, zero starting point is used.

    INPUT PARAMETERS:
        X       -   starting point, array[N]

    OUTPUT PARAMETERS:
        State   -   structure which stores algorithm state

      -- ALGLIB --
         Copyright 14.11.2011 by Bochkanov Sergey
    *************************************************************************/
    public static void lincgsetstartingpoint(lincgstate state, double[] x)
    {
    
        lincg.lincgsetstartingpoint(state.innerobj, x, null);
    }
    
    public static void lincgsetstartingpoint(lincgstate state, double[] x, alglib.xparams _params)
    {
    
        lincg.lincgsetstartingpoint(state.innerobj, x, _params);
    }
    
    /*************************************************************************
    This  function  changes  preconditioning  settings  of  LinCGSolveSparse()
    function. By default, SolveSparse() uses diagonal preconditioner,  but  if
    you want to use solver without preconditioning, you can call this function
    which forces solver to use unit matrix for preconditioning.

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state

      -- ALGLIB --
         Copyright 19.11.2012 by Bochkanov Sergey
    *************************************************************************/
    public static void lincgsetprecunit(lincgstate state)
    {
    
        lincg.lincgsetprecunit(state.innerobj, null);
    }
    
    public static void lincgsetprecunit(lincgstate state, alglib.xparams _params)
    {
    
        lincg.lincgsetprecunit(state.innerobj, _params);
    }
    
    /*************************************************************************
    This  function  changes  preconditioning  settings  of  LinCGSolveSparse()
    function.  LinCGSolveSparse() will use diagonal of the  system  matrix  as
    preconditioner. This preconditioning mode is active by default.

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state

      -- ALGLIB --
         Copyright 19.11.2012 by Bochkanov Sergey
    *************************************************************************/
    public static void lincgsetprecdiag(lincgstate state)
    {
    
        lincg.lincgsetprecdiag(state.innerobj, null);
    }
    
    public static void lincgsetprecdiag(lincgstate state, alglib.xparams _params)
    {
    
        lincg.lincgsetprecdiag(state.innerobj, _params);
    }
    
    /*************************************************************************
    This function sets stopping criteria.

    INPUT PARAMETERS:
        EpsF    -   algorithm will be stopped if norm of residual is less than
                    EpsF*||b||.
        MaxIts  -   algorithm will be stopped if number of iterations is  more
                    than MaxIts.

    OUTPUT PARAMETERS:
        State   -   structure which stores algorithm state

    NOTES:
    If  both  EpsF  and  MaxIts  are  zero then small EpsF will be set to small
    value.

      -- ALGLIB --
         Copyright 14.11.2011 by Bochkanov Sergey
    *************************************************************************/
    public static void lincgsetcond(lincgstate state, double epsf, int maxits)
    {
    
        lincg.lincgsetcond(state.innerobj, epsf, maxits, null);
    }
    
    public static void lincgsetcond(lincgstate state, double epsf, int maxits, alglib.xparams _params)
    {
    
        lincg.lincgsetcond(state.innerobj, epsf, maxits, _params);
    }
    
    /*************************************************************************
    Procedure for solution of A*x=b with sparse A.

    INPUT PARAMETERS:
        State   -   algorithm state
        A       -   sparse matrix in the CRS format (you MUST contvert  it  to
                    CRS format by calling SparseConvertToCRS() function).
        IsUpper -   whether upper or lower triangle of A is used:
                    * IsUpper=True  => only upper triangle is used and lower
                                       triangle is not referenced at all
                    * IsUpper=False => only lower triangle is used and upper
                                       triangle is not referenced at all
        B       -   right part, array[N]

    RESULT:
        This function returns no result.
        You can get solution by calling LinCGResults()

    NOTE: this function uses lightweight preconditioning -  multiplication  by
          inverse of diag(A). If you want, you can turn preconditioning off by
          calling LinCGSetPrecUnit(). However, preconditioning cost is low and
          preconditioner  is  very  important  for  solution  of  badly scaled
          problems.

      -- ALGLIB --
         Copyright 14.11.2011 by Bochkanov Sergey
    *************************************************************************/
    public static void lincgsolvesparse(lincgstate state, sparsematrix a, bool isupper, double[] b)
    {
    
        lincg.lincgsolvesparse(state.innerobj, a.innerobj, isupper, b, null);
    }
    
    public static void lincgsolvesparse(lincgstate state, sparsematrix a, bool isupper, double[] b, alglib.xparams _params)
    {
    
        lincg.lincgsolvesparse(state.innerobj, a.innerobj, isupper, b, _params);
    }
    
    /*************************************************************************
    CG-solver: results.

    This function must be called after LinCGSolve

    INPUT PARAMETERS:
        State   -   algorithm state

    OUTPUT PARAMETERS:
        X       -   array[N], solution
        Rep     -   optimization report:
                    * Rep.TerminationType completetion code:
                        * -5    input matrix is either not positive definite,
                                too large or too small
                        * -4    overflow/underflow during solution
                                (ill conditioned problem)
                        *  1    ||residual||<=EpsF*||b||
                        *  5    MaxIts steps was taken
                        *  7    rounding errors prevent further progress,
                                best point found is returned
                    * Rep.IterationsCount contains iterations count
                    * NMV countains number of matrix-vector calculations

      -- ALGLIB --
         Copyright 14.11.2011 by Bochkanov Sergey
    *************************************************************************/
    public static void lincgresults(lincgstate state, out double[] x, out lincgreport rep)
    {
        x = new double[0];
        rep = new lincgreport();
        lincg.lincgresults(state.innerobj, ref x, rep.innerobj, null);
    }
    
    public static void lincgresults(lincgstate state, out double[] x, out lincgreport rep, alglib.xparams _params)
    {
        x = new double[0];
        rep = new lincgreport();
        lincg.lincgresults(state.innerobj, ref x, rep.innerobj, _params);
    }
    
    /*************************************************************************
    This function sets restart frequency. By default, algorithm  is  restarted
    after N subsequent iterations.

      -- ALGLIB --
         Copyright 14.11.2011 by Bochkanov Sergey
    *************************************************************************/
    public static void lincgsetrestartfreq(lincgstate state, int srf)
    {
    
        lincg.lincgsetrestartfreq(state.innerobj, srf, null);
    }
    
    public static void lincgsetrestartfreq(lincgstate state, int srf, alglib.xparams _params)
    {
    
        lincg.lincgsetrestartfreq(state.innerobj, srf, _params);
    }
    
    /*************************************************************************
    This function sets frequency of residual recalculations.

    Algorithm updates residual r_k using iterative formula,  but  recalculates
    it from scratch after each 10 iterations. It is done to avoid accumulation
    of numerical errors and to stop algorithm when r_k starts to grow.

    Such low update frequence (1/10) gives very  little  overhead,  but  makes
    algorithm a bit more robust against numerical errors. However, you may
    change it

    INPUT PARAMETERS:
        Freq    -   desired update frequency, Freq>=0.
                    Zero value means that no updates will be done.

      -- ALGLIB --
         Copyright 14.11.2011 by Bochkanov Sergey
    *************************************************************************/
    public static void lincgsetrupdatefreq(lincgstate state, int freq)
    {
    
        lincg.lincgsetrupdatefreq(state.innerobj, freq, null);
    }
    
    public static void lincgsetrupdatefreq(lincgstate state, int freq, alglib.xparams _params)
    {
    
        lincg.lincgsetrupdatefreq(state.innerobj, freq, _params);
    }
    
    /*************************************************************************
    This function turns on/off reporting.

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state
        NeedXRep-   whether iteration reports are needed or not

    If NeedXRep is True, algorithm will call rep() callback function if  it is
    provided to MinCGOptimize().

      -- ALGLIB --
         Copyright 14.11.2011 by Bochkanov Sergey
    *************************************************************************/
    public static void lincgsetxrep(lincgstate state, bool needxrep)
    {
    
        lincg.lincgsetxrep(state.innerobj, needxrep, null);
    }
    
    public static void lincgsetxrep(lincgstate state, bool needxrep, alglib.xparams _params)
    {
    
        lincg.lincgsetxrep(state.innerobj, needxrep, _params);
    }

}
public partial class alglib
{
    public class directdensesolvers
    {
        public class densesolverreport : apobject
        {
            public double r1;
            public double rinf;
            public densesolverreport()
            {
                init();
            }
            public override void init()
            {
            }
            public override alglib.apobject make_copy()
            {
                densesolverreport _result = new densesolverreport();
                _result.r1 = r1;
                _result.rinf = rinf;
                return _result;
            }
        };


        public class densesolverlsreport : apobject
        {
            public double r2;
            public double[,] cx;
            public int n;
            public int k;
            public densesolverlsreport()
            {
                init();
            }
            public override void init()
            {
                cx = new double[0,0];
            }
            public override alglib.apobject make_copy()
            {
                densesolverlsreport _result = new densesolverlsreport();
                _result.r2 = r2;
                _result.cx = (double[,])cx.Clone();
                _result.n = n;
                _result.k = k;
                return _result;
            }
        };




        /*************************************************************************
        Dense solver for A*x=b with N*N real matrix A and N*1 real vectorx  x  and
        b. This is "slow-but-feature rich" version of the  linear  solver.  Faster
        version is RMatrixSolveFast() function.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * iterative refinement
        * O(N^3) complexity

        IMPORTANT: ! this function is NOT the most efficient linear solver provided
                   ! by ALGLIB. It estimates condition  number  of  linear  system
                   ! and  performs  iterative   refinement,   which   results   in
                   ! significant performance penalty  when  compared  with  "fast"
                   ! version  which  just  performs  LU  decomposition  and  calls
                   ! triangular solver.
                   !
                   ! This  performance  penalty  is  especially  visible  in   the
                   ! multithreaded mode, because both condition number  estimation
                   ! and   iterative    refinement   are   inherently   sequential
                   ! calculations. It is also very significant on small matrices.
                   !
                   ! Thus, if you need high performance and if you are pretty sure
                   ! that your system is well conditioned, we  strongly  recommend
                   ! you to use faster solver, RMatrixSolveFast() function.

          ! COMMERCIAL EDITION OF ALGLIB:
          ! 
          ! Commercial Edition of ALGLIB includes following important improvements
          ! of this function:
          ! * high-performance native backend with same C# interface (C# version)
          ! * multithreading support (C++ and C# versions)
          ! * hardware vendor (Intel) implementations of linear algebra primitives
          !   (C++ and C# versions, x86/x64 platform)
          ! 
          ! We recommend you to read 'Working with commercial version' section  of
          ! ALGLIB Reference Manual in order to find out how to  use  performance-
          ! related features provided by commercial edition of ALGLIB.

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    matrix is very badly conditioned or exactly singular.
                        * -1    N<=0 was passed
                        *  1    task is solved (but matrix A may be ill-conditioned,
                                check R1/RInf parameters for condition numbers).
            Rep     -   additional report, following fields are set:
                        * rep.r1    condition number in 1-norm
                        * rep.rinf  condition number in inf-norm
            X       -   array[N], it contains:
                        * info>0    =>  solution
                        * info=-3   =>  filled by zeros

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixsolve(double[,] a,
            int n,
            double[] b,
            ref int info,
            densesolverreport rep,
            ref double[] x,
            alglib.xparams _params)
        {
            double[,] bm = new double[0,0];
            double[,] xm = new double[0,0];
            int i_ = 0;

            info = 0;
            x = new double[0];

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new double[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            rmatrixsolvem(a, n, bm, 1, true, ref info, rep, ref xm, _params);
            x = new double[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver.

        This  subroutine  solves  a  system  A*x=b,  where A is NxN non-denegerate
        real matrix, x  and  b  are  vectors.  This is a "fast" version of  linear
        solver which does NOT provide  any  additional  functions  like  condition
        number estimation or iterative refinement.

        Algorithm features:
        * efficient algorithm O(N^3) complexity
        * no performance overhead from additional functionality

        If you need condition number estimation or iterative refinement, use  more
        feature-rich version - RMatrixSolve().

          ! COMMERCIAL EDITION OF ALGLIB:
          ! 
          ! Commercial Edition of ALGLIB includes following important improvements
          ! of this function:
          ! * high-performance native backend with same C# interface (C# version)
          ! * multithreading support (C++ and C# versions)
          ! * hardware vendor (Intel) implementations of linear algebra primitives
          !   (C++ and C# versions, x86/x64 platform)
          ! 
          ! We recommend you to read 'Working with commercial version' section  of
          ! ALGLIB Reference Manual in order to find out how to  use  performance-
          ! related features provided by commercial edition of ALGLIB.

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    matrix is exactly singular (ill conditioned matrices
                                are not recognized).
                        * -1    N<=0 was passed
                        *  1    task is solved 
            B       -   array[N]:
                        * info>0    =>  overwritten by solution
                        * info=-3   =>  filled by zeros

          -- ALGLIB --
             Copyright 16.03.2015 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixsolvefast(double[,] a,
            int n,
            double[] b,
            ref int info,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;
            int[] p = new int[0];

            a = (double[,])a.Clone();
            info = 0;

            if( n<=0 )
            {
                info = -1;
                return;
            }
            trfac.rmatrixlu(ref a, n, n, ref p, _params);
            for(i=0; i<=n-1; i++)
            {
                if( (double)(a[i,i])==(double)(0) )
                {
                    for(j=0; j<=n-1; j++)
                    {
                        b[j] = 0.0;
                    }
                    info = -3;
                    return;
                }
            }
            rbasiclusolve(a, p, n, b, _params);
            info = 1;
        }


        /*************************************************************************
        Dense solver.

        Similar to RMatrixSolve() but solves task with multiple right parts (where
        b and x are NxM matrices). This is  "slow-but-robust"  version  of  linear
        solver with additional functionality  like  condition  number  estimation.
        There also exists faster version - RMatrixSolveMFast().

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * optional iterative refinement
        * O(N^3+M*N^2) complexity

        IMPORTANT: ! this function is NOT the most efficient linear solver provided
                   ! by ALGLIB. It estimates condition  number  of  linear  system
                   ! and  performs  iterative   refinement,   which   results   in
                   ! significant performance penalty  when  compared  with  "fast"
                   ! version  which  just  performs  LU  decomposition  and  calls
                   ! triangular solver.
                   !
                   ! This  performance  penalty  is  especially  visible  in   the
                   ! multithreaded mode, because both condition number  estimation
                   ! and   iterative    refinement   are   inherently   sequential
                   ! calculations. It also very significant on small matrices.
                   !
                   ! Thus, if you need high performance and if you are pretty sure
                   ! that your system is well conditioned, we  strongly  recommend
                   ! you to use faster solver, RMatrixSolveMFast() function.

          ! COMMERCIAL EDITION OF ALGLIB:
          ! 
          ! Commercial Edition of ALGLIB includes following important improvements
          ! of this function:
          ! * high-performance native backend with same C# interface (C# version)
          ! * multithreading support (C++ and C# versions)
          ! * hardware vendor (Intel) implementations of linear algebra primitives
          !   (C++ and C# versions, x86/x64 platform)
          ! 
          ! We recommend you to read 'Working with commercial version' section  of
          ! ALGLIB Reference Manual in order to find out how to  use  performance-
          ! related features provided by commercial edition of ALGLIB.

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size
            RFS     -   iterative refinement switch:
                        * True - refinement is used.
                          Less performance, more precision.
                        * False - refinement is not used.
                          More performance, less precision.

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    A is ill conditioned or singular.
                                X is filled by zeros in such cases.
                        * -1    N<=0 was passed
                        *  1    task is solved (but matrix A may be ill-conditioned,
                                check R1/RInf parameters for condition numbers).
            Rep     -   additional report, following fields are set:
                        * rep.r1    condition number in 1-norm
                        * rep.rinf  condition number in inf-norm
            X       -   array[N], it contains:
                        * info>0    =>  solution
                        * info=-3   =>  filled by zeros


          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixsolvem(double[,] a,
            int n,
            double[,] b,
            int m,
            bool rfs,
            ref int info,
            densesolverreport rep,
            ref double[,] x,
            alglib.xparams _params)
        {
            double[,] da = new double[0,0];
            double[,] emptya = new double[0,0];
            int[] p = new int[0];
            int i = 0;
            int i_ = 0;

            info = 0;
            x = new double[0,0];

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 || m<=0 )
            {
                info = -1;
                return;
            }
            da = new double[n, n];
            
            //
            // 1. factorize matrix
            // 3. solve
            //
            for(i=0; i<=n-1; i++)
            {
                for(i_=0; i_<=n-1;i_++)
                {
                    da[i,i_] = a[i,i_];
                }
            }
            trfac.rmatrixlu(ref da, n, n, ref p, _params);
            if( rfs )
            {
                rmatrixlusolveinternal(da, p, n, a, true, b, m, ref info, rep, ref x, _params);
            }
            else
            {
                rmatrixlusolveinternal(da, p, n, emptya, false, b, m, ref info, rep, ref x, _params);
            }
        }


        /*************************************************************************
        Dense solver.

        Similar to RMatrixSolve() but solves task with multiple right parts (where
        b and x are NxM matrices). This is "fast" version of linear  solver  which
        does NOT offer additional functions like condition  number  estimation  or
        iterative refinement.

        Algorithm features:
        * O(N^3+M*N^2) complexity
        * no additional functionality, highest performance

          ! COMMERCIAL EDITION OF ALGLIB:
          ! 
          ! Commercial Edition of ALGLIB includes following important improvements
          ! of this function:
          ! * high-performance native backend with same C# interface (C# version)
          ! * multithreading support (C++ and C# versions)
          ! * hardware vendor (Intel) implementations of linear algebra primitives
          !   (C++ and C# versions, x86/x64 platform)
          ! 
          ! We recommend you to read 'Working with commercial version' section  of
          ! ALGLIB Reference Manual in order to find out how to  use  performance-
          ! related features provided by commercial edition of ALGLIB.

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size
            RFS     -   iterative refinement switch:
                        * True - refinement is used.
                          Less performance, more precision.
                        * False - refinement is not used.
                          More performance, less precision.

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    matrix is exactly singular (ill conditioned matrices
                                are not recognized).
                                X is filled by zeros in such cases.
                        * -1    N<=0 was passed
                        *  1    task is solved
            Rep     -   additional report, following fields are set:
                        * rep.r1    condition number in 1-norm
                        * rep.rinf  condition number in inf-norm
            B       -   array[N]:
                        * info>0    =>  overwritten by solution
                        * info=-3   =>  filled by zeros


          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixsolvemfast(double[,] a,
            int n,
            double[,] b,
            int m,
            ref int info,
            alglib.xparams _params)
        {
            double v = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int[] p = new int[0];

            a = (double[,])a.Clone();
            info = 0;

            
            //
            // Check for exact degeneracy
            //
            if( n<=0 || m<=0 )
            {
                info = -1;
                return;
            }
            trfac.rmatrixlu(ref a, n, n, ref p, _params);
            for(i=0; i<=n-1; i++)
            {
                if( (double)(a[i,i])==(double)(0) )
                {
                    for(j=0; j<=n-1; j++)
                    {
                        for(k=0; k<=m-1; k++)
                        {
                            b[j,k] = 0.0;
                        }
                    }
                    info = -3;
                    return;
                }
            }
            
            //
            // Solve with TRSM()
            //
            for(i=0; i<=n-1; i++)
            {
                if( p[i]!=i )
                {
                    for(j=0; j<=m-1; j++)
                    {
                        v = b[i,j];
                        b[i,j] = b[p[i],j];
                        b[p[i],j] = v;
                    }
                }
            }
            ablas.rmatrixlefttrsm(n, m, a, 0, 0, false, true, 0, b, 0, 0, _params);
            ablas.rmatrixlefttrsm(n, m, a, 0, 0, true, false, 0, b, 0, 0, _params);
            info = 1;
        }


        /*************************************************************************
        Dense solver.

        This  subroutine  solves  a  system  A*x=b,  where A is NxN non-denegerate
        real matrix given by its LU decomposition, x and b are real vectors.  This
        is "slow-but-robust" version of the linear LU-based solver. Faster version
        is RMatrixLUSolveFast() function.

        Algorithm features:
        * automatic detection of degenerate cases
        * O(N^2) complexity
        * condition number estimation

        No iterative refinement  is provided because exact form of original matrix
        is not known to subroutine. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        IMPORTANT: ! this function is NOT the most efficient linear solver provided
                   ! by ALGLIB. It estimates condition  number  of  linear system,
                   ! which results in 10-15x  performance  penalty  when  compared
                   ! with "fast" version which just calls triangular solver.
                   !
                   ! This performance penalty is insignificant  when compared with
                   ! cost of large LU decomposition.  However,  if you  call  this
                   ! function many times for the same  left  side,  this  overhead
                   ! BECOMES significant. It  also  becomes significant for small-
                   ! scale problems.
                   !
                   ! In such cases we strongly recommend you to use faster solver,
                   ! RMatrixLUSolveFast() function.

        INPUT PARAMETERS
            LUA     -   array[N,N], LU decomposition, RMatrixLU result
            P       -   array[N], pivots array, RMatrixLU result
            N       -   size of A
            B       -   array[N], right part

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    matrix is very badly conditioned or exactly singular.
                        * -1    N<=0 was passed
                        *  1    task is solved (but matrix A may be ill-conditioned,
                                check R1/RInf parameters for condition numbers).
            Rep     -   additional report, following fields are set:
                        * rep.r1    condition number in 1-norm
                        * rep.rinf  condition number in inf-norm
            X       -   array[N], it contains:
                        * info>0    =>  solution
                        * info=-3   =>  filled by zeros

            
          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixlusolve(double[,] lua,
            int[] p,
            int n,
            double[] b,
            ref int info,
            densesolverreport rep,
            ref double[] x,
            alglib.xparams _params)
        {
            double[,] bm = new double[0,0];
            double[,] xm = new double[0,0];
            int i_ = 0;

            info = 0;
            x = new double[0];

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new double[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            rmatrixlusolvem(lua, p, n, bm, 1, ref info, rep, ref xm, _params);
            x = new double[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver.

        This  subroutine  solves  a  system  A*x=b,  where A is NxN non-denegerate
        real matrix given by its LU decomposition, x and b are real vectors.  This
        is "fast-without-any-checks" version of the linear LU-based solver. Slower
        but more robust version is RMatrixLUSolve() function.

        Algorithm features:
        * O(N^2) complexity
        * fast algorithm without ANY additional checks, just triangular solver

        INPUT PARAMETERS
            LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
            P       -   array[0..N-1], pivots array, RMatrixLU result
            N       -   size of A
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    matrix is exactly singular (ill conditioned matrices
                                are not recognized).
                                X is filled by zeros in such cases.
                        * -1    N<=0 was passed
                        *  1    task is solved 
            B       -   array[N]:
                        * info>0    =>  overwritten by solution
                        * info=-3   =>  filled by zeros

          -- ALGLIB --
             Copyright 18.03.2015 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixlusolvefast(double[,] lua,
            int[] p,
            int n,
            double[] b,
            ref int info,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;

            info = 0;

            if( n<=0 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                if( (double)(lua[i,i])==(double)(0) )
                {
                    for(j=0; j<=n-1; j++)
                    {
                        b[j] = 0.0;
                    }
                    info = -3;
                    return;
                }
            }
            rbasiclusolve(lua, p, n, b, _params);
            info = 1;
        }


        /*************************************************************************
        Dense solver.

        Similar to RMatrixLUSolve() but solves  task  with  multiple  right  parts
        (where b and x are NxM matrices). This  is  "robust-but-slow"  version  of
        LU-based solver which performs additional  checks  for  non-degeneracy  of
        inputs (condition number estimation). If you need  best  performance,  use
        "fast-without-any-checks" version, RMatrixLUSolveMFast().

        Algorithm features:
        * automatic detection of degenerate cases
        * O(M*N^2) complexity
        * condition number estimation

        No iterative refinement  is provided because exact form of original matrix
        is not known to subroutine. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        IMPORTANT: ! this function is NOT the most efficient linear solver provided
                   ! by ALGLIB. It estimates condition  number  of  linear system,
                   ! which  results  in  significant  performance   penalty   when
                   ! compared with "fast"  version  which  just  calls  triangular
                   ! solver.
                   !
                   ! This performance penalty is especially apparent when you  use
                   ! ALGLIB parallel capabilities (condition number estimation  is
                   ! inherently  sequential).  It   also   becomes significant for
                   ! small-scale problems.
                   !
                   ! In such cases we strongly recommend you to use faster solver,
                   ! RMatrixLUSolveMFast() function.

          ! COMMERCIAL EDITION OF ALGLIB:
          ! 
          ! Commercial Edition of ALGLIB includes following important improvements
          ! of this function:
          ! * high-performance native backend with same C# interface (C# version)
          ! * multithreading support (C++ and C# versions)
          ! * hardware vendor (Intel) implementations of linear algebra primitives
          !   (C++ and C# versions, x86/x64 platform)
          ! 
          ! We recommend you to read 'Working with commercial version' section  of
          ! ALGLIB Reference Manual in order to find out how to  use  performance-
          ! related features provided by commercial edition of ALGLIB.
          
        INPUT PARAMETERS
            LUA     -   array[N,N], LU decomposition, RMatrixLU result
            P       -   array[N], pivots array, RMatrixLU result
            N       -   size of A
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    matrix is very badly conditioned or exactly singular.
                                X is filled by zeros in such cases.
                        * -1    N<=0 was passed
                        *  1    task is solved (but matrix A may be ill-conditioned,
                                check R1/RInf parameters for condition numbers).
            Rep     -   additional report, following fields are set:
                        * rep.r1    condition number in 1-norm
                        * rep.rinf  condition number in inf-norm
            X       -   array[N,M], it contains:
                        * info>0    =>  solution
                        * info=-3   =>  filled by zeros


          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixlusolvem(double[,] lua,
            int[] p,
            int n,
            double[,] b,
            int m,
            ref int info,
            densesolverreport rep,
            ref double[,] x,
            alglib.xparams _params)
        {
            double[,] emptya = new double[0,0];

            info = 0;
            x = new double[0,0];

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 || m<=0 )
            {
                info = -1;
                return;
            }
            
            //
            // solve
            //
            rmatrixlusolveinternal(lua, p, n, emptya, false, b, m, ref info, rep, ref x, _params);
        }


        /*************************************************************************
        Dense solver.

        Similar to RMatrixLUSolve() but solves  task  with  multiple  right parts,
        where b and x are NxM matrices.  This is "fast-without-any-checks" version
        of LU-based solver. It does not estimate  condition number  of  a  system,
        so it is extremely fast. If you need better detection  of  near-degenerate
        cases, use RMatrixLUSolveM() function.

        Algorithm features:
        * O(M*N^2) complexity
        * fast algorithm without ANY additional checks, just triangular solver

          ! COMMERCIAL EDITION OF ALGLIB:
          ! 
          ! Commercial Edition of ALGLIB includes following important improvements
          ! of this function:
          ! * high-performance native backend with same C# interface (C# version)
          ! * multithreading support (C++ and C# versions)
          ! * hardware vendor (Intel) implementations of linear algebra primitives
          !   (C++ and C# versions, x86/x64 platform)
          ! 
          ! We recommend you to read 'Working with commercial version' section  of
          ! ALGLIB Reference Manual in order to find out how to  use  performance-
          ! related features provided by commercial edition of ALGLIB.

        INPUT PARAMETERS:
            LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
            P       -   array[0..N-1], pivots array, RMatrixLU result
            N       -   size of A
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS:
            Info    -   return code:
                        * -3    matrix is exactly singular (ill conditioned matrices
                                are not recognized).
                        * -1    N<=0 was passed
                        *  1    task is solved
            B       -   array[N,M]:
                        * info>0    =>  overwritten by solution
                        * info=-3   =>  filled by zeros

          -- ALGLIB --
             Copyright 18.03.2015 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixlusolvemfast(double[,] lua,
            int[] p,
            int n,
            double[,] b,
            int m,
            ref int info,
            alglib.xparams _params)
        {
            double v = 0;
            int i = 0;
            int j = 0;
            int k = 0;

            info = 0;

            
            //
            // Check for exact degeneracy
            //
            if( n<=0 || m<=0 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                if( (double)(lua[i,i])==(double)(0) )
                {
                    for(j=0; j<=n-1; j++)
                    {
                        for(k=0; k<=m-1; k++)
                        {
                            b[j,k] = 0.0;
                        }
                    }
                    info = -3;
                    return;
                }
            }
            
            //
            // Solve with TRSM()
            //
            for(i=0; i<=n-1; i++)
            {
                if( p[i]!=i )
                {
                    for(j=0; j<=m-1; j++)
                    {
                        v = b[i,j];
                        b[i,j] = b[p[i],j];
                        b[p[i],j] = v;
                    }
                }
            }
            ablas.rmatrixlefttrsm(n, m, lua, 0, 0, false, true, 0, b, 0, 0, _params);
            ablas.rmatrixlefttrsm(n, m, lua, 0, 0, true, false, 0, b, 0, 0, _params);
            info = 1;
        }


        /*************************************************************************
        Dense solver.

        This  subroutine  solves  a  system  A*x=b,  where BOTH ORIGINAL A AND ITS
        LU DECOMPOSITION ARE KNOWN. You can use it if for some  reasons  you  have
        both A and its LU decomposition.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * iterative refinement
        * O(N^2) complexity

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
            P       -   array[0..N-1], pivots array, RMatrixLU result
            N       -   size of A
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    matrix is very badly conditioned or exactly singular.
                        * -1    N<=0 was passed
                        *  1    task is solved (but matrix A may be ill-conditioned,
                                check R1/RInf parameters for condition numbers).
            Rep     -   additional report, following fields are set:
                        * rep.r1    condition number in 1-norm
                        * rep.rinf  condition number in inf-norm
            X       -   array[N], it contains:
                        * info>0    =>  solution
                        * info=-3   =>  filled by zeros

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixmixedsolve(double[,] a,
            double[,] lua,
            int[] p,
            int n,
            double[] b,
            ref int info,
            densesolverreport rep,
            ref double[] x,
            alglib.xparams _params)
        {
            double[,] bm = new double[0,0];
            double[,] xm = new double[0,0];
            int i_ = 0;

            info = 0;
            x = new double[0];

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new double[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            rmatrixmixedsolvem(a, lua, p, n, bm, 1, ref info, rep, ref xm, _params);
            x = new double[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver.

        Similar to RMatrixMixedSolve() but  solves task with multiple right  parts
        (where b and x are NxM matrices).

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * iterative refinement
        * O(M*N^2) complexity

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
            P       -   array[0..N-1], pivots array, RMatrixLU result
            N       -   size of A
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    matrix is very badly conditioned or exactly singular.
                        * -1    N<=0 was passed
                        *  1    task is solved (but matrix A may be ill-conditioned,
                                check R1/RInf parameters for condition numbers).
            Rep     -   additional report, following fields are set:
                        * rep.r1    condition number in 1-norm
                        * rep.rinf  condition number in inf-norm
            X       -   array[N,M], it contains:
                        * info>0    =>  solution
                        * info=-3   =>  filled by zeros

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixmixedsolvem(double[,] a,
            double[,] lua,
            int[] p,
            int n,
            double[,] b,
            int m,
            ref int info,
            densesolverreport rep,
            ref double[,] x,
            alglib.xparams _params)
        {
            info = 0;
            x = new double[0,0];

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 || m<=0 )
            {
                info = -1;
                return;
            }
            
            //
            // solve
            //
            rmatrixlusolveinternal(lua, p, n, a, true, b, m, ref info, rep, ref x, _params);
        }


        /*************************************************************************
        Complex dense solver for A*X=B with N*N  complex  matrix  A,  N*M  complex
        matrices  X  and  B.  "Slow-but-feature-rich"   version   which   provides
        additional functions, at the cost of slower  performance.  Faster  version
        may be invoked with CMatrixSolveMFast() function.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * iterative refinement
        * O(N^3+M*N^2) complexity

        IMPORTANT: ! this function is NOT the most efficient linear solver provided
                   ! by ALGLIB. It estimates condition  number  of  linear  system
                   ! and  performs  iterative   refinement,   which   results   in
                   ! significant performance penalty  when  compared  with  "fast"
                   ! version  which  just  performs  LU  decomposition  and  calls
                   ! triangular solver.
                   !
                   ! This  performance  penalty  is  especially  visible  in   the
                   ! multithreaded mode, because both condition number  estimation
                   ! and   iterative    refinement   are   inherently   sequential
                   ! calculations.
                   !
                   ! Thus, if you need high performance and if you are pretty sure
                   ! that your system is well conditioned, we  strongly  recommend
                   ! you to use faster solver, CMatrixSolveMFast() function.

          ! COMMERCIAL EDITION OF ALGLIB:
          ! 
          ! Commercial Edition of ALGLIB includes following important improvements
          ! of this function:
          ! * high-performance native backend with same C# interface (C# version)
          ! * multithreading support (C++ and C# versions)
          ! * hardware vendor (Intel) implementations of linear algebra primitives
          !   (C++ and C# versions, x86/x64 platform)
          ! 
          ! We recommend you to read 'Working with commercial version' section  of
          ! ALGLIB Reference Manual in order to find out how to  use  performance-
          ! related features provided by commercial edition of ALGLIB.

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size
            RFS     -   iterative refinement switch:
                        * True - refinement is used.
                          Less performance, more precision.
                        * False - refinement is not used.
                          More performance, less precision.

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    matrix is very badly conditioned or exactly singular.
                                X is filled by zeros in such cases.
                        * -1    N<=0 was passed
                        *  1    task is solved (but matrix A may be ill-conditioned,
                                check R1/RInf parameters for condition numbers).
            Rep     -   additional report, following fields are set:
                        * rep.r1    condition number in 1-norm
                        * rep.rinf  condition number in inf-norm
            X       -   array[N,M], it contains:
                        * info>0    =>  solution
                        * info=-3   =>  filled by zeros

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixsolvem(complex[,] a,
            int n,
            complex[,] b,
            int m,
            bool rfs,
            ref int info,
            densesolverreport rep,
            ref complex[,] x,
            alglib.xparams _params)
        {
            complex[,] da = new complex[0,0];
            complex[,] emptya = new complex[0,0];
            int[] p = new int[0];
            int i = 0;
            int i_ = 0;

            info = 0;
            x = new complex[0,0];

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 || m<=0 )
            {
                info = -1;
                return;
            }
            da = new complex[n, n];
            
            //
            // factorize, solve
            //
            for(i=0; i<=n-1; i++)
            {
                for(i_=0; i_<=n-1;i_++)
                {
                    da[i,i_] = a[i,i_];
                }
            }
            trfac.cmatrixlu(ref da, n, n, ref p, _params);
            if( rfs )
            {
                cmatrixlusolveinternal(da, p, n, a, true, b, m, ref info, rep, ref x, _params);
            }
            else
            {
                cmatrixlusolveinternal(da, p, n, emptya, false, b, m, ref info, rep, ref x, _params);
            }
        }


        /*************************************************************************
        Complex dense solver for A*X=B with N*N  complex  matrix  A,  N*M  complex
        matrices  X  and  B.  "Fast-but-lightweight" version which  provides  just
        triangular solver - and no additional functions like iterative  refinement
        or condition number estimation.

        Algorithm features:
        * O(N^3+M*N^2) complexity
        * no additional time consuming functions

          ! COMMERCIAL EDITION OF ALGLIB:
          ! 
          ! Commercial Edition of ALGLIB includes following important improvements
          ! of this function:
          ! * high-performance native backend with same C# interface (C# version)
          ! * multithreading support (C++ and C# versions)
          ! * hardware vendor (Intel) implementations of linear algebra primitives
          !   (C++ and C# versions, x86/x64 platform)
          ! 
          ! We recommend you to read 'Working with commercial version' section  of
          ! ALGLIB Reference Manual in order to find out how to  use  performance-
          ! related features provided by commercial edition of ALGLIB.

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS:
            Info    -   return code:
                        * -3    matrix is exactly singular (ill conditioned matrices
                                are not recognized).
                        * -1    N<=0 was passed
                        *  1    task is solved 
            B       -   array[N,M]:
                        * info>0    =>  overwritten by solution
                        * info=-3   =>  filled by zeros

          -- ALGLIB --
             Copyright 16.03.2015 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixsolvemfast(complex[,] a,
            int n,
            complex[,] b,
            int m,
            ref int info,
            alglib.xparams _params)
        {
            complex v = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int[] p = new int[0];

            a = (complex[,])a.Clone();
            info = 0;

            
            //
            // Check for exact degeneracy
            //
            if( n<=0 || m<=0 )
            {
                info = -1;
                return;
            }
            trfac.cmatrixlu(ref a, n, n, ref p, _params);
            for(i=0; i<=n-1; i++)
            {
                if( a[i,i]==0 )
                {
                    for(j=0; j<=n-1; j++)
                    {
                        for(k=0; k<=m-1; k++)
                        {
                            b[j,k] = 0.0;
                        }
                    }
                    info = -3;
                    return;
                }
            }
            
            //
            // Solve with TRSM()
            //
            for(i=0; i<=n-1; i++)
            {
                if( p[i]!=i )
                {
                    for(j=0; j<=m-1; j++)
                    {
                        v = b[i,j];
                        b[i,j] = b[p[i],j];
                        b[p[i],j] = v;
                    }
                }
            }
            ablas.cmatrixlefttrsm(n, m, a, 0, 0, false, true, 0, b, 0, 0, _params);
            ablas.cmatrixlefttrsm(n, m, a, 0, 0, true, false, 0, b, 0, 0, _params);
            info = 1;
        }


        /*************************************************************************
        Complex dense solver for A*x=B with N*N complex matrix A and  N*1  complex
        vectors x and b. "Slow-but-feature-rich" version of the solver.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * iterative refinement
        * O(N^3) complexity

        IMPORTANT: ! this function is NOT the most efficient linear solver provided
                   ! by ALGLIB. It estimates condition  number  of  linear  system
                   ! and  performs  iterative   refinement,   which   results   in
                   ! significant performance penalty  when  compared  with  "fast"
                   ! version  which  just  performs  LU  decomposition  and  calls
                   ! triangular solver.
                   !
                   ! This  performance  penalty  is  especially  visible  in   the
                   ! multithreaded mode, because both condition number  estimation
                   ! and   iterative    refinement   are   inherently   sequential
                   ! calculations.
                   !
                   ! Thus, if you need high performance and if you are pretty sure
                   ! that your system is well conditioned, we  strongly  recommend
                   ! you to use faster solver, CMatrixSolveFast() function.

          ! COMMERCIAL EDITION OF ALGLIB:
          ! 
          ! Commercial Edition of ALGLIB includes following important improvements
          ! of this function:
          ! * high-performance native backend with same C# interface (C# version)
          ! * multithreading support (C++ and C# versions)
          ! * hardware vendor (Intel) implementations of linear algebra primitives
          !   (C++ and C# versions, x86/x64 platform)
          ! 
          ! We recommend you to read 'Working with commercial version' section  of
          ! ALGLIB Reference Manual in order to find out how to  use  performance-
          ! related features provided by commercial edition of ALGLIB.

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    matrix is very badly conditioned or exactly singular.
                        * -1    N<=0 was passed
                        *  1    task is solved (but matrix A may be ill-conditioned,
                                check R1/RInf parameters for condition numbers).
            Rep     -   additional report, following fields are set:
                        * rep.r1    condition number in 1-norm
                        * rep.rinf  condition number in inf-norm
            X       -   array[N], it contains:
                        * info>0    =>  solution
                        * info=-3   =>  filled by zeros

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixsolve(complex[,] a,
            int n,
            complex[] b,
            ref int info,
            densesolverreport rep,
            ref complex[] x,
            alglib.xparams _params)
        {
            complex[,] bm = new complex[0,0];
            complex[,] xm = new complex[0,0];
            int i_ = 0;

            info = 0;
            x = new complex[0];

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new complex[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            cmatrixsolvem(a, n, bm, 1, true, ref info, rep, ref xm, _params);
            x = new complex[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Complex dense solver for A*x=B with N*N complex matrix A and  N*1  complex
        vectors x and b. "Fast-but-lightweight" version of the solver.

        Algorithm features:
        * O(N^3) complexity
        * no additional time consuming features, just triangular solver

          ! COMMERCIAL EDITION OF ALGLIB:
          ! 
          ! Commercial Edition of ALGLIB includes following important improvements
          ! of this function:
          ! * high-performance native backend with same C# interface (C# version)
          ! * multithreading support (C++ and C# versions)
          ! * hardware vendor (Intel) implementations of linear algebra primitives
          !   (C++ and C# versions, x86/x64 platform)
          ! 
          ! We recommend you to read 'Working with commercial version' section  of
          ! ALGLIB Reference Manual in order to find out how to  use  performance-
          ! related features provided by commercial edition of ALGLIB.

        INPUT PARAMETERS:
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS:
            Info    -   return code:
                        * -3    matrix is exactly singular (ill conditioned matrices
                                are not recognized).
                        * -1    N<=0 was passed
                        *  1    task is solved 
            B       -   array[N]:
                        * info>0    =>  overwritten by solution
                        * info=-3   =>  filled by zeros

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixsolvefast(complex[,] a,
            int n,
            complex[] b,
            ref int info,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;
            int[] p = new int[0];

            a = (complex[,])a.Clone();
            info = 0;

            if( n<=0 )
            {
                info = -1;
                return;
            }
            trfac.cmatrixlu(ref a, n, n, ref p, _params);
            for(i=0; i<=n-1; i++)
            {
                if( a[i,i]==0 )
                {
                    for(j=0; j<=n-1; j++)
                    {
                        b[j] = 0.0;
                    }
                    info = -3;
                    return;
                }
            }
            cbasiclusolve(a, p, n, b, _params);
            info = 1;
        }


        /*************************************************************************
        Dense solver for A*X=B with N*N complex A given by its  LU  decomposition,
        and N*M matrices X and B (multiple right sides).   "Slow-but-feature-rich"
        version of the solver.

        Algorithm features:
        * automatic detection of degenerate cases
        * O(M*N^2) complexity
        * condition number estimation

        No iterative refinement  is provided because exact form of original matrix
        is not known to subroutine. Use CMatrixSolve or CMatrixMixedSolve  if  you
        need iterative refinement.

        IMPORTANT: ! this function is NOT the most efficient linear solver provided
                   ! by ALGLIB. It estimates condition  number  of  linear system,
                   ! which  results  in  significant  performance   penalty   when
                   ! compared with "fast"  version  which  just  calls  triangular
                   ! solver.
                   !
                   ! This performance penalty is especially apparent when you  use
                   ! ALGLIB parallel capabilities (condition number estimation  is
                   ! inherently  sequential).  It   also   becomes significant for
                   ! small-scale problems.
                   !
                   ! In such cases we strongly recommend you to use faster solver,
                   ! CMatrixLUSolveMFast() function.

          ! COMMERCIAL EDITION OF ALGLIB:
          ! 
          ! Commercial Edition of ALGLIB includes following important improvements
          ! of this function:
          ! * high-performance native backend with same C# interface (C# version)
          ! * multithreading support (C++ and C# versions)
          ! * hardware vendor (Intel) implementations of linear algebra primitives
          !   (C++ and C# versions, x86/x64 platform)
          ! 
          ! We recommend you to read 'Working with commercial version' section  of
          ! ALGLIB Reference Manual in order to find out how to  use  performance-
          ! related features provided by commercial edition of ALGLIB.

        INPUT PARAMETERS
            LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
            P       -   array[0..N-1], pivots array, RMatrixLU result
            N       -   size of A
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    matrix is very badly conditioned or exactly singular.
                        * -1    N<=0 was passed
                        *  1    task is solved (but matrix A may be ill-conditioned,
                                check R1/RInf parameters for condition numbers).
            Rep     -   additional report, following fields are set:
                        * rep.r1    condition number in 1-norm
                        * rep.rinf  condition number in inf-norm
            X       -   array[N,M], it contains:
                        * info>0    =>  solution
                        * info=-3   =>  filled by zeros

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixlusolvem(complex[,] lua,
            int[] p,
            int n,
            complex[,] b,
            int m,
            ref int info,
            densesolverreport rep,
            ref complex[,] x,
            alglib.xparams _params)
        {
            complex[,] emptya = new complex[0,0];

            info = 0;
            x = new complex[0,0];

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 || m<=0 )
            {
                info = -1;
                return;
            }
            
            //
            // solve
            //
            cmatrixlusolveinternal(lua, p, n, emptya, false, b, m, ref info, rep, ref x, _params);
        }


        /*************************************************************************
        Dense solver for A*X=B with N*N complex A given by its  LU  decomposition,
        and N*M matrices X and B (multiple  right  sides).  "Fast-but-lightweight"
        version of the solver.

        Algorithm features:
        * O(M*N^2) complexity
        * no additional time-consuming features

          ! COMMERCIAL EDITION OF ALGLIB:
          ! 
          ! Commercial Edition of ALGLIB includes following important improvements
          ! of this function:
          ! * high-performance native backend with same C# interface (C# version)
          ! * multithreading support (C++ and C# versions)
          ! * hardware vendor (Intel) implementations of linear algebra primitives
          !   (C++ and C# versions, x86/x64 platform)
          ! 
          ! We recommend you to read 'Working with commercial version' section  of
          ! ALGLIB Reference Manual in order to find out how to  use  performance-
          ! related features provided by commercial edition of ALGLIB.

        INPUT PARAMETERS
            LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
            P       -   array[0..N-1], pivots array, RMatrixLU result
            N       -   size of A
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    matrix is exactly singular (ill conditioned matrices
                                are not recognized).
                        * -1    N<=0 was passed
                        *  1    task is solved 
            B       -   array[N,M]:
                        * info>0    =>  overwritten by solution
                        * info=-3   =>  filled by zeros


          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixlusolvemfast(complex[,] lua,
            int[] p,
            int n,
            complex[,] b,
            int m,
            ref int info,
            alglib.xparams _params)
        {
            complex v = 0;
            int i = 0;
            int j = 0;
            int k = 0;

            info = 0;

            
            //
            // Check for exact degeneracy
            //
            if( n<=0 || m<=0 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                if( lua[i,i]==0 )
                {
                    for(j=0; j<=n-1; j++)
                    {
                        for(k=0; k<=m-1; k++)
                        {
                            b[j,k] = 0.0;
                        }
                    }
                    info = -3;
                    return;
                }
            }
            
            //
            // Solve with TRSM()
            //
            for(i=0; i<=n-1; i++)
            {
                if( p[i]!=i )
                {
                    for(j=0; j<=m-1; j++)
                    {
                        v = b[i,j];
                        b[i,j] = b[p[i],j];
                        b[p[i],j] = v;
                    }
                }
            }
            ablas.cmatrixlefttrsm(n, m, lua, 0, 0, false, true, 0, b, 0, 0, _params);
            ablas.cmatrixlefttrsm(n, m, lua, 0, 0, true, false, 0, b, 0, 0, _params);
            info = 1;
        }


        /*************************************************************************
        Complex dense linear solver for A*x=b with complex N*N A  given  by its LU
        decomposition and N*1 vectors x and b. This is  "slow-but-robust"  version
        of  the  complex  linear  solver  with  additional  features   which   add
        significant performance overhead. Faster version  is  CMatrixLUSolveFast()
        function.

        Algorithm features:
        * automatic detection of degenerate cases
        * O(N^2) complexity
        * condition number estimation

        No iterative refinement is provided because exact form of original matrix
        is not known to subroutine. Use CMatrixSolve or CMatrixMixedSolve  if  you
        need iterative refinement.

        IMPORTANT: ! this function is NOT the most efficient linear solver provided
                   ! by ALGLIB. It estimates condition  number  of  linear system,
                   ! which results in 10-15x  performance  penalty  when  compared
                   ! with "fast" version which just calls triangular solver.
                   !
                   ! This performance penalty is insignificant  when compared with
                   ! cost of large LU decomposition.  However,  if you  call  this
                   ! function many times for the same  left  side,  this  overhead
                   ! BECOMES significant. It  also  becomes significant for small-
                   ! scale problems.
                   !
                   ! In such cases we strongly recommend you to use faster solver,
                   ! CMatrixLUSolveFast() function.

        INPUT PARAMETERS
            LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
            P       -   array[0..N-1], pivots array, CMatrixLU result
            N       -   size of A
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    matrix is very badly conditioned or exactly singular.
                        * -1    N<=0 was passed
                        *  1    task is solved (but matrix A may be ill-conditioned,
                                check R1/RInf parameters for condition numbers).
            Rep     -   additional report, following fields are set:
                        * rep.r1    condition number in 1-norm
                        * rep.rinf  condition number in inf-norm
            X       -   array[N], it contains:
                        * info>0    =>  solution
                        * info=-3   =>  filled by zeros

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixlusolve(complex[,] lua,
            int[] p,
            int n,
            complex[] b,
            ref int info,
            densesolverreport rep,
            ref complex[] x,
            alglib.xparams _params)
        {
            complex[,] bm = new complex[0,0];
            complex[,] xm = new complex[0,0];
            int i_ = 0;

            info = 0;
            x = new complex[0];

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new complex[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            cmatrixlusolvem(lua, p, n, bm, 1, ref info, rep, ref xm, _params);
            x = new complex[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Complex dense linear solver for A*x=b with N*N complex A given by  its  LU
        decomposition and N*1 vectors x and b. This is  fast  lightweight  version
        of solver, which is significantly faster than CMatrixLUSolve(),  but  does
        not provide additional information (like condition numbers).

        Algorithm features:
        * O(N^2) complexity
        * no additional time-consuming features, just triangular solver

        INPUT PARAMETERS
            LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
            P       -   array[0..N-1], pivots array, CMatrixLU result
            N       -   size of A
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    matrix is exactly singular (ill conditioned matrices
                                are not recognized).
                        * -1    N<=0 was passed
                        *  1    task is solved 
            B       -   array[N]:
                        * info>0    =>  overwritten by solution
                        * info=-3   =>  filled by zeros
            
        NOTE: unlike  CMatrixLUSolve(),  this   function   does   NOT   check  for
              near-degeneracy of input matrix. It  checks  for  EXACT  degeneracy,
              because this check is easy to do. However,  very  badly  conditioned
              matrices may went unnoticed.


          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixlusolvefast(complex[,] lua,
            int[] p,
            int n,
            complex[] b,
            ref int info,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;

            info = 0;

            if( n<=0 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                if( lua[i,i]==0 )
                {
                    for(j=0; j<=n-1; j++)
                    {
                        b[j] = 0.0;
                    }
                    info = -3;
                    return;
                }
            }
            cbasiclusolve(lua, p, n, b, _params);
            info = 1;
        }


        /*************************************************************************
        Dense solver. Same as RMatrixMixedSolveM(), but for complex matrices.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * iterative refinement
        * O(M*N^2) complexity

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
            P       -   array[0..N-1], pivots array, CMatrixLU result
            N       -   size of A
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    matrix is very badly conditioned or exactly singular.
                        * -1    N<=0 was passed
                        *  1    task is solved (but matrix A may be ill-conditioned,
                                check R1/RInf parameters for condition numbers).
            Rep     -   additional report, following fields are set:
                        * rep.r1    condition number in 1-norm
                        * rep.rinf  condition number in inf-norm
            X       -   array[N,M], it contains:
                        * info>0    =>  solution
                        * info=-3   =>  filled by zeros

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixmixedsolvem(complex[,] a,
            complex[,] lua,
            int[] p,
            int n,
            complex[,] b,
            int m,
            ref int info,
            densesolverreport rep,
            ref complex[,] x,
            alglib.xparams _params)
        {
            info = 0;
            x = new complex[0,0];

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 || m<=0 )
            {
                info = -1;
                return;
            }
            
            //
            // solve
            //
            cmatrixlusolveinternal(lua, p, n, a, true, b, m, ref info, rep, ref x, _params);
        }


        /*************************************************************************
        Dense solver. Same as RMatrixMixedSolve(), but for complex matrices.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * iterative refinement
        * O(N^2) complexity

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
            P       -   array[0..N-1], pivots array, CMatrixLU result
            N       -   size of A
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    matrix is very badly conditioned or exactly singular.
                        * -1    N<=0 was passed
                        *  1    task is solved (but matrix A may be ill-conditioned,
                                check R1/RInf parameters for condition numbers).
            Rep     -   additional report, following fields are set:
                        * rep.r1    condition number in 1-norm
                        * rep.rinf  condition number in inf-norm
            X       -   array[N], it contains:
                        * info>0    =>  solution
                        * info=-3   =>  filled by zeros

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixmixedsolve(complex[,] a,
            complex[,] lua,
            int[] p,
            int n,
            complex[] b,
            ref int info,
            densesolverreport rep,
            ref complex[] x,
            alglib.xparams _params)
        {
            complex[,] bm = new complex[0,0];
            complex[,] xm = new complex[0,0];
            int i_ = 0;

            info = 0;
            x = new complex[0];

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new complex[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            cmatrixmixedsolvem(a, lua, p, n, bm, 1, ref info, rep, ref xm, _params);
            x = new complex[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver for A*X=B with N*N symmetric positive definite matrix A,  and
        N*M vectors X and B. It is "slow-but-feature-rich" version of the solver.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * O(N^3+M*N^2) complexity
        * matrix is represented by its upper or lower triangle

        No iterative refinement is provided because such partial representation of
        matrix does not allow efficient calculation of extra-precise  matrix-vector
        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        IMPORTANT: ! this function is NOT the most efficient linear solver provided
                   ! by ALGLIB. It estimates condition  number  of  linear system,
                   ! which  results  in  significant   performance   penalty  when
                   ! compared with "fast" version  which  just  performs  Cholesky
                   ! decomposition and calls triangular solver.
                   !
                   ! This  performance  penalty  is  especially  visible  in   the
                   ! multithreaded mode, because both condition number  estimation
                   ! and   iterative    refinement   are   inherently   sequential
                   ! calculations.
                   !
                   ! Thus, if you need high performance and if you are pretty sure
                   ! that your system is well conditioned, we  strongly  recommend
                   ! you to use faster solver, SPDMatrixSolveMFast() function.

          ! COMMERCIAL EDITION OF ALGLIB:
          ! 
          ! Commercial Edition of ALGLIB includes following important improvements
          ! of this function:
          ! * high-performance native backend with same C# interface (C# version)
          ! * multithreading support (C++ and C# versions)
          ! * hardware vendor (Intel) implementations of linear algebra primitives
          !   (C++ and C# versions, x86/x64 platform)
          ! 
          ! We recommend you to read 'Working with commercial version' section  of
          ! ALGLIB Reference Manual in order to find out how to  use  performance-
          ! related features provided by commercial edition of ALGLIB.

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            IsUpper -   what half of A is provided
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    matrix is very badly conditioned or non-SPD.
                        * -1    N<=0 was passed
                        *  1    task is solved (but matrix A may be ill-conditioned,
                                check R1/RInf parameters for condition numbers).
            Rep     -   additional report, following fields are set:
                        * rep.r1    condition number in 1-norm
                        * rep.rinf  condition number in inf-norm
            X       -   array[N,M], it contains:
                        * info>0    =>  solution
                        * info=-3   =>  filled by zeros

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void spdmatrixsolvem(double[,] a,
            int n,
            bool isupper,
            double[,] b,
            int m,
            ref int info,
            densesolverreport rep,
            ref double[,] x,
            alglib.xparams _params)
        {
            double[,] da = new double[0,0];
            int i = 0;
            int j = 0;
            int j1 = 0;
            int j2 = 0;
            int i_ = 0;

            info = 0;
            x = new double[0,0];

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 || m<=0 )
            {
                info = -1;
                return;
            }
            da = new double[n, n];
            
            //
            // factorize
            // solve
            //
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
                for(i_=j1; i_<=j2;i_++)
                {
                    da[i,i_] = a[i,i_];
                }
            }
            if( !trfac.spdmatrixcholesky(ref da, n, isupper, _params) )
            {
                x = new double[n, m];
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x[i,j] = 0;
                    }
                }
                rep.r1 = 0;
                rep.rinf = 0;
                info = -3;
                return;
            }
            info = 1;
            spdmatrixcholeskysolveinternal(da, n, isupper, a, true, b, m, ref info, rep, ref x, _params);
        }


        /*************************************************************************
        Dense solver for A*X=B with N*N symmetric positive definite matrix A,  and
        N*M vectors X and B. It is "fast-but-lightweight" version of the solver.

        Algorithm features:
        * O(N^3+M*N^2) complexity
        * matrix is represented by its upper or lower triangle
        * no additional time consuming features

          ! COMMERCIAL EDITION OF ALGLIB:
          ! 
          ! Commercial Edition of ALGLIB includes following important improvements
          ! of this function:
          ! * high-performance native backend with same C# interface (C# version)
          ! * multithreading support (C++ and C# versions)
          ! * hardware vendor (Intel) implementations of linear algebra primitives
          !   (C++ and C# versions, x86/x64 platform)
          ! 
          ! We recommend you to read 'Working with commercial version' section  of
          ! ALGLIB Reference Manual in order to find out how to  use  performance-
          ! related features provided by commercial edition of ALGLIB.

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            IsUpper -   what half of A is provided
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    A is is exactly singular
                        * -1    N<=0 was passed
                        *  1    task was solved
            B       -   array[N,M], it contains:
                        * info>0    =>  solution
                        * info=-3   =>  filled by zeros

          -- ALGLIB --
             Copyright 17.03.2015 by Bochkanov Sergey
        *************************************************************************/
        public static void spdmatrixsolvemfast(double[,] a,
            int n,
            bool isupper,
            double[,] b,
            int m,
            ref int info,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;

            a = (double[,])a.Clone();
            info = 0;

            info = 1;
            if( n<=0 )
            {
                info = -1;
                return;
            }
            if( !trfac.spdmatrixcholesky(ref a, n, isupper, _params) )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        b[i,j] = 0.0;
                    }
                }
                info = -3;
                return;
            }
            if( isupper )
            {
                ablas.rmatrixlefttrsm(n, m, a, 0, 0, true, false, 1, b, 0, 0, _params);
                ablas.rmatrixlefttrsm(n, m, a, 0, 0, true, false, 0, b, 0, 0, _params);
            }
            else
            {
                ablas.rmatrixlefttrsm(n, m, a, 0, 0, false, false, 0, b, 0, 0, _params);
                ablas.rmatrixlefttrsm(n, m, a, 0, 0, false, false, 1, b, 0, 0, _params);
            }
        }


        /*************************************************************************
        Dense linear solver for A*x=b with N*N real  symmetric  positive  definite
        matrix A,  N*1 vectors x and b.  "Slow-but-feature-rich"  version  of  the
        solver.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * O(N^3) complexity
        * matrix is represented by its upper or lower triangle

        No iterative refinement is provided because such partial representation of
        matrix does not allow efficient calculation of extra-precise  matrix-vector
        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        IMPORTANT: ! this function is NOT the most efficient linear solver provided
                   ! by ALGLIB. It estimates condition  number  of  linear system,
                   ! which  results  in  significant   performance   penalty  when
                   ! compared with "fast" version  which  just  performs  Cholesky
                   ! decomposition and calls triangular solver.
                   !
                   ! This  performance  penalty  is  especially  visible  in   the
                   ! multithreaded mode, because both condition number  estimation
                   ! and   iterative    refinement   are   inherently   sequential
                   ! calculations.
                   !
                   ! Thus, if you need high performance and if you are pretty sure
                   ! that your system is well conditioned, we  strongly  recommend
                   ! you to use faster solver, SPDMatrixSolveFast() function.

          ! COMMERCIAL EDITION OF ALGLIB:
          ! 
          ! Commercial Edition of ALGLIB includes following important improvements
          ! of this function:
          ! * high-performance native backend with same C# interface (C# version)
          ! * multithreading support (C++ and C# versions)
          ! * hardware vendor (Intel) implementations of linear algebra primitives
          !   (C++ and C# versions, x86/x64 platform)
          ! 
          ! We recommend you to read 'Working with commercial version' section  of
          ! ALGLIB Reference Manual in order to find out how to  use  performance-
          ! related features provided by commercial edition of ALGLIB.

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            IsUpper -   what half of A is provided
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    matrix is very badly conditioned or non-SPD.
                        * -1    N<=0 was passed
                        *  1    task is solved (but matrix A may be ill-conditioned,
                                check R1/RInf parameters for condition numbers).
            Rep     -   additional report, following fields are set:
                        * rep.r1    condition number in 1-norm
                        * rep.rinf  condition number in inf-norm
            X       -   array[N], it contains:
                        * info>0    =>  solution
                        * info=-3   =>  filled by zeros

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void spdmatrixsolve(double[,] a,
            int n,
            bool isupper,
            double[] b,
            ref int info,
            densesolverreport rep,
            ref double[] x,
            alglib.xparams _params)
        {
            double[,] bm = new double[0,0];
            double[,] xm = new double[0,0];
            int i_ = 0;

            info = 0;
            x = new double[0];

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new double[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            spdmatrixsolvem(a, n, isupper, bm, 1, ref info, rep, ref xm, _params);
            x = new double[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense linear solver for A*x=b with N*N real  symmetric  positive  definite
        matrix A,  N*1 vectors x and  b.  "Fast-but-lightweight"  version  of  the
        solver.

        Algorithm features:
        * O(N^3) complexity
        * matrix is represented by its upper or lower triangle
        * no additional time consuming features like condition number estimation

          ! COMMERCIAL EDITION OF ALGLIB:
          ! 
          ! Commercial Edition of ALGLIB includes following important improvements
          ! of this function:
          ! * high-performance native backend with same C# interface (C# version)
          ! * multithreading support (C++ and C# versions)
          ! * hardware vendor (Intel) implementations of linear algebra primitives
          !   (C++ and C# versions, x86/x64 platform)
          ! 
          ! We recommend you to read 'Working with commercial version' section  of
          ! ALGLIB Reference Manual in order to find out how to  use  performance-
          ! related features provided by commercial edition of ALGLIB.

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            IsUpper -   what half of A is provided
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    A is is exactly singular or non-SPD
                        * -1    N<=0 was passed
                        *  1    task was solved
            B       -   array[N], it contains:
                        * info>0    =>  solution
                        * info=-3   =>  filled by zeros

          -- ALGLIB --
             Copyright 17.03.2015 by Bochkanov Sergey
        *************************************************************************/
        public static void spdmatrixsolvefast(double[,] a,
            int n,
            bool isupper,
            double[] b,
            ref int info,
            alglib.xparams _params)
        {
            int i = 0;

            a = (double[,])a.Clone();
            info = 0;

            info = 1;
            if( n<=0 )
            {
                info = -1;
                return;
            }
            if( !trfac.spdmatrixcholesky(ref a, n, isupper, _params) )
            {
                for(i=0; i<=n-1; i++)
                {
                    b[i] = 0.0;
                }
                info = -3;
                return;
            }
            spdbasiccholeskysolve(a, n, isupper, b, _params);
        }


        /*************************************************************************
        Dense solver for A*X=B with N*N symmetric positive definite matrix A given
        by its Cholesky decomposition, and N*M vectors X and B. It  is  "slow-but-
        feature-rich" version of the solver which estimates  condition  number  of
        the system.

        Algorithm features:
        * automatic detection of degenerate cases
        * O(M*N^2) complexity
        * condition number estimation
        * matrix is represented by its upper or lower triangle

        No iterative refinement is provided because such partial representation of
        matrix does not allow efficient calculation of extra-precise  matrix-vector
        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        IMPORTANT: ! this function is NOT the most efficient linear solver provided
                   ! by ALGLIB. It estimates condition  number  of  linear system,
                   ! which  results  in  significant  performance   penalty   when
                   ! compared with "fast"  version  which  just  calls  triangular
                   ! solver. Amount of  overhead  introduced  depends  on  M  (the
                   ! larger - the more efficient).
                   !
                   ! This performance penalty is insignificant  when compared with
                   ! cost of large LU decomposition.  However,  if you  call  this
                   ! function many times for the same  left  side,  this  overhead
                   ! BECOMES significant. It  also  becomes significant for small-
                   ! scale problems (N<50).
                   !
                   ! In such cases we strongly recommend you to use faster solver,
                   ! SPDMatrixCholeskySolveMFast() function.

        INPUT PARAMETERS
            CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
                        SPDMatrixCholesky result
            N       -   size of CHA
            IsUpper -   what half of CHA is provided
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    A is is exactly singular or badly conditioned
                                X is filled by zeros in such cases.
                        * -1    N<=0 was passed
                        *  1    task was solved
            Rep     -   additional report, following fields are set:
                        * rep.r1    condition number in 1-norm
                        * rep.rinf  condition number in inf-norm
            X       -   array[N]:
                        * for info>0 contains solution
                        * for info=-3 filled by zeros

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void spdmatrixcholeskysolvem(double[,] cha,
            int n,
            bool isupper,
            double[,] b,
            int m,
            ref int info,
            densesolverreport rep,
            ref double[,] x,
            alglib.xparams _params)
        {
            double[,] emptya = new double[0,0];

            info = 0;
            x = new double[0,0];

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 || m<=0 )
            {
                info = -1;
                return;
            }
            
            //
            // solve
            //
            spdmatrixcholeskysolveinternal(cha, n, isupper, emptya, false, b, m, ref info, rep, ref x, _params);
        }


        /*************************************************************************
        Dense solver for A*X=B with N*N symmetric positive definite matrix A given
        by its Cholesky decomposition, and N*M vectors X and B. It  is  "fast-but-
        lightweight" version of  the  solver  which  just  solves  linear  system,
        without any additional functions.

        Algorithm features:
        * O(M*N^2) complexity
        * matrix is represented by its upper or lower triangle
        * no additional functionality

        INPUT PARAMETERS
            CHA     -   array[N,N], Cholesky decomposition,
                        SPDMatrixCholesky result
            N       -   size of CHA
            IsUpper -   what half of CHA is provided
            B       -   array[N,M], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    A is is exactly singular or badly conditioned
                                X is filled by zeros in such cases.
                        * -1    N<=0 was passed
                        *  1    task was solved
            B       -   array[N]:
                        * for info>0 overwritten by solution
                        * for info=-3 filled by zeros

          -- ALGLIB --
             Copyright 18.03.2015 by Bochkanov Sergey
        *************************************************************************/
        public static void spdmatrixcholeskysolvemfast(double[,] cha,
            int n,
            bool isupper,
            double[,] b,
            int m,
            ref int info,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;
            int k = 0;

            info = 0;

            info = 1;
            if( n<=0 )
            {
                info = -1;
                return;
            }
            for(k=0; k<=n-1; k++)
            {
                if( (double)(cha[k,k])==(double)(0.0) )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            b[i,j] = 0.0;
                        }
                    }
                    info = -3;
                    return;
                }
            }
            if( isupper )
            {
                ablas.rmatrixlefttrsm(n, m, cha, 0, 0, true, false, 1, b, 0, 0, _params);
                ablas.rmatrixlefttrsm(n, m, cha, 0, 0, true, false, 0, b, 0, 0, _params);
            }
            else
            {
                ablas.rmatrixlefttrsm(n, m, cha, 0, 0, false, false, 0, b, 0, 0, _params);
                ablas.rmatrixlefttrsm(n, m, cha, 0, 0, false, false, 1, b, 0, 0, _params);
            }
        }


        /*************************************************************************
        Dense solver for A*x=b with N*N symmetric positive definite matrix A given
        by its Cholesky decomposition, and N*1 real vectors x and b. This is "slow-
        but-feature-rich"  version  of  the  solver  which,  in  addition  to  the
        solution, performs condition number estimation.

        Algorithm features:
        * automatic detection of degenerate cases
        * O(N^2) complexity
        * condition number estimation
        * matrix is represented by its upper or lower triangle

        No iterative refinement is provided because such partial representation of
        matrix does not allow efficient calculation of extra-precise  matrix-vector
        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        IMPORTANT: ! this function is NOT the most efficient linear solver provided
                   ! by ALGLIB. It estimates condition  number  of  linear system,
                   ! which results in 10-15x  performance  penalty  when  compared
                   ! with "fast" version which just calls triangular solver.
                   !
                   ! This performance penalty is insignificant  when compared with
                   ! cost of large LU decomposition.  However,  if you  call  this
                   ! function many times for the same  left  side,  this  overhead
                   ! BECOMES significant. It  also  becomes significant for small-
                   ! scale problems (N<50).
                   !
                   ! In such cases we strongly recommend you to use faster solver,
                   ! SPDMatrixCholeskySolveFast() function.

        INPUT PARAMETERS
            CHA     -   array[N,N], Cholesky decomposition,
                        SPDMatrixCholesky result
            N       -   size of A
            IsUpper -   what half of CHA is provided
            B       -   array[N], right part

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    A is is exactly singular or ill conditioned
                                X is filled by zeros in such cases.
                        * -1    N<=0 was passed
                        *  1    task is solved
            Rep     -   additional report, following fields are set:
                        * rep.r1    condition number in 1-norm
                        * rep.rinf  condition number in inf-norm
            X       -   array[N]:
                        * for info>0  - solution
                        * for info=-3 - filled by zeros

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void spdmatrixcholeskysolve(double[,] cha,
            int n,
            bool isupper,
            double[] b,
            ref int info,
            densesolverreport rep,
            ref double[] x,
            alglib.xparams _params)
        {
            double[,] bm = new double[0,0];
            double[,] xm = new double[0,0];
            int i_ = 0;

            info = 0;
            x = new double[0];

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new double[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            spdmatrixcholeskysolvem(cha, n, isupper, bm, 1, ref info, rep, ref xm, _params);
            x = new double[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver for A*x=b with N*N symmetric positive definite matrix A given
        by its Cholesky decomposition, and N*1 real vectors x and b. This is "fast-
        but-lightweight" version of the solver.

        Algorithm features:
        * O(N^2) complexity
        * matrix is represented by its upper or lower triangle
        * no additional features

        INPUT PARAMETERS
            CHA     -   array[N,N], Cholesky decomposition,
                        SPDMatrixCholesky result
            N       -   size of A
            IsUpper -   what half of CHA is provided
            B       -   array[N], right part

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    A is is exactly singular or ill conditioned
                                X is filled by zeros in such cases.
                        * -1    N<=0 was passed
                        *  1    task is solved
            B       -   array[N]:
                        * for info>0  - overwritten by solution
                        * for info=-3 - filled by zeros

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void spdmatrixcholeskysolvefast(double[,] cha,
            int n,
            bool isupper,
            double[] b,
            ref int info,
            alglib.xparams _params)
        {
            int i = 0;
            int k = 0;

            info = 0;

            info = 1;
            if( n<=0 )
            {
                info = -1;
                return;
            }
            for(k=0; k<=n-1; k++)
            {
                if( (double)(cha[k,k])==(double)(0.0) )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        b[i] = 0.0;
                    }
                    info = -3;
                    return;
                }
            }
            spdbasiccholeskysolve(cha, n, isupper, b, _params);
        }


        /*************************************************************************
        Dense solver for A*X=B, with N*N Hermitian positive definite matrix A  and
        N*M  complex  matrices  X  and  B.  "Slow-but-feature-rich" version of the
        solver.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * O(N^3+M*N^2) complexity
        * matrix is represented by its upper or lower triangle

        No iterative refinement is provided because such partial representation of
        matrix does not allow efficient calculation of extra-precise  matrix-vector
        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        IMPORTANT: ! this function is NOT the most efficient linear solver provided
                   ! by ALGLIB. It estimates condition  number  of  linear system,
                   ! which  results  in  significant  performance   penalty   when
                   ! compared with "fast"  version  which  just  calls  triangular
                   ! solver.
                   !
                   ! This performance penalty is especially apparent when you  use
                   ! ALGLIB parallel capabilities (condition number estimation  is
                   ! inherently  sequential).  It   also   becomes significant for
                   ! small-scale problems (N<100).
                   !
                   ! In such cases we strongly recommend you to use faster solver,
                   ! HPDMatrixSolveMFast() function.

          ! COMMERCIAL EDITION OF ALGLIB:
          ! 
          ! Commercial Edition of ALGLIB includes following important improvements
          ! of this function:
          ! * high-performance native backend with same C# interface (C# version)
          ! * multithreading support (C++ and C# versions)
          ! * hardware vendor (Intel) implementations of linear algebra primitives
          !   (C++ and C# versions, x86/x64 platform)
          ! 
          ! We recommend you to read 'Working with commercial version' section  of
          ! ALGLIB Reference Manual in order to find out how to  use  performance-
          ! related features provided by commercial edition of ALGLIB.

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            IsUpper -   what half of A is provided
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve.
                        Returns -3 for non-HPD matrices.
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void hpdmatrixsolvem(complex[,] a,
            int n,
            bool isupper,
            complex[,] b,
            int m,
            ref int info,
            densesolverreport rep,
            ref complex[,] x,
            alglib.xparams _params)
        {
            complex[,] da = new complex[0,0];
            int i = 0;
            int j = 0;
            int j1 = 0;
            int j2 = 0;
            int i_ = 0;

            info = 0;
            x = new complex[0,0];

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 || m<=0 )
            {
                info = -1;
                return;
            }
            da = new complex[n, n];
            
            //
            // factorize matrix, solve
            //
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
                for(i_=j1; i_<=j2;i_++)
                {
                    da[i,i_] = a[i,i_];
                }
            }
            if( !trfac.hpdmatrixcholesky(ref da, n, isupper, _params) )
            {
                x = new complex[n, m];
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x[i,j] = 0;
                    }
                }
                rep.r1 = 0;
                rep.rinf = 0;
                info = -3;
                return;
            }
            info = 1;
            hpdmatrixcholeskysolveinternal(da, n, isupper, a, true, b, m, ref info, rep, ref x, _params);
        }


        /*************************************************************************
        Dense solver for A*X=B, with N*N Hermitian positive definite matrix A  and
        N*M complex matrices X and B. "Fast-but-lightweight" version of the solver.

        Algorithm features:
        * O(N^3+M*N^2) complexity
        * matrix is represented by its upper or lower triangle
        * no additional time consuming features like condition number estimation

          ! COMMERCIAL EDITION OF ALGLIB:
          ! 
          ! Commercial Edition of ALGLIB includes following important improvements
          ! of this function:
          ! * high-performance native backend with same C# interface (C# version)
          ! * multithreading support (C++ and C# versions)
          ! * hardware vendor (Intel) implementations of linear algebra primitives
          !   (C++ and C# versions, x86/x64 platform)
          ! 
          ! We recommend you to read 'Working with commercial version' section  of
          ! ALGLIB Reference Manual in order to find out how to  use  performance-
          ! related features provided by commercial edition of ALGLIB.

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            IsUpper -   what half of A is provided
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    A is is exactly  singular or is not positive definite.
                                B is filled by zeros in such cases.
                        * -1    N<=0 was passed
                        *  1    task is solved
            B       -   array[0..N-1]:
                        * overwritten by solution
                        * zeros, if problem was not solved

          -- ALGLIB --
             Copyright 17.03.2015 by Bochkanov Sergey
        *************************************************************************/
        public static void hpdmatrixsolvemfast(complex[,] a,
            int n,
            bool isupper,
            complex[,] b,
            int m,
            ref int info,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;

            a = (complex[,])a.Clone();
            info = 0;

            info = 1;
            if( n<=0 )
            {
                info = -1;
                return;
            }
            if( !trfac.hpdmatrixcholesky(ref a, n, isupper, _params) )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        b[i,j] = 0.0;
                    }
                }
                info = -3;
                return;
            }
            if( isupper )
            {
                ablas.cmatrixlefttrsm(n, m, a, 0, 0, true, false, 2, b, 0, 0, _params);
                ablas.cmatrixlefttrsm(n, m, a, 0, 0, true, false, 0, b, 0, 0, _params);
            }
            else
            {
                ablas.cmatrixlefttrsm(n, m, a, 0, 0, false, false, 0, b, 0, 0, _params);
                ablas.cmatrixlefttrsm(n, m, a, 0, 0, false, false, 2, b, 0, 0, _params);
            }
        }


        /*************************************************************************
        Dense solver for A*x=b, with N*N Hermitian positive definite matrix A, and
        N*1 complex vectors  x  and  b.  "Slow-but-feature-rich"  version  of  the
        solver.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * O(N^3) complexity
        * matrix is represented by its upper or lower triangle

        No iterative refinement is provided because such partial representation of
        matrix does not allow efficient calculation of extra-precise  matrix-vector
        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        IMPORTANT: ! this function is NOT the most efficient linear solver provided
                   ! by ALGLIB. It estimates condition  number  of  linear system,
                   ! which  results  in  significant   performance   penalty  when
                   ! compared with "fast" version  which  just  performs  Cholesky
                   ! decomposition and calls triangular solver.
                   !
                   ! This  performance  penalty  is  especially  visible  in   the
                   ! multithreaded mode, because both condition number  estimation
                   ! and   iterative    refinement   are   inherently   sequential
                   ! calculations.
                   !
                   ! Thus, if you need high performance and if you are pretty sure
                   ! that your system is well conditioned, we  strongly  recommend
                   ! you to use faster solver, HPDMatrixSolveFast() function.

          ! COMMERCIAL EDITION OF ALGLIB:
          ! 
          ! Commercial Edition of ALGLIB includes following important improvements
          ! of this function:
          ! * high-performance native backend with same C# interface (C# version)
          ! * multithreading support (C++ and C# versions)
          ! * hardware vendor (Intel) implementations of linear algebra primitives
          !   (C++ and C# versions, x86/x64 platform)
          ! 
          ! We recommend you to read 'Working with commercial version' section  of
          ! ALGLIB Reference Manual in order to find out how to  use  performance-
          ! related features provided by commercial edition of ALGLIB.

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            IsUpper -   what half of A is provided
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
                        Returns -3 for non-HPD matrices.
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void hpdmatrixsolve(complex[,] a,
            int n,
            bool isupper,
            complex[] b,
            ref int info,
            densesolverreport rep,
            ref complex[] x,
            alglib.xparams _params)
        {
            complex[,] bm = new complex[0,0];
            complex[,] xm = new complex[0,0];
            int i_ = 0;

            info = 0;
            x = new complex[0];

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new complex[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            hpdmatrixsolvem(a, n, isupper, bm, 1, ref info, rep, ref xm, _params);
            x = new complex[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver for A*x=b, with N*N Hermitian positive definite matrix A, and
        N*1 complex vectors  x  and  b.  "Fast-but-lightweight"  version  of   the
        solver without additional functions.

        Algorithm features:
        * O(N^3) complexity
        * matrix is represented by its upper or lower triangle
        * no additional time consuming functions

          ! COMMERCIAL EDITION OF ALGLIB:
          ! 
          ! Commercial Edition of ALGLIB includes following important improvements
          ! of this function:
          ! * high-performance native backend with same C# interface (C# version)
          ! * multithreading support (C++ and C# versions)
          ! * hardware vendor (Intel) implementations of linear algebra primitives
          !   (C++ and C# versions, x86/x64 platform)
          ! 
          ! We recommend you to read 'Working with commercial version' section  of
          ! ALGLIB Reference Manual in order to find out how to  use  performance-
          ! related features provided by commercial edition of ALGLIB.

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            IsUpper -   what half of A is provided
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    A is is exactly singular or not positive definite
                                X is filled by zeros in such cases.
                        * -1    N<=0 was passed
                        *  1    task was solved 
            B       -   array[0..N-1]:
                        * overwritten by solution
                        * zeros, if A is exactly singular (diagonal of its LU
                          decomposition has exact zeros).

          -- ALGLIB --
             Copyright 17.03.2015 by Bochkanov Sergey
        *************************************************************************/
        public static void hpdmatrixsolvefast(complex[,] a,
            int n,
            bool isupper,
            complex[] b,
            ref int info,
            alglib.xparams _params)
        {
            int i = 0;

            a = (complex[,])a.Clone();
            info = 0;

            info = 1;
            if( n<=0 )
            {
                info = -1;
                return;
            }
            if( !trfac.hpdmatrixcholesky(ref a, n, isupper, _params) )
            {
                for(i=0; i<=n-1; i++)
                {
                    b[i] = 0.0;
                }
                info = -3;
                return;
            }
            hpdbasiccholeskysolve(a, n, isupper, b, _params);
        }


        /*************************************************************************
        Dense solver for A*X=B with N*N Hermitian positive definite matrix A given
        by its Cholesky decomposition and N*M complex matrices X  and  B.  This is
        "slow-but-feature-rich" version of the solver which, in  addition  to  the
        solution, estimates condition number of the system.

        Algorithm features:
        * automatic detection of degenerate cases
        * O(M*N^2) complexity
        * condition number estimation
        * matrix is represented by its upper or lower triangle

        No iterative refinement is provided because such partial representation of
        matrix does not allow efficient calculation of extra-precise  matrix-vector
        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        IMPORTANT: ! this function is NOT the most efficient linear solver provided
                   ! by ALGLIB. It estimates condition  number  of  linear system,
                   ! which  results  in  significant  performance   penalty   when
                   ! compared with "fast"  version  which  just  calls  triangular
                   ! solver. Amount of  overhead  introduced  depends  on  M  (the
                   ! larger - the more efficient).
                   !
                   ! This performance penalty is insignificant  when compared with
                   ! cost of large Cholesky decomposition.  However,  if  you call
                   ! this  function  many  times  for  the same  left  side,  this
                   ! overhead BECOMES significant. It  also   becomes  significant
                   ! for small-scale problems (N<50).
                   !
                   ! In such cases we strongly recommend you to use faster solver,
                   ! HPDMatrixCholeskySolveMFast() function.


        INPUT PARAMETERS
            CHA     -   array[N,N], Cholesky decomposition,
                        HPDMatrixCholesky result
            N       -   size of CHA
            IsUpper -   what half of CHA is provided
            B       -   array[N,M], right part
            M       -   right part size

        OUTPUT PARAMETERS:
            Info    -   return code:
                        * -3    A is singular, or VERY close to singular.
                                X is filled by zeros in such cases.
                        * -1    N<=0 was passed
                        *  1    task was solved
            Rep     -   additional report, following fields are set:
                        * rep.r1    condition number in 1-norm
                        * rep.rinf  condition number in inf-norm
            X       -   array[N]:
                        * for info>0 contains solution
                        * for info=-3 filled by zeros

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void hpdmatrixcholeskysolvem(complex[,] cha,
            int n,
            bool isupper,
            complex[,] b,
            int m,
            ref int info,
            densesolverreport rep,
            ref complex[,] x,
            alglib.xparams _params)
        {
            complex[,] emptya = new complex[0,0];

            info = 0;
            x = new complex[0,0];

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 || m<=0 )
            {
                info = -1;
                return;
            }
            
            //
            // 1. scale matrix, max(|U[i,j]|)
            // 2. factorize scaled matrix
            // 3. solve
            //
            hpdmatrixcholeskysolveinternal(cha, n, isupper, emptya, false, b, m, ref info, rep, ref x, _params);
        }


        /*************************************************************************
        Dense solver for A*X=B with N*N Hermitian positive definite matrix A given
        by its Cholesky decomposition and N*M complex matrices X  and  B.  This is
        "fast-but-lightweight" version of the solver.

        Algorithm features:
        * O(M*N^2) complexity
        * matrix is represented by its upper or lower triangle
        * no additional time-consuming features

        INPUT PARAMETERS
            CHA     -   array[N,N], Cholesky decomposition,
                        HPDMatrixCholesky result
            N       -   size of CHA
            IsUpper -   what half of CHA is provided
            B       -   array[N,M], right part
            M       -   right part size

        OUTPUT PARAMETERS:
            Info    -   return code:
                        * -3    A is singular, or VERY close to singular.
                                X is filled by zeros in such cases.
                        * -1    N<=0 was passed
                        *  1    task was solved
            B       -   array[N]:
                        * for info>0 overwritten by solution
                        * for info=-3 filled by zeros

          -- ALGLIB --
             Copyright 18.03.2015 by Bochkanov Sergey
        *************************************************************************/
        public static void hpdmatrixcholeskysolvemfast(complex[,] cha,
            int n,
            bool isupper,
            complex[,] b,
            int m,
            ref int info,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;
            int k = 0;

            info = 0;

            info = 1;
            if( n<=0 )
            {
                info = -1;
                return;
            }
            for(k=0; k<=n-1; k++)
            {
                if( (double)(cha[k,k].x)==(double)(0.0) && (double)(cha[k,k].y)==(double)(0.0) )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            b[i,j] = 0.0;
                        }
                    }
                    info = -3;
                    return;
                }
            }
            if( isupper )
            {
                ablas.cmatrixlefttrsm(n, m, cha, 0, 0, true, false, 2, b, 0, 0, _params);
                ablas.cmatrixlefttrsm(n, m, cha, 0, 0, true, false, 0, b, 0, 0, _params);
            }
            else
            {
                ablas.cmatrixlefttrsm(n, m, cha, 0, 0, false, false, 0, b, 0, 0, _params);
                ablas.cmatrixlefttrsm(n, m, cha, 0, 0, false, false, 2, b, 0, 0, _params);
            }
        }


        /*************************************************************************
        Dense solver for A*x=b with N*N Hermitian positive definite matrix A given
        by its Cholesky decomposition, and N*1 complex vectors x and  b.  This  is
        "slow-but-feature-rich" version of the solver  which  estimates  condition
        number of the system.

        Algorithm features:
        * automatic detection of degenerate cases
        * O(N^2) complexity
        * condition number estimation
        * matrix is represented by its upper or lower triangle

        No iterative refinement is provided because such partial representation of
        matrix does not allow efficient calculation of extra-precise  matrix-vector
        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        IMPORTANT: ! this function is NOT the most efficient linear solver provided
                   ! by ALGLIB. It estimates condition  number  of  linear system,
                   ! which results in 10-15x  performance  penalty  when  compared
                   ! with "fast" version which just calls triangular solver.
                   !
                   ! This performance penalty is insignificant  when compared with
                   ! cost of large LU decomposition.  However,  if you  call  this
                   ! function many times for the same  left  side,  this  overhead
                   ! BECOMES significant. It  also  becomes significant for small-
                   ! scale problems (N<50).
                   !
                   ! In such cases we strongly recommend you to use faster solver,
                   ! HPDMatrixCholeskySolveFast() function.

        INPUT PARAMETERS
            CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
                        SPDMatrixCholesky result
            N       -   size of A
            IsUpper -   what half of CHA is provided
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    A is is exactly singular or ill conditioned
                                X is filled by zeros in such cases.
                        * -1    N<=0 was passed
                        *  1    task is solved
            Rep     -   additional report, following fields are set:
                        * rep.r1    condition number in 1-norm
                        * rep.rinf  condition number in inf-norm
            X       -   array[N]:
                        * for info>0  - solution
                        * for info=-3 - filled by zeros

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void hpdmatrixcholeskysolve(complex[,] cha,
            int n,
            bool isupper,
            complex[] b,
            ref int info,
            densesolverreport rep,
            ref complex[] x,
            alglib.xparams _params)
        {
            complex[,] bm = new complex[0,0];
            complex[,] xm = new complex[0,0];
            int i_ = 0;

            info = 0;
            x = new complex[0];

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new complex[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            hpdmatrixcholeskysolvem(cha, n, isupper, bm, 1, ref info, rep, ref xm, _params);
            x = new complex[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver for A*x=b with N*N Hermitian positive definite matrix A given
        by its Cholesky decomposition, and N*1 complex vectors x and  b.  This  is
        "fast-but-lightweight" version of the solver.

        Algorithm features:
        * O(N^2) complexity
        * matrix is represented by its upper or lower triangle
        * no additional time-consuming features

        INPUT PARAMETERS
            CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
                        SPDMatrixCholesky result
            N       -   size of A
            IsUpper -   what half of CHA is provided
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    A is is exactly singular or ill conditioned
                                B is filled by zeros in such cases.
                        * -1    N<=0 was passed
                        *  1    task is solved
            B       -   array[N]:
                        * for info>0  - overwritten by solution
                        * for info=-3 - filled by zeros

          -- ALGLIB --
             Copyright 18.03.2015 by Bochkanov Sergey
        *************************************************************************/
        public static void hpdmatrixcholeskysolvefast(complex[,] cha,
            int n,
            bool isupper,
            complex[] b,
            ref int info,
            alglib.xparams _params)
        {
            int i = 0;
            int k = 0;

            info = 0;

            info = 1;
            if( n<=0 )
            {
                info = -1;
                return;
            }
            for(k=0; k<=n-1; k++)
            {
                if( (double)(cha[k,k].x)==(double)(0.0) && (double)(cha[k,k].y)==(double)(0.0) )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        b[i] = 0.0;
                    }
                    info = -3;
                    return;
                }
            }
            hpdbasiccholeskysolve(cha, n, isupper, b, _params);
        }


        /*************************************************************************
        Dense solver.

        This subroutine finds solution of the linear system A*X=B with non-square,
        possibly degenerate A.  System  is  solved in the least squares sense, and
        general least squares solution  X = X0 + CX*y  which  minimizes |A*X-B| is
        returned. If A is non-degenerate, solution in the usual sense is returned.

        Algorithm features:
        * automatic detection (and correct handling!) of degenerate cases
        * iterative refinement
        * O(N^3) complexity

          ! COMMERCIAL EDITION OF ALGLIB:
          ! 
          ! Commercial Edition of ALGLIB includes following important improvements
          ! of this function:
          ! * high-performance native backend with same C# interface (C# version)
          ! * multithreading support (C++ and C# versions)
          ! * hardware vendor (Intel) implementations of linear algebra primitives
          !   (C++ and C# versions, x86/x64 platform)
          ! 
          ! We recommend you to read 'Working with commercial version' section  of
          ! ALGLIB Reference Manual in order to find out how to  use  performance-
          ! related features provided by commercial edition of ALGLIB.

        INPUT PARAMETERS
            A       -   array[0..NRows-1,0..NCols-1], system matrix
            NRows   -   vertical size of A
            NCols   -   horizontal size of A
            B       -   array[0..NCols-1], right part
            Threshold-  a number in [0,1]. Singular values  beyond  Threshold  are
                        considered  zero.  Set  it to 0.0, if you don't understand
                        what it means, so the solver will choose good value on its
                        own.
                        
        OUTPUT PARAMETERS
            Info    -   return code:
                        * -4    SVD subroutine failed
                        * -1    if NRows<=0 or NCols<=0 or Threshold<0 was passed
                        *  1    if task is solved
            Rep     -   solver report, see below for more info
            X       -   array[0..N-1,0..M-1], it contains:
                        * solution of A*X=B (even for singular A)
                        * zeros, if SVD subroutine failed

        SOLVER REPORT

        Subroutine sets following fields of the Rep structure:
        * R2        reciprocal of condition number: 1/cond(A), 2-norm.
        * N         = NCols
        * K         dim(Null(A))
        * CX        array[0..N-1,0..K-1], kernel of A.
                    Columns of CX store such vectors that A*CX[i]=0.

          -- ALGLIB --
             Copyright 24.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixsolvels(double[,] a,
            int nrows,
            int ncols,
            double[] b,
            double threshold,
            ref int info,
            densesolverlsreport rep,
            ref double[] x,
            alglib.xparams _params)
        {
            double[] sv = new double[0];
            double[,] u = new double[0,0];
            double[,] vt = new double[0,0];
            double[] rp = new double[0];
            double[] utb = new double[0];
            double[] sutb = new double[0];
            double[] tmp = new double[0];
            double[] ta = new double[0];
            double[] tx = new double[0];
            double[] buf = new double[0];
            double[] w = new double[0];
            int i = 0;
            int j = 0;
            int nsv = 0;
            int kernelidx = 0;
            double v = 0;
            double verr = 0;
            bool svdfailed = new bool();
            bool zeroa = new bool();
            int rfs = 0;
            int nrfs = 0;
            bool terminatenexttime = new bool();
            bool smallerr = new bool();
            int i_ = 0;

            info = 0;
            x = new double[0];

            if( (nrows<=0 || ncols<=0) || (double)(threshold)<(double)(0) )
            {
                info = -1;
                return;
            }
            if( (double)(threshold)==(double)(0) )
            {
                threshold = 1000*math.machineepsilon;
            }
            
            //
            // Factorize A first
            //
            svdfailed = !svd.rmatrixsvd(a, nrows, ncols, 1, 2, 2, ref sv, ref u, ref vt, _params);
            zeroa = (double)(sv[0])==(double)(0);
            if( svdfailed || zeroa )
            {
                if( svdfailed )
                {
                    info = -4;
                }
                else
                {
                    info = 1;
                }
                x = new double[ncols];
                for(i=0; i<=ncols-1; i++)
                {
                    x[i] = 0;
                }
                rep.n = ncols;
                rep.k = ncols;
                rep.cx = new double[ncols, ncols];
                for(i=0; i<=ncols-1; i++)
                {
                    for(j=0; j<=ncols-1; j++)
                    {
                        if( i==j )
                        {
                            rep.cx[i,j] = 1;
                        }
                        else
                        {
                            rep.cx[i,j] = 0;
                        }
                    }
                }
                rep.r2 = 0;
                return;
            }
            nsv = Math.Min(ncols, nrows);
            if( nsv==ncols )
            {
                rep.r2 = sv[nsv-1]/sv[0];
            }
            else
            {
                rep.r2 = 0;
            }
            rep.n = ncols;
            info = 1;
            
            //
            // Iterative refinement of xc combined with solution:
            // 1. xc = 0
            // 2. calculate r = bc-A*xc using extra-precise dot product
            // 3. solve A*y = r
            // 4. update x:=x+r
            // 5. goto 2
            //
            // This cycle is executed until one of two things happens:
            // 1. maximum number of iterations reached
            // 2. last iteration decreased error to the lower limit
            //
            utb = new double[nsv];
            sutb = new double[nsv];
            x = new double[ncols];
            tmp = new double[ncols];
            ta = new double[ncols+1];
            tx = new double[ncols+1];
            buf = new double[ncols+1];
            for(i=0; i<=ncols-1; i++)
            {
                x[i] = 0;
            }
            kernelidx = nsv;
            for(i=0; i<=nsv-1; i++)
            {
                if( (double)(sv[i])<=(double)(threshold*sv[0]) )
                {
                    kernelidx = i;
                    break;
                }
            }
            rep.k = ncols-kernelidx;
            nrfs = densesolverrfsmaxv2(ncols, rep.r2, _params);
            terminatenexttime = false;
            rp = new double[nrows];
            for(rfs=0; rfs<=nrfs; rfs++)
            {
                if( terminatenexttime )
                {
                    break;
                }
                
                //
                // calculate right part
                //
                if( rfs==0 )
                {
                    for(i_=0; i_<=nrows-1;i_++)
                    {
                        rp[i_] = b[i_];
                    }
                }
                else
                {
                    smallerr = true;
                    for(i=0; i<=nrows-1; i++)
                    {
                        for(i_=0; i_<=ncols-1;i_++)
                        {
                            ta[i_] = a[i,i_];
                        }
                        ta[ncols] = -1;
                        for(i_=0; i_<=ncols-1;i_++)
                        {
                            tx[i_] = x[i_];
                        }
                        tx[ncols] = b[i];
                        xblas.xdot(ta, tx, ncols+1, ref buf, ref v, ref verr, _params);
                        rp[i] = -v;
                        smallerr = smallerr && (double)(Math.Abs(v))<(double)(4*verr);
                    }
                    if( smallerr )
                    {
                        terminatenexttime = true;
                    }
                }
                
                //
                // solve A*dx = rp
                //
                for(i=0; i<=ncols-1; i++)
                {
                    tmp[i] = 0;
                }
                for(i=0; i<=nsv-1; i++)
                {
                    utb[i] = 0;
                }
                for(i=0; i<=nrows-1; i++)
                {
                    v = rp[i];
                    for(i_=0; i_<=nsv-1;i_++)
                    {
                        utb[i_] = utb[i_] + v*u[i,i_];
                    }
                }
                for(i=0; i<=nsv-1; i++)
                {
                    if( i<kernelidx )
                    {
                        sutb[i] = utb[i]/sv[i];
                    }
                    else
                    {
                        sutb[i] = 0;
                    }
                }
                for(i=0; i<=nsv-1; i++)
                {
                    v = sutb[i];
                    for(i_=0; i_<=ncols-1;i_++)
                    {
                        tmp[i_] = tmp[i_] + v*vt[i,i_];
                    }
                }
                
                //
                // update x:  x:=x+dx
                //
                for(i_=0; i_<=ncols-1;i_++)
                {
                    x[i_] = x[i_] + tmp[i_];
                }
            }
            
            //
            // fill CX
            //
            if( rep.k>0 )
            {
                rep.cx = new double[ncols, rep.k];
                for(i=0; i<=rep.k-1; i++)
                {
                    for(i_=0; i_<=ncols-1;i_++)
                    {
                        rep.cx[i_,i] = vt[kernelidx+i,i_];
                    }
                }
            }
        }


        /*************************************************************************
        Internal LU solver

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void rmatrixlusolveinternal(double[,] lua,
            int[] p,
            int n,
            double[,] a,
            bool havea,
            double[,] b,
            int m,
            ref int info,
            densesolverreport rep,
            ref double[,] x,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int rfs = 0;
            int nrfs = 0;
            double[] xc = new double[0];
            double[] y = new double[0];
            double[] bc = new double[0];
            double[] xa = new double[0];
            double[] xb = new double[0];
            double[] tx = new double[0];
            double v = 0;
            double verr = 0;
            double mxb = 0;
            bool smallerr = new bool();
            bool terminatenexttime = new bool();
            int i_ = 0;

            info = 0;
            x = new double[0,0];

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 || m<=0 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                if( p[i]>n-1 || p[i]<i )
                {
                    info = -1;
                    return;
                }
            }
            x = new double[n, m];
            y = new double[n];
            xc = new double[n];
            bc = new double[n];
            tx = new double[n+1];
            xa = new double[n+1];
            xb = new double[n+1];
            
            //
            // estimate condition number, test for near singularity
            //
            rep.r1 = rcond.rmatrixlurcond1(lua, n, _params);
            rep.rinf = rcond.rmatrixlurcondinf(lua, n, _params);
            if( (double)(rep.r1)<(double)(rcond.rcondthreshold(_params)) || (double)(rep.rinf)<(double)(rcond.rcondthreshold(_params)) )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x[i,j] = 0;
                    }
                }
                rep.r1 = 0;
                rep.rinf = 0;
                info = -3;
                return;
            }
            info = 1;
            
            //
            // First stage of solution: rough solution with TRSM()
            //
            mxb = 0.0;
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=m-1; j++)
                {
                    v = b[i,j];
                    mxb = Math.Max(mxb, Math.Abs(v));
                    x[i,j] = v;
                }
            }
            for(i=0; i<=n-1; i++)
            {
                if( p[i]!=i )
                {
                    for(j=0; j<=m-1; j++)
                    {
                        v = x[i,j];
                        x[i,j] = x[p[i],j];
                        x[p[i],j] = v;
                    }
                }
            }
            ablas.rmatrixlefttrsm(n, m, lua, 0, 0, false, true, 0, x, 0, 0, _params);
            ablas.rmatrixlefttrsm(n, m, lua, 0, 0, true, false, 0, x, 0, 0, _params);
            
            //
            // Second stage: iterative refinement
            //
            if( havea )
            {
                for(k=0; k<=m-1; k++)
                {
                    nrfs = densesolverrfsmax(n, rep.r1, rep.rinf, _params);
                    terminatenexttime = false;
                    for(rfs=0; rfs<=nrfs-1; rfs++)
                    {
                        if( terminatenexttime )
                        {
                            break;
                        }
                        
                        //
                        // generate right part
                        //
                        smallerr = true;
                        for(i_=0; i_<=n-1;i_++)
                        {
                            xb[i_] = x[i_,k];
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            for(i_=0; i_<=n-1;i_++)
                            {
                                xa[i_] = a[i,i_];
                            }
                            xa[n] = -1;
                            xb[n] = b[i,k];
                            xblas.xdot(xa, xb, n+1, ref tx, ref v, ref verr, _params);
                            y[i] = -v;
                            smallerr = smallerr && (double)(Math.Abs(v))<(double)(4*verr);
                        }
                        if( smallerr )
                        {
                            terminatenexttime = true;
                        }
                        
                        //
                        // solve and update
                        //
                        rbasiclusolve(lua, p, n, y, _params);
                        for(i_=0; i_<=n-1;i_++)
                        {
                            x[i_,k] = x[i_,k] + y[i_];
                        }
                    }
                }
            }
        }


        /*************************************************************************
        Internal Cholesky solver

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void spdmatrixcholeskysolveinternal(double[,] cha,
            int n,
            bool isupper,
            double[,] a,
            bool havea,
            double[,] b,
            int m,
            ref int info,
            densesolverreport rep,
            ref double[,] x,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;

            info = 0;
            x = new double[0,0];

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 || m<=0 )
            {
                info = -1;
                return;
            }
            x = new double[n, m];
            
            //
            // estimate condition number, test for near singularity
            //
            rep.r1 = rcond.spdmatrixcholeskyrcond(cha, n, isupper, _params);
            rep.rinf = rep.r1;
            if( (double)(rep.r1)<(double)(rcond.rcondthreshold(_params)) )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x[i,j] = 0;
                    }
                }
                rep.r1 = 0;
                rep.rinf = 0;
                info = -3;
                return;
            }
            info = 1;
            
            //
            // Solve with TRSM()
            //
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=m-1; j++)
                {
                    x[i,j] = b[i,j];
                }
            }
            if( isupper )
            {
                ablas.rmatrixlefttrsm(n, m, cha, 0, 0, true, false, 1, x, 0, 0, _params);
                ablas.rmatrixlefttrsm(n, m, cha, 0, 0, true, false, 0, x, 0, 0, _params);
            }
            else
            {
                ablas.rmatrixlefttrsm(n, m, cha, 0, 0, false, false, 0, x, 0, 0, _params);
                ablas.rmatrixlefttrsm(n, m, cha, 0, 0, false, false, 1, x, 0, 0, _params);
            }
        }


        /*************************************************************************
        Internal LU solver

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void cmatrixlusolveinternal(complex[,] lua,
            int[] p,
            int n,
            complex[,] a,
            bool havea,
            complex[,] b,
            int m,
            ref int info,
            densesolverreport rep,
            ref complex[,] x,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int rfs = 0;
            int nrfs = 0;
            complex[] xc = new complex[0];
            complex[] y = new complex[0];
            complex[] bc = new complex[0];
            complex[] xa = new complex[0];
            complex[] xb = new complex[0];
            complex[] tx = new complex[0];
            double[] tmpbuf = new double[0];
            complex v = 0;
            double verr = 0;
            bool smallerr = new bool();
            bool terminatenexttime = new bool();
            int i_ = 0;

            info = 0;
            x = new complex[0,0];

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 || m<=0 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                if( p[i]>n-1 || p[i]<i )
                {
                    info = -1;
                    return;
                }
            }
            x = new complex[n, m];
            y = new complex[n];
            xc = new complex[n];
            bc = new complex[n];
            tx = new complex[n];
            xa = new complex[n+1];
            xb = new complex[n+1];
            tmpbuf = new double[2*n+2];
            
            //
            // estimate condition number, test for near singularity
            //
            rep.r1 = rcond.cmatrixlurcond1(lua, n, _params);
            rep.rinf = rcond.cmatrixlurcondinf(lua, n, _params);
            if( (double)(rep.r1)<(double)(rcond.rcondthreshold(_params)) || (double)(rep.rinf)<(double)(rcond.rcondthreshold(_params)) )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x[i,j] = 0;
                    }
                }
                rep.r1 = 0;
                rep.rinf = 0;
                info = -3;
                return;
            }
            info = 1;
            
            //
            // First phase: solve with TRSM()
            //
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=m-1; j++)
                {
                    x[i,j] = b[i,j];
                }
            }
            for(i=0; i<=n-1; i++)
            {
                if( p[i]!=i )
                {
                    for(j=0; j<=m-1; j++)
                    {
                        v = x[i,j];
                        x[i,j] = x[p[i],j];
                        x[p[i],j] = v;
                    }
                }
            }
            ablas.cmatrixlefttrsm(n, m, lua, 0, 0, false, true, 0, x, 0, 0, _params);
            ablas.cmatrixlefttrsm(n, m, lua, 0, 0, true, false, 0, x, 0, 0, _params);
            
            //
            // solve
            //
            for(k=0; k<=m-1; k++)
            {
                for(i_=0; i_<=n-1;i_++)
                {
                    bc[i_] = b[i_,k];
                }
                for(i_=0; i_<=n-1;i_++)
                {
                    xc[i_] = x[i_,k];
                }
                
                //
                // Iterative refinement of xc:
                // * calculate r = bc-A*xc using extra-precise dot product
                // * solve A*y = r
                // * update x:=x+r
                //
                // This cycle is executed until one of two things happens:
                // 1. maximum number of iterations reached
                // 2. last iteration decreased error to the lower limit
                //
                if( havea )
                {
                    nrfs = densesolverrfsmax(n, rep.r1, rep.rinf, _params);
                    terminatenexttime = false;
                    for(rfs=0; rfs<=nrfs-1; rfs++)
                    {
                        if( terminatenexttime )
                        {
                            break;
                        }
                        
                        //
                        // generate right part
                        //
                        smallerr = true;
                        for(i_=0; i_<=n-1;i_++)
                        {
                            xb[i_] = xc[i_];
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            for(i_=0; i_<=n-1;i_++)
                            {
                                xa[i_] = a[i,i_];
                            }
                            xa[n] = -1;
                            xb[n] = bc[i];
                            xblas.xcdot(xa, xb, n+1, ref tmpbuf, ref v, ref verr, _params);
                            y[i] = -v;
                            smallerr = smallerr && (double)(math.abscomplex(v))<(double)(4*verr);
                        }
                        if( smallerr )
                        {
                            terminatenexttime = true;
                        }
                        
                        //
                        // solve and update
                        //
                        cbasiclusolve(lua, p, n, y, _params);
                        for(i_=0; i_<=n-1;i_++)
                        {
                            xc[i_] = xc[i_] + y[i_];
                        }
                    }
                }
                
                //
                // Store xc.
                // Post-scale result.
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    x[i_,k] = xc[i_];
                }
            }
        }


        /*************************************************************************
        Internal Cholesky solver

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void hpdmatrixcholeskysolveinternal(complex[,] cha,
            int n,
            bool isupper,
            complex[,] a,
            bool havea,
            complex[,] b,
            int m,
            ref int info,
            densesolverreport rep,
            ref complex[,] x,
            alglib.xparams _params)
        {
            int i = 0;
            int j = 0;
            complex[] xc = new complex[0];
            complex[] y = new complex[0];
            complex[] bc = new complex[0];
            complex[] xa = new complex[0];
            complex[] xb = new complex[0];
            complex[] tx = new complex[0];

            info = 0;
            x = new complex[0,0];

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 || m<=0 )
            {
                info = -1;
                return;
            }
            x = new complex[n, m];
            y = new complex[n];
            xc = new complex[n];
            bc = new complex[n];
            tx = new complex[n+1];
            xa = new complex[n+1];
            xb = new complex[n+1];
            
            //
            // estimate condition number, test for near singularity
            //
            rep.r1 = rcond.hpdmatrixcholeskyrcond(cha, n, isupper, _params);
            rep.rinf = rep.r1;
            if( (double)(rep.r1)<(double)(rcond.rcondthreshold(_params)) )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x[i,j] = 0;
                    }
                }
                rep.r1 = 0;
                rep.rinf = 0;
                info = -3;
                return;
            }
            info = 1;
            
            //
            // solve
            //
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=m-1; j++)
                {
                    x[i,j] = b[i,j];
                }
            }
            if( isupper )
            {
                ablas.cmatrixlefttrsm(n, m, cha, 0, 0, true, false, 2, x, 0, 0, _params);
                ablas.cmatrixlefttrsm(n, m, cha, 0, 0, true, false, 0, x, 0, 0, _params);
            }
            else
            {
                ablas.cmatrixlefttrsm(n, m, cha, 0, 0, false, false, 0, x, 0, 0, _params);
                ablas.cmatrixlefttrsm(n, m, cha, 0, 0, false, false, 2, x, 0, 0, _params);
            }
        }


        /*************************************************************************
        Internal subroutine.
        Returns maximum count of RFS iterations as function of:
        1. machine epsilon
        2. task size.
        3. condition number

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static int densesolverrfsmax(int n,
            double r1,
            double rinf,
            alglib.xparams _params)
        {
            int result = 0;

            result = 5;
            return result;
        }


        /*************************************************************************
        Internal subroutine.
        Returns maximum count of RFS iterations as function of:
        1. machine epsilon
        2. task size.
        3. norm-2 condition number

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static int densesolverrfsmaxv2(int n,
            double r2,
            alglib.xparams _params)
        {
            int result = 0;

            result = densesolverrfsmax(n, 0, 0, _params);
            return result;
        }


        /*************************************************************************
        Basic LU solver for PLU*x = y.

        This subroutine assumes that:
        * A=PLU is well-conditioned, so no zero divisions or overflow may occur

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void rbasiclusolve(double[,] lua,
            int[] p,
            int n,
            double[] xb,
            alglib.xparams _params)
        {
            int i = 0;
            double v = 0;
            int i_ = 0;

            for(i=0; i<=n-1; i++)
            {
                if( p[i]!=i )
                {
                    v = xb[i];
                    xb[i] = xb[p[i]];
                    xb[p[i]] = v;
                }
            }
            for(i=1; i<=n-1; i++)
            {
                v = 0.0;
                for(i_=0; i_<=i-1;i_++)
                {
                    v += lua[i,i_]*xb[i_];
                }
                xb[i] = xb[i]-v;
            }
            xb[n-1] = xb[n-1]/lua[n-1,n-1];
            for(i=n-2; i>=0; i--)
            {
                v = 0.0;
                for(i_=i+1; i_<=n-1;i_++)
                {
                    v += lua[i,i_]*xb[i_];
                }
                xb[i] = (xb[i]-v)/lua[i,i];
            }
        }


        /*************************************************************************
        Basic Cholesky solver for ScaleA*Cholesky(A)'*x = y.

        This subroutine assumes that:
        * A*ScaleA is well scaled
        * A is well-conditioned, so no zero divisions or overflow may occur

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void spdbasiccholeskysolve(double[,] cha,
            int n,
            bool isupper,
            double[] xb,
            alglib.xparams _params)
        {
            int i = 0;
            double v = 0;
            int i_ = 0;

            
            //
            // A = L*L' or A=U'*U
            //
            if( isupper )
            {
                
                //
                // Solve U'*y=b first.
                //
                for(i=0; i<=n-1; i++)
                {
                    xb[i] = xb[i]/cha[i,i];
                    if( i<n-1 )
                    {
                        v = xb[i];
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            xb[i_] = xb[i_] - v*cha[i,i_];
                        }
                    }
                }
                
                //
                // Solve U*x=y then.
                //
                for(i=n-1; i>=0; i--)
                {
                    if( i<n-1 )
                    {
                        v = 0.0;
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            v += cha[i,i_]*xb[i_];
                        }
                        xb[i] = xb[i]-v;
                    }
                    xb[i] = xb[i]/cha[i,i];
                }
            }
            else
            {
                
                //
                // Solve L*y=b first
                //
                for(i=0; i<=n-1; i++)
                {
                    if( i>0 )
                    {
                        v = 0.0;
                        for(i_=0; i_<=i-1;i_++)
                        {
                            v += cha[i,i_]*xb[i_];
                        }
                        xb[i] = xb[i]-v;
                    }
                    xb[i] = xb[i]/cha[i,i];
                }
                
                //
                // Solve L'*x=y then.
                //
                for(i=n-1; i>=0; i--)
                {
                    xb[i] = xb[i]/cha[i,i];
                    if( i>0 )
                    {
                        v = xb[i];
                        for(i_=0; i_<=i-1;i_++)
                        {
                            xb[i_] = xb[i_] - v*cha[i,i_];
                        }
                    }
                }
            }
        }


        /*************************************************************************
        Basic LU solver for ScaleA*PLU*x = y.

        This subroutine assumes that:
        * L is well-scaled, and it is U which needs scaling by ScaleA.
        * A=PLU is well-conditioned, so no zero divisions or overflow may occur

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void cbasiclusolve(complex[,] lua,
            int[] p,
            int n,
            complex[] xb,
            alglib.xparams _params)
        {
            int i = 0;
            complex v = 0;
            int i_ = 0;

            for(i=0; i<=n-1; i++)
            {
                if( p[i]!=i )
                {
                    v = xb[i];
                    xb[i] = xb[p[i]];
                    xb[p[i]] = v;
                }
            }
            for(i=1; i<=n-1; i++)
            {
                v = 0.0;
                for(i_=0; i_<=i-1;i_++)
                {
                    v += lua[i,i_]*xb[i_];
                }
                xb[i] = xb[i]-v;
            }
            xb[n-1] = xb[n-1]/lua[n-1,n-1];
            for(i=n-2; i>=0; i--)
            {
                v = 0.0;
                for(i_=i+1; i_<=n-1;i_++)
                {
                    v += lua[i,i_]*xb[i_];
                }
                xb[i] = (xb[i]-v)/lua[i,i];
            }
        }


        /*************************************************************************
        Basic Cholesky solver for ScaleA*Cholesky(A)'*x = y.

        This subroutine assumes that:
        * A*ScaleA is well scaled
        * A is well-conditioned, so no zero divisions or overflow may occur

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void hpdbasiccholeskysolve(complex[,] cha,
            int n,
            bool isupper,
            complex[] xb,
            alglib.xparams _params)
        {
            int i = 0;
            complex v = 0;
            int i_ = 0;

            
            //
            // A = L*L' or A=U'*U
            //
            if( isupper )
            {
                
                //
                // Solve U'*y=b first.
                //
                for(i=0; i<=n-1; i++)
                {
                    xb[i] = xb[i]/math.conj(cha[i,i]);
                    if( i<n-1 )
                    {
                        v = xb[i];
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            xb[i_] = xb[i_] - v*math.conj(cha[i,i_]);
                        }
                    }
                }
                
                //
                // Solve U*x=y then.
                //
                for(i=n-1; i>=0; i--)
                {
                    if( i<n-1 )
                    {
                        v = 0.0;
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            v += cha[i,i_]*xb[i_];
                        }
                        xb[i] = xb[i]-v;
                    }
                    xb[i] = xb[i]/cha[i,i];
                }
            }
            else
            {
                
                //
                // Solve L*y=b first
                //
                for(i=0; i<=n-1; i++)
                {
                    if( i>0 )
                    {
                        v = 0.0;
                        for(i_=0; i_<=i-1;i_++)
                        {
                            v += cha[i,i_]*xb[i_];
                        }
                        xb[i] = xb[i]-v;
                    }
                    xb[i] = xb[i]/cha[i,i];
                }
                
                //
                // Solve L'*x=y then.
                //
                for(i=n-1; i>=0; i--)
                {
                    xb[i] = xb[i]/math.conj(cha[i,i]);
                    if( i>0 )
                    {
                        v = xb[i];
                        for(i_=0; i_<=i-1;i_++)
                        {
                            xb[i_] = xb[i_] - v*math.conj(cha[i,i_]);
                        }
                    }
                }
            }
        }


    }
    public class linlsqr
    {
        /*************************************************************************
        This object stores state of the LinLSQR method.

        You should use ALGLIB functions to work with this object.
        *************************************************************************/
        public class linlsqrstate : apobject
        {
            public normestimator.normestimatorstate nes;
            public double[] rx;
            public double[] b;
            public int n;
            public int m;
            public int prectype;
            public double[] ui;
            public double[] uip1;
            public double[] vi;
            public double[] vip1;
            public double[] omegai;
            public double[] omegaip1;
            public double alphai;
            public double alphaip1;
            public double betai;
            public double betaip1;
            public double phibari;
            public double phibarip1;
            public double phii;
            public double rhobari;
            public double rhobarip1;
            public double rhoi;
            public double ci;
            public double si;
            public double theta;
            public double lambdai;
            public double[] d;
            public double anorm;
            public double bnorm2;
            public double dnorm;
            public double r2;
            public double[] x;
            public double[] mv;
            public double[] mtv;
            public double epsa;
            public double epsb;
            public double epsc;
            public int maxits;
            public bool xrep;
            public bool xupdated;
            public bool needmv;
            public bool needmtv;
            public bool needmv2;
            public bool needvmv;
            public bool needprec;
            public int repiterationscount;
            public int repnmv;
            public int repterminationtype;
            public bool running;
            public double[] tmpd;
            public double[] tmpx;
            public rcommstate rstate;
            public linlsqrstate()
            {
                init();
            }
            public override void init()
            {
                nes = new normestimator.normestimatorstate();
                rx = new double[0];
                b = new double[0];
                ui = new double[0];
                uip1 = new double[0];
                vi = new double[0];
                vip1 = new double[0];
                omegai = new double[0];
                omegaip1 = new double[0];
                d = new double[0];
                x = new double[0];
                mv = new double[0];
                mtv = new double[0];
                tmpd = new double[0];
                tmpx = new double[0];
                rstate = new rcommstate();
            }
            public override alglib.apobject make_copy()
            {
                linlsqrstate _result = new linlsqrstate();
                _result.nes = (normestimator.normestimatorstate)nes.make_copy();
                _result.rx = (double[])rx.Clone();
                _result.b = (double[])b.Clone();
                _result.n = n;
                _result.m = m;
                _result.prectype = prectype;
                _result.ui = (double[])ui.Clone();
                _result.uip1 = (double[])uip1.Clone();
                _result.vi = (double[])vi.Clone();
                _result.vip1 = (double[])vip1.Clone();
                _result.omegai = (double[])omegai.Clone();
                _result.omegaip1 = (double[])omegaip1.Clone();
                _result.alphai = alphai;
                _result.alphaip1 = alphaip1;
                _result.betai = betai;
                _result.betaip1 = betaip1;
                _result.phibari = phibari;
                _result.phibarip1 = phibarip1;
                _result.phii = phii;
                _result.rhobari = rhobari;
                _result.rhobarip1 = rhobarip1;
                _result.rhoi = rhoi;
                _result.ci = ci;
                _result.si = si;
                _result.theta = theta;
                _result.lambdai = lambdai;
                _result.d = (double[])d.Clone();
                _result.anorm = anorm;
                _result.bnorm2 = bnorm2;
                _result.dnorm = dnorm;
                _result.r2 = r2;
                _result.x = (double[])x.Clone();
                _result.mv = (double[])mv.Clone();
                _result.mtv = (double[])mtv.Clone();
                _result.epsa = epsa;
                _result.epsb = epsb;
                _result.epsc = epsc;
                _result.maxits = maxits;
                _result.xrep = xrep;
                _result.xupdated = xupdated;
                _result.needmv = needmv;
                _result.needmtv = needmtv;
                _result.needmv2 = needmv2;
                _result.needvmv = needvmv;
                _result.needprec = needprec;
                _result.repiterationscount = repiterationscount;
                _result.repnmv = repnmv;
                _result.repterminationtype = repterminationtype;
                _result.running = running;
                _result.tmpd = (double[])tmpd.Clone();
                _result.tmpx = (double[])tmpx.Clone();
                _result.rstate = (rcommstate)rstate.make_copy();
                return _result;
            }
        };


        public class linlsqrreport : apobject
        {
            public int iterationscount;
            public int nmv;
            public int terminationtype;
            public linlsqrreport()
            {
                init();
            }
            public override void init()
            {
            }
            public override alglib.apobject make_copy()
            {
                linlsqrreport _result = new linlsqrreport();
                _result.iterationscount = iterationscount;
                _result.nmv = nmv;
                _result.terminationtype = terminationtype;
                return _result;
            }
        };




        public const double atol = 1.0E-6;
        public const double btol = 1.0E-6;


        /*************************************************************************
        This function initializes linear LSQR Solver. This solver is used to solve
        non-symmetric (and, possibly, non-square) problems. Least squares solution
        is returned for non-compatible systems.

        USAGE:
        1. User initializes algorithm state with LinLSQRCreate() call
        2. User tunes solver parameters with  LinLSQRSetCond() and other functions
        3. User  calls  LinLSQRSolveSparse()  function which takes algorithm state 
           and SparseMatrix object.
        4. User calls LinLSQRResults() to get solution
        5. Optionally, user may call LinLSQRSolveSparse() again to  solve  another  
           problem  with different matrix and/or right part without reinitializing 
           LinLSQRState structure.
          
        INPUT PARAMETERS:
            M       -   number of rows in A
            N       -   number of variables, N>0

        OUTPUT PARAMETERS:
            State   -   structure which stores algorithm state
            
        NOTE: see also linlsqrcreatebuf()  for  version  which  reuses  previously
              allocated place as much as possible.

          -- ALGLIB --
             Copyright 30.11.2011 by Bochkanov Sergey
        *************************************************************************/
        public static void linlsqrcreate(int m,
            int n,
            linlsqrstate state,
            alglib.xparams _params)
        {
            alglib.ap.assert(m>0, "LinLSQRCreate: M<=0");
            alglib.ap.assert(n>0, "LinLSQRCreate: N<=0");
            linlsqrcreatebuf(m, n, state, _params);
        }


        /*************************************************************************
        This function initializes linear LSQR Solver.  It  provides  exactly  same
        functionality as linlsqrcreate(), but reuses  previously  allocated  space
        as much as possible.
          
        INPUT PARAMETERS:
            M       -   number of rows in A
            N       -   number of variables, N>0

        OUTPUT PARAMETERS:
            State   -   structure which stores algorithm state

          -- ALGLIB --
             Copyright 14.11.2018 by Bochkanov Sergey
        *************************************************************************/
        public static void linlsqrcreatebuf(int m,
            int n,
            linlsqrstate state,
            alglib.xparams _params)
        {
            int i = 0;

            alglib.ap.assert(m>0, "LinLSQRCreateBuf: M<=0");
            alglib.ap.assert(n>0, "LinLSQRCreateBuf: N<=0");
            state.m = m;
            state.n = n;
            state.prectype = 0;
            state.epsa = atol;
            state.epsb = btol;
            state.epsc = 1/Math.Sqrt(math.machineepsilon);
            state.maxits = 0;
            state.lambdai = 0;
            state.xrep = false;
            state.running = false;
            
            //
            // * allocate arrays
            // * set RX to NAN (just for the case user calls Results() without 
            //   calling SolveSparse()
            // * set B to zero
            //
            normestimator.normestimatorcreate(m, n, 2, 2, state.nes, _params);
            state.rx = new double[state.n];
            state.ui = new double[state.m+state.n];
            state.uip1 = new double[state.m+state.n];
            state.vip1 = new double[state.n];
            state.vi = new double[state.n];
            state.omegai = new double[state.n];
            state.omegaip1 = new double[state.n];
            state.d = new double[state.n];
            state.x = new double[state.m+state.n];
            state.mv = new double[state.m+state.n];
            state.mtv = new double[state.n];
            state.b = new double[state.m];
            for(i=0; i<=n-1; i++)
            {
                state.rx[i] = Double.NaN;
            }
            for(i=0; i<=m-1; i++)
            {
                state.b[i] = 0;
            }
            state.rstate.ia = new int[1+1];
            state.rstate.ra = new double[0+1];
            state.rstate.stage = -1;
        }


        /*************************************************************************
        This function sets right part. By default, right part is zero.

        INPUT PARAMETERS:
            B       -   right part, array[N].

        OUTPUT PARAMETERS:
            State   -   structure which stores algorithm state

          -- ALGLIB --
             Copyright 30.11.2011 by Bochkanov Sergey
        *************************************************************************/
        public static void linlsqrsetb(linlsqrstate state,
            double[] b,
            alglib.xparams _params)
        {
            int i = 0;

            alglib.ap.assert(!state.running, "LinLSQRSetB: you can not change B when LinLSQRIteration is running");
            alglib.ap.assert(state.m<=alglib.ap.len(b), "LinLSQRSetB: Length(B)<M");
            alglib.ap.assert(apserv.isfinitevector(b, state.m, _params), "LinLSQRSetB: B contains infinite or NaN values");
            state.bnorm2 = 0;
            for(i=0; i<=state.m-1; i++)
            {
                state.b[i] = b[i];
                state.bnorm2 = state.bnorm2+b[i]*b[i];
            }
        }


        /*************************************************************************
        This  function  changes  preconditioning  settings of LinLSQQSolveSparse()
        function. By default, SolveSparse() uses diagonal preconditioner,  but  if
        you want to use solver without preconditioning, you can call this function
        which forces solver to use unit matrix for preconditioning.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state

          -- ALGLIB --
             Copyright 19.11.2012 by Bochkanov Sergey
        *************************************************************************/
        public static void linlsqrsetprecunit(linlsqrstate state,
            alglib.xparams _params)
        {
            alglib.ap.assert(!state.running, "LinLSQRSetPrecUnit: you can not change preconditioner, because function LinLSQRIteration is running!");
            state.prectype = -1;
        }


        /*************************************************************************
        This  function  changes  preconditioning  settings  of  LinCGSolveSparse()
        function.  LinCGSolveSparse() will use diagonal of the  system  matrix  as
        preconditioner. This preconditioning mode is active by default.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state

          -- ALGLIB --
             Copyright 19.11.2012 by Bochkanov Sergey
        *************************************************************************/
        public static void linlsqrsetprecdiag(linlsqrstate state,
            alglib.xparams _params)
        {
            alglib.ap.assert(!state.running, "LinLSQRSetPrecDiag: you can not change preconditioner, because function LinCGIteration is running!");
            state.prectype = 0;
        }


        /*************************************************************************
        This function sets optional Tikhonov regularization coefficient.
        It is zero by default.

        INPUT PARAMETERS:
            LambdaI -   regularization factor, LambdaI>=0

        OUTPUT PARAMETERS:
            State   -   structure which stores algorithm state
            
          -- ALGLIB --
             Copyright 30.11.2011 by Bochkanov Sergey
        *************************************************************************/
        public static void linlsqrsetlambdai(linlsqrstate state,
            double lambdai,
            alglib.xparams _params)
        {
            alglib.ap.assert(!state.running, "LinLSQRSetLambdaI: you can not set LambdaI, because function LinLSQRIteration is running");
            alglib.ap.assert(math.isfinite(lambdai) && (double)(lambdai)>=(double)(0), "LinLSQRSetLambdaI: LambdaI is infinite or NaN");
            state.lambdai = lambdai;
        }


        /*************************************************************************

          -- ALGLIB --
             Copyright 30.11.2011 by Bochkanov Sergey
        *************************************************************************/
        public static bool linlsqriteration(linlsqrstate state,
            alglib.xparams _params)
        {
            bool result = new bool();
            int summn = 0;
            double bnorm = 0;
            int i = 0;
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
                summn = state.rstate.ia[0];
                i = state.rstate.ia[1];
                bnorm = state.rstate.ra[0];
            }
            else
            {
                summn = 359;
                i = -58;
                bnorm = -919;
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
            if( state.rstate.stage==4 )
            {
                goto lbl_4;
            }
            if( state.rstate.stage==5 )
            {
                goto lbl_5;
            }
            if( state.rstate.stage==6 )
            {
                goto lbl_6;
            }
            
            //
            // Routine body
            //
            alglib.ap.assert(alglib.ap.len(state.b)>0, "LinLSQRIteration: using non-allocated array B");
            bnorm = Math.Sqrt(state.bnorm2);
            state.running = true;
            state.repnmv = 0;
            clearrfields(state, _params);
            state.repiterationscount = 0;
            summn = state.m+state.n;
            state.r2 = state.bnorm2;
            
            //
            //estimate for ANorm
            //
            normestimator.normestimatorrestart(state.nes, _params);
        lbl_7:
            if( !normestimator.normestimatoriteration(state.nes, _params) )
            {
                goto lbl_8;
            }
            if( !state.nes.needmv )
            {
                goto lbl_9;
            }
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.x[i_] = state.nes.x[i_];
            }
            state.repnmv = state.repnmv+1;
            clearrfields(state, _params);
            state.needmv = true;
            state.rstate.stage = 0;
            goto lbl_rcomm;
        lbl_0:
            state.needmv = false;
            for(i_=0; i_<=state.m-1;i_++)
            {
                state.nes.mv[i_] = state.mv[i_];
            }
            goto lbl_7;
        lbl_9:
            if( !state.nes.needmtv )
            {
                goto lbl_11;
            }
            for(i_=0; i_<=state.m-1;i_++)
            {
                state.x[i_] = state.nes.x[i_];
            }
            
            //
            //matrix-vector multiplication
            //
            state.repnmv = state.repnmv+1;
            clearrfields(state, _params);
            state.needmtv = true;
            state.rstate.stage = 1;
            goto lbl_rcomm;
        lbl_1:
            state.needmtv = false;
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.nes.mtv[i_] = state.mtv[i_];
            }
            goto lbl_7;
        lbl_11:
            goto lbl_7;
        lbl_8:
            normestimator.normestimatorresults(state.nes, ref state.anorm, _params);
            
            //
            //initialize .RX by zeros
            //
            for(i=0; i<=state.n-1; i++)
            {
                state.rx[i] = 0;
            }
            
            //
            //output first report
            //
            if( !state.xrep )
            {
                goto lbl_13;
            }
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.x[i_] = state.rx[i_];
            }
            clearrfields(state, _params);
            state.xupdated = true;
            state.rstate.stage = 2;
            goto lbl_rcomm;
        lbl_2:
            state.xupdated = false;
        lbl_13:
            
            //
            // LSQR, Step 0.
            //
            // Algorithm outline corresponds to one which was described at p.50 of
            // "LSQR - an algorithm for sparse linear equations and sparse least 
            // squares" by C.Paige and M.Saunders with one small addition - we
            // explicitly extend system matrix by additional N lines in order 
            // to handle non-zero lambda, i.e. original A is replaced by
            //         [ A        ]
            // A_mod = [          ]
            //         [ lambda*I ].
            //
            // Step 0:
            //     x[0]          = 0
            //     beta[1]*u[1]  = b
            //     alpha[1]*v[1] = A_mod'*u[1]
            //     w[1]          = v[1]
            //     phiBar[1]     = beta[1]
            //     rhoBar[1]     = alpha[1]
            //     d[0]          = 0
            //
            // NOTE:
            // There are three criteria for stopping:
            // (S0) maximum number of iterations
            // (S1) ||Rk||<=EpsB*||B||;
            // (S2) ||A^T*Rk||/(||A||*||Rk||)<=EpsA.
            // It is very important that S2 always checked AFTER S1. It is necessary
            // to avoid division by zero when Rk=0.
            //
            state.betai = bnorm;
            if( (double)(state.betai)==(double)(0) )
            {
                
                //
                // Zero right part
                //
                state.running = false;
                state.repterminationtype = 1;
                result = false;
                return result;
            }
            for(i=0; i<=summn-1; i++)
            {
                if( i<state.m )
                {
                    state.ui[i] = state.b[i]/state.betai;
                }
                else
                {
                    state.ui[i] = 0;
                }
                state.x[i] = state.ui[i];
            }
            state.repnmv = state.repnmv+1;
            clearrfields(state, _params);
            state.needmtv = true;
            state.rstate.stage = 3;
            goto lbl_rcomm;
        lbl_3:
            state.needmtv = false;
            for(i=0; i<=state.n-1; i++)
            {
                state.mtv[i] = state.mtv[i]+state.lambdai*state.ui[state.m+i];
            }
            state.alphai = 0;
            for(i=0; i<=state.n-1; i++)
            {
                state.alphai = state.alphai+state.mtv[i]*state.mtv[i];
            }
            state.alphai = Math.Sqrt(state.alphai);
            if( (double)(state.alphai)==(double)(0) )
            {
                
                //
                // Orthogonality stopping criterion is met
                //
                state.running = false;
                state.repterminationtype = 4;
                result = false;
                return result;
            }
            for(i=0; i<=state.n-1; i++)
            {
                state.vi[i] = state.mtv[i]/state.alphai;
                state.omegai[i] = state.vi[i];
            }
            state.phibari = state.betai;
            state.rhobari = state.alphai;
            for(i=0; i<=state.n-1; i++)
            {
                state.d[i] = 0;
            }
            state.dnorm = 0;
            
            //
            // Steps I=1, 2, ...
            //
        lbl_15:
            if( false )
            {
                goto lbl_16;
            }
            
            //
            // At I-th step State.RepIterationsCount=I.
            //
            state.repiterationscount = state.repiterationscount+1;
            
            //
            // Bidiagonalization part:
            //     beta[i+1]*u[i+1]  = A_mod*v[i]-alpha[i]*u[i]
            //     alpha[i+1]*v[i+1] = A_mod'*u[i+1] - beta[i+1]*v[i]
            //     
            // NOTE:  beta[i+1]=0 or alpha[i+1]=0 will lead to successful termination
            //        in the end of the current iteration. In this case u/v are zero.
            // NOTE2: algorithm won't fail on zero alpha or beta (there will be no
            //        division by zero because it will be stopped BEFORE division
            //        occurs). However, near-zero alpha and beta won't stop algorithm
            //        and, although no division by zero will happen, orthogonality 
            //        in U and V will be lost.
            //
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.x[i_] = state.vi[i_];
            }
            state.repnmv = state.repnmv+1;
            clearrfields(state, _params);
            state.needmv = true;
            state.rstate.stage = 4;
            goto lbl_rcomm;
        lbl_4:
            state.needmv = false;
            for(i=0; i<=state.n-1; i++)
            {
                state.mv[state.m+i] = state.lambdai*state.vi[i];
            }
            state.betaip1 = 0;
            for(i=0; i<=summn-1; i++)
            {
                state.uip1[i] = state.mv[i]-state.alphai*state.ui[i];
                state.betaip1 = state.betaip1+state.uip1[i]*state.uip1[i];
            }
            if( (double)(state.betaip1)!=(double)(0) )
            {
                state.betaip1 = Math.Sqrt(state.betaip1);
                for(i=0; i<=summn-1; i++)
                {
                    state.uip1[i] = state.uip1[i]/state.betaip1;
                }
            }
            for(i_=0; i_<=state.m-1;i_++)
            {
                state.x[i_] = state.uip1[i_];
            }
            state.repnmv = state.repnmv+1;
            clearrfields(state, _params);
            state.needmtv = true;
            state.rstate.stage = 5;
            goto lbl_rcomm;
        lbl_5:
            state.needmtv = false;
            for(i=0; i<=state.n-1; i++)
            {
                state.mtv[i] = state.mtv[i]+state.lambdai*state.uip1[state.m+i];
            }
            state.alphaip1 = 0;
            for(i=0; i<=state.n-1; i++)
            {
                state.vip1[i] = state.mtv[i]-state.betaip1*state.vi[i];
                state.alphaip1 = state.alphaip1+state.vip1[i]*state.vip1[i];
            }
            if( (double)(state.alphaip1)!=(double)(0) )
            {
                state.alphaip1 = Math.Sqrt(state.alphaip1);
                for(i=0; i<=state.n-1; i++)
                {
                    state.vip1[i] = state.vip1[i]/state.alphaip1;
                }
            }
            
            //
            // Build next orthogonal transformation
            //
            state.rhoi = apserv.safepythag2(state.rhobari, state.betaip1, _params);
            state.ci = state.rhobari/state.rhoi;
            state.si = state.betaip1/state.rhoi;
            state.theta = state.si*state.alphaip1;
            state.rhobarip1 = -(state.ci*state.alphaip1);
            state.phii = state.ci*state.phibari;
            state.phibarip1 = state.si*state.phibari;
            
            //
            // Update .RNorm
            //
            // This tricky  formula  is  necessary  because  simply  writing
            // State.R2:=State.PhiBarIP1*State.PhiBarIP1 does NOT guarantees
            // monotonic decrease of R2. Roundoff error combined with 80-bit
            // precision used internally by Intel chips allows R2 to increase
            // slightly in some rare, but possible cases. This property is
            // undesirable, so we prefer to guard against R increase.
            //
            state.r2 = Math.Min(state.r2, state.phibarip1*state.phibarip1);
            
            //
            // Update d and DNorm, check condition-related stopping criteria
            //
            for(i=0; i<=state.n-1; i++)
            {
                state.d[i] = 1/state.rhoi*(state.vi[i]-state.theta*state.d[i]);
                state.dnorm = state.dnorm+state.d[i]*state.d[i];
            }
            if( (double)(Math.Sqrt(state.dnorm)*state.anorm)>=(double)(state.epsc) )
            {
                state.running = false;
                state.repterminationtype = 7;
                result = false;
                return result;
            }
            
            //
            // Update x, output report
            //
            for(i=0; i<=state.n-1; i++)
            {
                state.rx[i] = state.rx[i]+state.phii/state.rhoi*state.omegai[i];
            }
            if( !state.xrep )
            {
                goto lbl_17;
            }
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.x[i_] = state.rx[i_];
            }
            clearrfields(state, _params);
            state.xupdated = true;
            state.rstate.stage = 6;
            goto lbl_rcomm;
        lbl_6:
            state.xupdated = false;
        lbl_17:
            
            //
            // Check stopping criteria
            // 1. achieved required number of iterations;
            // 2. ||Rk||<=EpsB*||B||;
            // 3. ||A^T*Rk||/(||A||*||Rk||)<=EpsA;
            //
            if( state.maxits>0 && state.repiterationscount>=state.maxits )
            {
                
                //
                // Achieved required number of iterations
                //
                state.running = false;
                state.repterminationtype = 5;
                result = false;
                return result;
            }
            if( (double)(state.phibarip1)<=(double)(state.epsb*bnorm) )
            {
                
                //
                // ||Rk||<=EpsB*||B||, here ||Rk||=PhiBar
                //
                state.running = false;
                state.repterminationtype = 1;
                result = false;
                return result;
            }
            if( (double)(state.alphaip1*Math.Abs(state.ci)/state.anorm)<=(double)(state.epsa) )
            {
                
                //
                // ||A^T*Rk||/(||A||*||Rk||)<=EpsA, here ||A^T*Rk||=PhiBar*Alpha[i+1]*|.C|
                //
                state.running = false;
                state.repterminationtype = 4;
                result = false;
                return result;
            }
            
            //
            // Update omega
            //
            for(i=0; i<=state.n-1; i++)
            {
                state.omegaip1[i] = state.vip1[i]-state.theta/state.rhoi*state.omegai[i];
            }
            
            //
            // Prepare for the next iteration - rename variables:
            // u[i]   := u[i+1]
            // v[i]   := v[i+1]
            // rho[i] := rho[i+1]
            // ...
            //
            for(i_=0; i_<=summn-1;i_++)
            {
                state.ui[i_] = state.uip1[i_];
            }
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.vi[i_] = state.vip1[i_];
            }
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.omegai[i_] = state.omegaip1[i_];
            }
            state.alphai = state.alphaip1;
            state.betai = state.betaip1;
            state.phibari = state.phibarip1;
            state.rhobari = state.rhobarip1;
            goto lbl_15;
        lbl_16:
            result = false;
            return result;
            
            //
            // Saving state
            //
        lbl_rcomm:
            result = true;
            state.rstate.ia[0] = summn;
            state.rstate.ia[1] = i;
            state.rstate.ra[0] = bnorm;
            return result;
        }


        /*************************************************************************
        Procedure for solution of A*x=b with sparse A.

        INPUT PARAMETERS:
            State   -   algorithm state
            A       -   sparse M*N matrix in the CRS format (you MUST contvert  it 
                        to CRS format  by  calling  SparseConvertToCRS()  function
                        BEFORE you pass it to this function).
            B       -   right part, array[M]

        RESULT:
            This function returns no result.
            You can get solution by calling LinCGResults()
            
        NOTE: this function uses lightweight preconditioning -  multiplication  by
              inverse of diag(A). If you want, you can turn preconditioning off by
              calling LinLSQRSetPrecUnit(). However, preconditioning cost is   low
              and preconditioner is very important for solution  of  badly  scaled
              problems.

          -- ALGLIB --
             Copyright 30.11.2011 by Bochkanov Sergey
        *************************************************************************/
        public static void linlsqrsolvesparse(linlsqrstate state,
            sparse.sparsematrix a,
            double[] b,
            alglib.xparams _params)
        {
            int n = 0;
            int i = 0;
            int j = 0;
            int t0 = 0;
            int t1 = 0;
            double v = 0;

            n = state.n;
            alglib.ap.assert(!state.running, "LinLSQRSolveSparse: you can not call this function when LinLSQRIteration is running");
            alglib.ap.assert(alglib.ap.len(b)>=state.m, "LinLSQRSolveSparse: Length(B)<M");
            alglib.ap.assert(apserv.isfinitevector(b, state.m, _params), "LinLSQRSolveSparse: B contains infinite or NaN values");
            
            //
            // Allocate temporaries
            //
            apserv.rvectorsetlengthatleast(ref state.tmpd, n, _params);
            apserv.rvectorsetlengthatleast(ref state.tmpx, n, _params);
            
            //
            // Compute diagonal scaling matrix D
            //
            if( state.prectype==0 )
            {
                
                //
                // Default preconditioner - inverse of column norms
                //
                for(i=0; i<=n-1; i++)
                {
                    state.tmpd[i] = 0;
                }
                t0 = 0;
                t1 = 0;
                while( sparse.sparseenumerate(a, ref t0, ref t1, ref i, ref j, ref v, _params) )
                {
                    state.tmpd[j] = state.tmpd[j]+math.sqr(v);
                }
                for(i=0; i<=n-1; i++)
                {
                    if( (double)(state.tmpd[i])>(double)(0) )
                    {
                        state.tmpd[i] = 1/Math.Sqrt(state.tmpd[i]);
                    }
                    else
                    {
                        state.tmpd[i] = 1;
                    }
                }
            }
            else
            {
                
                //
                // No diagonal scaling
                //
                for(i=0; i<=n-1; i++)
                {
                    state.tmpd[i] = 1;
                }
            }
            
            //
            // Solve.
            //
            // Instead of solving A*x=b we solve preconditioned system (A*D)*(inv(D)*x)=b.
            // Transformed A is not calculated explicitly, we just modify multiplication
            // by A or A'. After solution we modify State.RX so it will store untransformed
            // variables
            //
            linlsqrsetb(state, b, _params);
            linlsqrrestart(state, _params);
            while( linlsqriteration(state, _params) )
            {
                if( state.needmv )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        state.tmpx[i] = state.tmpd[i]*state.x[i];
                    }
                    sparse.sparsemv(a, state.tmpx, ref state.mv, _params);
                }
                if( state.needmtv )
                {
                    sparse.sparsemtv(a, state.x, ref state.mtv, _params);
                    for(i=0; i<=n-1; i++)
                    {
                        state.mtv[i] = state.tmpd[i]*state.mtv[i];
                    }
                }
            }
            for(i=0; i<=n-1; i++)
            {
                state.rx[i] = state.tmpd[i]*state.rx[i];
            }
        }


        /*************************************************************************
        This function sets stopping criteria.

        INPUT PARAMETERS:
            EpsA    -   algorithm will be stopped if ||A^T*Rk||/(||A||*||Rk||)<=EpsA.
            EpsB    -   algorithm will be stopped if ||Rk||<=EpsB*||B||
            MaxIts  -   algorithm will be stopped if number of iterations
                        more than MaxIts.

        OUTPUT PARAMETERS:
            State   -   structure which stores algorithm state

        NOTE: if EpsA,EpsB,EpsC and MaxIts are zero then these variables will
        be setted as default values.
            
          -- ALGLIB --
             Copyright 30.11.2011 by Bochkanov Sergey
        *************************************************************************/
        public static void linlsqrsetcond(linlsqrstate state,
            double epsa,
            double epsb,
            int maxits,
            alglib.xparams _params)
        {
            alglib.ap.assert(!state.running, "LinLSQRSetCond: you can not call this function when LinLSQRIteration is running");
            alglib.ap.assert(math.isfinite(epsa) && (double)(epsa)>=(double)(0), "LinLSQRSetCond: EpsA is negative, INF or NAN");
            alglib.ap.assert(math.isfinite(epsb) && (double)(epsb)>=(double)(0), "LinLSQRSetCond: EpsB is negative, INF or NAN");
            alglib.ap.assert(maxits>=0, "LinLSQRSetCond: MaxIts is negative");
            if( ((double)(epsa)==(double)(0) && (double)(epsb)==(double)(0)) && maxits==0 )
            {
                state.epsa = atol;
                state.epsb = btol;
                state.maxits = state.n;
            }
            else
            {
                state.epsa = epsa;
                state.epsb = epsb;
                state.maxits = maxits;
            }
        }


        /*************************************************************************
        LSQR solver: results.

        This function must be called after LinLSQRSolve

        INPUT PARAMETERS:
            State   -   algorithm state

        OUTPUT PARAMETERS:
            X       -   array[N], solution
            Rep     -   optimization report:
                        * Rep.TerminationType completetion code:
                            *  1    ||Rk||<=EpsB*||B||
                            *  4    ||A^T*Rk||/(||A||*||Rk||)<=EpsA
                            *  5    MaxIts steps was taken
                            *  7    rounding errors prevent further progress,
                                    X contains best point found so far.
                                    (sometimes returned on singular systems)
                        * Rep.IterationsCount contains iterations count
                        * NMV countains number of matrix-vector calculations
                        
          -- ALGLIB --
             Copyright 30.11.2011 by Bochkanov Sergey
        *************************************************************************/
        public static void linlsqrresults(linlsqrstate state,
            ref double[] x,
            linlsqrreport rep,
            alglib.xparams _params)
        {
            int i_ = 0;

            x = new double[0];

            alglib.ap.assert(!state.running, "LinLSQRResult: you can not call this function when LinLSQRIteration is running");
            if( alglib.ap.len(x)<state.n )
            {
                x = new double[state.n];
            }
            for(i_=0; i_<=state.n-1;i_++)
            {
                x[i_] = state.rx[i_];
            }
            rep.iterationscount = state.repiterationscount;
            rep.nmv = state.repnmv;
            rep.terminationtype = state.repterminationtype;
        }


        /*************************************************************************
        This function turns on/off reporting.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state
            NeedXRep-   whether iteration reports are needed or not

        If NeedXRep is True, algorithm will call rep() callback function if  it is
        provided to MinCGOptimize().

          -- ALGLIB --
             Copyright 30.11.2011 by Bochkanov Sergey
        *************************************************************************/
        public static void linlsqrsetxrep(linlsqrstate state,
            bool needxrep,
            alglib.xparams _params)
        {
            state.xrep = needxrep;
        }


        /*************************************************************************
        This function restarts LinLSQRIteration

          -- ALGLIB --
             Copyright 30.11.2011 by Bochkanov Sergey
        *************************************************************************/
        public static void linlsqrrestart(linlsqrstate state,
            alglib.xparams _params)
        {
            state.rstate.ia = new int[1+1];
            state.rstate.ra = new double[0+1];
            state.rstate.stage = -1;
            clearrfields(state, _params);
        }


        /*************************************************************************
        Clears request fileds (to be sure that we don't forgot to clear something)
        *************************************************************************/
        private static void clearrfields(linlsqrstate state,
            alglib.xparams _params)
        {
            state.xupdated = false;
            state.needmv = false;
            state.needmtv = false;
            state.needmv2 = false;
            state.needvmv = false;
            state.needprec = false;
        }


    }
    public class polynomialsolver
    {
        public class polynomialsolverreport : apobject
        {
            public double maxerr;
            public polynomialsolverreport()
            {
                init();
            }
            public override void init()
            {
            }
            public override alglib.apobject make_copy()
            {
                polynomialsolverreport _result = new polynomialsolverreport();
                _result.maxerr = maxerr;
                return _result;
            }
        };




        /*************************************************************************
        Polynomial root finding.

        This function returns all roots of the polynomial
            P(x) = a0 + a1*x + a2*x^2 + ... + an*x^n
        Both real and complex roots are returned (see below).

        INPUT PARAMETERS:
            A       -   array[N+1], polynomial coefficients:
                        * A[0] is constant term
                        * A[N] is a coefficient of X^N
            N       -   polynomial degree

        OUTPUT PARAMETERS:
            X       -   array of complex roots:
                        * for isolated real root, X[I] is strictly real: IMAGE(X[I])=0
                        * complex roots are always returned in pairs - roots occupy
                          positions I and I+1, with:
                          * X[I+1]=Conj(X[I])
                          * IMAGE(X[I]) > 0
                          * IMAGE(X[I+1]) = -IMAGE(X[I]) < 0
                        * multiple real roots may have non-zero imaginary part due
                          to roundoff errors. There is no reliable way to distinguish
                          real root of multiplicity 2 from two  complex  roots  in
                          the presence of roundoff errors.
            Rep     -   report, additional information, following fields are set:
                        * Rep.MaxErr - max( |P(xi)| )  for  i=0..N-1.  This  field
                          allows to quickly estimate "quality" of the roots  being
                          returned.

        NOTE:   this function uses companion matrix method to find roots. In  case
                internal EVD  solver  fails  do  find  eigenvalues,  exception  is
                generated.

        NOTE:   roots are not "polished" and  no  matrix  balancing  is  performed
                for them.

          -- ALGLIB --
             Copyright 24.02.2014 by Bochkanov Sergey
        *************************************************************************/
        public static void polynomialsolve(double[] a,
            int n,
            ref complex[] x,
            polynomialsolverreport rep,
            alglib.xparams _params)
        {
            double[,] c = new double[0,0];
            double[,] vl = new double[0,0];
            double[,] vr = new double[0,0];
            double[] wr = new double[0];
            double[] wi = new double[0];
            int i = 0;
            int j = 0;
            bool status = new bool();
            int nz = 0;
            int ne = 0;
            complex v = 0;
            complex vv = 0;

            a = (double[])a.Clone();
            x = new complex[0];

            alglib.ap.assert(n>0, "PolynomialSolve: N<=0");
            alglib.ap.assert(alglib.ap.len(a)>=n+1, "PolynomialSolve: Length(A)<N+1");
            alglib.ap.assert(apserv.isfinitevector(a, n+1, _params), "PolynomialSolve: A contains infitite numbers");
            alglib.ap.assert((double)(a[n])!=(double)(0), "PolynomialSolve: A[N]=0");
            
            //
            // Prepare
            //
            x = new complex[n];
            
            //
            // Normalize A:
            // * analytically determine NZ zero roots
            // * quick exit for NZ=N
            // * make residual NE-th degree polynomial monic
            //   (here NE=N-NZ)
            //
            nz = 0;
            while( nz<n && (double)(a[nz])==(double)(0) )
            {
                nz = nz+1;
            }
            ne = n-nz;
            for(i=nz; i<=n; i++)
            {
                a[i-nz] = a[i]/a[n];
            }
            
            //
            // For NZ<N, build companion matrix and find NE non-zero roots
            //
            if( ne>0 )
            {
                c = new double[ne, ne];
                for(i=0; i<=ne-1; i++)
                {
                    for(j=0; j<=ne-1; j++)
                    {
                        c[i,j] = 0;
                    }
                }
                c[0,ne-1] = -a[0];
                for(i=1; i<=ne-1; i++)
                {
                    c[i,i-1] = 1;
                    c[i,ne-1] = -a[i];
                }
                status = evd.rmatrixevd(c, ne, 0, ref wr, ref wi, ref vl, ref vr, _params);
                alglib.ap.assert(status, "PolynomialSolve: inernal error - EVD solver failed");
                for(i=0; i<=ne-1; i++)
                {
                    x[i].x = wr[i];
                    x[i].y = wi[i];
                }
            }
            
            //
            // Remaining NZ zero roots
            //
            for(i=ne; i<=n-1; i++)
            {
                x[i] = 0;
            }
            
            //
            // Rep
            //
            rep.maxerr = 0;
            for(i=0; i<=ne-1; i++)
            {
                v = 0;
                vv = 1;
                for(j=0; j<=ne; j++)
                {
                    v = v+a[j]*vv;
                    vv = vv*x[i];
                }
                rep.maxerr = Math.Max(rep.maxerr, math.abscomplex(v));
            }
        }


    }
    public class nleq
    {
        public class nleqstate : apobject
        {
            public int n;
            public int m;
            public double epsf;
            public int maxits;
            public bool xrep;
            public double stpmax;
            public double[] x;
            public double f;
            public double[] fi;
            public double[,] j;
            public bool needf;
            public bool needfij;
            public bool xupdated;
            public rcommstate rstate;
            public int repiterationscount;
            public int repnfunc;
            public int repnjac;
            public int repterminationtype;
            public double[] xbase;
            public double fbase;
            public double fprev;
            public double[] candstep;
            public double[] rightpart;
            public double[] cgbuf;
            public nleqstate()
            {
                init();
            }
            public override void init()
            {
                x = new double[0];
                fi = new double[0];
                j = new double[0,0];
                rstate = new rcommstate();
                xbase = new double[0];
                candstep = new double[0];
                rightpart = new double[0];
                cgbuf = new double[0];
            }
            public override alglib.apobject make_copy()
            {
                nleqstate _result = new nleqstate();
                _result.n = n;
                _result.m = m;
                _result.epsf = epsf;
                _result.maxits = maxits;
                _result.xrep = xrep;
                _result.stpmax = stpmax;
                _result.x = (double[])x.Clone();
                _result.f = f;
                _result.fi = (double[])fi.Clone();
                _result.j = (double[,])j.Clone();
                _result.needf = needf;
                _result.needfij = needfij;
                _result.xupdated = xupdated;
                _result.rstate = (rcommstate)rstate.make_copy();
                _result.repiterationscount = repiterationscount;
                _result.repnfunc = repnfunc;
                _result.repnjac = repnjac;
                _result.repterminationtype = repterminationtype;
                _result.xbase = (double[])xbase.Clone();
                _result.fbase = fbase;
                _result.fprev = fprev;
                _result.candstep = (double[])candstep.Clone();
                _result.rightpart = (double[])rightpart.Clone();
                _result.cgbuf = (double[])cgbuf.Clone();
                return _result;
            }
        };


        public class nleqreport : apobject
        {
            public int iterationscount;
            public int nfunc;
            public int njac;
            public int terminationtype;
            public nleqreport()
            {
                init();
            }
            public override void init()
            {
            }
            public override alglib.apobject make_copy()
            {
                nleqreport _result = new nleqreport();
                _result.iterationscount = iterationscount;
                _result.nfunc = nfunc;
                _result.njac = njac;
                _result.terminationtype = terminationtype;
                return _result;
            }
        };




        /*************************************************************************
                        LEVENBERG-MARQUARDT-LIKE NONLINEAR SOLVER

        DESCRIPTION:
        This algorithm solves system of nonlinear equations
            F[0](x[0], ..., x[n-1])   = 0
            F[1](x[0], ..., x[n-1])   = 0
            ...
            F[M-1](x[0], ..., x[n-1]) = 0
        with M/N do not necessarily coincide.  Algorithm  converges  quadratically
        under following conditions:
            * the solution set XS is nonempty
            * for some xs in XS there exist such neighbourhood N(xs) that:
              * vector function F(x) and its Jacobian J(x) are continuously
                differentiable on N
              * ||F(x)|| provides local error bound on N, i.e. there  exists  such
                c1, that ||F(x)||>c1*distance(x,XS)
        Note that these conditions are much more weaker than usual non-singularity
        conditions. For example, algorithm will converge for any  affine  function
        F (whether its Jacobian singular or not).


        REQUIREMENTS:
        Algorithm will request following information during its operation:
        * function vector F[] and Jacobian matrix at given point X
        * value of merit function f(x)=F[0]^2(x)+...+F[M-1]^2(x) at given point X


        USAGE:
        1. User initializes algorithm state with NLEQCreateLM() call
        2. User tunes solver parameters with  NLEQSetCond(),  NLEQSetStpMax()  and
           other functions
        3. User  calls  NLEQSolve()  function  which  takes  algorithm  state  and
           pointers (delegates, etc.) to callback functions which calculate  merit
           function value and Jacobian.
        4. User calls NLEQResults() to get solution
        5. Optionally, user may call NLEQRestartFrom() to  solve  another  problem
           with same parameters (N/M) but another starting  point  and/or  another
           function vector. NLEQRestartFrom() allows to reuse already  initialized
           structure.


        INPUT PARAMETERS:
            N       -   space dimension, N>1:
                        * if provided, only leading N elements of X are used
                        * if not provided, determined automatically from size of X
            M       -   system size
            X       -   starting point


        OUTPUT PARAMETERS:
            State   -   structure which stores algorithm state


        NOTES:
        1. you may tune stopping conditions with NLEQSetCond() function
        2. if target function contains exp() or other fast growing functions,  and
           optimization algorithm makes too large steps which leads  to  overflow,
           use NLEQSetStpMax() function to bound algorithm's steps.
        3. this  algorithm  is  a  slightly  modified implementation of the method
           described  in  'Levenberg-Marquardt  method  for constrained  nonlinear
           equations with strong local convergence properties' by Christian Kanzow
           Nobuo Yamashita and Masao Fukushima and further  developed  in  'On the
           convergence of a New Levenberg-Marquardt Method'  by  Jin-yan  Fan  and
           Ya-Xiang Yuan.


          -- ALGLIB --
             Copyright 20.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void nleqcreatelm(int n,
            int m,
            double[] x,
            nleqstate state,
            alglib.xparams _params)
        {
            alglib.ap.assert(n>=1, "NLEQCreateLM: N<1!");
            alglib.ap.assert(m>=1, "NLEQCreateLM: M<1!");
            alglib.ap.assert(alglib.ap.len(x)>=n, "NLEQCreateLM: Length(X)<N!");
            alglib.ap.assert(apserv.isfinitevector(x, n, _params), "NLEQCreateLM: X contains infinite or NaN values!");
            
            //
            // Initialize
            //
            state.n = n;
            state.m = m;
            nleqsetcond(state, 0, 0, _params);
            nleqsetxrep(state, false, _params);
            nleqsetstpmax(state, 0, _params);
            state.x = new double[n];
            state.xbase = new double[n];
            state.j = new double[m, n];
            state.fi = new double[m];
            state.rightpart = new double[n];
            state.candstep = new double[n];
            nleqrestartfrom(state, x, _params);
        }


        /*************************************************************************
        This function sets stopping conditions for the nonlinear solver

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state
            EpsF    -   >=0
                        The subroutine finishes  its work if on k+1-th iteration
                        the condition ||F||<=EpsF is satisfied
            MaxIts  -   maximum number of iterations. If MaxIts=0, the  number  of
                        iterations is unlimited.

        Passing EpsF=0 and MaxIts=0 simultaneously will lead to  automatic
        stopping criterion selection (small EpsF).

        NOTES:

          -- ALGLIB --
             Copyright 20.08.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void nleqsetcond(nleqstate state,
            double epsf,
            int maxits,
            alglib.xparams _params)
        {
            alglib.ap.assert(math.isfinite(epsf), "NLEQSetCond: EpsF is not finite number!");
            alglib.ap.assert((double)(epsf)>=(double)(0), "NLEQSetCond: negative EpsF!");
            alglib.ap.assert(maxits>=0, "NLEQSetCond: negative MaxIts!");
            if( (double)(epsf)==(double)(0) && maxits==0 )
            {
                epsf = 1.0E-6;
            }
            state.epsf = epsf;
            state.maxits = maxits;
        }


        /*************************************************************************
        This function turns on/off reporting.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state
            NeedXRep-   whether iteration reports are needed or not

        If NeedXRep is True, algorithm will call rep() callback function if  it is
        provided to NLEQSolve().

          -- ALGLIB --
             Copyright 20.08.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void nleqsetxrep(nleqstate state,
            bool needxrep,
            alglib.xparams _params)
        {
            state.xrep = needxrep;
        }


        /*************************************************************************
        This function sets maximum step length

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state
            StpMax  -   maximum step length, >=0. Set StpMax to 0.0,  if you don't
                        want to limit step length.

        Use this subroutine when target function  contains  exp()  or  other  fast
        growing functions, and algorithm makes  too  large  steps  which  lead  to
        overflow. This function allows us to reject steps that are too large  (and
        therefore expose us to the possible overflow) without actually calculating
        function value at the x+stp*d.

          -- ALGLIB --
             Copyright 20.08.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void nleqsetstpmax(nleqstate state,
            double stpmax,
            alglib.xparams _params)
        {
            alglib.ap.assert(math.isfinite(stpmax), "NLEQSetStpMax: StpMax is not finite!");
            alglib.ap.assert((double)(stpmax)>=(double)(0), "NLEQSetStpMax: StpMax<0!");
            state.stpmax = stpmax;
        }


        /*************************************************************************

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static bool nleqiteration(nleqstate state,
            alglib.xparams _params)
        {
            bool result = new bool();
            int n = 0;
            int m = 0;
            int i = 0;
            double lambdaup = 0;
            double lambdadown = 0;
            double lambdav = 0;
            double rho = 0;
            double mu = 0;
            double stepnorm = 0;
            bool b = new bool();
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
                b = state.rstate.ba[0];
                lambdaup = state.rstate.ra[0];
                lambdadown = state.rstate.ra[1];
                lambdav = state.rstate.ra[2];
                rho = state.rstate.ra[3];
                mu = state.rstate.ra[4];
                stepnorm = state.rstate.ra[5];
            }
            else
            {
                n = 359;
                m = -58;
                i = -919;
                b = true;
                lambdaup = 81;
                lambdadown = 255;
                lambdav = 74;
                rho = -788;
                mu = 809;
                stepnorm = 205;
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
            if( state.rstate.stage==4 )
            {
                goto lbl_4;
            }
            
            //
            // Routine body
            //
            
            //
            // Prepare
            //
            n = state.n;
            m = state.m;
            state.repterminationtype = 0;
            state.repiterationscount = 0;
            state.repnfunc = 0;
            state.repnjac = 0;
            
            //
            // Calculate F/G, initialize algorithm
            //
            clearrequestfields(state, _params);
            state.needf = true;
            state.rstate.stage = 0;
            goto lbl_rcomm;
        lbl_0:
            state.needf = false;
            state.repnfunc = state.repnfunc+1;
            for(i_=0; i_<=n-1;i_++)
            {
                state.xbase[i_] = state.x[i_];
            }
            state.fbase = state.f;
            state.fprev = math.maxrealnumber;
            if( !state.xrep )
            {
                goto lbl_5;
            }
            
            //
            // progress report
            //
            clearrequestfields(state, _params);
            state.xupdated = true;
            state.rstate.stage = 1;
            goto lbl_rcomm;
        lbl_1:
            state.xupdated = false;
        lbl_5:
            if( (double)(state.f)<=(double)(math.sqr(state.epsf)) )
            {
                state.repterminationtype = 1;
                result = false;
                return result;
            }
            
            //
            // Main cycle
            //
            lambdaup = 10;
            lambdadown = 0.3;
            lambdav = 0.001;
            rho = 1;
        lbl_7:
            if( false )
            {
                goto lbl_8;
            }
            
            //
            // Get Jacobian;
            // before we get to this point we already have State.XBase filled
            // with current point and State.FBase filled with function value
            // at XBase
            //
            clearrequestfields(state, _params);
            state.needfij = true;
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            state.rstate.stage = 2;
            goto lbl_rcomm;
        lbl_2:
            state.needfij = false;
            state.repnfunc = state.repnfunc+1;
            state.repnjac = state.repnjac+1;
            ablas.rmatrixmv(n, m, state.j, 0, 0, 1, state.fi, 0, state.rightpart, 0, _params);
            for(i_=0; i_<=n-1;i_++)
            {
                state.rightpart[i_] = -1*state.rightpart[i_];
            }
            
            //
            // Inner cycle: find good lambda
            //
        lbl_9:
            if( false )
            {
                goto lbl_10;
            }
            
            //
            // Solve (J^T*J + (Lambda+Mu)*I)*y = J^T*F
            // to get step d=-y where:
            // * Mu=||F|| - is damping parameter for nonlinear system
            // * Lambda   - is additional Levenberg-Marquardt parameter
            //              for better convergence when far away from minimum
            //
            for(i=0; i<=n-1; i++)
            {
                state.candstep[i] = 0;
            }
            fbls.fblssolvecgx(state.j, m, n, lambdav, state.rightpart, ref state.candstep, ref state.cgbuf, _params);
            
            //
            // Normalize step (it must be no more than StpMax)
            //
            stepnorm = 0;
            for(i=0; i<=n-1; i++)
            {
                if( (double)(state.candstep[i])!=(double)(0) )
                {
                    stepnorm = 1;
                    break;
                }
            }
            linmin.linminnormalized(ref state.candstep, ref stepnorm, n, _params);
            if( (double)(state.stpmax)!=(double)(0) )
            {
                stepnorm = Math.Min(stepnorm, state.stpmax);
            }
            
            //
            // Test new step - is it good enough?
            // * if not, Lambda is increased and we try again.
            // * if step is good, we decrease Lambda and move on.
            //
            // We can break this cycle on two occasions:
            // * step is so small that x+step==x (in floating point arithmetics)
            // * lambda is so large
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.x[i_] + stepnorm*state.candstep[i_];
            }
            b = true;
            for(i=0; i<=n-1; i++)
            {
                if( (double)(state.x[i])!=(double)(state.xbase[i]) )
                {
                    b = false;
                    break;
                }
            }
            if( b )
            {
                
                //
                // Step is too small, force zero step and break
                //
                stepnorm = 0;
                for(i_=0; i_<=n-1;i_++)
                {
                    state.x[i_] = state.xbase[i_];
                }
                state.f = state.fbase;
                goto lbl_10;
            }
            clearrequestfields(state, _params);
            state.needf = true;
            state.rstate.stage = 3;
            goto lbl_rcomm;
        lbl_3:
            state.needf = false;
            state.repnfunc = state.repnfunc+1;
            if( (double)(state.f)<(double)(state.fbase) )
            {
                
                //
                // function value decreased, move on
                //
                decreaselambda(ref lambdav, ref rho, lambdadown, _params);
                goto lbl_10;
            }
            if( !increaselambda(ref lambdav, ref rho, lambdaup, _params) )
            {
                
                //
                // Lambda is too large (near overflow), force zero step and break
                //
                stepnorm = 0;
                for(i_=0; i_<=n-1;i_++)
                {
                    state.x[i_] = state.xbase[i_];
                }
                state.f = state.fbase;
                goto lbl_10;
            }
            goto lbl_9;
        lbl_10:
            
            //
            // Accept step:
            // * new position
            // * new function value
            //
            state.fbase = state.f;
            for(i_=0; i_<=n-1;i_++)
            {
                state.xbase[i_] = state.xbase[i_] + stepnorm*state.candstep[i_];
            }
            state.repiterationscount = state.repiterationscount+1;
            
            //
            // Report new iteration
            //
            if( !state.xrep )
            {
                goto lbl_11;
            }
            clearrequestfields(state, _params);
            state.xupdated = true;
            state.f = state.fbase;
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            state.rstate.stage = 4;
            goto lbl_rcomm;
        lbl_4:
            state.xupdated = false;
        lbl_11:
            
            //
            // Test stopping conditions on F, step (zero/non-zero) and MaxIts;
            // If one of the conditions is met, RepTerminationType is changed.
            //
            if( (double)(Math.Sqrt(state.f))<=(double)(state.epsf) )
            {
                state.repterminationtype = 1;
            }
            if( (double)(stepnorm)==(double)(0) && state.repterminationtype==0 )
            {
                state.repterminationtype = -4;
            }
            if( state.repiterationscount>=state.maxits && state.maxits>0 )
            {
                state.repterminationtype = 5;
            }
            if( state.repterminationtype!=0 )
            {
                goto lbl_8;
            }
            
            //
            // Now, iteration is finally over
            //
            goto lbl_7;
        lbl_8:
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
            state.rstate.ba[0] = b;
            state.rstate.ra[0] = lambdaup;
            state.rstate.ra[1] = lambdadown;
            state.rstate.ra[2] = lambdav;
            state.rstate.ra[3] = rho;
            state.rstate.ra[4] = mu;
            state.rstate.ra[5] = stepnorm;
            return result;
        }


        /*************************************************************************
        NLEQ solver results

        INPUT PARAMETERS:
            State   -   algorithm state.

        OUTPUT PARAMETERS:
            X       -   array[0..N-1], solution
            Rep     -   optimization report:
                        * Rep.TerminationType completetion code:
                            * -4    ERROR:  algorithm   has   converged   to   the
                                    stationary point Xf which is local minimum  of
                                    f=F[0]^2+...+F[m-1]^2, but is not solution  of
                                    nonlinear system.
                            *  1    sqrt(f)<=EpsF.
                            *  5    MaxIts steps was taken
                            *  7    stopping conditions are too stringent,
                                    further improvement is impossible
                        * Rep.IterationsCount contains iterations count
                        * NFEV countains number of function calculations
                        * ActiveConstraints contains number of active constraints

          -- ALGLIB --
             Copyright 20.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void nleqresults(nleqstate state,
            ref double[] x,
            nleqreport rep,
            alglib.xparams _params)
        {
            x = new double[0];

            nleqresultsbuf(state, ref x, rep, _params);
        }


        /*************************************************************************
        NLEQ solver results

        Buffered implementation of NLEQResults(), which uses pre-allocated  buffer
        to store X[]. If buffer size is  too  small,  it  resizes  buffer.  It  is
        intended to be used in the inner cycles of performance critical algorithms
        where array reallocation penalty is too large to be ignored.

          -- ALGLIB --
             Copyright 20.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void nleqresultsbuf(nleqstate state,
            ref double[] x,
            nleqreport rep,
            alglib.xparams _params)
        {
            int i_ = 0;

            if( alglib.ap.len(x)<state.n )
            {
                x = new double[state.n];
            }
            for(i_=0; i_<=state.n-1;i_++)
            {
                x[i_] = state.xbase[i_];
            }
            rep.iterationscount = state.repiterationscount;
            rep.nfunc = state.repnfunc;
            rep.njac = state.repnjac;
            rep.terminationtype = state.repterminationtype;
        }


        /*************************************************************************
        This  subroutine  restarts  CG  algorithm from new point. All optimization
        parameters are left unchanged.

        This  function  allows  to  solve multiple  optimization  problems  (which
        must have same number of dimensions) without object reallocation penalty.

        INPUT PARAMETERS:
            State   -   structure used for reverse communication previously
                        allocated with MinCGCreate call.
            X       -   new starting point.
            BndL    -   new lower bounds
            BndU    -   new upper bounds

          -- ALGLIB --
             Copyright 30.07.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void nleqrestartfrom(nleqstate state,
            double[] x,
            alglib.xparams _params)
        {
            int i_ = 0;

            alglib.ap.assert(alglib.ap.len(x)>=state.n, "NLEQRestartFrom: Length(X)<N!");
            alglib.ap.assert(apserv.isfinitevector(x, state.n, _params), "NLEQRestartFrom: X contains infinite or NaN values!");
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.x[i_] = x[i_];
            }
            state.rstate.ia = new int[2+1];
            state.rstate.ba = new bool[0+1];
            state.rstate.ra = new double[5+1];
            state.rstate.stage = -1;
            clearrequestfields(state, _params);
        }


        /*************************************************************************
        Clears request fileds (to be sure that we don't forgot to clear something)
        *************************************************************************/
        private static void clearrequestfields(nleqstate state,
            alglib.xparams _params)
        {
            state.needf = false;
            state.needfij = false;
            state.xupdated = false;
        }


        /*************************************************************************
        Increases lambda, returns False when there is a danger of overflow
        *************************************************************************/
        private static bool increaselambda(ref double lambdav,
            ref double nu,
            double lambdaup,
            alglib.xparams _params)
        {
            bool result = new bool();
            double lnlambda = 0;
            double lnnu = 0;
            double lnlambdaup = 0;
            double lnmax = 0;

            result = false;
            lnlambda = Math.Log(lambdav);
            lnlambdaup = Math.Log(lambdaup);
            lnnu = Math.Log(nu);
            lnmax = 0.5*Math.Log(math.maxrealnumber);
            if( (double)(lnlambda+lnlambdaup+lnnu)>(double)(lnmax) )
            {
                return result;
            }
            if( (double)(lnnu+Math.Log(2))>(double)(lnmax) )
            {
                return result;
            }
            lambdav = lambdav*lambdaup*nu;
            nu = nu*2;
            result = true;
            return result;
        }


        /*************************************************************************
        Decreases lambda, but leaves it unchanged when there is danger of underflow.
        *************************************************************************/
        private static void decreaselambda(ref double lambdav,
            ref double nu,
            double lambdadown,
            alglib.xparams _params)
        {
            nu = 1;
            if( (double)(Math.Log(lambdav)+Math.Log(lambdadown))<(double)(Math.Log(math.minrealnumber)) )
            {
                lambdav = math.minrealnumber;
            }
            else
            {
                lambdav = lambdav*lambdadown;
            }
        }


    }
    public class directsparsesolvers
    {
        /*************************************************************************
        This structure is a sparse solver report.

        Following fields can be accessed by users:
        *************************************************************************/
        public class sparsesolverreport : apobject
        {
            public int terminationtype;
            public sparsesolverreport()
            {
                init();
            }
            public override void init()
            {
            }
            public override alglib.apobject make_copy()
            {
                sparsesolverreport _result = new sparsesolverreport();
                _result.terminationtype = terminationtype;
                return _result;
            }
        };




        /*************************************************************************
        Sparse linear solver for A*x=b with N*N  sparse  real  symmetric  positive
        definite matrix A, N*1 vectors x and b.

        This solver  converts  input  matrix  to  SKS  format,  performs  Cholesky
        factorization using  SKS  Cholesky  subroutine  (works  well  for  limited
        bandwidth matrices) and uses sparse triangular solvers to get solution  of
        the original system.

        INPUT PARAMETERS
            A       -   sparse matrix, must be NxN exactly
            N       -   size of A, N>0
            IsUpper -   which half of A is provided (another half is ignored)
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Rep     -   solver report, following fields are set:
                        * rep.terminationtype - solver status; >0 for success,
                          set to -3 on failure (degenerate or non-SPD system).
            X       -   array[N], it contains:
                        * rep.terminationtype>0    =>  solution
                        * rep.terminationtype=-3   =>  filled by zeros

          -- ALGLIB --
             Copyright 26.12.2017 by Bochkanov Sergey
        *************************************************************************/
        public static void sparsesolvesks(sparse.sparsematrix a,
            int n,
            bool isupper,
            double[] b,
            sparsesolverreport rep,
            ref double[] x,
            alglib.xparams _params)
        {
            int i = 0;
            sparse.sparsematrix a2 = new sparse.sparsematrix();

            x = new double[0];

            alglib.ap.assert(n>0, "SparseSolveSKS: N<=0");
            alglib.ap.assert(sparse.sparsegetnrows(a, _params)==n, "SparseSolveSKS: rows(A)!=N");
            alglib.ap.assert(sparse.sparsegetncols(a, _params)==n, "SparseSolveSKS: cols(A)!=N");
            alglib.ap.assert(alglib.ap.len(b)>=n, "SparseSolveSKS: length(B)<N");
            alglib.ap.assert(apserv.isfinitevector(b, n, _params), "SparseSolveSKS: B contains infinities or NANs");
            initreport(rep, _params);
            x = new double[n];
            sparse.sparsecopytosks(a, a2, _params);
            if( !trfac.sparsecholeskyskyline(a2, n, isupper, _params) )
            {
                rep.terminationtype = -3;
                for(i=0; i<=n-1; i++)
                {
                    x[i] = 0;
                }
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                x[i] = b[i];
            }
            if( isupper )
            {
                sparse.sparsetrsv(a2, isupper, false, 1, x, _params);
                sparse.sparsetrsv(a2, isupper, false, 0, x, _params);
            }
            else
            {
                sparse.sparsetrsv(a2, isupper, false, 0, x, _params);
                sparse.sparsetrsv(a2, isupper, false, 1, x, _params);
            }
            rep.terminationtype = 1;
        }


        /*************************************************************************
        Sparse linear solver for A*x=b with N*N real  symmetric  positive definite
        matrix A given by its Cholesky decomposition, and N*1 vectors x and b.

        IMPORTANT: this solver requires input matrix to be in  the  SKS  (Skyline)
                   sparse storage format. An exception will be  generated  if  you
                   pass matrix in some other format (HASH or CRS).

        INPUT PARAMETERS
            A       -   sparse NxN matrix stored in SKS format, must be NxN exactly
            N       -   size of A, N>0
            IsUpper -   which half of A is provided (another half is ignored)
            B       -   array[N], right part

        OUTPUT PARAMETERS
            Rep     -   solver report, following fields are set:
                        * rep.terminationtype - solver status; >0 for success,
                          set to -3 on failure (degenerate or non-SPD system).
            X       -   array[N], it contains:
                        * rep.terminationtype>0    =>  solution
                        * rep.terminationtype=-3   =>  filled by zeros

          -- ALGLIB --
             Copyright 26.12.2017 by Bochkanov Sergey
        *************************************************************************/
        public static void sparsecholeskysolvesks(sparse.sparsematrix a,
            int n,
            bool isupper,
            double[] b,
            sparsesolverreport rep,
            ref double[] x,
            alglib.xparams _params)
        {
            int i = 0;

            x = new double[0];

            alglib.ap.assert(n>0, "SparseSolveSKS: N<=0");
            alglib.ap.assert(sparse.sparsegetnrows(a, _params)==n, "SparseSolveSKS: rows(A)!=N");
            alglib.ap.assert(sparse.sparsegetncols(a, _params)==n, "SparseSolveSKS: cols(A)!=N");
            alglib.ap.assert(sparse.sparseissks(a, _params), "SparseSolveSKS: A is not an SKS matrix");
            alglib.ap.assert(alglib.ap.len(b)>=n, "SparseSolveSKS: length(B)<N");
            alglib.ap.assert(apserv.isfinitevector(b, n, _params), "SparseSolveSKS: B contains infinities or NANs");
            initreport(rep, _params);
            x = new double[n];
            for(i=0; i<=n-1; i++)
            {
                if( (double)(sparse.sparseget(a, i, i, _params))==(double)(0.0) )
                {
                    rep.terminationtype = -3;
                    for(i=0; i<=n-1; i++)
                    {
                        x[i] = 0;
                    }
                    return;
                }
            }
            for(i=0; i<=n-1; i++)
            {
                x[i] = b[i];
            }
            if( isupper )
            {
                sparse.sparsetrsv(a, isupper, false, 1, x, _params);
                sparse.sparsetrsv(a, isupper, false, 0, x, _params);
            }
            else
            {
                sparse.sparsetrsv(a, isupper, false, 0, x, _params);
                sparse.sparsetrsv(a, isupper, false, 1, x, _params);
            }
            rep.terminationtype = 1;
        }


        /*************************************************************************
        Reset report fields

          -- ALGLIB --
             Copyright 26.12.2017 by Bochkanov Sergey
        *************************************************************************/
        private static void initreport(sparsesolverreport rep,
            alglib.xparams _params)
        {
            rep.terminationtype = 0;
        }


    }
    public class lincg
    {
        /*************************************************************************
        This object stores state of the linear CG method.

        You should use ALGLIB functions to work with this object.
        Never try to access its fields directly!
        *************************************************************************/
        public class lincgstate : apobject
        {
            public double[] rx;
            public double[] b;
            public int n;
            public int prectype;
            public double[] cx;
            public double[] cr;
            public double[] cz;
            public double[] p;
            public double[] r;
            public double[] z;
            public double alpha;
            public double beta;
            public double r2;
            public double meritfunction;
            public double[] x;
            public double[] mv;
            public double[] pv;
            public double vmv;
            public double[] startx;
            public double epsf;
            public int maxits;
            public int itsbeforerestart;
            public int itsbeforerupdate;
            public bool xrep;
            public bool xupdated;
            public bool needmv;
            public bool needmtv;
            public bool needmv2;
            public bool needvmv;
            public bool needprec;
            public int repiterationscount;
            public int repnmv;
            public int repterminationtype;
            public bool running;
            public double[] tmpd;
            public rcommstate rstate;
            public lincgstate()
            {
                init();
            }
            public override void init()
            {
                rx = new double[0];
                b = new double[0];
                cx = new double[0];
                cr = new double[0];
                cz = new double[0];
                p = new double[0];
                r = new double[0];
                z = new double[0];
                x = new double[0];
                mv = new double[0];
                pv = new double[0];
                startx = new double[0];
                tmpd = new double[0];
                rstate = new rcommstate();
            }
            public override alglib.apobject make_copy()
            {
                lincgstate _result = new lincgstate();
                _result.rx = (double[])rx.Clone();
                _result.b = (double[])b.Clone();
                _result.n = n;
                _result.prectype = prectype;
                _result.cx = (double[])cx.Clone();
                _result.cr = (double[])cr.Clone();
                _result.cz = (double[])cz.Clone();
                _result.p = (double[])p.Clone();
                _result.r = (double[])r.Clone();
                _result.z = (double[])z.Clone();
                _result.alpha = alpha;
                _result.beta = beta;
                _result.r2 = r2;
                _result.meritfunction = meritfunction;
                _result.x = (double[])x.Clone();
                _result.mv = (double[])mv.Clone();
                _result.pv = (double[])pv.Clone();
                _result.vmv = vmv;
                _result.startx = (double[])startx.Clone();
                _result.epsf = epsf;
                _result.maxits = maxits;
                _result.itsbeforerestart = itsbeforerestart;
                _result.itsbeforerupdate = itsbeforerupdate;
                _result.xrep = xrep;
                _result.xupdated = xupdated;
                _result.needmv = needmv;
                _result.needmtv = needmtv;
                _result.needmv2 = needmv2;
                _result.needvmv = needvmv;
                _result.needprec = needprec;
                _result.repiterationscount = repiterationscount;
                _result.repnmv = repnmv;
                _result.repterminationtype = repterminationtype;
                _result.running = running;
                _result.tmpd = (double[])tmpd.Clone();
                _result.rstate = (rcommstate)rstate.make_copy();
                return _result;
            }
        };


        public class lincgreport : apobject
        {
            public int iterationscount;
            public int nmv;
            public int terminationtype;
            public double r2;
            public lincgreport()
            {
                init();
            }
            public override void init()
            {
            }
            public override alglib.apobject make_copy()
            {
                lincgreport _result = new lincgreport();
                _result.iterationscount = iterationscount;
                _result.nmv = nmv;
                _result.terminationtype = terminationtype;
                _result.r2 = r2;
                return _result;
            }
        };




        public const double defaultprecision = 1.0E-6;


        /*************************************************************************
        This function initializes linear CG Solver. This solver is used  to  solve
        symmetric positive definite problems. If you want  to  solve  nonsymmetric
        (or non-positive definite) problem you may use LinLSQR solver provided  by
        ALGLIB.

        USAGE:
        1. User initializes algorithm state with LinCGCreate() call
        2. User tunes solver parameters with  LinCGSetCond() and other functions
        3. Optionally, user sets starting point with LinCGSetStartingPoint()
        4. User  calls LinCGSolveSparse() function which takes algorithm state and
           SparseMatrix object.
        5. User calls LinCGResults() to get solution
        6. Optionally, user may call LinCGSolveSparse()  again  to  solve  another
           problem  with different matrix and/or right part without reinitializing
           LinCGState structure.
          
        INPUT PARAMETERS:
            N       -   problem dimension, N>0

        OUTPUT PARAMETERS:
            State   -   structure which stores algorithm state

          -- ALGLIB --
             Copyright 14.11.2011 by Bochkanov Sergey
        *************************************************************************/
        public static void lincgcreate(int n,
            lincgstate state,
            alglib.xparams _params)
        {
            int i = 0;

            alglib.ap.assert(n>0, "LinCGCreate: N<=0");
            state.n = n;
            state.prectype = 0;
            state.itsbeforerestart = n;
            state.itsbeforerupdate = 10;
            state.epsf = defaultprecision;
            state.maxits = 0;
            state.xrep = false;
            state.running = false;
            
            //
            // * allocate arrays
            // * set RX to NAN (just for the case user calls Results() without 
            //   calling SolveSparse()
            // * set starting point to zero
            // * we do NOT initialize B here because we assume that user should
            //   initializate it using LinCGSetB() function. In case he forgets
            //   to do so, exception will be thrown in the LinCGIteration().
            //
            state.rx = new double[state.n];
            state.startx = new double[state.n];
            state.b = new double[state.n];
            for(i=0; i<=state.n-1; i++)
            {
                state.rx[i] = Double.NaN;
                state.startx[i] = 0.0;
                state.b[i] = 0;
            }
            state.cx = new double[state.n];
            state.p = new double[state.n];
            state.r = new double[state.n];
            state.cr = new double[state.n];
            state.z = new double[state.n];
            state.cz = new double[state.n];
            state.x = new double[state.n];
            state.mv = new double[state.n];
            state.pv = new double[state.n];
            updateitersdata(state, _params);
            state.rstate.ia = new int[0+1];
            state.rstate.ra = new double[2+1];
            state.rstate.stage = -1;
        }


        /*************************************************************************
        This function sets starting point.
        By default, zero starting point is used.

        INPUT PARAMETERS:
            X       -   starting point, array[N]

        OUTPUT PARAMETERS:
            State   -   structure which stores algorithm state

          -- ALGLIB --
             Copyright 14.11.2011 by Bochkanov Sergey
        *************************************************************************/
        public static void lincgsetstartingpoint(lincgstate state,
            double[] x,
            alglib.xparams _params)
        {
            int i_ = 0;

            alglib.ap.assert(!state.running, "LinCGSetStartingPoint: you can not change starting point because LinCGIteration() function is running");
            alglib.ap.assert(state.n<=alglib.ap.len(x), "LinCGSetStartingPoint: Length(X)<N");
            alglib.ap.assert(apserv.isfinitevector(x, state.n, _params), "LinCGSetStartingPoint: X contains infinite or NaN values!");
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.startx[i_] = x[i_];
            }
        }


        /*************************************************************************
        This function sets right part. By default, right part is zero.

        INPUT PARAMETERS:
            B       -   right part, array[N].

        OUTPUT PARAMETERS:
            State   -   structure which stores algorithm state

          -- ALGLIB --
             Copyright 14.11.2011 by Bochkanov Sergey
        *************************************************************************/
        public static void lincgsetb(lincgstate state,
            double[] b,
            alglib.xparams _params)
        {
            int i_ = 0;

            alglib.ap.assert(!state.running, "LinCGSetB: you can not set B, because function LinCGIteration is running!");
            alglib.ap.assert(alglib.ap.len(b)>=state.n, "LinCGSetB: Length(B)<N");
            alglib.ap.assert(apserv.isfinitevector(b, state.n, _params), "LinCGSetB: B contains infinite or NaN values!");
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.b[i_] = b[i_];
            }
        }


        /*************************************************************************
        This  function  changes  preconditioning  settings  of  LinCGSolveSparse()
        function. By default, SolveSparse() uses diagonal preconditioner,  but  if
        you want to use solver without preconditioning, you can call this function
        which forces solver to use unit matrix for preconditioning.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state

          -- ALGLIB --
             Copyright 19.11.2012 by Bochkanov Sergey
        *************************************************************************/
        public static void lincgsetprecunit(lincgstate state,
            alglib.xparams _params)
        {
            alglib.ap.assert(!state.running, "LinCGSetPrecUnit: you can not change preconditioner, because function LinCGIteration is running!");
            state.prectype = -1;
        }


        /*************************************************************************
        This  function  changes  preconditioning  settings  of  LinCGSolveSparse()
        function.  LinCGSolveSparse() will use diagonal of the  system  matrix  as
        preconditioner. This preconditioning mode is active by default.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state

          -- ALGLIB --
             Copyright 19.11.2012 by Bochkanov Sergey
        *************************************************************************/
        public static void lincgsetprecdiag(lincgstate state,
            alglib.xparams _params)
        {
            alglib.ap.assert(!state.running, "LinCGSetPrecDiag: you can not change preconditioner, because function LinCGIteration is running!");
            state.prectype = 0;
        }


        /*************************************************************************
        This function sets stopping criteria.

        INPUT PARAMETERS:
            EpsF    -   algorithm will be stopped if norm of residual is less than 
                        EpsF*||b||.
            MaxIts  -   algorithm will be stopped if number of iterations is  more 
                        than MaxIts.

        OUTPUT PARAMETERS:
            State   -   structure which stores algorithm state

        NOTES:
        If  both  EpsF  and  MaxIts  are  zero then small EpsF will be set to small 
        value.

          -- ALGLIB --
             Copyright 14.11.2011 by Bochkanov Sergey
        *************************************************************************/
        public static void lincgsetcond(lincgstate state,
            double epsf,
            int maxits,
            alglib.xparams _params)
        {
            alglib.ap.assert(!state.running, "LinCGSetCond: you can not change stopping criteria when LinCGIteration() is running");
            alglib.ap.assert(math.isfinite(epsf) && (double)(epsf)>=(double)(0), "LinCGSetCond: EpsF is negative or contains infinite or NaN values");
            alglib.ap.assert(maxits>=0, "LinCGSetCond: MaxIts is negative");
            if( (double)(epsf)==(double)(0) && maxits==0 )
            {
                state.epsf = defaultprecision;
                state.maxits = maxits;
            }
            else
            {
                state.epsf = epsf;
                state.maxits = maxits;
            }
        }


        /*************************************************************************
        Reverse communication version of linear CG.

          -- ALGLIB --
             Copyright 14.11.2011 by Bochkanov Sergey
        *************************************************************************/
        public static bool lincgiteration(lincgstate state,
            alglib.xparams _params)
        {
            bool result = new bool();
            int i = 0;
            double uvar = 0;
            double bnorm = 0;
            double v = 0;
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
                i = state.rstate.ia[0];
                uvar = state.rstate.ra[0];
                bnorm = state.rstate.ra[1];
                v = state.rstate.ra[2];
            }
            else
            {
                i = 359;
                uvar = -58;
                bnorm = -919;
                v = -909;
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
            if( state.rstate.stage==4 )
            {
                goto lbl_4;
            }
            if( state.rstate.stage==5 )
            {
                goto lbl_5;
            }
            if( state.rstate.stage==6 )
            {
                goto lbl_6;
            }
            if( state.rstate.stage==7 )
            {
                goto lbl_7;
            }
            
            //
            // Routine body
            //
            alglib.ap.assert(alglib.ap.len(state.b)>0, "LinCGIteration: B is not initialized (you must initialize B by LinCGSetB() call");
            state.running = true;
            state.repnmv = 0;
            clearrfields(state, _params);
            updateitersdata(state, _params);
            
            //
            // Start 0-th iteration
            //
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.rx[i_] = state.startx[i_];
            }
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.x[i_] = state.rx[i_];
            }
            state.repnmv = state.repnmv+1;
            clearrfields(state, _params);
            state.needvmv = true;
            state.rstate.stage = 0;
            goto lbl_rcomm;
        lbl_0:
            state.needvmv = false;
            bnorm = 0;
            state.r2 = 0;
            state.meritfunction = 0;
            for(i=0; i<=state.n-1; i++)
            {
                state.r[i] = state.b[i]-state.mv[i];
                state.r2 = state.r2+state.r[i]*state.r[i];
                state.meritfunction = state.meritfunction+state.mv[i]*state.rx[i]-2*state.b[i]*state.rx[i];
                bnorm = bnorm+state.b[i]*state.b[i];
            }
            bnorm = Math.Sqrt(bnorm);
            
            //
            // Output first report
            //
            if( !state.xrep )
            {
                goto lbl_8;
            }
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.x[i_] = state.rx[i_];
            }
            clearrfields(state, _params);
            state.xupdated = true;
            state.rstate.stage = 1;
            goto lbl_rcomm;
        lbl_1:
            state.xupdated = false;
        lbl_8:
            
            //
            // Is x0 a solution?
            //
            if( !math.isfinite(state.r2) || (double)(Math.Sqrt(state.r2))<=(double)(state.epsf*bnorm) )
            {
                state.running = false;
                if( math.isfinite(state.r2) )
                {
                    state.repterminationtype = 1;
                }
                else
                {
                    state.repterminationtype = -4;
                }
                result = false;
                return result;
            }
            
            //
            // Calculate Z and P
            //
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.x[i_] = state.r[i_];
            }
            state.repnmv = state.repnmv+1;
            clearrfields(state, _params);
            state.needprec = true;
            state.rstate.stage = 2;
            goto lbl_rcomm;
        lbl_2:
            state.needprec = false;
            for(i=0; i<=state.n-1; i++)
            {
                state.z[i] = state.pv[i];
                state.p[i] = state.z[i];
            }
            
            //
            // Other iterations(1..N)
            //
            state.repiterationscount = 0;
        lbl_10:
            if( false )
            {
                goto lbl_11;
            }
            state.repiterationscount = state.repiterationscount+1;
            
            //
            // Calculate Alpha
            //
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.x[i_] = state.p[i_];
            }
            state.repnmv = state.repnmv+1;
            clearrfields(state, _params);
            state.needvmv = true;
            state.rstate.stage = 3;
            goto lbl_rcomm;
        lbl_3:
            state.needvmv = false;
            if( !math.isfinite(state.vmv) || (double)(state.vmv)<=(double)(0) )
            {
                
                //
                // a) Overflow when calculating VMV
                // b) non-positive VMV (non-SPD matrix)
                //
                state.running = false;
                if( math.isfinite(state.vmv) )
                {
                    state.repterminationtype = -5;
                }
                else
                {
                    state.repterminationtype = -4;
                }
                result = false;
                return result;
            }
            state.alpha = 0;
            for(i=0; i<=state.n-1; i++)
            {
                state.alpha = state.alpha+state.r[i]*state.z[i];
            }
            state.alpha = state.alpha/state.vmv;
            if( !math.isfinite(state.alpha) )
            {
                
                //
                // Overflow when calculating Alpha
                //
                state.running = false;
                state.repterminationtype = -4;
                result = false;
                return result;
            }
            
            //
            // Next step toward solution
            //
            for(i=0; i<=state.n-1; i++)
            {
                state.cx[i] = state.rx[i]+state.alpha*state.p[i];
            }
            
            //
            // Calculate R:
            // * use recurrent relation to update R
            // * at every ItsBeforeRUpdate-th iteration recalculate it from scratch, using matrix-vector product
            //   in case R grows instead of decreasing, algorithm is terminated with positive completion code
            //
            if( !(state.itsbeforerupdate==0 || state.repiterationscount%state.itsbeforerupdate!=0) )
            {
                goto lbl_12;
            }
            
            //
            // Calculate R using recurrent formula
            //
            for(i=0; i<=state.n-1; i++)
            {
                state.cr[i] = state.r[i]-state.alpha*state.mv[i];
                state.x[i] = state.cr[i];
            }
            goto lbl_13;
        lbl_12:
            
            //
            // Calculate R using matrix-vector multiplication
            //
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.x[i_] = state.cx[i_];
            }
            state.repnmv = state.repnmv+1;
            clearrfields(state, _params);
            state.needmv = true;
            state.rstate.stage = 4;
            goto lbl_rcomm;
        lbl_4:
            state.needmv = false;
            for(i=0; i<=state.n-1; i++)
            {
                state.cr[i] = state.b[i]-state.mv[i];
                state.x[i] = state.cr[i];
            }
            
            //
            // Calculating merit function
            // Check emergency stopping criterion
            //
            v = 0;
            for(i=0; i<=state.n-1; i++)
            {
                v = v+state.mv[i]*state.cx[i]-2*state.b[i]*state.cx[i];
            }
            if( (double)(v)<(double)(state.meritfunction) )
            {
                goto lbl_14;
            }
            for(i=0; i<=state.n-1; i++)
            {
                if( !math.isfinite(state.rx[i]) )
                {
                    state.running = false;
                    state.repterminationtype = -4;
                    result = false;
                    return result;
                }
            }
            
            //
            //output last report
            //
            if( !state.xrep )
            {
                goto lbl_16;
            }
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.x[i_] = state.rx[i_];
            }
            clearrfields(state, _params);
            state.xupdated = true;
            state.rstate.stage = 5;
            goto lbl_rcomm;
        lbl_5:
            state.xupdated = false;
        lbl_16:
            state.running = false;
            state.repterminationtype = 7;
            result = false;
            return result;
        lbl_14:
            state.meritfunction = v;
        lbl_13:
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.rx[i_] = state.cx[i_];
            }
            
            //
            // calculating RNorm
            //
            // NOTE: monotonic decrease of R2 is not guaranteed by algorithm.
            //
            state.r2 = 0;
            for(i=0; i<=state.n-1; i++)
            {
                state.r2 = state.r2+state.cr[i]*state.cr[i];
            }
            
            //
            //output report
            //
            if( !state.xrep )
            {
                goto lbl_18;
            }
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.x[i_] = state.rx[i_];
            }
            clearrfields(state, _params);
            state.xupdated = true;
            state.rstate.stage = 6;
            goto lbl_rcomm;
        lbl_6:
            state.xupdated = false;
        lbl_18:
            
            //
            //stopping criterion
            //achieved the required precision
            //
            if( !math.isfinite(state.r2) || (double)(Math.Sqrt(state.r2))<=(double)(state.epsf*bnorm) )
            {
                state.running = false;
                if( math.isfinite(state.r2) )
                {
                    state.repterminationtype = 1;
                }
                else
                {
                    state.repterminationtype = -4;
                }
                result = false;
                return result;
            }
            if( state.repiterationscount>=state.maxits && state.maxits>0 )
            {
                for(i=0; i<=state.n-1; i++)
                {
                    if( !math.isfinite(state.rx[i]) )
                    {
                        state.running = false;
                        state.repterminationtype = -4;
                        result = false;
                        return result;
                    }
                }
                
                //
                //if X is finite number
                //
                state.running = false;
                state.repterminationtype = 5;
                result = false;
                return result;
            }
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.x[i_] = state.cr[i_];
            }
            
            //
            //prepere of parameters for next iteration
            //
            state.repnmv = state.repnmv+1;
            clearrfields(state, _params);
            state.needprec = true;
            state.rstate.stage = 7;
            goto lbl_rcomm;
        lbl_7:
            state.needprec = false;
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.cz[i_] = state.pv[i_];
            }
            if( state.repiterationscount%state.itsbeforerestart!=0 )
            {
                state.beta = 0;
                uvar = 0;
                for(i=0; i<=state.n-1; i++)
                {
                    state.beta = state.beta+state.cz[i]*state.cr[i];
                    uvar = uvar+state.z[i]*state.r[i];
                }
                
                //
                //check that UVar is't INF or is't zero
                //
                if( !math.isfinite(uvar) || (double)(uvar)==(double)(0) )
                {
                    state.running = false;
                    state.repterminationtype = -4;
                    result = false;
                    return result;
                }
                
                //
                //calculate .BETA
                //
                state.beta = state.beta/uvar;
                
                //
                //check that .BETA neither INF nor NaN
                //
                if( !math.isfinite(state.beta) )
                {
                    state.running = false;
                    state.repterminationtype = -1;
                    result = false;
                    return result;
                }
                for(i=0; i<=state.n-1; i++)
                {
                    state.p[i] = state.cz[i]+state.beta*state.p[i];
                }
            }
            else
            {
                for(i_=0; i_<=state.n-1;i_++)
                {
                    state.p[i_] = state.cz[i_];
                }
            }
            
            //
            //prepere data for next iteration
            //
            for(i=0; i<=state.n-1; i++)
            {
                
                //
                //write (k+1)th iteration to (k )th iteration
                //
                state.r[i] = state.cr[i];
                state.z[i] = state.cz[i];
            }
            goto lbl_10;
        lbl_11:
            result = false;
            return result;
            
            //
            // Saving state
            //
        lbl_rcomm:
            result = true;
            state.rstate.ia[0] = i;
            state.rstate.ra[0] = uvar;
            state.rstate.ra[1] = bnorm;
            state.rstate.ra[2] = v;
            return result;
        }


        /*************************************************************************
        Procedure for solution of A*x=b with sparse A.

        INPUT PARAMETERS:
            State   -   algorithm state
            A       -   sparse matrix in the CRS format (you MUST contvert  it  to 
                        CRS format by calling SparseConvertToCRS() function).
            IsUpper -   whether upper or lower triangle of A is used:
                        * IsUpper=True  => only upper triangle is used and lower
                                           triangle is not referenced at all 
                        * IsUpper=False => only lower triangle is used and upper
                                           triangle is not referenced at all
            B       -   right part, array[N]

        RESULT:
            This function returns no result.
            You can get solution by calling LinCGResults()
            
        NOTE: this function uses lightweight preconditioning -  multiplication  by
              inverse of diag(A). If you want, you can turn preconditioning off by
              calling LinCGSetPrecUnit(). However, preconditioning cost is low and
              preconditioner  is  very  important  for  solution  of  badly scaled
              problems.

          -- ALGLIB --
             Copyright 14.11.2011 by Bochkanov Sergey
        *************************************************************************/
        public static void lincgsolvesparse(lincgstate state,
            sparse.sparsematrix a,
            bool isupper,
            double[] b,
            alglib.xparams _params)
        {
            int n = 0;
            int i = 0;
            double v = 0;
            double vmv = 0;
            int i_ = 0;

            n = state.n;
            alglib.ap.assert(alglib.ap.len(b)>=state.n, "LinCGSetB: Length(B)<N");
            alglib.ap.assert(apserv.isfinitevector(b, state.n, _params), "LinCGSetB: B contains infinite or NaN values!");
            
            //
            // Allocate temporaries
            //
            apserv.rvectorsetlengthatleast(ref state.tmpd, n, _params);
            
            //
            // Compute diagonal scaling matrix D
            //
            if( state.prectype==0 )
            {
                
                //
                // Default preconditioner - inverse of matrix diagonal
                //
                for(i=0; i<=n-1; i++)
                {
                    v = sparse.sparsegetdiagonal(a, i, _params);
                    if( (double)(v)>(double)(0) )
                    {
                        state.tmpd[i] = 1/Math.Sqrt(v);
                    }
                    else
                    {
                        state.tmpd[i] = 1;
                    }
                }
            }
            else
            {
                
                //
                // No diagonal scaling
                //
                for(i=0; i<=n-1; i++)
                {
                    state.tmpd[i] = 1;
                }
            }
            
            //
            // Solve
            //
            lincgrestart(state, _params);
            lincgsetb(state, b, _params);
            while( lincgiteration(state, _params) )
            {
                
                //
                // Process different requests from optimizer
                //
                if( state.needmv )
                {
                    sparse.sparsesmv(a, isupper, state.x, ref state.mv, _params);
                }
                if( state.needvmv )
                {
                    sparse.sparsesmv(a, isupper, state.x, ref state.mv, _params);
                    vmv = 0.0;
                    for(i_=0; i_<=state.n-1;i_++)
                    {
                        vmv += state.x[i_]*state.mv[i_];
                    }
                    state.vmv = vmv;
                }
                if( state.needprec )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        state.pv[i] = state.x[i]*math.sqr(state.tmpd[i]);
                    }
                }
            }
        }


        /*************************************************************************
        CG-solver: results.

        This function must be called after LinCGSolve

        INPUT PARAMETERS:
            State   -   algorithm state

        OUTPUT PARAMETERS:
            X       -   array[N], solution
            Rep     -   optimization report:
                        * Rep.TerminationType completetion code:
                            * -5    input matrix is either not positive definite,
                                    too large or too small                            
                            * -4    overflow/underflow during solution
                                    (ill conditioned problem)
                            *  1    ||residual||<=EpsF*||b||
                            *  5    MaxIts steps was taken
                            *  7    rounding errors prevent further progress,
                                    best point found is returned
                        * Rep.IterationsCount contains iterations count
                        * NMV countains number of matrix-vector calculations

          -- ALGLIB --
             Copyright 14.11.2011 by Bochkanov Sergey
        *************************************************************************/
        public static void lincgresults(lincgstate state,
            ref double[] x,
            lincgreport rep,
            alglib.xparams _params)
        {
            int i_ = 0;

            x = new double[0];

            alglib.ap.assert(!state.running, "LinCGResult: you can not get result, because function LinCGIteration has been launched!");
            if( alglib.ap.len(x)<state.n )
            {
                x = new double[state.n];
            }
            for(i_=0; i_<=state.n-1;i_++)
            {
                x[i_] = state.rx[i_];
            }
            rep.iterationscount = state.repiterationscount;
            rep.nmv = state.repnmv;
            rep.terminationtype = state.repterminationtype;
            rep.r2 = state.r2;
        }


        /*************************************************************************
        This function sets restart frequency. By default, algorithm  is  restarted
        after N subsequent iterations.

          -- ALGLIB --
             Copyright 14.11.2011 by Bochkanov Sergey
        *************************************************************************/
        public static void lincgsetrestartfreq(lincgstate state,
            int srf,
            alglib.xparams _params)
        {
            alglib.ap.assert(!state.running, "LinCGSetRestartFreq: you can not change restart frequency when LinCGIteration() is running");
            alglib.ap.assert(srf>0, "LinCGSetRestartFreq: non-positive SRF");
            state.itsbeforerestart = srf;
        }


        /*************************************************************************
        This function sets frequency of residual recalculations.

        Algorithm updates residual r_k using iterative formula,  but  recalculates
        it from scratch after each 10 iterations. It is done to avoid accumulation
        of numerical errors and to stop algorithm when r_k starts to grow.

        Such low update frequence (1/10) gives very  little  overhead,  but  makes
        algorithm a bit more robust against numerical errors. However, you may
        change it 

        INPUT PARAMETERS:
            Freq    -   desired update frequency, Freq>=0.
                        Zero value means that no updates will be done.

          -- ALGLIB --
             Copyright 14.11.2011 by Bochkanov Sergey
        *************************************************************************/
        public static void lincgsetrupdatefreq(lincgstate state,
            int freq,
            alglib.xparams _params)
        {
            alglib.ap.assert(!state.running, "LinCGSetRUpdateFreq: you can not change update frequency when LinCGIteration() is running");
            alglib.ap.assert(freq>=0, "LinCGSetRUpdateFreq: non-positive Freq");
            state.itsbeforerupdate = freq;
        }


        /*************************************************************************
        This function turns on/off reporting.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state
            NeedXRep-   whether iteration reports are needed or not

        If NeedXRep is True, algorithm will call rep() callback function if  it is
        provided to MinCGOptimize().

          -- ALGLIB --
             Copyright 14.11.2011 by Bochkanov Sergey
        *************************************************************************/
        public static void lincgsetxrep(lincgstate state,
            bool needxrep,
            alglib.xparams _params)
        {
            state.xrep = needxrep;
        }


        /*************************************************************************
        Procedure for restart function LinCGIteration

          -- ALGLIB --
             Copyright 14.11.2011 by Bochkanov Sergey
        *************************************************************************/
        public static void lincgrestart(lincgstate state,
            alglib.xparams _params)
        {
            state.rstate.ia = new int[0+1];
            state.rstate.ra = new double[2+1];
            state.rstate.stage = -1;
            clearrfields(state, _params);
        }


        /*************************************************************************
        Clears request fileds (to be sure that we don't forgot to clear something)
        *************************************************************************/
        private static void clearrfields(lincgstate state,
            alglib.xparams _params)
        {
            state.xupdated = false;
            state.needmv = false;
            state.needmtv = false;
            state.needmv2 = false;
            state.needvmv = false;
            state.needprec = false;
        }


        /*************************************************************************
        Clears request fileds (to be sure that we don't forgot to clear something)
        *************************************************************************/
        private static void updateitersdata(lincgstate state,
            alglib.xparams _params)
        {
            state.repiterationscount = 0;
            state.repnmv = 0;
            state.repterminationtype = 0;
        }


    }
}

