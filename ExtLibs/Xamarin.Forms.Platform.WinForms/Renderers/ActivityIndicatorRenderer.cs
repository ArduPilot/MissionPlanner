using System;
using System.ComponentModel;
using WForms = System.Windows.Forms;

namespace Xamarin.Forms.Platform.WinForms
{
	public class ActivityIndicatorRenderer : ViewRenderer<ActivityIndicator, WForms.ProgressBar>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<ActivityIndicator> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new WForms.ProgressBar());
				}

				UpdateIsIndeterminate();
				UpdateColor();
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == ActivityIndicator.IsRunningProperty.PropertyName)
				UpdateIsIndeterminate();
			else if (e.PropertyName == ActivityIndicator.ColorProperty.PropertyName)
				UpdateColor();
		}

		void UpdateColor()
		{
			UpdatePropertyHelper((element, control) => control.ForeColor = element.Color.ToWindowsColor());
		}

		void UpdateIsIndeterminate()
		{
			UpdatePropertyHelper((element, control) =>
			{
				if (element.IsRunning)
				{
					control.Style = WForms.ProgressBarStyle.Marquee;
				}
				else
				{
					control.Style = WForms.ProgressBarStyle.Continuous;
					control.Value = 0;
				}
			});
		}
	}
}
