using System;
using System.ComponentModel;
using WForms = System.Windows.Forms;
using WDrawing = System.Drawing;

namespace Xamarin.Forms.Platform.WinForms
{
	public class SearchBarRenderer : ViewRenderer<SearchBar, WFormsSearchBar>
	{
		const string DefaultPlaceholder = "Search";

		protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new WFormsSearchBar());
				}

				UpdateText();
				UpdatePlaceholder();
				UpdateAlignment();
				UpdateFont();
				UpdatePlaceholderColor();
				UpdateTextColor();
			}

			base.OnElementChanged(e);
		}

		protected override void OnNativeElementChanged(NativeElementChangedEventArgs<WFormsSearchBar> e)
		{
			base.OnNativeElementChanged(e);
			if (e.OldControl != null)
			{
				e.OldControl.SearchTextChanged -= OnSearchTextChanged;
				e.OldControl.SearchButtonClick -= OnSearchButtonClick;
				e.OldControl.CancelButtonClick -= OnCancelButtonClick;
			}

			if (e.NewControl != null)
			{
				e.NewControl.SearchTextChanged += OnSearchTextChanged;
				e.NewControl.SearchButtonClick += OnSearchButtonClick;
				e.NewControl.CancelButtonClick += OnCancelButtonClick;
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == SearchBar.TextProperty.PropertyName)
				UpdateText();
			else if (e.PropertyName == SearchBar.PlaceholderProperty.PropertyName)
				UpdatePlaceholder();
			else if (e.PropertyName == SearchBar.FontAttributesProperty.PropertyName ||
					 e.PropertyName == SearchBar.FontFamilyProperty.PropertyName ||
					 e.PropertyName == SearchBar.FontSizeProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == SearchBar.HorizontalTextAlignmentProperty.PropertyName)
				UpdateAlignment();
			else if (e.PropertyName == SearchBar.PlaceholderColorProperty.PropertyName)
				UpdatePlaceholderColor();
			else if (e.PropertyName == SearchBar.TextColorProperty.PropertyName)
				UpdateTextColor();
		}

		void OnSearchButtonClick(object sender, EventArgs e)
		{
			Element?.OnSearchButtonPressed();
		}

		void OnCancelButtonClick(object sender, EventArgs e)
		{
			var element = Element;
			if (element != null)
			{
				element.Text = string.Empty;
			}
		}

		void OnSearchTextChanged(object sender, EventArgs e)
		{
			UpdatePropertyHelper((element, control) =>
			{
				((IElementController)element).SetValueFromRenderer(SearchBar.TextProperty, control.SearchText);
			});
		}


		void UpdateText()
		{
			UpdatePropertyHelper((element, control) => control.SearchText = element.Text);
		}

		void UpdatePlaceholder()
		{
			UpdatePropertyHelper((element, control) =>
			{
				control.PlaceHolder = element.Placeholder ?? DefaultPlaceholder;
			});
		}

		void UpdateFont()
		{
			UpdatePropertyHelper((element, control) =>
				control.Font = new System.Drawing.Font(
					element.FontFamily,
					Math.Max((float)element.FontSize, 1.0f),
					element.FontAttributes.ToWindowsFontStyle()));
		}

		void UpdateAlignment()
		{
			UpdatePropertyHelper((element, control) => control.SearchTextAlign =
				element.HorizontalTextAlignment.ToWindowsHorizontalAlignment());
		}

		void UpdatePlaceholderColor()
		{
			UpdatePropertyHelper((element, control) => control.PlaceHolderColor = element.PlaceholderColor.ToWindowsColor(WDrawing.SystemColors.GrayText));
		}

		void UpdateTextColor()
		{
			UpdatePropertyHelper((element, control) => control.SearchTextColor = element.TextColor.ToWindowsColor(WDrawing.SystemColors.ControlText));
		}

	}
}
