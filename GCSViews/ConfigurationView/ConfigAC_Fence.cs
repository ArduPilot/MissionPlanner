using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArdupilotMega.Controls.BackstageView;
using ArdupilotMega.Utilities;

namespace ArdupilotMega.GCSViews.ConfigurationView
{
    public partial class ConfigAC_Fence : UserControl, IActivate
    {
        public ConfigAC_Fence()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            mavlinkCheckBox1.setup(1, 0, "FENCE_ENABLE", MainV2.comPort.MAV.param);

            Utilities.ParameterMetaDataRepository repo = new Utilities.ParameterMetaDataRepository();

            // 1
            string availableValuesRaw = repo.GetParameterMetaData("FENCE_TYPE", ParameterMetaDataConstants.Values);

            string[] availableValues = availableValuesRaw.Split(new[] { ',' },StringSplitOptions.RemoveEmptyEntries);
            if (availableValues.Any())
            {
                var splitValues = new List<KeyValuePair<string, string>>();
                mavlinkComboBox1.setup(splitValues, "FENCE_TYPE", MainV2.comPort.MAV.param);
            }

            // 2
            availableValuesRaw = repo.GetParameterMetaData("FENCE_ACTION", ParameterMetaDataConstants.Values);

            availableValues = availableValuesRaw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (availableValues.Any())
            {
                var splitValues = new List<KeyValuePair<string, string>>();
                // Add the values to the ddl
                foreach (string val in availableValues)
                {
                    string[] valParts = val.Split(new[] { ':' });
                    splitValues.Add(new KeyValuePair<string, string>(valParts[0].Trim(), (valParts.Length > 1) ? valParts[1].Trim() : valParts[0].Trim()));
                };

                mavlinkComboBox2.setup(splitValues, "FENCE_ACTION", MainV2.comPort.MAV.param);
            }

            // 3
            mavlinkNumericUpDown1.setup(1000, 100000, 100, 1, "FENCE_ALT_MAX", MainV2.comPort.MAV.param);

            mavlinkNumericUpDown2.setup(0, 65536, 100, 1, "FENCE_RADIUS", MainV2.comPort.MAV.param);
        }
    }
}
