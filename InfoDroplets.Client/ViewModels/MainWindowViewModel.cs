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

//Next: irány é-d-k-ny, magasság trend, mi legyen a dbcontext erorral

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

        public List<int> AvaliableDropletIds
        {
            get 
            {
                return DropletLogic.ReadAllIds().ToList();
            }
        }

        public int? SelectedId{ get; set; }
        public Droplet? SelectedDroplet { 
            get 
            {
                if (SelectedId == null)
                    return null;

                else
                    return DropletLogic.Read(SelectedId.Value); 
            } 
        }

        public PointLatLng MapPos
        {
            get { 
                if(SelectedDroplet != null)
                    return new PointLatLng(SelectedDroplet.LastData.Latitude, SelectedDroplet.LastData.Longitude); 
                else
                    return new PointLatLng(0,0);
            }
        }

        public bool IsRCEnabled { get { return serialWrapper.IsOpen && AvaliableDropletIds.Count > 0; } }

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
                    OnPropertyChanged("IsRCEnabled");

                    (StartSerialCommand as RelayCommand).NotifyCanExecuteChanged();
                    (StopSerialCommand as RelayCommand).NotifyCanExecuteChanged();
                },
                () => !serialWrapper.IsOpen && selectedBaudRate != 0 && selectedPort != null && selectedBaudRate != 921600);

            StopSerialCommand = new RelayCommand(
                () =>
                {
                    serialWrapper.SafeClose();
                    serialWrapper.WrapperDataReceived -= OnDataReceived;
                    Messenger.Send("PortClosed", "SerialPortInfo");
                    OnPropertyChanged("IsRCEnabled");

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
                        var newDropletId = int.Parse(line.Trim().Split(';')[0]);

                        DropletLogic.UpdateDropletStatus(newDropletId, new GpsPos(47.500429, 19.084596, 100));
                        OnPropertyChanged("SelectedDroplet");
                        OnPropertyChanged("MapPos");
                    }
                    catch (NullReferenceException ex)
                    {
                        var newDropletId = int.Parse(line.Trim().Split(';')[0]);
                        DropletLogic.Create(new Droplet(newDropletId));
                        OnPropertyChanged("AvaliableDropletIds");
                        OnPropertyChanged("IsRCEnabled");
                    }
                }
                catch (Exception ex)
                { 
                    //Console.WriteLine(ex.Message); 
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
