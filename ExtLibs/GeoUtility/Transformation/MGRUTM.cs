//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/Transformation/MGRUTM.cs $
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
// File description   : Klasse Transformation, Funktion: MGRUTM 
//===================================================================================================


using System;
using GeoUtility.GeoSystem;


namespace GeoUtility
{
    internal partial class Transformation
    {

        /// <summary><para>Die Funktion wandelt militärische MGRS-Koordinaten (Military Grid Reference System, UTMREF) in UTM-Koordinaten um.
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
        /// <param name="mgrs">Ein <see cref="MGRS"/>-Objekt.</param>
        /// <returns>Ein <see cref="UTM"/>-Objekt.</returns>
        internal UTM MGRUTM(MGRS mgrs) 
        {
            int zone = mgrs.Zone;
            string eastgrid = mgrs.Grid.Substring(0, 1);
            string northgrid = mgrs.Grid.Substring(1, 1);

            // Führende Stelle East-Koordinate aus Planquadrat(east) und Koordinaten berechnen
            int i = mgrs.Zone % 3;
            int stellenwert = 0;
            if (i == 0) stellenwert = MGRS_EAST0.IndexOf(eastgrid) + 1;
            if (i == 1) stellenwert = MGRS_EAST1.IndexOf(eastgrid) + 1;
            if (i == 2) stellenwert = MGRS_EAST2.IndexOf(eastgrid) + 1;
            string sEast = stellenwert.ToString() + mgrs.EastString.PadRight(5, '0');

            // Führende Stelle North-Koordinate aus Planquadrat(north) 
            i = mgrs.Zone % 2;
            stellenwert = 0;
            if (i == 0)
            {
                stellenwert = MGRS_NORTH0.IndexOf(northgrid);
            }
            else
            {
                stellenwert = MGRS_NORTH1.IndexOf(northgrid);
            }

            // Führende Stelle North-Koordinate berechnen 
            char band = mgrs.Band.ToCharArray()[0];

            if (zone < MIN_ZONE || zone > MAX_ZONE || band < MIN_BAND || band > MAX_BAND)
            {
                throw new ErrorProvider.GeoException(new ErrorProvider.ErrorMessage("ERROR_UTM_ZONE"));
            }

            if (band >= 'N')
            {
                if ((band == 'Q') && (stellenwert < 10)) stellenwert += 20;
                if (band >= 'R') stellenwert += 20;
                if ((band == 'S') && (stellenwert < 30)) stellenwert += 20;
                if (band >= 'T') stellenwert += 20;
                if ((band == 'U') && (stellenwert < 50)) stellenwert += 20;
                if (band >= 'V') stellenwert += 20;
                if ((band == 'W') && (stellenwert < 70)) stellenwert += 20;
                if (band >= 'X') stellenwert += 20;
            }
            else
            {
                if ((band == 'C') && (stellenwert < 10)) stellenwert += 20;
                if (band >= 'D') stellenwert += 20;
                if ((band == 'F') && (stellenwert < 30)) stellenwert += 20;
                if (band >= 'G') stellenwert += 20;
                if ((band == 'H') && (stellenwert < 50)) stellenwert += 20;
                if (band >= 'J') stellenwert += 20;
                if ((band == 'K') && (stellenwert < 70)) stellenwert += 20;
                if (band >= 'L') stellenwert += 20;
            }

            // North-Koordinate 
            string sNorth = "";
            if ((stellenwert.ToString()).Length == 1) sNorth = "0";
            sNorth += stellenwert.ToString() + mgrs.NorthString.PadRight(5, '0');

            return new UTM(zone, band.ToString(), double.Parse(sEast), double.Parse(sNorth));
        }

    }

}
