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
        MainWindowViewModel vm;
        public MainWindow()
        {
            InitializeComponent();

            myMap.CacheLocation = Environment.CurrentDirectory + "\\GMapCache2\\";
            myMap.MapProvider = GMapProviders.OpenStreetMap;
            myMap.MinZoom = 1;
            myMap.MaxZoom = 20;
            myMap.Zoom = 13;
            myMap.ShowCenter = false;
            myMap.CanDragMap = false;
            myMap.MouseWheelZoomType = MouseWheelZoomType.ViewCenter;
            myMap.FillEmptyTiles = true;
            myMap.Position = new PointLatLng(46.180327, 19.011035);

            vm = (MainWindowViewModel)DataContext;

            vm.PropertyChanged += RefreshMapOnUI;
        }

        void RefreshMapOnUI(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "MapPos":
                    Dispatcher.Invoke(() =>
                    {
                        myMap.Position = vm.MapPos;
                    });
                    break;
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
    }
}