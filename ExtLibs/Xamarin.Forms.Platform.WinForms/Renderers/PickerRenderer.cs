using System;
using System.Collections.Specialized;
using System.ComponentModel;
using WForms = System.Windows.Forms;
using WDrawing = System.Drawing;

namespace Xamarin.Forms.Platform.WinForms
{
	public class PickerRenderer : ViewRenderer<Picker, WForms.ComboBox>
	{
		/*-----------------------------------------------------------------*/
		#region Event Handler

		protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
		{
			if (e.OldElement != null)
			{
				e.OldElement.Items.RemoveCollectionChangedEvent(OnCollectionChanged);
			}
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new WForms.ComboBox());
				}

				e.NewElement.Items.AddCollectionChangedEvent(OnCollectionChanged);
				Control.DropDownStyle = WForms.ComboBoxStyle.DropDownList;

				UpdateItems();
				UpdateSelectedIndex();
				UpdateTextColor();
			}

			base.OnElementChanged(e);
		}

		protected override void OnNativeElementChanged(NativeElementChangedEventArgs<WForms.ComboBox> e)
		{
			base.OnNativeElementChanged(e);
			if (e.OldControl != null)
			{
				e.OldControl.SelectedIndexChanged -= OnSelectedIndexChanged;
			}

			if (e.NewControl != null)
			{
				e.NewControl.SelectedIndexChanged += OnSelectedIndexChanged;
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Picker.SelectedIndexProperty.PropertyName)
			{
				UpdateSelectedIndex();
			}
			else if (e.PropertyName == Picker.TextColorProperty.PropertyName)
			{
				UpdateTextColor();
			}

			base.OnElementPropertyChanged(sender, e);
		}

		void OnSelectedIndexChanged(object sender, EventArgs e)
		{
			((IElementController)Element).SetValueFromRenderer(Picker.SelectedIndexProperty, Control.SelectedIndex);
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Internals

		void UpdateSelectedIndex()
		{
			if (Control != null && Element != null)
			{
				Control.SelectedIndex = Element.SelectedIndex;
			}
		}

		void UpdateTextColor()
		{
			if (Control != null && Element != null)
			{
				Control.ForeColor = Element.TextColor.ToWindowsColor(WDrawing.SystemColors.ControlText);
			}
		}


		void UpdateItems()
		{
			Control.Items.Clear();
			int count = Element.Items.Count;
			foreach (var item in Element.Items)
			{
				Control.Items.Add(item);
			}
		}

		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			UpdateItems();
			UpdateSelectedIndex();
		}


		#endregion
	}
}
