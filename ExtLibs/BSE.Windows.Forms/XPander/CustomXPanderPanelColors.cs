using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace BSE.Windows.Forms
{
    /// <summary>
    /// Class for the custom colors at a XPanderPanel control. 
    /// </summary>
    /// <copyright>Copyright © 2008 Uwe Eichkorn
    /// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    /// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    /// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    /// PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
    /// REMAINS UNCHANGED.
    /// </copyright>
    public class CustomXPanderPanelColors : CustomColors
    {
        #region FieldsPrivate
        private Color m_backColor = SystemColors.Control;
        private Color m_flatCaptionGradientBegin = System.Windows.Forms.ProfessionalColors.ToolStripGradientMiddle;
        private Color m_flatCaptionGradientEnd = System.Windows.Forms.ProfessionalColors.ToolStripGradientBegin;
        private Color m_captionPressedGradientBegin = System.Windows.Forms.ProfessionalColors.ButtonPressedGradientBegin;
        private Color m_captionPressedGradientEnd = System.Windows.Forms.ProfessionalColors.ButtonPressedGradientEnd;
        private Color m_captionPressedGradientMiddle = System.Windows.Forms.ProfessionalColors.ButtonPressedGradientMiddle;
        private Color m_captionCheckedGradientBegin = System.Windows.Forms.ProfessionalColors.ButtonCheckedGradientBegin;
        private Color m_captionCheckedGradientEnd = System.Windows.Forms.ProfessionalColors.ButtonCheckedGradientEnd;
        private Color m_captionCheckedGradientMiddle = System.Windows.Forms.ProfessionalColors.ButtonCheckedGradientMiddle;
        private Color m_captionSelectedGradientBegin = System.Windows.Forms.ProfessionalColors.ButtonSelectedGradientBegin;
        private Color m_captionSelectedGradientEnd = System.Windows.Forms.ProfessionalColors.ButtonSelectedGradientEnd;
        private Color m_captionSelectedGradientMiddle = System.Windows.Forms.ProfessionalColors.ButtonSelectedGradientMiddle;
        private Color m_captionSelectedText = SystemColors.ControlText;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the backcolor of a XPanderPanel.
        /// </summary>
        [Description("The backcolor of a XPanderPanel.")]
        public virtual Color BackColor
        {
            get { return this.m_backColor; }
            set
            {
                if (value.Equals(this.m_backColor) == false)
                {
                    this.m_backColor = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the starting color of the gradient on a flat XPanderPanel captionbar.
        /// </summary>
        [Description("The starting color of the gradient on a flat XPanderPanel captionbar.")]
        public virtual Color FlatCaptionGradientBegin
        {
            get { return this.m_flatCaptionGradientBegin; }
            set
            {
                if (value.Equals(this.m_flatCaptionGradientBegin) == false)
                {
                    this.m_flatCaptionGradientBegin = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the end color of the gradient on a flat XPanderPanel captionbar.
        /// </summary>
        [Description("The end color of the gradient on a flat XPanderPanel captionbar.")]
        public virtual Color FlatCaptionGradientEnd
        {
            get { return this.m_flatCaptionGradientEnd; }
            set
            {
                if (value.Equals(this.m_flatCaptionGradientEnd) == false)
                {
                    this.m_flatCaptionGradientEnd = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the starting color of the gradient used when the XPanderPanel is pressed down.
        /// </summary>
        [Description("The starting color of the gradient used when the XPanderPanel is pressed down.")]
        public virtual Color CaptionPressedGradientBegin
        {
            get { return this.m_captionPressedGradientBegin; }
            set
            {
                if (value.Equals(this.m_captionPressedGradientBegin) == false)
                {
                    this.m_captionPressedGradientBegin = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the end color of the gradient used when the XPanderPanel is pressed down.
        /// </summary>
        [Description("The end color of the gradient used when the XPanderPanel is pressed down.")]
        public virtual Color CaptionPressedGradientEnd
        {
            get { return this.m_captionPressedGradientEnd; }
            set
            {
                if (value.Equals(this.m_captionPressedGradientEnd) == false)
                {
                    this.m_captionPressedGradientEnd = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the middle color of the gradient used when the XPanderPanel is pressed down.
        /// </summary>
        [Description("The middle color of the gradient used when the XPanderPanel is pressed down.")]
        public virtual Color CaptionPressedGradientMiddle
        {
            get { return this.m_captionPressedGradientMiddle; }
            set
            {
                if (value.Equals(this.m_captionPressedGradientMiddle) == false)
                {
                    this.m_captionPressedGradientMiddle = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the starting color of the gradient used when the XPanderPanel is checked.
        /// </summary>
        [Description("The starting color of the gradient used when the XPanderPanel is checked.")]
        public virtual Color CaptionCheckedGradientBegin
        {
            get { return this.m_captionCheckedGradientBegin; }
            set
            {
                if (value.Equals(this.m_captionCheckedGradientBegin) == false)
                {
                    this.m_captionCheckedGradientBegin = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the end color of the gradient used when the XPanderPanel is checked.
        /// </summary>
        [Description("The end color of the gradient used when the XPanderPanel is checked.")]
        public virtual Color CaptionCheckedGradientEnd
        {
            get { return this.m_captionCheckedGradientEnd; }
            set
            {
                if (value.Equals(this.m_captionCheckedGradientEnd) == false)
                {
                    this.m_captionCheckedGradientEnd = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the middle color of the gradient used when the XPanderPanel is checked.
        /// </summary>
        [Description("The middle color of the gradient used when the XPanderPanel is checked.")]
        public virtual Color CaptionCheckedGradientMiddle
        {
            get { return this.m_captionCheckedGradientMiddle; }
            set
            {
                if (value.Equals(this.m_captionCheckedGradientMiddle) == false)
                {
                    this.m_captionCheckedGradientMiddle = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the starting color of the gradient used when the XPanderPanel is selected.
        /// </summary>
        [Description("The starting color of the gradient used when the XPanderPanel is selected.")]
        public virtual Color CaptionSelectedGradientBegin
        {
            get { return this.m_captionSelectedGradientBegin; }
            set
            {
                if (value.Equals(this.m_captionSelectedGradientBegin) == false)
                {
                    this.m_captionSelectedGradientBegin = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the end color of the gradient used when the XPanderPanel is selected.
        /// </summary>
        [Description("The end color of the gradient used when the XPanderPanel is selected.")]
        public virtual Color CaptionSelectedGradientEnd
        {
            get { return this.m_captionSelectedGradientEnd; }
            set
            {
                if (value.Equals(this.m_captionSelectedGradientEnd) == false)
                {
                    this.m_captionSelectedGradientEnd = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the middle color of the gradient used when the XPanderPanel is selected.
        /// </summary>
        [Description("The middle color of the gradient used when the XPanderPanel is selected.")]
        public virtual Color CaptionSelectedGradientMiddle
        {
            get { return this.m_captionSelectedGradientMiddle; }
            set
            {
                if (value.Equals(this.m_captionSelectedGradientMiddle) == false)
                {
                    this.m_captionSelectedGradientMiddle = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the text color used when the XPanderPanel is selected.
        /// </summary>
        [Description("The text color used when the XPanderPanel is selected.")]
        public virtual Color CaptionSelectedText
        {
            get { return this.m_captionSelectedText; }
            set
            {
                if (value.Equals(this.m_captionSelectedText) == false)
                {
                    this.m_captionSelectedText = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }
        #endregion
    }
}
