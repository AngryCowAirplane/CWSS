using cwssWpf.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView_Dialog : Window
    {
        private List<Log> logs = new List<Log>();

        public LogView_Dialog()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();

            dpFrom.SelectedDate = DateTime.Now;
            dpTo.SelectedDate = DateTime.Now;
            dpFrom.SelectedDateChanged += datesChanged;
            dpTo.SelectedDateChanged += datesChanged;
            tbSearch.TextChanged += lbFilters_Changed;

            Logger.SaveLog();
            logs = Logger.GetTodaysLog().Logs;
            lvLogs.ItemsSource = logs;

            populateFilters();
            setSourceOnChanger();
        }

        private void setSourceOnChanger()
        {
            var changeDependency = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(ListView));
            if (changeDependency != null)
                changeDependency.AddValueChanged(lvLogs, updateScreen);

            updateScreen(null, null);
        }

        private void populateFilters()
        {
            lbFilters.SelectionMode = SelectionMode.Multiple;
            lbFilters.ItemsSource = (Enum.GetValues(typeof(LogType)).Cast<LogType>().ToList());
            lbFilters.SelectionChanged += lbFilters_Changed;
            lbFilters.Focusable = false;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void lbFilters_Changed(object sender, RoutedEventArgs e)
        {
            lvLogs.ItemsSource = logs.Where(log => 
                ((lbFilters.SelectedItems.Count > 0) ?
                lbFilters.SelectedItems.Contains(log.Action) : 
                logs.Contains(log)) &&
                log.Comment.ToLower().Contains(tbSearch.Text.ToLower())
            );

            if(lbFilters.SelectedItems.Count == 0 && String.IsNullOrEmpty(tbSearch.Text))
                lvLogs.ItemsSource = logs;
        }

        private void datesChanged(object sender, RoutedEventArgs e)
        {
            logs = new List<Log>();
            for (DateTime date = (DateTime)dpFrom.SelectedDate; date <= dpTo.SelectedDate; date = date.AddDays(1))
            {
                var log = Logger.GetLog(date);
                if(log != null)
                {
                    logs.AddRange(log.Logs);
                }
            }

            lvLogs.ItemsSource = logs;
            lvLogs.Items.Refresh();
            lbFilters_Changed(null, null);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog();

            saveDialog.FileName = "cwLogger " + dpFrom.SelectedDate.Value.ToShortDateString().Replace('/','_') + " - " + dpTo.SelectedDate.Value.ToShortDateString().Replace('/', '_');
            saveDialog.DefaultExt = ".text";
            saveDialog.Filter = "Text documents (.txt)|*.txt";

            var result = saveDialog.ShowDialog();
            if (result == true)
            {
                string fileName = saveDialog.FileName;
                var list = new List<string>();
                foreach (var item in lvLogs.Items)
                {
                    list.Add(item.ToString());
                }

                File.WriteAllLines(fileName, list.ToArray<string>());
            }
        }

        private void updateScreen(object sender, EventArgs e)
        {
            lbCount.Content = "Logs: " + lvLogs.Items.Count;

            // For color changes:
            // need to create new LogListViewItem : ListViewItem class

            //foreach (var item in lvLogs.Items)
            //{
            //    var log = (Log)item;
            //    if(log.Action == LogType.Error)
            //    {
            //         log.Foreground = Brushes.Red;
            //    }
            //}
        }
    }
}
