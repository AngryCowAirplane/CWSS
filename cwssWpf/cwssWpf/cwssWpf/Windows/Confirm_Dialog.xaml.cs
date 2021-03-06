﻿using System;
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
    /// Interaction logic for Confirm.xaml
    /// </summary>
    public partial class Confirm_Dialog : Window
    {
        public bool Confirmed { get; set; }
        private string message;

        public Confirm_Dialog(Window window, string Message = "")
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            message = Message;
            InitializeComponent();
            Confirmed = false;
            if(window != null)
            {
                Top = window.Top + 50;
                Left = window.Left + 50;
            }
            lbMessage.Content = message;
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            Confirmed = true;
            this.Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            Confirmed = false;
            this.Close();
        }
    }
}
