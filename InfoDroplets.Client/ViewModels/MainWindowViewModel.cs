using CommunityToolkit.Mvvm.ComponentModel;
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
        SerialWrapper serialWrapper;
        public MainWindowViewModel(SerialWrapper serialWrapper)
        {
            var a = serialWrapper.GetPortName();
            var b = serialWrapper.GetBaudeRate();
            this.serialWrapper = serialWrapper;
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
