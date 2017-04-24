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
using System.Globalization;
using System.IO;

namespace cwssWpf.Windows
{
    /// <summary>
    /// Interaction logic for UserManager.xaml
    /// </summary>
    public partial class ListServ_Dialog : Window
    {
        public List<DataGridColumn> Columns = new List<DataGridColumn>();
        public ListServ_Dialog()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            dataGrid.AutoGenerateColumns = false;
            dataGrid.CanUserAddRows = false;
            dataGrid.ItemsSource = Db.dataBase.Users;

            dataGrid.SelectionMode = DataGridSelectionMode.Extended;
            MouseLeftButtonDown += Helpers.Window_MouseDown;
            PreviewKeyDown += Helpers.HandleEsc;

            setupColumns();
        }

        private void setupColumns()
        {
            var textColumn = new DataGridTextColumn();
            textColumn.Header = "Account";
            textColumn.Binding = new Binding("UserType");
            textColumn.IsReadOnly = true;
            dataGrid.Columns.Add(textColumn);
            Columns.Add(textColumn);

            textColumn = new DataGridTextColumn();
            textColumn.Header = "First Name";
            textColumn.Binding = new Binding("Info.FirstName");
            textColumn.IsReadOnly = true;
            dataGrid.Columns.Add(textColumn);
            Columns.Add(textColumn);

            textColumn = new DataGridTextColumn();
            textColumn.Header = "Last Name";
            textColumn.Binding = new Binding("Info.LastName");
            textColumn.IsReadOnly = true;
            dataGrid.Columns.Add(textColumn);
            Columns.Add(textColumn);

            textColumn = new DataGridTextColumn();
            textColumn.Header = "User Id";
            textColumn.Binding = new Binding("LoginId");
            textColumn.IsReadOnly = true;
            dataGrid.Columns.Add(textColumn);
            Columns.Add(textColumn);

            var hyperColumn = new DataGridHyperlinkColumn();
            hyperColumn.Header = "Email";
            hyperColumn.Binding = new Binding("Info.Email");
            hyperColumn.IsReadOnly = true;
            dataGrid.Columns.Add(hyperColumn);
            Columns.Add(textColumn);

            textColumn = new DataGridTextColumn();
            textColumn.Header = "Address";
            textColumn.Binding = new Binding("Info.Address");
            textColumn.IsReadOnly = true;
            dataGrid.Columns.Add(textColumn);
            Columns.Add(textColumn);

            textColumn = new DataGridTextColumn();
            textColumn.Header = "City";
            textColumn.Binding = new Binding("Info.City");
            textColumn.IsReadOnly = true;
            dataGrid.Columns.Add(textColumn);
            Columns.Add(textColumn);

            textColumn = new DataGridTextColumn();
            textColumn.Header = "State";
            textColumn.Binding = new Binding("Info.State");
            textColumn.IsReadOnly = true;
            dataGrid.Columns.Add(textColumn);
            Columns.Add(textColumn);

            textColumn = new DataGridTextColumn();
            textColumn.Header = "Zip";
            textColumn.Binding = new Binding("Info.Zip");
            textColumn.IsReadOnly = true;
            dataGrid.Columns.Add(textColumn);
            Columns.Add(textColumn);

            textColumn = new DataGridTextColumn();
            textColumn.Header = "Phone";
            textColumn.Binding = new Binding("Info.Phone");
            textColumn.IsReadOnly = true;
            dataGrid.Columns.Add(textColumn);
            Columns.Add(textColumn);

            textColumn = new DataGridTextColumn();
            textColumn.Header = "Gender";
            textColumn.Binding = new Binding("Info.Gender");
            textColumn.IsReadOnly = true;
            dataGrid.Columns.Add(textColumn);
            Columns.Add(textColumn);

            textColumn = new DataGridTextColumn();
            textColumn.Header = "DateOfBirth";
            textColumn.Binding = new Binding("Info.DateOfBirth");
            textColumn.IsReadOnly = true;
            dataGrid.Columns.Add(textColumn);
            Columns.Add(textColumn);

            textColumn = new DataGridTextColumn();
            textColumn.Header = "Created";
            textColumn.Binding = new Binding("DateCreated");
            textColumn.IsReadOnly = true;
            dataGrid.Columns.Add(textColumn);
            Columns.Add(textColumn);

            var checkColumn = new DataGridCheckBoxColumn();
            checkColumn.Header = "IsLead";
            checkColumn.Binding = new Binding("IsLead");
            textColumn.IsReadOnly = true;
            dataGrid.Columns.Add(checkColumn);
            Columns.Add(checkColumn);

            checkColumn = new DataGridCheckBoxColumn();
            checkColumn.Header = "IsBelay";
            checkColumn.Binding = new Binding("IsBelay");
            textColumn.IsReadOnly = true;
            dataGrid.Columns.Add(checkColumn);
            Columns.Add(checkColumn);
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
            dataGrid.SelectAllCells();

            dataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, dataGrid);

            dataGrid.UnselectAllCells();

            SaveClipboardToCSVFile();
        }

        private void State_Click(object sender, RoutedEventArgs e)
        {
            var col = Columns.Where(c => c.Header.ToString() == "State").First();
            if (col.Visibility == Visibility.Visible)
                col.Visibility = Visibility.Hidden;
            else
                col.Visibility = Visibility.Visible;
        }

        private void DOB_Click(object sender, RoutedEventArgs e)
        {
            var col = Columns.Where(c => c.Header.ToString() == "DateOfBirth").First();
            if (col.Visibility == Visibility.Visible)
                col.Visibility = Visibility.Hidden;
            else
                col.Visibility = Visibility.Visible;
        }

        private void Gender_Click(object sender, RoutedEventArgs e)
        {
            var col = Columns.Where(c => c.Header.ToString() == "Gender").First();
            if (col.Visibility == Visibility.Visible)
                col.Visibility = Visibility.Hidden;
            else
                col.Visibility = Visibility.Visible;
        }

        private void Phone_Click(object sender, RoutedEventArgs e)
        {
            var col = Columns.Where(c => c.Header.ToString() == "Phone").First();
            if (col.Visibility == Visibility.Visible)
                col.Visibility = Visibility.Hidden;
            else
                col.Visibility = Visibility.Visible;
        }

        private void Zip_Click(object sender, RoutedEventArgs e)
        {
            var col = Columns.Where(c => c.Header.ToString() == "Zip").First();
            if (col.Visibility == Visibility.Visible)
                col.Visibility = Visibility.Hidden;
            else
                col.Visibility = Visibility.Visible;
        }

        private void FirstName_Click(object sender, RoutedEventArgs e)
        {
            var col = Columns.Where(c => c.Header.ToString() == "First Name").First();
            if (col.Visibility == Visibility.Visible)
                col.Visibility = Visibility.Hidden;
            else
                col.Visibility = Visibility.Visible;
        }

        private void LastName_Click(object sender, RoutedEventArgs e)
        {
            var col = Columns.Where(c => c.Header.ToString() == "Last Name").First();
            if (col.Visibility == Visibility.Visible)
                col.Visibility = Visibility.Hidden;
            else
                col.Visibility = Visibility.Visible;
        }

        private void UserId_Click(object sender, RoutedEventArgs e)
        {
            var col = Columns.Where(c => c.Header.ToString() == "User Id").First();
            if (col.Visibility == Visibility.Visible)
                col.Visibility = Visibility.Hidden;
            else
                col.Visibility = Visibility.Visible;
        }

        private void Email_Click(object sender, RoutedEventArgs e)
        {
            var col = Columns.Where(c => c.Header.ToString() == "Email").First();
            if (col.Visibility == Visibility.Visible)
                col.Visibility = Visibility.Hidden;
            else
                col.Visibility = Visibility.Visible;
        }

        private void Address_Click(object sender, RoutedEventArgs e)
        {
            var col = Columns.Where(c => c.Header.ToString() == "Address").First();
            if (col.Visibility == Visibility.Visible)
                col.Visibility = Visibility.Hidden;
            else
                col.Visibility = Visibility.Visible;
        }

        private void City_Click(object sender, RoutedEventArgs e)
        {
            var col = Columns.Where(c => c.Header.ToString() == "City").First();
            if (col.Visibility == Visibility.Visible)
                col.Visibility = Visibility.Hidden;
            else
                col.Visibility = Visibility.Visible;
        }

        private void Type_Click(object sender, RoutedEventArgs e)
        {
            var col = Columns.Where(c => c.Header.ToString() == "Type").First();
            if (col.Visibility == Visibility.Visible)
                col.Visibility = Visibility.Hidden;
            else
                col.Visibility = Visibility.Visible;
        }

        private void Created_Click(object sender, RoutedEventArgs e)
        {
            var col = Columns.Where(c => c.Header.ToString() == "Created").First();
            if (col.Visibility == Visibility.Visible)
                col.Visibility = Visibility.Hidden;
            else
                col.Visibility = Visibility.Visible;
        }

        private void IsBelay_Click(object sender, RoutedEventArgs e)
        {
            var col = Columns.Where(c => c.Header.ToString() == "IsBelay").First();
            if (col.Visibility == Visibility.Visible)
                col.Visibility = Visibility.Hidden;
            else
                col.Visibility = Visibility.Visible;
        }

        private void IsLead_Click(object sender, RoutedEventArgs e)
        {
            var col = Columns.Where(c => c.Header.ToString() == "IsLead").First();
            if (col.Visibility == Visibility.Visible)
                col.Visibility = Visibility.Hidden;
            else
                col.Visibility = Visibility.Visible;
        }

        private void menuSaveSelected_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, dataGrid);

            SaveClipboardToCSVFile();
        }


        public static void SaveClipboardToCSVFile()
        {
            string result = (string)System.Windows.Clipboard.GetData(System.Windows.DataFormats.CommaSeparatedValue);
            result.Replace(',', ';');

            Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog();

            saveDialog.FileName = "list.csv";
            saveDialog.DefaultExt = ".csv";
            saveDialog.Filter = "Comma Seperated Values (.csv)|*.csv";

            var success = (bool)saveDialog.ShowDialog();

            if (success)
            {
                File.WriteAllText(saveDialog.FileName, result, UnicodeEncoding.UTF8);
            }
        }
    }
}