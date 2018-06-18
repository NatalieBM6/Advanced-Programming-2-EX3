using ClientGui.Communication;
using Infrastructure.Enums;
using Infrastructure.Event;
using Logging.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientGui.Model
{
    class LogsModel
    {
        public event EventHandler<MessageRecievedEventArgs> ReceivedLog;

        public LogsModel()
        {
            Client client = Client.GetInstance();
            client.MessageReceived += MessageFromServer;
            Task.Run(() => client.SendMessageToServer(CommandEnum.GetAllLogsCommand, new string[] { }));
        }


        public MessageRecievedEventArgs ParseLogFromString(string log)
        {
            string[] args = log.Split(';');
            string message = args[0];
            MessageTypeEnum status = (MessageTypeEnum)(int.Parse(args[1]));
            return new MessageRecievedEventArgs(message, status);
        }

        public void MessageFromServer(object sender, CommandEventArgs args)
        {
            CommandEnum commandID = args.CommandID;
            if (commandID == CommandEnum.GetAllLogsCommand)
            {
                Client client = (Client)sender;
                string message = args.CommandArgs[0];
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageRecievedEventArgs>>(message);
                List<MessageRecievedEventArgs> logs = (List<MessageRecievedEventArgs>)obj;
                logs.Reverse();
                foreach (MessageRecievedEventArgs log in (List<MessageRecievedEventArgs>)obj) { ReceivedLog?.Invoke(this, log); }
            }
            else if (commandID == CommandEnum.LogCommand)
            {
                Client client = (Client)sender;
                string message = args.CommandArgs[0];
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<MessageRecievedEventArgs>(args.CommandArgs[0]);

                MessageRecievedEventArgs SpecificLogView = (MessageRecievedEventArgs)obj;
                ReceivedLog?.Invoke(this, SpecificLogView);
            }
        }
    }
}
