using cwssWpf.Data;
using cwssWpf.DataBase;
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
    /// Interaction logic for Request_Dialog.xaml
    /// </summary>
    public partial class Request_Dialog : Window
    {
        User selectedUser;
        Request request;
        mode requestMode = mode.Request;

        public Request_Dialog(User user)
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            selectedUser = user;
            InitializeComponent();
            lblEmployee.Content = MainWindow.CurrentUser.GetName();
            tbUser.Text = user.GetName();
            cbTime.ItemsSource = (Enum.GetValues(typeof(Suspension)).Cast<Suspension>().ToList());
            cbTime.SelectedIndex = 0;
            MouseLeftButtonDown += Helpers.Window_MouseDown;
            requestMode = mode.Request;
        }

        public Request_Dialog(Request request)
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.request = request;
            selectedUser = request.Patron;
            InitializeComponent();
            lblEmployee.Content = request.Employee.GetName();
            tbUser.Text = request.Patron.GetName();
            cbTime.ItemsSource = (Enum.GetValues(typeof(Suspension)).Cast<Suspension>().ToList());
            cbTime.SelectedItem = request.SuspensionLength;
            tbReason.Text = request.Reason;
            tbReason.IsEnabled = false;
            tbUser.IsEnabled = false;
            btnSubmit.Content = "Enforce";
            btnCancel.Content = "Delete";
            requestMode = mode.Respond;
        }

        private enum mode
        {
            Request,
            Respond
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if(requestMode == mode.Request)
            {
                Db.dataBase.Notes.AddRequest(selectedUser, tbReason.Text, (Suspension)cbTime.SelectedItem);
                MessageBox.Show("Revoke Request Successfull, " + selectedUser.GetName() + " - " + tbReason.Text);
                this.Close();
            }
            else
            {
                request.EnforceRequest();
                MessageBox.Show("Revoke Request Enforced, Climbing Privileges Revoked for " + selectedUser.GetName());
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if(requestMode == mode.Request)
            {
                this.Close();
            }
            else
            {
                request.ReleaseRequest();
                MessageBox.Show("Revoke Request Canceled.");
                // need to send reason back to requester?
                this.Close();
            }
        }
    }
}
