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
    /// Interaction logic for Postit_Dialog.xaml
    /// </summary>
    public partial class Postit_Dialog : Window
    {
        public Note note;

        public Postit_Dialog(Note note)
        {
            //WindowStartupLocation = WindowStartupLocation.CenterOwner;
            InitializeComponent();

            this.note = note;
            Top = note.Top;
            Left = note.Left;
            Width = note.Width;
            Height = note.Height;

            message.Text = note.Contents;
            message.IsReadOnly = true;

            Deactivated += Window_Deactivated;
            MouseLeftButtonDown += Helpers.Window_MouseDown;
            LocationChanged += windowLocation_Changed;
            SizeChanged += windowLocation_Changed;

            if((int)MainWindow.CurrentUser.UserType < 2)
            {
                userMenu.IsEnabled = false;
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }

        private void cmDelete_Click(object sender, RoutedEventArgs e)
        {
            Db.dataBase.Notes.WallNotes.Remove(note);
            this.Close();
        }

        private void windowLocation_Changed(object sender, EventArgs e)
        {
            var dbNote = Db.dataBase.Notes.WallNotes.Where(t => t == note).First();
            dbNote.Top = Top;
            dbNote.Left = Left;
            dbNote.Width = Width;
            dbNote.Height = Height;
        }
    }
}
