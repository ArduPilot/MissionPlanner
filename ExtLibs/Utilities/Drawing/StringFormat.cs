using System;

namespace MissionPlanner.Utilities.Drawing
{
    public class StringFormat:IDisposable
    {
        public static StringFormat GenericDefault { get; set; }
        public StringAlignment LineAlignment { get; set; }
        public StringAlignment Alignment { get; set; }

        public void Dispose()
        {
           
        }
    }
}