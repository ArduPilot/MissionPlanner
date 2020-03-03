# Blazor WebSocket Helper

![](https://placehold.it/15/4747d1/000000?text=+) 
If you like my work on blazor and want to see more open sourced demos please support me with donations.
![](https://placehold.it/15/4747d1/000000?text=+) 

[donate via paypal](https://www.paypal.me/VakhtangiAbashidze/50)


![](https://placehold.it/15/00e600/000000?text=+) 
Please send [email](VakhtangiAbashidze@gmail.com) if you consider to **hire me**.
![](https://placehold.it/15/00e600/000000?text=+) 



![](https://placehold.it/15/f03c15/000000?text=+) 
**!!!!!!!!!!!! Readme should be updated because it is relevant to version 1.0.1 !!!!!!!!!!!!**
![](https://placehold.it/15/f03c15/000000?text=+) 



This repo contains Pure WebSocket library for blazor.

[Project](https://www.nuget.org/packages/BlazorWebSocketHelper/) is available on nuget.

For install use command - **Install-Package BlazorWebSocketHelper -Version 1.0.4**

You can use websocket in blazor easy and convenient way using this library.

![image](https://raw.githubusercontent.com/Lupusa87/BlazorWebSocketHelper/master/Untitled.png)


Usage sample:

markup:
```
<div style="margin:5px">
    <button class="btn btn-primary" style="width:120px;margin:5px" onclick="@WsConnect">@Ws_Button</button>
    <span style="margin:5px">URL</span>
    <input bind="@Ws_URL" style="margin:5px;width:250px" />
    <span style="width:120px;margin:5px">status:@Ws_Status</span>
    <button class="btn btn-primary" style="margin:5px" onclick="@WsGetStatus">Get status</button>
</div>
<br />
<div style="margin:5px">
    <input style="margin:5px;width:250px" bind="@Ws_Message" />
    <button class="btn btn-primary" style="margin:5px" onclick="@WsSendMessage">Send</button>
</div>
<br />
<span style="margin:5px">Log:</span>
<br />
@foreach (var item in WebSocketHelper1.Log)
{
    <br />
    <CompMessage bwsMessage="@item" parent="@this" />
    <br />

}
```


code:
```
  public string Ws_URL = "wss://echo.websocket.org";
  protected WebSocketHelper WebSocketHelper1;
  
  public void WsConnect()
  {
    WebSocketHelper1 = new WebSocketHelper(Ws_URL);

    WebSocketHelper1.OnStateChange = WsOnStateChange;
    WebSocketHelper1.OnMessage = WsOnMessage;
    WebSocketHelper1.OnError = WsOnError;
  }
  public void WsOnStateChange(short par_state)
  {
    Ws_Status = WebSocketHelper1.bwsState.ToString();
    StateHasChanged();
  }
  public void WsOnError(string par_error)
  {
    BwsJsInterop.Alert(par_error);
  }
  public void WsOnMessage(string par_message)
  {
    StateHasChanged();
  }
  public void WsSendMessage()
  {
      if (WebSocketHelper1.bwsState == BwsEnums.BwsState.Open)
      {
          if (!string.IsNullOrEmpty(Ws_Message))
          {
              WebSocketHelper1.send(Ws_Message);
              Ws_Message = string.Empty;
              StateHasChanged();
          }
          else
          {
              BwsJsInterop.Alert("Please input message");
          }
      }
      else
      {
          BwsJsInterop.Alert("Connection is closed");
      }
  }
  
```


For example this helper was used [here](https://lupblazordemos.z13.web.core.windows.net/WebSocketPage)

You can see code [here](https://github.com/Lupusa87/LupusaBlazorProjects/blob/master/BlazorApp1/Pages/WebSocketPage.cshtml)

Any PRs are welcome.
