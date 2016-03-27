using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews
{
    public partial class NewAutoWP : Form
    {
        public NewAutoWP()
        {
            InitializeComponent();
        }

        private void btnProduceRoute_Click(object sender, EventArgs e)
        {
             FlightPlanner.strBulidingHeigth = txtBulidingHeigth.Text;          
             FlightPlanner.strCircleHeigth = txtCircleHeigth.Text;
             FlightPlanner.strRadius = txtRadius.Text;
             FlightPlanner.strPhotoNumber = txtPhotoNumber.Text;
             FlightPlanner.strStartAzimuth = txtStartAzimuth.Text;

             this.Dispose();

            // txtCircleHeigth = "";
            // txtRadius = "";
            // txtPhotoNumber = "";
            // txtStartAzimuth = "";

           //  FlightPlanner  FP;
           //  FP = (FlightPlanner)this.Owner;
           ////  f1.Refresh_Method();

         //   FlightPlanner

            // DoSomeThing();
        }

        private void txtBulidingHeigth_TextChanged(object sender, EventArgs e)
        {

        }


    }
}
