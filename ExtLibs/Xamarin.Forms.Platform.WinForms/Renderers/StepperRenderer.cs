using System;
using System.ComponentModel;
using WForms = System.Windows.Forms;

namespace Xamarin.Forms.Platform.WinForms
{
	public class StepperRenderer : ViewRenderer<Stepper, WForms.NumericUpDown>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Stepper> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new WForms.NumericUpDown());
				}

				UpdateMinimum();
				UpdateMaximum();
				UpdateValue();
				UpdateIncrement();
			}

			base.OnElementChanged(e);
		}

		protected override void OnNativeElementChanged(NativeElementChangedEventArgs<WForms.NumericUpDown> e)
		{
			base.OnNativeElementChanged(e);
			if (e.OldControl != null)
			{
				e.OldControl.ValueChanged -= OnValueChanged;
			}

			if (e.NewControl != null)
			{
				e.NewControl.ValueChanged += OnValueChanged;
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Stepper.ValueProperty.PropertyName)
				UpdateValue();
			else if (e.PropertyName == Stepper.MinimumProperty.PropertyName)
				UpdateMinimum();
			else if (e.PropertyName == Stepper.MaximumProperty.PropertyName)
				UpdateMaximum();
			else if (e.PropertyName == Stepper.IncrementProperty.PropertyName)
				UpdateIncrement();

			base.OnElementPropertyChanged(sender, e);
		}

		void OnValueChanged(object sender, EventArgs e)
		{
			((IElementController)Element).SetValueFromRenderer(Stepper.ValueProperty, Control.Value);
		}

		void UpdateValue()
		{
			var value = (decimal)Element.Value;

			if (Control.Value != value)
				Control.Value = value;
		}

		void UpdateMinimum()
		{
			var minimum = (decimal)Element.Minimum;

			Control.Minimum = minimum;
		}

		void UpdateMaximum()
		{
			var maximum = (decimal)Element.Maximum;

			Control.Maximum = maximum;
		}

		void UpdateIncrement()
		{
			var increment = (decimal)Element.Increment;

			Control.Increment = increment;
		}
	}
}