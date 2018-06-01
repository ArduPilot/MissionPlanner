
#region GPL License
/*
Copyright (c) 2010 Miguel Angel Guirado López

This file is part of CsAssortedWidgets.

    Trixion3D is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    CsAssortedWidgets is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with CsAssortedWidgets.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

namespace AssortedWidgets.Graphics
{
    /// <summary>
    /// A tipoBuffer of vertex index data.  These indices represent offsets into a vertex stream.
    /// </summary>
    public class IndexStream
    {
        private int[] _IndexArray = null;

        public int[] IndexArray
        {
            get { return this._IndexArray; }
        }
        /// <summary>
        /// Initialises a new instancia of the IndexStream class.
        /// </summary>
        /// <param Name_="numIndices"></param>
        public IndexStream( int numIndices )
        {
            this._NumIndices = numIndices;
            this._IndexArray = new int[numIndices];
        }

        /// <summary>
        /// Gets or sets the index value at this index.
        /// </summary>
        /// <param Name_="index"></param>
        /// <returns></returns>
        public int this[int index]
        {
            get
            {
                return this._IndexArray[index];
            }
            set
            {
                this._IndexArray[index] = value;
            }
        }
        /// <summary>
        /// Unlocks the stream, preventing it from being written to or read from, but allowing other threads to access it.
        /// </summary>
        public void Unlock()
        {
            //throw new Exception( "The method or operation is not implemented." );
        }

        /// <summary>
        /// Makes this index stream the current one.
        /// </summary>
        public void Apply()
        {
            //throw new Exception( "The method or operation is not implemented." );
        }

        /// <summary>
        /// Locks the index stream, allowing it to be written to or read from, while preventing other threads from accessing it.
        /// </summary>
        public void Lock()
        {
            //throw new Exception( "The method or operation is not implemented." );
        }

        private int _CurrentOffset = 0;
        /// <summary>
        /// Gets or sets the current offset into the stream.
        /// </summary>
        public int CurrentOffset
        {
            get { return _CurrentOffset; }
            set { _CurrentOffset = value; }
        }

        /// <summary>
        /// Gets the number of indices that this stream is capable of holding.
        /// </summary>
        public int NumIndices
        {
            get { return this._NumIndices; }
        }
        private int _NumIndices;

        /// <summary>
        /// Allows the addition of data to the stream.  The offset is incremented each time, allowing consecutive additions.
        /// </summary>
        /// <param Name_="stream"></param>
        /// <param Name_="data"></param>
        /// <returns></returns>
        public static IndexStream operator +( IndexStream stream, int data )
        {
            stream[stream.CurrentOffset] = data;
            stream.CurrentOffset++;
            return stream;
        }

        /// <summary>
        /// Writes and array of data into the stream, starting at index 0.
        /// </summary>
        /// <param Name_="data"></param>
        public void Write(int[] data)
        {
            for (int i = 0; i < data.Length; i++)
                this[i] = data[i];
        }
    }
}
