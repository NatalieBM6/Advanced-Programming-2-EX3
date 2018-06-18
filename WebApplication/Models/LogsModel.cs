using Infrastructure.Enums;
using Infrastructure.Event;
using Logging.Modal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication.ServerCommunication;

namespace WebApplication.Models
{
    public class LogsModel
    {

        public event EventHandler<MessageRecievedEventArgs> ReceivedLog;

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Log Enteries")]
        public List<Log> LogEntries { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogsModel"/> class.
        /// </summary>
        public LogsModel()
        {
            LogEntries = new List<Log>();
            Client client = Client.GetInstance();
            client.MessageReceived += MessageFromServer;
            client.SendCommand(CommandEnum.GetAllLogsCommand);

        }


        /// <summary>
        /// Parses the log from string.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public MessageRecievedEventArgs ParseLogFromString(string log)
        {
            string[] args = log.Split(';');
            string message = args[0];
            MessageTypeEnum status = (MessageTypeEnum)(int.Parse(args[1]));
            return new MessageRecievedEventArgs(message, status);
        }

        /// <summary>
        /// Messages from server.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="CommandEventArgs"/> instance containing the event data.</param>
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
                foreach (MessageRecievedEventArgs log in (List<MessageRecievedEventArgs>)obj) {
                    String StatusString = ConvertEnumType(log.Status);

                    LogEntries.Add(new Log { EntryType = log.Status, Message = log.Message, Status = StatusString });
                }
            }
            else if (commandID == CommandEnum.LogCommand)
            {
                Client client = (Client)sender;
                string message = args.CommandArgs[0];
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<MessageRecievedEventArgs>(args.CommandArgs[0]);

                MessageRecievedEventArgs SpecificLogView = (MessageRecievedEventArgs)obj;
                String StatusString = ConvertEnumType(SpecificLogView.Status);
                LogEntries.Insert(0, new Log { EntryType = SpecificLogView.Status, Message = SpecificLogView.Message,
                    Status = StatusString });
            }
        }

        /// <summary>
        /// Converts the type of the enum.
        /// </summary>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        public string ConvertEnumType(MessageTypeEnum num)
        {
            String StatusString = "";

            switch (num)
            {
                case MessageTypeEnum.FAIL:
                    StatusString = "FAIL";
                    break;
                case MessageTypeEnum.INFO:
                    StatusString = "INFO";
                    break;
                case MessageTypeEnum.WARNING:
                    StatusString = "WARNING";
                    break;
                default:
                    {
                        StatusString = "INFO";
                        break;
                    }
            }
            return StatusString;
        }

    }
}