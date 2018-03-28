using System;
using System.Runtime.InteropServices;
using System.Text;

namespace tlogThumbnailHandler
{
    [ComImport]
    [Guid("BB2E617C-0920-11d1-9A0B-00C04FC2D6C1")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IExtractImage
    {       
        [PreserveSig]
        long GetLocation(out StringBuilder pszPathBuffer, int cch, ref int pdwPriority, ref SIZE prgSize, int dwRecClrDepth, ref int pdwFlags);
   
        [PreserveSig]
        long Extract(out IntPtr phBmpThumbnail);
    }
}