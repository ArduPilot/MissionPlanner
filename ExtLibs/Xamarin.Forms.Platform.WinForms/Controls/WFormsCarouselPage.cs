using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using WDrawing = System.Drawing;
using WForms = System.Windows.Forms;

namespace Xamarin.Forms.Platform.WinForms
{
	public class WFormsCarouselPage : WForms.UserControl, ILogicalChildrenContainer
	{
		WForms.Button _btnBack;
		WForms.Button _btnForward;
		WForms.Panel _content;
		ObservableCollection<WForms.Control> _children = new ObservableCollection<WForms.Control>(); 

		int _selectedIndex = -1;

		public WFormsCarouselPage()
		{
			var size = ClientSize;
			int fw = (Font?.Height).GetValueOrDefault(1) * 2 + 4;

			_content = new WForms.Panel()
			{
				Parent = this,
				Left = fw,
				Top = 0,
				Width = size.Width - fw * 2,
				Height = size.Height,
				Anchor =
					WForms.AnchorStyles.Left |
					WForms.AnchorStyles.Right |
					WForms.AnchorStyles.Top |
					WForms.AnchorStyles.Bottom
			};

			_btnBack = new WForms.Button()
			{
				Parent = this,
				Left = 0,
				Top = 0,
				Width = fw,
				Height = size.Height,
				Text = "<",
				Anchor =
					WForms.AnchorStyles.Left |
					WForms.AnchorStyles.Top |
					WForms.AnchorStyles.Bottom
			};

			_btnForward = new WForms.Button()
			{
				Parent = this,
				Left = size.Width - fw,
				Top = 0,
				Width = fw,
				Height = size.Height,
				Text = ">",
				Anchor =
					WForms.AnchorStyles.Right |
					WForms.AnchorStyles.Top |
					WForms.AnchorStyles.Bottom
			};

			_btnBack.Click += OnBackButtonClicked;
			_btnForward.Click += OnForwardButtonClicked;
			_children.CollectionChanged += OnChildrenCollectionChanged;
		}


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_btnBack?.Dispose();
				_btnBack = null;
				_btnForward?.Dispose();
				_btnForward = null;
				_content?.Dispose();
				_content = null;
			}
			base.Dispose(disposing);
		}


		public IList<WForms.Control> LogicalChildren => _children;

		public WForms.Panel Content => _content;

		public int SelectedIndex
		{
			get => _selectedIndex;
			set
			{
				if (value != _selectedIndex)
				{
					UpdateSelectedIndex(value);
				}
			}
		}

		void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					{
						foreach (WForms.Panel item in e.NewItems)
						{
							_content.Controls.Add(item);
						}
					}
					break;

				case NotifyCollectionChangedAction.Remove:
					{
						foreach (WForms.Panel item in e.OldItems)
						{
							_content.Controls.Remove(item);
						}
					}
					break;

				case NotifyCollectionChangedAction.Replace:
					{
						foreach (WForms.Panel item in e.OldItems)
						{
							_content.Controls.Remove(item);
						}
						foreach (WForms.Panel item in e.NewItems)
						{
							_content.Controls.Add(item);
						}
					}
					break;

				case NotifyCollectionChangedAction.Move:
					{
					}
					break;

				case NotifyCollectionChangedAction.Reset:
					{
						_content.Controls.Clear();
					}
					break;
			}
			UpdateSelectedIndex(_selectedIndex);
		}

		void UpdateSelectedIndex(int index)
		{
			_selectedIndex = Math.Max(0, Math.Min(index, _children.Count - 1));
			UpdateSelectedItem();
		}

		void UpdateSelectedItem()
		{
			if (_content.Controls.Count > 0 && _selectedIndex >= 0)
			{
				_content.Controls.SetChildIndex(_children[_selectedIndex], 0);
			}
		}

		void OnBackButtonClicked(object sender, EventArgs e)
		{
			SelectedIndex--;
		}

		void OnForwardButtonClicked(object sender, EventArgs e)
		{
			SelectedIndex++;
		}
	}
}
