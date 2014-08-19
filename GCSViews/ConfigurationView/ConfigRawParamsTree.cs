using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Utilities;
using log4net;
using MissionPlanner.Controls;
using System.Collections.Generic;
using System.Net;
using System.Globalization;
using BrightIdeasSoftware;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigRawParamsTree : UserControl, IActivate
    {
        private static readonly ILog log =
          LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Changes made to the params between writing to the copter
        readonly Hashtable _changes = new Hashtable();

        static Hashtable tooltips = new Hashtable();

        List<GitHubContent.FileInfo> paramfiles;

        // ?
        internal bool startup = true;

        public struct paramsettings // hk's
        {
            public string name;
            public float minvalue;
            public float maxvalue;
            public float normalvalue;
            public float scale;
            public string desc;
        }


        public ConfigRawParamsTree()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                BUT_writePIDS_Click(null,null);
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
            var ofd = new OpenFileDialog
                          {
                              AddExtension = true,
                              DefaultExt = ".param",
                              RestoreDirectory = true,
                              Filter = "Param List|*.param;*.parm"
                          };
            var dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                loadparamsfromfile(ofd.FileName);
            }
        }

        void loadparamsfromfile(string fn)
        {
            Hashtable param2 = Utilities.ParamFile.loadParamFile(fn);

            foreach (string name in param2.Keys)
            {
                string value = param2[name].ToString();

                checkandupdateparam(name, value);
            }
        }

        void checkandupdateparam(string name, string value)
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
            var sfd = new SaveFileDialog
                          {
                              AddExtension = true,
                              DefaultExt = ".param",
                              RestoreDirectory = true,
                              Filter = "Param List|*.param;*.parm"
                          };

            var dr = sfd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                Hashtable data = new Hashtable();
                foreach (data row in Params.Objects)
                {

                    foreach (var item in row.children) 
                    {
                        if (item.Value != null)
                        {
                            float value = float.Parse(item.Value.ToString());

                            data[item.paramname.ToString()] = value;
                        }
                    }

                    if (row.Value != null)
                    {
                        float value = float.Parse(row.Value.ToString());

                        data[row.paramname.ToString()] = value;
                    }
                }

                Utilities.ParamFile.SaveParamFile(sfd.FileName,data);

            }
        }

        private void BUT_writePIDS_Click(object sender, EventArgs e)
        {
            var temp = (Hashtable)_changes.Clone();

            foreach (string value in temp.Keys)
            {
                try
                {
                    MainV2.comPort.setParam(value, (float)_changes[value]);

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
            Hashtable param2 = new Hashtable();

            var ofd = new OpenFileDialog
                          {
                              AddExtension = true,
                              DefaultExt = ".param",
                              RestoreDirectory = true,
                              Filter = "Param List|*.param;*.parm"
                          };

            var dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                param2 = Utilities.ParamFile.loadParamFile(ofd.FileName);

                ParamCompare paramCompareForm = new ParamCompare(null, MainV2.comPort.MAV.param, param2);

                paramCompareForm.dtlvcallback += paramCompareForm_dtlvcallback;
                
                ThemeManager.ApplyThemeTo(paramCompareForm);
                paramCompareForm.ShowDialog();
            }
        }


        private void BUT_rerequestparams_Click(object sender, EventArgs e)
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
                return;

            if (DialogResult.OK == CustomMessageBox.Show("Update Params\nDON'T DO THIS IF YOU ARE IN THE AIR\n", "Error", MessageBoxButtons.OKCancel))
            {
                ((Control)sender).Enabled = false;

                try
                {
                    MainV2.comPort.getParamList();
                }
                catch (Exception ex)
                {
                    log.Error("Exception getting param list", ex);
                    CustomMessageBox.Show("Error: getting param list", "Error");
                }


                ((Control)sender).Enabled = true;

                startup = true;

                processToScreen();

                startup = false;
            }            
        }

        // from http://stackoverflow.com/questions/2512781/winforms-big-paragraph-tooltip/2512895#2512895
        private const int maximumSingleLineTooltipLength = 50;

        private static string AddNewLinesForTooltip(string text)
        {
            if (text.Length < maximumSingleLineTooltipLength)
                return text;
            int lineLength = (int)Math.Sqrt((double)text.Length) * 2;
            StringBuilder sb = new StringBuilder();
            int currentLinePosition = 0;
            for (int textIndex = 0; textIndex < text.Length; textIndex++)
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

        public class data
        {
            public string root;
            public string paramname;
            public string Value;
            public string unit;
            public string range;
            public string desc;
            public List<data> children = new List<ConfigRawParamsTree.data>();
        }

        internal void processToScreen()
        {
            toolTip1.RemoveAll();
            Params.Items.Clear();

            Params.Objects.ForEach(x => { Params.RemoveObject(x); });

            Params.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;

            Params.CanExpandGetter = delegate(object x) 
            {
                data y = (data)x;
                if (y.children != null && y.children.Count > 0)
                    return true;
                return false;
            };

            Params.ChildrenGetter = delegate(object x)
            {
                data y = (data)x;
                return new ArrayList(y.children);
            };

            //Params.Sort(Params.Columns[0], ListSortDirection.Ascending);

            List<string> sorted = new List<string>();
            foreach (string item in MainV2.comPort.MAV.param.Keys)
                sorted.Add(item);

            sorted.Sort();

            List<data> roots = new List<data>();
            data lastroot = new data();            

            // process hashdefines and update display
            foreach (string value in sorted)
            {
                if (value == null || value == "")
                    continue;

                //System.Diagnostics.Debug.WriteLine("Doing: " + value);

                data data = new ConfigRawParamsTree.data();

                string[] split = value.Split('_');
                data.root = split[0];

                data.paramname = value;
                data.Value = ((float)MainV2.comPort.MAV.param[value]).ToString();
                try
                {
                    string metaDataDescription = ParameterMetaDataRepository.GetParameterMetaData(value, ParameterMetaDataConstants.Description);
                    if (!String.IsNullOrEmpty(metaDataDescription))
                    {
                        string range = ParameterMetaDataRepository.GetParameterMetaData(value, ParameterMetaDataConstants.Range);
                        string options = ParameterMetaDataRepository.GetParameterMetaData(value, ParameterMetaDataConstants.Values);
                        string units = ParameterMetaDataRepository.GetParameterMetaData(value, ParameterMetaDataConstants.Units);

                        data.unit = (units);
                        data.range =( range + options.Replace(","," "));
                        data.desc = (metaDataDescription);

                    }
                    else if (tooltips[value] != null)
                    {
                        //Params.Rows[Params.RowCount - 1].Cells[Command.Index].ToolTipText = ((paramsettings)tooltips[value]).desc;
                        //Params.Rows[Params.RowCount - 1].Cells[RawValue.Index].ToolTipText = ((paramsettings)tooltips[value]).desc;
                       // Params.Rows[Params.RowCount - 1].Cells[Value.Index].ToolTipText = ((paramsettings)tooltips[value]).desc;

                        //  Params.Rows[Params.RowCount - 1].Cells[Desc.Index].Value = "Old: "+((paramsettings)tooltips[value]).desc;

                        //Params.Rows[Params.RowCount - 1].Cells[Default.Index].Value = ((paramsettings)tooltips[value]).normalvalue;
                        //Params.Rows[Params.RowCount - 1].Cells[mavScale.Index].Value = ((paramsettings)tooltips[value]).scale;
                        //Params.Rows[Params.RowCount - 1].Cells[Value.Index].Value = float.Parse(Params.Rows[Params.RowCount - 1].Cells[RawValue.Index].Value.ToString()) / float.Parse(Params.Rows[Params.RowCount - 1].Cells[mavScale.Index].Value.ToString());
                    }

                    
                    if (lastroot.root == split[0])
                    {
                        lastroot.children.Add(data);
                    }
                    else
                    {
                        data newroot = new ConfigRawParamsTree.data() { root = split[0], paramname = split[0] };
                        newroot.children.Add(data);
                        roots.Add(newroot);
                        lastroot = newroot;
                    }
                }
                catch (Exception ex) { log.Error(ex); }

            }

            foreach (var item in roots)
            {
                // if the child has no children, we dont need the root.
                if ( ((List<data>)item.children).Count == 1)
                {
                    Params.AddObject(((List<data>)item.children)[0]);
                    continue;
                }

                Params.AddObject(item);
            }
        }

        public void Activate()
        {
            startup = true;

            this.SuspendLayout();

            processToScreen();

            this.ResumeLayout();

            Common.MessageShowAgain("Raw Param Warning", "All values on this screen are not min/max checked. Please double check your input.\n Please use Standard/Advanced Params for the safe settings");

            CMB_paramfiles.Enabled = false;
            BUT_paramfileload.Enabled = false;


            System.Threading.ThreadPool.QueueUserWorkItem(updatedefaultlist);

            startup = false;
        }

        void updatedefaultlist(object crap)
        {
            try
            {
                if (paramfiles == null)
                {
                    paramfiles = GitHubContent.GetDirContent("diydrones", "ardupilot", "/Tools/Frame_params/",".param");
                }

                this.BeginInvoke((Action)delegate
                {
                    CMB_paramfiles.DataSource = paramfiles.ToArray();
                    CMB_paramfiles.DisplayMember = "name";
                    CMB_paramfiles.Enabled = true;
                    BUT_paramfileload.Enabled = true;
                });
            }
            catch (Exception ex) { log.Error(ex); }
        }

        private void BUT_find_Click(object sender, EventArgs e)
        {
            string searchfor = "";
            InputBox.Show("Search For", "Enter a single word to search for", ref searchfor);

            Params.UseFiltering = true;
            Params.ModelFilter = TextMatchFilter.Regex(this.Params, searchfor.ToLower());
            Params.DefaultRenderer = new HighlightTextRenderer((TextMatchFilter)Params.ModelFilter);

        }

        private void BUT_paramfileload_Click(object sender, EventArgs e)
        {
            string filepath = Application.StartupPath + Path.DirectorySeparatorChar + CMB_paramfiles.Text;

            try
            {

                byte[] data = GitHubContent.GetFileContent("diydrones", "ardupilot", ((GitHubContent.FileInfo)CMB_paramfiles.SelectedValue).path);

                File.WriteAllBytes(filepath, data);

                Hashtable param2 = Utilities.ParamFile.loadParamFile(filepath);

                ParamCompare paramCompareForm = new ParamCompare(null, MainV2.comPort.MAV.param, param2);

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

        void paramCompareForm_dtlvcallback(string param, float value)
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

                foreach (data item2 in item.children)
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
            if (CustomMessageBox.Show("Reset all parameters to default\nAre you sure!!", "Reset", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                MainV2.comPort.setParam(new string[] {"FORMAT_VERSION","SYSID_SW_MREV"}, 0);
                System.Threading.Thread.Sleep(1000);
                MainV2.comPort.doReboot(false);
                MainV2.comPort.BaseStream.Close();

                CustomMessageBox.Show("Your board is now rebooting, You will be required to reconnect to the autopilot.");
            }
        }

        private void Params_CellEditFinishing(object sender, BrightIdeasSoftware.CellEditEventArgs e)
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
                catch { CustomMessageBox.Show("Bad number"); e.Cancel = true; return; }

                if (ParameterMetaDataRepository.GetParameterRange(((data)e.RowObject).paramname, ref min, ref max))
                {
                    if (newvalue > max || newvalue < min)
                    {
                        if (CustomMessageBox.Show(((data)e.RowObject).paramname + " value is out of range. Do you want to continue?", "Out of range", MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            return;
                        }
                    }
                }

                _changes[((data)e.RowObject).paramname] = newvalue;

                ((data)e.RowObject).Value = e.NewValue.ToString();

                var typer =e.RowObject.GetType();

                e.Cancel = true;

                Params.RefreshObject(e.RowObject);
                
            }

        }

        private void Params_FormatRow(object sender, FormatRowEventArgs e)
        {
            if (e != null && e.ListView != null && e.ListView.Items.Count > 0)
            {
                if (_changes.ContainsKey(((data)e.Model).paramname))
                    e.Item.BackColor = Color.Green;
                else
                    e.Item.BackColor = this.BackColor;
            }
        }
    }
}
