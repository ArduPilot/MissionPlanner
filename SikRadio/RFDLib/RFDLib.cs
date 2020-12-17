using System;


namespace RFDLib
{

    public static class Utils
    {

        public static bool Retry<T>(Func<T> ToTry, Func<T, bool> PassedCondition, int QTYTries)
        {
            int TryCount;

            for (TryCount = 0; TryCount < QTYTries; TryCount++)
            {
                if (PassedCondition(ToTry()))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool Retry(Func<bool> PassedCondition, int QTYTries)
        {
            return Retry(() => (int)1, (x) => PassedCondition(), QTYTries);
        }

        public static System.Collections.Generic.KeyValuePair<string, int>[] EnumToStrings(Type EnumType)
        {
            var Values = Enum.GetValues(EnumType);
            System.Collections.Generic.KeyValuePair<string, int>[] Result =
                new System.Collections.Generic.KeyValuePair<string, int>[Values.Length];

            for (int n = 0; n < Values.Length; n++)
            {
                Result[n] = new System.Collections.Generic.KeyValuePair<string, int>(
                    Enum.GetName(EnumType, Values.GetValue(n)), (int)Values.GetValue(n));

            }

            return Result;
        }

    }


}