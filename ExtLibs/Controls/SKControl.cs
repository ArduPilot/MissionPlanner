using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace SkiaSharp.Views.Desktop
{
	[DefaultEvent("PaintSurface")]
	[DefaultProperty("Name")]
	public class SKControl : Control
	{
		private readonly bool designMode;

		private Bitmap bitmap;

		public SKControl()
		{
			DoubleBuffered = false;
			SetStyle(ControlStyles.ResizeRedraw, true);

            designMode = DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;
		}

		[Bindable(false)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public SKSize CanvasSize => Bitmap == null ? SKSize.Empty : new SKSize(Bitmap.Width, Bitmap.Height);

        public Bitmap Bitmap
        {
            get
            {
                return bitmap;
            }
        }

        [Category("Appearance")]
		public event EventHandler<SKPaintSurfaceEventArgs> PaintSurface;

		protected override void OnPaint(PaintEventArgs e)
		{
			if (designMode)
				return;

			base.OnPaint(e);

			Draw();

			e.Graphics.DrawImage(Bitmap, 0, 0);
		}

		public void Draw()
        {
			// get the bitmap
			var info = CreateBitmap();

			if (info.Width == 0 || info.Height == 0)
				return;

			var data = Bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, Bitmap.PixelFormat);

			// create the surface
			using (var surface = SKSurface.Create(info, data.Scan0, data.Stride))
			{
				// start drawing
				OnPaintSurface(new SKPaintSurfaceEventArgs(surface, info));

				surface.Canvas.Flush();
			}

			// write the bitmap to the graphics
			Bitmap.UnlockBits(data);
		}

		protected virtual void OnPaintSurface(SKPaintSurfaceEventArgs e)
		{
			// invoke the event
			PaintSurface?.Invoke(this, e);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			FreeBitmap();
		}

		private SKImageInfo CreateBitmap()
		{
			var info = new SKImageInfo(Width, Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);

			if (Bitmap == null || Bitmap.Width != info.Width || Bitmap.Height != info.Height)
			{
				FreeBitmap();

				if (info.Width != 0 && info.Height != 0)
					bitmap = new Bitmap(info.Width, info.Height, PixelFormat.Format32bppPArgb);
            }

			return info;
		}

		private void FreeBitmap()
		{
			if (Bitmap != null)
			{
				Bitmap.Dispose();
				bitmap = null;
			}
		}
	}
}
