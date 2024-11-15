using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InfoDroplets.Client
{
    /// <summary>
    /// Interaction logic for SerialWindow.xaml
    /// </summary>
    public partial class SerialWindow : Window
    {
        public string SelectedSerialPort { get; set; }
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
            SelectedSerialPort = cb_portOptions.SelectedItem.ToString();
            DialogResult = true;
        }
    }
}
