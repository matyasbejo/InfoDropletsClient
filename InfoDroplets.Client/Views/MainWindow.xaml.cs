using GMap.NET;
using GMap.NET.MapProviders;
using InfoDroplets.Client;
using InfoDroplets.Client.ViewModels;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static GMap.NET.Entity.OpenStreetMapRouteEntity;

namespace InfoDropletsClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            myMap.CacheLocation = Environment.CurrentDirectory + "\\GMapCache2\\";
            myMap.MapProvider = GMapProviders.OpenStreetMap;
            myMap.MinZoom = 1;
            myMap.MaxZoom = 20;
            myMap.Zoom = 16;
            myMap.ShowCenter = true;
            myMap.CanDragMap = false;
            myMap.MouseWheelZoomType = MouseWheelZoomType.ViewCenter;
            myMap.FillEmptyTiles = true;
            myMap.Position = new PointLatLng(0, 0);

            var vm = (MainWindowViewModel)DataContext;

            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(vm.MapPos))
                {
                    Dispatcher.Invoke(() =>
                    {
                        myMap.Position = vm.MapPos;
                    });
                }
            };
        }
    }
}