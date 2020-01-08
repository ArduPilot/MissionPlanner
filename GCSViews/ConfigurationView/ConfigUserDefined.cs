using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System;
using System.Linq;
using System.Windows.Forms;

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
            "CH14_OPT",

            "RC6_OPTION",
            "RC7_OPTION",
            "RC8_OPTION",
            "RC9_OPTION",
            "RC10_OPTION",
            "RC11_OPTION",
            "RC12_OPTION",
            "RC13_OPTION",
            "RC14_OPTION"
        };

        public void LoadOptions()
        {
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowCount = 0;

            var button = new MyButton() { Text = "Modify", Name = "Modify" };
            button.Click += (o, e) =>
            {
                var opts = Options.Aggregate((a, b) => a + "\r\n" + b);
                InputBox.Show("Params", "Enter Param Names", ref opts, false, true);
                Options = opts.Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                Settings.Instance["UserParams"] = Options.Aggregate((a, b) => a.Trim() + "," + b.Trim());
                Activate();
            };
            tableLayoutPanel1.RowCount++;
            tableLayoutPanel1.Controls.Add(button);
            tableLayoutPanel1.SetColumnSpan(button, 2);

            foreach (var option in Options)
            {
                if (!MainV2.comPort.MAV.param.ContainsKey(option))
                    continue;
                tableLayoutPanel1.RowCount++;
                tableLayoutPanel1.Controls.Add(new Label() { Text = option, Name = option });
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
