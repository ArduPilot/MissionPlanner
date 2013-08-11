using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArdupilotMega.Controls;
using ArdupilotMega.Controls.BackstageView;
using ArdupilotMega.Utilities;
using System.Globalization;

namespace ArdupilotMega.Wizard
{
    public partial class _12FailSafe : MyUserControl, IWizard, IDeactivate, IActivate
    {
        string[] ac_failsafe = { "FS_BATT_ENABLE", "FS_GPS_ENABLE", "FS_GCS_ENABLE", "FS_THR_ENABLE", "FS_THR_VALUE" };
        string[] ap_failsafe = { "THR_FAILSAFE", "THR_FS_VALUE", "FS_BATT_VOLTAGE", "FS_BATT_MAH", "FS_GCS_ENABL", "FS_SHORT_ACTN", "FS_SHORT_TIMEOUT", "FS_LONG_ACTN", "FS_LONG_TIMEOUT" };

        public _12FailSafe()
        {
            InitializeComponent();
         }

        public void Activate()
        {
            flowLayoutPanel1.Controls.Clear();

            if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2)
            {
                foreach (var item in ac_failsafe)
                {
                    createValueControl(new KeyValuePair<string,string>(item,MainV2.comPort.param[item].ToString()));
                }
            }
            else if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduPlane)
            {
                foreach (var item in ap_failsafe)
                {
                    createValueControl(new KeyValuePair<string, string>(item, ""));
                }
            }
        }

        void createValueControl(KeyValuePair<string,string> x)
        {
            ParameterMetaDataRepository _parameterMetaDataRepository = new ParameterMetaDataRepository();

            string value = ((float)MainV2.comPort.MAV.param[x.Key]).ToString("0.###", CultureInfo.InvariantCulture);
            string description = _parameterMetaDataRepository.GetParameterMetaData(x.Key, ParameterMetaDataConstants.Description);
            string displayName = _parameterMetaDataRepository.GetParameterMetaData(x.Key, ParameterMetaDataConstants.DisplayName) +" (" + x.Key + ")";
            string units = _parameterMetaDataRepository.GetParameterMetaData(x.Key, ParameterMetaDataConstants.Units);

            var valueControl = new ValuesControl();
            valueControl.Name = x.Key;
            valueControl.DescriptionText = FitDescriptionText(units, description);
            valueControl.LabelText = displayName;

            ThemeManager.ApplyThemeTo(valueControl);

            valueControl.ComboBoxControl.DisplayMember = "Value";
            valueControl.ComboBoxControl.ValueMember = "Key";
            valueControl.ComboBoxControl.DataSource = _parameterMetaDataRepository.GetParameterOptionsInt(x.Key);
            valueControl.ComboBoxControl.SelectedItem = value;

            valueControl.ValueChanged += valueControl_ValueChanged;

            flowLayoutPanel1.Controls.Add(valueControl);
        }

        void valueControl_ValueChanged(object sender, string Name, string Value)
        {
            MainV2.comPort.setParam(Name, float.Parse(Value, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Fits the description text.
        /// </summary>
        /// <param name="units">The units.</param>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        private string FitDescriptionText(string units, string description)
        {
            var returnDescription = new StringBuilder();

            if (!String.IsNullOrEmpty(units))
            {
                returnDescription.Append(String.Format("Units: {0}{1}", units, Environment.NewLine));
            }

            if (!String.IsNullOrEmpty(description))
            {
                returnDescription.Append("Description: ");
                var descriptionParts = description.Split(new char[] { ' ' });
                for (int i = 0; i < descriptionParts.Length; i++)
                {
                    returnDescription.Append(String.Format("{0} ", descriptionParts[i]));
                    if (i != 0 && i % 12 == 0) returnDescription.Append(Environment.NewLine);
                }
            }

            return returnDescription.ToString();
        }


        public void Deactivate()
        {
           
        }

        public int WizardValidate()
        {
            return 1;
        }
    }
}
