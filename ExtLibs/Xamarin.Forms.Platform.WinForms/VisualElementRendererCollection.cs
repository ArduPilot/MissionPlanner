using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WForms = System.Windows.Forms;

namespace Xamarin.Forms.Platform.WinForms
{
	public class VisualElementRendererCollection : IEnumerable<IVisualElementRenderer>
	{
		WForms.Control _parentNativeElement = null;
		List<IVisualElementRenderer> _collection = new List<IVisualElementRenderer>();

		public VisualElementRendererCollection()
		{
		}

		public WForms.Control ParentNativeElement
		{
			get => _parentNativeElement;

			set
			{
				if (value != _parentNativeElement)
				{
					UpdateParent(value);
					_parentNativeElement = value;
				}
			}
		}

		public IVisualElementRenderer this[int index] => _collection[index];

		public int Count => _collection.Count;

		public void Add(IVisualElementRenderer renderer)
		{
			if (renderer != null && _collection.IndexOf(renderer) < 0)
			{
				SetNativeElementParent(renderer, _parentNativeElement);
				_collection.Add(renderer);
			}
		}

		public void Insert(int index, IVisualElementRenderer renderer)
		{
			if (renderer != null)
			{
				SetNativeElementParent(renderer, _parentNativeElement);
				_collection.Insert(index, renderer);
			}
		}

		public void RemoveAt(int index)
		{
			var renderer = _collection[index];
			SetNativeElementParent(renderer, null);
			_collection.RemoveAt(index);
		}

		public void Remove(IVisualElementRenderer renderer)
		{
			var index = _collection.IndexOf(renderer);
			if (index < 0)
			{
				return;
			}
			RemoveAt(index);
		}

		public void Clear()
		{
			UpdateParent(null);
			_collection.Clear();
		}

		public IVisualElementRenderer Find(WForms.Control control)
		{
			foreach (var item in _collection)
			{
				if (control == item?.NativeElement)
				{
					return item;
				}
			}

			return null;
		}

		IEnumerator<IVisualElementRenderer> IEnumerable<IVisualElementRenderer>.GetEnumerator()
			=> ((IEnumerable<IVisualElementRenderer>)_collection).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> ((IEnumerable<IVisualElementRenderer>)_collection).GetEnumerator();

		void SetNativeElementParent(IVisualElementRenderer renderer, WForms.Control parent)
		{
			var nativeElement = renderer?.NativeElement;
			if (nativeElement != null)
			{
				var parent2 = parent as ILogicalChildrenContainer;
				if (parent2 != null)
				{
					parent2.LogicalChildren.Add(nativeElement);
				}
				else
				{
					nativeElement.Parent = parent;
				}
			}
		}

		void UpdateParent(WForms.Control parent)
		{
			foreach (var item in _collection)
			{
				SetNativeElementParent(item, parent);
			}
		}
	}
}
