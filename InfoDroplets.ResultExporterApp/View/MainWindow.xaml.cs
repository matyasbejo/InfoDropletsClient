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
    public partial class MainWindow : Window
    {
        string DefaultLogPath = "E:\\";
        string DefaultOutPath = "D:\\";
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
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
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
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
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
    }
}