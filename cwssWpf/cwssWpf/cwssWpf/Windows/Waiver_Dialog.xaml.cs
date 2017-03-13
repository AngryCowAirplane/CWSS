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

namespace cwssWpf.Windows
{
    /// <summary>
    /// Interaction logic for Waiver.xaml
    /// </summary>
    public partial class Waiver_Dialog : Window
    {
        public Waiver_Dialog()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            var waiverPath = System.IO.Path.Combine(Environment.CurrentDirectory, "AppData", @"CW Acknowledgement of Risk and Sign-In Sheet.pdf");
            
            InitializeComponent();
            tbName.Width = (this.Width / 2) - 100;
            tbDate.Width = (this.Width / 2) - 100;

            MouseLeftButtonDown += Helpers.Window_MouseDown;

            webBrowser.Navigate("file:///" + waiverPath);
            this.Focus();
            this.Activate();
            this.Topmost = true;
        }

        private void btnSignForm_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void btnNoSignForm_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
