using MissionPlanner.Comms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MissionPlanner.Utilities
{
    public class AP_GPS_COMNAV
    {

byte const SYNC1  = 0xAA   ;     /* comnav message start sync code 1 */  
byte const SYNC2  = 0x44    ;    /* comnav message start sync code 2 */  
byte const SYNC3 =  0x12   ;     /* comnav message start sync code 3 */  

 //#define ID_RANGECMP 140         /* message id: comnav range compressed */  
 //#define ID_RAWEPHEM 41          /* message id: comnav raw ephemeris */  
 //#define ID_GPSBDSEPHEM 71       /* message id: comnav decoded gps/bds ephemeris */  
 //#define ID_GLOEPHEMERIS 723     /* message id: comnav glonass ephemeris */  


        Stream port = null;// File.Open(@"C:\Users\hog\Desktop\gps data\asterx-m", FileMode.Open);

        public AP_GPS_COMNAV()
        {
            var sport = new SerialPort("COM13", 115200);

            sport.Open();

            port = sport.BaseStream;
        }


        public bool read()
        {
            var st = File.OpenWrite("comnav.log");

            bool ret = false;
            while (true)
            {
                var temp = port.ReadByte();
                ret |= parse((byte)temp);

                st.WriteByte((byte)temp);
            }

            return ret;
        }

        private bool parse(byte p)
        {
            
        }
    }
}
