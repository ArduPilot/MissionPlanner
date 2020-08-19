using System;
using System.ComponentModel;
using WForms = System.Windows.Forms;
using WDrawing = System.Drawing;

namespace Xamarin.Forms.Platform.WinForms
{
	public class ButtonRenderer : ViewRenderer<Button, WForms.Button>
	{
		/*-----------------------------------------------------------------*/
		#region Event Handler

		protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new WForms.Button());
				}

				UpdateText();
				UpdateTextColor();
				UpdateFont();
			}

			base.OnElementChanged(e);
		}

		protected override void OnNativeElementChanged(NativeElementChangedEventArgs<WForms.Button> e)
		{
			base.OnNativeElementChanged(e);
			if (e.OldControl != null)
			{
				e.OldControl.Click -= OnClick;
			}

			if (e.NewControl != null)
			{
				e.NewControl.Click += OnClick;
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Button.TextProperty.PropertyName)
			{
				UpdateText();
			}
			else if (e.PropertyName == Button.TextColorProperty.PropertyName)
			{
				UpdateTextColor();
			}
			else if (e.PropertyName == Button.FontFamilyProperty.PropertyName ||
				e.PropertyName == Button.FontSizeProperty.PropertyName ||
				e.PropertyName == Button.FontAttributesProperty.PropertyName)
			{
				UpdateFont();
			}

			base.OnElementPropertyChanged(sender, e);
		}

		void OnClick(object sender, EventArgs e)
		{
			((IButtonController)Element)?.SendReleased();
			((IButtonController)Element)?.SendClicked();
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
			{
				control.Font = new System.Drawing.Font(
					element.FontFamily,
					(float)element.FontSize,
					element.FontAttributes.ToWindowsFontStyle());
			});
		}

		#endregion
	}
}
