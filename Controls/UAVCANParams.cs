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
using log4net;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using UAVCAN;

namespace MissionPlanner.Controls
{
    public partial class UAVCANParams : MyUserControl, IActivate, IDeactivate
    {
        // ?
        internal bool startup = true;

        // from http://stackoverflow.com/questions/2512781/winforms-big-paragraph-tooltip/2512895#2512895
        private const int maximumSingleLineTooltipLength = 50;

        private static readonly ILog log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Hashtable tooltips = new Hashtable();
        // Changes made to the params between writing to the copter
        private readonly Hashtable _changes = new Hashtable();
        private UAVCAN.uavcan _can;
        private byte _node;
        private List<UAVCAN.uavcan.uavcan_protocol_param_GetSet_res> _paramlist;
        private System.Timers.Timer filterTimer = new System.Timers.Timer();
        private List<GitHubContent.FileInfo> paramfiles;
        public UAVCANParams(UAVCAN.uavcan can, byte node, List<UAVCAN.uavcan.uavcan_protocol_param_GetSet_res> paramlist)
        {
            _can = can;
            _paramlist = paramlist;
            _node = node;

            InitializeComponent();
        }
        
        public void Activate()
        {
            startup = true;

            _changes.Clear();

            BUT_writePIDS.Enabled = true;
            BUT_rerequestparams.Enabled = true;
            BUT_reset_params.Enabled = true;
            BUT_commitToFlash.Visible = true;
            

            Params.Enabled = false;

            foreach (DataGridViewColumn col in Params.Columns)
            {
                if (!String.IsNullOrEmpty(Settings.Instance["rawparam_" + col.Name + "_width"]))
                {
                    col.Width = Math.Max(50,Settings.Instance.GetInt32("rawparam_" + col.Name + "_width"));
                    log.InfoFormat("{0} to {1}", col.Name, col.Width);
                }
            }

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
                Settings.Instance["rawparam_" + col.Name + "_width"] = col.Width.ToString();
            }
        }

        internal void processToScreen()
        {
            toolTip1.RemoveAll();
            Params.Rows.Clear();

            log.Info("processToScreen");

            var list = new List<string>();
            foreach (string item in _paramlist.Select(a => ASCIIEncoding.ASCII.GetString(a.name, 0, a.name_len)))
                list.Add(item);

            var rowlist = new List<DataGridViewRow>();

            // process hashdefines and update display
            Parallel.ForEach(list, value =>
            {
                if (value == null || value == "")
                    return;

                var row = new DataGridViewRow();
                lock (rowlist)
                    rowlist.Add(row);
                row.CreateCells(Params);
                row.Cells[Command.Index].Value = value;
                row.Cells[Value.Index].Value = _paramlist
                    .First(a => ASCIIEncoding.ASCII.GetString(a.name, 0, a.name_len) == value).value.GetValue();
                row.Cells[Min.Index].Value = _paramlist
                    .First(a => ASCIIEncoding.ASCII.GetString(a.name, 0, a.name_len) == value).min_value.GetValue();
                row.Cells[Max.Index].Value = _paramlist
                    .First(a => ASCIIEncoding.ASCII.GetString(a.name, 0, a.name_len) == value).max_value.GetValue();
                row.Cells[Default.Index].Value = _paramlist
                    .First(a => ASCIIEncoding.ASCII.GetString(a.name, 0, a.name_len) == value).default_value.GetValue();
                var fav_params = Settings.Instance.GetList("fav_params");
                row.Cells[Fav.Index].Value = fav_params.Contains(value);

            });

            log.Info("about to add all");

            Params.SuspendLayout();
            Params.Visible = false;
            Params.Enabled = false;

            Params.Rows.AddRange(rowlist.ToArray());

            log.Info("about to sort");

            Params.SortCompare += OnParamsOnSortCompare;

            Params.Sort(Params.Columns[Command.Index], ListSortDirection.Ascending);

            Params.Enabled = true;
            Params.Visible = true;
            Params.ResumeLayout();

            log.Info("Done");
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

        private void BUT_commitToFlash_Click(object sender, EventArgs e)
        {
            try
            {
                MainV2.comPort.doCommand(MAVLink.MAV_CMD.PREFLIGHT_STORAGE, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
            }
            catch
            {
                CustomMessageBox.Show("Invalid command");
                return;
            }

            CustomMessageBox.Show("Parameters committed to non-volatile memory");
            return;
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
                    loadparamsfromfile(ofd.FileName, true);

                        Activate();
                }
            }
        }

        private void BUT_rerequestparams_Click(object sender, EventArgs e)
        {

            if ( (int)DialogResult.OK ==
                CustomMessageBox.Show(Strings.WarningUpdateParamList, Strings.ERROR, MessageBoxButtons.OKCancel))
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

                startup = false;
            }
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
                            var value = double.Parse(row.Cells[1].Value.ToString());

                            data[row.Cells[0].Value.ToString()] = value;
                        }
                        catch (Exception)
                        {
                            CustomMessageBox.Show(Strings.InvalidNumberEntered + " " + row.Cells[0].Value);
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
            var temp = new List<string>();
            foreach (var item in _changes.Keys)
            {
                temp.Add((string)item);
            }

            temp.SortENABLE();

            foreach (string value in temp)
            {
                try
                {
                    _can.SetParameter(_node, value, _changes[value]);

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
                            if (row.Cells[0].Value.ToString() == value)
                            {
                                row.Cells[1].Style.BackColor = ThemeManager.ControlBGColor;
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
                    CustomMessageBox.Show("Set " + value + " Failed");
                }
            }

            CustomMessageBox.Show("Parameters successfully saved.", "Saved");
        }

        private void chk_modified_CheckedChanged(object sender, EventArgs e)
        {
            FilterTimerOnElapsed(null, null);
        }

        void filterList(string searchfor)
        {
            DateTime start = DateTime.Now;
            Params.SuspendLayout();
            Params.Enabled = false;
            if (searchfor.Length >= 2 || searchfor.Length == 0)
            {
                Regex filter = new Regex(searchfor.Replace("*", ".*").Replace("..*", ".*"), RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);

                foreach (DataGridViewRow row in Params.Rows)
                {
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
            Params.Enabled = true;
            Params.ResumeLayout();

            log.InfoFormat("Filter: {0}ms", (DateTime.Now - start).TotalMilliseconds);
        }

        private void FilterTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            filterTimer.Stop();
            Invoke((Action)delegate
           {
               filterList(txt_search.Text);
           });
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
                    if (row.Cells[0].Value.ToString() == name)
                    {
                        set = true;
                        if (row.Cells[1].Value.ToString() != value)
                            row.Cells[1].Value = value;
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
                CustomMessageBox.Show("Missing " + missed + " params\n"+ list, "No matching Params", MessageBoxButtons.OK);
            }
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

        private void Params_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Only process the Description column
            if (e.RowIndex == -1 || startup)
                return;


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

        private void Params_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1 || startup || e.ColumnIndex != 1)
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

                var value = (string) Params[e.ColumnIndex, e.RowIndex].Value;

                var newvalue = float.Parse(value.Replace(',', '.'), CultureInfo.InvariantCulture);

       

                Params[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.Green;
                _changes[Params[Command.Index, e.RowIndex].Value] =
                    float.Parse(((string) Params[e.ColumnIndex, e.RowIndex].Value));
            }
            catch (Exception)
            {
                Params[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.Red;
            }


            Params.Focus();
        }
        private void txt_search_TextChanged(object sender, EventArgs e)
        {
            filterTimer.Elapsed -= FilterTimerOnElapsed;
            filterTimer.Stop();
            filterTimer.Interval = 500;
            filterTimer.Elapsed += FilterTimerOnElapsed;
            filterTimer.Start();
        }
    }
}