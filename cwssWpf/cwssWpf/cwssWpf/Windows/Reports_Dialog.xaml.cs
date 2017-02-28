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
    /// Interaction logic for Reports_Dialog.xaml
    /// </summary>
    public partial class Reports_Dialog : Window
    {
        public Reports_Dialog()
        {
            InitializeComponent();
            MouseLeftButtonDown += Helpers.Window_MouseDown;
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void populateLabels()
        {

        }
    }
}
