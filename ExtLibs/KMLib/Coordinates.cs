using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using Core;
using Core.Geometry;
using Core.Utils;

namespace KMLib
{
    public class Coordinates : List<Point3D>, IXmlSerializable
    {
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

        public virtual string Serialize()
        {
            string ans = "";
            for (int i = 0; i < Count; i++) {
                ans += this[i].Serialize();
                if (i < Count - 1) {
                    ans += " ";
                }
            }
            return ans;
        }

        public virtual void Deserialize(string str)
        {
            str = StringUtils.RemoveExcessWhiteSpace(str);
            List<string> bits = StringUtils.SplitToList(str, " ");
            Clear();
            for (int i = 0; i < bits.Count; i++) {               
                Add(Point3D.MakePointFromStr(bits[i]));
            }
        }

    }
}
