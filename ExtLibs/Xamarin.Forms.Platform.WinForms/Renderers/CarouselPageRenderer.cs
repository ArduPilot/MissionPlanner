using System;
using System.ComponentModel;
using WForms = System.Windows.Forms;
using Xamarin.Forms.Internals;
using System.Collections.Specialized;


namespace Xamarin.Forms.Platform.WinForms
{
	public class CarouselPageRenderer : MultiPageRenderer<CarouselPage, ContentPage, WFormsCarouselPage>
	{
		public override IVisualElementRenderer CreateChildRenderer(VisualElement element)
		{
			if (element is ContentPage)
			{
				return new CarouselInternalPageRenderer();
			}
			return base.CreateChildRenderer(element);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<CarouselPage> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new WFormsCarouselPage());
				}

			}

			base.OnElementChanged(e);
		}
	}
}
