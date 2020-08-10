using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using WForms = System.Windows.Forms;

namespace Xamarin.Forms.Platform.WinForms
{
	public class DrawingViewRenderer<TElement, TNativeElement> :
		ViewRenderer<TElement, TNativeElement>
		where TElement : View
		where TNativeElement : WForms.Control
	{
		Matrix _matrix = new Matrix();

		protected override void OnElementChanged(ElementChangedEventArgs<TElement> e)
		{
			UpdateTransform();

			base.OnElementChanged(e);
		}

		protected override void OnNativeElementChanged(NativeElementChangedEventArgs<TNativeElement> e)
		{
			base.OnNativeElementChanged(e);
			if (e.OldControl != null)
			{
				e.OldControl.Paint -= OnPaint;
			}

			if (e.NewControl != null)
			{
				e.NewControl.Paint += OnPaint;
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == VisualElement.WidthProperty.PropertyName ||
				e.PropertyName == VisualElement.HeightProperty.PropertyName ||
				e.PropertyName == VisualElement.AnchorXProperty.PropertyName ||
				e.PropertyName == VisualElement.AnchorYProperty.PropertyName ||
				e.PropertyName == VisualElement.RotationProperty.PropertyName ||
				e.PropertyName == VisualElement.ScaleProperty.PropertyName ||
				e.PropertyName == VisualElement.TranslationXProperty.PropertyName ||
				e.PropertyName == VisualElement.TranslationYProperty.PropertyName)
			{
				UpdateTransform();
			}

		}

		protected virtual void OnPaint(object sender, WForms.PaintEventArgs e)
		{
			var element = Element;
			if (element == null)
			{
				return;
			}

			e.Graphics.Clear(
				element.BackgroundColor.A > 0.0 ?
					element.BackgroundColor.ToWindowsColor() :
					SystemColors.Window);
			e.Graphics.Transform = _matrix;
		}

		void UpdateTransform()
		{
			_matrix.Reset();
			UpdatePropertyHelper((element, control) =>
			{
				var pt = new PointF(
					(float)(element.Width * element.AnchorX),
					(float)(element.Height * element.AnchorY));
				if (pt.X >= 0.0f && pt.Y >= 0.0f)
				{
					var scale = (float)element.Scale;

					_matrix.Translate(-pt.X, -pt.Y);
					_matrix.Scale(scale, scale, MatrixOrder.Append);
					_matrix.Rotate((float)element.Rotation, MatrixOrder.Append);
					_matrix.Translate(
						(float)(element.TranslationX + pt.X),
						(float)(element.TranslationY + pt.Y),
						MatrixOrder.Append);

					/*
					var matrixT1 = new OpenTK.Matrix3 { M31 = -pt.X, M32 = -pt.Y, M11 = 1.0f, M22 = 1.0f, M33 = 1.0f };
					var matrixR = OpenTK.Matrix3.CreateRotationZ(OpenTK.MathHelper.DegreesToRadians((float)element.Rotation));
					var matrixS = OpenTK.Matrix3.CreateScale(scale, scale, 1.0f);
					var matrixT2 = new OpenTK.Matrix3 { M31 = pt.X + (float)(element.TranslationX), M32 = pt.Y + (float)(element.TranslationY), M11 = 1.0f, M22 = 1.0f, M33 = 1.0f };

					var matrix = matrixT1 * matrixS * matrixR * matrixT2;

					_matrix?.Dispose();
					_matrix = null;
					_matrix = new Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.M31, matrix.M32);
					*/
				}
				control.Invalidate();
			});
		}
	}
}
