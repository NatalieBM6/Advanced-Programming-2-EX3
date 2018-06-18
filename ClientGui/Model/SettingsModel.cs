using ClientGui.Communication;
using Infrastructure.Enums;
using Infrastructure.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientGui.Model
{
    class SettingsModel
    {
        public event EventHandler<string> OnHandlerRemoved;
        public event EventHandler<SettingsEventArgs> changeInModel;


        public SettingsModel()
        {
            Client client = Client.GetInstance();
            client.MessageReceived += MessageFromServer;
            Task.Run(() => client.SendMessageToServer(CommandEnum.GetConfigCommand, new string[] { "" }));
        }


        public void SendMessageToServer(CommandEnum commandID, string args)
        {
            Client client = Client.GetInstance();
            client.SendMessageToServer(commandID, new string[] { args });
        }

        public void MessageFromServer(object sender, CommandEventArgs args)
        {
            CommandEnum commandID = args.CommandID;
            if (commandID == CommandEnum.GetConfigCommand)
            {
                string jsonSettings = args.CommandArgs[0];
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<SettingsEventArgs>(jsonSettings);
                changeInModel?.Invoke(this, (SettingsEventArgs)obj);
            }
            if (commandID == CommandEnum.RemoveHandlerCommand)
            {
                string jsonHandler = args.CommandArgs[0];
                OnHandlerRemoved?.Invoke(this, jsonHandler);
            }
        }
    }
}
