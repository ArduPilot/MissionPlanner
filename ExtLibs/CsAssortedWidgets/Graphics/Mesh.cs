
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

using OpenTK.Graphics.OpenGL;

using AssortedWidgets.Math3D;

namespace AssortedWidgets.Graphics
{
    public abstract class Mesh
    {
        public Matrix4f transform = new Matrix4f();

        protected VectorStream<Vector3f> PositionStream_ = null;
        protected VectorStream<TColor> colorStream_ = null;
        protected VectorStream<Vector3f> normalStream_ = null;
        protected VectorStream<Vector2f> texCoordStream_ = null;
        protected IndexStream indexBuffer_ = null;

        public string nombre = string.Empty;

        public bool esVisible = true;
        public bool sombraPlana = false;

        public Mesh(string nombre)
        {
            this.nombre = nombre;
            //TEventLog.Singleton.Escribe("Creada malla_simple: " + nombre, Prioridad.Low);
        }

        public IndexStream IndexBuffer
        {
            get { return this.indexBuffer_; }
        }

        private string _materialName = "";
        public string MaterialName
        {
            get { return _materialName; }
            set { _materialName = value; }
        }

        public VectorStream<Vector3f> PositionStream
        {
            get { return this.PositionStream_; }
        }

        public VectorStream<Vector2f> TexCoordStream
        {
            get { return this.texCoordStream_; }
        }

        protected PrimitiveGroup[] primitiveGroups_ = null;
        public PrimitiveGroup[] PrimitiveGroup
        {
            get
            {
                return this.primitiveGroups_;
            }
        }

        protected BeginMode primitiveType_ = BeginMode.Triangles;
        public BeginMode PrimitiveType
        {
            get { return this.primitiveType_; }
        }

        public virtual void Render(bool setStreams)
        {
            if (setStreams)
            {
                if (PositionStream_ != null)
                {
                    GL.EnableClientState(ArrayCap.VertexArray);
                    GL.VertexPointer(3, VertexPointerType.Float, 0, PositionStream_.Data);
                }
                if (texCoordStream_ != null)
                {
                    GL.EnableClientState(ArrayCap.TextureCoordArray);
                    GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, texCoordStream_.Data);
                }
                if (colorStream_ != null)
                {
                    GL.EnableClientState(ArrayCap.ColorArray);
                    GL.ColorPointer(3, ColorPointerType.Float, 0, colorStream_.Data);
                }
                if (normalStream_ != null)
                {
                    GL.EnableClientState(ArrayCap.NormalArray);
                    GL.NormalPointer(NormalPointerType.Float, 0, normalStream_.Data);
                }
            }
            this.RenderPrimitives();

            GL.DisableClientState(ArrayCap.NormalArray);
            GL.DisableClientState(ArrayCap.ColorArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);
            GL.DisableClientState(ArrayCap.VertexArray);
        }

        protected void RenderPrimitives()
        {
            int numVertices = 0;
            if (this.PositionStream_.VariableVertices > 0)
                numVertices = this.PositionStream_.VariableVertices;
            else
                numVertices = this.PositionStream_.NumVertices;

            if (this.IndexBuffer != null)
                GL.DrawElements(this.PrimitiveType, this.IndexBuffer.IndexArray.Length,
                                DrawElementsType.UnsignedInt, this.IndexBuffer.IndexArray);
            else
                if (this.primitiveGroups_ != null)
                    foreach (PrimitiveGroup pg in this.primitiveGroups_)
                    {
                        GL.DrawArrays(pg.PrimitiveType, pg.StartVertex, pg.NumVertices);
                    }
                else
                    GL.DrawArrays(this.primitiveType_, 0, numVertices);
        }
    }

    public struct PrimitiveGroup
    {
        public PrimitiveGroup(int startVertex, int numVertices, BeginMode primtiveType)
        {
            this.StartVertex = startVertex;
            this.NumVertices = numVertices;
            this.PrimitiveType = primtiveType;
        }

        public int StartVertex;
        public int NumVertices;
        public BeginMode PrimitiveType;
    }
}
