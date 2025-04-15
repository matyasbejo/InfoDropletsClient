using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using GMap.NET;
using InfoDroplets.Logic;
using InfoDroplets.Models;
using InfoDroplets.Utils.SerialCommunication;
using System.ComponentModel;
using System.IO.Ports;
using System.Windows;
using System.Windows.Input;

namespace InfoDroplets.Client.ViewModels
{
    internal class MainWindowViewModel : ObservableRecipient
    {
        #region Serial declarations

        private ISerialWrapper serialWrapper;
        public ISerialWrapper SerialWrapper
        {
            get { return serialWrapper; }
            set
            {
                SetProperty(ref serialWrapper, value);
                (StartSerialCommand as RelayCommand).NotifyCanExecuteChanged();
                (StopSerialCommand as RelayCommand).NotifyCanExecuteChanged();
            }
        }

        private string selectedPort;
        public string SelectedPort
        {
            get { return selectedPort; }
            set
            {
                SetProperty(ref selectedPort, value);
                (StartSerialCommand as RelayCommand).NotifyCanExecuteChanged();
                (StopSerialCommand as RelayCommand).NotifyCanExecuteChanged();
            }
        }

        private int selectedBaudRate;
        public int SelectedBaudRate
        {
            get { return selectedBaudRate; }
            set
            {
                SetProperty(ref selectedBaudRate, value);
                (StartSerialCommand as RelayCommand).NotifyCanExecuteChanged();
                (StopSerialCommand as RelayCommand).NotifyCanExecuteChanged();
            }
        }
        public ICommand StartSerialCommand { get; set; }
        public ICommand StopSerialCommand { get; set; }

        #endregion

        public Droplet? SelectedDroplet { get { return DropletLogic.Read(8); } }

        public PointLatLng MapCenterPos
        {
            get
            {
                return new PointLatLng(SelectedDroplet.LastData.Latitude, SelectedDroplet.LastData.Longitude);
            }
        }

        IDropletLogic DropletLogic { get; set; }
        ITrackingEntryLogic TrackingEntryLogic { get; set; }

        public MainWindowViewModel() : this(IsInDesignMode ? null : 
            Ioc.Default.GetService<ISerialWrapper>(), Ioc.Default.GetService<IDropletLogic>(), Ioc.Default.GetService<ITrackingEntryLogic>())
        {

        }

        public MainWindowViewModel(ISerialWrapper wrapper, IDropletLogic dropletLogic, ITrackingEntryLogic trackingEntryLogic)
        {
            Messenger.Register<MainWindowViewModel, string, string>(this, "SerialPortInfo", (recipient, msg) =>
            {
                OnPropertyChanged("SerialWrapper");
            });

            StartSerialCommand = new RelayCommand(
                () =>
                {
                    serialWrapper.SetBaudeRate(selectedBaudRate);
                    serialWrapper.SetPortName(selectedPort);
                    serialWrapper.SafeOpen();
                    serialWrapper.WrapperDataReceived += OnDataReceived;
                    Messenger.Send("PortOpened", "SerialPortInfo");
                    OnPropertyChanged("SerialWrapper");
                    (StartSerialCommand as RelayCommand).NotifyCanExecuteChanged();
                    (StopSerialCommand as RelayCommand).NotifyCanExecuteChanged();
                },
                () => !serialWrapper.IsOpen && selectedBaudRate != 0 && selectedPort != null && selectedBaudRate != 921600);

            StopSerialCommand = new RelayCommand(
                () =>
                {
                    serialWrapper.WrapperDataReceived -= OnDataReceived;
                    serialWrapper.SafeClose();
                    Messenger.Send("PortClosed", "SerialPortInfo");
                    (StopSerialCommand as RelayCommand).NotifyCanExecuteChanged();
                    (StartSerialCommand as RelayCommand).NotifyCanExecuteChanged();
                },
                () => serialWrapper.IsOpen && selectedBaudRate != 0 && selectedPort != null && selectedBaudRate != 4800);

            serialWrapper = wrapper;
            DropletLogic = dropletLogic;
            TrackingEntryLogic = trackingEntryLogic;
        }

        void OnDataReceived(object sender, EventArgs e)
        {
            if (serialWrapper.IsOpen)
            {
                var line = serialWrapper.ReadLine();
                try
                {
                    try
                    {
                        TrackingEntryLogic.Create(line);
                    }
                    catch (NullReferenceException ex)
                    {
                        var newDropletId = int.Parse(line.Trim().Split(';')[0]);
                        DropletLogic.Create(new Droplet(newDropletId));
                        //Console.WriteLine($"Droplet {newDropletId} added.");
                    }
                    DropletLogic.UpdateDropletStatus(8, new GpsPos(47.500429, 19.084596, 100));
                    OnPropertyChanged("MapCenterPos");
                    OnPropertyChanged("SelectedDroplet");

                    //Console.WriteLine($"Added: {line}");
                }
                catch (Exception ex)
                { //Console.WriteLine(ex.Message); }
                }
            }
        }
        public static bool IsInDesignMode
        {
            get
            {
                var isInDesignModeProp = DesignerProperties.IsInDesignModeProperty;
                return (bool)DependencyPropertyDescriptor.FromProperty(isInDesignModeProp, typeof(FrameworkElement)).Metadata.DefaultValue;
            }
        }
    }
}
