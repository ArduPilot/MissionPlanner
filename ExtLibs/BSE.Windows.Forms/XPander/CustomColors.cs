using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace BSE.Windows.Forms
{
    /// <summary>
    /// Base class for the custom colors at a panel or xpanderpanel control. 
    /// </summary>
    /// <remarks>
    /// If you use the <see cref="ColorScheme.Custom"/> ColorScheme, this is the base class for the custom colors.
    /// </remarks>
    /// <copyright>Copyright © 2008 Uwe Eichkorn
    /// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    /// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    /// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    /// PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
    /// REMAINS UNCHANGED.
    /// </copyright>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Description("The colors used in a panel")]
    public class CustomColors
    {
        #region Events
        /// <summary>
        /// Occurs when the value of the CustomColors property changes.
        /// </summary>
        [Description("Occurs when the value of the CustomColors property changes.")]
        public event EventHandler<EventArgs> CustomColorsChanged;
        #endregion
        
        #region FieldsPrivate
        private Color m_borderColor = System.Windows.Forms.ProfessionalColors.GripDark;
        private Color m_captionCloseIcon = SystemColors.ControlText;
        private Color m_captionExpandIcon = SystemColors.ControlText;
        private Color m_captionGradientBegin = System.Windows.Forms.ProfessionalColors.ToolStripGradientBegin;
        private Color m_captionGradientEnd = System.Windows.Forms.ProfessionalColors.ToolStripGradientEnd;
        private Color m_captionGradientMiddle = System.Windows.Forms.ProfessionalColors.ToolStripGradientMiddle;
        private Color m_captionText = SystemColors.ControlText;
        private Color m_innerBorderColor = System.Windows.Forms.ProfessionalColors.GripLight;
        
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the border color of a Panel or XPanderPanel.
        /// </summary>
        [Description("The border color of a Panel or XPanderPanel.")]
        public virtual Color BorderColor
        {
            get { return this.m_borderColor; }
            set
            {
                if (value.Equals(this.m_borderColor) == false)
                {
                    this.m_borderColor = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the forecolor of a close icon in a Panel or XPanderPanel.
        /// </summary>
        [Description("The forecolor of a close icon in a Panel or XPanderPanel.")]
        public virtual Color CaptionCloseIcon
        {
            get { return this.m_captionCloseIcon; }
            set
            {
                if (value.Equals(this.m_captionCloseIcon) == false)
                {
                    this.m_captionCloseIcon = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the forecolor of an expand icon in a Panel or XPanderPanel.
        /// </summary>
        [Description("The forecolor of an expand icon in a Panel or XPanderPanel.")]
        public virtual Color CaptionExpandIcon
        {
            get { return this.m_captionExpandIcon; }
            set
            {
                if (value.Equals(this.m_captionExpandIcon) == false)
                {
                    this.m_captionExpandIcon = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the starting color of the gradient at the caption on a Panel or XPanderPanel.
        /// </summary>
        [Description("The starting color of the gradient at the caption on a Panel or XPanderPanel.")]
        public virtual Color CaptionGradientBegin
        {
            get { return this.m_captionGradientBegin; }
            set
            {
                if (value.Equals(this.m_captionGradientBegin) == false)
                {
                    this.m_captionGradientBegin = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the end color of the gradient at the caption on a Panel or XPanderPanel.
        /// </summary>
        [Description("The end color of the gradient at the caption on a Panel or XPanderPanel")]
        public virtual Color CaptionGradientEnd
        {
            get { return this.m_captionGradientEnd; }
            set
            {
                if (value.Equals(this.m_captionGradientEnd) == false)
                {
                    this.m_captionGradientEnd = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the middle color of the gradient at the caption on a Panel or XPanderPanel.
        /// </summary>
        [Description("The middle color of the gradient at the caption on a Panel or XPanderPanel.")]
        public virtual Color CaptionGradientMiddle
        {
            get { return this.m_captionGradientMiddle; }
            set
            {
                if (value.Equals(this.m_captionGradientMiddle) == false)
                {
                    this.m_captionGradientMiddle = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the text color at the caption on a Panel or XPanderPanel.
        /// </summary>
        [Description("The text color at the caption on a Panel or XPanderPanel.")]
        public virtual Color CaptionText
        {
            get { return this.m_captionText; }
            set
            {
                if (value.Equals(this.m_captionText) == false)
                {
                    this.m_captionText = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the inner border color of a Panel.
        /// </summary>
        [Description("The inner border color of a Panel.")]
        public virtual Color InnerBorderColor
        {
            get { return this.m_innerBorderColor; }
            set
            {
                if (value.Equals(this.m_innerBorderColor) == false)
                {
                    this.m_innerBorderColor = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }
        #endregion

        #region MethodsProtected
        /// <summary>
        /// Raises the CustomColors changed event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A EventArgs that contains the event data.</param>
        protected virtual void OnCustomColorsChanged(object sender, EventArgs e)
        {
            if (this.CustomColorsChanged != null)
            {
                this.CustomColorsChanged(sender, e);
            }
        }
        #endregion
    }
}
