using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using InfoDroplets.Logic;
using InfoDroplets.Models;
using InfoDroplets.Utils.SerialCommunication;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace InfoDroplets.Client.ViewModels
{
    internal class MainWindowViewModel : ObservableRecipient
    {
        ISerialWrapper serialWrapper;
        public MainWindowViewModel(SerialWrapper serialWrapper)
        {
            var a = serialWrapper.GetPortName();
            var b = serialWrapper.GetBaudeRate();
            this.serialWrapper = serialWrapper;
        }

        public MainWindowViewModel() : this(IsInDesignMode ? null : Ioc.Default.GetService<ISerialWrapper>())
        {

        }

        public MainWindowViewModel(ISerialWrapper serialWrapper)
        {

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
