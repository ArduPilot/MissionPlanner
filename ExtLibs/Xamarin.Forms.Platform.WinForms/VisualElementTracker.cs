using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Xamarin.Forms.Platform.WinForms
{
	public class VisualElementTracker<TElement, TNativeElement> : IDisposable
		where TElement : VisualElement
		where TNativeElement : Control
	{
		TNativeElement _control;
		TElement _element;

		bool _invalidateArrangeNeeded;

		public VisualElementTracker()
		{
		}

		#region IDisposable Support

		bool disposedValue = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion



		public TNativeElement Control
		{
			get { return _control; }
			set
			{
				if (_control == value)
					return;

				if (_control != null)
				{
					/*
					_control.Tapped -= HandleTapped;
					_control.DoubleTapped -= HandleDoubleTapped;
					*/
				}

				_control = value;
				UpdateNativeControl();
			}
		}

		public TElement Element
		{
			get { return _element; }
			set
			{
				if (_element == value)
					return;

				if (_element != null)
				{
					_element.BatchCommitted -= OnRedrawNeeded;
					_element.PropertyChanged -= OnPropertyChanged;
				}

				_element = value;

				if (_element != null)
				{
					_element.BatchCommitted += OnRedrawNeeded;
					_element.PropertyChanged += OnPropertyChanged;
				}

				UpdateNativeControl();
			}
		}

		public event EventHandler Updated;


		protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (Element.Batched)
			{
				if (e.PropertyName == VisualElement.XProperty.PropertyName || e.PropertyName == VisualElement.YProperty.PropertyName || e.PropertyName == VisualElement.WidthProperty.PropertyName ||
					e.PropertyName == VisualElement.HeightProperty.PropertyName)
				{
					_invalidateArrangeNeeded = true;
				}
				return;
			}

			if (e.PropertyName == VisualElement.XProperty.PropertyName || e.PropertyName == VisualElement.YProperty.PropertyName || e.PropertyName == VisualElement.WidthProperty.PropertyName ||
				e.PropertyName == VisualElement.HeightProperty.PropertyName)
			{
				MaybeInvalidate();
			}
			else if (e.PropertyName == VisualElement.AnchorXProperty.PropertyName || e.PropertyName == VisualElement.AnchorYProperty.PropertyName)
			{
				UpdateScaleAndRotation(Element, Control);
			}
			else if (e.PropertyName == VisualElement.ScaleProperty.PropertyName)
			{
				UpdateScaleAndRotation(Element, Control);
			}
			else if (e.PropertyName == VisualElement.TranslationXProperty.PropertyName || e.PropertyName == VisualElement.TranslationYProperty.PropertyName ||
					 e.PropertyName == VisualElement.RotationProperty.PropertyName || e.PropertyName == VisualElement.RotationXProperty.PropertyName || e.PropertyName == VisualElement.RotationYProperty.PropertyName)
			{
				UpdateRotation(Element, Control);
			}
			else if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName)
			{
				UpdateVisibility(Element, Control);
			}
			else if (e.PropertyName == VisualElement.OpacityProperty.PropertyName)
			{
				UpdateOpacity(Element, Control);
			}
			else if (e.PropertyName == VisualElement.InputTransparentProperty.PropertyName)
			{
				UpdateInputTransparent(Element, Control);
			}
		}

		protected virtual void UpdateNativeControl()
		{
			var element = Element;
			var control = Control;
			if (element == null || control == null)
				return;

			UpdateVisibility(element, control);
			UpdateOpacity(element, control);
			UpdateScaleAndRotation(element, control);
			UpdateInputTransparent(element, control);

			if (_invalidateArrangeNeeded)
			{
				MaybeInvalidate();
			}
			_invalidateArrangeNeeded = false;

			OnUpdated();
		}

		void OnUpdated()
		{
			Updated?.Invoke(this, EventArgs.Empty);
		}

		void OnRedrawNeeded(object sender, EventArgs e)
		{
			UpdateNativeControl();
		}

		void MaybeInvalidate()
		{
			if (Element.IsInNativeLayout)
				return;

			UpdatePositionSizeAnchor(Element, Control);
			//var parent = Control.Parent;
			//parent?.InvalidateMeasure();
			//Container.InvalidateMeasure();
		}

		static void UpdatePositionSizeAnchor(VisualElement view, Control control)
		{
			control.SetBounds(
				(int)view.X,
				(int)view.Y,
				Math.Max((int)view.Width, 0),
				Math.Max((int)view.Height, 0),
				BoundsSpecified.All);
		}

		static void UpdateInputTransparent(VisualElement view, Control control)
		{
			control.Enabled = view.IsEnabled && !view.InputTransparent;
		}

		static void UpdateOpacity(VisualElement view, Control control)
		{
			//control.Opacity = view.Opacity;
		}

		static void UpdateRotation(VisualElement view, Control control)
		{
			/*
			double anchorX = view.AnchorX;
			double anchorY = view.AnchorY;
			double rotationX = view.RotationX;
			double rotationY = view.RotationY;
			double rotation = view.Rotation;
			double translationX = view.TranslationX;
			double translationY = view.TranslationY;
			double scale = view.Scale;

			if (rotationX % 360 == 0 && rotationY % 360 == 0 && rotation % 360 == 0 && translationX == 0 && translationY == 0 && scale == 1)
			{
				control.Projection = null;
			}
			else
			{
				control.Projection = new PlaneProjection
				{
					CenterOfRotationX = anchorX,
					CenterOfRotationY = anchorY,
					GlobalOffsetX = scale == 0 ? 0 : translationX / scale,
					GlobalOffsetY = scale == 0 ? 0 : translationY / scale,
					RotationX = -rotationX,
					RotationY = -rotationY,
					RotationZ = -rotation
				};
			}
			*/
		}

		static void UpdateScaleAndRotation(VisualElement view, Control control)
		{
			double anchorX = view.AnchorX;
			double anchorY = view.AnchorY;
			double scale = view.Scale;
			//control.RenderTransformOrigin = new Windows.Foundation.Point(anchorX, anchorY);
			//control.RenderTransform = new ScaleTransform { ScaleX = scale, ScaleY = scale };

			UpdateRotation(view, control);
		}

		static void UpdateVisibility(VisualElement view, Control control)
		{
			control.Visible = view.IsVisible;
		}

	}
}
