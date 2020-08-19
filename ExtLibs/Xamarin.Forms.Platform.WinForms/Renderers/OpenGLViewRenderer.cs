using OpenTK;
using OpenTK.Graphics;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Xamarin.Forms.Platform.WinForms
{
	public class OpenGLViewRenderer : ViewRenderer<OpenGLView, GLControl>
	{
		private Timer _timer;
		private Action<Rectangle> _action;
		private bool _hasRenderLoop;
		private bool _disposed;

		public Action<Rectangle> Action
		{
			get { return _action; }
			set { _action = value; }
		}

		public bool HasRenderLoop
		{
			get { return _hasRenderLoop; }
			set { _hasRenderLoop = value; }
		}

		protected override void Dispose(bool disposing)
		{
			if (!_disposed && disposing)
			{
				_disposed = true;

				if (Element != null)
					((IOpenGlViewController)Element).DisplayRequested -= Render;

				var glControl = Control;
				if (glControl != null)
					glControl.Paint -= OnPaint;

				if (_timer != null)
					_timer.Tick -= OnTick;
			}

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<OpenGLView> e)
		{
			if (e.OldElement != null)
				((IOpenGlViewController)e.OldElement).DisplayRequested -= Render;

			if (e.NewElement != null)
			{
				var glControl = new GLControl(new GraphicsMode(32, 24), 2, 0, GraphicsContextFlags.Default);
				glControl.MakeCurrent();
				glControl.Dock = DockStyle.None;

				SetNativeControl(glControl);

				_timer = new Timer();
				_timer.Interval = 16;
				_timer.Tick += OnTick;
				_timer.Start();

				((IOpenGlViewController)e.NewElement).DisplayRequested += Render;

				SetRenderMode();
				SetupRenderAction();
			}

			base.OnElementChanged(e);
		}

		protected override void OnNativeElementChanged(NativeElementChangedEventArgs<GLControl> e)
		{
			base.OnNativeElementChanged(e);
			if (e.OldControl != null)
			{
				e.OldControl.Paint -= OnPaint;
			}

			if (e.NewControl != null)
			{
				e.NewControl.Paint += OnPaint;
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == OpenGLView.HasRenderLoopProperty.PropertyName)
			{
				SetRenderMode();
				SetupRenderAction();
			}
		}

		public void Render(object sender, EventArgs eventArgs)
		{
			if (HasRenderLoop)
				return;

			SetupRenderAction();
		}

		private void SetRenderMode()
		{
			HasRenderLoop = Element.HasRenderLoop;
		}

		private void SetupRenderAction()
		{
			var model = Element;
			var onDisplay = model.OnDisplay;

			Action = onDisplay;
		}

		private void OnPaint(object sender, PaintEventArgs e)
		{
			var glControl = Control;

			if (glControl == null)
			{
				return;
			}

			glControl.MakeCurrent();
			Action.Invoke(new Rectangle(0, 0, glControl.Width, glControl.Height));
			glControl.SwapBuffers();
		}

		private void OnTick(object sender, EventArgs e)
		{
			if (!HasRenderLoop)
				return;

			Control?.Invalidate();
		}
	}
}
