using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace KMLib.Abstract
{
    
    public abstract class AObject
    {
        private string m_id;
        public string id {
            get {
                return m_id;
            }
            set {
                m_id = value;
            }
        }
    }
}
