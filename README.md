# EasyChat-SignalR

To use the  sample there is a solution under the folder easychat, there should be two Xamarin projects inside, one is called MyEasyChat, this is using plain websockets, combine this project with the server in the folder myeasychat und you can see that the concept of using websockets with System.Net.WebSockets does work. If you now want to try the faulty projekt, you have to use the project SignalRMyEasyChat under the EasyChat.sln and combine this with the myeasychat-signalr server under the folder with tehe same name(myeasychat-signalr) this, should currently promt the same error as I provided earlier.

`Unable to connect to the server with any of the available transports. (WebSockets failed: Unable to connect to the remote server) (LongPolling failed: The transport is disabled by the client.)`

if you now change the line of code in the EasyChat.sln -> SignalRMyEasyChat -> ChatPageViewModel line: 41

From:
`    opts.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets /*| Microsoft.AspNetCore.Http.Connections.HttpTransportType.ServerSentEvents | Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling*/;`

To:
`    opts.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets | Microsoft.AspNetCore.Http.Connections.HttpTransportType.ServerSentEvents | Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;`

you should see that Signal-R is indeed working, but it does not work with websockets as transport.

you will have to change the IP-Adresses, in the ChatPageViewModel files, to your Server-IP.
