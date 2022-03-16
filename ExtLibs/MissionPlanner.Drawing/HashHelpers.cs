namespace System.Numerics.Hashing
{
    internal static class HashHelpers
    {
        public static int Combine(int h1, int h2)
        {
            return (((h1 << 5) | (int) ((uint) h1 >> 27)) + h1) ^ h2;
        }
    }
}