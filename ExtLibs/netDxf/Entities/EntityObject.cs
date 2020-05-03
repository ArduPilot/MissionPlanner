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
using netDxf.Blocks;
using netDxf.Collections;
using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a generic entity.
    /// </summary>
    public abstract class EntityObject :
        DxfObject,
        IHasXData,
        ICloneable
    {
        #region delegates and events

        public delegate void LayerChangedEventHandler(EntityObject sender, TableObjectChangedEventArgs<Layer> e);
        public event LayerChangedEventHandler LayerChanged;
        protected virtual Layer OnLayerChangedEvent(Layer oldLayer, Layer newLayer)
        {
            LayerChangedEventHandler ae = this.LayerChanged;
            if (ae != null)
            {
                TableObjectChangedEventArgs<Layer> eventArgs = new TableObjectChangedEventArgs<Layer>(oldLayer, newLayer);
                ae(this, eventArgs);
                return eventArgs.NewValue;
            }
            return newLayer;
        }

        public delegate void LinetypeChangedEventHandler(EntityObject sender, TableObjectChangedEventArgs<Linetype> e);
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

        public event XDataAddAppRegEventHandler XDataAddAppReg;
        protected virtual void OnXDataAddAppRegEvent(ApplicationRegistry item)
        {
            XDataAddAppRegEventHandler ae = this.XDataAddAppReg;
            if (ae != null)
                ae(this, new ObservableCollectionEventArgs<ApplicationRegistry>(item));
        }

        public event XDataRemoveAppRegEventHandler XDataRemoveAppReg;
        protected virtual void OnXDataRemoveAppRegEvent(ApplicationRegistry item)
        {
            XDataRemoveAppRegEventHandler ae = this.XDataRemoveAppReg;
            if (ae != null)
                ae(this, new ObservableCollectionEventArgs<ApplicationRegistry>(item));
        }

        #endregion

        #region private fields

        private readonly EntityType type;
        private AciColor color;
        private Layer layer;
        private Linetype linetype;
        private Lineweight lineweight;
        private Transparency transparency;
        private double linetypeScale;
        private bool isVisible;
        private Vector3 normal;
        private readonly XDataDictionary xData;
        private readonly List<DxfObject> reactors;

        #endregion

        #region constructors

        protected EntityObject(EntityType type, string dxfCode)
            : base(dxfCode)
        {
            this.type = type;
            this.color = AciColor.ByLayer;
            this.layer = Layer.Default;
            this.linetype = Linetype.ByLayer;
            this.lineweight = Lineweight.ByLayer;
            this.transparency = Transparency.ByLayer;
            this.linetypeScale = 1.0;
            this.isVisible = true;
            this.normal = Vector3.UnitZ;
            this.reactors = new List<DxfObject>();
            this.xData = new XDataDictionary();
            this.xData.AddAppReg += this.XData_AddAppReg;
            this.xData.RemoveAppReg += this.XData_RemoveAppReg;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the list of dxf objects that has been attached to this entity.
        /// </summary>
        public IReadOnlyList<DxfObject> Reactors
        {
            get { return this.reactors; }
        }

        /// <summary>
        /// Gets the entity <see cref="EntityType">type</see>.
        /// </summary>
        public EntityType Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets or sets the entity <see cref="AciColor">color</see>.
        /// </summary>
        public AciColor Color
        {
            get { return this.color; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.color = value;
            }
        }

        /// <summary>
        /// Gets or sets the entity <see cref="Layer">layer</see>.
        /// </summary>
        public Layer Layer
        {
            get { return this.layer; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.layer = this.OnLayerChangedEvent(this.layer, value);
            }
        }

        /// <summary>
        /// Gets or sets the entity <see cref="Linetype">line type</see>.
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
        /// Gets or sets the entity <see cref="Lineweight">line weight</see>, one unit is always 1/100 mm (default = ByLayer).
        /// </summary>
        public Lineweight Lineweight
        {
            get { return this.lineweight; }
            set { this.lineweight = value; }
        }

        /// <summary>
        /// Gets or sets layer <see cref="Transparency">transparency</see> (default: ByLayer).
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
        /// Gets or sets the entity line type scale.
        /// </summary>
        public double LinetypeScale
        {
            get { return this.linetypeScale; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The line type scale must be greater than zero.");
                this.linetypeScale = value;
            }
        }

        /// <summary>
        /// Gets or set the entity visibility.
        /// </summary>
        public bool IsVisible
        {
            get { return this.isVisible; }
            set { this.isVisible = value; }
        }

        /// <summary>
        /// Gets or sets the entity <see cref="Vector3">normal</see>.
        /// </summary>
        public Vector3 Normal
        {
            get { return this.normal; }
            set
            {
                this.normal = Vector3.Normalize(value);
                if (Vector3.IsNaN(this.normal))
                    throw new ArgumentException("The normal can not be the zero vector.", nameof(value));
            }
        }

        /// <summary>
        /// Gets the owner of the actual dxf object.
        /// </summary>
        public new Block Owner
        {
            get { return (Block) base.Owner; }
            internal set { base.Owner = value; }
        }

        /// <summary>
        /// Gets the entity <see cref="XDataDictionary">extended data</see>.
        /// </summary>
        public XDataDictionary XData
        {
            get { return this.xData; }
        }

        #endregion

        #region internal methods

        internal void AddReactor(DxfObject o)
        {
            this.reactors.Add(o);
        }

        internal bool RemoveReactor(DxfObject o)
        {
            return this.reactors.Remove(o);
        }

        #endregion

        #region overrides

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return this.type.ToString();
        }

        #endregion

        #region ICloneable

        /// <summary>
        /// Creates a new entity that is a copy of the current instance.
        /// </summary>
        /// <returns>A new entity that is a copy of this instance.</returns>
        public abstract object Clone();

        #endregion

        #region XData events

        private void XData_AddAppReg(XDataDictionary sender, ObservableCollectionEventArgs<ApplicationRegistry> e)
        {
            this.OnXDataAddAppRegEvent(e.Item);
        }

        private void XData_RemoveAppReg(XDataDictionary sender, ObservableCollectionEventArgs<ApplicationRegistry> e)
        {
            this.OnXDataRemoveAppRegEvent(e.Item);
        }

        #endregion
    }
}