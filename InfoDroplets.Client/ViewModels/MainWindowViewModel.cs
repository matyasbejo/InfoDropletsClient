using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using InfoDroplets.Logic;
using InfoDroplets.Models;
using InfoDroplets.Utils.SerialCommunication;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace InfoDroplets.Client.ViewModels
{
    internal class MainWindowViewModel : ObservableRecipient
    {
        public ISerialWrapper SerialWrapper { get; set; }

        IDropletLogic DropletLogic { get; set; }

        ITrackingEntryLogic TrackingEntryLogic { get; set; }
        SerialWindow serialWindow { get; set; }

        public ICommand ChangeSerialPortSettingsCommand { get; set; }

        public MainWindowViewModel() : this(IsInDesignMode ? null : 
            Ioc.Default.GetService<ISerialWrapper>(), Ioc.Default.GetService<IDropletLogic>(), Ioc.Default.GetService<ITrackingEntryLogic>())
        {

        }

        public MainWindowViewModel(ISerialWrapper wrapper, IDropletLogic dropletLogic, ITrackingEntryLogic trackingEntryLogic)
        {
            SerialWrapper = wrapper;
            DropletLogic = dropletLogic;
            TrackingEntryLogic = trackingEntryLogic;

            serialWindow = new SerialWindow();

            Messenger.Register<MainWindowViewModel, string, string>(this, "SerialPortInfo", (recipient, msg) =>
            {
                OnPropertyChanged("serialWrapper");
            });

            ChangeSerialPortSettingsCommand = new RelayCommand(
                () =>
                {
                    SerialWrapper.SafeClose();
                    serialWindow.Show();
                });

            SerialWrapper.WrapperDataReceived += OnDataReceived;
        }

        public static bool IsInDesignMode
        {
            get
            {
                var isInDesignModeProp = DesignerProperties.IsInDesignModeProperty;
                return (bool)DependencyPropertyDescriptor.FromProperty(isInDesignModeProp, typeof(FrameworkElement)).Metadata.DefaultValue;
            }
        }

        void OnDataReceived(object sender, EventArgs e)
        {
            var line = SerialWrapper.ReadLine();
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
                //Console.WriteLine($"Added: {line}");
            }
            catch (Exception ex)
            { //Console.WriteLine(ex.Message); }
            }
        }
    }
}
