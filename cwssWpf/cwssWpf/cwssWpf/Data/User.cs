﻿using cwssWpf.DataBase;
using cwssWpf.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace cwssWpf.Data
{
    public class User
    {
        public int UserId { get; set; }
        public int LoginId { get; set; }
        public string Password { get; set; }
        public UserType UserType { get; set; }
        public PersonalInfo Info { get; set; }

        public bool CanClimb { get; set; }
        public bool CheckedIn { get; set; }
        public DateTime TimeStamp { get; set; }

        public List<int> Items;
        public List<Document> Documents;

        public User()
        {
            Info = new PersonalInfo();
            Items = new List<int>();
            Documents = new List<Document>();
        }

        public bool HasWaiver()
        {
            try
            {
                var waivers = Documents.Where(t => t.DocumentType == DocType.Waiver);
                var waiver = waivers.Where(t=>t.UserId == LoginId).First();
                if (waiver.Expires > DateTime.Now)
                {
                    var daysLeft = DateTime.Now - waiver.Expires;
                    //if(daysLeft < TimeSpan.FromDays(7))
                    //{
                    //    var alert = new Alert_Dialog("Waiver Expires Soon!", "Your Waiver Expires in " + daysLeft.ToString() + ".\n  Please Re-Read and Sign the Waiver Form.", AlertType.Notice);
                    //    MainWindow.WindowsOpen.Add(alert, new TimerVal(3));
                    //    alert.ShowDialog();
                    //    Helpers.ShowWaiver(this);
                    //}
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return false;
        }

        public string GetName()
        {
            return Info.FirstName + " " + Info.LastName;
        }

        public int GetUserId()
        {
            return LoginId;
        }

        public bool GetCheckedIn()
        {
            return CheckedIn;
        }

        public bool CheckOut()
        {
            if (CheckedIn)
            {
                //var length = DateTime.Now - TimeStamp;
                //var message = Info.FirstName + " " + Info.LastName + " Checked Out.";
                //Logger.Log(UserId, LogType.CheckOut, message);
                //message = message + "\nDuration: " + length.TotalMinutes.ToString() + " minutes.";
                //Helpers.PlayCheckOut();
                //var alert = new Alert_Dialog("Checked Out", message);
                //alert.ShowDialog();
                //CheckedIn = false;
                //return true;
                CheckedIn = false;
                return true;
            }
            else
                return false;
        }

        public bool CheckIn()
        {
            if (!CheckedIn)
            {
                TimeStamp = DateTime.Now;
                var message = Info.FirstName + " " + Info.LastName + " Checked In.";
                Helpers.PlayCheckIn();
                Logger.Log(UserId, LogType.CheckIn, message);
                CheckedIn = true;
                return true;
            }
            else
                return false;
        }

        public bool AddWaiver()
        {
            var waiverDoc = new Document();
            waiverDoc.DocumentType = DocType.Waiver;
            waiverDoc.Date = DateTime.Now;
            waiverDoc.Expires = DateTime.Now + TimeSpan.FromDays(Config.Data.Data.DaysWaiverExpires);
            waiverDoc.FileLocation = "not yet implemented";
            waiverDoc.UserId = LoginId;

            Documents.Add(waiverDoc);
            Logger.Log(LoginId, LogType.Waiver, GetName() + " Signed Waiver.");
            return true;
        }

        public Document GetWaiver()
        {
            try
            {
                var waivers = Documents.Where(t => t.DocumentType == DocType.Waiver);
                var waiver = waivers.Where(t => t.UserId == LoginId).First();
                return waiver;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public DateTime GetWaiverDate()
        {
            var waiver = GetWaiver();
            if (waiver != null)
                return waiver.Date;
            else
                return DateTime.MinValue;
        }

        public Uri GetEmailUri()
        {
            var builder = new UriBuilder();
            var uri = new Uri("mailto:" + GetEmailAddress());
            return uri;
        }

        public string GetEmailAddress()
        {
            return Info.Email;
        }

        public bool UpdateUserType(UserType type)
        {
            if (type != UserType)
            {
                Logger.Log(MainWindow.CurrentUser.LoginId, LogType.EditUser, UserType.ToString() + ", " + GetName() + " (" + LoginId + ") Was Updated To " + type.ToString());
                UserType = type;
                return true;
            }
            else
                return false;
        }

        public bool SetClimbingPrivilege(bool canClimb)
        {
            if (canClimb != CanClimb)
            {
                var message = "";
                CanClimb = canClimb;

                if (CanClimb == false)
                    message = MainWindow.CurrentUser.GetName() + " Revoked " + GetName() + "'s Climbing Privilege.";
                else
                    message = MainWindow.CurrentUser.GetName() + " Restored " + GetName() + "'s Climbing Privilege.";

                Logger.Log(MainWindow.CurrentUser.GetUserId(), LogType.Privilege, message);
                return true;
            }
            else
                return false;
        }

        public Request CheckRequest()
        {
            return Db.dataBase.Notes.CheckRequest(this);
        }

        public override string ToString()
        {
            return (GetName() + " (" + LoginId + ")");
        }
    }

    public class PersonalInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderType Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }

    public enum UserType
    {
        Patron = 0,
        Employee = 1,
        Manager = 2,
        Admin = 3
    }

    public enum GenderType
    {
        Male = 0,
        Female = 1
    }
}
