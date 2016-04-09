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
    public partial class setBaBiaoFlyPlan : Form
    {
        public setBaBiaoFlyPlan()
        {
            InitializeComponent();
        }

        private bool checkInput()
        {
            return true;
        }

        private void btnCalMoveByEdgeSpaceRatio_Click(object sender, EventArgs e)
        {
            double temp1 = 0, temp2 = 0;

            FlightPlanner.baBiaoStartAzimuth = double.Parse(txtStartAzimuth.Text);

            temp1 = double.Parse(txtFlyHeight.Text) * double.Parse(txtSensorLength.Text) / double.Parse(txtFocus.Text) * (50 - double.Parse(txtEdgeSpace.Text)) / 100 - double.Parse(txtBaBiaoLength.Text) / 2 - double.Parse(txtGPS.Text) - double.Parse(txtWindEffect.Text);
            temp2 = double.Parse(txtFlyHeight.Text) * double.Parse(txtSensorWidth.Text) / double.Parse(txtFocus.Text) * (50 - double.Parse(txtEdgeSpace.Text)) / 100 - double.Parse(txtBaBiaoWidth.Text) / 2 - double.Parse(txtGPS.Text) - double.Parse(txtWindEffect.Text);
            temp1 = Math.Round(temp1, 1);
            temp2 = Math.Round(temp2, 1);

            

            if (rbtnSensorDirection1.Checked)
            {
                txtLengthMove.Text = temp1.ToString();
                txtWidthMove.Text = temp2.ToString();
               // FlightPlanner.MousePosition
                double theda =double.Parse(txtStartAzimuth.Text);  // 初始方位角
                double halfWidth = temp2+ double.Parse(txtBaBiaoWidth.Text) / 2;
                double halfLength = temp1 + double.Parse(txtBaBiaoLength.Text) / 2;
              
                FlightPlanner.arrayPointLatLngAlt[0] = FlightPlanner.targetCentral;
                FlightPlanner.arrayPointLatLngAlt[0].Alt = double.Parse(txtFlyHeight.Text);
             
                FlightPlanner.arrayPointLatLngAlt[1] =FlightPlanner.targetCentral.newpos(theda,halfWidth);
                theda =(theda+90)>=360 ? (theda+90-360):(theda +90);
                FlightPlanner.arrayPointLatLngAlt[2] =FlightPlanner.arrayPointLatLngAlt[1].newpos(theda,halfLength);

                theda = (theda + 90) >= 360 ? (theda + 90 - 360) : (theda + 90);
                FlightPlanner.arrayPointLatLngAlt[3] = FlightPlanner.arrayPointLatLngAlt[2].newpos(theda, halfWidth);

                //theda = (theda + 90) >= 360 ? (theda + 90 - 360) : (theda + 90);
                FlightPlanner.arrayPointLatLngAlt[4] = FlightPlanner.arrayPointLatLngAlt[3].newpos(theda, halfWidth);

                theda = (theda + 90) >= 360 ? (theda + 90 - 360) : (theda + 90);
                FlightPlanner.arrayPointLatLngAlt[5] = FlightPlanner.arrayPointLatLngAlt[4].newpos(theda, halfLength);

                //theda = (theda + 90) >= 360 ? (theda + 90 - 360) : (theda + 90);
                FlightPlanner.arrayPointLatLngAlt[6] = FlightPlanner.arrayPointLatLngAlt[5].newpos(theda, halfLength);

                theda = (theda + 90) >= 360 ? (theda + 90 - 360) : (theda + 90);
                FlightPlanner.arrayPointLatLngAlt[7] = FlightPlanner.arrayPointLatLngAlt[6].newpos(theda, halfWidth);

                //theda = (theda + 90) >= 360 ? (theda + 90 - 360) : (theda + 90);
                FlightPlanner.arrayPointLatLngAlt[8] = FlightPlanner.arrayPointLatLngAlt[7].newpos(theda, halfWidth);


                //for(int i=3 ; i<=8 ;)
                //{
                //    theda =(theda+90)>=360 ? (theda+90-360):(theda +90);
                //    if(i%2 !=0)
                //    {
                //        theda = (theda + 90) >= 360 ? (theda + 90 - 360) : (theda + 90);
                //        FlightPlanner.arrayPointLatLngAlt[i] = FlightPlanner.arrayPointLatLngAlt[i - 1].newpos(theda, halfWidth);
                //        i++;
                //       // theda = (theda + 90) >= 360 ? (theda + 90 - 360) : (theda + 90);
                //        FlightPlanner.arrayPointLatLngAlt[i] = FlightPlanner.arrayPointLatLngAlt[i - 1].newpos(theda, halfWidth);
                //        i++;
                //    }
                //    else
                //    {
                //        theda = (theda + 90) >= 360 ? (theda + 90 - 360) : (theda + 90);
                //        FlightPlanner.arrayPointLatLngAlt[i] = FlightPlanner.arrayPointLatLngAlt[i - 1].newpos(theda, halfLength);
                //        i++;
                //       // theda = (theda + 90) >= 360 ? (theda + 90 - 360) : (theda + 90);
                //        FlightPlanner.arrayPointLatLngAlt[i] = FlightPlanner.arrayPointLatLngAlt[i - 1].newpos(theda, halfLength);
                //        i++;
                //    }

                //}

            }
            else
            {

            }
        }
    }
}
