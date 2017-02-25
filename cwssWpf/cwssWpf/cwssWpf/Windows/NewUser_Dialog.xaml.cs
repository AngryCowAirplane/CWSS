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
    /// Interaction logic for NewUser.xaml
    /// </summary>
    public partial class NewUser_Dialog : Window
    {
        // TODO:
        // Rename all objects in the window editor and set appropriate label text

        public NewUser_Dialog(Window mainWindow)
        {
            InitializeComponent();
            this.Left = mainWindow.Left + 50;
            this.Top = mainWindow.Top + 50;
            cbGender.ItemsSource = (Enum.GetValues(typeof(GenderType)).Cast<GenderType>().ToList());
            FocusManager.SetFocusedElement(this, tbFirstName);
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            var success = Db.dataBase.AddUser(
                tbFirstName, tbLastName,
                tbIdNumber, tbPassword, tbPassword2,
                tbEmail, tbAddress, tbCity, tbState, tbZip,
                tbPhone, cbGender
                );

            if (success)
            {
                if (MainWindow.CurrentUser == null)
                    MainWindow.CurrentUser = new Data.User();

                Logger.Log(MainWindow.CurrentUser.UserId, LogType.AddUser,
                    MainWindow.CurrentUser.GetName() + " Added User: " +
                    tbFirstName.Text + " " + tbLastName.Text);
            }
            else
            {
                if (MainWindow.CurrentUser == null)
                    MainWindow.CurrentUser = new Data.User();

                Logger.Log(MainWindow.CurrentUser.UserId, LogType.Error,
                    MainWindow.CurrentUser.GetName() + " Failed Add User: " +
                    tbFirstName.Text + " " + tbLastName.Text);

                // TODO:
                // Give feedback on why failed
                MessageBox.Show("Add User Failed");
            }

            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // TODO:
        // Event for cancel button click
        // -- window.close()
        // Event for submit button click
        // -- call below function
        // Function for checking values and calling DB.AddUser with correct infos
    }
}
