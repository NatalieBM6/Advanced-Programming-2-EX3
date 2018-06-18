using Communication;
using ImageService.Controller;
using Infrastructure.Enums;
using Infrastructure.Event;
using Logging;
using Logging.Modal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService.Server
{
    class ClientHandler : IClientHandler
    {
        public event EventHandler<TcpClient> ClientDisconnect;

        private IImageController controller;
        private ILoggingService logger;
        private bool stop;
        private static Mutex mutex;

        public ClientHandler(IImageController imageController, ILoggingService imageLogger)
        {
            stop = false;
            mutex = new Mutex();
            controller = imageController;
            logger = imageLogger;
        }

        public void HandleClient(TcpClient client)
        {
            while (!stop)
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    BinaryReader reader = new BinaryReader(stream);
                    BinaryWriter writer = new BinaryWriter(stream);
                    {

                        try
                        {
                            // mutex.WaitOne();
                            string jsonString = reader.ReadString();
                            // mutex.ReleaseMutex();
                            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<CommandEventArgs>(jsonString);
                            CommandEventArgs args = (CommandEventArgs)obj;
                            bool result;
                            string logMessage = controller.ExecuteCommand(args.CommandID, args.CommandArgs, out result); //make sure controller has try/catch
                            if (!result)
                            {
                                logger.Log(logMessage, MessageTypeEnum.FAIL);
                                return;
                            }
                            jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(new CommandEventArgs() { CommandID = args.CommandID, CommandArgs = new string[] { logMessage } });
                            mutex.WaitOne();
                            if (args.CommandID != CommandEnum.RemoveHandlerCommand)
                            {
                                writer.Write(jsonString);
                                mutex.ReleaseMutex();
                                mutex.WaitOne();
                            }
                            logger.Log("Command " + args.CommandID.ToString() + " received" + System.Environment.NewLine +
                               "Arguments returned:\n" + logMessage, MessageTypeEnum.INFO);
                            mutex.ReleaseMutex();
                        }
                        catch
                        {
                            ClientDisconnect?.Invoke(this, client);
                            break;
                        }
                    }
                }
                catch
                {
                    ClientDisconnect?.Invoke(this, client);
                    break;
                }
            }
        }

        public Mutex UpdateMutex()
        {
            return mutex;
        }
    }
}
