﻿using cwssWpf.Data;
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
    public partial class ClimberView_Dialog : Window
    {
        private MainWindow mainWindow;
        private User selectedUser;

        public ClimberView_Dialog(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            Top = mainWindow.Top + 30;
            Left = mainWindow.Left + 30;
            lvClimbers.ItemsSource = Db.dataBase.Users.Where(user => user.CheckedIn == true);
            updateList();
            lvClimbers.PreviewMouseRightButtonDown += rightMouseButtonClicked;
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cmCheckOut_Click(object sender, RoutedEventArgs e)
        {
            userMenu.IsOpen = false;

            selectedUser.CheckOut();
            mainWindow.UpdateClimberStats();
            lvClimbers.ItemsSource = Db.dataBase.Users.Where(user => user.CheckedIn == true);
            updateList();
        }

        private void cmRevoke_Click(object sender, RoutedEventArgs e)
        {
            //Db.dataBase.Notes.Requests.Clear();
            if(selectedUser != null)
            {
                var request = new Request();
                request.Patron = selectedUser;
                request.Reason = "Smelly";
                request.SuspensionLength = Suspension.Month;
                request.TimeStamp = DateTime.Now;
                request.Enforced = false;

                Db.dataBase.Notes.Requests.Add(request);
            }
        }

        private void cmStats_Click(object sender, RoutedEventArgs e)
        {

        }

        private void rightMouseButtonClicked(object sender, MouseEventArgs e)
        {
            if (lvClimbers.Items.Count > 0)
            {
                if (lvClimbers.SelectedIndex == -1)
                    return;

                selectedUser = (User)lvClimbers.SelectedItem;
            }
        }

        private void updateList()
        {
            lvClimbers.Items.Refresh();
            //lblNumUsers.Content = lvClimbers.Items.Count;
        }
    }
}