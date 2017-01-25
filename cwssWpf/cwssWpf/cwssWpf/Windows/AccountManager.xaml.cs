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
        public AccountManager()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            lvUsers.ItemsSource = Db.dataBase.Users;
            lbTypes.ItemsSource = (Enum.GetValues(typeof(UserType)).Cast<UserType>().ToList());
            tbSearch.TextChanged += tbSearch_Changed;
            lvUsers.SelectionChanged += lbSelection_Changed;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var user = (User)lvUsers.SelectedItem;
            var type = (UserType)lbTypes.SelectedItem;

            user.UserType = type;
            lvUsers.Items.Refresh();
        }

        private void lbSelection_Changed(object sender, RoutedEventArgs e)
        {
            if(lvUsers.SelectedItem != null)
                lbTypes.SelectedItem = ((User)lvUsers.SelectedItem).UserType;
            else
            {
                lbTypes.SelectedItem = null;
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
            var confirm = new Confirm(this);
            confirm.ShowDialog();

            if (confirm.Confirmed)
                Db.dataBase.Users.Remove((User)lvUsers.SelectedItem);

            lvUsers.ItemsSource = Db.dataBase.Users;
            lvUsers.Items.Refresh();
            lvUsers.SelectedItem = null;
            tbSearch_Changed(this, null);
        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            lvUsers.Items.Refresh();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var user = new NewUser(this);
            user.Show();
            lvUsers.ItemsSource = null;
            lvUsers.ItemsSource = Db.dataBase.Users;
            lvUsers.Items.Refresh();
            tbSearch_Changed(this, null);
        }
    }
}
