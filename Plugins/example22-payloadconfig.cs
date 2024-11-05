using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.GCSViews;
using MissionPlanner.Plugin;
using log4net;

namespace MissionPlanner.plugins
{
/// <summary>
/// This plugin adds a "Payload Selection" page to Mission Planner's Config tab.
/// It allows users to select various payload configurations, such as gimbals or cameras, 
/// by checking boxes associated with each payload type. Each payload setting adjusts 
/// specific parameters and reverts to defaults when unchecked. Parameter adjustments
/// are only allowed while the vehicle is disarmed. 
/// 
/// ### WARNING:
/// This is a simple example plugin with minimal testing and limited error handling. 
/// It is recommended to review and test this code thoroughly in a safe environment 
/// before relying on it in operational settings.
///
/// ### Modifying the Plugin:
/// - To enable this plugin, add payloads to the `payloadParameters` dictionary. If
///   the dictionary is empty, the plugin will not initialize.
/// - To add new payloads, modify the `payloadParameters` dictionary with the desired 
///   payload name as the key and a dictionary of parameter settings as the value.
/// - Example: Adding a new payload:
///       { "NewPayload", new Dictionary<string, double> { { "PARAM_NAME", value } } }
/// - Each payload’s parameters are activated when checked and reverted to their default 
///   when unchecked, if defaults exist.
/// </summary>
    public class example22_payloadconfig : Plugin.Plugin
    {
        public override string Name => "Payload Select Page";
        public override string Version => "0.1";
        public override string Author => "Bob Long";


        // DEFINE YOUR PAYLOADS, AND THEIR PARAMETERS, HERE
        public static readonly Dictionary<string, Dictionary<string, double>> payloadParameters = new Dictionary<string, Dictionary<string, double>>
        {
            // {
            //     "Gimbal", new Dictionary<string, double>
            //     {
            //         { "MNT1_TYPE", 9},          // Set Mount1 Type to Scripting
            //     }
            // },
            // {
            //     "Mapping Camera", new Dictionary<string, double>
            //     {
            //         { "CAM_TRIGG_TYPE", 1 },     // Set trigger type to PWM
            //         { "SERVO9_FUNCTION", 10 },   // Set servo 9 to camera trigger
            //     }
            // }
        };

        private CheckedListBox payloadCheckboxList;

        public override bool Init()
        {
            // Only initialize the plugin if payloads have been configured.
            return payloadParameters.Count > 0;
        }

        public override bool Loaded()
        {
            // Register the ConfigPayload view on the Config tab
            SoftwareConfig.AddPluginViewPage(typeof(ConfigPayload), "Payload Selection", SoftwareConfig.pageOptions.isConnected | SoftwareConfig.pageOptions.gotAllParams);

            return true;
        }

        public override bool Exit() { return true; }
    }

    public class ConfigPayload : MyUserControl, IActivate
    {
        private Dictionary<string, Dictionary<string, double>> payloadParameters = example22_payloadconfig.payloadParameters;
        private readonly CheckedListBox payloadCheckboxList;
        private readonly static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes the ConfigPayload control with a checklist of available payload selections.
        /// </summary>
        public ConfigPayload()
        {
            // Initialize CheckedListBox for payload selection
            payloadCheckboxList = new CheckedListBox
            {
                CheckOnClick = true,
                Dock = DockStyle.Left,
            };

            // Add payload options as checkboxes
            foreach (var payload in payloadParameters.Keys)
            {
                payloadCheckboxList.Items.Add(payload);
            }

            // Handle check state changes
            payloadCheckboxList.ItemCheck += PayloadCheckboxList_ItemCheck;

            // Add the CheckedListBox to the control
            Controls.Add(payloadCheckboxList);
        }

        /// <summary>
        /// Called when the ConfigPayload view is activated.
        /// </summary>
        public void Activate()
        {
            UpdateCheckboxState();
            CheckArmed();
        }

        /// <summary>
        /// Checks if the vehicle is armed, disables the checkbox list and warns the user if it is.
        /// </summary>
        private void CheckArmed()
        {
            payloadCheckboxList.Enabled = !MainV2.comPort.MAV.cs.armed;
            if (MainV2.comPort.MAV.cs.armed)
            {
                CustomMessageBox.Show("The vehicle is armed. Payload selection is disabled.", "Payload Selection");
            }
        }

        /// <summary>
        /// Updates the checked state of each checkbox based on current parameter values.
        /// If all parameters for a payload match the predefined values, the checkbox is checked.
        /// </summary>
        private void UpdateCheckboxState()
        {
            for (int i = 0; i < payloadCheckboxList.Items.Count; i++)
            {
                string payload = payloadCheckboxList.Items[i].ToString();
                if (IsPayloadActive(payloadParameters[payload]))
                {
                    payloadCheckboxList.SetItemChecked(i, true);
                }
            }
        }

        /// <summary>
        /// Checks if all parameters in the vehicle match a given dictionary.
        /// </summary>
        /// <param name="parameters">Dictionary of parameter names and values</param>
        /// <returns>True if all parameters match, false otherwise.</returns>
        private bool IsPayloadActive(Dictionary<string, double> parameters)
        {
            foreach (var param in parameters)
            {
                if (!MainV2.comPort.MAV.param.ContainsKey(param.Key))
                {
                    log.Warn($"Parameter {param.Key} not found for comparison.");
                    return false;
                }
                if (!ParamEquality(MainV2.comPort.MAV.param[param.Key].Value, param.Value))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Handles checkbox state changes, applying or reverting parameters based on the checked state.
        /// </summary>
        private void PayloadCheckboxList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            CheckArmed();

            string payload = payloadCheckboxList.Items[e.Index].ToString();
            var parameters = payloadParameters[payload];

            if (e.NewValue == CheckState.Checked)
            {
                ApplyPayloadParameters(parameters);
            }
            else
            {
                RevertParametersToDefault(parameters);
            }
        }

        /// <summary>
        /// Applies the specified parameters.
        /// </summary>
        /// <param name="parameters">Dictionary of parameter names and values to set.</param>
        private void ApplyPayloadParameters(Dictionary<string, double> parameters)
        {
            foreach (var param in parameters)
            {
                MainV2.comPort.setParam(param.Key, param.Value);
            }

            if (CheckParamCountChanged() && parameters.Count > 1)
            {
                // Apply parameters again (we assume there will not be nested enable params for this plugin)
                foreach (var param in parameters)
                {
                    MainV2.comPort.setParam(param.Key, param.Value);
                }
            }
        }

        /// <summary>
        /// Checks if the parameter count has changed, which might indicate new parameters were added.
        /// If so, triggers a parameter list refresh and logs the update.
        /// </summary>
        private bool CheckParamCountChanged()
        {
            if (MainV2.comPort.MAV.param.TotalReceived != MainV2.comPort.MAV.param.TotalReported)
            {
                CustomMessageBox.Show("The number of available parameters changed. A full param refresh will be done.", "Params");
                try
                {
                    MainV2.comPort.getParamList();
                }
                catch (Exception ex)
                {
                    log.Error("Exception getting param list", ex);
                    CustomMessageBox.Show(Strings.ErrorReceivingParams, Strings.ERROR);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Reverts parameters for an unchecked payload to their default values.
        /// </summary>
        /// <param name="parameters">Dictionary of parameter names and expected default values.</param>
        private void RevertParametersToDefault(Dictionary<string, double> parameters)
        {
            foreach (var param in parameters)
            {
                if (!MainV2.comPort.MAV.param.ContainsKey(param.Key))
                {
                    log.Warn($"Parameter {param.Key} missing during revert to default.");
                    continue;
                }
                if (MainV2.comPort.MAV.param[param.Key].default_value == null)
                {
                    log.Warn($"No default value found for parameter {param.Key}");
                    continue;
                }
                // This doesn't technically reset params if they get hidden after an enable param changes,
                // but that is okay for our purposes.
                MainV2.comPort.setParam(param.Key, MainV2.comPort.MAV.param[param.Key].default_value.Value);
            }

            CheckParamCountChanged();
        }

        /// <summary>
        /// Checks if two parameter values are equal within a small tolerance.
        /// </summary>
        private bool ParamEquality(double a, double b)
        {
            return Math.Abs(a - b) < Math.Max(1e-6, Math.Max(Math.Abs(a), Math.Abs(b)) * 1e-6);
        }
    }
}
