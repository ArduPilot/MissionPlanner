//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/GeoSystem/Base/Geocentric.cs $
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
// File description   : Definition der abstrakten Geocentric Basisklasse 
//===================================================================================================


using System;

namespace GeoUtility.GeoSystem.Base
{

    /// <summary><para>Abstrakte Basislasse für geozentrische Koordinatensysteme.</para></summary>
    /// <remarks><para>
    /// Die Klasse <c>Geocentric</c> beinhaltet Member und Methoden, die für alle <i>geozentrischen Koordinatensysteme</i>
    /// gleich sind. Die Klassen <b>UTM</b> und <b>MGRS</b> werden von dieser Basisklasse abgeleitet.
    /// </para></remarks>
    public abstract class Geocentric : BaseSystem
    {
        #region ==================== Konstanten ====================

        private const string UTM_BAND = "CDEFGHJKLMNPQRSTUVWXX";            // Bandauswahl

        #endregion ==================== Konstanten ====================




        #region ==================== Membervariablen ====================

        private int _zone = 0;                      // Speicherplatz für die UTM-Zone
        private string _band = "";                  // Speicherplatz für das UTM-Band
        private double _north = 0;                  // Speicherplatz für den North-Wert
        private double _east = 0;                   // Speicherplatz für den East-Wert
        private string _northstring = "";           // Speicherplatz für den North-Wert als String
        private string _eaststring = "";            // Speicherplatz für den East-Wert als String
        private int _precision = 0;                 // Speicherplatz für Nachkommastellen bei Stringausgabe

        #endregion ==================== Membervariablen ====================




        #region ==================== Konstruktoren ====================

        /// <summary><para>Standard-Konstruktor</para></summary>
        protected Geocentric() { }


        /// <summary><para>Ein Konstruktor mit den Parametern für Zone, Band, East und Nordwert.</para></summary>
        /// 
        /// <param name="zone">Zone</param>
        /// <param name="band">Band</param>
        /// <param name="east">East</param>
        /// <param name="north">North</param>
        protected Geocentric(int zone, string band, double east, double north)
        {
            this.East = east;
            this.North = north;
            this.Zone = zone;
            this.Band = band;
        }


        /// <summary><para>Ein Konstruktor mit den Parametern für Zone, East und Nordwert, sowie Hemisphäre.</para></summary>
        /// <param name="zone">Zone</param>
        /// <param name="east">East</param>
        /// <param name="north">North</param>
        /// <param name="hem">Hemisphäre: Nord, Süd</param>
        protected Geocentric(int zone, double east, double north, Hemisphere hem)
        {
            string band = "N";                                                      // Transformationsroutine auf Nordhalbkugel setzen 
            if (hem == Hemisphere.South) band = "A";                                // Transformationsroutine auf Südhalbkugel setzen 
            Geographic geo = Transform.UTMWGS(new UTM(zone, band, east, north));
            geo.Longitude = Math.Round(geo.Longitude, 4);                           // Ohne Rundung könnte Bandberechnung fehlerbehaftet sein
            geo.Latitude = Math.Round(geo.Latitude, 4);                             // Ohne Rundung könnte Bandberechnung fehlerbehaftet sein
            UTM utm = Transform.WGSUTM(geo);                                       // Doppelte Transformation um Band zu berechnen

            this.East = east;
            this.North = north;
            this.Zone = zone;
            this.Band = utm.Band;

        }

        #endregion ==================== Konstruktoren ====================




        #region ==================== Eigenschaften ====================

        /// <summary><para>East-Wert (Ost) des geozentrischen Systems als Datentyp <i>double</i>.</para></summary>
        /// 
        /// <example>Dieses Beispiel demonstriert die Benutzung der East-Eigenschaft in der UTM Klasse.
        /// <code>
        /// GeoUtility.GeoSystem.UTM utm = new GeoUtility.GeoSystem.UTM();
        /// utm.East = 5678901;
        /// </code>
        /// </example>
        public double East
        {
            get { return _east; }
            set
            {
                _east = value;
                this.Format(Target.East);
            }
        }


        /// <summary><para>North-Wert (Nord) des geozentrischen Systems als Datentyp <i>double</i>.</para></summary>
        /// 
        /// <example>Dieses Beispiel demonstriert die Benutzung der North-Eigenschaft in der UTM Klasse.
        /// <code>
        /// GeoUtility.GeoSystem.UTM utm = new GeoUtility.GeoSystem.UTM();
        /// utm.North = 456789;
        /// </code>
        /// </example>
        public double North
        {
            get { return _north; }
            set
            {
                _north = value;
                this.Format(Target.North);
            }
        }


        /// <summary><para>Die Zone Eigenschaft übernimmt den Zonenwert des geozentrischen Systems.</para></summary>
        /// 
        /// <example>Dieses Beispiel demonstriert die Benutzung der Zone Eigenschaft in einem UTM Objekt.
        /// <code>
        /// GeoUtility.GeoSystem.UTM utm = new GeoUtility.GeoSystem.UTM();
        /// utm.Zone = 32;
        /// </code>
        /// </example>
        public int Zone { get { return _zone; } set { _zone = value; } }


        /// <summary><para>Die Band Eigenschaft übernimmt den Bandwert des geozentrischen Systems.</para></summary>
        /// 
        /// <example>Dieses Beispiel demonstriert die Benutzung der Band Eigenschaft in einem UTM Objekt.
        /// <code>
        /// GeoUtility.GeoSystem.UTM utm = new GeoUtility.GeoSystem.UTM();
        /// utm.Band = "U";
        /// </code>
        /// </example>
        public string Band 
        { 
            get 
            { 
                return _band; 
            } 
            set 
            {
                string band = value.ToUpper();
                char cband = band.ToCharArray()[0];
                char lband = UTM_BAND.ToCharArray()[0];
                char uband = UTM_BAND.ToCharArray()[UTM_BAND.Length - 1];

                if (cband < lband)
                {
                    band = lband.ToString();
                }
                else if (cband > uband)
                {
                    band = uband.ToString();
                }
                _band = band; 
            } 
        }


        /// <summary><para>Die Eigenschaft <i>Zoneband</i> gibt das Zonenband zurück.</para></summary>
        /// 
        /// <example>Dieses Beispiel demonstriert die Benutzung der Zoneband Eigenschaft in der UTM Klasse.
        /// <code>
        /// GeoUtility.GeoSystem.UTM utm = new GeoUtility.GeoSystem.UTM(32, "U", 5678901, 456789);
        /// string output = utm.Zoneband;                               // Ausgabe: "32U"
        /// </code>
        /// </example>
        public string Zoneband { get { return this.Zone.ToString() + this.Band; } }


        /// <summary><para>Wert East als Datentyp string. Dieser Wert hat keine Auswirkung auf die Berechnung 
        /// von Koordinaten, sondern wird nur für spezielle Ausgaben benötigt.</para></summary>
        public string EastString { get { return _eaststring; } set { _eaststring = value; } }


        /// <summary><para>Wert North als Datentyp string. Dieser Wert hat keine Auswirkung auf die Berechnung 
        /// von Koordinaten, sondern wird nur für spezielle Ausgaben benötigt.</para></summary>
        public string NorthString { get { return _northstring; } set { _northstring = value; } }

        
        /// <summary><para>Bestimmt die Anzahl der Nachkommastellen in der Ausgabe.</para></summary>
        public int Precision
        {
            get
            {
                return _precision;
            }
            set
            {
                _precision = value;
                this.Format(Target.All);
            }
        }

        #endregion ==================== Eigenschaften ====================




        #region ==================== Methoden ====================

        /// <summary><para>Abstrakte Methode für die Formatierung des Ausgabestrings. 
        /// Die abgeleiteten Klassen implementieren jeweils unterschiedliche Methoden.</para></summary>
        /// 
        /// <param name="target">Zu formatierendes Element</param>
        internal abstract void Format(Target target);


        /// <summary><para>Erstellt einen langen Koordinatenstring</para></summary>
        /// <returns>Zusammengesetzter Koordinaten-String</returns>
        public abstract string ToLongString();

        #endregion ==================== Methoden ====================




        #region ==================== Subklassen, Enum, etc. ====================

        /// <summary><para>Der Hemisphere Enumerator wird in einigen Konstruktoren verwendet.</para></summary>
        public enum Hemisphere
        {
            /// <summary><para>Nördliche Hemisphäre</para></summary>
            North,

            /// <summary><para>Südliche Hemisphäre</para></summary>
            South
        }


        /// <summary><para>Der Target Enumerator legt fest welcher Wert formatiert werden soll.</para></summary>
        public enum Target
        {
            /// <summary><para>Formatierung des East- und North-Wertes.</para></summary>
            All,

            /// <summary><para>Formatierung des East-Wertes.</para></summary>
            East,

            /// <summary><para>Formatierung des North-Wertes.</para></summary>
            North,

            /// <summary><para>Kann für MGRS verwendet werden.</para></summary>
            Nothing

        }

        #endregion ==================== Subklassen, Enum, etc. ====================

    }

}
