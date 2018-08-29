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
using ProjNet.CoordinateSystems.Transformations;

namespace ProjNet.Converters.WellKnownText
{
    /// <summary>
    /// Creates an math transform based on the supplied Well Known Text (WKT).
    /// </summary>
    public static class MathTransformWktReader
    {
        /// <summary>
        /// Reads and parses a WKT-formatted projection string.
        /// </summary>
        /// <param name="wkt">String containing WKT.</param>
        /// <param name="encoding">The parameter is not used.</param>
        /// <returns>Object representation of the WKT.</returns>
        /// <exception cref="System.ArgumentException">If a token is not recognised.</exception>
        [Obsolete("The encoding is no longer used and will be removed in a future release.")]
        public static IMathTransform Parse (string wkt, Encoding encoding) => Parse(wkt);

        /// <summary>
        /// Reads and parses a WKT-formatted projection string.
        /// </summary>
        /// <param name="wkt">String containing WKT.</param>
        /// <returns>Object representation of the WKT.</returns>
        /// <exception cref="System.ArgumentException">If a token is not recognised.</exception>
        public static IMathTransform Parse (string wkt)
        {
            if (String.IsNullOrEmpty (wkt))
                throw new ArgumentNullException ("wkt");

            using (TextReader reader = new StringReader (wkt))
            {
                WktStreamTokenizer tokenizer = new WktStreamTokenizer (reader);
                tokenizer.NextToken ();
                string objectName = tokenizer.GetStringValue ();
                switch (objectName)
                {
                    case "PARAM_MT":
                        return ReadMathTransform (tokenizer);
                    default:
                        throw new ArgumentException (String.Format ("'{0}' is not recognized.", objectName));
                }
            }
        }

        /// <summary>
        /// Reads math transform from using current token from the specified tokenizer
        /// </summary>
        /// <param name="tokenizer"></param>
        /// <returns></returns>
        internal static IMathTransform ReadMathTransform (WktStreamTokenizer tokenizer)
        {
            if (tokenizer.GetStringValue () != "PARAM_MT")
                tokenizer.ReadToken ("PARAM_MT");
            tokenizer.ReadToken ("[");
            string transformName = tokenizer.ReadDoubleQuotedWord ();
            tokenizer.ReadToken (",");

            switch (transformName.ToUpperInvariant ())
            {
                case "AFFINE":
                    return ReadAffineTransform (tokenizer);
                default:
                    throw new NotSupportedException ("Transform not supported '" + transformName + "'");
            }
        }

        private static IParameterInfo ReadParameters (WktStreamTokenizer tokenizer)
        {
            List<GeoAPI.CoordinateSystems.Parameter> paramList = new List<GeoAPI.CoordinateSystems.Parameter> ();
            while (tokenizer.GetStringValue () == "PARAMETER")
            {
                tokenizer.ReadToken ("[");
                string paramName = tokenizer.ReadDoubleQuotedWord ();
                tokenizer.ReadToken (",");
                tokenizer.NextToken ();
                double paramValue = tokenizer.GetNumericValue ();
                tokenizer.ReadToken ("]");
                //test, whether next parameter is delimited by comma
                tokenizer.NextToken ();
                if (tokenizer.GetStringValue () != "]")
                    tokenizer.NextToken ();
                paramList.Add (new GeoAPI.CoordinateSystems.Parameter (paramName, paramValue));
            }
            IParameterInfo info = new ParameterInfo () { Parameters = paramList };
            return info;
        }

        private static IMathTransform ReadAffineTransform (WktStreamTokenizer tokenizer)
        {
            /*
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
                    PARAMETER["elt_2_2", 1]
                 ]
            */
            //tokenizer stands on the first PARAMETER
            if (tokenizer.GetStringValue () != "PARAMETER")
                tokenizer.ReadToken ("PARAMETER");

            IParameterInfo paramInfo = ReadParameters (tokenizer);
            //manage required parameters - row, col
            var rowParam = paramInfo.GetParameterByName ("num_row");
            var colParam = paramInfo.GetParameterByName ("num_col");

            if (rowParam == null)
            {
                throw new ArgumentNullException ("Affine transform does not contain 'num_row' parameter");
            }
            if (colParam == null)
            {
                throw new ArgumentNullException ("Affine transform does not contain 'num_col' parameter");
            }
            int rowVal = (int)rowParam.Value;
            int colVal = (int)colParam.Value;

            if (rowVal <= 0)
            {
                throw new ArgumentException ("Affine transform contains invalid value of 'num_row' parameter");
            }

            if (colVal <= 0)
            {
                throw new ArgumentException ("Affine transform contains invalid value of 'num_col' parameter");
            }

            //creates working matrix;
            double[,] matrix = new double[rowVal, colVal];

            //simply process matrix values - no elt_ROW_COL parsing
            foreach (var param in paramInfo.Parameters)
            {
                if (param == null || param.Name == null)
                {
                    continue;
                }
                switch (param.Name)
                {
                    case "num_row":
                    case "num_col":
                        break;
                    case "elt_0_0":
                        matrix[0, 0] = param.Value;
                        break;
                    case "elt_0_1":
                        matrix[0, 1] = param.Value;
                        break;
                    case "elt_0_2":
                        matrix[0, 2] = param.Value;
                        break;
                    case "elt_0_3":
                        matrix[0, 3] = param.Value;
                        break;
                    case "elt_1_0":
                        matrix[1, 0] = param.Value;
                        break;
                    case "elt_1_1":
                        matrix[1, 1] = param.Value;
                        break;
                    case "elt_1_2":
                        matrix[1, 2] = param.Value;
                        break;
                    case "elt_1_3":
                        matrix[1, 3] = param.Value;
                        break;
                    case "elt_2_0":
                        matrix[2, 0] = param.Value;
                        break;
                    case "elt_2_1":
                        matrix[2, 1] = param.Value;
                        break;
                    case "elt_2_2":
                        matrix[2, 2] = param.Value;
                        break;
                    case "elt_2_3":
                        matrix[2, 3] = param.Value;
                        break;
                    case "elt_3_0":
                        matrix[3, 0] = param.Value;
                        break;
                    case "elt_3_1":
                        matrix[3, 1] = param.Value;
                        break;
                    case "elt_3_2":
                        matrix[3, 2] = param.Value;
                        break;
                    case "elt_3_3":
                        matrix[3, 3] = param.Value;
                        break;
                    default:
                        //unknown parameter
                        break;
                }
            }

            //read rest of WKT
            if (tokenizer.GetStringValue () != "]")
                tokenizer.ReadToken ("]");

            //use "matrix" constructor to create transformation matrix
            IMathTransform affineTransform = new AffineTransform (matrix);
            return affineTransform;
        }
    }
}
