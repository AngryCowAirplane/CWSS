using cwssWpf.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using System.Windows.Threading;
using WebEye.Controls.Wpf;

namespace cwssWpf.Windows
{
    /// <summary>
    /// Interaction logic for Camera.xaml
    /// </summary>
    public partial class Camera : Window
    {
        public WebCameraControl WebCam;
        public List<WebCameraId> Cameras;
        public User CurrentUser;
        public static DispatcherTimer Timer = new DispatcherTimer();

        public Camera(User user)
        {
            InitializeComponent();
            CurrentUser = user;

            createControls();

            Cameras = new List<WebCameraId>(WebCam.GetVideoCaptureDevices());
            MouseLeftButtonDown += Helpers.Window_MouseDown;
            Timer.Interval = TimeSpan.FromSeconds(Config.Data.General.SignatureWaitDelay);
            Timer.Tick += Start_Click;
            Timer.Start();
        }

        private void createControls()
        {
            WebCam = new WebCameraControl();
            WebCam.Height = double.NaN;
            WebCam.Width = double.NaN;
            WebCam.Margin = new Thickness(10, 10, 10, 10);
            Grid.SetColumn(WebCam, 0);
            Grid.SetRow(WebCam, 0);
            Grid.SetColumnSpan(WebCam, 2);
            MainGrid.Children.Add(WebCam);
        }

        private void Capture_Click(object sender, RoutedEventArgs e)
        {
            Bitmap image = WebCam.GetCurrentImage();
            var path = System.IO.Path.Combine(Environment.CurrentDirectory,"AppData", "Images", "sig.bmp");

            if(!Directory.Exists(System.IO.Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            }

            image.Save(path);
            image.Dispose();
            WebCam.StopCapture();
            this.Close();
        }

        private void Start_Click(object sender, EventArgs e)
        {
            if (WebCam.IsCapturing)
            {
                WebCam.StopCapture();
                this.Close();
            }
            else
                WebCam.StartCapture(Cameras[0]);

            Timer.Stop();
        }
    }
}
