using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using GMap.NET;
using InfoDroplets.Logic;
using InfoDroplets.Models;
using InfoDroplets.Utils.Enums;
using InfoDroplets.Utils.SerialCommunication;
using System.ComponentModel;
using System.IO.Ports;
using System.Security;
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

        public ICommand StartSerialCommand { get; set; }
        public ICommand StopSerialCommand { get; set; }
        public ICommand RefreshPortsCommand { get; set; }

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

        #endregion

        #region Radio control declarations

        public ICommand RCFullResetCommand { get; set; }
        public ICommand RCPingCommand { get; set; }
        public ICommand RCFileVersionCommand { get; set; }

        private string rcStateMessage;
        private bool rcResponseReceived;
        public string RcStateMessage
        {
            get
            {
                return rcStateMessage;
            }
            set 
            {
                RcStateUpdateTime = DateTime.Now.ToString("H:m:ss");
                OnPropertyChanged("RcStateUpdateTime");
                rcStateMessage = value;
            } 
        }
        public string RcStateUpdateTime { get; set; }
        public bool RcButtonsEnabled { get; set; }

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
                    serialWrapper.WrapperDataReceived += OnDataReceived;
                    bool success = serialWrapper.SafeOpen();
                    if (success)
                    {
                        Messenger.Send("PortOpened", "SerialPortInfo");
                        OnPropertyChanged("IsRCEnabled");

                        (RefreshPortsCommand as RelayCommand).NotifyCanExecuteChanged();
                        (StartSerialCommand as RelayCommand).NotifyCanExecuteChanged();
                        (StopSerialCommand as RelayCommand).NotifyCanExecuteChanged();
                    }
                    else
                        MessageBox.Show("Can't ground unit. Check the selected port!","Error",MessageBoxButton.OK, MessageBoxImage.Error);
                },
                () => !serialWrapper.IsOpen && selectedBaudRate != 0 && selectedPort != null);

            StopSerialCommand = new RelayCommand(
                () =>
                {
                    serialWrapper.SafeClose();
                    serialWrapper.WrapperDataReceived -= OnDataReceived;
                    Messenger.Send("PortClosed", "SerialPortInfo");
                    OnPropertyChanged("IsRCEnabled");

                    (RefreshPortsCommand as RelayCommand).NotifyCanExecuteChanged();
                    (StopSerialCommand as RelayCommand).NotifyCanExecuteChanged();
                    (StartSerialCommand as RelayCommand).NotifyCanExecuteChanged();
                },
                () => serialWrapper.IsOpen);

            RefreshPortsCommand = new RelayCommand(
                () =>
                {
                    OnPropertyChanged("SerialWrapper");
                },
                () => !serialWrapper.IsOpen);

            RCFullResetCommand = new RelayCommand(
                () => SendRcCommand(RadioCommand.FullReset),
                () => RcButtonsEnabled
                );
            
            RCFileVersionCommand = new RelayCommand(
                () => SendRcCommand(RadioCommand.GetFileVersion),
                () => RcButtonsEnabled
                );
            
            RCPingCommand = new RelayCommand(
                () => SendRcCommand(RadioCommand.Ping),
                () => RcButtonsEnabled
                );

            serialWrapper = wrapper;
            DropletLogic = dropletLogic;
            TrackingEntryLogic = trackingEntryLogic;
            dropletLogic.CommandGenerated += wrapper.SendCommand;
            rcStateMessage = "Information will be displayed here";
            RcButtonsEnabled = true;
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
                    catch (ArgumentException ex)
                    {
                        var rcResponse = TryToGenerateRCResponse(line);
                        if(rcResponse != string.Empty)
                        {
                            rcResponseReceived = true;

                            RcStateMessage = rcResponse;
                            OnPropertyChanged("RcStateMessage");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ex.StackTrace,"Exception",MessageBoxButton.OK, MessageBoxImage.Error); 
                }
            }
        }

        string TryToGenerateRCResponse(string input)
        {
            if (!input.ToLower().Contains("rcresponse:"))
            {
                return string.Empty;
            }
            else
            {
                return input.Trim().Replace("RCRESPONSE:", "");
            }
        }

        async Task<bool> SendRcCommand(RadioCommand command)
        {

            if (command == RadioCommand.FullReset)
            {
                var result = MessageBox.Show("Are you sure you want to restart the device?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                    return true;
            }

            rcResponseReceived = false;
            DropletLogic.SendCommand(SelectedDroplet.Id, command);

            ChangeAndNotifyRCCommands(false);

            RcStateMessage = $"{command} #{SelectedDroplet.Id} sent";
            OnPropertyChanged("RcStateMessage");
            await Task.Delay(10000);

            ChangeAndNotifyRCCommands(true);

            if (!rcResponseReceived)
            { 
                RcStateMessage = $"Not ok: {command} #{SelectedDroplet.Id} failed";
                OnPropertyChanged("RcStateMessage");
                return false;
            }

            return true;
        }

        void ChangeAndNotifyRCCommands(bool state)
        {
            RcButtonsEnabled = state;
            (RCPingCommand as RelayCommand).NotifyCanExecuteChanged();
            (RCFullResetCommand as RelayCommand).NotifyCanExecuteChanged();
            (RCFileVersionCommand as RelayCommand).NotifyCanExecuteChanged();
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
