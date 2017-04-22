using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
        public AlertType Alert_Type;

        public Alert_Dialog(string alertTitle, string alertText = "", AlertType alertType = AlertType.Failure, Vector? screenCoords = null, bool autoClose = false, bool returnResult = false)
        {
            Alert_Type = alertType;
            if(screenCoords == null)
                this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            else
            {
                Left = screenCoords.Value.X;
                Top = screenCoords.Value.Y;
            }

            if (autoClose)
            {
                MainWindow.WindowsOpen.Add(this, new TimerVal(3));
            }

            InitializeComponent();
            LoadImage();
            PlaySound();
            this.Activate();
            this.Topmost = true;
            Title.Content = alertTitle;
            AlertText.Text = alertText;
            MouseLeftButtonDown += Window_MouseDown;
            PreviewKeyDown += Helpers.HandleEsc;
            KeyDown += EnterPressed;

            if (returnResult)
            {
                Answer.IsEnabled = true;
                Answer.Visibility = Visibility.Visible;
                Close.Content = "No";
            }
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
            if (Alert_Type == AlertType.Success)
                path = System.IO.Path.Combine(Environment.CurrentDirectory, "Images\\Success.png");

            b.UriSource = new Uri(path);
            b.EndInit();

            Image.Source = b;
        }

        private void PlaySound()
        {
            if(Alert_Type == AlertType.Failure)
            {
                Helpers.PlayFail();
            }
            else if (Alert_Type == AlertType.Success)
            {
                Helpers.PlaySuccess();
            }
            else
            {
                Helpers.PlayFail();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Answer_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
    }

    public enum AlertType
    {
        Success = 0,
        Failure = 1,
        Notice = 2
    }
}
