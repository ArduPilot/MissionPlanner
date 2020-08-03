using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.ComponentModel.Design;
using BSE.Windows.Forms.Properties;

namespace BSE.Windows.Forms
{
   #region Class XPanderPanel
   /// <summary>
   /// Used to group collections of controls. 
   /// </summary>
   /// <remarks>
   /// XPanderPanel controls represent the expandable and collapsable panels in XPanderPanelList.
   /// The XpanderPanel is a control that contains other controls.
   /// You can use a XPanderPanel to group collections of controls such as the XPanderPanelList.
   /// The order of xpanderpanels in the XPanderPanelList.XPanderPanels collection reflects the order
   /// of xpanderpanels controls. To change the order of tabs in the control, you must change
   /// their positions in the collection by removing them and inserting them at new indexes.
   /// You can change the xpanderpanel's appearance. For example, to make it appear flat,
   /// set the CaptionStyle property to CaptionStyle.Flat.
   /// On top of the XPanderPanel there is the captionbar.
   /// This captionbar may contain an image and text. According to it's properties the panel is closable.
   /// </remarks>
   /// <copyright>Copyright © 2006-2008 Uwe Eichkorn
   /// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
   /// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
   /// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
   /// PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
   /// REMAINS UNCHANGED.
   /// </copyright>
 
   [DesignTimeVisible(false)]
   public partial class XPanderPanel : BasePanel
   {
      #region EventsPublic
      /// <summary>
      /// The CaptionStyleChanged event occurs when CaptionStyle flags have been changed.
      /// </summary>
      [Description("The CaptionStyleChanged event occurs when CaptionStyle flags have been changed.")]
      public event EventHandler<EventArgs> CaptionStyleChanged;
      #endregion

      #region Constants
      #endregion

      #region FieldsPrivate

      private System.Drawing.Image m_imageChevron;
      private System.Drawing.Image m_imageChevronUp;
      private System.Drawing.Image m_imageChevronDown;
      private CustomXPanderPanelColors m_customColors;
      private System.Drawing.Image m_imageClosePanel;
      private bool m_bIsClosable = true;
      private CaptionStyle m_captionStyle;

      #endregion

      #region Properties
      /// <summary>
      /// Gets or sets a value indicating whether the expand icon in a XPanderPanel is visible.
      /// </summary>
      [Description("Gets or sets a value indicating whether the expand icon in a XPanderPanel is visible.")]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [DefaultValue(false)]
      [Browsable(false)]
      [Category("Appearance")]
      public override bool ShowExpandIcon
      {
         get
         {
            return base.ShowExpandIcon;
         }
         set
         {
            base.ShowExpandIcon = value;
         }
      }
      /// <summary>
      /// Gets or sets a value indicating whether the close icon in a XPanderPanel is visible.
      /// </summary>
      [Description("Gets or sets a value indicating whether the close icon in a XPanderPanel is visible.")]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [DefaultValue(false)]
      [Browsable(false)]
      [Category("Appearance")]
      public override bool ShowCloseIcon
      {
         get
         {
            return base.ShowCloseIcon;
         }
         set
         {
            base.ShowCloseIcon = value;
         }
      }
      /// <summary>
      /// Gets the custom colors which are used for the XPanderPanel.
      /// </summary>
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
      [Description("The custom colors which are used for the XPanderPanel.")]
      [Category("Appearance")]
      public CustomXPanderPanelColors CustomColors
      {
         get
         {
            return this.m_customColors;
         }
      }
      /// <summary>
      /// Gets or sets the style of the caption (not for PanelStyle.Aqua).
      /// </summary>
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [Browsable(false)]
      public CaptionStyle CaptionStyle
      {
         get
         {
            return this.m_captionStyle;
         }
         set
         {
            if(value.Equals(this.m_captionStyle) == false)
            {
               this.m_captionStyle = value;
               OnCaptionStyleChanged(this, EventArgs.Empty);
            }
         }
      }
      /// <summary>
      /// Gets or sets a value indicating whether this XPanderPanel is closable.
      /// </summary>
      [Description("Gets or sets a value indicating whether this XPanderPanel is closable.")]
      [DefaultValue(true)]
      [Category("Appearance")]
      public bool IsClosable
      {
         get
         {
            return this.m_bIsClosable;
         }
         set
         {
            if(value.Equals(this.m_bIsClosable) == false)
            {
               this.m_bIsClosable = value;
               this.Invalidate(false);
            }
         }
      }
      /// <summary>
      /// Gets or sets the height and width of the XPanderPanel.
      /// </summary>
      [Browsable(false)]
      public new Size Size
      {
         get
         {
            return base.Size;
         }
         set
         {
            base.Size = value;
         }
      }
      #endregion

      #region MethodsPublic
      /// <summary>
      /// Initializes a new instance of the XPanderPanel class.
      /// </summary>
      public XPanderPanel()
      {
         InitializeComponent();

         this.BackColor = Color.Transparent;
         this.CaptionStyle = CaptionStyle.Normal;
         this.ForeColor = SystemColors.ControlText;
         this.Height = this.CaptionHeight;
         this.ShowBorder = true;
         this.m_customColors = new CustomXPanderPanelColors();
         this.m_customColors.CustomColorsChanged += OnCustomColorsChanged;
      }

      /// <summary>
      /// Gets the rectangle that represents the display area of the XPanderPanel.
      /// </summary>
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public override Rectangle DisplayRectangle
      {
         get
         {
            Padding padding = this.Padding;

            Rectangle displayRectangle = new Rectangle(
                padding.Left + Constants.BorderThickness,
                padding.Top + this.CaptionHeight,
                this.ClientRectangle.Width - padding.Left - padding.Right - (2 * Constants.BorderThickness),
                this.ClientRectangle.Height - this.CaptionHeight - padding.Top - padding.Bottom);

            if(this.Controls.Count > 0)
            {
               XPanderPanelList xpanderPanelList = this.Controls[0] as XPanderPanelList;
               if((xpanderPanelList != null) && (xpanderPanelList.Dock == DockStyle.Fill))
               {
                  displayRectangle = new Rectangle(
                      padding.Left,
                      padding.Top + this.CaptionHeight,
                      this.ClientRectangle.Width - padding.Left - padding.Right,
                      this.ClientRectangle.Height - this.CaptionHeight - padding.Top - padding.Bottom - Constants.BorderThickness);
               }
            }
            return displayRectangle;
         }
      }
      #endregion

      #region MethodsProtected
      /// <summary>
      /// Paints the background of the control.
      /// </summary>
      /// <param name="pevent">A PaintEventArgs that contains information about the control to paint.</param>
      protected override void OnPaintBackground(PaintEventArgs pevent)
      {
         base.OnPaintBackground(pevent);
         base.BackColor = Color.Transparent;
         Color backColor = this.PanelColors.XPanderPanelBackColor;
         if((backColor != Color.Empty) && backColor != Color.Transparent)
         {
            Rectangle rectangle = new Rectangle(
                0,
                this.CaptionHeight,
                this.ClientRectangle.Width,
                this.ClientRectangle.Height - this.CaptionHeight);

            using(SolidBrush backgroundBrush = new SolidBrush(backColor))
            {
               pevent.Graphics.FillRectangle(backgroundBrush, rectangle);
            }
         }
      }
      /// <summary>
      /// Raises the Paint event.
      /// </summary>
      /// <param name="e">A PaintEventArgs that contains the event data.</param>
      protected override void OnPaint(PaintEventArgs e)
      {
         if(IsZeroWidthOrHeight(this.CaptionRectangle) == true)
         {
            return;
         }

         using(UseAntiAlias antiAlias = new UseAntiAlias(e.Graphics))
         {
            Graphics graphics = e.Graphics;
            using(UseClearTypeGridFit clearTypeGridFit = new UseClearTypeGridFit(graphics))
            {
               bool bExpand = this.Expand;
               bool bShowBorder = this.ShowBorder;
               Color borderColor = this.PanelColors.BorderColor;
               Rectangle borderRectangle = this.ClientRectangle;

               switch(this.PanelStyle)
               {
                  case PanelStyle.Default:
                  case PanelStyle.Office2007:
                  DrawCaptionbar(graphics, bExpand, bShowBorder, this.PanelStyle);
                  CalculatePanelHeights();
                  DrawBorders(graphics, this);
                  break;
               }
            }
         }
      }
      /// <summary>
      /// Raises the PanelExpanding event.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">A XPanderStateChangeEventArgs that contains the event data.</param>
      protected override void OnPanelExpanding(object sender, XPanderStateChangeEventArgs e)
      {
         bool bExpand = e.Expand;
         if(bExpand == true)
         {
            this.Expand = bExpand;
            this.Invalidate(false);
         }
         base.OnPanelExpanding(sender, e);
      }
      /// <summary>
      /// Raises the CaptionStyleChanged event.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">An EventArgs that contains the event data.</param>
      protected virtual void OnCaptionStyleChanged(object sender, EventArgs e)
      {
         this.Invalidate(this.CaptionRectangle);
         if(this.CaptionStyleChanged != null)
         {
            this.CaptionStyleChanged(sender, e);
         }
      }
      /// <summary>
      /// Raises the MouseUp event.
      /// </summary>
      /// <param name="e">A MouseEventArgs that contains data about the OnMouseUp event.</param>
      protected override void OnMouseUp(MouseEventArgs e)
      {
         if(this.CaptionRectangle.Contains(e.X, e.Y) == true)
         {
            if((this.ShowCloseIcon == false) && (this.ShowExpandIcon == false))
            {
               OnExpandClick(this, EventArgs.Empty);
            }
            else if((this.ShowCloseIcon == true) && (this.ShowExpandIcon == false))
            {
               if(this.RectangleCloseIcon.Contains(e.X, e.Y) == false)
               {
                  OnExpandClick(this, EventArgs.Empty);
               }
            }
            if(this.ShowExpandIcon == true)
            {
               if(this.RectangleExpandIcon.Contains(e.X, e.Y) == true)
               {
                  OnExpandClick(this, EventArgs.Empty);
               }
            }
            if((this.ShowCloseIcon == true) && (this.m_bIsClosable == true))
            {
               if(this.RectangleCloseIcon.Contains(e.X, e.Y) == true)
               {
                  OnCloseClick(this, EventArgs.Empty);
               }
            }
         }
      }
      /// <summary>
      /// Raises the VisibleChanged event.
      /// </summary>
      /// <param name="e">An EventArgs that contains the event data.</param>
      protected override void OnVisibleChanged(EventArgs e)
      {
         base.OnVisibleChanged(e);

         if(this.DesignMode == true)
         {
            return;
         }
         if(this.Visible == false)
         {
            if(this.Expand == true)
            {
               this.Expand = false;
               foreach(Control control in this.Parent.Controls)
               {
                  BSE.Windows.Forms.XPanderPanel xpanderPanel =
                            control as BSE.Windows.Forms.XPanderPanel;

                  if(xpanderPanel != null)
                  {
                     if(xpanderPanel.Visible == true)
                     {
                        xpanderPanel.Expand = true;
                        return;
                     }
                  }
               }
            }
         }
#if DEBUG
         //System.Diagnostics.Trace.WriteLine("Visibility: " + this.Name + this.Visible);
#endif
         CalculatePanelHeights();
      }

      #endregion

      #region MethodsPrivate

      private void DrawCaptionbar(Graphics graphics, bool bExpand, bool bShowBorder, PanelStyle panelStyle)
      {
         Rectangle captionRectangle = this.CaptionRectangle;
         Color colorGradientBegin = this.PanelColors.XPanderPanelCaptionGradientBegin;
         Color colorGradientEnd = this.PanelColors.XPanderPanelCaptionGradientEnd;
         Color colorGradientMiddle = this.PanelColors.XPanderPanelCaptionGradientMiddle;
         Color colorText = this.PanelColors.XPanderPanelCaptionText;
         Color foreColorCloseIcon = this.PanelColors.XPanderPanelCaptionCloseIcon;
         Color foreColorExpandIcon = this.PanelColors.XPanderPanelCaptionExpandIcon;
         bool bHover = this.HoverStateCaptionBar == HoverState.Hover ? true : false;

         if(this.m_imageClosePanel == null)
         {
            this.m_imageClosePanel = Resources.closePanel;
         }
         if(this.m_imageChevronUp == null)
         {
            this.m_imageChevronUp = Resources.ChevronUp;
         }
         if(this.m_imageChevronDown == null)
         {
            this.m_imageChevronDown = Resources.ChevronDown;
         }

         this.m_imageChevron = this.m_imageChevronDown;
         if(bExpand == true)
         {
            this.m_imageChevron = this.m_imageChevronUp;
         }

         if(this.m_captionStyle == CaptionStyle.Normal)
         {
            if(bHover == true)
            {
               colorGradientBegin = this.PanelColors.XPanderPanelSelectedCaptionBegin;
               colorGradientEnd = this.PanelColors.XPanderPanelSelectedCaptionEnd;
               colorGradientMiddle = this.PanelColors.XPanderPanelSelectedCaptionMiddle;
               if(bExpand == true)
               {
                  colorGradientBegin = this.PanelColors.XPanderPanelPressedCaptionBegin;
                  colorGradientEnd = this.PanelColors.XPanderPanelPressedCaptionEnd;
                  colorGradientMiddle = this.PanelColors.XPanderPanelPressedCaptionMiddle;
               }
               colorText = this.PanelColors.XPanderPanelSelectedCaptionText;
               foreColorCloseIcon = colorText;
               foreColorExpandIcon = colorText;
            }
            else
            {
               if(bExpand == true)
               {
                  colorGradientBegin = this.PanelColors.XPanderPanelCheckedCaptionBegin;
                  colorGradientEnd = this.PanelColors.XPanderPanelCheckedCaptionEnd;
                  colorGradientMiddle = this.PanelColors.XPanderPanelCheckedCaptionMiddle;
                  colorText = this.PanelColors.XPanderPanelSelectedCaptionText;
                  foreColorCloseIcon = colorText;
                  foreColorExpandIcon = colorText;
               }
            }
            if(panelStyle != PanelStyle.Office2007)
            {
               RenderDoubleBackgroundGradient(
               graphics,
               captionRectangle,
               colorGradientBegin,
               colorGradientMiddle,
               colorGradientEnd,
               LinearGradientMode.Vertical,
               false);
            }
            else
            {
               RenderButtonBackground(
                   graphics,
                   captionRectangle,
                   colorGradientBegin,
                   colorGradientMiddle,
                   colorGradientEnd);
            }
         }
         else
         {
            Color colorFlatGradientBegin = this.PanelColors.XPanderPanelFlatCaptionGradientBegin;
            Color colorFlatGradientEnd = this.PanelColors.XPanderPanelFlatCaptionGradientEnd;
            Color colorInnerBorder = this.PanelColors.InnerBorderColor;
            colorText = this.PanelColors.XPanderPanelCaptionText;
            foreColorExpandIcon = colorText;

            RenderFlatButtonBackground(graphics, captionRectangle, colorFlatGradientBegin, colorFlatGradientEnd, bHover);
            DrawInnerBorders(graphics, this);
         }

         DrawImagesAndText(
             graphics,
             captionRectangle,
             CaptionSpacing,
             this.ImageRectangle,
             this.Image,
             this.RightToLeft,
             this.m_bIsClosable,
             this.ShowCloseIcon,
             this.m_imageClosePanel,
             foreColorCloseIcon,
             ref this.RectangleCloseIcon,
             this.ShowExpandIcon,
             this.m_imageChevron,
             foreColorExpandIcon,
             ref this.RectangleExpandIcon,
             this.CaptionFont,
             colorText,
             this.Text);
      }

      private static void DrawBorders(Graphics graphics, XPanderPanel xpanderPanel)
      {
         if(xpanderPanel.ShowBorder == true)
         {
            using(GraphicsPath graphicsPath = new GraphicsPath())
            {
               using(Pen borderPen = new Pen(xpanderPanel.PanelColors.BorderColor, Constants.BorderThickness))
               {
                  Rectangle captionRectangle = xpanderPanel.CaptionRectangle;
                  Rectangle borderRectangle = captionRectangle;

                  if(xpanderPanel.Expand == true)
                  {
                     borderRectangle = xpanderPanel.ClientRectangle;

                     graphics.DrawLine(
                         borderPen,
                         captionRectangle.Left,
                         captionRectangle.Top + captionRectangle.Height - Constants.BorderThickness,
                         captionRectangle.Left + captionRectangle.Width,
                         captionRectangle.Top + captionRectangle.Height - Constants.BorderThickness);
                  }

                  XPanderPanelList xpanderPanelList = xpanderPanel.Parent as XPanderPanelList;
                  if((xpanderPanelList != null) && (xpanderPanelList.Dock == DockStyle.Fill))
                  {
                     BSE.Windows.Forms.Panel panel = xpanderPanelList.Parent as BSE.Windows.Forms.Panel;
                     XPanderPanel parentXPanderPanel = xpanderPanelList.Parent as XPanderPanel;
                     if(((panel != null) && (panel.Padding == new Padding(0))) ||
                                ((parentXPanderPanel != null) && (parentXPanderPanel.Padding == new Padding(0))))
                     {
                        if(xpanderPanel.Top != 0)
                        {
                           graphicsPath.AddLine(
                               borderRectangle.Left,
                               borderRectangle.Top,
                               borderRectangle.Left + captionRectangle.Width,
                               borderRectangle.Top);
                        }

                        // Left vertical borderline
                        graphics.DrawLine(borderPen,
                            borderRectangle.Left,
                            borderRectangle.Top,
                            borderRectangle.Left,
                            borderRectangle.Top + borderRectangle.Height);

                        // Right vertical borderline
                        graphics.DrawLine(borderPen,
                            borderRectangle.Left + borderRectangle.Width - Constants.BorderThickness,
                            borderRectangle.Top,
                            borderRectangle.Left + borderRectangle.Width - Constants.BorderThickness,
                            borderRectangle.Top + borderRectangle.Height);
                     }
                     else
                     {
                        // Upper horizontal borderline only at the top xpanderPanel
                        if(xpanderPanel.Top == 0)
                        {
                           graphicsPath.AddLine(
                               borderRectangle.Left,
                               borderRectangle.Top,
                               borderRectangle.Left + borderRectangle.Width,
                               borderRectangle.Top);
                        }

                        // Left vertical borderline
                        graphicsPath.AddLine(
                            borderRectangle.Left,
                            borderRectangle.Top,
                            borderRectangle.Left,
                            borderRectangle.Top + borderRectangle.Height);

                        //Lower horizontal borderline
                        graphicsPath.AddLine(
                            borderRectangle.Left,
                            borderRectangle.Top + borderRectangle.Height - Constants.BorderThickness,
                            borderRectangle.Left + borderRectangle.Width - Constants.BorderThickness,
                            borderRectangle.Top + borderRectangle.Height - Constants.BorderThickness);

                        // Right vertical borderline
                        graphicsPath.AddLine(
                            borderRectangle.Left + borderRectangle.Width - Constants.BorderThickness,
                            borderRectangle.Top,
                            borderRectangle.Left + borderRectangle.Width - Constants.BorderThickness,
                            borderRectangle.Top + borderRectangle.Height);
                     }
                  }
                  else
                  {
                     // Upper horizontal borderline only at the top xpanderPanel
                     if(xpanderPanel.Top == 0)
                     {
                        graphicsPath.AddLine(
                            borderRectangle.Left,
                            borderRectangle.Top,
                            borderRectangle.Left + borderRectangle.Width,
                            borderRectangle.Top);
                     }

                     // Left vertical borderline
                     graphicsPath.AddLine(
                         borderRectangle.Left,
                         borderRectangle.Top,
                         borderRectangle.Left,
                         borderRectangle.Top + borderRectangle.Height);

                     //Lower horizontal borderline
                     graphicsPath.AddLine(
                         borderRectangle.Left,
                         borderRectangle.Top + borderRectangle.Height - Constants.BorderThickness,
                         borderRectangle.Left + borderRectangle.Width - Constants.BorderThickness,
                         borderRectangle.Top + borderRectangle.Height - Constants.BorderThickness);

                     // Right vertical borderline
                     graphicsPath.AddLine(
                         borderRectangle.Left + borderRectangle.Width - Constants.BorderThickness,
                         borderRectangle.Top,
                         borderRectangle.Left + borderRectangle.Width - Constants.BorderThickness,
                         borderRectangle.Top + borderRectangle.Height);
                  }
               }
               using(Pen borderPen = new Pen(xpanderPanel.PanelColors.BorderColor, Constants.BorderThickness))
               {
                  graphics.DrawPath(borderPen, graphicsPath);
               }
            }
         }
      }


      private static void DrawInnerBorders(Graphics graphics, XPanderPanel xpanderPanel)
      {
         if(xpanderPanel.ShowBorder == true)
         {
            using(GraphicsPath graphicsPath = new GraphicsPath())
            {
               Rectangle captionRectangle = xpanderPanel.CaptionRectangle;
               XPanderPanelList xpanderPanelList = xpanderPanel.Parent as XPanderPanelList;
               if((xpanderPanelList != null) && (xpanderPanelList.Dock == DockStyle.Fill))
               {
                  BSE.Windows.Forms.Panel panel = xpanderPanelList.Parent as BSE.Windows.Forms.Panel;
                  XPanderPanel parentXPanderPanel = xpanderPanelList.Parent as XPanderPanel;
                  if(((panel != null) && (panel.Padding == new Padding(0))) ||
                            ((parentXPanderPanel != null) && (parentXPanderPanel.Padding == new Padding(0))))
                  {
                     //Left vertical borderline
                     graphicsPath.AddLine(captionRectangle.X, captionRectangle.Y + captionRectangle.Height, captionRectangle.X, captionRectangle.Y + Constants.BorderThickness);
                     if(xpanderPanel.Top == 0)
                     {
                        //Upper horizontal borderline
                        graphicsPath.AddLine(captionRectangle.X, captionRectangle.Y, captionRectangle.X + captionRectangle.Width, captionRectangle.Y);
                     }
                     else
                     {
                        //Upper horizontal borderline
                        graphicsPath.AddLine(captionRectangle.X, captionRectangle.Y + Constants.BorderThickness, captionRectangle.X + captionRectangle.Width, captionRectangle.Y + Constants.BorderThickness);
                     }
                  }
               }
               else
               {
                  //Left vertical borderline
                  graphicsPath.AddLine(captionRectangle.X + Constants.BorderThickness, captionRectangle.Y + captionRectangle.Height, captionRectangle.X + Constants.BorderThickness, captionRectangle.Y);
                  if(xpanderPanel.Top == 0)
                  {
                     //Upper horizontal borderline
                     graphicsPath.AddLine(captionRectangle.X + Constants.BorderThickness, captionRectangle.Y + Constants.BorderThickness, captionRectangle.X + captionRectangle.Width - Constants.BorderThickness, captionRectangle.Y + Constants.BorderThickness);
                  }
                  else
                  {
                     //Upper horizontal borderline
                     graphicsPath.AddLine(captionRectangle.X + Constants.BorderThickness, captionRectangle.Y, captionRectangle.X + captionRectangle.Width - Constants.BorderThickness, captionRectangle.Y);
                  }
               }

               using(Pen borderPen = new Pen(xpanderPanel.PanelColors.InnerBorderColor))
               {
                  graphics.DrawPath(borderPen, graphicsPath);
               }
            }
         }
      }

      private void CalculatePanelHeights()
      {
         if(this.Parent == null)
         {
            return;
         }

         int iPanelHeight = this.Parent.Padding.Top;

         foreach(Control control in this.Parent.Controls)
         {
            BSE.Windows.Forms.XPanderPanel xpanderPanel =
					control as BSE.Windows.Forms.XPanderPanel;

            if((xpanderPanel != null) && (xpanderPanel.Visible == true))
            {
               iPanelHeight += xpanderPanel.CaptionHeight;
            }
         }

         iPanelHeight += this.Parent.Padding.Bottom;

         foreach(Control control in this.Parent.Controls)
         {
            BSE.Windows.Forms.XPanderPanel xpanderPanel =
					control as BSE.Windows.Forms.XPanderPanel;

            if(xpanderPanel != null)
            {
               if(xpanderPanel.Expand == true)
               {
                  xpanderPanel.Height = this.Parent.Height
                            + xpanderPanel.CaptionHeight
                            - iPanelHeight;
               }
               else
               {
                  xpanderPanel.Height = xpanderPanel.CaptionHeight;
               }
            }
         }

         int iTop = this.Parent.Padding.Top;
         foreach(Control control in this.Parent.Controls)
         {
            BSE.Windows.Forms.XPanderPanel xpanderPanel =
					control as BSE.Windows.Forms.XPanderPanel;

            if((xpanderPanel != null) && (xpanderPanel.Visible == true))
            {
               xpanderPanel.Top = iTop;
               iTop += xpanderPanel.Height;
            }
         }
      }

      #endregion
   }

   #endregion

 
}
