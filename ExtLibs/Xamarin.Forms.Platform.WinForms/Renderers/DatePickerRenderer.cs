using System;
using System.ComponentModel;
using WForms = System.Windows.Forms;
using WDrawing = System.Drawing;

namespace Xamarin.Forms.Platform.WinForms
{
	public class DatePickerRenderer : ViewRenderer<DatePicker, WForms.DateTimePicker>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new WForms.DateTimePicker());
				}

				UpdateFormat();
				UpdateDate();
				UpdateMinimumDate();
				UpdateMaximumDate();
				UpdateTextColor();
				UpdateFont();
			}

			base.OnElementChanged(e);
		}

		protected override void OnNativeElementChanged(NativeElementChangedEventArgs<WForms.DateTimePicker> e)
		{
			base.OnNativeElementChanged(e);
			if (e.OldControl != null)
			{
				e.OldControl.ValueChanged -= DateTimePicker_OnValueChanged;
			}

			if (e.NewControl != null)
			{
				e.NewControl.ValueChanged += DateTimePicker_OnValueChanged;
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == DatePicker.DateProperty.PropertyName)
			{
				UpdateDate();
			}
			else if (e.PropertyName == DatePicker.FormatProperty.PropertyName)
			{
				UpdateFormat();
			}
			else if (e.PropertyName == DatePicker.MinimumDateProperty.PropertyName)
			{
				UpdateMinimumDate();
			}
			else if (e.PropertyName == DatePicker.MaximumDateProperty.PropertyName)
			{
				UpdateMaximumDate();
			}
			else if (e.PropertyName == DatePicker.TextColorProperty.PropertyName)
			{
				UpdateTextColor();
			}
			else if (e.PropertyName == Button.FontFamilyProperty.PropertyName ||
				e.PropertyName == Button.FontSizeProperty.PropertyName ||
				e.PropertyName == Button.FontAttributesProperty.PropertyName)
			{
				UpdateFont();
			}
		}

		void UpdateDate()
		{
			UpdatePropertyHelper((element, control) => control.Value = element.Date);
		}

		void UpdateFormat()
		{
            UpdatePropertyHelper((element, control) => { control.Format = WForms.DateTimePickerFormat.Custom; control.CustomFormat = element.Format; });
		}

		void UpdateMaximumDate()
		{
			UpdatePropertyHelper((element, control) => control.MaxDate = element.MaximumDate);
		}

		void UpdateMinimumDate()
		{
			UpdatePropertyHelper((element, control) => control.MinDate = element.MinimumDate);
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

		void DateTimePicker_OnValueChanged(object sender, EventArgs e)
		{
			var nativeElement = Control;
			if (nativeElement != null)
			{
				((IElementController)Element).SetValueFromRenderer(DatePicker.DateProperty, nativeElement.Value);
			}
		}
	}
}
