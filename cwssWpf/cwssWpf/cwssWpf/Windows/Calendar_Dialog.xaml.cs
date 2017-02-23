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
    /// Interaction logic for Calendar_Dialog.xaml
    /// </summary>
    public partial class Calendar_Dialog : Window
    {
        public Calendar_Dialog()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            var style = Calendar.CalendarDayButtonStyle;
            var displayDate = Calendar.DisplayDate; // DisplayDateStart DisplayDateEnd
            var mode = Calendar.DisplayMode = CalendarMode.Month;

            this.SizeChanged += sizeChanged;


            for (int i = 0; i < 20; i++)
            {
                lbEvents.Items.Add(i);
            }

        }

        private void sizeChanged(object sender, RoutedEventArgs e)
        {
            lbEvents.Height = vbCalendar.ActualHeight - 20;
            lbEvents.Width = vbCalendar.ActualWidth - 20;
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void calendarMenuAddEvent_Click(object sender, RoutedEventArgs e)
        {
            var eventWindow = new Event_Dialog();
            eventWindow.ShowDialog();
        }

        private void eventMenuTest_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Event At Time Clicked");
        }
    }

    public class CalendarHelper
    {
        public static readonly DependencyProperty DateProperty =
        DependencyProperty.RegisterAttached("Date", typeof(DateTime), typeof(CalendarHelper), new PropertyMetadata { PropertyChangedCallback = DatePropertyChanged });

        //-----------------------------------------------------------------------------------------------------

        private static readonly DependencyPropertyKey IsHolidayPropertyKey =
        DependencyProperty.RegisterAttachedReadOnly("IsHoliday", typeof(bool), typeof(CalendarHelper), new PropertyMetadata());
        public static readonly DependencyProperty IsHolidayProperty = IsHolidayPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey IsEventPropertyKey =
        DependencyProperty.RegisterAttachedReadOnly("IsEvent", typeof(bool), typeof(CalendarHelper), new PropertyMetadata());
        public static readonly DependencyProperty IsEventProperty = IsHolidayPropertyKey.DependencyProperty;

        //------------------------------------------------------------------------------------------------

        public static DateTime GetDate(DependencyObject obj)
        {
            return (DateTime)obj.GetValue(DateProperty);
        }

        public static void SetDate(DependencyObject obj, DateTime value)
        {
            obj.SetValue(DateProperty, value);
        }

        public static bool GetIsHoliday(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsHolidayProperty);
        }

        public static bool GetIsEvent(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEventProperty);
        }

        //------------------------------------------------------------------------------------------

        private static void DatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var date = GetDate(d);
            SetIsHoliday(d, CheckIsHoliday(date));
            SetIsEvent(d, CheckIsEvent(date));
        }

        private static bool CheckIsHoliday(DateTime date)
        {
            if (date.Day > 20)
                return true;
            else
                return false;
        }

        private static bool CheckIsEvent(DateTime date)
        {
            if (date.Day == 05)
                return true;
            else
                return false;
        }

        private static void SetIsHoliday(DependencyObject obj, bool value)
        {
            obj.SetValue(IsHolidayPropertyKey, value);
        }

        private static void SetIsEvent(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEventPropertyKey, value);
        }
    }
}
