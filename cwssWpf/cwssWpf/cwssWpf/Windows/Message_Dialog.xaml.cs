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
    /// Interaction logic for Message_Dialog.xaml
    /// </summary>
    public partial class Message_Dialog : Window
    {
        public List<User> Users;
        public Message_Dialog(List<User> users)
        {
            Users = users;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            tbTo.IsEnabled = false;
            tbFrom.IsEnabled = false;
            tbFrom.Text = MainWindow.CurrentUser.GetName();
            cbMode.ItemsSource = (Enum.GetValues(typeof(MessageMode)).Cast<MessageMode>().ToList());
            cbMode.SelectionChanged += modeChange;
            setMessageMode();
        }

        private void setMessageMode()
        {
            if (Users.Count > 1)
            {
                cbMode.SelectedItem = MessageMode.Multi;
                tbTo.Text = listToString();
            }
            else
            {
                cbMode.SelectedItem = MessageMode.Single;
                if(Users.Count == 1)
                    tbTo.Text = Users.First().GetName();
            }
        }

        private void modeChange(object sender, RoutedEventArgs e)
        {
            switch((MessageMode)cbMode.SelectedItem)
            {
                case MessageMode.Employees:
                    tbTo.IsEnabled = false;
                    tbTo.Text = "<All Employees>";
                    break;
                case MessageMode.Managers:
                    tbTo.IsEnabled = false;
                    tbTo.Text = "<All Managers>";
                    break;
                case MessageMode.Everyone:
                    tbTo.IsEnabled = false;
                    tbTo.Text = "<Everyone>";
                    break;
                case MessageMode.Single:
                    tbTo.IsEnabled = true;
                    if (Users.Count > 0)
                        tbTo.Text = Users.First().GetName();
                    break;
                default:
                    cbMode.SelectedItem = MessageMode.Multi;
                    tbTo.IsEnabled = false;
                    tbTo.Text = listToString();
                    break;
            }
        }

        private string listToString()
        {
            var toString = string.Empty;
            if(Users.Count > 0)
            {
                foreach (var user in Users)
                {
                    toString += user.GetName() + ", ";
                }
            }
            toString = toString.Remove(toString.Length - 2, 2);
            return toString;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            var message = new Message();
            message.SetSender(MainWindow.CurrentUser);
            message.SetRecipients(Users);
            message.Subject = tbSubject.Text;
            message.Contents = tbBody.Text;
            message.ExpireDate = DateTime.Now + TimeSpan.FromDays(45);
            message.TimeStamp = DateTime.Now;
            Db.dataBase.AddMessage(message);
            MessageBox.Show("Message Sent!");
            this.Close();
        }
    }
}
