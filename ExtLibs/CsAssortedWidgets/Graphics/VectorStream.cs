
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

using System;
using System.Drawing;

using AssortedWidgets.Math3D;

namespace AssortedWidgets.Graphics
{
    public class VectorStream<T> where T : class, new()
    {
        public VectorStream(int numVertices)
        {
            this._NumVertices = numVertices;

            T vt = new T();
            if (vt is Vector2f)
                this._data = new float[this.NumVertices * 2];
            if (vt is Vector3f)
                this._data = new float[this.NumVertices * 3];
            if (vt is Vector4f)
                this._data = new float[this.NumVertices * 4];
            if (vt is TColor)
                this._data = new float[this.NumVertices * 4];
        }
        private float[] _data = null;
        public float[] Data
        {
            get { return this._data; }
        }

        private int _NumVertices;
        /// <summary>
        /// Gets the number of vertices that this stream is capable of containing.
        /// </summary>
	    public int NumVertices
	    {
	        get { return _NumVertices;}
	    }

        private int _VariableVertices = 0;
        /// <summary>
        /// Define un rango de vertices, que puede variar. Para indicar que el rango no es válido poner a cero.
        /// </summary>
        public int VariableVertices
        {
            get { return _VariableVertices; }
            set 
            {
                if (value > _NumVertices)
                    _VariableVertices = _NumVertices;
                else
                    _VariableVertices = value; 
            }
        }

        protected int CurrentOffset_ = 0;

        /// <summary>
        /// Gets or sets the current offset into the stream.
        /// </summary>
        public int CurrentOffset
        {
            get { return CurrentOffset_; }
            set { CurrentOffset_ = value; }
        }

        /// <summary>
        /// <para>Get: Devuelve una nueva instanacia de tipo T, segun el indice index</para>
        /// <para>Set: Establece el valor de tipo T en el índice index especificado</para>
        /// </summary>
        /// <param Name_="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                if (this._data == null)
                    throw new ArgumentNullException();

                Type vectorType = typeof(T);

                try
                {
                    if (vectorType == typeof(Vector2f))
                    {
                        int floatindex = index * 2;
                        Vector2f vector = new Vector2f();
                        vector.X = this._data[floatindex];
                        vector.Y = this._data[floatindex + 1];
                        return (T)Convert.ChangeType(vector, typeof(T));
                    }
                    if (vectorType == typeof(Vector3f))
                    {
                        int floatindex = index * 3;
                        Vector3f v = new Vector3f();
                        v.X = this._data[floatindex];
                        v.Y = this._data[floatindex + 1];
                        v.Z = this._data[floatindex + 2];
                        return (T)Convert.ChangeType(v, typeof(T));
                    }
                    if (vectorType == typeof(Vector4f))
                    {
                        int floatindex = index * 4;
                        Vector4f v = new Vector4f();
                        v.X = this._data[floatindex];
                        v.Y = this._data[floatindex + 1];
                        v.Z = this._data[floatindex + 2];
                        v.W = this._data[floatindex + 3];
                        return (T)Convert.ChangeType(v, typeof(T));
                    }
                    if (vectorType == typeof(TColor))
                    {
                        int floatindex = index * 4;
                        byte r, g, b, a;
                        r = (byte)this._data[floatindex];
                        g = (byte)this._data[floatindex + 1];
                        b = (byte)this._data[floatindex + 2];
                        a = (byte)this._data[floatindex + 3];
                        TColor v = new TColor(Color.FromArgb(a, r, g, b));
                        return (T)Convert.ChangeType(v, typeof(T));
                    }
                }
                catch (Exception e)
                {
                    if (e is IndexOutOfRangeException)
                        throw new IndexOutOfRangeException();
                }
                return null;
            }
            set
            {
                // is this._data still null?
                if (this._data == null)
                    throw new ArgumentNullException();

                Object data = value;

                try
                {
                    if (value is Vector2f)
                    {
                        int floatindex = index * 2;
                        Vector2f v = (Vector2f)data;
                        this._data[floatindex] = v.X;
                        this._data[floatindex + 1] = v.Y;
                    }
                    if (value is Vector3f)
                    {
                        int floatindex = index * 3;
                        Vector3f v = (Vector3f)data;
                        this._data[floatindex] = v.X;
                        this._data[floatindex + 1] = v.Y;
                        this._data[floatindex + 2] = v.Z;
                    }
                    if (value is Vector4f)
                    {
                        int floatindex = index * 4;
                        Vector4f v = (Vector4f)data;
                        this._data[floatindex] = v.X;
                        this._data[floatindex + 1] = v.Y;
                        this._data[floatindex + 2] = v.Z;
                        this._data[floatindex + 3] = v.W;
                    }
                    if (value is TColor)
                    {
                        int floatindex = index * 4;
                        TColor v = (TColor)data;
                        this._data[floatindex] = v.R;
                        this._data[floatindex + 1] = v.G;
                        this._data[floatindex + 2] = v.B;
                        this._data[floatindex + 3] = v.A;
                    }
                }
                catch (Exception e)
                {
                    if (e is IndexOutOfRangeException)
                        throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Allows one to add data to the stream through addition.  This operation also incrememts the current offset, so can be
        /// used sequentially.
        /// </summary>
        /// <param Name_="stream"></param>
        /// <param Name_="data"></param>
        /// <returns></returns>
        public static VectorStream<T> operator +(VectorStream<T> stream, T data)
        {
            stream[stream.CurrentOffset] = data;
            stream.CurrentOffset++;
            return stream;
        }

        public void Write(T valor)
        {
            // is this._data still null?
            if (this._data == null)
                throw new ArgumentNullException();

            Object data = valor;

            try
            {
                if (valor is Vector2f)
                {
                    int floatindex = CurrentOffset_ * 2;
                    Vector2f v = (Vector2f)data;
                    this._data[floatindex] = v.X;
                    this._data[floatindex + 1] = v.Y;
                }
                if (valor is Vector3f)
                {
                    int floatindex = CurrentOffset_ * 3;
                    Vector3f v = (Vector3f)data;
                    this._data[floatindex] = v.X;
                    this._data[floatindex + 1] = v.Y;
                    this._data[floatindex + 2] = v.Z;
                }
                if (valor is Vector4f)
                {
                    int floatindex = CurrentOffset_ * 4;
                    Vector4f v = (Vector4f)data;
                    this._data[floatindex] = v.X;
                    this._data[floatindex + 1] = v.Y;
                    this._data[floatindex + 2] = v.Z;
                    this._data[floatindex + 3] = v.W;
                }
                if (valor is TColor)
                {
                    int floatindex = CurrentOffset_ * 4;
                    TColor v = (TColor)data;
                    this._data[floatindex] = v.R;
                    this._data[floatindex + 1] = v.G;
                    this._data[floatindex + 2] = v.B;
                    this._data[floatindex + 3] = v.A;
                }
                CurrentOffset_++;
            }
            catch (Exception e)
            {
                if (e is IndexOutOfRangeException)
                    throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Locks the vertex stream, allowing it to be read or used by a single thread.
        /// </summary>
        public virtual void Lock()
        {
            this.CurrentOffset_ = 0;
        }

        /// <summary>
        /// Unlocks the vertex stream, preventing reading and writing, and allowing other threads to use it.
        /// </summary>
        public virtual void Unlock()
        {
        }

        /// <summary>
        /// Writes and array of data into the stream, starting at index 0.
        /// </summary>
        /// <param Name_="data"></param>
        public void Write( T[] data )
        {
            for ( int i = 0; i < data.Length; i++ )
                this[i] = data[i];
        }

        /// <summary>
        /// Called by Cierra, cleans up any unmanaged resources.
        /// </summary>
        protected virtual void Cleanup()
        {
            this._data = null;
        }
    }
}
