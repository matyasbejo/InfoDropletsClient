using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InfoDroplets.Logic;
using InfoDroplets.Models;
using InfoDroplets.Utils.SerialCommunication;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace InfoDroplets.Client.ViewModels
{
    public class SerialWindowViewModel: ObservableRecipient
    {
        SerialWrapper serialWrapper;
        DropletLogic dropletLogic;

        public ObservableCollection<Droplet> DropletDetails { get; set; }

        public ObservableCollection<string> SerialPorts { get; set; }

        public ObservableCollection<int> PossibleBaudRates { get; set; }

        private string selectedPort;

        public string SelectedPort
        {
            get { return selectedPort; }
            set
            {
                if (SetProperty(ref selectedPort, value))
                {
                    selectedPort = value;
                    serialWrapper.SetPortName(value);
                }
            }
        }

        private int selectedBaudRate;

        public int SelectedBaudRate
        {
            get { return selectedBaudRate; }
            set
            {
                if (SetProperty(ref selectedBaudRate, value))
                {
                    selectedBaudRate = value;
                    serialWrapper.SetBaudeRate(value);
                }
            }
        }

        public SerialWindowViewModel()
        {
            this.PossibleBaudRates = new ObservableCollection<int>{ 4800, 9600, 19200, 38400, 57600, 115200, 230400, 460800, 921600 };
            serialWrapper = new SerialWrapper();
            var PortNamesList = SerialWrapper.GetPortNames();

            SerialPorts = new ObservableCollection<string>();
            foreach (var item in PortNamesList)
            {
                SerialPorts.Add(item);
            }
        }

        public static bool IsInDesignMode
        {
            get
            {
                var prop = DesignerProperties.IsInDesignModeProperty;
                return (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
            }
        }
    }
}
