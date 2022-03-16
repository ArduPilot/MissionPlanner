using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinForms;
using Button = Xamarin.Forms.Button;
using ButtonRenderer = Xamarin.Forms.Platform.WinForms.ButtonRenderer;
using Label = Xamarin.Forms.Label;
using ProgressBar = Xamarin.Forms.ProgressBar;
using ProgressBarRenderer = Xamarin.Forms.Platform.WinForms.ProgressBarRenderer;


[assembly: Xamarin.Forms.Dependency(typeof(WinFormsSystemResourcesProvider))]

[assembly: ExportRenderer(typeof(Page), typeof(PageRenderer))]
[assembly: ExportRenderer(typeof(Label), typeof(LabelRenderer))]
[assembly: ExportRenderer(typeof(Layout), typeof(LayoutRenderer))]
[assembly: ExportRenderer(typeof(Button), typeof(ButtonRenderer))]
[assembly: ExportRenderer(typeof(Slider), typeof(SliderRenderer))]
[assembly: ExportRenderer(typeof(Stepper), typeof(StepperRenderer))]
[assembly: ExportRenderer(typeof(Switch), typeof(SwitchRenderer))]
[assembly: ExportRenderer(typeof(Editor), typeof(EditorRenderer))]
[assembly: ExportRenderer(typeof(Entry), typeof(EntryRenderer))]
[assembly: ExportRenderer(typeof(Picker), typeof(PickerRenderer))]
[assembly: ExportRenderer(typeof(ActivityIndicator), typeof(ActivityIndicatorRenderer))]
[assembly: ExportRenderer(typeof(BoxView), typeof(BoxViewRenderer))]
[assembly: ExportRenderer(typeof(DatePicker), typeof(DatePickerRenderer))]
[assembly: ExportRenderer(typeof(Frame), typeof(FrameRenderer))]
[assembly: ExportRenderer(typeof(Image), typeof(ImageRenderer))]
[assembly: ExportRenderer(typeof(ImageButton), typeof(ImageButtonRenderer))]
[assembly: ExportRenderer(typeof(ProgressBar), typeof(ProgressBarRenderer))]
[assembly: ExportRenderer(typeof(SearchBar), typeof(SearchBarRenderer))]
[assembly: ExportRenderer(typeof(ScrollView), typeof(ScrollViewRenderer))]
[assembly: ExportRenderer(typeof(TabbedPage), typeof(TabbedPageRenderer))]
[assembly: ExportRenderer(typeof(CarouselPage), typeof(CarouselPageRenderer))]
#if GL
[assembly: ExportRenderer(typeof(OpenGLView), typeof(OpenGLViewRenderer))]
#endif
[assembly: ExportRenderer(typeof(WebView), typeof(WebViewRenderer))]

[assembly: ExportImageSourceHandler(typeof(FileImageSource), typeof(FileImageSourceHandler))]
[assembly: ExportImageSourceHandler(typeof(StreamImageSource), typeof(StreamImageSourceHandler))]
[assembly: ExportImageSourceHandler(typeof(UriImageSource), typeof(UriImageSourceHandler))]



namespace Xamarin.Forms.Platform.WinForms
{
	public class Platform : INavigation, IDisposable
#pragma warning disable CS0618
		, IPlatform
#pragma warning restore
	{
		Rectangle _bounds = new Rectangle();
		readonly Form _form;
		readonly VisualElementRendererCollection _children = new VisualElementRendererCollection();
		Page _currentPage;
		readonly NavigationModel _navModel = new NavigationModel();

		#region Constructor / Dispose

		internal Platform(Form form)
		{
			_form = form;
			_children.ParentNativeElement = form;
			_form.SizeChanged += OnRendererSizeChanged;
			UpdateBounds();
		}

		public void Dispose()
		{
		}

		#endregion

		internal static readonly BindableProperty RendererProperty = BindableProperty.CreateAttached("Renderer",
			typeof(IVisualElementRenderer), typeof(Platform), default(IVisualElementRenderer));

		public static IVisualElementRenderer GetRenderer(VisualElement element)
		{
			return (IVisualElementRenderer)element.GetValue(RendererProperty);
		}

		public static void SetRenderer(VisualElement element, IVisualElementRenderer value)
		{
			element.SetValue(RendererProperty, value);
			element.IsPlatformEnabled = value != null;
		}

		public static IVisualElementRenderer CreateRenderer(VisualElement element, IVisualElementRenderer parent)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			IVisualElementRenderer renderer = parent?.CreateChildRenderer(element);

			if (renderer == null)
            {
				// a page is not a view
               /* if (element is Page)
                    renderer = new PageRenderer<Page, Control>();
                else*/
                    renderer = Registrar.Registered.GetHandlerForObject<IVisualElementRenderer>(element) ??
                               new DefaultRenderer();
            }
			renderer.SetElement(element);
			return renderer;
		}

		public static EventHandler BlockReenter(EventHandler h)
		{
			bool entered = false;
			return (s, e) =>
			{
				if (!entered)
				{
					entered = true;
					try
					{
						h(s, e);
					}
					finally
					{
						entered = false;
					}
				}
			};
		}

		public static EventHandler<T> BlockReenter<T>(EventHandler<T> h)
			where T : EventArgs
		{
			bool entered = false;
			return (s, e) =>
			{
				if (!entered)
				{
					entered = true;
					try
					{
						h(s, e);
					}
					finally
					{
						entered = false;
					}
				}
			};
		}

		public static System.Drawing.ContentAlignment ToWindowsContentAlignment(TextAlignment horizonatal, TextAlignment vertical)
		{
			switch (horizonatal)
			{
				case TextAlignment.Start:
					switch (vertical)
					{
						case TextAlignment.Start: return System.Drawing.ContentAlignment.TopLeft;
						case TextAlignment.Center: return System.Drawing.ContentAlignment.MiddleLeft;
						case TextAlignment.End: return System.Drawing.ContentAlignment.BottomLeft;
					}
					break;

				case TextAlignment.Center:
					switch (vertical)
					{
						case TextAlignment.Start: return System.Drawing.ContentAlignment.TopCenter;
						case TextAlignment.Center: return System.Drawing.ContentAlignment.MiddleCenter;
						case TextAlignment.End: return System.Drawing.ContentAlignment.BottomCenter;
					}
					break;

				case TextAlignment.End:
					switch (vertical)
					{
						case TextAlignment.Start: return System.Drawing.ContentAlignment.TopRight;
						case TextAlignment.Center: return System.Drawing.ContentAlignment.MiddleRight;
						case TextAlignment.End: return System.Drawing.ContentAlignment.BottomRight;
					}
					break;
			}
			return System.Drawing.ContentAlignment.MiddleCenter;
		}


		internal void SetPage(Page newRoot)
		{
			if (newRoot == null)
				throw new ArgumentNullException(nameof(newRoot));

			_navModel.Clear();

			_navModel.Push(newRoot, null);
			SetCurrent(newRoot, true);
			Application.Current.NavigationProxy.Inner = this;
		}

		void OnRendererSizeChanged(object sender, EventArgs e)
		{
			UpdateBounds();
			UpdatePageSizes();
		}

		/*async*/ void SetCurrent(Page newPage, bool popping = false, Action completedCallback = null)
		{
			if (newPage == _currentPage)
				return;

			newPage.Platform = this;

			if (_currentPage != null)
			{
				Page previousPage = _currentPage;
				IVisualElementRenderer previousRenderer = GetRenderer(previousPage);
				_children.Remove(previousRenderer);

				if (popping)
					previousPage.Cleanup();
			}

			IVisualElementRenderer pageRenderer = newPage.GetOrCreateRenderer(null);
			_children.Add(pageRenderer);

			newPage.Layout(ContainerBounds);

			var size = _form.ClientSize;
			pageRenderer.NativeElement.Width = size.Width;
			pageRenderer.NativeElement.Height = size.Height;

			completedCallback?.Invoke();

			_currentPage = newPage;

			//UpdateToolbarTracker();
			//await UpdateToolbarItems();
		}

		internal Rectangle ContainerBounds => _bounds;

		internal void UpdatePageSizes()
		{
			Rectangle bounds = ContainerBounds;
			var size = _form.ClientSize;
			if (bounds.IsEmpty)
				return;
			foreach (Page root in _navModel.Roots)
			{
				root.Layout(bounds);
				IVisualElementRenderer renderer = GetRenderer(root);
				if (renderer != null)
				{
					renderer.NativeElement.Width = size.Width;
					renderer.NativeElement.Height = size.Height;
				}
			}
		}

		void UpdateBounds()
		{
			_bounds = _form.ClientRectangle.ToXamarinRectangle();
		}

		#region IPlatform

		public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
		{
			return GetNativeSizeInternal(view, widthConstraint, heightConstraint);
		}

		internal static SizeRequest GetNativeSizeInternal(VisualElement view, double widthConstraint, double heightConstraint)
		{
			//	暫定
			var viewRenderer = GetRenderer(view);
			if (viewRenderer == null)
			{
				return new SizeRequest();
			}

			widthConstraint = widthConstraint < 0 ? double.PositiveInfinity : widthConstraint;
			heightConstraint = heightConstraint < 0 ? double.PositiveInfinity : heightConstraint;
			var rawResult = viewRenderer.GetDesiredSize(widthConstraint, heightConstraint);
			if (rawResult.Minimum == Size.Zero)
			{
				rawResult.Minimum = rawResult.Request;
			}
			return rawResult;
		}

		#endregion

		#region INavigation

		public IReadOnlyList<Page> ModalStack
        {
            get
            {
				return _navModel.Tree.Last();
			}
        }

		public IReadOnlyList<Page> NavigationStack
        {
            get
            {
				return _navModel.Modals.ToList();
			}
        }

		public void InsertPageBefore(Page page, Page before)
		{
			throw new NotImplementedException();
		}

		public Task<Page> PopAsync()
		{
			throw new NotImplementedException();
		}

		public Task<Page> PopAsync(bool animated)
		{
			throw new NotImplementedException();
		}

		public Task<Page> PopModalAsync()
		{
			throw new NotImplementedException();
		}

		public Task<Page> PopModalAsync(bool animated)
		{
			throw new NotImplementedException();
		}

		public Task PopToRootAsync()
		{
			throw new NotImplementedException();
		}

		public Task PopToRootAsync(bool animated)
		{
			throw new NotImplementedException();
		}

		public Task PushAsync(Page page)
		{
			throw new NotImplementedException();
		}

		public Task PushAsync(Page page, bool animated)
		{
			throw new NotImplementedException();
		}

		public Task PushModalAsync(Page page)
		{
			if (page == null)
				throw new ArgumentNullException(nameof(page));

			var tcs = new TaskCompletionSource<bool>();
			_navModel.PushModal(page);
			//SetCurrent(page, completedCallback: () => tcs.SetResult(true));
			return tcs.Task;
		}

		public Task PushModalAsync(Page page, bool animated)
		{
			var tcs = new TaskCompletionSource<Page>();
			Page result = _navModel.PopModal();
			//SetCurrent(_navModel.CurrentPage, true, () => tcs.SetResult(result));
			return tcs.Task;
		}

		public void RemovePage(Page page)
		{
			throw new NotImplementedException();
		}

		#endregion


	}
}
