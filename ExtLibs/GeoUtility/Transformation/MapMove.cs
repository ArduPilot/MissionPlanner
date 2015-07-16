//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/Transformation/MapMove.cs $
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
// File description   : Klasse Transformation, Funktionen: MapMove
//===================================================================================================



using System;
using System.Text;
using GeoUtility.GeoSystem;
using GeoUtility.GeoSystem.Helper;


namespace GeoUtility
{
    internal partial class Transformation
    {

        /// <summary><para>Die Funktion berechnet den neuen internen Schlüssel für das Satellitenbild (Tile), 
        /// welches durch die Verschiebung in die angegebene Richtung ausgewählt wird. Die Funktion wird von der 
        /// <see cref="MapService"/>-Klasse verwendet, und sollte nur über deren Methoden verwendet werden.
        /// <para>Die Funktion ist nur für interne Berechnungen bestimmt.</para></para></summary>
        /// 
        /// <param name="tile">Interne generische Repräsentation des Schlüssels.</param>
        /// <param name="dir">Richtung der Verschiebung.</param>
        /// <returns>Den internen generischen Schlüssel im <see cref="MapService.Info.MapServiceInternalMapTile"/>-Objekt.</returns>
        internal MapService.Info.MapServiceInternalMapTile MapMove(MapService.Info.MapServiceInternalMapTile tile, MapService.Info.MapDirection dir)
        {
            char[] keyArray = tile.Key.ToCharArray();
            switch (dir)
            {
                case (MapService.Info.MapDirection.North):
                    for (int i = keyArray.Length - 1; i >= 0; i--)
                    {
                        if (keyArray[i] == 's')
                        {
                            keyArray[i] = 'r';
                            break;
                        }
                        else if (keyArray[i] == 't')
                        {
                            keyArray[i] = 'q';
                            break;
                        }
                        else if (keyArray[i] == 'r')
                        {
                            keyArray[i] = 's';
                        }
                        else if (keyArray[i] == 'q')
                        {
                            keyArray[i] = 't';
                        }
                    };
                    break;
                case (MapService.Info.MapDirection.Northeast):
                    for (int i = keyArray.Length - 1; i >= 0; i--)
                    {
                        if (keyArray[i] == 't')
                        {
                            keyArray[i] = 'r';
                            break;
                        }
                        else if (keyArray[i] == 'q')
                        {
                            keyArray[i] = 's';
                        }
                        else if (keyArray[i] == 'r')
                        {
                            keyArray[i] = 't';
                        }
                        else if (keyArray[i] == 's')
                        {
                            keyArray[i] = 'q';
                        }
                    };
                    break;
                case (MapService.Info.MapDirection.East):
                    for (int i = keyArray.Length - 1; i >= 0; i--)
                    {
                        if (keyArray[i] == 'q')
                        {
                            keyArray[i] = 'r';
                            break;
                        }
                        else if (keyArray[i] == 't')
                        {
                            keyArray[i] = 's';
                            break;
                        }
                        else if (keyArray[i] == 'r')
                        {
                            keyArray[i] = 'q';
                        }
                        else if (keyArray[i] == 's')
                        {
                            keyArray[i] = 't';
                        }
                    };
                    break;
                case (MapService.Info.MapDirection.Southeast):
                    for (int i = keyArray.Length - 1; i >= 0; i--)
                    {
                        if (keyArray[i] == 'q')
                        {
                            keyArray[i] = 's';
                            break;
                        }
                        else if (keyArray[i] == 'r')
                        {
                            keyArray[i] = 't';
                        }
                        else if (keyArray[i] == 's')
                        {
                            keyArray[i] = 'q';
                        }
                        else if (keyArray[i] == 't')
                        {
                            keyArray[i] = 'r';
                        }
                    };
                    break;
                case (MapService.Info.MapDirection.South):
                    for (int i = keyArray.Length - 1; i >= 0; i--)
                    {
                        if (keyArray[i] == 'r')
                        {
                            keyArray[i] = 's';
                            break;
                        }
                        else if (keyArray[i] == 'q')
                        {
                            keyArray[i] = 't';
                            break;
                        }
                        else if (keyArray[i] == 's')
                        {
                            keyArray[i] = 'r';
                        }
                        else if (keyArray[i] == 't')
                        {
                            keyArray[i] = 'q';
                        }
                    };
                    break;
                case (MapService.Info.MapDirection.Southwest):
                    for (int i = keyArray.Length - 1; i >= 0; i--)
                    {
                        if (keyArray[i] == 'r')
                        {
                            keyArray[i] = 't';
                            break;
                        }
                        else if (keyArray[i] == 'q')
                        {
                            keyArray[i] = 's';
                        }
                        else if (keyArray[i] == 's')
                        {
                            keyArray[i] = 'q';
                        }
                        else if (keyArray[i] == 't')
                        {
                            keyArray[i] = 'r';
                        }
                    };
                    break;
                case (MapService.Info.MapDirection.West):
                    for (int i = keyArray.Length - 1; i >= 0; i--)
                    {
                        if (keyArray[i] == 'r')
                        {
                            keyArray[i] = 'q';
                            break;
                        }
                        else if (keyArray[i] == 's')
                        {
                            keyArray[i] = 't';
                            break;
                        }
                        else if (keyArray[i] == 'q')
                        {
                            keyArray[i] = 'r';
                        }
                        else if (keyArray[i] == 't')
                        {
                            keyArray[i] = 's';
                        }
                    };
                    break;
                case (MapService.Info.MapDirection.Northwest):
                    for (int i = keyArray.Length - 1; i >= 0; i--)
                    {
                        if (keyArray[i] == 's')
                        {
                            keyArray[i] = 'q';
                            break;
                        }
                        else if (keyArray[i] == 'q')
                        {
                            keyArray[i] = 's';
                        }
                        else if (keyArray[i] == 'r')
                        {
                            keyArray[i] = 't';
                        }
                        else if (keyArray[i] == 't')
                        {
                            keyArray[i] = 'r';
                        }
                    };
                    break;
            }

            StringBuilder key = new StringBuilder();
            foreach (char c in keyArray)
            {
                key.Append(c.ToString());
            }

            return new MapService.Info.MapServiceInternalMapTile(key.ToString());
        }

    }

}
