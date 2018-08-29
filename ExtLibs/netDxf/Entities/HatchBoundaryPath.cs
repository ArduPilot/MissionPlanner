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

namespace netDxf.Entities
{
    /// <summary>
    /// Represent a loop of a <see cref="Hatch">hatch</see> boundaries.
    /// </summary>
    /// <remarks>
    /// The entities that make a loop can be any combination of lines, polylines, circles, arcs, ellipses, and splines.<br />
    /// The entities that define a loop must define a closed path and they have to be on the same plane as the hatch, 
    /// if these conditions are not met the result will be unpredictable.<br />
    /// The entity normal and the elevation will be ignored. Only the x and y coordinates of the line, ellipse, the circle, and spline will be used.
    /// Circles, full ellipses, closed polylines, closed splines are closed paths so only one should exist in the edges list.
    /// Lines, arcs, ellipse arcs, open polylines, and open splines are open paths so more entities should exist to make a closed loop.
    /// </remarks>
    public class HatchBoundaryPath :
        ICloneable
    {
        #region Hatch boundary path edge classes

        public enum EdgeType
        {
            Polyline = 0,
            Line = 1,
            Arc = 2,
            Ellipse = 3,
            Spline = 4
        }

        public abstract class Edge :
            ICloneable
        {
            public readonly EdgeType Type;

            protected Edge(EdgeType type)
            {
                this.Type = type;
            }

            public abstract EntityObject ConvertTo();
            public abstract object Clone();
        }

        public class Polyline :
            Edge
        {
            public Vector3[] Vertexes; // location: (x, y) bulge: z
            public bool IsClosed;

            public Polyline()
                : base(EdgeType.Polyline)
            {
            }

            public Polyline(EntityObject entity)
                : base(EdgeType.Polyline)
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));
                if (entity.Type == EntityType.LightWeightPolyline)
                {
                    Entities.LwPolyline poly = (Entities.LwPolyline) entity;
                    if (!poly.IsClosed)
                        throw new ArgumentException("Only closed polyline are supported as hatch boundary edges.", nameof(entity));

                    this.Vertexes = new Vector3[poly.Vertexes.Count];
                    for (int i = 0; i < poly.Vertexes.Count; i++)
                    {
                        this.Vertexes[i] = new Vector3(poly.Vertexes[i].Position.X, poly.Vertexes[i].Position.Y, poly.Vertexes[i].Bulge);
                    }
                    this.IsClosed = true;
                }
                else if (entity.Type == EntityType.Polyline)
                {
                    Entities.Polyline poly = (Entities.Polyline) entity;
                    if (!poly.IsClosed)
                        throw new ArgumentException("Only closed polyline are supported as hatch boundary edges.", nameof(entity));

                    this.Vertexes = new Vector3[poly.Vertexes.Count];
                    for (int i = 0; i < poly.Vertexes.Count; i++)
                    {
                        this.Vertexes[i] = new Vector3(poly.Vertexes[i].Position.X, poly.Vertexes[i].Position.Y, 0.0);
                    }
                    this.IsClosed = true;
                }
                else
                    throw new ArgumentException("The entity is not a LwPolyline or a Polyline", nameof(entity));
            }

            public static Polyline ConvertFrom(EntityObject entity)
            {
                return new Polyline(entity);
            }

            public override EntityObject ConvertTo()
            {
                List<LwPolylineVertex> points = new List<LwPolylineVertex>(this.Vertexes.Length);
                foreach (Vector3 point in this.Vertexes)
                {
                    points.Add(new LwPolylineVertex(point.X, point.Y, point.Z));
                }
                return new Entities.LwPolyline(points, this.IsClosed);
            }

            public override object Clone()
            {
                Polyline copy = new Polyline
                {
                    Vertexes = new Vector3[this.Vertexes.Length],
                    IsClosed = this.IsClosed
                };

                for (int i = 0; i < this.Vertexes.Length; i++)
                {
                    copy.Vertexes[i] = this.Vertexes[i];
                }
                return copy;
            }
        }

        public class Line :
            Edge
        {
            public Vector2 Start;
            public Vector2 End;

            public Line()
                : base(EdgeType.Line)
            {
            }

            public Line(EntityObject entity)
                : base(EdgeType.Line)
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                Entities.Line line = entity as Entities.Line;
                if (line == null)
                    throw new ArgumentException("The entity is not a Line", nameof(entity));

                this.Start = new Vector2(line.StartPoint.X, line.StartPoint.Y);
                this.End = new Vector2(line.EndPoint.X, line.EndPoint.Y);
            }

            public static Line ConvertFrom(EntityObject entity)
            {
                return new Line(entity);
            }

            public override EntityObject ConvertTo()
            {
                return new Entities.Line(this.Start, this.End);
            }

            public override object Clone()
            {
                Line copy = new Line
                {
                    Start = this.Start,
                    End = this.End
                };

                return copy;
            }
        }

        public class Arc :
            Edge
        {
            public Vector2 Center;
            public double Radius;
            public double StartAngle;
            public double EndAngle;
            public bool IsCounterclockwise;

            public Arc()
                : base(EdgeType.Arc)
            {
            }

            public Arc(EntityObject entity)
                : base(EdgeType.Arc)
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                switch (entity.Type)
                {
                    case EntityType.Arc:
                        Entities.Arc arc = (Entities.Arc) entity;
                        this.Center = new Vector2(arc.Center.X, arc.Center.Y);
                        this.Radius = arc.Radius;
                        this.StartAngle = arc.StartAngle;
                        this.EndAngle = arc.EndAngle;
                        this.IsCounterclockwise = true;
                        break;
                    case EntityType.Circle:
                        Entities.Circle circle = (Circle) entity;
                        this.Center = new Vector2(circle.Center.X, circle.Center.Y);
                        this.Radius = circle.Radius;
                        this.StartAngle = 0.0;
                        this.EndAngle = 360.0;
                        this.IsCounterclockwise = true;
                        break;
                    default:
                        throw new ArgumentException("The entity is not a Circle or an Arc", nameof(entity));
                }
            }

            public static Arc ConvertFrom(EntityObject entity)
            {
                return new Arc(entity);
            }

            public override EntityObject ConvertTo()
            {
                if (MathHelper.IsEqual(MathHelper.NormalizeAngle(this.StartAngle), MathHelper.NormalizeAngle(this.EndAngle)))
                    return new Entities.Circle(this.Center, this.Radius);
                if (this.IsCounterclockwise)
                    return new Entities.Arc(this.Center, this.Radius, this.StartAngle, this.EndAngle);

                return new Entities.Arc(this.Center, this.Radius, 360 - this.EndAngle, 360 - this.StartAngle);
            }

            public override object Clone()
            {
                Arc copy = new Arc
                {
                    Center = this.Center,
                    Radius = this.Radius,
                    StartAngle = this.StartAngle,
                    EndAngle = this.EndAngle,
                    IsCounterclockwise = this.IsCounterclockwise
                };

                return copy;
            }
        }

        public class Ellipse :
            Edge
        {
            public Vector2 Center;
            public Vector2 EndMajorAxis;
            public double MinorRatio;
            public double StartAngle;
            public double EndAngle;
            public bool IsCounterclockwise;

            public Ellipse()
                : base(EdgeType.Ellipse)
            {
            }

            public Ellipse(EntityObject entity)
                : base(EdgeType.Ellipse)
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                Entities.Ellipse ellipse = entity as Entities.Ellipse;
                if (ellipse == null)
                    throw new ArgumentException("The entity is not an Ellipse", nameof(entity));

                this.Center = new Vector2(ellipse.Center.X, ellipse.Center.Y);
                double sine = 0.5*ellipse.MajorAxis*Math.Sin(ellipse.Rotation*MathHelper.DegToRad);
                double cosine = 0.5*ellipse.MajorAxis*Math.Cos(ellipse.Rotation*MathHelper.DegToRad);
                this.EndMajorAxis = new Vector2(cosine, sine);
                this.MinorRatio = ellipse.MinorAxis/ellipse.MajorAxis;
                if (ellipse.IsFullEllipse)
                {
                    this.StartAngle = 0.0;
                    this.EndAngle = 360.0;
                }
                else
                {
                    this.StartAngle = ellipse.StartAngle;
                    this.EndAngle = ellipse.EndAngle;
                }
                this.IsCounterclockwise = true;
            }

            public static Ellipse ConvertFrom(EntityObject entity)
            {
                return new Ellipse(entity);
            }

            public override EntityObject ConvertTo()
            {
                Vector3 center = new Vector3(this.Center.X, this.Center.Y, 0.0);
                Vector3 axisPoint = new Vector3(this.EndMajorAxis.X, this.EndMajorAxis.Y, 0.0);
                Vector3 ocsAxisPoint = MathHelper.Transform(axisPoint,
                    Vector3.UnitZ,
                    CoordinateSystem.World,
                    CoordinateSystem.Object);
                double rotation = Vector2.Angle(new Vector2(ocsAxisPoint.X, ocsAxisPoint.Y))*MathHelper.RadToDeg;
                double majorAxis = 2*axisPoint.Modulus();
                return new Entities.Ellipse
                {
                    MajorAxis = majorAxis,
                    MinorAxis = majorAxis*this.MinorRatio,
                    Rotation = rotation,
                    Center = center,
                    StartAngle = this.IsCounterclockwise ? this.StartAngle : 360 - this.EndAngle,
                    EndAngle = this.IsCounterclockwise ? this.EndAngle : 360 - this.StartAngle,
                };
            }

            public override object Clone()
            {
                Ellipse copy = new Ellipse
                {
                    Center = this.Center,
                    EndMajorAxis = this.EndMajorAxis,
                    MinorRatio = this.MinorRatio,
                    StartAngle = this.StartAngle,
                    EndAngle = this.EndAngle,
                    IsCounterclockwise = this.IsCounterclockwise
                };

                return copy;
            }
        }

        public class Spline :
            Edge
        {
            public short Degree;
            public bool IsRational;
            public bool IsPeriodic;
            public double[] Knots;
            public Vector3[] ControlPoints; // location: (x, y) weight: z

            public Spline()
                : base(EdgeType.Spline)
            {
            }

            public Spline(EntityObject entity)
                : base(EdgeType.Spline)
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                Entities.Spline spline = entity as Entities.Spline;
                if (spline == null)
                    throw new ArgumentException("The entity is not an Spline", nameof(entity));

                this.Degree = spline.Degree;
                this.IsRational = spline.Flags.HasFlag(SplinetypeFlags.Rational);
                this.IsPeriodic = spline.IsPeriodic;
                if (spline.ControlPoints.Count == 0)
                    throw new ArgumentException("The HatchBoundaryPath spline edge requires a spline entity with control points.", nameof(entity));
                this.ControlPoints = new Vector3[spline.ControlPoints.Count];
                for (int i = 0; i < spline.ControlPoints.Count; i++)
                {
                    this.ControlPoints[i] = new Vector3(spline.ControlPoints[i].Position.X, spline.ControlPoints[i].Position.Y, spline.ControlPoints[i].Weigth);
                }
                this.Knots = new double[spline.Knots.Count];
                for (int i = 0; i < spline.Knots.Count; i++)
                {
                    this.Knots[i] = spline.Knots[i];
                }
            }

            public static Spline ConvertFrom(EntityObject entity)
            {
                return new Spline(entity);
            }

            public override EntityObject ConvertTo()
            {
                List<SplineVertex> ctrl = new List<SplineVertex>(this.ControlPoints.Length);
                foreach (Vector3 point in this.ControlPoints)
                {
                    ctrl.Add(new SplineVertex(point.X, point.Y, point.Z));
                }
                return new Entities.Spline(ctrl, new List<double>(this.Knots), this.Degree);
            }

            public override object Clone()
            {
                Spline copy = new Spline
                {
                    Degree = this.Degree,
                    IsRational = this.IsRational,
                    IsPeriodic = this.IsPeriodic,
                    Knots = new double[this.Knots.Length],
                    ControlPoints = new Vector3[this.ControlPoints.Length],
                };
                for (int i = 0; i < this.Knots.Length; i++)
                {
                    copy.Knots[i] = this.Knots[i];
                }
                for (int i = 0; i < this.ControlPoints.Length; i++)
                {
                    copy.ControlPoints[i] = this.ControlPoints[i];
                }
                return copy;
            }
        }

        #endregion

        #region private fields

        private readonly List<EntityObject> contour;
        private readonly List<Edge> edges;
        private HatchBoundaryPathTypeFlags pathType;

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <c>Hatch</c> class.
        /// </summary>
        /// <param name="edges">List of entities that makes a loop for the hatch boundary paths.</param>
        public HatchBoundaryPath(IEnumerable<EntityObject> edges)
        {
            if (edges == null)
                throw new ArgumentNullException(nameof(edges));
            this.edges = new List<Edge>();
            this.pathType = HatchBoundaryPathTypeFlags.Derived | HatchBoundaryPathTypeFlags.External;
            this.contour = new List<EntityObject>(edges);
            this.Update();
        }

        internal HatchBoundaryPath(IEnumerable<Edge> edges)
        {
            if (edges == null)
                throw new ArgumentNullException(nameof(edges));
            this.pathType = HatchBoundaryPathTypeFlags.Derived | HatchBoundaryPathTypeFlags.External;
            this.contour = new List<EntityObject>();
            this.edges = new List<Edge>(edges);
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the list of entities that makes a loop for the hatch boundary paths.
        /// </summary>
        public IReadOnlyList<Edge> Edges
        {
            get { return this.edges; }
        }

        /// <summary>
        /// Gets the boundary path type flag.
        /// </summary>
        public HatchBoundaryPathTypeFlags PathType
        {
            get { return this.pathType; }
            internal set { this.pathType = value; }
        }

        /// <summary>
        /// Gets the list of entities that makes the boundary.
        /// </summary>
        /// <remarks>If the boundary path belongs to a non-associative hatch this list will contain zero entities.</remarks>
        public IReadOnlyList<EntityObject> Entities
        {
            get { return this.contour; }
        }

        #endregion

        #region internal methods

        internal void AddContour(EntityObject entity)
        {
            this.contour.Add(entity);
        }

        internal void ClearContour()
        {
            this.contour.Clear();
        }

        internal bool RemoveContour(EntityObject entity)
        {
            return this.contour.Remove(entity);
        }

        #endregion

        #region public methods

        /// <summary>
        /// Updates the internal HatchBoundaryPath data. 
        /// </summary>
        /// <remarks>
        /// It is necessary to manually call this method when changes to the boundary entities are made. This is only applicable to associative hatches,
        /// non-associative hatches has no associated boundary entities.
        /// </remarks>
        public void Update()
        {
            this.SetInternalInfo(this.contour);
        }

        #endregion

        #region private methods

        private void SetInternalInfo(IEnumerable<EntityObject> entities)
        {
            bool containsClosedPolyline = false;
            this.edges.Clear();

            foreach (EntityObject entity in entities)
            {
                if (this.pathType.HasFlag(HatchBoundaryPathTypeFlags.Polyline))
                    if (this.edges.Count >= 1)
                        throw new ArgumentException("Closed polylines cannot be combined with other entities to make a hatch boundary path.");

                // it seems that AutoCad does not have problems on creating loops that theoretically does not make sense, like, for example an internal loop that is made of a single arc.
                // so if AutoCAD is OK with that I am too, the program that make use of this information will take care of this inconsistencies
                switch (entity.Type)
                {
                    case EntityType.Arc:
                        if (containsClosedPolyline)
                            throw new ArgumentException("Closed polylines cannot be combined with other entities to make a hatch boundary path.");
                        this.edges.Add(Arc.ConvertFrom(entity));
                        break;
                    case EntityType.Circle:
                        if (containsClosedPolyline)
                            throw new ArgumentException("Closed polylines cannot be combined with other entities to make a hatch boundary path.");
                        this.edges.Add(Arc.ConvertFrom(entity));
                        break;
                    case EntityType.Ellipse:
                        if (containsClosedPolyline)
                            throw new ArgumentException("Closed polylines cannot be combined with other entities to make a hatch boundary path.");
                        this.edges.Add(Ellipse.ConvertFrom(entity));
                        break;
                    case EntityType.Line:
                        if (containsClosedPolyline)
                            throw new ArgumentException("Closed polylines cannot be combined with other entities to make a hatch boundary path.");
                        this.edges.Add(Line.ConvertFrom(entity));
                        break;
                    case EntityType.LightWeightPolyline:
                        if (containsClosedPolyline)
                            throw new ArgumentException("Closed polylines cannot be combined with other entities to make a hatch boundary path.");
                        LwPolyline poly = (LwPolyline) entity;
                        if (poly.IsClosed)
                        {
                            this.edges.Add(Polyline.ConvertFrom(entity)); // A polyline HatchBoundaryPath must be closed
                            this.pathType |= HatchBoundaryPathTypeFlags.Polyline;
                            containsClosedPolyline = true;
                        }
                        else
                            this.SetInternalInfo(poly.Explode()); // open polylines will always be exploded, only one polyline can be present in a path
                        break;
                    case EntityType.Spline:
                        if (containsClosedPolyline)
                            throw new ArgumentException("Closed polylines cannot be combined with other entities to make a hatch boundary path.");
                        this.edges.Add(Spline.ConvertFrom(entity));
                        break;
                    default:
                        throw new ArgumentException(string.Format("The entity type {0} cannot be part of a hatch boundary.", entity.Type));
                }
            }
        }

        #endregion

        #region ICloneable

        /// <summary>
        /// Creates a new HatchBoundaryPath that is a copy of the current instance.
        /// </summary>
        /// <returns>A new HatchBoundaryPath that is a copy of this instance.</returns>
        public object Clone()
        {
            HatchBoundaryPath copy;

            //// the hatch is associative
            if (this.contour.Count > 0)
            {
                List<EntityObject> copyContour = new List<EntityObject>();
                foreach (EntityObject entity in this.contour)
                    copyContour.Add((EntityObject)entity.Clone());
                copy = new HatchBoundaryPath(copyContour);
            }
            else
            {
                List<Edge> copyEdges = new List<Edge>();
                foreach (Edge edge in this.edges)
                    copyEdges.Add((Edge) edge.Clone());
                copy = new HatchBoundaryPath(copyEdges);
            }
            copy.PathType = this.pathType;
            return copy;
        }

        #endregion
    }
}