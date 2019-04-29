using System;
using MissionPlanner.Utilities.Drawing;

namespace Xamarin.Controls
{
    public class MouseEventArgs : EventArgs
    {
        public int X { get; set; }
        public int Y { get; set; }
        public MouseButtons Button { get; set; }
        public int Delta { get; internal set; }
    }
}