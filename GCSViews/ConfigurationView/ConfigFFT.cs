using System;
using System.Reflection;
using System.Windows.Forms;
using log4net;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public class ConfigFFT : MyUserControl, IActivate, IDeactivate
    {
        private MavlinkCheckBoxBitMask mavlinkCheckBoxBitMask1;
        private RangeControl rangeControl1;
        private MyButton but_fft;
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /*
        INS_LOG_BAT_CNT is just the number of samples taken. Must be at least 
        twice the frequency you're interested in.
        INS_LOG_BAT_MASK is the mask of the IMUs to be logged into the match.
            With only one IMU a value of "1" will be OK.
        */

        public ConfigFFT()
        {
            InitializeComponent();
            rangeControl1.MinRange = 32;
            rangeControl1.MaxRange = 1024 * 4;
            rangeControl1.DescriptionText = ParameterMetaDataRepository.GetParameterMetaData("INS_LOG_BAT_CNT",
                ParameterMetaDataConstants.Description, MainV2.comPort.MAV.cs.firmware.ToString());
            rangeControl1.DisplayScale = 1;
            var inc = 32.0;
            ParameterMetaDataRepository.GetParameterIncrement("INS_LOG_BAT_CNT",
                ref inc, MainV2.comPort.MAV.cs.firmware.ToString());
            rangeControl1.Increment = (float) inc;
            rangeControl1.LabelText = ParameterMetaDataRepository.GetParameterMetaData("INS_LOG_BAT_CNT",
                ParameterMetaDataConstants.DisplayName, MainV2.comPort.MAV.cs.firmware.ToString());
            rangeControl1.Value = MainV2.comPort.MAV.param["INS_LOG_BAT_CNT"].ToString();
            rangeControl1.Name = "INS_LOG_BAT_CNT";

            rangeControl1.ValueChanged += RangeControl1OnValueChanged;


            mavlinkCheckBoxBitMask1.setup("INS_LOG_BAT_MASK", MainV2.comPort.MAV.param);
        }

        private void RangeControl1OnValueChanged(object sender, string name, string value)
        {
            MainV2.comPort.setParam(name, double.Parse(value));
        }

        public void Activate()
        {
            if (!MainV2.comPort.MAV.param.ContainsKey("INS_LOG_BAT_CNT"))
            {
                Enabled = false;
                return;
            }
        }

        public void Deactivate()
        {
            MainV2.comPort.giveComport = false;
        }

        private void InitializeComponent()
        {
            this.mavlinkCheckBoxBitMask1 = new MissionPlanner.Controls.MavlinkCheckBoxBitMask();
            this.rangeControl1 = new MissionPlanner.Controls.RangeControl();
            this.but_fft = new MissionPlanner.Controls.MyButton();
            this.SuspendLayout();
            // 
            // mavlinkCheckBoxBitMask1
            // 
            this.mavlinkCheckBoxBitMask1.Enabled = false;
            this.mavlinkCheckBoxBitMask1.Location = new System.Drawing.Point(3, 117);
            this.mavlinkCheckBoxBitMask1.Name = "mavlinkCheckBoxBitMask1";
            this.mavlinkCheckBoxBitMask1.ParamName = null;
            this.mavlinkCheckBoxBitMask1.Size = new System.Drawing.Size(620, 115);
            this.mavlinkCheckBoxBitMask1.TabIndex = 3;
            this.mavlinkCheckBoxBitMask1.Value = 0F;
            // 
            // rangeControl1
            // 
            this.rangeControl1.DescriptionText = "Y\r\nY\r\nY";
            this.rangeControl1.DisplayScale = 1F;
            this.rangeControl1.Increment = 1F;
            this.rangeControl1.LabelText = "myLabel1";
            this.rangeControl1.Location = new System.Drawing.Point(3, 3);
            this.rangeControl1.MaxRange = 10F;
            this.rangeControl1.MinRange = 0F;
            this.rangeControl1.Name = "rangeControl1";
            this.rangeControl1.Size = new System.Drawing.Size(620, 108);
            this.rangeControl1.TabIndex = 4;
            this.rangeControl1.Value = "0";
            // 
            // but_fft
            // 
            this.but_fft.Location = new System.Drawing.Point(3, 238);
            this.but_fft.Name = "but_fft";
            this.but_fft.Size = new System.Drawing.Size(75, 23);
            this.but_fft.TabIndex = 5;
            this.but_fft.Text = "FFT";
            this.but_fft.UseVisualStyleBackColor = true;
            this.but_fft.Click += new System.EventHandler(this.but_fft_Click);
            // 
            // ConfigFFT
            // 
            this.Controls.Add(this.but_fft);
            this.Controls.Add(this.rangeControl1);
            this.Controls.Add(this.mavlinkCheckBoxBitMask1);
            this.Name = "ConfigFFT";
            this.Size = new System.Drawing.Size(626, 294);
            this.ResumeLayout(false);

        }

        private void but_fft_Click(object sender, EventArgs e)
        {
            new fftui().Show();
        }
    }
}