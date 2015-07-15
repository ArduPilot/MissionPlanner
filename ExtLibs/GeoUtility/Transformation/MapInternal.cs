//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/Transformation/MapInternal.cs $
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
// File description   : Klasse Transformation, Alle MapService Transformationen 
//===================================================================================================



using System;
using GeoUtility.GeoSystem;


namespace GeoUtility
{
    internal partial class Transformation
    {

        #region ===================== GoogleMaps =====================

        /// <summary><para>Die Funktion transformiert den internen generischen Schlüssel den Google Maps Schlüssel.
        /// <para>Die Funktion ist nur für interne Berechnungen bestimmt.</para></para></summary>
        /// 
        /// <param name="tile">Ein <see cref="MapService.Info.MapServiceInternalMapTile"/>-Objekt mit dem internen Schlüssel.</param>
        /// <returns>Ein <see cref="MapService.Info.MapServiceGoogleMapsTile"/>-Objekt mit den Zugriffsdaten auf das Satellitenbild.</returns>
        internal MapService.Info.MapServiceGoogleMapsTile InternalToGoogle(MapService.Info.MapServiceInternalMapTile tile)
        {
            string key = tile.Key;
            int zoom = key.Length - 1;
            int iExp = key.Length - 1;
            int x = 0; int y = 0;

            for (int i = 1; i < key.Length; i++)
            {
                iExp -= 1;
                string s = key.Substring(i, 1);
                if (s == "r") { x += (int)Math.Pow(2, iExp); };
                if (s == "t") { y += (int)Math.Pow(2, iExp); };
                if (s == "s") { x += (int)Math.Pow(2, iExp); y += (int)Math.Pow(2, iExp); };
            }

            return new MapService.Info.MapServiceGoogleMapsTile(x, y, zoom);
        
        }


        /// <summary><para>Die Funktion transformiert den Google Maps Schlüssel in die intern benutzte Repräsentation.
        /// <para>Die Funktion ist nur für interne Berechnungen bestimmt.</para></para></summary>
        /// 
        /// <param name="tile">Ein <see cref="MapService.Info.MapServiceGoogleMapsTile"/>-Objekt mit den Zugriffsdaten auf das Satellitenbild.</param>
        /// <returns>Ein <see cref="MapService.Info.MapServiceInternalMapTile"/>-Objekt mit dem internen Schlüssel.</returns>
        internal MapService.Info.MapServiceInternalMapTile GoogleToInternal(MapService.Info.MapServiceGoogleMapsTile tile)
        {
            int x = tile.X;
            int y = tile.Y;
            int zoom = tile.Zoom;
            string key = "";
            int wert = 0;
            int exp = zoom;
            int[] keyArray = new int[zoom + 1];

            // 1. Buchstabe konstant
            keyArray[0] = 2;

            // Werte des Feldes iKey[]: q=0, r=1, t=2, s=3
            for (int i = 1; i < keyArray.Length; i++)           
            {
                exp -= 1;

                // Test auf Rechtswert
                wert = (x - (int)Math.Pow(2, exp));   
                if (wert >= 0) { keyArray[i] = 1; x = wert; }

                // Test auf Hochwert
                wert = (y - (int)Math.Pow(2, exp));       
                if (wert >= 0) { keyArray[i] += 2; y = wert; }
            }
            for (int i = 0; i < keyArray.Length; i++)
            {
                switch (keyArray[i])
                {
                    case 0: key += "q"; break;
                    case 1: key += "r"; break;
                    case 2: key += "t"; break;
                    case 3: key += "s"; break;
                }
            }

            return new MapService.Info.MapServiceInternalMapTile(key);
 
        }

        #endregion ===================== GoogleMaps =====================




        #region ===================== VirtualEarth =====================

        /// <summary><para>Die Funktion konvertiert den internen generischen Schlüssel in das Virtual Earth Format. 
        /// <para>Die Funktion ist nur für interne Berechnungen bestimmt.</para></para></summary>
        /// 
        /// <param name="tile">Ein <see cref="MapService.Info.MapServiceInternalMapTile"/>-Objekt mit dem internen Schlüssel.</param>
        /// <returns>Ein <see cref="MapService.Info.MapServiceVirtualEarthMapsTile"/>-Objekt mit den Zugriffsdaten auf das Satellitenbild.</returns>
        internal MapService.Info.MapServiceVirtualEarthMapsTile InternalToVirtualEarth(MapService.Info.MapServiceInternalMapTile tile)
        {
            string key = tile.Key;
            string earth = "a";
            for (int i = 1; i < key.Length; i++)
            {
                switch (key.Substring(i, 1))
                {
                    case "q": earth += "0"; break;
                    case "r": earth += "1"; break;
                    case "t": earth += "2"; break;
                    case "s": earth += "3"; break;
                }
            }

            return new MapService.Info.MapServiceVirtualEarthMapsTile(earth);
        }


        /// <summary><para>Die Funktion konvertiert das Virtual Earth Format in den internen Schlüssel.
        /// <para>Die Funktion ist nur für interne Berechnungen bestimmt.</para></para></summary>
        /// 
        /// <param name="tile">Ein <see cref="MapService.Info.MapServiceVirtualEarthMapsTile"/>-Objekt mit den Zugriffsdaten auf das Satellitenbild.</param>
        /// <returns>Ein <see cref="MapService.Info.MapServiceInternalMapTile"/>-Objekt mit dem internen Schlüssel.</returns>
        internal MapService.Info.MapServiceInternalMapTile VirtualEarthToInternal(MapService.Info.MapServiceVirtualEarthMapsTile tile)
        {
            string earth = tile.Key;
            string key = "t";                                   // 1. Buchstabe immer "t"

            for (int i = 1; i < earth.Length; i++)
            {
                switch (earth.Substring(i, 1))
                {
                    case "0": key += "q"; break;
                    case "1": key += "r"; break;
                    case "2": key += "t"; break;
                    case "3": key += "s"; break;
                }
            }

            return new MapService.Info.MapServiceInternalMapTile(key);
        }

        #endregion ===================== VirtualEarth =====================




        #region ===================== YahooMaps =====================

        /// <summary><para>Die Funktion konvertiert den internen Schlüssel in das Yahoo Maps Format.
        /// <para>Die Funktion ist nur für interne Berechnungen bestimmt.</para></para></summary>
        /// 
        /// <param name="tile">Ein <see cref="MapService.Info.MapServiceInternalMapTile"/>-Objekt mit dem internen Schlüssel.</param>
        /// <returns>Ein <see cref="MapService.Info.MapServiceYahooMapsTile"/>-Objekt mit den Zugriffsdaten auf das Satellitenbild.</returns>
        internal MapService.Info.MapServiceYahooMapsTile InternalToYahoo(MapService.Info.MapServiceInternalMapTile tile)
        {
            string key = tile.Key;
            int zoom = key.Length;
            int iExp = key.Length - 1;
            int x = 0; int y = 0;
            y = y - (int)Math.Pow(2, iExp - 1);
            for (int i = 1; i < key.Length; i++)
            {
                iExp -= 1;
                string s = key.Substring(i, 1);
                if (s == "q") { y += (int)Math.Pow(2, iExp); };
                if (s == "r") { x += (int)Math.Pow(2, iExp); y += (int)Math.Pow(2, iExp); };
                if (s == "s") { x += (int)Math.Pow(2, iExp); };
            }

            return new MapService.Info.MapServiceYahooMapsTile(x, y, zoom);
        }


        /// <summary><para>Die Funktion konvertiert das Yahoo Maps Format in die interne Repräsentation eines Satellitenbildes. 
        /// <para>Die Funktion ist nur für interne Berechnungen bestimmt.</para></para></summary>
        /// 
        /// <param name="tile">Ein <see cref="MapService.Info.MapServiceYahooMapsTile"/>-Objekt mit den Zugriffsdaten auf das Satellitenbild.</param>
        /// <returns>Ein <see cref="MapService.Info.MapServiceInternalMapTile"/>-Objekt mit den Zugriffsdaten auf das Satellitenbild.</returns>
        internal MapService.Info.MapServiceInternalMapTile YahooToInternal(MapService.Info.MapServiceYahooMapsTile tile)
        {
            int x = tile.X;
            int y = tile.Y;
            int zoom = tile.Zoom;
            string key = "";
            int wert = 0;
            int exp = zoom - 1;
            int[] keyArray = new int[zoom];
            
            // 1. Buchstabe immer gleich
            keyArray[0] = 2;
            
            // y normalisieren nur bei Yahoo                      
            if (y < 0) { y -= 1; };
            y = (int)Math.Pow(2, (exp - 1)) + y;

            // Werte des Feldes iKey[]: q=0, r=1, t=2, s=3
            for (int i = 1; i < keyArray.Length; i++)                       
            {
                exp -= 1;

                //Test auf Rechtswert
                wert = (x - (int)Math.Pow(2, exp));               
                if (wert >= 0) { keyArray[i] = 1; x = wert; }

                //Test auf Hochwert
                wert = (y - (int)Math.Pow(2, exp));                   
                if (wert >= 0) { y = wert; } else { keyArray[i] += 2; }
            }

            // In internes Format umwandeln.
            for (int i = 0; i < keyArray.Length; i++)
            {
                switch (keyArray[i])
                {
                    case 0: key += "q"; break;
                    case 1: key += "r"; break;
                    case 2: key += "t"; break;
                    case 3: key += "s"; break;
                }
            }

            return new MapService.Info.MapServiceInternalMapTile(key);
        
        }

        #endregion ===================== YahooMaps =====================

    }
}
