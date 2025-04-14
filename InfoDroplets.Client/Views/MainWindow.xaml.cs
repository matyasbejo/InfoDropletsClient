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
        double lat = 47.533835;
        double lng = 19.033115;

        public MainWindow()
        {
            InitializeComponent();
            
            myMap.CacheLocation = Environment.CurrentDirectory + "\\GMapCache2\\";
            myMap.MapProvider = GMapProviders.OpenStreetMap;
            myMap.MinZoom = 1;
            myMap.MaxZoom = 20;
            myMap.Zoom = 14;
            myMap.ShowCenter = true;
            myMap.DragButton = System.Windows.Input.MouseButton.Left;
            myMap.CanDragMap = false;
            myMap.MouseWheelZoomType = MouseWheelZoomType.ViewCenter;
            myMap.FillEmptyTiles = true;
            myMap.Position = new PointLatLng(lat, lng);
        }

        private void movemap_btn_Click(object sender, RoutedEventArgs e)
        {
            lat += 0.0015;
            lng += 0.0015;

            myMap.Position = new PointLatLng(lat, lng);
        }
    }
}