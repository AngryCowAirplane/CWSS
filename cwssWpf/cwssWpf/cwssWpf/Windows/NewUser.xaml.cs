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
    public partial class NewUser : Window
    {
        // TODO:
        // Rename all objects in the window editor and set appropriate label text

        public NewUser(MainWindow mainWindow)
        {
            InitializeComponent();
            this.Left = mainWindow.Left + 50;
            this.Top = mainWindow.Top + 50;
            FocusManager.SetFocusedElement(this, tbFirstName);
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            Db.AddUser(tbIdNumber.Text, UserType.Patron, tbPassword.Text, true, tbFirstName.Text + " " + tbLastName.Text, tbEmail.Text, "");
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
