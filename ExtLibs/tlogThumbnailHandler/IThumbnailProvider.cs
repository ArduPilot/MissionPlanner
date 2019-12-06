using System;
using System.Runtime.InteropServices;

namespace tlogThumbnailHandler
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("e357fccd-a995-4576-b01f-234630154e96")]
    interface IThumbnailProvider
    {
        [PreserveSig]
        long GetThumbnail(int squareLength,
            out IntPtr hBitmap, out int bitmapType);
    }
}