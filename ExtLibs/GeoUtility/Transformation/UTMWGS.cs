//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/Transformation/UTMWGS.cs $
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
// File description   : Klasse Transformation, Funktion: UTMWGS 
//===================================================================================================



using System;
using GeoUtility.GeoSystem;


namespace GeoUtility
{
    internal partial class Transformation
    {

        /// <summary><para>Die Funktion wandelt UTM Koordinaten in geographische Koordinaten (Länge/Breite) um.
        /// <para>Die Funktion ist nur für interne Berechnungen bestimmt.</para></para></summary>
        /// <remarks><para>
        /// Hintergründe zum Problem der Koordinatentransformationen sowie entsprechende  mathematische 
        /// Formeln können den einschlägigen Fachbüchern oder dem Internet entnommen werden.<p />
        /// Quellen: 
        /// Bundesamt für Kartographie und Geodäsie<br />
        /// <a href="http://www.bkg.bund.de" target="_blank">http://www.bkg.bund.de</a><br />
        /// <a href="http://crs.bkg.bund.de" target="_blank">http://crs.bkg.bund.de</a><br />
        /// </para></remarks>
        /// 
        /// <param name="utm">Ein <see cref="GeoUtility.GeoSystem.UTM"/>-Objekt.</param>
        /// <returns>Ein <see cref="GeoUtility.GeoSystem.Geographic"/>-Objekt im <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84">WGS84-Datum</see>.</returns>
        internal Geographic UTMWGS(UTM utm)
        {

            int zone = utm.Zone;
            string band = utm.Band;
            double east = utm.East;
            double north = utm.North;

            // Koeffizienten zur Berechnung der geographischen Breite aus gegebener Meridianbogenlänge
            double koeff0 = WGS84_POL * (Math.PI / 180) * (1 - 3 * WGS84_EXZENT2 / 4 + 45 * WGS84_EXZENT4 / 64 - 175 * WGS84_EXZENT6 / 256 + 11025 * WGS84_EXZENT8 / 16384);
            double koeff2 = (180 / Math.PI) * (3 * WGS84_EXZENT2 / 8 - 3 * WGS84_EXZENT4 / 16 + 213 * WGS84_EXZENT6 / 2048 - 255 * WGS84_EXZENT8 / 4096);
            double koeff4 = (180 / Math.PI) * (21 * WGS84_EXZENT4 / 256 - 21 * WGS84_EXZENT6 / 256 + 533 * WGS84_EXZENT8 / 8192);
            double koeff6 = (180 / Math.PI) * (151 * WGS84_EXZENT6 / 6144 - 453 * WGS84_EXZENT8 / 12288);

            // Nord-/Süd Halbkugel
            char b = band.ToCharArray()[0];
            if (b < 'N' && band != "" && north > 0)
            {
                north = north - 10E+06;
            }

            // Breite (Rad)
            double sig = (north / UTM_FAKTOR) / koeff0;
            double sigRad = sig * Math.PI / 180;
            double fbreite = sig + koeff2 * Math.Sin(2 * sigRad) + koeff4 * Math.Sin(4 * sigRad) + koeff6 * Math.Sin(6 * sigRad);
            double breiteRad = fbreite * Math.PI / 180;

            double tangens1 = Math.Tan(breiteRad);
            double tangens2 = tangens1 * tangens1;
            double tangens4 = tangens2 * tangens2;
            double cosinus1 = Math.Cos(breiteRad);
            double cosinus2 = cosinus1 * cosinus1;

            double eta = WGS84_EXZENT2 * cosinus2;

            // Querkrümmung
            double qkhm1 = WGS84_POL / Math.Sqrt(1 + eta);
            double qkhm2 = Math.Pow(qkhm1, 2);
            double qkhm3 = Math.Pow(qkhm1, 3);
            double qkhm4 = Math.Pow(qkhm1, 4);
            double qkhm5 = Math.Pow(qkhm1, 5);
            double qkhm6 = Math.Pow(qkhm1, 6);

            // Differenz zum Bezugsmeridian
            double merid = (zone - 30) * 6 - 3;
            double dlaenge1 = (east - UTM_FALSE_EASTING) / UTM_FAKTOR;
            double dlaenge2 = Math.Pow(dlaenge1, 2);
            double dlaenge3 = Math.Pow(dlaenge1, 3);
            double dlaenge4 = Math.Pow(dlaenge1, 4);
            double dlaenge5 = Math.Pow(dlaenge1, 5);
            double dlaenge6 = Math.Pow(dlaenge1, 6);

            // Faktor für Berechnung Breite
            double bfakt2 = -tangens1 * (1 + eta) / (2 * qkhm2);
            double bfakt4 = tangens1 * (5 + 3 * tangens2 + 6 * eta * (1 - tangens2)) / (24 * qkhm4);
            double bfakt6 = -tangens1 * (61 + 90 * tangens2 + 45 * tangens4) / (720 * qkhm6);

            // Faktor für Berechnung Länge
            double lfakt1 = 1 / (qkhm1 * cosinus1);
            double lfakt3 = -(1 + 2 * tangens2 + eta) / (6 * qkhm3 * cosinus1);
            double lfakt5 = (5 + 28 * tangens2 + 24 * tangens4) / (120 * qkhm5 * cosinus1);

            // Geographische Breite Länge WGS84
            double breite = fbreite + (180 / Math.PI) * (bfakt2 * dlaenge2 + bfakt4 * dlaenge4 + bfakt6 * dlaenge6);
            double laenge = merid + (180 / Math.PI) * (lfakt1 * dlaenge1 + lfakt3 * dlaenge3 + lfakt5 * dlaenge5);

            return new Geographic(laenge, breite);
        }

    }

}
