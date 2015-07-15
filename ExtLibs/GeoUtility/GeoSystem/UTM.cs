//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/GeoSystem/UTM.cs $
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
// File description   : Definition der UTM Klasse (Universal Transverse Mercator System)
//===================================================================================================


using System;
using System.Globalization;
using GeoUtility.GeoSystem.Base;
using GeoUtility.GeoSystem.Helper;
using System.Collections.Generic;
using System.Text;

namespace GeoUtility.GeoSystem
{

    /// <summary><para>UTM Koordinaten, von der Basisklasse <see cref="Geocentric"/> abgeleitet.</para></summary>
    /// <remarks><para>
    /// Die <see cref="UTM"/>-Klasse implementiert Methoden und Eigenschaften für das international  
    /// verwendete UTM (Universal Transverse Mercator) Koordinatensystem.
    /// </para></remarks>
    public class UTM : Geocentric
    {

        #region ==================== Konstruktoren ====================

        /// <summary><para>Der Standard-Konstruktor.</para></summary>
        /// 
        /// <example>Das folgende Beispiel erzeugt eine Instanz der <see cref="UTM"/>-Klasse und weist 
        /// anschließend neue Werte für <see cref="Geocentric.Zone"/>, <see cref="Geocentric.Band"/>, 
        /// <see cref="Geocentric.East"/> und <see cref="Geocentric.North"/> zu: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// UTM utm = new UTM();
        /// utm.Zone = 32;
        /// utm.Band = "U";
        /// utm.East = 412345;
        /// utm.North = 5567890;
        /// </code>
        /// </example>
        public UTM() { }


        /// <summary><para>Konstruktor mit Parametern für <see cref="Geocentric.Zone"/>, <see cref="Geocentric.Band"/>, 
        /// <see cref="Geocentric.East"/> und <see cref="Geocentric.North"/>.</para></summary>
        /// 
        /// <example>Das folgende Beispiel erzeugt eine Instanz der <see cref="UTM"/>-Klasse und übergibt als Parameter 
        /// die Werte für <see cref="Geocentric.Zone"/>, <see cref="Geocentric.Band"/>, 
        /// <see cref="Geocentric.East"/> und <see cref="Geocentric.North"/>: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// UTM utm = new UTM(32, "U", 412345, 5567890);
        /// </code>
        /// </example>
        /// 
        /// <param name="zone">Zone</param>
        /// <param name="band">Band</param>
        /// <param name="east">East Wert</param>
        /// <param name="north">North Wert</param>
        public UTM(int zone, string band, double east, double north) : base(zone, band, east, north) { }


        /// <summary><para>Konstruktor mit Parametern für <see cref="Geocentric.Zone"/>, 
        /// <see cref="Geocentric.East"/>, <see cref="Geocentric.North"/> und <see cref="Geocentric.Hemisphere"/>, 
        /// ohne <see cref="Geocentric.Band"/>.</para></summary>
        /// 
        /// <example>Das folgende Beispiel erzeugt eine Instanz der UTM Klasse und übergibt als Parameter 
        /// die Werte für <see cref="Geocentric.Zone"/>, <see cref="Geocentric.East"/>, <see cref="Geocentric.North"/> 
        /// und <see cref="Geocentric.Hemisphere"/>. Bitte beachten sie, dass das Band automatisch berechnet wird: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// UTM utm = new UTM(32, 412345, 5567890, Hemisphere.North);
        /// </code>
        /// </example>
        /// 
        /// <param name="zone">Zone</param>
        /// <param name="east">East Wert</param>
        /// <param name="north">North Wert</param>
        /// <param name="hem">Nördliche oder südliche Hemisphäre</param>
        public UTM(int zone, double east, double north, Hemisphere hem) : base(zone, east, north, hem) { }

        #endregion ==================== Konstruktoren ====================




        #region ==================== Eigenschaften ====================

        /// <summary><para>Prüft ob bereits Koordinatenwerte gesetzt wurden.</para></summary>
        public bool isEmpty { get { return ((this.Zone == 0) && (this.Band == "") && (this.East == 0d) && (this.North == 0d)); } }

        #endregion ==================== Eigenschaften ====================




        #region ==================== Methoden ====================

        /// <summary><para>Die statische Methode kann dazu verwendet werden, als String-Werte übergebene UTM Parameter 
        /// auf ihre Gültigkeit zu überprüfen. Die Methode gibt eine Liste gültiger Parameter, eine Fehlermeldung und 
        /// ein <see cref="UTM"/>-Objekt zurück. Ist einer der Parameter ungültig, wird ein <see cref="UTM"/>-Objekt 
        /// mir dem Wert null zurückgegeben.</para></summary>
        /// <param name="Zone">Zone als Typ <see cref="System.String"/>.</param>
        /// <param name="Band">Band als Typ <see cref="System.String"/>.</param>
        /// <param name="East">East-Wert als Typ <see cref="System.String"/>.</param>
        /// <param name="North">North-Wert als Typ <see cref="System.String"/>.</param>
        /// <param name="Utm">Ein gültiges <see cref="UTM"/>-Objekt oder null.</param>
        /// <param name="ErrorMessage">Eine ausführliche Fehlermeldung, falls ein Fehler aufgetreten ist.</param>
        /// <param name="ValidItems">Ein <see cref="System.Collections.Generic.Dictionary{T, T}"/>-Objekt, in dem die gültigen und ungültigen Parameter aufgeführt werden.</param>
        /// <returns>True, wenn alle Parameter gültig sind, sonst False.</returns>
        static public bool TryParse(string Zone, string Band, string East, string North, out UTM Utm, out string ErrorMessage, out Dictionary<string, bool>ValidItems)
        { 
            bool result = true;
            bool converted = true;
            StringBuilder sb = new StringBuilder();
            UTM utm = null;
            Dictionary<string, bool> list = new Dictionary<string, bool>();
            int zone = 0, east = 0, north = 0;
            char band = 'A';
            Band = Band.ToUpper();

            try { zone = int.Parse(Zone); } catch { converted = false; }
            if ((zone < 1) || (zone > 60)) converted = false;
            list.Add("Zone", converted);
            if (list["Zone"] == false)
            {
                sb.Append(new ErrorProvider.ErrorMessage("ERROR_UTM_ZONE2").Text + "\r\n");
                result = false;
            }
            converted = true;

            converted = char.IsLetter(Band, 0);
            try { band = Convert.ToChar(Band); } catch { converted = false; }
            if ((band < 'C') || (band > 'X') || (band == 'I') || (band == 'O')) converted = false;
            list.Add("Band", converted);
            if (list["Band"] == false)
            {
                sb.Append(new ErrorProvider.ErrorMessage("ERROR_UTM_Band").Text + "\r\n");
                result = false;
            }
            converted = true;

            try { east = int.Parse(East); } catch { converted = false; }
            if ((east < 160000) || (east > 840000)) converted = false;
            list.Add("East", converted);
            if (list["East"] == false)
            {
                sb.Append(new ErrorProvider.ErrorMessage("ERROR_UTM_EAST").Text + "\r\n");
                result = false;
            }
            converted = true;

            try { north = int.Parse(North); } catch { converted = false; }
            if ((band <= 'M' && ((north < 1116915) || (north > 10000000))) || (band >= 'N' && ((north < 0) || (north > 9329005)))) converted = false;
            list.Add("North", converted);
            if (list["North"] == false)
            {
                sb.Append(new ErrorProvider.ErrorMessage("ERROR_UTM_NORTH").Text + "\r\n");
                result = false;
            }

            if (result == true) utm = new UTM(zone, band.ToString(), east, north);
            Utm = utm;
            ErrorMessage = sb.ToString();
            ValidItems = list;
            return result;
        }


        /// <summary><para>Die statische Methode kann dazu verwendet werden, als String-Werte übergebene UTM Parameter 
        /// auf ihre Gültigkeit zu überprüfen. Die Methode gibt eine Liste gültiger Parameter, eine Fehlermeldung und 
        /// ein <see cref="UTM"/>-Objekt zurück. Ist einer der Parameter ungültig, wird ein <see cref="UTM"/>-Objekt 
        /// mir dem Wert null zurückgegeben.</para></summary>
        /// <param name="UTMString">Vollständige UTM-Koordinate als <see cref="System.String"/>-Typ (Bsp.: 32U 312345 4123456 oder 32 U 312345 4123456).</param>
        /// <param name="Utm">Ein gültiges <see cref="UTM"/>-Objekt oder null.</param>
        /// <param name="ErrorMessage">Eine ausführliche Fehlermeldung, falls ein Fehler aufgetreten ist.</param>
        /// <param name="ValidItems">Gibt eine Liste der gültigen bzw. ungültigen Parameter zurück.</param>
        /// <returns>True, wenn der String gültig ist, sonst False.</returns>
        static public bool TryParse(string UTMString, out UTM Utm, out string ErrorMessage, out Dictionary<string, bool>ValidItems)
        {
            Dictionary<string, bool> list = new Dictionary<string, bool>();
            string utmstring = UTMString.Trim();
            char[] splitter = { ' ' };
            string[] field = utmstring.Split(splitter);

            if ((field.Length == 3) || (field.Length == 4))
            {
                if (field.Length == 3)
                {
                    if ((field[0].Length == 2) || (field[0].Length == 3))
                    {
                        utmstring = utmstring.Insert(field[0].Length - 1, " ");
                        field = utmstring.Split(splitter);
                    }
                }
                if (field.Length == 4) return TryParse(field[0], field[1], field[2], field[3], out Utm, out ErrorMessage, out ValidItems);
            }

            Utm = null;
            ErrorMessage = new ErrorProvider.ErrorMessage("ERROR_UTM_STRING").Text;
            list.Add("UTMString", false);
            ValidItems = list;
            return false;
        }


        /// <summary><para>Die Funktion formatiert die East-Koordinate (6 Ziffern) und die North-Koordinate (7 Ziffern), 
        /// und schreibt die Werte in die EastString- bzw. NorthString-Eigenschaft der Basisklasse <see cref="Geocentric"/>.
        /// Diese Funktion ist nur für interne Zwecke bestimmt.</para></summary>
        /// 
        /// <param name="target">Der Target-Enumerator legt das zu formatierendes Element fest.</param>
        internal override void Format(Target target)
        {
            string f = "000000";
            if (this.Precision > 0)
            {
                f = "000000.";
                f = f.PadRight(7 + this.Precision, '0');
            }
            if ((target == Target.East) || (target == Target.All)) this.EastString = this.East.ToString(f, CultureInfo.CurrentCulture);
            if ((target == Target.North) || (target == Target.All)) this.NorthString = this.North.ToString("0" + f, CultureInfo.CurrentCulture);
        }


        /// <summary><para>Die Funktion gibt einen kurzen String der UTM Koordinaten zurück.</para></summary>
        /// 
        /// <example>Das folgende Beispiel gibt einen kurzen String zurück:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// UTM utm = new UTM(32, "U", 412345, 5567890);
        /// string output = utm.ToString()               // Ausgabe: "32U 412345 5567890"
        /// </code>
        /// </example>
        /// 
        /// <returns>Zusammengesetzter kurzer String der UTM Koordinaten.</returns>
        public new string ToString()
        {
            if (isEmpty) return "";
            return this.Zoneband + " " + this.East.ToString() + " " + this.North.ToString();
        }


        /// <summary><para>Die Funktion gibt einen kurzen String mit formatierten UTM Koordinaten zurück.</para></summary>
        /// 
        /// <example>Das folgende Beispiel gibt einen kurzen String zurück:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// UTM utm = new UTM(32, "U", 412345, 5567890);
        /// string output = utm.ToShortString()               // Ausgabe: "32U 412345 5567890"
        /// </code>
        /// </example>
        /// 
        /// <returns>Zusammengesetzter kurzer String mit formatierten UTM Koordinaten.</returns>
        public string ToShortString()
        {
            if (isEmpty) return "";
            return this.Zoneband + " " + this.EastString + " " + this.NorthString;
        }


        /// <summary><para>Die Funktion gibt einen kurzen String mit formatierten UTM Koordinaten zurück. Diese Methode ist 
        /// veraltet. Bitte benutzen Sie die Methode <see cref="ToShortString()"/>.</para></summary>
        /// 
        /// <example>Das folgende Beispiel gibt einen kurzen String zurück:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// UTM utm = new UTM(32, "U", 412345, 5567890);
        /// string output = utm.ToFormatString()               // Ausgabe: "32U 412345 5567890"
        /// </code>
        /// </example>
        /// 
        /// <returns>Zusammengesetzter kurzer String mit formatierten UTM Koordinaten.</returns>
        [Obsolete("Diese Methode ist veraltet. Bitte benutzen Sie die ToShortString-Methode.",true)]
        public string ToFormatString()
        {
            return this.ToShortString();
        }


        /// <summary><para>Die Funktion gibt einen langen String mit formatierten UTM Koordinaten zurück.</para></summary>
        /// 
        /// <example>Das folgende Beispiel gibt einen langen String zurück:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// UTM utm = new UTM(32, "U", 412345, 5567890);
        /// string output = utm.ToLongString()           // Ausgabe: "32U E 412345 N 5567890"
        /// </code>
        /// </example>
        /// 
        /// <returns>Zusammengesetzter kurzer String mit formatierten UTM Koordinaten.</returns>
        public override string ToLongString()
        {
            if (isEmpty) return "";
            return this.Zoneband + " E " + this.East.ToString() + " N " + this.North.ToString();
        }


        /// <summary><para>Die Funktion gibt einen langen String mit formatierten UTM Koordinaten zurück.</para></summary>
        /// 
        /// <example>Das folgende Beispiel gibt einen langen String zurück:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// UTM utm = new UTM(32, "U", 412345, 5567890);
        /// string output = utm.ToLongString()           // Ausgabe: "32U E 412345 N 5567890"
        /// </code>
        /// </example>
        /// 
        /// <returns>Zusammengesetzter kurzer String mit formatierten UTM Koordinaten.</returns>
        public string ToLongFormatString()
        {
            if (isEmpty) return "";
            return this.Zoneband + " E " + this.EastString + " N " + this.NorthString;
        }

        #endregion ==================== Methoden ====================




        #region ==================== Operatoren/Typumwandlung ====================

        /// <summary><para>Konvertierungsoperator für die Transformation von <see cref="UTM"/> nach <see cref="MGRS"/>.</para></summary>
        /// 
        /// <example>Das Beispiel konvertiert ein <see cref="UTM"/> Objekt in ein <see cref="MGRS"/> (Military Grid Reference System, UTMREF) Objekt:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// // Erzeugen eines UTM Objektes
        /// UTM utm = new UTM(32, "U", 412345, 5567890);
        /// // Typumwandlung in ein MGRS Objekt
        /// MGRS mgrs = (MGRS)utm; 
        /// </code>
        /// </example>
        /// 
        /// <param name="utm">Das aktuelle <see cref="UTM"/> Objekt.</param>
        /// <returns>Ein <see cref="MGRS"/> (Military Grid Reference System, UTMREF) Objekt.</returns>
        public static explicit operator MGRS(UTM utm)
        {
            return Transform.UTMMGR(utm);
        }


        /// <summary><para>Konvertierungsoperator für die Transformation von <see cref="UTM"/> nach <see cref="Geographic"/>.</para></summary>
        /// 
        /// <example>Das Beispiel konvertiert ein <see cref="UTM"/> Objekt in ein <see cref="Geographic"/> Objekt 
        /// im internationalen <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84">WGS84-Datum</see>:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// // Erzeugen eines UTM Objektes
        /// UTM utm = new UTM(32, "U", 412345, 5567890);
        /// // Typumwandlung in ein Geographic Objekt
        /// Geographic geo = (Geographic)utm; 
        /// </code>
        /// </example>
        /// 
        /// <param name="utm">Das aktuelle <see cref="UTM"/> Objekt.</param>
        /// <returns>Ein <see cref="Geographic"/> Objekt (Längen-/Breitengrad).</returns>
        public static explicit operator Geographic(UTM utm)
        {
            return Transform.UTMWGS(utm);
        }


        /// <summary><para>Konvertierungsoperator für die Transformation von <see cref="UTM"/> nach <see cref="GaussKrueger">Gauss-Krüger</see>.</para></summary>
        /// 
        /// <example>Das Beispiel konvertiert ein <see cref="UTM"/> Objekt in ein <see cref="GaussKrueger"/> Objekt:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// // Erzeugen eines UTM Objektes
        /// UTM utm = new UTM(32, "U", 412345, 5567890);
        /// // Typumwandlung in ein GaussKrueger Objekt
        /// GaussKrueger gauss = (GaussKrueger)utm; 
        /// </code>
        /// </example>
        /// 
        /// <param name="utm">Das aktuelle <see cref="UTM"/> Objekt.</param>
        /// <returns>Ein <see cref="GaussKrueger"/> Objekt.</returns>
        public static explicit operator GaussKrueger(UTM utm)
        {
            Geographic geo = Transform.UTMWGS(utm);
            geo.SetDatum(GeoDatum.Potsdam);
            return Transform.WGSGK(geo);
        }


        /// <summary><para>Konvertierungsoperator für die Transformation von <see cref="UTM"/> in ein <see cref="MapService"/> Objekt.</para></summary>
        /// 
        /// <remarks><para>Das <see cref="MapService"/> Objekt repräsentiert für eine gegebene Koordinate ein dazugehöriges 
        /// Satelliten- bzw. Luftbild eines zu wählenden MapService Providers. MapService Provider sind 
        /// Interdienste die Satelliten-/Luftbilder anbieten. Derzeit werden die Anbieter Google Maps, 
        /// Microsoft Virtual Earth und Yahoo Maps unterstützt.</para></remarks>
        /// <example>Das folgende Beispiel konvertiert ein <see cref="UTM"/> Objekt in ein <see cref="MapService"/> Objekt mit dem 
        /// Provider Google Maps und der Zoomstufe 18:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// UTM utm = new UTM(32, "U", 412345, 5567890);                // Erzeugen eines UTM Objektes
        /// MapService maps = (MapService)utm;                          // Typumwandlung in ein MapService Objekt
        /// maps.MapServer = MapService.Info.MapServer.GoogleMaps;      // enum MapService.Info.MapServer
        /// maps.Zoom = 18;                                             // zulässige Zoomstufen: 1 (min) bis 21 (max)
        /// </code>
        /// </example>
        /// 
        /// <param name="utm">Das aktuelle <see cref="UTM"/> Objekt.</param>
        /// <returns>Ein <see cref="MapService"/> Objekt.</returns>
        public static explicit operator MapService(UTM utm)
        {
            return new MapService(Transform.UTMWGS(utm));
        }


        /// <summary><para>Die generische Methode konvertiert den generischen Typ T in das aktuelle <see cref="UTM"/> Objekt.</para></summary>
        /// 
        /// <example>Das Beispiel konvertiert ein bestehendes <see cref="Geographic "/> Objekt in das aktuelle 
        /// <see cref="UTM"/> Objekt mit Hilfe der generischen Methode <see cref="ConvertFrom{T}(T)">ConvertFrom&lt;T&gt;(T)</see>:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789);
        /// UTM utm;
        /// utm.ConvertFrom&lt;Geographic&gt;(geo);
        /// </code>
        /// </example>
        /// 
        /// <typeparam name="T">Ein aus der Basisklasse <see cref="GeoUtility.GeoSystem.Base.BaseSystem"/> abgeleiteter Typ.</typeparam>
        /// <param name="t">Das zu konvertierende Objekt als generischer Parameter.</param>
        public void ConvertFrom<T>(T t) where T : BaseSystem
        {
            UTM o = null;
            try
            {
                if (typeof(T) == typeof(MGRS)) o = (UTM)(MGRS)(BaseSystem)t;
                else if (typeof(T) == typeof(Geographic)) o = (UTM)(Geographic)(BaseSystem)t;
                else if (typeof(T) == typeof(GaussKrueger)) o = (UTM)(GaussKrueger)(BaseSystem)t;
                else if (typeof(T) == typeof(MapService)) o = (UTM)(MapService)(BaseSystem)t;
                if (o != null)
                {
                    this.Zone = o.Zone;
                    this.Band = o.Band;
                    this.East = o.East;
                    this.North = o.North;
                    o = null;
                }
            }
            catch (Exception ex)
            {
                throw new ErrorProvider.GeoException(new ErrorProvider.ErrorMessage("ERROR_CONVERTFROM"), ex);
            }
        }


        /// <summary><para>Die generische Methode konvertiert ein Objekt aus der Basisklasse 
        /// <see cref="GeoUtility.GeoSystem.Base.BaseSystem"/> in ein <see cref="UTM"/> Objekt.</para></summary>
        /// 
        /// <example>Das Beispiel konvertiert ein bestehendes <see cref="Geographic"/> Objekt in ein neues 
        /// <see cref="UTM"/> Objekt mit Hilfe der generischen Methode <see cref="ConvertTo{T}">ConvertTo&lt;T&gt;</see>:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789);
        /// UTM utm;
        /// utm = geo.ConvertTo&lt;UTM&gt;();
        /// </code>
        /// </example>
        /// 
        /// <typeparam name="T">Ein aus der Basisklasse <see cref="GeoUtility.GeoSystem.Base.BaseSystem"/> abgeleiteter Typ.</typeparam>
        /// <returns>Ein aus der Basisklasse <see cref="GeoUtility.GeoSystem.Base.BaseSystem"/> abgeleiteter Typ.</returns>
        public T ConvertTo<T>() where T : BaseSystem
        {

            if (typeof(T) == typeof(MGRS)) return (T)(BaseSystem)(MGRS)this;
            else if (typeof(T) == typeof(Geographic)) return (T)(BaseSystem)(Geographic)this;
            else if (typeof(T) == typeof(GaussKrueger)) return (T)(BaseSystem)(GaussKrueger)this;
            else if (typeof(T) == typeof(MapService)) return (T)(BaseSystem)(MapService)this;
            else return null;
        }


        /// <summary><para>Erstellt eine flache Kopie des aktuellen Objekts.</para></summary>
        /// <returns>Ein neues <see cref="UTM"/>-Objekt als flache Kopie.</returns>
        public new UTM MemberwiseClone()
        {
            return new UTM(this.Zone, this.Band, this.East, this.North);
        }


        /// <summary>Die Funktion wird aus performancegründen implementiert. Das Einfügen in eine Hashtabelle wird durch die die 
        /// Bereitstellung eines Hashwertes wesentlich beschleunigt.</summary>
        /// <returns>Ein Hashwert.</returns>
        public override int GetHashCode()
        {
            return (int)(this.East / 100) ^ (int)(this.North / 100);
        }


        /// <summary>Die Funktion wird aus performancegründen implementiert.</summary>
        /// <param name="obj">Ein beliebiges Objekt.</param>
        /// <returns>Das übergebene Objekt ist gleich oder nicht.</returns>
        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            UTM utm = (UTM)obj;

            return (this.ToString() == utm.ToString());
        }

        
        /// <summary>Überladener Gleichheitsoperator.</summary>
        /// <param name="utmA">Das erste zu vergleichende Objekt.</param>
        /// <param name="utmB">Das zweite zu vergleichende Objekt.</param>
        /// <returns>True, wenn beide Objekte die gleichen Werte haben. False, wenn die Werte nicht gleich sind.</returns>
        public static bool operator ==(UTM utmA, UTM utmB)
        {
            if (System.Object.ReferenceEquals(utmA, utmB)) return true;           // True, wenn beide null, oder gleiche Instanz.
            if (((object)utmA == null) || ((object)utmB == null)) return false;   // False, wenn ein Objekt null, oder beide nicht null
            return (utmA.ToString() == utmB.ToString());
        }


        /// <summary>Überladener Ungleichheitsoperator.</summary>
        /// <param name="utmA">Das erste zu vergleichende Objekt.</param>
        /// <param name="utmB">Das zweite zu vergleichende Objekt.</param>
        /// <returns>True, wenn beide Objekte mindestens einen unterschiedlichen Wert haben. False, wenn alle Werte gleich sind.</returns>
        public static bool operator !=(UTM utmA, UTM utmB)
        {
            return !(utmA == utmB);
        }

        #endregion ==================== Operatoren/Typumwandlung ====================

    }

}
