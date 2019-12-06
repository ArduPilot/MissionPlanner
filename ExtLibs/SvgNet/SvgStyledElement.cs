/*
    Copyright © 2003 RiskCare Ltd. All rights reserved.
    Copyright © 2010 SvgNet & SvgGdi Bridge Project. All rights reserved.
    Copyright © 2015, 2017 Rafael Teixeira, Mojmír Němeček, Benjamin Peterson and Other Contributors

    Original source code licensed with BSD-2-Clause spirit, treat it thus, see accompanied LICENSE for more
*/

using System.Text;
using System;
using SvgNet.SvgTypes;
using System.Xml;
using System.Globalization;

namespace SvgNet
{
    /// <summary>
    /// This is an SvgElement that can have a CSS style and an SVG transformation list.  It contains special properties to make reading and setting the style
    /// and the transformation easier.  All SVG elements that actually represent visual entities or groups of entities are <c>SvgStyledTransformedElements</c>.
    /// </summary>
    public class SvgStyledTransformedElement : SvgElement
    {
        public SvgStyledTransformedElement()
        {
        }

        public SvgStyledTransformedElement(string id) : base(id)
        {
        }

        /// <summary>
        /// Provides an easy way to get the attribute called "style" as an <c>SvgStyle</c> object.  If no such attribute has been set, one is created when
        /// this property is read.
        /// </summary>
        public SvgStyle Style
        {
            get
            {
                var o = _atts["style"];
                SvgStyle st = null;

                if (o == null)
                {
                    st = new SvgStyle();
                    _atts["style"] = st;
                    return st;
                }

                if (o.GetType() == typeof(SvgStyle))
                    return (SvgStyle)o;
                else
                {
                    //in case the property was set as a string, make a real object and save it.
                    st = new SvgStyle(o.ToString());
                    _atts["style"] = st;
                    return st;
                }
            }

            set { _atts["style"] = value; }
        }

        /// <summary>
        /// Provides an easy way to get the attribute called "transform" as an <c>SvgTransformList</c> object.  If no such attribute has been set, one is created when
        /// this property is read.
        /// </summary>
        public SvgTransformList Transform
        {
            get
            {
                var o = _atts["transform"];
                SvgTransformList tl = null;

                if (o == null)
                {
                    tl = new SvgTransformList();
                    _atts["transform"] = tl;
                    return tl;
                }

                if (o.GetType() == typeof(SvgTransformList))
                    return (SvgTransformList)o;
                else
                {
                    //in case the property was set as a string, make a real object and save it.
                    tl = new SvgTransformList(o.ToString());
                    _atts["transform"] = tl;
                    return tl;
                }
            }

            set { _atts["transform"] = value; }
        }

        /// <summary>
        /// Given a document and a current node, read this element from the node.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="el"></param>
        public override void ReadXmlElement(XmlDocument doc, XmlElement el)
        {
            foreach (XmlAttribute att in el.Attributes)
            {
                if (att.Name == "style")
                {
                    Style = new SvgStyle(att.Value);
                }
                else if (att.Name == "transform")
                {
                    Transform = new SvgTransformList(att.Value);
                }
                else
                {
                    this[att.Name] = att.Value;
                }
            }
        }

        /// <summary>
        /// Overridden in this class to provide special handling for the style and transform attributes,
        /// which are often long and complicated.  For instance, it may be desirable for styles to be written as entities or as separate
        /// attributes.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="parent"></param>
        public override void WriteXmlElements(XmlDocument doc, XmlElement parent)
        {
            var me = doc.CreateElement("", Name, doc.NamespaceURI);
            foreach (string s in _atts.Keys)
            {
                var attribute = _atts[s];
                if (attribute != null)
                {
                    if (s == "style")
                    {
                        WriteStyle(doc, me, attribute);
                    }
                    else if (s == "transform")
                    {
                        WriteTransform(doc, me, attribute);
                    }
                    else if (s.Contains(":"))
                    {
                        var s1 = s.Split(':');
                        me.SetAttribute(s1[1], doc.NamespaceURI, attribute.ToString());
                    }
                    else
                    {
                        me.SetAttribute(s, doc.NamespaceURI, attribute.ToString());
                    }
                }
            }

            foreach (SvgElement el in _children)
            {
                el.WriteXmlElements(doc, me);
            }

            if (parent == null)
                doc.AppendChild(me);
            else
                parent.AppendChild(me);
        }

        private static void WriteStyle(XmlDocument doc, XmlElement me, object o)
        {
            if (o.GetType() != typeof(SvgStyle))
            {
                me.SetAttribute("style", doc.NamespaceURI, o.ToString());
                return;
            }

            var style = (SvgStyle)o;

            /*
            foreach(string s in style.Keys)
            {
                me.SetAttribute(s, doc.NamespaceURI, style.Get(s).ToString());
            }
            */

            me.SetAttribute("style", doc.NamespaceURI, style.ToString());

            doc.CreateEntityReference("pingu");
        }

        private static void WriteTransform(XmlDocument doc, XmlElement me, object o)
        {
            //if (o.GetType() != typeof(SvgTransformList))
            //{
            me.SetAttribute("transform", doc.NamespaceURI, o.ToString());
            return;
            //}
        }
    }
}
