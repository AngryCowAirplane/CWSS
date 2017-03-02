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
        public Message_Dialog(List<User> users = null)
        {
            if(users == null)
                this.users = new List<User>();
            else
                this.users = users;

            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            tbTo.IsEnabled = false;
            tbFrom.IsEnabled = false;
            tbFrom.Text = MainWindow.CurrentUser.GetName();
            cbMode.ItemsSource = (Enum.GetValues(typeof(MessageMode)).Cast<MessageMode>().ToList());
            cbMode.SelectionChanged += modeChange;
            tbTo.TextChanged += toTextChange;
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

        public Message_Dialog(int userId, Message message)
        {
            var user = Db.dataBase.Users.Where(u => u.LoginId == userId).First();
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
            if (users != null && users.Count > 1)
            {
                cbMode.SelectedItem = MessageMode.Multi;
                tbTo.Text = listToString();
            }
            else
            {
                cbMode.SelectedItem = MessageMode.Single;
                if (users != null && users.Count == 1)
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
                    tbTo.IsEnabled = true;
                    if (users != null && users.Count > 1)
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
                toString = toString.Remove(toString.Length - 2, 2);
            }
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

                if((MessageMode)cbMode.SelectedItem == MessageMode.Employees)
                {
                    message.SetRecipients(Db.dataBase.Users.Where(user => (int)user.UserType > 0).ToList());
                }
                else if ((MessageMode)cbMode.SelectedItem == MessageMode.Managers)
                {
                    message.SetRecipients(Db.dataBase.Users.Where(user => (int)user.UserType > 1).ToList());
                }
                else if ((MessageMode)cbMode.SelectedItem == MessageMode.Everyone)
                {
                    message.SetRecipients(Db.dataBase.Users.ToList());
                }
                else
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

        private void toTextChange(object sender, RoutedEventArgs e)
        {
            // build dictionary of names to User object in db.
            // custom TextBox for To box?
            if((MessageMode)cbMode.SelectedItem == MessageMode.Single)
            {
                users = new List<User>();
                tbTo.Foreground = Brushes.DarkRed;
                foreach (var user in Db.dataBase.Users)
                {
                    if(user.GetName().ToLower() == (tbTo.Text.ToLower()))
                    {
                        this.users.Add(user);
                        tbTo.Foreground = Brushes.Black;
                    }
                }
            }
            // this dont work yet
            else if ((MessageMode)cbMode.SelectedItem == MessageMode.Multi)
            {
                tbTo.Foreground = Brushes.DarkRed;
                foreach (var user in Db.dataBase.Users)
                {
                    if (user.GetName().ToLower() == (tbTo.Text.ToLower()))
                    {
                        if(!users.Contains(user))
                            this.users.Add(user);

                        tbTo.Foreground = Brushes.Black;
                    }
                }
            }
            else
            {
                tbTo.Foreground = Brushes.Black;
            }
        }
    }
}
