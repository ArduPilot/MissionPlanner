//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/Transformation/MapDimension.cs $
// Last changed by    : $LastChangedBy: sh $
// Revision           : $LastChangedRevision: 255 $
// Last changed date  : $LastChangedDate: 2011-04-30 11:15:00 +0200 (Sat, 30. Apr 2011) $
// Author             : $Author: sh $
// Copyright	      : Copyight (c) 2009-2011 Steffen Habermehl
// Contact            : geoutility@freenet.de
// License            : GNU GENERAL PUBLIC LICENSE Ver. 3, GPLv3
//                      This program is free software; you can redistribute it and/or modify it under the terms 
//                      of the GNU General Public License as published by the Free Software Foundation; 
//                      either version 3 of the License, or (at your option) any later version. 
//                      This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
//                      without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
//                      See the GNU General Public License for more details. You should have received a copy of the 
//                      GNU General Public License along with this program; if not, see <http://www.gnu.org/licenses/>. 
//
// File description   : Klasse Transformation, Funktion: MapDimension 
//===================================================================================================


using System;
using GeoUtility.GeoSystem;
using GeoUtility.GeoSystem.Helper;
using GeoUtility.ErrorProvider;


namespace GeoUtility
{
    internal partial class Transformation
    {

        /// <summary><para>Die Funktion berechnet die linke untere Ecke und die Breite und Höhe des 
        /// aktuellen Satellitenbilds (Tiles) aus dem übergebenen internen Schlüssel.
        /// <para>Die Funktion ist nur für interne Berechnungen bestimmt.</para></para></summary>
        /// 
        /// <param name="tile">Ein <see cref="MapService.Info.MapServiceInternalMapTile"/>-Objekt mit dem internen generischen Schlüssel.</param>
        /// <returns>Koordinaten (linke untere Ecke), Größe und Mittelpunkt des Luftbilds.</returns>
        internal GeoRect MapDimension(MapService.Info.MapServiceInternalMapTile tile)
        {
            string key = tile.Key;
            if ((key == null) || (key.Length == 0) || (key.Substring(0, 1) != "t"))
            {
                throw new ErrorProvider.GeoException(new ErrorProvider.ErrorMessage("ERROR_MAPDIMENSION"));
            }

            double geoLaenge = -180;    // geographische Länge
            double gridLaenge = 360;    // Breite des jeweiligen Quadranten 
            double geoBreite = -1;      // geographische Breite
            double gridHoehe = 2;       // Höhe des jeweiligen Quadranten

            for (int i = 1; i < key.Length; i++)
            {
                gridLaenge /= 2;
                gridHoehe /= 2;
                string c = key.Substring(i, 1);
                switch (c)
                {
                    case "s":
                        geoLaenge += gridLaenge;
                        break;
                    case "r":
                        geoBreite += gridHoehe;
                        geoLaenge += gridLaenge;
                        break;
                    case "q":
                        geoBreite += gridHoehe;
                        break;
                    case "t":
                        break;
                    default:
                        throw new ErrorProvider.GeoException(new ErrorProvider.ErrorMessage("ERROR_MAPDIMENSION"));
                }
            }

            // Konvertierung nach Grad
            gridHoehe += geoBreite;
            gridHoehe = (2 * Math.Atan(Math.Exp(Math.PI * gridHoehe))) - (Math.PI / 2);
            gridHoehe *= (180 / Math.PI);

            geoBreite = (2 * Math.Atan(Math.Exp(Math.PI * geoBreite))) - (Math.PI / 2);
            geoBreite *= (180 / Math.PI);

            gridHoehe -= geoBreite;

            if (gridLaenge < 0)
            {
                geoLaenge = geoLaenge + gridLaenge;
                gridLaenge = -gridLaenge;
            }

            if (gridHoehe < 0)
            {
                geoBreite = geoBreite + gridHoehe;
                gridHoehe = -gridHoehe;
            }

            return new GeoRect(geoLaenge, geoBreite, gridLaenge, gridHoehe);
        }

    }

}
