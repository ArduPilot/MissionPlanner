using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public partial class MAVLink
{
    public class MAVLinkParamList: List<MAVLinkParam>
    {
        object locker = new object();

        public MAVLinkParam this[string name]
        { 
            get
            {
                lock (locker)
                {
                    foreach (var item in this)
                    {
                        if (item.Name == name)
                            return item;
                    }
                }

                return null;
            }

            set
            {
                int index = 0;
                lock (locker)
                {
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
        }

        public IEnumerable<string> Keys
        {
            get
            {
                lock (locker)
                {
                    foreach (MAVLinkParam item in this)
                    {
                        yield return item.Name;
                    }
                }
            }
        }

        public bool ContainsKey(string v)
        {
            lock (locker)
            {
                foreach (MAVLinkParam item in this)
                {
                    if (item.Name == v)
                        return true;
                }
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
