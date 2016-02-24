using System;
using System.Drawing;
using System.Windows.Forms;
using MissionPlanner.Controls.BackstageView;
using log4net;
using MissionPlanner.Controls;
using System.IO;
using System.Collections.Generic;
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
        public static Color ButBG;
        public static Color ButBorder;

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

        /// <summary>
        /// Will recursively apply the current theme to 'control'
        /// </summary>
        /// <param name="control"></param>
        public static void ApplyThemeTo(Control control)
        {
            switch (_currentTheme)
            {
                case Themes.BurntKermit:
                    ApplyBurntKermitTheme(control, 0);
                    break;

                case Themes.HighContrast:
                    ApplyHighContrast(control, 0);
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
            List<Control> temp = new List<Control>();

            temp.Add(new GCSViews.FlightData());
            temp.Add(new GCSViews.FlightPlanner());
            temp.Add(new GCSViews.Help());
            temp.Add(new GCSViews.InitialSetup());
            temp.Add(new GCSViews.Simulation());
            temp.Add(new GCSViews.SoftwareConfig());
            temp.Add(new GCSViews.Terminal());

            temp.Add(new ConnectionControl());
            temp.Add(new ConnectionStats());
            temp.Add(new MavlinkCheckBox());
            temp.Add(new MavlinkComboBox());
            //temp.Add(new MavlinkNumericUpDown());
            temp.Add(new ModifyandSet());
            temp.Add(new ServoOptions());
            temp.Add(new ThemeColors());

            temp.Add(new MissionPlanner.Controls.BackstageView.BackstageView());
            temp.Add(new MissionPlanner.Controls.BackstageView.BackstageViewButton());
            temp.Add(new MissionPlanner.Controls.BackstageView.BackStageViewMenuPanel());
            temp.Add(new MissionPlanner.Controls.ConnectionControl());
            temp.Add(new MissionPlanner.Controls.ConnectionStats());
            temp.Add(new MissionPlanner.Controls.Coords());
            temp.Add(new MissionPlanner.Controls.FileBrowse());
            temp.Add(new MissionPlanner.Controls.FlashMessage());
            temp.Add(new MissionPlanner.Controls.GradientBG());
            temp.Add(new MissionPlanner.Controls.HorizontalProgressBar());
            temp.Add(new MissionPlanner.Controls.HorizontalProgressBar2());
            temp.Add(new MissionPlanner.Controls.HSI());
            temp.Add(new MissionPlanner.Controls.HUD());
            temp.Add(new MissionPlanner.Controls.ImageLabel());
            temp.Add(new MissionPlanner.Controls.LabelWithPseudoOpacity());
            temp.Add(new MissionPlanner.Controls.LineSeparator());

            temp.Add(new MissionPlanner.Controls.MyButton());
            temp.Add(new MissionPlanner.Controls.myGMAP());
            temp.Add(new MissionPlanner.Controls.MyLabel());
            temp.Add(new MissionPlanner.Controls.MyProgressBar());
            temp.Add(new MissionPlanner.Controls.MyTrackBar());
            temp.Add(new MissionPlanner.Controls.OpenGLtest());
            temp.Add(new MissionPlanner.Controls.OptionForm());
            temp.Add(new MissionPlanner.Controls.PictureBoxMouseOver());
            temp.Add(new MissionPlanner.Controls.PictureBoxWithPseudoOpacity());
            temp.Add(new MissionPlanner.Controls.ProgressReporterDialogue());
            temp.Add(new MissionPlanner.Controls.ProgressStep());
            temp.Add(new MissionPlanner.Controls.QuickView());
            temp.Add(new MissionPlanner.Controls.RadialGradientBG());
            temp.Add(new MissionPlanner.Controls.RangeControl());
            temp.Add(new MissionPlanner.Controls.ServoOptions());
            temp.Add(new MissionPlanner.Controls.ValuesControl());
            temp.Add(new MissionPlanner.Controls.VerticalProgressBar());
            temp.Add(new MissionPlanner.Controls.VerticalProgressBar2());


            temp.Add(new Wizard._1Intro());
            temp.Add(new Wizard._2FrameFW());
            temp.Add(new Wizard._3ConnectAP());
            temp.Add(new Wizard._4FrameType());
            temp.Add(new Wizard._5AccelCalib());
            temp.Add(new Wizard._6CompassCalib());
            temp.Add(new Wizard._7BatteryMonitor());
            temp.Add(new Wizard._8OptionalItemsAC());
            temp.Add(new Wizard._9RadioCalibration());
            temp.Add(new Wizard._10FlightModes());
            temp.Add(new Wizard._11Verify());
            temp.Add(new Wizard._12FailSafe());
            temp.Add(new Wizard._13GeoFence());
            temp.Add(new Wizard._98DontForget());

            temp.Add(new GCSViews.ConfigurationView.ConfigAC_Fence());
            temp.Add(new GCSViews.ConfigurationView.ConfigAP_Limits());
            temp.Add(new GCSViews.ConfigurationView.ConfigArducopter());
            temp.Add(new GCSViews.ConfigurationView.ConfigArduplane());
            temp.Add(new GCSViews.ConfigurationView.ConfigArdurover());
            temp.Add(new GCSViews.ConfigurationView.ConfigAteryx());
            temp.Add(new GCSViews.ConfigurationView.ConfigAteryxSensors());
            temp.Add(new GCSViews.ConfigurationView.ConfigBatteryMonitoring());
            temp.Add(new GCSViews.ConfigurationView.ConfigCameraStab());
            temp.Add(new GCSViews.ConfigurationView.ConfigFailSafe());
            temp.Add(new GCSViews.ConfigurationView.ConfigFirmwareDisabled());
            temp.Add(new GCSViews.ConfigurationView.ConfigFlightModes());
            temp.Add(new GCSViews.ConfigurationView.ConfigFrameType());
            temp.Add(new GCSViews.ConfigurationView.ConfigFriendlyParams());
            temp.Add(new GCSViews.ConfigurationView.ConfigHWAirspeed());
            temp.Add(new GCSViews.ConfigurationView.ConfigHWCompass());
            temp.Add(new GCSViews.ConfigurationView.ConfigHWOptFlow());
            temp.Add(new GCSViews.ConfigurationView.ConfigHWOSD());
            temp.Add(new GCSViews.ConfigurationView.ConfigHWRangeFinder());
            temp.Add(new GCSViews.ConfigurationView.ConfigMandatory());
            temp.Add(new GCSViews.ConfigurationView.ConfigMount());
            temp.Add(new GCSViews.ConfigurationView.ConfigOptional());
            temp.Add(new GCSViews.ConfigurationView.ConfigPlanner());
            temp.Add(new GCSViews.ConfigurationView.ConfigPlannerAdv());
            temp.Add(new GCSViews.ConfigurationView.ConfigRadioInput());
            temp.Add(new GCSViews.ConfigurationView.ConfigRawParams());
            temp.Add(new GCSViews.ConfigurationView.ConfigSimplePids());
            temp.Add(new GCSViews.ConfigurationView.ConfigTradHeli());

            foreach (var ctl in temp)
            {
                xaml(ctl);
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
                else if (ctl.GetType() == typeof (Form))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor; // Color.FromArgb(0xe6, 0xe8, 0xea);
                }
                else if (ctl.GetType() == typeof (RichTextBox))
                {
                    ctl.BackColor = ControlBGColor;
                    ctl.ForeColor = TextColor;
                    RichTextBox txtr = (RichTextBox) ctl;
                    txtr.BorderStyle = BorderStyle.None;
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
                else if (ctl.GetType() == typeof (DataGridView))
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

        private static void ApplyHighContrast(Control temp, int level)
        {
            unchecked
            {
                BGColor = Color.FromArgb((int) 0xffeeeeee); // background
                ControlBGColor = Color.FromArgb((int) 0xffe2e2e2); // editable bg color
                TextColor = Color.Black;
                ButBG = Color.FromArgb((int) 0xffffff99);
            }

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
                    but.BGGradBot = Color.FromArgb(ButBG.ToArgb() - 0x333333);
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
                else if (ctl.GetType() == typeof (GroupBox) || ctl.GetType() == typeof (UserControl) || ctl.GetType() == typeof(DataTreeListView))
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
                else if (ctl.GetType() == typeof (Form))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor; // Color.FromArgb(0xe6, 0xe8, 0xea);
                }
                else if (ctl.GetType() == typeof (RichTextBox))
                {
                    ctl.BackColor = TextColor;
                    ctl.ForeColor = TextColor;
                    RichTextBox txtr = (RichTextBox) ctl;
                    txtr.BorderStyle = BorderStyle.None;

                    txtr.SelectAll();
                    txtr.SelectionColor = TextColor;
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
                else if (ctl.GetType() == typeof (DataGridView))
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

                    dgv.AlternatingRowsDefaultCellStyle = rs;

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

                if (ctl.Controls.Count > 0)
                    ApplyHighContrast(ctl, 1);
            }
        }

        private static void ApplyBurntKermitTheme(Control temp, int level)
        {
            BGColor = Color.FromArgb(0x26, 0x27, 0x28); // background
            ControlBGColor = Color.FromArgb(0x43, 0x44, 0x45); // editable bg color
            TextColor = Color.White;

            if (level == 0)
            {
                temp.BackColor = BGColor;
                temp.ForeColor = TextColor;
            }

            foreach (Control ctl in temp.Controls)
            {
                if (ctl.GetType() == typeof (TreeView))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                    TreeView txtr = (TreeView) ctl;
                    txtr.LineColor = TextColor;
                }
                else if (ctl.GetType() == typeof (Panel))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof (GroupBox))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof (MyLabel))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof (Button))
                {
                    ctl.ForeColor = Color.Black;
                }
                else if (ctl.GetType() == typeof (MyButton))
                {
                    Controls.MyButton but = (MyButton) ctl;
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
                    zg1.GraphPane.Chart.Fill = new ZedGraph.Fill(Color.FromArgb(0x1f, 0x1f, 0x20));
                    zg1.GraphPane.Fill = new ZedGraph.Fill(Color.FromArgb(0x37, 0x37, 0x38));

                    try
                    {
                        foreach (ZedGraph.LineItem li in zg1.GraphPane.CurveList)
                            li.Line.Width = 2;
                    }
                    catch
                    {
                    }

                    zg1.GraphPane.Title.FontSpec.FontColor = TextColor;

                    zg1.GraphPane.XAxis.MajorTic.Color = Color.White;
                    zg1.GraphPane.XAxis.MinorTic.Color = Color.White;
                    zg1.GraphPane.YAxis.MajorTic.Color = Color.White;
                    zg1.GraphPane.YAxis.MinorTic.Color = Color.White;
                    zg1.GraphPane.Y2Axis.MajorTic.Color = Color.White;
                    zg1.GraphPane.Y2Axis.MinorTic.Color = Color.White;

                    zg1.GraphPane.XAxis.MajorGrid.Color = Color.White;
                    zg1.GraphPane.YAxis.MajorGrid.Color = Color.White;
                    zg1.GraphPane.Y2Axis.MajorGrid.Color = Color.White;

                    zg1.GraphPane.YAxis.Scale.FontSpec.FontColor = Color.White;
                    zg1.GraphPane.YAxis.Title.FontSpec.FontColor = Color.White;
                    zg1.GraphPane.Y2Axis.Title.FontSpec.FontColor = Color.White;
                    zg1.GraphPane.Y2Axis.Scale.FontSpec.FontColor = Color.White;

                    zg1.GraphPane.XAxis.Scale.FontSpec.FontColor = Color.White;
                    zg1.GraphPane.XAxis.Title.FontSpec.FontColor = Color.White;

                    zg1.GraphPane.Legend.Fill = new ZedGraph.Fill(Color.FromArgb(0x85, 0x84, 0x83));
                    zg1.GraphPane.Legend.FontSpec.FontColor = TextColor;
                }
                else if (ctl.GetType() == typeof (BSE.Windows.Forms.Panel) || ctl.GetType() == typeof (SplitterPanel))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor; // Color.FromArgb(0xe6, 0xe8, 0xea);
                }
                else if (ctl.GetType() == typeof (Form))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor; // Color.FromArgb(0xe6, 0xe8, 0xea);
                }
                else if (ctl.GetType() == typeof (RichTextBox))
                {
                    ctl.BackColor = ControlBGColor;
                    ctl.ForeColor = TextColor;
                    RichTextBox txtr = (RichTextBox) ctl;
                    txtr.BorderStyle = BorderStyle.None;
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
                else if (ctl.GetType() == typeof (DataGridView))
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

                    dgv.AlternatingRowsDefaultCellStyle.BackColor = BGColor;

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
                    bsv.ButtonsAreaBgColor = Color.Black; // ControlBGColor;
                    bsv.HighlightColor2 = Color.FromArgb(0x94, 0xc1, 0x1f);
                    bsv.HighlightColor1 = Color.FromArgb(0x40, 0x57, 0x04);
                    bsv.SelectedTextColor = Color.White;
                    bsv.UnSelectedTextColor = Color.WhiteSmoke;
                    bsv.ButtonsAreaPencilColor = Color.DarkGray;
                }
                else if (ctl.GetType() == typeof (HorizontalProgressBar2) ||
                         ctl.GetType() == typeof (VerticalProgressBar2))
                {
                    ((HorizontalProgressBar2) ctl).BackgroundColor = ControlBGColor;
                    ((HorizontalProgressBar2) ctl).ValueColor = Color.FromArgb(148, 193, 31);
                }

                if (ctl.Controls.Count > 0)
                    ApplyBurntKermitTheme(ctl, 1);
            }
        }
    }
}