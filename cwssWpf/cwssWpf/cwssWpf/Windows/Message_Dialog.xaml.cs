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
        private List<User> users = new List<User>();
        private Message message;
        private bool Write = true;

        // Write Message
        public Message_Dialog(List<User> users)
        {
            this.users = users;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            tbTo.IsEnabled = false;
            tbFrom.IsEnabled = false;
            tbFrom.Text = MainWindow.CurrentUser.GetName();
            cbMode.ItemsSource = (Enum.GetValues(typeof(MessageMode)).Cast<MessageMode>().ToList());
            cbMode.SelectionChanged += modeChange;
            setMessageMode();
        }

        // Read Message
        public Message_Dialog(User user, Message message)
        {
            users.Add(user);
            this.message = message;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            tbTo.IsEnabled = false;
            tbFrom.IsEnabled = false;
            cbMode.IsEnabled = false;
            tbTo.Text = user.GetName();
            tbFrom.Text = Db.dataBase.GetUser(message.SenderId).GetName();
            tbSubject.Text = message.Subject;
            tbBody.Text = message.Contents;
            btnSend.Content = "Mark as Read";
            Write = false;
        }


        private void setMessageMode()
        {
            if (users.Count > 1)
            {
                cbMode.SelectedItem = MessageMode.Multi;
                tbTo.Text = listToString();
            }
            else
            {
                cbMode.SelectedItem = MessageMode.Single;
                if(users.Count == 1)
                    tbTo.Text = users.First().GetName();
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
                // future maybe make to editable on a single message and autocomplete based on users
                case MessageMode.Single:
                    tbTo.IsEnabled = false;
                    if (users.Count > 0)
                        tbTo.Text = users.First().GetName();
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
            if(users.Count > 0)
            {
                foreach (var user in users)
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
            if(Write)
            {
                var message = new Message();
                message.SetSender(MainWindow.CurrentUser);
                message.SetRecipients(users);
                message.Subject = tbSubject.Text;
                message.Contents = tbBody.Text;
                message.ExpireDate = DateTime.Now + TimeSpan.FromDays(45);
                message.TimeStamp = DateTime.Now;
                Db.dataBase.AddMessage(message);
                MessageBox.Show("Message Sent!");
                this.Close();
            }
            else
            {
                message.ReadMessage(users.First());
                this.Close();
            }
        }
    }
}
