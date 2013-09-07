//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/Transformation/WGSPixel.cs $
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
// File description   : Klasse Transformation, Funktionen: WGSPixel
//===================================================================================================



using System;
using System.Text;
using GeoUtility.GeoSystem;
using GeoUtility.GeoSystem.Helper;


namespace GeoUtility
{
    internal partial class Transformation
    {
        /// <summary><para>Die interne Funktion berechnet den X/Y Bildpunkt auf einem Satellitenbild (Tile), 
        /// der den geographischen Koordinaten im übergebenen <see cref="GeoUtility.GeoSystem.Geographic"/>-Objekt entspricht.
        /// <para>Die Funktion ist nur für interne Berechnungen bestimmt.</para></para></summary>
        /// 
        /// <param name="tail">Ein <see cref="GeoUtility.GeoSystem.MapService.Info.MapServiceInternalMapTile"/>-Objekt.</param>
        /// <param name="size">Größe des Satellitenbildes (i.d.R. 256 Pixel)</param>
        /// <returns>Bildpunkt auf dem Satellitenbild als <see cref="GeoUtility.GeoSystem.Helper.GeoPoint"/>-Objekt, der den Koordinaten entspricht.</returns>
        internal GeoPoint WGSPixel(MapService.Info.MapServiceInternalMapTile tail, int size)
        {
            string key = tail.Key;                                  // Restkey, der nur die Pixelverschiebung im Bild enthält
            double left = 0, top = 0;
            double dsize = Convert.ToDouble(size);
            for (int i = 0; i < key.Length; i++)
            {
                double shift = dsize / (Math.Pow(2.0, i + 1));
                if ((key[i] == 's') || (key[i] == 't')) top += shift;
                if ((key[i] == 'r') || (key[i] == 's')) left += shift;
            }
            top = Math.Round(top, 0);
            left = Math.Round(left, 0);
            return new GeoPoint((int)left, (int)top);
        }


        /// <summary><para>Die interne Funktion berechnet den X/Y Bildpunkt auf einem Satellitenbild (Tile), 
        /// der den geographischen Koordinaten im übergebenen <see cref="GeoUtility.GeoSystem.Geographic"/>-Objekt entspricht.
        /// <para>Die Funktion ist nur für interne Berechnungen bestimmt.</para></para></summary>
        /// 
        /// <param name="tail">Ein <see cref="GeoUtility.GeoSystem.MapService.Info.MapServiceInternalMapTile"/>-Objekt.</param>
        /// <returns>Bildpunkt auf dem Satellitenbild als <see cref="GeoUtility.GeoSystem.Helper.GeoPoint"/>-Objekt, der den Koordinaten entspricht.</returns>
        internal GeoPoint WGSPixel(MapService.Info.MapServiceInternalMapTile tail)
        {
            return WGSPixel(tail, 256);
        }

    }

}
