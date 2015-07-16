//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/GeoUtility.cs $
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
// File description   : Diese Datei beschreibt die Struktur der Bibliothek
//===================================================================================================


//======================================= LICENSE ===================================================
//
// Copyright (c) 2009-2010 Steffen Habermehl
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT 
// OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//****************************************************************************************************
//
// Copyright (c) 2009-2010 Steffen Habermehl
//
// Hiermit wird unentgeltlich, jeder Person, die eine Kopie der Software und der 
// zugehörigen Dokumentationen (die "Software") erhält, die Erlaubnis erteilt, 
// uneingeschränkt zu benutzen, inklusive und ohne Ausnahme, dem Recht, sie zu 
// verwenden, kopieren, ändern, fusionieren, verlegen, verbreiten, unterlizenzieren 
// und/oder zu verkaufen, und Personen, die diese Software erhalten, diese Rechte zu 
// geben, unter den folgenden Bedingungen:
//
// Der obige Urheberrechtsvermerk und dieser Erlaubnisvermerk sind in alle Kopien 
// oder Teilkopien der Software beizulegen.
//
// DIE SOFTWARE WIRD OHNE JEDE AUSDRÜCKLICHE ODER IMPLIZIERTE GARANTIE BEREITGESTELLT, 
// EINSCHLIESSLICH DER GARANTIE ZUR BENUTZUNG FÜR DEN VORGESEHENEN ODER EINEM 
// BESTIMMTEN ZWECK SOWIE JEGLICHER RECHTSVERLETZUNG, JEDOCH NICHT DARAUF BESCHRÄNKT. 
// IN KEINEM FALL SIND DIE AUTOREN ODER COPYRIGHTINHABER FÜR JEGLICHEN SCHADEN ODER 
// SONSTIGE ANSPRÜCHE HAFTBAR ZU MACHEN, OB INFOLGE DER ERFÜLLUNG EINES VERTRAGES, 
// EINES DELIKTES ODER ANDERS IM ZUSAMMENHANG MIT DER SOFTWARE ODER SONSTIGER 
// VERWENDUNG DER SOFTWARE ENTSTANDEN.
//
//===================================================================================================



// Die Programmbibliothek stellt Funktionalitäten zur Verfügung, um Koordinaten 
// eines Koordinatensystems in ein anderes Koordinatensystem zu transformieren.
// Die Bibliothek deckt alle in Deutschland derzeit verwendeten 
// Systeme ab. Folgende Koordinatensysteme können transformiert werden:
// - Gauss-Krueger (Topografische Karten)
// - UTM (neuere Topografische Karten)
// - Geografische Koordinaten (Längen-Breiten-System, z. B. in GPS-Geräte)
// - MGRS (Militärisches System)
// - MapService Dienste
//
// Anders als andere Bibliotheken wurde bei der Entwicklung von GeoUtility auf eine 
// möglichst einfache Verwendung großen Wert gelegt. Den Entwicklern sind die komplexen 
// Hintergründe einer Koordinatentransformation normalerweise unbekannt. Sie möchten 
// eine Koordinatentransformation wie jede andere Typkonvertierung handhaben. Hier setzt 
// GeoUtility an. Als Entwickler braucht man nun im Normalfall keine Parameter zu setzen, 
// denn GeoUtility erledigt dies automatisch. 
//
// <example>
// Beispiel für eine Transformation von <see cref="GeoUtility.GeoSystem.GaussKrueger">Gauss-Krüger</see> in ein Längen/-Breitensystem:
// <code>
// using GeoUtility.GeoSystem (s. a. <see cref="GeoSystem.GaussKrueger.Geographic(GaussKrueger)"/>):
// GaussKrueger gauss = new GaussKrueger(3456789, 5612345);
// Geographic geo = (Geographic)gauss; 
// </code>
// </example>
//
// Darüber hinaus kann mit Hilfe der GeoUtility Programmbibliothek als einzig derzeit 
// verfügbare Bibliothek für gegebene Koordinaten ein passendes Satelliten-/
// Luftbild abgerufen werden. Dazu berechnet die<see cref="GeoUtility.GeoSystem.MapService"/> Klasse aus der geographischen
// Koordinate eine URL zu einem MapService Provider. MapService Provider sind Internetdienste
// die Satelliten-/Luftbilder anbieten. Zur Zeit unterstützte Anbieter sind:
// - Google Maps, 
// - Microsoft Virtual Earth und 
// - Yahoo Maps
//  
// In der Praxis wird ein<see cref="GeoUtility.GeoSystem.MapService"/> Objekt wie alle anderen Koordinatensystem-Objekte 
// behandelt. Daraus folgt, dass auch eine Typkonvertierung zwischen MapService- und 
// Koordinatensystem-Objekten genau so einfach durchgeführt werden kann, wie 
// Koordinatensystem-Objekte untereinander. 
//
// Die Bibliothek wurde für die CLI Laufzeitumgebung entwickelt, und ist demnach auf
// Win32/64 Systemen (.NET Framework 2.0), sowie auf Linux-Systemen (mit Mono-Laufzeitumgebung), 
// diversen Unix-Derivaten und Apple OSX einsetzbar (siehe: http://www.mono-project.com).
namespace GeoUtility
{

    // In diesem Namespace befinden sich die folgenden Koordinatensystem-Klassen:
    // - <see cref="GeoSystem.Geographic"/> (Längen-/Breitenkoordinatensystem)
    // - <see cref="GeoSystem.UTM"/> (Universal Transverse Mercator System)
    // - <see cref="GeoSystem.GaussKrueger"/> (nur in Deutschland verwendetes Koordinatensystem)
    // - <see cref="GeoSystem.MGRS"/> (Military Grid Reference System)
    // - <see cref="GeoSystem.MapService"/> (Systelliten-/Luftbilder von MapService Providern)
    namespace GeoSystem
    {

        // Definition der Basisklassen, von denen die Koordinatensystem-Klassen abgeleitet werden:
        // - <see cref="GeoSystem.Base.BaseSystem"/> (Transformationsfunktionen für alle Koordinatensystem-Klassen)
        // - <see cref="GeoSystem.Base.Geocentric"/> (Basisklasse für geozentrische Koordinatensysteme)
        namespace Base { }


        // Hilfsklassen, Enumerationen und Strukturen, die in den Koordinatensystem-Klassen verwendet werden.
        // - <see cref="GeoSystem.GeoSystem.Helper.GeoDatum"/> (Geodätisches Datum: WGS84, Potsdam)
        // - <see cref="GeoSystem.GeoSystem.Helper.GeoPoint"/> (Bildpunkte als Pixel im X/Y-Format)
        // - <see cref="GeoSystem.GeoSystem.Helper.GeoRect"/> (Dimensionen und Koordinaten eines Satellitenbilds)
        // - <see cref="GeoSystem.GeoSystem.Helper.Sexagesimal"/> (Umwandlungen von Dazimalgrad in Sexagesimal und umgekehrt)
        namespace Helper { }

    }


    /// <summary><para>
    /// Die interne Klasse Transformation stellt die Funktionen zur Transformation der verschiedenen Koordinatensysteme
    /// für alle Klassen zur Verfügung. Diese Klasse ist nicht zur direkten Benutzung vorgesehen. 
    /// Transformationen sollten über die Typumwandlungsfunktionen der Koordinatensystem-Klassen erfolgen.
    /// </para></summary>
    internal partial class Transformation { }


    // Die im Namespace enthaltenen Klassen stellen der Bibliothek lokalisierte Fehlermeldungen, entsprechnend der 
    // Spracheinstellungen des Benutzer-Betriebssystems, zur Verfügung:
    // - <see cref="GeoUtility.ErrorProvider.ErrorMessage"/> (Holt die lokalisierten Fehlermeldungen.)
    // - <see cref="GeoUtility.ErrorProvider.GeoException"/> (Von System.Exception abgeleitete Klasse.)
    // 
    // Die interne Klasse <see cref="T:GeoUtility.ErrorProvider.ErrorMessage"/> holt dabei eine lokalisierte Fehlermeldung, 
    // die durch eine ID spezifiziert wird. Sie stellt diese Nachricht der von <see cref="T:System.Exception">System.Exeption</see> 
    // abgeleiteten Klasse <see cref="T:GeoUtility.ErrorProvider.GeoException"/> zur Verfügung. 
    // Damit wird die Ausnahmebehandlung der Laufzeitumgebung initiiert. Eine Anwendung kann diese Ausnahme kontrolliert 
    // in einem try-catch Block abfangen und auswerten.
    namespace ErrorProvider { }

}

