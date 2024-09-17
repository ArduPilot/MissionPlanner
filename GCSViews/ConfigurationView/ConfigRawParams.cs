using log4net;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using org.mariuszgromada.math.mxparser;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigRawParams : MyUserControl, IActivate, IDeactivate
    {
        // from http://stackoverflow.com/questions/2512781/winforms-big-paragraph-tooltip/2512895#2512895
        private const int maximumSingleLineTooltipLength = 50;

        private static readonly ILog log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Hashtable tooltips = new Hashtable();
        // Changes made to the params between writing to the copter
        private readonly Hashtable _changes = new Hashtable();
        private static List<GitHubContent.FileInfo> paramfiles;
        // ?
        internal static bool startup = true;
        internal static List<DataGridViewRow> rowlist = new List<DataGridViewRow>();

        // Used by Param Tree to filter by prefix
        private string filterPrefix = "";

        public ConfigRawParams()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            if ((rowlist.Count == 0) || (!Settings.Instance.GetBoolean("SlowMachine", false))) startup = true;
            //If we connected to another vehicle the do a full refresh
            if (rowlist.Count != MainV2.comPort.MAV.param.Count()) startup = true;

            _changes.Clear();

            BUT_writePIDS.Enabled = MainV2.comPort.BaseStream.IsOpen;
            BUT_rerequestparams.Enabled = MainV2.comPort.BaseStream.IsOpen;
            BUT_reset_params.Enabled = MainV2.comPort.BaseStream.IsOpen;
            BUT_commitToFlash.Visible = MainV2.DisplayConfiguration.displayParamCommitButton;
            BUT_refreshTable.Visible = Settings.Instance.GetBoolean("SlowMachine", false);

            CMB_paramfiles.Enabled = false;
            BUT_paramfileload.Enabled = false;
            ThreadPool.QueueUserWorkItem(updatedefaultlist);

            Params.Enabled = false;

            foreach (DataGridViewColumn col in Params.Columns)
            {
                // Don't need to size a fill column
                if (col.AutoSizeMode == DataGridViewAutoSizeColumnMode.Fill) continue;

                // Don't need to size a column that can't be resized
                if (col.Resizable == DataGridViewTriState.False) continue;

                if (!String.IsNullOrEmpty(Settings.Instance["rawparam_" + col.Name + "_width"]))
                {
                    col.Width = (int)Math.Max(5, Settings.Instance.GetInt32("rawparam_" + col.Name + "_width"));
                    log.InfoFormat("{0} to {1}", col.Name, col.Width);
                }
            }
            splitContainer1.SplitterDistance = Settings.Instance.GetInt32("rawparam_splitterdistance", 180);
            splitContainer1.Panel1Collapsed = Settings.Instance.GetBoolean("rawparam_panel1collapsed", false);
            but_collapse.Text = splitContainer1.Panel1Collapsed ? ">" : "<";

            processToScreen();

            Params.Enabled = true;

            Common.MessageShowAgain(Strings.RawParamWarning, Strings.RawParamWarningi);

            startup = false;

            txt_search.Focus();
        }

        public void Deactivate()
        {
            foreach (DataGridViewColumn col in Params.Columns)
            {
                // Don't need to save the width of a fill column
                if (col.AutoSizeMode == DataGridViewAutoSizeColumnMode.Fill) continue;

                // Don't need to save the width of a column that can't be resized
                if (col.Resizable == DataGridViewTriState.False) continue;

                Settings.Instance["rawparam_" + col.Name + "_width"] = col.Width.ToString("0", CultureInfo.InvariantCulture);
            }

            Settings.Instance["rawparam_splitterdistance"] = splitContainer1.SplitterDistance.ToString();
            Settings.Instance["rawparam_panel1collapsed"] = splitContainer1.Panel1Collapsed.ToString();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                BUT_writePIDS_Click(null, null);
                return true;
            }

            return false;
        }

        private void BUT_load_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog
            {
                AddExtension = true,
                DefaultExt = ".param",
                RestoreDirectory = true,
                Filter = ParamFile.FileMask
            })
            {
                var dr = ofd.ShowDialog();

                if (dr == DialogResult.OK)
                {
                    loadparamsfromfile(ofd.FileName, !MainV2.comPort.BaseStream.IsOpen);

                    if (!MainV2.comPort.BaseStream.IsOpen)
                        Activate();
                }
            }
        }

        private void loadparamsfromfile(string fn, bool offline = false)
        {
            var param2 = ParamFile.loadParamFile(fn);

            var loaded = 0;
            var missed = 0;
            List<string> missing = new List<string>();

            foreach (string name in param2.Keys)
            {
                var set = false;
                var value = param2[name].ToString();
                // set param table as well
                foreach (DataGridViewRow row in Params.Rows)
                {
                    if (name == "SYSID_SW_MREV")
                        continue;
                    if (name == "WP_TOTAL")
                        continue;
                    if (name == "CMD_TOTAL")
                        continue;
                    if (name == "FENCE_TOTAL")
                        continue;
                    if (name == "SYS_NUM_RESETS")
                        continue;
                    if (name == "ARSPD_OFFSET")
                        continue;
                    if (name == "GND_ABS_PRESS")
                        continue;
                    if (name == "GND_TEMP")
                        continue;
                    if (name == "CMD_INDEX")
                        continue;
                    if (name == "LOG_LASTFILE")
                        continue;
                    if (name == "FORMAT_VERSION")
                        continue;
                    if (row.Cells[Command.Index].Value != null && row.Cells[Command.Index].Value?.ToString() == name)
                    {
                        set = true;
                        if (row.Cells[Value.Index].Value.ToString() != value)
                            row.Cells[Value.Index].Value = value;
                        break;
                    }
                }

                if (offline && !set)
                {
                    set = true;
                    MainV2.comPort.MAV.param.Add(new MAVLink.MAVLinkParam(name, double.Parse(value),
                        MAVLink.MAV_PARAM_TYPE.REAL32));
                }

                if (set)
                {
                    loaded++;
                }
                else
                {
                    missed++;
                    missing.Add(name);
                }
            }

            if (missed > 0)
            {
                string list = "";
                foreach (var item in missing)
                {
                    list += item + " ";
                }
                CustomMessageBox.Show("Missing " + missed + " params\n" + list, "No matching Params", MessageBoxButtons.OK);
            }
        }

        private void BUT_save_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = ".param",
                RestoreDirectory = true,
                Filter = "Param List|*.param;*.parm"
            })
            {
                var dr = sfd.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    var data = new Hashtable();
                    foreach (DataGridViewRow row in Params.Rows)
                    {
                        try
                        {
                            var value = double.Parse(row.Cells[Value.Index].Value.ToString());

                            data[row.Cells[Command.Index].Value.ToString()] = value;
                        }
                        catch (Exception)
                        {
                            CustomMessageBox.Show(Strings.InvalidNumberEntered + " " + row.Cells[Command.Index].Value);
                        }
                    }

                    ParamFile.SaveParamFile(sfd.FileName, data);
                }
            }
        }

        private void BUT_writePIDS_Click(object sender, EventArgs e)
        {
            if (Common.MessageShowAgain("Write Raw Params", "Are you Sure?") != DialogResult.OK)
                return;

            // sort with enable at the bottom - this ensures params are set before the function is disabled
            var temp = _changes.Keys.Cast<string>().ToList();

            temp.SortENABLE();

            bool enable = temp.Any(a => a.EndsWith("_ENABLE"));

            int error = 0;
            bool reboot = false;

            foreach (string value in temp)
            {
                try
                {
                    if (MainV2.comPort.BaseStream == null || !MainV2.comPort.BaseStream.IsOpen)
                    {
                        CustomMessageBox.Show("Your are not connected", Strings.ERROR);
                        return;
                    }

                    MainV2.comPort.setParam(value, (double)_changes[value]);
                    //check if reboot required
                    if (ParameterMetaDataRepository.GetParameterRebootRequired(value, MainV2.comPort.MAV.cs.firmware.ToString()))
                    {
                        reboot = true;
                    }
                    try
                    {
                        // set control as well
                        var textControls = Controls.Find(value, true);
                        if (textControls.Length > 0)
                        {
                            ThemeManager.ApplyThemeTo(textControls[0]);
                        }
                    }
                    catch
                    {
                    }

                    try
                    {
                        // set param table as well
                        foreach (DataGridViewRow row in Params.Rows)
                        {
                            if (row.Cells[Command.Index].Value.ToString() == value)
                            {
                                row.Cells[Value.Index].Style.BackColor = ThemeManager.ControlBGColor;
                                _changes.Remove(value);
                                break;
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                catch
                {
                    error++;
                    CustomMessageBox.Show("Set " + value + " Failed");
                }
            }

            if (error > 0)
                CustomMessageBox.Show("Not all parameters successfully saved.", "Saved");
            else
                CustomMessageBox.Show("Parameters successfully saved.", "Saved");

            //Check if reboot is required
            if (reboot)
            {
               CustomMessageBox.Show("Reboot is required for some parameters to take effect.", "Reboot Required");
            }

            if (MainV2.comPort.MAV.param.TotalReceived != MainV2.comPort.MAV.param.TotalReported )
            {
                if (MainV2.comPort.MAV.cs.armed)
                {
                    CustomMessageBox.Show("The number of available parameters changed, until full param refresh is done, some parameters will not be available.", "Params");
                    //Hack the number of reported params to keep params list available
                    MainV2.comPort.MAV.param.TotalReported = MainV2.comPort.MAV.param.TotalReceived;
                }
                else
                {
                    CustomMessageBox.Show("The number of available parameters changed. A full param refresh will be done to show all params.", "Params");
                    //Click on refresh button
                    BUT_rerequestparams_Click(BUT_rerequestparams, null);
                }
            }
        }

        private void BUT_compare_Click(object sender, EventArgs e)
        {
            var param2 = new Dictionary<string, double>();

            using (var ofd = new OpenFileDialog
            {
                AddExtension = true,
                DefaultExt = ".param",
                RestoreDirectory = true,
                Filter = ParamFile.FileMask
            })
            {
                var dr = ofd.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    param2 = ParamFile.loadParamFile(ofd.FileName);

                    Form paramCompareForm = new ParamCompare(Params, MainV2.comPort.MAV.param, param2);

                    ThemeManager.ApplyThemeTo(paramCompareForm);
                    paramCompareForm.ShowDialog();
                }
            }
        }

        private void BUT_rerequestparams_Click(object sender, EventArgs e)
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
                return;

            if (!MainV2.comPort.MAV.cs.armed || DialogResult.OK ==
                Common.MessageShowAgain("Refresh Params", Strings.WarningUpdateParamList, true))
            {
                ((Control)sender).Enabled = false;

                try
                {
                    MainV2.comPort.getParamList();
                }
                catch (Exception ex)
                {
                    log.Error("Exception getting param list", ex);
                    CustomMessageBox.Show(Strings.ErrorReceivingParams, Strings.ERROR);
                }


                ((Control)sender).Enabled = true;

                startup = true;

                processToScreen();

                FilterTimerOnElapsed(null, null);

                startup = false;
            }
        }

        private void Params_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1 || startup || e.ColumnIndex != Value.Index)
                return;
            try
            {
                if (Params[Command.Index, e.RowIndex].Value.ToString().EndsWith("_REV") &&
                    (Params[Command.Index, e.RowIndex].Value.ToString().StartsWith("RC") ||
                     Params[Command.Index, e.RowIndex].Value.ToString().StartsWith("HS")))
                {
                    if (Params[e.ColumnIndex, e.RowIndex].Value.ToString() == "0")
                        Params[e.ColumnIndex, e.RowIndex].Value = "-1";
                }

                double min = 0;
                double max = 0;

                var value = Params[e.ColumnIndex, e.RowIndex].Value.ToString();
                value = value.Replace(',', '.');

                var newvalue = (double) new Expression(value).calculate();
                if (double.IsNaN(newvalue) || double.IsInfinity(newvalue))
                {
                    throw new Exception();
                }

                var readonly1 = ParameterMetaDataRepository.GetParameterMetaData(
                    Params[Command.Index, e.RowIndex].Value.ToString(),
                    ParameterMetaDataConstants.ReadOnly, MainV2.comPort.MAV.cs.firmware.ToString());
                if (!String.IsNullOrEmpty(readonly1))
                {
                    var readonly2 = bool.Parse(readonly1);
                    if (readonly2)
                    {
                        CustomMessageBox.Show(
                            Params[Command.Index, e.RowIndex].Value +
                            " is marked as ReadOnly, and will not be changed", "ReadOnly",
                            MessageBoxButtons.OK);
                        Params.CellValueChanged -= Params_CellValueChanged;
                        Params[e.ColumnIndex, e.RowIndex].Value = cellEditValue;
                        Params.CellValueChanged += Params_CellValueChanged;
                        return;
                    }
                }

                if (ParameterMetaDataRepository.GetParameterRange(Params[Command.Index, e.RowIndex].Value.ToString(),
                    ref min, ref max, MainV2.comPort.MAV.cs.firmware.ToString()))
                {
                    if (newvalue > max || newvalue < min)
                    {
                        if (
                            CustomMessageBox.Show(
                                Params[Command.Index, e.RowIndex].Value +
                                " value is out of range. Do you want to continue?", "Out of range",
                                MessageBoxButtons.YesNo) == (int)DialogResult.No)
                        {
                            Params.CellValueChanged -= Params_CellValueChanged;
                            Params[e.ColumnIndex, e.RowIndex].Value = cellEditValue;
                            Params.CellValueChanged += Params_CellValueChanged;
                            return;
                        }
                    }
                }

                Params[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.Green;
                log.InfoFormat("Queue change {0} = {1} ({2})", Params[Command.Index, e.RowIndex].Value, Params[e.ColumnIndex, e.RowIndex].Value, newvalue);
                _changes[Params[Command.Index, e.RowIndex].Value] = newvalue;

                Params.CellValueChanged -= Params_CellValueChanged;
                Params[e.ColumnIndex, e.RowIndex].Value = newvalue.ToString();
                Params.CellValueChanged += Params_CellValueChanged;
            }
            catch (Exception)
            {
                Params[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.Red;
            }


            Params.Focus();
        }

        private static string AddNewLinesForTooltip(string text)
        {
            if (text.Length < maximumSingleLineTooltipLength)
                return text;
            var lineLength = (int)Math.Sqrt(text.Length) * 2;
            var sb = new StringBuilder();
            var currentLinePosition = 0;
            for (var textIndex = 0; textIndex < text.Length; textIndex++)
            {
                // If we have reached the target line length and the next
                // character is whitespace then begin a new line.
                if (currentLinePosition >= lineLength &&
                    char.IsWhiteSpace(text[textIndex]))
                {
                    sb.Append(Environment.NewLine);
                    currentLinePosition = 0;
                }
                // If we have just started a new line, skip all the whitespace.
                if (currentLinePosition == 0)
                    while (textIndex < text.Length && char.IsWhiteSpace(text[textIndex]))
                        textIndex++;
                // Append the next character.
                if (textIndex < text.Length) sb.Append(text[textIndex]);
                currentLinePosition++;
            }
            return sb.ToString();
        }

        internal void processToScreen()
        {
            toolTip1.RemoveAll();
            Params.Rows.Clear();
            log.Info("processToScreen");

            var list = new List<string>();

            // process hashdefines and update display
            // But only if startup is true, otherwise we assume that rowlist is valid and just put it back
            // to the gridview

            if (startup)
            {
                foreach (string item in MainV2.comPort.MAV.param.Keys)
                    list.Add(item);

                rowlist.Clear();

                bool has_defaults = false;

                Parallel.ForEach(list, value =>
                {
                    if (value == null || value == "")
                        return;

                    var row = new DataGridViewRow() { Height = 36 };
                    lock (rowlist)
                        rowlist.Add(row);
                    row.CreateCells(Params);
                    row.Cells[Command.Index].Value = value;
                    row.Cells[Value.Index].Value = MainV2.comPort.MAV.param[value].ToString();
                    var fav_params = Settings.Instance.GetList("fav_params");
                    row.Cells[Fav.Index].Value = fav_params.Contains(value);

                    if (MainV2.comPort.MAV.param[value].default_value.HasValue) {
                        has_defaults = true;
                        row.Cells[Default_value.Index].Value = MainV2.comPort.MAV.param[value].default_value_to_string();
                    } else {
                        row.Cells[Default_value.Index].Value = "NaN";
                    }
                    try
                    {
                        var metaDataDescription = ParameterMetaDataRepository.GetParameterMetaData(value,
                            ParameterMetaDataConstants.Description, MainV2.comPort.MAV.cs.firmware.ToString());
                        if (!string.IsNullOrEmpty(metaDataDescription))
                        {
                            row.Cells[Command.Index].ToolTipText = AddNewLinesForTooltip(metaDataDescription);
                            row.Cells[Value.Index].ToolTipText = AddNewLinesForTooltip(metaDataDescription);

                            var range = ParameterMetaDataRepository.GetParameterMetaData(value,
                                ParameterMetaDataConstants.Range, MainV2.comPort.MAV.cs.firmware.ToString());
                            var options = ParameterMetaDataRepository.GetParameterMetaData(value,
                                ParameterMetaDataConstants.Values, MainV2.comPort.MAV.cs.firmware.ToString());
                            var units = ParameterMetaDataRepository.GetParameterMetaData(value,
                                ParameterMetaDataConstants.Units, MainV2.comPort.MAV.cs.firmware.ToString());

                            row.Cells[Units.Index].Value = units;
                            row.Cells[Options.Index].Value = (range + "\n" + options.Replace(",", "\n")).Trim();
                            if (options.Length > 0) row.Cells[Options.Index].ToolTipText = options.Replace(',', '\n');
                            int N = options.Count(c => c.Equals(','));
                            if (N > 50)
                            {
                                int columns = (N - 1) / 50 + 1;
                                StringBuilder ans = new StringBuilder();
                                var opts = options.Split(',');
                                int i = 0;
                                while(true)
                                {
                                    for(int j=0; j<columns; j++)
                                    {
                                        ans.Append(opts[i] + ", ");
                                        i++;
                                        if (i >= N) break;
                                    }
                                    if (i >= N) break;
                                    ans.Append("\n");
                                }
                                row.Cells[Options.Index].ToolTipText = ans.ToInvariantString();
                            }
                            row.Cells[Desc.Index].Value = metaDataDescription;
                            row.Cells[Desc.Index].ToolTipText = AddNewLinesForTooltip(metaDataDescription);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                });

                Default_value.Visible = has_defaults;
                chk_none_default.Visible = has_defaults;
            }
            //update values in rowlist
            if (!startup)
            {
                foreach (DataGridViewRow r in rowlist)
                {
                    r.Cells[Value.Index].Value = MainV2.comPort.MAV.param[r.Cells[Command.Index].Value.ToString()].ToString();
                }
            }


            log.Info("about to add all");

            Params.Visible = false;

            Params.Rows.AddRange(rowlist.ToArray());

            log.Info("about to sort");

            Params.SortCompare += OnParamsOnSortCompare;

            Params.Sort(Params.Columns[Command.Index], ListSortDirection.Ascending);

            Params.Visible = true;

            if (splitContainer1.Panel1Collapsed == false)
            {
                BuildTree();
            }

            log.Info("Done");
        }

        private void BuildTree()
        {
            treeView1.Nodes.Clear();
            var currentNode = treeView1.Nodes.Add("All");
            string currentPrefix = "";

            // Get command names from the gridview
            List<string> commands = new List<string>();
            foreach (DataGridViewRow row in Params.Rows)
            {
                string command = row.Cells[Command.Index].Value.ToString();
                if (!commands.Contains(command))
                {
                    commands.Add(command);
                }
            }

            // Sort them again (because of the favorites, they may be out of order)
            commands.Sort();

            for (int i = 0; i < commands.Count; i++)
            {
                string param = commands[i];

                // While param does not start with currentPrefix, step up a layer in the tree
                while (!param.StartsWith(currentPrefix))
                {
                    currentPrefix = currentPrefix.RemoveFromEnd(currentNode.Text.Split('_').Last() + "_");
                    currentNode = currentNode.Parent;
                }

                // If this is the last parameter, add it
                if (i == commands.Count - 1)
                {
                    currentNode.Nodes.Add(param);
                    break;
                }

                string next_param = commands[i + 1];
                // While the next parameter has a common prefix with this, add branch nodes
                string nodeToAdd = param.Substring(currentPrefix.Length).Split('_')[0] + "_";
                while (nodeToAdd.Length > 1 // While the currentPrefix is smaller than param
                    && param.StartsWith(currentPrefix + nodeToAdd) // And while this parameter starts with currentPrefix+nodeToAdd (needed for edge case where next_param starts with the full name of this param; see Q_PLT_Y_RATE and Q_PLT_Y_RATE_TC)
                    && next_param.StartsWith(currentPrefix + nodeToAdd)) // And the next parameter also starts with currentPrefix
                {
                    currentPrefix += nodeToAdd;
                    currentNode = currentNode.Nodes.Add(currentPrefix.Substring(0, currentPrefix.Length - 1));
                    nodeToAdd = param.Substring(currentPrefix.Length).Split('_')[0] + "_";
                }
                currentNode.Nodes.Add(param);
            }
            treeView1.TopNode.Expand();
        }


        private void OnParamsOnSortCompare(object sender, DataGridViewSortCompareEventArgs args)
        {
            var fav1obj = Params[Fav.Index, args.RowIndex1].Value;
            var fav2obj = Params[Fav.Index, args.RowIndex2].Value;

            var fav1 = fav1obj == null ? false : (bool)fav1obj;

            var fav2 = fav2obj == null ? false : (bool)fav2obj;

            if (args.CellValue1 == null)
                return;

            if (args.CellValue2 == null)
                return;

            args.SortResult = args.CellValue1.ToString().CompareTo(args.CellValue2.ToString());
            args.Handled = true;

            if (fav1 && fav2)
            {
                return;
            }

            if (fav1 || fav2)
                args.SortResult = fav1.CompareTo(fav2) * (Params.SortOrder == SortOrder.Ascending ? -1 : 1);
        }

        private void updatedefaultlist(object crap)
        {
            try
            {
                if (paramfiles == null)
                {
                    string subdir = "";
                    if (MainV2.comPort.MAV.param.ContainsKey("Q_ENABLE") &&
                        MainV2.comPort.MAV.param["Q_ENABLE"].Value >= 1.0)
                    {
                        subdir = "QuadPlanes/";
                    }
                    paramfiles = GitHubContent.GetDirContent("ardupilot", "ardupilot", "/Tools/Frame_params/" + subdir, ".param");
                }

                BeginInvoke((Action)delegate
               {
                   CMB_paramfiles.DataSource = paramfiles.ToArray();
                   CMB_paramfiles.DisplayMember = "name";
                   CMB_paramfiles.Enabled = true;
                   BUT_paramfileload.Enabled = true;
               });
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        void filterList(string searchfor)
        {
            DateTime start = DateTime.Now;
            Params.Visible = false;
            if (searchfor.Length >= 2 || searchfor.Length == 0)
            {
                Regex filter = new Regex(searchfor.Replace("*", ".*").Replace("..*", ".*"), RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);

                foreach (DataGridViewRow row in Params.Rows)
                {
                    string name = row.Cells[Command.Index].Value.ToString();
                    if (name != filterPrefix.TrimEnd('_') && !name.StartsWith(filterPrefix))
                    {
                        row.Visible = false;
                        continue;
                    }
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Value != null && filter.IsMatch(cell.Value.ToString()))
                        {
                            row.Visible = true;
                            break;
                        }
                        row.Visible = false;
                    }
                }
            }

            if (chk_modified.Checked)
            {
                foreach (DataGridViewRow row in Params.Rows)
                {
                    // is it modified? - always show
                    if (_changes.ContainsKey(row.Cells[Command.Index].Value))
                    {
                        row.Visible = true;
                    }
                    else
                    {
                        row.Visible = false;
                    }
                }
            }

            if (chk_none_default.Checked)
            {
                foreach (DataGridViewRow row in Params.Rows)
                {
                    row.Visible = row.Cells[Default_value.Index].Value.ToString() != row.Cells[Value.Index].Value.ToString();
                }
            }

            Params.Visible = true;

            log.InfoFormat("Filter: {0}ms", (DateTime.Now - start).TotalMilliseconds);
        }

        private void BUT_paramfileload_Click(object sender, EventArgs e)
        {
            var filepath = Settings.GetUserDataDirectory() + CMB_paramfiles.Text;

            try
            {
                var data = GitHubContent.GetFileContent("ardupilot", "ardupilot",
                    ((GitHubContent.FileInfo)CMB_paramfiles.SelectedValue).path);

                File.WriteAllBytes(filepath, data);

                var param2 = ParamFile.loadParamFile(filepath);

                Form paramCompareForm = new ParamCompare(Params, MainV2.comPort.MAV.param, param2);

                ThemeManager.ApplyThemeTo(paramCompareForm);
                if (paramCompareForm.ShowDialog() == DialogResult.OK)
                {
                    CustomMessageBox.Show("Loaded parameters, please make sure you write them!", "Loaded");
                }

                // no activate the user needs to click write.
                //this.Activate();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Failed to load file.\n" + ex);
            }
        }

        private void CMB_paramfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void BUT_reset_params_Click(object sender, EventArgs e)
        {
            if (
                CustomMessageBox.Show("Reset all parameters to default\nAre you sure!!", "Reset",
                    MessageBoxButtons.YesNo) == (int)DialogResult.Yes)
            {
                try
                {
                    MainV2.comPort.setParam(new[] { "FORMAT_VERSION", "SYSID_SW_MREV" }, 0);
                    Thread.Sleep(1000);
                    MainV2.comPort.doReboot(false, true);
                    MainV2.comPort.BaseStream.Close();


                    CustomMessageBox.Show(
                        "Your board is now rebooting, You will be required to reconnect to the autopilot.");
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    CustomMessageBox.Show(Strings.ErrorCommunicating + "\n" + ex, Strings.ERROR);
                }
            }
        }

        private readonly System.Timers.Timer _filterTimer = new System.Timers.Timer();
        private string cellEditValue;

        private void txt_search_TextChanged(object sender, EventArgs e)
        {
            _filterTimer.Elapsed -= FilterTimerOnElapsed;
            _filterTimer.Stop();
            _filterTimer.Interval = 500;
            _filterTimer.Elapsed += FilterTimerOnElapsed;
            _filterTimer.Start();
        }

        public void FilterTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _filterTimer.Stop();
            Invoke((Action)delegate
           {
               filterList(txt_search.Text);
               optionsControlUpateBounds();
           });
        }

        private void Params_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Only process the Description column
            if (e.RowIndex == -1 || startup)
                return;

            if (e.ColumnIndex == Desc.Index)
            {
                try
                {
                    string descStr = Params[e.ColumnIndex, e.RowIndex].Value.ToString();
                    CheckForUrlAndLaunchInBrowser(descStr);
                }
                catch
                {
                }
            }

            if (e.ColumnIndex == Fav.Index)
            {
                var check = Params[e.ColumnIndex, e.RowIndex].EditedFormattedValue;
                var name = Params[Command.Index, e.RowIndex].Value.ToString();

                if (check != null && (bool)check)
                {
                    // add entry
                    Settings.Instance.AppendList("fav_params", name);
                }
                else
                {
                    // remove entry
                    var list = Settings.Instance.GetList("fav_params");
                    Settings.Instance.SetList("fav_params", list.Where(s => s != name));
                }

                Params.Sort(Command, ListSortDirection.Ascending);
            }
        }

        public static void CheckForUrlAndLaunchInBrowser(string stringWithPossibleUrl)
        {
            if (stringWithPossibleUrl == null)
                return;

            foreach (string url in stringWithPossibleUrl.Split(' '))
            {
                Uri uriResult;
                if (Uri.TryCreate(url, UriKind.Absolute, out uriResult) &&
                    (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                {
                    try
                    {
                        // launch the URL in your default browser
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        process.StartInfo.UseShellExecute = true;
                        process.StartInfo.FileName = url;
                        process.Start();
                    }
                    catch { }

                    // only handle the first valid URL
                    return;
                }
            }
        }

        private void BUT_commitToFlash_Click(object sender, EventArgs e)
        {
            try
            {
                MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.MAV_CMD.PREFLIGHT_STORAGE, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
            }
            catch
            {
                CustomMessageBox.Show("Invalid command");
                return;
            }

            CustomMessageBox.Show("Parameters committed to non-volatile memory");
            return;
        }

        private void chk_filter_CheckedChanged(object sender, EventArgs e)
        {
            FilterTimerOnElapsed(null, null);
        }

        private void Params_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            cellEditValue = Params[e.ColumnIndex, e.RowIndex].Value.ToString();
        }

        private void BUT_refreshTable_Click(object sender, EventArgs e)
        {
            startup = true;
            processToScreen();
            startup = false;
        }
        
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string txt = treeView1.SelectedNode.Text + "_";
            if (txt == "All_") txt = "";
            filterPrefix = txt;
            FilterTimerOnElapsed(null, null);
        }

        private void but_collapse_Click(object sender, EventArgs e)
        {
            if (splitContainer1.Panel1Collapsed)
            {
                but_collapse.Text = "<";
                splitContainer1.Panel1Collapsed = false;
                BuildTree();
            }
            else
            {
                but_collapse.Text = ">";
                splitContainer1.Panel1Collapsed = true;
                filterPrefix = "";
                FilterTimerOnElapsed(null, null);
            }
        }

        Control optionsControl;
        // Create and place the relevant control in the options column when a row is entered
        private void Params_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            
            if (optionsControl != null)
            {
                Params.Controls.Remove(optionsControl);
                optionsControl.Dispose();
                optionsControl = null;
            }

            string param_name = Params[Command.Index, e.RowIndex].Value.ToString();
            string vehicle = MainV2.comPort.MAV.cs.firmware.ToString();
            var options = ParameterMetaDataRepository.GetParameterOptionsInt(param_name, vehicle);
            var bitmask = ParameterMetaDataRepository.GetParameterBitMaskInt(param_name, vehicle);
            // If this is a bitmask, create a button to open the bitmask editor
            // (this is better than trying to cram the bitmask checkboxes into the small cell)
            if (bitmask.Count > 0)
            {
                optionsControl = new MyButton() { Text = "Set Bitmask" };
                optionsControl.Click += (s, a) =>
                {
                    var mcb = new MavlinkCheckBoxBitMask();
                    var list = new MAVLink.MAVLinkParamList();

                    // Try and get type so the correct bitmask to value convertion is done
                    var type = MAVLink.MAV_PARAM_TYPE.INT32;
                    if (MainV2.comPort.MAV.param.ContainsKey(param_name))
                    {
                        type = MainV2.comPort.MAV.param[param_name].TypeAP;
                    }

                    list.Add(new MAVLink.MAVLinkParam(param_name, double.Parse(Params[Value.Index, e.RowIndex].Value.ToString(), CultureInfo.InvariantCulture),
                        type));
                    mcb.setup(param_name, list);
                    mcb.ValueChanged += (o, x, value) =>
                    {
                        Params.CurrentRow.Cells[Value.Index].Value = value;
                        Params.Invalidate();
                        mcb.Focus();
                    };
                    var frm = mcb.ShowUserControl();
                    frm.TopMost = true;
                };

                ThemeManager.ApplyThemeTo(optionsControl);
                optionsControl.Bounds = Params.GetCellDisplayRectangle(Options.Index, e.RowIndex, false);
                Params.Controls.Add(optionsControl);
            }
            // If there are options, create a combo box and populate it with the options
            else if (options.Count > 0)
            {
                ComboBox cmb = new ComboBox() { Dock = DockStyle.Fill };
                cmb.DropDownStyle = ComboBoxStyle.DropDownList;
                cmb.DataSource = options;
                cmb.DisplayMember = "Value";
                cmb.ValueMember = "Key";

                // Widen the dropdown menu if the text is too long
                // https://www.codeproject.com/Articles/5801/Adjust-combo-box-drop-down-list-width-to-longest-s
                cmb.DropDown += (s, ev) =>
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

                ThemeManager.ApplyThemeTo(cmb);

                // Create a blank panel to hold the combo box
                // (this blanks out the cell so that the text doesn't peak through)
                optionsControl = new Panel();
                ((Panel)optionsControl).BackColor = Params.Rows[e.RowIndex].InheritedStyle.BackColor;
                optionsControl.Controls.Add(cmb);
                optionsControl.Bounds = Params.GetCellDisplayRectangle(Options.Index, e.RowIndex, false);
                Params.Controls.Add(optionsControl);

                // Populate the current selection from the cell value, if it's valid
                int val = -1;
                if(int.TryParse(Params[Value.Index, e.RowIndex].Value.ToString(), out val))
                {
                    cmb.SelectedValue = val;
                }
                else
                {
                    cmb.SelectedIndex = -1;
                }

                // When the combo box selection changes, update the cell value
                cmb.SelectedIndexChanged += (s, a) =>
                {
                    Params.CurrentRow.Cells[Value.Index].Value = cmb.SelectedValue.ToString();
                    Params.Invalidate();
                };
            }

            // Otherwise, this is a simple numeric parameter
            else
            {
                double min = -32768.0;
                double max = 32768.0;
                if (ParameterMetaDataRepository.GetParameterRange(param_name, ref min, ref max, vehicle))
                {
                    // Default increment is the range divided by 1000, rounded to the nearest power of 10
                    double inc = Math.Pow(10, Math.Floor(Math.Log10((max - min) / 1000)));

                    ParameterMetaDataRepository.GetParameterIncrement(param_name, ref inc, vehicle);

                    NumericUpDown num = new NumericUpDown() { Dock = DockStyle.Fill };

                    // Set the number of decimal places based on the increment, or the minimum value if it's smaller (but not zero)
                    int decimalPlaces = (int)Math.Round(Math.Max(0, -Math.Log10(Math.Abs(inc))));
                    if (Math.Abs(min) < inc && Math.Abs(min) >= 1e-9)
                    {
                        decimalPlaces = (int)Math.Round(Math.Max(0, -Math.Log10(Math.Abs(min))));
                    }
                    num.DecimalPlaces = decimalPlaces;
                    num.Minimum = Math.Round((decimal)min, num.DecimalPlaces);
                    num.Maximum = Math.Round((decimal)max, num.DecimalPlaces);
                    num.Increment = Math.Round((decimal)inc, num.DecimalPlaces);
                    
                    // Parse the cell. Clamp the value to the bounds.
                    decimal val = num.Minimum;
                    decimal.TryParse(Params[Value.Index, e.RowIndex].Value?.ToString(), out val);
                    val = Math.Min(val, num.Maximum);
                    val = Math.Max(val, num.Minimum);
                    num.Value = Math.Round(val, num.DecimalPlaces);

                    // Update the cell if the text in the box changes
                    num.TextChanged += (s, a) =>
                    {
                        Params.CurrentRow.Cells[Value.Index].Value = num.Text;
                        Params.Invalidate();
                    };

                    ThemeManager.ApplyThemeTo(num);

                    optionsControl = new Panel();
                    ((Panel)optionsControl).BackColor = Params.Rows[e.RowIndex].InheritedStyle.BackColor;
                    optionsControl.Controls.Add(num);
                    optionsControl.Bounds = Params.GetCellDisplayRectangle(Options.Index, e.RowIndex, false);
                    Params.Controls.Add(optionsControl);

                }

            }

        }

        // Upate the size and location of our options control whenever a scroll or resize happens
        private void optionsControlUpateBounds()
        {
            if (optionsControl != null)
            {
                if (Params.CurrentRow == null)
                {
                    Params.Controls.Remove(optionsControl);
                    optionsControl.Dispose();
                    optionsControl = null;
                    return;
                }
                var bounds = Params.GetCellDisplayRectangle(Options.Index, Params.CurrentRow.Index, false);
                optionsControl.Bounds = bounds;
                optionsControl.Visible = bounds.Height > 0;
            }
        }

        private void Params_Scroll(object sender, ScrollEventArgs e)
        {
            optionsControlUpateBounds();
        }

        private void Params_RowHeightChanged(object sender, DataGridViewRowEventArgs e)
        {
            if(e.Row == Params.CurrentRow || e.Row.Index + 1 == Params.CurrentRow.Index)
            {
                optionsControlUpateBounds();
            }
        }

        private void Params_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (e.Column.Index == Options.Index || e.Column.Index + 1 == Options.Index)
            {
                optionsControlUpateBounds();
            }
        }
    }

}
