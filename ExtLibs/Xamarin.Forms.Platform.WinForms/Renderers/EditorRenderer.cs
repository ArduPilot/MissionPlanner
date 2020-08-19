using System;
using System.ComponentModel;
using WForms = System.Windows.Forms;
using WDrawing = System.Drawing;

namespace Xamarin.Forms.Platform.WinForms
{
	public class EditorRenderer : ViewRenderer<Editor, WForms.TextBox>
	{
		/*-----------------------------------------------------------------*/
		#region Event Handler

		protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
		{
			if (e.NewElement != null)
			{
				if (e.NewElement != null)
				{
					if (Control == null)
					{
						SetNativeControl(new WForms.TextBox());
					}

					Control.AutoSize = false;
					Control.Multiline = true;
					Control.ScrollBars = WForms.ScrollBars.Vertical;

					UpdateText();
					UpdateTextColor();
					UpdateFont();
				}
			}

			base.OnElementChanged(e);
		}

		protected override void OnNativeElementChanged(NativeElementChangedEventArgs<WForms.TextBox> e)
		{
			base.OnNativeElementChanged(e);
			if (e.OldControl != null)
			{
				e.OldControl.TextChanged -= OnTextChanged;
			}

			if (e.NewControl != null)
			{
				e.NewControl.TextChanged += OnTextChanged;
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Editor.TextProperty.PropertyName)
			{
				UpdateText();
			}
			else if (e.PropertyName == Editor.TextColorProperty.PropertyName)
			{
				UpdateTextColor();
			}
			else if (e.PropertyName == Editor.FontSizeProperty.PropertyName ||
				e.PropertyName == Editor.FontAttributesProperty.PropertyName)
			{
				UpdateFont();
			}

			base.OnElementPropertyChanged(sender, e);
		}

		void OnTextChanged(object sender, EventArgs e)
		{
			((IElementController)Element).SetValueFromRenderer(Editor.TextProperty, Control.Text);
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Internals

		void UpdateText()
		{
			UpdatePropertyHelper((element, control) => control.Text = element.Text);
		}

		void UpdateTextColor()
		{
			UpdatePropertyHelper((element, control) => control.ForeColor = element.TextColor.ToWindowsColor(WDrawing.SystemColors.ControlText));
		}

		void UpdateFont()
		{
			UpdatePropertyHelper((element, control) =>
				control.Font = new System.Drawing.Font(
					element.FontFamily,
					Math.Max((float)element.FontSize, 1.0f),
					element.FontAttributes.ToWindowsFontStyle()));
		}

		#endregion
	}
}
