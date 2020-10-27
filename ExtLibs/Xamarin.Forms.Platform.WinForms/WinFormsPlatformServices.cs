using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using WForms = System.Windows.Forms;

namespace Xamarin.Forms.Platform.WinForms
{
	internal class WinFormsPlatformServices : IPlatformServices
	{
		WForms.Form _mainForm;
		int _currentThreadId;
		Dictionary<NamedSize, double> _fontSizes = new Dictionary<NamedSize, double>();
		static readonly MD5CryptoServiceProvider _md5 = new MD5CryptoServiceProvider();

		internal WinFormsPlatformServices(WForms.Form mainForm, int currentThreadId)
		{
			if (mainForm == null)
			{
				throw new ArgumentNullException(nameof(mainForm));
			}
			_mainForm = mainForm;
			_currentThreadId = currentThreadId;

			var defFontSize = System.Drawing.SystemFonts.DefaultFont.Size;
			_fontSizes.Add(NamedSize.Default, defFontSize);

			//	暫定
			_fontSizes.Add(NamedSize.Micro, defFontSize / 2);
			_fontSizes.Add(NamedSize.Small, defFontSize);
			_fontSizes.Add(NamedSize.Medium, defFontSize * 1.5);
			_fontSizes.Add(NamedSize.Large, defFontSize * 3);
		}

		public bool IsInvokeRequired => Thread.CurrentThread.ManagedThreadId != _currentThreadId;
        public OSAppTheme RequestedTheme { get; } = OSAppTheme.Unspecified;

        public string RuntimePlatform => "WinForms";

		public void BeginInvokeOnMainThread(Action action)
		{
			if (_mainForm.IsHandleCreated)
			{
				_mainForm.BeginInvoke(action);
			}
			else
			{
				action();
			}
		}

		public Ticker CreateTicker()
		{
			return new WinFormsTicker();
		}

		public Assembly[] GetAssemblies()
		{
			return AppDomain.CurrentDomain.GetAssemblies();
		}

        public string GetHash(string input)
        {
            return GetMD5Hash(input);
        }

        public string GetMD5Hash(string input)
		{
			var bytes = _md5.ComputeHash(Encoding.UTF8.GetBytes(input));
			var sb = new StringBuilder();
			foreach (var c in bytes)
			{
				sb.AppendFormat("{0:X2}", c);
			}
			return sb.ToString();
		}

		public double GetNamedSize(NamedSize size, Type targetElementType, bool useOldSizes)
		{
			if (_fontSizes.ContainsKey(size))
			{
				return _fontSizes[size];
			}
			throw new ArgumentException(nameof(size));
		}

        public Color GetNamedColor(string name)
        {
            return Color.FromHex(name);
        }

        public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
		{
			return Platform.GetNativeSizeInternal(view, widthConstraint, heightConstraint);
		}

		public async Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)
		{
			using (var client = new HttpClient())
			{
				HttpResponseMessage streamResponse = await client.GetAsync(uri.AbsoluteUri).ConfigureAwait(false);

				if (!streamResponse.IsSuccessStatusCode)
				{
					Log.Warning("HTTP Request", $"Could not retrieve {uri}, status code {streamResponse.StatusCode}");
					return null;
				}

				return await streamResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
			}
		}

		public IIsolatedStorageFile GetUserStoreForApplication()
		{
			return new WinFormsIsolatedStorage();
		}

		public void OpenUriAction(Uri uri)
		{
			try
			{
				System.Diagnostics.Process.Start(uri.PathAndQuery);
			}
			catch
			{
			}
		}

		public void StartTimer(TimeSpan interval, Func<bool> callback)
		{
			var timer = new WForms.Timer();
			timer.Interval = (int)interval.TotalMilliseconds;
			timer.Tick += (s, e) =>
			{
				if (callback() == false)
				{
					timer.Stop();
				}
			};
			timer.Start();
		}

		public void QuitApplication()
		{
			WForms.Application.Exit();
		}
	}
}
