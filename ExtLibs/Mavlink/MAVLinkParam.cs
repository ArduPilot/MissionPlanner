using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

public partial class MAVLink
{
    public class MAVLinkParam
    {
        /// <summary>
        /// Paramater name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Value of paramter as a double
        /// </summary>

        public double Value
        {
            get
            {
                return GetValue();
            }
            set
            {
                SetValue(value);
            }
        }
        /// <summary>
        /// Over the wire storage format
        /// </summary>
        public MAV_PARAM_TYPE Type { get; set; }

        private MAV_PARAM_TYPE _typeap = 0;
        public MAV_PARAM_TYPE TypeAP {
            get 
            { 
                if (_typeap != 0) 
                    return _typeap;
                return Type;
            }
            set
            {
                _typeap = value;
                
            }
        }

        byte uint8_value { get { return data[0]; } }
        sbyte int8_value { get { return (sbyte)data[0]; } }
        ushort uint16_value { get { return BitConverter.ToUInt16(data, 0); } }
        short int16_value { get { return BitConverter.ToInt16(data, 0); } }
        UInt32 uint32_value { get { return BitConverter.ToUInt32(data, 0); } }
        Int32 int32_value { get { return BitConverter.ToInt32(data, 0); } }
        [IgnoreDataMember]
        public float float_value { get { return BitConverter.ToSingle(data, 0); } }

        byte[] _data = new byte[4];

        [IgnoreDataMember]
        public byte[] data
        {
            get { return _data; }
            set
            {
                _data = value;
                Array.Resize(ref _data, 4);
            }
        }

        /// <summary>
        /// used as a generic input to type the input data
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public MAVLinkParam(string name, double value, MAV_PARAM_TYPE type)
        {
            Name = name;
            Type = type;
            Value = value;
        }

        /// <summary>
        /// Used to set Ardupilot Params
        /// </summary>
        /// <param name="name"></param>
        /// <param name="inputwire"></param>
        /// <param name="type"></param>
        /// <param name="typeap"></param>
        public MAVLinkParam(string name, byte[] inputwire, MAV_PARAM_TYPE type, MAV_PARAM_TYPE typeap)
        {
            Name = name;
            Type = type;
            TypeAP = typeap;
            Array.Copy(inputwire, _data, 4);
        }

        public double GetValue()
        {
            switch (Type)
            {
                case MAV_PARAM_TYPE.UINT8:
                    return (double)uint8_value;
                case MAV_PARAM_TYPE.INT8:
                    return (double)int8_value;
                case MAV_PARAM_TYPE.UINT16:
                    return (double)uint16_value;
                case MAV_PARAM_TYPE.INT16:
                    return (double)int16_value;
                case MAV_PARAM_TYPE.UINT32:
                    return (double)uint32_value;
                case MAV_PARAM_TYPE.INT32:
                    return (double)int32_value;
                case MAV_PARAM_TYPE.REAL32:
                    return (double)float_value;
            }

            throw new FormatException("invalid type");
        }

        public void SetValue(double input)
        {
            switch (Type)
            {
                case MAV_PARAM_TYPE.UINT8:
                    data = BitConverter.GetBytes((byte)input);
                    Array.Resize(ref _data, 4);
                    break;
                case MAV_PARAM_TYPE.INT8:
                    data = BitConverter.GetBytes((sbyte)input);
                    Array.Resize(ref _data, 4);
                    break;
                case MAV_PARAM_TYPE.UINT16:
                    data = BitConverter.GetBytes((ushort)input);
                    Array.Resize(ref _data, 4);
                    break;
                case MAV_PARAM_TYPE.INT16:
                    data = BitConverter.GetBytes((short)input);
                    Array.Resize(ref _data, 4);
                    break;
                case MAV_PARAM_TYPE.UINT32:
                    data = BitConverter.GetBytes((UInt32)input);
                    break;
                case MAV_PARAM_TYPE.INT32:
                    data = BitConverter.GetBytes((Int32)input);
                    break;
                case MAV_PARAM_TYPE.REAL32:
                    data = BitConverter.GetBytes((float)input);
                    break;
            }
        }

        public static explicit operator byte(MAVLinkParam v)
        {
            return (byte)v.Value;
        }

        public static explicit operator sbyte(MAVLinkParam v)
        {
            return (sbyte)v.Value;
        }

        public static explicit operator short(MAVLinkParam v)
        {
            return (short)v.Value;
        }

        public static explicit operator ushort(MAVLinkParam v)
        {
            return (ushort)v.Value;
        }

        public static explicit operator int(MAVLinkParam v)
        {
            return (int)v.Value;
        }

        public static explicit operator uint(MAVLinkParam v)
        {
            return (uint)v.Value;
        }

        public static explicit operator float (MAVLinkParam v)
        {
            return (float)v.Value;
        }

        public static explicit operator double(MAVLinkParam v)
        {
            return (double)v.Value;
        }

        public override string ToString()
        {
            if (Type == MAV_PARAM_TYPE.REAL32)
                return ((float)this).ToString();
            return Value.ToString();
        }
    }
}
