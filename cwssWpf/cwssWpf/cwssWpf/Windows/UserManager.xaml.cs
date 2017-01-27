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
    /// Interaction logic for UserManager.xaml
    /// </summary>
    public partial class UserManager : Window
    {
        public UserManager()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            dataGrid.AutoGenerateColumns = false;
            dataGrid.CanUserAddRows = false;
            dataGrid.ItemsSource = Db.dataBase.Users;

            dataGrid.SelectionMode = DataGridSelectionMode.Extended;
            dataGrid.PreviewMouseRightButtonDown += rightButtonDown;

            setupColumns();
        }

        private void setupColumns()
        {
            var textColumn = new DataGridTextColumn();
            textColumn.Header = "First Name";
            textColumn.Binding = new Binding("Info.FirstName");
            textColumn.IsReadOnly = true;
            dataGrid.Columns.Add(textColumn);

            textColumn = new DataGridTextColumn();
            textColumn.Header = "Last Name";
            textColumn.Binding = new Binding("Info.LastName");
            textColumn.IsReadOnly = true;
            dataGrid.Columns.Add(textColumn);

            textColumn = new DataGridTextColumn();
            textColumn.Header = "User Id";
            textColumn.Binding = new Binding("LoginId");
            textColumn.IsReadOnly = true;
            dataGrid.Columns.Add(textColumn);

            var hyperColumn = new DataGridHyperlinkColumn();
            hyperColumn.Header = "Email";
            hyperColumn.Binding = new Binding("Info.Email");
            hyperColumn.IsReadOnly = true;
            dataGrid.Columns.Add(hyperColumn);

            var cbColumn = new DataGridCheckBoxColumn();
            cbColumn.Header = "CanClimb";
            cbColumn.Binding = new Binding("CanClimb");
            cbColumn.IsReadOnly = true;
            dataGrid.Columns.Add(cbColumn);
        }

        private void cmCanClimb_Click(object sender, RoutedEventArgs e)
        {
            var items = dataGrid.SelectedItems;

            foreach (var item in dataGrid.SelectedItems)
            {
                var user = (User)item;
                user.SetClimbingPrivilege(!user.CanClimb);
            }

            saveAndRefresh();
        }

        private void cmSendMessage_Click(object sender, RoutedEventArgs e)
        {
        }

        private void cmEmailUsers_Click(object sender, RoutedEventArgs e)
        {
        }

        private void rightButtonDown(object sender, MouseEventArgs e)
        {
            var test = dataGrid.SelectedItems;
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void saveAndRefresh()
        {
            dataGrid.CommitEdit();
            dataGrid.CommitEdit();
            dataGrid.Items.Refresh();
            dataGrid.Items.Refresh();
        }
    }
}

//private void menuSave_Click(object sender, RoutedEventArgs e)
//{
//    dataGrid.CommitEdit();
//    dataGrid.CommitEdit();
//}

//private void menuCancel_Click(object sender, RoutedEventArgs e)
//{
//    dataGrid.CancelEdit();
//    dataGrid.CancelEdit();
//}

//private void menuSaveExit_Click(object sender, RoutedEventArgs e)
//{
//    dataGrid.CommitEdit();
//    dataGrid.CommitEdit();
//    this.Close();
//}

//private void menuCancelExit_Click(object sender, RoutedEventArgs e)
//{
//    dataGrid.CancelEdit();
//    dataGrid.CancelEdit();
//    this.Close();
//}
