using ResultExporterApp;
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

namespace InfoDroplets.ResultExporterApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void teszt_Click(object sender, RoutedEventArgs e)
        {
            string[] dummypaths =
            {
                @"D:\UNI\_Szakdolgozat\TestData\L8_V0a.txt",
                @"D:\UNI\_Szakdolgozat\TestData\L8_V0b.txt",
                @"D:\UNI\_Szakdolgozat\TestData\L8_V1a.txt",
                @"D:\UNI\_Szakdolgozat\TestData\L8_V1b.txt",
                @"D:\UNI\_Szakdolgozat\TestData\L8_V2a.txt",
                @"D:\UNI\_Szakdolgozat\TestData\L8_V2b.txt"
            };

            LogProcessor.Execute(dummypaths);
            MapGenerator.CreateMap(LogProcessor.DropletNumber, LogProcessor.ElevationRange, LogProcessor.CenterPos.Longitude, LogProcessor.CenterPos.Latitude, LogProcessor.GlobalLogCollection, LogProcessor.GlobalBreakCollection);
        }
    }
}