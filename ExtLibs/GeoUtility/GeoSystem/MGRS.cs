//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/GeoSystem/MGRS.cs $
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
// File description   : Definition der MGRS Klasse (Military Grid Reference System, UTMREF)
//===================================================================================================


using System;
using GeoUtility.GeoSystem.Base;
using GeoUtility.GeoSystem.Helper;
using System.Collections.Generic;
using System.Text;

namespace GeoUtility.GeoSystem
{

    /// <summary><para>MGRS Koordinaten, von der Basisklasse Geocentric abgeleitet.</para></summary>
    /// <remarks><para>
    /// Die <see cref="MGRS"/>-Klasse implementiert Methoden und Eigenschaften für das in der NATO  
    /// verwendete MGRS (Military Grid Reference System, UTMREF).
    /// </para></remarks>
    public class MGRS : Geocentric
    {

        #region ==================== Membervariablen ====================

        private string _grid = "";                                 // Speicherplatz für das Planquadrat im MGRS

        #endregion ==================== Membervariablen ====================




        #region ==================== Konstruktoren ====================

        /// <summary><para>Der Standard-Konstruktor.</para></summary>
        /// 
        /// <example>Das folgende Beispiel erzeugt eine Instanz der <see cref="MGRS"/>-Klasse und weist 
        /// anschließend neue Werte für <see cref="Geocentric.Zone"/>, <see cref="Geocentric.Band"/>, <see cref="Grid"/>, 
        /// <see cref="Geocentric.East"/> und <see cref="Geocentric.North"/> zu: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// MGRS mgrs = new MGRS();
        /// mgrs.Zone = 32;
        /// mgrs.Band = "U";
        /// mgrs.Grid = "MA";
        /// mgrs.East = 12345;
        /// mgrs.North = 67890;
        /// </code>
        /// </example>
        public MGRS() { }


        /// <summary><para>Konstruktor mit zusammengesetzter MGRS Koordinate.</para></summary>
        /// 
        /// <example>Das folgende Beispiel erzeugt eine Instanz der <see cref="MGRS"/>-Klasse und übergibt als Parameter
        /// eine zusammengesetzte MGRS Koordinate:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// MGRS mgrs = new MGRS("32UMA1234567890");
        /// </code>
        /// </example>
        /// 
        /// <param name="mgrs">Zusammengesetze MGRS Koordinate (z. B. 32UMN1234567890)</param>
        public MGRS(string mgrs)
        {
            string error;
            try
            {
                MGRS Mgrs = new MGRS();
                if (MGRS.TryParse(mgrs, out Mgrs, out error) == false)
                {
                    throw new Exception(error);
                }
                else
                {
                    this.Zone = Mgrs.Zone;
                    this.Band = Mgrs.Band;
                    this.Grid = Mgrs.Grid;
                    this.East = Mgrs.East;
                    this.North = Mgrs.North;
                }
            }
            catch (Exception ex)
            {
                throw new ErrorProvider.GeoException(new ErrorProvider.ErrorMessage("ERROR_MGRS"), ex);
            }
        }

        /// <summary><para>Konstruktor mit Parametern für <see cref="Geocentric.Zone"/>, <see cref="Geocentric.Band"/>, 
        /// <see cref="Grid"/>, <see cref="Geocentric.East"/> und <see cref="Geocentric.North"/>.</para></summary>
        /// 
        /// <example>Das folgende Beispiel erzeugt eine Instanz der <see cref="MGRS"/>-Klasse und übergibt als Parameter 
        /// die Werte für <see cref="Geocentric.Zone"/>, <see cref="Geocentric.Band"/>, <see cref="Grid"/>, 
        /// <see cref="Geocentric.East"/> und <see cref="Geocentric.North"/>: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// UTM utm = new UTM(32, "U", "MA", 12345, 67890);
        /// </code>
        /// </example>
        /// 
        /// <param name="zone">Zone</param>
        /// <param name="band">Band</param>
        /// <param name="grid">Planquadrat</param>
        /// <param name="east">East Wert</param>
        /// <param name="north">North Wert</param>
        public MGRS(int zone, string band, string grid, double east, double north)
        {
            this.Zone = zone;
            this.Band = band;
            this.Grid = grid;

            string eastvalue = (Math.Round(east)).ToString();
            string northvalue = (Math.Round(north)).ToString();

            if (eastvalue.Length < northvalue.Length)
            {
                eastvalue = eastvalue.PadLeft(northvalue.Length, '0');
            }
            else if (northvalue.Length < eastvalue.Length)
            {
                northvalue = northvalue.PadLeft(eastvalue.Length, '0');
            }
            // TODO: Hier wurde von PadRight entfernt, bitte testen! 
            //eastvalue = eastvalue.PadRight(5, '0');
            //northvalue = northvalue.PadRight(5, '0');

            this.East = double.Parse(eastvalue);
            this.North = double.Parse(northvalue);
        }

        #endregion ==================== Konstruktoren ====================




        #region ==================== Eigenschaften ====================

        /// <summary><para>Die Eigenschaft Grid legt das Planquadrat im MGRS fest. 
        /// Beispiel <see cref="MGRS()"/></para></summary>
        public string Grid { get { return _grid; } set { _grid = value; } }


        /// <summary><para>Prüft ob bereits Koordinatenwerte gesetzt wurden.</para></summary>
        public bool isEmpty { get { return ((this.Zone == 0) && (this.Band == "") && (this.Grid == "") && (this.East == 0d) && (this.North == 0d)); } }

        #endregion ==================== Eigenschaften ====================




        #region ==================== Methoden ====================


        /// <summary><para>Die statische Methode kann dazu verwendet werden, einen zusammengesetzten MGRS-String 
        /// auf seine Gültigkeit zu überprüfen. eine Fehlermeldung und 
        /// ein <see cref="MGRS"/>-Objekt zurück. Ist einer der Parameter ungültig, wird ein <see cref="MGRS"/>-Objekt 
        /// mir dem Wert null zurückgegeben.</para></summary>
        /// <param name="mstring">MGRS Koordinatenstring als Typ <see cref="System.String"/>.</param>
        /// <param name="Mgrs">Ein gültiges <see cref="MGRS"/>-Objekt oder null.</param>
        /// <param name="ErrorMessage">Eine ausführliche Fehlermeldung, falls ein Fehler aufgetreten ist.</param>
        /// <returns>True, wenn alle Parameter gültig sind, sonst False.</returns>
        static public bool TryParse(string mstring, out MGRS Mgrs, out string ErrorMessage)
        {
            string message = "";
            MGRS mgrs = new MGRS();
            bool result = true;

            try
            {
                mstring = mstring.Replace(" ", "");
                mstring = mstring.ToUpper();

                // Mindestens Zone (1), Band (1), Grid (2), East (1), North (1) = 6
                if (mstring.Length < 6) throw new ErrorProvider.GeoException(new ErrorProvider.ErrorMessage("ERROR_MGRS"));

                int zone = 0;
                // Test auf 1 oder 2-stellige Zonenangabe

                if (char.IsDigit(Convert.ToChar(mstring.Substring(0, 1))))
                {
                    if (char.IsDigit(Convert.ToChar(mstring.Substring(1, 1))))
                    {
                        zone = int.Parse(mstring.Substring(0, 2));
                        mstring = mstring.Remove(0, 2);
                    }
                    else
                    {
                        zone = int.Parse(mstring.Substring(0, 1));
                        mstring = mstring.Remove(0, 1);
                    }
                }
                else
                {
                    throw new ErrorProvider.GeoException(new ErrorProvider.ErrorMessage("ERROR_MGRS"));
                }
                mgrs.Zone = zone;

                if (mstring.Length < 5) throw new ErrorProvider.GeoException(new ErrorProvider.ErrorMessage("ERROR_MGRS"));

                char band = mstring.Substring(0, 1).ToCharArray()[0];
                if ((band < 'C') || (band > 'X') || (band == 'I') || (band == 'O')) throw new ErrorProvider.GeoException(new ErrorProvider.ErrorMessage("ERROR_MGRS_GRID"));
                mgrs.Band = band.ToString();
                mstring = mstring.Remove(0, 1);

                string grid = mstring.Substring(0, 2);
                char c0 = grid.ToCharArray()[0];
                char c1 = grid.ToCharArray()[1];
                if ((c0 < 'A') || (c0 >= 'Z') || (c1 < 'A') || (c1 >= 'Z') || (c0 == 'I') || (c0 == 'O') || (c1 == 'I') || (c1 == 'O')) throw new ErrorProvider.GeoException(new ErrorProvider.ErrorMessage("ERROR_MGRS_GRID"));
                mgrs.Grid = grid;
                mstring = mstring.Remove(0, 2);

                if (mstring.Length % 2 > 0) throw new ErrorProvider.GeoException(new ErrorProvider.ErrorMessage("ERROR_MGRS"));
                int len = mstring.Length / 2;
                string eaststring = mstring.Substring(0, len);
                mstring = mstring.Remove(0, len);
                string northstring = mstring;

                if (eaststring.Length > 5) eaststring = eaststring.Substring(0, 5);
                else if (eaststring.Length < 5) eaststring = eaststring.PadRight(5, '0');
                
                if (northstring.Length > 5) northstring = northstring.Substring(0, 5);
                else if (northstring.Length < 5) northstring = northstring.PadRight(5, '0');

                mgrs.East = double.Parse(eaststring);
                mgrs.North = double.Parse(northstring);
            }
            catch
            {
                mgrs = null;
                result = false;
                message = new ErrorProvider.ErrorMessage("ERROR_MGRS").Text;
            }
            Mgrs = mgrs;
            ErrorMessage = message;
            return result;

        }


        /// <summary><para>Die statische Methode kann dazu verwendet werden, als String-Werte übergebene MGRS Parameter 
        /// auf ihre Gültigkeit zu überprüfen. Die Methode gibt eine Liste gültiger Parameter, eine Fehlermeldung und 
        /// ein <see cref="MGRS"/>-Objekt zurück. Ist einer der Parameter ungültig, wird ein <see cref="MGRS"/>-Objekt 
        /// mir dem Wert null zurückgegeben.</para></summary>
        /// <param name="Zone">Zone als Typ <see cref="System.String"/>.</param>
        /// <param name="Band">Band als Typ <see cref="System.String"/>.</param>
        /// <param name="Grid">Band als Typ <see cref="System.String"/>.</param>
        /// <param name="East">East-Wert als Typ <see cref="System.String"/>.</param>
        /// <param name="North">North-Wert als Typ <see cref="System.String"/>.</param>
        /// <param name="Mgrs">Ein gültiges <see cref="MGRS"/>-Objekt oder null.</param>
        /// <param name="ErrorMessage">Eine ausführliche Fehlermeldung, falls ein Fehler aufgetreten ist.</param>
        /// <param name="ValidItems">Ein <see cref="System.Collections.Generic.Dictionary{T, T}"/>-Objekt, in dem die gültigen und ungültigen Parameter aufgeführt werden.</param>
        /// <returns>True, wenn alle Parameter gültig sind, sonst False.</returns>
        static public bool TryParse(string Zone, string Band, string Grid, string East, string North, out MGRS Mgrs, out string ErrorMessage, out Dictionary<string, bool> ValidItems)
        {
            bool result = true;
            bool converted = true;
            StringBuilder sb = new StringBuilder();
            MGRS mgrs = null;
            Dictionary<string, bool> list = new Dictionary<string, bool>();
            int zone = 0, east = 0, north = 0;
            char band = 'A';
            string grid = Grid;
            Band = Band.ToUpper();
            Grid = Grid.ToUpper();


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
            try { band = Convert.ToChar(Band); }
            catch { converted = false; }
            if ((band < 'C') || (band > 'X') || (band == 'I') || (band == 'O')) converted = false;
            list.Add("Band", converted);
            if (list["Band"] == false)
            {
                sb.Append(new ErrorProvider.ErrorMessage("ERROR_UTM_Band").Text + "\r\n");
                result = false;
            }
            converted = true;

            list.Add("Grid", (Grid.Length == 2));
            if (list["Grid"] == true)
            {
                char c0 = Grid.ToCharArray()[0];
                char c1 = Grid.ToCharArray()[1];

                if ((c0 < 'A') || (c0 >= 'Z') || (c1 < 'A') || (c1 >= 'Z') || (c0 == 'I') || (c0 == 'O') || (c1 == 'I') || (c1 == 'O'))
                {
                    list["Grid"] = false;
                }
            }
            if (list["Grid"] == false) 
            {
                sb.Append(new ErrorProvider.ErrorMessage("ERROR_MGRS_GRID").Text + "\r\n");
                result = false;
            }

            if (East.Length > 5) East = East.Substring(0, 5);
            else if (East.Length < 5) East = East.PadRight(5, '0');
            try { east = int.Parse(East); } catch { converted = false; }
            if ((east < 0) || (east >= 100000)) converted = false;
            list.Add("East", converted);
            if (list["East"] == false)
            {
                sb.Append(new ErrorProvider.ErrorMessage("ERROR_MGRS_EAST").Text + "\r\n");
                result = false;
            }
            converted = true;

            if (North.Length > 5) North = North.Substring(0, 5);
            else if (North.Length < 5) North = North.PadRight(5, '0');
            try { north = int.Parse(North); } catch { converted = false; }
            if ((north < 0) || (north >= 100000)) converted = false;
            list.Add("North", converted);
            if (list["North"] == false)
            {
                sb.Append(new ErrorProvider.ErrorMessage("ERROR_MGRS_NORTH").Text + "\r\n");
                result = false;
            }

            if (result == true) mgrs = new MGRS(zone, band.ToString(), grid, east, north);
            Mgrs = mgrs;
            ErrorMessage = sb.ToString();
            ValidItems = list;
            return result;
        }


        /// <summary><para>Die Funktion gleicht die Länge der East- und North-Koordinate an. Im MGRS müssen beide Werte 
        /// die gleiche Länge haben, um in einem zusammengesetzten Koordinatenstring korrekt interpretiert werden zu können.</para></summary>
        /// <remarks><para>Die genaue Länge ist nicht festgelegt, sondern richtet sich nach der Anzahl der gemeinsamen Nullstellen. 
        /// Die maximale Länge beträgt 5 Ziffern je East-/North-Wert.
        /// <para>Diese Funktion ist nur für interne Zwecke bestimmt.</para></para></remarks>
        /// 
        /// <param name="target">In MGRS fix Target.All</param>
        internal override void Format(Target target)
        {
            string e = Math.Round(this.East).ToString().PadLeft(5, '0');
            string n = Math.Round(this.North).ToString().PadLeft(5, '0');
            while (e.EndsWith("0") && n.EndsWith("0"))
            {
                e = e.Substring(0, e.Length - 1);
                n = n.Substring(0, n.Length - 1);
            }
            this.EastString = e;
            this.NorthString = n;
        }


        /// <summary><para>Die Funktion gibt einen kurzen String der MGRS Koordinaten zurück.</para></summary>
        /// 
        /// <example>Das folgende Beispiel gibt einen kurzen String zurück:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// MGRS mgrs = new MGRS(32, "U", "MA", 12345, 67890);
        /// string output = utm.ToString()                                         // Ausgabe: "32UMA1234567890"
        /// </code>
        /// </example>
        /// 
        /// <returns>Zusammengesetzter kurzer String der MGRS Koordinaten.</returns>
        public new string ToString()
        {
            if (isEmpty) return "";
            return this.Zoneband + this.Grid + this.EastString + this.NorthString;
        }


        /// <summary><para>Die Funktion gibt einen langen String der MGRS Koordinaten zurück.</para></summary>
        /// 
        /// <example>Das folgende Beispiel gibt einen langen String zurück:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// MGRS mgrs = new MGRS(32, "U", "MA", 12345, 67890);
        /// string output = mgrs.ToLongString()                                    // Ausgabe: "32U MA 12345 67890"
        /// </code>
        /// </example>
        /// 
        /// <returns>Zusammengesetzter langer String der MGRS Koordinaten.</returns>
        public override string ToLongString()
        {
            if (isEmpty) return "";
            return this.Zoneband + " " + this.Grid + " " + this.EastString + " " + this.NorthString;
        }

        #endregion ==================== Methoden ====================




        #region ==================== Operatoren/Typumwandlung ====================

        /// <summary><para>Konvertierungsoperator für die Transformation von <see cref="MGRS"/> nach <see cref="UTM"/>.</para></summary>
        /// 
        /// <example>Das Beispiel konvertiert ein <see cref="MGRS"/> Objekt in ein <see cref="UTM"/> Objekt:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// // Erzeugen eines MGRS Objektes
        /// MGRS mgrs = new MGRS(32, "U", "MA", 12345, 67890);
        /// // Typumwandlung in ein UTM Objekt
        /// UTM utm = (UTM)mgrs; 
        /// </code>
        /// </example>
        /// 
        /// <param name="mgrs">Das aktuelle <see cref="MGRS"/> Objekt.</param>
        /// <returns>Ein <see cref="UTM"/> Objekt.</returns>
        public static explicit operator UTM(MGRS mgrs)
        {
            return Transform.MGRUTM(mgrs);
        }


        /// <summary><para>Konvertierungsoperator für die Transformation von <see cref="MGRS"/> nach <see cref="Geographic"/>.</para></summary>
        /// 
        /// <example>Das Beispiel konvertiert ein <see cref="MGRS"/> Objekt in ein <see cref="Geographic"/> Objekt:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// // Erzeugen eines MGRS Objektes
        /// MGRS mgrs = new MGRS(32, "U", "MA", 12345, 67890);
        /// // Typumwandlung in ein Geographic Objekt
        /// Geographic geo = (Geographic)mgrs; 
        /// </code>
        /// </example>
        /// 
        /// <param name="mgrs">Das aktuelle <see cref="MGRS"/> Objekt.</param>
        /// <returns>Ein <see cref="Geographic"/> Objekt (Längen-/Breitengrad, WGS84).</returns>
        public static explicit operator Geographic(MGRS mgrs)
        {
            UTM utm = Transform.MGRUTM(mgrs);
            return Transform.UTMWGS(utm);
        }


        /// <summary><para>Konvertierungsoperator für die Transformation von <see cref="MGRS"/> nach <see cref="GaussKrueger"/>.</para></summary>
        /// 
        /// <example>Das Beispiel konvertiert ein <see cref="MGRS"/> Objekt in ein <see cref="GaussKrueger"/> Objekt:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// // Erzeugen eines MGRS Objektes
        /// MGRS mgrs = new MGRS(32, "U", "MA", 12345, 67890);
        /// // Typumwandlung in ein GaussKrueger Objekt
        /// GaussKrueger gauss = (GaussKrueger)mgrs; 
        /// </code>
        /// </example>
        /// 
        /// <param name="mgrs">Das aktuelle <see cref="MGRS"/> Objekt.</param>
        /// <returns>Ein <see cref="GaussKrueger"/> Objekt (<see cref="GeoUtility.GeoSystem.Helper.GeoDatum.Potsdam">Potsdam-Datum</see>).</returns>
        public static explicit operator GaussKrueger(MGRS mgrs)
        {
            UTM utm = Transform.MGRUTM(mgrs);
            Geographic geo = Transform.UTMWGS(utm);
            geo.SetDatum(GeoDatum.Potsdam);
            return Transform.WGSGK(geo);
        }


        /// <summary><para>Konvertierungsoperator für die Transformation von <see cref="MGRS"/> in ein <see cref="MapService"/> Objekt.</para></summary>
        /// <remarks><para>Das <see cref="MapService"/> Objekt repräsentiert für eine gegebene Koordinate ein dazugehöriges 
        /// Satelliten- bzw. Luftbild eines zu wählenden MapService Providers. MapService Provider sind 
        /// Interdienste die Satelliten-/Luftbilder anbieten. Derzeit werden die Anbieter Google Maps, 
        /// Microsoft Virtual Earth und Yahoo Maps unterstützt. </para></remarks>
        /// 
        /// <example>Das folgende Beispiel konvertiert ein <see cref="MGRS"/> Objekt in ein <see cref="MapService"/> Objekt mit dem 
        /// Provider Google Maps und der Zoomstufe 18:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// MGRS mgrs = new MGRS(32, "U", "MA", 12345, 67890);      // Erzeugen eines MGRS Objektes
        /// MapService maps = (MapService)mgrs;                     // Typumwandlung in ein MapService Objekt
        /// maps.MapServer = MapService.Info.MapServer.GoogleMaps;  // enum MapService.Info.MapServer
        /// maps.Zoom = 18;                                         // zulässige Zoomstufen: 1 (min) bis 21 (max)
        /// </code>
        /// </example>
        /// 
        /// <param name="mgrs">Das aktuelle <see cref="MGRS"/> Objekt.</param>
        /// <returns>Ein <see cref="MapService"/> Objekt.</returns>
        public static explicit operator MapService(MGRS mgrs)
        {
            UTM utm = Transform.MGRUTM(mgrs);                     // Erst Umwandlung nach UTM
            return new MapService(Transform.UTMWGS(utm));
        }


        /// <summary><para>Die generische Methode konvertiert den generischen Typ T in das aktuelle <see cref="MGRS"/> Objekt.</para></summary>
        /// 
        /// <example>Das Beispiel konvertiert ein bestehendes <see cref="Geographic"/> Objekt in das aktuelle 
        /// <see cref="MGRS"/> Objekt mit Hilfe der generischen Methode <see cref="ConvertFrom{T}(T)">ConvertFrom&lt;T&gt;(T)</see>:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789);
        /// MGRS mgrs;
        /// mgrs.ConvertFrom&lt;Geographic&gt;(geo);
        /// </code>
        /// </example>
        /// 
        /// <typeparam name="T">Ein aus der Basisklasse <see cref="GeoUtility.GeoSystem.Base.BaseSystem"/> abgeleiteter Typ.</typeparam>
        /// <param name="t">Das zu konvertierende Objekt als generischer Parameter.</param>
        public void ConvertFrom<T>(T t) where T : BaseSystem
        {
            MGRS o = null;
            try
            {
                if (typeof(T) == typeof(UTM)) o = (MGRS)(UTM)(BaseSystem)t;
                else if (typeof(T) == typeof(Geographic)) o = (MGRS)(Geographic)(BaseSystem)t;
                else if (typeof(T) == typeof(GaussKrueger)) o = (MGRS)(GaussKrueger)(BaseSystem)t;
                else if (typeof(T) == typeof(MapService)) o = (MGRS)(MapService)(BaseSystem)t;
                if (o != null)
                {
                    this.Zone = o.Zone;
                    this.Band = o.Band;
                    this.Grid = o.Grid;
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
        /// <see cref="GeoUtility.GeoSystem.Base.BaseSystem"/> in ein <see cref="MGRS"/> Objekt.</para></summary>
        /// 
        /// <example>Das Beispiel konvertiert ein bestehendes <see cref="Geographic"/> Objekt in ein neues 
        /// <see cref="MGRS"/> Objekt mit Hilfe der generischen Methode <see cref="ConvertTo{T}">ConvertTo&lt;T&gt;</see>:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789);
        /// MGRS mgrs;
        /// mgrs = geo.ConvertTo&lt;MGRS&gt;();
        /// </code>
        /// </example>
        /// 
        /// <typeparam name="T">Ein aus der Basisklasse <see cref="GeoUtility.GeoSystem.Base.BaseSystem"/> abgeleiteter Typ.</typeparam>
        /// <returns>Ein aus der Basisklasse <see cref="GeoUtility.GeoSystem.Base.BaseSystem"/> abgeleiteter Typ.</returns>
        public T ConvertTo<T>() where T : BaseSystem
        {

            if (typeof(T) == typeof(UTM)) return (T)((BaseSystem)((UTM)this));
            else if (typeof(T) == typeof(Geographic)) return (T)((BaseSystem)((Geographic)this));
            else if (typeof(T) == typeof(GaussKrueger)) return (T)((BaseSystem)((GaussKrueger)this));
            else if (typeof(T) == typeof(MapService)) return (T)((BaseSystem)((MapService)this));
            else return null;
        }

        /// <summary><para>Erstellt eine flache Kopie des aktuellen Objekts.</para></summary>
        /// <returns>Ein neues <see cref="MGRS"/>-Objekt als flache Kopie.</returns>
        public new MGRS MemberwiseClone()
        {
            return new MGRS(this.Zone, this.Band, this.Grid, this.East, this.North);
        }


        /// <summary>Die Funktion wird aus performancegründen implementiert. Das Einfügen in eine Hashtabelle wird durch die die 
        /// Bereitstellung eines Hashwertes wesentlich beschleunigt.</summary>
        /// <returns>Ein Hashwert.</returns>
        public override int GetHashCode()
        {
            return (int)(this.East) ^ (int)(this.North);
        }


        /// <summary>Die Funktion wird aus performancegründen implementiert.</summary>
        /// <param name="obj">Ein beliebiges Objekt.</param>
        /// <returns>Das übergebene Objekt ist gleich oder nicht.</returns>
        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            MGRS mgrs = (MGRS)obj;

            return (this.ToString() == mgrs.ToString());
        }

        
        /// <summary>Überladener Gleichheitsoperator.</summary>
        /// <param name="mgrsA">Das erste zu vergleichende Objekt.</param>
        /// <param name="mgrsB">Das zweite zu vergleichende Objekt.</param>
        /// <returns>True, wenn beide Objekte die gleichen Werte haben. False, wenn die Werte nicht gleich sind.</returns>
        public static bool operator ==(MGRS mgrsA, MGRS mgrsB)
        {
            if (System.Object.ReferenceEquals(mgrsA, mgrsB)) return true;           // True, wenn beide null, oder gleiche Instanz.
            if (((object)mgrsA == null) || ((object)mgrsB == null)) return false;   // False, wenn ein Objekt null, oder beide nicht null
            return (mgrsA.ToString() == mgrsB.ToString());
        }


        /// <summary>Überladener Ungleichheitsoperator.</summary>
        /// <param name="mgrsA">Das erste zu vergleichende Objekt.</param>
        /// <param name="mgrsB">Das zweite zu vergleichende Objekt.</param>
        /// <returns>True, wenn beide Objekte mindestens einen unterschiedlichen Wert haben. False, wenn alle Werte gleich sind.</returns>
        public static bool operator !=(MGRS mgrsA, MGRS mgrsB)
        {
            return !(mgrsA == mgrsB);
        }

        #endregion ==================== Operatoren/Typumwandlung ====================

    }

}
