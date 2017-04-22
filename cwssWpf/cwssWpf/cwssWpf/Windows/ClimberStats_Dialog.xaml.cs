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
    /// Interaction logic for ClimberStats_Dialog.xaml
    /// </summary>
    public partial class ClimberStats_Dialog : Window
    {
        public User SelectedUser;
        public ClimberStats_Dialog(User user)
        {
            InitializeComponent();
            SelectedUser = user;
            doStats();
            MouseLeftButtonDown += Helpers.Window_MouseDown;
        }

        private void doStats()
        {
            // Member Since
            lblUserCreated.Content = lblUserCreated.Content + SelectedUser.DateCreated.ToShortDateString();

            // Waiver
            try
            {
                var waiver = SelectedUser.Documents.Where(doc => doc.DocumentType == DocType.Waiver).Last();
                lblWavier.Content = lblWavier.Content + waiver.Date.ToShortDateString();
                lblWaiverExpires.Content = lblWaiverExpires.Content + waiver.Expires.ToShortDateString();
            }
            catch
            { }

            // Requests
            if (Db.dataBase.Notes.Requests.Count > 0)
            {
                try
                {
                    var req = Db.dataBase.Notes.Requests.Where(request => request.Patron.LoginId == SelectedUser.LoginId).First();

                    if (req != null)
                    {
                        var expires = req.TimeStamp + TimeSpan.FromDays((int)req.SuspensionLength * 7);
                        lblRevoked.Content = lblRevoked.Content + "YES - Expires: " + expires.ToShortDateString();
                        lblReason.Content = req.Reason;
                    }
                }
                catch
                {

                }
            }
            else if(SelectedUser.CanClimb == false)
            {
                lblRevoked.Content = lblRevoked.Content + "YES";
                lblReason.Content = "Hard Revoked.";
            }
            else
                lblRevoked.Content = lblRevoked.Content + "NO";
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
