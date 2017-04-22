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
    /// Interaction logic for Notes_Dialog.xaml
    /// </summary>
    public partial class Notes_Dialog : Window
    {
        public Notes_Dialog()
        {
            InitializeComponent();
            MouseLeftButtonDown += Helpers.Window_MouseDown;
            PreviewKeyDown += Helpers.HandleEsc;
            KeyUp += EnterPressed;
            Message.Focus();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            var note = new Data.Note();
            note.Contents = Message.Text;
            note.Top = Top;
            note.Left = Left;
            note.Width = Width;
            note.Height = Height;
            note.UserId = MainWindow.CurrentUser.LoginId;

            Db.dataBase.Notes.WallNotes.Add(note);
            var postit = new Postit_Dialog(note);
            postit.Show();
            MainWindow.WindowsOpen.Add(postit, new TimerVal(-1));
            this.Close();
        }

        private void EnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            add_Click(null, null);
        }
    }
}
