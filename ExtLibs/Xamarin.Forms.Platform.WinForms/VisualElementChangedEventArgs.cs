using System;

namespace Xamarin.Forms.Platform.WinForms
{
    public class VisualElementChangedEventArgs : ElementChangedEventArgs<VisualElement>
	{
		public VisualElementChangedEventArgs(VisualElement oldElement, VisualElement newElement) : base(oldElement, newElement)
		{
		}
	}

	public class ElementChangedEventArgs<TElement> : EventArgs where TElement : Element
	{
		public ElementChangedEventArgs(TElement oldElement, TElement newElement)
		{
			OldElement = oldElement;
			NewElement = newElement;
		}

		public TElement NewElement { get; private set; }

		public TElement OldElement { get; private set; }
	}

	public class NativeElementChangedEventArgs<TNativeElement> : EventArgs
		where TNativeElement : System.Windows.Forms.Control
	{
		public NativeElementChangedEventArgs(TNativeElement oldControl, TNativeElement newControl)
		{
			OldControl = oldControl;
			NewControl = newControl;
		}

		public TNativeElement NewControl { get; private set; }

		public TNativeElement OldControl { get; private set; }
	}
}