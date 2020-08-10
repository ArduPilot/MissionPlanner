using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.WinForms
{
	internal class WinFormsIsolatedStorage : IIsolatedStorageFile
	{
		public WinFormsIsolatedStorage()
		{
		}

		public Task CreateDirectoryAsync(string path)
		{
			return Task.Run(() => Directory.CreateDirectory(path));
		}

		public Task<bool> GetDirectoryExistsAsync(string path)
		{
			return Task.Run<bool>(() => Directory.Exists(path));
		}

		public Task<bool> GetFileExistsAsync(string path)
		{
			return Task.Run<bool>(() => File.Exists(path));
		}

		public Task<DateTimeOffset> GetLastWriteTimeAsync(string path)
		{
			return Task.Run<DateTimeOffset>(() => new DateTimeOffset(File.GetLastWriteTime(path)));
		}

		public Task<Stream> OpenFileAsync(string path, FileMode mode, FileAccess access)
		{
			return Task.Run<Stream>(() =>
			{
				return new FileStream(path, mode, access);
			});
		}

		public Task<Stream> OpenFileAsync(string path, FileMode mode, FileAccess access, FileShare share)
		{
			return Task.Run<Stream>(() =>
			{
				return new FileStream(path, mode, access, share);
			});
		}
	}
}