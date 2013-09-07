using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Utilities;
using GeoUtility.GeoSystem;

namespace MissionPlanner.Controls
{
    public partial class Coords : UserControl
    {
        [System.ComponentModel.Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public string System { get { return CMB_coordsystem.Text; } set { CMB_coordsystem.Text = value; } }
        public double Lat { get { return point.Latitude; } set { point.Latitude = value; this.Invalidate(); } }
        public double Lng { get { return point.Longitude; } set { point.Longitude = value; this.Invalidate(); } }
        public double Alt { get { return _alt; } set { _alt = value; this.Invalidate(); } }

        double _alt = 0;
        Geographic point = new Geographic();

        public enum CoordsSystems
        {
            GEO,
            UTM,
            MGRS
        }

        public Coords()
        {
            InitializeComponent();

            CMB_coordsystem.DataSource = Enum.GetNames(typeof(CoordsSystems));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            PointF text = new PointF(CMB_coordsystem.Right + 3, 3);

            //Enum.GetValues(typeof(CoordsSystems), CMB_coordsystem.Text);

            if (System == CoordsSystems.GEO.ToString())
            {
                e.Graphics.DrawString(Lat.ToString("0.000000") + " " + Lng.ToString("0.000000") + "   " + Alt.ToString("0.00"), this.Font, new SolidBrush(this.ForeColor), text, StringFormat.GenericDefault);
            } 
            else  if (System == CoordsSystems.UTM.ToString())
            {
                UTM utm = (UTM)point;
                //utm.East.ToString("0.00") + " " + utm.North.ToString("0.00")
                e.Graphics.DrawString(utm.ToString() + "   " + Alt.ToString("0.00"), this.Font, new SolidBrush(this.ForeColor), text, StringFormat.GenericDefault);
            }
            else if (System == CoordsSystems.MGRS.ToString())
            {
                MGRS mgrs = (MGRS)point;
                mgrs.Precision = 5;
                e.Graphics.DrawString(mgrs.ToString() + "   " + Alt.ToString("0.00"), this.Font, new SolidBrush(this.ForeColor), text, StringFormat.GenericDefault);
            }  
        }

        private void CMB_coordsystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }
    }
}
