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

namespace netDxf.IO
{
    internal class TextCodeValueReader :
        ICodeValueReader
    {
        #region private fields

        private readonly TextReader reader;
        private short code;
        private string value;
        private long currentPosition;

        #endregion

        #region constructors

        public TextCodeValueReader(TextReader reader)
        {
            this.reader = reader;
            this.code = 0;
            this.value = null;
            this.currentPosition = 0;
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
            get { return this.currentPosition; }
        }
        #endregion

        #region public methods

        public void Next()
        {
            string readCode = this.reader.ReadLine();
            this.currentPosition += 1;
            if (!short.TryParse(readCode, NumberStyles.Integer, CultureInfo.InvariantCulture, out this.code))
                throw new Exception(string.Format("Code {0} not valid at line {1}", this.code, this.currentPosition));
            this.value = this.reader.ReadLine();
            this.currentPosition += 1;
        }

        public byte ReadByte()
        {
            byte result;
            if (byte.TryParse(this.value, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture, out result))
                return result;

            throw new Exception(string.Format("Value {0} not valid at line {1}", this.value, this.currentPosition));
        }

        public byte[] ReadBytes()
        {
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < this.value.Length; i++)
            {
                string hex = string.Concat(this.value[i], this.value[++i]);
                byte result;
                if (byte.TryParse(hex, NumberStyles.AllowHexSpecifier | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture, out result))
                    bytes.Add(result);
                else
                    throw new Exception(string.Format("Value {0} not valid at line {1}", hex, this.currentPosition));
            }
            return bytes.ToArray();
        }

        public short ReadShort()
        {
            short result;
            if (short.TryParse(this.value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
                return result;

            throw new Exception(string.Format("Value {0} not valid at line {1}", this.value, this.currentPosition));
        }

        public int ReadInt()
        {
            int result;
            if (int.TryParse(this.value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
                return result;

            throw new Exception(string.Format("Value {0} not valid at line {1}", this.value, this.currentPosition));
        }

        public long ReadLong()
        {
            long result;
            if (long.TryParse(this.value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
                return result;

            throw new Exception(string.Format("Value {0} not valid at line {1}", this.value, this.currentPosition));
        }

        public bool ReadBool()
        {
            byte result = this.ReadByte();
            return result > 0;
        }

        public double ReadDouble()
        {
            double result;
            if (double.TryParse(this.value, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
                return result;

            throw new Exception(string.Format("Value {0} not valid at line {1}", this.value, this.currentPosition));
        }

        public string ReadString()
        {
            return this.value;
        }

        public string ReadHex()
        {
            long test;
            if (long.TryParse(this.value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out test))
                return test.ToString("X");

            throw new Exception(string.Format("Value {0} not valid at line {1}", this.value, this.currentPosition));
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.code, this.value);
        }
        #endregion      
    }
}