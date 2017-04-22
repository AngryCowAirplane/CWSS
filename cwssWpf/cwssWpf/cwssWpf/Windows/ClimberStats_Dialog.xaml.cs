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
            PreviewKeyDown += Helpers.HandleEsc;
        }

        private void doStats()
        {
            if(SelectedUser != null)
            {
                lblName.Content = SelectedUser.GetName();
                // Member Since
                lblUserCreated.Content = lblUserCreated.Content + SelectedUser.DateCreated.ToShortDateString();
                lblLastCheckIn.Content = lblLastCheckIn.Content + SelectedUser.LastCheckIn.ToShortDateString() + "-" + SelectedUser.LastCheckIn.ToShortTimeString();

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

                // CERTS
                var docs = SelectedUser.Documents;
                if(docs.Count > 0)
                {
                    var lead = docs.Where(d => d.DocumentType == DocType.LeadClimb).ToList();
                    var belay = docs.Where(d => d.DocumentType == DocType.BelayCert).ToList();
                    if(lead != null && lead.Count > 0)
                    {
                        var cert = lead.First();
                        lblLead.Content = "Lead Climber - " + cert.Expires.ToShortDateString();
                        if (DateTime.Now > cert.Expires)
                            lblLead.Foreground = Brushes.Red;
                    }
                    if(belay != null && belay.Count > 0)
                    {
                        var cert = belay.First();
                        lblBelay.Content = "Belay Cert. - " + cert.Expires.ToShortDateString();
                        if (cert.Expires > cert.Expires)
                            lblBelay.Foreground = Brushes.Red;
                    }
                }
            }
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
