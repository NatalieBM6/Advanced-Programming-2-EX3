using Communication;
using Infrastructure.Enums;
using Infrastructure.Event;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace WebApplication.ServerCommunication
{
    public class Client
    {
        private static Client instance = null;

        public static bool isConnected { get; set; }
        public event EventHandler<CommandEventArgs> MessageReceived;
        public event EventHandler<bool> CheckConnection;

        private TCPConnectionClient clientChannel;

        public static Client GetInstance()
        {
            if (instance == null) instance = new Client();
            return instance;
        }


        private Client()
        {
            clientChannel = new TCPConnectionClient();
            clientChannel.OnMessageFromServer += ReceiveMessageFromServer;
            clientChannel.DisconnectedFromServer += OnDisconnectFromServer;
            isConnected = clientChannel.ConnectToServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000));
            CheckConnection?.Invoke(this, isConnected);
        }


        public void SendMessageToServer(CommandEnum commandID, string[] args)
        {
            clientChannel.SendMessageToServer(new CommandEventArgs() { CommandID = commandID, CommandArgs = args });
        }

        public void ReceiveMessageFromServer(object sender, CommandEventArgs args)
        {
            Task.Run(() =>
            {

                MessageReceived?.Invoke(this, args);
            });
        }

        public void OnDisconnectFromServer(object sender, bool connected)
        {
            isConnected = connected;
            CheckConnection?.Invoke(this, isConnected);
        }

        public void SendCommand(CommandEnum commandID)
        {
            Task.Run(() => {
                SendMessageToServer(commandID, new string[] { });
            });
        }


        public void DisconnectFromServer()
        {
            //client.Close();
        }

        internal void SendCommand(CommandEnum removeHandlerCommand, string[] v)
        {
            Task.Run(() => {
                SendMessageToServer(removeHandlerCommand, v);
                });
           

        }
    }
}