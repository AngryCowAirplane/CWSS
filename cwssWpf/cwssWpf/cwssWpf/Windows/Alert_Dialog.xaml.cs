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
    /// Interaction logic for Alert_Dialog.xaml
    /// </summary>
    public partial class Alert_Dialog : Window
    {
        public Alert_Dialog(string alertTitle, string alertText)
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            LoadImage();
            this.Activate();
            this.Topmost = true;
            Title.Content = alertTitle;
            AlertText.Text = alertText;
        }

        private void LoadImage()
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            var path = System.IO.Path.Combine(Environment.CurrentDirectory, "Images\\Alert.png");
            b.UriSource = new Uri(path);
            b.EndInit();

            Image.Source = b;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
