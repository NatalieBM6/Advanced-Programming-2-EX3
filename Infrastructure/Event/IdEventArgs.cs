using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Event
{
    public class IdEventArgs : EventArgs
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FirstId { get; set; }
        public string SecondId { get; set; }
    }
}
