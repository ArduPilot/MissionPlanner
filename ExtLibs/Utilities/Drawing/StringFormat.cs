using System;

namespace MissionPlanner.Utilities.Drawing
{
    public class StringFormat:IDisposable
    {
        public static StringFormat GenericDefault { get; set; } = new StringFormat();
        public StringAlignment LineAlignment { get; set; } = StringAlignment.Near;
        public StringAlignment Alignment { get; set; } = StringAlignment.Near;
        public static StringFormat GenericTypographic { get; set; } = new StringFormat();

        public void Dispose()
        {
           
        }
    }
}