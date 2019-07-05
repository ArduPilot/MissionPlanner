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
    public partial class ConfigUserDefined : MyUserControl, IActivate, IDeactivate
    {
        public ConfigUserDefined()
        {
            InitializeComponent();

            if (Settings.Instance.ContainsKey("UserParams"))
                Options = Settings.Instance["UserParams"].Split(',');
        }

        public string[] Options { get; set; } = new string[]
        {
            "CH6_OPT",
            "CH7_OPT",
            "CH8_OPT",
            "CH9_OPT",
            "CH10_OPT",
            "CH11_OPT",
            "CH12_OPT",
            "CH13_OPT",
            "CH14_OPT"
        };

        public void LoadOptions()
        {
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowCount = 0;

            foreach (var option in Options)
            {
                tableLayoutPanel1.RowCount++;
                tableLayoutPanel1.Controls.Add(new Label() {Text = option, Name = option});
                var cmb = new MavlinkComboBox();
                cmb.setup(
                    ParameterMetaDataRepository.GetParameterOptionsInt(option,
                        MainV2.comPort.MAV.cs.firmware.ToString()), option, MainV2.comPort.MAV.param);
                tableLayoutPanel1.Controls.Add(cmb);
            }

            tableLayoutPanel1.ResumeLayout(true);
        }

        public void Activate()
        {
            LoadOptions();
        }

        public void Deactivate()
        {
            
        }
    }
}
