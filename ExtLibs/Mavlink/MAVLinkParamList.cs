using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public partial class MAVLink
{
    public class MAVLinkParamList: List<MAVLinkParam>
    {
        public MAVLinkParam this[string name]
        { 
            get
            {
                foreach (var item in this)
                {
                    if (item.Name == name)
                        return item;
                }

                return null;
            }

            set
            {
                int index = 0;
                foreach (var item in this)
                {
                    if (item.Name == name)
                    {
                        this[index] = value;
                        return;
                    }

                    index++;
                }

                this.Add(value);
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                foreach (MAVLinkParam item in this)
                {
                    yield return item.Name;
                }
            }
        }

        public bool ContainsKey(string v)
        {
            foreach (MAVLinkParam item in this)
            {
                if (item.Name == v)
                    return true;
            }

            return false;
        }

        public static implicit operator Hashtable(MAVLinkParamList list)
        {
            Hashtable copy = new Hashtable();

            foreach (MAVLinkParam item in list)
            {
                copy[item.Name] = item.Value;
            }

            return copy;
        }            
    }
}
