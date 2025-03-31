using ResultExporterApp;
using System.IO;
using System.Windows;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using Window = System.Windows.Window;

namespace InfoDroplets.ResultExporterApp
{
    public partial class MainWindow : Window
    {
        string DefaultLogPath = "D:\\UNI\\_Szakdolgozat\\TestData";
        string DefaultOutPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public MainWindow()
        {
            InitializeComponent();
            ExportButton.IsEnabled = false;
            LogTextBox.Text = DefaultLogPath;
            OutTextBox.Text = DefaultOutPath;
            DataContext = this;
        }

        private void LogBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalLabelHelper.Instance.LabelText = "Selecting log folder...";

            DirectoryInfo logpathDirInfo = new DirectoryInfo(LogTextBox.Text);
            var BrowseDialog = new FolderBrowserDialog()
            {
                InitialDirectory = logpathDirInfo.Exists? logpathDirInfo.FullName : 
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                UseDescriptionForTitle = true,
                Description = "Select log folder"
            };

            var result = BrowseDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var LogPath = BrowseDialog.SelectedPath;
                LogTextBox.Text = LogPath;
                ExportButton.IsEnabled = true;
                GlobalLabelHelper.Instance.LabelText = "Log folder selected";
            }
            else
                GlobalLabelHelper.Instance.LabelText = "Log folder selection cancelled";
        }

        private void OutBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalLabelHelper.Instance.LabelText = "Selecting output folder...";

            DirectoryInfo outpathDirInfo = new DirectoryInfo(OutTextBox.Text);
            var BrowseDialog = new FolderBrowserDialog()
            {
                InitialDirectory = outpathDirInfo.Exists ? outpathDirInfo.FullName :
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                ShowNewFolderButton = true,
                UseDescriptionForTitle = true,
                Description = "Select output folder"
            };

            var result = BrowseDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var LogPath = BrowseDialog.SelectedPath;
                OutTextBox.Text = LogPath;
                GlobalLabelHelper.Instance.LabelText = "Output folder selected";
            }
            else
                GlobalLabelHelper.Instance.LabelText = "Output folder selection cancelled";
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalLabelHelper.Instance.LabelText = "Start execute....";

            var logpaths = Directory.GetFiles(LogTextBox.Text);
            var outputpath = OutTextBox.Text;

            LogProcessor processor = new LogProcessor(logpaths);
            MapGenerator generator = new MapGenerator();

            bool ExportSuceeded = false;
            try
            {
                GlobalLabelHelper.Instance.LabelText = "Start log file processing....";
                processor.Execute();

                Thread.Sleep(500);

                GlobalLabelHelper.Instance.LabelText = "Start file export....";
                generator.PrepareValues(processor.DropletNumber,processor.ElevationLimit,processor.CenterPos.Longitude,processor.CenterPos.Latitude, processor.LogCollection, processor.BreakCollection);
                ExportSuceeded = generator.Execute(outputpath);
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Fatal error:\n{ex.Message}{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            if( ExportSuceeded )
            {
                var result = System.Windows.MessageBox.Show("The log export was succesful. Do you want to open the file now?", "Success", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start("explorer.exe", generator.GetNewFilePath(outputpath));
                }
                ExportButton.IsEnabled = true;
            }
        }
    }
}