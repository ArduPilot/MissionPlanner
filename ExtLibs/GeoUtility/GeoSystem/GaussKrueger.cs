//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/GeoSystem/GaussKrueger.cs $
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
// File description   : Definition der Gauss-Krüger Klasse
//===================================================================================================


using System;
using System.Globalization;
using GeoUtility.GeoSystem.Base;
using GeoUtility.GeoSystem.Helper;
using System.Collections.Generic;
using System.Text;

namespace GeoUtility.GeoSystem
{

    /// <summary><para>Definition der <see cref="GaussKrueger"/>-Koordinaten Klasse.</para></summary>
    /// <remarks><para>
    /// Die <see cref="GaussKrueger"/> Klasse implementiert Methoden und Eigenschaften für das nur in Deutschland 
    /// verwendete <see cref="GeoUtility.GeoSystem.GaussKrueger">Gauss-Krüger</see> Koordinatensystem.
    /// </para></remarks>
    public class GaussKrueger : BaseSystem
    {

        #region ==================== Membervariablen ====================

        private double _east;           // Speicherplatz für den Rechtswert
        private double _north;          // Speicherplatz für den Hochwert
        private int _precision = 0;     // Speicherplatz für Nachkommastellen bei Stringausgabe

        #endregion ==================== Membervariablen ====================




        #region ==================== Konstruktoren ====================

        /// <summary><para>Der Standard-Konstruktor.</para></summary>
        /// 
        /// <example>Das folgende Beispiel erzeugt eine Instanz der <see cref="GaussKrueger"/> Klasse und weist 
        /// anschließend neue Werte für den Rechtswert und den Hochwert zu: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// GaussKrueger gauss = new GaussKrueger();
        /// gauss.East = 3456789;
        /// gauss.North = 5612345;
        /// </code>
        /// </example>
        public GaussKrueger() { }


        /// <summary><para>Konstruktor mit Parametern für den Rechts- und Hochwert.</para></summary>
        /// 
        /// <example>Das folgende Beispiel erzeugt eine Instanz der <see cref="GaussKrueger"/> Klasse und übergibt
        /// dabei die Parameter für den Rechtswert und den Hochwert: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// GaussKrueger gauss = new GaussKrueger(3456789, 5612345);
        /// </code>
        /// </example>
        /// 
        /// <param name="east">Rechtswert</param>
        /// <param name="north">Hochwert</param>
        public GaussKrueger(double east, double north)
        {
            this.East = east;
            this.North = north;
        }

        #endregion ==================== Konstruktoren ====================




        #region ==================== Eigenschaften ====================

        /// <summary><para>Die öffentliche Eigenschaft <i>East</i> speichert den Rechtswert, oder 
        /// gibt ihn als Datentyp <i>double</i> zurück. Beispiel siehe auch <see cref="GaussKrueger(double, double)"/>.</para></summary>
        public double East { get { return _east; } set { _east = value; } }


        /// <summary><para>Die öffentliche Eigenschaft <i>North</i> speichert den Hochwert, oder 
        /// gibt ihn als Datentyp <i>double</i> zurück. Beispiel siehe auch <see cref="GaussKrueger(double, double)"/>.</para></summary>
        public double North { get { return _north; } set { _north = value; } }


        /// <summary><para>Die öffentliche Eigenschaft <i>EastString</i> gibt den Rechtswert als formatierten Wert vom
        /// Datentyp <i>string</i> zurück. Beispiel siehe auch <see cref="GaussKrueger(double, double)"/>.</para></summary>
        public string EastString { get { return this.Format(this.East); } }


        /// <summary><para>Die öffentliche Eigenschaft <i>NorthString</i> gibt den Hochwert als formatierten Wert vom
        /// Datentyp <i>string</i> zurück. Beispiel siehe auch <see cref="GaussKrueger(double, double)"/>.</para></summary>
        public string NorthString { get { return this.Format(this.North); } }


        /// <summary><para>Bestimmt die Anzahl der Nachkommastellen in der formatierten Ausgabe.</para></summary>
        public int Precision { get { return _precision; } set { _precision = value; } }


        /// <summary><para>Prüft ob bereits Koordinatenwerte gesetzt wurden.</para></summary>
        public bool isEmpty { get { return ((this.East == 0d) && (this.North == 0d)); } }

        #endregion ==================== Eigenschaften ====================




        #region ==================== Methoden ====================

        /// <summary><para>Die statische Methode prüft, ob die Koordinaten des übergebenen <see cref="Geographic"/>-Objekts
        /// in also in Deutschland liegen. Das Gauss-Krüger Koordinatensystem ist nur für Deutschland definiert.</para></summary>
        /// <param name="geo">Ein gültiges <see cref="Geographic"/>-Objekt.</param>
        /// <returns>True, wenn die Koordinaten in Deutschland liegen, sonst False.</returns>
        public static bool ValidRange(Geographic geo)
        {
            if (geo == null) return false;

            if ((geo.Longitude < 5.0) || (geo.Longitude > 16.0) || (geo.Latitude < 46.0) || (geo.Latitude > 56.0)) return false;
            else return true;
        }

        /// <summary><para>Die statische Methode prüft, ob die Koordinaten des übergebenen <see cref="BaseSystem"/>-Objekts
        /// in also in Deutschland liegen. Das Gauss-Krüger Koordinatensystem ist nur für Deutschland definiert.</para></summary>
        /// <param name="system">Ein gültiges Objekt einer von <see cref="BaseSystem"/> abgeleiteten Klasse.</param>
        /// <returns>True, wenn die Koordinaten in Deutschland liegen, sonst False.</returns>
        public static bool ValidRange(BaseSystem system)
        {
            bool result = false;
            try
            {
                Geographic geo = null;
                if (system == null) return false;
                else if (system.GetType() == typeof(Geographic)) geo = (Geographic)system;
                else if (system.GetType() == typeof(UTM)) geo = (Geographic)(UTM)system;
                else if (system.GetType() == typeof(MGRS)) geo = (Geographic)(MGRS)system;
                else if (system.GetType() == typeof(GaussKrueger)) return true;
                if (geo == null) return false;

                if ((geo.Longitude < 5.0) || (geo.Longitude > 16.0) || (geo.Latitude < 46.0) || (geo.Latitude > 56.0)) result = false;
                else result = true;
            }
            catch (Exception) { }
            return result;
        }


        /// <summary><para>Die statische Methode kann dazu verwendet werden, als String-Werte übergebene Rechts- und Hochwert-Parameter 
        /// auf ihre Gültigkeit zu überprüfen. Die Methode gibt eine Liste gültiger Parameter, eine Fehlermeldung und 
        /// ein <see cref="GaussKrueger"/>-Objekt zurück. Ist einer der Parameter ungültig, wird ein <see cref="GaussKrueger"/>-Objekt 
        /// mir dem Wert null zurückgegeben.</para></summary>
        /// <param name="Rechts">Längengrad-Wert als Typ <see cref="System.String"/>.</param>
        /// <param name="Hoch">Breitengrad-Wert als Typ <see cref="System.String"/>.</param>
        /// <param name="Gauss">Ein gültiges <see cref="GaussKrueger"/>-Objekt oder null.</param>
        /// <param name="ErrorMessage">Eine ausführliche Fehlermeldung, falls ein Fehler aufgetreten ist.</param>
        /// <param name="ValidItems">Ein <see cref="System.Collections.Generic.Dictionary{T, T}"/>-Objekt, in dem die gültigen und ungültigen Parameter aufgeführt werden.</param>
        /// <returns>True, wenn alle Parameter gültig sind, sonst False.</returns>
        public static bool TryParse(string Rechts, string Hoch, out GaussKrueger Gauss, out string ErrorMessage, out Dictionary<string, bool> ValidItems)
        {
            bool result = true;
            bool converted = true;
            StringBuilder sb = new StringBuilder();
            GaussKrueger gauss = null;
            Dictionary<string, bool> list = new Dictionary<string, bool>();
            double rechts = 0.0;
            double hoch = 0.0;
            Rechts = Rechts.Replace('.', ',');
            Hoch = Hoch.Replace('.', ',');

            try { rechts = double.Parse(Rechts); } catch { converted = false; }
            if ((rechts < 2422500.0) || (rechts > 5570000.0)) converted = false;
            list.Add("East", converted);
            if (list["East"] == false)
            {
                sb.Append(new ErrorProvider.ErrorMessage("ERROR_GAUSS_EAST").Text + "\r\n");
                result = false;
            }
            converted = true;

            try { hoch = double.Parse(Hoch); } catch { converted = false; }
            if ((hoch < 5100000.0) || (hoch > 6200000.0)) converted = false;
            list.Add("North", converted);
            if (list["North"] == false)
            {
                sb.Append(new ErrorProvider.ErrorMessage("ERROR_GAUSS_NORTH").Text + "\r\n");
                result = false;
            }

            if (result == true)
            {
                gauss = new GaussKrueger(rechts, hoch);
                try
                {
                    Geographic geo = (Geographic)gauss;
                    if ((geo.Longitude < 5.0) || (geo.Longitude > 16.0))
                    {
                        sb.Append(new ErrorProvider.ErrorMessage("ERROR_GAUSS_EAST").Text + "\r\n");
                        list["East"] = false;
                        result = false;
                    }
                    if ((geo.Latitude < 46.0) || (geo.Latitude > 56.0))
                    {
                        sb.Append(new ErrorProvider.ErrorMessage("ERROR_GAUSS_NORTH").Text + "\r\n");
                        list["North"] = false;
                        result = false;
                    }
                }
                catch
                {
                    gauss = null;
                    result = false;
                    sb.Append(new ErrorProvider.ErrorMessage("ERROR_GK_OUT_OF_RANGE").Text + "\r\n");
                }
            }

            Gauss = gauss;
            ErrorMessage = sb.ToString();
            ValidItems = list;
            return result;
        }
        
        /// <summary><para>Die Funktion gibt einen formatierten String des übergebenen double Wertes zurück.</para></summary>
        /// 
        /// <remarks><para>Diese Funktion ist nur für interne Zwecke bestimmt. Sie wird von den öffentlichen 
        /// Eigenschaften <see cref="EastString"/> und <see cref="NorthString"/> benutzt. Die Anzahl der Nachkommastellen 
        /// wird von der Eigenschaft <see cref="Precision"/> festgelegt. Standardmäßig werden keine Nachkommastellen 
        /// ausgegeben, das heißt die Ausgabe ist <i>metergenau</i>.</para></remarks>
        /// <param name="value">Koordinate (Double)</param>
        /// <returns>Formatierte Koordinate entsprechend dem System</returns>
        private string Format(double value)
        {
            string digits = "0000000";
            if (this.Precision > 0)
            {
                digits = "0000000.";
                digits = digits.PadRight(8 + this.Precision, '0');
            }

            return value.ToString(digits, CultureInfo.CurrentCulture);
        }


        /// <summary><para>Die Funktion gibt einen String der Koordinaten zurück.</para></summary>
        /// 
        /// <example>Das folgende Beispiel gibt einen kurzen String zurück:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// GaussKrueger gauss = new GaussKrueger(3456789, 5612345);
        /// string output = gauss.ToString()               // Ausgabe: "R: 3456789  H: 5612345"
        /// </code>
        /// </example>
        /// 
        /// <returns>Zusammengesetzter kurzer String der Gauss-Krueger Koordinaten.</returns>
        public new string ToString()
        {
            if (isEmpty) return "";
            return string.Format(new Localizer.Message("GAUSS_SHORT_STRING").Text, this.EastString, this.NorthString);
        }

        
        /// <summary><para>Die Funktion gibt einen kurzen String der Koordinaten zurück.</para></summary>
        /// 
        /// <example>Das folgende Beispiel gibt einen kurzen String zurück:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// GaussKrueger gauss = new GaussKrueger(3456789, 5612345);
        /// string output = gauss.ToString()               // Ausgabe: "3456789  5612345"
        /// </code>
        /// </example>
        /// 
        /// <returns>Zusammengesetzter kurzer String der Gauss-Krueger Koordinaten.</returns>
        public string ToShortString()
        {
            if (isEmpty) return "";
            return this.EastString + "  " + this.NorthString;
        }


        /// <summary><para>Die Funktion gibt einen kurzen String der <see cref="GaussKrueger">Gauss-Krüger</see> Koordinaten zurück.</para></summary>
        /// 
        /// <example>Das folgende Beispiel gibt einen langen String zurück:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// GaussKrueger gauss = new GaussKrueger(3456789, 5612345);
        /// string output = gauss.ToLongString()           // Ausgabe: "Rechts: 3456789; Hoch: 5612345"
        /// </code>
        /// </example>
        /// 
        /// <returns>Zusammengesetzter langer String der <see cref="GaussKrueger">Gauss-Krüger</see> Koordinaten.</returns>
        public string ToLongString()
        {
            if (isEmpty) return "";
            return string.Format(new Localizer.Message("GAUSS_LONG_STRING").Text, this.EastString, this.NorthString);
        }

        #endregion ==================== Methoden ====================




        #region ==================== Operatoren/Typumwandlung ====================

        /// <summary><para>Konvertierungsoperator für die Transformation von <see cref="GaussKrueger">Gauss-Krüger</see> 
        /// nach <see cref="Geographic"/>.</para></summary>
        /// 
        /// <example>Das Beispiel konvertiert ein <see cref="GaussKrueger"/> Objekt in ein <see cref="Geographic"/> 
        /// (Längen-/Breitensystem) Objekt im internationalen <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84">WGS84-Datum</see>:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// // Erzeugen eines Gauss-Krueger Objektes
        /// GaussKrueger gauss = new GaussKrueger(3456789, 5612345);
        /// // Typumwandlung in das Längen-/Breiten-Koordinatensystems im WGS84-Datum
        /// Geographic geo = (Geographic)gauss; 
        /// </code>
        /// </example>
        /// 
        /// <param name="gauss">Das aktuelle <see cref="GaussKrueger"/>-Objekt</param>
        /// <returns>Ein <see cref="Geographic"/> Objekt (Länge/Breite, latitude/longitude) im <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84">WGS84-Datum</see></returns>
        public static explicit operator Geographic(GaussKrueger gauss)
        {
            Geographic geo = Transform.GKPOD(gauss);
            geo.SetDatum(GeoDatum.WGS84);
            return geo;
        }


        /// <summary><para>Konvertierungsoperator für die Transformation von <see cref="GaussKrueger">Gauss-Krüger</see> nach <see cref="UTM"/>.</para></summary>
        /// 
        /// <example>Das folgende Beispiel konvertiert ein <see cref="GaussKrueger"/> Objekt in ein <see cref="UTM"/> Objekt:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// // Erzeugen eines Gauss-Krueger Objektes
        /// GaussKrueger gauss = new GaussKrueger(3456789, 5612345);
        /// // Typumwandlung in ein UTM Objekt
        /// UTM utm = (UTM)gauss; 
        /// </code>
        /// </example>
        /// 
        /// <param name="gauss">Das aktuelle <see cref="GaussKrueger"/> Objekt.</param>
        /// <returns>Ein <see cref="UTM"/> Objekt.</returns>
        public static explicit operator UTM(GaussKrueger gauss)
        {
            Geographic geo = Transform.GKPOD(gauss);
            geo.SetDatum(GeoDatum.WGS84);
            return Transform.WGSUTM(geo);
        }


        /// <summary><para>Konvertierungsoperator für die Transformation von <see cref="GaussKrueger">Gauss-Krüger</see> nach 
        /// <see cref="MGRS"/> (Military Grid Reference System, UTMREF).</para></summary>
        /// 
        /// <example>Das folgende Beispiel konvertiert ein <see cref="GaussKrueger"/> Objekt in ein <see cref="MGRS"/> Objekt:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// // Erzeugen eines Gauss-Krueger Objektes
        /// GaussKrueger gauss = new GaussKrueger(3456789, 5612345);
        /// // Typumwandlung in ein MGRS Objekt
        /// MGRS mgrs = (MGRS)gauss; 
        /// </code>
        /// </example>
        /// 
        /// <param name="gauss">Das aktuelle <see cref="GaussKrueger"/> Objekt.</param>
        /// <returns>Ein <see cref="MGRS"/> Objekt.</returns>
        public static explicit operator MGRS(GaussKrueger gauss)
        {
            Geographic geo = Transform.GKPOD(gauss);
            geo.SetDatum(GeoDatum.WGS84);
            UTM utm = Transform.WGSUTM(geo);
            return Transform.UTMMGR(utm);
        }


        /// <summary><para>Konvertierungsoperator für die Transformation von <see cref="GaussKrueger">Gauss-Krüger</see> 
        /// in ein <see cref="MapService"/> Objekt.</para></summary>
        /// <remarks><para>Das <see cref="MapService"/> Objekt repräsentiert für eine gegebene Koordinate ein dazugehöriges 
        /// Satelliten- bzw. Luftbild eines zu wählenden MapService Providers. MapService Provider sind 
        /// Interdienste die Satelliten-/Luftbilder anbieten. Derzeit werden die Anbieter Google Maps, 
        /// Microsoft Virtual Earth und Yahoo Maps unterstützt.</para></remarks>
        /// 
        /// <example>Das folgende Beispiel konvertiert ein <see cref="GaussKrueger"/> Objekt in ein <see cref="MapService"/> 
        /// Objekt mit dem Provider Google Maps und der Zoomstufe 18:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// GaussKrueger gauss = new GaussKrueger(3456789, 5612345); // Erzeugen eines GaussKrueger Objektes
        /// MapService maps = (MapService)gauss; // Typumwandlung in ein MapService Objekt
        /// maps.MapServer = MapService.Info.MapServer.GoogleMaps; // enum MapService.Info.MapServer
        /// maps.Zoom = 18; // zulässige Zoomstufen: 1 (min) bis 21 (max)
        /// </code>
        /// </example>
        /// 
        /// <param name="gauss">Das aktuelle <see cref="GaussKrueger"/> Objekt.</param>
        /// <returns>Ein <see cref="MapService"/> Objekt.</returns>
        public static explicit operator MapService(GaussKrueger gauss)
        {
            Geographic geo = Transform.GKPOD(gauss);
            geo.SetDatum(GeoDatum.WGS84);
            return new MapService(geo);
        }


        /// <summary><para>Die generische Methode konvertiert den generischen Typ T in das aktuelle <see cref="GaussKrueger"/> Objekt.</para></summary>
        /// 
        /// <example>Das Beispiel konvertiert ein bestehendes <see cref="Geographic "/> Objekt in das aktuelle 
        /// <see cref="GaussKrueger"/> Objekt mit Hilfe der generischen Methode <see cref="ConvertFrom{T}(T)">ConvertFrom&lt;T&gt;(T)</see>:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789);
        /// GaussKrueger gauss;
        /// gauss.ConvertFrom&lt;Geographic&gt;(geo);
        /// </code>
        /// </example>
        /// 
        /// <typeparam name="T">Ein aus der Basisklasse <see cref="GeoUtility.GeoSystem.Base.BaseSystem"/> abgeleiteter Typ.</typeparam>
        /// <param name="t">Das zu konvertierende Objekt als generischer Parameter.</param>
        public void ConvertFrom<T>(T t) where T : BaseSystem
        {
            GaussKrueger gauss = null;
            try
            {
                if (typeof(T) == typeof(UTM)) gauss = (GaussKrueger)(UTM)(BaseSystem)t;
                else if (typeof(T) == typeof(Geographic)) gauss = (GaussKrueger)(Geographic)(BaseSystem)t;
                else if (typeof(T) == typeof(MGRS)) gauss = (GaussKrueger)(MGRS)(BaseSystem)t;
                else if (typeof(T) == typeof(MapService)) gauss = (GaussKrueger)(MapService)(BaseSystem)t;
                if (gauss != null)
                {
                    this.East = gauss.East;
                    this.North = gauss.North;
                    gauss = null;
                }
            }
            catch (System.Exception ex)
            {
                throw new ErrorProvider.GeoException(new ErrorProvider.ErrorMessage("ERROR_CONVERTFROM"), ex);
            }
        }


        /// <summary><para>Die generische Methode konvertiert ein Objekt aus der Basisklasse 
        /// <see cref="GeoUtility.GeoSystem.Base.BaseSystem"/> in ein <see cref="GaussKrueger"/> Objekt.</para></summary>
        /// 
        /// <example>Das Beispiel konvertiert ein bestehendes <see cref="Geographic"/> Objekt in ein neues 
        /// <see cref="GaussKrueger"/> Objekt mit Hilfe der generischen Methode <see cref="ConvertTo{T}">ConvertTo&lt;T&gt;</see>:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789);
        /// GaussKrueger gauss;
        /// gauss = geo.ConvertTo&lt;GaussKrueger&gt;();
        /// </code>
        /// </example>
        /// 
        /// <typeparam name="T">Ein aus der Basisklasse <see cref="GeoUtility.GeoSystem.Base.BaseSystem"/> abgeleiteter Typ.</typeparam>
        /// <returns>Ein aus der Basisklasse <see cref="GeoUtility.GeoSystem.Base.BaseSystem"/> abgeleiteter Typ.</returns>
        public T ConvertTo<T>() where T : BaseSystem
        {

            if (typeof(T) == typeof(UTM)) return (T)(BaseSystem)(UTM)this;
            else if (typeof(T) == typeof(Geographic)) return (T)(BaseSystem)(Geographic)this;
            else if (typeof(T) == typeof(MGRS)) return (T)(BaseSystem)(MGRS)this;
            else if (typeof(T) == typeof(MapService)) return (T)(BaseSystem)(MapService)this;
            else return null;
        }


        /// <summary><para>Erstellt eine flache Kopie des aktuellen Objekts.</para></summary>
        /// <returns>Ein neues <see cref="GaussKrueger"/>-Objekt als flache Kopie.</returns>
        public new GaussKrueger MemberwiseClone()
        {
            return new GaussKrueger(this.East, this.North);
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
            GaussKrueger gauss = (GaussKrueger)obj;

            return ((this.East == gauss.East) && (this.North == gauss.North));
        }

        
        /// <summary>Überladener Gleichheitsoperator.</summary>
        /// <param name="gaussA">Das erste zu vergleichende Objekt.</param>
        /// <param name="gaussB">Das zweite zu vergleichende Objekt.</param>
        /// <returns>True, wenn beide Objekte die gleichen Werte haben. False, wenn die Werte nicht gleich sind.</returns>
        public static bool operator ==(GaussKrueger gaussA, GaussKrueger gaussB)
        {
            if (System.Object.ReferenceEquals(gaussA, gaussB)) return true;           // True, wenn beide null, oder gleiche Instanz.
            if (((object)gaussA == null) || ((object)gaussB == null)) return false;   // False, wenn ein Objekt null, oder beide nicht null
            return ((gaussA.East == gaussB.East) && (gaussA.North == gaussB.North));
        }


        /// <summary>Überladener Ungleichheitsoperator.</summary>
        /// <param name="gaussA">Das erste zu vergleichende Objekt.</param>
        /// <param name="gaussB">Das zweite zu vergleichende Objekt.</param>
        /// <returns>True, wenn beide Objekte mindestens einen unterschiedlichen Wert haben. False, wenn alle Werte gleich sind.</returns>
        public static bool operator !=(GaussKrueger gaussA, GaussKrueger gaussB)
        {
            return !(gaussA == gaussB);
        }

        #endregion ==================== Operatoren/Typumwandlung ====================

    }

}
