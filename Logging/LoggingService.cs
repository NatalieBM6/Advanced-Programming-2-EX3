using Logging.Modal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging
{
    public class LoggingService : ILoggingService
    {
        public event EventHandler<MessageRecievedEventArgs> MessageReceived;
        /************************************************************************
        *The Input: a message and it's type.
        *The Output: -
        *The Function operation: The function Notifies all of the subscribers about
        *a new message recieved and logging it into the entry log.
        *************************************************************************/
        public void Log(string message, MessageTypeEnum type)
        {
            MessageReceived?.Invoke(this, new MessageRecievedEventArgs(message, type));
        }

        public EventLogEntryType GetMessageType(MessageTypeEnum type)
        {
            switch (type)
            {
                case (MessageTypeEnum.INFO): return EventLogEntryType.Information;
                case (MessageTypeEnum.WARNING): return EventLogEntryType.Warning;
                case (MessageTypeEnum.FAIL): return EventLogEntryType.FailureAudit;
            }
            return EventLogEntryType.Information;
        }

    }
}
