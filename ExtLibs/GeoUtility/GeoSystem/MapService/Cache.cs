//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/GeoSystem/MapService/Cache.cs $
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
// File description   : Definition der statischen MapService.Helper.Images.Cache-Klasse 
//===================================================================================================


using System;
using System.Collections.Generic;
using System.Drawing;
using GeoUtility.GeoSystem.Base;

namespace GeoUtility.GeoSystem
{
    public partial class MapService : BaseSystem, IEquatable<MapService>
    {

        /// <summary><para><see cref="Helper"/> beinhaltet Klassen, die nicht zu den Kernfunktionen der 
        /// <see cref="MapService"/>-Klasse gehören, aber nützliche Aufgaben verrichten. Sie selbst implementiert 
        /// keine eigenen Methoden oder Eigenschaften, sondern dient der Strukturierung. Methoden und Eigenschaften 
        /// werden von den Unterklassen bereitgestellt.</para></summary>
        public partial class Helper
        {
            /// <summary><para>Die Klasse <see cref="Images"/> implementiert Methoden, mit deren Hilfe Bilder abgerufen 
            /// bzw. manipuliert werden können. Die Klasse sollte in der Regel nicht direkt instanziert werden, sondern 
            /// über die <see cref="MapService.Images"/>-Eigenschaft eines <see cref="MapService"/>-Objekts.</para></summary>
            public partial class Images
            {
                /// <summary><para>Die statische Klasse <see cref="Cache"/> implementiert Caching-Funktionalitäten 
                /// und verwendet dabei eine LRU-Strategie (Least recently used).</para></summary>
                /// 
                /// <example>Das folgende Beispiel zeigt die Verwendung des Satellitenbildcachings:
                /// <code>
                /// MapService.Helper.Images.Cache.Size = 50;        // Maximale Größe des Cache, hier 50 Bilder
                /// MapService.Helper.Images.Cache.Enabled = true;   // Cache aktivieren
                /// </code>
                /// </example>
                public static class Cache
                {
                    #region ==================== Klassenvariablen ====================
                    
                    internal static bool _enabled = false;
                    internal static int _size = 50;
                    //internal static Dictionary<MapService, Image> _cache;                       // Beinhaltet MapService-Objekt und dazugehörendes Bild
                    internal static SortedList<long, MapService> _lruList;                // Eine LRU-Liste
                    internal static CacheEqualityMode _equalityMode = CacheEqualityMode.Image;  // Vergleichsmodus, s. Enumeration
                    
                    #endregion ==================== Klassenvariablen ====================




                    #region ==================== Eigenschaften ====================

                    /// <summary><para>Aktiviert oder deaktiviert den Cache. Bei Deaktivierung werden alle eventuell 
                    /// vorhandenen Bilder aus dem Cache gelöscht.</para></summary>
                    public static bool Enabled
                    {
                        get
                        {
                            return _enabled;
                        }
                        set
                        {
                            if (value != _enabled)
                            {
                                _enabled = value;
                                if (value == true)
                                {
                                    _cache = new Dictionary<MapService, Image>(_size);
                                    _lruList = new SortedList<long, MapService>();
                                }
                                else
                                {
                                    _cache = null;
                                    _lruList = null;
                                }
                            }
                        }
                    }

                    /// <summary><para>Legt die maximale Cachegröße fest. Bei einer Änderung der Cachegröße, 
                    /// werden alle eventuell im Cache vorhandenen Bilder gelöscht.</para></summary>
                    public static int Size
                    {
                        get
                        {
                            return _size;
                        }
                        set
                        {
                            if (value != _size)
                            {
                                _size = value;
                                if (_size < 5) _size = 5;
                                else if (_size > 200) _size = 200;

                                if (_enabled == true)
                                {
                                    if ((_size < _cache.Count) || (_size < _lruList.Count)) Shrink(_size);
                                }
                            }
                        }
                    }

                    /// <summary><para>Gibt die Anzahl der gespeicherten Bilder im Cache zurück.</para></summary>
                    public static int Count 
                    { 
                        get 
                        {
                            if ((_enabled == false) || (_cache == null)) return 0;
                            else return _cache.Count; 
                        } 
                    }

                    /// <summary><para>Gibt den internen Cachespeicher zurück.</para></summary>
                    internal static Dictionary<MapService, Image> ImageCache { get { return _cache; } }


                    /// <summary><para>Gibt eine Auflistung der im Cache vorhandenen <see cref="MapService"/>-Objekte 
                    /// zurück, die mit den gespeicherten Bildern assoziiert sind.</para></summary>
                    public static Dictionary<MapService, Image>.KeyCollection Maps { get { return _cache.Keys; } }

                    /// <summary><para>Gibt eine Auflistung der im Cache gespeicherten Bilder zurück.</para></summary>
                    public static Dictionary<MapService, Image>.ValueCollection Images { get { return _cache.Values; } }

                    /// <summary><para>Legt fest, wie interne Vergleichsoperationen durchgeführt werden. 
                    /// Nähere Informationen unter <see cref="CacheEqualityMode"/>.</para></summary>
                    public static CacheEqualityMode EqualityMode { get { return _equalityMode; } set { _equalityMode = value; } }

                    #endregion ==================== Eigenschaften ====================




                    #region ==================== Methoden ====================

                    /// <summary><para>Die interne Funktion fügt ein neues <see cref="MapService"/>/<see cref="Image"/>-Paar mit 
                    /// Hilfe der <see cref="CacheItem"/>-Klasse zum Cache hinzu. Verfügt der Cache bereits über die 
                    /// maximale Anzahl an Bildern, wird mit Hilfe der LRU-Strategie zuvor ein Bild gelöscht.</para></summary>
                    /// 
                    /// <param name="item">Ein <see cref="CacheItem"/>-Objekt, das hizugefügt werden soll.</param>
                    internal static void Add(CacheItem item)
                    {
                        if (_cache.Count >= _size)
                        {
                            if (Shrink(_size - 1) == false) Clear();
                        }

                        _cache[item.Map] = item.SatImage;
                        _lruList.Add(DateTime.Now.Ticks, item.Map);
                    }

                    /// <summary><para>Löscht alle Elemente aus dem Cache.</para></summary>
                    internal static void Clear()
                    {
                        _cache = new Dictionary<MapService, Image>(_size);
                        _lruList = new SortedList<long, MapService>();
                    }

                    /// <summary><para>Prüft, ob ein Bild bereits im Cache vorhanden ist.</para></summary>
                    /// 
                    /// <param name="item">Ein <see cref="MapService"/>-Objekt, das das Bild repräsentiert.</param>
                    /// <returns>True, wenn ein passendes Bild vorhanden ist, sonst False.</returns>
                    internal static bool Contains(MapService item)
                    {
                        return _cache.ContainsKey(item);
                    }

                    /// <summary><para>Verkleinert die Cachegröße.</para></summary>
                    /// 
                    /// <param name="size">Neue Größe.</param>
                    /// <returns>True, wenn der Vorgang erfolgreich war, sonst False.</returns>
                    public static bool Shrink(int size)
                    {
                        CacheEqualityMode em = _equalityMode;
                        _equalityMode = CacheEqualityMode.MapService;
                        MapService map = null;
                        bool result = false;
                        long key = -1;

                        if (_cache.Count < size)
                        {
                            result = true;
                        }
                        else if (_lruList.Count != _cache.Count)
                        {
                            Clear();
                            result = true;
                        }
                        else
                        {
                            try
                            {
                                while (_cache.Count > size)
                                {
                                    foreach (KeyValuePair<long, MapService> keys in _lruList)
                                    {
                                        key = keys.Key;
                                        map = keys.Value;
                                        break;
                                    }
                                    if (_cache.ContainsKey(map) == true)
                                    {
                                        if (_cache.Remove(map) == true) _lruList.Remove(key);
                                        else Clear();
                                    }
                                    else
                                    {
                                        Clear();
                                    }
                                }
                                result = true;
                            }
                            catch
                            {
                                Clear();
                                result = true;
                            }
                        }
                        _equalityMode = em;
                        return result;
                    }

                    /// <summary><para>Löscht ein Bild aus dem Cache.</para></summary>
                    /// 
                    /// <param name="map">Ein <see cref="MapService"/>-Objekt, das das Bild repräsentiert.</param>
                    /// <returns>True, wenn ein Bild gelöscht wurde, sonst False.</returns>
                    public static bool Remove(MapService map)
                    {
                        CacheEqualityMode em = _equalityMode;
                        _equalityMode = CacheEqualityMode.MapService;

                        long key = -1;
                        foreach( KeyValuePair<long, MapService> keys in _lruList )
                        {
                            if (keys.Value == map)
                            {
                                key = keys.Key;
                                break;
                            }
                        }
                        if (key != -1)
                        {
                            if (_cache.ContainsKey(map) == true)
                            {
                                if (_cache.Remove(map) == true) _lruList.Remove(key);
                            }
                        }

                        _equalityMode = em;
                        return (key != -1);
                    }

                    /// <summary><para>Die Methode <see cref="GetImage"/> versucht ein entsprechendes Bild im Cache 
                    /// zu finden. Ist kein Bild vorhanden, gibt die Methode null zurück.</para></summary>
                    /// 
                    /// <param name="item">Ein <see cref="MapService"/>-Objekt, welches ein Bild repräsentiert.</param>
                    /// <returns>Das Satellitenbild, oder null, wenn kein passendes Bild gefunden wurde.</returns>
                    public static Image GetImage(MapService item)
                    {
                        Image image = null;
                        _cache.TryGetValue(item, out image);
                        return image;
                    }
                }

                #endregion ==================== Methoden ====================




                #region ==================== Hilfsklassen ====================

                /// <summary><para>Repräsentiert ein Cache-Element. Es besteht aus einem <see cref="MapService"/>-Objekt 
                /// und einem dazugehörenden Bild.</para></summary>
                public class CacheItem
                {
                    internal Image _image;
                    internal MapService _map;


                    /// <summary><para>Standart-Kontruktor.</para></summary>
                    public CacheItem() { }

                    /// <summary><para>Konstruktor mit den Parametern für das Bild, und das 
                    /// <see cref="MapService"/>-Objekt.</para></summary>
                    /// 
                    /// <param name="map">Das <see cref="MapService"/>-Objekt.</param>
                    /// <param name="image">Das Satellitenbild als Typ <see cref="Image"/>-Objekt.</param>
                    public CacheItem(MapService map, Image image)
                    {
                        _image = image;
                        _map = map.MemberwiseClone();
                    }



                    /// <summary><para>Das Satellitenbild.</para></summary>
                    public Image SatImage { get { return _image; } set { _image = value; } }

                    /// <summary><para>Das <see cref="MapService"/>-Objekt.</para></summary>
                    public MapService Map { get { return _map; } set { _map = value; } }


                }

                /// <summary><para>Mit Hilfe dieser Enumeration kann gesteuert werden, wie Vergleiche mit 
                /// Cache-Elementen durchgeführt werden. Bei der Wahl von <strong>MapService</strong> wird die 
                /// Wertgleichheit zweier <see cref="MapService"/>-Objekte getestet. Bei der Wahl von 
                /// <strong>Image</strong> werden mehr Bilder gefunden. Hier wird auch getestet, ob ein 
                /// <see cref="MapService"/>-Objekt den gleichen Bildbereich abdeckt. Genauer gesagt, werden 
                /// auch Bilder im Cache gefunden, bei denen ein <see cref="MapService"/>-Objekt unzentriert ist, und ein 
                /// <see cref="MapService"/>-Objekt zwar zentriert, sich die Koordinaten aber genau in der Bildmitte befinden. 
                /// Insbesondere sind davon Bilder betroffen, die als Umgebungsbilder geladen wurden und damit 
                /// den gleichen Bildbereich wie das entsprechende unzentrierte Bild besitzen.</para></summary>
                public enum CacheEqualityMode
                { 
                    /// <summary>Versuch ein Bild im Cach zu finden, auch wenn die Geo-Koordinaten 
                    /// beider <see cref="MapService"/>-Objekte nicht übereinstimmen, beide aber auf das gleiche Bild verweisen.</summary>
                    Image,

                    /// <summary>Prüft nur die Wertgleichheit zweier <see cref="MapService"/>-Objekte.</summary>
                    MapService
                }

                #endregion ==================== Hilfsklasse ====================

            }
        }
    }
}
