using Infrastructure.Enums;
using Infrastructure.Event;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebApplication.ServerCommunication;

namespace WebApplication.Models
{
    public class ImageWeb
    {

        public bool ServerConnected;

        private static string outputDir;
        public string[] studentNamesAndIds;

        public event EventHandler<IdEventArgs> changeInModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageWeb"/> class.
        /// </summary>
        public ImageWeb()
        {
            Client client = Client.GetInstance();
            client.MessageReceived += MessageFromServer;
            client.SendCommand(CommandEnum.GetDevsIdCommand);

        }


        /// <summary>
        /// Checks the connection.
        /// </summary>
        /// <returns></returns>
        public bool CheckConnection()
        {
            Client client = Client.GetInstance();
            ServerConnected = Client.isConnected;
            client.CheckConnection += CheckConnection;

            if (this.ServerConnected != true)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks the connection.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="isConnected">if set to <c>true</c> [is connected].</param>
        public void CheckConnection(object sender, bool isConnected)
        {
            ServerConnected = isConnected;
        }


        /// <summary>
        /// Counts the images in output dir.
        /// </summary>
        /// <returns></returns>
        public int CountImagesInOutputDir()
        {
            int sum = 0;

            outputDir = AppDomain.CurrentDomain.BaseDirectory;
            outputDir += "OutPutImages";
            DirectoryInfo di = new DirectoryInfo(outputDir);
            sum += di.GetFiles("*.PNG", SearchOption.AllDirectories).Length;
            sum += di.GetFiles("*.BMP", SearchOption.AllDirectories).Length;
            sum += di.GetFiles("*.JPG", SearchOption.AllDirectories).Length;
            sum += di.GetFiles("*.GIF", SearchOption.AllDirectories).Length;

            return sum / 2;
        }

        /// <summary>
        /// Messages from server.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="CommandEventArgs"/> instance containing the event data.</param>
        public void MessageFromServer(object sender, CommandEventArgs args)
        {

            CommandEnum commandID = args.CommandID;
            if (commandID == CommandEnum.GetDevsIdCommand)
            {
     
                string jsonSettings = args.CommandArgs[0];
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<IdEventArgs>(jsonSettings);
                IdEventArgs settings = (IdEventArgs)obj;
                changeInModel?.Invoke(this, settings);

            }

        }
    }
}


