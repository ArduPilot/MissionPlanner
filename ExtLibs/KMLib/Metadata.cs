using System;
using System.Collections.Generic;
using System.Text;

namespace KMLib
{
    public class Metadata : AXmlWrapper
    {
        private string m_Text;
        public string Text {
            get {
                return m_Text;
            }
            set {
                m_Text = value;
            }
        }

        public override string Serialize() {
            return m_Text;
        }

        public override void Deserialize(string str) {
            m_Text = str;
        }
    }
}
