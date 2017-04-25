using cwssWpf.Data;
using cwssWpf.DataBase;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for Reports_Dialog.xaml
    /// </summary>
    public partial class Reports_Dialog : Window
    {
        public Reports_Dialog()
        {
            InitializeComponent();
            MouseLeftButtonDown += Helpers.Window_MouseDown;
            PreviewKeyDown += Helpers.HandleEsc;
            populateLabels();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void populateLabels()
        {
            var totalUsers = Db.dataBase.Users.Count;
            var totalEmployees = Db.dataBase.Users.Where(t => (int)t.UserType > 0).Count();

            lblTotalUsers.Content = lblTotalUsers.Content + " " + totalUsers.ToString();
            lblTotalEmployees.Content = lblTotalEmployees.Content + " " + totalEmployees.ToString();
        }

        private void ClearLabels()
        {
            var removeList = new List<System.Windows.UIElement>();
            foreach (var item in MainGrid.Children)
            {
                if(item.ToString().ToLower().Contains("system.windows.controls.label"))
                {
                    removeList.Add((System.Windows.UIElement)item);
                }
            }

            foreach (var item in removeList)
            {
                MainGrid.Children.Remove(item);
            }
        }



        private void Revocations_Click(object sender, RoutedEventArgs e)
        {
            ClearLabels();
            var revocations = Db.dataBase.Notes.Requests;

            var label = new Label();
            label.Content = "Revocation Requests: " + revocations.Count.ToString();
            Grid.SetRow(label, 1);
            MainGrid.Children.Add(label);

            int x = 2;
            foreach (var item in revocations)
            {
                label = new Label();
                label.Content = "-" + item.Patron.GetName() + " : " + item.Reason;
                Grid.SetRow(label, x);
                MainGrid.Children.Add(label);
                x++;
            }
        }

        private void Waivers_Click(object sender, RoutedEventArgs e)
        {
            ClearLabels();

            var waivers = new List<Document>();

            foreach (var user in Db.dataBase.Users)
            {
                if(user.Documents.Count>0)
                {
                    var waiv = user.Documents.Where(d => d.DocumentType == DocType.Waiver);
                    if(waiv != null)
                    {
                        waivers.Add(waiv.Last());
                    }
                }
            }

            var waiversExp = waivers.Where(w => (w.Expires) - DateTime.Now < TimeSpan.FromDays(7)).ToList();

            var label = new Label();
            label.Content = "Waivers Expiring (7 days): " + waiversExp.Count.ToString();
            Grid.SetRow(label, 1);
            MainGrid.Children.Add(label);

            if (waiversExp != null && waiversExp.Count > 0)
            {
                int x = 2;
                foreach (var item in waiversExp)
                {
                    var time = (item.Expires - DateTime.Now).Days;
                    var user = Db.dataBase.GetUser(item.UserId);

                    label = new Label();
                    label.Content = "-" + user.GetName() + " : " + item.Expires.ToShortDateString();
                    if (time <= 0) label.Foreground = Brushes.Red;
                    Grid.SetRow(label, x);
                    MainGrid.Children.Add(label);
                    x++;
                }
            }
        }

        private void Belay_Click(object sender, RoutedEventArgs e)
        {
            ClearLabels();
            var label = new Label();
            label.Content = "*Not Implemented";
            Grid.SetRow(label, 1);
            MainGrid.Children.Add(label);
        }

        private void LeadClimb_Click(object sender, RoutedEventArgs e)
        {
            ClearLabels();
            var label = new Label();
            label.Content = "*Not Implemented";
            Grid.SetRow(label, 1);
            MainGrid.Children.Add(label);
        }

        #region notUsed
        //private void Stats_Click(object sender, RoutedEventArgs e)
        //{
        //    ClearLabels();

        //    var monthDailyLogs = new List<DailyLog>();

        //    foreach (DateTime date in Helpers.AllDatesInMonth(DateTime.Now.Year, DateTime.Now.Month))
        //    {
        //        var dayLog = Logger.GetLog(date);
        //        if (dayLog != null)
        //            monthDailyLogs.Add(Logger.GetLog(date));
        //    }

        //    if(monthDailyLogs.Count > 0)
        //    {
        //        var checkinLogsForMonth = new List<Log>();
        //        foreach (var log in monthDailyLogs)
        //        {
        //            var logs = log.Logs.Where(l => l.Action == LogType.CheckIn);
        //            checkinLogsForMonth.AddRange(logs);
        //        }

        //        if(checkinLogsForMonth.Count > 0)
        //        {
        //            var label = new Label();
        //            label.Content = "USER CHECK IN STATS";
        //            Grid.SetRow(label, 1);
        //            MainGrid.Children.Add(label);

        //            // Total Month Count
        //            label = new Label();
        //            label.Content = "Total For " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month) + ": " + checkinLogsForMonth.Count;
        //            Grid.SetRow(label, 2);
        //            MainGrid.Children.Add(label);
        //        }
        //    }
        //}
        #endregion
    }
}
