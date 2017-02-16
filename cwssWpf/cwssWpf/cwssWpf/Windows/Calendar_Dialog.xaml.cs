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
    /// Interaction logic for Calendar_Dialog.xaml
    /// </summary>
    public partial class Calendar_Dialog : Window
    {
        public Calendar_Dialog()
        {
            InitializeComponent();
            var style = Calendar.CalendarDayButtonStyle;
            var displayDate = Calendar.DisplayDate; // DisplayDateStart DisplayDateEnd
            var mode = Calendar.DisplayMode = CalendarMode.Month;


        }
    }
}
