#region netDxf library, Copyright (C) 2009-2018 Daniel Carvajal (haplokuon@gmail.com)

//                        netDxf library
// Copyright (C) 2009-2018 Daniel Carvajal (haplokuon@gmail.com)
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace netDxf.IO
{
    internal class TextCodeValueWriter :
        ICodeValueWriter
    {
        #region private fields

        private readonly TextWriter writer;
        private long currentPosition;
        private short dxfCode;
        private object dxfValue;

        #endregion

        #region constructors

        public TextCodeValueWriter(TextWriter writer)
        {
            this.writer = writer;
            this.currentPosition = 0;
            this.dxfCode = 0;
            this.dxfValue = null;
        }

        #endregion

        #region public properties

        public short Code
        {
            get { return this.dxfCode; }
        }

        public object Value
        {
            get { return this.dxfValue; }
        }

        public long CurrentPosition
        {
            get { return this.currentPosition; }
        }

        #endregion

        #region public methods

        public void Write(short code, object value)
        {
            this.dxfCode = code;
            this.writer.WriteLine(code);
            this.currentPosition += 1;

            if (code >= 0 && code <= 9) // string
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if (code >= 10 && code <= 39) // double precision 3D point value
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double) value);
            }
            else if (code >= 40 && code <= 59) // double precision floating point value
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double) value);
            }
            else if (code >= 60 && code <= 79) // 16-bit integer value
            {
                Debug.Assert(value is short, "Incorrect value type.");
                this.WriteShort((short) value);
            }
            else if (code >= 90 && code <= 99) // 32-bit integer value
            {
                Debug.Assert(value is int, "Incorrect value type.");
                this.WriteInt((int) value);
            }
            else if (code == 100) // string (255-character maximum; less for Unicode strings)
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if (code == 101) // string (255-character maximum; less for Unicode strings). This code is undocumented and seems to affect only the AcdsData in dxf version 2013
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if (code == 102) // string (255-character maximum; less for Unicode strings)
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if (code == 105) // string representing hexadecimal (hex) handle value
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if (code >= 110 && code <= 119) // double precision floating point value
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double) value);
            }
            else if (code >= 120 && code <= 129) // double precision floating point value
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double) value);
            }
            else if (code >= 130 && code <= 139) // double precision floating point value
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double) value);
            }
            else if (code >= 140 && code <= 149) // double precision scalar floating-point value
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double) value);
            }
            else if (code >= 160 && code <= 169) // 64-bit integer value
            {
                Debug.Assert(value is long, "Incorrect value type.");
                this.WriteLong((long) value);
            }
            else if (code >= 170 && code <= 179) // 16-bit integer value
            {
                Debug.Assert(value is short, "Incorrect value type.");
                this.WriteShort((short) value);
            }
            else if (code >= 210 && code <= 239) // double precision scalar floating-point value
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double) value);
            }
            else if (code >= 270 && code <= 279) // 16-bit integer value
            {
                Debug.Assert(value is short, "Incorrect value type.");
                this.WriteShort((short) value);
            }
            else if (code >= 280 && code <= 289) // 16-bit integer value
            {
                Debug.Assert(value is short, "Incorrect value type.");
                this.WriteShort((short) value);
            }
            else if (code >= 290 && code <= 299) // byte (boolean flag value)
            {
                Debug.Assert(value is bool, "Incorrect value type.");
                this.WriteBool((bool) value);
            }
            else if (code >= 300 && code <= 309) // arbitrary text string
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if (code >= 310 && code <= 319) // string representing hex value of binary chunk
            {
                Debug.Assert(value is byte[], "Incorrect value type.");
                this.WriteBytes((byte[]) value);
            }
            else if (code >= 320 && code <= 329) // string representing hex handle value
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if (code >= 330 && code <= 369) // string representing hex object IDs
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if (code >= 370 && code <= 379) // 16-bit integer value
            {
                Debug.Assert(value is short, "Incorrect value type.");
                this.WriteShort((short) value);
            }
            else if (code >= 380 && code <= 389) // 16-bit integer value
            {
                Debug.Assert(value is short, "Incorrect value type.");
                this.WriteShort((short) value);
            }
            else if (code >= 390 && code <= 399) // string representing hex handle value
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if (code >= 400 && code <= 409) // 16-bit integer value
            {
                Debug.Assert(value is short, "Incorrect value type.");
                this.WriteShort((short) value);
            }
            else if (code >= 410 && code <= 419) // string
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if (code >= 420 && code <= 429) // 32-bit integer value
            {
                Debug.Assert(value is int, "Incorrect value type.");
                this.WriteInt((int) value);
            }
            else if (code >= 430 && code <= 439) // string
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if (code >= 440 && code <= 449) // 32-bit integer value
            {
                Debug.Assert(value is int, "Incorrect value type.");
                this.WriteInt((int) value);
            }
            else if (code >= 450 && code <= 459) // 32-bit integer value
            {
                Debug.Assert(value is int, "Incorrect value type.");
                this.WriteInt((int) value);
            }
            else if (code >= 460 && code <= 469) // double-precision floating-point value
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double) value);
            }
            else if (code >= 470 && code <= 479) // string
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if (code >= 480 && code <= 481) // string representing hex handle value
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if (code == 999) // comment (string)
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if (code >= 1010 && code <= 1059) // double-precision floating-point value
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double) value);
            }
            else if (code >= 1000 && code <= 1003) // string (same limits as indicated with 0-9 code range)
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if (code == 1004) // string representing hex value of binary chunk
            {
                Debug.Assert(value is byte[], "Incorrect value type.");
                this.WriteBytes((byte[]) value);
            }
            else if (code >= 1005 && code <= 1009) // string (same limits as indicated with 0-9 code range)
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if (code >= 1060 && code <= 1070) // 16-bit integer value
            {
                Debug.Assert(value is short, "Incorrect value type.");
                this.WriteShort((short) value);
            }
            else if (code == 1071) // 32-bit integer value
            {
                Debug.Assert(value is int, "Incorrect value type.");
                this.WriteInt((int) value);
            }
            else
            {
                throw new Exception(string.Format("Code {0} not valid at line {1}", this.dxfCode, this.currentPosition));
            }

            this.dxfValue = value;
            this.currentPosition += 1;
        }

        public void WriteByte(byte value)
        {
            this.writer.WriteLine(value.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteBytes(byte[] value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte v in value)
            {
                sb.Append(string.Format("{0:X2}", v));
            }
            this.writer.WriteLine(sb.ToString());
        }

        public void WriteShort(short value)
        {
            this.writer.WriteLine(value.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteInt(int value)
        {
            this.writer.WriteLine(value.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteLong(long value)
        {
            this.writer.WriteLine(value.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteBool(bool value)
        {
            this.writer.WriteLine(value ? 1 : 0);
        }

        public void WriteDouble(double value)
        {
            // float values always use the dot as the decimal separator
            this.writer.WriteLine(value.ToString("0.0###############", CultureInfo.InvariantCulture));
        }

        public void Flush()
        {
            this.writer.Flush();
        }

        public void WriteString(string value)
        {
            this.writer.WriteLine(value);
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.dxfCode, this.dxfValue);
        }
        #endregion       
    }
}