using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using log4net;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigRawParams : UserControl, IActivate, IDeactivate
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

        public ConfigRawParams()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            startup = true;

            CMB_paramfiles.Enabled = false;
            BUT_paramfileload.Enabled = false;
            ThreadPool.QueueUserWorkItem(updatedefaultlist);

            SuspendLayout();

            foreach (DataGridViewColumn col in Params.Columns)
            {
                if (!String.IsNullOrEmpty(MainV2.getConfig("rawparam_" + col.Name + "_width")))
                {
                    col.Width = int.Parse(MainV2.config["rawparam_" + col.Name + "_width"].ToString());
                }
            }

            processToScreen();

            ResumeLayout();

            Common.MessageShowAgain(Strings.RawParamWarning, Strings.RawParamWarningi);

            startup = false;

            txt_search.Focus();
        }

        public void Deactivate()
        {
            foreach (DataGridViewColumn col in Params.Columns)
            {
                MainV2.config["rawparam_" + col.Name + "_width"] = col.Width;
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
                        if (row.Cells[1].Value.ToString() != value)
                            row.Cells[1].Value = value;
                        break;
                    }
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

            var temp = (Hashtable) _changes.Clone();

            foreach (string value in temp.Keys)
            {
                try
                {
                    MainV2.comPort.setParam(value, (float) _changes[value]);

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

                if (ParameterMetaDataRepository.GetParameterRange(Params[Command.Index, e.RowIndex].Value.ToString(),
                    ref min, ref max, MainV2.comPort.MAV.cs.firmware.ToString()))
                {
                    if (newvalue > max || newvalue < min)
                    {
                        if (
                            CustomMessageBox.Show(
                                Params[Command.Index, e.RowIndex].Value +
                                " value is out of range. Do you want to continue?", "Out of range",
                                MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            return;
                        }
                    }
                }

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
            Params.Rows.Clear();


            //Params.Sort(Params.Columns[0], ListSortDirection.Ascending);

            var sorted = new List<string>();
            foreach (string item in MainV2.comPort.MAV.param.Keys)
                sorted.Add(item);

            sorted.Sort();

            var rowlist = new List<DataGridViewRow>();

            // process hashdefines and update display
            foreach (var value in sorted)
            {
                if (value == null || value == "")
                    continue;

                //System.Diagnostics.Debug.WriteLine("Doing: " + value);

                var row = new DataGridViewRow();
                rowlist.Add(row);
                row.CreateCells(Params);
                row.Cells[Command.Index].Value = value;
                row.Cells[Value.Index].Value = ((float) MainV2.comPort.MAV.param[value]).ToString();
                try
                {
                    var metaDataDescription = ParameterMetaDataRepository.GetParameterMetaData(value,
                        ParameterMetaDataConstants.Description, MainV2.comPort.MAV.cs.firmware.ToString());
                    if (!string.IsNullOrEmpty(metaDataDescription))
                    {
                        row.Cells[Command.Index].ToolTipText = metaDataDescription;
                        row.Cells[Value.Index].ToolTipText = metaDataDescription;

                        var range = ParameterMetaDataRepository.GetParameterMetaData(value,
                            ParameterMetaDataConstants.Range, MainV2.comPort.MAV.cs.firmware.ToString());
                        var options = ParameterMetaDataRepository.GetParameterMetaData(value,
                            ParameterMetaDataConstants.Values, MainV2.comPort.MAV.cs.firmware.ToString());
                        var units = ParameterMetaDataRepository.GetParameterMetaData(value,
                            ParameterMetaDataConstants.Units, MainV2.comPort.MAV.cs.firmware.ToString());

                        row.Cells[Units.Index].Value = units;
                        row.Cells[Options.Index].Value = range + options.Replace(",", " ");
                        row.Cells[Desc.Index].Value = metaDataDescription;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }

            Params.Rows.AddRange(rowlist.ToArray());
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

        void filterList(string searchfor)
        {
            if (searchfor.Length >= 2 || searchfor.Length == 0)
            {
                Regex filter = new Regex(searchfor,RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);

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

                Params.Refresh();
            }
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
                catch (Exception ex)
                {
                    log.Error(ex);
                    CustomMessageBox.Show(Strings.ErrorCommunicating + "\n" + ex, Strings.ERROR);
                }
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

        private void txt_search_TextChanged(object sender, EventArgs e)
        {
            filterList(txt_search.Text);
        }

        private void Params_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Only process the Description column
            if (e.RowIndex == -1 || startup || e.ColumnIndex != 4)
                return;

            try
            {
                string descStr = Params[e.ColumnIndex, e.RowIndex].Value.ToString();
                CheckForUrlAndLaunchInBrowser(descStr);
            }
            catch { }
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
    }
}