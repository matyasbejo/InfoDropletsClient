using InfoDroplets.Client.ViewModels;
using System.Windows;

namespace InfoDroplets.Client
{
    /// <summary>
    /// Interaction logic for SerialWindow.xaml
    /// </summary>
    public partial class SerialWindow : Window
    {
        public SerialWindow()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void bt_submit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
