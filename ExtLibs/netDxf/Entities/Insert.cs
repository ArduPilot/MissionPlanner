#region netDxf library, Copyright (C) 2009-2016 Daniel Carvajal (haplokuon@gmail.com)

//                        netDxf library
// Copyright (C) 2009-2016 Daniel Carvajal (haplokuon@gmail.com)
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
using netDxf.Units;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a block insertion <see cref="EntityObject">entity</see>.
    /// </summary>
    public class Insert :
        EntityObject
    {
        #region delegates and events

        public delegate void AttributeAddedEventHandler(Insert sender, AttributeChangeEventArgs e);

        public event AttributeAddedEventHandler AttributeAdded;

        protected virtual void OnAttributeAddedEvent(Attribute item)
        {
            AttributeAddedEventHandler ae = this.AttributeAdded;
            if (ae != null)
                ae(this, new AttributeChangeEventArgs(item));
        }

        public delegate void AttributeRemovedEventHandler(Insert sender, AttributeChangeEventArgs e);

        public event AttributeRemovedEventHandler AttributeRemoved;

        protected virtual void OnAttributeRemovedEvent(Attribute item)
        {
            AttributeRemovedEventHandler ae = this.AttributeRemoved;
            if (ae != null)
                ae(this, new AttributeChangeEventArgs(item));
        }

        #endregion

        #region private fields

        private readonly EndSequence endSequence;
        private Block block;
        private Vector3 position;
        private Vector3 scale;
        private double rotation;
        private AttributeCollection attributes;

        #endregion

        #region constructors

        internal Insert(List<Attribute> attributes)
            : base(EntityType.Insert, DxfObjectCode.Insert)
        {
            if(attributes == null)
                throw new ArgumentNullException(nameof(attributes));
            this.attributes = new AttributeCollection(attributes);
            foreach (Attribute att in this.attributes)
            {
                if(att.Owner!=null)
                    throw new ArgumentException("The attributes list contains at least an attribute that already has an owner.", nameof(attributes));
                att.Owner = this;
            }

            this.endSequence = new EndSequence(this);

        }

        /// <summary>
        /// Initializes a new instance of the <c>Insert</c> class.
        /// </summary>
        /// <param name="block">Insert <see cref="Block">block definition</see>.</param>
        public Insert(Block block)
            : this(block, Vector3.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Insert</c> class.
        /// </summary>
        /// <param name="block">Insert block definition.</param>
        /// <param name="position">Insert <see cref="Vector2">position</see> in world coordinates.</param>
        public Insert(Block block, Vector2 position)
            : this(block, new Vector3(position.X, position.Y, 0.0))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Insert</c> class.
        /// </summary>
        /// <param name="block">Insert block definition.</param>
        /// <param name="position">Insert <see cref="Vector3">point</see> in world coordinates.</param>
        public Insert(Block block, Vector3 position)
            : base(EntityType.Insert, DxfObjectCode.Insert)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            this.block = block;
            this.position = position;
            this.scale = new Vector3(1.0);
            this.rotation = 0.0;
            this.endSequence = new EndSequence(this);

            List<Attribute> atts = new List<Attribute>(block.AttributeDefinitions.Count);
            foreach (AttributeDefinition attdef in block.AttributeDefinitions.Values)
            {
                Attribute att = new Attribute(attdef)
                {
                    Position = attdef.Position + this.position - this.block.Origin,
                    Owner = this
                };
                atts.Add(att);
            }

            this.attributes = new AttributeCollection(atts);
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the insert list of <see cref="Attribute">attributes</see>.
        /// </summary>
        public AttributeCollection Attributes
        {
            get { return this.attributes; }
        }

        /// <summary>
        /// Gets the insert <see cref="Block">block definition</see>.
        /// </summary>
        public Block Block
        {
            get { return this.block; }
            internal set { this.block = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="Vector3">position</see> in world coordinates.
        /// </summary>
        public Vector3 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        /// <summary>
        /// Gets or sets the insert <see cref="Vector3">scale</see>.
        /// </summary>
        public Vector3 Scale
        {
            get { return this.scale; }
            set { this.scale = value; }
        }

        /// <summary>
        /// Gets or sets the insert rotation along the normal vector in degrees.
        /// </summary>
        public double Rotation
        {
            get { return this.rotation; }
            set { this.rotation = MathHelper.NormalizeAngle(value); }
        }

        #endregion

        #region internal properties

        internal EndSequence EndSequence
        {
            get { return this.endSequence; }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Updates the actual insert with the attribute properties currently defined in the block. This does not affect any values assigned to attributes in each block.
        /// </summary>
        /// <remarks>This method will automatically call the TransformAttributes method, to keep all attributes position and orientation up to date.</remarks>
        /// <remarks></remarks>
        public void Sync()
        {
            List<Attribute> atts = new List<Attribute>(this.block.AttributeDefinitions.Count);

            // remove all attributes in the actual insert
            foreach (Attribute att in this.attributes)
            {
                this.OnAttributeRemovedEvent(att);
                att.Handle = null;
                att.Owner = null;
            }

            // add any new attributes from the attribute definitions of the block
            foreach (AttributeDefinition attdef in this.block.AttributeDefinitions.Values)
            {
                Attribute att = new Attribute(attdef)
                {
                    Owner = this
                };
                atts.Add(att);
                this.OnAttributeAddedEvent(att);
            }
            this.attributes = new AttributeCollection(atts);

            this.TransformAttributes();
        }

        /// <summary>
        /// Calculates the insertion rotation matrix.
        /// </summary>
        /// <param name="insertionUnits">The insertion units.</param>
        /// <returns>The insert rotation matrix.</returns>
        public Matrix3 GetTransformation(DrawingUnits insertionUnits)
        {
            double docScale = UnitHelper.ConversionFactor(this.Block.Record.Units, insertionUnits);
            Matrix3 trans = MathHelper.ArbitraryAxis(this.Normal);
            trans *= Matrix3.RotationZ(this.rotation*MathHelper.DegToRad);
            trans *= Matrix3.Scale(this.scale*docScale);

            return trans;
        }

        /// <summary>
        /// Recalculate the attributes position, normal, rotation, text height, width factor, and oblique angle from the values applied to the insertion.
        /// </summary>
        /// <remarks>
        /// Changes to the insert, the block, or the document insertion units will require this method to be called manually.<br />
        /// The attributes position, normal, rotation, text height, width factor, and oblique angle values includes the transformations applied to the insertion,
        /// if required this method will calculate the proper values according to the ones defined by the attribute definition.<br />
        /// All the attribute values can be changed manually independently to its definition,
        /// but, usually, you will want them to be transformed with the insert based on the local values defined by the attribute definition.<br />
        /// This method only applies to attributes that have a definition, some dxf files might generate attributes that have no definition in the block.<br />
        /// At the moment the attribute width factor and oblique angle are not calculated, this is applied to inserts with non uniform scaling.
        /// </remarks>
        public void TransformAttributes()
        {
            // if the insert does not contain attributes there is nothing to do
            if (this.attributes.Count == 0)
                return;

            DrawingUnits insUnits;

            if (this.Owner == null)
                insUnits = DrawingUnits.Unitless;
            else
                // if the insert belongs to a block the units to use are the ones defined in the BlockRecord
                // if the insert belongs to a layout the units to use are the ones defined in the Document
                insUnits = this.Owner.Record.Layout == null ? this.Owner.Record.Units : this.Owner.Record.Owner.Owner.DrawingVariables.InsUnits;

            Vector3 insScale = this.scale*UnitHelper.ConversionFactor(this.block.Record.Units, insUnits);
            Matrix3 insTrans = this.GetTransformation(insUnits);

            foreach (Attribute att in this.attributes)
            {
                AttributeDefinition attdef = att.Definition;
                if (attdef == null)
                    continue;

                Vector3 wcsAtt = insTrans*(attdef.Position - this.block.Origin);
                att.Position = this.position + wcsAtt;

                Vector2 txtU = new Vector2(attdef.WidthFactor, 0.0);
                txtU = MathHelper.Transform(txtU, attdef.Rotation*MathHelper.DegToRad, CoordinateSystem.Object, CoordinateSystem.World);
                Vector3 ocsTxtU = MathHelper.Transform(new Vector3(txtU.X, txtU.Y, 0.0), attdef.Normal, CoordinateSystem.Object, CoordinateSystem.World);
                Vector3 wcsTxtU = insTrans*ocsTxtU;

                Vector2 txtV = new Vector2(0.0, attdef.Height);
                txtV = MathHelper.Transform(txtV, attdef.Rotation*MathHelper.DegToRad, CoordinateSystem.Object, CoordinateSystem.World);
                Vector3 ocsTxtV = MathHelper.Transform(new Vector3(txtV.X, txtV.Y, 0.0), attdef.Normal, CoordinateSystem.Object, CoordinateSystem.World);
                Vector3 wcsTxtV = insTrans*ocsTxtV;

                Vector3 txtNormal = Vector3.CrossProduct(wcsTxtU, wcsTxtV);
                att.Normal = txtNormal;

                double txtHeight = MathHelper.PointLineDistance(wcsTxtV, Vector3.Zero, Vector3.Normalize(wcsTxtU));
                att.Height = txtHeight;

                double txtAng = Vector2.Angle(new Vector2(txtU.X*insScale.X, txtU.Y*insScale.Y))*MathHelper.RadToDeg;
                if (Vector3.Equals(attdef.Normal, Vector3.UnitZ))
                {
                    att.Rotation = this.rotation + txtAng;

                    //double txtWidth = MathHelper.PointLineDistance(wcsTxtU, Vector3.Zero, Vector3.Normalize(wcsTxtV));
                    //att.WidthFactor = txtWidth;
                    att.WidthFactor = attdef.WidthFactor;

                    //double a1d1Ang = Vector2.Angle(new Vector2(txtV.X * insScale.X, txtV.Y * insScale.Y)) * MathHelper.RadToDeg;
                    //double oblique = 90 - (a1d1Ang - txtAng);
                    //if (oblique < -85.0 || oblique > 85.0) oblique = Math.Sign(oblique) * 85;
                    //att.ObliqueAngle = oblique;
                    att.ObliqueAngle = attdef.ObliqueAngle;
                }
                else
                {
                    att.Rotation = txtAng;
                    att.WidthFactor = attdef.WidthFactor;
                    att.ObliqueAngle = attdef.ObliqueAngle;
                }
            }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Assigns a handle to the object based in a integer counter.
        /// </summary>
        /// <param name="entityNumber">Number to assign.</param>
        /// <returns>Next available entity number.</returns>
        /// <remarks>
        /// Some objects might consume more than one, is, for example, the case of polylines that will assign
        /// automatically a handle to its vertexes. The entity number will be converted to an hexadecimal number.
        /// </remarks>
        internal override long AsignHandle(long entityNumber)
        {
            entityNumber = this.endSequence.AsignHandle(entityNumber);
            foreach (Attribute attrib in this.attributes)
            {
                entityNumber = attrib.AsignHandle(entityNumber);
            }
            return base.AsignHandle(entityNumber);
        }


        /// <summary>
        /// Creates a new Insert that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Insert that is a copy of this instance.</returns>
        public override object Clone()
        {
            // copy attributes
            List<Attribute> copyAttributes = new List<Attribute>();
            foreach (Attribute att in this.attributes)
                copyAttributes.Add((Attribute)att.Clone());

            Insert entity = new Insert(copyAttributes)
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
                //Insert properties
                Position = this.position,
                Block = (Block) this.block.Clone(),
                Scale = this.scale,
                Rotation = this.rotation,
            };

            // copy extended data
            foreach (XData data in this.XData.Values)
                entity.XData.Add((XData) data.Clone());

            return entity;
        }

        #endregion

        #region Explode

        //public List<EntityObject> Explode()
        //{
        //    List<EntityObject> entities = new List<EntityObject>();
        //    Matrix3 trans = this.GetTransformation();
        //    Vector3 pos = this.position - trans*this.block.Position;

        //    foreach (EntityObject entity in this.block.Entities)
        //    {
        //        switch (entity.Type)
        //        {
        //            case (EntityType.Arc):
        //                entities.Add(ProcessArc((Arc)entity, trans, pos, this.scale, this.rotation));
        //                break;
        //            case (EntityType.Circle):
        //                entities.Add(ProcessCircle((Circle)entity, trans, pos, this.scale, this.rotation));
        //                break;
        //            case (EntityType.Ellipse):
        //                entities.Add(ProcessEllipse((Ellipse)entity, trans, pos, this.scale, this.rotation));
        //                break;
        //            case (EntityType.Face3D):
        //                entities.Add(ProcessFace3d((Face3d)entity, trans, pos));
        //                break;
        //            case(EntityType.Hatch):
        //                entities.Add(ProcessHatch((Hatch)entity, trans, pos, this.position, this.Normal, this.scale, this.rotation));
        //                break;
        //            case (EntityType.Line):
        //                entities.Add(ProcessLine((Line)entity, trans, pos));
        //                break;
        //        }
        //    }

        //    return entities;
        //}

        //#region private methods

        //private static EntityObject ProcessArc(Arc arc, Matrix3 trans, Vector3 pos, Vector3 scale, double rotation)
        //{
        //    EntityObject copy;
        //    if (MathHelper.IsEqual(scale.X, scale.Y))
        //    {
        //        copy = (Arc)arc.Clone();
        //        ((Arc)copy).Center = trans * arc.Center + pos;
        //        ((Arc)copy).Radius = arc.Radius * scale.X;
        //        ((Arc)copy).StartAngle = arc.StartAngle + rotation;
        //        ((Arc)copy).EndAngle = arc.EndAngle + rotation;
        //        copy.Normal = trans * arc.Normal;
        //        return copy;
        //    }
        //    copy = new Ellipse
        //    {
        //        Center = trans * arc.Center + pos,
        //        MajorAxis = 2 * arc.Radius * scale.X,
        //        MinorAxis = 2 * arc.Radius * scale.Y,
        //        StartAngle = arc.StartAngle,
        //        EndAngle = arc.EndAngle,
        //        Rotation = rotation,
        //        Thickness = arc.Thickness,
        //        Color = arc.Color,
        //        Layer = arc.Layer,
        //        Linetype = arc.Linetype,
        //        LinetypeScale = arc.LinetypeScale,
        //        Lineweight = arc.Lineweight,
        //        Normal = trans * arc.Normal,
        //        XData = arc.XData
        //    };

        //    return copy;
        //}

        //private static EntityObject ProcessCircle(Circle circle, Matrix3 trans, Vector3 pos, Vector3 scale, double rotation)
        //{
        //    EntityObject copy;
        //    if (MathHelper.IsEqual(scale.X, scale.Y))
        //    {
        //        copy = (Circle)circle.Clone();
        //        ((Circle)copy).Center = trans * circle.Center + pos;
        //        ((Circle)copy).Radius = circle.Radius * scale.X;
        //        copy.Normal = trans * circle.Normal;
        //        return copy;
        //    }
        //    copy = new Ellipse
        //    {
        //        Center = trans * circle.Center + pos,
        //        MajorAxis = 2 * circle.Radius * scale.X,
        //        MinorAxis = 2 * circle.Radius * scale.Y,
        //        Rotation = rotation,
        //        Thickness = circle.Thickness,
        //        Color = circle.Color,
        //        Layer = circle.Layer,
        //        Linetype = circle.Linetype,
        //        LinetypeScale = circle.LinetypeScale,
        //        Lineweight = circle.Lineweight,
        //        Normal = trans * circle.Normal,
        //        XData = circle.XData
        //    };
        //    return copy;
        //}

        //private static Ellipse ProcessEllipse(Ellipse ellipse, Matrix3 trans, Vector3 pos, Vector3 scale, double rotation)
        //{
        //    Ellipse copy = (Ellipse)ellipse.Clone();
        //    copy.Center = trans * ellipse.Center + pos;
        //    copy.MajorAxis = ellipse.MajorAxis * scale.X;
        //    copy.MinorAxis = ellipse.MinorAxis * scale.Y;
        //    copy.Rotation = rotation;
        //    copy.Normal = trans * ellipse.Normal;

        //    return copy;
        //}

        //private static Face3d ProcessFace3d(Face3d face3d, Matrix3 trans, Vector3 pos)
        //{
        //    Face3d copy = (Face3d)face3d.Clone();
        //    copy.FirstVertex = trans * face3d.FirstVertex + pos;
        //    copy.SecondVertex = trans * face3d.SecondVertex + pos;
        //    copy.ThirdVertex = trans * face3d.ThirdVertex + pos;
        //    copy.FourthVertex = trans * face3d.FourthVertex + pos;

        //    return copy;
        //}

        //private static Hatch ProcessHatch(Hatch hatch, Matrix3 trans, Vector3 pos, Vector3 insertPos, Vector3 normal, Vector3 scale, double rotation)
        //{
        //    List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>();
        //    Matrix3 dataTrans = Matrix3.RotationZ(rotation * MathHelper.DegToRad) * Matrix3.Scale(scale);
        //    Vector3 localPos = MathHelper.Transform(insertPos, normal, MathHelper.CoordinateSystem.World, MathHelper.CoordinateSystem.Object);
        //    foreach (HatchBoundaryPath path in hatch.BoundaryPaths)
        //    {
        //        List<EntityObject> data = new List<EntityObject>();
        //        foreach (EntityObject entity in path.Data)
        //        {
        //            switch (entity.Type)
        //            {
        //                case (EntityType.Arc):
        //                    data.Add(ProcessArc((Arc)entity, trans, pos, scale, 0));
        //                    break;
        //                case (EntityType.Circle):
        //                    data.Add(ProcessCircle((Circle)entity, trans, pos, scale, 0));
        //                    break;
        //                case (EntityType.Ellipse):
        //                    data.Add(ProcessEllipse((Ellipse)entity, trans, pos, scale, 0));
        //                    break;
        //                case (EntityType.Line):
        //                    data.Add(ProcessLine((Line)entity, dataTrans, localPos));
        //                    break;
        //            }
        //        }

        //        boundary.Add(new HatchBoundaryPath(data));
        //    }

        //    // the insert scale will not modify the hatch pattern even thought AutoCad does
        //    Hatch copy = (Hatch)hatch.Clone();
        //    copy.BoundaryPaths = boundary;
        //    copy.Elevation = localPos.Z + hatch.Elevation;
        //    copy.Normal = trans * hatch.Normal;
        //    return copy;
        //}

        //private static Line ProcessLine(Line line, Matrix3 trans, Vector3 pos)
        //{
        //    Line copy = (Line)line.Clone();
        //    copy.StartPoint = trans * line.StartPoint + pos;
        //    copy.EndPoint = trans * line.EndPoint + pos;
        //    copy.Normal = trans * line.Normal;

        //    return copy;
        //}

        //#endregion

        #endregion
    }
}