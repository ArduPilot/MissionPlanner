using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Drawing;
using Core.Utils;

namespace Core.Geometry
{
    [TypeConverter(typeof(SizeEExpandableConverter))]
    public class SizeE : IXmlSerializable
    {//Size2D
        #region Private Fields
        private double m_Width;
        private double m_Height;
        #endregion

        #region Constructors
        public SizeE() { }
        public SizeE(SizeE size)
            : this(size.Width, size.Height)
        { }
        public SizeE(PointE pt)
            : this(pt.X, pt.Y)
        { }
        public SizeE(double width, double height)
        {
            m_Width = width;
            m_Height = height;
        }
        #endregion

        #region GetSet
        [XmlAttribute()]
        public double Width
        {
            get
            {
                return m_Width;
            }
            set
            {
                m_Width = value;
            }
        }

        [XmlAttribute()]
        public double Height
        {
            get
            {
                return m_Height;
            }
            set
            {
                m_Height = value;
            }
        }

        [XmlIgnore(), Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                return (m_Height == 0 || m_Width == 0);
            }
        }

        #endregion

        #region Operator Overloads

        public static SizeE operator +(SizeE c)
        {
            SizeE ans = new SizeE();
            ans.Width = +c.Width;
            ans.Height = +c.Height;
            return ans;
        }

        public static SizeE operator -(SizeE c)
        {
            SizeE ans = new SizeE();
            ans.Width = -c.Width;
            ans.Height = -c.Height;
            return ans;
        }

        public static SizeE operator +(SizeE a, SizeE b)
        {
            return new SizeE(a.Width + b.Width, a.Height + b.Height);
        }

        public static SizeE operator -(SizeE a, SizeE b)
        {
            return new SizeE(a.Width - b.Width, a.Height - b.Height);
        }

        public static SizeE operator *(SizeE a, SizeE b)
        {
            return new SizeE(a.Width * b.Width, a.Height * b.Height);
        }

        public static SizeE operator *(SizeE a, double scalar)
        {
            return new SizeE(a.Width * scalar, a.Height * scalar);
        }

        public static SizeE operator *(SizeE a, float scalar)
        {
            return new SizeE(a.Width * scalar, a.Height * scalar);
        }

        public static SizeE operator /(SizeE a, SizeE b)
        {
            return new SizeE(a.Width / b.Width, a.Height / b.Height);
        }

        public static SizeE operator /(SizeE a, double scalar)
        {
            return new SizeE(a.Width / scalar, a.Height / scalar);
        }

        public static SizeE operator /(SizeE a, float scalar)
        {
            return new SizeE(a.Width / scalar, a.Height / scalar);
        }


        #endregion

        #region Conversions

        static public implicit operator SizeE(System.Drawing.SizeF floatSize)
        {
            return new SizeE((double)floatSize.Width, (double)floatSize.Height);
        }

        static public explicit operator SizeF(SizeE doublePt)
        {
            return new System.Drawing.SizeF((float)doublePt.Width, (float)doublePt.Height);
        }

        static public implicit operator SizeE(System.Drawing.Size intSize)
        {
            return new SizeE((double)intSize.Width, (double)intSize.Height);
        }

        static public explicit operator Size(SizeE doublePt)
        {
            return new System.Drawing.Size((int)doublePt.Width, (int)doublePt.Height);
        }
        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader r)
        {
            string str = r.ReadElementContentAsString();
            Deserialize(str);
        }

        public virtual void WriteXml(XmlWriter w)
        {
            w.WriteString(Serialize());
        }

        #endregion

        #region (De)Serialize
        public virtual string Serialize()
        {
            return m_Width + "," + m_Height;
        }

        public virtual void Deserialize(string str)
        {
            List<string> bits = StringUtils.SplitToList(str, ",");
            m_Width = Convert.ToDouble(bits[0]);
            m_Height = Convert.ToDouble(bits[1]);
        }
        #endregion

        public SizeE Inflate(double amount)
        {
            return Inflate(amount, amount);
        }

        public SizeE Inflate(SizeE amount)
        {
            return Inflate(amount.Width, amount.Height);
        }

        public SizeE Inflate(double X, double Y)
        {
            return new SizeE(m_Width + 2 * X, m_Height + 2 * Y);
        }

        public void SetMaxDim(double maxSize)
        {
            if (m_Height > m_Width) {
                m_Width = maxSize * m_Width / m_Height;
                m_Height = maxSize;
            } else {
                m_Height = maxSize * m_Height / m_Width;
                m_Width = maxSize;
            }
        }
        public void SetMinDim(double minSize)
        {
            if (m_Height > m_Width) {
                m_Height = minSize * m_Height / m_Width;
                m_Width = minSize;
            } else {
                m_Width = minSize * m_Width / m_Height;
                m_Height = minSize;
            }
        }
    }

    internal class SizeEExpandableConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string)) return true;
            return base.CanConvertTo(context, destinationType);
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
            return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            SizeE size = value as SizeE;
            if (size != null && destinationType == typeof(string)) {
                return size.Width + ", " + size.Height;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string str = value as string;
            if (str != null) {
                try {
                    if (str.Contains(",")) {
                        int commapos = str.IndexOf(",");
                        double width = Convert.ToDouble((str.Substring(0, commapos).Trim()));
                        double height = Convert.ToDouble((str.Substring(commapos + 1)).Trim());
                        return new SizeE(width, height);
                    }
                }
                catch {
                    throw new ArgumentException("Please enter a width and height seperated by a comma.");
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

}
