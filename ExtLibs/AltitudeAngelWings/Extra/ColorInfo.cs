using System;

namespace AltitudeAngelWings.Extra
{
    [Serializable]
    public class ColorInfo
    {
        public uint FillColor { get; set; } = 0x77FF0000;
        public uint StrokeColor { get; set; } = 0xFFFFFFFF;
        public int StrokeWidth { get; set; } = 1;
    }
}
