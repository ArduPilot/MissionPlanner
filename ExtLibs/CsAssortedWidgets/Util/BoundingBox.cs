
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

namespace AssortedWidgets.Util
{
    public abstract class BoundingBox
    {
        Position Position_;
        Size Size_;

        #region Constructor

        public BoundingBox()
        {
            Position = new Position();
            Size = new Size();
        }
        public BoundingBox(int x, int y, uint width, uint height)
        {
            Position = new Position(x, y);
            Size = new Size(width, height);
        }
        public BoundingBox(BoundingBox bbox)
        {
            Position = new Position(bbox.Position);
            Size = new Size(bbox.Size);
        }
        #endregion Constructor

        public Position Position
        {
            get { return Position_; }
            set { Position_ = value; }
        }
        public Size Size
        {
            get { return Size_; }
            set { Size_ = value; }
        }
        public void CopyFrom(BoundingBox bbox)
        {
            Position = bbox.Position;
            Size = bbox.Size;
        }

        public override bool Equals(object obj)
        {
            if (obj is BoundingBox)
                return (this == (BoundingBox)obj);
            else
                return false;
        }
        public override int GetHashCode()
        {
            return Position.GetHashCode() ^ (Size.GetHashCode());
        }

        #region Operators

        public static bool operator ==(BoundingBox bbox1, BoundingBox bbox2)
        {
            if ((object)bbox1 == null || (object)bbox2 == null)
                return (object)bbox1 == (object)bbox2;
            return (bbox1.Position == bbox2.Position && bbox1.Size == bbox2.Size);
        }
        public static bool operator !=(BoundingBox bbox1, BoundingBox bbox2)
        {
            if ((object)bbox1 == null || (object)bbox2 == null)
                return (object)bbox1 != (object)bbox2;
            return (bbox1.Position != bbox2.Position && bbox1.Size != bbox2.Size);
        }
        #endregion Operators

        public bool IsIn(int x, int y)
        {
            return ((Position.X < x) && (x < (Position.X + Size.width)) && (Position.Y < y) && (y < (Position.Y + Size.height)));
        }
    }
}
