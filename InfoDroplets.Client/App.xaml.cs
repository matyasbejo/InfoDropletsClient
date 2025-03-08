using CommunityToolkit.Mvvm.DependencyInjection;
using InfoDroplets.Utils.SerialCommunication;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace InfoDropletsClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                    .AddSingleton<SerialWrapper>()
                    .BuildServiceProvider()
            );
        }
    }
}
