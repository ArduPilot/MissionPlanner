using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using System.Threading;

using WDrawing = System.Drawing;
using WForms = System.Windows.Forms;
using System.IO;

namespace Xamarin.Forms.Platform.WinForms
{
	public class ImageRenderer : DrawingViewRenderer<Image, WForms.Control>
	{
		WDrawing.Image _source = null;

		protected override async void OnElementChanged(ElementChangedEventArgs<Image> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new WForms.ContainerControl());
				}

				await TryUpdateSource();
				UpdateAspect();
			}

			base.OnElementChanged(e);
		}

		protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Image.SourceProperty.PropertyName)
				await TryUpdateSource();
			else if (e.PropertyName == Image.AspectProperty.PropertyName)
				UpdateAspect();
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
				if (_source.Width > 0 && _source.Height > 0)
				{
					switch (element.Aspect)
					{
						case Aspect.AspectFit:
							{
								var w = (float)control.Width;
								var h = (float)(w * _source.Height / _source.Width);
								if (h > control.Height)
								{
									h = (float)control.Height;
									w = (float)(h * _source.Width / _source.Height);
								}
								e.Graphics.DrawImage(
									_source,
									(control.Width - w) / 2,
									(control.Height - h) / 2,
									w, h);
							}
							return;

						case Aspect.AspectFill:
							{
								var w = (float)_source.Width;
								var h = (float)(w * control.Height / control.Width);
								if (h > _source.Height)
								{
									h = (float)_source.Height;
									w = (float)(h * control.Width / control.Height);
								}
								e.Graphics.DrawImage(
									_source,
									new WDrawing.RectangleF(
										0, 0,
										(float)control.Width, (float)control.Height),
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
					0, 0,
					control.Width, control.Height);
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

	public interface IImageSourceHandler : IRegisterable
	{
		Task<WDrawing.Image> LoadImageAsync(ImageSource imagesoure, CancellationToken cancelationToken = default(CancellationToken));
	}

	public sealed class FileImageSourceHandler : IImageSourceHandler
	{
		public Task<WDrawing.Image> LoadImageAsync(ImageSource imagesoure, CancellationToken cancelationToken = new CancellationToken())
		{
			WDrawing.Image image = null;
			FileImageSource filesource = imagesoure as FileImageSource;
			if (filesource != null)
			{
				string file = filesource.File;
				image = new WDrawing.Bitmap(file);
			}
			return Task.FromResult(image);
		}
	}

	public sealed class StreamImageSourceHandler : IImageSourceHandler
	{
		public async Task<WDrawing.Image> LoadImageAsync(ImageSource imagesource, CancellationToken cancelationToken = new CancellationToken())
		{
			WDrawing.Image bitmapimage = null;
			StreamImageSource streamsource = imagesource as StreamImageSource;
			if (streamsource != null && streamsource.Stream != null)
			{
				using (Stream stream = await ((IStreamImageSource)streamsource).GetStreamAsync(cancelationToken))
				{
					bitmapimage = new WDrawing.Bitmap(stream);
				}
			}
			return bitmapimage;
		}
	}

	public sealed class UriImageSourceHandler : IImageSourceHandler
	{
		public async Task<WDrawing.Image> LoadImageAsync(ImageSource imagesoure, CancellationToken cancelationToken = new CancellationToken())
		{
			WDrawing.Image bitmapimage = null;
			var imageLoader = imagesoure as UriImageSource;
			if (imageLoader?.Uri != null)
			{
				using (var http = new System.Net.Http.HttpClient())
				{
					using (var stream = await http.GetStreamAsync(imageLoader.Uri))
					{
						bitmapimage = new WDrawing.Bitmap(stream);
					}
				}
			}
			return bitmapimage;
		}
	}

}
