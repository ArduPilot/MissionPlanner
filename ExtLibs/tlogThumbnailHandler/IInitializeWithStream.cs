using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace tlogThumbnailHandler
{
    /// <summary>
    /// Provides a method used to initialize a handler, such as a property handler, thumbnail provider, or preview handler, with a file stream.
    /// </summary>
    [ComVisible(true), Guid("b824b49d-22ac-4161-ac8a-9916e8fa3f7f"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IInitializeWithStream
    {
        /// <summary>
        /// Initializes a handler with a file stream.
        /// </summary>
        /// <param name="stream">Pointer to an <see cref="IStream"/> interface that represents the file stream source.</param>
        /// <param name="grfMode">Indicates the access mode for <paramref name="stream"/>.</param>
        [PreserveSig]
        long Initialize(IStream stream, int grfMode);
    }
}