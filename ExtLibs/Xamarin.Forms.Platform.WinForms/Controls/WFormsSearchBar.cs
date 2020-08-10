using System;
using System.ComponentModel;
using WDrawing = System.Drawing;
using WForms = System.Windows.Forms;

namespace Xamarin.Forms.Platform.WinForms
{
	public class WFormsSearchBar : WForms.UserControl
	{
		public class CustomTextBox : WForms.TextBox
		{
			string _placeholder = "Search";
			WDrawing.Color _placeholderColor = WDrawing.SystemColors.InactiveCaptionText;
			WDrawing.Brush _brush = null;

			public CustomTextBox()
			{
			}

			public string Placeholder
			{
				get => _placeholder;
				set
				{
					if (!string.Equals(value, _placeholder))
					{
						_placeholder = value;
						Invalidate();
					}
				}
			}

			public WDrawing.Color PlaceholderColor
			{
				get => _placeholderColor;
				set
				{
					if (value != _placeholderColor)
					{
						_placeholderColor = value;
						_brush?.Dispose();
						_brush = null;
						Invalidate();
					}
				}
			}

			protected override void WndProc(ref WForms.Message m)
			{
				if (m.Msg == 15 &&
					Enabled &&
					string.IsNullOrEmpty(Text) &&
					!string.IsNullOrEmpty(_placeholder))
				{
					using (var g = CreateGraphics())
					{
						if (_brush == null)
						{
							_brush = new WDrawing.SolidBrush(_placeholderColor);
						}
						g.DrawString(_placeholder, Font, _brush, 1, 1);
					}
				}
				base.WndProc(ref m);
			}
		}

		CustomTextBox _textbox;
		WForms.Button _btnSearch;

		public WFormsSearchBar()
		{
			_textbox = new CustomTextBox()
			{
				Anchor = WForms.AnchorStyles.None,
				Parent = this,
				AutoSize = false,
				Multiline = false,
				ScrollBars = WForms.ScrollBars.None
			};

			_btnSearch = new WForms.Button()
			{
				Anchor = WForms.AnchorStyles.None,
				Parent = this,
			};

			_btnSearch.Click += (s, e) =>
			{
				SearchButtonClick?.Invoke(this, e);
			};

			_textbox.KeyDown += (s, e) =>
			{
				switch (e.KeyCode)
				{
					case WForms.Keys.Enter:
						{
							SearchButtonClick?.Invoke(this, e);
						}
						break;

					case WForms.Keys.Escape:
						{
							CancelButtonClick?.Invoke(this, e);
						}
						break;
				};
			};

			_textbox.TextChanged += (s, e) =>
			{
				SearchTextChanged?.Invoke(this, e);
			};
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_textbox?.Dispose();
				_textbox = null;
				_btnSearch?.Dispose();
				_btnSearch = null;
			}
			base.Dispose(disposing);
		}

		public string SearchText
		{
			get => _textbox.Text;
			set => _textbox.Text = value;
		}

		public WDrawing.Color SearchTextColor
		{
			get => _textbox.ForeColor;
			set => _textbox.ForeColor = value;
		}

		public WForms.HorizontalAlignment SearchTextAlign
		{
			get => _textbox.TextAlign;
			set => _textbox.TextAlign = value;
		}

		public string PlaceHolder
		{
			get => _textbox.Placeholder;
			set => _textbox.Placeholder = value;
		}

		public WDrawing.Color PlaceHolderColor
		{
			get => _textbox.PlaceholderColor;
			set => _textbox.PlaceholderColor = value;
		}

		public event EventHandler SearchButtonClick;
		public event EventHandler CancelButtonClick;
		public event EventHandler SearchTextChanged;

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			UpdateLayout();
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			UpdateLayout();
		}

		void UpdateLayout()
		{
			int bw = Math.Min(Width, Height);
			int bh = Height;

			if (_textbox != null)
			{
				_textbox.Left = 0;
				_textbox.Top = 0;
				_textbox.Width = Width - bw;
				_textbox.Height = bh;
			}

			if (_btnSearch != null)
			{
				_btnSearch.Left = Width - bw;
				_btnSearch.Top = 0;
				_btnSearch.Width = bw;
				_btnSearch.Height = bh;
			}
		}
	}
}
