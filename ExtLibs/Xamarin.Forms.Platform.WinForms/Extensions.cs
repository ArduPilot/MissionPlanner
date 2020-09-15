using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms.Platform.WinForms
{
	public static class Extensions
	{
		static public bool AddCollectionChangedEvent(this System.Collections.IEnumerable coll, NotifyCollectionChangedEventHandler handler)
		{
			var cc = coll as INotifyCollectionChanged;
			if (cc != null)
			{
				cc.CollectionChanged += handler;
				return true;
			}
			return false;
		}

		static public bool RemoveCollectionChangedEvent(this System.Collections.IEnumerable coll, NotifyCollectionChangedEventHandler handler)
		{
			var cc = coll as INotifyCollectionChanged;
			if (cc != null)
			{
				cc.CollectionChanged -= handler;
				return true;
			}
			return false;
		}
	}
}
