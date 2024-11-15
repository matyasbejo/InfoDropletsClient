using CommunityToolkit.Mvvm.ComponentModel;
using InfoDroplets.Logic;
using InfoDroplets.Models;
using System.Collections.ObjectModel;

namespace InfoDroplets.Client.ViewModels
{
    internal class MainWindowViewModel : ObservableRecipient
    {
        DropletLogic dropletLogic;

        public ObservableCollection<Droplet> DropletDetails { get; set; }


    }
}
