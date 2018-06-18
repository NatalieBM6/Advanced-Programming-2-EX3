using ImageService.Commands;
using Infrastructure;
using Infrastructure.Enums;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller
{
    public class ImageController : IImageController
    {
        private IImageServiceModal m_model; // The Model Object
        private Dictionary<CommandEnum, ICommand> commands;

        public ImageController(IImageServiceModal model)
        {
            m_model = model; // Storing the Model Of The System
            commands = new Dictionary<CommandEnum, ICommand>
            {
                { CommandEnum.NewFileCommand, new NewFileCommand(m_model) },
                {CommandEnum.GetConfigCommand, new GetConfigCommand() },
                {CommandEnum.RemoveHandlerCommand, new RemoveHandlerCommand() },
                {CommandEnum.GetAllLogsCommand, new GetAllLogsCommand() },
                {CommandEnum.GetDevsIdCommand, new GetDevsIdCommand() }

            };
        }

        /************************************************************************
        *The Input: The command's descriptors.
        *The Output: The path of the file if the creation was succesful.
        *The Function operation: The function executes the given command.
        *************************************************************************/
        public string ExecuteCommand(CommandEnum commandID, string[] args, out bool resultSuccesful)
        {
            {
                if (!commands.ContainsKey(commandID))
                {
                    resultSuccesful = false;
                    return "Command not found";
                }
                ICommand command = commands[commandID];

                // Defining thread for moving files.
                Task<Tuple<string, bool>> executeTask = new Task<Tuple<string, bool>>(() =>
                {
                    bool result;
                    string retVal = commands[commandID].Execute(args, out result);
                    return Tuple.Create(retVal, result);
                });

                executeTask.Start();
                resultSuccesful = executeTask.Result.Item2;
                return executeTask.Result.Item1;
            }
        }


        public void AddCommand(CommandEnum commandID, ICommand command)
        {
            if (commands.ContainsKey(commandID)) commands[commandID] = command;
            else commands.Add(commandID, command);
        }
    }
}