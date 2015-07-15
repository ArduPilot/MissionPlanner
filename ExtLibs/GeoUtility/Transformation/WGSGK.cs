//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/Transformation/WGSGK.cs $
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
// File description   : Klasse Transformation, Funktion: WGSGK 
//===================================================================================================



using System;
using GeoUtility.GeoSystem;
using GeoUtility.GeoSystem.Helper;

namespace GeoUtility
{
    internal partial class Transformation
    {

        /// <summary><para>Die Funktion wandelt geographische Koordinaten (Länge/Breite) eines <see cref="GeoSystem.Geographic"/>-Objektes 
        /// in ein <see cref="GeoUtility.GeoSystem.GaussKrueger">GaussKrueger</see>-Objekt um.
        /// <para>Die Funktion ist nur für interne Berechnungen bestimmt.</para></para></summary>
        /// <remarks><para>
        /// Hintergründe zum Problem der Koordinatentransformationen sowie entsprechende mathematische 
        /// Formeln können den einschlägigen Fachbüchern oder dem Internet entnommen werden.<p />
        /// Quellen: 
        /// Bundesamt für Kartographie und Geodäsie<br />
        /// <a href="http://www.bkg.bund.de" target="_blank">http://www.bkg.bund.de</a><br />
        /// <a href="http://crs.bkg.bund.de" target="_blank">http://crs.bkg.bund.de</a><br />
        /// </para></remarks>
        /// 
        /// <param name="geo"><see cref="GeoSystem.Geographic"/>-Objekt (<see cref="GeoUtility.GeoSystem.Helper.GeoDatum.Potsdam">Potsdam-Datum</see>)</param>
        /// <returns>Ein <see cref="GeoUtility.GeoSystem.GaussKrueger "/>-Objekt.</returns>
        internal GaussKrueger WGSGK(Geographic geo)
        {

            double laenge = geo.Longitude;
            double breite = geo.Latitude;

            if (breite < MIN_NORD || breite > MAX_NORD || laenge < MIN_OST || laenge > MAX_OST)
            {
                throw new ErrorProvider.GeoException(new ErrorProvider.ErrorMessage("ERROR_GK_OUT_OF_RANGE"));
            }

            // Datum muss eventuell erst nach Potsdam transformiert werden.
            if (geo.Datum == GeoDatum.WGS84) geo.SetDatum(GeoDatum.Potsdam);

            // Koeffizienten für Länge Meridianbogen
            double koeff0 = POL * (Math.PI / 180) * (1 - 3 * EXZENT2 / 4 + 45 * EXZENT4 / 64 - 175 * EXZENT6 / 256 + 11025 * EXZENT8 / 16384);
            double koeff2 = POL * (-3 * EXZENT2 / 8 + 15 * EXZENT4 / 32 - 525 * EXZENT6 / 1024 + 2205 * EXZENT8 / 4096);
            double koeff4 = POL * (15 * EXZENT4 / 256 - 105 * EXZENT6 / 1024 + 2205 * EXZENT8 / 16384);
            double koeff6 = POL * (-35 * EXZENT6 / 3072 + 315 * EXZENT8 / 12288);

            // Breite (Rad)
            double breiteRad = breite * Math.PI / 180;

            double tangens1 = Math.Tan(breiteRad);
            double tangens2 = Math.Pow(tangens1, 2);
            double tangens4 = Math.Pow(tangens1, 4);
            double cosinus1 = Math.Cos(breiteRad);
            double cosinus2 = Math.Pow(cosinus1, 2);
            double cosinus3 = Math.Pow(cosinus1, 3);
            double cosinus4 = Math.Pow(cosinus1, 4);
            double cosinus5 = Math.Pow(cosinus1, 5);

            double eta = EXZENT2 * cosinus2;

            // Querkrümmung
            double qkhm = POL / Math.Sqrt(1 + eta);

            // Länge des Meridianbogens
            double lmbog = koeff0 * breite + koeff2 * Math.Sin(2 * breiteRad) + koeff4 * Math.Sin(4 * breiteRad) + koeff6 * Math.Sin(6 * breiteRad);

            // Differenz zum Bezugsmeridian
            int kfakt = (int)((laenge + 1.5) / 3);
            int merid = kfakt * 3;
            double dlaenge1 = (laenge - merid) * Math.PI / 180;
            double dlaenge2 = Math.Pow(dlaenge1, 2);
            double dlaenge3 = Math.Pow(dlaenge1, 3);
            double dlaenge4 = Math.Pow(dlaenge1, 4);
            double dlaenge5 = Math.Pow(dlaenge1, 5);

            // Hochwert, Rechtswert
            double hoch = (lmbog + qkhm * cosinus2 * tangens1 * dlaenge2 / 2 + qkhm * cosinus4 * tangens1 * (5 - tangens2 + 9 * eta) * dlaenge4 / 24);
            double rechts = (qkhm * cosinus1 * dlaenge1 + qkhm * cosinus3 * (1 - tangens2 + eta) * dlaenge3 / 6 + qkhm * cosinus5 * (5 - 18 * tangens2 + tangens4) * dlaenge5 / 120 + kfakt * 1e6 + 500000);

            return new GaussKrueger(rechts, hoch); ;
        }

    }

}
