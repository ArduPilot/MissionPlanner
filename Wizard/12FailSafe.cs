using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.Controls.BackstageView;
using MissionPlanner.Utilities;
using System.Globalization;

namespace MissionPlanner.Wizard
{
    public partial class _12FailSafe : MyUserControl, IWizard, IDeactivate, IActivate
    {
        string[] ac_failsafe = {"FS_BATT_ENABLE", "FS_GPS_ENABLE", "FS_GCS_ENABLE", "FS_THR_ENABLE", "FS_THR_VALUE"};

        string[] ap_failsafe =
        {
            "THR_FAILSAFE", "THR_FS_VALUE", "FS_BATT_VOLTAGE", "FS_BATT_MAH", "FS_GCS_ENABL",
            "FS_SHORT_ACTN", "FS_SHORT_TIMEOUT", "FS_LONG_ACTN", "FS_LONG_TIMEOUT"
        };

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
                    try
                    {
                        createValueControl(new KeyValuePair<string, string>(item,
                            MainV2.comPort.MAV.param[item].ToString()));
                    }
                    catch
                    {
                    }
                }
            }
            else if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduPlane)
            {
                foreach (var item in ap_failsafe)
                {
                    try
                    {
                        createValueControl(new KeyValuePair<string, string>(item, ""));
                    }
                    catch
                    {
                    }
                }
            }
        }

        void createValueControl(KeyValuePair<string, string> x)
        {
            string value = ((float) MainV2.comPort.MAV.param[x.Key]).ToString("0.###", CultureInfo.InvariantCulture);
            string description = ParameterMetaDataRepository.GetParameterMetaData(x.Key,
                ParameterMetaDataConstants.Description, MainV2.comPort.MAV.cs.firmware.ToString());
            string displayName =
                ParameterMetaDataRepository.GetParameterMetaData(x.Key, ParameterMetaDataConstants.DisplayName,
                    MainV2.comPort.MAV.cs.firmware.ToString()) + " (" + x.Key + ")";
            string units = ParameterMetaDataRepository.GetParameterMetaData(x.Key, ParameterMetaDataConstants.Units,
                MainV2.comPort.MAV.cs.firmware.ToString());
            string rangeRaw = ParameterMetaDataRepository.GetParameterMetaData(x.Key, ParameterMetaDataConstants.Range,
                MainV2.comPort.MAV.cs.firmware.ToString());
            string incrementRaw = ParameterMetaDataRepository.GetParameterMetaData(x.Key,
                ParameterMetaDataConstants.Increment, MainV2.comPort.MAV.cs.firmware.ToString());
            string availableValuesRaw = ParameterMetaDataRepository.GetParameterMetaData(x.Key,
                ParameterMetaDataConstants.Values, MainV2.comPort.MAV.cs.firmware.ToString());

            float displayscale = 1;

            float increment, intValue;
            float.TryParse(incrementRaw, NumberStyles.Float, CultureInfo.InvariantCulture, out increment);
            // this is in local culture
            float.TryParse(value, out intValue);

            string[] rangeParts = rangeRaw.Split(new[] {' '});
            if (rangeParts.Count() == 2 && increment > 0)
            {
                float lowerRange;
                float.TryParse(rangeParts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out lowerRange);
                float upperRange;
                float.TryParse(rangeParts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out upperRange);

                var valueControl = new RangeControl(x.Key, FitDescriptionText(units, description), displayName,
                    increment, displayscale, lowerRange, upperRange, value);

                ThemeManager.ApplyThemeTo(valueControl);

                valueControl.ValueChanged += valueControl_ValueChanged;

                flowLayoutPanel1.Controls.Add(valueControl);
            }
            else if (availableValuesRaw.Length > 0)
            {
                var valueControl = new ValuesControl();
                valueControl.Name = x.Key;
                valueControl.DescriptionText = FitDescriptionText(units, description);
                valueControl.LabelText = displayName;

                ThemeManager.ApplyThemeTo(valueControl);

                valueControl.ComboBoxControl.DisplayMember = "Value";
                valueControl.ComboBoxControl.ValueMember = "Key";
                valueControl.ComboBoxControl.DataSource = ParameterMetaDataRepository.GetParameterOptionsInt(x.Key,
                    MainV2.comPort.MAV.cs.firmware.ToString());
                valueControl.ComboBoxControl.SelectedItem = value;

                valueControl.ValueChanged += valueControl_ValueChanged;

                flowLayoutPanel1.Controls.Add(valueControl);
            }
            else
            {
                Console.WriteLine("No valid param metadata for " + x.Key);
            }
        }

        void valueControl_ValueChanged(object sender, string Name, string Value)
        {
            try
            {
                MainV2.comPort.setParam(Name, float.Parse(Value));
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.ErrorSettingParameter + Name + "\n" + ex.ToString(), Strings.ERROR);
            }
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
                var descriptionParts = description.Split(new char[] {' '});
                for (int i = 0; i < descriptionParts.Length; i++)
                {
                    returnDescription.Append(String.Format("{0} ", descriptionParts[i]));
                    if (i != 0 && i%12 == 0) returnDescription.Append(Environment.NewLine);
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

        public bool WizardBusy()
        {
            return false;
        }
    }
}