//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/GeoSystem/Helper/Sexagesimal.cs $
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
// File description   : Definition des Sexagesimal Klasse (Grad/Minuten/Sekunden/Tertien) 
//===================================================================================================


using System;


namespace GeoUtility.GeoSystem.Helper
{
    /// <summary><para>Die Klasse <see cref="Sexagesimal"/> hält Koordinaten im Sexagesimal-Format, d.h. Grad/Bogenminuten/Bogensekunden und Tertien bzw. 
    /// Millisekunden.<para>Siehe auch den Wikipedia-Artikel unter der Adresse 
    /// <a href="http://de.wikipedia.org/wiki/Grad_(Winkel)" target="_blank">http://de.wikipedia.org/wiki/Grad_(Winkel)</a>.</para></para></summary>
    public class Sexagesimal
    {
        private int _degree = 0;                                       // Speicherplatz für Grad
        private int _minutes = 0;                                      // Speicherplatz für Minuten
        private int _seconds = 0;                                      // Speicherplatz für Sekunden
        private int _thirds = 0;                                       // Speicherplatz für Tertien (1/60 Sekunden)
        private double _millisec = 0.0;                                // Speicherplatz für Millisekunden (1/1000 Sekunden)


        /// <summary><para>Standard Konstruktor</para></summary>
        public Sexagesimal() { }

        /// <summary><para>Konstruktor mit Dezimalgrad Parameter.</para></summary>
        /// <param name="dec">Dezimalgrad</param>
        public Sexagesimal(double dec)
        {
            double l = Math.Round(dec, 8);
            this.Degree = (int)l;
            l = Math.Round(Math.Abs((l - this.Degree) * 60), 6);
            this.Minutes = (int)l;
            l = Math.Round((l - this.Minutes) * 60, 4);
            this.Seconds = (int)l;
            l = Math.Round((l - this.Seconds), 2);
            _millisec = l;
            l = l * 60;
            _thirds = (int)Math.Round(l);

        }
        
        /// <summary><para>Konstruktor mit Grad/Minuten/Sekunden/Tertien Parametern.</para></summary>
        /// <param name="degree">Grad</param>
        /// <param name="minutes">Minuten</param>
        /// <param name="seconds">Sekunden</param>
        /// <param name="thirds">Tertien (1/60 Sekunden). Nur selten verwendet.</param>
        public Sexagesimal(int degree, int minutes, int seconds, int thirds)
        {
            this.Degree = degree;
            this.Minutes = minutes;
            this.Seconds = seconds;
            this.Thirds = thirds;
            this.Millisec = (double)thirds / 60;
        }

        /// <summary><para>Konstruktor mit Grad/Minuten/Sekunden/Millisekunden Parametern.</para></summary>
        /// <param name="degree">Grad</param>
        /// <param name="minutes">Minuten</param>
        /// <param name="seconds">Sekunden als Dezimalwert</param>
        public Sexagesimal(int degree, int minutes, double seconds)
        {
            this.Degree = degree;
            this.Minutes = minutes;
            this.Seconds = (int)seconds;
            this.Millisec = Math.Abs(seconds) - this.Seconds;
            this.Thirds = (int)Math.Round(this.Millisec * 60);
        }


        /// <summary><para>Grad</para></summary>
        public int Degree { get { return _degree; } set { _degree = value; } }

        /// <summary><para>Minuten</para></summary>
        public int Minutes { get { return _minutes; } set { _minutes = Math.Abs(value); } }

        /// <summary><para>Sekunden</para></summary>
        public int Seconds { get { return _seconds; } set { _seconds = Math.Abs(value); } }

        /// <summary><para>Tertien (1/60 Sekunden). Sie werden jedoch nur selten verwendet.</para></summary>
        public int Thirds { get { return _thirds; } set { _thirds = Math.Abs(value); } }

        /// <summary><para>Millisekunden</para></summary>
        public double Millisec { get { return _millisec; } set { _millisec = Math.Abs(value); } }


        /// <summary><para>Die Funktion wandelt das <see cref="Sexagesimal"/> Format in Dezimalgrad um.</para></summary>
        /// <returns>Gibt den <see cref="Sexagesimal"/>-Wert als Dezimalwert zurück.</returns>
        public double ToDecimal()
        {
            double msec = this.Millisec;
            double sec = (msec + this.Seconds) / 60;
            double min = (sec + this.Minutes) / 60;
            if (this.Degree < 0) min *= (-1);                   // Bei negativen Graden muss die Nachkommastelle substrahiert werden
            double deg = min + this.Degree;
            return deg;
        }

        /// <summary><para>Die Funktion wandelt das <see cref="Sexagesimal"/> Format in Dezimalgrad um.</para></summary>
        /// <returns>Gibt den <see cref="Sexagesimal"/>-Wert als Dezimalwert zurück.</returns>
        public new string ToString()
        {

            return this.Degree.ToString() + "° " + this.Minutes.ToString("00") + "' " + this.Seconds.ToString("00") + "'' " + this.Thirds.ToString("00") + "'''";
        }
    }

}
