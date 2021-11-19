using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public partial class MAVLink
{
    public class MAVLinkParamList : List<MAVLinkParam>, INotifyPropertyChanged
    {
        object locker = new object();

        public int TotalReported { get; set; }

        public int TotalReceived
        {
            get { return this.Count; }
        }

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
                            OnPropertyChanged();
                            return;
                        }

                        index++;
                    }

                    base.Add(value);
                }
            }
        }

        // Only works if one param from the name list if found, will fail if multiple list items are found
        // for use in cases of param conversion where the two names will not coexist
        public MAVLinkParam this[string[] names]
        {
            get
            {
                MAVLinkParam item = null;
                foreach (var s in names)
                {
                    MAVLinkParam new_item = this[s];
                    if (new_item != null)
                    {
                        if (item != null)
                        {
                            // found multiple items in list
                            return null;
                        }
                        item = new_item;
                    }
                }
                return item;
            }

            set
            {
                MAVLinkParam item = this[names];
                if (item != null)
                {
                    item = value;
                }
            }

        }

        public IEnumerable<string> Keys
        {
            get
            {
                foreach (MAVLinkParam item in this.ToArray())
                {
                    yield return item.Name;
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

        public new void Clear()
        {
            lock (locker)
            {
                TotalReported = 0;
                base.Clear();
            }
        }

        public new void Add(MAVLinkParam item)
        {
            lock (locker)
            {
                this[item.Name] = item;
            }
        }

        public new void AddRange(IEnumerable<MAVLinkParam> collection)
        {
            lock (locker)
            {
                base.AddRange(collection);
                OnPropertyChanged();
            }
        }

        public static implicit operator Dictionary<string, double>(MAVLinkParamList list)
        {
            var copy = new Dictionary<string, double>();
            lock (list.locker)
            {
                foreach (MAVLinkParam item in list.ToArray())
                {
                    copy[item.Name] = item.Value;
                }
            }

            return copy;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
