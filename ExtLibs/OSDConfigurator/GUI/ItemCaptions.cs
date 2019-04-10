using OSDConfigurator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OSDConfigurator.GUI
{
    public static class ItemCaptions
    {
        private static Regex digitPointRegex = new Regex("(\\d)(\\.)(\\d)");

        public static string GetCaption(OSDItem item, out int xOffset)
        {
            var caption = DoGetCaption(item, out xOffset);

            caption = digitPointRegex.Replace(caption, DigitPointEvaluator);

            return caption;
        }

        private static string DigitPointEvaluator(Match match)
        {
            const int SYM_NUM_WITH_DIGIT_AT_END = 192;
            const int SYM_NUM_WITH_DIGIT_AT_BEGIN = 208;

            char c1 = (char)(SYM_NUM_WITH_DIGIT_AT_END + int.Parse(match.Groups[1].Value));
            char c2 = (char)(SYM_NUM_WITH_DIGIT_AT_BEGIN + int.Parse(match.Groups[3].Value));

            return string.Concat(c1, c2);
        }

        private static string DoGetCaption(OSDItem item, out int xOffset)
        {
            xOffset = 0;

            switch (item.Name)
            {
                case "ALTITUDE":
                    xOffset = -2;
                    return $"11{Symbols.SYM_ALT_M}";

                case "BAT_VOLT":
                    return $"{(char)(Symbols.SYM_BATT_FULL + 1)}11.8{Symbols.SYM_VOLT}";

                case "RSSI":
                    return $"{Symbols.SYM_RSSI}93";

                case "CURRENT":
                    xOffset = 0;
                    return $"8.3{Symbols.SYM_AMP}";

                case "FLTMODE":
                    return "STAB" + Symbols.SYM_DISARMED;

                case "SATS":
                    return $"{Symbols.SYM_SAT_L}{Symbols.SYM_SAT_R}13";

                case "BATUSED":
                    xOffset = -1;
                    return $"125{Symbols.SYM_MAH}";

                case "HORIZON":
                    xOffset = 4;
                    var h = (char)(Symbols.SYM_AH_H_START + 4);
                    return $"{h}{h}{h}{Symbols.SYM_AH_CENTER_LINE_LEFT}{Symbols.SYM_AH_CENTER}{Symbols.SYM_AH_CENTER_LINE_RIGHT}{h}{h}{h}";

                case "COMPASS":
                    xOffset = 4;
                    return string.Concat(Symbols.SYM_HEADING_N, Symbols.SYM_HEADING_LINE, Symbols.SYM_HEADING_DIVIDED_LINE, Symbols.SYM_HEADING_LINE,
                                         Symbols.SYM_HEADING_E, Symbols.SYM_HEADING_LINE, Symbols.SYM_HEADING_DIVIDED_LINE, Symbols.SYM_HEADING_LINE,
                                         Symbols.SYM_HEADING_S);

                case "GPSLONG":
                    return $"{Symbols.SYM_GPS_LONG}  30.5003901";

                case "GPSLAT":
                    return $"{Symbols.SYM_GPS_LAT}  50.3534305";

                case "HOME":
                    return $"{Symbols.SYM_HOME}{Symbols.SYM_ARROW_START} 101{Symbols.SYM_M}";

                case "GSPEED":
                    return $"{Symbols.SYM_GSPD}{Symbols.SYM_ARROW_START} 17{Symbols.SYM_KMH}";

                case "PITCH":
                    return $"{Symbols.SYM_PTCHDWN} 10{Symbols.SYM_DEGR}";

                case "ROLL":
                    return $"{Symbols.SYM_ROLLL}  3{Symbols.SYM_DEGR}";

                case "VSPEED":
                    return $"{Symbols.SYM_UP} 0{Symbols.SYM_MS}";

                case "THROTTLE":
                    xOffset = -2;
                    return $"0{Symbols.SYM_PCNT}";

                case "HEADING":
                    xOffset = -1;
                    return $"32{Symbols.SYM_DEGR}";


                default:
                    return item.Name;
            }
        }
    }
}
