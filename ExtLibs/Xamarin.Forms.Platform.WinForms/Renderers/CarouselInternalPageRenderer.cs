using System;
using System.ComponentModel;
using WForms = System.Windows.Forms;

namespace Xamarin.Forms.Platform.WinForms
{
	public class CarouselInternalPageRenderer : PageRenderer<ContentPage, WForms.Panel>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<ContentPage> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new WForms.Panel());
				}
			}
			base.OnElementChanged(e);
		}

		protected override void OnNativeElementChanged(NativeElementChangedEventArgs<WForms.Panel> e)
		{
			base.OnNativeElementChanged(e);
			if (e.OldControl != null)
			{
				e.OldControl.SizeChanged -= OnSizeChanged;
			}

			if (e.NewControl != null)
			{
				e.NewControl.SizeChanged += OnSizeChanged;
			}
		}

		void OnSizeChanged(object sender, System.EventArgs e)
		{
			UpdatePropertyHelper((element, control) =>
			{
				element.Layout(new Rectangle(0, 0, control.Parent.Width, control.Parent.Height));
			});
		}
	}
}
