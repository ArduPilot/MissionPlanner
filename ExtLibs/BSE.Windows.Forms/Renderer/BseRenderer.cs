using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Diagnostics;

namespace BSE.Windows.Forms
{
    /// <summary>
    /// Draw ToolStrips using the Office 2007 themed appearance.
    /// </summary>
    public class BseRenderer : ToolStripProfessionalRenderer
    {
        #region FieldsPrivate
        private static Rectangle[] baseSizeGripRectangles;
        private static int MarginInset;
        private static Blend MenuItemBlend;
        private static Blend ButtonBlend;
        #endregion

		#region MethodsPublic
		static BseRenderer()
        {
            MarginInset = 2;
            
            // One time creation of the blend for the button gradient brush
            ButtonBlend = new Blend();
            ButtonBlend.Positions = new float[] { 0.0F, 0.1F, 0.2F, 0.5F, 1.0F };
            ButtonBlend.Factors = new float[] { 0.6F, 0.7F, 0.8F, 1.0F, 1.0F };
            // One time creation of the blend for the menuitem gradient brush
            MenuItemBlend = new Blend();
            MenuItemBlend.Positions = new float[] { 0.0F, 0.1F, 0.2F, 0.5F, 1.0F };
            MenuItemBlend.Factors = new float[] { 0.7F, 0.8F, 0.9F, 1.0F, 1.0F };

            baseSizeGripRectangles = new Rectangle[] { new Rectangle(8, 0, 2, 2), new Rectangle(8, 4, 2, 2), new Rectangle(8, 8, 2, 2), new Rectangle(4, 4, 2, 2), new Rectangle(4, 8, 2, 2), new Rectangle(0, 8, 2, 2) };
        }
        /// <summary>
        /// Initialize a new instance of the BseRenderer class.
        /// </summary>
        public BseRenderer()
            : base(new BSE.Windows.Forms.ColorTableBlack())
        {
			this.ColorTable.UseSystemColors = false;
		}
        /// <summary>
        /// Initializes a new instance of the BseRenderer class.
        /// </summary>
        /// <param name="professionalColorTable">A <see cref="BSE.Windows.Forms.ProfessionalColorTable"/> to be used for painting.</param>
        public BseRenderer(ProfessionalColorTable professionalColorTable)
            : base(professionalColorTable)
        {
        }
        #endregion

		#region MethodsProtected
        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.ToolStripRenderer.RenderArrow"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.Windows.Forms.ToolStripArrowRenderEventArgs"/> that contains the event data.</param>
        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            if (ColorTable.UseSystemColors == true)
            {
                base.OnRenderArrow(e);
            }
            else
            {
                ProfessionalColorTable colorTable = ColorTable as BSE.Windows.Forms.ProfessionalColorTable;
                if ((colorTable != null) && (e.Item.Enabled == true))
                {
                    if (e.Item.Owner is MenuStrip)
                    {
                        e.ArrowColor = colorTable.MenuItemText;
                    }
                    else if (e.Item.Owner is StatusStrip)
                    {
                        e.ArrowColor = colorTable.StatusStripText;
                    }
                    else
                    {
                        if (e.Item.Owner.GetType() != typeof(ToolStripDropDownMenu))
                        {
                            e.ArrowColor = colorTable.ToolStripText;
                        }
                    }
                }
                base.OnRenderArrow(e);
            }
        }
        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.ToolStripRenderer.RenderButtonBackground"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.Windows.Forms.ToolStripItemRenderEventArgs"/> that contains the event data.</param>
        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (ColorTable.UseSystemColors == true)
            {
                base.OnRenderDropDownButtonBackground(e);
            }
            else
            {
                ToolStripButton item = e.Item as ToolStripButton;
                Rectangle buttonBounds = new Rectangle(Point.Empty, item.Size);
                if (IsZeroWidthOrHeight(buttonBounds) == true)
                {
                    return;
                }
                Graphics graphics = e.Graphics;
                ProfessionalColorTable colorTable = ColorTable as ProfessionalColorTable;
                if (colorTable != null)
                {
                    using (UseAntiAlias antiAlias = new UseAntiAlias(graphics))
                    {
                        Rectangle buttonRectangle = GetButtonRectangle(buttonBounds);

                        if (item.Checked == true)
                        {
                            //Draws the border of the button for the checked ToolStripButton control
                            DrawButtonBorder(graphics, buttonRectangle, colorTable.ButtonPressedBorder);
                        }
                        if ((item.Selected == true) && (item.Pressed == false))
                        {
                            //Renders the upper button part of the selected ToolStripButton control
                            RenderButton(graphics, buttonRectangle, colorTable.MenuItemTopLevelSelectedGradientBegin);
                            //Draws the border of the button for the selected ToolStripButton control
                            DrawButtonBorder(graphics, buttonRectangle, colorTable.ButtonSelectedHighlightBorder);
                            //DrawButtonBorder(graphics, buttonRectangle, Color.FromArgb(196, 194, 196));
                        }
                        if (item.Pressed == true)
                        {
                            //Renders the upper button part of the pressed ToolStripButton control
                            RenderButton(graphics, buttonRectangle, colorTable.MenuItemPressedGradientBegin);
                            //Draws the inner border of the button for the pressed ToolStripButton control
                            DrawInnerButtonBorder(graphics, buttonRectangle, colorTable.ButtonSelectedHighlightBorder);
                            //Draws the outer border of the button for the pressed ToolStripButton control
                            DrawButtonBorder(graphics, buttonRectangle, colorTable.MenuBorder);
                        }
                    }
                }
                else
                {
                    base.OnRenderDropDownButtonBackground(e);
                }
            }
        }
        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.ToolStripRenderer.RenderDropDownButtonBackground"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.Windows.Forms.ToolStripItemRenderEventArgs"/> that contains the event data.</param>
        protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (ColorTable.UseSystemColors == true)
            {
                base.OnRenderDropDownButtonBackground(e);
            }
            else
            {
                ToolStripDropDownButton item = e.Item as ToolStripDropDownButton;
                Rectangle buttonBounds = new Rectangle(Point.Empty, item.Size);
                if (IsZeroWidthOrHeight(buttonBounds) == true)
                {
                    return;
                }
                Graphics graphics = e.Graphics;
                ProfessionalColorTable colorTable = ColorTable as ProfessionalColorTable;

                if (colorTable != null)
                {
                    using (UseAntiAlias antiAlias = new UseAntiAlias(graphics))
                    {
                        Rectangle buttonRectangle = GetButtonRectangle(buttonBounds);
                        if ((item.Selected == true) && (item.Pressed == false))
                        {
                            //Renders the upper button part of the selected ToolStripDropDownButton control
                            RenderButton(graphics, buttonRectangle, colorTable.MenuItemTopLevelSelectedGradientBegin);
                            //Draws the border of the button for the selected ToolStripDropDownButton control
                            DrawButtonBorder(graphics, buttonRectangle, colorTable.ButtonSelectedHighlightBorder);
                        }
                        if (item.Pressed == true)
                        {
                            //Renders the upper button part of the pressed ToolStripDropDownButton control
                            RenderButton(graphics, buttonRectangle, colorTable.MenuItemPressedGradientBegin);
                            //Draws the inner border of the button for the pressed ToolStripDropDownButton control
                            DrawInnerButtonBorder(graphics, buttonRectangle, colorTable.ButtonSelectedHighlightBorder);
                            //Draws the outer border of the button for the pressed ToolStripDropDownButton control
                            DrawButtonBorder(graphics, buttonRectangle, colorTable.MenuBorder);
                        }
                    }
                }
                else
                {
                    base.OnRenderDropDownButtonBackground(e);
                }
            }
        }
        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.ToolStripRenderer.OnRenderSplitButtonBackground"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.Windows.Forms.ToolStripItemRenderEventArgs"/> that contains the event data.</param>
        protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (ColorTable.UseSystemColors == true)
            {
                base.OnRenderDropDownButtonBackground(e);
            }
            else
            {
                ToolStripSplitButton item = e.Item as ToolStripSplitButton;
                Rectangle buttonBounds = new Rectangle(Point.Empty, item.ButtonBounds.Size);
                if (IsZeroWidthOrHeight(buttonBounds) == true)
                {
                    return;
                }
                Graphics graphics = e.Graphics;
                ProfessionalColorTable colorTable = ColorTable as ProfessionalColorTable;
                if (colorTable != null)
                {
                    using (UseAntiAlias antiAlias = new UseAntiAlias(graphics))
                    {
                        Rectangle buttonRectangle = GetButtonRectangle(buttonBounds);
                        Rectangle dropDownButtonBounds = new Rectangle(item.DropDownButtonBounds.Location, item.DropDownButtonBounds.Size);
                        Rectangle dropDownButtonRectangle = GetButtonRectangle(dropDownButtonBounds);

                        if ((item.Selected == true) && (item.Pressed == false) && (item.ButtonPressed == false))
                        {
                            //Renders the upper button part of the selected ToolStripSplitButton control
                            RenderButton(graphics, buttonRectangle, colorTable.MenuItemTopLevelSelectedGradientBegin);
                            //Renders the dropDownButton part of the selected ToolStripSplitButton control
                            RenderButton(graphics, dropDownButtonRectangle, colorTable.MenuItemTopLevelSelectedGradientBegin);
                            //Draws the border of the button part for the selected ToolStripSplitButton control
                            DrawButtonBorder(graphics, buttonRectangle, colorTable.ButtonSelectedHighlightBorder);
                            //Draws the border of the dropDownButton part for the selected ToolStripSplitButton control
                            DrawButtonBorder(graphics, dropDownButtonRectangle, colorTable.ButtonSelectedHighlightBorder);
                        }
                        if (item.ButtonPressed == true)
                        {
                            //Renders the upper button part of the pressed ToolStripSplitButton control
                            RenderButton(graphics, buttonRectangle, colorTable.MenuItemPressedGradientBegin);
                            //Renders the dropDownButton part of the pressed ToolStripSplitButton control
                            RenderButton(graphics, dropDownButtonRectangle, colorTable.MenuItemPressedGradientBegin);
                            //Draws the inner border of the button part for the pressed ToolStripSplitButton control
                            DrawInnerButtonBorder(graphics, buttonRectangle, colorTable.ButtonSelectedHighlightBorder);
                            //Draws the outer border of the button part for the pressed ToolStripSplitButton control
                            DrawButtonBorder(graphics, buttonRectangle, colorTable.MenuBorder);
                            //Draws the inner border of the dropDownButton part for the pressed ToolStripSplitButton control
                            DrawInnerButtonBorder(graphics, dropDownButtonRectangle, colorTable.ButtonSelectedHighlightBorder);
                            //Draws the outer border of the dropDownButton part for the pressed ToolStripSplitButton control
                            DrawButtonBorder(graphics, dropDownButtonRectangle, colorTable.MenuBorder);
                        }
                        if (item.DropDownButtonPressed == true)
                        {
                            //Renders the upper button part of the pressed ToolStripSplitButton control
                            RenderButton(graphics, buttonRectangle, colorTable.MenuItemTopLevelSelectedGradientBegin);
                            //Renders the dropDownButton part of the pressed ToolStripSplitButton control
                            RenderButton(graphics, dropDownButtonRectangle, colorTable.MenuItemTopLevelSelectedGradientBegin);
                            //Draws the border of the button part for the pressed ToolStripSplitButton control
                            DrawButtonBorder(graphics, buttonRectangle, ColorTable.ButtonSelectedHighlightBorder);
                            //Draws the border of the dropDownButton part for the pressed ToolStripSplitButton control
                            DrawButtonBorder(graphics, dropDownButtonRectangle, ColorTable.ButtonSelectedHighlightBorder);
                        }
                        if (e.Item.Owner is MenuStrip)
                        {
                            base.DrawArrow(new ToolStripArrowRenderEventArgs(graphics, item, dropDownButtonBounds, colorTable.MenuItemText, ArrowDirection.Down));
                        }
                        if (e.Item.Owner is StatusStrip)
                        {
                            base.DrawArrow(new ToolStripArrowRenderEventArgs(graphics, item, dropDownButtonBounds, colorTable.StatusStripText, ArrowDirection.Down));
                        }
                        if (e.Item.Owner is ToolStrip)
                        {
                            base.DrawArrow(new ToolStripArrowRenderEventArgs(graphics, item, dropDownButtonBounds, colorTable.ToolStripText, ArrowDirection.Down));
                        }
                    }
                }
                else
                {
                    base.OnRenderDropDownButtonBackground(e);
                }
            }
        }
        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.ToolStripRenderer.RenderMenuItemBackground"/> event. 
        /// </summary>
        /// <param name="e">A <see cref="System.Windows.Forms.ToolStripItemRenderEventArgs"/> that contains the event data.</param>
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            ToolStripMenuItem item = e.Item as ToolStripMenuItem;
            Rectangle bounds = new Rectangle(Point.Empty, item.Size);
            if (IsZeroWidthOrHeight(bounds) == true)
            {
                return;
            }
            Graphics graphics = e.Graphics;
            ProfessionalColorTable colorTable = ColorTable as BSE.Windows.Forms.ProfessionalColorTable;
            if (colorTable != null)
            {
                using (UseAntiAlias useAntiAlias = new UseAntiAlias(graphics))
                {
                    if (e.ToolStrip is MenuStrip)
                    {
                        if ((item.Selected == true) && (item.Pressed == false))
                        {
                            RenderMenuItem(graphics, bounds, colorTable.MenuItemTopLevelSelectedGradientBegin);
                            ControlPaint.DrawBorder(e.Graphics, bounds, colorTable.MenuItemTopLevelSelectedBorder, ButtonBorderStyle.Solid);
                        }
                        if (item.Pressed == true)
                        {
                            RenderButton(graphics, bounds, ColorTable.MenuItemPressedGradientBegin);
                            Rectangle innerBorderRectangle = bounds;
                            innerBorderRectangle.Inflate(-1, -1);
                            ControlPaint.DrawBorder(e.Graphics, innerBorderRectangle, ColorTable.ButtonSelectedHighlightBorder, ButtonBorderStyle.Solid);
                            ControlPaint.DrawBorder(e.Graphics, bounds, ColorTable.MenuBorder, ButtonBorderStyle.Solid);
                        }
                    }
                    else
                    {
                        base.OnRenderMenuItemBackground(e);
                    }
                }
            }
            else
            {
                base.OnRenderMenuItemBackground(e);
            }
        }
        /// <summary>
        /// Raises the RenderItemText event.
        /// </summary>
        /// <param name="e">A ToolStripItemTextRenderEventArgs that contains the event data.</param>
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
			if (ColorTable.UseSystemColors == false)
			{
				ProfessionalColorTable colorTable = ColorTable as BSE.Windows.Forms.ProfessionalColorTable;
				if (colorTable != null)
				{
                    if ((e.ToolStrip is MenuStrip))// && (e.Item.Selected == false) && e.Item.Pressed == false)
                    {
                        if (colorTable.MenuItemText != Color.Empty)
                        {
                            e.TextColor = colorTable.MenuItemText;
                        }
                    }
                    else if ((e.ToolStrip is StatusStrip))// && (e.Item.Selected == false) && e.Item.Pressed == false)
                    {
                        if (colorTable.StatusStripText != Color.Empty)
                        {
                            e.TextColor = colorTable.StatusStripText;
                        }
                    }
                    else if (e.ToolStrip is ToolStripDropDown)
                    {
                        //base.OnRenderItemText(e);
                    }
                    else
                    {
                        if (colorTable.ToolStripText != Color.Empty)
                        {
                            e.TextColor = colorTable.ToolStripText;
                        }
                    }
				}
			}
            base.OnRenderItemText(e);
        }
        /// <summary>
        /// Raises the RenderToolStripContentPanelBackground event. 
        /// </summary>
        /// <param name="e">An ToolStripContentPanelRenderEventArgs containing the event data.</param>
        protected override void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e)
        {
            // Must call base class, otherwise the subsequent drawing does not appear!
            base.OnRenderToolStripContentPanelBackground(e);
			if (ColorTable.UseSystemColors == false)
			{
				// Cannot paint a zero sized area
				if ((e.ToolStripContentPanel.Width > 0) &&
					(e.ToolStripContentPanel.Height > 0))
				{
					using (LinearGradientBrush backBrush = new LinearGradientBrush(e.ToolStripContentPanel.ClientRectangle,
																				   ColorTable.ToolStripContentPanelGradientBegin,
																				   ColorTable.ToolStripContentPanelGradientEnd,
																				   LinearGradientMode.Vertical))
					{
						e.Graphics.FillRectangle(backBrush, e.ToolStripContentPanel.ClientRectangle);
					}
				}
			}
        }
        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.ToolStripRenderer.RenderOverflowButtonBackground"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.Windows.Forms.ToolStripItemRenderEventArgs"/> that contains the event data.</param>
        protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderOverflowButtonBackground(e);
            ToolStripItem item = e.Item;
            if ((item.Selected == false) && (item.Pressed == false))
            {
                ProfessionalColorTable colorTable = ColorTable as ProfessionalColorTable;
                if (colorTable != null)
                {
                    Graphics graphics = e.Graphics;
                    bool bRightToLeft = item.RightToLeft == RightToLeft.Yes;

                    bool bOrientation = e.ToolStrip.Orientation == Orientation.Horizontal;
                    Rectangle arrowRectangle = Rectangle.Empty;
                    if (bRightToLeft)
                    {
                        arrowRectangle = new Rectangle(0, item.Height - 8, 9, 5);
                    }
                    else
                    {
                        arrowRectangle = new Rectangle(item.Width - 12, item.Height - 8, 9, 5);
                    }

                    ArrowDirection arrowDirection = bOrientation ? ArrowDirection.Down : ArrowDirection.Right;
                    int x = (bRightToLeft && bOrientation) ? -1 : 1;
                    arrowRectangle.Offset(x, 1);
                    RenderArrowInternal(graphics, arrowRectangle, arrowDirection, colorTable.ToolStripGradientMiddle);
                    arrowRectangle.Offset(-1 * x, -1);
                    RenderArrowInternal(graphics, arrowRectangle, arrowDirection, colorTable.ToolStripText);
                    if (bOrientation)
                    {
                        x = bRightToLeft ? -2 : 0;
                        RenderOverflowButtonLine(graphics, colorTable.ToolStripText, (int)(arrowRectangle.Right - 6), (int)(arrowRectangle.Y - 2), (int)(arrowRectangle.Right - 2), (int)(arrowRectangle.Y - 2));
                        RenderOverflowButtonLine(graphics, colorTable.ToolStripGradientMiddle, (int)((arrowRectangle.Right - 5) + x), (int)(arrowRectangle.Y - 1), (int)((arrowRectangle.Right - 1) + x), (int)(arrowRectangle.Y - 1));
                    }
                    else
                    {
                        RenderOverflowButtonLine(graphics, colorTable.ToolStripText, arrowRectangle.X, arrowRectangle.Y, arrowRectangle.X, arrowRectangle.Bottom - 1);
                        RenderOverflowButtonLine(graphics, colorTable.ToolStripGradientMiddle, arrowRectangle.X + 1, arrowRectangle.Y + 1, arrowRectangle.X + 1, arrowRectangle.Bottom);
                    }
                }
            }
        }
        /// <summary>
        /// Raises the RenderSeparator event. 
        /// </summary>
        /// <param name="e">An ToolStripSeparatorRenderEventArgs containing the event data.</param>
        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
			if (ColorTable.UseSystemColors == false)
			{
				e.Item.ForeColor = ColorTable.RaftingContainerGradientBegin;
			}
            base.OnRenderSeparator(e);
        }
        /// <summary>
        /// Raises the RenderStatusStripSizingGrip event.
        /// </summary>
        /// <param name="e">A ToolStripRenderEventArgs that contains the event data.</param>
        protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
        {
            Graphics graphics = e.Graphics;
            StatusStrip toolStrip = e.ToolStrip as StatusStrip;
            if (toolStrip != null)
            {
                Rectangle sizeGripBounds = toolStrip.SizeGripBounds;
                if (IsZeroWidthOrHeight(sizeGripBounds) == false)
                {
                    Rectangle[] rectanglesLight = new Rectangle[baseSizeGripRectangles.Length];
                    Rectangle[] rectanglesDark = new Rectangle[baseSizeGripRectangles.Length];
                    for (int i = 0; i < baseSizeGripRectangles.Length; i++)
                    {
                        Rectangle rectangleDark = baseSizeGripRectangles[i];
                        if (toolStrip.RightToLeft == RightToLeft.Yes)
                        {
                            rectangleDark.X = (sizeGripBounds.Width - rectangleDark.X) - rectangleDark.Width;
                        }
                        rectangleDark.Offset(sizeGripBounds.X, sizeGripBounds.Bottom - 12);
                        rectanglesLight[i] = rectangleDark;
                        if (toolStrip.RightToLeft == RightToLeft.Yes)
                        {
                            rectangleDark.Offset(1, -1);
                        }
                        else
                        {
                            rectangleDark.Offset(-1, -1);
                        }
                        rectanglesDark[i] = rectangleDark;
                    }
                    using (SolidBrush darkBrush = new SolidBrush(ColorTable.GripDark),
                        lightBrush = new SolidBrush(ColorTable.GripDark))
                    {
                        graphics.FillRectangles(lightBrush, rectanglesLight);
                        graphics.FillRectangles(darkBrush, rectanglesDark);
                    }
                }
            }
        }
        /// <summary>
        /// Raises the RenderToolStripBackground event. 
        /// </summary>
        /// <param name="e">An ToolStripRenderEventArgs containing the event data.</param>
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            if (ColorTable.UseSystemColors == true)
            {
                base.OnRenderToolStripBackground(e);
            }
            else
            {
                Trace.WriteLine("ToolStrip: " + e.ToolStrip.GetType());
                Rectangle backgroundRectangle = new Rectangle(0, 0, e.ToolStrip.Width, e.ToolStrip.Height);
                Rectangle innerRectangle = backgroundRectangle;
                innerRectangle.Height = (backgroundRectangle.Height / 2) + 1;
                // Cannot paint a zero sized area
                if ((backgroundRectangle.Width > 0) && (backgroundRectangle.Height > 0))
                {
                    if (e.ToolStrip is StatusStrip)
                    {
                        using (SolidBrush outerBrush = new SolidBrush(ColorTable.StatusStripGradientEnd))
                        {
                            e.Graphics.FillRectangle(outerBrush, backgroundRectangle);
                        }

                        int y2 = backgroundRectangle.Height / 2;
                        Rectangle upperRectangle = new Rectangle(backgroundRectangle.X, backgroundRectangle.Y, backgroundRectangle.Width, y2);
                        upperRectangle.Height += 1;
                        using (LinearGradientBrush innerRectangleBrush = new LinearGradientBrush(
                            upperRectangle,
                            ColorTable.StatusStripGradientBegin,
                            Color.FromArgb(128,ColorTable.StatusStripGradientBegin),
                            LinearGradientMode.Vertical))
                        {
                            e.Graphics.FillRectangle(innerRectangleBrush, upperRectangle); //draw top bubble
                        }

                        y2 = (backgroundRectangle.Height / 4) + 1;
                        Rectangle lowerRectangle = new Rectangle(backgroundRectangle.X, backgroundRectangle.Height - y2, backgroundRectangle.Width, y2);

                        using (LinearGradientBrush innerRectangleBrush = new LinearGradientBrush(
                            lowerRectangle,
                            ColorTable.StatusStripGradientEnd,
                            Color.FromArgb(128,ColorTable.StatusStripGradientBegin),
                            LinearGradientMode.Vertical))
                        {
                            e.Graphics.FillRectangle(innerRectangleBrush, lowerRectangle); //draw top bubble
                        }
                    }
                    else if (e.ToolStrip is MenuStrip)
                    {
                        using (SolidBrush outerBrush = new SolidBrush(ColorTable.MenuStripGradientEnd))
                        {
                            e.Graphics.FillRectangle(outerBrush, backgroundRectangle);
                        }

                        int y2 = backgroundRectangle.Height / 3;
                        Rectangle lowerRectangle = new Rectangle(backgroundRectangle.X, backgroundRectangle.Y, backgroundRectangle.Width, y2);

                        using (LinearGradientBrush innerRectangleBrush = new LinearGradientBrush(
                            lowerRectangle,
                            ColorTable.MenuStripGradientBegin,
                            Color.FromArgb(128, ColorTable.StatusStripGradientBegin),
                            LinearGradientMode.Vertical))
                        {
                            e.Graphics.FillRectangle(innerRectangleBrush, lowerRectangle); //draw top bubble
                        }
                    }
                    else if (e.ToolStrip is ToolStripDropDown)
                    {
                        base.OnRenderToolStripBackground(e);
                    }
                    else
                    {
                        using (SolidBrush outerBrush = new SolidBrush(ColorTable.ToolStripGradientEnd))
                        {
                            e.Graphics.FillRectangle(outerBrush, backgroundRectangle);
                        }

                        int y2 = backgroundRectangle.Height / 2;
                        Rectangle upperRectangle = new Rectangle(backgroundRectangle.X, backgroundRectangle.Y, backgroundRectangle.Width, y2);

                        using (LinearGradientBrush innerRectangleBrush = new LinearGradientBrush(
                            upperRectangle,
                            ColorTable.ToolStripGradientBegin,
                            ColorTable.ToolStripGradientMiddle,
                            LinearGradientMode.Vertical))
                        {
                            e.Graphics.FillRectangle(innerRectangleBrush, upperRectangle); //draw top bubble
                        }

                        y2 = backgroundRectangle.Height / 4; 
                        Rectangle lowerRectangle = new Rectangle(backgroundRectangle.X, backgroundRectangle.Height - y2, backgroundRectangle.Width, y2);

                        using (LinearGradientBrush innerRectangleBrush = new LinearGradientBrush(
                            lowerRectangle,
                            ColorTable.ToolStripGradientEnd,
                            ColorTable.ToolStripGradientMiddle,
                            LinearGradientMode.Vertical))
                        {
                            e.Graphics.FillRectangle(innerRectangleBrush, lowerRectangle); //draw top bubble
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Raises the RenderImageMargin event. 
        /// </summary>
        /// <param name="e">An ToolStripRenderEventArgs containing the event data.</param>
        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
			if (ColorTable.UseSystemColors == true)
			{
				base.OnRenderToolStripBackground(e);
			}
			else
			{
				if ((e.ToolStrip is ContextMenuStrip) ||
					(e.ToolStrip is ToolStripDropDownMenu))
				{
					// Start with the total margin area
					Rectangle marginRectangle = e.AffectedBounds;

					// Do we need to draw with separator on the opposite edge?
					bool bIsRightToLeft = (e.ToolStrip.RightToLeft == RightToLeft.Yes);

					marginRectangle.Y += MarginInset;
					marginRectangle.Height -= MarginInset * 2;

					// Reduce so it is inside the border
					if (bIsRightToLeft == false)
					{
						marginRectangle.X += MarginInset;
					}
					else
					{
						marginRectangle.X += MarginInset / 2;
					}

					// Draw the entire margine area in a solid color
					using (SolidBrush backBrush = new SolidBrush(
						ColorTable.ImageMarginGradientBegin))
						e.Graphics.FillRectangle(backBrush, marginRectangle);
				}
				else
				{
					base.OnRenderImageMargin(e);
				}
			}
        }

		#endregion

		#region MethodsPrivate
        private static GraphicsPath GetBackgroundPath(Rectangle bounds, int radius)
        {
            int x = bounds.X;
            int y = bounds.Y;
            int width = bounds.Width;
            int height = bounds.Height;
            GraphicsPath graphicsPath = new GraphicsPath();
            graphicsPath.AddArc(x, y, radius, radius, 180, 90);				                    //Upper left corner
            graphicsPath.AddArc(x + width - radius, y, radius, radius, 270, 90);			    //Upper right corner
            graphicsPath.AddArc(x + width - radius, y + height - radius, radius, radius, 0, 90);//Lower right corner
            graphicsPath.AddArc(x, y + height - radius, radius, radius, 90, 90);			    //Lower left corner
            graphicsPath.CloseFigure();
            return graphicsPath;
        }

        private static void RenderButton(Graphics graphics, Rectangle buttonBounds, Color gradientColor)
        {
            using (GraphicsPath graphicsPath = GetBackgroundPath(buttonBounds, 3))
            {
                using (LinearGradientBrush backBrush = new LinearGradientBrush(buttonBounds,
                    gradientColor,
                    Color.Transparent,
                    LinearGradientMode.Vertical))
                {
                    backBrush.Blend = ButtonBlend;
                    graphics.FillPath(backBrush, graphicsPath);
                }
            }
        }

        private static void RenderMenuItem(Graphics graphics, Rectangle menuBounds, Color gradientColor)
        {
            using (LinearGradientBrush backBrush = new LinearGradientBrush(
                menuBounds,
                gradientColor,
                Color.Transparent,
                LinearGradientMode.Vertical))
            {
                backBrush.Blend = MenuItemBlend;
                graphics.FillRectangle(backBrush, menuBounds);
            }
        }

        private static void DrawButtonBorder(Graphics graphics, Rectangle buttonBounds, Color borderColor)
        {
            using (GraphicsPath itemPath = GetBackgroundPath(buttonBounds, 3))
            {
                using (Pen itemPen = new Pen(borderColor))
                {
                    graphics.DrawPath(itemPen, itemPath);
                }
            }
        }

        private static void DrawInnerButtonBorder(Graphics graphics, Rectangle buttonBounds, Color innerBorderColor)
        {
            Rectangle innerButtonRectangle = buttonBounds;
            innerButtonRectangle.Height -= 1;
            innerButtonRectangle.Width -= 1;
            using (GraphicsPath innerBorderPath = GetBackgroundPath(innerButtonRectangle, 3))
            {
                using (Pen itemPen = new Pen(innerBorderColor))
                {
                    graphics.DrawPath(itemPen, innerBorderPath);
                }
            }
        }

        private static Rectangle GetButtonRectangle(Rectangle bounds)
        {
            Rectangle buttonRectangle = bounds;
            buttonRectangle.Width -= 1;
            buttonRectangle.Height -= 1;
            buttonRectangle.Inflate(0, -1);
            return buttonRectangle;
        }
        /// <summary>
        /// Renders the arrows in the OverflowButton.
        /// </summary>
        /// <param name="graphics">The Graphics to draw on.</param>
        /// <param name="dropDownRectangle">The rectangle in which the arrows should drawn.</param>
        /// <param name="direction">the direction of the arrows.</param>
        /// <param name="color">The color used to fill the arrow polygons</param>
        private static void RenderArrowInternal(Graphics graphics, Rectangle dropDownRectangle, ArrowDirection direction, Color color)
        {
            Point point = new Point(dropDownRectangle.Left + (dropDownRectangle.Width / 2), dropDownRectangle.Top + (dropDownRectangle.Height / 2));
            point.X += dropDownRectangle.Width % 2;
            Point[] points = null;
            switch (direction)
            {
                case ArrowDirection.Left:
                    points = new Point[] { new Point(point.X + 2, point.Y - 3), new Point(point.X + 2, point.Y + 3), new Point(point.X - 1, point.Y) };
                    break;

                case ArrowDirection.Up:
                    points = new Point[] { new Point(point.X - 2, point.Y + 1), new Point(point.X + 3, point.Y + 1), new Point(point.X, point.Y - 2) };
                    break;

                case ArrowDirection.Right:
                    points = new Point[] { new Point(point.X - 2, point.Y - 3), new Point(point.X - 2, point.Y + 3), new Point(point.X + 1, point.Y) };
                    break;

                default:
                    points = new Point[] { new Point(point.X - 2, point.Y - 1), new Point(point.X + 3, point.Y - 1), new Point(point.X, point.Y + 2) };
                    break;
            }
            using (SolidBrush backBrush = new SolidBrush(color))
            {
                graphics.FillPolygon(backBrush, points);
            }
        }
        /// <summary>
        /// Renders the lines in the OverflowButton.
        /// </summary>
        /// <param name="graphics">The Graphics to draw on.</param>
        /// <param name="color">The color used to fill the line</param>
        /// <param name="x1">The x-coordinate of the first point.</param>
        /// <param name="y1">The y-coordinate of the first point.</param>
        /// <param name="x2">The x-coordinate of the second point.</param>
        /// <param name="y2">The y-coordinate of the second point.</param>
        private static void RenderOverflowButtonLine(Graphics graphics,Color color, int x1, int y1, int x2, int y2)
        {
            using (Pen pen = new Pen(color))
            {
                graphics.DrawLine(pen, x1, y1, x2, y2);
            }
        }
        /// <summary>
        /// Checks if the rectangle width or height is equal to 0.
        /// </summary>
        /// <param name="rectangle">the rectangle to check</param>
        /// <returns>true if the with or height of the rectangle is 0 else false</returns>
        private static bool IsZeroWidthOrHeight(Rectangle rectangle)
        {
            if (rectangle.Width != 0)
            {
                return (rectangle.Height == 0);
            }
            return true;
        }
        #endregion
    }
}
