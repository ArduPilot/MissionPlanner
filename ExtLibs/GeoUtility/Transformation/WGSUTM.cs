//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/Transformation/WGSUTM.cs $
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
// File description   : Klasse Transformation, Funktion: WGSUTM 
//===================================================================================================



using System;
using GeoUtility.GeoSystem;
using GeoUtility.GeoSystem.Helper;


namespace GeoUtility
{
    internal partial class Transformation
    {

        /// <summary><para>Die Funktion wandelt die geographischen Koordinaten eines <see cref="GeoSystem.Geographic"/>-Objekts 
        /// in die Koordinaten eines <see cref="GeoSystem.UTM"/>-Objekts um.
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
        /// <param name="geo"><see cref="GeoSystem.Geographic"/>-Objekt im <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84">WGS84-Datum</see>.</param>
        /// <returns>Ein <see cref="GeoSystem.UTM"/>-Objekt.</returns>
        internal UTM WGSUTM(Geographic geo)
        {

            // falls geo Objekt nicht im WGS84-Datum
            if (geo.Datum != GeoDatum.WGS84) geo = this.PODWGS(geo);

            double laenge = geo.Longitude;
            double breite = geo.Latitude;

            if (laenge <= MIN_LAENGE || laenge > MAX_LAENGE || breite < MIN_BREITE || breite > MAX_BREITE)
            {
                throw new ErrorProvider.GeoException(new ErrorProvider.ErrorMessage("ERROR_GEO2UTM"));
            }

            // Koeffizienten für Länge Meridianbogen
            double koeff0 = WGS84_POL * (Math.PI / 180) * (1 - 3 * WGS84_EXZENT2 / 4 + 45 * WGS84_EXZENT4 / 64 - 175 * WGS84_EXZENT6 / 256 + 11025 * WGS84_EXZENT8 / 16384);
            double koeff2 = WGS84_POL * (-3 * WGS84_EXZENT2 / 8 + 15 * WGS84_EXZENT4 / 32 - 525 * WGS84_EXZENT6 / 1024 + 2205 * WGS84_EXZENT8 / 4096);
            double koeff4 = WGS84_POL * (15 * WGS84_EXZENT4 / 256 - 105 * WGS84_EXZENT6 / 1024 + 2205 * WGS84_EXZENT8 / 16384);
            double koeff6 = WGS84_POL * (-35 * WGS84_EXZENT6 / 3072 + 315 * WGS84_EXZENT8 / 12288);

            // Berechnung Zone, Band
            int zone = (int)((laenge + 180) / 6) + 1;
            // Handling UTM exception 
            if ((breite >= 56d) && (breite < 64d) && (laenge >= 3d) && (laenge < 12d)) 
            {
                // South-Norway 31V-32V (32V extends to the west from 3 to 12 degrees, with degree 9 as meridian)
                zone = 32;

            }
            else if (breite >= 72d) 
            {
                // Arctic region exceptions
                if ((laenge >= 0d) && (laenge < 9d)) zone = 31;
                else if ((laenge >= 9d) && (laenge < 21d)) zone = 33;
                else if ((laenge >= 21d) && (laenge < 33d)) zone = 35;
                else if ((laenge >= 33d) && (laenge < 42d)) zone = 37;
            }

            string sZone = string.Format("00", zone);
            int bandIndex = (int)(1 + (breite + 80) / 8);
            string band = UTM_BAND.Substring(bandIndex - 1, 1);

            // Geographische Breite (Rad)
            double breiteRad = breite * Math.PI / 180;

            double tangens1 = Math.Tan(breiteRad);
            double tangens2 = Math.Pow(tangens1, 2);
            double tangens4 = Math.Pow(tangens1, 4);
            double cosinus1 = Math.Cos(breiteRad);
            double cosinus2 = Math.Pow(cosinus1, 2);
            double cosinus3 = Math.Pow(cosinus1, 3);
            double cosinus4 = Math.Pow(cosinus1, 4);
            double cosinus5 = Math.Pow(cosinus1, 5);

            double eta = WGS84_EXZENT2 * cosinus2;

            // Querkrümmung
            double qkhm = WGS84_POL / Math.Sqrt(1 + eta);

            // Länge des Meridianbogens
            double lmbog = (koeff0 * breite) + (koeff2 * Math.Sin(2 * breiteRad)) + (koeff4 * Math.Sin(4 * breiteRad)) + (koeff6 * Math.Sin(6 * breiteRad));

            // Differenz zum Bezugsmeridian
            int merid = (zone - 30) * 6 - 3;

            double dlaenge1 = (laenge - merid) * Math.PI / 180;
            double dlaenge2 = Math.Pow(dlaenge1, 2);
            double dlaenge3 = Math.Pow(dlaenge1, 3);
            double dlaenge4 = Math.Pow(dlaenge1, 4);
            double dlaenge5 = Math.Pow(dlaenge1, 5);

            // Berechnung East, North
            double east, north;
            if (breite < 0)
            {
                north = 10E+06 + UTM_FAKTOR * (lmbog + qkhm * cosinus2 * tangens1 * dlaenge2 / 2 + qkhm * cosinus4 * tangens1 * (5 - tangens2 + 9 * eta) * dlaenge4 / 24);
            }
            else
            {
                north = UTM_FAKTOR * (lmbog + qkhm * cosinus2 * tangens1 * dlaenge2 / 2 + qkhm * cosinus4 * tangens1 * (5 - tangens2 + 9 * eta) * dlaenge4 / 24);
            }
            east = UTM_FAKTOR * (qkhm * cosinus1 * dlaenge1 + qkhm * cosinus3 * (1 - tangens2 + eta) * dlaenge3 / 6 + qkhm * cosinus5 * (5 - 18 * tangens2 + tangens4) * dlaenge5 / 120) + UTM_FALSE_EASTING;

            north = Math.Round(north, 3);
            east = Math.Round(east, 3);

            return new UTM(zone, band, east, north);
        }

    }

}
