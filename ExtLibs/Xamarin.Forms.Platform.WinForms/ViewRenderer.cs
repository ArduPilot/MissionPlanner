using System.Windows.Forms;

namespace Xamarin.Forms.Platform.WinForms
{
	public class ViewRenderer<TElement, TNativeElement> :
		VisualElementRenderer<TElement, TNativeElement>
		where TElement : View
		where TNativeElement : Control
	{
		protected override void OnElementChanged(ElementChangedEventArgs<TElement> e)
		{
			if (e.NewElement != null)
			{
				UpdateBackgroundColor();
			}

			base.OnElementChanged(e);
		}

	}
}