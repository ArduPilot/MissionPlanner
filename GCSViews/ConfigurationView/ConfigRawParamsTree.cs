﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BrightIdeasSoftware;
using log4net;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigRawParamsTree : UserControl, IActivate
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

            SuspendLayout();

            processToScreen();

            ResumeLayout();

            Common.MessageShowAgain(Strings.RawParamWarning, Strings.RawParamWarningi);

            CMB_paramfiles.Enabled = false;
            BUT_paramfileload.Enabled = false;

            ThreadPool.QueueUserWorkItem(updatedefaultlist);

            startup = false;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                BUT_writePIDS_Click(null, null);
                return true;
            }

            if (keyData == (Keys.Control | Keys.F))
            {
                BUT_find_Click(null, null);
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
                Filter = "Param List|*.param;*.parm"
            })
            {
                var dr = ofd.ShowDialog();

                if (dr == DialogResult.OK)
                {
                    loadparamsfromfile(ofd.FileName);
                }
            }
        }

        private void loadparamsfromfile(string fn)
        {
            var param2 = ParamFile.loadParamFile(fn);

            foreach (string name in param2.Keys)
            {
                var value = param2[name].ToString();

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
                                var value = float.Parse(item.Value);

                                data[item.paramname] = value;
                            }
                        }

                        if (row.Value != null)
                        {
                            var value = float.Parse(row.Value);

                            data[row.paramname] = value;
                        }
                    }

                    ParamFile.SaveParamFile(sfd.FileName, data);
                }
            }
        }

        private void BUT_writePIDS_Click(object sender, EventArgs e)
        {
            var temp = (Hashtable) _changes.Clone();

            foreach (string value in temp.Keys)
            {
                try
                {
                    MainV2.comPort.setParam(value, (float) _changes[value]);

                    _changes.Remove(value);
                }
                catch
                {
                    CustomMessageBox.Show("Set " + value + " Failed");
                }
            }

            Params.Refresh();
        }

        private void BUT_compare_Click(object sender, EventArgs e)
        {
            var param2 = new Hashtable();

            using (var ofd = new OpenFileDialog
            {
                AddExtension = true,
                DefaultExt = ".param",
                RestoreDirectory = true,
                Filter = "Param List|*.param;*.parm"
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

            if (DialogResult.OK ==
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

            sorted.Sort();

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
                data.Value = ((float) MainV2.comPort.MAV.param[value]).ToString();
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

        private void updatedefaultlist(object crap)
        {
            try
            {
                if (paramfiles == null)
                {
                    paramfiles = GitHubContent.GetDirContent("diydrones", "ardupilot", "/Tools/Frame_params/", ".param");
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

        private void BUT_find_Click(object sender, EventArgs e)
        {
            var searchfor = "";
            InputBox.Show("Search For", "Enter a single word to search for", ref searchfor);

            Params.UseFiltering = true;
            Params.ModelFilter = TextMatchFilter.Regex(Params, searchfor.ToLower());
            Params.DefaultRenderer = new HighlightTextRenderer((TextMatchFilter) Params.ModelFilter);
        }

        private void BUT_paramfileload_Click(object sender, EventArgs e)
        {
            var filepath = Application.StartupPath + Path.DirectorySeparatorChar + CMB_paramfiles.Text;

            try
            {
                var data = GitHubContent.GetFileContent("diydrones", "ardupilot",
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
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    MainV2.comPort.setParam(new[] {"FORMAT_VERSION", "SYSID_SW_MREV"}, 0);
                    Thread.Sleep(1000);
                    MainV2.comPort.doReboot(false);
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
                                "Out of range", MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            return;
                        }
                    }
                }

                _changes[((data) e.RowObject).paramname] = newvalue;

                ((data) e.RowObject).Value = e.NewValue.ToString();

                var typer = e.RowObject.GetType();

                e.Cancel = true;

                Params.RefreshObject(e.RowObject);
            }
        }

        private void Params_FormatRow(object sender, FormatRowEventArgs e)
        {
            if (e != null && e.ListView != null && e.ListView.Items.Count > 0)
            {
                if (_changes.ContainsKey(((data) e.Model).paramname))
                    e.Item.BackColor = Color.Green;
                else
                    e.Item.BackColor = BackColor;
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
    }
}