using System.Runtime.InteropServices;

namespace tlogThumbnailHandler
{
    [ComImport]
    [Guid("953BB1EE-93B4-11d1-98A3-00C04FB687DA")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IExtractImage2
    {
        int GetDateStamp([In, Out]ref System.Runtime.InteropServices.ComTypes.FILETIME pDateStamp);
    }
}