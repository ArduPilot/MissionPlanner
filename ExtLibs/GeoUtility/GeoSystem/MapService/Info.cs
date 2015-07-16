//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/GeoSystem/MapService/Info.cs $
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
// File description   : Definition des MapService.Info Klasse 
//===================================================================================================


using System;
using GeoUtility.GeoSystem.Base;
using GeoUtility.GeoSystem.Helper;

namespace GeoUtility.GeoSystem
{
    public partial class MapService : BaseSystem, IEquatable<MapService>
    {

        /// <summary><para>Die Klasse <see cref="Info"/> enthält Klassen, die Informationen über das aktuelle Satelliten-/Luftbild 
        /// speichern. Daneben enthält die Klasse sonstige Strukturen, die Steuerungsfunktionalitäten für die 
        /// <see cref="MapService"/> Klasse zur Verfügung stellen.</para></summary>
        public class Info
        {

            /// <summary><para>Der Standard-Konstruktor.</para></summary>
            /// 
            /// <example>Das folgende Beispiel gibt ein Google Maps Satellitenbild direkt an und übergibt es an den
            /// Konstruktor eines <see cref="MapService"/> Objektes. Die <see cref="MapService.Geo">geographischen Koordinaten</see> 
            /// des <see cref="MapService"/> Objektes werden dabei auf die Bildmitte gesetzt:
            /// <code>
            /// using GeoUtility.GeoSystem;
            /// MapService.Info.MapServiceGoogleMapsTile tile = new MapService.Info.MapServiceGoogleMapsTile(8800, 5373, 14);
            /// MapService maps = new MapService(tile);
            /// </code>
            /// </example>
            public Info() { }


            /// <summary><para>Abstrakte Basisklasse, aus der sich die nachfolgenden Klassen ableiten, um die für jeden
            /// <see cref="MapService"/> Dienst unterschiedlichen Satellitenbild Zugriffsdaten zu speichern. Die 
            /// Basisklasse wird für Boxing/Unboxing benötigt, beispielsweise in einem Konstruktor.</para></summary>
            public abstract class MapServiceTileBase
            {
                /// <summary><para>Der Standard-Konstruktor.</para></summary>
                protected MapServiceTileBase() { }

                /// <summary><para>Ermöglicht den Zugriff auf die URL-Eigenschaft der abgeleiteten Klassen von der Basisklasse aus.</para></summary>
                public abstract string URL { get; }
            }


            /// <summary><para>Die Klasse speichert einen internen generischen Schlüssel, der dazu dient einheitlich für 
            /// alle <see cref="MapService"/> Dienste Transformationen durchzuführen. Andernfalls müssten für jeden einzelnen 
            /// Dienst eigene Transformationsroutinen erstellt werden, was sehr schwer zu warten wäre.</para></summary>
            public class MapServiceInternalMapTile : MapServiceTileBase
            {
                private string _key;                                    // Speicherplatz für den internen Schlüssel

                /// <summary><para>Der Standard-Konstruktor.</para></summary>
                public MapServiceInternalMapTile() { }

                /// <summary><para>Konstruktor mit einem Parameter für den internen generischen Schlüssel.</para></summary>
                /// <param name="key">Der interne generische Schlüssel.</param>
                public MapServiceInternalMapTile(string key) { _key = key.ToLower(); }

                /// <summary><para>Die Eigenschaft Key gibt den internen generischen Schlüssel zurück, oder legt ihn fest.</para></summary>
                public string Key { get { return _key; } set { _key = value.ToLower(); } }

                /// <summary><para>Wird nur verwendet, um eine konsistente Memberstruktur aller <see cref="MapServiceTileBase"/> beizubehalten.</para></summary>
                override public string URL { get { return ""; } }

                /// <summary><para>Wird nur verwendet, um eine konsistente Memberstruktur aller <see cref="MapServiceTileBase"/> beizubehalten.</para></summary>
                /// <returns>Gibt den internen generische Schlüssel zurück.</returns>
                override public string ToString() { return _key; }
            }


            /// <summary><para>Die Klasse speichert die Schlüsseldaten, um auf ein Satelliten-/Luftbild des 
            /// <see cref="MapService"/> Providers Google Maps zuzugreifen.</para></summary>
            public class MapServiceGoogleMapsTile : MapServiceTileBase
            {
                private int _x;                                         // Speicherplatz für den X-Wert
                private int _y;                                         // Speicherplatz für den Y-Wert
                private int _zoom;                                      // Speicherplatz für die Zoomstufe

                /// <summary><para>Der Standard-Konstruktor.</para></summary>
                public MapServiceGoogleMapsTile() { }

                /// <summary><para>Konstruktor mit den Parametern für die Werte X, Y und Zoom.</para></summary>
                /// <param name="x">X-Wert des Google Maps Schlüssels.</param>
                /// <param name="y">Y-Wert des Google Maps Schlüssels.</param>
                /// <param name="zoom">Zoomstufe des Google Maps Schlüssels.</param>
                public MapServiceGoogleMapsTile(int x, int y, int zoom) { _x = x; _y = y; _zoom = zoom; }

                /// <summary><para>Die Eigenschaft X gibt den X-Wert des Satellitenbildes zurück, oder legt ihn fest.</para></summary>
                public int X { get { return _x; } set { _x = value; } }

                /// <summary><para>Die Eigenschaft Y gibt den Y-Wert des Satellitenbildes zurück, oder legt ihn fest.</para></summary>
                public int Y { get { return _y; } set { _y = value; } }

                /// <summary><para>Die Eigenschaft Zoom gibt die Zoomstufe des Satellitenbildes zurück, oder legt die fest.</para></summary>
                public int Zoom { get { return _zoom; } set { _zoom = value; } }

                /// <summary><para>Gibt die vollständige URL für das Satelliten-/Luftbild zum Google Maps Server zurück.</para></summary>
                override public string URL
                {
                    get
                    {
                        string serv = (new Random().Next(0, 3)).ToString();                    // Ladeverteilung auf verschiedene Server 
                        return string.Format(GOOGLE_TILE_SERVER, serv, this.ToString());
                    }
                }

                /// <summary><para>Gibt die Google Maps Zugriffsdaten auf das Satelliten-/Luftbild als String aus.</para></summary>
                /// <returns>Gibt den Google Maps Schlüssel zurück.</returns>
                override public string ToString()
                {
                    return "x=" + _x.ToString() + "&y=" + _y.ToString() + "&z=" + _zoom.ToString();
                }

            }


            /// <summary><para>Die Klasse speichert den Schlüssel, um auf ein Satelliten-/Luftbild des <see cref="MapService"/> 
            /// Providers Microsoft Virtual Earth zuzugreifen.</para></summary>
            public class MapServiceVirtualEarthMapsTile : MapServiceTileBase
            {
                private string _key;                                                // Speicherplatz für den Schlüssel

                /// <summary><para>Der Standard-Konstruktor.</para></summary>
                public MapServiceVirtualEarthMapsTile() { }

                /// <summary><para>Konstruktor mit einem Parameter für den Schlüssel zum Zugriff auf das Satellitenbild.</para></summary>
                /// <param name="key">Der Schlüssel des Virtual Earth Bildes.</param>
                public MapServiceVirtualEarthMapsTile(string key) { _key = key.ToLower(); }

                /// <summary><para>Die Eigenschaft Key gibt den Schlüssel zurück, oder legt ihn fest.</para></summary>
                public string Key { get { return _key; } set { _key = value.ToLower(); } }

                /// <summary><para>Gibt die vollständige URL für das Satelliten-/Luftbild zum Virtual Earth Server zurück.</para></summary>
                override public string URL
                {
                    get
                    {
                        if (_key == "") return "";

                        string serv = (new Random().Next(0, 3)).ToString();             // Ladeverteilung auf verschiedene Server 
                        return string.Format(VIRTUAL_EARTH_TILE_SERVER, serv, _key);
                    }
                }

                /// <summary><para>Gibt die Virtual Earth Zugriffsdaten auf das Satelliten-/Luftbild als String aus.</para></summary>
                /// <returns>Gibt den Virtual Earth Schlüssel zurück.</returns>
                override public string ToString()
                {
                    return _key;
                }

            }


            /// <summary><para>Die Klasse speichert die Schlüsseldaten, um auf ein Satelliten-/Luftbild des <see cref="MapService"/> 
            /// Providers Yahoo Maps zuzugreifen.</para></summary>
            public class MapServiceYahooMapsTile : MapServiceTileBase
            {
                private int _x;                                         // Speicherplatz für den X-Wert
                private int _y;                                         // Speicherplatz für den Y-Wert
                private int _zoom;                                      // Speicherplatz für die Zoomstufe

                /// <summary><para>Der Standard-Konstruktor.</para></summary>
                public MapServiceYahooMapsTile() { }

                /// <summary><para>Konstruktor mit den Parametern für die Werte X, Y und Zoom.</para></summary>
                /// <param name="x">X-Wert des Yahoo Maps Schlüssels.</param>
                /// <param name="y">Y-Wert des Yahoo Maps Schlüssels.</param>
                /// <param name="zoom">Zoomstufe des Yahoo Maps Schlüssels.</param>
                public MapServiceYahooMapsTile(int x, int y, int zoom) { _x = x; _y = y; _zoom = zoom; }

                /// <summary><para>Die Eigenschaft X gibt den X-Wert des Satellitenbildes zurück, oder legt ihn fest.</para></summary>
                public int X { get { return _x; } set { _x = value; } }

                /// <summary><para>Die Eigenschaft Y gibt den Y-Wert des Satellitenbildes zurück, oder legt ihn fest.</para></summary>
                public int Y { get { return _y; } set { _y = value; } }

                /// <summary><para>Die Eigenschaft Zoom gibt die Zoomstufe des Satellitenbildes zurück, oder legt die fest.</para></summary>
                public int Zoom { get { return _zoom; } set { _zoom = value; } }

                /// <summary><para>Gibt die vollständige URL für das Satelliten-/Luftbild zum Yahoo Maps Server zurück.</para></summary>
                override public string URL
                {
                    get
                    {
                        // OBSOLET: Keine Ladeverteilung bei Yahoo
                        // string serv = (new Random().Next(0, 3)).ToString();                    // Ladeverteilung auf verschiedene Server 
                        string serv = "3";
                        return string.Format(YAHOO_TILE_SERVER, serv, this.ToString());
                    }
                }

                /// <summary><para>Gibt die Yahoo Maps Zugriffsdaten auf das Satelliten-/Luftbild als String aus.</para></summary>
                /// <returns>Gibt den Yahoo Maps Schlüssel zurück.</returns>
                override public string ToString()
                {
                    return "x=" + _x.ToString() + "&y=" + _y.ToString() + "&z=" + _zoom.ToString();
                }
            }


            /// <summary><para>Die Klasse <see cref="TileInfo"/> hält alle verfügbaren Informationen über ein aktuelles Satelliten-/Luftbild, 
            /// und damit über das <see cref="MapService"/> Objekt, bereit. Diese Klasse kann damit als zentraler Zugangspunkt 
            /// für Informationen über das aktuelle <see cref="MapService"/> Objekt (bzw. Satellitenbild) angesehen werden. 
            /// <see cref="TileInfo"/> hält unter anderem Informationen über URLs zum Luftbild der verschiedenen MapService 
            /// Providern, Dimensionen des Bildes oder Bildpunkten bereit.</para></summary>
            public class TileInfo
            {
                private GeoRect _dimension;                         // Speicherplatz für die Dimension des Bildes
                private GeoPoint _position;                         // Speicherplatz für die geographische Position auf dem Bild
                private MapServiceInternalMapTile _internal;        // Speicherplatz für den internen Schlüssel  
                private MapServiceGoogleMapsTile _google;           // Speicherplatz für den Google Maps Schlüssel 
                private MapServiceVirtualEarthMapsTile _earth;      // Speicherplatz für den Virtual Earth Schlüssel 
                private MapServiceYahooMapsTile _yahoo;             // Speicherplatz für den Yahoo Maps Schlüssel 
                private int _zoom;                                  // Speicherplatz für die Zoomstufe
                private MapServiceURL _url = new MapServiceURL();   // Speicherplatz für die URLs


                /// <summary><para>Der Standard-Konstruktor.</para></summary>
                /// 
                /// <example>Das folgende Beispiel erzeugt eine Instanz der <see cref="MapService"/> Klasse und verwendet anschließend die 
                /// Informationen aus der <see cref="TileInfo"/> Klasse um eine URL von dem Satellitenbild auf dem gewählten <see cref="MapService"/> zu erhalten: 
                /// <code>
                /// using GeoUtility.GeoSystem;
                /// MapService map = new MapService(new Geographic(8.12345, 50.56789));
                /// Info.TileInfo info = map.Tile;
                /// string url = info.URL.GoogleMaps;                        // URL zum Satellitenbild auf dem Server des Dienstes
                /// </code>
                /// </example>        
                public TileInfo() { }

                /// <summary><para>Dimension des aktuellen Satellitenbildes in einer <see cref="GeoUtility.GeoSystem.Helper.GeoRect"/>-Klasse.</para></summary>
                public GeoRect Dimension { get { return _dimension; } set { _dimension = value; } }

                /// <summary><para>Bildpunkt (x,y) auf dem Satellitenbild, der der geographischen Koordinate des <see cref="MapService"/> Objektes 
                /// entspricht. Bitte beachten: Der Ursprung (Bildpunkt 0,0) ist links oben auf dem Luftbild.</para></summary>
                public GeoPoint GeoPosition { get { return _position; } set { _position = value; } }

                /// <summary><para>Interner Schlüssel als Typ <see cref="Info.MapServiceInternalMapTile"/>.</para></summary>
                public MapServiceInternalMapTile InternalMap { get { return _internal; } set { _internal = value; } }

                /// <summary><para>Google Maps Schlüssel als Typ <see cref="Info.MapServiceGoogleMapsTile"/>.</para></summary>
                public MapServiceGoogleMapsTile GoogleMaps { get { return _google; } set { _google = value; } }

                /// <summary><para>Virtual Earth Schlüssel als <see cref="Info.MapServiceVirtualEarthMapsTile"/>.</para></summary>
                public MapServiceVirtualEarthMapsTile VirtualEarth { get { return _earth; } set { _earth = value; } }

                /// <summary><para>Yahoo Maps  Schlüssel als  <see cref="Info.MapServiceYahooMapsTile"/>.</para></summary>
                public MapServiceYahooMapsTile YahooMaps { get { return _yahoo; } set { _yahoo = value; } }

                /// <summary><para>Aktuell für das Satellitenbild bzw. <see cref="MapService"/>-Objektes verwendete Zoomstufe.</para></summary>
                public int Zoom { get { return _zoom; } set { _zoom = value; } }

                /// <summary><para>URLs aller MapService Provider zu den Satellitenbildern unter Berücksichtigung der aktuellen Zoomstufe.</para></summary>
                public MapServiceURL URL { get { return _url; } set { _url = value; } }

            }


            /// <summary><para>Hilfsklasse für <see cref="TileInfo"/>. Hält die URLs der aktuell unterstützen 
            /// <see cref="MapService"/> Dienste zu den Satellitenbildern.</para></summary>
            public class MapServiceURL
            {
                private string _google;                             // Interner Speicherplatz für die GoogleMaps URL
                private string _earth;                              // Interner Speicherplatz für die Virtual Eath URL
                private string _yahoo;                              // Interner Speicherplatz für die YahooMaps URL

                /// <summary><para>Der Standard-Konstruktor.</para></summary>
                public MapServiceURL() { }

                /// <summary><para>Setzt die URL des aktuellen Satellitenbildes zum Google Maps Server oder gibt sie zurück.</para></summary>
                public string GoogleMaps { get { return _google; } set { _google = value; } }

                /// <summary><para>Setzt die URL des aktuellen Satellitenbildes zum Virtual Earth Server oder gibt sie zurück.</para></summary>
                public string VirtualEarth { get { return _earth; } set { _earth = value; } }

                /// <summary><para>Setzt die URL des aktuellen Satellitenbildes zum Yahoo Maps Server oder gibt sie zurück.</para></summary>
                public string YahooMaps { get { return _yahoo; } set { _yahoo = value; } }
            }


            /// <summary><para>Auflistung der aktuell unterstützen <see cref="MapService"/> Dienste.</para></summary>
            public enum MapServer
            {
                /// <summary><para>Google Maps</para></summary>
                GoogleMaps,

                /// <summary><para>Microsoft Virtual Earth</para></summary>
                VirtualEarth,

                /// <summary><para>Yahoo Maps</para></summary>
                YahooMaps
            }


            /// <summary><para>Auflistung der möglichen Richtungen bei der Bewegung eines Bildes der Funktion <see cref="MapService.Move"/></para></summary>
            public enum MapDirection
            {
                /// <summary><para>Bewegungsrichtung: Norden</para></summary>
                North,

                /// <summary><para>Bewegungsrichtung: Nordosten</para></summary>
                Northeast,

                /// <summary><para>Bewegungsrichtung: Osten</para></summary>
                East,

                /// <summary><para>Bewegungsrichtung: Südosten</para></summary>
                Southeast,

                /// <summary><para>Bewegungsrichtung: Süden</para></summary>
                South,

                /// <summary><para>Bewegungsrichtung: Südwesten</para></summary>
                Southwest,

                /// <summary><para>Bewegungsrichtung: Westen</para></summary>
                West,

                /// <summary><para>Bewegungsrichtung: Nordwesten</para></summary>
                Northwest,

                /// <summary><para>Bewegungsrichtung: Keine</para></summary>
                Center
            }

        }

    }
}
