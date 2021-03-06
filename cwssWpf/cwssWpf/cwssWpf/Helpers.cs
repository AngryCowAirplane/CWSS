﻿using cwssWpf.Data;
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
using Newtonsoft.Json;
using System.Windows.Controls;
using System.IO;

namespace cwssWpf
{
    public class TimerVal { public int time { get; set; } public TimerVal(int time) { this.time = time; } }

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

        public static void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                ((Window)sender).Close();
        }

        public static IEnumerable<DateTime> AllDatesInMonth(int year, int month)
        {
            int days = DateTime.DaysInMonth(year, month);
            for (int day = 1; day <= days; day++)
            {
                yield return new DateTime(year, month, day);
            }
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

        internal static string TryGetCardId(string idString)
        {
            var id = idString.Substring(StaticValues.CardReaderStartIndexOfID, StaticValues.StudentIdLength);
            return id;
        }


        // Validate Field Methods
        internal static bool ValidateIdInput(string idString)
        {
            var cardIds = Db.dataBase.Users.Select(x => x.CardId);
            try
            {
                var match = cardIds.Where(id => id == idString).First();

                if (cardIds.Contains(idString))
                    return true;
            }

            catch { }

            {
                bool isGood = true;
                try
                {
                    var integerValue = int.Parse(idString);
                    if (integerValue < StaticValues.StartNonStudentIdNumber || integerValue > StaticValues.EndStudentIdNumber)
                        isGood = false;
                }
                catch
                {
                    isGood = false;
                }

                return isGood;
            }
        }

        public static User getUserFromCheckInText(string text = "")
        {
            if (text.Length > StaticValues.StudentIdLength)
            {
                var user = Db.dataBase.GetUser(text);
                return user;
            }
            else
            {
                var loginId = int.Parse(text);
                var user = Db.dataBase.GetUser(loginId);
                return user;
            }
        }

        public static bool ShowWaiver(User user)
        {
            var waiver = new Waiver_Dialog(user);
            var signedWaiver = waiver.ShowDialog();
            if ((bool)signedWaiver)
            {
                user.AddWaiver();
            }

            return (bool)signedWaiver;
        }

        public static User TryAddUser(TextBox firstName, TextBox LastName, TextBox userId, PasswordBox password1, PasswordBox password2,
            TextBox email, TextBox address, TextBox city, TextBox state, TextBox zip, TextBox phone, ComboBox gender, DatePicker dob, TextBox cardID, TextBox guardianID)
        {
            User NewUser = null;

            var success = Db.dataBase.AddUser(
                firstName, LastName,
                userId, password1, password2,
                email, address, city, state, zip,
                phone, gender, dob, cardID, guardianID
            );

            if (success)
            {
                var oldUser = MainWindow.CurrentUser;
                if (MainWindow.CurrentUser == null)
                    MainWindow.CurrentUser = new User();

                Logger.Log(MainWindow.CurrentUser.LoginId, LogType.AddUser,
                    MainWindow.CurrentUser.GetName() + " Added User: " +
                    firstName.Text + " " + LastName.Text);

                NewUser = Db.dataBase.GetUser(int.Parse(userId.Text));

                MainWindow.CurrentUser = oldUser;
                return NewUser;
            }
            else
            {
                if (MainWindow.CurrentUser == null)
                    MainWindow.CurrentUser = new User();

                Logger.Log(MainWindow.CurrentUser.LoginId, LogType.Error,
                    MainWindow.CurrentUser.GetName() + " Failed Add User: " +
                    firstName.Text + " " + LastName.Text);

                MainWindow.CurrentUser = null;
                return null;
            }
        }

        public static User TryAddUser(User user)
        {
            User NewUser = null;
            bool success = false;

            if(user != null)
            {
                success = Db.dataBase.AddUser(
                    user.Info.FirstName, user.Info.LastName,
                    user.LoginId, user.Password, user.Password,
                    user.Info.Email, user.Info.Address, user.Info.City, user.Info.State, int.Parse(user.Info.Zip),
                    user.Info.Phone, user.Info.Gender, user.Info.DateOfBirth, user.CardId, user.Info.Guardian
                );
            }


            if (success)
            {
                if (MainWindow.CurrentUser == null)
                    MainWindow.CurrentUser = new User();

                Logger.Log(MainWindow.CurrentUser.LoginId, LogType.AddUser,
                    MainWindow.CurrentUser.GetName() + " Added User: " +
                    user.Info.FirstName + " " + user.Info.LastName);

                NewUser = Db.dataBase.GetUser((user.LoginId));

                //var alert = new Alert_Dialog("User Created!", user.GetName(), AlertType.Success);
                //MainWindow.WindowsOpen.Add(alert, new TimerVal(2));
                //alert.Show();

                return NewUser;
            }
            else
            {
                if (MainWindow.CurrentUser == null)
                    MainWindow.CurrentUser = new User();


                Logger.Log(MainWindow.CurrentUser.LoginId, LogType.Error,
                    MainWindow.CurrentUser.GetName() + " Failed Add User");

                //var alert = new Alert_Dialog("Add User Failed", "");
                //MainWindow.WindowsOpen.Add(alert, new TimerVal(2));
                //alert.Show();
                return null;
            }
        }

    }

    public class CheckinResult
    {
        public bool Success = false;
        public string Heading = "";
        public string Body = "";

        [JsonIgnore]
        public Alert_Dialog Alert;


        public CheckinResult() { }
        public CheckinResult(bool success)
        {
            Success = success;
        }
        public CheckinResult(Alert_Dialog alert)
        {
            Alert = alert;
        }
        public CheckinResult(bool success, Alert_Dialog alert)
        {
            Success = success;
            Alert = alert;
        }

        public void Show()
        {
            var alert = new Alert_Dialog(Heading, Body, (Success == true ? AlertType.Success : AlertType.Failure));
            alert.Show();
        }

        public void Initialize()
        {
            Heading = (string)Alert.Title.Content;
            Body = (string)Alert.AlertText.Text;
        }
    }
}
