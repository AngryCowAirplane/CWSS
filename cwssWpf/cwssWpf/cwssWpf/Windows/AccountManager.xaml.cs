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
            InitializeComponent();
            lvUsers.ItemsSource = Db.dataBase.Users;
            lbTypes.ItemsSource = (Enum.GetValues(typeof(UserType)).Cast<UserType>().ToList());
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var user = (User)lvUsers.SelectedItem;
            var type = (UserType)lbTypes.SelectedItem;

            user.UserType = type;
            lvUsers.Items.Refresh();
        }
    }
}
