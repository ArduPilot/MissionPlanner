
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
    public class Position
    {
        int X_, Y_;

        public Position()
        {
            X = 0;
            Y = 0;
        }
        public Position(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public Position(Position pos)
        {
            X = pos.X;
            Y = pos.Y;
        }
        public void CopyFrom(Position pos)
        {
            X = pos.X;
            Y = pos.Y;
        }
        public int X
        {
            get { return X_; }
            set
            {
                X_ = value;
            }
        }
        public int Y
        {
            get { return Y_; }
            set
            {
                Y_ = value;
            }
        }
        public override bool Equals(object obj)
        {
            if (obj is Position)
                return (this == (Position)obj);
            else
                return false;
        }
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ (Y.GetHashCode());
        }
        #region Operators

        public static bool operator ==(Position pos1, Position pos2)
        {
            if ((object)pos1 == null || (object)pos2 == null)
                return (object)pos1 == (object)pos2;
            return (pos1.X == pos2.X && pos1.Y == pos2.Y);
        }
        public static bool operator !=(Position pos1, Position pos2)
        {
            if ((object)pos1 == null || (object)pos2 == null)
                return (object)pos1 != (object)pos2;
            return (pos1.X != pos2.X && pos1.Y != pos2.Y);
        }
        #endregion Operators

        public void SumEqual(Position pos)
        {
            X += pos.X;
            Y += pos.Y;
        }
        public void SubtractEqual(Position pos)
        {
            X -= pos.X;
            Y -= pos.Y;
        }
    }
}
