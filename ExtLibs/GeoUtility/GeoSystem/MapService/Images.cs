//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/GeoSystem/MapService/Images.cs $
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
// File description   : Definition der privaten MapService.Helper.Images Klasse 
//===================================================================================================


using System;
using System.Drawing;
using System.IO;
using System.Net;
using GeoUtility.GeoSystem.Base;
using System.Collections.Generic;

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
            /// <summary><para>Der Standard-Konstruktor.</para></summary>
            public Helper() { }


            /// <summary><para>Die Klasse <see cref="Images"/> implementiert Methoden, mit deren Hilfe Bilder abgerufen 
            /// bzw. manipuliert werden können. Die Klasse sollte in der Regel nicht direkt instanziert werden, sondern 
            /// über die <see cref="MapService.Images"/>-Eigenschaft eines <see cref="MapService"/>-Objekts.</para></summary>
            public partial class Images
            {

                #region ==================== Membervariablen ====================

                private MapService _parent = null;                              // Speicherplatz für die aktuelle MapService-Instanz.

                #endregion ==================== Membervariablen ====================




                #region ==================== Konstruktoren ====================

                /// <summary><para>Der Standard-Konstruktor.</para></summary>
                public Images() { }


                /// <summary><para>Ein Konstruktor mit einem Parameter als Verweis auf die aktuelle MapService-Instanz.</para></summary>
                /// <param name="parent">Die aktuelle MapService-Instanz.</param>
                public Images(MapService parent)
                {
                    _parent = parent;
                }

                #endregion ==================== Konstruktoren ====================




                #region ==================== Eigenschaften ====================

                /// <summary><para>Die interne Eigenschaft legt den Verweis auf die aktuelle MapService-Instanz fest, oder ruft sie ab.</para></summary>
                internal MapService Parent { get { return _parent; } set { _parent = value; } }

                /// <summary><para>Die Eigenschaft legt fest, ob Caching verwendet werden soll, oder ruft dei aktuelle Einstellungab.</para></summary>
                public bool Cached { get { return MapService.Helper.Images.Cache.Enabled; } set { MapService.Helper.Images.Cache.Enabled = value; } }

                /// <summary><para>Die Eigenschaft legt die Cachegröße fest oder ruft sie ab.</para></summary>
                public int CacheSize { get { return MapService.Helper.Images.Cache.Size; } set { MapService.Helper.Images.Cache.Size = value; } }
                

                #endregion ==================== Eigenschaften ====================




                #region ==================== Methoden ====================

                /// <summary><para>Die überladene Funktion <see cref="Load()"/> lädt das Satellitenbild, welches durch das aktuelle  
                /// <see cref="MapService"/>-Objekt definiert ist. Eventuell auftretende Exceptions werden unterdrückt.</para></summary>
                /// 
                /// <example>
                /// Das folgende Beispiel zeigt die Verwendung der Funktion. Der Beispielcode verwendet ein
                /// <strong>PictureBox</strong>-Steuerelement zur Anzeige des Bildes.
                /// <code>
                /// using System.Drawing;
                /// using GeoUtility.GeoSystem;
                /// Geographic geo = new Geographic(8.12345, 50.56789);
                /// MapService.Info.MapServer server = MapService.Info.MapServer.VirtualEarth;
                /// MapService map = new MapService(geo, server);
                /// map.Zoom = 15;
                /// Image img = map.Image.Load();
                /// picturebox.Image = img;
                /// </code>
                /// </example>
                /// 
                /// <returns>Ein Bild vom allgemeinen Typ <see cref="System.Drawing.Image"/>.</returns>
                public Image Load()
                {
                    if (Parent.Center == true) return Load(true, 0, true);
                    return Load(this.Parent.Tile, true);
                }

                /// <summary><para>Die überladene Funktion <see cref="Load(int, bool)"/> lädt das Satellitenbild, welches durch das aktuelle  
                /// <see cref="MapService"/>-Objekt definiert ist. Es wird ein Bild in der Größe der übergebenen Parameter 
                /// zurückgegeben. Eventuell auftretende Exceptions werden unterdrückt.</para></summary>
                /// 
                /// <example>
                /// Das folgende Beispiel zeigt die Verwendung der Funktion. Der Beispielcode verwendet ein
                /// <strong>PictureBox</strong>-Steuerelement zur Anzeige des Bildes.
                /// <code>
                /// using System.Drawing;
                /// using GeoUtility.GeoSystem;
                /// Geographic geo = new Geographic(8.12345, 50.56789);
                /// MapService.Info.MapServer server = MapService.Info.MapServer.VirtualEarth;
                /// MapService map = new MapService(geo, server);
                /// map.Zoom = 15;
                /// Image img = map.Image.Load(200, 200);
                /// picturebox.Image = img;
                /// </code>
                /// </example>
                /// <param name="size">Größe des zurückgegebenen Bildes in Pixel (20 - 2000)</param>
                /// <param name="mark">Position markieren</param>
                /// <returns>Ein Bild vom allgemeinen Typ <see cref="System.Drawing.Image"/>.</returns>
                public Image Load(int size, bool mark)
                {
                    if (size < 20) size = 20; else if (size > 2000) size = 2000;

                    Image image = null;
                    Image sizedImage = new Bitmap(size, size);

                    if (Parent.Center == true) image = Load(true, 0, true);
                    else image = Load(this.Parent.Tile, true);

                    if (image != null)
                    {
                        Graphics graph = Graphics.FromImage(sizedImage);
                        Rectangle destRect = new Rectangle(0, 0, size, size);
                        Rectangle sourceRect = new Rectangle(0,0, image.Width, image.Height);
                        graph.DrawImage(image, destRect, sourceRect, GraphicsUnit.Pixel);

                        if (mark == true)
                        {
                            int markSize = 2;
                            if (Parent.Zoom >= 18) markSize = 14;
                            else if (Parent.Zoom >= 15) markSize = 12;
                            else if (Parent.Zoom >= 12) markSize = 10;
                            else if (Parent.Zoom >= 9) markSize = 8;
                            else if (Parent.Zoom >= 6) markSize = 6;
                            else if (Parent.Zoom >= 3) markSize = 4;

                            double ratio = (double)size / (double)TILE_SIZE; 
                            markSize = Convert.ToInt32((double)markSize * ratio);
                            if (markSize == 0) markSize = 2;

                            GeoSystem.Helper.GeoPoint point = Parent.TileInfo.GeoPosition;
                            if (Parent.Center == true) point = new GeoUtility.GeoSystem.Helper.GeoPoint(TILE_SIZE / 2, TILE_SIZE / 2);
                            int left = Convert.ToInt32((double)point.X * ratio) - markSize / 2; 
                            int top = Convert.ToInt32((double)point.Y * ratio) - markSize / 2; 
                            Rectangle rect = new Rectangle(left, top, markSize, markSize);

                            Pen pen = new Pen(Color.Red, 1);
                            graph.DrawRectangle(pen, rect);
                        }
                        return sizedImage;
                    }
                    return null;
                }

                /// <summary><para>Die überladene Funktion <see cref="Load()"/> lädt das Satellitenbild, welches durch das aktuelle  
                /// <see cref="MapService"/>-Objekt definiert ist.</para></summary>
                /// 
                /// <param name="silent">Legt fest, ob im Fehlerfall eine Exception weitergegeben wird (false) oder nicht (true).</param>
                /// <returns>Ein Bild vom allgemeinen Typ <see cref="System.Drawing.Image"/>.</returns>
                public Image Load(bool silent)
                {
                    if (Parent.Center == true) return Load(true, 0, silent);
                    return Load(this.Parent.Tile, silent);
                }


                /// <summary><para>Die Funktion <see cref="Load(bool, int, bool)"/> gibt ein Satellitenbild zurück, so dass die
                /// so dass die Koordinate, die durch das <see cref="MapService"/>-Objekt festgelegt ist, sich in der Mitte
                /// des Bildes befindet.</para></summary>
                /// <remarks><para>Die Koordinate befindet sich in der Regel nicht in der Mitte eines von einem Provider 
                /// zurück gelieferten Satellitenbildes. Diese Funktion berechnet die Abweichung, und lädt intern die notwendigen 
                /// angrenzenden Satellitenbilder nach, fügt sie zusammen und beschneidet das so entstandene Bild auf die 
                /// Größe eines normalen Satellitenbildes. Über den Parameter <i>threshold</i> kann angegeben werden, ab welcher 
                /// Abweichung vom Mittelpunkt eines Bildes eine Koordinatennormalisierung stattfinden soll. Bitte beachten 
                /// Sie, dass bei einer notwendigen Normalisierung 4 Bilder nachgeladen werden müssen. Dies kann bei langsamen 
                /// Internetverbindungen eventuell zu kurzen Verzögerungen oder erhöhtem Netzverkehr führen.</para></remarks>
                /// 
                /// <param name="centered">Legt fest, ob der zugrundeliegende Koordinatenpunkt auf dem resultierenden Bild zentriert werden soll.</param>
                /// <param name="threshold">Abweichung vom Mittelpunkt in Bildpunkten (Pixel), ab der eine Normalisierung durchgeführt werden soll. Mögliche Werte liegen zwischen 0 (immer normalisieren) bis 100 (keine Normalisierung).</param>
                /// <param name="silent">Legt fest, ob im Fehlerfall eine Exception weitergegeben wird (false) oder nicht (true).</param>
                /// <returns>Ein Bild vom Typ <see cref="System.Drawing.Image"/> mit einem eventuell zentrierten Koordinatenpunkt.</returns>
                public Image Load(bool centered, int threshold, bool silent)
                {
                    Image image = new Bitmap(TILE_SIZE, TILE_SIZE);
                    int shiftX, shiftY, shift, radius;
                    int halfsize = TILE_SIZE / 2;
                    GeoSystem.Helper.GeoPoint point = this.Parent.TileInfo.GeoPosition;

                    // Radius mit Thresholdwert auf alle möglichen Bildgrößen umrechnen
                    if ((threshold < 0) || (threshold > 100)) threshold = 0;
                    radius = Convert.ToInt32(threshold * halfsize / 100);               

                    // Abstand vom Mittelpunkt (Pythagoras)
                    shiftX =  point.X - halfsize;
                    shiftY =  point.Y - halfsize;
                    Rectangle crop = new Rectangle(0, 0, TILE_SIZE, TILE_SIZE);
                    shift = Convert.ToInt32(Math.Sqrt((shiftX * shiftX) + (shiftY * shiftY)));

                    if ((shift <= radius) || (centered == false))        // Koordinate liegt innerhalb Schwellwert oder keine Normalisierung
                    {
                        image = Load(this.Parent.Tile, silent);
                    }
                    else
                    {
                        // Umgebungsbild laden, so dass die gesuchte Koordinate nahe am Zentrum liegt
                        Image area = null; 
                        if ((shiftX <= 0) && (shiftY <= 0)) area = this.Area(Info.MapDirection.Northwest, false, out point);
                        else if ((shiftX > 0) && (shiftY <= 0)) area = this.Area(Info.MapDirection.Northeast, false, out point);
                        else if ((shiftX <= 0) && (shiftY > 0)) area = this.Area(Info.MapDirection.Southwest, false, out point);
                        else if ((shiftX > 0) && (shiftY > 0)) area = this.Area(Info.MapDirection.Southeast, false, out point);
                        if (area == null) return null;
                        
                        // Umgebungsbild so beschneiden, dass die Koordinate genau im Zentrum liegt
                        crop.X = point.X - halfsize;
                        crop.Y = point.Y - halfsize;
                        Graphics graph = Graphics.FromImage(image);
                        Rectangle dest = new Rectangle(0, 0, TILE_SIZE, TILE_SIZE);
                        graph.DrawImage(area, dest, crop, GraphicsUnit.Pixel);
                    }

                    return image;
                }


                /// <summary><para>Die überladene Funktion <see cref="Load(Info.MapServiceTileBase, bool)"/> lädt das Satellitenbild, welches durch den Parameter vom Typ 
                /// <see cref="MapService.Info.MapServiceTileBase"/> festgelegt wurde von dem gewünschten MapService-Provider. 
                /// Ein Beispiel ist in der Funktion <see cref="Load()"/> zu finden.</para></summary>
                /// 
                /// <param name="tile">Ein Objekt vom Typ <see cref="MapService.Info.MapServiceTileBase"/>, das über die <see cref="MapService.Tile"/>-Eigenschaft des <see cref="MapService"/>-Objektes erhalten werden kann.</param>
                /// <param name="silent">Legt fest, ob im Fehlerfall eine Exception weitergegeben wird (false) oder nicht (true).</param>
                /// <returns>Ein Bild vom allgemeinen Typ <see cref="System.Drawing.Image"/>.</returns>
                public Image Load(Info.MapServiceTileBase tile, bool silent)
                {
                    Image image = null;
                    Stream data = null;
                    HttpWebResponse response = null;

                    // Erst versuchen Bild, aus dem Cach zu holen
                    if (MapService.Helper.Images.Cache.Enabled == true)
                    {
                        image = MapService.Helper.Images.Cache.GetImage(this.Parent);
                        if (image != null) return image;
                    }

                    // Bild nicht im Cache, oder Cache nicht verwendet
                    if ((tile != null) && (tile.URL != ""))
                    {
                        System.Net.HttpWebRequest request = (HttpWebRequest)WebRequest.Create(tile.URL);
                        request.UserAgent = USER_AGENT;
                        request.Timeout = 30000;
                        try
                        {
                            response = (HttpWebResponse)request.GetResponse();
                            data = response.GetResponseStream();
                            image = new Bitmap(data);

                            // Fehlerbild Virtual Earth und Yahoo Maps erkennen
                            if (this.Parent.MapServer != Info.MapServer.GoogleMaps)
                            {
                                bool isValidImage = ValidImage(image);
                                ErrorProvider.ErrorMessage err = new ErrorProvider.ErrorMessage("MS_NO_IMAGE");
                                if (isValidImage == false) throw new WebException(err.Text, WebExceptionStatus.ProtocolError);
                            }

                            // Bild im Cache ablegen
                            if (MapService.Helper.Images.Cache.Enabled == true)
                            { 
                                MapService m;
                                if (tile.URL == Parent.Tile.URL) m = this.Parent.MemberwiseClone();
                                else m = new MapService(tile);
                                MapService.Helper.Images.Cache.Add(new CacheItem(m, image));
                            }
                        }
                        catch (WebException ex)
                        {
                            image = null;
                            if (!silent) throw new Exception(ex.Message, ex);

                        }
                        catch (Exception ex)
                        {
                            image = null;
                            if (!silent) throw new Exception(ex.Message, ex);
                        }
                        finally
                        {
                            if (response != null) response.Close();
                            if (data != null) data.Close();
                        }
                    }
                    return image;
                }

                /// <summary>
                /// Versucht zu erkennen, ob das vom Virtual Earth Dienst zurückgegebene Bild ein Fehlerbild oder
                /// ein reguläres Satellitenbild ist. Andere Provider geben einen HTTP 404 Status zurück.
                /// </summary>
                /// <param name="image">Das zu überprüfende Bild</param>
                /// <returns>True, wenn es sich um ein Satellitenbild handelt. False, wenn es ein Fehlerbild ist.</returns>
                private bool ValidImage(Image image)
                {
                    bool isValid = false;
                    if (image != null)
                    {
                        Bitmap b = new Bitmap(image);
                        Color pixel = Color.Transparent; 
                        Color refpixel = Color.Transparent;

                        for (int i = 1; i <= 10; i++)
                        {
                            pixel = b.GetPixel(i, i);
                            if (i == 1)
                            {
                                refpixel = pixel;
                            }
                            else if (refpixel != pixel)
                            {
                                isValid = true;
                                break;
                            }
                        }
                        if (isValid) return true;

                        for (int i = 255; i >= 246; i--)
                        {
                            pixel = b.GetPixel(i, i);
                            if (refpixel != pixel)
                            {
                                isValid = true;
                                break;
                            }
                        }
                    }
                    return isValid;
                }


                /// <summary><para>Die überladene Funktion <see cref="Merge(Image[,])"/> verschmilzt mehrere Bilder, und gibt ein 
                /// Bild als Typ <see cref="System.Drawing.Image"/> zurück. Die Bilder werden in einem Image-Array übergeben. 
                /// Der erste Index bezeichnet die Zeilen, der zweite Index die Spalten der Bildermatrix. 
                /// In dieser Methode sollten die Bilder in der Höhe und Breite den Standardmaßen eines Satellitenbilds entsprechen (256 Pixel). 
                /// Prinzipiell ist die Anzahl der Zeilen und Spalten des Arrays nicht beschränkt, beachten Sie aber dennoch den 
                /// Speicherbedarf. Die <see cref="System.Drawing.Image"/>-Objekte in dem Array können auch <strong>null</strong> sein. 
                /// Die Einzelbilder im Array sollten jeweils die gleiche Breite und Höhe haben. Beispiel siehe 
                /// <see cref="Merge(Image[,], Rectangle)"/>.</para></summary>
                /// 
                /// <param name="array">Ein Array aus <see cref="System.Drawing.Image"/>-Objekten in der Form Image[Zeilen, Spalten].</param>
                /// <returns>Ein verschmolzenes <see cref="System.Drawing.Image"/>-Objekt.</returns>
                public Image Merge(Image[,] array)
                { 
                    return Merge(array, new Rectangle(0, 0, TILE_SIZE, TILE_SIZE));
                }


                /// <summary><para>Die überladene Funktion <see cref="Merge(Image[,], Rectangle)"/> verschmilzt mehrere Bilder, und gibt ein 
                /// Bild als Typ <see cref="System.Drawing.Image"/> zurück. Die Bilder werden in einem Image-Array übergeben. 
                /// Der erste Index bezeichnet die Zeilen, der zweite Index die Spalten der Bildermatrix. 
                /// Die Höhe und Breite der Einzelbilder im Array kann durch den Parameter angegeben werden. 
                /// Prinzipiell ist die Anzahl der Zeilen und Spalten des Arrays nicht beschränkt, beachten Sie aber dennoch den 
                /// Speicherbedarf. Die <see cref="System.Drawing.Image"/>-Objekte in dem Array können auch <strong>null</strong> sein. 
                /// Die Einzelbilder im Array sollten jeweils die gleiche Breite und Höhe haben.</para></summary>
                /// 
                /// <example>Das Beispiel zeigt die Verschmelzung einer Bildermatrix aus 2 mal 2 Bildern. 
                /// <code>
                /// using System.Drawing;
                /// using GeoUtility.GeoSystem;
                /// Image[,] array = new Image[2, 2];
                /// array[0, 0] = new Bitmap(@"c:\norrthwest.jpg");
                /// array[0, 1] = new Bitmap(@"c:\northeast.jpg");
                /// array[1, 0] = new Bitmap(@"c:\southwest.jpg");
                /// array[1, 1] = new Bitmap(@"c:\southeast.jpg");
                /// Rectangle rect = new Rectangle(0, 0, 256, 256);                     // Höhe und Breite der Bilder ist 256 Pixel (Bildpunkte)           
                /// Image mergedImage = MapService.Image.Merge(array, rect);
                /// </code>
                /// </example>
                /// 
                /// <param name="array">Ein Array aus <see cref="System.Drawing.Image"/>-Objekten in der Form Image[Zeilen, Spalten].</param>
                /// <param name="rect">Der <see cref="System.Drawing.Rectangle"/>-Parameter legt die Höhe und Breite eines Einzelbildes fest.</param>
                /// <returns>Ein verschmolzenes <see cref="System.Drawing.Image"/>-Objekt.</returns>
                public Image Merge(Image[,] array, Rectangle rect)
                {
                    int rows = array.GetUpperBound(0);
                    int cols = array.GetUpperBound(1);
                    
                    Image image = new Bitmap(rect.Width * (cols + 1), rect.Height * (rows + 1));
                    Graphics graph = Graphics.FromImage(image);

                    for (int i = 0; i <= rows; i++)
                    {
                        for (int j = 0; j <= cols; j++)
                        {
                            if (array[i, j] != null)
                            {
                                graph.DrawImage(array[i, j], rect.Width * j, rect.Height * i);
                            }
                        }
                    }
                    //graph.Save();
                    return image;
                }


                /// <summary><para>Die Funktion ruft Einzelbilder aus der Umgebung des aktuellen Satellitenbilds ab, und fügt sie zu 
                /// einem Bild zusammen. Der Parameter vom Typ <see cref="Info.MapDirection"/> bestimmt die Richtung, in der 
                /// die Bilder abgerufen werden. Der Wert <see cref="Info.MapDirection.Center"/> fügt alle angrenzenden Luftbilder, 
                /// also 9 Einzelbilder in einer 3x3 Matrix, zu einem Bild zusammen. Die Werte <see cref="Info.MapDirection.North"/>, 
                /// <see cref="Info.MapDirection.South"/>, <see cref="Info.MapDirection.West"/> und <see cref="Info.MapDirection.East"/> 
                /// fügen 6 Einzelbilder in einer 2x3 bzw. 3x2 Matrix zusammen. Alle anderen Werte der <see cref="Info.MapDirection"/>-Enumeration 
                /// fügen 4 Einzelbilder in einer 2x2 Matrix zusammen. Bitte beachten Sie, dass der Vorgang je nach Verbindung oder 
                /// Serverauslastung längern andauern kann, und es möglicherweise zu Fehlern kommt.</para></summary>
                /// 
                /// <example>Das Beispiel zeigt eine mögliche Anwendung der Methode, indem das aktuelle Satellitenbild, und alle 
                /// angrenzenden Bilder (9x9 Matrix) zu einem Gesamtbild verschmolzen werden. 
                /// <code>
                /// using System.Drawing;
                /// using GeoUtility.GeoSystem;
                /// Geographic geo = new Geographic(8.12345, 50.56789);
                /// MapService.Info.MapServer server = MapService.Info.MapServer.GoogleMaps;
                /// MapService map = new MapService(geo, server);
                /// map.Zoom = 18;
                /// Image imageArea = map.Image.Area(MapService.Info.MapDirection.Center);
                /// </code>
                /// </example>
                /// 
                /// <param name="direction">Bestimmt die Richtung in der die Bilder zusammengefügt werden sollen.</param>
                /// <param name="extended">Wenn True, wird ein erweiterter Umgebungsbereich geladen.</param>
                /// <returns>Ein zusammengesetztes Bild vom Typ <see cref="System.Drawing.Image"/></returns>
                public Image Area(Info.MapDirection direction, bool extended)
                {
                    Image[,] array = null;
                    //MapService maps = Parent.MemberwiseClone();
                    MapService maps = new MapService(Parent.Tile);
                    int rows, row, cols, col;
                    rows = row = cols = col = 0;

                    if (direction == Info.MapDirection.Center)
                    {
                        rows = 3;
                        cols = 3;
                        maps.Move(MapService.Info.MapDirection.Northwest, 1, true);
                    }
                    else if ((direction == Info.MapDirection.Northwest) || (direction == Info.MapDirection.Northeast) || (direction == Info.MapDirection.Southwest) || (direction == Info.MapDirection.Southeast))
                    {
                        rows = 2;
                        cols = 2;
                        if (direction == Info.MapDirection.Northwest) maps.Move(MapService.Info.MapDirection.Northwest, 1, true);
                        else if (direction == Info.MapDirection.Northeast) maps.Move(MapService.Info.MapDirection.North, 1, true);
                        else if (direction == Info.MapDirection.Southwest) maps.Move(MapService.Info.MapDirection.West, 1, true);
                    }
                    else if ((direction == Info.MapDirection.North) || (direction == Info.MapDirection.South))
                    {
                        rows = 2;
                        cols = 3;
                        if (direction == Info.MapDirection.North) maps.Move(MapService.Info.MapDirection.Northwest, 1, true);
                        else maps.Move(MapService.Info.MapDirection.West, 1, true);
                    }
                    else if ((direction == Info.MapDirection.West) || (direction == Info.MapDirection.East))
                    {
                        rows = 3;
                        cols = 2;
                        if (direction == Info.MapDirection.West) maps.Move(MapService.Info.MapDirection.Northwest, 1, true);
                        else maps.Move(MapService.Info.MapDirection.North, 1, true);
                    }
                    if (extended == true)
                    {
                        rows += 2;
                        cols += 2;
                        maps.Move(MapService.Info.MapDirection.Northwest, 1, true);
                    }

                    array = new Image[rows, cols];
                    for (row = 0; row < rows; row++)
                    {
                        for (col = 0; col < cols; col++)
                        {
                            Image image = maps.Images.Load(true);
                            if (image == null) return null;
                            
                            array[row, col] = image;
                            maps.Move(MapService.Info.MapDirection.East, 1, true);
                        }
                        maps.Move(MapService.Info.MapDirection.West, cols, true);
                        maps.Move(MapService.Info.MapDirection.South, 1, true);
                    }

                    return this.Merge(array);
                }


                /// <summary><para>Die Methode gibt zusätzlich zur <see cref="Area(Info.MapDirection, bool)"/>-Methode den Bildpunkt (x/y)
                /// auf dem Satellitenbild zurück, der der Koordinate entspricht.</para></summary>
                /// 
                /// <param name="direction">Bestimmt die Richtung in der die Bilder zusammengefügt werden sollen.</param>
                /// <param name="extended">Wenn True, wird ein erweiterter Umgebungsbereich geladen.</param>
                /// <param name="pointInArea">Der out-Parameter gibt den Bildpunkt der Koordinate im Umgebungsbild zurück.</param>
                /// <returns>Ein zusammengesetztes Bild vom Typ <see cref="System.Drawing.Image"/></returns>
                public Image Area(Info.MapDirection direction, bool extended, out GeoUtility.GeoSystem.Helper.GeoPoint pointInArea)
                {
                    GeoUtility.GeoSystem.Helper.GeoPoint gp = Parent.TileInfo.GeoPosition;
                    pointInArea = new GeoUtility.GeoSystem.Helper.GeoPoint();
                    pointInArea.X = gp.X;
                    pointInArea.Y = gp.Y;

                    if ((direction == Info.MapDirection.Northwest) || (direction == Info.MapDirection.North) || (direction == Info.MapDirection.West) || (direction == Info.MapDirection.Center))
                    {
                        pointInArea.X += TILE_SIZE;
                        pointInArea.Y += TILE_SIZE;
                    }
                    if ((direction == Info.MapDirection.Northeast) || (direction == Info.MapDirection.East))
                    {
                        pointInArea.Y += TILE_SIZE;
                    }
                    if ((direction == Info.MapDirection.Southwest) || (direction == Info.MapDirection.South))
                    {
                        pointInArea.X += TILE_SIZE;
                    }
                    if (extended == true)
                    { 
                        pointInArea.X += TILE_SIZE;
                        pointInArea.Y += TILE_SIZE;
                    }

                    return Area(direction, extended);
                }

                #endregion ==================== Methoden ====================

            }

        }
    }
}
