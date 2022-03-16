using System;

namespace System.Drawing
{
    public class StringFormat : IDisposable, ICloneable
    {
        public CharacterRange[] ranges;
        private HotkeyPrefix _hotkeyPrefix;

        public StringFormat()
        {
        }

        public StringFormat(StringFormat genericTypographic)
        {
        }

        public StringFormat(StringFormatFlags flags)
        {
            FormatFlags = flags;
        }

        public static StringFormat GenericDefault {
            get { return new StringFormat(); }
        }
        public StringAlignment LineAlignment { get; set; } = StringAlignment.Near;
        public StringAlignment Alignment { get; set; } = StringAlignment.Near;
        public static StringFormat GenericTypographic {
            get { return new StringFormat(); }
        }
        public StringTrimming Trimming { get; set; }

        public HotkeyPrefix HotkeyPrefix
        {
            get => _hotkeyPrefix;
            set => _hotkeyPrefix = value;
        }

        public StringFormatFlags FormatFlags { get; set; }

        public void Dispose()
        {
        }

        public void SetTabStops(float f, float[] floats)
        {
            //throw new NotImplementedException();
        }

        public void SetMeasurableCharacterRanges(CharacterRange[] ranges)
        {
            this.ranges = ranges;
        }

        public object Clone()
        {
            return new StringFormat()
                {LineAlignment = this.LineAlignment, Alignment = this.Alignment, FormatFlags = this.FormatFlags};
        }
    }
}