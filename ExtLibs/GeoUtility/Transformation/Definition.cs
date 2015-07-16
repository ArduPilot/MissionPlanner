//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/Transformation/Definition.cs $
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
// File description   : Konstanten der Transformationsklasse 
//===================================================================================================



namespace GeoUtility
{
    internal partial class Transformation
    {

        /// <summary><para>Der Standard- Konstruktor.</para></summary>
        internal Transformation() { }



        // Konstanten der Transformationsklasse

        #region ===================== UTM =====================

        // Faktor Maßstab und False Easting
        const double UTM_FAKTOR = 0.9996;
        const double UTM_FALSE_EASTING = 500000;

        // Grosse Halbachse 
        const double WGS84_HALBACHSE = 6378137.000;

        // Abplattung WGS84 = 298,257223563 (1/x)
        const double WGS84_ABPLATTUNG = 3.35281066474748E-03; 

        // UTM-Band Werte von C-X (XX nötwendig, damit Index nicht außerhalb)
        const string UTM_BAND = "CDEFGHJKLMNPQRSTUVWXX";
        
        // Polkrümmung
        const double WGS84_POL = WGS84_HALBACHSE / (1 - WGS84_ABPLATTUNG);

        // Num. Exzentrizitäten
        const double WGS84_EXZENT = ((2 * WGS84_ABPLATTUNG) - (WGS84_ABPLATTUNG * WGS84_ABPLATTUNG));
        const double WGS84_EXZENT2 = ((2 * WGS84_ABPLATTUNG) - (WGS84_ABPLATTUNG * WGS84_ABPLATTUNG)) / ((1 - WGS84_ABPLATTUNG) * (1 - WGS84_ABPLATTUNG));
        const double WGS84_EXZENT4 = WGS84_EXZENT2 * WGS84_EXZENT2;
        const double WGS84_EXZENT6 = WGS84_EXZENT4 * WGS84_EXZENT2;
        const double WGS84_EXZENT8 = WGS84_EXZENT4 * WGS84_EXZENT4;

        // Geographische Grenzen des UTM-Systems in Grad
        const double MIN_LAENGE = -180.0;
        const double MAX_LAENGE = +180.0;
        const double MIN_BREITE = -80.0;
        const double MAX_BREITE = +84.0;

        #endregion ===================== UTM =====================



        #region ===================== MGRS =====================

        const string MGRS_EAST0 = "STUVWXYZ";
        const string MGRS_EAST1 = "ABCDEFGH";
        const string MGRS_EAST2 = "JKLMNPQR";
        const string MGRS_EAST = MGRS_EAST1 + MGRS_EAST2 + MGRS_EAST0;

        const string MGRS_NORTH0 = "FGHJKLMNPQRSTUVABCDE";
        const string MGRS_NORTH1 = "ABCDEFGHJKLMNPQRSTUV";

        const int MIN_ZONE = 1;
        const int MAX_ZONE = 60;
        const char MIN_BAND = 'C';
        const char MAX_BAND = 'X';

        #endregion ********************* MGRS ************************



        #region ===================== Gauss-Krueger =====================

        // Große Halbachse und Abplattung BESSEL
        const double HALBACHSE = 6377397.155;
        const double ABPLATTUNG = 3.342773182E-03;

        // Polkrümmung
        const double POL = HALBACHSE / (1 - ABPLATTUNG);

        // Num. Exzentrizitäten
        const double EXZENT = ((2 * ABPLATTUNG) - (ABPLATTUNG * ABPLATTUNG));
        const double EXZENT2 = ((2 * ABPLATTUNG) - (ABPLATTUNG * ABPLATTUNG)) / ((1 - ABPLATTUNG) * (1 - ABPLATTUNG));
        const double EXZENT4 = EXZENT2 * EXZENT2;
        const double EXZENT6 = EXZENT4 * EXZENT2;
        const double EXZENT8 = EXZENT4 * EXZENT4;

        // Geographische Grenzen des Gauss-Krüger-Systems in Grad
        const double MIN_OST = 5.0;
        const double MAX_OST = 16.0;
        const double MIN_NORD = 46.0;
        const double MAX_NORD = 56.0;

        // Parameter für Datumsverschiebung Potsdam - WGS84 (entgegengesetzte Verschiebung dann negative Werte)
        const double POTSDAM_DATUM_SHIFT_X = 587;
        const double POTSDAM_DATUM_SHIFT_Y = 16;
        const double POTSDAM_DATUM_SHIFT_Z = 393;

        #endregion ===================== Gauss-Krueger =====================
    }
}
