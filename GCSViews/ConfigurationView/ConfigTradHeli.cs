using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using ZedGraph;
using Timer = System.Windows.Forms.Timer;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigTradHeli : UserControl, IActivate, IDeactivate
    {
        private readonly Timer timer = new Timer();
        private bool inpwmdetect;
        private bool startup;

        public ConfigTradHeli()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            if (MainV2.comPort.MAV.param["H_SWASH_TYPE"] == null)
            {
                Enabled = false;
                return;
            }

            timer.Tick += timer_Tick;

            timer.Enabled = true;
            timer.Interval = 100;
            timer.Start();

            // setup graph
            GraphPane myPane = zedGraphControl1.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "Collective Control";
            myPane.XAxis.Title.Text = "Collective Input (%)";
            myPane.XAxis.Scale.Min = 0;
            myPane.XAxis.Scale.Max = 100;
            myPane.XAxis.Scale.BaseTic = 0;
            myPane.XAxis.Scale.MinorStep = 10;
            myPane.XAxis.Scale.MajorStep = 20;
            myPane.YAxis.Title.Text = "Collective Output";
            myPane.YAxis.Scale.Min = 0;
            myPane.YAxis.Scale.Max = 1000;
            myPane.YAxis.Scale.BaseTic = 0;
            myPane.YAxis.Scale.MinorStep = 100;
            myPane.YAxis.Scale.MajorStep = 200;

            mavlinkNumericUpDownH_PHANG.setup(0, 99, 1, 1f, "H_PHANG", MainV2.comPort.MAV.param);
            mavlinkCheckBoxatc_piro_comp.setup(1, 0, "ATC_PIRO_COMP", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownh_sv_test.setup(0, 99, 1, 1, "H_SV_TEST", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownatc_hovr_rol_trm.setup(0, 99, 100, 0.1f, "ATC_HOVR_ROL_TRM", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownh_cyc_max.setup(0, 99, 100f, 0.1f, "H_CYC_MAX", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownH_RSC_CRITICAL.setup(0, 99, 1, 1f, "H_RSC_CRITICAL", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownh_rsc_max.setup(800, 2200, 1, 1f, "H_RSC_MAX", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownh_rsc_min.setup(800, 2200, 1, 1f, "H_RSC_MIN", MainV2.comPort.MAV.param);
            h_rsc_rev.setup(-1, 1, "H_RSC_REV", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownh_rsc_power_high.setup(0, 99, 1, 1f, "H_RSC_POWER_HIGH", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownh_rsc_power_low.setup(0, 99, 1, 1f, "H_RSC_POWER_LOW", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownh_rsc_idle.setup(0, 99, 1, 1f, "H_RSC_IDLE", MainV2.comPort.MAV.param);

            mavlinkNumericUpDownim_stab_col_1.setup(0, 1000, 1, 1f, "IM_STAB_COL_1", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownim_stab_col_2.setup(0, 1000, 1, 1f, "IM_STAB_COL_2", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownim_stab_col_3.setup(0, 1000, 1, 1f, "IM_STAB_COL_3", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownim_stab_col_4.setup(0, 1000, 1, 1f, "IM_STAB_COL_4", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownim_acro_col_exp.setup(0, 1, 1, 0.01f, "IM_ACRO_COL_EXP", MainV2.comPort.MAV.param);

            mavlinkComboBoxTailType.setup(
                ParameterMetaDataRepository.GetParameterOptionsInt("H_TAIL_TYPE",
                    MainV2.comPort.MAV.cs.firmware.ToString()), "H_TAIL_TYPE", MainV2.comPort.MAV.param);

            mavlinkNumericUpDowntailspeed.setup(0, 1000, 1, 1, "H_TAIL_SPEED", MainV2.comPort.MAV.param);

            mavlinkNumericUpDownland_col_min.setup(0, 1000, 1, 1, "H_LAND_COL_MIN", MainV2.comPort.MAV.param);

            H_COLYAW.setup(0, 5, 1, 0.01f, "H_COLYAW", MainV2.comPort.MAV.param);
            mavlinkudH_RSC_RATE.setup(0, 60, 1, 1, "H_RSC_RAMP_TIME", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownrunuptime.setup(0, 60, 1, 1, "H_RSC_RUNUP_TIME", MainV2.comPort.MAV.param);
            H_RSC_MODE.setup(
                ParameterMetaDataRepository.GetParameterOptionsInt("H_RSC_MODE",
                    MainV2.comPort.MAV.cs.firmware.ToString()), "H_RSC_MODE", MainV2.comPort.MAV.param);
            mavlinkudH_RSC_SETPOINT.setup(800, 2200, 1, 1, "H_RSC_SETPOINT", MainV2.comPort.MAV.param);

            startup = true;
            try
            {
                if (MainV2.comPort.MAV.param.ContainsKey("H_SWASH_TYPE"))
                {
                    CCPM.Checked = MainV2.comPort.MAV.param["H_SWASH_TYPE"].ToString() == "0" ? true : false;
                    H_SWASH_TYPE.Checked = !CCPM.Checked;
                }

                if (MainV2.comPort.MAV.param.ContainsKey("H_FLYBAR_MODE"))
                {
                    fbl_modeFBL.Checked = MainV2.comPort.MAV.param["H_FLYBAR_MODE"].ToString() == "0" ? true : false;
                }

                foreach (string value in MainV2.comPort.MAV.param.Keys)
                {
                    if (value == "")
                        continue;

                    var control = Controls.Find(value, true);
                    if (control.Length > 0)
                    {
                        if (control[0].GetType() == typeof (TextBox))
                        {
                            var temp = (TextBox) control[0];
                            var option = MainV2.comPort.MAV.param[value].ToString();
                            temp.Text = option;
                        }
                        if (control[0].GetType() == typeof (NumericUpDown))
                        {
                            var temp = (NumericUpDown) control[0];
                            var option = MainV2.comPort.MAV.param[value].ToString();
                            temp.Text = option;
                        }
                        if (control[0].GetType() == typeof (CheckBox))
                        {
                            var temp = (CheckBox) control[0];
                            var option = MainV2.comPort.MAV.param[value].ToString();
                            temp.Checked = option == "1" ? true : false;
                        }
                        if (control[0].GetType() == typeof (MyTrackBar))
                        {
                            var temp = (MyTrackBar) control[0];
                            var option = MainV2.comPort.MAV.param[value].ToString();
                            temp.Value = int.Parse(option);
                        }
                    }
                }

                HS1_REV.Checked = MainV2.comPort.MAV.param["HS1_REV"].ToString() == "-1";
                HS2_REV.Checked = MainV2.comPort.MAV.param["HS2_REV"].ToString() == "-1";
                HS3_REV.Checked = MainV2.comPort.MAV.param["HS3_REV"].ToString() == "-1";
                HS4_REV.Checked = MainV2.comPort.MAV.param["HS4_REV"].ToString() == "-1";
            }
            catch
            {
            }
            startup = false;
        }

        void GenerateGraphData()
        {
            PointPairList stab = new PointPairList();
            stab.Add(0, MainV2.comPort.MAV.param["IM_STAB_COL_1"].Value);
            stab.Add(40, MainV2.comPort.MAV.param["IM_STAB_COL_2"].Value);
            stab.Add(60, MainV2.comPort.MAV.param["IM_STAB_COL_3"].Value);
            stab.Add(100, MainV2.comPort.MAV.param["IM_STAB_COL_4"].Value);

            PointPairList acro = new PointPairList();

            double _acro_col_expo = MainV2.comPort.MAV.param["IM_ACRO_COL_EXP"].Value;

            // 100 point curve
            for (int a = 0; a <= 100; a++)
            {
                double col_in = (a-50.0)/50.0;
                double col_in3 = col_in*col_in*col_in;
                double col_out = (_acro_col_expo*col_in3) + ((1 - _acro_col_expo)*col_in);
                double acro_col_out = 500 + col_out*500;

                acro.Add(a, acro_col_out);
            }

            zedGraphControl1.GraphPane.CurveList.Clear();
            zedGraphControl1.GraphPane.GraphObjList.Clear();

            var myCurve = zedGraphControl1.GraphPane.AddCurve("Stabalize Collective", stab, Color.DodgerBlue, SymbolType.Circle);

            foreach (PointPair pp in stab)
            {
                // Add a another text item to to point out a graph feature
                TextObj text = new TextObj(pp.X.ToString(), pp.X, pp.Y);
                // rotate the text 90 degrees
                text.FontSpec.Angle = 0;
                text.FontSpec.FontColor = Color.White;
                // Align the text such that the Right-Center is at (700, 50) in user scale coordinates
                text.Location.AlignH = AlignH.Right;
                text.Location.AlignV = AlignV.Center;
                // Disable the border and background fill options for the text
                text.FontSpec.Fill.IsVisible = false;
                text.FontSpec.Border.IsVisible = false;
                zedGraphControl1.GraphPane.GraphObjList.Add(text);
            }

            zedGraphControl1.GraphPane.AddCurve("Acro Collective", acro, Color.Yellow, SymbolType.None);

            double posx = map(MainV2.comPort.MAV.cs.ch6out, MainV2.comPort.MAV.param["H_COL_MIN"].Value,
                MainV2.comPort.MAV.param["H_COL_MAX"].Value, 0, 100);

            // set current marker
            var m_cursorLine = new LineObj(Color.Black, posx, 0, posx, 1);

            m_cursorLine.Location.CoordinateFrame = CoordType.XScaleYChartFraction; // This do the trick !
            m_cursorLine.IsClippedToChartRect = true;
            m_cursorLine.Line.Style = System.Drawing.Drawing2D.DashStyle.Dash;
            m_cursorLine.Line.Width = 2f;
            m_cursorLine.Line.Color = Color.Red;
            m_cursorLine.ZOrder = ZOrder.E_BehindCurves;
            zedGraphControl1.GraphPane.GraphObjList.Add(m_cursorLine);

            try
            {
                //zedGraphControl1.AxisChange();
            }
            catch
            {
            }
            // Force a redraw
            zedGraphControl1.Invalidate();
        }

        double map(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        public void Deactivate()
        {
            timer.Stop();

            startup = true;
        }

        private void H_SWASH_TYPE_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            try
            {
                if (MainV2.comPort.MAV.param["H_SWASH_TYPE"] == null)
                {
                    CustomMessageBox.Show("Not Available on " + MainV2.comPort.MAV.cs.firmware);
                }
                else
                {
                    MainV2.comPort.setParam("H_SWASH_TYPE", ((RadioButton) sender).Checked ? 1 : 0);
                }
            }
            catch
            {
                CustomMessageBox.Show("Set H_SWASH_TYPE Failed");
            }
        }

        private void HS4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void HS3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void COL_MAX__Enter(object sender, EventArgs e)
        {
            inpwmdetect = true;
        }



        private void COL_MAX__Leave(object sender, EventArgs e)
        {
            inpwmdetect = false;
        }

      

        private void HS4_MAX_Enter(object sender, EventArgs e)
        {
            inpwmdetect = true;
        }

        private void HS4_MAX_Leave(object sender, EventArgs e)
        {
            inpwmdetect = false;
        }

        private void PWM_Validating(object sender, CancelEventArgs e)
        {
            var temp = (Control) (sender);

            var tempint = 0;
            if (int.TryParse(temp.Text, out tempint))
            {
                if (tempint < 900)
                    temp.Text = "900";
            }
            else
            {
                CustomMessageBox.Show("Bad Min PWM");
                return;
            }

            if (int.TryParse(temp.Text, out tempint))
            {
                if (tempint > 2100)
                    temp.Text = "2100";
            }
            else
            {
                CustomMessageBox.Show("Bad Max PWM");
                return;
            }

            MainV2.comPort.setParam(((TextBox)sender).Name, tempint);
        }

        private void TXT_srvpos1_Validating(object sender, CancelEventArgs e)
        {
            if (startup || Disposing)
                return;
            var test = 0;
            if (!int.TryParse(((TextBox) sender).Text, out test))
            {
                e.Cancel = true;
            }

            Gservoloc.Value0 = test;

            try
            {
                MainV2.comPort.setParam("H_SV_MAN", 1); // randy request
                MainV2.comPort.setParam(((TextBox) sender).Name, test);
                Thread.Sleep(100);
                MainV2.comPort.setParam("H_SV_MAN", 0); // randy request - last
            }
            catch
            {
                CustomMessageBox.Show("Set " + ((TextBox) sender).Name + " failed");
            }
        }

        private void TXT_srvpos2_Validating(object sender, CancelEventArgs e)
        {
            if (startup || Disposing)
                return;
            var test = 0;
            if (!int.TryParse(((TextBox) sender).Text, out test))
            {
                e.Cancel = true;
            }

            Gservoloc.Value1 = test;

            try
            {
                MainV2.comPort.setParam("H_SV_MAN", 1); // randy request
                MainV2.comPort.setParam(((TextBox) sender).Name, test);
                Thread.Sleep(100);
                MainV2.comPort.setParam("H_SV_MAN", 0); // randy request - last
            }
            catch
            {
                CustomMessageBox.Show("Set " + ((TextBox) sender).Name + " failed");
            }
        }

        private void TXT_srvpos3_Validating(object sender, CancelEventArgs e)
        {
            if (startup || Disposing)
                return;
            var test = 0;
            if (!int.TryParse(((TextBox) sender).Text, out test))
            {
                e.Cancel = true;
            }

            Gservoloc.Value2 = test;

            try
            {
                MainV2.comPort.setParam("H_SV_MAN", 1); // randy request
                MainV2.comPort.setParam(((TextBox) sender).Name, test);
                Thread.Sleep(100);
                MainV2.comPort.setParam("H_SV_MAN", 0); // randy request - last
            }
            catch
            {
                CustomMessageBox.Show("Set " + ((TextBox) sender).Name + " failed");
            }
        }

        private void HS1_REV_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            MainV2.comPort.setParam(((CheckBox) sender).Name, ((CheckBox) sender).Checked == false ? 1.0f : -1.0f);
        }

        private void HS2_REV_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            MainV2.comPort.setParam(((CheckBox) sender).Name, ((CheckBox) sender).Checked == false ? 1.0f : -1.0f);
        }

        private void HS3_REV_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            MainV2.comPort.setParam(((CheckBox) sender).Name, ((CheckBox) sender).Checked == false ? 1.0f : -1.0f);
        }

        private void HS4_REV_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            MainV2.comPort.setParam(((CheckBox) sender).Name, ((CheckBox) sender).Checked == false ? 1.0f : -1.0f);
        }

        private void HS1_TRIM_ValueChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            MainV2.comPort.setParam(((NumericUpDown) sender).Name, (float) ((NumericUpDown) sender).Value);
        }

        private void HS2_TRIM_ValueChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            MainV2.comPort.setParam(((NumericUpDown) sender).Name, (float) ((NumericUpDown) sender).Value);
        }

        private void HS3_TRIM_ValueChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            MainV2.comPort.setParam(((NumericUpDown) sender).Name, (float) ((NumericUpDown) sender).Value);
        }

        private void HS4_TRIM_ValueChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            MainV2.comPort.setParam(((NumericUpDown) sender).Name, (float) ((NumericUpDown) sender).Value);
        }


        private void GYR_GAIN__Validating(object sender, CancelEventArgs e)
        {
            if (startup || Disposing || !Enabled)
                return;
            var test = 0;
            if (!int.TryParse(((TextBox) sender).Text, out test))
            {
                e.Cancel = true;
            }

            try
            {
                MainV2.comPort.setParam(((TextBox) sender).Name, test);
            }
            catch
            {
                CustomMessageBox.Show("Failed to set Gyro Gain");
            }
        }


        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                MainV2.comPort.MAV.cs.UpdateCurrentSettings(currentStateBindingSource);

                GenerateGraphData();
            }
            catch
            {
            }

            if (MainV2.comPort.MAV.param["H_SV_MAN"] == null || MainV2.comPort.MAV.param["H_SV_MAN"].ToString() == "0")
                return;

            if (HS3.minline == 0)
                HS3.minline = 2200;

            if (HS4.minline == 0)
                HS4.minline = 2200;

            HS3.minline = Math.Min(HS3.minline, (int) MainV2.comPort.MAV.cs.ch3in);
            HS3.maxline = Math.Max(HS3.maxline, (int) MainV2.comPort.MAV.cs.ch3in);

            HS4.minline = Math.Min(HS4.minline, (int) MainV2.comPort.MAV.cs.ch4in);
            HS4.maxline = Math.Max(HS4.maxline, (int) MainV2.comPort.MAV.cs.ch4in);

            if (!inpwmdetect)
            {
                HS3_Paint(null, null);
                HS4_Paint(null, null);
            }
            else
            {
                try
                {
                    HS3.minline = int.Parse(H_COL_MIN.Text);
                    HS3.maxline = int.Parse(H_COL_MAX.Text);
                    HS4.maxline = int.Parse(HS4_MIN.Text);
                    HS4.minline = int.Parse(HS4_MAX.Text);
                }
                catch
                {
                }
            }
        }

        private void fbl_modeFBL_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            try
            {
                if (MainV2.comPort.MAV.param["H_FLYBAR_MODE"] == null)
                {
                    CustomMessageBox.Show("Not Available on " + MainV2.comPort.MAV.cs.firmware);
                }
                else
                {
                    MainV2.comPort.setParam("H_FLYBAR_MODE", fbl_modeFBL.Checked ? 0 : 1);
                }
            }
            catch
            {
                CustomMessageBox.Show("Set H_FLYBAR_MODE Failed");
            }
        }

        private void myButtonH_SV_MAN_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.comPort.MAV.param["H_SV_MAN"] == null)
                {
                    CustomMessageBox.Show("Not Available on " + MainV2.comPort.MAV.cs.firmware);
                }
                else
                {
                    MainV2.comPort.setParam("H_SV_MAN", 0);
                }
            }
            catch
            {
                CustomMessageBox.Show("Set H_SV_MAN Failed");
            }
        }

        private void myButtonH_SV_MANmanual_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.comPort.MAV.param["H_SV_MAN"] == null)
                {
                    CustomMessageBox.Show("Not Available on " + MainV2.comPort.MAV.cs.firmware);
                }
                else
                {
                    MainV2.comPort.setParam("H_SV_MAN", 1);
                }
            }
            catch
            {
                CustomMessageBox.Show("Set H_SV_MAN Failed");
            }
        }

        private void myButtonH_SV_MANmax_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.comPort.MAV.param["H_SV_MAN"] == null)
                {
                    CustomMessageBox.Show("Not Available on " + MainV2.comPort.MAV.cs.firmware);
                }
                else
                {
                    MainV2.comPort.setParam("H_SV_MAN", 2);
                }
            }
            catch
            {
                CustomMessageBox.Show("Set H_SV_MAN Failed");
            }
        }

        private void myButtonH_SV_MANzero_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.comPort.MAV.param["H_SV_MAN"] == null)
                {
                    CustomMessageBox.Show("Not Available on " + MainV2.comPort.MAV.cs.firmware);
                }
                else
                {
                    MainV2.comPort.setParam("H_SV_MAN", 3);
                }
            }
            catch
            {
                CustomMessageBox.Show("Set H_SV_MAN Failed");
            }
        }

        private void myButtonH_SV_MANmin_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.comPort.MAV.param["H_SV_MAN"] == null)
                {
                    CustomMessageBox.Show("Not Available on " + MainV2.comPort.MAV.cs.firmware);
                }
                else
                {
                    MainV2.comPort.setParam("H_SV_MAN", 4);
                }
            }
            catch
            {
                CustomMessageBox.Show("Set H_SV_MAN Failed");
            }
        }

        private void myButtonH_SV_MANtest_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.comPort.MAV.param["H_SV_MAN"] == null)
                {
                    CustomMessageBox.Show("Not Available on " + MainV2.comPort.MAV.cs.firmware);
                }
                else
                {
                    MainV2.comPort.setParam("H_SV_MAN", 5);
                }
            }
            catch
            {
                CustomMessageBox.Show("Set H_SV_MAN Failed");
            }
        }
    }
}