using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace Rtsp.Sdp
{
    public class SdpFile
    {
        private static KeyValuePair<string, string> GetKeyValue(TextReader sdpStream)
        {
            string line = sdpStream.ReadLine();

            // end of file ?
            if(string.IsNullOrEmpty(line))
                return new KeyValuePair<string, string>(null, null);

            
            string[] parts = line.Split(new char[] { '=' }, 2);
            if (parts.Length != 2)
                throw new InvalidDataException();
            if (parts[0].Length != 1)
                throw new InvalidDataException();

            KeyValuePair<string, string> value = new KeyValuePair<string, string>(parts[0], parts[1]);
            return value;
        }

        /// <summary>
        /// Reads the specified SDP stream.
        /// As define in RFC 4566
        /// </summary>
        /// <param name="sdpStream">The SDP stream.</param>
        /// <returns></returns>
        public static SdpFile Read(TextReader sdpStream)
        {
            SdpFile returnValue = new SdpFile();
            KeyValuePair<string, string> value = GetKeyValue(sdpStream);

            // Version mandatory
            if (value.Key == "v")
            {
                returnValue.Version = int.Parse(value.Value, CultureInfo.InvariantCulture);
                value = GetKeyValue(sdpStream);
            }
            else {
                throw new InvalidDataException();
            }

            // Origin mandatory
            if (value.Key == "o")
            {
                returnValue.Origin = Origin.Parse(value.Value);
                value = GetKeyValue(sdpStream);
            }
            else {
                throw new InvalidDataException();
            }

            // Session mandatory.
            // However the MuxLab HDMI Encoder (TX-500762) Firmware 1.0.6
            // does not include the 'Session' so supress InvalidDatarException
            if (value.Key == "s")
            {
                returnValue.Session = value.Value;
                value = GetKeyValue(sdpStream);
            }
            else {
                // throw new InvalidDataException(); // we should throw, but instead we just ignore the error
            }

            // Session Information optional
            if (value.Key == "i")
            {
                returnValue.SessionInformation = value.Value;
                value = GetKeyValue(sdpStream);
            }

            // Uri optional
            if (value.Key == "u")
            {
                returnValue.Url = new Uri(value.Value);
                value = GetKeyValue(sdpStream);
            }

            // Email optional
            if (value.Key == "e")
            {
                returnValue.Email = value.Value;
                value = GetKeyValue(sdpStream);
            }

            // Phone optional
            if (value.Key == "p")
            {
                returnValue.Phone = value.Value;
                value = GetKeyValue(sdpStream);
            }

            // Connection optional
            if (value.Key == "c")
            {
                returnValue.Connection = Connection.Parse(value.Value);
                value = GetKeyValue(sdpStream);
            }

            // bandwidth optional
            if (value.Key == "b")
            {
                returnValue.Bandwidth = Bandwidth.Parse(value.Value);
                value = GetKeyValue(sdpStream);
            }

            // Timing mandatory
            while (value.Key == "t")
            {
                string timing = value.Value;
                string repeat = string.Empty;
                value = GetKeyValue(sdpStream);
                if (value.Key == "r")
                {
                    repeat = value.Value;
                    value = GetKeyValue(sdpStream);
                }
                returnValue.Timings.Add(new Timing(timing, repeat));
            }

            // timezone optional
            if (value.Key == "z")
            {

                returnValue.TimeZone = SdpTimeZone.ParseInvariant(value.Value);
                value = GetKeyValue(sdpStream);
            }

            // encryption key optional
            if (value.Key == "k")
            {

                returnValue.EncriptionKey = EncriptionKey.ParseInvariant(value.Value);
                value = GetKeyValue(sdpStream);
            }

            //Attribute optional multiple
            while (value.Key == "a")
            {
                returnValue.Attributs.Add(Attribut.ParseInvariant(value.Value));
                value = GetKeyValue(sdpStream);
            }

            // Hack for MuxLab HDMI Encoder (TX-500762) Firmware 1.0.6
            // Skip over all other Key/Value pairs until the 'm=' key
            while (value.Key != "m") {
                value = GetKeyValue(sdpStream);
            }

            // Media
            while (value.Key == "m")
            {
                Media newMedia = ReadMedia(sdpStream, ref value);
                returnValue.Medias.Add(newMedia);
            }


            return returnValue;
        }

        private static Media ReadMedia(TextReader sdpStream, ref KeyValuePair<string, string> value)
        {
            Media returnValue = new Media(value.Value);
            value = GetKeyValue(sdpStream);

            // Media title
            if (value.Key == "i")
            {
                value = GetKeyValue(sdpStream);
            }

            // Connexion optional
            if (value.Key == "c")
            {
                returnValue.Connection = Connection.Parse(value.Value);
                value = GetKeyValue(sdpStream);
            }

            // bandwidth optional
            if (value.Key == "b")
            {
                returnValue.Bandwidth = Bandwidth.Parse(value.Value);
                value = GetKeyValue(sdpStream);
            }

            // enkription key optional
            if (value.Key == "k")
            {

                returnValue.EncriptionKey = EncriptionKey.ParseInvariant(value.Value);
                value = GetKeyValue(sdpStream);
            }

            //Attribut optional multiple
            while (value.Key == "a")
            {
                returnValue.Attributs.Add(Attribut.ParseInvariant(value.Value));
                value = GetKeyValue(sdpStream);
            }

            return returnValue;
        }


        public int Version { get; set; }


        public Origin Origin { get; set; }

        public string Session { get; set; }

        public string SessionInformation { get; set; }

        public Uri Url { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public Connection Connection { get; set; }

        public Bandwidth Bandwidth { get; set; }

        private readonly List<Timing> timingList = new List<Timing>();

        public IList<Timing> Timings
        {
            get
            {
                return timingList;
            }
        }

        public SdpTimeZone TimeZone { get; set; }

        public EncriptionKey EncriptionKey { get; set; }

        private readonly List<Attribut> attributs = new List<Attribut>();

        public IList<Attribut> Attributs
        {
            get
            {
                return attributs;
            }
        }

        private readonly List<Media> medias = new List<Media>();

        public IList<Media> Medias
        {
            get
            {
                return medias;
            }

        }
    
    }
}
