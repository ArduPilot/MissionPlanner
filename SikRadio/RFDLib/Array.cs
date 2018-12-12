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

        /// <summary>
        /// Copy the given array, removing elements which meet the given condition.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x"></param>
        /// <param name="Condition"></param>
        /// <returns></returns>
        public static T[] ConditionalRemove<T>(T[] x, Func<T, bool> Condition)
        {
            T[] Temp = new T[x.Length];
            int Length = 0;
            foreach (var a in x)
            {
                if (!Condition(a))
                {
                    Temp[Length++] = a;
                }
            }

            T[] Result = new T[Length];
            System.Array.Copy(Temp, Result, Length);
            return Result;
        }
    }
}
