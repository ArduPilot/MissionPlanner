//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/GeoSystem/MapService.cs $
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
// File description   : Definition der MapsService Klasse (Google Maps, Virtual Earth, Yahoo Maps)
//===================================================================================================


using System;
using GeoUtility.GeoSystem.Base;
using GeoUtility.GeoSystem.Helper;

namespace GeoUtility.GeoSystem
{

    /// <summary><para>Definition der <see cref="MapService"/> Klasse.</para></summary>
    /// <remarks><para>
    /// Die <see cref="MapService"/> Klasse implementiert Methoden und Eigenschaften für die Verwendung vom MapService Diensten. 
    /// Ein <see cref="MapService"/> Objekt repräsentiert für eine gegebene Koordinate ein dazugehöriges 
    /// Satelliten- bzw. Luftbild eines zu wählenden MapService Providers. MapService Provider sind 
    /// Interdienste die Satelliten-/Luftbilder anbieten. Derzeit werden die Anbieter Google Maps, 
    /// Microsoft Virtual Earth und Yahoo Maps unterstützt.
    /// </para></remarks>
    public partial class MapService : BaseSystem, IEquatable<MapService>
    {
        #region ==================== Konstanten ====================

        private const int TILE_SIZE = 256;
        private const string USER_AGENT = @"Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2;)";
        private const string GOOGLE_TILE_SERVER = @"http://khm{0}.google.com/kh/v=84&{1}&s=Galileo";
        private const string VIRTUAL_EARTH_TILE_SERVER = @"http://t{0}.tiles.virtualearth.net/tiles/{1}.jpeg?g=1";
        private const string YAHOO_TILE_SERVER = @"http://maps{0}.yimg.com/ae/ximg?v=1.9&t=a&s=256&{1}&r=1";
        
        #endregion ==================== Konstanten ====================




        #region ==================== Membervariablen ====================

        private Info.MapServer _mapserver = Info.MapServer.GoogleMaps;          // Google Maps ist Standard
        private Geographic _geo;                                                // Speicherplatz für Längen-/Breitengrad
        private int _zoom = 18;                                                 // Speicherplatz für Zoom, Standard 18
        private bool _center = false;                                           // Zentriert die Koordinate auf dem Bild

        #endregion ==================== Membervariablen ====================




        #region ==================== Konstruktoren ====================

        /// <summary><para>Der Standard-Konstruktor.</para></summary>
        /// 
        /// <example>Das folgende Beispiel erzeugt eine Instanz der <see cref="MapService"/>-Klasse und weist 
        /// anschließend neue Werte für <see cref="Geo"/>, <see cref="Zoom"/> (15) und <see cref="Info.MapServer"/> (VirtualEarth) zu: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// MapService map = new MapService();
        /// map.Geo = new Geographic(8.12345, 50.56789);
        /// map.Zoom = 15;
        /// map.MapServer = MapService.Info.MapServer.VirtualEarth;
        /// </code>
        /// </example>
        public MapService() { }


        /// <summary><para>Konstruktor mit Parameter für ein <see cref="Geographic"/> Objekt.</para></summary>
        /// 
        /// <example>Das folgende Beispiel erzeugt eine Instanz der <see cref="MapService"/> Klasse und übergibt 
        /// ein zuvor erstelltes <see cref="Geographic "/> Objekt (<see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84">WGS84-Datum</see>): 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789);
        /// MapService map = new MapService(geo);
        /// </code>
        /// </example>
        /// 
        /// <param name="geo"><see cref="Geographic "/> Objekt im <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84">WGS84-Datum</see>.</param>
        public MapService(Geographic geo)
        {
            _geo = geo;
        }


        /// <summary><para>Konstruktor mit Parametern für ein <see cref="Geographic"/> Objekt und die Enumeration 
        /// <see cref="Info.MapServer"/>, die festlegt, welcher MapService Dienst gewählt werden soll. 
        /// Diese Angabe kann jederzeit über die <see cref="MapServer"/>-Eigenschaft geändert werden.</para></summary>
        /// 
        /// <example>Das folgende Beispiel erzeugt eine Instanz der <see cref="MapService"/>-Klasse und übergibt 
        /// ein zuvor erstelltes <see cref="Geographic "/> Objekt (<see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84">WGS84-Datum</see>) 
        /// und den zu wählenden <see cref="Info.MapServer"/>: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789);
        /// MapService.Info.MapServer server = MapService.Info.MapServer.YahooMaps;
        /// MapService map = new MapService(geo, server);
        /// </code>
        /// </example>
        /// 
        /// <param name="geo"><see cref="Geographic "/> Objekt im <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84">WGS84-Datum</see>.</param>
        /// <param name="server">Name des MapService Dienstes.</param>
        public MapService(Geographic geo, Info.MapServer server)
        {
            this.Geo = geo;
            this.MapServer = server;
        }


        /// <summary><para>Konstruktor mit Parametern für ein <see cref="Geographic"/>-Objekt und <see cref="Zoom"/>.</para></summary>
        /// 
        /// <example>Das folgende Beispiel erzeugt eine Instanz der <see cref="MapService"/> Klasse, übergibt ein 
        /// zuvor erstelltes <see cref="Geographic"/>-Objekt (<see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84">WGS84-Datum</see>) 
        /// und legt die initiale Zoomstufe fest. Diese Einstellung kann jederzeit über die Eigenschaft <see cref="Zoom"/> geändert werden: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789);
        /// MapService map = new MapService(geo, 15);                           // Zoomstufe 15
        /// </code>
        /// </example>
        /// 
        /// <param name="geo"><see cref="Geographic "/> Objekt im <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84">WGS84-Datum</see>.</param>        
        /// <param name="zoom">Zoomstufe. Mögliche Werte liegen im Bereich 1-21.</param>
        public MapService(Geographic geo, int zoom)
        {
            this.Geo = geo;
            this.Zoom = zoom;
        }


        /// <summary><para>Konstruktor mit Parametern für ein <see cref="Geographic"/> Objekt, <see cref="Zoom"/> 
        /// und <see cref="MapServer"/>.</para></summary>
        /// 
        /// <example>Das folgende Beispiel erzeugt eine Instanz der <see cref="MapService"/> Klasse, übergibt ein 
        /// zuvor erstelltes <see cref="Geographic"/> Objekt (<see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84">WGS84-Datum</see>) und legt die initiale Zoomstufe fest. 
        /// Diese Einstellung kann jederzeit über die Eigenschaft <see cref="Zoom"/> geändert werden: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789);
        /// MapService.Info.MapServer server = MapService.Info.MapServer.YahooMaps;     // MapService Dienst: Yahoo Maps
        /// MapService map = new MapService(geo, 15, server);                           // Zoomstufe 15
        /// </code>
        /// </example>
        /// 
        /// <param name="geo"><see cref="Geographic "/> Objekt im <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84">WGS84-Datum</see>.</param>        
        /// <param name="zoom">Zoomstufe. Mögliche Werte liegen im Bereich 1-21.</param>
        /// <param name="server">Name des MapService Dienstes als <see cref="MapService.Info.MapServer"/>-Enumeration.</param>
        public MapService(Geographic geo, int zoom, Info.MapServer server)
        {
            this.Geo = geo;
            this.MapServer = server;
            this.Zoom = zoom;
        }


        /// <summary><para>Konstruktor mit einem Parameter für ein aus der Klasse <see cref="Info.MapServiceTileBase"/> 
        /// abgeleitetes Objekt. Diese Objekte speichern die individuellen Einstellungen für einen jeweiligen MapService Dienst. 
        /// Diese Einstellungen sind für jeden Dienst unterschiedlich. Die <see cref="MapService"/>-Klasse übernimmt 
        /// normalerweise die Verwaltung der unterschiedlichen Dienste und Sie müssen sich nicht darum kümmern.</para></summary>
        /// 
        /// <example>Das folgende Beispiel erzeugt eine Instanz der <see cref="MapService"/> Klasse, übergibt ein 
        /// <see cref="Info.MapServiceVirtualEarthMapsTile"/> Objekt, welches ein Satelliten-/Luftbild des 
        /// MapService Dienstes Microsoft Virtual Earth repräsentiert: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// MapService.Info.MapServiceVirtualEarthMapsTile tile; 
        /// tile = new MapService.Info.MapServiceVirtualEarthMapsTile("a12020312");     // Virtual Earth Satelliten-/Luftbild
        /// MapService map = new MapService(tile);                           
        /// </code>
        /// </example>
        /// 
        /// <param name="tile">Ein aus <see cref="Info.MapServiceTileBase"/> abgeleitetes Objekt. Repräsentiert ein Satelliten-/Luftbild.</param>
        public MapService(Info.MapServiceTileBase tile)
        {
            Info.MapServiceInternalMapTile intern = null;

            if (tile.GetType() == typeof(Info.MapServiceInternalMapTile))                   // interner generischer MapService
            {
                intern = (Info.MapServiceInternalMapTile)tile;
                _mapserver = Info.MapServer.GoogleMaps;                                     // Standard
            }
            else if (tile.GetType() == typeof(Info.MapServiceGoogleMapsTile))               // Google Maps
            {
                Info.MapServiceGoogleMapsTile google = (Info.MapServiceGoogleMapsTile)tile;
                intern = Transform.GoogleToInternal(google);
                _mapserver = Info.MapServer.GoogleMaps;

            }
            else if (tile.GetType() == typeof(Info.MapServiceVirtualEarthMapsTile))         // Microsoft Virtual Earth
            {
                Info.MapServiceVirtualEarthMapsTile earth = (Info.MapServiceVirtualEarthMapsTile)tile;
                intern = Transform.VirtualEarthToInternal(earth);
                _mapserver = Info.MapServer.VirtualEarth;
            }
            else if (tile.GetType() == typeof(Info.MapServiceYahooMapsTile))                // Yahoo Maps
            {
                Info.MapServiceYahooMapsTile yahoo = (Info.MapServiceYahooMapsTile)tile;
                intern = Transform.YahooToInternal(yahoo);
                _mapserver = Info.MapServer.YahooMaps;
            }

            if (intern == null || intern.Key.Length > 22)
            {
                throw new ErrorProvider.GeoException(new ErrorProvider.ErrorMessage("ERROR_TILE"));
            }

            GeoRect r = Transform.MapDimension(intern);
            _geo = new Geographic(r.LonCenter, r.LatCenter);
            _zoom = intern.Key.Length - 1;
        }

        #endregion ==================== Konstruktoren ====================




        #region ==================== Eigenschaften ====================

        /// <summary><para>Die Eigenschaft <see cref="Geo"/> gibt die aktuellen Koordinaten (Längen-/Breitengrad) des 
        /// <see cref="MapService"/>-Objektes zurück, oder legt sie fest.</para></summary>
        /// 
        /// <example>Das folgende Beispiel erzeugt eine Instanz der <see cref="MapService"/>-Klasse und legt anschließend 
        /// neue Koordinaten fest: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// MapService map = new MapService();
        /// map.Geo = new Geographic(8.12345, 50.56789);
        /// </code>
        /// </example>
        public Geographic Geo { get { return _geo; } set { _geo = value; } }


        /// <summary><para>Die Eigenschaft <see cref="Zoom"/> gibt die aktuelle Zoomstufe des <see cref="MapService"/> Objektes zurück, oder legt sie fest.</para></summary>
        /// <remarks><para>Zulässige Werte für die Zoomstufe sind im Bereich von 1-21. Dabei bedeutet 1: kein Zoom, das heisst, 
        /// es ist ein Satellitenbild der Erde zu sehen. Je höher die Zoomstufe, desto genauere Details sind zu erkennen, bis 
        /// hin zu der maximalen Zoomstufe 21. Bitte beachten Sie, dass nicht alle Satelliten- bzw. Luftbilder in hohen 
        /// Auflösungen verfügbar sind. Einige Gegenden der Erde sind nur in weitaus geringeren Zoomstufen verfügbar. Dies kann
        /// an der Bedeutung dieser Region liegen, oder auch Sicherheitsaspekte können eine Rolle spielen. Möglicherweise 
        /// bietet ein anderer MapService Dienst für eine bestimmte Region eine bessere Abdeckung mit hochauflösenden Bildern.</para></remarks>
        /// 
        /// <example>Das folgende Beispiel erzeugt eine Instanz der <see cref="MapService"/> Klasse und legt die Zoomstufe fest: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789);
        /// MapService map = new MapService(geo);
        /// map.Zoom = 15;
        /// </code>
        /// </example>        
        public int Zoom
        {
            get
            {
                // OBSOLET: if (_mapserver == Info.MapServer.YahooMaps) return _zoom + 1;
                return _zoom;
            }
            set
            {
                int z = value;
                // OBSOLET: if (_mapserver == Info.MapServer.YahooMaps) { z += 1; };
                if (value >= 0 || value <= 21) _zoom = z;
            }
        }


        /// <summary><para>Die Eigenschaft <see cref="MapServer"/> legt fest, welcher MapService Dienst gewählt werden soll. 
        /// Diese Angabe kann jederzeit geändert werden.</para></summary>
        /// 
        /// <example>Das folgende Beispiel erzeugt eine Instanz der <see cref="MapService"/> Klasse legt den 
        /// MapService-Dienst durch die <see cref="Info.MapServer"/>-Enumeration fest: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789);
        /// MapService map = new MapService(geo);
        /// map.MapServer = MapService.Info.MapServer.GoogleMaps;
        /// </code>
        /// </example>
        public Info.MapServer MapServer { get { return _mapserver; } set { _mapserver = value; } }


        /// <summary><para>Die Eigenschaft <see cref="Center"/> legt fest, ob Die Koordinate auf dem Bild 
        /// zentriert werden soll. Dies wirkt sich auf das Laden des Bildes aus, da ein Pseudo-Satellitenbild 
        /// geladen wird, das die Koordinate in der Mitte des Bildes darstellt. Desweiteren wirkt es sich auf 
        /// Funktionen aus, die Bildpositionen in Koordinatenpositionen und umgekehrt berechnen.</para></summary>
        public bool Center { get { return _center; } set { _center = value; } }


        /// <summary><para>Die Eigenschaft <see cref="TileInfo"/> hält die verfügbaren Informationen über das aktuell gewählte 
        /// Satelliten-/Luftbild bereit und gibt diese Informationen in einem <see cref="Info.TileInfo"/> Objekt zurück.</para></summary>
        /// 
        /// <example>Das folgende Beispiel erzeugt eine Instanz der <see cref="MapService"/> Klasse und verwendet 
        /// anschließend die Informationen aus der <see cref="TileInfo"/>-Klasse um eine URL von dem Satellitenbild 
        /// auf dem gewählten MapService zu erhalten: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789);
        /// MapService map = new MapService(geo, 20);               // Zoomstufe 20;
        /// Info.TileInfo info = map.Tile;
        /// string url = info.URLGoogleMaps;                        // URL zum Satellitenbild auf dem Server des Dienstes
        /// </code>
        /// </example>        
        public Info.TileInfo TileInfo
        {
            get
            {
                Info.TileInfo ti = new Info.TileInfo();
                ti.Zoom = this.Zoom;
                // OBSOLET: if (_mapserver == MapService.Info.MapServer.YahooMaps) { ti.Zoom += 1; };   // Yahoo Maps 1 Zoomstufe höher

                // Relevanter Teil für den Zugriff auf das Satellitenbild
                ti.InternalMap = Transform.WGSIMAP(_geo, _zoom);
                ti.GoogleMaps = Transform.InternalToGoogle(ti.InternalMap);
                ti.VirtualEarth = Transform.InternalToVirtualEarth(ti.InternalMap);
                ti.YahooMaps = Transform.InternalToYahoo(ti.InternalMap);

                // Vollständige URL zum Satellitenbild
                string serv = (new Random().Next(0, 3)).ToString();                         // Ladeverteilung auf verschiedene Server 
                ti.URL.GoogleMaps = ti.GoogleMaps.URL;
                ti.URL.VirtualEarth = ti.VirtualEarth.URL;
                ti.URL.YahooMaps = ti.YahooMaps.URL;

                // Berechnung der Dimensionen
                MapService.Info.MapServiceInternalMapTile tile = new Info.MapServiceInternalMapTile(ti.InternalMap.Key);
                ti.Dimension = Transform.MapDimension(tile);
                
                //Berechnung der Pixel-Position dem Bild
                MapService.Info.MapServiceInternalMapTile tail = Transform.WGSIMAP(_geo, 60);
                tail.Key = tail.Key.Substring(tile.Key.Length);                     // Restkey, der nur die Pixelverschiebung im Bild enthält
                ti.GeoPosition = Transform.WGSPixel(tail, TILE_SIZE);
                return ti;
            }
        }


        /// <summary><para>Die Eigenschaft <see cref="Tile"/> gibt die vom aktuell gewählten MapService Dienst benutzten 
        /// Schlüssel-Koordinaten zurück, oder legt über diese Daten ein neues Satelliten-/Luftbild fest.</para></summary>
        /// <remarks><para>Statt ein neues Satelliten-/Luftbild über eine geographische Koordinatenangabe zu auszuwählen,
        /// wird hier der interne Schlüssel des gewählten MapService Dienstes direkt angesprochen. Dies erfordert einige 
        /// Kenntnisse bezüglich der Syntax des gewünschten MapService-Dienstes. Die Eigenschaft wird daher vermutlich 
        /// nur in besonderen Fällen verwendet. Üblicherweise ist die <see cref="Geo"/>-Eigenschaft geeigneter, um ein
        /// neues Bild zu erhalten. 
        /// Die Eigenschaft verwendet eine aus der Basisklasse <see cref="Info.MapServiceTileBase"/> abgeleitete Klasse. 
        /// Diese Klassen speichern für einen jeweiligen MapService Dienst die entsprechend benötigten Informationen. 
        /// Diese Informationen entsprechen der Repräsentation eines Satelliten-/Luftbildes und sie sind daher
        /// für jeden MapService Provider unterschiedlich. Die <see cref="MapService"/>-Klasse übernimmt normalerweise die 
        /// Verwaltung der unterschiedlichen Dienste und Sie müssen sich nicht darum kümmern.</para></remarks>
        /// 
        /// <example>Das folgende Beispiel erzeugt eine Instanz der <see cref="MapService"/> Klasse, übergibt ein 
        /// <see cref="Info.MapServiceVirtualEarthMapsTile"/> Objekt, welches ein Satelliten-/Luftbild des 
        /// MapService Dienstes Microsoft Virtual Earth repräsentiert: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789);
        /// MapService map = new MapService(geo);                           
        /// MapService.Info.MapServiceVirtualEarthMapsTile tile; 
        /// tile = new MapService.Info.MapServiceVirtualEarthMapsTile("a12020312");     // Virtual Earth Satelliten-/Luftbild
        /// map.Tile(tile);
        /// </code>
        /// </example>
        public Info.MapServiceTileBase Tile
        {
            get
            {
                Info.MapServiceTileBase tile = null;
                Info.MapServiceInternalMapTile intern = Transform.WGSIMAP(this.Geo, this.Zoom);

                if (this.MapServer == Info.MapServer.GoogleMaps)
                {
                    tile = Transform.InternalToGoogle(intern);
                }
                if (this.MapServer == Info.MapServer.VirtualEarth)
                {
                    tile = Transform.InternalToVirtualEarth(intern);
                }
                if (this.MapServer == Info.MapServer.YahooMaps)
                {
                    tile = Transform.InternalToYahoo(intern);
                }

                return tile;
            }
            set
            {
                MapService map = new MapService(value);                     // temporäres Objekt für die Schlüsselumwandlung
                string newKey = map.TileInfo.InternalMap.Key;               // neuen internen generischen Schlüssel holen
                string intKey = this.TileInfo.InternalMap.Key;              // aktuellen internen generischen Schlüssel holen
                bool bNew = false;                                          // true, wenn neues Bild benötigt wird                                   

                if (intKey.Length <= newKey.Length)
                {
                    if (intKey != newKey.Substring(0, intKey.Length)) bNew = true;
                }
                else
                {
                    if (newKey != intKey.Substring(0, newKey.Length)) bNew = true;
                }
                if (bNew)
                {
                    Info.MapServiceInternalMapTile tile = new Info.MapServiceInternalMapTile(newKey);
                    GeoRect r = Transform.MapDimension(tile);
                    _geo.Longitude = r.LonCenter;                           // Neue Koordinaten werden auf Mitte des Bildes gesetzt.
                    _geo.Latitude = r.LatCenter;
                    _zoom = newKey.Length - 1;                              // Neuen Zoom berechnen
                }
            }
        }


        /// <summary><para>Die Eigenschaft bindet die Klasse <see cref="Helper.Images"/> ein, und stellt deren Funktionalitäten zur 
        /// Verfügung. Nähere Angaben und Beispiele siehe unter <see cref="Helper.Images"/>.</para></summary>
        //public Helper.Images Images { get { return new Helper.Images(this); } }

        #endregion ==================== Eigenschaften ====================




        #region ==================== Methoden ====================

        /// <summary><para>Die Funktion berechnet die geographische Koordinate eines Punktes auf dem aktuellen 
        /// Satellitenbild. Bildursprung ist die linke obere Ecke (Punkt 0,0).</para></summary>
        /// 
        /// <remarks><para>Ein anderer Algorithmus ist hier zu finden: <see cref="PixelToGeographic2(int, int)"/>.</para></remarks>
        /// 
        /// <example>Das folgende Beispiel zeigt ein Beispiel für die Anwendung der Funktion: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789);
        /// MapService map = new MapService(geo);
        /// // berechnet die geographische Koordinate der linken oberen Ecke des Satellitenbildes 
        /// Geographic geo2 = map.GetGeoFromTilePos(0, 0);
        /// </code>
        /// </example>
        /// 
        /// <param name="x">Abstand in Pixel (0-255) gemessen von der linken Seite des Bildes nach rechts.</param>
        /// <param name="y">Abstand in Pixel (0-255) gemessen von der oberen Seite des Bildes nach unten.</param>
        /// <returns>Geographische Koordinate als <see cref="Geographic "/> Objekt (Längen-/Breitengrad)</returns>
        public Geographic PixelToGeographic(int x, int y)
        {
            if (!(x >= 0 && x <= TILE_SIZE && y >= 0 && y <= TILE_SIZE)) return null;
            y = TILE_SIZE - y;                                                  // Von Süden nach Norden; i. d. R. 0 - 255 (deshalb -1)
            Info.MapServiceInternalMapTile tile = new Info.MapServiceInternalMapTile(this.TileInfo.InternalMap.Key);
            int shiftx = 0;
            int shifty = 0;
            if (this.Center == true)
            {
                shiftx = this.TileInfo.GeoPosition.X - (TILE_SIZE / 2);
                shifty = (TILE_SIZE / 2) - this.TileInfo.GeoPosition.Y;
            }
            // Grobberechnung 
            GeoRect rect = Transform.MapDimension(tile);
            double lonPixel = rect.Width / (TILE_SIZE);
            double latPixel = rect.Height / (TILE_SIZE);
            double lonPos = (lonPixel * x) + (lonPixel * shiftx) + rect.Longitude;
            double latPos = (latPixel * y) + (latPixel * shifty) + rect.Latitude;

            // Feinjustierung nur wenn nicht normalisiert
            MapService map = new MapService(new Geographic(lonPos, latPos), _zoom);
            if (this.Center == false)
            {
                y = TILE_SIZE - y;                                             // Obigen Schritt wieder umdrehen
                latPixel /= 2;                                                 // kleinere Auflösung für genauere Schrittweite
                lonPixel /= 2;                                                 // kleinere Auflösung für genauere Schrittweite
                while (map.TileInfo.GeoPosition.Y < y) { map.Geo.Latitude -= latPixel; };
                while (map.TileInfo.GeoPosition.Y > y) { map.Geo.Latitude += latPixel; };
                while (map.TileInfo.GeoPosition.X < x) { map.Geo.Longitude += lonPixel; };
                while (map.TileInfo.GeoPosition.X > x) { map.Geo.Longitude -= lonPixel; };
            }
            return map.Geo;
        }


        /// <summary><para>Die Funktion berechnet die geographische Koordinate eines Punktes auf dem aktuellen 
        /// Satellitenbild. Bildursprung ist die linke obere Ecke (Punkt 0,0).</para></summary>
        /// 
        /// <remarks><para>Dies ist ein alternetiver Algorithmus. Siehe auch: <see cref="PixelToGeographic(int, int)"/>.</para></remarks>
        /// 
        /// <example>Das folgende Beispiel zeigt ein Beispiel für die Anwendung der Funktion: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789);
        /// MapService map = new MapService(geo);
        /// // berechnet die geographische Koordinate der linken oberen Ecke des Satellitenbildes 
        /// Geographic geo2 = map.GetGeoFromTilePos(0, 0);
        /// </code>
        /// </example>
        /// 
        /// <param name="x">Abstand in Pixel (0-255) gemessen von der linken Seite des Bildes nach rechts.</param>
        /// <param name="y">Abstand in Pixel (0-255) gemessen von der oberen Seite des Bildes nach unten.</param>
        /// <returns>Geographische Koordinate als <see cref="Geographic "/> Objekt (Längen-/Breitengrad)</returns>
        public Geographic PixelToGeographic2(int x, int y)
        {
            if (!(x >= 0 && x < TILE_SIZE && y >= 0 && y < TILE_SIZE)) return null;
            if (this.Center == true) return PixelToGeographic(x, y);

            Info.MapServiceInternalMapTile tile = new Info.MapServiceInternalMapTile(this.TileInfo.InternalMap.Key);
            string detailkey = "";
            int size = TILE_SIZE;
            GeoPoint position = new GeoPoint(0, 0);
            for (int i = 1; i <= 8; i++)
            {
                size /= 2;
                if ((x > (position.X + size)) && (y > (position.Y + size)))
                {
                    detailkey += "s";
                    position.X += size;
                    position.Y += size;
                }
                else if ((x > (position.X + size)) && (y <= (position.Y + size)))
                {
                    detailkey += "r";
                    position.X += size;
                }
                else if ((x <= (position.X + size)) && (y > (position.Y + size)))
                {
                    detailkey += "t";
                    position.Y += size;
                }
                else
                {
                    detailkey += "q";
                }

            }
            tile.Key += detailkey + "ssssssssss";
            return new MapService(tile).Geo;
        }



        /// <summary><para>Die Funktion prüft, ob die übergebene geographische Koordinate auf dem aktuellen Satellitenbild liegt.</para></summary>
        /// 
        /// <example>Das folgende Beispiel prüft, ob die übergebene Koordinate auf dem Satellitenbild liegt. In diesem
        /// Beispiel gibt die Funktion <i>true</i> zurück, da offensichtlich die gleiche Koordinate ist, mit dem das 
        /// <see cref="MapService"/>-Objekt erzeugt wurde: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789);
        /// MapService map = new MapService(geo);
        /// bool issame = map.CoordsOnSameTile(geo);                //gibt true zurück, da die geographischen Koordinaten gleich sind. 
        /// </code>
        /// </example>
        /// 
        /// <param name="geo">Die zu überprüfende Koordinate als <see cref="Geographic "/> Objekt.</param>
        /// <returns>True, wenn die Koordinate auf dem Bild liegt. False, wenn die Koordinate außerhalb des Bildes liegt.</returns>
        public bool CoordsOnSameTile(Geographic geo)
        {
            Info.MapServiceInternalMapTile tile = Transform.WGSIMAP(geo, _zoom);
            return (this.TileInfo.InternalMap.Key == tile.Key);
        }


        /// <summary><para>Die überladene Funktion sucht ein gemeinsames Satelliten-/Luftbild, auf welchem 
        /// die aktuelle Koordinate und auch die übergebene Koordinate liegen. In dieser Funktion 
        /// kann auch der MapService Dienst angegeben werden.</para></summary>
        /// <remarks><para>Bei weit auseinander liegenden Koordinaten, wie beispielsweise Punkte in Amerika und Europa, 
        /// ist die Auflösung des Satellitenbildes erwartungsgemäß sehr gering.</para></remarks>
        /// 
        /// <example>Das Beispiel erstellt für eine geographische Koordinate ein <see cref="MapService"/> Objekt, und legt damit 
        /// ein Satellitenbild fest. Anschließend berechnet die Funktion <see cref="CalculateCommonTile(Geographic, Info.MapServer)"/> 
        /// für eine zweite Koordinate ein gemeinsames Satellitenbild, und gibt eine Instanz der <see cref="MapService.Info.MapServiceTileBase"/>
        /// Klasse zurück. In diesem Objekt hat man Zugriff auf eine <see cref="Info.MapServiceTileBase.URL"/>-Eigenschaft, die 
        /// den kompletten Pfad zu dem gemeinsamen Bild auf einem Server des gewählten MapService Providers darstellt. 
        /// <para>Benötigt man genauere Inforationen über das zurückgegebene Bild, kann man statt des
        /// <see cref="Info.MapServiceTileBase"/>-Objektes eine davon abgeleitete Klasse verwenden. Siehe dazu
        /// auch das Beispiel unter <see cref="CalculateCommonTile(Geographic)"/></para>
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(13.377829, 52.515934);              // Berlin Brandenburger Tor
        /// Geographic geo2 = new Geographic(13.376026, 52.518597);             // Berlin Reichstag
        /// MapService map = new MapService(geo);
        /// MapService.Info.MapServiceTileBase tile;                
        /// tile = map.CalculateCommonTile(geo2, MapService.Info.MapServer.GoogleMaps);
        /// string output = tile.URL;
        /// </code>
        /// </example>
        /// 
        /// <param name="geo">Vergleichsposition als <see cref="Geographic "/> Objekt.</param>
        /// <param name="server">Ein <see cref="Info.MapServer"/> Element, das den zu wählenden MapService Dienst angibt.</param>
        /// <returns>Ein <see cref="Info.MapServiceTileBase"/> Objekt, das genauere Informationen zum Satellitenbild liefert.</returns>
        public Info.MapServiceTileBase CalculateCommonTile(Geographic geo, Info.MapServer server)
        {
            string calculatedKey = "";
            Info.MapServiceInternalMapTile tile;

            string internalKey = this.TileInfo.InternalMap.Key;
            tile = Transform.WGSIMAP(geo, 21);
            string sencondKey = tile.Key;
            int len = internalKey.Length;

            string s1, s2;
            for (int i = 0; i < len; i++)
            {
                s1 = internalKey.Substring(i, 1);
                s2 = sencondKey.Substring(i, 1);
                if (s1 == s2) calculatedKey += s1;
                else break;
            }
            tile.Key = calculatedKey;

            if (server == Info.MapServer.GoogleMaps) return (Transform.InternalToGoogle(tile));
            else if (server == Info.MapServer.VirtualEarth) return (Transform.InternalToVirtualEarth(tile));
            else if (server == Info.MapServer.YahooMaps) return (Transform.InternalToYahoo(tile));
            else return this.TileInfo.InternalMap;
        }


        /// <summary><para>Die überladene Funktion sucht ein gemeinsames Satelliten-/Luftbild, auf welchem 
        /// die aktuelle Koordinate und auch die übergebene Koordinate liegen. Für weitere Informationen 
        /// siehe auch <see cref="CalculateCommonTile(Geographic, Info.MapServer)"/></para></summary>
        /// 
        /// <example>Das Beispiel erstellt für eine geographische Koordinate ein <see cref="MapService"/> Objekt, und legt damit 
        /// ein Satellitenbilds fest. Anschließend berechnet die Funktion <see cref="CalculateCommonTile(Geographic)"/> für eine zweite 
        /// Koordinate ein gemeinsames Satellitenbild, und gibt eine Instanz der <see cref="MapService.Info.MapServiceTileBase"/>
        /// Klasse zurück. In diesem Objekt hat man Zugriff auf eine <see cref="Info.MapServiceTileBase.URL"/>-Eigenschaft, die den kompletten
        /// Pfad zu dem gemeinsamen Bild auf einem Server des gewählten MapService Providers darstellt. 
        /// <para>Benötigt man genauere Inforationen über das zurückgegebene Bild, kann man anstatt  
        /// <see cref="MapService.Info.MapServiceTileBase"/> eine davon abgeleitete Klasse verwenden. Siehe 
        /// auch das Beispiel unter <see cref="CalculateCommonTile(Geographic, Info.MapServer)"/></para>
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(13.377829, 52.515934);              // Berlin Brandenburger Tor
        /// Geographic geo2 = new Geographic(13.376026, 52.518597);             // Berlin Reichstag
        /// MapService map = new MapService(geo);
        /// MapService.Info.MapServiceTileBase tile;
        /// tile = map.CalculateCommonTile(geo2, MapService.Info.MapServer.YahooMaps);
        /// MapService.Info.MapServiceYahooMapsTile yahoo = (MapService.Info.MapServiceYahooMapsTile)tile;
        /// string output = "Um beide Punkte auf einem Bild zu haben muss mindestens die Zoomstufe ";
        /// output += yahoo.Zoom.ToString() + " verwendet werden.";
        /// </code>
        /// </example>
        /// 
        /// <param name="geo">Vergleichsposition als <see cref="Geographic "/> Objekt.</param>
        /// <returns>Ein <see cref="Info.MapServiceTileBase"/> Objekt, das genauere Informationen zum Satellitenbild liefert.</returns>
        public Info.MapServiceTileBase CalculateCommonTile(Geographic geo)
        {
            return CalculateCommonTile(geo, _mapserver);
        }


        /// <summary><para>Die Funktion berechnet das Satellitenbild, das in der angegebenen Richtung liegt.</para></summary>
        /// <remarks><para>Dabei können auch mehrere Bewegungsschritte mithilfe des <i>steps</i> Parameters auf einmal 
        /// ausgeführt werden. <para>Der <i>center</i> Parameter legt fest, ob bei einer Verschiebung der Bewegung auch die 
        /// zugrundeliegende geographische Koordinate, mit dem das <see cref="MapService"/>-Objekt erzeugt wurde, angepasst werden soll. 
        /// Bei <i>true</i> wird die Koordinate auf die Mitte des neuen Bildes festgelegt, bei <i>false</i> wird die 
        /// Koordinate nicht angepasst, und zeigt somit auf ein anderes Satellitenbild. Dies kann dennoch gewünscht sein, 
        /// wenn beispielsweise eine Ursprungsposition später wiederverwendet werden soll.</para></para></remarks>
        /// 
        /// <example>Das folgende Beispiel berechnet mit Hife der <see cref="Info.MapDirection"/>-Enumeration das 
        /// Satellitenbild, das in nordwestlicher Richtung angrenzt. Die zugrundeliegende geographische Koordinate wird nicht 
        /// angepasst, kann also später wiederverwendet werden:
        /// <code>
        /// Geographic geo = new Geographic(8.12345, 50.56789);
        /// MapService map = new MapService(geo);
        /// MapService.Info.MapServiceTileBase tile;
        /// tile = map.Move(MapService.Info.MapDirection.Northwest, 1, false);      // Ein Schritt nach Nordwesten
        /// string output = tile.URL;
        /// </code>
        /// </example>
        /// 
        /// <param name="direction">Eine in <see cref="Info.MapDirection"/> definierte Verschiebungsrichtung.</param>
        /// <param name="steps"> Die Anzahl der Bewegungen in die Angegebene Richtung (Maximal 10, Minimal 1).</param>
        /// <param name="center">True, wenn die zugrundeliegende geographische Position auf die neue Bildmitte verschoben werden soll. 
        /// False, falls die zugrundeliegende geographische Position erhalten bleiben soll.</param>
        /// <returns>Ein <see cref="Info.MapServiceTileBase"/> Objekt, welches eine vollständige URL zur Verfügung stellt und über 
        /// die abgeleiteten Klassen den Zugriff auf weitere Informationen ermöglicht.</returns>
        public Info.MapServiceTileBase Move(Info.MapDirection direction, int steps, bool center)
        {
            Info.MapServiceInternalMapTile tile = Transform.WGSIMAP(_geo, _zoom);

            if (steps < 1 || steps > 10) { steps = 1; };

            for (int i = 0; i < steps; i++)
            {
                if (direction == Info.MapDirection.North || direction == Info.MapDirection.Northeast || direction == Info.MapDirection.Northwest)
                {
                    tile = Transform.MapMove(tile, Info.MapDirection.North);
                }
                if (direction == Info.MapDirection.South || direction == Info.MapDirection.Southeast || direction == Info.MapDirection.Southwest)
                {
                    tile = Transform.MapMove(tile, Info.MapDirection.South);
                }
                if (direction == Info.MapDirection.East || direction == Info.MapDirection.Southeast || direction == Info.MapDirection.Northeast)
                {
                    tile = Transform.MapMove(tile, Info.MapDirection.East);
                }
                if (direction == Info.MapDirection.West || direction == Info.MapDirection.Southwest || direction == Info.MapDirection.Northwest)
                {
                    tile = Transform.MapMove(tile, Info.MapDirection.West);
                }
            }

            if (center)
            {
                GeoRect r = Transform.MapDimension(tile);
                _geo.Longitude = r.LonCenter;
                _geo.Latitude = r.LatCenter;
            }

            if (_mapserver == Info.MapServer.GoogleMaps) return Transform.InternalToGoogle(tile);
            else if (_mapserver == Info.MapServer.VirtualEarth) return Transform.InternalToVirtualEarth(tile);
            else if (_mapserver == Info.MapServer.YahooMaps) return Transform.InternalToYahoo(tile);
            else return this.TileInfo.InternalMap;
        }


        /// <summary><para>Die Funktion gibt kurzen einen String mit den geographischen Koordinaten und der Zoomstufe zurück.</para></summary>
        /// 
        /// <example>Das folgende Beispiel zeigt eine Ausgebe der Methode: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789); 
        /// MapService map = new MapService(geo, 15);
        /// map.ToString();                           // Ausgabe: "8.12345; 50.56789; 15"
        /// </code>
        /// </example>
        /// 
        /// <returns>Kurzer String mit den geographischen Koordinaten und der Zoomstufe des <see cref="MapService"/>-Objektes.</returns>
        public new string ToString()
        {
            return this.Geo.LonString + "; " + this.Geo.LatString + "; " + this.Zoom.ToString();
        }


        /// <summary><para>Die Funktion gibt kurzen einen String mit den geographischen Koordinaten und der Zoomstufe zurück.</para></summary>
        /// 
        /// <example>Das folgende Beispiel zeigt eine Ausgebe der Methode: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789); 
        /// MapService map = new MapService(geo, 15);
        /// map.ToLongString();                        / Ausgabe: "Breite: 8.12345; Länge: 50.56789; Zoom: 15; MapService: GoogleMaps"
        /// </code>
        /// </example>
        /// 
        /// <returns>Langer String mit den geographischen Koordinaten und der Zoomstufe des <see cref="MapService"/> Objektes.</returns>
        public string ToLongString()
        {
            return string.Format(new Localizer.Message("GEOGRAPHIC_STRING").Text, this.Geo.LonString, this.Geo.LatString) + "; Zoom:" + this.Zoom.ToString() + "; Provider: " + this.MapServer.ToString();
        }

        #endregion ==================== Methoden ====================




        #region ==================== Operatoren/Typumwandlung ====================

        /// <summary><para>Konvertierungsoperator für die Transformation von <see cref="MapService"/> nach <see cref="Geographic"/>.</para></summary>
        /// 
        /// <example>Das Beispiel konvertiert ein <see cref="MapService"/>-Objekt in ein <see cref="Geographic"/>-Objekt 
        /// im internationalen <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84">WGS84-Datum</see>:
        /// <code>
        /// // Beispiel für eine Typumwandlung nach Geographic. Da keine Koordinaten bei der Erzeugung des 
        /// // <see cref="T:GeoUtility.GeoSystem.MapService" /> Objekts angegeben wurden, wird Bildmitte angenommen. 
        /// using GeoUtility.GeoSystem;
        /// MapService.Info.MapServiceVirtualEarthMapsTile tile; 
        /// tile = new MapService.Info.MapServiceVirtualEarthMapsTile("a12020312");     // Virtual Earth Satelliten-/Luftbild
        /// MapService map = new MapService(tile);                                      // MapService Objekt erzeugen
        /// Geographic geo = (Geographic)map;                                           
        /// </code>
        /// </example>
        /// 
        /// <param name="maps">Das aktuelle <see cref="MapService"/> Objekt.</param>
        /// <returns>Ein <see cref="Geographic"/> Objekt (Längen-/Breitengrad).</returns>
        public static explicit operator Geographic(MapService maps)
        {
            return maps.Geo;
        }


        /// <summary><para>Konvertierungsoperator für die Transformation von <see cref="MapService"/> nach <see cref="UTM"/>.</para></summary>
        /// 
        /// <example>Das Beispiel konvertiert ein <see cref="MapService"/>-Objekt in ein <see cref="UTM"/>-Objekt: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// MapService map = new MapService(new Geographic(8.12345, 50.56789));
        /// UTM utm = (UTM)map;                                                         // Typumwandlung in ein UTM Objekt
        /// string output = utm.ToString();                                             // Ausgabe: "32U 0437924 5602141"
        /// </code>
        /// </example>
        /// 
        /// <param name="maps">Das aktuelle <see cref="MapService"/> Objekt.</param>
        /// <returns>Ein <see cref="UTM"/> Objekt.</returns>
        public static explicit operator UTM(MapService maps)
        {
            return Transform.WGSUTM(maps.Geo);
        }


        /// <summary><para>Konvertierungsoperator für die Transformation von <see cref="MapService"/> nach <see cref="GaussKrueger"/>.</para></summary>
        /// 
        /// <example>Das Beispiel konvertiert ein <see cref="MapService"/>-Objekt in ein <see cref="GaussKrueger"/>-Objekt: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// MapService map = new MapService(new Geographic(8.12345, 50.56789));
        /// GaussKrueger gauss = (GaussKrueger)map;                                   // Typumwandlung in ein GaussKrueger Objekt
        /// string output = gauss.ToLongString();                                       // Ausgabe: "Rechts: 3437975; Hoch: 5603943"
        /// </code>
        /// </example>
        /// 
        /// <param name="maps">Das aktuelle <see cref="MapService"/> Objekt.</param>
        /// <returns>Ein <see cref="GaussKrueger"/> Objekt.</returns>
        public static explicit operator GaussKrueger(MapService maps)
        {
            return Transform.WGSGK(maps.Geo);
        }


        /// <summary><para>Konvertierungsoperator für die Transformation von <see cref="MapService"/> nach <see cref="MGRS"/>.</para></summary>
        /// 
        /// <example>Das Beispiel konvertiert ein <see cref="MapService"/>-Objekt in ein <see cref="MGRS"/>-Objekt: 
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// MapService map = new MapService(new Geographic(8.12345, 50.56789));
        /// MGRS mgrs = (MGRS)map;                                                      // Typumwandlung in ein MGRS Objekt
        /// string output = mgrs.ToString();                                            // Ausgabe: "32UMB3792402141"
        /// </code>
        /// </example>
        /// 
        /// <param name="maps">Das aktuelle <see cref="MapService"/>-Objekt.</param>
        /// <returns>Ein <see cref="UTM"/>-Objekt.</returns>
        public static explicit operator MGRS(MapService maps)
        {
            UTM utm = Transform.WGSUTM(maps.Geo);
            return Transform.UTMMGR(utm);
        }


        /// <summary><para>Die generische Methode konvertiert den generischen Typ T in das aktuelle <see cref="MapService"/> Objekt.</para></summary>
        /// 
        /// <example>Das Beispiel konvertiert ein bestehendes <see cref="Geographic"/>-Objekt in das aktuelle <see cref="MapService"/>-Objekt 
        /// mit Hilfe der generischen Methode <see cref="ConvertFrom{T}(T)">ConvertFrom&lt;T&gt;(T)</see>:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789);
        /// MapService map;
        /// map.ConvertFrom&lt;Geographic&gt;(geo);
        /// </code>
        /// </example>
        /// 
        /// <typeparam name="T">Ein aus der Basisklasse <see cref="GeoUtility.GeoSystem.Base.BaseSystem"/> abgeleiteter Typ.</typeparam>
        /// <param name="t">Das zu konvertierende Objekt als generischer Parameter.</param>
        public void ConvertFrom<T>(T t) where T : BaseSystem
        {
            MapService o = null;
            try
            {
                if (typeof(T) == typeof(UTM)) o = (MapService)(UTM)(BaseSystem)t;
                else if (typeof(T) == typeof(GaussKrueger)) o = (MapService)(GaussKrueger)(BaseSystem)t;
                else if (typeof(T) == typeof(MGRS)) o = (MapService)(MGRS)(BaseSystem)t;
                else if (typeof(T) == typeof(Geographic)) o = (MapService)(Geographic)(BaseSystem)t;
                if (o != null)
                {
                    this.Geo.Longitude = o.Geo.Longitude;
                    this.Geo.Latitude = o.Geo.Latitude;
                    o = null;
                }
            }
            catch (Exception ex)
            {
                throw new ErrorProvider.GeoException(new ErrorProvider.ErrorMessage("ERROR_CONVERTFROM"), ex);
            }
        }


        /// <summary><para>Die generische Methode konvertiert ein Objekt aus der Basisklasse 
        /// <see cref="GeoUtility.GeoSystem.Base.BaseSystem"/> in ein <see cref="MapService"/>-Objekt.</para></summary>
        /// 
        /// <example>Das Beispiel konvertiert ein bestehendes <see cref="Geographic"/>-Objekt in ein neues 
        /// <see cref="MapService"/>-Objekt mit Hilfe der generischen Methode <see cref="ConvertTo{T}">ConvertTo&lt;T&gt;</see>:
        /// <code>
        /// using GeoUtility.GeoSystem;
        /// Geographic geo = new Geographic(8.12345, 50.56789);
        /// MapService map;
        /// map = geo.ConvertTo&lt;MapService&gt;();
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
            else if (typeof(T) == typeof(Geographic)) return (T)((BaseSystem)((Geographic)this));
            else return null;
        }


        /// <summary><para>Erstellt eine flache Kopie des aktuellen Objekts.</para></summary>
        /// <returns>Ein neues <see cref="MapService"/>-Objekt als flache Kopie.</returns>
        public new MapService MemberwiseClone()
        {
            Geographic geo = new Geographic(this.Geo.Longitude, this.Geo.Latitude);
            MapService map = new MapService(geo, this.Zoom, this.MapServer);
            map.Center = this.Center;
            return map;
        }


        /// <summary>Die Funktion wird aus performancegründen implementiert. Das Einfügen in eine Hashtabelle wird durch die die 
        /// Bereitstellung eines Hashwertes wesentlich beschleunigt.</summary>
        /// <returns>Ein Hashwert.</returns>
        public override int GetHashCode()
        {
            return ((int)(this.Geo.Longitude * 100) ^ (int)this.Geo.Latitude) * Zoom;
        }


        /// <summary>Die Funktion wird aus performancegründen implementiert.</summary>
        /// <param name="obj">Ein beliebiges Objekt.</param>
        /// <returns>Das übergebene Objekt ist gleich oder nicht.</returns>
        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            MapService map = (MapService)obj;

            return Equals(map);
            // return ((this.Geo == map.Geo) && (this.MapServer == map.MapServer) && (this.Center == map.Center) && (this.Zoom == map.Zoom));
        }


        /// <summary>Die Funktion wird wegen der Implementierung der Schnittstelle IEquatable notwendig.</summary>
        /// <param name="map">Ein beliebiges <see cref="MapService"/>-Objekt.</param>
        /// <returns>Das übergebene Objekt ist gleich oder nicht.</returns>
        public bool Equals(MapService map)
        {
            bool result = false;
            if ((this.MapServer == map.MapServer) && (this.Zoom == map.Zoom)) 
            {
                if ((this.Geo == map.Geo) && (this.Center == map.Center)) 
                { 
                    result = true;                                              // MapService-Objekte sind Wertgleich
                }
                /*else if ((MapService.Helper.Images.Cache.Enabled == true) && (MapService.Helper.Images.Cache.EqualityMode == Helper.Images.CacheEqualityMode.Image))
                {
                    if ((this.TileInfo.InternalMap.Key) == (map.TileInfo.InternalMap.Key))
                    {
                        int half_size = TILE_SIZE / 2;
                        if ((map.Center == false) && (this.Center == false)) result = true;
                        else if ((this.Center == false) && (map.TileInfo.GeoPosition.X == half_size) && (map.TileInfo.GeoPosition.Y == half_size)) result = true;
                        else if ((map.Center == false) && (this.TileInfo.GeoPosition.X == half_size) && (this.TileInfo.GeoPosition.Y == half_size)) result = true;
                    }
                }*/
            } 
            return result;
        }


        /// <summary>Überladener Gleichheitsoperator.</summary>
        /// <param name="mapA">Das erste zu vergleichende Objekt.</param>
        /// <param name="mapB">Das zweite zu vergleichende Objekt.</param>
        /// <returns>True, wenn beide Objekte die gleichen Werte haben. False, wenn die Werte nicht gleich sind.</returns>
        public static bool operator ==(MapService mapA, MapService mapB)
        {
            if (System.Object.ReferenceEquals(mapA, mapB)) return true;           // True, wenn beide null, oder gleiche Instanz.
            if (((object)mapA == null) || ((object)mapB == null)) return false;   // False, wenn ein Objekt null, oder beide nicht null
            return ((mapA.Geo == mapB.Geo) && (mapA.MapServer == mapB.MapServer) && (mapA.Center == mapB.Center) && (mapA.Zoom == mapB.Zoom));
            // return mapA.Equals(mapB);
        }


        /// <summary>Überladener Ungleichheitsoperator.</summary>
        /// <param name="mapA">Das erste zu vergleichende Objekt.</param>
        /// <param name="mapB">Das zweite zu vergleichende Objekt.</param>
        /// <returns>True, wenn beide Objekte mindestens einen unterschiedlichen Wert haben. False, wenn alle Werte gleich sind.</returns>
        public static bool operator !=(MapService mapA, MapService mapB)
        {
            return !(mapA == mapB);
        }

        #endregion ==================== Operatoren/Typumwandlung ====================




        #region ==================== Subklassen, Enum, etc. ====================

        // Die Klasse MapService.Info wurde in eine eigene Datei kopiert, da sie aufgrund der Größe 
        // hier nicht mehr übersichtlich dargestellt werden konnte. 
        // Sie befindet sich nun im Unterordner MapService, Datei Info.cs.

        #endregion ==================== Subklassen, Enum, etc. ====================


    }

}
