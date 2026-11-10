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

        // The index of the first param of each name asked for, -1 when there is none. The list can also
        // be changed through List<T>, behind this class, so the answers are only kept for as long as the
        // list's own change counter stays the same.
        readonly Dictionary<string, int> nameIndex = new Dictionary<string, int>();
        int nameIndexVersion = -1;

        static readonly Func<List<MAVLinkParam>, int> listVersion = GetListVersion();

        /// <summary>
        /// reads the change counter of a List, null where the runtime's List has none we can read
        /// </summary>
        static Func<List<MAVLinkParam>, int> GetListVersion()
        {
            try
            {
                var field = typeof(List<MAVLinkParam>).GetField("_version",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (field == null || field.FieldType != typeof(int))
                    return null;

                try
                {
                    var list = System.Linq.Expressions.Expression.Parameter(typeof(List<MAVLinkParam>), "list");
                    return System.Linq.Expressions.Expression.Lambda<Func<List<MAVLinkParam>, int>>(
                        System.Linq.Expressions.Expression.Field(list, field), list).Compile();
                }
                catch
                {
                    return list => (int) field.GetValue(list);
                }
            }
            catch
            {
                return null;
            }
        }

        int ScanForName(string name)
        {
            int count = Count;
            for (int index = 0; index < count; index++)
            {
                if (base[index].Name == name)
                    return index;
            }

            return -1;
        }

        /// <summary>
        /// index of the first param with this name, or -1. The caller holds the reader or writer lock
        /// </summary>
        int IndexOfName(string name)
        {
            if (listVersion == null || name == null)
                return ScanForName(name);

            lock (nameIndex)
            {
                int version = listVersion(this);
                if (version != nameIndexVersion)
                {
                    nameIndex.Clear();
                    nameIndexVersion = version;
                }

                int index;
                if (!nameIndex.TryGetValue(name, out index))
                {
                    index = ScanForName(name);
                    nameIndex[name] = index;
                }

                return index;
            }
        }

        public MAVLinkParam this[string name]
        {
            get
            {
                try
                {
                    locker.AcquireReaderLock(1000);
                    int index = IndexOfName(name);
                    if (index >= 0)
                        return base[index];
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
                try
                {
                    locker.AcquireWriterLock(1000);
                    int index = IndexOfName(name);
                    if (index >= 0)
                    {
                        base[index] = value;
                        OnPropertyChanged();
                        return;
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
                if (IndexOfName(v) >= 0)
                    return true;
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
