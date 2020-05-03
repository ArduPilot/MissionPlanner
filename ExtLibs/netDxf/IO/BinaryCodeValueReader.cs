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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace netDxf.IO
{
    internal class BinaryCodeValueReader :
        ICodeValueReader
    {
        #region private fields

        private readonly BinaryReader reader;
        private readonly Encoding encoding;
        private short code;
        private object value;

        #endregion

        #region constructors

        public BinaryCodeValueReader(BinaryReader reader, Encoding encoding)
        {
            this.reader = reader;
            this.encoding = encoding;
            byte[] sentinel = this.reader.ReadBytes(22);
            StringBuilder sb = new StringBuilder(18);
            for (int i = 0; i < 18; i++)
            {
                sb.Append((char) sentinel[i]);
            }
            if (sb.ToString() != "AutoCAD Binary DXF")
                throw new ArgumentException("Not a valid binary DXF.");

            this.code = 0;
            this.value = null;
        }

        #endregion

        #region public properties

        public short Code
        {
            get { return this.code; }
        }

        public object Value
        {
            get { return this.value; }
        }

        public long CurrentPosition
        {
            get { return this.reader.BaseStream.Position; }
        }

        #endregion

        #region public methods

        public void Next()
        {
            this.code = this.reader.ReadInt16();
            if (this.code >= 0 && this.code <= 9) // string
                this.value = this.NullTerminatedString();
            else if (this.code >= 10 && this.code <= 39) // double precision 3D point value
                this.value = this.reader.ReadDouble();
            else if (this.code >= 40 && this.code <= 59) // double precision floating point value
                this.value = this.reader.ReadDouble();
            else if (this.code >= 60 && this.code <= 79) // 16-bit integer value
                this.value = this.reader.ReadInt16();
            else if (this.code >= 90 && this.code <= 99) // 32-bit integer value
                this.value = this.reader.ReadInt32();
            else if (this.code == 100) // string (255-character maximum; less for Unicode strings)
                this.value = this.NullTerminatedString();
            else if (this.code == 101) // string (255-character maximum; less for Unicode strings). This code is undocumented and seems to affect only the AcdsData in dxf version 2013
                this.value = this.NullTerminatedString();
            else if (this.code == 102) // string (255-character maximum; less for Unicode strings)
                this.value = this.NullTerminatedString();
            else if (this.code == 105) // string representing hexadecimal (hex) handle value
                this.value = this.NullTerminatedString();
            else if (this.code >= 110 && this.code <= 119) // double precision floating point value
                this.value = this.reader.ReadDouble();
            else if (this.code >= 120 && this.code <= 129) // double precision floating point value
                this.value = this.reader.ReadDouble();
            else if (this.code >= 130 && this.code <= 139) // double precision floating point value
                this.value = this.reader.ReadDouble();
            else if (this.code >= 140 && this.code <= 149) // double precision scalar floating-point value
                this.value = this.reader.ReadDouble();
            else if (this.code >= 160 && this.code <= 169) // 64-bit integer value
                this.value = this.reader.ReadInt64();
            else if (this.code >= 170 && this.code <= 179) // 16-bit integer value
                this.value = this.reader.ReadInt16();
            else if (this.code >= 210 && this.code <= 239) // double precision scalar floating-point value
                this.value = this.reader.ReadDouble();
            else if (this.code >= 270 && this.code <= 279) // 16-bit integer value
                this.value = this.reader.ReadInt16();
            else if (this.code >= 280 && this.code <= 289) // 16-bit integer value
                this.value = this.reader.ReadInt16();
            else if (this.code >= 290 && this.code <= 299) // byte (boolean flag value)
                this.value = this.reader.ReadByte();
            else if (this.code >= 300 && this.code <= 309) // arbitrary text string
                this.value = this.NullTerminatedString();
            else if (this.code >= 310 && this.code <= 319) // string representing hex value of binary chunk
                this.value = this.ReadBinaryData();
            else if (this.code >= 320 && this.code <= 329) // string representing hex handle value
                this.value = this.NullTerminatedString();
            else if (this.code >= 330 && this.code <= 369) // string representing hex object IDs
                this.value = this.NullTerminatedString();
            else if (this.code >= 370 && this.code <= 379) // 16-bit integer value
                this.value = this.reader.ReadInt16();
            else if (this.code >= 380 && this.code <= 389) // 16-bit integer value
                this.value = this.reader.ReadInt16();
            else if (this.code >= 390 && this.code <= 399) // string representing hex handle value
                this.value = this.NullTerminatedString();
            else if (this.code >= 400 && this.code <= 409) // 16-bit integer value
                this.value = this.reader.ReadInt16();
            else if (this.code >= 410 && this.code <= 419) // string
                this.value = this.NullTerminatedString();
            else if (this.code >= 420 && this.code <= 429) // 32-bit integer value
                this.value = this.reader.ReadInt32();
            else if (this.code >= 430 && this.code <= 439) // string
                this.value = this.NullTerminatedString();
            else if (this.code >= 440 && this.code <= 449) // 32-bit integer value
                this.value = this.reader.ReadInt32();
            else if (this.code >= 450 && this.code <= 459) // 32-bit integer value
                this.value = this.reader.ReadInt32();
            else if (this.code >= 460 && this.code <= 469) // double-precision floating-point value
                this.value = this.reader.ReadDouble();
            else if (this.code >= 470 && this.code <= 479) // string
                this.value = this.NullTerminatedString();
            else if (this.code >= 480 && this.code <= 481) // string representing hex handle value
                this.value = this.NullTerminatedString();
            else if (this.code == 999) // comment (string)
                throw new Exception(string.Format("The comment group, 999, is not used in binary DXF files at byte address {0}", this.reader.BaseStream.Position));
            else if (this.code >= 1010 && this.code <= 1059) // double-precision floating-point value
                this.value = this.reader.ReadDouble();
            else if (this.code >= 1000 && this.code <= 1003) // string (same limits as indicated with 0-9 code range)
                this.value = this.NullTerminatedString();
            else if (this.code == 1004) // string representing hex value of binary chunk
                this.value = this.ReadBinaryData();
            else if (this.code >= 1005 && this.code <= 1009) // string (same limits as indicated with 0-9 code range)
                this.value = this.NullTerminatedString();
            else if (this.code >= 1060 && this.code <= 1070) // 16-bit integer value
                this.value = this.reader.ReadInt16();
            else if (this.code == 1071) // 32-bit integer value
                this.value = this.reader.ReadInt32();
            else
                throw new Exception(string.Format("Code {0} not valid at byte address {1}", this.code, this.reader.BaseStream.Position));
        }

        public byte ReadByte()
        {
            return (byte)this.value;
        }

        public byte[] ReadBytes()
        {
            return (byte[])this.value;
        }

        public short ReadShort()
        {
            return (short)this.value;
        }

        public int ReadInt()
        {
            return (int)this.value;
        }

        public long ReadLong()
        {
            return (long)this.value;
        }

        public bool ReadBool()
        {
            byte result = this.ReadByte();
            return result > 0;
        }

        public double ReadDouble()
        {
            return (double)this.value;
        }

        public string ReadString()
        {
            return (string)this.value;
        }

        public string ReadHex()
        {
            long test;
            if (long.TryParse((string)this.value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out test))
                return test.ToString("X");

            throw new Exception(string.Format("Value {0} not valid at line {1}", this.value, this.CurrentPosition));
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.code, this.value);
        }

        #endregion

        #region private methods

        private byte[] ReadBinaryData()
        {
            byte length = this.reader.ReadByte();
            return this.reader.ReadBytes(length);
        }

        private string NullTerminatedString()
        {
            byte c = this.reader.ReadByte();
            List<byte> bytes = new List<byte>();
            while (c != 0) // strings always end with a 0 byte (char NULL)
            {
                bytes.Add(c);
                c = this.reader.ReadByte();
            }
            return this.encoding.GetString(bytes.ToArray(), 0, bytes.Count);
        }

        #endregion       
    }
}