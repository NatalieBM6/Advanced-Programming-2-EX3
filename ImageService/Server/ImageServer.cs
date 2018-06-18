using ImageService.Controller;
using ImageService.Controller.Handlers;
using Infrastructure.Enums;
using Logging;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication;
using Infrastructure.Event;
using Logging.Modal;
using ImageService.Commands;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        private List<IDirectoryHandler> handlers;
        private string[] handlersPath;

        private TCPConnectionServer serverChannel;
        private Dictionary<string, CommandEnum> commands;

        #endregion

        #region Properties
        //The event that notifies about a new Command being recieved.
        public event EventHandler<CommandRecievedEventArgs> CommandReceived;
        #endregion

        /************************************************************************
        *The Input: ImageController a LoggingService and the way to a handler.
        *The Output: -
        *The Function operation: The function builds a server.
        *************************************************************************/
        public ImageServer(IImageController controller, ILoggingService logging, string[] handlersPath)
        {
            m_controller = controller;
            m_logging = logging;
            m_logging.MessageReceived += OnLogMessageReceived;

            RemoveHandlerCommand removeHandlerCommand = new RemoveHandlerCommand();
            removeHandlerCommand.RemoveHandler += OnRemoveHandler;

            controller.AddCommand(CommandEnum.RemoveHandlerCommand, removeHandlerCommand);

            commands = new Dictionary<string, CommandEnum>() { { "Close Handler", CommandEnum.CloseCommand } };

            serverChannel = new TCPConnectionServer(new ClientHandler(m_controller, m_logging));

            serverChannel.Start();


            handlers = new List<IDirectoryHandler>();
            this.handlersPath = handlersPath;
            //Creating handlers.
            for (int i = 0; i < handlersPath.Length; i++)
            {
                handlers.Add(new DirectoryHandler(this.m_controller, this.m_logging));
                handlers[i].StartHandleDirectory(handlersPath[i]);
                CommandReceived += handlers[i].OnCommandRecieved;
                handlers[i].DirectoryClose += OnHandlerClose;
                // Logging each handler into the entry.
                m_logging.Log("Directory Handler created at path:" + handlersPath[i], Logging.Modal.MessageTypeEnum.INFO);
            }



        }

        public void Start()
        {
            serverChannel.Start();
        }

        /************************************************************************
        *The Input: -.
        *The Output: -
        *The Function operation: The function does nothing for now.
        *************************************************************************/
        public void SendCommand(string command, string path, string[] args)
        {

            CommandReceived?.Invoke(this, new CommandRecievedEventArgs(commands[command], args, path));
        }

        /************************************************************************
        *The Input: -.
        *The Output: -
        *The Function operation: The function invoke the event.
        *************************************************************************/
        public void CloseServer()
        {
            CommandRecievedEventArgs commandRecEventArgs = new CommandRecievedEventArgs(CommandEnum.CloseCommand, null, null);
            CommandReceived?.Invoke(this, commandRecEventArgs);
        }

        /************************************************************************
        *The Input: sender and an event.
        *The Output: -
        *The Function operation: The function is called when the handler is closed.
        *************************************************************************/
        public void OnHandlerClose(object sender, DirectoryCloseEventArgs e)
        {
            IDirectoryHandler dirHandler = (IDirectoryHandler)sender;
            CommandReceived -= dirHandler.OnCommandRecieved;
            m_logging.Log("Stop handle directory " + e.Message, Logging.Modal.MessageTypeEnum.INFO);
        }

        public void OnLogMessageReceived(object sender, MessageRecievedEventArgs args)
        {
            string jsonMessage = Newtonsoft.Json.JsonConvert.SerializeObject(args);
            Task.Run(() => serverChannel.SendMessageToAllClients(new CommandEventArgs() { CommandID = CommandEnum.LogCommand, CommandArgs = new string[] { jsonMessage } }));
        }

        public void OnRemoveHandler(object sender, string path)
        {
            SendCommand("Close Handler", path, new string[] { });
            Task.Run(() => serverChannel.SendMessageToAllClients(new CommandEventArgs() {
                CommandID = CommandEnum.RemoveHandlerCommand, CommandArgs = new string[] { path }
            }));
        }

        public void OnCloseServer(object sender, DirectoryCloseEventArgs args)
        {
            try
            {
                IDirectoryHandler h = (DirectoryHandler)sender;
                CommandReceived -= h.OnCommandRecieved;
                h.DirectoryClose -= OnCloseServer;
            }
            catch
            {
                m_logging.Log("Couldn't cast the sender to handler", MessageTypeEnum.FAIL);
            }
        }

    }
}
