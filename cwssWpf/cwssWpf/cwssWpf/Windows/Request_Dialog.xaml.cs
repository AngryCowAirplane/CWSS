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
                var alert = new Alert_Dialog("Revoked.", "Revoke Request Successful, " + selectedUser.GetName() + " - " + tbReason.Text, AlertType.Success);
                MainWindow.WindowsOpen.Add(alert, new cwssWpf.TimerVal(4));
                alert.Show();
                this.Close();
            }
            else
            {
                request.SuspensionLength = (Suspension)cbTime.SelectedItem;
                request.EnforceRequest();
                var alert = new Alert_Dialog("Revoke Enforced.", "Revoke Request Enforced, Climbing Privileges Revoked for " + selectedUser.GetName(), AlertType.Success);
                MainWindow.WindowsOpen.Add(alert, new cwssWpf.TimerVal(4));
                alert.Show();
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
                var alert = new Alert_Dialog("Canceled", "Revoke Request Canceled.", AlertType.Notice);
                MainWindow.WindowsOpen.Add(alert, new cwssWpf.TimerVal(2));
                alert.Show();
                this.Close();
            }
        }
    }
}
