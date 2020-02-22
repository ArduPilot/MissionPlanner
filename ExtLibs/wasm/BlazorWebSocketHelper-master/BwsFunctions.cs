using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BlazorWebSocketHelper.Classes.BwsEnums;

namespace BlazorWebSocketHelper
{
    public static class BwsFunctions
    {
        public static string Cmd_Get_UniqueID()
        {
            long j = DateTime.Now.Ticks;
            string a = j.ToString();
            return a.Substring(a.Length - 8, 4) + Guid.NewGuid().ToString("d").Substring(1, 4);
        }


        public static BwsState ConvertStatus(short a)
        {
            BwsState result = BwsState.Undefined;
            

            switch (a)
            {
                case -1:
                    result = BwsState.Error;
                    break;
                case 0:
                    result = BwsState.Connecting;
                    break;
                case 1:
                    result = BwsState.Open;
                    break;
                case 2:
                    result = BwsState.Closing;
                    break;
                case 3:
                    result = BwsState.Close;
                    break;
                default:
                    result = BwsState.Undefined;
                    break;
            }

            return result;

        }
    }
}
