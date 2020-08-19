using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using System.Windows.Forms;

namespace Xamarin.Forms.Platform.WinForms
{
	internal class WinFormsTicker : Ticker
	{
		readonly Timer _timer = null;

		public WinFormsTicker()
		{
			_timer = new Timer { Interval = 15 };
			_timer.Tick += (s, e) => SendSignals();
		}

		protected override void DisableTimer()
		{
			_timer.Stop();
		}

		protected override void EnableTimer()
		{
			_timer.Start();
		}
	}
}
