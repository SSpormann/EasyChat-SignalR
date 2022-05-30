
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace myeasychat
{
    public class EasyChatHub : Hub
    {

        public void send(byte[] message)
        {
            Clients.All.SendCoreAsync("send", new object[] { message });
        }

    }
}