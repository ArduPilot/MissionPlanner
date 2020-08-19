using WForms = System.Windows.Forms;

namespace Xamarin.Forms.Platform.WinForms
{
	public class PageRenderer<TElement, TNativeElement> : VisualElementRenderer<TElement, TNativeElement>
		where TElement : Page
		where TNativeElement : WForms.Control
	{
		protected override void Appearing()
		{
			base.Appearing();
			Element?.SendAppearing();
		}

		protected override void Disappearing()
		{
			Element?.SendDisappearing();
			base.Disappearing();
		}
	}

	public class PageRenderer : PageRenderer<Page, WForms.Panel>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
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
	}
}
