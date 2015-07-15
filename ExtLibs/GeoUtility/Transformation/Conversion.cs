//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/Transformation/Conversion.cs $
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
// File description   : Definition des Conversion Klasse 
//===================================================================================================


using GeoUtility.GeoSystem;
using GeoUtility.GeoSystem.Base;


namespace GeoUtility
{

    /// <summary><para>Die öffentliche Klasse <see cref="Conversion"/> bietet eine von mehreren Varianten zur 
    /// Transformierung von Koordinaten eines Systems in ein anderes.</para></summary>
    /// 
    /// <remarks><para>Die Klasse verwendet für die Transformation generische Methoden. Sollten Sie nicht
    /// mit generischen Methoden vertraut sein, ist möglicherweise die Verwendung der Methoden 
    /// für die Typkonvertierung der Koordinatensystem-Klassen vorzuziehen.</para></remarks>
    /// 
    /// <example>Dieses Beispiel zeigt die Verwendung der generischen Methode <see cref="Convert{S, T}">Convert&lt;S, T&gt;</see>. 
    /// In dem Beispiel wird eine MGRS-Koordinate nach Längengrad-Breitengrad (<see cref="Geographic"/>) transformiert:
    /// <code>
    /// using GeoUtility.GeoSystem;
    /// MGRS mgrs = new MGRS(32, "U", "MB", 78123, 29123);
    /// Geographic geo;
    /// GeoUtility.Conversion c = new GeoUtility.Conversion();
    /// geo = c.Convert&lt;MGRS, Geographic&gt;(mgrs);
    /// </code>
    /// </example>
    public class Conversion
    {

        /// <summary><para>Der Standard- Konstruktor.</para></summary>
        public Conversion() { }


        /// <summary><para>Generische Funktion konvertiert ein Objekt eines Quellsystems S in das Zielsystem T</para></summary>
        /// 
        /// <example>Dieses Beispiel zeigt die Verwendung der generischen Methode <see cref="Convert{S, T}">Convert&lt;S, T&gt;</see>. 
        /// In dem Beispiel wird ein <see cref="MGRS"/>-Objekt in ein <see cref="UTM"/>-Objekt transformiert:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// MGRS mgrs = new MGRS("32UMB7812329123");
        /// UTM utm;
        /// GeoUtility.Conversion c = new GeoUtility.Conversion();
        /// utm = c.Convert&lt;MGRS, UTM&gt;(mgrs);
        /// </code>
        /// </example>
        /// 
        /// <typeparam name="S">Typ des Quellsystems.</typeparam>
        /// <typeparam name="T">Typ des Zielsystems.</typeparam>
        /// <param name="source">Zu konvertierendes Objekt</param>
        /// <returns>Ein Objekt des Zieltyps T</returns>
        public T Convert<S, T>(S source)
            where S : BaseSystem
            where T : BaseSystem
        {
            if (typeof(S) == typeof(UTM))
            {
                if (typeof(T) == typeof(MGRS)) return (T)(BaseSystem)(MGRS)(UTM)(BaseSystem)source;
                if (typeof(T) == typeof(Geographic)) return (T)(BaseSystem)(Geographic)(UTM)(BaseSystem)source;
                if (typeof(T) == typeof(GaussKrueger)) return (T)(BaseSystem)(GaussKrueger)(UTM)(BaseSystem)source;
                if (typeof(T) == typeof(MapService)) return (T)(BaseSystem)(MapService)(UTM)(BaseSystem)source;
            }
            else if (typeof(S) == typeof(MGRS))
            {
                if (typeof(T) == typeof(UTM)) return (T)(BaseSystem)(UTM)(MGRS)(BaseSystem)source;
                if (typeof(T) == typeof(Geographic)) return (T)(BaseSystem)(Geographic)(MGRS)(BaseSystem)source;
                if (typeof(T) == typeof(GaussKrueger)) return (T)(BaseSystem)(GaussKrueger)(MGRS)(BaseSystem)source;
                if (typeof(T) == typeof(MapService)) return (T)(BaseSystem)(MapService)(MGRS)(BaseSystem)source;
            }
            else if (typeof(S) == typeof(Geographic))
            {
                if (typeof(T) == typeof(UTM)) return (T)(BaseSystem)(UTM)(Geographic)(BaseSystem)source;
                if (typeof(T) == typeof(MGRS)) return (T)(BaseSystem)(MGRS)(Geographic)(BaseSystem)source;
                if (typeof(T) == typeof(GaussKrueger)) return (T)(BaseSystem)(GaussKrueger)(Geographic)(BaseSystem)source;
                if (typeof(T) == typeof(MapService)) return (T)(BaseSystem)(MapService)(Geographic)(BaseSystem)source;
            }
            else if (typeof(S) == typeof(GaussKrueger))
            {
                if (typeof(T) == typeof(UTM)) return (T)(BaseSystem)(UTM)(GaussKrueger)(BaseSystem)source;
                if (typeof(T) == typeof(MGRS)) return (T)(BaseSystem)(MGRS)(GaussKrueger)(BaseSystem)source;
                if (typeof(T) == typeof(Geographic)) return (T)(BaseSystem)(Geographic)(GaussKrueger)(BaseSystem)source;
                if (typeof(T) == typeof(MapService)) return (T)(BaseSystem)(MapService)(GaussKrueger)(BaseSystem)source;
            }
            else if (typeof(S) == typeof(MapService))
            {
                if (typeof(T) == typeof(UTM)) return (T)(BaseSystem)(UTM)(MapService)(BaseSystem)source;
                if (typeof(T) == typeof(MGRS)) return (T)(BaseSystem)(MGRS)(MapService)(BaseSystem)source;
                if (typeof(T) == typeof(GaussKrueger)) return (T)(BaseSystem)(GaussKrueger)(MapService)(BaseSystem)source;
                if (typeof(T) == typeof(Geographic)) return (T)(BaseSystem)(Geographic)(MapService)(BaseSystem)source;
            }
            return null;
        }
    }

}
