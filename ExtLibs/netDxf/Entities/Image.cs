#region netDxf library, Copyright (C) 2009-2017 Daniel Carvajal (haplokuon@gmail.com)

//                        netDxf library
// Copyright (C) 2009-2017 Daniel Carvajal (haplokuon@gmail.com)
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
using netDxf.Objects;
using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a raster image <see cref="EntityObject">entity</see>.
    /// </summary>
    public class Image :
        EntityObject
    {
        #region private fields

        private Vector3 position;
        private double width;
        private double height;
        private double rotation;
        private ImageDefinition imageDefinition;
        private bool clipping;
        private short brightness;
        private short contrast;
        private short fade;
        private ImageDisplayFlags displayOptions;
        private ClippingBoundary clippingBoundary;

        #endregion

        #region constructors

        internal Image()
            : base(EntityType.Image, DxfObjectCode.Image)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Image</c> class.
        /// </summary>
        /// <param name="imageDefinition">Image definition.</param>
        /// <param name="position">Image <see cref="Vector2">position</see> in world coordinates.</param>
        /// <param name="size">Image <see cref="Vector2">size</see> in world coordinates.</param>
        public Image(ImageDefinition imageDefinition, Vector2 position, Vector2 size)
            : this(imageDefinition, new Vector3(position.X, position.Y, 0.0), size.X, size.Y)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Image</c> class.
        /// </summary>
        /// <param name="imageDefinition">Image definition.</param>
        /// <param name="position">Image <see cref="Vector3">position</see> in world coordinates.</param>
        /// <param name="size">Image <see cref="Vector2">size</see> in world coordinates.</param>
        public Image(ImageDefinition imageDefinition, Vector3 position, Vector2 size)
            : this(imageDefinition, position, size.X, size.Y)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Image</c> class.
        /// </summary>
        /// <param name="imageDefinition">Image definition.</param>
        /// <param name="position">Image <see cref="Vector2">position</see> in world coordinates.</param>
        /// <param name="width">Image width in world coordinates.</param>
        /// <param name="height">Image height in world coordinates.</param>
        public Image(ImageDefinition imageDefinition, Vector2 position, double width, double height)
            : this(imageDefinition, new Vector3(position.X, position.Y, 0.0), width, height)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Image</c> class.
        /// </summary>
        /// <param name="imageDefinition">Image definition.</param>
        /// <param name="position">Image <see cref="Vector3">position</see> in world coordinates.</param>
        /// <param name="width">Image width in world coordinates.</param>
        /// <param name="height">Image height in world coordinates.</param>
        public Image(ImageDefinition imageDefinition, Vector3 position, double width, double height)
            : base(EntityType.Image, DxfObjectCode.Image)
        {
            if (imageDefinition == null)
                throw new ArgumentNullException(nameof(imageDefinition));

            this.imageDefinition = imageDefinition;
            this.position = position;
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), width, "The Image width must be greater than zero.");
            this.width = width;
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height), height, "The Image height must be greater than zero.");
            this.height = height;
            this.rotation = 0;
            this.clipping = false;
            this.brightness = 50;
            this.contrast = 50;
            this.fade = 0;
            this.displayOptions = ImageDisplayFlags.ShowImage | ImageDisplayFlags.ShowImageWhenNotAlignedWithScreen | ImageDisplayFlags.UseClippingBoundary;
            this.clippingBoundary = new ClippingBoundary(0, 0, imageDefinition.Width, imageDefinition.Height);
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the image <see cref="Vector3">position</see> in world coordinates.
        /// </summary>
        public Vector3 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        /// <summary>
        /// Gets or sets the height of the image in drawing units.
        /// </summary>
        public double Height
        {
            get { return this.height; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The Image height must be greater than zero.");
                this.height = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the image in drawing units.
        /// </summary>
        public double Width
        {
            get { return this.width; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The Image width must be greater than zero.");
                this.width = value;
            }
        }

        /// <summary>
        /// Gets or sets the image rotation in degrees.
        /// </summary>
        public double Rotation
        {
            get { return this.rotation; }
            set { this.rotation = MathHelper.NormalizeAngle(value); }
        }

        /// <summary>
        /// Gets the <see cref="ImageDefinition">image definition</see>.
        /// </summary>
        public ImageDefinition Definition
        {
            get { return this.imageDefinition; }
            internal set { this.imageDefinition = value; }
        }

        /// <summary>
        /// Gets or sets the clipping state: false = off, true = on.
        /// </summary>
        public bool Clipping
        {
            get { return this.clipping; }
            set { this.clipping = value; }
        }

        /// <summary>
        /// Gets or sets the brightness value (0-100; default = 50)
        /// </summary>
        public short Brightness
        {
            get { return this.brightness; }
            set
            {
                if (value < 0 && value > 100)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Accepted brightness values range from 0 to 100.");
                this.brightness = value;
            }
        }

        /// <summary>
        /// Gets or sets the contrast value (0-100; default = 50)
        /// </summary>
        public short Contrast
        {
            get { return this.contrast; }
            set
            {
                if (value < 0 && value > 100)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Accepted contrast values range from 0 to 100.");
                this.contrast = value;
            }
        }

        /// <summary>
        /// Gets or sets the fade value (0-100; default = 0)
        /// </summary>
        public short Fade
        {
            get { return this.fade; }
            set
            {
                if (value < 0 && value > 100)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Accepted fade values range from 0 to 100.");
                this.fade = value;
            }
        }

        /// <summary>
        /// Gets or sets the image display options.
        /// </summary>
        public ImageDisplayFlags DisplayOptions
        {
            get { return this.displayOptions; }
            set { this.displayOptions = value; }
        }

        /// <summary>
        /// Gets or sets the image clipping boundary.
        /// </summary>
        /// <remarks>
        /// The vertexes coordinates of the clipping boundary are expressed in local coordinates of the image in pixels.
        /// Set as null to restore the default clipping boundary, full image.
        /// </remarks>
        public ClippingBoundary ClippingBoundary
        {
            get { return this.clippingBoundary; }
            set { this.clippingBoundary = value ?? new ClippingBoundary(0, 0, this.Definition.Width, this.Definition.Height); }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new Image that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Image that is a copy of this instance.</returns>
        public override object Clone()
        {
            Image entity = new Image
            {
                //EntityObject properties
                Layer = (Layer) this.Layer.Clone(),
                Linetype = (Linetype) this.Linetype.Clone(),
                Color = (AciColor) this.Color.Clone(),
                Lineweight = this.Lineweight,
                Transparency = (Transparency) this.Transparency.Clone(),
                LinetypeScale = this.LinetypeScale,
                Normal = this.Normal,
                IsVisible = this.IsVisible,
                //Image properties
                Position = this.position,
                Height = this.height,
                Width = this.width,
                Rotation = this.rotation,
                Definition = (ImageDefinition) this.imageDefinition.Clone(),
                Clipping = this.clipping,
                Brightness = this.brightness,
                Contrast = this.contrast,
                Fade = this.fade,
                DisplayOptions = this.displayOptions,
                ClippingBoundary = (ClippingBoundary) this.clippingBoundary.Clone()
            };

            foreach (XData data in this.XData.Values)
                entity.XData.Add((XData) data.Clone());

            return entity;
        }

        #endregion
    }
}