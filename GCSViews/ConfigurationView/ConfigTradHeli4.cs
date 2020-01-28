using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigTradHeli4 : UserControl, IActivate, IDeactivate
    {
        public ConfigTradHeli4()
        {
            InitializeComponent();
        }

        public class ItemInfo
        {
            public string name { get; set; }
            public uitype type { get; set; }
        }

        public void Activate()
        {
            this.Visible = false;

            setup(mavlinkCheckBoxrev1, mavlinkComboBoxfunc1, mavlinkNumericUpDownmin1, mavlinkNumericUpDowntrim1,
                mavlinkNumericUpDownmax1, 1);
            setup(mavlinkCheckBoxrev2, mavlinkComboBoxfunc2, mavlinkNumericUpDownmin2, mavlinkNumericUpDowntrim2,
                mavlinkNumericUpDownmax2, 2);
            setup(mavlinkCheckBoxrev3, mavlinkComboBoxfunc3, mavlinkNumericUpDownmin3, mavlinkNumericUpDowntrim3,
                mavlinkNumericUpDownmax3, 3);
            setup(mavlinkCheckBoxrev4, mavlinkComboBoxfunc4, mavlinkNumericUpDownmin4, mavlinkNumericUpDowntrim4,
                mavlinkNumericUpDownmax4, 4);
            setup(mavlinkCheckBoxrev5, mavlinkComboBoxfunc5, mavlinkNumericUpDownmin5, mavlinkNumericUpDowntrim5,
                mavlinkNumericUpDownmax5, 5);
            setup(mavlinkCheckBoxrev6, mavlinkComboBoxfunc6, mavlinkNumericUpDownmin6, mavlinkNumericUpDowntrim6,
                mavlinkNumericUpDownmax6, 6);
            setup(mavlinkCheckBoxrev7, mavlinkComboBoxfunc7, mavlinkNumericUpDownmin7, mavlinkNumericUpDowntrim7,
                mavlinkNumericUpDownmax7, 7);
            setup(mavlinkCheckBoxrev8, mavlinkComboBoxfunc8, mavlinkNumericUpDownmin8, mavlinkNumericUpDowntrim8,
                mavlinkNumericUpDownmax8, 8);

            TableLayoutPanel current = null;

            Func<ItemInfo, int, object> populatetable = (a, index) =>
            {
                var name = ParameterMetaDataRepository.GetParameterMetaData(a.name,
                    ParameterMetaDataConstants.DisplayName, MainV2.comPort.MAV.cs.firmware.ToString());
                var unit = ParameterMetaDataRepository.GetParameterMetaData(a.name,
                    ParameterMetaDataConstants.Units, MainV2.comPort.MAV.cs.firmware.ToString());
                var desc = ParameterMetaDataRepository.GetParameterMetaData(a.name,
                    ParameterMetaDataConstants.Description, MainV2.comPort.MAV.cs.firmware.ToString());

                var label = new Label()
                {
                    Text = (name != "" ? name: a.name) + (unit != "" ? " (" + unit + ")" : ""),
                    AutoSize = true
                   
                };
                current.Controls.Add(label, 0, index);
                if (a.type == uitype.Combo)
                {
                    var ctl = new MavlinkComboBox() { Padding = new Padding(4)};
                    current.Controls.Add(ctl, 1, index);
                    ctl.setup(new[] { a.name }, MainV2.comPort.MAV.param);
                    toolTip1.SetToolTip(ctl, desc);
                }
                else if (a.type == uitype.Num)
                {
                    var ctl = new MavlinkNumericUpDown() { Padding = new Padding(4)};
                    current.Controls.Add(ctl, 1, index);
                    ctl.setup(0, 0, 1, 1, new[] { a.name }, MainV2.comPort.MAV.param);
                    toolTip1.SetToolTip(ctl, desc);
                }

                toolTip1.SetToolTip(label, desc);

                return null;
            };

            {
                var swashplatelist = new[]
                {
                    new ItemInfo {name = "H_SV_MAN", type = uitype.Combo},
                    new ItemInfo {name = "H_SW_TYPE", type = uitype.Combo},
                    new ItemInfo {name = "H_SW_COL_DIR", type = uitype.Combo},
                    new ItemInfo {name = "H_SW_LIN_SVO", type = uitype.Combo},
                    new ItemInfo {name = "H_FLYBAR_MODE", type = uitype.Combo},
                    new ItemInfo {name = "H_COL_MAX", type = uitype.Num},
                    new ItemInfo {name = "H_COL_MID", type = uitype.Num},
                    new ItemInfo {name = "H_COL_MIN", type = uitype.Num},
                    new ItemInfo {name = "H_CYC_MAX", type = uitype.Num},
                };

                current = tableLayoutPanel5;
                current.RowCount = swashplatelist.Length;
                swashplatelist.Select(populatetable).ToList();
            }

            {
                var throttlelist = new[]
                {
                    new ItemInfo {name = "H_RSC_MODE", type = uitype.Combo},
                    new ItemInfo {name = "H_RSC_CRITICAL", type = uitype.Num},
                    new ItemInfo {name = "H_RSC_RAMP_TIME", type = uitype.Num},
                    new ItemInfo {name = "H_RSC_RUNUP_TIME", type = uitype.Num},
                    new ItemInfo {name = "H_RSC_SETPOINT", type = uitype.Num},
                    new ItemInfo {name = "H_RSC_IDLE", type = uitype.Num},
                    new ItemInfo {name = "H_RSC_THRCRV_0", type = uitype.Num},
                    new ItemInfo {name = "H_RSC_THRCRV_25", type = uitype.Num},
                    new ItemInfo {name = "H_RSC_THRCRV_50", type = uitype.Num},
                    new ItemInfo {name = "H_RSC_THRCRV_75", type = uitype.Num},
                    new ItemInfo {name = "H_RSC_THRCRV_100", type = uitype.Num},
                };

                current = tableLayoutPanel3;
                current.RowCount = throttlelist.Length;
                throttlelist.Select(populatetable).ToList();

            }
            {
                var governor = new[]
                {
                    new ItemInfo {name = "H_RSC_GOV_SETPNT", type = uitype.Num},
                    new ItemInfo {name = "H_RSC_GOV_DISGAG", type = uitype.Num},
                    new ItemInfo {name = "H_RSC_GOV_DROOP", type = uitype.Num},
                    new ItemInfo {name = "H_RSC_GOV_TCGAIN", type = uitype.Num},
                    new ItemInfo {name = "H_RSC_GOV_RANGE", type = uitype.Num},
                };

                current = tableLayoutPanel2;
                current.RowCount = governor.Length;
                governor.Select(populatetable).ToList();
            }
            {
                var misc = new[]
                {
                    new ItemInfo {name = "IM_STB_COL_1", type = uitype.Num},
                    new ItemInfo {name = "IM_STB_COL_2", type = uitype.Num},
                    new ItemInfo {name = "IM_STB_COL_3", type = uitype.Num},
                    new ItemInfo {name = "IM_STB_COL_4", type = uitype.Num},
                
                    new ItemInfo {name = "H_TAIL_TYPE", type = uitype.Combo},
                    new ItemInfo {name = "H_TAIL_SPEED", type = uitype.Num},
                    new ItemInfo {name = "H_GYR_GAIN", type = uitype.Num},
                    new ItemInfo {name = "H_GYR_GAIN_ACRO", type = uitype.Num},
                    new ItemInfo {name = "H_COLYAW", type = uitype.Num},
                 
                };

                current = tableLayoutPanel1;
                current.RowCount = misc.Length;
                misc.Select(populatetable).ToList();
            }

            this.Visible = true;
        }

        private void setup(MavlinkCheckBox rev1, MavlinkComboBox func1,
            MavlinkNumericUpDown min1, MavlinkNumericUpDown trim1, MavlinkNumericUpDown max1, int servono)
        {
            var servo = String.Format("SERVO{0}", servono);

            rev1.setup(1, 0, servo + "_REVERSED", MainV2.comPort.MAV.param);
            func1.setup(ParameterMetaDataRepository.GetParameterOptionsInt(servo + "_FUNCTION",
                    MainV2.comPort.MAV.cs.firmware.ToString()), servo + "_FUNCTION", MainV2.comPort.MAV.param);
            min1.setup(800, 2200, 1, 1, servo + "_MIN", MainV2.comPort.MAV.param);
            trim1.setup(800, 2200, 1, 1, servo + "_TRIM", MainV2.comPort.MAV.param);
            max1.setup(800, 2200, 1, 1, servo + "_MAX", MainV2.comPort.MAV.param);
        }

        public void Deactivate()
        {
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel2.Controls.Clear();
            tableLayoutPanel3.Controls.Clear();
            tableLayoutPanel5.Controls.Clear();
        }
    }

    public enum uitype  
    {
        Combo,
        Num,
        Check,
        Mask
    }
}
