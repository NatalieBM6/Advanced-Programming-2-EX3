using Infrastructure.Event;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Communication
{
    public class TCPConnectionServer
    {
        private bool stop;
        private TcpListener listener;
        private List<TcpClient> clients = new List<TcpClient>();
        public event EventHandler<CommandEventArgs> OnMessageToServer;
        private Mutex mutex;
        private IClientHandler clientHandler;



        public TCPConnectionServer(IClientHandler handler)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            listener = new TcpListener(ep);
            clientHandler = handler;
            mutex = handler.UpdateMutex();
            clientHandler.ClientDisconnect += OnClientDisconnect;
            stop = false;
        }

        public void Start()
        {
            Task.Run(() => {
                listener.Start();
                Console.WriteLine("Waiting for client connections...");

                while (!stop)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("Client connected");
                    clients.Add(client);
                    Task.Run(() => clientHandler.HandleClient(client));
                }
            });
        }

        private void OnClientDisconnect(object sender, TcpClient client)
        {
            mutex.WaitOne();
            clients.Remove(client);
            mutex.ReleaseMutex();
        }

        public void SendMessageToClient(TcpClient client, CommandEventArgs commandArgs)
        {
            NetworkStream stream = client.GetStream();
            BinaryWriter writer = new BinaryWriter(stream);
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(commandArgs);
            mutex.WaitOne();
            writer.Write(jsonString);
            mutex.ReleaseMutex();
        }

        public void SendMessageToAllClients(CommandEventArgs commandArgs)
        {
            foreach (TcpClient client in clients)
            {
                SendMessageToClient(client, commandArgs);
            }
        }

        public void Stop()
        {
            stop = true;
            listener.Stop();
        }
    }
}
