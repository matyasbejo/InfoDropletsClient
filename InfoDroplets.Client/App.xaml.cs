using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using InfoDroplets.Logic;
using InfoDroplets.Models;
using InfoDroplets.Repository;
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
                    .AddSingleton<ISerialWrapper,SerialWrapper>()
                    .AddSingleton<IDropletLogic, DropletLogic>()
                    .AddSingleton<ITrackingEntryLogic, TrackingEntryLogic>()
                    .AddSingleton<IRepository<Droplet>, DropletRepository>()
                    .AddSingleton<IRepository<TrackingEntry>, TrackingEntryRepository>()
                    .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)
                    .AddDbContext<ClientDbContext>()
                    .BuildServiceProvider()
            );
        }
    }
}
