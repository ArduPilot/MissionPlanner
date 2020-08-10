using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using WForms = System.Windows.Forms;

namespace Xamarin.Forms.Platform.WinForms
{
	public class FrameRenderer : DrawingViewRenderer<Frame, WForms.Control>
	{
		Pen _pen = null;

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				_pen?.Dispose();
				_pen = null;
			}
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new WForms.Control());
				}

				UpdateOutlineColor();
			}

			base.OnElementChanged(e);
		}

		protected override void OnPaint(object sender, PaintEventArgs e)
		{
			base.OnPaint(sender, e);

			var element = Element;

			if (element != null && _pen != null)
			{
				var padding = element.Padding;
				var rect = new System.Drawing.Rectangle(
						(int)(element.X + padding.Left / 2),
						(int)(element.Y + padding.Top / 2),
						(int)(element.Width - padding.Right),
						(int)(element.Height - padding.Bottom));
				var radius = element.CornerRadius;
				var diameter = radius * 2;
				if (radius > 0.0)
				{
					using (var gp = new GraphicsPath())
					{
						gp.StartFigure();
						gp.AddArc(
							(float)rect.Left,
							(float)rect.Top,
							diameter, diameter, 180, 90);
						gp.AddLine(
							(float)(rect.Left + radius), (float)rect.Top,
							(float)(rect.Right - radius), (float)rect.Top);
						gp.AddArc(
							(float)rect.Right - diameter,
							(float)rect.Top,
							diameter, diameter, 270, 90);
						gp.AddLine(
							(float)rect.Right, (float)rect.Top + radius,
							(float)rect.Right, (float)rect.Bottom - radius);
						gp.AddArc(
							(float)rect.Right - diameter,
							(float)rect.Bottom - diameter,
							diameter, diameter, 0, 90);
						gp.AddLine(
							(float)(rect.Right - radius), (float)rect.Bottom,
							(float)(rect.Left + radius), (float)rect.Bottom);
						gp.AddArc(
							(float)rect.Left,
							(float)rect.Bottom - diameter,
							diameter, diameter, 90, 90);
						gp.AddLine(
							(float)rect.Left, (float)rect.Bottom - radius,
							(float)rect.Left, (float)rect.Top + radius);

						gp.CloseFigure();

						e.Graphics.DrawPath(_pen, gp);
					}
				}
				else
				{
					e.Graphics.DrawRectangle(_pen, rect);
				}
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Frame.BorderColorProperty.PropertyName ||
				e.PropertyName == Frame.HasShadowProperty.PropertyName)
			{
				UpdateOutlineColor();
			}
			else if (e.PropertyName == Frame.CornerRadiusProperty.PropertyName)
			{
				UpdateCornerRadius();
			}
		}

		void UpdateOutlineColor()
		{
			UpdatePropertyHelper((element, control) =>
			{
				_pen?.Dispose();
				_pen = null;
				if (element.BorderColor != Color.Default)
				{
					_pen = new Pen(element.BorderColor.ToWindowsColor());
				}

				control.Invalidate();
			});
		}

		void UpdateCornerRadius()
		{
			UpdatePropertyHelper((element, control) => control.Invalidate());
		}
	}
}
