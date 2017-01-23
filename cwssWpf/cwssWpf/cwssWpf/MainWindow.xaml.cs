using cwssWpf.Data;
using cwssWpf.DataBase;
using cwssWpf.Migrations;
using cwssWpf.Windows;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
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
    /// 

    // mySQL infos
    // add name="DefaultConnection" providerName="MySql.Data.MySqlClient" connectionString="Server=localhost;Database=cwss;Uid=root;Pwd=9087intxJON" />

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
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<Context, Configuration>());

            // Other Loading/Initializing done here between Status texts
            StatusText.Text = "Loading...";

            //DataBase.Load();
            //Logger.Initialize();

            Db.dataBase = new Context();
            Db.dataBase.Database.Log = delegate (string message) { Console.Write(message); };

            if (Db.dataBase.Users.Count() == 0)
                Db.AddUser(DefaultAdminId, UserType.Admin, DefaultAdminPassword, true, "Admin", "admin@admin.com","");

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
                var loginId = int.Parse(tbLoginId.Text);

                var findUser = Db.GetUser(loginId);

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
