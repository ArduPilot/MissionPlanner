using log4net;
using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using MissionPlanner.ArduPilot;
using static MAVLink;
using MissionPlanner.Properties;
using System.ComponentModel;

namespace MissionPlanner.Utilities
{
    public class ProximityControl : Form
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        MAVState _parent;
        private Proximity.directionState _dS => _parent.Proximity.DirectionState;

        private Timer timer1;
        private IContainer components;

        public bool DataAvailable { get; set; } = false;

        public ProximityControl(MAVState state)
        {
            InitializeComponent();

            _parent = state;

            Paint += Temp_Paint;
            KeyPress += Temp_KeyPress;
            Resize += Temp_Resize;
            FormClosing += ProximityControl_FormClosing; ;
            
            timer1.Interval = 100;
            timer1.Tick += (s, e) => { Invalidate(); };

            timer1.Start();
        }

        private void ProximityControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
        }

        private void Temp_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void Temp_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '+':
                    screenradius -= 50;
                    e.Handled = true;
                    break;
                case '-':
                    screenradius += 50;
                    e.Handled = true;
                    break;
                case ']':
                    mavsize++;
                    e.Handled = true;
                    break;
                case '[':
                    mavsize--;
                    e.Handled = true;
                    break;
            }

            // prevent 0's
            if (screenradius < 1)
                screenradius = 50;
            if (mavsize < 1)
                mavsize = 1;

            Invalidate();
        }

        //cm's
        public float screenradius = 500;
        public float mavsize = 80;

        private void Temp_Paint(object sender, PaintEventArgs e)
        {
            var rawdata = _dS.GetRaw();

            e.Graphics.Clear(BackColor);

            var midx = e.ClipRectangle.Width / 2.0f;
            var midy = e.ClipRectangle.Height / 2.0f;

            //e.Graphics.DrawArc(System.Drawing.Pens.Green, midx - 10, midy - 10, 20, 20, 0, 360);

            Text = "Radius(+/-): " + (screenradius / 100.0) + "m MAV size([/]): " + (mavsize / 100.0) + "m";

            // 11m radius = 22 m coverage
            var scale = ((screenradius+50) * 2) / Math.Min(Height,Width);
            // 80cm quad / scale
            var size = mavsize / scale;

            switch(_parent.cs.firmware)
            {
                case Firmwares.ArduCopter2:
                    var imw = size/2;
                    e.Graphics.DrawImage(Resources.quadicon, midx - imw, midy - imw, size, size);
                    break;
            }

            foreach (var temp in rawdata.ToList())
            {
                Vector3 location = new Vector3(0, Math.Min(temp.Distance / scale, (screenradius) / scale), 0);

                var halflength = location.length() / 2.0f;
                var doublelength = location.length() * 2.0f;
                var length = location.length();

                Pen redpen = new Pen(Color.Red, 3);
                float move = 5;
                var font = new Font(SystemFonts.DefaultFont.FontFamily, SystemFonts.DefaultFont.Size+2, FontStyle.Bold);

                switch (temp.Orientation)
                {
                    case MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_NONE:
                        location.rotate(Rotation.ROTATION_NONE);
                        e.Graphics.DrawString((temp.Distance/100).ToString("0.0m"), font, System.Drawing.Brushes.Green, midx - (float)location.x-move*2, midy - (float)location.y + move);
                        e.Graphics.DrawArc(redpen, (float)(midx - length), (float)(midy - length), (float)doublelength, (float)doublelength, -22.5f - 90f, 45f);
                        break;
                    case MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_YAW_45:
                        location.rotate(Rotation.ROTATION_YAW_45);
                        e.Graphics.DrawString((temp.Distance/100).ToString("0.0m"), font, System.Drawing.Brushes.Green, midx - (float)location.x- move*8, midy - (float)location.y+ move);
                        e.Graphics.DrawArc(redpen, (float)(midx - length), (float)(midy - length), (float)doublelength, (float)doublelength, 22.5f - 90f, 45f);
                        break;
                    case MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_YAW_90:
                        location.rotate(Rotation.ROTATION_YAW_90);
                        e.Graphics.DrawString((temp.Distance/100).ToString("0.0m"), font, System.Drawing.Brushes.Green, midx - (float)location.x- move*8, midy - (float)location.y);
                        e.Graphics.DrawArc(redpen, (float)(midx - length), (float)(midy - length), (float)doublelength, (float)doublelength, 67.5f - 90f, 45f);
                        break;
                    case MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_YAW_135:
                        location.rotate(Rotation.ROTATION_YAW_135);
                        e.Graphics.DrawString((temp.Distance/100).ToString("0.0m"), font, System.Drawing.Brushes.Green, midx - (float)location.x- move*8, midy - (float)location.y- move);
                        e.Graphics.DrawArc(redpen, (float)(midx - length), (float)(midy - length), (float)doublelength, (float)doublelength, 112.5f - 90f, 45f);
                        break;
                    case MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_YAW_180:
                        location.rotate(Rotation.ROTATION_YAW_180);
                        e.Graphics.DrawString((temp.Distance/100).ToString("0.0m"), font, System.Drawing.Brushes.Green, midx - (float)location.x-move*2, midy - (float)location.y-move*3);
                        e.Graphics.DrawArc(redpen, (float)(midx - length), (float)(midy - length), (float)doublelength, (float)doublelength, 157.5f - 90f, 45f);
                        break;
                    case MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_YAW_225:
                        location.rotate(Rotation.ROTATION_YAW_225);
                        e.Graphics.DrawString((temp.Distance/100).ToString("0.0m"), font, System.Drawing.Brushes.Green, midx - (float)location.x+ move, midy - (float)location.y- move);
                        e.Graphics.DrawArc(redpen, (float)(midx - length), (float)(midy - length), (float)doublelength, (float)doublelength, 202.5f - 90f, 45f);
                        break;
                    case MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_YAW_270:
                        location.rotate(Rotation.ROTATION_YAW_270);
                        e.Graphics.DrawString((temp.Distance/100).ToString("0.0m"), font, System.Drawing.Brushes.Green, midx - (float)location.x+move, midy - (float)location.y);
                        e.Graphics.DrawArc(redpen, (float)(midx - length), (float)(midy - length), (float)doublelength, (float)doublelength, 247.5f - 90f, 45f);
                        break;
                    case MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_YAW_315:
                        location.rotate(Rotation.ROTATION_YAW_315);
                        e.Graphics.DrawString((temp.Distance/100).ToString("0.0m"), font, System.Drawing.Brushes.Green, midx - (float)location.x+move, midy - (float)location.y);
                        e.Graphics.DrawArc(redpen, (float)(midx - length), (float)(midy - length), (float)doublelength, (float)doublelength, 292.5f - 90f, 45f);
                        break;
                }
            }
        }

        public new void Show()
        {
            if (!IsDisposed)
            {
                if (Visible)
                    return;

                base.Show();
            }
            else
            {
                Dispose();
                _parent.Proximity = new Proximity(_parent);
                base.Show();
            }
        }

        public new void Dispose()
        {
            timer1.Stop();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // ProximityControl
            // 
            this.ClientSize = new System.Drawing.Size(430, 391);
            this.Name = "ProximityControl";
            this.ResumeLayout(false);

        }
    }
}