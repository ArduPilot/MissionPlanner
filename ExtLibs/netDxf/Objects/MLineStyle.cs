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
using netDxf.Collections;
using netDxf.Tables;

namespace netDxf.Objects
{
    /// <summary>
    /// Represents as MLine style.
    /// </summary>
    public class MLineStyle :
        TableObject
    {
        #region delegates and events

        public delegate void MLineStyleElementAddedEventHandler(MLineStyle sender, MLineStyleElementChangeEventArgs e);

        public event MLineStyleElementAddedEventHandler MLineStyleElementAdded;

        protected virtual void OnMLineStyleElementAddedEvent(MLineStyleElement item)
        {
            MLineStyleElementAddedEventHandler ae = this.MLineStyleElementAdded;
            if (ae != null)
                ae(this, new MLineStyleElementChangeEventArgs(item));
        }

        public delegate void MLineStyleElementRemovedEventHandler(MLineStyle sender, MLineStyleElementChangeEventArgs e);

        public event MLineStyleElementRemovedEventHandler MLineStyleElementRemoved;

        protected virtual void OnMLineStyleElementRemovedEvent(MLineStyleElement item)
        {
            MLineStyleElementRemovedEventHandler ae = this.MLineStyleElementRemoved;
            if (ae != null)
                ae(this, new MLineStyleElementChangeEventArgs(item));
        }

        public delegate void MLineStyleElementLinetypeChangedEventHandler(MLineStyle sender, TableObjectChangedEventArgs<Linetype> e);

        public event MLineStyleElementLinetypeChangedEventHandler MLineStyleElementLinetypeChanged;

        protected virtual Linetype OnMLineStyleElementLinetypeChangedEvent(Linetype oldLinetype, Linetype newLinetype)
        {
            MLineStyleElementLinetypeChangedEventHandler ae = this.MLineStyleElementLinetypeChanged;
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

        private MLineStyleFlags flags;
        private string description;
        private AciColor fillColor;
        private double startAngle;
        private double endAngle;
        private readonly ObservableCollection<MLineStyleElement> elements;

        #endregion

        #region constants

        /// <summary>
        /// Default multiline style name.
        /// </summary>
        public const string DefaultName = "Standard";

        /// <summary>
        /// Gets the default MLine style.
        /// </summary>
        public static MLineStyle Default
        {
            get { return new MLineStyle(DefaultName); }
        }

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>MLineStyle</c> class.
        /// </summary>
        /// <param name="name">MLine style name.</param>
        /// <remarks>By default the multiline style has to elements with offsets 0.5 y -0.5.</remarks>
        public MLineStyle(string name)
            : this(name, null, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>MLineStyle</c> class.
        /// </summary>
        /// <param name="name">MLine style name.</param>
        /// <param name="description">MLine style description.</param>
        /// <remarks>By default the multiline style has to elements with offsets 0.5 y -0.5.</remarks>
        public MLineStyle(string name, string description)
            : this(name, null, description)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>MLineStyle</c> class.
        /// </summary>
        /// <param name="name">MLine style name.</param>
        /// <param name="elements">Elements of the multiline, if null two default elements will be added.</param>
        public MLineStyle(string name, IEnumerable<MLineStyleElement> elements)
            : this(name, elements, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>MLineStyle</c> class.
        /// </summary>
        /// <param name="name">MLine style name.</param>
        /// <param name="elements">Elements of the multiline, if null two default elements will be added.</param>
        /// <param name="description">MLine style description (optional).</param>
        public MLineStyle(string name, IEnumerable<MLineStyleElement> elements, string description)
            : base(name, DxfObjectCode.MLineStyle, true)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name), "The multiline style name should be at least one character long.");

            this.flags = MLineStyleFlags.None;
            this.description = string.IsNullOrEmpty(description) ? string.Empty : description;
            this.fillColor = AciColor.ByLayer;
            this.startAngle = 90.0;
            this.endAngle = 90.0;

            this.elements = new ObservableCollection<MLineStyleElement>();
            this.elements.BeforeAddItem += this.Elements_BeforeAddItem;
            this.elements.AddItem += this.Elements_AddItem;
            this.elements.BeforeRemoveItem += this.Elements_BeforeRemoveItem;
            this.elements.RemoveItem += this.Elements_RemoveItem;
            this.elements.AddRange(elements ?? new[] { new MLineStyleElement(0.5), new MLineStyleElement(-0.5) });
            this.elements.Sort(); // the elements list must be ordered

            if (this.elements.Count < 1)
                throw new ArgumentOutOfRangeException(nameof(elements), this.elements.Count, "The elements list must have at least one element.");
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the MLine style flags.
        /// </summary>
        public MLineStyleFlags Flags
        {
            get { return this.flags; }
            set { this.flags = value; }
        }

        /// <summary>
        /// Gets or sets the line type description (optional).
        /// </summary>
        public string Description
        {
            get { return this.description; }
            set { this.description = string.IsNullOrEmpty(value) ? string.Empty : value; }
        }

        /// <summary>
        /// Gets or sets the MLine fill color.
        /// </summary>
        /// <remarks>
        /// AutoCad2000 dxf version does not support true colors for MLineStyle fill color.
        /// </remarks>
        public AciColor FillColor
        {
            get { return this.fillColor; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.fillColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the MLine start angle in degrees.
        /// </summary>
        public double StartAngle
        {
            get { return this.startAngle; }
            set
            {
                if (value < 10.0 || value > 170.0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The MLine style start angle valid values range from 10 to 170 degrees.");
                this.startAngle = value;
            }
        }

        /// <summary>
        /// Gets or sets the MLine end angle in degrees.
        /// </summary>
        public double EndAngle
        {
            get { return this.endAngle; }
            set
            {
                if (value < 10.0 || value > 170.0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The MLine style end angle valid values range from 10 to 170 degrees.");
                this.endAngle = value;
            }
        }

        /// <summary>
        /// Gets the list of elements that make up the multiline.
        /// </summary>
        /// <remarks>
        /// The elements list must be ordered, this will be done automatically,
        /// but if new elements are added individually to the list it will have to be sorted manually calling the Sort() method.
        /// </remarks>
        public ObservableCollection<MLineStyleElement> Elements
        {
            get { return this.elements; }
        }

        /// <summary>
        /// Gets the owner of the actual multi line style.
        /// </summary>
        public new MLineStyles Owner
        {
            get { return (MLineStyles) base.Owner; }
            internal set { base.Owner = value; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new MLineStyle that is a copy of the current instance.
        /// </summary>
        /// <param name="newName">MLineStyle name of the copy.</param>
        /// <returns>A new MLineStyle that is a copy of this instance.</returns>
        public override TableObject Clone(string newName)
        {
            List<MLineStyleElement> copyElements = new List<MLineStyleElement>();
            foreach (MLineStyleElement e in this.elements)
                copyElements.Add((MLineStyleElement) e.Clone());

            MLineStyle copy = new MLineStyle(newName, copyElements)
            {
                Flags = this.flags,
                Description = this.description,
                FillColor = (AciColor) this.fillColor.Clone(),
                StartAngle = this.startAngle,
                EndAngle = this.endAngle,
            };

            foreach (XData data in this.XData.Values)
                copy.XData.Add((XData)data.Clone());

            return copy;
        }

        /// <summary>
        /// Creates a new MLineStyle that is a copy of the current instance.
        /// </summary>
        /// <returns>A new MLineStyle that is a copy of this instance.</returns>
        public override object Clone()
        {
            return this.Clone(this.Name);
        }

        #endregion

        #region Elements collection events

        private void Elements_BeforeAddItem(ObservableCollection<MLineStyleElement> sender, ObservableCollectionEventArgs<MLineStyleElement> e)
        {
            // null items are not allowed
            if (e.Item == null)
                e.Cancel = true;
            else
                e.Cancel = false;
        }

        private void Elements_AddItem(ObservableCollection<MLineStyleElement> sender, ObservableCollectionEventArgs<MLineStyleElement> e)
        {
            this.OnMLineStyleElementAddedEvent(e.Item);
            e.Item.LinetypeChanged += this.MLineStyleElement_LinetypeChanged;
        }

        private void Elements_BeforeRemoveItem(ObservableCollection<MLineStyleElement> sender, ObservableCollectionEventArgs<MLineStyleElement> e)
        {
        }

        private void Elements_RemoveItem(ObservableCollection<MLineStyleElement> sender, ObservableCollectionEventArgs<MLineStyleElement> e)
        {
            this.OnMLineStyleElementRemovedEvent(e.Item);
            e.Item.LinetypeChanged -= this.MLineStyleElement_LinetypeChanged;
        }

        #endregion

        #region MLineStyleElement events

        private void MLineStyleElement_LinetypeChanged(MLineStyleElement sender, TableObjectChangedEventArgs<Linetype> e)
        {
            e.NewValue = this.OnMLineStyleElementLinetypeChangedEvent(e.OldValue, e.NewValue);
        }

        #endregion
    }
}