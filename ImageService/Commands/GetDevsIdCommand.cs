using Infrastructure.Event;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    class GetDevsIdCommand : ICommand
    {
        public string Execute(string[] args, out bool result)
        {
            IdEventArgs settings = new IdEventArgs();

            string firstName = ConfigurationManager.AppSettings["FirstName"];
            settings.FirstName = firstName;
            string secondName = ConfigurationManager.AppSettings["SecondName"];
            settings.SecondName = secondName;
            string firstId = ConfigurationManager.AppSettings["FirstId"];
            settings.FirstId = firstId;
            string secondId = ConfigurationManager.AppSettings["SecondId"];
            settings.SecondId = secondId;

            result = true;
            return Newtonsoft.Json.JsonConvert.SerializeObject(settings); ;
        }
    }
}
