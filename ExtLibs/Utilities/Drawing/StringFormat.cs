using System;

namespace MissionPlanner.Utilities.Drawing
{
    public class StringFormat:IDisposable,ICloneable
    {
        public CharacterRange[] ranges;
        public StringFormat(){}
        public StringFormat(StringFormat genericTypographic)
        {
            
        }

        public static StringFormat GenericDefault { get; set; } = new StringFormat();
        public StringAlignment LineAlignment { get; set; } = StringAlignment.Near;
        public StringAlignment Alignment { get; set; } = StringAlignment.Near;
        public static StringFormat GenericTypographic { get; set; } = new StringFormat();
        public StringTrimming Trimming { get; set; }
        public HotkeyPrefix HotkeyPrefix { get; set; }
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
            return new StringFormat();
        }
    }
}