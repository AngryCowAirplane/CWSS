using cwssWpf.Data;
using ImageManipTesting;
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
        public Waiver_Dialog(User user = null)
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            var waiverPath = System.IO.Path.Combine(Environment.CurrentDirectory, "AppData", @"CW Acknowledgement of Risk and Sign-In Sheet.pdf");
            
            InitializeComponent();
            tbName.Width = (this.Width / 2) - 100;
            tbDate.Width = (this.Width / 2) - 100;

            if (user != null) 
                tbName.Text = user.GetName();
            tbDate.Text = DateTime.Now.ToShortDateString();

            MouseLeftButtonDown += Helpers.Window_MouseDown;

            webBrowser.Navigate("file:///" + waiverPath);
            this.Focus();
            this.Activate();
            this.Topmost = true;

            if (!Config.Data.General.GetSignature)
                btnSignForm.Visibility = Visibility.Hidden;
        }

        private void btnSignForm_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            var alert = new Alert_Dialog("Waiting For Signature", "Please upload signature using the mobile device and photoshare.", AlertType.Notice);
            alert.Close.IsEnabled = false;
            alert.Show();
            string[] args = new string[] { tbName.Text };
            ImageProgram.Main(args);
            alert.Close();
            this.Close();
            btnNoSignForm.IsEnabled = true;
        }

        private void btnNoSignForm_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cbAgree_Checked(object sender, RoutedEventArgs e)
        {
            DateTime date = new DateTime();
            var success = DateTime.TryParse(tbDate.Text, out date);

            if (tbName.Text.Length > 1 && success && (bool)cbAgree.IsChecked)
            {
                btnSignForm.IsEnabled = true;
                if(!Config.Data.General.GetSignature)
                    btnNoSignForm.IsEnabled = true;
            }
            else
            {
                btnSignForm.IsEnabled = false;
                btnNoSignForm.IsEnabled = false;
                cbAgree.IsChecked = false;
                var alert = new Alert_Dialog("Accept Failed", "Missing Name or Incorrect Date Format", AlertType.Failure, autoClose:true);
                alert.ShowDialog();
            }
        }

        private void cbAgree_UnChecked(object sender, RoutedEventArgs e)
        {
            btnSignForm.IsEnabled = false;
            btnNoSignForm.IsEnabled = false;
            cbAgree.IsChecked = false;
        }
    }
}
