using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

namespace MissionPlanner.Utilities
{
    // Mono's XML converter does not mark layouts as serialized, but the
    // TableLayoutPanel setter requires that flag when ApplyResources assigns one.
    public sealed class MonoTableLayoutSettingsConverter : TypeConverter
    {
        private static TypeConverter original;
        private static readonly FieldInfo serialized = typeof(TableLayoutSettings).GetField(
            "isSerialized", BindingFlags.Instance | BindingFlags.NonPublic);

        internal static void Register()
        {
            if (original != null || serialized == null)
                return;
            original = TypeDescriptor.GetConverter(typeof(TableLayoutSettings));
            TypeDescriptor.AddAttributes(typeof(TableLayoutSettings),
                new TypeConverterAttribute(typeof(MonoTableLayoutSettingsConverter)));
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return original.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var result = original.ConvertFrom(context, culture, value);
            if (result is TableLayoutSettings)
                serialized.SetValue(result, true);
            return result;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return original.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return original.ConvertTo(context, culture, value, destinationType);
        }
    }
}
