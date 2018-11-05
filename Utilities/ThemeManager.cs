using System;
using System.Drawing;
using System.Windows.Forms;
using MissionPlanner.Controls.BackstageView;
using log4net;
using MissionPlanner.Controls;
using System.IO;
using System.Linq;
using System.Reflection;
using BrightIdeasSoftware;

namespace MissionPlanner.Utilities
{
    /// <summary>
    /// Helper class for the stylng 'theming' of forms and controls, and provides MessageBox
    /// replacements which are also styled
    /// </summary>
    public class ThemeManager
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Themes _currentTheme = Themes.BurntKermit;

        public static Themes CurrentTheme
        {
            get { return _currentTheme; }
        }

        public enum Themes
        {
            /// <summary>
            /// no theme - standard Winforms appearance
            /// </summary>
            None,

            /// <summary>
            /// Standard Planner Charcoal & Green colours
            /// </summary>
            BurntKermit,
            HighContrast,
            Test,
            Custom,
        }

        // Initialize to the default theme (BurntKermit)
        public static Color BGColor = Color.FromArgb(0x26, 0x27, 0x28);
        public static Color ControlBGColor = Color.FromArgb(0x43, 0x44, 0x45);
        public static Color TextColor = Color.White;
        public static Color BGColorTextBox;
        public static Color ButBG;
        public static Color ButBGBot;
        public static Color ButBorder;
        public static Color ProgressBarColorTop;
        public static Color ProgressBarColorBot;
        public static Color ProgressBarOutlineColor;
        public static Color ColorNotEnabled;
        public static Color ColorMouseOver;
        public static Color ColorMouseDown;
        public static Color BannerColor1;
        public static Color BannerColor2;
        public static Color ButtonTextColor;
        public static Color CurrentPPMBackground;
        public static Color ZedGraphChartFill;
        public static Color ZedGraphPaneFill;
        public static Color ZedGraphLegendFill;
        public static Color RTBForeColor;
        public static Color BSVButtonAreaBGColor;
        public static Color UnselectedTextColour;
        public static Color HorizontalPBValueColor;


        /// <summary>
        /// Change the current theme. Existing controls are not affected
        /// </summary>
        /// <param name="theme"></param>
        public static void SetTheme(Themes theme)
        {
            log.Debug("Theme set to " + Enum.GetName(typeof (Themes), theme));
            _currentTheme = theme;
        }

        public static void CustomColor()
        {
            new ThemeColors().ShowDialog();
        }

        public static void ApplyThemeTo(object control)
        {
            if (control is Control)
                ApplyThemeTo(control as Control);
        }

        /// <summary>
        /// Will recursively apply the current theme to 'control'
        /// </summary>
        /// <param name="control"></param>
        public static void ApplyThemeTo(Control control)
        {
            if(control is ContainerControl)
                ((ContainerControl)control).AutoScaleMode = AutoScaleMode.None;

            switch (_currentTheme)
            {
                case Themes.BurntKermit:
                    SetBurntKermitColors();
                    ApplyTheme(control, 0);
                    break;

                case Themes.HighContrast:
                    SetHighContrastColours();
                    ApplyTheme(control, 0);
                    break;

                case Themes.Test:
                    ApplyTestTheme(control, 0);
                    break;

                case Themes.Custom:
                    ApplyCustomTheme(control, 0);
                    break;

                default:
                    break;
            }
        }


        public static void doxamlgen()
        {
            var asm = Assembly.GetExecutingAssembly();

            var temp = asm.GetTypes().Select(a =>
            {
                if (a.IsSubclassOf(typeof(Control)))
                {
                    try
                    {
                        return (Control) Activator.CreateInstance(a);
                    }
                    catch { }
                }

                return null;
            }).ToList();

            foreach (var ctl in temp)
            {
                if(ctl == null)
                    continue;

                xaml(ctl);

                html(ctl);
            }
        }

        public static void html(Control control)
        {
            Type ty = control.GetType();

            StreamWriter st = new StreamWriter(File.Open(ty.FullName + ".html", FileMode.Create));

            dohtmlctls(control, st);

            st.Close();
        }

        private static void dohtmlctls(Control control, StreamWriter st, int x=0, int y=0)
        {
            foreach (Control ctl in control.Controls)
            {
                var font = "font-family:" + ctl.Font.FontFamily + ";";
                var fontsize = "font-size:" + ctl.Font.SizeInPoints + "pt;";
                var fontcol = "color:" + System.Drawing.ColorTranslator.ToHtml(ctl.ForeColor) + ";";
                var bgcol = "background-color:" + System.Drawing.ColorTranslator.ToHtml(ctl.BackColor) + ";";


                st.WriteLine(@"<div class='" + ctl.GetType() + " " + ctl.Name + @"' " +
                             "style='" + font + fontsize + fontcol + bgcol +
                             "overflow:hidden;position: absolute; top: " + (y + ctl.Location.Y) + "; left: " +
                             (x + ctl.Location.X) + ";width:" + ctl.Width + ";height:" + ctl.Height + ";' >" +
                             ctl.Text);

                if (ctl.Controls.Count > 0)
                {
                    dohtmlctls(ctl, st, 0, 0);
                }

                st.WriteLine(@"</div>");
            }
        }

        static object locker = new object();

        public static void xaml(Control control)
        {
            try
            {
                lock (locker)
                {
                    Type ty = control.GetType();

                    StreamWriter st = new StreamWriter(File.Open(ty.FullName + ".xaml", FileMode.Create));

                    string header = @"<UserControl x:Class=""" + ty.FullName + @""" d:DesignHeight=""" + control.Height +
                                    @""" d:DesignWidth=""" + control.Width + @"""
xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
xmlns:xctk=""http://schemas.xceed.com/wpf/xaml/toolkit""
xmlns:Custom=""http://schemas.microsoft.com/wpf/2008/toolkit""
xmlns:BackstageView=""clr-namespace:MissionPlanner.Controls.BackstageView""
xmlns:Controls=""clr-namespace:MissionPlanner.Controls""
xmlns:GCSViews=""clr-namespace:MissionPlanner.GCSViews""
xmlns:Wizard=""clr-namespace:MissionPlanner.Wizard""
xmlns:ConfigurationView=""clr-namespace:MissionPlanner.GCSViews.ConfigurationView""
mc:Ignorable=""d""
> <Grid>";

                    st.Write(header);

                    doxamlctls(control, st);

                    string footer = "</Grid></UserControl>";

                    st.Write(footer);

                    st.Close();
                }
            }
            catch
            {
            }
        }

        private static void doxamlctls(Control control, StreamWriter st)
        {
            foreach (Control ctl in control.Controls)
            {
                if (ctl is QuickView || ctl is ServoOptions || ctl is ModifyandSet
                    || ctl is Coords /*|| ctl is AGaugeApp.AGauge*/|| ctl is MissionPlanner.Controls.HUD)
                {
                    //   st.WriteLine(@"<WindowsFormsHost HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" + ctl.Location.X + "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width + @""" Height=""" + ctl.Height + @""">");

                    string[] names = ctl.GetType().FullName.Split(new char[] {'.'});

                    string name = names[names.Length - 2] + ":" + names[names.Length - 1];

                    st.WriteLine(@"<" + name + @" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" +
                                 ctl.Location.X + "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width +
                                 @""" Height=""" + ctl.Height + @"""></" + name + ">");

                    //st.WriteLine(@"</WindowsFormsHost>");
                }
                else if (ctl is Label)
                {
                    st.WriteLine(@"<Label Name=""" + ctl.Name +
                                 @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" + ctl.Location.X +
                                 "," + (ctl.Location.Y - 10) + @",0,0"" FontFamily=""Microsoft Sans Serif"" >" +
                                 ctl.Text + "</Label>");
                }
                else if (ctl is MyLabel)
                {
                    st.WriteLine(@"<Label Name=""" + ctl.Name +
                                 @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" + ctl.Location.X +
                                 "," + (ctl.Location.Y - 10) + @",0,0"" FontFamily=""Microsoft Sans Serif"" >" +
                                 ctl.Text + "</Label>");
                }
                else if (ctl is ComboBox)
                {
                    st.WriteLine(@"<ComboBox Name=""" + ctl.Name +
                                 @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" + ctl.Location.X +
                                 "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width + @""">" + ctl.Text +
                                 "</ComboBox>");
                }
                else if (ctl is TextBox)
                {
                    st.WriteLine(@"<TextBox Name=""" + ctl.Name +
                                 @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" + ctl.Location.X +
                                 "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width + @""">" + ctl.Text +
                                 "</TextBox>");
                }
                else if (ctl is NumericUpDown)
                {
                    st.WriteLine(@"<xctk:DecimalUpDown Name=""" + ctl.Name +
                                 @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" + ctl.Location.X +
                                 "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width + @"""></xctk:DecimalUpDown>");
                }
                else if (ctl is RichTextBox)
                {
                    st.WriteLine(@"<TextBlock Name=""" + ctl.Name +
                                 @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" + ctl.Location.X +
                                 "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width + @""" Height=""" + ctl.Height +
                                 @""">" + ctl.Text + "</TextBlock>");
                }
                else if (ctl is MyButton)
                {
                    st.WriteLine(@"<Button Name=""" + ctl.Name +
                                 @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" + ctl.Location.X +
                                 "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width + @""">" +
                                 ctl.Text.Replace("&", "") + "</Button>");
                }
                else if (ctl is Button)
                {
                    st.WriteLine(@"<Button Name=""" + ctl.Name +
                                 @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" + ctl.Location.X +
                                 "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width + @""">" + ctl.Text + "</Button>");
                }
                else if (ctl is CheckBox)
                {
                    st.WriteLine(@"<CheckBox Name=""" + ctl.Name +
                                 @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" + ctl.Location.X +
                                 "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width + @""">" + ctl.Text +
                                 "</CheckBox>");
                }
                else if (ctl is RadioButton)
                {
                    st.WriteLine(@"<RadioButton Name=""" + ctl.Name +
                                 @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" + ctl.Location.X +
                                 "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width + @""">" + ctl.Text +
                                 "</RadioButton>");
                }
                else if (ctl is PictureBox)
                {
                    st.WriteLine(@"<Image Name=""" + ctl.Name +
                                 @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" + ctl.Location.X +
                                 "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width + @""" Height=""" + ctl.Height +
                                 @""">" + ctl.Text + "</Image>");
                }
                else if (ctl is TrackBar)
                {
                    if (((TrackBar) ctl).Orientation == Orientation.Horizontal)
                        st.WriteLine(@"<Slider Name=""" + ctl.Name +
                                     @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" +
                                     ctl.Location.X + "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width +
                                     @""" Height=""" + ctl.Height + @""">" + ctl.Text + "</Slider>");
                    else
                        st.WriteLine(@"<Slider Name=""" + ctl.Name +
                                     @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" +
                                     ctl.Location.X + "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width +
                                     @""" Height=""" + ctl.Height + @""" Orientation=""Vertical"">" + ctl.Text +
                                     "</Slider>");
                }
                else if (ctl is VerticalProgressBar || ctl is VerticalProgressBar2)
                {
                    st.WriteLine(@"<ProgressBar Name=""" + ctl.Name +
                                 @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" + ctl.Location.X +
                                 "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width + @""" Height=""" + ctl.Height +
                                 @""" Orientation=""Vertical"">" + ctl.Text + "</ProgressBar>");
                }
                else if (ctl is ProgressBar || ctl is HorizontalProgressBar2 || ctl is HorizontalProgressBar)
                {
                    st.WriteLine(@"<ProgressBar Name=""" + ctl.Name +
                                 @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" + ctl.Location.X +
                                 "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width + @""" Height=""" + ctl.Height +
                                 @""">" + ctl.Text + "</ProgressBar>");
                }
                else if (ctl is DataGridView)
                {
                    st.WriteLine(@"<Custom:DataGrid Name=""" + ctl.Name +
                                 @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" + ctl.Location.X +
                                 "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width + @""">" + ctl.Text +
                                 "</Custom:DataGrid>");
                }
                else if (ctl is GroupBox)
                {
                    st.WriteLine(@"<Grid Name=""" + ctl.Name +
                                 @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" + ctl.Location.X +
                                 "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width + @""" Height=""" + ctl.Height +
                                 @""">");

                    if (ctl.Controls.Count > 0)
                        doxamlctls(ctl, st);

                    st.WriteLine(@"</Grid>");
                }
                else if (ctl is TabControl)
                {
                    st.WriteLine(@"<TabControl Name=""" + ctl.Name +
                                 @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" + ctl.Location.X +
                                 "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width + @""" Height=""" + ctl.Height +
                                 @""">");

                    if (ctl.Controls.Count > 0)
                        doxamlctls(ctl, st);

                    st.WriteLine(@"</TabControl>");
                }
                else if (ctl is TabPage)
                {
                    // Margin=""" + ctl.Location.X + "," + ctl.Location.Y + @",0,0""
                    st.WriteLine(@"<TabItem Name=""" + ctl.Name + @""" Header=""" + ctl.Text +
                                 @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" ><Grid Width=""" +
                                 ctl.Width + @""" Height=""" + ctl.Height + @""">");

                    if (ctl.Controls.Count > 0)
                        doxamlctls(ctl, st);

                    st.WriteLine(@"</Grid></TabItem>");
                }
                else if (ctl is SplitContainer)
                {
                    st.WriteLine(@"<Grid Name=""" + ctl.Name +
                                 @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" + ctl.Location.X +
                                 "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width + @""" Height=""" + ctl.Height +
                                 @""">");

                    if (ctl.Controls.Count > 0)
                        doxamlctls(ctl, st);

                    st.WriteLine(@"</Grid>");
                }
                else if (ctl is SplitterPanel)
                {
                    st.WriteLine(@"<Grid HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" +
                                 ctl.Location.X + "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width +
                                 @""" Height=""" + ctl.Height + @""">");

                    if (ctl.Controls.Count > 0)
                        doxamlctls(ctl, st);

                    st.WriteLine(@"</Grid>");
                }
                else if (ctl is Panel || ctl is BSE.Windows.Forms.Panel)
                {
                    st.WriteLine(@"<Grid Name=""" + ctl.Name +
                                 @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" + ctl.Location.X +
                                 "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width + @""" Height=""" + ctl.Height +
                                 @""">");

                    if (ctl.Controls.Count > 0)
                        doxamlctls(ctl, st);

                    st.WriteLine(@"</Grid>");
                }
                else
                {
                    //<WindowsFormsHost HorizontalAlignment="Left" Height="185.075" Margin="35.821,477.612,0,0" VerticalAlignment="Top" Width="608.179"/>

                    Console.WriteLine("XAML fail " + ctl.GetType().FullName);
                    if (ctl.Controls.Count > 0)
                    {
                        doxamlctls(ctl, st);
                    }
                }
            }
        }

        private static void ApplyCustomTheme(Control temp, int level)
        {
            if (level == 0)
            {
                temp.BackColor = BGColor;
                temp.ForeColor = TextColor;
            }

            foreach (Control ctl in temp.Controls)
            {
                if (ctl.GetType() == typeof (Panel))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof (GroupBox))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof (TreeView))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                    TreeView txtr = (TreeView) ctl;
                    txtr.LineColor = TextColor;
                }
                else if (ctl.GetType() == typeof (MyLabel))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof (Button))
                {
                    ctl.ForeColor = TextColor;
                    ctl.BackColor = ButBG;
                }
                else if (ctl.GetType() == typeof (MyButton))
                {
                    Controls.MyButton but = (MyButton) ctl;
                    but.BGGradTop = ButBG;
                    try
                    {
                        but.BGGradBot = Color.FromArgb(ButBG.ToArgb() - 0x333333);
                    }
                    catch
                    {
                    }
                    but.TextColor = TextColor;
                    but.Outline = ButBorder;
                }
                else if (ctl.GetType() == typeof (TextBox))
                {
                    ctl.BackColor = ControlBGColor;
                    ctl.ForeColor = TextColor;
                    TextBox txt = (TextBox) ctl;
                    txt.BorderStyle = BorderStyle.None;
                }
                else if (ctl.GetType() == typeof (DomainUpDown))
                {
                    ctl.BackColor = ControlBGColor;
                    ctl.ForeColor = TextColor;
                    DomainUpDown txt = (DomainUpDown) ctl;
                    txt.BorderStyle = BorderStyle.None;
                }
                else if (ctl.GetType() == typeof (GroupBox) || ctl.GetType() == typeof (UserControl))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof (ZedGraph.ZedGraphControl))
                {
                    var zg1 = (ZedGraph.ZedGraphControl) ctl;
                    zg1.GraphPane.Chart.Fill = new ZedGraph.Fill(ControlBGColor);
                    zg1.GraphPane.Fill = new ZedGraph.Fill(BGColor);

                    foreach (ZedGraph.LineItem li in zg1.GraphPane.CurveList)
                        li.Line.Width = 2;

                    zg1.GraphPane.Title.FontSpec.FontColor = TextColor;

                    zg1.GraphPane.XAxis.MajorTic.Color = TextColor;
                    zg1.GraphPane.XAxis.MinorTic.Color = TextColor;
                    zg1.GraphPane.YAxis.MajorTic.Color = TextColor;
                    zg1.GraphPane.YAxis.MinorTic.Color = TextColor;
                    zg1.GraphPane.Y2Axis.MajorTic.Color = TextColor;
                    zg1.GraphPane.Y2Axis.MinorTic.Color = TextColor;

                    zg1.GraphPane.XAxis.MajorGrid.Color = TextColor;
                    zg1.GraphPane.YAxis.MajorGrid.Color = TextColor;
                    zg1.GraphPane.Y2Axis.MajorGrid.Color = TextColor;

                    zg1.GraphPane.YAxis.Scale.FontSpec.FontColor = TextColor;
                    zg1.GraphPane.YAxis.Title.FontSpec.FontColor = TextColor;
                    zg1.GraphPane.Y2Axis.Title.FontSpec.FontColor = TextColor;
                    zg1.GraphPane.Y2Axis.Scale.FontSpec.FontColor = TextColor;

                    zg1.GraphPane.XAxis.Scale.FontSpec.FontColor = TextColor;
                    zg1.GraphPane.XAxis.Title.FontSpec.FontColor = TextColor;

                    zg1.GraphPane.Legend.Fill = new ZedGraph.Fill(ControlBGColor);
                    zg1.GraphPane.Legend.FontSpec.FontColor = TextColor;
                }
                else if (ctl.GetType() == typeof (BSE.Windows.Forms.Panel) || ctl.GetType() == typeof (SplitterPanel))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor; // Color.FromArgb(0xe6, 0xe8, 0xea);
                }
                else if (ctl.GetType() == typeof(RadialGradientBG))
                {
                    var rbg = ctl as RadialGradientBG;
                    rbg.CenterColor = ControlBGColor;
                    rbg.OutsideColor = ButBG;
                }
                else if (ctl.GetType() == typeof(GradientBG))
                {
                    var rbg = ctl as GradientBG;
                    rbg.CenterColor = ControlBGColor;
                    rbg.OutsideColor = ButBG;
                }
                else if (ctl.GetType() == typeof (Form))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                    if (Program.IconFile != null)
                        ((Form)ctl).Icon = Icon.FromHandle(((Bitmap)Program.IconFile).GetHicon());
                }
                else if (ctl.GetType() == typeof (RichTextBox))
                {

                    if ((ctl.Name == "TXT_terminal") && !MainV2.TerminalTheming)
                    {
                        RichTextBox txtr = (RichTextBox)ctl;
                        txtr.BorderStyle = BorderStyle.None;
                        txtr.ForeColor = Color.White;
                        txtr.BackColor = Color.Black;
                    }
                    else
                    {

                        ctl.BackColor = ControlBGColor;
                        ctl.ForeColor = TextColor;
                        RichTextBox txtr = (RichTextBox)ctl;
                    txtr.BorderStyle = BorderStyle.None;
                }
            }
                else if (ctl.GetType() == typeof (CheckedListBox))
                {
                    ctl.BackColor = ControlBGColor;
                    ctl.ForeColor = TextColor;
                    CheckedListBox txtr = (CheckedListBox) ctl;
                    txtr.BorderStyle = BorderStyle.None;
                }
                else if (ctl.GetType() == typeof (TabPage))
                {
                    ctl.BackColor = BGColor; //ControlBGColor
                    ctl.ForeColor = TextColor;
                    TabPage txtr = (TabPage) ctl;
                    txtr.BorderStyle = BorderStyle.None;
                }
                else if (ctl.GetType() == typeof (TabControl))
                {
                    ctl.BackColor = BGColor; //ControlBGColor
                    ctl.ForeColor = TextColor;
                    TabControl txtr = (TabControl) ctl;
                }
                else if (ctl.GetType() == typeof (DataGridView) || ctl.GetType() == typeof(MyDataGridView))
                {
                    ctl.ForeColor = TextColor;
                    DataGridView dgv = (DataGridView) ctl;
                    dgv.EnableHeadersVisualStyles = false;
                    dgv.BorderStyle = BorderStyle.None;
                    dgv.BackgroundColor = BGColor;
                    DataGridViewCellStyle rs = new DataGridViewCellStyle();
                    rs.BackColor = ControlBGColor;
                    rs.ForeColor = TextColor;
                    dgv.RowsDefaultCellStyle = rs;

                    DataGridViewCellStyle hs = new DataGridViewCellStyle(dgv.ColumnHeadersDefaultCellStyle);
                    hs.BackColor = BGColor;
                    hs.ForeColor = TextColor;

                    dgv.ColumnHeadersDefaultCellStyle = hs;
                    dgv.RowHeadersDefaultCellStyle = hs;
                }
                else if (ctl.GetType() == typeof (CheckBox) || ctl.GetType() == typeof (MavlinkCheckBox))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                    CheckBox CHK = (CheckBox) ctl;
                    // CHK.FlatStyle = FlatStyle.Flat;
                }
                else if (ctl.GetType() == typeof (ComboBox) || ctl.GetType() == typeof (MavlinkComboBox))
                {
                    ctl.BackColor = ControlBGColor;
                    ctl.ForeColor = TextColor;
                    ComboBox CMB = (ComboBox) ctl;
                    CMB.FlatStyle = FlatStyle.Flat;
                }
                else if (ctl.GetType() == typeof (NumericUpDown) || ctl.GetType() == typeof (MavlinkNumericUpDown))
                {
                    ctl.BackColor = ControlBGColor;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof (TrackBar))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof (LinkLabel))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                    LinkLabel LNK = (LinkLabel) ctl;
                    LNK.ActiveLinkColor = TextColor;
                    LNK.LinkColor = TextColor;
                    LNK.VisitedLinkColor = TextColor;
                }
                else if (ctl.GetType() == typeof (BackstageView))
                {
                    var bsv = ctl as BackstageView;

                    bsv.BackColor = BGColor;
                    bsv.ButtonsAreaBgColor = ControlBGColor;
                    bsv.HighlightColor2 = Color.FromArgb(0x94, 0xc1, 0x1f);
                    bsv.HighlightColor1 = Color.FromArgb(0x40, 0x57, 0x04);
                    bsv.SelectedTextColor = Color.White;
                    bsv.UnSelectedTextColor = Color.Gray;
                    bsv.ButtonsAreaPencilColor = Color.DarkGray;
                }
                else if (ctl.GetType() == typeof (HorizontalProgressBar2) ||
                         ctl.GetType() == typeof (VerticalProgressBar2))
                {
                    ((HorizontalProgressBar2) ctl).BackgroundColor = ControlBGColor;
                    ((HorizontalProgressBar2) ctl).ValueColor = Color.FromArgb(148, 193, 31);
                }
                else if (ctl.GetType() == typeof(MyProgressBar))
                {
                    ((MyProgressBar)ctl).BGGradBot = ControlBGColor;
                    ((MyProgressBar)ctl).BGGradTop = BGColor;
                }

                if (ctl.Controls.Count > 0)
                    ApplyCustomTheme(ctl, 1);
            }
        }

        private static void ApplyTestTheme(Control temp, int level)
        {
            foreach (Control ctl in temp.Controls)
            {
                if (ctl.GetType() == typeof (MyButton))
                {
                    Controls.MyButton but = (MyButton) ctl;
                    but.BGGradTop = SystemColors.ControlLight;
                    but.BGGradBot = SystemColors.ControlDark;
                    but.TextColor = SystemColors.ControlText;
                    but.Outline = SystemColors.ControlDark;
                }

                if (ctl.Controls.Count > 0)
                    ApplyTestTheme(ctl, 1);
            }
        }

        private static void SetBurntKermitColors()
        {
            BGColor = Color.FromArgb(0x26, 0x27, 0x28);                     // This changes the colour of the main menu background
            ControlBGColor = Color.FromArgb(0x43, 0x44, 0x45);              // This changes the colour of the sub menu backgrounds
            TextColor = Color.White;                                        // This changes the colour of text
            BGColorTextBox = Color.FromArgb(0x43, 0x44, 0x45);              // This changes the colour of the background of textboxes
            ButtonTextColor = Color.FromArgb(64, 87, 4);                    // This changes the colour of button text
            ButBG = Color.FromArgb(148, 193, 31);                           // This changes the colour of button backgrounds (Top)
            ButBGBot = Color.FromArgb(205, 226, 150);                       // This changes the colour of button backgrounds (Bot)
            ProgressBarColorTop = Color.FromArgb(102, 139, 26);             // These three variables change the colours of progress bars
            ProgressBarColorBot = Color.FromArgb(124, 164, 40);
            ProgressBarOutlineColor = Color.FromArgb(150, 174, 112);
            BannerColor1 = Color.FromArgb(0x40, 0x57, 0x04);                // These two variables change the colours of banners such as "planner" umder configuration
            BannerColor2 = Color.FromArgb(0x94, 0xC1, 0x1F);
            ColorNotEnabled = Color.FromArgb(150, 43, 58, 3);               // This changes the background color of buttons when not enabled
            ColorMouseOver = Color.FromArgb(73, 43, 58, 3);                 // This changes the background color of buttons when the mouse is hovering over a button
            ColorMouseDown = Color.FromArgb(73, 43, 58, 3);                 // This changes the background color of buttons when the mouse is clicked down on a button
            CurrentPPMBackground = Color.Green;                             // This changes the background colour of the current PPM setting in the flight modes tab
            ZedGraphChartFill = Color.FromArgb(0x1F, 0x1F, 0x20);           // These three variables change the fill colours of Zed Graphs
            ZedGraphPaneFill = Color.FromArgb(0x37, 0x37, 0x38);
            ZedGraphLegendFill = Color.FromArgb(0x85, 0x84, 0x83);
            RTBForeColor = Color.WhiteSmoke;                                // This changes the colour of text in rich text boxes
            BSVButtonAreaBGColor = Color.Black;                             // This changes the colour of a backstageview button area
            UnselectedTextColour = Color.WhiteSmoke;                        // This changes the colour of unselected text in a BSV button
            HorizontalPBValueColor = Color.FromArgb(148, 193, 31);          // This changes the colour of the horizontal progressbar

            if (MainV2.instance != null && MainV2.instance.FlightPlanner != null)
            {
                BSE.Windows.Forms.Panel actionPanel = MainV2.instance.FlightPlanner.panelAction;
                BSE.Windows.Forms.Panel waypointsPanel = MainV2.instance.FlightPlanner.panelWaypoints;

                actionPanel.CustomColors.BorderColor = Color.Black;       //these statements control the colours of the actions panel in the flight planner window
                actionPanel.CustomColors.CaptionGradientBegin = ButBG;
                actionPanel.CustomColors.CaptionGradientEnd = ButBGBot;
                actionPanel.CustomColors.CaptionText = ButtonTextColor;
                actionPanel.CustomColors.CollapsedCaptionText = ButtonTextColor;

                waypointsPanel.CustomColors.BorderColor = Color.Black;    //these statements control the colours of the Waypoints panel in the flight planner window
                waypointsPanel.CustomColors.CaptionGradientBegin = ButBG;
                waypointsPanel.CustomColors.CaptionGradientEnd = ButBGBot;
                waypointsPanel.CustomColors.CaptionText = ButtonTextColor;
                waypointsPanel.CustomColors.CollapsedCaptionText = ButtonTextColor;
            }

            if (MainV2.instance != null)
            {
                MainV2.instance.switchicons(new MainV2.burntkermitmenuicons());
            }

            MainV2.TerminalTheming = true;
            Settings.Instance["terminaltheming"] = true.ToString();
        }

        private static void SetHighContrastColours()
        {

            BGColor = Color.FromArgb(0xEE, 0xEE, 0xEE);                     // This changes the colour of the main menu background
            ControlBGColor = Color.FromArgb(0xE2, 0xE2, 0xE2);              // This changes the colour of the sub menu backgrounds
            TextColor = Color.Black;                                        // This changes the colour of text
            BGColorTextBox = ControlBGColor;                                // This changes the colour of the background of textboxes
            ButtonTextColor = Color.Black;                                  // This changes the colour of button text
            ButBG = Color.FromArgb(0xFF, 0xFF, 0x99);                       // This changes the colour of button backgrounds (Top)
            ButBGBot = Color.FromArgb(0xCC, 0xCC, 0x66);                    // This changes the colour of button backgrounds (Bot)
            ProgressBarColorTop = Color.FromArgb(227, 227, 227);            // These three variables change the colours of progress bars
            ProgressBarColorBot = Color.FromArgb(227, 227, 227);
            ProgressBarOutlineColor = Color.FromArgb(150, 174, 112);
            BannerColor1 = Color.FromArgb(0x40, 0x57, 0x04);                // These two variables change the colours of banners such as "planner" umder configuration
            BannerColor2 = Color.FromArgb(0x94, 0xC1, 0x1F);
            ColorNotEnabled = Color.FromArgb(150, 43, 58, 3);               // This changes the background color of buttons when not enabled
            ColorMouseOver = Color.FromArgb(73, 43, 58, 3);                 // This changes the background color of buttons when the mouse is hovering over a button
            ColorMouseDown = Color.FromArgb(73, 43, 58, 3);                 // This changes the background color of buttons when the mouse is clicked down on a button
            CurrentPPMBackground = Color.Green;                             // This changes the background colour of the current PPM setting in the flight modes tab
            ZedGraphChartFill = ControlBGColor;                             // These three variables change the fill colours of Zed Graphs
            ZedGraphPaneFill = BGColor;
            ZedGraphLegendFill = ControlBGColor;
            RTBForeColor = TextColor;                                       // This changes the colour of text in rich text boxes
            BSVButtonAreaBGColor = Color.White;                             // This changes the colour of a backstageview button area
            UnselectedTextColour = Color.Gray;                              // This changes the colour of unselected text in a BSV button
            HorizontalPBValueColor = Color.FromArgb(148, 193, 31);          // This changes the colour of the horizontal progressbar

            if (MainV2.instance != null && MainV2.instance.FlightPlanner != null)
            {
                BSE.Windows.Forms.Panel actionPanel = MainV2.instance.FlightPlanner.panelAction;
                BSE.Windows.Forms.Panel waypointsPanel = MainV2.instance.FlightPlanner.panelWaypoints;

                actionPanel.CustomColors.BorderColor = Color.Black;       //these statements control the colours of the actions panel in the flight planner window
                actionPanel.CustomColors.CaptionGradientBegin = ButBG;
                actionPanel.CustomColors.CaptionGradientEnd = ButBGBot;
                actionPanel.CustomColors.CaptionText = ButtonTextColor;
                actionPanel.CustomColors.CollapsedCaptionText = ButtonTextColor;

                waypointsPanel.CustomColors.BorderColor = Color.Black;    //these statements control the colours of the Waypoints panel in the flight planner window
                waypointsPanel.CustomColors.CaptionGradientBegin = ButBG;
                waypointsPanel.CustomColors.CaptionGradientEnd = ButBGBot;
                waypointsPanel.CustomColors.CaptionText = ButtonTextColor;
                waypointsPanel.CustomColors.CollapsedCaptionText = ButtonTextColor;
            }

            if (MainV2.instance != null)
            {
                MainV2.instance.switchicons(new MainV2.highcontrastmenuicons());
            }

            MainV2.TerminalTheming = true;
            Settings.Instance["terminaltheming"] = true.ToString();
        }

        private static void ApplyTheme(Control temp, int level)
        {
            if (level == 0)
            {
                temp.BackColor = BGColor;
                temp.ForeColor = TextColor;
            }

            foreach (Control ctl in temp.Controls)
            {
                if (ctl.GetType() == typeof(Label))
                {
                    if (!(ctl.Tag is string && (string)ctl.Tag == "custom"))
                    {
                        ctl.ForeColor = TextColor;
                    }
                }
                else if (ctl.GetType() == typeof(TreeView))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                    TreeView txtr = (TreeView)ctl;
                    txtr.LineColor = TextColor;
                }
                else if (ctl.GetType() == typeof(Panel))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof(GroupBox))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof(MyLabel))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof(Button))
                {
                    ctl.ForeColor = Color.Black;
                    ctl.BackColor = ButBG;
                }
                else if (ctl.GetType() == typeof(MyButton))
                {
                    Controls.MyButton but = (MyButton)ctl;
                    but.BGGradTop = ButBG;
                    but.BGGradBot = ButBGBot;
                    but.TextColor = ButtonTextColor;
                    but.Outline = ButBorder;
                    but.ColorMouseDown = ColorMouseDown;        //sets the colour of buttons for different situations
                    but.ColorMouseOver = ColorMouseOver;
                    but.ColorNotEnabled = ColorNotEnabled;
                }
                else if (ctl.GetType() == typeof(TextBox))
                {
                    ctl.BackColor = BGColorTextBox;             //sets the BG colour of text boxes to specified colour
                    ctl.ForeColor = TextColor;
                    TextBox txt = (TextBox)ctl;
                    txt.BorderStyle = BorderStyle.None;
                }
                else if (ctl.GetType() == typeof(DomainUpDown))
                {
                    ctl.BackColor = ControlBGColor;
                    ctl.ForeColor = TextColor;
                    DomainUpDown txt = (DomainUpDown)ctl;
                    txt.BorderStyle = BorderStyle.None;
                }
                else if (ctl.GetType() == typeof(GroupBox) || ctl.GetType() == typeof(UserControl) ||
                         ctl.GetType() == typeof(DataTreeListView))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof(ZedGraph.ZedGraphControl))
                {
                    var zg1 = (ZedGraph.ZedGraphControl)ctl;
                    zg1.GraphPane.Chart.Fill = new ZedGraph.Fill(ZedGraphChartFill);
                    zg1.GraphPane.Fill = new ZedGraph.Fill(ZedGraphPaneFill);

                    foreach (ZedGraph.LineItem li in zg1.GraphPane.CurveList)
                        li.Line.Width = 2;

                    zg1.GraphPane.Title.FontSpec.FontColor = TextColor;

                    zg1.GraphPane.XAxis.MajorTic.Color = TextColor;
                    zg1.GraphPane.XAxis.MinorTic.Color = TextColor;
                    zg1.GraphPane.YAxis.MajorTic.Color = TextColor;
                    zg1.GraphPane.YAxis.MinorTic.Color = TextColor;
                    zg1.GraphPane.Y2Axis.MajorTic.Color = TextColor;
                    zg1.GraphPane.Y2Axis.MinorTic.Color = TextColor;

                    zg1.GraphPane.XAxis.MajorGrid.Color = TextColor;
                    zg1.GraphPane.YAxis.MajorGrid.Color = TextColor;
                    zg1.GraphPane.Y2Axis.MajorGrid.Color = TextColor;

                    zg1.GraphPane.YAxis.Scale.FontSpec.FontColor = TextColor;
                    zg1.GraphPane.YAxis.Title.FontSpec.FontColor = TextColor;
                    zg1.GraphPane.Y2Axis.Title.FontSpec.FontColor = TextColor;
                    zg1.GraphPane.Y2Axis.Scale.FontSpec.FontColor = TextColor;

                    zg1.GraphPane.XAxis.Scale.FontSpec.FontColor = TextColor;
                    zg1.GraphPane.XAxis.Title.FontSpec.FontColor = TextColor;

                    zg1.GraphPane.Legend.Fill = new ZedGraph.Fill(ZedGraphLegendFill);
                    zg1.GraphPane.Legend.FontSpec.FontColor = TextColor;
                }
                else if (ctl.GetType() == typeof(BSE.Windows.Forms.Panel) || ctl.GetType() == typeof(SplitterPanel))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof(RadialGradientBG)) //not included in original burntkermit theme
                {
                    //    var rbg = ctl as RadialGradientBG;
                    //    rbg.CenterColor = ControlBGColor;
                    //    rbg.OutsideColor = ButBG;
                }
                else if (ctl.GetType() == typeof(GradientBG))
                {
                    //    var rbg = ctl as GradientBG;
                    //    rbg.CenterColor = ControlBGColor;
                    //    rbg.OutsideColor = ButBG;
                }
                else if (ctl.GetType() == typeof(Form))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                    if (Program.IconFile != null)
                        ((Form)ctl).Icon = Icon.FromHandle(((Bitmap)Program.IconFile).GetHicon());
                }
                else if (ctl.GetType() == typeof(RichTextBox))
                {
                    if ((ctl.Name == "TXT_terminal") && !MainV2.TerminalTheming)
                    {
                        RichTextBox txtr = (RichTextBox)ctl;
                        txtr.BorderStyle = BorderStyle.None;
                        txtr.ForeColor = Color.White;
                        txtr.BackColor = Color.Black;
                    }
                    else
                    {
                        RichTextBox txtr = (RichTextBox)ctl;
                        txtr.BorderStyle = BorderStyle.None;
                        txtr.ForeColor = RTBForeColor;
                        txtr.BackColor = ControlBGColor;
                    }
                }
                else if (ctl.GetType() == typeof(CheckedListBox))
                {
                    ctl.BackColor = ControlBGColor;
                    ctl.ForeColor = TextColor;
                    CheckedListBox txtr = (CheckedListBox)ctl;
                    txtr.BorderStyle = BorderStyle.None;
                }
                else if (ctl.GetType() == typeof(TabPage))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                    TabPage txtr = (TabPage)ctl;
                    txtr.BorderStyle = BorderStyle.None;
                }
                else if (ctl.GetType() == typeof(TabControl))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                    TabControl txtr = (TabControl)ctl;
                }
                else if (ctl.GetType() == typeof(DataGridView) || ctl.GetType() == typeof(MyDataGridView))
                {
                    ctl.ForeColor = TextColor;
                    DataGridView dgv = (DataGridView)ctl;
                    dgv.EnableHeadersVisualStyles = false;
                    dgv.BorderStyle = BorderStyle.None;
                    dgv.BackgroundColor = BGColor;
                    DataGridViewCellStyle rs = new DataGridViewCellStyle();
                    rs.BackColor = ControlBGColor;
                    rs.ForeColor = TextColor;
                    dgv.RowsDefaultCellStyle = rs;

                    dgv.AlternatingRowsDefaultCellStyle.BackColor = BGColor;

                    DataGridViewCellStyle hs = new DataGridViewCellStyle(dgv.ColumnHeadersDefaultCellStyle);
                    hs.BackColor = BGColor;
                    hs.ForeColor = TextColor;

                    dgv.ColumnHeadersDefaultCellStyle = hs;
                    dgv.RowHeadersDefaultCellStyle = hs;
                }
                else if (ctl.GetType() == typeof(CheckBox) || ctl.GetType() == typeof(MavlinkCheckBox))
                {
                    if (!(ctl.Tag is string && (string)ctl.Tag == "custom"))
                    {
                        ctl.BackColor = BGColor;
                    }
                    ctl.ForeColor = TextColor;
                    CheckBox CHK = (CheckBox) ctl;
                    // CHK.FlatStyle = FlatStyle.Flat;
                }
                else if (ctl.GetType() == typeof (ComboBox) || ctl.GetType() == typeof (MavlinkComboBox))
                {
                    ctl.BackColor = ControlBGColor;
                    ctl.ForeColor = TextColor;
                    ComboBox CMB = (ComboBox) ctl;
                    CMB.FlatStyle = FlatStyle.Flat;
                }
                else if (ctl.GetType() == typeof (NumericUpDown) || ctl.GetType() == typeof (MavlinkNumericUpDown))
                {
                    ctl.BackColor = BGColorTextBox;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof (TrackBar))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof (LinkLabel))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                    LinkLabel LNK = (LinkLabel) ctl;
                    LNK.ActiveLinkColor = TextColor;
                    LNK.LinkColor = TextColor;
                    LNK.VisitedLinkColor = TextColor;
                }
                else if (ctl.GetType() == typeof (BackstageView))
                {
                    var bsv = ctl as BackstageView;

                    bsv.BackColor = BGColor;
                    bsv.ButtonsAreaBgColor = BSVButtonAreaBGColor;
                    bsv.HighlightColor2 = BannerColor2;
                    bsv.HighlightColor1 = BannerColor1;
                    bsv.SelectedTextColor = TextColor;
                    bsv.UnSelectedTextColor = UnselectedTextColour;
                    bsv.ButtonsAreaPencilColor = Color.DarkGray;
                }
                else if (ctl.GetType() == typeof (HorizontalProgressBar2) ||
                         ctl.GetType() == typeof (VerticalProgressBar2))
                {
                    ((HorizontalProgressBar2) ctl).BackgroundColor = ControlBGColor;
                    ((HorizontalProgressBar2) ctl).ValueColor = HorizontalPBValueColor;
                }
                else if (ctl.GetType() == typeof(MyProgressBar))
                {
                    ((MyProgressBar)ctl).BGGradTop = ProgressBarColorTop;
                    ((MyProgressBar)ctl).BGGradBot = ProgressBarColorBot;
                    ((MyProgressBar)ctl).Outline = ProgressBarOutlineColor;        //sets the colour of the progress bar box
                } else if (ctl.GetType() == typeof (QuickView))
                {
                    //set default QuickView item color to mix with background
                    Color mix = Color.FromArgb(ThemeManager.BGColor.ToArgb() ^ 0xffffff);

                    Controls.QuickView but = (QuickView) ctl;
                    if (but.Name == "quickView6")
                    {
                        but.numberColor = Color.FromArgb((0 + mix.R) / 2, (255 + mix.G) / 2, (252 + mix.B) / 2);
                    }
                    else if (but.Name == "quickView5")
                    {
                        but.numberColor = Color.FromArgb((254 + mix.R) / 2, (254 + mix.G) / 2, (86 + mix.B) / 2);
                    }
                    else if (but.Name == "quickView4")
                    {
                        but.numberColor = Color.FromArgb((0 + mix.R) / 2, (255 + mix.G) / 2, (83 + mix.B) / 2);
                    }
                    else if (but.Name == "quickView3")
                    {
                        but.numberColor = Color.FromArgb((255 + mix.R) / 2, (96 + mix.G) / 2, (91 + mix.B) / 2);
                    }
                    else if (but.Name == "quickView2")
                    {
                        but.numberColor = Color.FromArgb((254 + mix.R) / 2, (132 + mix.G) / 2, (46 + mix.B) / 2);
                    }
                    else if (but.Name == "quickView1")
                    {
                        but.numberColor = Color.FromArgb((209 + mix.R) / 2, (151 + mix.G) / 2, (248 + mix.B) / 2);
                    }
                    //return;  //return removed to process all quickView controls
                }
                if ( (ctl.Controls.Count > 0) && (ctl.GetType() != typeof(QuickView)))      //Do not iterate into quickView type leave labels as they are
                    ApplyTheme(ctl, 1);

            }
        }
    }
}