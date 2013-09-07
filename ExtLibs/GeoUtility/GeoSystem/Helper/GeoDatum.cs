//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/GeoSystem/Helper/GeoDatum.cs $
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
// File description   : Definition des GeoDatum Enumerators (Potsdam, WGS84) 
//===================================================================================================


namespace GeoUtility.GeoSystem.Helper
{
    /// <summary><para>Enumeration Geodatum: <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84"/> (International), 
    /// <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.Potsdam">Potsdam-Datum</see> (nur Deutschland)</para></summary>
    /// <remarks><para>
    /// Wird eine Koordinatentransformation in <see cref="GeoUtility.GeoSystem.GaussKrueger">Gauss-Krüger</see> 
    /// Koordinatensystem durchgeführt, so müssen die Koordinaten des <see cref="GeoUtility.GeoSystem.Geographic">Längen-/Breitensystems</see> 
    /// im <see cref="Potsdam">Potsdam-Datum</see> vorliegen. Alle anderen Systeme benutzen das Datum WGS84. 
    /// Bei der Transformation eines Koordinatensystems in das <see cref="GeoUtility.GeoSystem.GaussKrueger">Gauss-Krüger</see> 
    /// System müssen die Koordinaten also in das <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.Potsdam">Potsdam-Datum</see> 
    /// gebracht werden. Dies geschieht jedoch normalerweise bei der Typkonvertierung automatisch. 
    /// <para>Hintergründe dazu siehe auch den Wikipedia-Artikel <i>Geodätisches Datum</i> unter dem Link: <br />
    /// <a href="http://de.wikipedia.org/wiki/Geod%C3%A4tisches_Datum" target="_blank">http://de.wikipedia.org/wiki/Geod%C3%A4tisches_Datum</a></para>
    /// </para></remarks>
    /// <seealso cref="GeoUtility.GeoSystem.Geographic.SetDatum"/>
    /// 
    /// <example>Dieses Beispiel zeigt das explizite Setzen des Datums bei einer Konvertierung von einem
    /// <see cref="GeoUtility.GeoSystem.Geographic">Längen-/Breitensystem</see> in ein 
    /// <see cref="GeoUtility.GeoSystem.GaussKrueger">Gauss-Krüger-System</see>. <para>Bitte beachten: Das Setzen 
    /// des Datums geschieht bei der Typkonvertierung automatisch, ist also nicht erforderlich. Bei der Rückkonvertierung 
    /// von <see cref="GeoUtility.GeoSystem.GaussKrueger">Gauss-Krüger</see> in das Längen-Breiten-System, wird 
    /// automatisch das geodätische Datum <see cref="WGS84"/> gesetzt.</para>
    /// <code>
    /// GeoUtility.GeoSystem.Geographic geo = new GeoUtility.GeoSystem.Geographic(8.12345, 50.54321);
    /// geo.SetDatum(GeoDatum.Potsdam)
    /// GeoUtility.GeoSystem.GaussKrueger gk = (GeoUtility.GeoSystem.GaussKrueger)geo;
    /// </code>
    /// </example>
    public enum GeoDatum
    {
        /// <summary><para>Geodätisches Datum <see cref="WGS84"/> (International)</para></summary>
        WGS84,

        /// <summary><para>Geodätisches Datum <see cref="Potsdam"/> (nur für Deutschland gültig)</para></summary>
        Potsdam
    }
}
