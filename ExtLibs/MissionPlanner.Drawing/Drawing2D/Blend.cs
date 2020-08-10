namespace System.Drawing.Drawing2D
{
    public sealed class Blend
    {
        public float[] Factors { get; set; }

        public float[] Positions { get; set; }

        public Blend()
        {
            Factors = new float[1];
            Positions = new float[1];
        }

        public Blend(int count)
        {
            Factors = new float[count];
            Positions = new float[count];
        }
    }
}