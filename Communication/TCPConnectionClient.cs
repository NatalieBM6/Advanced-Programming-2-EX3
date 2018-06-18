using Infrastructure.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Communication
{
    public class TCPConnectionClient
    {
        public event EventHandler<bool> DisconnectedFromServer;
        public event EventHandler<CommandEventArgs> OnMessageFromServer;
        protected TcpClient client;

        public bool Stop { get; set; }
        protected static Mutex mutex;


        public TCPConnectionClient()
        {
            client = new TcpClient();
            Stop = false;
            mutex = new Mutex();
        }


        public bool ConnectToServer(IPEndPoint ep)
        {
            try
            {
                client.Connect(ep);
                Console.WriteLine("You are connected");
                Task.Run(() => ReceiveMessageFromServer());
                Stop = false;
                return true;
            }
            catch
            {
                Console.WriteLine("Unable to connect to server. Please check your connection.");
                return false;
            }
        }


        public void ReceiveMessageFromServer()
        {
            while (!Stop)
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    BinaryReader reader = new BinaryReader(stream);
                    try
                    {
                        if (stream.DataAvailable)
                        {
                            mutex.WaitOne();
                            string jsonString = reader.ReadString();
                            mutex.ReleaseMutex();
                            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<CommandEventArgs>(jsonString);
                            if (obj != null)
                            {
                                OnMessageFromServer?.Invoke(this, (CommandEventArgs)obj);

                            }
                        }
                        else { Thread.Sleep(2); }
                    }
                    catch { DisconnectFromServer(); }
                }
                catch { DisconnectFromServer(); }
            }
        }

        public void SendMessageToServer(CommandEventArgs commandArgs)
        {
            if (!client.Connected)
            {
                DisconnectFromServer();
                //DisconnectedFromServer?.Invoke(this, client.Connected);
                //Stop = true;
                return;
            }
            NetworkStream stream = client.GetStream();
            BinaryWriter writer = new BinaryWriter(stream);
            BinaryReader reader = new BinaryReader(stream);
            {
                try
                {
                    string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(commandArgs);
                    mutex.WaitOne();
                    writer.Write(jsonString);
                    jsonString = reader.ReadString();
                    mutex.ReleaseMutex();
                    var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<CommandEventArgs>(jsonString);
                    if (obj != null)
                    {
                        OnMessageFromServer?.Invoke(this, (CommandEventArgs)obj);
                    }

                }
                catch { Debug.WriteLine("Can't write to server"); }
            }
        }


        public void DisconnectFromServer()
        {
            Stop = true;
            //  client.Close();
            DisconnectedFromServer?.Invoke(this, false);
        }

    }
}
