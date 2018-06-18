using Infrastructure.Enums;
using Infrastructure.Event;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebApplication.ServerCommunication;

namespace WebApplication.Models
{
    public class ConfigModel
    {
        private string m_toBeDeletedHandler;
        private bool isInitialized;
        //private string[] handlers;
        [DataType(DataType.Text)]
        [Display(Name = "Directory Handlers")]
        public string[] Handlers { get; set; }
        public string ToBeDeletedHandler { get => m_toBeDeletedHandler; set => m_toBeDeletedHandler = value; }

        SettingsEventArgs settings;

        public event EventHandler<SettingsEventArgs> changeInModel;

        public event EventHandler<string> OnHandlerRemoved;
        //public event EventHandler<SettingsEventArgs> changeInModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigModel"/> class.
        /// </summary>
        public ConfigModel()
        {
            isInitialized = false;
            Client client = Client.GetInstance();
            client.MessageReceived += MessageFromServer;
            client.SendCommand(CommandEnum.GetConfigCommand);
        }

        /// <summary>
        /// Sends the message to server.
        /// </summary>
        /// <param name="commandID">The command identifier.</param>
        /// <param name="args">The arguments.</param>
        public void SendMessageToServer(CommandEnum commandID, string args)
        {
            Client client = Client.GetInstance();
            client.SendMessageToServer(commandID, new string[] { args });
        }

        /// <summary>
        /// Messages from server.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="CommandEventArgs"/> instance containing the event data.</param>
        public void MessageFromServer(object sender, CommandEventArgs args)
        {
            CommandEnum commandID = args.CommandID;
            if (commandID == CommandEnum.GetConfigCommand)
            {
                string jsonSettings = args.CommandArgs[0];
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<SettingsEventArgs>(jsonSettings);
                this.settings = (SettingsEventArgs)obj;

                if (isInitialized == false)
                {
                    Handlers = settings.Handlers;
                }

                changeInModel?.Invoke(this, settings);

            }
            if (commandID == CommandEnum.RemoveHandlerCommand)
            {
                string jsonHandler = args.CommandArgs[0];
                OnHandlerRemoved?.Invoke(this, jsonHandler);
            }
        }


        /// <summary>
        /// Removes the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        public void RemoveHandler(string handler)
        {
            foreach (string item in Handlers)
            {
                bool result = item.Equals(handler);
                if (result == true)
                {
                    m_toBeDeletedHandler = item;
                }
            }


            Client client = Client.GetInstance();
            client.SendCommand(CommandEnum.RemoveHandlerCommand, new string[] { m_toBeDeletedHandler });
            List<string> handlers = new List<string>(Handlers);
            handlers.Remove(m_toBeDeletedHandler);
            Handlers = handlers.ToArray();

        }
    }
}