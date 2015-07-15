//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/Transformation/WGSIMAP.cs $
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
// File description   : Klasse Transformation, Funktion: WGSIMAP 
//===================================================================================================


using System;
using System.Text;
using GeoUtility.GeoSystem;


namespace GeoUtility
{
    internal partial class Transformation
    {

        /// <summary><para>Die Funktion berechnet den internen Schlüssel für ein Luftbild, welches zu den Geodaten gehört.</para></summary>
        /// <remarks><para>Um nicht für die verschiedenen MapService-Dienste jeweils eigene Funktionen anbieten zu müssen, 
        /// werden alle Berechnungen intern mit einem generischen Schlüssel durchgeführt und später gegebenenfalls 
        /// in den gewählten MapService-Dienst übersetzt. 
        /// <para>Die Funktion ist nur für interne Berechnungen bestimmt.</para>
        /// </para></remarks>
        /// 
        /// <param name="geo"><see cref="Geographic"/>-Objekt im <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84">WGS84-Datum</see></param>
        /// <param name="zoom">Zoomlevel 1-21</param>
        /// <returns>Interner Schlüssel als <see cref="MapService.Info.MapServiceInternalMapTile"/>-Objekt.</returns>
        internal MapService.Info.MapServiceInternalMapTile WGSIMAP(Geographic geo, int zoom)
        {
            double laenge = geo.Longitude;
            double breite = geo.Latitude;

            if (laenge > 180) laenge -= 360;
            laenge /= 180;

            // Breitengrad in den Bereich -1 bis +1 bringen
            breite = Math.Log(Math.Tan((Math.PI / 4) + ((0.5 * Math.PI * breite) / 180))) / Math.PI;

            double tBreite = -1;
            double tLaenge = -1;
            double laengeWidth = 2;
            double breiteHeight = 2;
            StringBuilder key = new StringBuilder("t");

            // Interner Schlüssel berechnen 
            for (int i = 0; i < zoom; i++)
            {
                laengeWidth /= 2;
                breiteHeight /= 2;
                if ((tBreite + breiteHeight) > breite)
                {
                    if ((tLaenge + laengeWidth) > laenge)
                    {
                        key.Append('t');
                    }
                    else
                    {
                        tLaenge += laengeWidth;
                        key.Append('s');
                    }
                }
                else
                {
                    tBreite += breiteHeight;
                    if ((tLaenge + laengeWidth) > laenge)
                    {
                        key.Append('q');
                    }
                    else
                    {
                        tLaenge += laengeWidth;
                        key.Append('r');
                    }
                }
            }

            return new MapService.Info.MapServiceInternalMapTile(key.ToString());
        }

    }

}
