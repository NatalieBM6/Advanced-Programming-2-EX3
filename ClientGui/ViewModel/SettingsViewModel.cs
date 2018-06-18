using ClientGui.Model;
using Infrastructure.Enums;
using Infrastructure.Event;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClientGui.ViewModel
{
    class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsViewModel()
        {
            this.settingsModel = new SettingsModel();
            settingsModel.changeInModel += this.SettingsEvent;
            settingsModel.OnHandlerRemoved += RemoveSelectedHandler;
            handlers = new ObservableCollection<HandlerPath>();
            this.RemoveHandler = new DelegateCommand<object>(this.OnRemoveHandler, this.CanRemoveHandler);
        }

        private string outputDir = "default";
        private string sourceName = "default";
        private string logName = "default";
        private string thumbnailSize = "default";

        public ObservableCollection<HandlerPath> handlers;
        private HandlerPath selectedHandler;
        private bool isSelected;

        private SettingsModel settingsModel;

        public ICommand RemoveHandler { get; set; }



        public string OutputDir
        {
            get => outputDir; set
            {
                outputDir = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OutputDir"));
            }
        }
        public string SourceName
        {
            get => sourceName; set
            {
                sourceName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SourceName"));
            }
        }
        public string LogName
        {
            get => logName; set
            {
                logName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LogName"));

            }
        }
        public string ThumbnailSize
        {
            get => thumbnailSize; set
            {
                thumbnailSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ThumbnailSize"));
            }
        }



        public HandlerPath SelectedHandler
        {
            get { return selectedHandler; }
            set
            {
                if (value != this.selectedHandler)
                {
                    selectedHandler = value;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedHandler"));
                RaiseCanExecuteChange();
            }
        }

        /// Gets or sets the handlers.
        public ObservableCollection<HandlerPath> Handlers
        {
            get { return handlers; }
            set
            {
                if (value != this.handlers)
                    handlers = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Handlers"));
            }
        }

        private void RaiseCanExecuteChange()
        {
            DelegateCommand<object> command = RemoveHandler as DelegateCommand<object>;
            command.RaiseCanExecuteChanged();
        }



        public void SettingsEvent(object sender, SettingsEventArgs e)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                OutputDir = e.OutputDir;
                SourceName = e.SourceName;
                LogName = e.LogName;
                ThumbnailSize = e.ThumbnailSize;
                foreach (string handler in e.Handlers)
                {
                    handlers.Add(new HandlerPath() { Path = handler });
                }
            });
        }


        private void RemoveSelectedHandler(object sender, string handler)
        {
            HandlerPath removedHandler = new HandlerPath() { Path = handler };
            foreach (HandlerPath h in handlers) { if (removedHandler.Path == h.Path) removedHandler = h; }
            try { App.Current.Dispatcher.Invoke((System.Action)delegate { handlers.Remove(removedHandler); }); }
            catch { }
        }

        private bool CanRemoveHandler(object obj)
        {
            return selectedHandler != null;
        }

        private void OnRemoveHandler(object obj)
        {
            string handler = selectedHandler.Path;
            selectedHandler = null;
            Task.Run(() => { settingsModel.SendMessageToServer(CommandEnum.RemoveHandlerCommand, handler); });
            //handlers.Remove(selectedHandler);

        }

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (value != isSelected) isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }

    }
}
