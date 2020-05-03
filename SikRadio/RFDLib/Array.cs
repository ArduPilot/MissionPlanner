using System;


namespace RFDLib
{
    public static class Array
    {
        public static T[] CherryPickArray<U, T>(U[] x, Func<U, T> PickFn)
        {
            T[] Result = new T[x.Length];
            for (int n = 0; n < x.Length; n++)
            {
                Result[n] = PickFn(x[n]);
            }
            return Result;
        }
    }
}
