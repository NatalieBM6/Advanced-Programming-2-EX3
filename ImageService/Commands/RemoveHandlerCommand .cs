using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    public class RemoveHandlerCommand : ICommand
    {
        public event EventHandler<string> RemoveHandler;

        public string Execute(string[] args, out bool result)
        {
            RemoveHandler?.Invoke(this, args[0]);
            result = true;
            return args[0];
        }
    }
}
