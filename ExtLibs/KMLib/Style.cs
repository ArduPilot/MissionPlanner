using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Drawing;
using KMLib.Abstract;

namespace KMLib
{
    public class Style : AStyleSelector
    {
        private string m_Id;
        [XmlAttribute("id")]
        public string Id
        {
            get
            {
                return m_Id;
            }
            set
            {
                m_Id = value;
            }
        }
        public void Add(Style style)
        {
            if (m_Style == null)
            {
                m_Style = new List<Style>();
            }
            m_Style.Add(style);
        }

        private List<Style> m_Style;
        //[XmlElement(ElementName = "BallonStyle", Type = typeof(BallonStyle))]
        [XmlElement(ElementName = "IconStyle", Type = typeof(IconStyle))]
        [XmlElement(ElementName = "LabelStyle", Type = typeof(LabelStyle))]
        [XmlElement(ElementName = "LineStyle", Type = typeof(LineStyle))]
        //[XmlElement(ElementName = "ListStyle", Type = typeof(ListStyle))]
        [XmlElement(ElementName = "PolyStyle", Type = typeof(PolyStyle))]
        public List<Style> Lists
        {
            get
            {
                return m_Style;
            }
            set
            {
                m_Style = value;
            }
        }
        [XmlIgnore()]
        private bool StyleSpecified = false;
    }

    public class ColorStyle : Style
    {
        public enum ColorMode { normal, random };
        public ColorStyle(Color color)
        {
            m_Color = color;
        }

        public ColorStyle() { }


        private ColorKML m_Color;
        [XmlElement("color")]
        public ColorKML Color
        {
            get
            {
                return m_Color;
            }
            set
            {
                m_Color = value;
            }
        }

        private ColorMode m_colorMode;
        [XmlElement("colorMode")]
        public ColorMode colorMode
        {
            get
            {
                return m_colorMode;
            }
            set
            {
                m_colorMode = value;
                colorModeSpecified = true;
            }
        }
        [XmlIgnore()]
        private bool colorModeSpecified = false;
        
    }

    public class PolyStyle : ColorStyle
    {
    }

    public class LineStyle : ColorStyle
    {
        public LineStyle(Color color)
        {
            Color = color;
        }
        public LineStyle(Color color, float width)
        {
            Color = color;
            m_Width = width;
        }

        public LineStyle() { }

        private float m_Width = 1.0f;
        [XmlElement("width")]
        public float Width
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
    }

    public class IconStyle : PolyStyle
    {
        public IconStyle(Color color)
        {
            Color = color;
        }
        public IconStyle(Icon icon)
        {
            m_Icon = icon;
        }
        public IconStyle(Color color, float scale, Icon icon)
        {
            Color = color;
            m_scale = scale;
            m_Icon = icon;
        }

        public IconStyle() { }

        private Icon m_Icon;
        [XmlElement("Icon")]
        public Icon Icon
        {
            get
            {
                return m_Icon;
            }
            set
            {
                m_Icon = value;
                IconSpecified = true;
            }
        }
        [XmlIgnore()]
        private bool IconSpecified = false;

        private float m_scale = 1.0f;
        [XmlElement("scale")]
        public float scale
        {
            get
            {
                return m_scale;
            }
            set
            {
                m_scale = value;
                scaleSpecified = true;
            }
        }
        [XmlIgnore()]
        private bool scaleSpecified = false;
    }


    public class LabelStyle : ColorStyle
    {

        public LabelStyle() { }
        public LabelStyle(Color color)
        {
            Color = color;
        }
        public LabelStyle(float scale)
        {
            m_scale = scale;
        }
        public LabelStyle(Color color, float scale)
        {
            Color = color;
            m_scale = scale;
        }
        public LabelStyle(Color color, float scale, ColorMode newColorMode)
        {
            Color = color;
            m_scale = scale;
            colorMode = newColorMode;
        }


        private float m_scale = 1.0f;
        [XmlElement("scale")]
        public float scale
        {
            get
            {
                return m_scale;
            }
            set
            {
                m_scale = value;
                scaleSpecified = true;
            }
        }
        [XmlIgnore()]
        private bool scaleSpecified = false;
    }
}
