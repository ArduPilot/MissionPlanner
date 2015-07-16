//===================================================================================================
// Source Control URL : $HeadURL: file:///D:/svn/branch/3.1.7.0/GeoUtility/ErrorProvider/ErrorProvider.cs $
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
// File description   : Definition der internen ErrorMessage und GeoExeption Klasse 
//===================================================================================================


using System;

namespace GeoUtility
{

    namespace ErrorProvider
    {

        /// <summary><para>Die interne Klasse holt eine lokalisierte Fehlermeldung, die durch eine ID spezifiziert wird, 
        /// und stellt sie der GeoException-Klasse zur Verfügung.</para></summary>
        internal class ErrorMessage 
        {
            private string _id;                                         // Speicherplatz für Fehler-ID

            /// <summary><para>Der Standard-Konstruktor.</para></summary>
            internal ErrorMessage() 
            {
                _id = "NO_ERROR_MESSAGE";
            }

            /// <summary><para>Konstruktor mit Parameter für die Fehler-ID.</para></summary>
            /// <param name="id">Eine globale Fehler-ID.</param>
            internal ErrorMessage(string id) 
            {
                _id = id;
            }


            /// <summary><para>Die Eigenschaft legt die Fehler-ID fest, sofern sie nicht bei der Erzeugung der Klasse übergeben wurde.</para></summary>
            internal string ID { get { return _id; } set { _id = value; } }

            /// <summary><para>Die Eigenschaft Text gibt die lokalisierte Fehlermeldung zurück, welche durch die ID-Eigenschaft definiert ist.</para></summary>
            internal string Text
            {
                get
                {
                    return new GeoUtility.Localizer.Message(this.ID).Text;
                }
            }
        }



        /// <summary><para>Die interne Klasse stellt der Bibliothek lokalisierte Fehlermeldungen zur Verfügung, 
        /// entsprechnend der Spracheinstellungen des Benutzer-Betriebssystems.</para></summary>
        [Serializable]
        internal class GeoException : Exception
        {
            internal string _id = "NO_ERROR_MESSAGE";

            /// <summary><para>Der Standard-Konstruktor.</para></summary>
            internal GeoException() : base()
            {
#if COMPACT_FRAMEWORK
#else
                this.Data["ExtraInfo"] = "NO_ERROR_MESSAGE";
#endif
            }


            /// <summary><para>Überladener Konstruktor mit einem Parameter für die Fehler-Nachricht.</para></summary>
            /// <param name="message">Ein Fehlertext.</param>
            internal GeoException(ErrorMessage message) : base(message.Text)
            {
#if COMPACT_FRAMEWORK
#else
                if (this.Data != null) this.Data["ExtraInfo"] = message.ID;
#endif

                _id = message.ID;
            }


            /// <summary><para>Überladener Konstruktor mit einem Parameter für die Fehler-Nachricht und eine innere Exception.</para></summary>
            /// <param name="message">Ein Fehlertext.</param>
            /// <param name="inner">Eine innere Ausnahme.</param>
            internal GeoException(ErrorMessage message, Exception inner) : base(message.Text, inner)
            {
#if COMPACT_FRAMEWORK
#else
                if (this.Data != null) this.Data["ExtraInfo"] = message.ID;
#endif

                _id = message.ID;
            }


#if COMPACT_FRAMEWORK
            /// <summary>
            /// Überschreibt die ToString-Methode der Basisklasse, und gibt die interne Fehler-ID zurück.
            /// </summary>
            /// <returns>Interne Fehler-ID</returns>
            public override string ToString()
            {
                return "Errorcode: " + _id;
            }
#endif

        }
    }
}
