//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/Transformation/UTMMGR.cs $
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
// File description   : Klasse Transformation, Funktion: UTMMGR 
//===================================================================================================



using System;
using GeoUtility.GeoSystem;


namespace GeoUtility
{
    internal partial class Transformation
    {

        /// <summary><para>Die Funktion wandelt zivile UTM Koordinaten in militärische Koordinaten um.
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
        /// <returns>Ein <see cref="GeoUtility.GeoSystem.MGRS"/>-Objekt (UTMREF/MGRS).</returns>
        internal MGRS UTMMGR(UTM utm)
        {

            int zone = utm.Zone;
            string band = utm.Band;
            char cz2 = band.ToCharArray()[0];
            char[] sep = { ',', '.' };

            // Die höchsten Stellen der East und North Koordinate werden für die Berechnung des Planquadrates benötigt.
            string e = utm.EastString.Split(sep)[0];
            int east_plan = int.Parse(e.Substring(0, 1));   // 1. Stelle des Ostwertes
            string n = utm.NorthString.Split(sep)[0];
            int north_plan = int.Parse(n.Substring(0, 2));  // 1. und 2. Stelle des Nordwertes
            
            // East Koordinate
            string east = ((int)Math.Round(utm.East)).ToString();
            if (east.Length > 2) east = east.Remove(0, 1);

            // North Koordinate
            string north = ((int)Math.Round(utm.North)).ToString();
            if (north.Length > 2) north = north.Remove(0, 2);

            // Anzahl Stellen bei East und North müssen gleich sein
            if (east.Length < north.Length)
            {
                east = east.PadLeft(north.Length, '0');
            }
            else if (north.Length < east.Length)
            {
                north = north.PadLeft(east.Length, '0');
            }

            if (zone < MIN_ZONE || zone > MAX_ZONE || cz2 < MIN_BAND || cz2 > MAX_BAND)
            {
                throw new ErrorProvider.GeoException(new ErrorProvider.ErrorMessage("ERROR_UTM_ZONE"));
            }

            // Berechnung des Indexes für die Ost-Planquadrat Komponente
            int eastgrid = 0;
            int i = zone % 3;
            if (i == 1) eastgrid = east_plan - 1;
            if (i == 2) eastgrid = east_plan + 7;
            if (i == 0) eastgrid = east_plan + 15;

            // Berechnung des Indexes für die Nord-Planquadrat Komponente
            int northgrid = 0;
            i = zone % 2;
            if (i != 1) northgrid = 5; 
            i = north_plan;
            while (i - 20 >= 0) 
            { 
                i = i - 20; 
            }
            northgrid += i;
            if (northgrid > 19) northgrid = northgrid - 20;

            // Planquadrat aus den vorher berechneten Indizes zusammensetzen
            string plan = MGRS_EAST.Substring(eastgrid, 1) + MGRS_NORTH1.Substring(northgrid, 1);

            return new MGRS(utm.Zone, utm.Band, plan, double.Parse(east), double.Parse(north));
        }
    }

}
