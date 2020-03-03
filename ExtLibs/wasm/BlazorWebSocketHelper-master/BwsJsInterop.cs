using Microsoft.JSInterop;
using Mono.WebAssembly.Interop;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace BlazorWebSocketHelper
{
    public class BwsJsInterop
    {
        private IJSRuntime _JSRuntime;
        public BwsJsInterop(IJSRuntime jsRuntime) => _JSRuntime = jsRuntime;

        public ValueTask<string> Alert(string message)
        {
            return _JSRuntime.InvokeAsync<string>(
                "BwsJsFunctions.alert",
                message);
        }

        public ValueTask<string> Prompt(string message)
        {
            return _JSRuntime.InvokeAsync<string>(
                "BwsJsFunctions.showPrompt",
                message);
        }

        public ValueTask<bool> WsAdd(string WsID, string WsUrl, string WsTransportType, DotNetObjectReference<WebSocketHelper> dotnethelper)
        {
            return _JSRuntime.InvokeAsync<bool>("BwsJsFunctions.WsAdd", new { WsID, WsUrl, WsTransportType, dotnethelper });
        }


        public ValueTask<bool> WsSend(string WsID, string WsMessage)
        {
            return _JSRuntime.InvokeAsync<bool>("BwsJsFunctions.WsSend", new { WsID, WsMessage});
        }

        public bool WsSend(string WsID, byte[] WsMessage)
        {
            if (_JSRuntime is MonoWebAssemblyJSRuntime mono)
            {
                return mono.InvokeUnmarshalled<string, byte[], bool>(
                    "BwsJsFunctions.WsSendBinary",
                    WsID,
                    WsMessage);
            }

            return false;
        }

        public ValueTask<bool> WsSetBinaryType(string WsID, string WsBinaryType)
        {
            return _JSRuntime.InvokeAsync<bool>("BwsJsFunctions.WsSetBinaryType", new { WsID, WsBinaryType });
        }

        public ValueTask<bool> WsClose(string WsID)
        {
            return _JSRuntime.InvokeAsync<bool>("BwsJsFunctions.WsClose", WsID);
        }

        public ValueTask<bool> WsRemove(string WsID)
        {
            return _JSRuntime.InvokeAsync<bool>("BwsJsFunctions.WsRemove", WsID);
        }

        public ValueTask<short> WsGetStatus(string WsID)
        {
            return _JSRuntime.InvokeAsync<short>("BwsJsFunctions.WsGetStatus", WsID);
        }
    }
}
