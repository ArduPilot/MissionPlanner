using MissionPlanner;
using MissionPlanner.GCSViews;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static MAVLink;

namespace generator
{
    public class generator : MyUserControl
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
            this.powerTxt = new System.Windows.Forms.Label();
            this.voltageTxt = new System.Windows.Forms.Label();
            this.runStatusTxt = new System.Windows.Forms.Label();
            this.powerLabel = new System.Windows.Forms.Label();
            this.voltageLabel = new System.Windows.Forms.Label();
            this.runstatusLabel = new System.Windows.Forms.Label();
            this.runTimeTxt = new System.Windows.Forms.Label();
            this.nextMainTimeTxt = new System.Windows.Forms.Label();
            this.runTimeLabel = new System.Windows.Forms.Label();
            this.nextMainTimeLabel = new System.Windows.Forms.Label();
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
            this.splitContainer3.Panel2.Controls.Add(this.powerTxt);
            this.splitContainer3.Panel2.Controls.Add(this.voltageTxt);
            this.splitContainer3.Panel2.Controls.Add(this.runStatusTxt);
            this.splitContainer3.Panel2.Controls.Add(this.powerLabel);
            this.splitContainer3.Panel2.Controls.Add(this.voltageLabel);
            this.splitContainer3.Panel2.Controls.Add(this.runstatusLabel);
            this.splitContainer3.Panel2.Controls.Add(this.runTimeTxt);
            this.splitContainer3.Panel2.Controls.Add(this.nextMainTimeTxt);
            this.splitContainer3.Panel2.Controls.Add(this.runTimeLabel);
            this.splitContainer3.Panel2.Controls.Add(this.nextMainTimeLabel);
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
            this.aGaugeSpeed.Dock = System.Windows.Forms.DockStyle.Fill;
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
            this.aGaugeSpeed.Size = new System.Drawing.Size(150, 150);
            this.aGaugeSpeed.TabIndex = 0;
            this.aGaugeSpeed.Value = 0F;
            this.aGaugeSpeed.Value0 = 0F;
            this.aGaugeSpeed.Value1 = 0F;
            this.aGaugeSpeed.Value2 = 0F;
            this.aGaugeSpeed.Value3 = 0F;
            // 
            // powerTxt
            // 
            this.powerTxt.AutoSize = true;
            this.powerTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.powerTxt.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.powerTxt.Location = new System.Drawing.Point(123, 83);
            this.powerTxt.Name = "powerTxt";
            this.powerTxt.Size = new System.Drawing.Size(39, 15);
            this.powerTxt.TabIndex = 10;
            this.powerTxt.Text = "100W";
            // 
            // voltageTxt
            // 
            this.voltageTxt.AutoSize = true;
            this.voltageTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.voltageTxt.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.voltageTxt.Location = new System.Drawing.Point(123, 60);
            this.voltageTxt.Name = "voltageTxt";
            this.voltageTxt.Size = new System.Drawing.Size(28, 15);
            this.voltageTxt.TabIndex = 9;
            this.voltageTxt.Text = "25V";
            // 
            // runStatusTxt
            // 
            this.runStatusTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.runStatusTxt.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.runStatusTxt.Location = new System.Drawing.Point(75, 5);
            this.runStatusTxt.Name = "runStatusTxt";
            this.runStatusTxt.Size = new System.Drawing.Size(130, 55);
            this.runStatusTxt.TabIndex = 8;
            this.runStatusTxt.Text = "OK";
            // 
            // powerLabel
            // 
            this.powerLabel.AutoSize = true;
            this.powerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.powerLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.powerLabel.Location = new System.Drawing.Point(7, 83);
            this.powerLabel.Name = "powerLabel";
            this.powerLabel.Size = new System.Drawing.Size(42, 15);
            this.powerLabel.TabIndex = 4;
            this.powerLabel.Text = "Power";
            // 
            // voltageLabel
            // 
            this.voltageLabel.AutoSize = true;
            this.voltageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.voltageLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.voltageLabel.Location = new System.Drawing.Point(7, 60);
            this.voltageLabel.Name = "voltageLabel";
            this.voltageLabel.Size = new System.Drawing.Size(48, 15);
            this.voltageLabel.TabIndex = 3;
            this.voltageLabel.Text = "Voltage";
            // 
            // runstatusLabel
            // 
            this.runstatusLabel.AutoSize = true;
            this.runstatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.runstatusLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.runstatusLabel.Location = new System.Drawing.Point(7, 5);
            this.runstatusLabel.Name = "runstatusLabel";
            this.runstatusLabel.Size = new System.Drawing.Size(67, 15);
            this.runstatusLabel.TabIndex = 2;
            this.runstatusLabel.Text = "Run Status";
            // 
            // runTimeTxt
            // 
            this.runTimeTxt.AutoSize = true;
            this.runTimeTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.runTimeTxt.Location = new System.Drawing.Point(123, 104);
            this.runTimeTxt.Name = "runTimeTxt";
            this.runTimeTxt.Size = new System.Drawing.Size(31, 15);
            this.runTimeTxt.TabIndex = 0;
            this.runTimeTxt.Text = "0:00";
            // 
            // nextMainTimeTxt
            // 
            this.nextMainTimeTxt.AutoSize = true;
            this.nextMainTimeTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nextMainTimeTxt.Location = new System.Drawing.Point(123, 127);
            this.nextMainTimeTxt.Name = "nextMainTimeTxt";
            this.nextMainTimeTxt.Size = new System.Drawing.Size(14, 15);
            this.nextMainTimeTxt.TabIndex = 0;
            this.nextMainTimeTxt.Text = "0";
            // 
            // runTimeLabel
            // 
            this.runTimeLabel.AutoSize = true;
            this.runTimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.runTimeLabel.Location = new System.Drawing.Point(7, 104);
            this.runTimeLabel.Name = "runTimeLabel";
            this.runTimeLabel.Size = new System.Drawing.Size(101, 15);
            this.runTimeLabel.TabIndex = 0;
            this.runTimeLabel.Text = "Run Time(h:mm)";
            // 
            // nextMainTimeLabel
            // 
            this.nextMainTimeLabel.AutoSize = true;
            this.nextMainTimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nextMainTimeLabel.Location = new System.Drawing.Point(7, 127);
            this.nextMainTimeLabel.Name = "nextMainTimeLabel";
            this.nextMainTimeLabel.Size = new System.Drawing.Size(84, 15);
            this.nextMainTimeLabel.TabIndex = 0;
            this.nextMainTimeLabel.Text = "Next Maint (h)";
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
        internal System.Windows.Forms.Label powerTxt;
        internal System.Windows.Forms.Label voltageTxt;
        internal System.Windows.Forms.Label runStatusTxt;

        private System.Windows.Forms.Label runTimeLabel;
        private System.Windows.Forms.Label nextMainTimeLabel;
        internal System.Windows.Forms.Label runTimeTxt;
        internal System.Windows.Forms.Label nextMainTimeTxt;


        public class Plugin : MissionPlanner.Plugin.Plugin
        {
            public override string Name
            {
                get { return "generator"; }
            }

            public override string Version
            {
                get { return "0.11"; }
            }

            public override string Author
            {
                get { return "Michael Oborne"; }
            }

            private KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>>? sub = null;

            public override bool Init()
            {
                // change to enable
                return true;
            }

            public override bool Loaded()
            {
                gen = new generator();
                // force it to the top of the container to push down the tabcontrol
                gen.Dock = DockStyle.Top;
                

                return true;
            }

            public override bool Loop()
            {

                if (MainV2.comPort.BaseStream.IsOpen)
                {
                    if (sub == null)
                        sub = MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.GENERATOR_STATUS, message =>
                        {
                            MainV2.instance.BeginInvoke((MethodInvoker) delegate
                            {
                                if (!FlightData.instance.tabControlactions.Parent.Controls.Contains(gen))
                                    FlightData.instance.tabControlactions.Parent.Controls.Add(gen);
                            });
                            
                            var genmsg = (MAVLink.mavlink_generator_status_t)message.data;
                            status = genmsg.status;
                            generator_speed = genmsg.generator_speed;
                            load_current = genmsg.load_current;
                            bus_voltage = genmsg.bus_voltage;
                            run_time = genmsg.runtime;
                            timemaint = genmsg.time_until_maintenance;
                            return true;
                        });

                    MainV2.instance.BeginInvoke((MethodInvoker)delegate
                   {
                       gen.aGaugeSpeed.Value1 = (float)(generator_speed / 1000.0);
                       uint min = (run_time) / 60;
                       uint hour = ((run_time) / 3600);
                       gen.runTimeTxt.Text = hour.ToString("D4") + ":" + min.ToString("D2");
                       int nhour = timemaint / 3600;
                       gen.nextMainTimeTxt.Text = nhour.ToString("D4");

                       gen.runStatusTxt.Text = "";
                       for (ulong bitvalue = 1; bitvalue <= (int)MAVLink.MAV_GENERATOR_STATUS_FLAG.IDLE; bitvalue = bitvalue << 1)
                       {
                           ulong currentbit = (status & bitvalue);

                           var currentflag = (MAVLink.MAV_GENERATOR_STATUS_FLAG)Enum.Parse(typeof(MAVLink.MAV_GENERATOR_STATUS_FLAG), bitvalue.ToString());

                           if (currentbit > 0)
                           {
                               gen.runStatusTxt.Text += currentflag.ToString().ToLower() + " ";
                           }
                       }          

                       gen.voltageTxt.Text = ((float)bus_voltage) + " V";
                       gen.powerTxt.Text =
                           (((float)load_current) *
                               ((float)bus_voltage)).ToString("F2") + "W";
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
            private uint run_time;
            private int timemaint;
            private generator gen;

            public override bool Exit()
            {
                return true;
            }
        }
    }
}