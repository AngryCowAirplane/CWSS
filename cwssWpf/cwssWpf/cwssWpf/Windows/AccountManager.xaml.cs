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
    /// Interaction logic for AccountManager.xaml
    /// </summary>
    public partial class AccountManager : Window
    {
        private User selectedUser;

        public AccountManager()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            lvUsers.ItemsSource = Db.dataBase.Users;
            tbSearch.TextChanged += tbSearch_Changed;
            lvUsers.PreviewMouseRightButtonDown += rightButtonDown;
        }

        private void updateUser(UserType type)
        {
            if(selectedUser != null)
            {
                selectedUser.UpdateUserType(type);
                lvUsers.Items.Refresh();
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
            var confirm = new Confirm(this, message);
            confirm.ShowDialog();

            if (confirm.Confirmed)
                Db.DeleteUser(selectedUser);

            lvUsers.ItemsSource = Db.dataBase.Users;
            lvUsers.Items.Refresh();
            lvUsers.SelectedItem = null;
            tbSearch_Changed(this, null);
        }

        private void addUser_Click(object sender, RoutedEventArgs e)
        {
            var user = new NewUser(this);
            user.Show();
            lvUsers.ItemsSource = null;
            lvUsers.ItemsSource = Db.dataBase.Users;
            lvUsers.Items.Refresh();
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
    }
}
