using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace System.Drawing
{
    /// <summary>
    /// Builds an Icon from .ico bytes, for preserialized resx resources (see ImageConverter).
    /// </summary>
    public class IconConverter : ImageConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is byte[] bytes)
                return new Icon(new MemoryStream(bytes));
            return base.ConvertFrom(context, culture, value);
        }
    }
}
