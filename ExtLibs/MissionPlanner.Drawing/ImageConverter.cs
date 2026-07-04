using System.ComponentModel;
using System.Globalization;
using System.IO;
using SkiaSharp;

namespace System.Drawing
{
    /// <summary>
    /// Converts <see cref="Image"/>/<see cref="Bitmap"/> instances to and from a
    /// <see cref="T:System.Byte[]"/> payload.
    ///
    /// This is required so WinForms designer resources (images embedded in .resx and emitted
    /// by System.Resources.Extensions as a byte[] + TypeConverter reference) can be
    /// deserialized when this SkiaSharp-based System.Drawing shim is used in place of the
    /// framework System.Drawing — e.g. when Mission Planner runs under Mono on macOS/Android.
    ///
    /// Without a converter registered on <see cref="Image"/>/<see cref="Bitmap"/>,
    /// TypeDescriptor.GetConverter(...) returns the base <see cref="TypeConverter"/>, whose
    /// ConvertFrom throws "TypeConverter cannot convert from System.Byte[]" during
    /// Splash.InitializeComponent()/FlightData.InitializeComponent() and aborts startup.
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
            if (destinationType == typeof(byte[]))
            {
                if (value == null)
                    return new byte[0];

                if (value is Image image)
                {
                    using (var ms = new MemoryStream())
                    {
                        image.Save(ms, SKEncodedImageFormat.Png);
                        return ms.ToArray();
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
