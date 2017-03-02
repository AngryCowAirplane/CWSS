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
        public Alert_Dialog(string alertTitle, string alertText, Vector? screenCoords = null)
        {
            if(screenCoords == null)
                this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            else
            {
                Left = screenCoords.Value.X;
                Top = screenCoords.Value.Y;
            }

            InitializeComponent();
            LoadImage();
            this.Activate();
            this.Topmost = true;
            Title.Content = alertTitle;
            AlertText.Text = alertText;
            MouseLeftButtonDown += Window_MouseDown;
            KeyUp += EnterPressed;
        }

        private void EnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            Close_Click(null, null);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
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
