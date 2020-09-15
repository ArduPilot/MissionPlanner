using System;
using System.IO;
using System.Runtime.Serialization;
using SkiaSharp;

namespace System.Drawing
{ 
    [Serializable]
    public class Icon : Bitmap, ISerializable, ICloneable, IDisposable
    {
        private byte[] iconData;
        private Size iconSize;

        private Icon(SerializationInfo info, StreamingContext context)
        {
            iconData = (byte[])info.GetValue("IconData", typeof(byte[]));
            iconSize = (Size)info.GetValue("IconSize", typeof(Size));
            if (iconSize.IsEmpty)
            {
                //Initialize(0, 0);
                nativeSkBitmap = new SKBitmap();
            }
            else
            {
                nativeSkBitmap = SKBitmap.FromImage(SKImage.FromEncodedData(new MemoryStream(iconData)));
                iconSize = new Size(nativeSkBitmap.Width, nativeSkBitmap.Height);
            }
        }


        void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
        {
            if (iconData != null)
            {
                si.AddValue("IconData", iconData, typeof(byte[]));
            }
            else
            {
                MemoryStream memoryStream = new MemoryStream();
                Save(memoryStream);
                si.AddValue("IconData", memoryStream.ToArray(), typeof(byte[]));
            }
            si.AddValue("IconSize", iconSize, typeof(Size));
        }

        private void Save(MemoryStream memoryStream)
        {
            Save(memoryStream, SKEncodedImageFormat.Png);
        }

        public Icon(Icon value, int i, int i1) : base(value, i, i1)
        {
            iconSize = new Size(nativeSkBitmap.Width, nativeSkBitmap.Height);
        }

        public Icon(MemoryStream value) : base(value)
        {
            iconSize = new Size(nativeSkBitmap.Width, nativeSkBitmap.Height);
        }

        public Icon(Stream stream) : base(stream)
        {
            iconSize = new Size(nativeSkBitmap.Width, nativeSkBitmap.Height);
        }

        public IntPtr Handle
        {
            get { return base.nativeSkBitmap.Handle; }
        }

        public Bitmap ToBitmap()
        {
            return (Bitmap) this;
        }

        public static Icon FromHandle(object getHicon)
        {
            return null;
        }
    }
}