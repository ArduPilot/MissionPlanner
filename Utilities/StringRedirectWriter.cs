using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ArdupilotMega.Utilities
{
    public delegate void StringWrittenEvent(object sender, string writtenString);

    /// <summary>
    /// This class only pretends to be a string writer
    /// when in fact it raises an event whenever a string is written
    /// with the written string as an argument. The string is then
    /// discarded.
    /// </summary>
    class StringRedirectWriter : StreamWriter
    {
        public event StringWrittenEvent StringWritten;

        public override void Write(bool value)
        {
            if (StringWritten != null)
                StringWritten(this, value.ToString());
        }
        public override void Write(char[] buffer)
        {
            if (StringWritten != null)
                StringWritten(this, new string(buffer));
        }

        public override void Write(char value)
        {
            if (StringWritten != null)
                StringWritten(this, value.ToString());
        }
        public override void Write(char[] buffer, int index, int count)
        {
            if (StringWritten != null)
                StringWritten(this, (new string(buffer)).Substring(index,count));
        }

        public override void Write(decimal value)
        {
            if (StringWritten != null)
                StringWritten(this, value.ToString());
        }
        public override void Write(double value)
        {
            if (StringWritten != null)
                StringWritten(this, value.ToString());
        }
        public override void Write(float value)
        {
            if (StringWritten != null)
                StringWritten(this, value.ToString());
        }
        public override void Write(int value)
        {
            if (StringWritten != null)
                StringWritten(this, value.ToString());
        }
        public override void Write(long value)
        {
            if (StringWritten != null)
                StringWritten(this, value.ToString());
        }
        public override void Write(object value)
        {
            if (StringWritten != null)
                StringWritten(this, value.ToString());
        }
        public override void Write(string format, object arg0)
        {
            if (StringWritten != null)
                StringWritten(this, String.Format(format, arg0));
        }
        public override void Write(string format, object arg0, object arg1)
        {
            if (StringWritten != null)
                StringWritten(this, String.Format(format, arg0, arg1));
        }
        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            if (StringWritten != null)
                StringWritten(this, String.Format(format, arg0, arg1, arg2));
        }
        public override void Write(string format, params object[] arg)
        {
            if (StringWritten != null)
                StringWritten(this, String.Format(format, arg));
        }
        public override void Write(string value)
        {
            if (StringWritten != null)
                StringWritten(this, value.ToString());
        }
        public override void Write(uint value)
        {
            if (StringWritten != null)
                StringWritten(this, value.ToString());
        }
        public override void Write(ulong value)
        {
            if (StringWritten != null)
                StringWritten(this, value.ToString());
        }


        public override void WriteLine(bool value)
        {
            if (StringWritten != null)
                StringWritten(this, value.ToString() + "\r\n");
        }
        public override void WriteLine(char[] buffer)
        {
            if (StringWritten != null)
                StringWritten(this, new string(buffer) + "\r\n");
        }

        public override void WriteLine(char value)
        {
            if (StringWritten != null)
                StringWritten(this, value.ToString() + "\r\n");
        }
        public override void WriteLine(char[] buffer, int index, int count)
        {
            if (StringWritten != null)
                StringWritten(this, (new string(buffer)).Substring(index, count) + "\r\n");
        }

        public override void WriteLine(decimal value)
        {
            if (StringWritten != null)
                StringWritten(this, value.ToString() + "\r\n");
        }
        public override void WriteLine(double value)
        {
            if (StringWritten != null)
                StringWritten(this, value.ToString() + "\r\n");
        }
        public override void WriteLine(float value)
        {
            if (StringWritten != null)
                StringWritten(this, value.ToString() + "\r\n");
        }
        public override void WriteLine(int value)
        {
            if (StringWritten != null)
                StringWritten(this, value.ToString() + "\r\n");
        }
        public override void WriteLine(long value)
        {
            if (StringWritten != null)
                StringWritten(this, value.ToString() + "\r\n");
        }
        public override void WriteLine(object value)
        {
            if (StringWritten != null)
                StringWritten(this, value.ToString() + "\r\n");
        }
        public override void WriteLine(string format, object arg0)
        {
            if (StringWritten != null)
                StringWritten(this, String.Format(format, arg0) + "\r\n");
        }
        public override void WriteLine(string format, object arg0, object arg1)
        {
            if (StringWritten != null)
                StringWritten(this, String.Format(format, arg0, arg1) + "\r\n");
        }
        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            if (StringWritten != null)
                StringWritten(this, String.Format(format, arg0, arg1, arg2) + "\r\n");
        }
        public override void WriteLine(string format, params object[] arg)
        {
            if (StringWritten != null)
                StringWritten(this, String.Format(format, arg) + "\r\n");
        }
        public override void WriteLine(string value)
        {
            if (StringWritten != null)
                StringWritten(this, value.ToString() + "\r\n");
        }
        public override void WriteLine(uint value)
        {
            if (StringWritten != null)
                StringWritten(this, value.ToString() + "\r\n");
        }
        public override void WriteLine(ulong value)
        {
            if (StringWritten != null)
                StringWritten(this, value.ToString() + "\r\n");
        }
        
        
    }
}
