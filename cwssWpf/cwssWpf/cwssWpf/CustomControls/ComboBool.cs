using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace cwssWpf.CustomControls
{
    public class ComboBool : ComboBox
    {
        public bool Value = false;

        public ComboBool()
        {
            this.Items.Add(false);
            this.Items.Add(true);
            this.SelectedItem = (false);
            this.SelectionChanged += selectionChanged;
        }

        private void selectionChanged(object sender, RoutedEventArgs e)
        {
            Value = (bool)this.SelectedItem;
        }
    }
}
