using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing;
using System.ComponentModel;

namespace BSE.Windows.Forms
{
    /// <summary>
    /// Represents a Windows progress bar control contained in a StatusStrip.
    /// </summary>
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
    [ToolboxBitmap(typeof(System.Windows.Forms.ProgressBar))]
    public class ToolStripProgressBar : ToolStripControlHost
    {
        #region Events
        #endregion

        #region Constants
        #endregion

        #region FieldsPrivate
        #endregion

        #region Properties
        /// <summary>
        /// Gets the ProgressBar.
        /// </summary>
        /// <value>
        /// Type: <see cref="BSE.Windows.Forms.ProgressBar"/>
        /// A ProgressBar.
        /// </value>
        public ProgressBar ProgressBar
        {
            get { return base.Control as ProgressBar; }
        }
        /// <summary>
        /// Gets or sets a value indicating whether items are to be placed from right to left
        /// and text is to be written from right to left.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.Windows.Forms.RightToLeft"/>
        /// true if items are to be placed from right to left and text is to be written from right to left; otherwise, false.
        /// </value>
        public override RightToLeft RightToLeft
        {
            get { return this.ProgressBar.RightToLeft; }
            set { this.ProgressBar.RightToLeft = value; }
        }
        /// <summary>
        /// Gets or sets the color used for the background rectangle for this <see cref="ToolStripProgressBar"/>.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.Drawing.Color"/>
        /// A Color used for the background rectangle of this ToolStripProgressBar.
        /// </value>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The color used for the background rectangle of this control.")]
        public Color BackgroundColor
        {
            get { return this.ProgressBar.BackgroundColor; }
            set { this.ProgressBar.BackgroundColor = value; }
        }
        /// <summary>
        /// Gets or sets the color used for the value rectangle of this control.
        /// Gets or sets color used for the value rectangle for this <see cref="ToolStripProgressBar"/>.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.Drawing.Color"/>
        /// A Color used for the value rectangle for this ToolStripProgressBar.
        /// </value>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The end color of the gradient used for the value rectangle of this control.")]
        public Color ValueColor
        {
            get { return this.ProgressBar.ValueColor; }
            set { this.ProgressBar.ValueColor = value; }
        }
        /// <summary>
        /// Gets or sets the foreground color of the hosted control.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.Drawing.Color"/>
        /// A <see cref="Color"/> representing the foreground color of the hosted control.
        /// </value>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The Foreground color used to display text on the progressbar")]
        public override Color ForeColor
        {
            get { return this.ProgressBar.ForeColor; }
            set { this.ProgressBar.ForeColor = value; }
        }
        /// <summary>
        /// Gets or sets the font to be used on the hosted control.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.Drawing.Font"/>
        /// The Font for the hosted control.
        /// </value>
        [Category("Appearance")]
        [Description("The font used to display text on the progressbar")]
        public override Font Font
        {
            get { return this.ProgressBar.Font; }
            set { this.ProgressBar.Font = value; }
        }
        /// <summary>
        /// Gets or sets the upper bound of the range that is defined for this <see cref="ToolStripProgressBar"/>.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.Int32"/>
        /// An integer representing the upper bound of the range. The default is 100.
        /// </value>
        [Category("Behavior")]
        [DefaultValue(100)]
        [RefreshProperties(RefreshProperties.All)]
        [Description("The upper bound of the range this progressbar is working with.")]
        public int Maximum
        {
            get { return this.ProgressBar.Maximum; }
            set { this.ProgressBar.Maximum = value; }
        }
        /// <summary>
        /// Gets or sets the lower bound of the range that is defined for this <see cref="ToolStripProgressBar"/>.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.Int32"/>
        /// An integer representing the lower bound of the range. The default is 0.
        /// </value>
        [Category("Behavior")]
        [DefaultValue(0)]
        [RefreshProperties(RefreshProperties.All)]
        [Description("The lower bound of the range this progressbar is working with.")]
        public int Minimum
        {
            get { return this.ProgressBar.Minimum; }
            set { this.ProgressBar.Minimum = value; }
        }
        /// <summary>
        /// Gets or sets the current value of the <see cref="ToolStripProgressBar"/>.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.Int32"/>
        /// An integer representing the current value.
        /// </value>
        [Category("Behavior")]
        [DefaultValue(0)]
        [RefreshProperties(RefreshProperties.All)]
        [Description("The current value for the progressbar, in the range specified by the minimum and maximum.")]
        public int Value
        {
            get { return this.ProgressBar.Value; }
            set { this.ProgressBar.Value = value; }
        }
        /// <summary>
        /// Gets or sets the text displayed on the <see cref="BSE.Windows.Forms.ToolStripProgressBar"/>.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.String"/>
        /// A <see cref="System.String"/> representing the display text.
        /// </value>
        [Category("Behavior")]
        [Description("The text to display on the progressbar")]
        public override string Text
        {
            get { return this.ProgressBar.Text; }
            set { this.ProgressBar.Text = value; }
        }
        /// <summary>
        /// Gets the height and width of the ToolStripProgressBar in pixels.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.Drawing.Size"/>
        /// A Point value representing the height and width.
        /// </value>
        protected override Size DefaultSize
        {
            get { return new Size(100, 15); }
        }
        /// <summary>
        /// Gets the spacing between the <see cref="ToolStripProgressBar"/> and adjacent items.
        /// </summary>
        protected override Padding DefaultMargin
        {
            get
            {
                if ((base.Owner != null) && (base.Owner is StatusStrip))
                {
                    return new Padding(1, 3, 1, 3);
                }
                return new Padding(1, 1, 1, 2);
            }
        }
        #endregion

        #region MethodsPublic
        /// <summary>
        /// Initializes a new instance of the ToolStripProgressBar class. 
        /// </summary>
        public ToolStripProgressBar()
            : base(CreateControlInstance())
        {
        }
        #endregion

        #region MethodsProtected
        /// <summary>
        /// Raises the OwnerChanged event. 
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnOwnerChanged(EventArgs e)
        {
            if (base.Owner != null)
            {
                base.Owner.RendererChanged += new EventHandler(OwnerRendererChanged);
            }
            base.OnOwnerChanged(e);
        }
        #endregion

        #region MethodsPrivate
        private static Control CreateControlInstance()
        {
            BSE.Windows.Forms.ProgressBar progressBar = new BSE.Windows.Forms.ProgressBar();
            progressBar.Size = new Size(100, 15);

            return progressBar;
        }
        private void OwnerRendererChanged(object sender, EventArgs e)
        {
            ToolStripRenderer toolsTripRenderer = this.Owner.Renderer;
            if (toolsTripRenderer != null)
            {
                if (toolsTripRenderer is BseRenderer)
                {
                    ToolStripProfessionalRenderer renderer = toolsTripRenderer as ToolStripProfessionalRenderer;
                        if (renderer != null)
                        {
                            this.ProgressBar.BorderColor = renderer.ColorTable.ToolStripBorder;
                        }
                    if (this.Owner.GetType() != typeof(StatusStrip))
                    {
                        this.Margin = new Padding(1, 1, 1, 3);
                    }
                }
                else
                {
                    this.Margin = DefaultMargin;
                }
            }
        }
        #endregion

    }
}
