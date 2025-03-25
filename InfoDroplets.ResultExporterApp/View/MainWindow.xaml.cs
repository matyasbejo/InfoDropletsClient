using ResultExporterApp;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Application = System.Windows.Application;
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
        }

        private void LogBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var BrowseDialog = new FolderBrowserDialog()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                UseDescriptionForTitle = true,
                Description = "Select log folder"
            };

            var result = BrowseDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var LogPath = BrowseDialog.SelectedPath;
                LogTextBox.Text = LogPath;
                ExportButton.IsEnabled = true;
            }
        }

        private void OutBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var BrowseDialog = new FolderBrowserDialog()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                ShowNewFolderButton = true,
                UseDescriptionForTitle = true,
                Description = "Select export folder"
            };

            var result = BrowseDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var LogPath = BrowseDialog.SelectedPath;
                OutTextBox.Text = LogPath;
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            ExportButton.IsEnabled = false;
            ForceUIToUpdate();

            var logpaths = Directory.GetFiles(LogTextBox.Text);
            var outputpath = OutTextBox.Text;
            LogProcessor processor = new LogProcessor(logpaths);
            MapGenerator generator = new MapGenerator(processor);

            processor.Execute();
            bool ExportSuceeded = generator.Execute(outputpath);

            if (!ExportSuceeded) 
                System.Windows.MessageBox.Show("The log export failed.", "Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                var result = System.Windows.MessageBox.Show("The log export was succesful. Do you want to open the file now?", "Success", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start("explorer.exe", generator.GetNewFilePath(outputpath));
                }
                ExportButton.IsEnabled = true;
            }
        }

        void ForceUIToUpdate()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate (object parameter)
            {
                frame.Continue = false;
                return null;
            }), null);

            Dispatcher.PushFrame(frame);
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                          new Action(delegate { }));
        }
    }
}