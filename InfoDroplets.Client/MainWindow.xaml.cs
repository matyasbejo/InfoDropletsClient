﻿using InfoDroplets.Client;
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
            SerialWindow serialSetupWindow = new SerialWindow();
            if(serialSetupWindow.ShowDialog() == true)
                lb_selectedPort.Content = serialSetupWindow.SelectedSerialPort;
        }
    }
}