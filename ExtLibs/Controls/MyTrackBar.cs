using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public class MyTrackBar: TrackBar
    {
        [System.ComponentModel.Browsable(true)]
        public new float Maximum { get { return base.Maximum / 1000.0f; } set { base.Maximum = (int)(float)(value * 1000.0f); } }
        [System.ComponentModel.Browsable(true)]
        public new float Minimum { get { return base.Minimum / 1000.0f; } set { base.Minimum = (int)(float)(value * 1000.0f); } }
        [System.ComponentModel.Browsable(true)]
        public new float Value { get { return base.Value / 1000.0f; } set { base.Value = (int)(float)(value * 1000.0f); } }
        [System.ComponentModel.Browsable(true)]
        public new float TickFrequency { get { return base.TickFrequency / 1000.0f; } set { base.TickFrequency = (int)(float)(value * 1000.0f); } }
        [System.ComponentModel.Browsable(true)]
        public new float SmallChange { get { return base.SmallChange / 1000.0f; } set { base.SmallChange = (int)(float)(value * 1000.0f); } }
        [System.ComponentModel.Browsable(true)]
        public new float LargeChange { get { return base.LargeChange / 1000.0f; } set { base.LargeChange = (int)(float)(value * 1000.0f); } }

    }
}
