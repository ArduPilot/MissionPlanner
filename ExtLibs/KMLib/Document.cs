using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.ComponentModel;
using Core.Xml;
using KMLib.Abstract;

namespace KMLib
{
    public class ADoc
    {
        private Style m_Style;
        public Style Style
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

        private string m_description;
        public string description
        {
            get
            {
                return m_description;
            }
            set
            {
                m_description = value;
            }
        }

        private IntBool m_Visible;
        [XmlElement("visibility")]
        public IntBool Visible
        {
            get
            {
                return m_Visible;
            }
            set
            {
                m_Visible = value;
            }
        }

        private string m_name;
        public string name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        private LookAt m_LookAt;
        public LookAt LookAt
        {
            get
            {
                return m_LookAt;
            }
            set
            {
                m_LookAt = value;
            }
        }
    }
}
