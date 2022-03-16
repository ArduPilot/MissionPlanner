using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Aberus.VisualStudio.Debugger.ImageVisualizer
{
    [Serializable]
    public class SerializableBitmapImage : ISerializable
    {
        public byte[] Image { get; private set; }

        public SerializableBitmapImage(byte[] image)
        {
            this.Image = image;
        }

        public SerializableBitmapImage(Stream image)
        {
            var ms = new MemoryStream();
            image.CopyTo(ms);
            this.Image = ms.ToArray();
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
                            Image = (byte[])i.Value;
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
                    
                }
            }
        }

        public static implicit operator SerializableBitmapImage(byte[] bitmapImage)
        {
            return new SerializableBitmapImage(bitmapImage);
        }

        public static implicit operator byte[](SerializableBitmapImage serializableBitmapImage)
        {
            return serializableBitmapImage.Image;
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            {
                using (var memoryStream = new MemoryStream())
                {
                    info.AddValue("Image", Image, typeof(byte[]));
                }

                info.AddValue("Name", "Names");
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
