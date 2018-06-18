using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using ImageService.Server;
using ImageService.Controller;
using ImageService.Modal;
using Logging;
using Logging.Modal;
using System.Configuration;
using Infrastructure;

namespace ImageService
{
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    };

    public partial class ImageService : ServiceBase
    {
        private int eventId = 1;
        private ImageServer m_imageServer; // The Image Server
        private IImageServiceModal model;
        private IImageController controller;
        private ILoggingService logging;

        /************************************************************************
        *The Input: arguments.
        *The Output: -.
        *The Function operation: The function builds an ImageService.
        *************************************************************************/
        public ImageService(string[] args)
        {
            // First reading from the appConfig creating the eventLog.
            InitializeComponent();
            string eventSourceName = ConfigurationManager.AppSettings["SourceName"];
            string logName = ConfigurationManager.AppSettings["LogName"];

            //If there are arguments.
            if (args.Count() > 0)
            {
                eventSourceName = args[0];
            }
            if (args.Count() > 1)
            {
                logName = args[1];
            }
            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists(eventSourceName))
            {
                System.Diagnostics.EventLog.CreateEventSource(eventSourceName, logName);
            }
            eventLog1.Log = logName;
            eventLog1.Source = eventSourceName;
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        /************************************************************************
        *The Input: arguments.
        *The Output: -.
        *The Function operation: The function starts the service.
        *************************************************************************/
        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("Starting The ImageService");
            InitializeService();
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("Stopping The ImageService");
            this.m_imageServer.CloseServer();
        }

        /************************************************************************
        *The Input: The sender and the events descriptors.
        *The Output: -.
        *The Function operation: The function updates the entry according to the 
        *recieved message.
        *************************************************************************/
        public void OnLog(object sender, MessageRecievedEventArgs e)
        {
            //Updating the log according to the message (INFO/ FAIL/ WARNING).
            switch (e.Status)
            {
                case MessageTypeEnum.INFO:
                    eventLog1.WriteEntry(e.Message, EventLogEntryType.Information, eventId++);
                    break;
                case MessageTypeEnum.FAIL:
                    eventLog1.WriteEntry(e.Message, EventLogEntryType.FailureAudit, eventId++);
                    break;
                case MessageTypeEnum.WARNING:
                    eventLog1.WriteEntry(e.Message, EventLogEntryType.Warning, eventId++);
                    break;
            }
        }


        /************************************************************************
        *The Input: -.
        *The Output: -.
        *The Function operation: The function reads from the appConfig and initiallizes
        * and creates members.
        *************************************************************************/
        private void InitializeService()
        {
            //Reading appConfig.
            string[] handlerPaths = ConfigurationManager.AppSettings["Handler"].Split(';');
            string outputDir = ConfigurationManager.AppSettings["OutputDir"];
            int thumbnailSize = Int32.Parse(ConfigurationManager.AppSettings["ThumbnailSize"]);

            //Initializing and creating the members.
            this.model = new ImageServiceModel(outputDir, thumbnailSize);
            this.logging = new LoggingService();
            this.controller = new ImageController(this.model);
            logging.MessageReceived += OnLog;
            this.m_imageServer = new ImageServer(this.controller, this.logging, handlerPaths);
            //Updating the entry.
            eventLog1.WriteEntry("End Of Initialization");
        }
    }
}