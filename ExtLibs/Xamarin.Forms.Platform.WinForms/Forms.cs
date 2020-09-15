using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xamarin.Forms.Internals;
using WForms = System.Windows.Forms;

namespace Xamarin.Forms.Platform.WinForms
{
	public static class Forms
	{
		public static bool IsInitialized
		{
			get;
			private set;
		}

		public static void Init(WForms.Form mainForm)
		{
			if (IsInitialized)
			{
				return;
			}

			Device.PlatformServices = new WinFormsPlatformServices(mainForm, Thread.CurrentThread.ManagedThreadId);
			Device.SetIdiom(TargetIdiom.Desktop);
			Device.Info = new WinFormsDeviceInfo();

			Internals.Registrar.RegisterAll(new[]
				{ typeof(ExportRendererAttribute), typeof(ExportCellAttribute), typeof(ExportImageSourceHandlerAttribute) });
			ExpressionSearch.Default = new WinFormsExpressionSearch();

			IsInitialized = true;
		}
	}
}
