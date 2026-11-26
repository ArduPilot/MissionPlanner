using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace System.Drawing
{
    /// <summary>
    /// Builds a Bitmap from encoded image bytes. Resources compiled with
    /// GenerateResourceUsePreserializedResources store resx bytearray images this way, and
    /// System.Resources.Extensions converts them back through the type's TypeConverter.
    /// </summary>
    public class ImageConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(byte[]) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(byte[]) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is byte[] bytes)
                return new Bitmap(new MemoryStream(bytes));
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == typeof(byte[]) && value is Image image)
            {
                using (var ms = new MemoryStream())
                {
                    image.Save(ms, Imaging.ImageFormat.Png);
                    return ms.ToArray();
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
