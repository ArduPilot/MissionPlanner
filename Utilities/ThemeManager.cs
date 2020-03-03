using BrightIdeasSoftware;
using log4net;
using MissionPlanner.Controls;
using MissionPlanner.Controls.BackstageView;
using MissionPlanner.Controls.PreFlight;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;

namespace MissionPlanner.Utilities
{

    //ThemeColor class is describe an item in a theme. 
    // strColorItemName is the variable name of the 
    public class ThemeColor
    {
        public String strColorItemName { get; set; }
        
        [XmlElement(Type = typeof(XmlColor))]
        public Color clrColor { get; set; }
        public String strVariableName { get; set; }
    }

    public class ThemeColorList : List<ThemeColor>
    {
        public void Add(String _strColor, Color _clrColor, String _strVariable)
        {
            var data = new ThemeColor
            {
                strColorItemName = _strColor,
                clrColor = _clrColor,
                strVariableName = _strVariable
            };
            this.Add(data);
        }
    }

    public class ThemeColorTable
    {

        public enum IconSet
        {
            BurnKermitIconSet,
            HighContrastIconSet,
        }

        public String strThemeName { get; set; }
        public ThemeColorList colors { get; set; }
        public IconSet iconSet { get; set; }
        public bool terminalTheming { get; set; }

        public ThemeColorTable()
        {
            colors = new ThemeColorList();
        }
        public void InitColors()
        {
            iconSet = IconSet.BurnKermitIconSet;
            terminalTheming = true;
            strThemeName = "BurntKermit.mpsystheme";

            colors.Add("Background", Color.FromArgb(0x26, 0x27, 0x28), "BGColor");						// This changes the colour of the main menu background
            colors.Add("Control Background", Color.FromArgb(0x43, 0x44, 0x45), "ControlBGColor");		// This changes the colour of the sub menu backgrounds
            colors.Add("Text", Color.White, "TextColor");										// This changes the colour of text
            colors.Add("TextBox Background", Color.FromArgb(0x43, 0x44, 0x45), "BGColorTextBox");		// This changes the colour of the background of textboxes
            colors.Add("Button Text", Color.FromArgb(64, 87, 4), "ButtonTextColor");				// This changes the colour of button text
            colors.Add("Button Background top", Color.FromArgb(148, 193, 31), "ButBG");								// This changes the colour of button backgrounds (Top)
            colors.Add("Button Background bottom", Color.FromArgb(205, 226, 150), "ButBGBot");						// This changes the colour of button backgrounds (Bot)
            colors.Add("ProgressBar Top", Color.FromArgb(102, 139, 26), "ProgressBarColorTop");	// These three variables change the colours of progress bars
            colors.Add("ProgressBar Bottom", Color.FromArgb(124, 164, 40), "ProgressBarColorBot");
            colors.Add("ProgressBar Outline", Color.FromArgb(150, 174, 112), "ProgressBarOutlineColor");
            colors.Add("BannerColor1", Color.FromArgb(0x40, 0x57, 0x04), "BannerColor1");			// These two variables change the colours of banners such as "planner" umder configuration
            colors.Add("BannerColor2", Color.FromArgb(0x94, 0xC1, 0x1F), "BannerColor2");
            colors.Add("Disabled Button", Color.FromArgb(150, 43, 58, 3), "ColorNotEnabled");		// This changes the background color of buttons when not enabled
            colors.Add("Button Mouseover", Color.FromArgb(73, 43, 58, 3), "ColorMouseOver");			// This changes the background color of buttons when the mouse is hovering over a button
            colors.Add("Button Mousedown", Color.FromArgb(73, 43, 58, 3), "ColorMouseDown");			// This changes the background color of buttons when the mouse is clicked down on a button
            colors.Add("CurrentPPM Background", Color.Green, "CurrentPPMBackground");					// This changes the background colour of the current PPM setting in the flight modes tab
            colors.Add("Graph Chart Fill", Color.FromArgb(0x1F, 0x1F, 0x20), "ZedGraphChartFill"); 	// These three variables change the fill colours of Zed Graphs
            colors.Add("Graph Pane Fill", Color.FromArgb(0x37, 0x37, 0x38), "ZedGraphPaneFill");
            colors.Add("Graph Legend Fill", Color.FromArgb(0x85, 0x84, 0x83), "ZedGraphLegendFill");
            colors.Add("Rich Text Box text", Color.WhiteSmoke, "RTBForeColor");							// This changes the colour of text in rich text boxes
            colors.Add("BackStageView Button Area", Color.Black, "BSVButtonAreaBGColor");					// This changes the colour of a backstageview button area
            colors.Add("BSV Unselected Text", Color.WhiteSmoke, "UnselectedTextColour");			// This changes the colour of unselected text in a BSV button
            colors.Add("Horizontal ProgressBar", Color.FromArgb(148, 193, 31), "HorizontalPBValueColor"); // This changes the colour of the horizontal progressbar
            colors.Add("HUD text and drawings", Color.LightGray, "HudText");                       
            colors.Add("HUD Ground top", Color.FromArgb(0x9b, 0xb8, 0x24), "HudGroundTop");
            colors.Add("HUD Ground bottom", Color.FromArgb(0x41, 0x4f, 0x07), "HudGroundBot");
            colors.Add("HUD Sky top", Color.Blue, "HudSkyTop");
            colors.Add("HUD Sky bottom", Color.LightBlue, "HudSkyBot");

        }

        public void SetTheme()
        {

            foreach (ThemeColor _color in colors)
            {
                Type objType = typeof(ThemeManager);
                FieldInfo info = objType.GetField(_color.strVariableName);

                if (info != null && info.FieldType.Name.Equals("Color") )
                {
                    info.SetValue(objType, _color.clrColor);
                    Console.WriteLine(_color.strColorItemName + " to " + _color.clrColor);
                }
                else
                {
                    Console.WriteLine("No such field as :" + _color.strVariableName);
                }
            }

            if (MainV2.instance != null)
            {
                switch (iconSet)
                {
                    case IconSet.BurnKermitIconSet:
                        MainV2.instance.switchicons(new MainV2.burntkermitmenuicons());
                        break;
                    case IconSet.HighContrastIconSet:
                        MainV2.instance.switchicons(new MainV2.highcontrastmenuicons());
                        break;
                    default:                                                            
                        MainV2.instance.switchicons(new MainV2.burntkermitmenuicons());     //Fall back to BurntKermit
                        break;
                }
            }

            MainV2.TerminalTheming = terminalTheming;
            Settings.Instance["terminaltheming"] = terminalTheming.ToString();
            //HUD Color setting
            if (GCSViews.FlightData.myhud != null)
            {
                GCSViews.FlightData.myhud.groundColor1 = ThemeManager.HudGroundTop;
                GCSViews.FlightData.myhud.groundColor2 = ThemeManager.HudGroundBot;
                GCSViews.FlightData.myhud.skyColor1 = ThemeManager.HudSkyTop;
                GCSViews.FlightData.myhud.skyColor2 = ThemeManager.HudSkyBot;
                GCSViews.FlightData.myhud.hudcolor = ThemeManager.HudText;
            }
        }
    }

    /// <summary>
    /// Helper class for the stylng 'theming' of forms and controls, and provides MessageBox
    /// replacements which are also styled
    /// </summary>
    public class ThemeManager
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
        public static Color HudText;
        public static Color HudGroundTop;
        public static Color HudGroundBot;
        public static Color HudSkyTop;
        public static Color HudSkyBot;

        public static ThemeColorTable thmColor;

        public static List<String> ThemeNames;
        

        
        public static void GetThemesList()
        {

            String runningDir = Settings.GetRunningDirectory();
            String userDir = Settings.GetUserDataDirectory();

            if (ThemeNames == null)
            {
                ThemeNames = new List<String>();
            }
            else
            {
                ThemeNames.Clear();
            }

            //Get default themes from program directory (system themes are read only)
            var themeFiles = Directory.EnumerateFiles(runningDir, "*.mpsystheme");
            foreach (string currentFile in themeFiles)
            {
                ThemeNames.Add(Path.GetFileName(currentFile));
            }
            //Get theme files from user directory (user themes can be overwritten)
            themeFiles = Directory.EnumerateFiles(userDir, "*.mpusertheme");
            foreach (string currentFile in themeFiles)
            {
                ThemeNames.Add(Path.GetFileName(currentFile));
            }

        }


        public static void LoadTheme(string strThemeName)
        {

            string themeFileToLoad = "";

            Console.WriteLine(strThemeName + " theme is loading");

            ThemeManager.GetThemesList();
            
            //check theme extension to determine location (mpsystheme is in the program directory, mpusertheme is in the userdata directory)
            if (Path.GetExtension(strThemeName).Equals(".mpsystheme", StringComparison.OrdinalIgnoreCase))
            {
                themeFileToLoad = Settings.GetRunningDirectory() + strThemeName;
            }
            else
            {
                themeFileToLoad = Settings.GetUserDataDirectory() + strThemeName;
            }

            try
            {
                ThemeManager.thmColor = ThemeManager.ReadFromXmlFile<ThemeColorTable>(themeFileToLoad);
                ThemeManager.thmColor.strThemeName = strThemeName;

            }
            catch
            {
                ThemeManager.thmColor = new ThemeColorTable(); //Init colortable
                ThemeManager.thmColor.InitColors();
            }

            if (ThemeManager.thmColor == null)
            {
                ThemeManager.thmColor = new ThemeColorTable(); //Init colortable
                ThemeManager.thmColor.InitColors();
            }

            //Copy color values to the ThemeManager color variables
            ThemeManager.thmColor.SetTheme();
            Settings.Instance["theme"] = ThemeManager.thmColor.strThemeName;
        }


        public static void StartThemeEditor()
        {
            new ThemeEditor().ShowDialog();
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
            if (control is ContainerControl)
                ((ContainerControl)control).AutoScaleMode = AutoScaleMode.None;

            ApplyTheme(control, 0);
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
                        return (Control)Activator.CreateInstance(a);
                    }
                    catch { }
                }

                return null;
            }).ToList();

            asm = typeof(ImageLabel).Assembly;

            var temp2 = asm.GetTypes().Select(a =>
            {
                if (a.IsSubclassOf(typeof(Control)))
                {
                    try
                    {
                        return (Control)Activator.CreateInstance(a);
                    }
                    catch { }
                }

                return null;
            }).ToList();

            temp.AddRange(temp2);

            foreach (var ctl in temp)
            {
                if (ctl == null)
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

        private static void dohtmlctls(Control control, StreamWriter st, int x = 0, int y = 0)
        {
            foreach (Control ctl in control.Controls)
            {
                var font = "font-family:" + ctl.Font.FontFamily + ";";
                var fontsize = "font-size:" + ctl.Font.SizeInPoints + "pt;";
                var fontcol = "color:" + System.Drawing.ColorTranslator.ToHtml(ctl.ForeColor) + ";";
                var bgcol = "background-color:" + System.Drawing.ColorTranslator.ToHtml(ctl.BackColor) + ";";

                if (ctl.Parent != null)
                {
                    if (ctl.Parent.Font == ctl.Font)
                        font = "";
                    if (ctl.Parent.ForeColor == ctl.ForeColor)
                        font = "";
                    if (ctl.Parent.BackColor == ctl.BackColor)
                        font = "";
                }

                if (ctl.AutoSize == false)
                {
                    st.WriteLine(@"<div class='" + ctl.GetType() + " " + ctl.Name + @"' " +
                                 "style='" + font + fontsize + fontcol + bgcol +
                                 "overflow:hidden;position: absolute; top: " + (y + ctl.Location.Y) + "; left: " +
                                 (x + ctl.Location.X) + ";width:" + ctl.Width + ";height:" + ctl.Height + ";' >");
                }
                else
                {
                    st.WriteLine(@"<div class='" + ctl.GetType() + " " + ctl.Name + @"' " +
                                 "style='" + font + fontsize + fontcol + bgcol +
                                 "overflow:hidden;position: absolute; top: " + (y + ctl.Location.Y) + "; left: " +
                                 (x + ctl.Location.X) + ";' >");
                }

                if (ctl.GetType() == typeof(ComboBox) || ctl.GetType() == typeof(MavlinkComboBox))
                {
                    st.WriteLine(@"<select name='{0}'>", ctl.Name);
                    (ctl as ComboBox).Items.ForEach(a => st.WriteLine(@"<option value='{0}'>{1}</option>", a, a));
                    st.WriteLine(@"</select>");
                }
                else if (ctl.GetType() == typeof(TextBox))
                {
                    st.WriteLine(@"<input name='{0}' value='{1}'>", ctl.Name, ctl.Text);
                    st.WriteLine(@"</input>");
                }
                else if (ctl.GetType() == typeof(TrackBar))
                {
                    var tb = ctl as TrackBar;
                    st.WriteLine(@"<input name='{0}' type='range' style=' width:100%; height:100%;' value='{1}' orient='{2}'>", ctl.Name, ctl.Text, tb.Orientation == Orientation.Vertical ? "vertical" : "horizontal");
                    st.WriteLine(@"</input>");
                }
                else if (ctl.GetType() == typeof(NumericUpDown))
                {
                    st.WriteLine(@"<input name='{0}' type='number'>", ctl.Name);
                    st.Write(ctl.Text);
                    st.WriteLine(@"</input>");
                }
                else if (ctl.GetType() == typeof(DomainUpDown))
                {
                    st.WriteLine(@"<input name='{0}' type='number'>", ctl.Name);
                    st.Write(ctl.Text);
                    st.WriteLine(@"</input>");
                }
                else if (ctl.GetType() == typeof(RadioButton))
                {
                    st.WriteLine(@"<input name='{0}' type='radio'>", ctl.Name);
                    st.Write(ctl.Text);
                    st.WriteLine(@"</input>");
                }
                else if (ctl.GetType() == typeof(CheckBox))
                {
                    st.WriteLine(@"<input name='{0}' type='checkbox'>", ctl.Name);
                    st.Write(ctl.Text);
                    st.WriteLine(@"</input>");
                }
                else if (ctl.GetType() == typeof(MyButton) || ctl.GetType() == typeof(Button))
                {
                    st.WriteLine(@"<input name='{0}' type='button' value='{1}'>", ctl.Name, ctl.Text);

                    st.WriteLine(@"</input>");
                }
                else if (ctl.GetType() == typeof(MyLabel) || ctl.GetType() == typeof(Label))
                {
                    st.Write(ctl.Text);
                }
                else if (ctl.GetType() == typeof(FlowLayoutPanel))
                {
                    var flow = ctl as FlowLayoutPanel;

                    st.Write(ctl.Text);
                }
                else if (ctl.GetType() == typeof(VerticalProgressBar2))
                {
                    st.WriteLine(@"<progress name='{0}' type='button' value='{1}' style=' width:100%; height:100%;    margin-top: 100px;    margin-left: -50px;    transform: rotate(90deg);'>", ctl.Name, ctl.Text);

                    st.WriteLine(@"</progress>");
                }
                else
                {
                    st.Write(ctl.Text);
                }

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
xmlns:BackstageView=""using:MissionPlanner.Controls.BackstageView""
xmlns:Controls=""using:MissionPlanner.Controls""
xmlns:GCSViews=""using:MissionPlanner.GCSViews""
xmlns:Wizard=""using:MissionPlanner.Wizard""
xmlns:ConfigurationView=""using:MissionPlanner.GCSViews.ConfigurationView""
xmlns:Custom=""using:Custom""
xmlns:controls=""using:Microsoft.Toolkit.Uwp.UI.Controls""
xmlns:PreFlight=""using:MissionPlanner.Controls.PreFlight""
mc:Ignorable=""d""
> <Grid>";

                    st.Write(header);

                    doxamlctls(control, st);

                    string footer = "</Grid></UserControl>";

                    st.Write(footer);

                    st.Close();

                    var ctl = control;

                    File.WriteAllText(ctl.GetType().FullName + ".xaml.cs", @"namespace " + ctl.GetType().Namespace + " { public partial class " + ctl.GetType().Name + "{public " + ctl.GetType().Name + "(){this.InitializeComponent();}}}");

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
                    || ctl is Coords /*|| ctl is AGaugeApp.AGauge*/|| ctl is MissionPlanner.Controls.HUD
                    || ctl is ImageLabel || ctl is RelayOptions || ctl is CheckListControl
                    || ctl is MavlinkCheckBox)
                {
                    //   st.WriteLine(@"<WindowsFormsHost HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" + ctl.Location.X + "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width + @""" Height=""" + ctl.Height + @""">");

                    string[] names = ctl.GetType().FullName.Split(new char[] { '.' });

                    string name = names[names.Length - 2] + ":" + names[names.Length - 1];

                    st.WriteLine(@"<" + name + @" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" +
                                 ctl.Location.X + "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width +
                                 @""" Height=""" + ctl.Height + @"""></" + name + ">");


                    //st.WriteLine(@"</WindowsFormsHost>");
                }
                else if (ctl is Label || ctl is MyLabel)
                {
                    var label = String.Format(
                        "<{0} Name=\"{1}\" HorizontalAlignment=\"Left\" VerticalAlignment=\"Top\" " +
                        "FontFamily=\"Microsoft Sans Serif\" FontSize=\"{7}\" Margin=\"{3},{4},0,0\" Width=\"{6}\" Height=\"{5}\">{2}</{0}>",
                        "TextBlock", ctl.Name, ctl.Text, ctl.Location.X, ctl.Location.Y,
                        ctl.Size.Height, ctl.Size.Width, ctl.Font.Size);

                    st.WriteLine(label);
                }
                else if (ctl is ComboBox)
                {
                    var label = String.Format(
                        "<{0} Name=\"{1}\" HorizontalAlignment=\"Left\" VerticalAlignment=\"Top\" " +
                        "FontFamily=\"Microsoft Sans Serif\" FontSize=\"{7}\" Margin=\"{3},{4},0,0\" Width=\"{6}\" Height=\"{5}\"></{0}>",
                        "ComboBox", ctl.Name, ctl.Text, ctl.Location.X, ctl.Location.Y,
                        ctl.Size.Height, ctl.Size.Width, ctl.Font.Size);

                    st.WriteLine(label);
                }
                else if (ctl is TextBox)
                {
                    var label = String.Format(
                        "<{0} Name=\"{1}\" HorizontalAlignment=\"Left\" VerticalAlignment=\"Top\" " +
                        "FontFamily=\"Microsoft Sans Serif\" Text=\"{2}\" FontSize=\"{7}\" Margin=\"{3},{4},0,0\" Width=\"{6}\" Height=\"{5}\"></{0}>",
                        "TextBox", ctl.Name, ctl.Text, ctl.Location.X, ctl.Location.Y,
                        ctl.Size.Height, ctl.Size.Width, ctl.Font.Size);

                    st.WriteLine(label);
                }
                else if (ctl is NumericUpDown)
                {
                    st.WriteLine(@"<Custom:DecimalUpDown Name=""" + ctl.Name +
                                 @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" + ctl.Location.X +
                                 "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width + @"""></Custom:DecimalUpDown>");
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
                    var label = String.Format(
                        "<{0} Name=\"{1}\" HorizontalAlignment=\"Left\" VerticalAlignment=\"Top\" " +
                        "FontFamily=\"Microsoft Sans Serif\" FontSize=\"{7}\" Margin=\"{3},{4},0,0\" Width=\"{6}\" Height=\"{5}\">{2}</{0}>",
                        "Button", ctl.Name, ctl.Text, ctl.Location.X, ctl.Location.Y,
                        ctl.Size.Height, ctl.Size.Width, ctl.Font.Size);

                    st.WriteLine(label);
                }
                else if (ctl is Button)
                {
                    var label = String.Format(
                        "<{0} Name=\"{1}\" HorizontalAlignment=\"Left\" VerticalAlignment=\"Top\" " +
                        "FontFamily=\"Microsoft Sans Serif\" FontSize=\"{7}\" Margin=\"{3},{4},0,0\" Width=\"{6}\" Height=\"{5}\">{2}</{0}>",
                        "Button", ctl.Name, ctl.Text, ctl.Location.X, ctl.Location.Y,
                        ctl.Size.Height, ctl.Size.Width, ctl.Font.Size);

                    st.WriteLine(label);
                }
                else if (ctl is CheckBox)
                {
                    var label = String.Format(
                        "<{0} Name=\"{1}\" HorizontalAlignment=\"Left\" VerticalAlignment=\"Top\" " +
                        "FontFamily=\"Microsoft Sans Serif\" FontSize=\"{7}\" Margin=\"{3},{4},0,0\" Width=\"{6}\" Height=\"{5}\">{2}</{0}>",
                        "CheckBox", ctl.Name, ctl.Text, ctl.Location.X, ctl.Location.Y,
                        ctl.Size.Height, ctl.Size.Width, ctl.Font.Size);

                    st.WriteLine(label);
                }
                else if (ctl is RadioButton)
                {
                    var label = String.Format(
                        "<{0} Name=\"{1}\" HorizontalAlignment=\"Left\" VerticalAlignment=\"Top\" " +
                        "FontFamily=\"Microsoft Sans Serif\" FontSize=\"{7}\" Margin=\"{3},{4},0,0\" Width=\"{6}\" Height=\"{5}\">{2}</{0}>",
                        "RadioButton", ctl.Name, ctl.Text, ctl.Location.X, ctl.Location.Y,
                        ctl.Size.Height, ctl.Size.Width, ctl.Font.Size);

                    st.WriteLine(label);
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
                    if (((TrackBar)ctl).Orientation == Orientation.Horizontal)
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
                                 @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" +
                                 ctl.Location.X +
                                 "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Height + @""" Height=""" + ctl.Width +
                                 @""" >" + @"    <ProgressBar.RenderTransform>
                <CompositeTransform Rotation=""90"" TranslateX=""" + ctl.Width + @"""/>
            </ProgressBar.RenderTransform>" + ctl.Text + " </ProgressBar>");
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
                    st.WriteLine(@"<controls:DataGrid Name=""" + ctl.Name +
                                 @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" + ctl.Location.X +
                                 "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width + @""">" + ctl.Text +
                                 "</controls:DataGrid>");
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
                {/*
                    st.WriteLine(@"<TabView Name=""" + ctl.Name +
                                 @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" Margin=""" + ctl.Location.X +
                                 "," + ctl.Location.Y + @",0,0"" Width=""" + ctl.Width + @""" Height=""" + ctl.Height +
                                 @""">");
                                 
                    if (ctl.Controls.Count > 0)
                        doxamlctls(ctl, st);

                    st.WriteLine(@"</TabView>");*/
                }
                else if (ctl is TabPage)
                {
                    /*
                    st.WriteLine(@"<TabViewItem Name=""" + ctl.Name + @""" Header=""" + ctl.Text +
                                 @""" HorizontalAlignment=""Left"" VerticalAlignment=""Top"" ><Grid Width=""" +
                                 ctl.Width + @""" Height=""" + ctl.Height + @""">");

                    if (ctl.Controls.Count > 0)
                        doxamlctls(ctl, st);

                    st.WriteLine(@"</Grid></TabViewItem>");
                    */
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
                if (ctl.GetType() == typeof(Panel))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof(GroupBox))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof(TreeView))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                    TreeView txtr = (TreeView)ctl;
                    txtr.LineColor = TextColor;
                }
                else if (ctl.GetType() == typeof(ListView))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof(SplitContainer))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                    SplitContainer txtr = (SplitContainer)ctl;
                    ApplyCustomTheme(txtr.Panel1, level);
                    ApplyCustomTheme(txtr.Panel2, level);
                }
                else if (ctl.GetType() == typeof(MyLabel))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof(Button))
                {
                    ctl.ForeColor = TextColor;
                    ctl.BackColor = ButBG;
                }
                else if (ctl.GetType() == typeof(MyButton))
                {
                    Controls.MyButton but = (MyButton)ctl;
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
                else if (ctl.GetType() == typeof(TextBox))
                {
                    ctl.BackColor = ControlBGColor;
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
                else if (ctl.GetType() == typeof(GroupBox) || ctl.GetType() == typeof(UserControl))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof(ZedGraph.ZedGraphControl))
                {
                    var zg1 = (ZedGraph.ZedGraphControl)ctl;
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
                else if (ctl.GetType() == typeof(BSE.Windows.Forms.Panel) || ctl.GetType() == typeof(SplitterPanel))
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

                        ctl.BackColor = ControlBGColor;
                        ctl.ForeColor = TextColor;
                        RichTextBox txtr = (RichTextBox)ctl;
                        txtr.BorderStyle = BorderStyle.None;
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
                    ctl.BackColor = BGColor; //ControlBGColor
                    ctl.ForeColor = TextColor;
                    TabPage txtr = (TabPage)ctl;
                    txtr.BorderStyle = BorderStyle.None;
                }
                else if (ctl.GetType() == typeof(TabControl))
                {
                    ctl.BackColor = BGColor; //ControlBGColor
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

                    DataGridViewCellStyle hs = new DataGridViewCellStyle(dgv.ColumnHeadersDefaultCellStyle);
                    hs.BackColor = BGColor;
                    hs.ForeColor = TextColor;

                    dgv.ColumnHeadersDefaultCellStyle = hs;
                    dgv.RowHeadersDefaultCellStyle = hs;

                    dgv.AlternatingRowsDefaultCellStyle.BackColor = BGColor;
                }
                else if (ctl.GetType() == typeof(CheckBox) || ctl.GetType() == typeof(MavlinkCheckBox))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                    CheckBox CHK = (CheckBox)ctl;
                    // CHK.FlatStyle = FlatStyle.Flat;
                }
                else if (ctl.GetType() == typeof(ComboBox) || ctl.GetType() == typeof(MavlinkComboBox))
                {
                    ctl.BackColor = ControlBGColor;
                    ctl.ForeColor = TextColor;
                    ComboBox CMB = (ComboBox)ctl;
                    CMB.FlatStyle = FlatStyle.Flat;
                }
                else if (ctl.GetType() == typeof(NumericUpDown) || ctl.GetType() == typeof(MavlinkNumericUpDown))
                {
                    ctl.BackColor = ControlBGColor;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof(TrackBar))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof(LinkLabel))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                    LinkLabel LNK = (LinkLabel)ctl;
                    LNK.ActiveLinkColor = TextColor;
                    LNK.LinkColor = TextColor;
                    LNK.VisitedLinkColor = TextColor;
                }
                else if (ctl.GetType() == typeof(BackstageView))
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
                else if (ctl.GetType() == typeof(HorizontalProgressBar2) ||
                         ctl.GetType() == typeof(VerticalProgressBar2))
                {
                    ((HorizontalProgressBar2)ctl).BackgroundColor = ControlBGColor;
                    ((HorizontalProgressBar2)ctl).ValueColor = Color.FromArgb(148, 193, 31);
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
                else if (ctl.GetType() == typeof(QuickView))
                {
                    //set default QuickView item color to mix with background
                    Color mix = Color.FromArgb(ThemeManager.BGColor.ToArgb() ^ 0xffffff);

                    Controls.QuickView but = (QuickView)ctl;
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
                else if (ctl.GetType() == typeof(TreeView))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                    TreeView txtr = (TreeView)ctl;
                    txtr.LineColor = TextColor;
                }
                else if (ctl.GetType() == typeof(ListView))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof(SplitContainer))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                    SplitContainer txtr = (SplitContainer)ctl;
                    ApplyCustomTheme(txtr.Panel1, level);
                    ApplyCustomTheme(txtr.Panel2, level);
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
                    CheckBox CHK = (CheckBox)ctl;
                    // CHK.FlatStyle = FlatStyle.Flat;
                }
                else if (ctl.GetType() == typeof(ComboBox) || ctl.GetType() == typeof(MavlinkComboBox))
                {
                    ctl.BackColor = ControlBGColor;
                    ctl.ForeColor = TextColor;
                    ComboBox CMB = (ComboBox)ctl;
                    CMB.FlatStyle = FlatStyle.Flat;
                }
                else if (ctl.GetType() == typeof(NumericUpDown) || ctl.GetType() == typeof(MavlinkNumericUpDown))
                {
                    ctl.BackColor = BGColorTextBox;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof(TrackBar))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                }
                else if (ctl.GetType() == typeof(LinkLabel))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;
                    LinkLabel LNK = (LinkLabel)ctl;
                    LNK.ActiveLinkColor = TextColor;
                    LNK.LinkColor = TextColor;
                    LNK.VisitedLinkColor = TextColor;
                }
                else if (ctl.GetType() == typeof(BackstageView))
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
                else if (ctl.GetType() == typeof(HorizontalProgressBar2) ||
                         ctl.GetType() == typeof(VerticalProgressBar2))
                {
                    ((HorizontalProgressBar2)ctl).BackgroundColor = ControlBGColor;
                    ((HorizontalProgressBar2)ctl).ValueColor = HorizontalPBValueColor;
                }
                else if (ctl.GetType() == typeof(MyProgressBar))
                {
                    ((MyProgressBar)ctl).BGGradTop = ProgressBarColorTop;
                    ((MyProgressBar)ctl).BGGradBot = ProgressBarColorBot;
                    ((MyProgressBar)ctl).Outline = ProgressBarOutlineColor;        //sets the colour of the progress bar box
                }
                if ((ctl.Controls.Count > 0) && (ctl.GetType() != typeof(QuickView)))      //Do not iterate into quickView type leave labels as they are
                    ApplyTheme(ctl, 1);

            }
        }

        public static void Init()
        {

            thmColor = new ThemeColorTable();


        }

        /// <summary>
        /// Writes the given object instance to an XML file.
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [XmlIgnore] attribute.</para>
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                writer = new StreamWriter(filePath, append);
                serializer.Serialize(writer, objectToWrite);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Reads an object instance from an XML file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the XML file.</returns>
        public static T ReadFromXmlFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;

            try
            {
                var serializer = new XmlSerializer(typeof(T));
                reader = new StreamReader(filePath);
                return (T)serializer.Deserialize(reader);
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return default(T);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

    }
}