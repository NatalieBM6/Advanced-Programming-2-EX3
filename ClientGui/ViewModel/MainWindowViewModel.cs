using ClientGui.Communication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ClientGui.ViewModel
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Brush WindowColor { get; set; }

        private bool serverConnected;
        public bool ServerConnected
        {
            get { return serverConnected; }
            set
            {
                serverConnected = value;
                if (serverConnected) WindowColor = Brushes.White;
                else WindowColor = Brushes.Gray;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WindowColor"));
            }
        }

        public MainWindowViewModel()
        {
            Client client = Client.GetInstance();
            ServerConnected = Client.isConnected;
            client.CheckConnection += CheckConnection;
            WindowColor = Brushes.Gray;
            if (ServerConnected) WindowColor = Brushes.MediumVioletRed;
        }

        public void CheckConnection(object sender, bool isConnected)
        {
            ServerConnected = isConnected;
        }
    }
}
