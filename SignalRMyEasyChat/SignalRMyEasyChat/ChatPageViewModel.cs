using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.DeviceInfo;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;

namespace SignalRMyEasyChat
{
    public sealed class ChatPageViewModel : INotifyPropertyChanged
    {
        readonly HubConnection client;
        readonly CancellationTokenSource cts;
        readonly string username;


        Command<string> sendMessageCommand;
        ObservableCollection<Message> messages;
        string messageText;

        public ChatPageViewModel(string username)
        {

            client = new HubConnectionBuilder().WithUrl($"https://172.31.20.12:5001/EasyChatHub?deviceId={CrossDeviceInfo.Current.Id}", (opts) =>
            //client = new HubConnectionBuilder().WithUrl($"https://172.31.20.12:5001/EasyChatHub", (opts) =>
                {
                    opts.HttpMessageHandlerFactory = (x) => new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (a, b, c, d) => 
                        { 
                            return true; 
                        },
                    };
                    opts.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets /*| Microsoft.AspNetCore.Http.Connections.HttpTransportType.ServerSentEvents | Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling*/;
                })
                .WithAutomaticReconnect()
                .Build();
            client.On<byte[]>("send", RecieveMessage);
            cts = new CancellationTokenSource();
            messages = new ObservableCollection<Message>();

            this.username = username;

            ConnectToServerAsync();
        }

        public bool IsConnected => client.State == HubConnectionState.Connected;

        public Command SendMessage => sendMessageCommand ??
            (sendMessageCommand = new Command<string>(SendMessageAsync, CanSendMessage));

        public ObservableCollection<Message> Messages => messages;

        public string MessageText
        {
            get
            {
                return messageText;
            }
            set
            {
                messageText = value;
                OnPropertyChanged();

                sendMessageCommand.ChangeCanExecute();
            }
        }

        async void ConnectToServerAsync()
        {
            while (client.State != HubConnectionState.Connected)
            {
                try
                {
                    if (client.State != HubConnectionState.Connected)
                        await client.StartAsync();
               
                    
                    
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"Signalr connect failed");
                    Console.WriteLine(exc.ToString());
                    await Task.Delay(2500);
                }

                //UpdateClientState();
            }
        }
        void UpdateClientState()
        {

            Device.BeginInvokeOnMainThread(delegate
            {
                OnPropertyChanged(nameof(IsConnected));
                sendMessageCommand.ChangeCanExecute();
                Console.WriteLine($"Websocket state {client.State}");
            });
        }


        void RecieveMessage(byte[] message)
        {

            string serialisedMessae = Encoding.UTF8.GetString(message);

            Device.BeginInvokeOnMainThread(delegate
            {
                try
                {
                    var msg = JsonConvert.DeserializeObject<Message>(serialisedMessae);
                    Messages.Add(msg);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Invalide message format. {ex.Message}");
                }

                UpdateClientState();
            });
        }



        async void SendMessageAsync(string message)
        {
            var msg = new Message
            {
                Name = username,
                MessagDateTime = DateTime.Now,
                Text = message,
                UserId = CrossDeviceInfo.Current.Id
            };

            string serialisedMessage = JsonConvert.SerializeObject(msg);

            var byteMessage = Encoding.UTF8.GetBytes(serialisedMessage);
            var segmnet = new ArraySegment<byte>(byteMessage);

            await client.SendAsync("send", segmnet, cts.Token);
            MessageText = string.Empty;
            //UpdateClientState();
        }

        bool CanSendMessage(string message)
        {
            return IsConnected && !string.IsNullOrEmpty(message);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
