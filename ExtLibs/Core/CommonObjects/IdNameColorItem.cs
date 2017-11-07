using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Core.CommonObjects
{
    public interface IIdItem
    {
        string Id { get; set; }
    }

    public interface INameItem
    {
        string Name { get; set; }
    }

    public interface IDisplayNameItem
    {
        string DisplayName { get; set; }
    }

    public interface IIdNameItem : IIdItem, INameItem
    {        
    }

    public interface IIdNameColorItem : IIdNameItem
    {
        Color Color { get; set; }
    }

    public abstract class AIdItem : IIdItem
    {
        public static int CompareIds(IIdItem a, IIdItem b)
        {
            return a.Id.CompareTo(b.Id);
        }

        protected string m_Id;
        [XmlAttribute(), Category("Appearance")]
        public virtual string Id
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

        public void MatchId(IIdItem item)
        {
            m_Id = item.Id;            
        }
    }

    public abstract class AIdNameItem : AIdItem, IIdNameItem
    {
        public static int CompareNames(IIdNameItem a, IIdNameItem b)
        {
            if (a.Name == null) {
                if (b.Name != null) {
                    return 1;
                } else {
                    return AIdItem.CompareIds(a, b);
                }
            }
            return a.Name.CompareTo(b.Name);
        }

        protected string m_Name;

        [XmlAttribute(), Category("Appearance")]
        public virtual string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }

        [XmlIgnore()]
        public string NameOrId
        {
            get
            {
                if (m_Name == null) {
                    return m_Id;
                }
                return m_Name;
            }
        }

        protected virtual void OnIdChange(string oldId) { }

        public void MatchIdName(IIdNameItem item)
        {
            m_Id = item.Id;
            m_Name = item.Name;            
        }
    }

    public abstract class AIdNameColorItem : AIdNameItem, IIdNameColorItem
    {
        public AIdNameColorItem(string id, string name, Color color)
        {
            m_Id = id;
            m_Name = name;
            m_Color = color;
        }
        public AIdNameColorItem() {}

        protected Color m_Color;

        [XmlIgnore(), Category("Appearance")]
        public virtual Color Color
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
        
        [XmlAttribute("Color"), Browsable(false)]
        public string Color_XML
        {
            get
            {
                return ColorTranslator.ToHtml(m_Color);
            }
            set
            {
                Color = ColorTranslator.FromHtml(value);
            }
        }
        
        public void MatchIdNameColor(IIdNameColorItem item)
        {
            m_Id = item.Id;
            m_Name = item.Name;
            m_Color = item.Color;
        }

        /*
        public override bool Equals(object obj)
        {
            AIdNameColorItem other = obj as AIdNameColorItem;
            if (other.GetType() != this.GetType()) return false;
            return (m_Id == other.Id && m_Name == other.Name && m_Color == other.Color );
        }

        public override int GetHashCode()
        {
            return string.Concat(m_Id, m_Name, m_Color.Name).GetHashCode();
        }*/
    }

}
