using System;
using System.Windows.Forms;

namespace SkiaSharp.Views.Forms
{
	internal class SKTouchHandler
	{
		private Action<SKTouchEventArgs> onTouchAction;
		private Func<double, double, SKPoint> scalePixels;

		public SKTouchHandler(Action<SKTouchEventArgs> onTouchAction, Func<double, double, SKPoint> scalePixels)
		{
			this.onTouchAction = onTouchAction;
			this.scalePixels = scalePixels;
		}

		public void SetEnabled(Control view, bool enableTouchEvents)
		{
			if (view != null)
			{
				//view.PointerEntered -= OnPointerEntered;
				//view.PointerExited -= OnPointerExited;
				view.MouseDown -= OnPointerPressed;
				view.MouseMove -= OnPointerMoved;
				view.MouseUp -= OnPointerReleased;
				//view.PointerCanceled -= OnPointerCancelled;
				if (enableTouchEvents)
				{
					//view.PointerEntered += OnPointerEntered;
					//view.PointerExited += OnPointerExited;
					view.MouseDown += OnPointerPressed;
					view.MouseMove += OnPointerMoved;
					view.MouseUp += OnPointerReleased;
					//view.PointerCanceled += OnPointerCancelled;
				}
			}
		}

		public void Detach(Control view)
		{
			// clean the view
			SetEnabled(view, false);

			// remove references
			onTouchAction = null;
			scalePixels = null;
		}

		private void OnPointerEntered(object sender, MouseEventArgs args)
		{
			CommonHandler(sender, SKTouchAction.Entered, args);
		}

		private void OnPointerExited(object sender, MouseEventArgs args)
		{
			CommonHandler(sender, SKTouchAction.Exited, args);
		}

		private void OnPointerPressed(object sender, MouseEventArgs args)
		{
			CommonHandler(sender, SKTouchAction.Pressed, args);

			var view = sender as Control;
			//view.CapturePointer(args.Pointer);
		}

		private void OnPointerMoved(object sender, MouseEventArgs args)
		{
			CommonHandler(sender, SKTouchAction.Moved, args);
		}

		private void OnPointerReleased(object sender, MouseEventArgs args)
		{
			CommonHandler(sender, SKTouchAction.Released, args);
		}

		private void OnPointerCancelled(object sender, MouseEventArgs args)
		{
			CommonHandler(sender, SKTouchAction.Cancelled, args);
		}

		private bool CommonHandler(object sender, SKTouchAction touchActionType, MouseEventArgs evt)
		{
			if (onTouchAction == null || scalePixels == null)
				return false;

			var view = sender as Control;

			var id = 0L;// evt.Pointer.PointerId;

			var windowsPoint = evt.Location;
			var skPoint = new SKPoint((float)windowsPoint.X, (float)windowsPoint.Y);

			var mouse = GetMouseButton(evt.Button);
			var device = SKTouchDeviceType.Mouse;// GetTouchDevice(evt);

			var args = new SKTouchEventArgs(id, touchActionType, mouse, device, skPoint, false/*evt.Pointer.IsInContact*/);
			onTouchAction(args);
			return args.Handled;
		}
/*
		private static SKTouchDeviceType GetTouchDevice(PointerRoutedEventArgs evt)
		{
			var device = SKTouchDeviceType.Touch;
			switch (evt.Pointer.PointerDeviceType)
			{
				case PointerDeviceType.Pen:
					device = SKTouchDeviceType.Pen;
					break;
				case PointerDeviceType.Mouse:
					device = SKTouchDeviceType.Mouse;
					break;
				case PointerDeviceType.Touch:
					device = SKTouchDeviceType.Touch;
					break;
			}

			return device;
		}
*/
		private static SKMouseButton GetMouseButton(MouseButtons btns)
		{
			var mouse = SKMouseButton.Unknown;

			// this is mainly for touch
			if (btns.HasFlag(MouseButtons.Left))
			{
				mouse = SKMouseButton.Left;
			}
			else if (btns.HasFlag(MouseButtons.Middle))
			{
				mouse = SKMouseButton.Middle;
			}
			else if (btns.HasFlag(MouseButtons.Right))
			{
				mouse = SKMouseButton.Right;
			}

			return mouse;
		}
	}
}
