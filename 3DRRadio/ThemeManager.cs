﻿using System;
using System.Drawing;
using System.Windows.Forms;
using MissionPlanner.Controls.BackstageView;
using log4net;
using MissionPlanner.Controls;

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
        }

        /// <summary>
        /// Change the current theme. Existing controls are not affected
        /// </summary>
        /// <param name="theme"></param>
        public static void SetTheme(Themes theme)
        {
            log.Debug("Theme set to " +  Enum.GetName(typeof(Themes), theme));
            _currentTheme = theme;
        }

        /// <summary>
        /// Will recursively apply the current theme to 'control'
        /// </summary>
        /// <param name="control"></param>
        public static void ApplyThemeTo(Control control)
        {
            switch (_currentTheme)
            {
                case Themes.BurntKermit: ApplyBurntKermitTheme(control, 0);
                    break;

                case Themes.None: ApplyNone(control, 0);
                    break;

                    // More themes go here


                default:
                    break;
            }

                    
        }

        private static void ApplyNone(Control temp, int level)
        {
            foreach (Control ctl in temp.Controls)
            {
                if (ctl.GetType() == typeof(MyButton))
                {
                    Controls.MyButton but = (MyButton)ctl;
                    but.BGGradTop = Color.FromArgb(242,242,242);
                    but.BGGradBot = Color.FromArgb(207,207,207);
                    but.ForeColor = Color.Black;
                    but.TextColor = Color.Black;
                    but.Outline = Color.FromArgb(112,112,112);
                }

                if (ctl.Controls.Count > 0)
                    ApplyNone(ctl, 1);
            }
        }

        private static void ApplyBurntKermitTheme(Control temp, int level)
        {
            Color BGColor = Color.FromArgb(0x26, 0x27, 0x28); // background
            Color ControlBGColor = Color.FromArgb(0x43, 0x44, 0x45); // editable bg color
            Color TextColor = Color.White;

            if (level == 0)
            {
                temp.BackColor = BGColor;
                temp.ForeColor = TextColor;// Color.FromArgb(0xe6, 0xe8, 0xea);
            }

            //temp.Font = new Font("Lucida Console", 8.25f);

            foreach (Control ctl in temp.Controls)
            {
                if (ctl.GetType() == typeof(Button))
                {
                    ctl.ForeColor = Color.Black;
                }
                else if (ctl.GetType() == typeof(MyButton))
                {
                    Color PrimeColor = Color.FromArgb(0x94, 0xc1, 0x1f);

                    Controls.MyButton but = (MyButton)ctl;
                    //but.BGGradTop = Color.FromArgb(PrimeColor.R, PrimeColor.G, PrimeColor.B);
                    //but.BGGradBot = Color.FromArgb(255 - (int)(PrimeColor.R * 0.27), 255 - (int)(PrimeColor.G * 0.14), 255 - (int)(PrimeColor.B * 0.79));
                    //but.ForeColor = Color.FromArgb(0x40, 0x57, 0x04); //Color.FromArgb(255 - (int)(PrimeColor.R * 0.7), 255 - (int)(PrimeColor.G * 0.8), 255 - (int)(PrimeColor.B * 0.1));
                    //but.Outline = ControlBGColor;
                }
                else if (ctl.GetType() == typeof(TextBox))
                {
                    ctl.BackColor = ControlBGColor;
                    ctl.ForeColor = TextColor;// Color.FromArgb(0xe6, 0xe8, 0xea);
                    TextBox txt = (TextBox)ctl;
                    txt.BorderStyle = BorderStyle.None;
                }
                else if (ctl.GetType() == typeof(DomainUpDown))
                {
                    ctl.BackColor = ControlBGColor;
                    ctl.ForeColor = TextColor;// Color.FromArgb(0xe6, 0xe8, 0xea);
                    DomainUpDown txt = (DomainUpDown)ctl;
                    txt.BorderStyle = BorderStyle.None;
                }
                else if (ctl.GetType() == typeof(GroupBox))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;// Color.FromArgb(0xe6, 0xe8, 0xea);
                }
               
                else if (ctl.GetType() == typeof(Form))
                {
                    ctl.BackColor = BGColor;
                    ctl.ForeColor = TextColor;// Color.FromArgb(0xe6, 0xe8, 0xea);
                }
                else if (ctl.GetType() == typeof(RichTextBox))
                {
                    ctl.BackColor = ControlBGColor;
                    ctl.ForeColor = TextColor;
                    RichTextBox txtr = (RichTextBox)ctl;
                    txtr.BorderStyle = BorderStyle.None;
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
                    ctl.BackColor = BGColor;  //ControlBGColor
                    ctl.ForeColor = TextColor;
                    TabPage txtr = (TabPage)ctl;
                    txtr.BorderStyle = BorderStyle.None;
                }
                else if (ctl.GetType() == typeof(TabControl))
                {
                    ctl.BackColor = BGColor;  //ControlBGColor
                    ctl.ForeColor = TextColor;
                    TabControl txtr = (TabControl)ctl;

                }
                else if (ctl.GetType() == typeof(DataGridView))
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
                }
                else if (ctl.GetType() == typeof(ComboBox))
                {
                    ctl.BackColor = ControlBGColor;
                    ctl.ForeColor = TextColor;
                    ComboBox CMB = (ComboBox)ctl;
                    CMB.FlatStyle = FlatStyle.Flat;
                }
                else if (ctl.GetType() == typeof(NumericUpDown))
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
        
                if (ctl.Controls.Count > 0)
                    ApplyBurntKermitTheme(ctl, 1);
            }
        }

    }
}