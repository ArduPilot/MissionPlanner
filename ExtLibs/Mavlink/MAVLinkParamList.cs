using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

public partial class MAVLink
{
    public class MAVLinkParamList : List<MAVLinkParam>, INotifyPropertyChanged
    {
        ReaderWriterLock locker = new ReaderWriterLock();

        public int TotalReported { get; set; }

        public int TotalReceived
        {
            get { return this.Count; }
        }

        public MAVLinkParam this[string name]
        {
            get
            {
                try
                {
                    locker.AcquireReaderLock(1000);
                    foreach (var item in this)
                    {
                        if (item.Name == name)
                            return item;
                    }
                }
                finally
                {
                    if (locker.IsReaderLockHeld)
                        locker.ReleaseReaderLock();
                }

                return null;
            }

            set
            {
                int index = 0;
                try
                {
                    locker.AcquireWriterLock(1000);
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
                finally
                {
                    locker.ReleaseWriterLock();
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
            try
            {
                locker.AcquireReaderLock(1000);
                foreach (MAVLinkParam item in this)
                {
                    if (item.Name == v)
                        return true;
                }
            }
            finally
            {
                if (locker.IsReaderLockHeld)
                    locker.ReleaseReaderLock();
            }

            return false;
        }

        public new void Clear()
        {
            try
            {
                locker.AcquireWriterLock(1000);
                TotalReported = 0;
                base.Clear();
            }
            finally
            {
                locker.ReleaseWriterLock();
            }
        }

        public new void Add(MAVLinkParam item)
        {
            try
            {
                locker.AcquireWriterLock(1000);
                this[item.Name] = item;
            }
            finally
            {
                locker.ReleaseWriterLock();
            }
        }

        public new void AddRange(IEnumerable<MAVLinkParam> collection)
        {
            try
            {
                locker.AcquireWriterLock(1000);
                base.AddRange(collection);
                OnPropertyChanged();
            }
            finally
            {
                locker.ReleaseWriterLock();
            }
        }

        public static implicit operator Dictionary<string, double>(MAVLinkParamList list)
        {
            var copy = new Dictionary<string, double>();
            try
            {
                list.locker.AcquireReaderLock(1000);
                foreach (MAVLinkParam item in list.ToArray())
                {
                    copy[item.Name] = item.Value;
                }
            }
            finally
            {
                if (list.locker.IsReaderLockHeld)
                    list.locker.ReleaseReaderLock();
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
