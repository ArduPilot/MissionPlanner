using BlazorWebSocketHelper.Classes;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static BlazorWebSocketHelper.Classes.BwsEnums;

namespace BlazorWebSocketHelper
{
    public class WebSocketHelper : IDisposable
    {

        public BwsState bwsState = BwsState.Undefined;


        public BwsTransportType TransportType { get; private set; } = BwsTransportType.Text;

        public bool IsDisposed = false;

        public Action<short> OnStateChange { get; set; }

        public Action<BwsMessage> OnMessage { get; set; }

        public Action<string> OnError { get; set; }


        public List<BwsMessage> Log = new List<BwsMessage>();
        public bool DoLog { get; set; } = true;
        public int LogMaxCount { get; set; } = 100;

        public string _id { get; private set; } = BwsFunctions.Cmd_Get_UniqueID();

        private string _url = string.Empty;

        public List<BwsError> BwsError = new List<BwsError>();

        public async Task<string> Get_WsStatus()
        {
            
            short a = await new BwsJsInterop(_JSRuntime).WsGetStatus(_id);

            return BwsFunctions.ConvertStatus(a).ToString();
            
        }

        private IJSRuntime _JSRuntime;

        public WebSocketHelper(string Par_URL, BwsTransportType Par_TransportType, IJSRuntime jsRuntime)
        {
            _JSRuntime = jsRuntime ??
                throw new ArgumentNullException($"{nameof(jsRuntime)} missing. Try injecting it in your component, then passing it from OnAfterRender.");

            _initialize(Par_URL, Par_TransportType);
        }

        private void _initialize(string Par_URL, BwsTransportType Par_TransportType)
        {
            if (!string.IsNullOrEmpty(Par_URL))
            {
                StaticClass.webSocketHelpers_List.Add(this);
                _url = Par_URL;
                TransportType = Par_TransportType;
                _connect();
            }
            else
            {
                BwsError.Add(new BwsError { Message = "Url is not provided!", Description = string.Empty });
            }
        }

        private void _connect()
        {
            new BwsJsInterop(_JSRuntime).WsAdd(_id, _url, TransportType.ToString(), DotNetObjectReference.Create(this));
            _setTransportType();
        }


        private int GetNewIDFromLog()
        {

            if (Log.Any())
            {
                return Log.Max(x => x.ID) + 1;
            }
            else
            {
                return 1;
            }
        }

        public void Send(string Par_Message, bool AddToLog = true)
        {
            if (!string.IsNullOrEmpty(Par_Message))
            {

                new BwsJsInterop(_JSRuntime).WsSend(_id, Par_Message);

                if (DoLog && AddToLog)
                {
                    
                    Log.Add(new BwsMessage { ID = GetNewIDFromLog(),
                                             Date = DateTime.Now,
                                             Message = Par_Message,
                                             MessageType = BwsMessageType.send,
                                             TransportType = TransportType});
                    if (Log.Count > LogMaxCount)
                    {
                        Log.RemoveAt(0);
                    }
                }
              
            }
        }


        public void Send(byte[] Par_Message, bool AddToLog = true)
        {
            

            if (Par_Message.Length>0)
            {

                new BwsJsInterop(_JSRuntime).WsSend(_id, Par_Message);


                if (DoLog && AddToLog)
                {

                    Log.Add(new BwsMessage { ID = GetNewIDFromLog(),
                                             Date = DateTime.Now,
                                             MessageBinary = Par_Message,
                                             MessageType = BwsMessageType.send,
                                             TransportType = TransportType });
                    if (Log.Count > LogMaxCount)
                    {
                        Log.RemoveAt(0);
                    }
                }

            }

          
        }

        [JSInvokable]
        public void InvokeStateChanged(short par_state)
        {
            bwsState = BwsFunctions.ConvertStatus(par_state);
            OnStateChange?.Invoke(par_state);
        }


        [JSInvokable]
        public void InvokeOnError(string par_error)
        {
            OnError?.Invoke(par_error);
        }


        [JSInvokable]
        public void InvokeOnMessage(string par_message)
        {

            BwsMessage b = new BwsMessage
            {
                ID = GetNewIDFromLog(),
                Date = DateTime.Now,
                Message = par_message,
                MessageType = BwsMessageType.received,
                TransportType = TransportType
            };

            if (DoLog)
            {
                
                Log.Add(b);
                
                if (Log.Count > LogMaxCount)
                {
                    Log.RemoveAt(0);
                }
            }

            
            OnMessage?.Invoke(b);

        }



        public void InvokeOnMessageBinary(byte[] data)
        {

            BwsMessage b = new BwsMessage
            {
                ID = GetNewIDFromLog(),
                Date = DateTime.Now,
                Message = Encoding.UTF8.GetString(data),
                MessageBinary = data,
                MessageBinaryVisual = string.Join(", ", data),
                MessageType = BwsMessageType.received,
                TransportType = TransportType
            };

            if (DoLog)
            {

                Log.Add(b);

                if (Log.Count > LogMaxCount)
                {
                    Log.RemoveAt(0);
                }
            }


            OnMessage?.Invoke(b);
        }

        public void SetTransportType(BwsTransportType par_bwsTransportType)
        {
            if (TransportType != par_bwsTransportType)
            {
                TransportType = par_bwsTransportType;

                _setTransportType();
            }
        }

        private void _setTransportType()
        {

                switch (TransportType)
                {
                    case BwsTransportType.Text:
                        break;
                    case BwsTransportType.ArrayBuffer:
                        new BwsJsInterop(_JSRuntime).WsSetBinaryType(_id, "arraybuffer");
                        break;
                    case BwsTransportType.Blob:
                        new BwsJsInterop(_JSRuntime).WsSetBinaryType(_id, "blob");
                        break;
                    default:
                        break;
                }
            
        }


        public void Close()
        {
            if (DoLog)
            {
                Log = new List<BwsMessage>();
            }
            new BwsJsInterop(_JSRuntime).WsClose(_id);
        }

        public void Dispose()
        {
            if (DoLog)
            {
                Log = new List<BwsMessage>();
            }


            InvokeStateChanged(2);

            new BwsJsInterop(_JSRuntime).WsRemove(_id);


            IsDisposed = true;
            GC.SuppressFinalize(this);
        }




    }
}
