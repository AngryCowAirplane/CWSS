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
using System.Windows.Threading;

namespace cwssWpf.Windows
{
    public partial class ClimberView_Dialog : Window
    {
        private MainWindow mainWindow;
        private User selectedUser;
        public static DispatcherTimer QuickTimer = new DispatcherTimer();
        public List<User> UsersCheckedIn = new List<User>();

        public ClimberView_Dialog(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            //Top = mainWindow.Top + 50;
            //Left = mainWindow.Left + 50;
            UsersCheckedIn = Db.dataBase.Users.Where(user => user.CheckedIn == true).ToList();
            UsersCheckedIn.OrderByDescending(x => x.LastCheckIn);
            lvClimbers.ItemsSource = UsersCheckedIn;
            updateList();
            lvClimbers.PreviewMouseRightButtonDown += rightMouseButtonClicked;
            MouseLeftButtonDown += Helpers.Window_MouseDown;
            //PreviewKeyDown += Helpers.HandleEsc;
            QuickTimer.Interval = TimeSpan.FromSeconds(1);
            QuickTimer.Tick += OnQuickTimerTick;
            QuickTimer.Start();
            this.Topmost = true;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }

        private void OnQuickTimerTick(object sender, EventArgs e)
        {
            UsersCheckedIn = Db.dataBase.Users.Where(user => user.CheckedIn == true).ToList();
            lvClimbers.ItemsSource = null;
            lvClimbers.Items.Refresh();
            lvClimbers.ItemsSource = UsersCheckedIn.OrderByDescending(x => x.LastCheckIn);
            lvClimbers.Items.Refresh();
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            Config.Data.Misc.ClimberView.Open = false;
            this.Close();
        }

        private void cmCheckOut_Click(object sender, RoutedEventArgs e)
        {
            if (selectedUser != null)
            {
                userMenu.IsOpen = false;

                selectedUser.CheckOut();
                mainWindow.UpdateClimberStats();
                lvClimbers.ItemsSource = Db.dataBase.Users.Where(user => user.CheckedIn == true);
                updateList();
            }
        }

        private void cmRevoke_Click(object sender, RoutedEventArgs e)
        {
            if(selectedUser != null)
            {
                var request = new Request_Dialog(selectedUser);
                request.ShowDialog();
            }
        }

        private void cmStats_Click(object sender, RoutedEventArgs e)
        {
            if (selectedUser != null)
            {
                var stats = new ClimberStats_Dialog(selectedUser);
                stats.ShowDialog();
            }
        }

        private void rightMouseButtonClicked(object sender, MouseEventArgs e)
        {
            if (lvClimbers.Items.Count > 0)
            {
                if (lvClimbers.SelectedIndex == -1)
                    return;

                selectedUser = (User)lvClimbers.SelectedItem;
                lvClimbers.ScrollIntoView(selectedUser);
            }
        }

        private void updateList()
        {
            lvClimbers.Items.Refresh();
        }

        private void cmPromote_Click(object sender, RoutedEventArgs e)
        {
            if (selectedUser != null)
            {
                selectedUser.PromoteLead();
            }
        }

        private void cmBelayCert_Click(object sender, RoutedEventArgs e)
        {
            if (selectedUser != null)
            {
                selectedUser.AddBelayCert();
            }
        }
    }
}
