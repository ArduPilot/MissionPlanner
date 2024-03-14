using MissionPlanner.ArduPilot;
using MissionPlanner.ArduPilot.Mavlink;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigSerial : MyUserControl, IActivate, IDeactivate
    {

        private bool _gotUARTNames = false;
        private bool _gotOptionRules = false;
        private int serialPorts = 0; 
        private Dictionary<int, string> _uartNames = new Dictionary<int, string>();
        private Dictionary<int,SerialOptionRuleItem> _optionRules = new Dictionary<int, SerialOptionRuleItem>();

        private Label noteLabel;

        public ConfigSerial()
        {
            InitializeComponent();
        }

        //This is a long function, but I did not extract methods that are used once only
        //because it is easier to follow the logic this way

        public void Activate()
        {
            //Get OptionRules if not already done
            if (!_gotOptionRules)
            {
                _optionRules.Clear();
                var filename = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "SerialOptionRules.json");
                //Check if the file exist
                if (File.Exists(filename))
                {
                    try
                    {
                        using (StreamReader r = new StreamReader(filename))
                        {
                            string json = r.ReadToEnd();
                            _optionRules = JsonConvert.DeserializeObject<Dictionary<int, SerialOptionRuleItem>>(json);
                        }
                    }
                    catch(Exception ex)
                    {
                        CustomMessageBox.Show("Error reading SerialOptionRules.json file: " + ex.Message);
                    }
                }
                var baudOptions = ParameterMetaDataRepository.GetParameterOptionsInt("SERIAL1_BAUD", MainV2.comPort.MAV.cs.firmware.ToString());
                //sanitize data, check for baudrates 
                foreach (var item in _optionRules)
                {
                    int key = item.Key;
                    int baudrate = item.Value.baudrate;
                    if (baudrate > -1)
                    {
                        if (!baudOptions.Any(pair => pair.Key == baudrate))
                        {
                            _optionRules[key].baudrate = -1;
                        }
                    }
                }
                _gotOptionRules = true;
            }

            //Try to Get UART names from MAVftp, if not already done
            if (!_gotUARTNames)
            {
                _gotUARTNames = true;

                var filename = @"@SYS/uarts.txt";
                MAVFtp _mavftp = new MAVFtp(MainV2.comPort, MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid);
                ProgressReporterDialogue prd = new ProgressReporterDialogue();
                CancellationTokenSource cancel = new CancellationTokenSource();
                MemoryStream ms = new MemoryStream();

                prd.doWorkArgs.CancelRequestChanged += (o, args) =>
                {
                    prd.doWorkArgs.ErrorMessage = "User Cancel";
                    cancel.Cancel();
                    _mavftp.kCmdResetSessions();
                };
                prd.doWorkArgs.ForceExit = false;
                Action<string, int> progress = delegate (string message, int i)
                {
                    prd.UpdateProgressAndStatus(i, "Trying to download uarts.txt\r\nFrom FC");
                };
                _mavftp.Progress += progress;

                prd.DoWork += (iprd) =>
                {
                    try
                    {
                        ms = _mavftp.GetFile(filename, cancel, false);
                    }
                    catch (Exception ex)
                    {
                        //prd.doWorkArgs.ErrorMessage = ex.Message;
                        //fail silently
                    }
                    if (cancel.IsCancellationRequested)
                    {
                        iprd.doWorkArgs.CancelAcknowledged = true;
                        iprd.doWorkArgs.CancelRequested = true;
                        return;
                    }
                };

                prd.RunBackgroundOperationAsync();
                _mavftp.Progress -= progress;

                if (ms.Length > 0)
                {
                    if (_uartNames.Count > 0)
                    {
                        _uartNames.Clear();
                    }

                    //catch any errors that happens when parsing the file
                    try
                    {

                        using (System.IO.StreamReader reader = new System.IO.StreamReader(ms))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                var s = line.Split(' ');
                                if (s.Length >= 2)
                                {
                                    //get the trailing number from the first string
                                    if (s[0].Length >= 7 && s[0].Substring(0, 6) == "SERIAL")
                                    {
                                        var n = 0;
                                        var success = Int32.TryParse(s[0].Substring(6), out n);
                                        if (success)
                                        {
                                            if (n > 0)
                                            {
                                                _uartNames.Add(n, s[1]);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch { } //fail silently
                }
            }

            //find the largest numbered serialx_baud param
            serialPorts = 0;
            foreach (var param in MainV2.comPort.MAV.param.Keys)
            {
                if (param.StartsWith("SERIAL") && param.EndsWith("_BAUD"))
                {
                    int port;
                    Int32.TryParse(param.Substring(6, 1), out port); //if unable to parse, then port = 0
                    if (port > serialPorts)
                    {
                        serialPorts = port;
                    }
                }
            }

            if (serialPorts == 0)
            {
                //No serial ports found
                return;
            }

            //Populate the table
            tableLayoutPanel1.SuspendLayout();
            for (int i = 1; i <= serialPorts; i++)
            {
                string portName = "SERIAL" + i.ToString();
                string uartName = "";

                if (_uartNames.ContainsKey(i))
                {
                    uartName = _uartNames[i];
                }

                //Get the RTS/CTS options if there is any
                string ctsrtsParamName = "BRD_SER" +i.ToString() + "_RTSCTS";
                if (MainV2.comPort.MAV.param.ContainsKey(ctsrtsParamName))
                {
                    var ctsValue = MainV2.comPort.MAV.param[ctsrtsParamName].Value;
                    if (ctsValue == 1)
                    {
                        uartName = uartName + " (RTS/CTS)";
                    }
                    if (ctsValue == 2)
                    {
                        uartName = uartName + " (RTS/CTS Auto)";
                    }
                }
                //Port Name Label
                Label label = new Label();
                label.Text = "SERIAL PORT " + i.ToString() + "\n" + uartName;
                label.Location = new Point(0, 0);
                label.Size = new Size(100, 40);
                label.Anchor = AnchorStyles.None;
                label.Dock = DockStyle.Fill;
                label.TextAlign = ContentAlignment.MiddleCenter;
                Font f = label.Font;
                label.Font = new Font(Font, FontStyle.Bold);
                ThemeManager.ApplyThemeTo(label);
                tableLayoutPanel1.GetControlFromPosition(0, i)?.Dispose() ;
                tableLayoutPanel1.Controls.Add(label, 0, i);

                //Baud setting combobox
                string baudParamName = portName + "_BAUD";
                var baudOptions = ParameterMetaDataRepository.GetParameterOptionsInt(baudParamName, MainV2.comPort.MAV.cs.firmware.ToString());
                if (baudOptions.Count > 0)
                {
                    ComboBox cmb = new ComboBox() { Dock = DockStyle.Fill };
                    cmb.DropDownStyle = ComboBoxStyle.DropDownList;
                    cmb.DataSource = baudOptions;
                    cmb.DisplayMember = "Value";
                    cmb.ValueMember = "Key";
                    cmb.Anchor = AnchorStyles.None;
                    cmb.Name = baudParamName;
                    widenComboBox(cmb);
                    ThemeManager.ApplyThemeTo(cmb);
                    tableLayoutPanel1.GetControlFromPosition(1, i)?.Dispose();
                    tableLayoutPanel1.Controls.Add(cmb, 1, i);
                    int val = -1;
                    if (int.TryParse(MainV2.comPort.MAV.param[baudParamName].Value.ToString(), out val))
                    {
                        cmb.SelectedValue = val;
                    }
                    else
                    {
                        cmb.SelectedIndex = -1;
                    }
                    cmb.SelectedIndexChanged += (s, a) =>
                    {
                        setParam(cmb.Name, cmb.SelectedValue.ToString());
                    };
                }

                //Protocol setting combobox
                string protParamName = portName + "_PROTOCOL";
                var protOptions = ParameterMetaDataRepository.GetParameterOptionsInt(protParamName, MainV2.comPort.MAV.cs.firmware.ToString());
                if (protOptions.Count > 0)
                {
                    ComboBox cmb = new ComboBox()
                    {
                        Dock = DockStyle.Fill,
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        DataSource = protOptions,
                        DisplayMember = "Value",
                        ValueMember = "Key",
                        Anchor = AnchorStyles.None,
                        Name = protParamName
                    };

                    widenComboBox(cmb);
                    ThemeManager.ApplyThemeTo(cmb);
                    tableLayoutPanel1.GetControlFromPosition(2, i)?.Dispose();
                    tableLayoutPanel1.Controls.Add(cmb, 2, i);

                    // Populate the current selection from the cell value, if it's valid
                    int val = -1;
                    if (int.TryParse(MainV2.comPort.MAV.param[protParamName].Value.ToString(), out val))
                    {
                        cmb.SelectedValue = val;
                    }
                    else
                    {
                        cmb.SelectedIndex = -1;
                    }
                    cmb.SelectedIndexChanged += (s, a) =>
                    {
                        setParam(cmb.Name, cmb.SelectedValue.ToString());
                        //Apply rules
                        doApplyRules(portName, cmb.SelectedValue.ToString());
                    };
                }

                //Options label
                string param_name = portName + "_OPTIONS";

                var binlist = ParameterMetaDataRepository.GetParameterBitMaskInt(param_name, MainV2.comPort.MAV.cs.firmware.ToString());
                var param_value = MainV2.comPort.MAV.param[param_name].Value;

                Label label1 = new Label()
                {
                    Text = "",
                    Anchor = AnchorStyles.None,
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    MaximumSize = new Size(300, 200),
                    Location = new Point(0, 0),
                    Size = new Size(100, 20),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                //populate the label based on the options value
                setLabelOptions(param_name, label1);
                ThemeManager.ApplyThemeTo(label1);
                tableLayoutPanel1.GetControlFromPosition(3, i)?.Dispose();
                tableLayoutPanel1.Controls.Add(label1, 3, i);

                //Bit setting button and form
                var bitmask = ParameterMetaDataRepository.GetParameterBitMaskInt(param_name, MainV2.comPort.MAV.cs.firmware.ToString());
                if (bitmask.Count > 0)
                {
                    MyButton optionsControl = new MyButton() { Text = "Set Bitmask" };
                    optionsControl.Click += (s, a) =>
                    {
                        var mcb = new MavlinkCheckBoxBitMask();
                        var list = new MAVLink.MAVLinkParamList();

                        // Try and get type so the correct bitmask to value conversion is done
                        var type = MAVLink.MAV_PARAM_TYPE.INT32;
                        if (MainV2.comPort.MAV.param.ContainsKey(param_name))
                        {
                            type = MainV2.comPort.MAV.param[param_name].TypeAP;
                        }

                        list.Add(new MAVLink.MAVLinkParam(param_name, double.Parse(MainV2.comPort.MAV.param[param_name].Value.ToString(), CultureInfo.InvariantCulture),
                            type));
                        mcb.setup(param_name, list);
                        mcb.ValueChanged += (o, x, value) =>
                        {
                            setParam(param_name, value.ToString());
                            setLabelOptions(param_name, label1);
                            mcb.Focus();
                        };
                        var frm = mcb.ShowUserControl();
                        frm.TopMost = true;
                        //set the location of the form to center of the screen
                        frm.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - frm.Width) / 2,
                                                       (Screen.PrimaryScreen.WorkingArea.Height - frm.Height) / 2);
                    };

                    ThemeManager.ApplyThemeTo(optionsControl);
                    tableLayoutPanel1.GetControlFromPosition(4, i)?.Dispose();
                    tableLayoutPanel1.Controls.Add(optionsControl, 4, i);
                }
            }
            //Add the message to the bottom of the table
            noteLabel = new Label()
            {
                Text = "Note: Changes to the serial port settings will not take effect until the board is rebooted.",
                Anchor = AnchorStyles.None,
                Dock = DockStyle.Fill,
                AutoSize = true,
                MaximumSize = new Size(600, 200),
                Location = new Point(0, 0),
                Size = new Size(100, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };
            ThemeManager.ApplyThemeTo(noteLabel);
            tableLayoutPanel1.GetControlFromPosition(0, serialPorts + 1)?.Dispose();
            tableLayoutPanel1.Controls.Add(noteLabel, 0, serialPorts + 1);
            //make the label span the whole row
            tableLayoutPanel1.SetColumnSpan(noteLabel, 5);
            
            tableLayoutPanel1.ResumeLayout();

        }

        //Apply baud rate and options rules based on protocol
        //Rule definition is in SerialOptionRules.json file
        private void doApplyRules(string portName, string v)
        {
            //extract the port number from the port name
            int port;
            Int32.TryParse(portName.Substring(6), out port); //if unable to parse, then port = 0
            //Check if there is a rule for this protocol
            if (_optionRules.ContainsKey(int.Parse(v)))
            {
                //Apply the rule
                var rule = _optionRules[int.Parse(v)];
                if (rule.baudrate > -1)
                {
                    //CMB selected value will set the param via the event handler
                    ComboBox cmb = tableLayoutPanel1.GetControlFromPosition(1, port) as ComboBox;
                    cmb.SelectedValue = rule.baudrate;
                }
                if (rule.options > -1)
                {
                    //set the options
                    setParam(portName + "_OPTIONS", rule.options.ToString());
                    setLabelOptions(portName + "_OPTIONS", tableLayoutPanel1.GetControlFromPosition(3, port) as Label);
                }
                noteLabel.Text = portName + " : " + rule.comment;
            }
            else 
            {
                noteLabel.Text = "";
            }
            //Check if there are four or more port set to mavlink
            //In Ardupilot GCS_Mavlink.cpp  : #define MAVLINK_COMM_NUM_BUFFERS 5
            //But this includes the USB port, so we can only have 4 ports set to mavlink

            int mavlinkPorts = 0;
            for (int i = 1; i <= serialPorts; i++)
            {
                string protParamName = "SERIAL" + i.ToString() + "_PROTOCOL";
                if (MainV2.comPort.MAV.param[protParamName].Value.ToString() == "1"
                    || MainV2.comPort.MAV.param[protParamName].Value.ToString() == "2")
                {
                    mavlinkPorts++;
                }
            }
            if (mavlinkPorts >= 4)
            {
                noteLabel.Text = noteLabel.Text + "\r\nWarning: Maximum number of Mavlink ports are 5 including the USB port!";
            }
       }

        // Widen the drop-down menu if the text is too long
        // https://www.codeproject.com/Articles/5801/Adjust-combo-box-drop-down-list-width-to-longest-s
        void widenComboBox(ComboBox c)
        {
            c.DropDown += (s, ev) =>
            {
                ComboBox senderComboBox = (ComboBox)s;
                int width = senderComboBox.DropDownWidth;
                Graphics g = senderComboBox.CreateGraphics();
                Font font = senderComboBox.Font;
                int vertScrollBarWidth =
                    (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems)
                    ? SystemInformation.VerticalScrollBarWidth : 0;

                int newWidth;
                foreach (KeyValuePair<int, string> item in ((ComboBox)s).Items)
                {
                    newWidth = (int)g.MeasureString(item.Value, font).Width
                        + vertScrollBarWidth;
                    if (width < newWidth)
                    {
                        width = newWidth;
                    }
                }
                senderComboBox.DropDownWidth = width;
            };
        }

        public void setLabelOptions(string param_name, Label l)
        {
            var binlist = ParameterMetaDataRepository.GetParameterBitMaskInt(param_name, MainV2.comPort.MAV.cs.firmware.ToString());
            var param_value = MainV2.comPort.MAV.param[param_name].Value;
            l.Text = string.Join(" / ", binlist.Where(bin => ((uint)param_value & (1 << bin.Key)) > 0).Select(bin => bin.Value));
        }

        public bool setParam(string param_name, string param_value)
        {

            double val = 0;
            val = double.TryParse(param_value, out val) ? val : 0;

            if (MainV2.comPort.MAV.param.ContainsKey(param_name))
            {
                bool ans = MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, param_name, val);
                if (!ans)
                {
                    CustomMessageBox.Show("Unable to set parameter " + param_name);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                CustomMessageBox.Show("Parameter " + param_name + " not found");
                return false;
            }
        }

        public void Deactivate()
        {
            //This needs to be here even when empty 
        }
    }

    //Class to hold the rules for baud rate and options
    //The SerialOptionRules.json file is deserialized into a dictionary of this class where the key is the protocol number
    public class SerialOptionRuleItem
    {
        [JsonPropertyAttribute("PresetBaudRate")]
        // if -1 , then baud rate is not changed
        public int baudrate;

        [JsonPropertyAttribute("PresetOptionsByte")]
        // if -1 , then options byte is not changed
        public int options;

        [JsonPropertyAttribute("Comment")]
        public string comment;
    }

}
