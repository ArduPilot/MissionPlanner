using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoUtility.GeoSystem;

namespace MissionPlanner.Controls
{
    public partial class Coords : MyUserControl
    {
        [System.ComponentModel.Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public string System
        {
            get { return CMB_coordsystem.Text; }
            set { CMB_coordsystem.Text = value; }
        }

        public event EventHandler SystemChanged;

        public double Lat
        {
            get { return point.Latitude; }
            set
            {
                if (point.Latitude == value) return;
                point.Latitude = value;
                this.Invalidate();
            }
        }

        public double Lng
        {
            get { return point.Longitude; }
            set
            {
                if (point.Longitude == value) return;
                point.Longitude = value;
                this.Invalidate();
            }
        }

        public double Alt
        {
            get { return _alt; }
            set
            {
                if (_alt == value) return;
                _alt = value;
                this.Invalidate();
            }
        }

        public string AltUnit
        {
            get { return _unit; }
            set
            {
                if (_unit == value) return;
                _unit = value;
                this.Invalidate();
            }
        }

        public string AltSource { get; set; }

        [System.ComponentModel.Browsable(true)]
        public bool Vertical { get; set; }

        double _alt = 0;
        private string _unit = "m";
        Geographic point = new Geographic();

        public enum CoordsSystems
        {
            GEO,
            UTM,
            MGRS
        }

        public Coords()
        {
            Vertical = false;

            InitializeComponent();
            this.DoubleBuffered = true;
            CMB_coordsystem.DataSource = Enum.GetNames(typeof(CoordsSystems));
            AltSource = "";
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            PointF text = new PointF(CMB_coordsystem.Right + 3, 3);

            if (System == CoordsSystems.GEO.ToString())
            {
                if (Vertical)
                {
                    e.Graphics.DrawString(Lat.ToString("0.0000000") + "\n" + Lng.ToString("0.0000000") + "\n" + Alt.ToString("0.00") + AltUnit, this.Font, new SolidBrush(this.ForeColor), text, StringFormat.GenericDefault);
                    e.Graphics.DrawString(AltSource, this.Font, new SolidBrush(this.ForeColor),
                        new PointF(CMB_coordsystem.Left, CMB_coordsystem.Bottom + 4), StringFormat.GenericDefault);
                }
                else
                {
                    e.Graphics.DrawString(Lat.ToString("0.0000000") + " " + Lng.ToString("0.0000000") + "   " + Alt.ToString("0.00") + AltUnit, this.Font, new SolidBrush(this.ForeColor), text, StringFormat.GenericDefault);
                }
            } 
            else  if (System == CoordsSystems.UTM.ToString())
            {
                try
                {
                    if (point.Latitude > 84 || point.Latitude < -80 || point.Longitude >= 180 || point.Longitude <= -180)
                        return;

                    UTM utm = (UTM)point;
                    //utm.East.ToString("0.00") + " " + utm.North.ToString("0.00")
                    string[] parts = utm.ToString().Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                    if (Vertical)
                    {
                        e.Graphics.DrawString(parts[0] + "\n" + parts[1] + "\n" + parts[2] + "\n" + Alt.ToString("0.00") + AltUnit, this.Font, new SolidBrush(this.ForeColor), text, StringFormat.GenericDefault);
                        e.Graphics.DrawString(AltSource, this.Font, new SolidBrush(this.ForeColor),
                        new PointF(CMB_coordsystem.Left, CMB_coordsystem.Bottom + 4), StringFormat.GenericDefault);
                    }
                    else
                    {
                        e.Graphics.DrawString(utm.ToString() + "   " + Alt.ToString("0.00") + AltUnit, this.Font, new SolidBrush(this.ForeColor), text, StringFormat.GenericDefault);
                    }
                }
                catch { }
            }
            else if (System == CoordsSystems.MGRS.ToString())
            {
                try
                {
                    if (point.Latitude > 84 || point.Latitude < -80 || point.Longitude >= 180 || point.Longitude <= -180)
                        return;

                    MGRS mgrs = (MGRS)point;
                    mgrs.Precision = 5;

                    if (Vertical)
                    {
                        e.Graphics.DrawString(mgrs.ToString() + "\n" + Alt.ToString("0.00") + AltUnit, this.Font, new SolidBrush(this.ForeColor), new Point(5, CMB_coordsystem.Bottom + 2), StringFormat.GenericDefault);
                        e.Graphics.DrawString(AltSource, this.Font, new SolidBrush(this.ForeColor),
                        new PointF(CMB_coordsystem.Right + 4, CMB_coordsystem.Top), StringFormat.GenericDefault);
                    }
                    else
                    {
                        e.Graphics.DrawString(mgrs.ToString() + "   " + Alt.ToString("0.00") + AltUnit, this.Font, new SolidBrush(this.ForeColor), text, StringFormat.GenericDefault);
                    }
                }
                catch { }
            }  
        }

        private void CMB_coordsystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Invalidate();

            if (SystemChanged != null)
            {
                SystemChanged(this, null);
            }
        }
    }
}
