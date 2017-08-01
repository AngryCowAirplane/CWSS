using cwssWpf.Data;
using cwssWpf.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
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
    /// Interaction logic for AccountManager.xaml
    /// </summary>
    public partial class AccountManager_Dialog : Window
    {
        private User selectedUser;

        public AccountManager_Dialog()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            lvUsers.ItemsSource = Db.dataBase.Users;
            tbSearch.TextChanged += tbSearch_Changed;
            lvUsers.PreviewMouseRightButtonDown += rightButtonDown;

            if (MainWindow.CurrentUser.UserType == UserType.Admin)
                UserTypeMenu.IsEnabled = true;
            else
                UserTypeMenu.IsEnabled = false;

            MouseLeftButtonDown += Helpers.Window_MouseDown;
        }

        private void updateUser(UserType type)
        {
            if(selectedUser != null)
            {
                if(selectedUser.UserType < UserType.Admin || MainWindow.CurrentUser.LoginId == selectedUser.LoginId)
                {
                    selectedUser.UpdateUserType(type);
                    lvUsers.Items.Refresh();
                }
                else
                {
                    var alert = new Alert_Dialog("Failed", "Admins may only demote their own account.");
                    MainWindow.WindowsOpen.Add(alert, new TimerVal(4));
                    alert.Show();
                }
            }
        }

        private void tbSearch_Changed(object sencer, RoutedEventArgs e)
        {
            lvUsers.ItemsSource = Db.dataBase.Users.Where(data => 
                data.GetName().ToLower().Contains(tbSearch.Text.ToLower()) ||
                data.UserType.ToString().ToLower().Contains(tbSearch.Text.ToLower()) ||
                data.Info.Email.ToLower().Contains(tbSearch.Text.ToLower()) ||
                data.LoginId.ToString().Contains(tbSearch.Text)
                );

            if (lvUsers.Items.Count == 1)
                lvUsers.SelectedIndex = 0;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var message = "Delete " + selectedUser.GetName();
            var confirm = new Confirm_Dialog(this, message);
            confirm.ShowDialog();

            if (confirm.Confirmed)
            {
                if(selectedUser.UserType != UserType.Admin || selectedUser.LoginId == MainWindow.CurrentUser.LoginId)
                {
                    Db.dataBase.DeleteUser(selectedUser);
                }
                else
                {
                    var alert = new Alert_Dialog("Failed", "Admins may only delete their own account.");
                    MainWindow.WindowsOpen.Add(alert, new TimerVal(4));
                    alert.Show();
                }
            }

            lvUsers.ItemsSource = Db.dataBase.Users;
            lvUsers.Items.Refresh();
            lvUsers.SelectedItem = null;
            tbSearch_Changed(this, null);
        }

        private void addUser_Click(object sender, RoutedEventArgs e)
        {
            var user = new NewUser_Dialog(this);
            user.ShowDialog();
            var oldText = tbSearch.Text;
            tbSearch.Text = " update ";
            lvUsers.ItemsSource = Db.dataBase.Users;
            lvUsers.Items.Refresh();
            tbSearch.Text = oldText;
            tbSearch_Changed(this, null);
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void patron_Click(object sender, RoutedEventArgs e)
        {
            updateUser(UserType.Patron);
        }

        private void employee_Click(object sender, RoutedEventArgs e)
        {
            updateUser(UserType.Employee);
        }

        private void manager_Click(object sender, RoutedEventArgs e)
        {
            updateUser(UserType.Manager);
        }

        private void admin_Click(object sender, RoutedEventArgs e)
        {
            updateUser(UserType.Admin);
        }

        private void rightButtonDown(object sender, MouseEventArgs e)
        {
            if(lvUsers.Items.Count > 0 && lvUsers.SelectedItem != null)
                selectedUser = (User)lvUsers.SelectedItem;
        }

        private void toggleCanClimb_Click(object sender, RoutedEventArgs e)
        {
            if (selectedUser != null)
            {
                var request = Db.dataBase.Notes.CheckRequest(selectedUser);
                if(request != null)
                {
                    var confirm = new Request_Dialog(request);
                    confirm.ShowDialog();
                }
                else
                {
                    selectedUser.SetClimbingPrivilege(!selectedUser.CanClimb);
                }

                lvUsers.Items.Refresh();
            }
        }

        private void documents_Click(object sender, RoutedEventArgs e)
        {
            var stats = new ClimberStats_Dialog(selectedUser);
            stats.Show();
        }
    }

    public class MyUserManagerColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var user = (User)value;
            var requests = Db.dataBase.Notes.Requests.Where(u => u.Patron == user);
            var brush = Brushes.Black;

            if (requests.Count() > 0)
            {
                var request = requests.First();
                if (request.Enforced == false)
                    brush = Brushes.DarkOrange;
                else
                    brush = Brushes.DarkRed;
            }
            else
            {
                if (!user.CanClimb)
                    brush = Brushes.Red;
            }

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
