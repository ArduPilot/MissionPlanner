
#region BSD License
/*
    Copyright (c) 2010 Miguel Angel Guirado López

    This file is part of CsAssortedWidgets.

    All rights reserved.
 
    This file is a C# port of AssortedWidgets project. Original authors see readme.txt file.

    Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

    Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    Neither the name of the <ORGANIZATION> nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

using System;

namespace AssortedWidgets.Util
{
    public class Size
    {
        public uint width;
        public uint height;

        #region Constructor

        public Size()
        {
            width = 0;
            height = 0;
        }
        public Size(double width, double height)
        {
            this.width = (uint)width;
            this.height = (uint)height;
        }
        public Size(uint width, uint height)
        {
            this.width = width;
            this.height = height;
        }
        public Size(uint SizeValue)
        {
            width = SizeValue;
            height = SizeValue;
        }
        public Size(Size pSize)
        {
            width = pSize.width;
            height = pSize.height;
        }
        #endregion Constructor

        public void CopyFrom(Size pSize)
        {
            width = pSize.width;
            height = pSize.height;
        }
        public void SumEqual(Size pSize)
        {
            width += pSize.width;
            height += pSize.height;
        }
        public void SumEqual(uint SizeOffset)
        {
            width += SizeOffset;
            height += SizeOffset;
        }
        public void SubtractEqual(Size pSize)
        {
            width -= pSize.width;
            height -= pSize.height;
        }
        public void SubtractEqual(uint SizeOffset)
        {
            width -= SizeOffset;
            height -= SizeOffset;
        }
        public override bool Equals(object obj)
        {
            if (obj is Size)
                return (this == (Size)obj);
            else
                return false;
        }
        public override int GetHashCode()
        {
            return width.GetHashCode() ^ (height.GetHashCode());
        }

        #region Operators

        public static bool operator ==(Size Size1, Size Size2)
        {
            if ((object)Size1 == null || (object)Size2 == null)
                return (object)Size1 == (object)Size2;
            return (Size1.width == Size2.width && Size1.height == Size2.height);
        }
        public static bool operator !=(Size Size1, Size Size2)
        {
            if ((object)Size1 != null || (object)Size2 != null)
                return (object)Size1 != (object)Size2;
            return (Size1.width != Size2.width && Size1.height != Size2.height);
        }
        public static Size operator +(Size Size1, Size Size2)
        {
            return new Size(Size1.width + Size2.width, Size1.height + Size2.height);
        }
        public static Size operator -(Size Size1, Size Size2)
        {
            return new Size(Size1.width - Size2.width, Size1.height - Size2.height);
        }
        public static Size operator +(Size Size1, uint offset)
        {
            return new Size(Size1.width + offset, Size1.height + offset);
        }

        #endregion Operators
    }
}
