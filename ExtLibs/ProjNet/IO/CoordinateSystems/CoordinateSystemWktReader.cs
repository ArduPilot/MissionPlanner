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

// SOURCECODE IS MODIFIED FROM ANOTHER WORK AND IS ORIGINALLY BASED ON GeoTools.NET:
/*
 *  Copyright (C) 2002 Urban Science Applications, Inc. 
 *
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 2.1 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; if not, write to the Free Software
 *  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems;

namespace ProjNet.Converters.WellKnownText
{
    /// <summary>
    /// Creates an object based on the supplied Well Known Text (WKT).
    /// </summary>
    public static class CoordinateSystemWktReader
    {
        /// <summary>
        /// Reads and parses a WKT-formatted projection string.
        /// </summary>
        /// <param name="wkt">String containing WKT.</param>
        /// <param name="encoding">The parameter is not used.</param>
        /// <returns>Object representation of the WKT.</returns>
        /// <exception cref="System.ArgumentException">If a token is not recognised.</exception>
        [Obsolete("The encoding is no longer used and will be removed in a future release.")]
        public static IInfo Parse(string wkt, Encoding encoding) => Parse(wkt);

        /// <summary>
        /// Reads and parses a WKT-formatted projection string.
        /// </summary>
        /// <param name="wkt">String containing WKT.</param>
        /// <returns>Object representation of the WKT.</returns>
        /// <exception cref="System.ArgumentException">If a token is not recognised.</exception>
        public static IInfo Parse(string wkt)
        {
            if (String.IsNullOrEmpty(wkt))
                throw new ArgumentNullException("wkt");

            using (TextReader reader = new StringReader(wkt))
            {
                WktStreamTokenizer tokenizer = new WktStreamTokenizer(reader);
                tokenizer.NextToken();
                string objectName = tokenizer.GetStringValue();
                switch (objectName)
                {
                    case "UNIT":
                        return ReadUnit(tokenizer);
                    case "SPHEROID":
                        return ReadEllipsoid(tokenizer);
                    case "DATUM":
                        return ReadHorizontalDatum(tokenizer);
                    case "PRIMEM":
                        return ReadPrimeMeridian(tokenizer);
                    case "VERT_CS":
                    case "GEOGCS":
                    case "PROJCS":
                    case "COMPD_CS":
                    case "GEOCCS":
                    case "FITTED_CS":
                    case "LOCAL_CS":
                        return ReadCoordinateSystem(wkt, tokenizer);
                    default:
                        throw new ArgumentException(String.Format("'{0}' is not recognized.", objectName));
                }
            }
        }

        /// <summary>
        /// Returns a IUnit given a piece of WKT.
        /// </summary>
        /// <param name="tokenizer">WktStreamTokenizer that has the WKT.</param>
        /// <returns>An object that implements the IUnit interface.</returns>
        private static IUnit ReadUnit(WktStreamTokenizer tokenizer)
        {
            tokenizer.ReadToken("[");
            string unitName = tokenizer.ReadDoubleQuotedWord();
            tokenizer.ReadToken(",");
            tokenizer.NextToken();
            double unitsPerUnit = tokenizer.GetNumericValue();
            string authority = String.Empty;
            long authorityCode = -1;
            tokenizer.NextToken();
            if (tokenizer.GetStringValue() == ",")
            {
                tokenizer.ReadAuthority(ref authority, ref authorityCode);
                tokenizer.ReadToken("]");
            }
            return new Unit(unitsPerUnit, unitName, authority, authorityCode, String.Empty, String.Empty, String.Empty);
        }
        /// <summary>
        /// Returns a <see cref="LinearUnit"/> given a piece of WKT.
        /// </summary>
        /// <param name="tokenizer">WktStreamTokenizer that has the WKT.</param>
        /// <returns>An object that implements the IUnit interface.</returns>
        private static ILinearUnit ReadLinearUnit(WktStreamTokenizer tokenizer)
        {
            tokenizer.ReadToken("[");
            string unitName = tokenizer.ReadDoubleQuotedWord();
            tokenizer.ReadToken(",");
            tokenizer.NextToken();
            double unitsPerUnit = tokenizer.GetNumericValue();
            string authority = String.Empty;
            long authorityCode = -1;
            tokenizer.NextToken();
            if (tokenizer.GetStringValue() == ",")
            {
                tokenizer.ReadAuthority(ref authority, ref authorityCode);
                tokenizer.ReadToken("]");
            }
            return new LinearUnit(unitsPerUnit, unitName, authority, authorityCode, String.Empty, String.Empty, String.Empty);
        }
        /// <summary>
        /// Returns a <see cref="AngularUnit"/> given a piece of WKT.
        /// </summary>
        /// <param name="tokenizer">WktStreamTokenizer that has the WKT.</param>
        /// <returns>An object that implements the IUnit interface.</returns>
        private static IAngularUnit ReadAngularUnit(WktStreamTokenizer tokenizer)
        {
            tokenizer.ReadToken("[");
            string unitName = tokenizer.ReadDoubleQuotedWord();
            tokenizer.ReadToken(",");
            tokenizer.NextToken();
            double unitsPerUnit = tokenizer.GetNumericValue();
            string authority = String.Empty;
            long authorityCode = -1;
            tokenizer.NextToken();
            if (tokenizer.GetStringValue() == ",")
            {
                tokenizer.ReadAuthority(ref authority, ref authorityCode);
                tokenizer.ReadToken("]");
            }
            return new AngularUnit(unitsPerUnit, unitName, authority, authorityCode, String.Empty, String.Empty, String.Empty);
        }

        /// <summary>
        /// Returns a <see cref="AxisInfo"/> given a piece of WKT.
        /// </summary>
        /// <param name="tokenizer">WktStreamTokenizer that has the WKT.</param>
        /// <returns>An AxisInfo object.</returns>
        private static AxisInfo ReadAxis(WktStreamTokenizer tokenizer)
        {
            if (tokenizer.GetStringValue() != "AXIS")
                tokenizer.ReadToken("AXIS");
            tokenizer.ReadToken("[");
            string axisName = tokenizer.ReadDoubleQuotedWord();
            tokenizer.ReadToken(",");
            tokenizer.NextToken();
            string unitname = tokenizer.GetStringValue();
            tokenizer.ReadToken("]");
            switch (unitname.ToUpperInvariant())
            {
                case "DOWN": return new AxisInfo(axisName, AxisOrientationEnum.Down);
                case "EAST": return new AxisInfo(axisName, AxisOrientationEnum.East);
                case "NORTH": return new AxisInfo(axisName, AxisOrientationEnum.North);
                case "OTHER": return new AxisInfo(axisName, AxisOrientationEnum.Other);
                case "SOUTH": return new AxisInfo(axisName, AxisOrientationEnum.South);
                case "UP": return new AxisInfo(axisName, AxisOrientationEnum.Up);
                case "WEST": return new AxisInfo(axisName, AxisOrientationEnum.West);
                default:
                    throw new ArgumentException("Invalid axis name '" + unitname + "' in WKT");
            }
        }

        private static ICoordinateSystem ReadCoordinateSystem(string coordinateSystem, WktStreamTokenizer tokenizer)
        {
            switch (tokenizer.GetStringValue())
            {
                case "GEOGCS":
                    return ReadGeographicCoordinateSystem(tokenizer);
                case "PROJCS":
                    return ReadProjectedCoordinateSystem(tokenizer);
                case "FITTED_CS":
                    return ReadFittedCoordinateSystem (tokenizer);
                case "COMPD_CS":
                case "VERT_CS":
                case "GEOCCS":
                case "LOCAL_CS":
                    throw new NotSupportedException(String.Format("{0} coordinate system is not supported.", coordinateSystem));
                default:
                    throw new InvalidOperationException(String.Format("{0} coordinate system is not recognized.", coordinateSystem));
            }
        }

        // Reads either 3, 6 or 7 parameter Bursa-Wolf values from TOWGS84 token
        private static Wgs84ConversionInfo ReadWGS84ConversionInfo(WktStreamTokenizer tokenizer)
        {
            //TOWGS84[0,0,0,0,0,0,0]
            tokenizer.ReadToken("[");
            Wgs84ConversionInfo info = new Wgs84ConversionInfo();
            tokenizer.NextToken();
            info.Dx = tokenizer.GetNumericValue();
            tokenizer.ReadToken(",");

            tokenizer.NextToken();
            info.Dy = tokenizer.GetNumericValue();
            tokenizer.ReadToken(",");

            tokenizer.NextToken();
            info.Dz = tokenizer.GetNumericValue();
            tokenizer.NextToken();
            if (tokenizer.GetStringValue() == ",")
            {
                tokenizer.NextToken();
                info.Ex = tokenizer.GetNumericValue();

                tokenizer.ReadToken(",");
                tokenizer.NextToken();
                info.Ey = tokenizer.GetNumericValue();

                tokenizer.ReadToken(",");
                tokenizer.NextToken();
                info.Ez = tokenizer.GetNumericValue();

                tokenizer.NextToken();
                if (tokenizer.GetStringValue() == ",")
                {
                    tokenizer.NextToken();
                    info.Ppm = tokenizer.GetNumericValue();
                }
            }
            if (tokenizer.GetStringValue() != "]")
                tokenizer.ReadToken("]");
            return info;
        }

        private static IEllipsoid ReadEllipsoid(WktStreamTokenizer tokenizer)
        {
            //SPHEROID["Airy 1830",6377563.396,299.3249646,AUTHORITY["EPSG","7001"]]
            tokenizer.ReadToken("[");
            string name = tokenizer.ReadDoubleQuotedWord();
            tokenizer.ReadToken(",");
            tokenizer.NextToken();
            double majorAxis = tokenizer.GetNumericValue();
            tokenizer.ReadToken(",");
            tokenizer.NextToken();
            double e = tokenizer.GetNumericValue();
            tokenizer.NextToken();
            string authority = String.Empty;
            long authorityCode = -1;
            if (tokenizer.GetStringValue() == ",") //Read authority
            {
                tokenizer.ReadAuthority(ref authority, ref authorityCode);
                tokenizer.ReadToken("]");
            }
            IEllipsoid ellipsoid = new Ellipsoid(majorAxis, 0.0, e, true, LinearUnit.Metre, name, authority, authorityCode, String.Empty, string.Empty, string.Empty);
            return ellipsoid;
        }

        private static IProjection ReadProjection(WktStreamTokenizer tokenizer)
        {
            if (tokenizer.GetStringValue() != "PROJECTION")
                tokenizer.ReadToken("PROJECTION");
            tokenizer.ReadToken("[");//[
            string projectionName = tokenizer.ReadDoubleQuotedWord();
            string authority = string.Empty;
            long authorityCode = -1L;

            tokenizer.NextToken(true);
            if (tokenizer.GetStringValue() == ",")
            {
                tokenizer.ReadAuthority(ref authority, ref authorityCode);
                tokenizer.ReadToken("]");
            }

            tokenizer.ReadToken(",");//,
            tokenizer.ReadToken("PARAMETER");
            List<ProjectionParameter> paramList = new List<ProjectionParameter>();
            while (tokenizer.GetStringValue() == "PARAMETER")
            {
                tokenizer.ReadToken("[");
                string paramName = tokenizer.ReadDoubleQuotedWord();
                tokenizer.ReadToken(",");
                tokenizer.NextToken();
                double paramValue = tokenizer.GetNumericValue();
                tokenizer.ReadToken("]");
                tokenizer.ReadToken(",");
                paramList.Add(new ProjectionParameter(paramName, paramValue));
                tokenizer.NextToken();
            }
            IProjection projection = new Projection(projectionName, paramList, projectionName, authority, authorityCode, String.Empty, String.Empty, string.Empty);
            return projection;
        }

        private static IProjectedCoordinateSystem ReadProjectedCoordinateSystem(WktStreamTokenizer tokenizer)
        {
            /*PROJCS[
                "OSGB 1936 / British National Grid",
                GEOGCS[
                    "OSGB 1936",
                    DATUM[...]
                    PRIMEM[...]
                    AXIS["Geodetic latitude","NORTH"]
                    AXIS["Geodetic longitude","EAST"]
                    AUTHORITY["EPSG","4277"]
                ],
                PROJECTION["Transverse Mercator"],
                PARAMETER["latitude_of_natural_origin",49],
                PARAMETER["longitude_of_natural_origin",-2],
                PARAMETER["scale_factor_at_natural_origin",0.999601272],
                PARAMETER["false_easting",400000],
                PARAMETER["false_northing",-100000],
                AXIS["Easting","EAST"],
                AXIS["Northing","NORTH"],
                AUTHORITY["EPSG","27700"]
            ]
            */
            tokenizer.ReadToken("[");
            string name = tokenizer.ReadDoubleQuotedWord();
            tokenizer.ReadToken(",");
            tokenizer.ReadToken("GEOGCS");
            IGeographicCoordinateSystem geographicCS = ReadGeographicCoordinateSystem(tokenizer);
            tokenizer.ReadToken(",");
            IProjection projection = null;
            IUnit unit = null;
            List<AxisInfo> axes = new List<AxisInfo>(2);
            string authority = String.Empty;
            long authorityCode = -1;

            TokenType ct = tokenizer.NextToken();
            while (ct != TokenType.Eol && ct != TokenType.Eof)
            {
                switch (tokenizer.GetStringValue())
                {
                    case ",":
                    case "]":
                        break;
                    case "PROJECTION":
                        projection = ReadProjection(tokenizer);
                        ct = tokenizer.GetTokenType();
                        continue;
                    //break;
                    case "UNIT":
                        unit = ReadLinearUnit(tokenizer);
                        break;
                    case "AXIS":
                        axes.Add(ReadAxis(tokenizer));
                        tokenizer.NextToken();
                        break;
                    case "AUTHORITY":
                        tokenizer.ReadAuthority(ref authority, ref authorityCode);
                        //tokenizer.ReadToken("]");
                        break;
                }
                ct = tokenizer.NextToken();
            }

            //This is default axis values if not specified.
            if (axes.Count == 0)
            {
                axes.Add(new AxisInfo("X", AxisOrientationEnum.East));
                axes.Add(new AxisInfo("Y", AxisOrientationEnum.North));
            }
            IProjectedCoordinateSystem projectedCS = new ProjectedCoordinateSystem(geographicCS.HorizontalDatum, geographicCS, unit as LinearUnit, projection, axes, name, authority, authorityCode, String.Empty, String.Empty, String.Empty);
            return projectedCS;
        }

        private static IGeographicCoordinateSystem ReadGeographicCoordinateSystem(WktStreamTokenizer tokenizer)
        {
            /*
            GEOGCS["OSGB 1936",
            DATUM["OSGB 1936",SPHEROID["Airy 1830",6377563.396,299.3249646,AUTHORITY["EPSG","7001"]],TOWGS84[0,0,0,0,0,0,0],AUTHORITY["EPSG","6277"]]
            PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]]
            AXIS["Geodetic latitude","NORTH"]
            AXIS["Geodetic longitude","EAST"]
            AUTHORITY["EPSG","4277"]
            ]
            */
            tokenizer.ReadToken("[");
            string name = tokenizer.ReadDoubleQuotedWord();
            tokenizer.ReadToken(",");
            tokenizer.ReadToken("DATUM");
            IHorizontalDatum horizontalDatum = ReadHorizontalDatum(tokenizer);
            tokenizer.ReadToken(",");
            tokenizer.ReadToken("PRIMEM");
            IPrimeMeridian primeMeridian = ReadPrimeMeridian(tokenizer);
            tokenizer.ReadToken(",");
            tokenizer.ReadToken("UNIT");
            IAngularUnit angularUnit = ReadAngularUnit(tokenizer);

            string authority = String.Empty;
            long authorityCode = -1;
            tokenizer.NextToken();
            List<AxisInfo> info = new List<AxisInfo>(2);
            if (tokenizer.GetStringValue() == ",")
            {
                tokenizer.NextToken();
                while (tokenizer.GetStringValue() == "AXIS")
                {
                    info.Add(ReadAxis(tokenizer));
                    tokenizer.NextToken();
                    if (tokenizer.GetStringValue() == ",") tokenizer.NextToken();
                }
                if (tokenizer.GetStringValue() == ",") tokenizer.NextToken();
                if (tokenizer.GetStringValue() == "AUTHORITY")
                {
                    tokenizer.ReadAuthority(ref authority, ref authorityCode);
                    tokenizer.ReadToken("]");
                }
            }
            //This is default axis values if not specified.
            if (info.Count == 0)
            {
                info.Add(new AxisInfo("Lon", AxisOrientationEnum.East));
                info.Add(new AxisInfo("Lat", AxisOrientationEnum.North));
            }
            IGeographicCoordinateSystem geographicCS = new GeographicCoordinateSystem(angularUnit, horizontalDatum,
                    primeMeridian, info, name, authority, authorityCode, String.Empty, String.Empty, String.Empty);
            return geographicCS;
        }

        private static IHorizontalDatum ReadHorizontalDatum(WktStreamTokenizer tokenizer)
        {
            //DATUM["OSGB 1936",SPHEROID["Airy 1830",6377563.396,299.3249646,AUTHORITY["EPSG","7001"]],TOWGS84[0,0,0,0,0,0,0],AUTHORITY["EPSG","6277"]]
            Wgs84ConversionInfo wgsInfo = null;
            string authority = String.Empty;
            long authorityCode = -1;

            tokenizer.ReadToken("[");
            string name = tokenizer.ReadDoubleQuotedWord();
            tokenizer.ReadToken(",");
            tokenizer.ReadToken("SPHEROID");
            IEllipsoid ellipsoid = ReadEllipsoid(tokenizer);
            tokenizer.NextToken();
            while (tokenizer.GetStringValue() == ",")
            {
                tokenizer.NextToken();
                if (tokenizer.GetStringValue() == "TOWGS84")
                {
                    wgsInfo = ReadWGS84ConversionInfo(tokenizer);
                    tokenizer.NextToken();
                }
                else if (tokenizer.GetStringValue() == "AUTHORITY")
                {
                    tokenizer.ReadAuthority(ref authority, ref authorityCode);
                    tokenizer.ReadToken("]");
                }
            }
            // make an assumption about the datum type.
            IHorizontalDatum horizontalDatum = new HorizontalDatum(ellipsoid, wgsInfo, DatumType.HD_Geocentric, name, authority, authorityCode, String.Empty, String.Empty, String.Empty);

            return horizontalDatum;
        }

        private static IPrimeMeridian ReadPrimeMeridian(WktStreamTokenizer tokenizer)
        {
            //PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]]
            tokenizer.ReadToken("[");
            string name = tokenizer.ReadDoubleQuotedWord();
            tokenizer.ReadToken(",");
            tokenizer.NextToken();
            double longitude = tokenizer.GetNumericValue();

            tokenizer.NextToken();
            string authority = String.Empty;
            long authorityCode = -1;
            if (tokenizer.GetStringValue() == ",")
            {
                tokenizer.ReadAuthority(ref authority, ref authorityCode);
                tokenizer.ReadToken("]");
            }
            // make an assumption about the Angular units - degrees.
            IPrimeMeridian primeMeridian = new PrimeMeridian(longitude, AngularUnit.Degrees, name, authority, authorityCode, String.Empty, String.Empty, String.Empty);

            return primeMeridian;
        }

        private static IFittedCoordinateSystem ReadFittedCoordinateSystem (WktStreamTokenizer tokenizer)
        {
            /*
             FITTED_CS[
                 "Local coordinate system MNAU (based on Gauss-Krueger)",
                 PARAM_MT[
                    "Affine",
                    PARAMETER["num_row",3],
                    PARAMETER["num_col",3],
                    PARAMETER["elt_0_0", 0.883485346527455],
                    PARAMETER["elt_0_1", -0.468458794848877],
                    PARAMETER["elt_0_2", 3455869.17937689],
                    PARAMETER["elt_1_0", 0.468458794848877],
                    PARAMETER["elt_1_1", 0.883485346527455],
                    PARAMETER["elt_1_2", 5478710.88035753],
                    PARAMETER["elt_2_2", 1],
                 ],
                 PROJCS["DHDN / Gauss-Kruger zone 3", GEOGCS["DHDN", DATUM["Deutsches_Hauptdreiecksnetz", SPHEROID["Bessel 1841", 6377397.155, 299.1528128, AUTHORITY["EPSG", "7004"]], TOWGS84[612.4, 77, 440.2, -0.054, 0.057, -2.797, 0.525975255930096], AUTHORITY["EPSG", "6314"]], PRIMEM["Greenwich", 0, AUTHORITY["EPSG", "8901"]], UNIT["degree", 0.0174532925199433, AUTHORITY["EPSG", "9122"]], AUTHORITY["EPSG", "4314"]], UNIT["metre", 1, AUTHORITY["EPSG", "9001"]], PROJECTION["Transverse_Mercator"], PARAMETER["latitude_of_origin", 0], PARAMETER["central_meridian", 9], PARAMETER["scale_factor", 1], PARAMETER["false_easting", 3500000], PARAMETER["false_northing", 0], AUTHORITY["EPSG", "31467"]]
                 AUTHORITY["CUSTOM","12345"]
             ]
            */
            tokenizer.ReadToken ("[");
            string name = tokenizer.ReadDoubleQuotedWord ();
            tokenizer.ReadToken (",");
            tokenizer.ReadToken ("PARAM_MT");
            IMathTransform toBaseTransform = MathTransformWktReader.ReadMathTransform (tokenizer);
            tokenizer.ReadToken (",");
            tokenizer.NextToken ();
            ICoordinateSystem baseCS = ReadCoordinateSystem (null, tokenizer);

            string authority = String.Empty;
            long authorityCode = -1;

            TokenType ct = tokenizer.NextToken ();
            while (ct != TokenType.Eol && ct != TokenType.Eof)
            {
                switch (tokenizer.GetStringValue ())
                {
                    case ",":
                    case "]":
                        break;
                    case "AUTHORITY":
                        tokenizer.ReadAuthority (ref authority, ref authorityCode);
                        //tokenizer.ReadToken("]");
                        break;
                }
                ct = tokenizer.NextToken ();
            }

            IFittedCoordinateSystem fittedCS = new FittedCoordinateSystem (baseCS, toBaseTransform, name, authority, authorityCode, string.Empty, string.Empty, string.Empty);
            return fittedCS;
        }
    }
}
