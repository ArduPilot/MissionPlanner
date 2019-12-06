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
using netDxf.Collections;

namespace netDxf.Tables
{
    /// <summary>
    /// Represents a layer.
    /// </summary>
    public class Layer :
        TableObject
    {
        #region delegates and events

        public delegate void LinetypeChangedEventHandler(TableObject sender, TableObjectChangedEventArgs<Linetype> e);

        public event LinetypeChangedEventHandler LinetypeChanged;

        protected virtual Linetype OnLinetypeChangedEvent(Linetype oldLinetype, Linetype newLinetype)
        {
            LinetypeChangedEventHandler ae = this.LinetypeChanged;
            if (ae != null)
            {
                TableObjectChangedEventArgs<Linetype> eventArgs = new TableObjectChangedEventArgs<Linetype>(oldLinetype, newLinetype);
                ae(this, eventArgs);
                return eventArgs.NewValue;
            }
            return newLinetype;
        }

        #endregion

        #region private fields

        private AciColor color;
        private bool isVisible;
        private bool isFrozen;
        private bool isLocked;
        private bool plot;
        private Linetype linetype;
        private Lineweight lineweight;
        private Transparency transparency;

        #endregion

        #region constants

        /// <summary>
        /// Default layer name.
        /// </summary>
        public const string DefaultName = "0";

        /// <summary>
        /// Gets the default Layer 0.
        /// </summary>
        public static Layer Default
        {
            get { return new Layer(DefaultName); }
        }

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Layer</c> class.
        /// </summary>
        /// <param name="name">Layer name.</param>
        public Layer(string name)
            : this(name, true)
        {
        }

        internal Layer(string name, bool checkName)
            : base(name, DxfObjectCode.Layer, checkName)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name), "The layer name should be at least one character long.");

            this.IsReserved = name.Equals(DefaultName, StringComparison.OrdinalIgnoreCase);
            this.color = AciColor.Default;
            this.linetype = Linetype.Continuous;
            this.isVisible = true;
            this.plot = true;
            this.lineweight = Lineweight.Default;
            this.transparency = new Transparency(0);
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the layer <see cref="Linetype">line type</see>.
        /// </summary>
        public Linetype Linetype
        {
            get { return this.linetype; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.linetype = this.OnLinetypeChangedEvent(this.linetype, value);
            }
        }

        /// <summary>
        /// Gets or sets the layer <see cref="AciColor">color</see>.
        /// </summary>
        public AciColor Color
        {
            get { return this.color; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (value.IsByLayer || value.IsByBlock)
                    throw new ArgumentException("The layer color cannot be ByLayer or ByBlock", nameof(value));
                this.color = value;
            }
        }

        /// <summary>
        /// Gets or sets the layer visibility.
        /// </summary>
        public bool IsVisible
        {
            get { return this.isVisible; }
            set { this.isVisible = value; }
        }

        /// <summary>
        /// Gets or sets if the layer is frozen; otherwise layer is thawed.
        /// </summary>
        public bool IsFrozen
        {
            get { return this.isFrozen; }
            set { this.isFrozen = value; }
        }

        /// <summary>
        /// Gets or sets if the layer is locked.
        /// </summary>
        public bool IsLocked
        {
            get { return this.isLocked; }
            set { this.isLocked = value; }
        }

        /// <summary>
        /// Gets or sets if the plotting flag.
        /// </summary>
        /// <remarks>If set to false, do not plot this layer.</remarks>
        public bool Plot
        {
            get { return this.plot; }
            set { this.plot = value; }
        }

        /// <summary>
        /// Gets or sets the layer line weight, one unit is always 1/100 mm (default = Default).
        /// </summary>
        public Lineweight Lineweight
        {
            get { return this.lineweight; }
            set
            {
                if (value == Lineweight.ByLayer || value == Lineweight.ByBlock)
                    throw new ArgumentException("The lineweight of a layer cannot be set to ByLayer or ByBlock.", nameof(value));
                this.lineweight = value;
            }
        }

        /// <summary>
        /// Gets or sets layer transparency (default: 0, opaque).
        /// </summary>
        public Transparency Transparency
        {
            get { return this.transparency; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.transparency = value;
            }
        }

        /// <summary>
        /// Gets the owner of the actual layer.
        /// </summary>
        public new Layers Owner
        {
            get { return (Layers) base.Owner; }
            internal set { base.Owner = value; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new Layer that is a copy of the current instance.
        /// </summary>
        /// <param name="newName">Layer name of the copy.</param>
        /// <returns>A new Layer that is a copy of this instance.</returns>
        public override TableObject Clone(string newName)
        {
            Layer copy = new Layer(newName)
            {
                Color = (AciColor) this.Color.Clone(),
                IsVisible = this.isVisible,
                IsFrozen = this.isFrozen,
                IsLocked = this.isLocked,
                Plot = this.plot,
                Linetype = (Linetype) this.Linetype.Clone(),
                Lineweight = this.Lineweight,
                Transparency = (Transparency) this.Transparency.Clone()
            };

            foreach (XData data in this.XData.Values)
                copy.XData.Add((XData)data.Clone());

            return copy;
        }

        /// <summary>
        /// Creates a new Layer that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Layer that is a copy of this instance.</returns>
        public override object Clone()
        {
            return this.Clone(this.Name);
        }

        #endregion
    }
}