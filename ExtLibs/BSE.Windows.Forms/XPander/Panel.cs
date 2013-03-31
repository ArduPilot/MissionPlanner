using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using BSE.Windows.Forms.Properties;
using System.Diagnostics;

namespace BSE.Windows.Forms
{
   #region Class Panel
   /// <summary>
   /// Used to group collections of controls. 
   /// </summary>
   /// <remarks>The Panel is a control that contains other controls.
   /// You can use a Panel to group collections of controls such as the XPanderPanelList control.
   /// On top of the Panel there is the captionbar. This captionbar may contain an image and text.
   /// According to it's dockstyle and properties the panel is collapsable and/or closable.</remarks>
   /// <copyright>Copyright © 2006-2008 Uwe Eichkorn
   /// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
   /// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
   /// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
   /// PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
   /// REMAINS UNCHANGED.
   /// </copyright>
   [Designer(typeof(PanelDesigner)),
   DesignTimeVisibleAttribute(true)]
   [DefaultEvent("CloseClick")]
   [ToolboxBitmap(typeof(System.Windows.Forms.Panel))]
   public partial class Panel : BasePanel
   {
      #region FieldsPrivate

      private Rectangle m_restoreBounds;
      private bool m_bShowTransparentBackground;
      private bool m_bShowXPanderPanelProfessionalStyle;
      private bool m_bShowCaptionbar;
      private LinearGradientMode m_linearGradientMode;
      private Image m_imageClosePanel;
      private CustomPanelColors m_customColors;
      private Image m_imgHoverBackground;
      private System.Windows.Forms.Splitter m_associatedSplitter;

      #endregion

      #region Properties
      /// <summary>
      /// Gets or sets the associated Splitter. If there is a splitter associated to a panel,
      /// the splitter visibility always changes when the visibilty of this panel changes.
      /// </summary>
      /// <value>The associated <see cref="Splitter"/></value>
      [Description("The associated Splitter.")]
      [Category("Behavior")]
      public virtual System.Windows.Forms.Splitter AssociatedSplitter
      {
         get
         {
            return this.m_associatedSplitter;
         }
         set
         {
            this.m_associatedSplitter = value;
         }
      }
      /// <summary>
      /// Gets the custom colors which are used for the panel.
      /// </summary>
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
      [Description("The custom colors which are used for the panel.")]
      [Category("Appearance")]
      public CustomPanelColors CustomColors
      {
         get
         {
            return this.m_customColors;
         }
      }
      /// <summary>
      /// Expands the panel.
      /// </summary>
      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public override bool Expand
      {
         get
         {
            return base.Expand;
         }
         set
         {
            base.Expand = value;
         }
      }
      /// <summary>
      /// LinearGradientMode of the panels background
      /// </summary>
      [Description("LinearGradientMode of the Panels background"),
      DefaultValue(1),
      Category("Appearance")]
      public LinearGradientMode LinearGradientMode
      {
         get
         {
            return this.m_linearGradientMode;
         }
         set
         {
            if(value.Equals(this.m_linearGradientMode) == false)
            {
               this.m_linearGradientMode = value;
               this.Invalidate(false);
            }
         }
      }
      /// <summary>
      /// Gets or sets a value indicating whether the panels captionbar is displayed.
      /// </summary>
      /// <example>
      /// <code>
      /// private void btnShowHideCaptionbar_Click(object sender, EventArgs e)
      /// {
      ///     //displaye or hides the captionbar at the top of the panel
      ///     panel6.ShowCaptionbar = !panel6.ShowCaptionbar;
      /// }
      /// </code>
      /// </example>
      [Description("A value indicating whether the panels captionbar is displayed.")]
      [DefaultValue(true)]
      [Category("Behavior")]
      public bool ShowCaptionbar
      {
         get
         {
            return this.m_bShowCaptionbar;
         }
         set
         {
            if(value.Equals(this.m_bShowCaptionbar) == false)
            {
               this.m_bShowCaptionbar = value;
               this.Invalidate(true);
            }
         }
      }
      /// <summary>
      /// Gets or sets a value indicating whether the controls background is transparent.
      /// </summary>
      [Description("Gets or sets a value indicating whether the controls background is transparent")]
      [DefaultValue(true)]
      [Category("Behavior")]
      public bool ShowTransparentBackground
      {
         get
         {
            return this.m_bShowTransparentBackground;
         }
         set
         {
            if(value.Equals(this.m_bShowTransparentBackground) == false)
            {
               this.m_bShowTransparentBackground = value;
               this.Invalidate(false);
            }
         }
      }
      /// <summary>
      /// Gets or sets a value indicating whether the controls caption professional colorscheme is the same then the XPanderPanels
      /// </summary>
      [Description("Gets or sets a value indicating whether the controls caption professional colorscheme is the same then the XPanderPanels")]
      [DefaultValue(false)]
      [Category("Behavior")]
      public bool ShowXPanderPanelProfessionalStyle
      {
         get
         {
            return this.m_bShowXPanderPanelProfessionalStyle;
         }
         set
         {
            if(value.Equals(this.m_bShowXPanderPanelProfessionalStyle) == false)
            {
               this.m_bShowXPanderPanelProfessionalStyle = value;
               this.Invalidate(false);
            }
         }
      }
      /// <summary>
      /// Gets the size and location of the panel in it's normal expanded state.
      /// </summary>
      /// <remarks>
      /// A Rect that specifies the size and location of a panel before being either collapsed
      /// </remarks>
      [Browsable(false)]
      public Rectangle RestoreBounds
      {
         get
         {
            return this.m_restoreBounds;
         }
      }
      #endregion

      #region MethodsPublic
      /// <summary>
      /// Initializes a new instance of the Panel class.
      /// </summary>
      public Panel()
      {
         InitializeComponent();

         this.CaptionFont = new Font(SystemFonts.CaptionFont.FontFamily, SystemFonts.CaptionFont.SizeInPoints + 2.75F, FontStyle.Bold);
         this.BackColor = Color.Transparent;
         this.ForeColor = SystemColors.ControlText;
         this.ShowTransparentBackground = true;
         this.ShowXPanderPanelProfessionalStyle = false;
         this.ColorScheme = ColorScheme.Professional;
         this.LinearGradientMode = LinearGradientMode.Vertical;
         this.Expand = true;
         this.CaptionHeight = 27;
         this.ImageSize = new Size(18, 18);
         this.m_bShowCaptionbar = true;
         this.m_customColors = new CustomPanelColors();
         this.m_customColors.CustomColorsChanged += OnCustomColorsChanged;
      }
      /// <summary>
      /// Sets the PanelProperties for the Panel
      /// </summary>
      /// <param name="panelColors">The PanelColors table</param>
      public override void SetPanelProperties(PanelColors panelColors)
      {
         this.m_imgHoverBackground = null;
         base.SetPanelProperties(panelColors);
      }
      /// <summary>
      /// Gets the rectangle that represents the display area of the Panel.
      /// </summary>
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public override Rectangle DisplayRectangle
      {
         get
         {
            Padding padding = this.Padding;
            Rectangle displayRectangle = new Rectangle(
                this.ClientRectangle.Left + padding.Left,
                this.ClientRectangle.Top + padding.Top,
                this.ClientRectangle.Width - padding.Left - padding.Right,
                this.ClientRectangle.Height - padding.Top - padding.Bottom);

            if(this.m_bShowCaptionbar == true)
            {
               if(this.Controls.Count > 0)
               {
                  XPanderPanelList xpanderPanelList = this.Controls[0] as XPanderPanelList;
                  if((xpanderPanelList != null) && (xpanderPanelList.Dock == DockStyle.Fill))
                  {
                     displayRectangle = new Rectangle(
                         padding.Left,
                         this.CaptionHeight + padding.Top + Constants.BorderThickness,
                         this.ClientRectangle.Width - padding.Left - padding.Right,
                         this.ClientRectangle.Height - this.CaptionHeight - padding.Top - padding.Bottom - (2 * Constants.BorderThickness));
                  }
                  else
                  {
                     displayRectangle = new Rectangle(
                         padding.Left + Constants.BorderThickness,
                         this.CaptionHeight + padding.Top + Constants.BorderThickness,
                         this.ClientRectangle.Width - padding.Left - padding.Right - (2 * Constants.BorderThickness),
                         this.ClientRectangle.Height - this.CaptionHeight - padding.Top - padding.Bottom - (2 * Constants.BorderThickness));
                  }
               }
            }
            return displayRectangle;
         }
      }
      #endregion

      #region MethodsProtected
      /// <summary>
      /// Raises the ExpandClick event.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">An EventArgs that contains the event data.</param>
      protected override void OnExpandClick(object sender, EventArgs e)
      {
         this.Expand = !this.Expand;
         base.OnExpandClick(sender, e);
      }
      /// <summary>
      /// Raises the ExpandIconHoverState changed event.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">A HoverStateChangeEventArgs that contains the event data.</param>
      protected override void OnExpandIconHoverStateChanged(object sender, HoverStateChangeEventArgs e)
      {
         Invalidate(this.RectangleExpandIcon);
         base.OnExpandIconHoverStateChanged(sender, e);
      }
      /// <summary>
      /// Raises the CloseIconHoverStat changed event.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">A HoverStateChangeEventArgs that contains the event data.</param>
      protected override void OnCloseIconHoverStateChanged(object sender, HoverStateChangeEventArgs e)
      {
         Invalidate(this.RectangleCloseIcon);
         base.OnCloseIconHoverStateChanged(sender, e);
      }
      /// <summary>
      /// Paints the background of the control.
      /// </summary>
      /// <param name="pevent">A PaintEventArgs that contains information about the control to paint.</param>
      protected override void OnPaintBackground(PaintEventArgs pevent)
      {
         if(this.ShowTransparentBackground == true)
         {
            base.OnPaintBackground(pevent);
            this.BackColor = Color.Transparent;
         }
         else
         {
            Rectangle rectangleBounds = this.ClientRectangle;
            if(this.m_bShowCaptionbar == true)
            {
               this.BackColor = Color.Transparent;
               rectangleBounds = new Rectangle(
                   this.ClientRectangle.Left,
                   this.ClientRectangle.Top + this.CaptionHeight,
                   this.ClientRectangle.Width,
                   this.ClientRectangle.Height - this.CaptionHeight);
            }
            RenderBackgroundGradient(
                pevent.Graphics,
                rectangleBounds,
                this.PanelColors.PanelContentGradientBegin,
                this.PanelColors.PanelContentGradientEnd,
                this.LinearGradientMode);
         }
      }
      /// <summary>
      /// Raises the Paint event.
      /// </summary>
      /// <param name="e">A PaintEventArgs that contains the event data.</param>
      protected override void OnPaint(PaintEventArgs e)
      {
         PanelStyle panelStyle = this.PanelStyle;
         if(this.m_bShowCaptionbar == false)
         {
            return;
         }

         using(UseAntiAlias antiAlias = new UseAntiAlias(e.Graphics))
         {
            Graphics graphics = e.Graphics;
            using(UseClearTypeGridFit clearTypeGridFit = new UseClearTypeGridFit(graphics))
            {
               Rectangle captionRectangle = this.CaptionRectangle;
               Color colorGradientBegin = this.PanelColors.PanelCaptionGradientBegin;
               Color colorGradientEnd = this.PanelColors.PanelCaptionGradientEnd;
               Color colorGradientMiddle = this.PanelColors.PanelCaptionGradientMiddle;
               Color colorText = this.PanelColors.PanelCaptionText;
               bool bShowXPanderPanelProfessionalStyle = this.ShowXPanderPanelProfessionalStyle;
               ColorScheme colorSchema = this.ColorScheme;

               if((bShowXPanderPanelProfessionalStyle == true)
						&& (colorSchema == ColorScheme.Professional)
						&& (panelStyle != PanelStyle.Office2007))
               {
                  colorGradientBegin = this.PanelColors.XPanderPanelCaptionGradientBegin;
                  colorGradientEnd = this.PanelColors.XPanderPanelCaptionGradientEnd;
                  colorGradientMiddle = this.PanelColors.XPanderPanelCaptionGradientMiddle;
                  colorText = this.PanelColors.XPanderPanelCaptionText;
               }

               Image image = this.Image;
               RightToLeft rightToLeft = this.RightToLeft;
               Font captionFont = this.CaptionFont;
               Rectangle clientRectangle = this.ClientRectangle;
               string strText = this.Text;
               DockStyle dockStyle = this.Dock;
               bool bExpand = this.Expand;
               if(this.m_imageClosePanel == null)
               {
                  this.m_imageClosePanel = Resources.closePanel;
               }
               Color colorCloseIcon = this.PanelColors.PanelCaptionCloseIcon;
               if(colorCloseIcon == Color.Empty)
               {
                  colorCloseIcon = colorText;
               }
               bool bShowExpandIcon = this.ShowExpandIcon;
               bool bShowCloseIcon = this.ShowCloseIcon;

               switch(panelStyle)
               {
                  case BSE.Windows.Forms.PanelStyle.Default:
                  case PanelStyle.Office2007:
                  DrawStyleDefault(graphics,
              captionRectangle,
              colorGradientBegin,
              colorGradientEnd,
              colorGradientMiddle);
                  break;
               }

               DrawBorder(
                   graphics,
                   clientRectangle,
                   captionRectangle,
                   this.PanelColors.BorderColor,
                   this.PanelColors.InnerBorderColor);

               if((dockStyle == DockStyle.Fill) || (dockStyle == DockStyle.None) ||
                        ((bShowExpandIcon == false) && (bShowCloseIcon == false)))
               {
                  DrawImagesAndText(
                      graphics,
                      captionRectangle,
                      CaptionSpacing,
                      this.ImageRectangle,
                      image,
                      rightToLeft,
                      captionFont,
                      colorText,
                      strText);

                  return;
               }
               if((bShowExpandIcon == true) || (bShowCloseIcon == true))
               {
                  Image imageExpandPanel = GetExpandImage(dockStyle, bExpand);

                  DrawImagesAndText(
                      graphics,
                      dockStyle,
                      CaptionSpacing,
                      captionRectangle,
                      clientRectangle,
                      this.ImageRectangle,
                      image,
                      rightToLeft,
                      bShowCloseIcon,
                      this.m_imageClosePanel,
                      colorCloseIcon,
                      ref this.RectangleCloseIcon,
                      bShowExpandIcon,
                      bExpand,
                      imageExpandPanel,
                      colorText,
                      ref this.RectangleExpandIcon,
                      captionFont,
                      colorText,
                      this.PanelColors.PanelCollapsedCaptionText,
                      strText);

                  if(this.m_imgHoverBackground == null)
                  {
                     this.m_imgHoverBackground = GetPanelIconBackground(
                         graphics,
                         this.ImageRectangle,
                         this.PanelColors.PanelCaptionSelectedGradientBegin,
                         this.PanelColors.PanelCaptionSelectedGradientEnd);
                  }
                  if(this.m_imgHoverBackground != null)
                  {
                     Rectangle rectangleCloseIcon = this.RectangleCloseIcon;
                     if(rectangleCloseIcon != Rectangle.Empty)
                     {
                        if(this.HoverStateCloseIcon == HoverState.Hover)
                        {
                           graphics.DrawImage(this.m_imgHoverBackground, rectangleCloseIcon);
                           DrawIcon(graphics, this.m_imageClosePanel, rectangleCloseIcon, colorCloseIcon, rectangleCloseIcon.Y);
                        }
                     }
                     Rectangle rectangleExpandIcon = this.RectangleExpandIcon;
                     if(rectangleExpandIcon != Rectangle.Empty)
                     {
                        if(this.HoverStateExpandIcon == HoverState.Hover)
                        {
                           graphics.DrawImage(this.m_imgHoverBackground, rectangleExpandIcon);
                           DrawIcon(graphics, imageExpandPanel, rectangleExpandIcon, colorText, rectangleExpandIcon.Y);
                        }
                     }
                  }
               }
            }
         }
      }
      /// <summary>
      /// Raises the PanelCollapsing event.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">A XPanderStateChangeEventArgs that contains the event data.</param>
      protected override void OnPanelCollapsing(object sender, XPanderStateChangeEventArgs e)
      {
         if((this.Dock == DockStyle.Left) || (this.Dock == DockStyle.Right))
         {
            foreach(Control control in this.Controls)
            {
               control.Hide();
            }
         }

         if((this.Dock == DockStyle.Left) || (this.Dock == DockStyle.Right))
         {
            if(this.ClientRectangle.Width > this.CaptionHeight)
            {
               this.m_restoreBounds = this.ClientRectangle;
            }
            this.Width = this.CaptionHeight;
         }

         if((this.Dock == DockStyle.Top) || (this.Dock == DockStyle.Bottom))
         {
            if(this.ClientRectangle.Height > this.CaptionHeight)
            {
               this.m_restoreBounds = this.ClientRectangle;
            }
            this.Height = this.CaptionHeight;
         }

         base.OnPanelCollapsing(sender, e);
      }
      /// <summary>
      /// Raises the PanelExpanding event.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">A XPanderStateChangeEventArgs that contains the event data.</param>
      protected override void OnPanelExpanding(object sender, XPanderStateChangeEventArgs e)
      {
         if((this.Dock == DockStyle.Left) || (this.Dock == DockStyle.Right))
         {
            foreach(Control control in this.Controls)
            {
               control.Show();
            }

            //When ClientRectangle.Width > CaptionHeight the panel size has changed
            //otherwise the captionclick event was executed
            if(this.ClientRectangle.Width == this.CaptionHeight)
            {
               this.Width = this.m_restoreBounds.Width;
            }
         }
         if((this.Dock == DockStyle.Top) || (this.Dock == DockStyle.Bottom))
         {
            this.Height = this.m_restoreBounds.Height;
         }

         base.OnPanelExpanding(sender, e);
      }
      /// <summary>
      /// Raises the PanelStyleChanged event
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">A EventArgs that contains the event data.</param>
      protected override void OnPanelStyleChanged(object sender, PanelStyleChangeEventArgs e)
      {
         OnLayout(new LayoutEventArgs(this, null));
         base.OnPanelStyleChanged(sender, e);

      }
      /// <summary>
      /// Raises the CreateControl method.
      /// </summary>
      protected override void OnCreateControl()
      {
         this.m_restoreBounds = this.ClientRectangle;
         this.MinimumSize = new Size(this.CaptionHeight, this.CaptionHeight);
         base.OnCreateControl();
      }
      /// <summary>
      /// Raises the Resize event.
      /// </summary>
      /// <param name="e">An EventArgs that contains the event data.</param>
      protected override void OnResize(EventArgs e)
      {
         if(this.ShowExpandIcon == true)
         {
            if(this.Expand == false)
            {
               if((this.Dock == DockStyle.Left) || (this.Dock == DockStyle.Right))
               {
                  if(this.Width > this.CaptionHeight)
                  {
                     this.Expand = true;
                  }
               }
               if((this.Dock == DockStyle.Top) || (this.Dock == DockStyle.Bottom))
               {
                  if(this.Height > this.CaptionHeight)
                  {
                     this.Expand = true;
                  }
               }
            }
            else
            {
               if((this.Dock == DockStyle.Left) || (this.Dock == DockStyle.Right))
               {
                  if(this.Width == this.CaptionHeight)
                  {
                     this.Expand = false;
                  }
               }
               if((this.Dock == DockStyle.Top) || (this.Dock == DockStyle.Bottom))
               {
                  if(this.Height == this.CaptionHeight)
                  {
                     this.Expand = false;
                  }
               }
            }
         }
         base.OnResize(e);
      }
      /// <summary>
      /// Raises the <see cref="Control.VisibleChanged"/>VisibleChanged event.
      /// </summary>
      /// <param name="e">An <see cref="System.EventArgs"/> that contains the event data.</param>
      protected override void OnVisibleChanged(EventArgs e)
      {
         System.Windows.Forms.Splitter associatedSplitter = this.AssociatedSplitter;
         if(associatedSplitter != null)
         {
            associatedSplitter.Visible = this.Visible;
         }
         base.OnVisibleChanged(e);
      }
      #endregion

      #region MethodsPrivate
      /// <summary>
      /// Gets the background for an panelicon image
      /// </summary>
      /// <param name="graphics">The Graphics to draw on.</param>
      /// <param name="rectanglePanelIcon"></param>
      /// <param name="backgroundColorBegin"></param>
      /// <param name="backgroundColorEnd"></param>
      /// <returns></returns>
      private static Image GetPanelIconBackground(Graphics graphics, Rectangle rectanglePanelIcon, Color backgroundColorBegin, Color backgroundColorEnd)
      {
         Rectangle rectangle = rectanglePanelIcon;
         rectangle.X = 0;
         rectangle.Y = 0;
         Image image = new Bitmap(rectanglePanelIcon.Width, rectanglePanelIcon.Height, graphics);
         using(Graphics imageGraphics = Graphics.FromImage(image))
         {
            RenderBackgroundGradient(
                imageGraphics,
                rectangle,
                backgroundColorBegin,
                backgroundColorEnd,
                LinearGradientMode.Vertical);
         }
         return image;
      }

      private static void DrawStyleDefault(Graphics graphics,
          Rectangle captionRectangle,
          Color colorGradientBegin,
          Color colorGradientEnd,
          Color colorGradientMiddle)
      {
         RenderDoubleBackgroundGradient(
             graphics,
             captionRectangle,
             colorGradientBegin,
             colorGradientMiddle,
             colorGradientEnd,
             LinearGradientMode.Vertical,
             true);
      }

      private static void DrawBorder(
         Graphics graphics,
         Rectangle panelRectangle,
         Rectangle captionRectangle,
            Color borderColor,
         Color innerBorderColor)
      {
         using(Pen borderPen = new Pen(borderColor))
         {
            // Draws the innerborder around the captionbar
            Rectangle innerBorderRectangle = captionRectangle;
            innerBorderRectangle.Width -= Constants.BorderThickness;
            innerBorderRectangle.Offset(Constants.BorderThickness, Constants.BorderThickness);
            ControlPaint.DrawBorder(
                graphics,
                innerBorderRectangle,
                innerBorderColor,
                ButtonBorderStyle.Solid);

            // Draws the outer border around the captionbar
            ControlPaint.DrawBorder(
                graphics,
                panelRectangle,
                borderColor,
                ButtonBorderStyle.Solid);

            // Draws the line below the captionbar
            graphics.DrawLine(
                borderPen,
                captionRectangle.X,
                captionRectangle.Y + captionRectangle.Height,
                captionRectangle.Width,
                captionRectangle.Y + captionRectangle.Height);

            if(panelRectangle.Height == captionRectangle.Height)
            {
               return;
            }

            // Draws the border lines around the whole panel
            Rectangle panelBorderRectangle = panelRectangle;
            panelBorderRectangle.Y = captionRectangle.Height;
            panelBorderRectangle.Height -= captionRectangle.Height + (int) borderPen.Width;
            panelBorderRectangle.Width -= (int) borderPen.Width;
            Point[] points =
                    {
                        new Point(panelBorderRectangle.X, panelBorderRectangle.Y),
                        new Point(panelBorderRectangle.X, panelBorderRectangle.Y + panelBorderRectangle.Height),
                        new Point(panelBorderRectangle.X + panelBorderRectangle.Width ,panelBorderRectangle.Y + panelBorderRectangle.Height),
                        new Point(panelBorderRectangle.X + panelBorderRectangle.Width ,panelBorderRectangle.Y)
                    };
            graphics.DrawLines(borderPen, points);
         }
      }

      private static Image GetExpandImage(DockStyle dockStyle, bool bIsExpanded)
      {
         Image image = null;
         if((dockStyle == DockStyle.Left) && (bIsExpanded == true))
         {
            image = Resources.ChevronLeft;
         }
         else if((dockStyle == DockStyle.Left) && (bIsExpanded == false))
         {
            image = Resources.ChevronRight;
         }
         else if((dockStyle == DockStyle.Right) && (bIsExpanded == true))
         {
            image = Resources.ChevronRight;
         }
         else if((dockStyle == DockStyle.Right) && (bIsExpanded == false))
         {
            image = Resources.ChevronLeft;
         }
         else if((dockStyle == DockStyle.Top) && (bIsExpanded == true))
         {
            image = Resources.ChevronUp;
         }
         else if((dockStyle == DockStyle.Top) && (bIsExpanded == false))
         {
            image = Resources.ChevronDown;
         }
         else if((dockStyle == DockStyle.Bottom) && (bIsExpanded == true))
         {
            image = Resources.ChevronDown;
         }
         else if((dockStyle == DockStyle.Bottom) && (bIsExpanded == false))
         {
            image = Resources.ChevronUp;
         }

         return image;
      }

      #endregion 
   }

   #endregion

   #region Class PanelDesigner
   /// <summary>
   /// Extends the design mode behavior of a Panel control that supports nested controls.
   /// </summary>
   internal class PanelDesigner : System.Windows.Forms.Design.ParentControlDesigner
   {
      #region FieldsPrivate
      #endregion

      #region MethodsPublic
      /// <summary>
      /// Initializes a new instance of the PanelDesigner class.
      /// </summary>
      public PanelDesigner()
      {
      }
      /// <summary>
      /// Initializes the designer with the specified component.
      /// </summary>
      /// <param name="component"></param>
      public override void Initialize(System.ComponentModel.IComponent component)
      {
         base.Initialize(component);
      }
      /// <summary>
      /// Gets the design-time action lists supported by the component associated with the designer.
      /// </summary>
      public override DesignerActionListCollection ActionLists
      {
         get
         {
            // Create action list collection
            DesignerActionListCollection actionLists = new DesignerActionListCollection();

            // Add custom action list
            actionLists.Add(new PanelDesignerActionList(this.Component));

            // Return to the designer action service
            return actionLists;
         }
      }

      #endregion

      #region MethodsProtected
      /// <summary>
      /// Called when the control that the designer is managing has painted 
      /// its surface so the designer can paint any additional adornments on
      /// top of the control.
      /// </summary>
      /// <param name="e">A PaintEventArgs that provides data for the event.</param>
      protected override void OnPaintAdornments(PaintEventArgs e)
      {
         base.OnPaintAdornments(e);
      }

      #endregion
   }

   #endregion

   #region Class XPanderPanelListDesignerActionList
   /// <summary>
   /// Provides the class for types that define a list of items used to create a smart tag panel for the Panel.
   /// </summary>
   public class PanelDesignerActionList : DesignerActionList
   {
      #region Properties
      /// <summary>
      /// Gets or sets a value indicating whether the panels captionbar is displayed.
      /// </summary>
      public bool ShowCaptionbar
      {
         get
         {
            return this.Panel.ShowCaptionbar;
         }
         set
         {
            SetProperty("ShowCaptionbar", value);
         }
      }
      /// <summary>
      /// Gets or sets a value indicating whether the controls background is transparent.
      /// </summary>
      public bool ShowTransparentBackground
      {
         get
         {
            return this.Panel.ShowTransparentBackground;
         }
         set
         {
            SetProperty("ShowTransparentBackground", value);
         }
      }
      /// <summary>
      /// Gets or sets a value indicating whether the controls caption professional colorscheme is the same then the XPanderPanels
      /// </summary>
      public bool ShowXPanderPanelProfessionalStyle
      {
         get
         {
            return this.Panel.ShowXPanderPanelProfessionalStyle;
         }
         set
         {
            SetProperty("ShowXPanderPanelProfessionalStyle", value);
         }
      }
      /// <summary>
      /// Gets or sets a value indicating whether the expand icon of the panel is visible
      /// </summary>
      public bool ShowExpandIcon
      {
         get
         {
            return this.Panel.ShowExpandIcon;
         }
         set
         {
            SetProperty("ShowExpandIcon", value);
         }
      }
      /// <summary>
      /// Gets or sets a value indicating whether the close icon is visible
      /// </summary>
      public bool ShowCloseIcon
      {
         get
         {
            return this.Panel.ShowCloseIcon;
         }
         set
         {
            SetProperty("ShowCloseIcon", value);
         }
      }
      /// <summary>
      /// Gets or sets the style of the panel.
      /// </summary>
      public PanelStyle PanelStyle
      {
         get
         {
            return this.Panel.PanelStyle;
         }
         set
         {
            SetProperty("PanelStyle", value);
         }
      }
      /// <summary>
      /// Gets or sets the color schema which is used for the panel.
      /// </summary>
      public ColorScheme ColorScheme
      {
         get
         {
            return this.Panel.ColorScheme;
         }
         set
         {
            SetProperty("ColorScheme", value);
         }
      }
      #endregion

      #region MethodsPublic
      /// <summary>
      /// Initializes a new instance of the PanelDesignerActionList class.
      /// </summary>
      /// <param name="component">A component related to the DesignerActionList.</param>
      public PanelDesignerActionList(System.ComponentModel.IComponent component)
         : base(component)
      {
         // Automatically display smart tag panel when
         // design-time component is dropped onto the
         // Windows Forms Designer
         base.AutoShow = true;
      }
      /// <summary>
      /// Returns the collection of DesignerActionItem objects contained in the list.
      /// </summary>
      /// <returns> A DesignerActionItem array that contains the items in this list.</returns>
      public override DesignerActionItemCollection GetSortedActionItems()
      {
         // Create list to store designer action items
         DesignerActionItemCollection actionItems = new DesignerActionItemCollection();

         actionItems.Add(
           new DesignerActionMethodItem(
             this,
             "ToggleDockStyle",
             GetDockStyleText(),
             "Design",
             "Dock or undock this control in it's parent container.",
             true));

         actionItems.Add(
            new DesignerActionPropertyItem(
            "ShowTransparentBackground",
            "Show transparent backcolor",
            GetCategory(this.Panel, "ShowTransparentBackground")));

         actionItems.Add(
            new DesignerActionPropertyItem(
            "ShowXPanderPanelProfessionalStyle",
            "Show the XPanderPanels professional colorscheme",
            GetCategory(this.Panel, "ShowXPanderPanelProfessionalStyle")));

         actionItems.Add(
             new DesignerActionPropertyItem(
             "ShowCaptionbar",
             "Show the captionbar on top of the panel",
             GetCategory(this.Panel, "ShowCaptionbar")));

         actionItems.Add(
         new DesignerActionPropertyItem(
         "ShowExpandIcon",
         "Show the expand panel icon (not at DockStyle.None or DockStyle.Fill)",
         GetCategory(this.Panel, "ShowExpandIcon")));

         actionItems.Add(
             new DesignerActionPropertyItem(
             "ShowCloseIcon",
         "Show the close panel icon (not at DockStyle.None or DockStyle.Fill)",
             GetCategory(this.Panel, "ShowCloseIcon")));

         actionItems.Add(
            new DesignerActionPropertyItem(
            "PanelStyle",
            "Select PanelStyle",
            GetCategory(this.Panel, "PanelStyle")));

         actionItems.Add(
            new DesignerActionPropertyItem(
            "ColorScheme",
            "Select ColorScheme",
            GetCategory(this.Panel, "ColorScheme")));

         return actionItems;
      }
      /// <summary>
      /// Dock/Undock designer action method implementation
      /// </summary>
      public void ToggleDockStyle()
      {

         // Toggle ClockControl's Dock property
         if(this.Panel.Dock != DockStyle.Fill)
         {
            SetProperty("Dock", DockStyle.Fill);
         }
         else
         {
            SetProperty("Dock", DockStyle.None);
         }
      }

      #endregion

      #region MethodsPrivate

      // Helper method that returns an appropriate
      // display name for the Dock/Undock property,
      // based on the ClockControl's current Dock 
      // property value
      private string GetDockStyleText()
      {
         if(this.Panel.Dock == DockStyle.Fill)
         {
            return "Undock in parent container";
         }
         else
         {
            return "Dock in parent container";
         }
      }

      private Panel Panel
      {
         get
         {
            return (Panel) this.Component;
         }
      }

      // Helper method to safely set a component’s property
      private void SetProperty(string propertyName, object value)
      {
         // Get property
         System.ComponentModel.PropertyDescriptor property
				= System.ComponentModel.TypeDescriptor.GetProperties(this.Panel)[propertyName];
         // Set property value
         property.SetValue(this.Panel, value);
      }

      // Helper method to return the Category string from a
      // CategoryAttribute assigned to a property exposed by 
      //the specified object
      private static string GetCategory(object source, string propertyName)
      {
         System.Reflection.PropertyInfo property = source.GetType().GetProperty(propertyName);
         CategoryAttribute attribute = (CategoryAttribute) property.GetCustomAttributes(typeof(CategoryAttribute), false)[0];
         if(attribute == null)
            return null;
         return attribute.Category;
      }

      #endregion
   }

   #endregion
}
