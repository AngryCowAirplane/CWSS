using System;
using System.Collections.Generic;
using System.Drawing;
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
using WebEye.Controls.Wpf;

namespace cwssWpf.Windows
{
    /// <summary>
    /// Interaction logic for Camera.xaml
    /// </summary>
    public partial class Camera : Window
    {
        public WebCameraControl webCameraControl1;
        public List<WebCameraId> cameras;

        public Camera()
        {
            InitializeComponent();

            webCameraControl1 = new WebCameraControl();
            webCameraControl1.Height = 200;
            webCameraControl1.Width = 300;
            webCameraControl1.Margin = new Thickness(10, 10, 10, 10);
            Grid.SetColumn(webCameraControl1, 0);
            Grid.SetRow(webCameraControl1, 0);
            MainGrid.Children.Add(webCameraControl1);

            cameras = new List<WebCameraId>(webCameraControl1.GetVideoCaptureDevices());
        }

        private void Capture_Click(object sender, RoutedEventArgs e)
        {
            

            Bitmap image = webCameraControl1.GetCurrentImage();
            var path = System.IO.Path.Combine(Environment.CurrentDirectory,"AppData", "Images", @"testImage.bmp");

            image.Save(path);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (webCameraControl1.IsCapturing)
                webCameraControl1.StopCapture();
            else
                webCameraControl1.StartCapture(cameras[0]);
        }
    }
}
