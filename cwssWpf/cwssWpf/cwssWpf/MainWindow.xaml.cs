using cwssWpf.Data;
using cwssWpf.Windows;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cwssWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // TODO:
        // find appropriate place and functionality for default admin user/password if DB is lost
        // need the ability for the admin to change this immediately so this no longer is accepted from
        // regular DB querying.
        public int DefaultAdminId = 12345;
        public string DefaultAdminPassword = "abc123";

        public MainWindow()
        {
            InitializeComponent();

            // Other Loading/Initializing done here between Status texts
            StatusText.Text = "Loading...";
            DataBase.Load();
            Logger.Initialize();
            StatusText.Text = "Ready";
        }

        private void menuNewUser_Click(object sender, RoutedEventArgs e)
        {
            var newUserForm = new NewUser();
            newUserForm.Show();
            // TODO:
            // Show Appropriate message in status bar
        }

        private void menuEmployeeLogIn_Click(object sender, RoutedEventArgs e)
        {
            // TODO:
            // Create Employee Login Window
            // Show Window()
            // Do All Logic Checking in the Employee Login Window
            // Nothing Else goes in this function
        }

        private void btnCheckIn_Click(object sender, RoutedEventArgs e)
        {
            // TODO:
            // validate input in tbUserId
            // if valid checkIn()
            checkIn();
        }

        // TODO:
        // Add event like above to call checkIn() when the
        // "Enter" Key is pressed from within the userId textBox

        private void checkIn()
        {
            var success = tryCheckinUser();
            // TODO:
            // LOG APPROPRIATE MESSAGE (both for success and failure)
            // Show Appropriate message in status bar

            // TODO:
            // Make Function to Check to see if any messages for the user
            // And display to user any messages
        }

        private bool tryCheckinUser()
        {
            try
            {
                var userId = int.Parse(tbUserId.Text);

                var findUser = DataBase.Data.Users.Where(user => user.UserId == userId).First();

                if(findUser != null)
                    MessageBox.Show("User Found: " + findUser.UserName);
            }
            catch
            {
                MessageBox.Show("User Not Found!");
                return false;
            }

            return true;
        }
    }
}
