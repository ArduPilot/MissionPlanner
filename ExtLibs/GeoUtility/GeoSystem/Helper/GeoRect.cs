//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/GeoSystem/Helper/GeoRect.cs $
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
// File description   : Definition des GeoRect Struktur 
//===================================================================================================


using System;
namespace GeoUtility.GeoSystem.Helper
{
    /// <summary><para>Einfache Rechteck Struktur für Satellitenbilder eines <see cref="GeoUtility.GeoSystem.MapService"/>-Objektes.</para></summary>
    /// <remarks><para>Die Struktur wird von der Klasse <see cref="GeoUtility.GeoSystem.MapService.Info.TileInfo"/>
    ///  und anderen Klassen implementiert, vorwiegend um die Dimension und andere Informationen 
    ///  eines Satellitenbildes festzuhalten.</para></remarks>
    public struct GeoRect
    {
        private double _lon;                                    // Speicherplatz für geographische Länge
        private double _lat;                                    // Speicherplatz für geographische Breite
        private double _width;                                  // Speicherplatz für Breite des Bildes in Grad
        private double _height;                                 // Speicherplatz für Höhe des Bildes in Grad
        private double _loncenter;                              // Speicherplatz für geogr. Länge in der Mitte des Bildes
        private double _latcenter;                              // Speicherplatz für geogr. Breite in der Mitte des Bildes

        /// <summary><para>Geographische Länge (Longitude)</para></summary>
        public double Longitude { get { return _lon; } set { _lon = value; } }


        /// <summary><para>Geographische Breite (Latitude)</para></summary>
        public double Latitude { get { return _lat; } set { _lat = value; } }


        /// <summary><para>Breite eines Satellitenbildes in Grad</para></summary>
        public double Width { get { return _width; } set { _width = value; } }


        /// <summary><para>Höhe eines Satellitenbildes in Grad</para></summary>
        public double Height { get { return _height; } set { _height = value; } }


        /// <summary><para>Geographische Länge in der Mitte eines Satellitenbildes</para></summary>
        public double LonCenter { get { return _loncenter; } set { _loncenter = value; } }


        /// <summary><para>Geographische Breite in der Mitte eines Satellitenbildes</para></summary>
        public double LatCenter { get { return _latcenter; } set { _latcenter = value; } }


        /// <summary><para>Ein Konstruktor mit Parametern für geographische Koordinaten.</para></summary>
        /// <param name="lon">Geographische Länge (Longitude)</param>
        /// <param name="lat">Geographische Breite (Latitude)</param>
        /// <param name="width">Breite eines Satellitenbildes in Grad</param>
        /// <param name="height">Höhe eines Satellitenbildes in Grad</param>
        public GeoRect(double lon, double lat, double width, double height)
        {
            _lon = lon;
            _lat = lat;
            _width = width;
            _height = height;
            _loncenter = lon + (width / 2);
            _latcenter = lat + (height / 2);
        }


        /// <summary>Die Funktion wird aus performancegründen implementiert. Das Einfügen in eine Hashtabelle wird durch die die 
        /// Bereitstellung eines Hashwertes wesentlich beschleunigt.</summary>
        /// <returns>Ein Hashwert.</returns>
        public override int GetHashCode()
        {
            return (int)this.Longitude ^ (int)this.Latitude ^ (int)this.Width ^ (int)this.Height;
        }


        /// <summary>Die Funktion wird aus performancegründen implementiert.</summary>
        /// <param name="obj">Ein GeoRect-Objekt.</param>
        /// <returns>Das übergebene Objekt ist gleich oder nicht.</returns>
        public override bool Equals(Object obj)
        {

            if (obj == null || GetType() != obj.GetType()) return false;
            GeoRect georect = (GeoRect)obj;

            return (this.Longitude == georect.Longitude) && (this.Latitude == georect.Latitude) && (this.Width == georect.Width) && (this.Height == georect.Height);
        }

        /// <summary>Überladener Gleichheitsoperator. Die Funktion wird aus performancegründen implementiert.</summary>
        /// <param name="rectA">Ein GeoRect-Objekt.</param>
        /// <param name="rectB">Ein zweites GeoRect-Objekt.</param>
        /// <returns>True, wenn beide Objekte die gleichen Werte haben. False, wenn die Werte nicht gleich sind.</returns>
        public static bool operator ==(GeoRect rectA, GeoRect rectB)
        {
            return (rectA.Longitude == rectB.Longitude) && (rectA.Latitude == rectB.Latitude) && (rectA.Width == rectB.Width) && (rectA.Height == rectB.Height);
        }


        /// <summary>Überladener Ungleichheitsoperator. Die Funktion wird aus performancegründen implementiert.</summary>
        /// <param name="rectA">Ein GeoRect-Objekt.</param>
        /// <param name="rectB">Ein zweites GeoRect-Objekt.</param>
        /// <returns>True, wenn beide Objekte nicht die gleichen Werte haben. False, wenn die Werte gleich sind.</returns>
        public static bool operator !=(GeoRect rectA, GeoRect rectB)
        {
            return (rectA.Longitude != rectB.Longitude) || (rectA.Latitude != rectB.Latitude) || (rectA.Width != rectB.Width) || (rectA.Height != rectB.Height);
        }
    }

}
