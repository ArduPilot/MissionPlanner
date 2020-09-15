using System;
using System.ComponentModel;
using WForms = System.Windows.Forms;
using WDrawing = System.Drawing;

namespace Xamarin.Forms.Platform.WinForms
{
	public class LabelRenderer : ViewRenderer<Label, WForms.Label>
	{
		/*-----------------------------------------------------------------*/
		#region Event Handler

		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new WForms.Label());
				}

				UpdateText();
				UpdateTextColor();
				UpdateAlign();
				UpdateFont();
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Label.TextProperty.PropertyName || e.PropertyName == Label.FormattedTextProperty.PropertyName)
			{
				UpdateText();
			}
			else if (e.PropertyName == Label.TextColorProperty.PropertyName)
			{
				UpdateTextColor();
			}
			else if (e.PropertyName == Label.HorizontalTextAlignmentProperty.PropertyName ||
				e.PropertyName == Label.VerticalTextAlignmentProperty.PropertyName)
			{
				UpdateAlign();
			}
			else if (e.PropertyName == Label.FontFamilyProperty.PropertyName ||
				e.PropertyName == Label.FontSizeProperty.PropertyName ||
				e.PropertyName == Label.FontAttributesProperty.PropertyName)
			{
				UpdateFont();
			}

			base.OnElementPropertyChanged(sender, e);
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

		void UpdateAlign()
		{
			UpdatePropertyHelper((element, control) => control.TextAlign =
				Platform.ToWindowsContentAlignment(
					element.HorizontalTextAlignment, element.VerticalTextAlignment));
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
