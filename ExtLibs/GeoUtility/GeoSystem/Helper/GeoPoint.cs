//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/GeoSystem/Helper/GeoPoint.cs $
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
// File description   : Definition des GeoPoint Struktur 
//===================================================================================================



using System;
namespace GeoUtility.GeoSystem.Helper
{
    /// <summary><para>Einfache Point Klasse.</para></summary>
    /// <remarks><para>Die Struktur wird von der Klasse <see cref="GeoUtility.GeoSystem.MapService.Info.TileInfo"/>
    /// und der Klasse <see cref="GeoUtility.Transformation"/> verwendet, um Bildpunkte eines 
    /// Satelliten-/Luftbildes (Tiles) in Koordinaten umzuwandeln.</para></remarks>
    public struct GeoPoint
    {
        private int _x;                                       // Speicherplatz für X
        private int _y;                                       // Speicherplatz für Y


        /// <summary><para>Bildpunkt nach rechts</para></summary>
        public int X { get { return _x; } set { _x = value; } }


        /// <summary><para>Bildpunkt nach unten</para></summary>
        public int Y { get { return _y; } set { _y = value; } }


        /// <summary><para>Ein Konstruktor mit Parametern für die Pixelangabe (x,y)</para></summary>
        /// <param name="x">X-Koordinate: Bildpunkt nach rechts</param>
        /// <param name="y">Y-Koordinate: Bildpunkt nach unten</param>
        public GeoPoint(int x, int y) { _x = x; _y = y; }


        /// <summary>Die Funktion wird aus performancegründen implementiert. Das Einfügen in eine Hashtable wird durch die die 
        /// Bereitstellung eines Hashwertes wesentlich beschleunigt.</summary>
        /// <returns>Ein Hashwert.</returns>
        public override int GetHashCode()
        {
            return this.X ^ this.Y;
        }


        /// <summary>Die Funktion wird aus performancegründen implementiert.</summary>
        /// <param name="obj">Ein GeoPoint-Objekt.</param>
        /// <returns>Das übergebene Objekt ist gleich oder nicht.</returns>
        public override bool Equals(Object obj)
        {

            if (obj == null || GetType() != obj.GetType()) return false;
            GeoPoint geopoint = (GeoPoint)obj;

            return (this.X == geopoint.X) && (this.Y == geopoint.Y);
        }


        /// <summary>Überladener Gleichheitsoperator. Die Funktion wird aus performancegründen implementiert.</summary>
        /// <param name="pointA">Ein GeoPoint-Objekt.</param>
        /// <param name="pointB">Ein weiteres GeoPoint-Objekt.</param>
        /// <returns>True, wenn beide Objekte die gleichen X/Y-Werte haben. False, wenn nicht beide Werte gleich sind.</returns>
        public static bool operator ==(GeoPoint pointA, GeoPoint pointB)
        {
            return ((pointA.X == pointB.X) && (pointA.Y == pointB.Y));
        }


        /// <summary>Überladener Ungleichheitsoperator. Die Funktion wird aus performancegründen implementiert.</summary>
        /// <param name="pointA">Ein GeoPoint-Objekt.</param>
        /// <param name="pointB">Ein weiteres GeoPoint-Objekt.</param>
        /// <returns>True, wenn beide Objekte nicht die gleichen X/Y-Werte haben. False, wenn beide Werte gleich sind.</returns>
        public static bool operator !=(GeoPoint pointA, GeoPoint pointB)
        {
            return ((pointA.X != pointB.X) || (pointA.Y != pointB.Y));
        }
    }
}
