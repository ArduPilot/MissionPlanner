using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Forms.Internals;
using WDrawing = System.Drawing;
using WForms = System.Windows.Forms;

namespace Xamarin.Forms.Platform.WinForms
{
	public class ImageButtonRenderer : DrawingViewRenderer<ImageButton, WForms.Button>
	{
		WDrawing.Image _source = null;
		bool _mouseDown = false;
		bool _mouseEnter = false;
		bool _keydown = false;

		protected override async void OnElementChanged(ElementChangedEventArgs<ImageButton> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new WForms.Button());
				}

				await TryUpdateSource();
				UpdateAspect();
			}

			base.OnElementChanged(e);
		}

		protected override void OnNativeElementChanged(NativeElementChangedEventArgs<WForms.Button> e)
		{
			base.OnNativeElementChanged(e);
			if (e.OldControl != null)
			{
				e.OldControl.Click -= OnClick;
				e.OldControl.MouseDown -= OnMouseDown;
				e.OldControl.MouseUp -= OnMouseUp;
				e.OldControl.MouseEnter -= OnMouseEnter;
				e.OldControl.MouseLeave -= OnMouseLeave;
				e.OldControl.KeyDown -= OnKeyDown;
				e.OldControl.KeyUp -= OnKeyUp;
			}

			if (e.NewControl != null)
			{
				e.NewControl.Click += OnClick;
				e.NewControl.MouseDown += OnMouseDown;
				e.NewControl.MouseUp += OnMouseUp;
				e.NewControl.MouseEnter += OnMouseEnter;
				e.NewControl.MouseLeave += OnMouseLeave;
				e.NewControl.KeyDown += OnKeyDown;
				e.NewControl.KeyUp += OnKeyUp;
				_mouseDown = false;
				_mouseEnter = false;
				_keydown = false;
			}
		}

		protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == ImageButton.SourceProperty.PropertyName)
				await TryUpdateSource();
			else if (e.PropertyName == ImageButton.AspectProperty.PropertyName)
				UpdateAspect();

			base.OnElementPropertyChanged(sender, e);
		}

		void OnClick(object sender, EventArgs e)
		{
			((IButtonController)Element)?.SendReleased();
			((IButtonController)Element)?.SendClicked();
		}

		void OnMouseDown(object sender, WForms.MouseEventArgs e)
		{
			_mouseDown = true;
		}

		void OnMouseUp(object sender, WForms.MouseEventArgs e)
		{
			_mouseDown = false;
		}

		void OnMouseEnter(object sender, EventArgs e)
		{
			_mouseEnter = true;
		}

		void OnMouseLeave(object sender, EventArgs e)
		{
			_mouseEnter = false;
		}

		void OnKeyDown(object sender, WForms.KeyEventArgs e)
		{
			if (!_keydown)
			{
				_keydown = e.KeyCode == WForms.Keys.Space;
				if (_keydown)
				{
					Control?.Update();
				}
			}
		}

		void OnKeyUp(object sender, WForms.KeyEventArgs e)
		{
			if (_keydown)
			{
				_keydown = false;
				Control?.Update();
			}
		}

		protected override void OnPaint(object sender, WForms.PaintEventArgs e)
		{
			base.OnPaint(sender, e);
			if (_source == null)
			{
				return;
			}
			UpdatePropertyHelper((element, control) =>
			{
				var rect = control.ClientRectangle;
				var state = WForms.VisualStyles.PushButtonState.Default;
				if (control.Enabled)
				{
					if (_mouseDown || _keydown)
					{
						state = WForms.VisualStyles.PushButtonState.Pressed;
					}
					else if (_mouseEnter)
					{
						state = WForms.VisualStyles.PushButtonState.Hot;
					}
				}
				else
				{
					state = WForms.VisualStyles.PushButtonState.Disabled;
				}
				WForms.ButtonRenderer.DrawButton(e.Graphics, rect, control.Focused, state);
				if (_source.Width > 0 && _source.Height > 0)
				{
					switch (element.Aspect)
					{
						case Aspect.AspectFit:
							{
								var w = (float)rect.Width;
								var h = (float)(w * _source.Height / _source.Width);
								if (h > rect.Height)
								{
									h = (float)rect.Height;
									w = (float)(h * _source.Width / _source.Height);
								}
								e.Graphics.DrawImage(
									_source,
									rect.Left + (rect.Width - w) / 2,
									rect.Top + (rect.Height - h) / 2,
									w, h);
							}
							return;

						case Aspect.AspectFill:
							{
								var w = (float)_source.Width;
								var h = (float)(w * rect.Height / rect.Width);
								if (h > _source.Height)
								{
									h = (float)_source.Height;
									w = (float)(h * rect.Width / rect.Height);
								}
								e.Graphics.DrawImage(
									_source,
									new WDrawing.RectangleF(
										rect.Left, rect.Top,
										(float)rect.Width, (float)rect.Height),
									new WDrawing.RectangleF(
										(_source.Width - w) / 2,
										(_source.Height - h) / 2,
										w, h),
									WDrawing.GraphicsUnit.Pixel);
							}
							return;
					}
				}
				e.Graphics.DrawImage(
					_source,
					rect.Left, rect.Top,
					rect.Width, rect.Height);
			});
		}

		void UpdateAspect()
		{
			Control?.Invalidate();
		}

		protected virtual async Task TryUpdateSource()
		{
			try
			{
				await UpdateSource().ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				Log.Warning(nameof(ImageRenderer), "Error loading image: {0}", ex);
			}
			finally
			{
				Element.SetIsLoading(false);
			}
		}

		protected async Task UpdateSource()
		{
			var element = Element;
			if (element != null)
			{
				element.SetIsLoading(true);
				try
				{
					_source?.Dispose();
					_source = null;

					var source = Element.Source;
					IImageSourceHandler handler;
					if (source != null && (handler = Registrar.Registered.GetHandler<IImageSourceHandler>(source.GetType())) != null)
					{
						try
						{
							_source = await handler.LoadImageAsync(source);
						}
						catch (OperationCanceledException)
						{
						}

						RefreshImage();
					}
				}
				finally
				{
					element.SetIsLoading(false);
				}

				Control?.Invalidate();
			}

		}

		void RefreshImage()
		{
			((IVisualElementController)Element)?.InvalidateMeasure(InvalidationTrigger.RendererReady);
		}

	}
}
