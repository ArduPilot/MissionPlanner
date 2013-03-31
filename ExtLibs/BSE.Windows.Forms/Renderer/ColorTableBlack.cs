using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BSE.Windows.Forms
{
    /// <summary>
    /// Provide Office 2007 black theme colors
    /// </summary>
    public class ColorTableBlack : BSE.Windows.Forms.BseColorTable
    {
        #region FieldsPrivate
        private PanelColors m_panelColorTable;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the associated ColorTable for the XPanderControls
        /// </summary>
        public override PanelColors PanelColorTable
        {
            get
            {
                if (this.m_panelColorTable == null)
                {
                    this.m_panelColorTable = new PanelColorsBlack();
                }
                return this.m_panelColorTable;
            }
        }
        #endregion
        
        #region MethodsProtected
        /// <summary>
        /// Initializes a color dictionary with defined colors
        /// </summary>
        /// <param name="rgbTable">Dictionary with defined colors</param>
        protected override void InitColors(Dictionary<ProfessionalColorTable.KnownColors, Color> rgbTable)
        {
            base.InitColors(rgbTable);
            rgbTable[KnownColors.ButtonPressedBorder] = Color.FromArgb(145, 153, 164);
            rgbTable[KnownColors.ButtonPressedGradientBegin] = Color.FromArgb(141, 170, 253);
            rgbTable[KnownColors.ButtonPressedGradientEnd] = Color.FromArgb(98, 101, 252);
            rgbTable[KnownColors.ButtonPressedGradientMiddle] = Color.FromArgb(43, 93, 255);
            rgbTable[KnownColors.ButtonSelectedGradientBegin] = Color.FromArgb(106, 109, 228);
            rgbTable[KnownColors.ButtonSelectedGradientEnd] = Color.FromArgb(88, 111, 226);
            rgbTable[KnownColors.ButtonSelectedGradientMiddle] = Color.FromArgb(39, 39, 217);
			rgbTable[KnownColors.ButtonSelectedHighlightBorder] = Color.FromArgb(145, 153, 164);
            rgbTable[KnownColors.GripDark] = Color.FromArgb(102, 102, 102);
            rgbTable[KnownColors.GripLight] = Color.FromArgb(182, 182, 182);
            rgbTable[KnownColors.ImageMarginGradientBegin] = Color.FromArgb(239, 239, 239);
            rgbTable[KnownColors.MenuBorder] = Color.FromArgb(0, 0, 0);
            rgbTable[KnownColors.MenuItemSelectedGradientBegin] = Color.FromArgb(231, 239, 243);
            rgbTable[KnownColors.MenuItemSelectedGradientEnd] = Color.FromArgb(218, 235, 243);
            rgbTable[KnownColors.MenuItemText] = Color.FromArgb(255, 255, 255);
            rgbTable[KnownColors.MenuItemTopLevelSelectedBorder] = Color.FromArgb(145, 153, 164);
            rgbTable[KnownColors.MenuItemTopLevelSelectedGradientBegin] = Color.FromArgb(205, 208, 213);
            rgbTable[KnownColors.MenuStripGradientBegin] = Color.FromArgb(102, 102, 102);
            rgbTable[KnownColors.MenuStripGradientEnd] = Color.FromArgb(0, 0, 0);
            rgbTable[KnownColors.OverflowButtonGradientBegin] = Color.FromArgb(136, 144, 254);
            rgbTable[KnownColors.OverflowButtonGradientEnd] = Color.FromArgb(111, 145, 255);
            rgbTable[KnownColors.OverflowButtonGradientMiddle] = Color.FromArgb(42, 52, 254);
            rgbTable[KnownColors.RaftingContainerGradientBegin] = Color.FromArgb(83, 83, 83);
            rgbTable[KnownColors.RaftingContainerGradientEnd] = Color.FromArgb(83, 83, 83);
            rgbTable[KnownColors.SeparatorDark] = Color.FromArgb(102, 102, 102);
            rgbTable[KnownColors.SeparatorLight] = Color.FromArgb(182, 182, 182);
            rgbTable[KnownColors.StatusStripGradientBegin] = Color.FromArgb(100, 100, 100);
            rgbTable[KnownColors.StatusStripGradientEnd] = Color.FromArgb(0, 0, 0);
			rgbTable[KnownColors.StatusStripText] = Color.FromArgb(255, 255, 255);
            rgbTable[KnownColors.ToolStripBorder] = Color.FromArgb(102, 102, 102);
            rgbTable[KnownColors.ToolStripContentPanelGradientBegin] = Color.FromArgb(42, 42, 42);
            rgbTable[KnownColors.ToolStripContentPanelGradientEnd] = Color.FromArgb(10, 10, 10);
            rgbTable[KnownColors.ToolStripDropDownBackground] = Color.FromArgb(250, 250, 250);
            rgbTable[KnownColors.ToolStripGradientBegin] = Color.FromArgb(102, 102, 102);
            rgbTable[KnownColors.ToolStripGradientEnd] = Color.FromArgb(0, 0, 0);
            rgbTable[KnownColors.ToolStripGradientMiddle] = Color.FromArgb(52, 52, 52);
            rgbTable[KnownColors.ToolStripPanelGradientBegin] = Color.FromArgb(12, 12, 12);
            rgbTable[KnownColors.ToolStripPanelGradientEnd] = Color.FromArgb(2, 2, 2);
            rgbTable[KnownColors.ToolStripText] = Color.FromArgb(255, 255, 255);
        }

        #endregion
    }
}
