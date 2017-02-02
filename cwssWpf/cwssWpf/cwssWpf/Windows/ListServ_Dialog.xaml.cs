using cwssWpf.Data;
using cwssWpf.DataBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class ListServ_Dialog : Window
    {
        public ListServ_Dialog()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            dataGrid.AutoGenerateColumns = false;
            dataGrid.CanUserAddRows = false;
            dataGrid.ItemsSource = Db.dataBase.Users;

            dataGrid.SelectionMode = DataGridSelectionMode.Extended;

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

        private void cmSendMessage_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItems.Count > 0)
            {

                var messageList = new List<User>();

                foreach (var item in dataGrid.SelectedItems)
                {
                    var user = (User)item;
                    messageList.Add(user);
                }

                var message = new Message_Dialog(messageList);

                message.ShowDialog();
            }
        }

        private void cmEmailUsers_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItems.Count > 0)
            {

                var emailList = new List<string>();
                Uri uri;

                if (dataGrid.SelectedItems.Count > 1)
                {
                    foreach (var item in dataGrid.SelectedItems)
                    {
                        var user = (User)item;
                        emailList.Add(user.GetEmailAddress());
                    }
                    uri = Helpers.GenerateEmailUriFromList(emailList);
                }
                else
                {
                    emailList.Add(((User)dataGrid.SelectedItem).GetEmailAddress());
                    uri = ((User)dataGrid.SelectedItem).GetEmailUri();
                }


                if (Config.Data.Email.Client == EmailClient.LocalClient)
                {
                    try
                    {
                        Process.Start(uri.AbsoluteUri);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Email Client Not Setup On Computer");
                    }
                }
                else
                {
                    try
                    {
                        var email = new Email_Dialog(emailList);
                        email.ShowDialog();
                        MessageBox.Show("Email Sent!");
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show("Email Send Failed");
                    }
                }
            }
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

        private void menuPrint_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuSave_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
