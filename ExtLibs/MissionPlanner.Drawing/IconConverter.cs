using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace System.Drawing
{
    /// <summary>
    /// Converts <see cref="Icon"/> instances from a <see cref="T:System.Byte[]"/> payload.
    /// Companion to <see cref="ImageConverter"/> — see that type for why this is required for
    /// WinForms .resx designer resources to deserialize under the SkiaSharp shim.
    /// </summary>
    public class IconConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(byte[]) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is byte[] bytes)
                return new Icon(new MemoryStream(bytes));

            return base.ConvertFrom(context, culture, value);
        }
    }
}
