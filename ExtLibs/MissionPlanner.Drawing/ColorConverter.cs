using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.Drawing
{
    public class ColorConverter : TypeConverter
    {
        private class ColorComparer : IComparer
        {
            public int Compare(object left, object right)
            {
                Color color = (Color) left;
                return string.Compare(strB: ((Color) right).Name, strA: color.Name, ignoreCase: false,
                    culture: CultureInfo.InvariantCulture);
            }
        }

        private static string ColorConstantsLock = "colorConstants";

        private static Hashtable colorConstants;

        private static string SystemColorConstantsLock = "systemColorConstants";

        private static Hashtable systemColorConstants;

        private static string ValuesLock = "values";

        private static StandardValuesCollection values;

        private static Hashtable Colors
        {
            get
            {
                if (colorConstants == null)
                {
                    lock (ColorConstantsLock)
                    {
                        if (colorConstants == null)
                        {
                            Hashtable hash = new Hashtable(StringComparer.OrdinalIgnoreCase);
                            FillConstants(hash, typeof(Color));
                            colorConstants = hash;
                        }
                    }
                }

                return colorConstants;
            }
        }

        private static Hashtable SystemColors
        {
            get
            {
                if (systemColorConstants == null)
                {
                    lock (SystemColorConstantsLock)
                    {
                        if (systemColorConstants == null)
                        {
                            Hashtable hash = new Hashtable(StringComparer.OrdinalIgnoreCase);
                            FillConstants(hash, typeof(SystemColors));
                            systemColorConstants = hash;
                        }
                    }
                }

                return systemColorConstants;
            }
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        internal static object GetNamedColor(string name)
        {
            object obj = null;
            obj = Colors[name];
            if (obj != null)
            {
                return obj;
            }

            return SystemColors[name];
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string text = value as string;
            if (text != null)
            {
                object obj = null;
                string text2 = text.Trim();
                if (text2.Length == 0)
                {
                    obj = Color.Empty;
                }
                else
                {
                    obj = GetNamedColor(text2);
                    if (obj == null)
                    {
                        if (culture == null)
                        {
                            culture = CultureInfo.CurrentCulture;
                        }

                        char c = culture.TextInfo.ListSeparator[0];
                        bool flag = true;
                        TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
                        if (text2.IndexOf(c) == -1)
                        {
                            if (text2.Length >= 2 && (text2[0] == '\'' || text2[0] == '"') &&
                                text2[0] == text2[text2.Length - 1])
                            {
                                obj = Color.FromName(text2.Substring(1, text2.Length - 2));
                                flag = false;
                            }
                            else if ((text2.Length == 7 && text2[0] == '#') ||
                                     (text2.Length == 8 && (text2.StartsWith("0x") || text2.StartsWith("0X"))) ||
                                     (text2.Length == 8 && (text2.StartsWith("&h") || text2.StartsWith("&H"))))
                            {
                                obj = Color.FromArgb(-16777216 |
                                                     (int) converter.ConvertFromString(context, culture, text2));
                            }
                        }

                        if (obj == null)
                        {
                            string[] array = text2.Split(c);
                            int[] array2 = new int[array.Length];
                            for (int i = 0; i < array2.Length; i++)
                            {
                                array2[i] = (int) converter.ConvertFromString(context, culture, array[i]);
                            }

                            switch (array2.Length)
                            {
                                case 1:
                                    obj = Color.FromArgb(array2[0]);
                                    break;
                                case 3:
                                    obj = Color.FromArgb(array2[0], array2[1], array2[2]);
                                    break;
                                case 4:
                                    obj = Color.FromArgb(array2[0], array2[1], array2[2], array2[3]);
                                    break;
                            }

                            flag = true;
                        }

                        if (obj != null && flag)
                        {
                            int num = ((Color) obj).ToArgb();
                            foreach (Color value2 in Colors.Values)
                            {
                                if (value2.ToArgb() == num)
                                {
                                    obj = value2;
                                    break;
                                }
                            }
                        }
                    }

                    if (obj == null)
                    {
                        throw new ArgumentException(SR.Format("Color '{0}' is not valid.", text2));
                    }
                }

                return obj;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == (Type) null)
            {
                throw new ArgumentNullException("destinationType");
            }

            if (value is Color)
            {
                if (destinationType == typeof(string))
                {
                    Color left = (Color) value;
                    if (left == Color.Empty)
                    {
                        return string.Empty;
                    }

                   /* if (left.IsKnownColor)
                    {
                        return left.Name;
                    }
                   */
                    if (left.IsNamedColor)
                    {
                        return "'" + left.Name + "'";
                    }

                    if (culture == null)
                    {
                        culture = CultureInfo.CurrentCulture;
                    }

                    string separator = culture.TextInfo.ListSeparator + " ";
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
                    int num = 0;
                    string[] array;
                    if (left.A < byte.MaxValue)
                    {
                        array = new string[4];
                        array[num++] = converter.ConvertToString(context, culture, left.A);
                    }
                    else
                    {
                        array = new string[3];
                    }

                    array[num++] = converter.ConvertToString(context, culture, left.R);
                    array[num++] = converter.ConvertToString(context, culture, left.G);
                    array[num++] = converter.ConvertToString(context, culture, left.B);
                    return string.Join(separator, array);
                }

                if (destinationType == typeof(InstanceDescriptor))
                {
                    MemberInfo memberInfo = null;
                    object[] arguments = null;
                    Color color = (Color) value;
                    if (color.IsEmpty)
                    {
                        memberInfo = typeof(Color).GetField("Empty");
                    }
                   /* else if (color.IsSystemColor)
                    {
                        memberInfo = typeof(SystemColors).GetProperty(color.Name);
                    }
                    else if (color.IsKnownColor)
                    {
                        memberInfo = typeof(Color).GetProperty(color.Name);
                    }*/
                    else if (color.A != byte.MaxValue)
                    {
                        memberInfo = typeof(Color).GetMethod("FromArgb", new Type[4]
                        {
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int)
                        });
                        arguments = new object[4]
                        {
                            color.A,
                            color.R,
                            color.G,
                            color.B
                        };
                    }
                    else if (color.IsNamedColor)
                    {
                        memberInfo = typeof(Color).GetMethod("FromName", new Type[1]
                        {
                            typeof(string)
                        });
                        arguments = new object[1]
                        {
                            color.Name
                        };
                    }
                    else
                    {
                        memberInfo = typeof(Color).GetMethod("FromArgb", new Type[3]
                        {
                            typeof(int),
                            typeof(int),
                            typeof(int)
                        });
                        arguments = new object[3]
                        {
                            color.R,
                            color.G,
                            color.B
                        };
                    }

                    if (memberInfo != (MemberInfo) null)
                    {
                        return new InstanceDescriptor(memberInfo, arguments);
                    }

                    return null;
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        private static void FillConstants(Hashtable hash, Type enumType)
        {
            MethodAttributes methodAttributes =
                MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static;
            PropertyInfo[] properties = enumType.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                if (propertyInfo.PropertyType == typeof(Color))
                {
                    MethodInfo getMethod = propertyInfo.GetGetMethod();
                    if (getMethod != (MethodInfo) null && (getMethod.Attributes & methodAttributes) == methodAttributes)
                    {
                        object[] index = null;
                        hash[propertyInfo.Name] = propertyInfo.GetValue(null, index);
                    }
                }
            }
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (values == null)
            {
                lock (ValuesLock)
                {
                    if (values == null)
                    {
                        ArrayList arrayList = new ArrayList();
                        arrayList.AddRange(Colors.Values);
                        arrayList.AddRange(SystemColors.Values);
                        int num = arrayList.Count;
                        for (int i = 0; i < num - 1; i++)
                        {
                            for (int j = i + 1; j < num; j++)
                            {
                                if (arrayList[i].Equals(arrayList[j]))
                                {
                                    arrayList.RemoveAt(j);
                                    num--;
                                    j--;
                                }
                            }
                        }

                        arrayList.Sort(0, arrayList.Count, new ColorComparer());
                        values = new StandardValuesCollection(arrayList.ToArray());
                    }
                }
            }

            return values;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}