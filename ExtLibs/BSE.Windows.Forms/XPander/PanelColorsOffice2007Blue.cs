using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace BSE.Windows.Forms
{
    /// <summary>
    /// Provide Office 2007 Blue Theme colors
    /// </summary>
    /// <copyright>Copyright © 2006-2008 Uwe Eichkorn
    /// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    /// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    /// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    /// PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
    /// REMAINS UNCHANGED.
    /// </copyright>
    public class PanelColorsOffice2007Blue : PanelColorsOffice
    {
		#region FieldsPrivate
		#endregion

		#region Properties
        #endregion

        #region MethodsPublic
        /// <summary>
        /// Initialize a new instance of the Office2007Colors class.
        /// </summary>
        public PanelColorsOffice2007Blue()
			: base()
		{
		}
        /// <summary>
        /// Initialize a new instance of the Office2007Colors class.
        /// </summary>
        /// <param name="basePanel">Base class for the panel or xpanderpanel control.</param>
        public PanelColorsOffice2007Blue(BasePanel basePanel)
            : base(basePanel)
        {
        }

        #endregion

        #region MethodsProtected
        /// <summary>
        /// Initialize a color Dictionary with defined colors
        /// </summary>
        /// <param name="rgbTable">Dictionary with defined colors</param>
        protected override void InitColors(Dictionary<PanelColors.KnownColors, Color> rgbTable)
        {
            base.InitColors(rgbTable);
            rgbTable[KnownColors.BorderColor] = Color.FromArgb(101, 147, 207);
            rgbTable[KnownColors.InnerBorderColor] = Color.White;
            rgbTable[KnownColors.PanelCaptionCloseIcon] = Color.Black;
            rgbTable[KnownColors.PanelCaptionExpandIcon] = Color.FromArgb(21, 66, 139);
            rgbTable[KnownColors.PanelCaptionGradientBegin] = Color.FromArgb(227, 239, 255);
            rgbTable[KnownColors.PanelCaptionGradientEnd] = Color.FromArgb(173, 209, 255);
            rgbTable[KnownColors.PanelCaptionGradientMiddle] = Color.FromArgb(199, 224, 255);
            rgbTable[KnownColors.PanelContentGradientBegin] = Color.FromArgb(227, 239, 255);
            rgbTable[KnownColors.PanelContentGradientEnd] = Color.FromArgb(227, 239, 255);
            rgbTable[KnownColors.PanelCaptionText] = Color.FromArgb(22, 65, 139);
            rgbTable[KnownColors.PanelCollapsedCaptionText] = Color.FromArgb(21, 66, 139);
            rgbTable[KnownColors.XPanderPanelBackColor] = Color.Transparent;
            rgbTable[KnownColors.XPanderPanelCaptionCloseIcon] = Color.Black;
            rgbTable[KnownColors.XPanderPanelCaptionExpandIcon] = Color.FromArgb(21, 66, 139);
            rgbTable[KnownColors.XPanderPanelCaptionText] = Color.FromArgb(21, 66, 139);
            rgbTable[KnownColors.XPanderPanelCaptionGradientBegin] = Color.FromArgb(227, 239, 255);
            rgbTable[KnownColors.XPanderPanelCaptionGradientEnd] = Color.FromArgb(199, 224, 255);
            rgbTable[KnownColors.XPanderPanelCaptionGradientMiddle] = Color.FromArgb(173, 209, 255);
            rgbTable[KnownColors.XPanderPanelFlatCaptionGradientBegin] = Color.FromArgb(214, 232, 255);
            rgbTable[KnownColors.XPanderPanelFlatCaptionGradientEnd] = Color.FromArgb(253, 253, 254);
        }

        #endregion

        #region MethodsPrivate
        #endregion
    }
}
