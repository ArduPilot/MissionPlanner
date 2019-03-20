using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Flurl.Util;
using log4net;
using Microsoft.Scripting.Utils;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigRawParamsTree : MyUserControl, IActivate, IDeactivate
    {
        // from http://stackoverflow.com/questions/2512781/winforms-big-paragraph-tooltip/2512895#2512895
        private const int maximumSingleLineTooltipLength = 50;

        private static readonly ILog log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Hashtable tooltips = new Hashtable();
        // Changes made to the params between writing to the copter
        private readonly Hashtable _changes = new Hashtable();
        private List<GitHubContent.FileInfo> paramfiles;
        // ?
        internal bool startup = true;

        public ConfigRawParamsTree()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            startup = true;

            _changes.Clear();

            BUT_writePIDS.Enabled = MainV2.comPort.BaseStream.IsOpen;
            BUT_rerequestparams.Enabled = MainV2.comPort.BaseStream.IsOpen;
            BUT_reset_params.Enabled = MainV2.comPort.BaseStream.IsOpen;
            BUT_commitToFlash.Visible = MainV2.DisplayConfiguration.displayParamCommitButton;

            SuspendLayout();

            foreach (ColumnHeader col in Params.Columns)
            {
                if (!String.IsNullOrEmpty(Settings.Instance["rawtree_" + col.Text + "_percent"]))
                {
                    col.Width = Math.Max(50,
                        Params.GetPixel(Settings.Instance.GetInt32("rawtree_" + col.Text + "_percent")));
                }
            }

            processToScreen();

            ResumeLayout();

            Common.MessageShowAgain(Strings.RawParamWarning, Strings.RawParamWarningi);

            CMB_paramfiles.Enabled = false;
            BUT_paramfileload.Enabled = false;

            ThreadPool.QueueUserWorkItem(updatedefaultlist);

            startup = false;

            txt_search.Focus();
        }

        public void Deactivate()
        {
            foreach (ColumnHeader col in Params.Columns)
            {
                Settings.Instance["rawtree_" + col.Text + "_percent"] = Params.GetPercent(col.Width).ToString();
            }
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

            foreach (string name in param2.Keys)
            {
                var value = param2[name].ToString();

                if (offline)
                {
                    MainV2.comPort.MAV.param.Add(new MAVLink.MAVLinkParam(name, double.Parse(value),
                        MAVLink.MAV_PARAM_TYPE.REAL32));
                }

                checkandupdateparam(name, value);
            }
        }

        private void checkandupdateparam(string name, string value)
        {
            if (name == "SYSID_SW_MREV")
                return;
            if (name == "WP_TOTAL")
                return;
            if (name == "CMD_TOTAL")
                return;
            if (name == "FENCE_TOTAL")
                return;
            if (name == "SYS_NUM_RESETS")
                return;
            if (name == "ARSPD_OFFSET")
                return;
            if (name == "GND_ABS_PRESS")
                return;
            if (name == "GND_TEMP")
                return;
            if (name == "CMD_INDEX")
                return;
            if (name == "LOG_LASTFILE")
                return;
            if (name == "FORMAT_VERSION")
                return;

            paramCompareForm_dtlvcallback(name, float.Parse(value));
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
                    foreach (data row in Params.Objects)
                    {
                        foreach (var item in row.children)
                        {
                            if (item.Value != null)
                            {
                                var value = double.Parse(item.Value);

                                data[item.paramname] = value;
                            }
                        }

                        if (row.Value != null)
                        {
                            var value = double.Parse(row.Value);

                            data[row.paramname] = value;
                        }
                    }

                    ParamFile.SaveParamFile(sfd.FileName, data);
                }
            }
        }

        private void BUT_writePIDS_Click(object sender, EventArgs e)
        {
            if (Common.MessageShowAgain("Write Raw Params Tree", "Are you Sure?") != DialogResult.OK)
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
                    if (MainV2.comPort.BaseStream == null || !MainV2.comPort.BaseStream.IsOpen)
                    {
                        CustomMessageBox.Show("Your are not connected", Strings.ERROR);
                        return;
                    }

                    MainV2.comPort.setParam(value, (float) _changes[value]);

                    _changes.Remove(value);
                }
                catch
                {
                    CustomMessageBox.Show("Set " + value + " Failed");
                }
            }

            Params.Refresh();
            CustomMessageBox.Show("Parameters successfully saved.", "Saved");
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

                    var paramCompareForm = new ParamCompare(null, MainV2.comPort.MAV.param, param2);

                    paramCompareForm.dtlvcallback += paramCompareForm_dtlvcallback;

                    ThemeManager.ApplyThemeTo(paramCompareForm);
                    paramCompareForm.ShowDialog();
                }
            }
        }

        private void BUT_rerequestparams_Click(object sender, EventArgs e)
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
                return;

            if (!MainV2.comPort.MAV.cs.armed || (int)DialogResult.OK ==
                CustomMessageBox.Show(Strings.WarningUpdateParamList, Strings.ERROR, MessageBoxButtons.OKCancel))
            {
                ((Control) sender).Enabled = false;

                try
                {
                    MainV2.comPort.getParamList();
                }
                catch (Exception ex)
                {
                    log.Error("Exception getting param list", ex);
                    CustomMessageBox.Show(Strings.ErrorReceivingParams, Strings.ERROR);
                }


                ((Control) sender).Enabled = true;

                startup = true;

                processToScreen();

                startup = false;
            }
        }

        private static string AddNewLinesForTooltip(string text)
        {
            if (text.Length < maximumSingleLineTooltipLength)
                return text;
            var lineLength = (int) Math.Sqrt(text.Length)*2;
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
            Params.Items.Clear();

            Params.Objects.ForEach(x => { Params.RemoveObject(x); });

            Params.CellEditActivation = ObjectListView.CellEditActivateMode.SingleClick;

            Params.CanExpandGetter = delegate(object x)
            {
                var y = (data) x;
                if (y.children != null && y.children.Count > 0)
                    return true;
                return false;
            };

            Params.ChildrenGetter = delegate(object x)
            {
                var y = (data) x;
                return new ArrayList(y.children);
            };

            //Params.Sort(Params.Columns[0], ListSortDirection.Ascending);

            var sorted = new List<string>();
            foreach (string item in MainV2.comPort.MAV.param.Keys)
                sorted.Add(item);

            sorted.Sort(ComparisonTree);

            var roots = new List<data>();
            var lastroot = new data();

            // process hashdefines and update display
            foreach (var value in sorted)
            {
                if (value == null || value == "")
                    continue;

                //System.Diagnostics.Debug.WriteLine("Doing: " + value);

                var data = new data();

                var split = value.Split('_');
                data.root = split[0];

                data.paramname = value;
                data.Value = MainV2.comPort.MAV.param[value].ToString();
                try
                {
                    var metaDataDescription = ParameterMetaDataRepository.GetParameterMetaData(value,
                        ParameterMetaDataConstants.Description, MainV2.comPort.MAV.cs.firmware.ToString());
                    if (!string.IsNullOrEmpty(metaDataDescription))
                    {
                        var range = ParameterMetaDataRepository.GetParameterMetaData(value,
                            ParameterMetaDataConstants.Range, MainV2.comPort.MAV.cs.firmware.ToString());
                        var options = ParameterMetaDataRepository.GetParameterMetaData(value,
                            ParameterMetaDataConstants.Values, MainV2.comPort.MAV.cs.firmware.ToString());
                        var units = ParameterMetaDataRepository.GetParameterMetaData(value,
                            ParameterMetaDataConstants.Units, MainV2.comPort.MAV.cs.firmware.ToString());

                        data.unit = (units);
                        data.range = (range + options.Replace(",", " "));
                        data.desc = (metaDataDescription);
                    }

                    if (lastroot.root == split[0])
                    {
                        lastroot.children.Add(data);
                    }
                    else
                    {
                        var newroot = new data {root = split[0], paramname = split[0]};
                        newroot.children.Add(data);
                        roots.Add(newroot);
                        lastroot = newroot;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }

            foreach (var item in roots)
            {
                // if the child has no children, we dont need the root.
                if (item.children.Count == 1)
                {
                    Params.AddObject(item.children[0]);
                    continue;
                }

                Params.AddObject(item);
            }
        }

        private int ComparisonTree(string s, string s1)
        {
            var list = Settings.Instance.GetList("fav_params");

            var fav1 = list.Contains(s);
            var fav2 = list.Contains(s1);

            var ans = s.CompareTo(s1);

            // both fav use string compare
            if (fav1 == fav2)
                return ans;

            // fav1 is greater
            if (fav1 && !fav2)
                return -1;

            // fav1 is not greater
            if (!fav1 && fav2)
                return 1;

            return ans;
        }

        private void updatedefaultlist(object crap)
        {
            try
            {
                if (paramfiles == null)
                {
                    paramfiles = GitHubContent.GetDirContent("ardupilot", "ardupilot", "/Tools/Frame_params/", ".param");
                }

                BeginInvoke((Action) delegate
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
            if (searchfor.Length >= 2 || searchfor.Length == 0)
            {
                var expanded = Params.ExpandedObjects.OfType<object>().Where((o, i) =>
                {
                    var count = Params.VirtualListDataSource.GetObjectCount();
                    for (int a = 0; a < count; a++)
                    {
                        var obj = Params.VirtualListDataSource.GetNthObject(a);
                        if (obj == o)
                            return true;
                    }

                    return false;
                }) .ToArray();

                Params.Visible = false;
                Params.UseFiltering = false;
                Params.ExpandAll();
                Params.ModelFilter = TextMatchFilter.Regex(Params, searchfor.Replace("*", ".*").Replace("..*", ".*").ToLower());
                Params.DefaultRenderer = new HighlightTextRenderer((TextMatchFilter) Params.ModelFilter);
                Params.UseFiltering = true;

                if (Params.Items.Count > 0)
                {
                    if(searchfor.Length == 0)
                        Params.CollapseAll();

                    foreach (var row in expanded)
                    {
                        Params.Expand(row);
                    }
                }
                Params.Visible = true;
            }

            if (chk_modified.Checked)
            {
                var filter = String.Format("({0})", String.Join("|", _changes.Keys.Select(a => a.ToString())));

                Params.ModelFilter = TextMatchFilter.Regex(Params, filter);
                Params.DefaultRenderer = new HighlightTextRenderer((TextMatchFilter)Params.ModelFilter);
                Params.UseFiltering = true;
            }
        }

        private void BUT_paramfileload_Click(object sender, EventArgs e)
        {
            var filepath = Settings.GetUserDataDirectory() + CMB_paramfiles.Text;

            try
            {
                var data = GitHubContent.GetFileContent("ardupilot", "ardupilot",
                    ((GitHubContent.FileInfo) CMB_paramfiles.SelectedValue).path);

                File.WriteAllBytes(filepath, data);

                var param2 = ParamFile.loadParamFile(filepath);

                var paramCompareForm = new ParamCompare(null, MainV2.comPort.MAV.param, param2);

                paramCompareForm.dtlvcallback += paramCompareForm_dtlvcallback;

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

        private void paramCompareForm_dtlvcallback(string param, float value)
        {
            foreach (data item in Params.Objects)
            {
                if (item.paramname == param)
                {
                    item.Value = value.ToString();
                    _changes[param] = value;
                    Params.RefreshObject(item);
                    Params.Expand(item);
                    break;
                }

                foreach (var item2 in item.children)
                {
                    if (item2.paramname == param)
                    {
                        item2.Value = value.ToString();
                        _changes[param] = value;
                        Params.RefreshObject(item2);
                        Params.Expand(item2);
                        break;
                    }
                }
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
                    MainV2.comPort.setParam(new[] {"FORMAT_VERSION", "SYSID_SW_MREV"}, 0);
                    Thread.Sleep(1000);
                    MainV2.comPort.doReboot(false, true);
                    MainV2.comPort.BaseStream.Close();

                    CustomMessageBox.Show(
                        "Your board is now rebooting, You will be required to reconnect to the autopilot.");
                }
                catch
                {
                    CustomMessageBox.Show(Strings.ErrorCommunicating, Strings.ERROR);
                }
            }
        }

        private void Params_CellEditFinishing(object sender, CellEditEventArgs e)
        {
            if (e.NewValue != e.Value && e.Cancel == false)
            {
                Console.WriteLine(e.NewValue + " " + e.NewValue.GetType());

                double min = 0;
                double max = 0;

                float newvalue = 0;

                try
                {
                    newvalue = float.Parse(e.NewValue.ToString());
                }
                catch
                {
                    CustomMessageBox.Show("Bad number");
                    e.Cancel = true;
                    return;
                }

                if (ParameterMetaDataRepository.GetParameterRange(((data) e.RowObject).paramname, ref min, ref max,
                    MainV2.comPort.MAV.cs.firmware.ToString()))
                {
                    if (newvalue > max || newvalue < min)
                    {
                        if (
                            CustomMessageBox.Show(
                                ((data) e.RowObject).paramname + " value is out of range. Do you want to continue?",
                                "Out of range", MessageBoxButtons.YesNo) == (int)DialogResult.No)
                        {
                            return;
                        }
                    }
                }

                // add to change record
                _changes[((data) e.RowObject).paramname] = newvalue;

                // update underlying data
                ((data) e.RowObject).Value = e.NewValue.ToString();


                e.Cancel = false;

                // refresh from underlying data
                Params.RefreshObject(e.RowObject);
            }
        }

        private void Params_FormatRow(object sender, FormatRowEventArgs e)
        {
            var shortv = _changes.Keys.Select(a => {
                if (a.ToString().Contains('_'))
                    return a.ToString().Substring(0, a.ToString().IndexOf('_'));
                return "";
            });

            if (e != null && e.ListView != null && e.ListView.Items.Count > 0)
            {
                var it = ((data) e.Model);
                if (_changes.ContainsKey(it.paramname) || shortv.Contains(it.paramname))
                {
                    e.Item.BackColor = Color.Green;
                }
                else
                    e.Item.BackColor = BackColor;
            }

            var item = e.Model as data;
            if (item != null)
            {
                //olvColumn4.WordWrap = true;
                //olvColumn5.WordWrap = true;
                //Params.RowHeight = 26;
                return;

               var size = TextRenderer.MeasureText(item.desc, Params.Font, new Size(olvColumn5.Width, 26), TextFormatFlags.WordBreak);
                if(size.Height >= Params.RowHeight)
                    Params.RowHeight = Math.Min(size.Height, 50);

                size = TextRenderer.MeasureText(item.range, Params.Font, new Size(olvColumn4.Width, 26), TextFormatFlags.WordBreak);
                if (size.Height >= Params.RowHeight)
                    Params.RowHeight = Math.Min(size.Height,50);
            }
        }

        public struct paramsettings // hk's
        {
            public string desc;
            public float maxvalue;
            public float minvalue;
            public string name;
            public float normalvalue;
            public float scale;
        }

        public class data
        {
            public List<data> children = new List<data>();
            public string desc;
            public string paramname;
            public string range;
            public string root;
            public string unit;
            public string Value;
        }

        private System.Timers.Timer filterTimer = new System.Timers.Timer();

        private void txt_search_TextChanged(object sender, EventArgs e)
        {
            filterTimer.Elapsed -= FilterTimerOnElapsed;
            filterTimer.Stop();
            filterTimer.Interval = 500;
            filterTimer.Elapsed += FilterTimerOnElapsed;
            filterTimer.Start();
        }

        private void FilterTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            filterTimer.Stop();
            Invoke((Action)delegate
            {
                filterList(txt_search.Text);
            });
        }

        private void Params_CellClick(object sender, CellClickEventArgs e)
        {
            // Only process the Description column
            if (e.RowIndex == -1 || startup)
                return;

            if (e.ColumnIndex == olvColumn2.Index)
            {
                var it = ((data)e.Model);
                var check = it.Value;
                var name = it.paramname;

                var availableBitMask =
                    ParameterMetaDataRepository.GetParameterBitMaskInt(name, MainV2.comPort.MAV.cs.firmware.ToString());
                if (availableBitMask.Count > 0)
                {
                    var mcb = new MavlinkCheckBoxBitMask();
                    var list = new MAVLink.MAVLinkParamList();
                    list.Add(new MAVLink.MAVLinkParam(name, double.Parse(check.ToString(), CultureInfo.InvariantCulture),
                        MAVLink.MAV_PARAM_TYPE.INT32));
                    mcb.setup(name, list);
                    mcb.ValueChanged += (o, s, value) =>
                    {
                        paramCompareForm_dtlvcallback(s, int.Parse(value));
                            ((data) e.HitTest.RowObject).Value = value;
                        Params.RefreshItem(e.HitTest.Item);
                        e.HitTest.SubItem.Text = value;
                        Params.CancelCellEdit();
                        e.Handled = true;
                        mcb.Focus();
                    };
                    var frm = mcb.ShowUserControl();
                    frm.TopMost = true;
                }
            }


            try
            {
                string descStr = e.SubItem.ModelValue.ToString();
                ConfigRawParams.CheckForUrlAndLaunchInBrowser(descStr);
            }
            catch { }
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

        private void Params_CellToolTipShowing(object sender, ToolTipShowingEventArgs e)
        {

            
        }

        private void Params_CellOver(object sender, CellOverEventArgs e)
        {
            if(e.ColumnIndex == 4 || e.ColumnIndex == 5)
            {
           //     toolTip1.Show(e.HitTest.Item.Text, this.Parent, 3000);
            }
            }

        private void chk_modified_CheckedChanged(object sender, EventArgs e)
        {
            FilterTimerOnElapsed(null, null);
        }
    }
}