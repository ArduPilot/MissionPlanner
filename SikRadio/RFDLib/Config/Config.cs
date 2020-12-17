using System;
using System.Collections.Generic;

namespace RFDLib.Config
{
    public class TSettingDescription
    {
        public readonly string Name;
        public readonly string Description;

        public TSettingDescription(string Name, string Description)
        {
            this.Name = Name;
            this.Description = Description;
        }
    }

    public abstract class TSettingDescriptor : TSettingDescription
    {
        public TSettingDescriptor(string Name, string Description)
            : base(Name, Description)
        {
        }

        public abstract ISetting CreateNewSetting();
    }

    public interface ISetting
    {
        TSettingDescriptor GetDescriptor();
    }

    public abstract class TSetting<T> : ISetting
    {
        TSettingDescriptor _Descriptor;
        protected T _Value;

        public TSetting(TSettingDescriptor Descriptor, T Default)
        {
            _Descriptor = Descriptor;
            _Value = Default;
        }

        public TSettingDescriptor GetDescriptor()
        {
            return _Descriptor;
        }

        public T Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }
    }

    public abstract class TMultiChoiceSettingDescriptor : TSettingDescriptor
    {
        public TMultiChoiceSettingDescriptor(string Name, string Description)
            : base(Name, Description)
        {
        }

        public abstract IEnumerable<TOption> GetOptions();

        public abstract class TOption
        {
            public abstract string Name
            {
                get;
            }

            public abstract bool IsSameAs(TOption Other);
        }
    }

    public abstract class TMultiChoiceSetting<T> : TSetting<T>
    {
        public TMultiChoiceSetting(TMultiChoiceSettingDescriptor Descriptor, T Default)
            : base(Descriptor, Default)
        {
        }
    }

    public abstract class TBoolSettingDescriptor : TSettingDescriptor
    {
        public TBoolSettingDescriptor(string Name, string Description)
            : base(Name, Description)
        {
        }
    }

    public abstract class TIntegerSettingDescriptor : TSettingDescriptor
    {
        public TIntegerSettingDescriptor(string Name, string Description)
            : base(Name, Description)
        {
        }
    }


    /*public abstract class TTextSettingDescriptor : TSettingDescriptor
    {

    }*/
}