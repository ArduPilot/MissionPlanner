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
        private MavlinkCheckBoxBitMask INS_LOG_BAT_MASK;
        private RangeControl INS_LOG_BAT_CNT;
        private MyButton but_fft;
        private MavlinkCheckBoxBitMask LOG_BITMASK;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
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

            if (!MainV2.comPort.MAV.param.ContainsKey("INS_LOG_BAT_CNT"))
            {
                Enabled = false;
                return;
            }

            var inc = 32.0;
            ParameterMetaDataRepository.GetParameterIncrement("INS_LOG_BAT_CNT",
                ref inc, MainV2.comPort.MAV.cs.firmware.ToString());

            INS_LOG_BAT_CNT.setup("INS_LOG_BAT_CNT", ParameterMetaDataRepository.GetParameterMetaData("INS_LOG_BAT_CNT",
                    ParameterMetaDataConstants.Description, MainV2.comPort.MAV.cs.firmware.ToString()),
                ParameterMetaDataRepository.GetParameterMetaData("INS_LOG_BAT_CNT",
                    ParameterMetaDataConstants.DisplayName, MainV2.comPort.MAV.cs.firmware.ToString()), (float) inc, 1,
                32, 1024 * 4, MainV2.comPort.MAV.param["INS_LOG_BAT_CNT"].ToString());
            
            INS_LOG_BAT_CNT.ValueChanged += RangeControl1OnValueChanged;

            INS_LOG_BAT_MASK.setup("INS_LOG_BAT_MASK", MainV2.comPort.MAV.param);

            LOG_BITMASK.setup("LOG_BITMASK", MainV2.comPort.MAV.param);
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
            this.INS_LOG_BAT_MASK = new MissionPlanner.Controls.MavlinkCheckBoxBitMask();
            this.INS_LOG_BAT_CNT = new MissionPlanner.Controls.RangeControl();
            this.but_fft = new MissionPlanner.Controls.MyButton();
            this.LOG_BITMASK = new MissionPlanner.Controls.MavlinkCheckBoxBitMask();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // INS_LOG_BAT_MASK
            // 
            this.INS_LOG_BAT_MASK.Enabled = false;
            this.INS_LOG_BAT_MASK.Location = new System.Drawing.Point(6, 138);
            this.INS_LOG_BAT_MASK.Name = "INS_LOG_BAT_MASK";
            this.INS_LOG_BAT_MASK.ParamName = null;
            this.INS_LOG_BAT_MASK.Size = new System.Drawing.Size(604, 115);
            this.INS_LOG_BAT_MASK.TabIndex = 3;
            this.INS_LOG_BAT_MASK.Value = 0F;
            // 
            // INS_LOG_BAT_CNT
            // 
            this.INS_LOG_BAT_CNT.DescriptionText = "Description";
            this.INS_LOG_BAT_CNT.DisplayScale = 1F;
            this.INS_LOG_BAT_CNT.Increment = 1F;
            this.INS_LOG_BAT_CNT.LabelText = "Label";
            this.INS_LOG_BAT_CNT.Location = new System.Drawing.Point(6, 19);
            this.INS_LOG_BAT_CNT.MaxRange = 10F;
            this.INS_LOG_BAT_CNT.MinRange = 0F;
            this.INS_LOG_BAT_CNT.Name = "INS_LOG_BAT_CNT";
            this.INS_LOG_BAT_CNT.Size = new System.Drawing.Size(604, 108);
            this.INS_LOG_BAT_CNT.TabIndex = 4;
            this.INS_LOG_BAT_CNT.Value = "0";
            // 
            // but_fft
            // 
            this.but_fft.Location = new System.Drawing.Point(629, 13);
            this.but_fft.Name = "but_fft";
            this.but_fft.Size = new System.Drawing.Size(75, 23);
            this.but_fft.TabIndex = 5;
            this.but_fft.Text = "FFT";
            this.but_fft.UseVisualStyleBackColor = true;
            this.but_fft.Click += new System.EventHandler(this.but_fft_Click);
            // 
            // LOG_BITMASK
            // 
            this.LOG_BITMASK.Enabled = false;
            this.LOG_BITMASK.Location = new System.Drawing.Point(6, 19);
            this.LOG_BITMASK.Name = "LOG_BITMASK";
            this.LOG_BITMASK.ParamName = null;
            this.LOG_BITMASK.Size = new System.Drawing.Size(604, 105);
            this.LOG_BITMASK.TabIndex = 6;
            this.LOG_BITMASK.Value = 0F;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.INS_LOG_BAT_CNT);
            this.groupBox1.Controls.Add(this.INS_LOG_BAT_MASK);
            this.groupBox1.Location = new System.Drawing.Point(7, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(616, 264);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "FFT Setup";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.LOG_BITMASK);
            this.groupBox2.Location = new System.Drawing.Point(7, 273);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(616, 130);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Please ensure IMU_RAW and IMU_FAST are turned off to use FFT";
            // 
            // ConfigFFT
            // 
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.but_fft);
            this.Controls.Add(this.groupBox1);
            this.Name = "ConfigFFT";
            this.Size = new System.Drawing.Size(713, 411);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void but_fft_Click(object sender, EventArgs e)
        {
            new fftui().Show();
        }
    }
}