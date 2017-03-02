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
        }

        // Keep Notes On Top
        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }

        private void cmDelete_Click(object sender, RoutedEventArgs e)
        {
            if ((int)MainWindow.CurrentUser.UserType > 1)
            {
                Db.dataBase.Notes.WallNotes.Remove(note);
                this.Close();
            }
            else
            {
                var alert = new Alert_Dialog("Can't Delete", "Only a manager or admin user may delete a note.");
                alert.ShowDialog();
            }
        }

        // Save new location of window when moved or resized.
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
