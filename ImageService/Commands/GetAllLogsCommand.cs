using Logging.Modal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    class GetAllLogsCommand : ICommand
    {
        public string Execute(string[] args, out bool result)
        {
            List<MessageRecievedEventArgs> logsList = new List<MessageRecievedEventArgs>();
            string logName = ConfigurationManager.AppSettings["LogName"];
            string sourceName = ConfigurationManager.AppSettings["SourceName"];
            EventLog[] allLogs = EventLog.GetEventLogs();
            EventLog log = null;
            foreach (EventLog currLog in allLogs)
            {
                if (currLog.Log == logName)
                {
                    log = currLog;
                    break;
                }
            }
            if (log == null)
            {
                result = false;
                return "Couldn't retreive log entries from Log: " + logName + "with Source " + sourceName;
            }
            EventLogEntryCollection entries = log.Entries;
            // foreach (EventLogEntry entry in entries)
            for (int i = entries.Count - 1; i >= 0; i--)
            {
                //sift through all enrties
                EventLogEntry entry = entries[i];
                MessageRecievedEventArgs e = new MessageRecievedEventArgs(entry.Message, MessageRecievedEventArgs.ConvertType(entry.EntryType));
                logsList.Add(e);
                if (entry.Message == "In OnStart") break;
            }
            result = true;
            return Newtonsoft.Json.JsonConvert.SerializeObject(logsList);
        }
    }
}
