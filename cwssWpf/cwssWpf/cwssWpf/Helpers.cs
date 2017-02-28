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

        public static void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                ((Window)sender).DragMove();
        }
    }
}
