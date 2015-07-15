//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/GeoSystem/Geographic.cs $
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
// File description   : Definition der Geographic Klasse (Längen-/Breitengrad-Koordinatensystem)
//===================================================================================================


using System;
using System.Globalization;
using GeoUtility.GeoSystem.Base;
using GeoUtility.GeoSystem.Helper;
using System.Collections.Generic;
using System.Text;

namespace GeoUtility.GeoSystem
{

    /// <summary><para>Definition der <see cref="Geographic"/>-Klasse (Längen-/Breiten-System).</para></summary>
    /// <remarks><para>
    /// Die <see cref="Geographic"/>-Klasse implementiert Methoden und Eigenschaften für das 
    /// Längen-/Breiten-Koordinatensystem (longitude/latitude) im internationalen 
    /// <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84">WGS84-Datum</see>.
    /// </para></remarks>
    public class Geographic : BaseSystem
    {

        #region ==================== Membervariablen ====================

        private double _longtude;                       // Speicherplatz für die geographische Länge
        private double _latitude;                       // Speicherplatz für die geographische Breite
        private GeoDatum _datum = GeoDatum.WGS84;       // Speicherplatz für das geodätische Datum

        #endregion ==================== Membervariablen ====================




        #region ==================== Konstruktoren ====================

        /// <summary><para>Der Standard-Konstruktor.</para></summary>
        /// 
        /// <example>Das folgende Beispiel erzeugt eine Instanz der <see cref="Geographic"/>-Klasse und weist 
        /// anschließend neue Werte für den Längen- und Breitengrad zu:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic();
        /// geo.Longitude = 8.12345;    // Längengrad
        /// geo.Latitude = 50.56789;    // Breitengrad
        /// </code>
        /// </example>
        public Geographic() { }


        /// <summary><para>Konstruktor mit Parametern für den Längen- und Breitengrad.</para></summary>
        /// 
        /// <example>Das folgende Beispiel erzeugt eine Instanz der <see cref="Geographic"/>-Klasse und übergibt
        /// dabei die Parameter für den Längen- und Breitengrad: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789);
        /// </code>
        /// </example>
        /// 
        /// <param name="lon">Geographische Länge (<see cref="Longitude"/>) in Dezimalgrad.</param>
        /// <param name="lat">Geographische Breite (<see cref="Latitude"/>) in Dezimalgrad.</param>
        public Geographic(double lon, double lat)
        {
            this.Longitude = lon;
            this.Latitude = lat;
            this.Datum = GeoDatum.WGS84;                // Standardwert ist WGS84
        }


        /// <summary><para>Konstruktor mit Parametern für den Längen- und Breitengrad und das 
        /// <see cref="GeoUtility.GeoSystem.Helper.GeoDatum">Datum</see>.</para></summary>
        /// 
        /// <example>Das folgende Beispiel erzeugt eine Instanz der GeoUtility.GeoSystem.Helper Klasse und übergibt
        /// dabei die Parameter für den <see cref="Longitude">Längengrad</see> und den <see cref="Latitude">Breitengrad</see> 
        /// und das <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.Potsdam">Potsdam-Datum</see> (nur Deutschland). 
        /// Achtung: International wird üblicherweise wird das 
        /// <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84">WGS84-Datum</see> verwendet: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789, GeoDatum.Potsdam);
        /// </code>
        /// </example>
        /// 
        /// <param name="lon">Geographische Länge ((<see cref="Longitude"/>) in Dezimalgrad.</param>
        /// <param name="lat">Geographische Breite (<see cref="Latitude"/>) in Dezimalgrad.</param>
        /// <param name="datum">Geodätisches Datum (<see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84"/> oder <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.Potsdam"/>).</param>
        public Geographic(double lon, double lat, GeoDatum datum)
        {
            this.Longitude = lon;
            this.Latitude = lat;
            this.Datum = datum;
        }

        #endregion ==================== Konstruktoren ====================




        #region ==================== Eigenschaften ====================

        /// <summary><para>Gibt die geographische Länge (Longitude) als Datentyp <i>double</i> zurück oder setzt den Wert.</para></summary>
        /// <remarks><para>Beispiel siehe <see cref="Geographic()">Standard-Konstruktor</see>.</para></remarks>
        public double Longitude { get { return _longtude; } set { _longtude = value; } }


        /// <summary><para>Gibt die geographische Breite (Latitude) als Datentyp <i>double</i> zurück oder setzt den Wert.</para></summary>
        /// <remarks><para>Beispiel siehe <see cref="Geographic()">Standard-Konstruktor</see>.</para></remarks>
        public double Latitude { get { return _latitude; } set { _latitude = value; } }


        /// <summary><para>Gibt die geographische Länge (Longitude) als Datentyp <i>string</i> zurück.</para></summary>
        public string LonString { get { return this.Format(this.Longitude); } }


        /// <summary><para>Gibt die geographische Breite (Latitude) als Datentyp <i>string</i> zurück.</para></summary>
        public string LatString { get { return this.Format(this.Latitude); } }


        /// <summary><para>Gibt ein <see cref="Helper.Sexagesimal">Sexagesimal</see> Objekt (Grad/Minute/Sekunde) 
        /// für die geographische Länge (Longitude) zurück.</para></summary>
        public Helper.Sexagesimal LonSexagesimal { get { return new Helper.Sexagesimal(this.Longitude); } }


        /// <summary><para>Gibt ein <see cref="Helper.Sexagesimal">Sexagesimal</see> Objekt (Grad/Minute/Sekunde) 
        /// für die geographische Breite (Latitude) zurück.</para></summary>
        public Helper.Sexagesimal LatSexagesimal { get { return new Helper.Sexagesimal(this.Latitude); } }


        /// <summary><para>Gibt das aktuelle geodätische Datum (WGS84 oder Potsdam) zurück. 
        /// Standard ist das <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84">WGS84-Datum</see> (World Geodetic System 1984).</para></summary>
        public GeoDatum Datum { get { return _datum; } internal set { _datum = value; } }


        /// <summary><para>Prüft ob bereits Koordinatenwerte gesetzt wurden.</para></summary>
        public bool isEmpty { get { return ((_longtude == 0d) && (_latitude == 0d)); } }

        #endregion ==================== Eigenschaften ====================




        #region ==================== Methoden ====================

        /// <summary><para>Die statische Methode kann dazu verwendet werden, als String-Werte übergebene Längen-/Breitengrad-Parameter 
        /// auf ihre Gültigkeit zu überprüfen. Die Methode gibt eine Liste gültiger Parameter, eine Fehlermeldung und 
        /// ein <see cref="Geographic"/>-Objekt zurück. Ist einer der Parameter ungültig, wird ein <see cref="Geographic"/>-Objekt 
        /// mir dem Wert null zurückgegeben.</para></summary>
        /// <param name="Lon">Längengrad-Wert als Typ <see cref="System.String"/>.</param>
        /// <param name="Lat">Breitengrad-Wert als Typ <see cref="System.String"/>.</param>
        /// <param name="Geograph">Ein gültiges <see cref="Geographic"/>-Objekt oder null.</param>
        /// <param name="ErrorMessage">Eine ausführliche Fehlermeldung, falls ein Fehler aufgetreten ist.</param>
        /// <param name="ValidItems">Ein <see cref="System.Collections.Generic.Dictionary{T, T}"/>-Objekt, in dem die gültigen und ungültigen Parameter aufgeführt werden.</param>
        /// <returns>True, wenn alle Parameter gültig sind, sonst False.</returns>
        static public bool TryParse(string Lon, string Lat, out Geographic Geograph, out string ErrorMessage, out Dictionary<string, bool> ValidItems)
        {
            bool result = true;
            bool converted = true;
            StringBuilder sb = new StringBuilder();
            Geographic geo = null;
            Dictionary<string, bool> list = new Dictionary<string, bool>();
            double lon = 0.0;
            double lat = 0.0;
            Lon = Lon.Replace('.', ',');
            Lat = Lat.Replace('.', ',');

            try { lon = double.Parse(Lon); } catch { converted = false; }
            if ((lon < -180.0) || (lon > +180.0)) converted = false;
            list.Add("Longitude", converted);
            if (list["Longitude"] == false)
            {
                sb.Append(new ErrorProvider.ErrorMessage("ERROR_GEO_LONGITUDE").Text + "\r\n");
                result = false;
            }
            converted = true;

            try { lat = double.Parse(Lat); } catch { converted = false; }
            if ((lat < -80.0) || (lat > 84.0)) converted = false;
            list.Add("Latitude", converted);
            if ((list["Latitude"] == false) || (lat < -80.0) || (lat > 84.0))
            {
                sb.Append(new ErrorProvider.ErrorMessage("ERROR_GEO_LATITUDE").Text + "\r\n");
                result = false;
            }

            if (result == true) geo = new Geographic(lon, lat);
            Geograph = geo;
            ErrorMessage = sb.ToString();
            ValidItems = list;
            return result;
        }



        /// <summary><para>Die Funktion verschiebt das aktuelle <see cref="GeoUtility.GeoSystem.Helper.GeoDatum">geodätische Datum</see>. 
        /// Mögliche Werte sind: <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84">WGS84</see> (Standard, international) oder 
        /// <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.Potsdam">Potsdam-Datum</see>.</para></summary>
        /// 
        /// <example>Das folgende Beispiel berechnet die neuen Koordinatenwerte, die sich durch 
        /// das Setzen des geodätischen Datums vom initialen (internationalen) Datum 
        /// <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84">WGS84</see> in das nur in 
        /// Deutschland verwendete <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.Potsdam">Potsdam-Datum</see> ändern. 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789, GeoDatum.WGS84); // initial WGS84
        /// geo.SetDatum(GeoDatum.Potsdam);                                     // neues Datum
        /// </code>
        /// </example>
        /// 
        /// <param name="datum">Das neu zu setzende <see cref="GeoUtility.GeoSystem.Helper.GeoDatum">geodätische Datum</see>.</param>
        public void SetDatum(GeoDatum datum)
        {
            if (datum != this.Datum)
            {
                this.Datum = datum;
                Geographic g = null;
                if (datum == GeoDatum.Potsdam)
                {
                    g = Transform.WGSPOD(this);
                }
                else 
                {
                    g = Transform.PODWGS(this);
                }
                this.Longitude = g.Longitude;
                this.Latitude = g.Latitude;
            }
        }


        /// <summary><para>Gibt die Koordinate in einem landesspezifischen Format aus. Die Funktion wird von der
        /// Eigenschaft <see cref="LonString"/> und <see cref="LatString"/> verwendet. 
        /// Die Funktion ist nur für den internen Gebrauch bestimmt.</para></summary>
        /// 
        /// <param name="value">Koordinate als Datentyp double</param>
        /// <returns>Landesspezifisch formatierte Koordinate als Datentyp string.</returns>
        private string Format(double value)
        {
            return value.ToString("#0.0#####", CultureInfo.CurrentCulture);
        }


        /// <summary><para>Erstellt einen kurzen geographischen Koordinaten-String.</para></summary>
        /// 
        /// <example>Das folgende Beispiel zeigt eine Ausgebe der Methode: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789); 
        /// geo.ToString();                                     // Ausgabe: "8.12345; 50.56789"
        /// </code>
        /// </example>
        /// 
        /// <returns>Zusammengesetzter kurzer geographischer Koordinaten-String</returns>
        public new string ToString()
        {
            return this.LonString + "; " + this.LatString;
        }


        /// <summary><para>Erstellt einen langen geographischen Koordinaten-String mit Datum.</para></summary>
        /// 
        /// <example>Das folgende Beispiel zeigt eine Ausgebe der Methode: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789); 
        /// geo.ToLongString();                                     // Ausgabe: "Länge: 8.12345; Breite: 50.56789 (WGS84)"
        /// </code>
        /// </example>
        /// 
        /// <returns>Zusammengesetzter langer geographischer Koordinaten-String</returns>
        public string ToLongString()
        {
            return string.Format(new Localizer.Message("GEOGRAPHIC_STRING").Text, this.LonString, this.LatString) + " (" + this.Datum + ")";
        }


        /// <summary><para>Erstellt einen langen geographischen Koordinaten-String mit Datum.</para></summary>
        /// 
        /// <example>Das folgende Beispiel zeigt eine Ausgebe der Methode: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789); 
        /// geo.ToSexagesimalString();
        /// </code>
        /// </example>
        /// 
        /// <returns>Zusammengesetzter langer geographischer Koordinaten-String im <see cref="Helper.Sexagesimal"/>-Format.</returns>
        public string ToSexagesimalString()
        {
            return string.Format(new Localizer.Message("GEOGRAPHIC_STRING").Text, this.LonSexagesimal.ToString(), this.LatSexagesimal.ToString()) + " (" + this.Datum + ")";
        }


        /// <summary><para>Erstellt einen geographischen Koordinaten-String mit Dezimal- und Sexagesimalkoordinaten.</para></summary>
        /// 
        /// <example>Das folgende Beispiel zeigt eine Ausgebe der Methode: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789); 
        /// geo.ToHybridString();
        /// </code>
        /// </example>
        /// 
        /// <returns>Zusammengesetzter geographischer Koordinaten-String mit Dezimal- und Sexagesimalkoordinaten.</returns>
        public string ToHybridString()
        {
            return string.Format(new Localizer.Message("GEOGRAPHIC_STRING").Text, this.LonSexagesimal.ToString() + " (" + this.LonString + ")", this.LatSexagesimal.ToString() + " (" + this.LatString + ")");
        }

        #endregion ==================== Methoden ====================




        #region ==================== Operatoren/Typumwandlung ====================

        /// <summary><para>Konvertierungsoperator für die Transformation von <see cref="Geographic "/> nach 
        /// <see cref="GaussKrueger">Gauss-Krüger</see>. Eine eventuell notwendige Berechnung der Verschiebung 
        /// vom <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84">WGS84-Datum</see> in das 
        /// <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.Potsdam">Potsdam-Datum</see> erfolgt automatisch.</para></summary>
        /// 
        /// <example>Das Beispiel konvertiert ein <see cref="Geographic "/>-Objekt (Längen-/Breitensystem) in ein 
        /// <see cref="GaussKrueger"/> Object:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789); // Erzeugen eines Geographic Objektes
        /// GaussKrueger gauss = (GaussKrueger)geo;           // Typumwandlung in ein GaussKrueger Objekt
        /// </code>
        /// </example>
        /// 
        /// <param name="geo">Zu konvertierendes geographisches Objekt vom Typ <see cref="Geographic"/>.</param>
        /// <returns>Das neue <see cref="GaussKrueger"/> Objekt.</returns>
        public static explicit operator GaussKrueger(Geographic geo)
        {
            if (geo.Datum != GeoDatum.Potsdam)
            {
                geo.SetDatum(GeoDatum.Potsdam);
            }
            return Transform.WGSGK(geo);
        }


        /// <summary><para>Konvertierungsoperator für die Transformation von <see cref="Geographic"/> nach <see cref="UTM"/>.</para></summary>
        /// 
        /// <example>Das Beispiel konvertiert ein <see cref="Geographic "/> Objekt (Längen-/Breitensystem) in ein <see cref="UTM"/> Object:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789); // Erzeugen eines Geographic Objektes
        /// UTM utm = (UTM)geo;                                 // Typumwandlung in ein UTM Objekt
        /// </code>
        /// </example>
        /// 
        /// <param name="geo">Zu konvertierendes geographisches Objekt vom Typ <see cref="Geographic"/>.</param>
        /// <returns>Das neue <see cref="UTM"/> Objekt.</returns>
        public static explicit operator UTM(Geographic geo)
        {
            return Transform.WGSUTM(geo);
        }


        /// <summary><para>Konvertierungsoperator für die Transformation von <see cref="Geographic"/> nach 
        /// <see cref="MGRS"/> (Military Grid Reference System, UTMREF).</para></summary>
        /// 
        /// <example>Das Beispiel konvertiert ein <see cref="Geographic "/> Objekt (Längen-/Breitensystem) in ein <see cref="MGRS"/> Object:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789); // Erzeugen eines Geographic Objektes
        /// MGRS mgrs = (MGRS)geo;                              // Typumwandlung in ein MGRS Objekt
        /// </code>
        /// </example>
        /// 
        /// <param name="geo">Zu konvertierendes geographisches Objekt vom Typ <see cref="Geographic"/>.</param>
        /// <returns>Das neue <see cref="MGRS"/> Objekt.</returns>
        public static explicit operator MGRS(Geographic geo)
        {
            if (geo.Datum != GeoDatum.WGS84)
            {
                geo.SetDatum(GeoDatum.WGS84);
            }
            UTM utm = Transform.WGSUTM(geo);
            return Transform.UTMMGR(utm);
        }


        /// <summary><para>Konvertierungsoperator für die Transformation von <see cref="Geographic"/> in ein <see cref="MapService"/> Objekt.</para></summary>
        /// 
        /// <remarks><para>Das <see cref="MapService"/> Objekt repräsentiert für eine gegebene Koordinate ein dazugehöriges 
        /// Satelliten- bzw. Luftbild eines zu wählenden MapService Providers. MapService Provider sind 
        /// Interdienste die Satelliten-/Luftbilder anbieten. Derzeit werden die Anbieter Google Maps, 
        /// Microsoft Virtual Earth und Yahoo Maps unterstützt.</para></remarks>
        /// <example>Das folgende Beispiel konvertiert ein <see cref="Geographic"/> Objekt in ein <see cref="MapService"/>-Objekt 
        /// mit dem Provider Google Maps und der Zoomstufe 18:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789);     // Erzeugen eines Geographic Objektes
        /// MapService maps = (MapService)geo;                      // Typumwandlung in ein MapService Objekt
        /// maps.MapServer = MapService.Info.MapServer.GoogleMaps;  // enum MapService.Info.MapServer
        /// maps.Zoom = 18;                                         // zulässige Zoomstufen: 1 (min) bis 21 (max)
        /// </code>
        /// </example>
        /// 
        /// <param name="geo">Zu konvertierendes geographisches Objekt vom Typ <see cref="Geographic"/>.</param>
        /// <returns>Das neue <see cref="MapService"/> Objekt.</returns>
        public static explicit operator MapService(Geographic geo)
        {
            if (geo.Datum != GeoDatum.WGS84)
            {
                geo.SetDatum(GeoDatum.WGS84);
            }
            return new MapService(geo);
        }


        /// <summary><para>Die generische Methode konvertiert den generischen Typ T in das aktuelle 
        /// <see cref="Geographic"/> Objekt.</para></summary>
        /// 
        /// <example>Das Beispiel konvertiert ein bestehendes <see cref="UTM"/> Objekt in das aktuelle 
        /// <see cref="Geographic"/> Objekt mit Hilfe der generischen Methode <see cref="ConvertFrom{T}(T)">ConvertFrom&lt;T&gt;(T)</see>:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// UTM utm = new UTM(32, "U", 478123, 5629123);
        /// Geographic geo;
        /// geo.ConvertFrom&lt;UTM&gt;(utm);
        /// </code>
        /// </example>
        /// 
        /// <typeparam name="T">Ein aus der Basisklasse <see cref="GeoUtility.GeoSystem.Base.BaseSystem"/> abgeleiteter Typ.</typeparam>
        /// <param name="t">Das zu konvertierende Objekt als generischer Parameter.</param>
        public void ConvertFrom<T>(T t) where T : BaseSystem
        {
            Geographic o = null;
            try
            {
                if (typeof(T) == typeof(UTM)) o = (Geographic)(UTM)(BaseSystem)t;
                else if (typeof(T) == typeof(GaussKrueger)) o = (Geographic)(GaussKrueger)(BaseSystem)t;
                else if (typeof(T) == typeof(MGRS)) o = (Geographic)(MGRS)(BaseSystem)t;
                else if (typeof(T) == typeof(MapService)) o = (Geographic)(MapService)(BaseSystem)t;
                if (o != null)
                {
                    this.Longitude = o.Longitude;
                    this.Latitude = o.Latitude;
                    this.Datum = o.Datum;
                    o = null;
                }
            }
            catch (Exception ex)
            {
                throw new ErrorProvider.GeoException(new ErrorProvider.ErrorMessage("ERROR_CONVERTFROM"), ex);
            }
        }


        /// <summary><para>Die generische Methode konvertiert ein Objekt aus der Basisklasse 
        /// <see cref="GeoUtility.GeoSystem.Base.BaseSystem"/> in ein <see cref="Geographic "/> Objekt.</para></summary>
        /// 
        /// <example>Das Beispiel konvertiert ein bestehendes <see cref="GaussKrueger">Gauss-Krüger</see> 
        /// Objekt in ein neues <see cref="Geographic"/> Objekt mit Hilfe der generischen Methode <see cref="ConvertTo{T}">ConvertTo&lt;T&gt;</see>:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// GaussKrueger gauss = new GaussKrueger(3456789, 5612345);
        /// Geographic geo;
        /// geo = gauss.ConvertTo&lt;Geographic&gt;();
        /// </code>
        /// </example>
        /// 
        /// <typeparam name="T">Ein aus der Basisklasse <see cref="GeoUtility.GeoSystem.Base.BaseSystem"/> abgeleiteter Typ.</typeparam>
        /// <returns>Ein aus der Basisklasse <see cref="GeoUtility.GeoSystem.Base.BaseSystem"/> abgeleiteter Typ.</returns>
        public T ConvertTo<T>() where T : BaseSystem
        {

            // HINSWEISTEXT SIEHE KLASSE MGRS
            if (typeof(T) == typeof(UTM)) return (T)((BaseSystem)((UTM)this));
            else if (typeof(T) == typeof(GaussKrueger)) return (T)((BaseSystem)((GaussKrueger)this));
            else if (typeof(T) == typeof(MGRS)) return (T)((BaseSystem)((MGRS)this));
            else if (typeof(T) == typeof(MapService)) return (T)((BaseSystem)((MapService)this));
            else return null;
        }

        /// <summary><para>Erstellt eine flache Kopie des aktuellen Objekts.</para></summary>
        /// <returns>Ein neues <see cref="Geographic"/>-Objekt als flache Kopie.</returns>
        public new Geographic MemberwiseClone()
        {
            return new Geographic(this.Longitude, this.Latitude);
        }


        /// <summary>Die Funktion wird aus performancegründen implementiert. Das Einfügen in eine Hashtabelle wird durch die die 
        /// Bereitstellung eines Hashwertes wesentlich beschleunigt.</summary>
        /// <returns>Ein Hashwert.</returns>
        public override int GetHashCode()
        {
            return (int)(this.Longitude * 100) ^ (int)this.Latitude;
        }


        /// <summary>Die Funktion wird aus performancegründen implementiert.</summary>
        /// <param name="obj">Ein beliebiges Objekt.</param>
        /// <returns>Das übergebene Objekt ist gleich oder nicht.</returns>
        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            Geographic geo = (Geographic)obj;

            return ((this.Longitude == geo.Longitude) && (this.Latitude == geo.Latitude));
        }

        
        /// <summary>Überladener Gleichheitsoperator.</summary>
        /// <param name="geoA">Das erste zu vergleichende Objekt.</param>
        /// <param name="geoB">Das zweite zu vergleichende Objekt.</param>
        /// <returns>True, wenn beide Objekte die gleichen Werte haben. False, wenn die Werte nicht gleich sind.</returns>
        public static bool operator ==(Geographic geoA, Geographic geoB)
        {
            if (System.Object.ReferenceEquals(geoA, geoB)) return true;           // True, wenn beide null, oder gleiche Instanz.
            if (((object)geoA == null) || ((object)geoB == null)) return false;   // False, wenn ein Objekt null, oder beide nicht null
            return ((geoA.Longitude == geoB.Longitude) && (geoA.Latitude == geoB.Latitude));
        }


        /// <summary>Überladener Ungleichheitsoperator.</summary>
        /// <param name="geoA">Das erste zu vergleichende Objekt.</param>
        /// <param name="geoB">Das zweite zu vergleichende Objekt.</param>
        /// <returns>True, wenn beide Objekte mindestens einen unterschiedlichen Wert haben. False, wenn alle Werte gleich sind.</returns>
        public static bool operator !=(Geographic geoA, Geographic geoB)
        {
            return !(geoA == geoB);
        }

        #endregion ==================== Operatoren/Typumwandlung ====================


    }

}
