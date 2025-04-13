using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using InfoDroplets.Logic;
using InfoDroplets.Models;
using InfoDroplets.Utils.SerialCommunication;
using InfoDropletsClient;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Documents;
using System.Windows.Input;

namespace InfoDroplets.Client.ViewModels
{
    public class SerialWindowViewModel: ObservableRecipient
    {
        ISerialWrapper serialWrapper;
        public ObservableCollection<string> SerialPorts { get; set; }

        public ObservableCollection<int> PossibleBaudRates { get; set; }

        public ICommand StartCommand {  get; set; }

        private string selectedPort;
        public string SelectedPort
        {
            get { return selectedPort; }
            set
            {
                SetProperty(ref selectedPort, value);
                (StartCommand as RelayCommand).NotifyCanExecuteChanged();
            }
        }

        private int selectedBaudRate;
        public int SelectedBaudRate
        {
            get { return selectedBaudRate; }
            set
            {
                SetProperty(ref selectedBaudRate, value);
                (StartCommand as RelayCommand).NotifyCanExecuteChanged();
            }
        }
        public SerialWindowViewModel() : this(IsInDesignMode? null : Ioc.Default.GetService<ISerialWrapper>())
        {
            
        }

        public SerialWindowViewModel(ISerialWrapper wrapper)
        {
            serialWrapper = wrapper;
            PossibleBaudRates = new ObservableCollection<int>{ 4800, 9600, 19200, 38400, 57600, 115200, 230400, 460800, 921600 };
            var PortNamesList = serialWrapper.AvaliableSerialPorts;
            SerialPorts = new ObservableCollection<string>(PortNamesList);

            StartCommand = new RelayCommand
                (
                    () => 
                    {
                        serialWrapper.SelectedBaudRate = SelectedBaudRate;
                        serialWrapper.SelectedSerialPort = SelectedPort;
                        serialWrapper.SetBaudeRate(SelectedBaudRate);
                        serialWrapper.SetPortName(SelectedPort);
                        serialWrapper.SafeOpen();
                        Messenger.Send("PortSetupDone", "SerialPortInfo");
                    },
                    () => selectedBaudRate != 0 && selectedPort != null
                );
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
