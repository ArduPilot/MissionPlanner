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
using System.IO;
using netDxf.Collections;

namespace netDxf.Tables
{
    /// <summary>
    /// Represent a shape style.
    /// </summary>
    public class ShapeStyle :
        TableObject
    {
        #region private fields

        private readonly string file;
        private readonly double size;
        private readonly double widthFactor;
        private readonly double obliqueAngle;

        #endregion

        #region constants

        /// <summary>
        /// Gets the default shape style
        /// </summary>
        /// <remarks>AutoCad stores the shapes for the predefined complex linetypes in the ltypeshp.shx file.</remarks>
        public static ShapeStyle Default
        {
            get { return new ShapeStyle("LTYPESHP.SHX");}
        }

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>ShapeStyle</c> class.
        /// </summary>
        /// <param name="file">Shape definitions SHX file.</param>
        public ShapeStyle(string file)
            : this(Path.GetFileNameWithoutExtension(file), file, 0.0, 1.0, 0.0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>ShapeStyle</c> class.
        /// </summary>
        /// <param name="name">Shape style name.</param>
        /// <param name="file">Shape definitions SHX file.</param>
        public ShapeStyle(string name, string file)
            : this(name, file, 0.0, 1.0, 0.0)
        {
        }

        internal ShapeStyle(string name, string file, double size, double widthFactor, double obliqueAngle)
            : base(name, DxfObjectCode.TextStyle, true)
        {
            if (string.IsNullOrEmpty(file))
                throw new ArgumentNullException(nameof(file));
            this.file = file;
            this.size = size;
            this.widthFactor = widthFactor;
            this.obliqueAngle = obliqueAngle;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the shape SHX file name.
        /// </summary>
        public string File
        {
            get { return this.file; }
        }

        /// <summary>
        /// Gets the shape size.
        /// </summary>
        /// <remarks>This value seems to have no effect on shapes or complex line types with shapes. Default: 0.0.</remarks>
        public double Size
        {
            get { return this.size; }
        }

        /// <summary>
        /// Gets the shape width factor.
        /// </summary>
        /// <remarks>This value seems to have no effect on shapes or complex line types with shapes. Default: 1.0.</remarks>
        public double WidthFactor
        {
            get { return this.widthFactor; }
        }

        /// <summary>
        /// Gets the shape oblique angle in degrees.
        /// </summary>
        /// <remarks>This value seems to have no effect on shapes or complex line types with shapes. Default: 0.0.</remarks>
        public double ObliqueAngle
        {
            get { return this.obliqueAngle; }
        }

        /// <summary>
        /// Gets the owner of the actual shape style.
        /// </summary>
        public new ShapeStyles Owner
        {
            get { return (ShapeStyles)base.Owner; }
            internal set { base.Owner = value; }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Checks if the actual shape style contains a shape with the specified name.
        /// </summary>
        /// <param name="name">Shape name.</param>
        /// <returns>True if the shape style that contains a shape with the specified name, false otherwise.</returns>
        /// <remarks>If the actual shape style belongs to a document, it will look for the SHP file also in the document support folders.</remarks>
        public bool ContainsShapeName(string name)
        {
            string f = Path.ChangeExtension(this.file, "SHP");
            if (this.Owner != null)
                f = this.Owner.Owner.SupportFolders.FindFile(f);
            else
                if(!System.IO.File.Exists(f)) f = string.Empty;

            // we will look for the shape name in the SHP file         
            if (string.IsNullOrEmpty(f)) return false;

            using (StreamReader reader = new StreamReader(System.IO.File.Open(f, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), true))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                        throw new FileLoadException("Unknown error reading SHP file.", f);
                    // lines starting with semicolons are comments
                    if (line.StartsWith(";"))
                        continue;
                    // every shape definition starts with '*'
                    if (!line.StartsWith("*"))
                        continue;

                    string[] tokens = line.TrimStart('*').Split(',');
                    if (string.Equals(name, tokens[2], StringComparison.InvariantCultureIgnoreCase))
                        return true; //the shape style that contains a shape with the specified name has been found
                }
            }
            // there are no shape styles that contain a shape with the specified name
            return false;
        }

        /// <summary>
        /// Gets the number of the shape with the specified name.
        /// </summary>
        /// <param name="name">Name of the shape.</param>
        /// <returns>The number of the shape, 0 in case the shape has not been found.</returns>
        /// <remarks>If the actual shape style belongs to a document, it will look for the SHP file also in the document support folders.</remarks>
        public short ShapeNumber(string name)
        {
            // we will look for the shape name in the SHP file
            string f = Path.ChangeExtension(this.file, "SHP");
            if (this.Owner != null)
                f = this.Owner.Owner.SupportFolders.FindFile(f);
            else
                if (!System.IO.File.Exists(f)) f = string.Empty;

            if (string.IsNullOrEmpty(f)) return 0;

            using (StreamReader reader = new StreamReader(System.IO.File.Open(f, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), true))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                        throw new FileLoadException("Unknown error reading SHP file.", f);
                    // lines starting with semicolons are comments
                    if (line.StartsWith(";"))
                        continue;
                    // every shape definition starts with '*'
                    if (!line.StartsWith("*"))
                        continue;

                    string[] tokens = line.TrimStart('*').Split(',');
                    // the third item is the name of the shape
                    if (string.Equals(tokens[2], name, StringComparison.InvariantCultureIgnoreCase))
                        return short.Parse(tokens[0]);
                }
            }
            return 0;
        }

        /// <summary>
        /// Gets the name of the shape with the specified number.
        /// </summary>
        /// <param name="number">Number of the shape.</param>
        /// <returns>The name of the shape, empty in case the shape has not been found.</returns>
        /// <remarks>If the actual shape style belongs to a document, it will look for the SHP file also in the document support folders.</remarks>
        public string ShapeName(short number)
        {
            // we will look for the shape name in the SHP file
            string f = Path.ChangeExtension(this.file, "SHP");
            if (this.Owner != null)
                f = this.Owner.Owner.SupportFolders.FindFile(f);
            else
                if (!System.IO.File.Exists(f)) f = string.Empty;

            if (string.IsNullOrEmpty(f)) return string.Empty;

            using (StreamReader reader = new StreamReader(System.IO.File.Open(f, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), true))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                        throw new FileLoadException("Unknown error reading SHP file.", f);
                    // lines starting with semicolons are comments
                    if (line.StartsWith(";"))
                        continue;
                    // every shape definition starts with '*'
                    if (!line.StartsWith("*"))
                        continue;

                    string[] tokens = line.TrimStart('*').Split(',');
                    // the first item is the number of the shape
                    if (short.Parse(tokens[0]) == number)
                        return tokens[2];
                }
            }

            return string.Empty;
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new TextStyle that is a copy of the current instance.
        /// </summary>
        /// <param name="newName">TextStyle name of the copy.</param>
        /// <returns>A new TextStyle that is a copy of this instance.</returns>
        public override TableObject Clone(string newName)
        {
            ShapeStyle copy = new ShapeStyle(newName, this.file, this.size, this.widthFactor, this.obliqueAngle);

            foreach (XData data in this.XData.Values)
                copy.XData.Add((XData)data.Clone());

            return copy;
        }

        /// <summary>
        /// Creates a new TextStyle that is a copy of the current instance.
        /// </summary>
        /// <returns>A new TextStyle that is a copy of this instance.</returns>
        public override object Clone()
        {
            return this.Clone(this.Name);
        }

        #endregion
    }
}