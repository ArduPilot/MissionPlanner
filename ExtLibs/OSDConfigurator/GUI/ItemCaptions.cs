using OSDConfigurator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSDConfigurator.GUI
{
    public static class ItemCaptions
    {
        public static string GetCaption(OSDItem item)
        {
            switch (item.Name)
            {
                case "ALTITUDE":
                    return $"130{Symbols.SYM_ALT_M}";

                case "BAT_VOLT":
                    return $"{(char)(Symbols.SYM_BATT_FULL + 1)}11.8{Symbols.SYM_VOLT}";

                case "RSSI":
                    return $"{Symbols.SYM_RSSI}93";

                case "CURRENT":
                    return $"18.3{Symbols.SYM_AMP}";

                case "FLTMODE":
                    return "STAB" + Symbols.SYM_DISARMED;

                case "SATS":
                    return $"{Symbols.SYM_SAT_L}{Symbols.SYM_SAT_R}13";

                case "BATUSED":
                    return $"3255{Symbols.SYM_MAH}";

                case "HORIZON":
                    var h = (char)(Symbols.SYM_AH_H_START + 4);
                    return $"{h}{h}{Symbols.SYM_AH_CENTER_LINE_LEFT}{Symbols.SYM_AH_CENTER}{Symbols.SYM_AH_CENTER_LINE_RIGHT}{h}{h}";

                case "COMPASS":
                    return string.Concat(Symbols.SYM_HEADING_N, Symbols.SYM_HEADING_LINE, Symbols.SYM_HEADING_DIVIDED_LINE, Symbols.SYM_HEADING_LINE,
                                         Symbols.SYM_HEADING_E, Symbols.SYM_HEADING_LINE, Symbols.SYM_HEADING_DIVIDED_LINE, Symbols.SYM_HEADING_LINE,
                                         Symbols.SYM_HEADING_S);

                case "GPSLONG":
                    return "30.5003901";

                case "GPSLAT":
                    return "50.3534305";

                case "HOME":
                    return $"{Symbols.SYM_HOME}{Symbols.SYM_ARROW_START} 101{Symbols.SYM_M}";

                case "GSPEED":
                    return $"{Symbols.SYM_GSPD}{Symbols.SYM_ARROW_START} 17{Symbols.SYM_KMH}";

                case "PITCH":
                    return $"{Symbols.SYM_PTCHDWN}10{Symbols.SYM_DEGR}";

                case "ROLL":
                    return $"{Symbols.SYM_ROLLL}3{Symbols.SYM_DEGR}";

                case "VSPEED":
                    return $"{Symbols.SYM_UP_UP}4{Symbols.SYM_MS}";

                case "THROTTLE":
                    return $"56{Symbols.SYM_PCNT}";

                case "HEADING":
                    return $"216{Symbols.SYM_DEGR}";


                default:
                    return item.Name;
            }
        }
    }
}
