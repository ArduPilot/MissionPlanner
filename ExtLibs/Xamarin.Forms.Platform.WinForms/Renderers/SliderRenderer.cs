using System;
using System.ComponentModel;
using WForms = System.Windows.Forms;

namespace Xamarin.Forms.Platform.WinForms
{
	public class SliderRenderer : ViewRenderer<Slider, WForms.TrackBar>
	{
		public static readonly int Scale = 100;

		/*-----------------------------------------------------------------*/
		#region Event Handler

		protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new WForms.TrackBar());

				}

				Control.TickFrequency = 0;

                UpdateMinimum();
                UpdateMaximum();
                UpdateValue();				
			}

			base.OnElementChanged(e);
		}

		protected override void OnNativeElementChanged(NativeElementChangedEventArgs<WForms.TrackBar> e)
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
			if (e.PropertyName == Slider.ValueProperty.PropertyName)
				UpdateValue();
			else if (e.PropertyName == Slider.MinimumProperty.PropertyName)
				UpdateMinimum();
			else if (e.PropertyName == Slider.MaximumProperty.PropertyName)
				UpdateMaximum();
			base.OnElementPropertyChanged(sender, e);
		}

		void OnValueChanged(object sender, EventArgs e)
		{
			((IElementController)Element).SetValueFromRenderer(Slider.ValueProperty, (double)Control.Value / Scale);
		}


		#endregion

		/*-----------------------------------------------------------------*/
		#region Internals

		void UpdateValue()
		{
			UpdatePropertyHelper((element, control) => control.Value = (int)(element.Value * Scale));
		}

		void UpdateMinimum()
		{
			UpdatePropertyHelper((element, control) => control.Minimum = (int)(element.Minimum * Scale));
		}

		void UpdateMaximum()
		{
			UpdatePropertyHelper((element, control) => control.Maximum = (int)(element.Maximum * Scale));
		}

		#endregion
	}
}
