using System;
using System.ComponentModel;
using WForms = System.Windows.Forms;

namespace Xamarin.Forms.Platform.WinForms
{
	public class SwitchRenderer : ViewRenderer<Switch, WForms.CheckBox>
	{
		/*-----------------------------------------------------------------*/
		#region Event Handler

		protected override void OnElementChanged(ElementChangedEventArgs<Switch> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new WForms.CheckBox());
				}

				UpdateToggle();
			}

			base.OnElementChanged(e);
		}

		protected override void OnNativeElementChanged(NativeElementChangedEventArgs<WForms.CheckBox> e)
		{
			base.OnNativeElementChanged(e);
			if (e.OldControl != null)
			{
				e.OldControl.CheckedChanged -= OnCheckedChanged;
			}

			if (e.NewControl != null)
			{
				e.NewControl.CheckedChanged += OnCheckedChanged;
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Switch.IsToggledProperty.PropertyName)
			{
				UpdateToggle();
			}
			base.OnElementPropertyChanged(sender, e);
		}

		void OnCheckedChanged(object sender, EventArgs e)
		{
			((IElementController)Element).SetValueFromRenderer(Switch.IsToggledProperty, Control.Checked);
		}


		#endregion

		/*-----------------------------------------------------------------*/
		#region Internals

		void UpdateToggle()
		{
			if (Control != null && Element != null)
			{
				Control.Checked = Element.IsToggled;
			}
		}

		#endregion
	}
}
