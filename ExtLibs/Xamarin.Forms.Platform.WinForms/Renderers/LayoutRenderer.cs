using WForms = System.Windows.Forms;

namespace Xamarin.Forms.Platform.WinForms
{
	public class LayoutRenderer : ViewRenderer<Layout, WForms.Panel>
	{
		/*-----------------------------------------------------------------*/
		#region Event Handler

		protected override void OnElementChanged(ElementChangedEventArgs<Layout> e)
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

		#endregion
	}
}