using MissionPlanner;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.GCSViews;


namespace generator
{
    public class generator : UserControl
    {
        public generator()
        {
            InitializeComponent();
        }  
        
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.aGaugeSpeed = new AGaugeApp.AGauge();
            this.statusTxt = new System.Windows.Forms.Label();
            this.powerTxt = new System.Windows.Forms.Label();
            this.voltageTxt = new System.Windows.Forms.Label();
            this.runStatusTxt = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.powerLabel = new System.Windows.Forms.Label();
            this.voltageLabel = new System.Windows.Forms.Label();
            this.runstatusLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.aGaugeSpeed);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.statusTxt);
            this.splitContainer3.Panel2.Controls.Add(this.powerTxt);
            this.splitContainer3.Panel2.Controls.Add(this.voltageTxt);
            this.splitContainer3.Panel2.Controls.Add(this.runStatusTxt);
            this.splitContainer3.Panel2.Controls.Add(this.statusLabel);
            this.splitContainer3.Panel2.Controls.Add(this.powerLabel);
            this.splitContainer3.Panel2.Controls.Add(this.voltageLabel);
            this.splitContainer3.Panel2.Controls.Add(this.runstatusLabel);
            this.splitContainer3.Size = new System.Drawing.Size(357, 150);
            this.splitContainer3.SplitterDistance = 148;
            this.splitContainer3.TabIndex = 0;
            // 
            // aGaugeSpeed
            // 
            this.aGaugeSpeed.BackColor = System.Drawing.Color.Transparent;
            this.aGaugeSpeed.BackgroundImage = global::MissionPlanner.Properties.Resources.Gaugebg;
            this.aGaugeSpeed.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.aGaugeSpeed.BaseArcColor = System.Drawing.Color.Transparent;
            this.aGaugeSpeed.BaseArcRadius = 70;
            this.aGaugeSpeed.BaseArcStart = 135;
            this.aGaugeSpeed.BaseArcSweep = 270;
            this.aGaugeSpeed.BaseArcWidth = 2;
            this.aGaugeSpeed.Cap_Idx = ((byte)(2));
            this.aGaugeSpeed.CapColor = System.Drawing.Color.Black;
            this.aGaugeSpeed.CapColors = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black};
            this.aGaugeSpeed.CapPosition = new System.Drawing.Point(10, 10);
            this.aGaugeSpeed.CapsPosition = new System.Drawing.Point[] {
        new System.Drawing.Point(58, 85),
        new System.Drawing.Point(50, 110),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10)};
            this.aGaugeSpeed.CapsText = new string[] {
        " Rate",
        "",
        "",
        "",
        ""};
            this.aGaugeSpeed.CapText = "";
            this.aGaugeSpeed.Center = new System.Drawing.Point(75, 75);
            this.aGaugeSpeed.Location = new System.Drawing.Point(0, 0);
            this.aGaugeSpeed.MaxValue = 15F;
            this.aGaugeSpeed.MinValue = 0F;
            this.aGaugeSpeed.Name = "aGaugeSpeed";
            this.aGaugeSpeed.Need_Idx = ((byte)(3));
            this.aGaugeSpeed.NeedleColor1 = AGaugeApp.AGauge.NeedleColorEnum.Gray;
            this.aGaugeSpeed.NeedleColor2 = System.Drawing.Color.Brown;
            this.aGaugeSpeed.NeedleEnabled = false;
            this.aGaugeSpeed.NeedleRadius = 70;
            this.aGaugeSpeed.NeedlesColor1 = new AGaugeApp.AGauge.NeedleColorEnum[] {
        AGaugeApp.AGauge.NeedleColorEnum.Gray,
        AGaugeApp.AGauge.NeedleColorEnum.Red,
        AGaugeApp.AGauge.NeedleColorEnum.Blue,
        AGaugeApp.AGauge.NeedleColorEnum.Gray};
            this.aGaugeSpeed.NeedlesColor2 = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.Brown};
            this.aGaugeSpeed.NeedlesEnabled = new bool[] {
        false,
        true,
        false,
        false};
            this.aGaugeSpeed.NeedlesRadius = new int[] {
        50,
        50,
        70,
        70};
            this.aGaugeSpeed.NeedlesType = new int[] {
        0,
        0,
        0,
        0};
            this.aGaugeSpeed.NeedlesWidth = new int[] {
        2,
        1,
        2,
        2};
            this.aGaugeSpeed.NeedleType = 0;
            this.aGaugeSpeed.NeedleWidth = 2;
            this.aGaugeSpeed.Range_Idx = ((byte)(2));
            this.aGaugeSpeed.RangeColor = System.Drawing.Color.Orange;
            this.aGaugeSpeed.RangeEnabled = false;
            this.aGaugeSpeed.RangeEndValue = 50F;
            this.aGaugeSpeed.RangeInnerRadius = 1;
            this.aGaugeSpeed.RangeOuterRadius = 70;
            this.aGaugeSpeed.RangesColor = new System.Drawing.Color[] {
        System.Drawing.Color.LightGreen,
        System.Drawing.Color.Red,
        System.Drawing.Color.Orange,
        System.Drawing.SystemColors.Control,
        System.Drawing.SystemColors.Control};
            this.aGaugeSpeed.RangesEnabled = new bool[] {
        false,
        false,
        false,
        false,
        false};
            this.aGaugeSpeed.RangesEndValue = new float[] {
        35F,
        60F,
        50F,
        0F,
        0F};
            this.aGaugeSpeed.RangesInnerRadius = new int[] {
        1,
        1,
        1,
        70,
        70};
            this.aGaugeSpeed.RangesOuterRadius = new int[] {
        70,
        70,
        70,
        80,
        80};
            this.aGaugeSpeed.RangesStartValue = new float[] {
        0F,
        50F,
        35F,
        0F,
        0F};
            this.aGaugeSpeed.RangeStartValue = 35F;
            this.aGaugeSpeed.ScaleLinesInterColor = System.Drawing.Color.White;
            this.aGaugeSpeed.ScaleLinesInterInnerRadius = 52;
            this.aGaugeSpeed.ScaleLinesInterOuterRadius = 60;
            this.aGaugeSpeed.ScaleLinesInterWidth = 1;
            this.aGaugeSpeed.ScaleLinesMajorColor = System.Drawing.Color.White;
            this.aGaugeSpeed.ScaleLinesMajorInnerRadius = 50;
            this.aGaugeSpeed.ScaleLinesMajorOuterRadius = 60;
            this.aGaugeSpeed.ScaleLinesMajorStepValue = 2F;
            this.aGaugeSpeed.ScaleLinesMajorWidth = 2;
            this.aGaugeSpeed.ScaleLinesMinorColor = System.Drawing.Color.White;
            this.aGaugeSpeed.ScaleLinesMinorInnerRadius = 55;
            this.aGaugeSpeed.ScaleLinesMinorNumOf = 13;
            this.aGaugeSpeed.ScaleLinesMinorOuterRadius = 60;
            this.aGaugeSpeed.ScaleLinesMinorWidth = 1;
            this.aGaugeSpeed.ScaleNumbersColor = System.Drawing.Color.White;
            this.aGaugeSpeed.ScaleNumbersFormat = null;
            this.aGaugeSpeed.ScaleNumbersRadius = 42;
            this.aGaugeSpeed.ScaleNumbersRotation = 0;
            this.aGaugeSpeed.ScaleNumbersStartScaleLine = 1;
            this.aGaugeSpeed.ScaleNumbersStepScaleLines = 1;
            this.aGaugeSpeed.Size = new System.Drawing.Size(152, 152);
            this.aGaugeSpeed.TabIndex = 0;
            this.aGaugeSpeed.Value = 0F;
            this.aGaugeSpeed.Value0 = 0F;
            this.aGaugeSpeed.Value1 = 0F;
            this.aGaugeSpeed.Value2 = 0F;
            this.aGaugeSpeed.Value3 = 0F;
            // 
            // statusTxt
            // 
            this.statusTxt.AutoSize = true;
            this.statusTxt.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this.statusTxt.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.statusTxt.Location = new System.Drawing.Point(136, 80);
            this.statusTxt.Name = "statusTxt";
            this.statusTxt.Size = new System.Drawing.Size(34, 17);
            this.statusTxt.TabIndex = 11;
            this.statusTxt.Text = "IDLE";
            // 
            // powerTxt
            // 
            this.powerTxt.AutoSize = true;
            this.powerTxt.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this.powerTxt.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.powerTxt.Location = new System.Drawing.Point(136, 57);
            this.powerTxt.Name = "powerTxt";
            this.powerTxt.Size = new System.Drawing.Size(41, 17);
            this.powerTxt.TabIndex = 10;
            this.powerTxt.Text = "100W";
            // 
            // voltageTxt
            // 
            this.voltageTxt.AutoSize = true;
            this.voltageTxt.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this.voltageTxt.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.voltageTxt.Location = new System.Drawing.Point(136, 34);
            this.voltageTxt.Name = "voltageTxt";
            this.voltageTxt.Size = new System.Drawing.Size(30, 17);
            this.voltageTxt.TabIndex = 9;
            this.voltageTxt.Text = "25V";
            // 
            // runStatusTxt
            // 
            this.runStatusTxt.AutoSize = true;
            this.runStatusTxt.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this.runStatusTxt.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.runStatusTxt.Location = new System.Drawing.Point(136, 13);
            this.runStatusTxt.Name = "runStatusTxt";
            this.runStatusTxt.Size = new System.Drawing.Size(26, 17);
            this.runStatusTxt.TabIndex = 8;
            this.runStatusTxt.Text = "OK";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this.statusLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.statusLabel.Location = new System.Drawing.Point(21, 80);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(43, 17);
            this.statusLabel.TabIndex = 5;
            this.statusLabel.Text = "Status";
            // 
            // powerLabel
            // 
            this.powerLabel.AutoSize = true;
            this.powerLabel.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this.powerLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.powerLabel.Location = new System.Drawing.Point(21, 57);
            this.powerLabel.Name = "powerLabel";
            this.powerLabel.Size = new System.Drawing.Size(44, 17);
            this.powerLabel.TabIndex = 4;
            this.powerLabel.Text = "Power";
            // 
            // voltageLabel
            // 
            this.voltageLabel.AutoSize = true;
            this.voltageLabel.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this.voltageLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.voltageLabel.Location = new System.Drawing.Point(21, 34);
            this.voltageLabel.Name = "voltageLabel";
            this.voltageLabel.Size = new System.Drawing.Size(53, 17);
            this.voltageLabel.TabIndex = 3;
            this.voltageLabel.Text = "Voltage";
            // 
            // runstatusLabel
            // 
            this.runstatusLabel.AutoSize = true;
            this.runstatusLabel.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this.runstatusLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.runstatusLabel.Location = new System.Drawing.Point(21, 13);
            this.runstatusLabel.Name = "runstatusLabel";
            this.runstatusLabel.Size = new System.Drawing.Size(69, 17);
            this.runstatusLabel.TabIndex = 2;
            this.runstatusLabel.Text = "Run Status";
            // 
            // generator
            // 
            this.Controls.Add(this.splitContainer3);
            this.Name = "generator";
            this.Size = new System.Drawing.Size(357, 150);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        internal AGaugeApp.AGauge aGaugeSpeed;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Label runstatusLabel;
        private System.Windows.Forms.Label powerLabel;
        private System.Windows.Forms.Label voltageLabel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Timer infoTimer;
        internal System.Windows.Forms.Label statusTxt;
        internal System.Windows.Forms.Label powerTxt;
        internal System.Windows.Forms.Label voltageTxt;
        internal System.Windows.Forms.Label runStatusTxt;
    }

    public class Plugin : MissionPlanner.Plugin.Plugin
    {
        public override string Name
        {
            get { return "generator"; }
        }

        public override string Version
        {
            get { return "0.10"; }
        }

        public override string Author
        {
            get { return "Michael Oborne"; }
        }

        private KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>>? sub = null;

        public override bool Init()
        {
            // change to enable
            return false;
        }

        public override bool Loaded()
        {
            gen = new generator();
            // force it to the top of the container to push down the tabcontrol
            gen.Dock = DockStyle.Top;
            FlightData.instance.tabControlactions.Parent.Controls.Add(gen);

            return true;
        }

        public override bool Loop()
        {

            if (MainV2.comPort.BaseStream.IsOpen)
            {
                if (sub == null)
                    sub = MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.GENERATOR_STATUS, message =>
                    {
                        var gen = (MAVLink.mavlink_generator_status_t) message.data;
                        status = gen.status;
                        generator_speed = gen.generator_speed;
                        load_current = gen.load_current;
                        bus_voltage = gen.bus_voltage;
                        return true;
                    });

                MainV2.instance.Invoke((MethodInvoker) delegate
                {
                    gen.aGaugeSpeed.Value1 = (float)(generator_speed/1000.0);
                    //uint min = (run_time & 0x0000FFFF) / 256;
                    //uint hour = ((run_time & 0xFFFF0000) >> 16);
                    //runTimeTxt.Text = hour.ToString("D4") + ":" + min.ToString("D2");
                    //uint nhour = next_time / 3600;
                    //nextMainTimeTxt.Text = nhour.ToString("D4");
                    if (status == 0)
                    {
                        gen.runStatusTxt.Text = "OK";
                    }
                    else
                    {
                        gen.runStatusTxt.Text = "ERROR";
                        string cul = System.Globalization.CultureInfo.InstalledUICulture.Name;
                        if (cul.ToLower() == "zh-cn")
                        {
                            if ((status & 0x00000001) > 0)
                            {
                                gen.runStatusTxt.Text += " 请保养";
                            }

                            if ((status & 0x00000002) > 0)
                            {
                                gen.runStatusTxt.Text += " 锁机";
                            }

                            if ((status & 0x00000004) > 0)
                            {
                                gen.runStatusTxt.Text += " 过载";
                            }

                            if ((status & 0x00000008) > 0)
                            {
                                gen.runStatusTxt.Text += " 发电电压低";
                            }

                            if ((status & 0x00000010) > 0)
                            {
                                gen.runStatusTxt.Text += " 电池电压低";
                            }
                        }
                        else
                        {
                            if ((status & 0x00000001) > 0)
                            {
                                gen.runStatusTxt.Text += " maintenance required";
                            }

                            if ((status & 0x00000002) > 0)
                            {
                                gen.runStatusTxt.Text += " start disenabled";
                            }

                            if ((status & 0x00000004) > 0)
                            {
                                gen.runStatusTxt.Text += " overload";
                            }

                            if ((status & 0x00000008) > 0)
                            {
                                gen.runStatusTxt.Text += " low voltage output";
                            }

                            if ((status & 0x00000010) > 0)
                            {
                                gen.runStatusTxt.Text += " battery low voltage";
                            }
                        }
                    }

                    gen.voltageTxt.Text = ((float)bus_voltage) / 100.0f + " V";
                    gen.powerTxt.Text =
                        (((float)load_current) / 100.0f *
                            ((float)bus_voltage) / 100.0f).ToString("F2") + "W";
                    switch (status)
                    {
                        case 0:
                            gen.statusTxt.Text = "IDLE";
                            break;
                        case 1:
                            gen.statusTxt.Text = "RUN";
                            break;
                        case 2:
                            gen.statusTxt.Text = "CHARGE";
                            break;
                        case 3:
                            gen.statusTxt.Text = "BALANCE";
                            break;

                    }
                });
            }
            else
            {
                if (sub != null)
                    MainV2.comPort.UnSubscribeToPacketType(sub.Value);
            }

            return true;
        }

        ulong status;
        private ushort generator_speed;
        private float load_current;
        private float bus_voltage;
        private generator gen;

        public override bool Exit()
        {
            return true;
        }
    }
}