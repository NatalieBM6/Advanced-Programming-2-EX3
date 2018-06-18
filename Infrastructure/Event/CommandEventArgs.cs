using Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Event
{
    public class CommandEventArgs
    {
        public CommandEventArgs() { }
        public CommandEnum CommandID { get; set; }
        public string[] CommandArgs { get; set; }

    }
}
