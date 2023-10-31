using log4net;
using MissionPlanner.ArduPilot;
using MissionPlanner.Properties;
using MissionPlanner.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using static MAVLink;

namespace MissionPlanner.Controls
{
    public class ProximityControl : Form
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        MAVState _parent;
        private Proximity.directionState _dS => _parent?.Proximity?.DirectionState;
        private bool _drawingInProgress => _parent?.Proximity?.DrawingInProgress ?? false;

        private Timer timer1;
        private IContainer components;

        public bool DataAvailable { get; set; } = false;

        public ProximityControl(MAVState state)
        {
            InitializeComponent();

            this.DoubleBuffered = true;

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
            if (_parent.Proximity != null)
                _parent.Proximity.DrawingInProgress = true;
            e.Graphics.Clear(BackColor);

            var midx = e.ClipRectangle.Width / 2.0f;
            var midy = e.ClipRectangle.Height / 2.0f;

            //e.Graphics.DrawArc(System.Drawing.Pens.Green, midx - 10, midy - 10, 20, 20, 0, 360);

            Text = "Radius(+/-): " + (screenradius / 100.0) + "m MAV size([/]): " + (mavsize / 100.0) + "m";

            // 11m radius = 22 m coverage
            var scale = ((screenradius + 50) * 2) / Math.Min(Height, Width);
            // 80cm quad / scale
            var size = mavsize / scale;

            switch (_parent.cs.firmware)
            {
                case Firmwares.ArduCopter2:
                    var imw = size / 2;
                    e.Graphics.DrawImage(Resources.quadicon, midx - imw, midy - imw, size, size);
                    break;
            }

            if (_dS == null)
            {
                if (_parent.Proximity != null)
                    _parent.Proximity.DrawingInProgress = false;
                return;
            }

            Pen redpen = new Pen(Color.Red, 3);
            Pen yellowpen = new Pen(Color.Yellow, 3);
            var font = new Font(SystemFonts.DefaultFont.FontFamily, SystemFonts.DefaultFont.Size + 2, FontStyle.Bold);

            float move = 5;

            for (float x = 50f; x <= screenradius; x+=50f)
            {
                Vector3 location = new Vector3(0, (x) / scale, 0);
                var doublelength = location.length() * 2.0f;
                var length = location.length();

                e.Graphics.DrawArc(Pens.DimGray, (float) (midx - length), (float) (midy - length),
                    (float) doublelength, (float) doublelength, 0f,
                    (float) 360);
                e.Graphics.DrawString((x / 100).ToString("0.0m"), font, System.Drawing.Brushes.Green, midx - (float)location.x + move, midy - (float)location.y);
            }

            for (float x = 0; x < 360; x+=45f)
            {
                Vector3 location = new Vector3(0, screenradius / scale, 0);
                var doublelength = location.length() * 2.0f;
                var length = location.length();

                location.rotate(x);
                e.Graphics.DrawString((x).ToString("0"), font, System.Drawing.Brushes.DimGray, midx - (float)location.x - move * 2, midy - (float)location.y + move);
                e.Graphics.DrawLine(Pens.DimGray, (float) (midx), (float) (midy),  midx-(float)location.X,
                     midy-(float)location.Y);
            }

            var rawdata = _dS.GetRaw();
            foreach (var temp in rawdata.ToList())
            {
                Vector3 location = new Vector3(0, Math.Min(temp.Distance / scale, (screenradius) / scale), 0);

                var halflength = location.length() / 2.0f;
                var doublelength = location.length() * 2.0f;
                var length = location.length();

                switch (temp.Orientation)
                {
                    case MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_NONE:
                        location.rotate(Rotation.ROTATION_NONE);
                        e.Graphics.DrawString((temp.Distance / 100).ToString("0.0m"), font, System.Drawing.Brushes.Green, midx - (float)location.x - move * 2, midy - (float)location.y + move);
                        e.Graphics.DrawArc(redpen, (float)(midx - length), (float)(midy - length), (float)doublelength, (float)doublelength, -22.5f - 90f, 45f);
                        break;
                    case MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_YAW_45:
                        location.rotate(Rotation.ROTATION_YAW_45);
                        e.Graphics.DrawString((temp.Distance / 100).ToString("0.0m"), font, System.Drawing.Brushes.Green, midx - (float)location.x - move * 8, midy - (float)location.y + move);
                        e.Graphics.DrawArc(redpen, (float)(midx - length), (float)(midy - length), (float)doublelength, (float)doublelength, 22.5f - 90f, 45f);
                        break;
                    case MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_YAW_90:
                        location.rotate(Rotation.ROTATION_YAW_90);
                        e.Graphics.DrawString((temp.Distance / 100).ToString("0.0m"), font, System.Drawing.Brushes.Green, midx - (float)location.x - move * 8, midy - (float)location.y);
                        e.Graphics.DrawArc(redpen, (float)(midx - length), (float)(midy - length), (float)doublelength, (float)doublelength, 67.5f - 90f, 45f);
                        break;
                    case MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_YAW_135:
                        location.rotate(Rotation.ROTATION_YAW_135);
                        e.Graphics.DrawString((temp.Distance / 100).ToString("0.0m"), font, System.Drawing.Brushes.Green, midx - (float)location.x - move * 8, midy - (float)location.y - move);
                        e.Graphics.DrawArc(redpen, (float)(midx - length), (float)(midy - length), (float)doublelength, (float)doublelength, 112.5f - 90f, 45f);
                        break;
                    case MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_YAW_180:
                        location.rotate(Rotation.ROTATION_YAW_180);
                        e.Graphics.DrawString((temp.Distance / 100).ToString("0.0m"), font, System.Drawing.Brushes.Green, midx - (float)location.x - move * 2, midy - (float)location.y - move * 3);
                        e.Graphics.DrawArc(redpen, (float)(midx - length), (float)(midy - length), (float)doublelength, (float)doublelength, 157.5f - 90f, 45f);
                        break;
                    case MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_YAW_225:
                        location.rotate(Rotation.ROTATION_YAW_225);
                        e.Graphics.DrawString((temp.Distance / 100).ToString("0.0m"), font, System.Drawing.Brushes.Green, midx - (float)location.x + move, midy - (float)location.y - move);
                        e.Graphics.DrawArc(redpen, (float)(midx - length), (float)(midy - length), (float)doublelength, (float)doublelength, 202.5f - 90f, 45f);
                        break;
                    case MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_YAW_270:
                        location.rotate(Rotation.ROTATION_YAW_270);
                        e.Graphics.DrawString((temp.Distance / 100).ToString("0.0m"), font, System.Drawing.Brushes.Green, midx - (float)location.x + move, midy - (float)location.y);
                        e.Graphics.DrawArc(redpen, (float)(midx - length), (float)(midy - length), (float)doublelength, (float)doublelength, 247.5f - 90f, 45f);
                        break;
                    case MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_YAW_315:
                        location.rotate(Rotation.ROTATION_YAW_315);
                        e.Graphics.DrawString((temp.Distance / 100).ToString("0.0m"), font, System.Drawing.Brushes.Green, midx - (float)location.x + move, midy - (float)location.y);
                        e.Graphics.DrawArc(redpen, (float)(midx - length), (float)(midy - length), (float)doublelength, (float)doublelength, 292.5f - 90f, 45f);
                        break;
                    case MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_CUSTOM:
                        location.rotate(temp.Angle);
                        //e.Graphics.DrawString((temp.Distance / 100).ToString("0.0m"), font, System.Drawing.Brushes.Green, midx - (float)location.x + move, midy - (float)location.y);
                        e.Graphics.DrawArc(yellowpen, (float) (midx - length), (float) (midy - length),
                            (float) doublelength, (float) doublelength, (float)temp.Angle - ((float)temp.Size / 2.0f) - 90f,
                            (float)temp.Size);
                        break;
                }
            }
            _parent.Proximity.DrawingInProgress = false;
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
                _parent.Proximity = new Proximity(_parent, (byte)MainV2.comPort.sysidcurrent,
                    (byte)MainV2.comPort.compidcurrent);
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