using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;

namespace Aberus.VisualStudio.Debugger.ImageVisualizer
{
    [Serializable]
    public class SerializableBitmapImage : ISerializable
    {
        public BitmapSource bitmapSource;
        private readonly string expression;

        public BitmapImage Image { get; private set; }

        public SerializableBitmapImage(BitmapImage image)
        {
            this.Image = image;
        }

        public SerializableBitmapImage(BitmapSource source)
        {
            bitmapSource = source;
        }

        protected SerializableBitmapImage(SerializationInfo info, StreamingContext context)
        {
            foreach (var i in info)
            {
                if (string.Equals(i.Name, "Image", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        if (i.Value is byte[] array)
                        {
                            var stream = new MemoryStream(array);
                            stream.Seek(0, SeekOrigin.Begin);

                            Image = new BitmapImage();
                            Image.CacheOption = BitmapCacheOption.OnLoad;
                            Image.BeginInit();
                            Image.StreamSource = stream;
                            Image.EndInit();
                            Image.Freeze();
                        }
                    }
                    catch (ExternalException)
                    {
                    }
                    catch (ArgumentException)
                    {
                    }
                    catch (OutOfMemoryException)
                    {
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    catch (NotImplementedException)
                    {
                    }
                    catch (FileNotFoundException)
                    {
                    }
                }
                else if(string.Equals(i.Name, "Name", StringComparison.OrdinalIgnoreCase))
                {
                    expression = (string)i.Value;
                }
            }
        }

        public static implicit operator SerializableBitmapImage(BitmapImage bitmapImage)
        {
            return new SerializableBitmapImage(bitmapImage);
        }

        public static implicit operator BitmapImage(SerializableBitmapImage serializableBitmapImage)
        {
            return serializableBitmapImage.Image;
        }

        //[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var source = Image ?? bitmapSource;

            if (source != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(source));
                    encoder.Save(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    info.AddValue("Image", memoryStream.ToArray(), typeof(byte[]));
                }

                info.AddValue("Name", source.ToString());
            }
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(expression))
                return expression;

            return base.ToString();
        }
    }
}
