using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Drawing;
using System.Xml;

namespace KMLib
{
    public abstract class AXmlSerializable : IXmlSerializable
    {
        #region IXmlSerializable Members

        public virtual System.Xml.Schema.XmlSchema GetSchema() {
            return null;
        }

        public abstract void ReadXml(XmlReader r);
        public abstract void WriteXml(XmlWriter w);

        #endregion
    }

    public abstract class AXmlWrapper : AXmlSerializable
    {
        public override void ReadXml(XmlReader r) {
            string str = r.ReadElementContentAsString();
            Deserialize(str);
        }

        public override void WriteXml(XmlWriter w) {
            w.WriteString(Serialize());
        }

        public abstract string Serialize();

        public abstract void Deserialize(string str);
    }

    public class KmlDouble
    {
        private double? value;

    }

    public class IntBool : AXmlWrapper
    {
        private bool isOn;

        public override string Serialize()
        {
            return (isOn) ? "1" : "0";
        }

        public override void Deserialize(string str)
        {
            isOn = (str == "1");
        }

        public static implicit operator bool(IntBool comp)
        {
            return comp.isOn;
        }

        public static implicit operator IntBool(bool comp)
        {
            IntBool ans = new IntBool();
            ans.isOn = comp;
            return ans;
        }
    }

    public class ColorKML : AXmlWrapper
    {
        private Color myCol;

        public override string Serialize()
        {
            //aabbggrr
            return ByteToHex(myCol.A) + ByteToHex(myCol.B) + ByteToHex(myCol.G) + ByteToHex(myCol.R);
        }

        private string ByteToHex(byte b)
        {
            string hex = b.ToString("X");
            if (hex.Length == 1) {
                return "0" + hex;
            }
            return hex;
        }

        public override void Deserialize(string str)
        {
            myCol = Color.White;//===NOT SUPPORTED
        }

        public static implicit operator Color(ColorKML comp)
        {
            return comp.myCol;
        }

        public static implicit operator ColorKML(Color comp)
        {
            ColorKML ans = new ColorKML();
            ans.myCol = comp;
            return ans;
        }
    }
}
