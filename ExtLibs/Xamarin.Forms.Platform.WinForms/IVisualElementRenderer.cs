using System;
using System.Windows.Forms;

namespace Xamarin.Forms.Platform.WinForms
{
	public interface IVisualElementRenderer : IRegisterable, IDisposable
	{
		VisualElementRendererCollection Children { get; }

		VisualElement Element { get; }

		Control NativeElement { get; }

		event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint);

		void SetElement(VisualElement element);

		IVisualElementRenderer CreateChildRenderer(VisualElement element);
	}
}