﻿using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System;
using System.Linq;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigRadioOutput : MyUserControl, IActivate, IDeactivate
    {
        public ConfigRadioOutput()
        {
            InitializeComponent();

            bindingSource1.DataSource = typeof(CurrentState);

            SuspendLayout();
            foreach (var i in Enumerable.Range(1, 16))
            {
                setup(i);
            }

            ResumeLayout(true);
        }

        private void setup(int servono)
        {
            var servo = String.Format("SERVO{0}", servono);

            var label = new Label()
                {Text = servono.ToString(), AutoSize = true, TextAlign = System.Drawing.ContentAlignment.MiddleCenter};
            var bAR1 = new HorizontalProgressBar2()
            {
                Minimum = 800, Maximum = 2200, Value = 1500, DrawLabel = true, Name = "BAR" + servono,
                Dock = DockStyle.Fill
            };
            var rev1 = new MissionPlanner.Controls.MavlinkCheckBox()
                {Enabled = false, Dock = DockStyle.Fill, AutoSize = true};
            var func1 = new MavlinkComboBox()
                {Enabled = false, Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList};
            var min1 = new MavlinkNumericUpDown() {Minimum = 800, Maximum = 2200, Value = 1500, Enabled = false};
            var trim1 = new MavlinkNumericUpDown() {Minimum = 800, Maximum = 2200, Value = 1500, Enabled = false};
            var max1 = new MavlinkNumericUpDown() {Minimum = 800, Maximum = 2200, Value = 1500, Enabled = false};

            this.tableLayoutPanel1.Controls.Add(label, 0, servono);
            this.tableLayoutPanel1.Controls.Add(bAR1, 1, servono);
            this.tableLayoutPanel1.Controls.Add(rev1, 2, servono);
            this.tableLayoutPanel1.Controls.Add(func1, 3, servono);
            this.tableLayoutPanel1.Controls.Add(min1, 4, servono);
            this.tableLayoutPanel1.Controls.Add(trim1, 5, servono);
            this.tableLayoutPanel1.Controls.Add(max1, 6, servono);

            bAR1.DataBindings.Add("Value", bindingSource1, "ch" + servono + "out");
            rev1.setup(1, 0, servo + "_REVERSED", MainV2.comPort.MAV.param);
            func1.setup(ParameterMetaDataRepository.GetParameterOptionsInt(servo + "_FUNCTION",
                    MainV2.comPort.MAV.cs.firmware.ToString()), servo + "_FUNCTION", MainV2.comPort.MAV.param);
            min1.setup(800, 2200, 1, 1, servo + "_MIN", MainV2.comPort.MAV.param);
            trim1.setup(800, 2200, 1, 1, servo + "_TRIM", MainV2.comPort.MAV.param);
            max1.setup(800, 2200, 1, 1, servo + "_MAX", MainV2.comPort.MAV.param);
        }

        public void Activate()
        {
            timer1.Start();
        }

        public void Deactivate()
        {
            timer1.Stop();
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                MainV2.comPort.MAV.cs.UpdateCurrentSettings(bindingSource1.UpdateDataSource(MainV2.comPort.MAV.cs));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}