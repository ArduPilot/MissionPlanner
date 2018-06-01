using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MissionPlanner.Utilities
{
    public delegate void StringWrittenEvent(object sender, string writtenString);

    /// <summary>
    /// This class only pretends to be a string writer
    /// when in fact it raises an event whenever a string is written
    /// with the written string as an argument. The string is then
    /// discarded.
    /// </summary>
    class StringRedirectWriter : TextWriter
    {
        public override Encoding Encoding
        {
            get { return UTF8Encoding.UTF8; }
        }

        private StringBuilder internalString = new StringBuilder();

        private void writeString(string s)
        {
            lock (internalString)
            {
                int lengthmax = 1024*1024;

                if (internalString.Length > lengthmax)
                {
                    // add 5kb free
                    string temp = internalString.ToString().Substring(internalString.Length - lengthmax + 5000);
                    internalString.Clear();
                    internalString.Append(temp);
                }
                internalString.Append(s);
            }
        }

        public string RetrieveWrittenString()
        {
            lock (internalString)
            {
                string retrieved = internalString.ToString();
                internalString.Length = 0;
                return retrieved;
            }
        }

        public override void Write(bool value)
        {
            writeString(value.ToString());
        }

        public override void Write(char[] buffer)
        {
            writeString(new string(buffer));
        }

        public override void Write(char value)
        {
            writeString(value.ToString());
        }

        public override void Write(char[] buffer, int index, int count)
        {
            writeString((new string(buffer)).Substring(index, count));
        }

        public override void Write(decimal value)
        {
            writeString(value.ToString());
        }

        public override void Write(double value)
        {
            writeString(value.ToString());
        }

        public override void Write(float value)
        {
            writeString(value.ToString());
        }

        public override void Write(int value)
        {
            writeString(value.ToString());
        }

        public override void Write(long value)
        {
            writeString(value.ToString());
        }

        public override void Write(object value)
        {
            writeString(value.ToString());
        }

        public override void Write(string format, object arg0)
        {
            writeString(String.Format(format, arg0));
        }

        public override void Write(string format, object arg0, object arg1)
        {
            writeString(String.Format(format, arg0, arg1));
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            writeString(String.Format(format, arg0, arg1, arg2));
        }

        public override void Write(string format, params object[] arg)
        {
            writeString(String.Format(format, arg));
        }

        public override void Write(string value)
        {
            writeString(value.ToString());
        }

        public override void Write(uint value)
        {
            writeString(value.ToString());
        }

        public override void Write(ulong value)
        {
            writeString(value.ToString());
        }


        public override void WriteLine(bool value)
        {
            writeString(value.ToString() + "\r\n");
        }

        public override void WriteLine(char[] buffer)
        {
            writeString(new string(buffer) + "\r\n");
        }

        public override void WriteLine(char value)
        {
            writeString(value.ToString() + "\r\n");
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            writeString((new string(buffer)).Substring(index, count) + "\r\n");
        }

        public override void WriteLine(decimal value)
        {
            writeString(value.ToString() + "\r\n");
        }

        public override void WriteLine(double value)
        {
            writeString(value.ToString() + "\r\n");
        }

        public override void WriteLine(float value)
        {
            writeString(value.ToString() + "\r\n");
        }

        public override void WriteLine(int value)
        {
            writeString(value.ToString() + "\r\n");
        }

        public override void WriteLine(long value)
        {
            writeString(value.ToString() + "\r\n");
        }

        public override void WriteLine(object value)
        {
            writeString(value.ToString() + "\r\n");
        }

        public override void WriteLine(string format, object arg0)
        {
            writeString(String.Format(format, arg0) + "\r\n");
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            writeString(String.Format(format, arg0, arg1) + "\r\n");
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            writeString(String.Format(format, arg0, arg1, arg2) + "\r\n");
        }

        public override void WriteLine(string format, params object[] arg)
        {
            writeString(String.Format(format, arg) + "\r\n");
        }

        public override void WriteLine(string value)
        {
            writeString(value.ToString() + "\r\n");
        }

        public override void WriteLine(uint value)
        {
            writeString(value.ToString() + "\r\n");
        }

        public override void WriteLine(ulong value)
        {
            writeString(value.ToString() + "\r\n");
        }

        public override void WriteLine()
        {
            writeString("\r\n");
        }
    }
}