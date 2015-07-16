//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/GeoSystem/Base/BaseSystem.cs $
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
// File description   : Definition der abstrakten BaseSystem Basisklasse 
//===================================================================================================


namespace GeoUtility.GeoSystem.Base
{

    /// <summary><para>Abstrakte Basisklasse von der alle Koordinatensystem-Klassen abgeleitet werden.</para></summary>
    /// <remarks><para>
    /// Diese Basisklasse stellt für alle abgeleiteten Klassen die Funktionen der internen Klasse
    /// <see cref="GeoUtility.Transformation"/> zur Verfügung. Die Klasse <see cref="GeoUtility.Transformation"/> führt 
    /// die zugrundeliegenden Transformationen aus, welche von den Koordinatensystem-Klassen in Form von
    /// impliziten und expliziten Typkonvertierungen, sowie den Funktionen <c>ConvertFrom</c> und <c>ConvertTo</c>
    /// zur Verfügung gestellt werden.
    /// </para></remarks>
    public abstract class BaseSystem
    {
        /// <summary><para>Instanz der internen Klasse <see cref="GeoUtility.Transformation"/>.</para></summary>
        internal static GeoUtility.Transformation Transform = new GeoUtility.Transformation();

        /// <summary><para>Der statische Konstruktor.</para></summary>
        static BaseSystem() { }

        /// <summary><para>Der Standard-Konstruktor.</para></summary>
        internal BaseSystem() { }
    }

}
