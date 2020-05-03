// Copyright 2005 - 2009 - Morten Nielsen (www.sharpgis.net)
//
// This file is part of ProjNet.
// ProjNet is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// ProjNet is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with ProjNet; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Globalization;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems.Projections;

namespace ProjNet.CoordinateSystems.Transformations
{
	/// <summary>
	/// Creates coordinate transformations.
	/// </summary>
	public class CoordinateTransformationFactory : ICoordinateTransformationFactory
	{
		#region ICoordinateTransformationFactory Members

		/// <summary>
		/// Creates a transformation between two coordinate systems.
		/// </summary>
		/// <remarks>
		/// This method will examine the coordinate systems in order to construct
		/// a transformation between them. This method may fail if no path between 
		/// the coordinate systems is found, using the normal failing behavior of 
		/// the DCP (e.g. throwing an exception).</remarks>
		/// <param name="sourceCS">Source coordinate system</param>
		/// <param name="targetCS">Target coordinate system</param>
		/// <returns></returns>		
		public ICoordinateTransformation CreateFromCoordinateSystems(ICoordinateSystem sourceCS, ICoordinateSystem targetCS)
		{
			ICoordinateTransformation trans;
            if (sourceCS is IProjectedCoordinateSystem && targetCS is IGeographicCoordinateSystem) //Projected -> Geographic
                trans = Proj2Geog((IProjectedCoordinateSystem)sourceCS, (IGeographicCoordinateSystem)targetCS);            
            else if (sourceCS is IGeographicCoordinateSystem && targetCS is IProjectedCoordinateSystem) //Geographic -> Projected
				trans = Geog2Proj((IGeographicCoordinateSystem)sourceCS, (IProjectedCoordinateSystem)targetCS);

            else if (sourceCS is IGeographicCoordinateSystem && targetCS is IGeocentricCoordinateSystem) //Geocentric -> Geographic
				trans = Geog2Geoc((IGeographicCoordinateSystem)sourceCS, (IGeocentricCoordinateSystem)targetCS);

            else if (sourceCS is IGeocentricCoordinateSystem && targetCS is IGeographicCoordinateSystem) //Geocentric -> Geographic
				trans = Geoc2Geog((IGeocentricCoordinateSystem)sourceCS, (IGeographicCoordinateSystem)targetCS);

            else if (sourceCS is IProjectedCoordinateSystem && targetCS is IProjectedCoordinateSystem) //Projected -> Projected
				trans = Proj2Proj((sourceCS as IProjectedCoordinateSystem), (targetCS as IProjectedCoordinateSystem));

            else if (sourceCS is IGeocentricCoordinateSystem && targetCS is IGeocentricCoordinateSystem) //Geocentric -> Geocentric
				trans = CreateGeoc2Geoc((IGeocentricCoordinateSystem)sourceCS, (IGeocentricCoordinateSystem)targetCS);

            else if (sourceCS is IGeographicCoordinateSystem && targetCS is IGeographicCoordinateSystem) //Geographic -> Geographic
				trans = CreateGeog2Geog(sourceCS as IGeographicCoordinateSystem, targetCS as IGeographicCoordinateSystem);
			else if (sourceCS is IFittedCoordinateSystem) //Fitted -> Any
                trans = Fitt2Any ((IFittedCoordinateSystem)sourceCS, targetCS);
            else if (targetCS is IFittedCoordinateSystem) //Any -> Fitted 
                trans = Any2Fitt (sourceCS, (IFittedCoordinateSystem)targetCS);
            else
				throw new NotSupportedException("No support for transforming between the two specified coordinate systems");
			
			//if (trans.MathTransform is ConcatenatedTransform) {
			//    List<ICoordinateTransformation> MTs = new List<ICoordinateTransformation>();
			//    SimplifyTrans(trans.MathTransform as ConcatenatedTransform, ref MTs);
			//    return new CoordinateTransformation(sourceCS,
			//        targetCS, TransformType.Transformation, new ConcatenatedTransform(MTs),
			//        String.Empty, String.Empty, -1, String.Empty, String.Empty);
			//}
			return trans;
		}
		#endregion

		private static void SimplifyTrans(ConcatenatedTransform mtrans, ref List<ICoordinateTransformation> MTs)
		{
			foreach(ICoordinateTransformation t in mtrans.CoordinateTransformationList)
			{
				if(t is ConcatenatedTransform)
					SimplifyTrans(t as ConcatenatedTransform, ref MTs);
				else
					MTs.Add(t);
			}			
		}

		#region Methods for converting between specific systems

        private static ICoordinateTransformation Geog2Geoc(IGeographicCoordinateSystem source, IGeocentricCoordinateSystem target)
        {
            IMathTransform geocMathTransform = CreateCoordinateOperation(target);
            if (source.PrimeMeridian.EqualParams(target.PrimeMeridian))
            {
                return new CoordinateTransformation(source, target, TransformType.Conversion, geocMathTransform, String.Empty, String.Empty, -1, String.Empty, String.Empty);
            }
            else
            {
                var ct = new ConcatenatedTransform();
                ct.CoordinateTransformationList.Add(new CoordinateTransformation(source, target, TransformType.Transformation, new PrimeMeridianTransform(source.PrimeMeridian, target.PrimeMeridian), String.Empty, String.Empty, -1, String.Empty, String.Empty));
                ct.CoordinateTransformationList.Add(new CoordinateTransformation(source, target, TransformType.Conversion, geocMathTransform, String.Empty, String.Empty, -1, String.Empty, String.Empty));
                return new CoordinateTransformation(source, target, TransformType.Conversion, ct, String.Empty, String.Empty, -1, String.Empty, String.Empty);
            }
        }

        private static ICoordinateTransformation Geoc2Geog(IGeocentricCoordinateSystem source, IGeographicCoordinateSystem target)
        {
            IMathTransform geocMathTransform = CreateCoordinateOperation(source).Inverse();
            if (source.PrimeMeridian.EqualParams(target.PrimeMeridian))
            {
                return new CoordinateTransformation(source, target, TransformType.Conversion, geocMathTransform, String.Empty, String.Empty, -1, String.Empty, String.Empty);
            }
            else
            {
                var ct = new ConcatenatedTransform();
                ct.CoordinateTransformationList.Add(new CoordinateTransformation(source, target, TransformType.Conversion, geocMathTransform, String.Empty, String.Empty, -1, String.Empty, String.Empty));
                ct.CoordinateTransformationList.Add(new CoordinateTransformation(source, target, TransformType.Transformation, new PrimeMeridianTransform(source.PrimeMeridian, target.PrimeMeridian), String.Empty, String.Empty, -1, String.Empty, String.Empty));
                return new CoordinateTransformation(source, target, TransformType.Conversion, ct, String.Empty, String.Empty, -1, String.Empty, String.Empty);
            }
        }
		
		private static ICoordinateTransformation Proj2Proj(IProjectedCoordinateSystem source, IProjectedCoordinateSystem target)
		{
			var ct = new ConcatenatedTransform();
			var ctFac = new CoordinateTransformationFactory();
			//First transform from projection to geographic
			ct.CoordinateTransformationList.Add(ctFac.CreateFromCoordinateSystems(source, source.GeographicCoordinateSystem));
			//Transform geographic to geographic:
		    var geogToGeog = ctFac.CreateFromCoordinateSystems(source.GeographicCoordinateSystem,
		                                                      target.GeographicCoordinateSystem);
            if (geogToGeog != null)
                ct.CoordinateTransformationList.Add(geogToGeog);
			//Transform to new projection
			ct.CoordinateTransformationList.Add(ctFac.CreateFromCoordinateSystems(target.GeographicCoordinateSystem, target));

			return new CoordinateTransformation(source,
				target, TransformType.Transformation, ct,
				String.Empty, String.Empty, -1, String.Empty, String.Empty);
		}		

        private static ICoordinateTransformation Geog2Proj(IGeographicCoordinateSystem source, IProjectedCoordinateSystem target)
        {
	        if (source.EqualParams(target.GeographicCoordinateSystem))
	        {
				IMathTransform mathTransform = CreateCoordinateOperation(target.Projection, 
                    target.GeographicCoordinateSystem.HorizontalDatum.Ellipsoid, target.LinearUnit);
		        return new CoordinateTransformation(source, target, TransformType.Transformation, mathTransform,
			        String.Empty, String.Empty, -1, String.Empty, String.Empty);
	        }
	        else
	        {
	        	// Geographic coordinatesystems differ - Create concatenated transform
		        ConcatenatedTransform ct = new ConcatenatedTransform();
		        CoordinateTransformationFactory ctFac = new CoordinateTransformationFactory();
		        ct.CoordinateTransformationList.Add(ctFac.CreateFromCoordinateSystems(source,target.GeographicCoordinateSystem));
		        ct.CoordinateTransformationList.Add(ctFac.CreateFromCoordinateSystems(target.GeographicCoordinateSystem, target));
		        return new CoordinateTransformation(source,
			        target, TransformType.Transformation, ct,
			        String.Empty, String.Empty, -1, String.Empty, String.Empty);
	        }
        }

        private static ICoordinateTransformation Proj2Geog(IProjectedCoordinateSystem source, IGeographicCoordinateSystem target)
        {
            if (source.GeographicCoordinateSystem.EqualParams(target))
            {
                IMathTransform mathTransform = CreateCoordinateOperation(source.Projection, source.GeographicCoordinateSystem.HorizontalDatum.Ellipsoid, source.LinearUnit).Inverse();
                return new CoordinateTransformation(source, target, TransformType.Transformation, mathTransform,
                    String.Empty, String.Empty, -1, String.Empty, String.Empty);
            }
            else
            {	// Geographic coordinatesystems differ - Create concatenated transform
                ConcatenatedTransform ct = new ConcatenatedTransform();
                CoordinateTransformationFactory ctFac = new CoordinateTransformationFactory();
                ct.CoordinateTransformationList.Add(ctFac.CreateFromCoordinateSystems(source, source.GeographicCoordinateSystem));
                ct.CoordinateTransformationList.Add(ctFac.CreateFromCoordinateSystems(source.GeographicCoordinateSystem, target));
                return new CoordinateTransformation(source,
                    target, TransformType.Transformation, ct,
                    String.Empty, String.Empty, -1, String.Empty, String.Empty);
            }
        }
		
		/// <summary>
		/// Geographic to geographic transformation
		/// </summary>
		/// <remarks>Adds a datum shift if nessesary</remarks>
		/// <param name="source"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		private static ICoordinateTransformation CreateGeog2Geog(IGeographicCoordinateSystem source, IGeographicCoordinateSystem target)
		{
			if (source.HorizontalDatum.EqualParams(target.HorizontalDatum))
			{
				//No datum shift needed
				return new CoordinateTransformation(source,
					target, TransformType.Conversion, new GeographicTransform(source, target),
					String.Empty, String.Empty, -1, String.Empty, String.Empty);
			}
			else
			{
				//Create datum shift
				//Convert to geocentric, perform shift and return to geographic
				CoordinateTransformationFactory ctFac = new CoordinateTransformationFactory();
				CoordinateSystemFactory cFac = new CoordinateSystemFactory();
				IGeocentricCoordinateSystem sourceCentric = cFac.CreateGeocentricCoordinateSystem(source.HorizontalDatum.Name + " Geocentric",
					source.HorizontalDatum, LinearUnit.Metre, source.PrimeMeridian);
				IGeocentricCoordinateSystem targetCentric = cFac.CreateGeocentricCoordinateSystem(target.HorizontalDatum.Name + " Geocentric", 
					target.HorizontalDatum, LinearUnit.Metre, source.PrimeMeridian);
				var ct = new ConcatenatedTransform();
				AddIfNotNull(ct, ctFac.CreateFromCoordinateSystems(source, sourceCentric));
                AddIfNotNull(ct, ctFac.CreateFromCoordinateSystems(sourceCentric, targetCentric));
                AddIfNotNull(ct, ctFac.CreateFromCoordinateSystems(targetCentric, target));
				
                
                return new CoordinateTransformation(source,
					target, TransformType.Transformation, ct,
					String.Empty, String.Empty, -1, String.Empty, String.Empty);
			}
		}

        private static void AddIfNotNull(ConcatenatedTransform concatTrans, ICoordinateTransformation trans)
        {
            if (trans != null)
                concatTrans.CoordinateTransformationList.Add(trans);
        }
		/// <summary>
		/// Geocentric to Geocentric transformation
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		private static ICoordinateTransformation CreateGeoc2Geoc(IGeocentricCoordinateSystem source, IGeocentricCoordinateSystem target)
		{
			var ct = new ConcatenatedTransform();

			//Does source has a datum different from WGS84 and is there a shift specified?
			if (source.HorizontalDatum.Wgs84Parameters != null && !source.HorizontalDatum.Wgs84Parameters.HasZeroValuesOnly)
				ct.CoordinateTransformationList.Add(
					new CoordinateTransformation(
					((target.HorizontalDatum.Wgs84Parameters == null || target.HorizontalDatum.Wgs84Parameters.HasZeroValuesOnly) ? target : GeocentricCoordinateSystem.WGS84),
					source, TransformType.Transformation,
						new DatumTransform(source.HorizontalDatum.Wgs84Parameters)
						, "", "", -1, "", ""));

			//Does target has a datum different from WGS84 and is there a shift specified?
			if (target.HorizontalDatum.Wgs84Parameters != null && !target.HorizontalDatum.Wgs84Parameters.HasZeroValuesOnly)
				ct.CoordinateTransformationList.Add(
					new CoordinateTransformation(
					((source.HorizontalDatum.Wgs84Parameters == null || source.HorizontalDatum.Wgs84Parameters.HasZeroValuesOnly) ? source : GeocentricCoordinateSystem.WGS84),
					target,
					TransformType.Transformation,
						new DatumTransform(target.HorizontalDatum.Wgs84Parameters).Inverse()
						, "", "", -1, "", ""));

            //If we don't have a transformation in this list, return null
		    if (ct.CoordinateTransformationList.Count == 0)
		        return null;
            //If we only have one shift, lets just return the datumshift from/to wgs84
            if (ct.CoordinateTransformationList.Count == 1)
				return new CoordinateTransformation(source, target, TransformType.ConversionAndTransformation, ct.CoordinateTransformationList[0].MathTransform, "", "", -1, "", "");
		    
            return new CoordinateTransformation(source, target, TransformType.ConversionAndTransformation, ct, "", "", -1, "", "");
		}

        /// <summary>
        /// Creates transformation from fitted coordinate system to the target one
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private static ICoordinateTransformation Fitt2Any (IFittedCoordinateSystem source, ICoordinateSystem target)
        {
            //transform from fitted to base system of fitted (which is equal to target)
            IMathTransform mt = CreateFittedTransform (source);

            //case when target system is equal to base system of the fitted
            if (source.BaseCoordinateSystem.EqualParams (target))
            {
                //Transform form base system of fitted to target coordinate system
                return CreateTransform (source, target, TransformType.Transformation, mt);
            }

            //Transform form base system of fitted to target coordinate system
            ConcatenatedTransform ct = new ConcatenatedTransform ();
            ct.CoordinateTransformationList.Add (CreateTransform (source, source.BaseCoordinateSystem, TransformType.Transformation, mt));

            //Transform form base system of fitted to target coordinate system
            CoordinateTransformationFactory ctFac = new CoordinateTransformationFactory ();
            ct.CoordinateTransformationList.Add (ctFac.CreateFromCoordinateSystems (source.BaseCoordinateSystem, target));

            return CreateTransform (source, target, TransformType.Transformation, ct);
        }

        /// <summary>
        /// Creates transformation from source coordinate system to specified target system which is the fitted one
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private static ICoordinateTransformation Any2Fitt (ICoordinateSystem source, IFittedCoordinateSystem target)
        {
            //Transform form base system of fitted to target coordinate system - use invered math transform
            IMathTransform invMt = CreateFittedTransform (target).Inverse ();

            //case when source system is equal to base system of the fitted
            if (target.BaseCoordinateSystem.EqualParams (source))
            {
                //Transform form base system of fitted to target coordinate system
                return CreateTransform (source, target, TransformType.Transformation, invMt);
            }

            ConcatenatedTransform ct = new ConcatenatedTransform ();
            //First transform from source to base system of fitted
            CoordinateTransformationFactory ctFac = new CoordinateTransformationFactory ();
            ct.CoordinateTransformationList.Add (ctFac.CreateFromCoordinateSystems (source, target.BaseCoordinateSystem));

            //Transform form base system of fitted to target coordinate system - use invered math transform
            ct.CoordinateTransformationList.Add (CreateTransform (target.BaseCoordinateSystem, target, TransformType.Transformation, invMt));

            return CreateTransform (source, target, TransformType.Transformation, ct);
        }

        private static IMathTransform CreateFittedTransform (IFittedCoordinateSystem fittedSystem)
        {
            //create transform From fitted to base and inverts it
            if (fittedSystem is FittedCoordinateSystem)
            {
                return ((FittedCoordinateSystem)fittedSystem).ToBaseTransform;
            }

            //MathTransformFactory mtFac = new MathTransformFactory ();
            ////create transform From fitted to base and inverts it
            //return mtFac.CreateFromWKT (fittedSystem.ToBase ());

            throw new NotImplementedException ();
        }

        /// <summary>
        /// Creates an instance of CoordinateTransformation as an anonymous transformation without neither autohority nor code defined.
        /// </summary>
        /// <param name="sourceCS">Source coordinate system</param>
        /// <param name="targetCS">Target coordinate system</param>
        /// <param name="transformType">Transformation type</param>
        /// <param name="mathTransform">Math transform</param>
        private static CoordinateTransformation CreateTransform (ICoordinateSystem sourceCS, ICoordinateSystem targetCS, TransformType transformType, IMathTransform mathTransform)
        {
            return new CoordinateTransformation (sourceCS, targetCS, transformType, mathTransform, string.Empty, string.Empty, -1, string.Empty, string.Empty);
        }
		#endregion

		private static IMathTransform CreateCoordinateOperation(IGeocentricCoordinateSystem geo)
		{
			var parameterList = new List<ProjectionParameter>(2);

		    var ellipsoid = geo.HorizontalDatum.Ellipsoid;
            //var toMeter = ellipsoid.AxisUnit.MetersPerUnit;
            if (parameterList.Find((p) => p.Name.ToLowerInvariant().Replace(' ', '_').Equals("semi_major")) == null)
                parameterList.Add(new ProjectionParameter("semi_major", /*toMeter * */ellipsoid.SemiMajorAxis));
            if (parameterList.Find((p) => p.Name.ToLowerInvariant().Replace(' ', '_').Equals("semi_minor")) == null)
                parameterList.Add(new ProjectionParameter("semi_minor", /*toMeter * */ellipsoid.SemiMinorAxis));

            return new GeocentricTransform(parameterList);
		}
		private static IMathTransform CreateCoordinateOperation(IProjection projection, IEllipsoid ellipsoid, ILinearUnit unit)
		{
			var parameterList = new List<ProjectionParameter>(projection.NumParameters);
			for (var i = 0; i < projection.NumParameters; i++)
				parameterList.Add(projection.GetParameter(i));

		    //var toMeter = 1d/ellipsoid.AxisUnit.MetersPerUnit;
            if (parameterList.Find((p) => p.Name.ToLowerInvariant().Replace(' ', '_').Equals("semi_major")) == null)
			    parameterList.Add(new ProjectionParameter("semi_major", /*toMeter * */ellipsoid.SemiMajorAxis));
            if (parameterList.Find((p) => p.Name.ToLowerInvariant().Replace(' ', '_').Equals("semi_minor")) == null)
                parameterList.Add(new ProjectionParameter("semi_minor", /*toMeter * */ellipsoid.SemiMinorAxis));
            if (parameterList.Find((p) => p.Name.ToLowerInvariant().Replace(' ', '_').Equals("unit")) == null)
                parameterList.Add(new ProjectionParameter("unit", unit.MetersPerUnit));

            var operation = ProjectionsRegistry.CreateProjection(projection.ClassName, parameterList);
		    /*
            var mpOperation = operation as MapProjection;
            if (mpOperation != null && projection.AuthorityCode !=-1)
            {
                mpOperation.Authority = projection.Authority;
                mpOperation.AuthorityCode = projection.AuthorityCode;
            }
             */

		    return operation;
		    /*
            switch (projection.ClassName.ToLower(CultureInfo.InvariantCulture).Replace(' ', '_'))
			{
				case "mercator":
				case "mercator_1sp":
				case "mercator_2sp":
					//1SP
					transform = new Mercator(parameterList);
					break;
				case "transverse_mercator":
					transform = new TransverseMercator(parameterList);
					break;
				case "albers":
				case "albers_conic_equal_area":
					transform = new AlbersProjection(parameterList);
					break;
				case "krovak":
					transform = new KrovakProjection(parameterList);
					break;
                case "polyconic":
                    transform = new PolyconicProjection(parameterList);
                    break;
                case "lambert_conformal_conic":
				case "lambert_conformal_conic_2sp":
				case "lambert_conic_conformal_(2sp)":
					transform = new LambertConformalConic2SP(parameterList);
					break;
				default:
					throw new NotSupportedException(String.Format("Projection {0} is not supported.", projection.ClassName));
			}
			return transform;
             */
		}
	}
}

