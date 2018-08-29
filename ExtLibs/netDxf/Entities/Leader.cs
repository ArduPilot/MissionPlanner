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
    /// Represents a leader <see cref="EntityObject">entity</see>.
    /// </summary>
    public class Leader :
        EntityObject
    {
        #region delegates and events

        public delegate void LeaderStyleChangedEventHandler(Leader sender, TableObjectChangedEventArgs<DimensionStyle> e);

        public event LeaderStyleChangedEventHandler LeaderStyleChanged;

        protected virtual DimensionStyle OnDimensionStyleChangedEvent(DimensionStyle oldStyle, DimensionStyle newStyle)
        {
            LeaderStyleChangedEventHandler ae = this.LeaderStyleChanged;
            if (ae != null)
            {
                TableObjectChangedEventArgs<DimensionStyle> eventArgs = new TableObjectChangedEventArgs<DimensionStyle>(oldStyle, newStyle);
                ae(this, eventArgs);
                return eventArgs.NewValue;
            }
            return newStyle;
        }

        #endregion

        #region delegates and events for style overrides

        public delegate void DimensionStyleOverrideAddedEventHandler(Leader sender, DimensionStyleOverrideChangeEventArgs e);

        public event DimensionStyleOverrideAddedEventHandler DimensionStyleOverrideAdded;

        protected virtual void OnDimensionStyleOverrideAddedEvent(DimensionStyleOverride item)
        {
            DimensionStyleOverrideAddedEventHandler ae = this.DimensionStyleOverrideAdded;
            if (ae != null)
                ae(this, new DimensionStyleOverrideChangeEventArgs(item));
        }

        public delegate void DimensionStyleOverrideRemovedEventHandler(Leader sender, DimensionStyleOverrideChangeEventArgs e);

        public event DimensionStyleOverrideRemovedEventHandler DimensionStyleOverrideRemoved;

        protected virtual void OnDimensionStyleOverrideRemovedEvent(DimensionStyleOverride item)
        {
            DimensionStyleOverrideRemovedEventHandler ae = this.DimensionStyleOverrideRemoved;
            if (ae != null)
                ae(this, new DimensionStyleOverrideChangeEventArgs(item));
        }

        #endregion

        #region private fields

        private DimensionStyle style;
        private bool showArrowhead;
        private LeaderPathType pathType;
        private readonly List<Vector2> vertexes;
        private EntityObject annotation;
        private bool hasHookline;
        private AciColor lineColor;
        private double elevation;
        private Vector2 offset;
        private readonly DimensionStyleOverrideDictionary styleOverrides;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Leader</c> class.
        /// </summary>
        public Leader(IEnumerable<Vector2> vertexes)
            : this(vertexes, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Leader</c> class.
        /// </summary>
        public Leader(IEnumerable<Vector2> vertexes, DimensionStyle style)
            : base(EntityType.Leader, DxfObjectCode.Leader)
        {
            if (vertexes == null)
                throw new ArgumentNullException(nameof(vertexes));
            this.vertexes = new List<Vector2>(vertexes);
            if (this.vertexes.Count < 2)
                throw new ArgumentOutOfRangeException(nameof(vertexes), this.vertexes.Count, "The leader vertexes list requires at least two points.");
            if(style == null)
                throw new ArgumentNullException(nameof(style));
            this.style = style;
            this.hasHookline = false;
            this.showArrowhead = true;
            this.pathType = LeaderPathType.StraightLineSegements;
            this.annotation = null;
            this.lineColor = AciColor.ByLayer;
            this.elevation = 0.0;
            this.offset = Vector2.Zero;
            this.styleOverrides = new DimensionStyleOverrideDictionary();
            this.styleOverrides.BeforeAddItem += this.StyleOverrides_BeforeAddItem;
            this.styleOverrides.AddItem += this.StyleOverrides_AddItem;
            this.styleOverrides.BeforeRemoveItem += this.StyleOverrides_BeforeRemoveItem;
            this.styleOverrides.RemoveItem += this.StyleOverrides_RemoveItem;
        }

        /// <summary>
        /// Initializes a new instance of the <c>Leader</c> class.
        /// </summary>
        public Leader(string text, IEnumerable<Vector2> vertexes)
            : this(text, vertexes, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Leader</c> class.
        /// </summary>
        public Leader(string text, IEnumerable<Vector2> vertexes, DimensionStyle style)
            : base(EntityType.Leader, DxfObjectCode.Leader)
        {
            if (vertexes == null)
                throw new ArgumentNullException(nameof(vertexes));
            this.vertexes = new List<Vector2>(vertexes);
            if (this.vertexes.Count < 2)
                throw new ArgumentOutOfRangeException(nameof(vertexes), this.vertexes.Count, "The leader vertexes list requires at least two points.");
            if (style == null)
                throw new ArgumentNullException(nameof(style));
            this.style = style;
            this.hasHookline = true;
            this.showArrowhead = true;
            this.pathType = LeaderPathType.StraightLineSegements;
            this.lineColor = AciColor.ByLayer;
            this.elevation = 0.0;
            this.offset = Vector2.Zero;
            this.styleOverrides = new DimensionStyleOverrideDictionary();
            this.styleOverrides.BeforeAddItem += this.StyleOverrides_BeforeAddItem;
            this.styleOverrides.AddItem += this.StyleOverrides_AddItem;
            this.styleOverrides.BeforeRemoveItem += this.StyleOverrides_BeforeRemoveItem;
            this.styleOverrides.RemoveItem += this.StyleOverrides_RemoveItem;
            this.annotation = this.BuildAnnotation(text);
            this.annotation.AddReactor(this);
            this.vertexes.Insert(this.vertexes.Count - 1, this.CalculateHookLine());
        }

        /// <summary>
        /// Initializes a new instance of the <c>Leader</c> class.
        /// </summary>
        public Leader(ToleranceEntry tolerance, IEnumerable<Vector2> vertexes)
            : this(tolerance, vertexes, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Leader</c> class.
        /// </summary>
        public Leader(ToleranceEntry tolerance, IEnumerable<Vector2> vertexes, DimensionStyle style)
            : base(EntityType.Leader, DxfObjectCode.Leader)
        {
            if (vertexes == null)
                throw new ArgumentNullException(nameof(vertexes));
            this.vertexes = new List<Vector2>(vertexes);
            if (this.vertexes.Count < 2)
                throw new ArgumentOutOfRangeException(nameof(vertexes), this.vertexes.Count, "The leader vertexes list requires at least two points.");
            if (style == null)
                throw new ArgumentNullException(nameof(style));
            this.style = style;
            this.hasHookline = false;
            this.showArrowhead = true;
            this.pathType = LeaderPathType.StraightLineSegements;
            this.lineColor = AciColor.ByLayer;
            this.elevation = 0.0;
            this.offset = Vector2.Zero;
            this.styleOverrides = new DimensionStyleOverrideDictionary();
            this.styleOverrides.BeforeAddItem += this.StyleOverrides_BeforeAddItem;
            this.styleOverrides.AddItem += this.StyleOverrides_AddItem;
            this.styleOverrides.BeforeRemoveItem += this.StyleOverrides_BeforeRemoveItem;
            this.styleOverrides.RemoveItem += this.StyleOverrides_RemoveItem;
            this.annotation = this.BuildAnnotation(tolerance);
            this.annotation.AddReactor(this);
        }

        /// <summary>
        /// Initializes a new instance of the <c>Leader</c> class.
        /// </summary>
        public Leader(Block block, IEnumerable<Vector2> vertexes)
            : this(block, vertexes, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Leader</c> class.
        /// </summary>
        public Leader(Block block, IEnumerable<Vector2> vertexes, DimensionStyle style)
            : base(EntityType.Leader, DxfObjectCode.Leader)
        {
            if (vertexes == null)
                throw new ArgumentNullException(nameof(vertexes));
            this.vertexes = new List<Vector2>(vertexes);
            if (this.vertexes.Count < 2)
                throw new ArgumentOutOfRangeException(nameof(vertexes), this.vertexes.Count, "The leader vertexes list requires at least two points.");
            if (style == null)
                throw new ArgumentNullException(nameof(style));
            this.style = style;
            this.hasHookline = false;
            this.showArrowhead = true;
            this.pathType = LeaderPathType.StraightLineSegements;
            this.lineColor = AciColor.ByLayer;
            this.elevation = 0.0;
            this.offset = Vector2.Zero;
            this.styleOverrides = new DimensionStyleOverrideDictionary();
            this.styleOverrides.BeforeAddItem += this.StyleOverrides_BeforeAddItem;
            this.styleOverrides.AddItem += this.StyleOverrides_AddItem;
            this.styleOverrides.BeforeRemoveItem += this.StyleOverrides_BeforeRemoveItem;
            this.styleOverrides.RemoveItem += this.StyleOverrides_RemoveItem;
            this.annotation = this.BuildAnnotation(block);
            this.annotation.AddReactor(this);
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the leader style.
        /// </summary>
        public DimensionStyle Style
        {
            get { return this.style; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.style = this.OnDimensionStyleChangedEvent(this.style, value);
            }
        }

        /// <summary>
        /// Gets the dimension style overrides list.
        /// </summary>
        /// <remarks>
        /// Any dimension style value stored in this list will override its corresponding value in the assigned style to
        /// the dimension.
        /// </remarks>
        public DimensionStyleOverrideDictionary StyleOverrides
        {
            get { return this.styleOverrides; }
        }

        /// <summary>
        /// Gets or sets if the arrowhead is drawn.
        /// </summary>
        public bool ShowArrowhead
        {
            get { return this.showArrowhead; }
            set { this.showArrowhead = value; }
        }

        /// <summary>
        /// Gets or sets the way the leader is drawn.
        /// </summary>
        public LeaderPathType PathType
        {
            get { return this.pathType; }
            set { this.pathType = value; }
        }

        /// <summary>
        /// Gets the leader vertexes list in local coordinates.
        /// </summary>
        public List<Vector2> Vertexes
        {
            get { return this.vertexes; }
        }

        /// <summary>
        /// Gets or sets the leader annotation entity.
        /// </summary>
        /// <remarks>
        /// Only MText, Text, Tolerance, and Insert entities are supported as a leader annotation.
        /// Even if AutoCad allows a Text entity to be part of a Leader it is not recommended, always use a MText entity instead.
        /// <br />
        /// Set the annotation property to null to create a Leader without annotation.
        /// </remarks>
        public EntityObject Annotation
        {
            get { return this.annotation; }
            set
            {
                if (value != null)
                {
                    if (!(value.Type == EntityType.MText ||
                          value.Type == EntityType.Text ||
                          value.Type == EntityType.Insert ||
                          value.Type == EntityType.Tolerance))
                        throw new ArgumentException("Only MText, Text, Insert, and Tolerance entities are supported as a leader annotation.", nameof(value));
                }

                if (ReferenceEquals(this.annotation, value))
                    return;

                if (this.annotation != null)
                    this.annotation.RemoveReactor(this);

                if (value != null)
                    value.AddReactor(this);

                this.annotation = value;
            }
        }

        /// <summary>
        /// Gets or sets the leader hook position (last leader vertex).
        /// </summary>
        /// <remarks>
        /// This property allows easy access to the last leader vertex, aka leader hook position.
        /// Remember the leader vertexes list must have at least two points.
        /// </remarks>
        public Vector2 Hook
        {
            get { return this.vertexes[this.vertexes.Count - 1]; }
            set { this.vertexes[this.vertexes.Count - 1] = value; }
        }

        /// <summary>
        /// Gets if the leader has a hook line.
        /// </summary>
        /// <remarks>
        /// If set to true an additional vertex point (StartHookLine) will be created before the leader end point (hook).
        /// By default, only leaders with text annotation have hook lines.
        /// </remarks>
        public bool HasHookline
        {
            get { return this.hasHookline; }
            set
            {
                if (this.hasHookline != value)
                {
                    if (value)
                        this.vertexes.Insert(this.vertexes.Count - 1, this.CalculateHookLine());
                    else
                        this.vertexes.RemoveAt(this.vertexes.Count - 2);
                }
                this.hasHookline = value;
            }
        }

        /// <summary>
        /// Gets or sets the leader line color if the style parameter DIMCLRD is set as BYBLOCK.
        /// </summary>
        public AciColor LineColor
        {
            get { return this.lineColor; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.lineColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the entity <see cref="Vector3">normal</see>.
        /// </summary>
        public new Vector3 Normal
        {
            get { return base.Normal; }
            set
            {
                this.ChangeAnnotationCoordinateSystem(value, this.elevation);
                base.Normal = value;
            }
        }

        /// <summary>
        /// Gets or sets the leader elevation.
        /// </summary>
        /// <remarks>This is the distance from the origin to the plane of the leader.</remarks>
        public double Elevation
        {
            get { return this.elevation; }
            set
            {
                this.ChangeAnnotationCoordinateSystem(this.Normal, value);
                this.elevation = value;
            }
        }

        /// <summary>
        /// Gets or sets the offset from the last leader vertex (hook) to the annotation position.
        /// </summary>
        public Vector2 Offset
        {
            get { return this.offset; }
            set { this.offset = value; }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Updates the leader entity to reflect the latest changes made to its properties.
        /// </summary>
        /// <param name="resetAnnotationPosition">
        /// If true the annotation position will be modified according to the position of the leader hook (last leader vertex),
        /// otherwise the leader hook will be moved according to the actual annotation position.
        /// </param>
        /// <remarks>
        /// This method should be manually called if the annotation position is modified, or the leader properties like Style, Annotation, TextVerticalPosition, and/or Offset.
        /// </remarks>
        public void Update(bool resetAnnotationPosition)
        {
            if (resetAnnotationPosition)
                this.ResetAnnotationPosition();
            else
                this.ResetHookPosition();

            if (this.hasHookline)
            {
                Vector2 vertex = this.CalculateHookLine();
                this.vertexes[this.vertexes.Count - 2] = vertex;
            }
        }

        #endregion

        #region private methods

        /// <summary>
        /// Resets the leader hook position according to the annotation position.
        /// </summary>
        private void ResetHookPosition()
        {
            if (this.vertexes.Count < 2)
                throw new Exception("The leader vertexes list requires at least two points.");

            if (this.annotation == null)
                return;

            DimensionStyleOverride styleOverride;
            DimensionStyleTextVerticalPlacement textVerticalPlacement = this.Style.TextVerticalPlacement;
            if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.TextVerticalPlacement, out styleOverride))
                textVerticalPlacement = (DimensionStyleTextVerticalPlacement)styleOverride.Value;
            double textOffset = this.Style.TextOffset;
            if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.TextOffset, out styleOverride))
                textOffset = (double)styleOverride.Value;
            double dimScale = this.Style.DimScaleOverall;
            if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.DimScaleOverall, out styleOverride))
                dimScale = (double)styleOverride.Value;
            double textHeight = this.Style.TextHeight;
            if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.TextHeight, out styleOverride))
                textHeight = (double)styleOverride.Value;
            AciColor textColor = this.Style.TextColor;
            if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.TextColor, out styleOverride))
                textColor = (AciColor)styleOverride.Value;

            Vector3 ocsHook;
            switch (this.annotation.Type)
            {
                case EntityType.MText:
                    MText mText = (MText) this.annotation;
                    ocsHook = MathHelper.Transform(mText.Position, this.Normal, CoordinateSystem.World, CoordinateSystem.Object);
                    int mTextSide = Math.Sign(ocsHook.X - this.vertexes[this.vertexes.Count - 2].X);

                    if (textVerticalPlacement == DimensionStyleTextVerticalPlacement.Centered)
                    {
                        double xOffset;

                        if (mTextSide >= 0)
                        {
                            mText.AttachmentPoint = MTextAttachmentPoint.MiddleLeft;
                            xOffset = -textOffset * dimScale;
                        }
                        else
                        {
                            mText.AttachmentPoint = MTextAttachmentPoint.MiddleRight;
                            xOffset = textOffset * dimScale;
                        }
                        this.vertexes[this.vertexes.Count - 1] = new Vector2(ocsHook.X + xOffset, ocsHook.Y) - this.offset;
                    }
                    else
                    {
                        ocsHook -= new Vector3(mTextSide* textOffset * dimScale, textOffset * dimScale, 0.0);
                        mText.AttachmentPoint = mTextSide >= 0 ? MTextAttachmentPoint.BottomLeft : MTextAttachmentPoint.BottomRight;
                        this.vertexes[this.vertexes.Count - 1] = new Vector2(ocsHook.X, ocsHook.Y) - this.offset;
                    }
                    mText.Height = textHeight * dimScale;
                    mText.Color = textColor.IsByBlock ? AciColor.ByLayer : textColor;
                    break;

                case EntityType.Insert:
                    Insert ins = (Insert) this.annotation;
                    ocsHook = MathHelper.Transform(ins.Position, this.Normal, CoordinateSystem.World, CoordinateSystem.Object);
                    this.vertexes[this.vertexes.Count - 1] = new Vector2(ocsHook.X, ocsHook.Y) - this.offset;
                    ins.Color = textColor.IsByBlock ? AciColor.ByLayer : textColor;
                    break;

                case EntityType.Tolerance:
                    Tolerance tol = (Tolerance) this.annotation;
                    ocsHook = MathHelper.Transform(tol.Position, this.Normal, CoordinateSystem.World, CoordinateSystem.Object);
                    this.vertexes[this.vertexes.Count - 1] = new Vector2(ocsHook.X, ocsHook.Y) - this.offset;
                    tol.Color = textColor.IsByBlock ? AciColor.ByLayer : textColor;
                    break;

                case EntityType.Text:
                    Text text = (Text) this.annotation;
                    ocsHook = MathHelper.Transform(text.Position, this.Normal, CoordinateSystem.World, CoordinateSystem.Object);
                    int textSide = Math.Sign(ocsHook.X - this.vertexes[this.vertexes.Count - 2].X);
                    if (textVerticalPlacement == DimensionStyleTextVerticalPlacement.Centered)
                    {
                        double xOffset;

                        if (textSide >= 0)
                        {
                            text.Alignment = TextAlignment.MiddleLeft;
                            xOffset = -textOffset * dimScale;
                        }
                        else
                        {
                            text.Alignment = TextAlignment.MiddleRight;
                            xOffset = textOffset * dimScale;
                        }
                        this.vertexes[this.vertexes.Count - 1] = new Vector2(ocsHook.X + xOffset, ocsHook.Y) - this.offset;
                    }
                    else
                    {
                        ocsHook -= new Vector3(textSide * textOffset * dimScale, textOffset * dimScale, 0.0);
                        text.Alignment = textSide >= 0 ? TextAlignment.BottomLeft : TextAlignment.BottomRight;
                        this.vertexes[this.vertexes.Count - 1] = new Vector2(ocsHook.X, ocsHook.Y) - this.offset;
                    }
                    text.Height = textHeight * dimScale;
                    text.Color = textColor.IsByBlock ? AciColor.ByLayer : textColor;
                    break;
                default:
                    throw new Exception(string.Format("The entity type: {0} not supported as a leader annotation.", this.annotation.Type));
            }
        }

        /// <summary>
        /// Resets the annotation position according to the leader hook.
        /// </summary>
        private void ResetAnnotationPosition()
        {
            if (this.vertexes.Count < 2)
                throw new Exception("The leader vertexes list requires at least two points.");

            if (this.annotation == null)
                return;

            DimensionStyleOverride styleOverride;
            DimensionStyleTextVerticalPlacement textVerticalPlacement = this.Style.TextVerticalPlacement;
            if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.TextVerticalPlacement, out styleOverride))
                textVerticalPlacement = (DimensionStyleTextVerticalPlacement)styleOverride.Value;
            double textOffset = this.Style.TextOffset;
            if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.TextOffset, out styleOverride))
                textOffset = (double)styleOverride.Value;
            double dimScale = this.Style.DimScaleOverall;
            if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.DimScaleOverall, out styleOverride))
                dimScale = (double)styleOverride.Value;
            double textHeight = this.Style.TextHeight;
            if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.TextHeight, out styleOverride))
                textHeight = (double)styleOverride.Value;
            AciColor textColor = this.Style.TextColor;
            if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.TextColor, out styleOverride))
                textColor = (AciColor)styleOverride.Value;

            Vector2 hook = this.vertexes[this.vertexes.Count - 1];
            Vector2 position;
            switch (this.annotation.Type)
            {
                case EntityType.MText:
                    MText mText = (MText) this.annotation;
                    Vector2 dir = this.vertexes[this.vertexes.Count - 1] - this.vertexes[this.vertexes.Count - 2];
                    double mTextXoffset = 0.0;
                    int mTextSide = Math.Sign(dir.X);
                    if (textVerticalPlacement == DimensionStyleTextVerticalPlacement.Centered)
                    {
                        if (mTextSide >= 0)
                        {
                            mText.AttachmentPoint = MTextAttachmentPoint.MiddleLeft;
                            mTextXoffset = -textOffset * dimScale;
                        }
                        else
                        {
                            mText.AttachmentPoint = MTextAttachmentPoint.MiddleRight;
                            mTextXoffset = textOffset * dimScale;
                        }
                        position = hook;
                    }
                    else
                    {
                        position = hook + new Vector2(mTextSide* textOffset * dimScale, textOffset * dimScale);
                        mText.AttachmentPoint = mTextSide >= 0 ? MTextAttachmentPoint.BottomLeft : MTextAttachmentPoint.BottomRight;
                    }

                    position = position + this.offset;
                    mText.Position = MathHelper.Transform(new Vector3(position.X - mTextXoffset, position.Y, this.elevation), this.Normal, CoordinateSystem.Object, CoordinateSystem.World);
                    mText.Height = textHeight * dimScale;
                    mText.Color = textColor.IsByBlock ? AciColor.ByLayer : textColor;
                    break;

                case EntityType.Insert:
                    Insert ins = (Insert) this.annotation;
                    position = hook + this.offset;
                    ins.Position = MathHelper.Transform(new Vector3(position.X, position.Y, this.elevation), this.Normal, CoordinateSystem.Object, CoordinateSystem.World);
                    ins.Color = textColor.IsByBlock ? AciColor.ByLayer : textColor;
                    break;

                case EntityType.Tolerance:
                    Tolerance tol = (Tolerance) this.annotation;
                    position = hook + this.offset;
                    tol.Position = MathHelper.Transform(new Vector3(position.X, position.Y, this.elevation), this.Normal, CoordinateSystem.Object, CoordinateSystem.World);
                    tol.Color = textColor.IsByBlock ? AciColor.ByLayer : textColor;
                    break;

                case EntityType.Text:
                    Text text = (Text) this.annotation;
                    double textXoffset = 0.0;
                    Vector2 textDir = this.vertexes[this.vertexes.Count - 1] - this.vertexes[this.vertexes.Count - 2];
                    int textSide = Math.Sign(textDir.X);

                    if (textVerticalPlacement == DimensionStyleTextVerticalPlacement.Centered)
                    {
                        if (textSide >= 0)
                        {
                            text.Alignment = TextAlignment.MiddleLeft;
                            textXoffset = -textOffset * dimScale;
                        }
                        else
                        {
                            text.Alignment = TextAlignment.MiddleRight;
                            textXoffset = textOffset * dimScale;
                        }
                        position = hook;
                    }
                    else
                    {
                        position = hook + new Vector2(textSide * textOffset * dimScale, textOffset * dimScale);
                        text.Alignment = textSide >= 0 ? TextAlignment.BottomLeft : TextAlignment.BottomRight;
                    }

                    position = position + this.offset;
                    text.Position = MathHelper.Transform(new Vector3(position.X - textXoffset, position.Y, this.elevation), this.Normal, CoordinateSystem.Object, CoordinateSystem.World);
                    text.Height = textHeight * dimScale;
                    text.Color = textColor.IsByBlock ? AciColor.ByLayer : textColor;
                    break;

                default:
                    throw new Exception(string.Format("The entity type: {0} not supported as a leader annotation.", this.annotation.Type));
            }
        }

        private MText BuildAnnotation(string text)
        {
            Vector2 hook = this.vertexes[this.vertexes.Count - 1];
            Vector2 dir = this.vertexes[this.vertexes.Count - 1] - this.vertexes[this.vertexes.Count - 2];
            int side = Math.Sign(dir.X);

            Vector2 position;
            MTextAttachmentPoint attachment;
            double mTextXoffset = 0.0;
            if (this.style.TextVerticalPlacement == DimensionStyleTextVerticalPlacement.Centered)
            {
                if (side >= 0)
                {
                    attachment = MTextAttachmentPoint.MiddleLeft;
                    mTextXoffset = -this.style.TextOffset * this.style.DimScaleOverall;
                }
                else
                {
                    attachment = MTextAttachmentPoint.MiddleRight;
                    mTextXoffset = this.style.TextOffset * this.style.DimScaleOverall;
                }
                position = hook;
            }
            else
            {
                position = hook + new Vector2(side * this.style.TextOffset * this.style.DimScaleOverall, this.style.TextOffset * this.style.DimScaleOverall);
                attachment = side >= 0 ? MTextAttachmentPoint.BottomLeft : MTextAttachmentPoint.BottomRight;
            }

            position = position + this.offset;
            Vector3 mTextPosition = MathHelper.Transform(new Vector3(position.X - mTextXoffset, position.Y, this.elevation), this.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            MText entity = new MText(text, mTextPosition, this.style.TextHeight * this.style.DimScaleOverall, 0.0, this.style.TextStyle)
            {
                Color = this.style.TextColor.IsByBlock ? AciColor.ByLayer : this.style.TextColor,
                AttachmentPoint = attachment
            };

            return entity;
        }

        private Insert BuildAnnotation(Block block)
        {
            return new Insert(block, this.vertexes[this.vertexes.Count - 1])
            {
                Color = this.style.TextColor.IsByBlock ? AciColor.ByLayer : this.style.TextColor
            };
        }

        private Tolerance BuildAnnotation(ToleranceEntry tolerance)
        {
            return new Tolerance(tolerance, this.vertexes[this.vertexes.Count - 1])
            {
                Color = this.style.TextColor.IsByBlock ? AciColor.ByLayer : this.style.TextColor,
                Style = this.style
            };
        }

        private Vector2 CalculateHookLine()
        {
            if (this.vertexes.Count < 2)
                throw new Exception("The leader vertexes list requires at least two points.");

            DimensionStyleOverride styleOverride;
            double dimScale = this.Style.DimScaleOverall;
            if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.DimScaleOverall, out styleOverride))
                dimScale = (double)styleOverride.Value;
            double arrowSize = this.Style.ArrowSize;
            if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.ArrowSize, out styleOverride))
                arrowSize = (double)styleOverride.Value;

            Vector2 v = this.Vertexes[this.Vertexes.Count - 1] - this.Vertexes[this.Vertexes.Count - 2];
            Vector2 dir = v.X >= 0 ? Vector2.UnitX : -Vector2.UnitX;
            if (this.annotation != null)
            {
                switch (this.annotation.Type)
                {
                    case EntityType.MText:
                        dir = Vector2.Rotate(dir, ((MText) this.annotation).Rotation*MathHelper.DegToRad);
                        break;
                    case EntityType.Text:
                        dir = Vector2.Rotate(dir, ((Text) this.annotation).Rotation*MathHelper.DegToRad);
                        break;
                    case EntityType.Insert:
                        dir = Vector2.Rotate(dir, ((Insert) this.annotation).Rotation*MathHelper.DegToRad);
                        break;
                    case EntityType.Tolerance:
                        dir = Vector2.Rotate(dir, ((Tolerance) this.annotation).Rotation*MathHelper.DegToRad);
                        break;
                    default:
                        throw new ArgumentException("Only MText, Text, Insert, and Tolerance entities are supported as a leader annotation.", nameof(this.annotation));
                }
            }
            return  this.Hook - dir * arrowSize * dimScale;
        }

        private void ChangeAnnotationCoordinateSystem(Vector3 newNormal, double newElevation)
        {
            if (this.annotation == null)
                return;

            Vector3 position;
            Vector3 ocsPosition;
            Vector3 wcsPosition;
            this.annotation.Normal = newNormal;
            switch (this.annotation.Type)
            {
                case EntityType.MText:
                    position = ((MText)this.annotation).Position;
                    ocsPosition = MathHelper.Transform(position, this.Normal, CoordinateSystem.World, CoordinateSystem.Object);
                    wcsPosition = MathHelper.Transform(new Vector3(ocsPosition.X, ocsPosition.Y, newElevation), newNormal, CoordinateSystem.Object, CoordinateSystem.World);
                    ((MText)this.annotation).Position = wcsPosition;
                    break;
                case EntityType.Insert:
                    position = ((Insert)this.annotation).Position;
                    ocsPosition = MathHelper.Transform(position, this.Normal, CoordinateSystem.World, CoordinateSystem.Object);
                    wcsPosition = MathHelper.Transform(new Vector3(ocsPosition.X, ocsPosition.Y, newElevation), newNormal, CoordinateSystem.Object, CoordinateSystem.World);
                    ((Insert)this.annotation).Position = wcsPosition;
                    break;
                case EntityType.Tolerance:
                    position = ((Tolerance)this.annotation).Position;
                    ocsPosition = MathHelper.Transform(position, this.Normal, CoordinateSystem.World, CoordinateSystem.Object);
                    wcsPosition = MathHelper.Transform(new Vector3(ocsPosition.X, ocsPosition.Y, newElevation), newNormal, CoordinateSystem.Object, CoordinateSystem.World);
                    ((Tolerance)this.annotation).Position = wcsPosition;
                    break;
                case EntityType.Text:
                    position = ((Text)this.annotation).Position;
                    ocsPosition = MathHelper.Transform(position, this.Normal, CoordinateSystem.World, CoordinateSystem.Object);
                    wcsPosition = MathHelper.Transform(new Vector3(ocsPosition.X, ocsPosition.Y, newElevation), newNormal, CoordinateSystem.Object, CoordinateSystem.World);
                    ((Text)this.annotation).Position = wcsPosition;
                    break;
            }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new Leader that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Leader that is a copy of this instance.</returns>
        public override object Clone()
        {
            Leader entity = new Leader(this.vertexes)
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
                //Leader properties
                Elevation = this.elevation,
                Style = (DimensionStyle) this.style.Clone(),
                ShowArrowhead = this.showArrowhead,
                PathType = this.pathType,
                Offset = this.offset,
                LineColor = this.lineColor,
                Annotation = (EntityObject) this.annotation?.Clone(),
                hasHookline = this.hasHookline // do not call directly the property, the vertexes list already includes it if it has a hook line
            };

            foreach (DimensionStyleOverride styleOverride in this.StyleOverrides.Values)
            {
                object copy;
                ICloneable value = styleOverride.Value as ICloneable;
                copy = value != null ? value.Clone() : styleOverride.Value;

                entity.StyleOverrides.Add(new DimensionStyleOverride(styleOverride.Type, copy));
            }

            foreach (XData data in this.XData.Values)
                entity.XData.Add((XData) data.Clone());

            return entity;
        }

        #endregion

        #region Dimension style overrides events

        private void StyleOverrides_BeforeAddItem(DimensionStyleOverrideDictionary sender, DimensionStyleOverrideDictionaryEventArgs e)
        {
            DimensionStyleOverride old;
            if (sender.TryGetValue(e.Item.Type, out old))
                if (ReferenceEquals(old.Value, e.Item.Value))
                    e.Cancel = true;
        }

        private void StyleOverrides_AddItem(DimensionStyleOverrideDictionary sender, DimensionStyleOverrideDictionaryEventArgs e)
        {
            this.OnDimensionStyleOverrideAddedEvent(e.Item);
        }

        private void StyleOverrides_BeforeRemoveItem(DimensionStyleOverrideDictionary sender, DimensionStyleOverrideDictionaryEventArgs e)
        {
        }

        private void StyleOverrides_RemoveItem(DimensionStyleOverrideDictionary sender, DimensionStyleOverrideDictionaryEventArgs e)
        {
            this.OnDimensionStyleOverrideRemovedEvent(e.Item);
        }

        #endregion
    }
}