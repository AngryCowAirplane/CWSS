using cwssWpf.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using System.Windows;
using cwssWpf.DataBase;
using System.Media;
using System.Windows.Input;
using System.Net;
using System.Net.Sockets;
using System.Web;
using cwssWpf.Windows;
using System.Web.Script.Serialization;

namespace cwssWpf
{
    public class Helpers
    {
        // encryption keys
        private static string passPhrase = "70392DE7-5FB4-4520-A9A6-3CD231E181C0";
        private static string saltValue = "09290C9F-1B71-4A0E-92C9-51E03E6868D3";

        public static string EncryptString(string str)
        {
            return RijndaelEncryptDecrypt.EncryptDecryptUtils.Encrypt(str, passPhrase, saltValue, "SHA1");
        }
        public static string DecryptString(string str)
        {
            return RijndaelEncryptDecrypt.EncryptDecryptUtils.Decrypt(str, passPhrase, saltValue, "SHA1");
        }

        public static void PlaySuccess()
        {
            SoundPlayer player = new SoundPlayer(System.IO.Path.Combine(Environment.CurrentDirectory + @"\Sounds\Success.wav"));
            bool soundFinished = true;

            if (soundFinished)
            {
                soundFinished = false;
                Task.Factory.StartNew(() => { player.PlaySync(); soundFinished = true; });
            }
        }

        public static void PlayFail()
        {
            SoundPlayer player = new SoundPlayer(System.IO.Path.Combine(Environment.CurrentDirectory + @"\Sounds\Fail.wav"));
            bool soundFinished = true;

            if (soundFinished)
            {
                soundFinished = false;
                Task.Factory.StartNew(() => { player.PlaySync(); soundFinished = true; });
            }
        }


        public static void PlayLogOff()
        {
            SoundPlayer player = new SoundPlayer(System.IO.Path.Combine(Environment.CurrentDirectory + @"\Sounds\LogOff.wav"));
            bool soundFinished = true;

            if (soundFinished)
            {
                soundFinished = false;
                Task.Factory.StartNew(() => { player.PlaySync(); soundFinished = true; });
            }
        }


        public static void PlayLogin()
        {
            SoundPlayer player = new SoundPlayer(System.IO.Path.Combine(Environment.CurrentDirectory + @"\Sounds\LogIn.wav"));
            bool soundFinished = true;

            if (soundFinished)
            {
                soundFinished = false;
                Task.Factory.StartNew(() => { player.PlaySync(); soundFinished = true; });
            }
        }


        public static void PlayCheckOut()
        {
            SoundPlayer player = new SoundPlayer(System.IO.Path.Combine(Environment.CurrentDirectory + @"\Sounds\CheckOut.wav"));
            bool soundFinished = true;

            if (soundFinished)
            {
                soundFinished = false;
                Task.Factory.StartNew(() => { player.PlaySync(); soundFinished = true; });
            }
        }

        public static void PlayCheckIn()
        {
            SoundPlayer player = new SoundPlayer(System.IO.Path.Combine(Environment.CurrentDirectory + @"\Sounds\CheckIn.wav"));
            bool soundFinished = true;

            if (soundFinished)
            {
                soundFinished = false;
                Task.Factory.StartNew(() => { player.PlaySync(); soundFinished = true; });
            }
        }

        public static Uri GenerateEmailUriFromList(List<string> emails)
        {
            var mailString = emails.First() + "?cc=";
            int count = 0;
            foreach (var email in emails)
            {
                if (count > 0)
                    mailString = mailString + email + ";";
                count++;
            }

            mailString = mailString.Remove(mailString.Length - 1, 1);

            var uri = new Uri("mailto:" + mailString);
            return uri;
        }

        public static bool SendEmail(string from, List<string> to, string subject, string body,
            string username = "", string password = "")
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                username = Config.Data.Email.EmailAddress;
                password = Config.Data.Email.Password;
            }

            MailMessage mail = new MailMessage();// from, recipients, subject, message);

            foreach (var mailAddress in to)
            {
                mail.To.Add(mailAddress);
            }

            mail.From = new MailAddress(from);
            mail.Subject = subject;
            mail.Body = body;

            SmtpClient client = new SmtpClient();
            client.Port = Config.Data.Email.SmtpPort;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = Config.Data.Email.UseDefaultCredentials;
            client.Credentials = new System.Net.NetworkCredential(username, password);
            client.EnableSsl = Config.Data.Email.EnableSsl;
            client.Host = Config.Data.Email.SmtpServer;

            try
            {
                client.Send(mail);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        internal static int GenerateNewID()
        {
            var firstNonStudentNumber = -1;
            var logins = Db.dataBase.Users.Select(t => t.LoginId).Where(
                t => t >= StaticValues.StartNonStudentIdNumber && t < StaticValues.EndNonStudentIdNumber);

            if (logins.Count() > 0)
            {
                for (int i = firstNonStudentNumber; i < StaticValues.EndNonStudentIdNumber; i++)
                {
                    if (!logins.Contains(i))
                        firstNonStudentNumber = i;
                }
            }
            else
                firstNonStudentNumber = StaticValues.StartNonStudentIdNumber;

            return firstNonStudentNumber;
        }

        public static void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                ((Window)sender).DragMove();
        }

        // http://stackoverflow.com/questions/6803073/get-local-ip-address
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
    }

    public class Result
    {
        public bool Success = false;
        public string Heading = "";
        public string Body = "";

        [ScriptIgnore]
        public Alert_Dialog Alert;


        public Result() { }
        public Result(bool success)
        {
            Success = success;
        }
        public Result(Alert_Dialog alert)
        {
            Alert = alert;
        }
        public Result(bool success, Alert_Dialog alert)
        {
            Success = success;
            Alert = alert;
        }

        public void Show()
        {
            var alert = new Alert_Dialog(Heading, Body);
                alert.ShowDialog();
        }

        public void Initialize()
        {
            Heading = (string)Alert.Title.Content;
            Body = (string)Alert.AlertText.Text;
        }
    }
}
