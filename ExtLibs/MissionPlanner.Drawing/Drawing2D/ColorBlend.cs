namespace System.Drawing.Drawing2D
{
    public sealed class ColorBlend
    {
        public Color[] Colors { get; set; }

        public float[] Positions { get; set; }

        public ColorBlend()
        {
            Colors = new Color[1];
            Positions = new float[1];
        }

        public ColorBlend(int count)
        {
            Colors = new Color[count];
            Positions = new float[count];
        }
    }
}