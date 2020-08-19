using System;
using System.ComponentModel;
using WForms = System.Windows.Forms;

namespace Xamarin.Forms.Platform.WinForms
{
	public class TabbedInternalPageRenderer : PageRenderer<Page, WForms.TabPage>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new WForms.TabPage());
				}
				UpdateTitle();
			}
			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Page.TitleProperty.PropertyName)
				UpdateTitle();
		}

		protected override void OnNativeElementChanged(NativeElementChangedEventArgs<WForms.TabPage> e)
		{
			base.OnNativeElementChanged(e);
			if (e.OldControl != null)
			{
				e.OldControl.SizeChanged -= OnSizeChanged;
				e.OldControl.VisibleChanged -= OnVisibleChanged;
			}

			if (e.NewControl != null)
			{
				e.NewControl.SizeChanged += OnSizeChanged;
				e.NewControl.VisibleChanged += OnVisibleChanged;
			}
		}

		void UpdateTitle()
		{
			UpdatePropertyHelper((element, Control) =>
			{
				Control.Text = element.Title;
			});
		}

		void OnSizeChanged(object sender, System.EventArgs e)
		{
			UpdatePropertyHelper((element, control) =>
			{
				element.Layout(new Rectangle(0, 0, control.Width, control.Height));
			});
		}

		void OnVisibleChanged(object sender, System.EventArgs e)
		{
			UpdatePropertyHelper((element, control) =>
			{
				if (control.Visible)
				{
					//	The layout might be invalidated while Visible is false.
					element.Layout(new Rectangle(0, 0, control.Width, control.Height));
				}
			});
		}
	}
}
