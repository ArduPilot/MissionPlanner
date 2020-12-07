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

    }


}