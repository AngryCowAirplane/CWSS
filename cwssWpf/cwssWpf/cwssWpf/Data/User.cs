﻿using System;
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
                    return true;
            }
            catch (Exception e)
            {
                // TODO:
                // throw exception up instead of MessageBox
                MessageBox.Show("Waiver Not Found!");
                return false;
            }
            return false;
        }

        public string GetName()
        {
            return Info.FirstName + " " + Info.LastName;
        }

        public bool GetCheckedIn()
        {
            return CheckedIn;
        }

        public bool CheckOut()
        {
            if (CheckedIn)
            {
                TimeStamp = DateTime.Now;
                var length = DateTime.Now - TimeStamp;
                var message = Info.FirstName + " " + Info.LastName + " Checked Out.";
                Logger.Log(UserId, LogType.CheckOut, message);
                message = Info.FirstName + " " + Info.LastName + "\nDuration: " + length.TotalMinutes.ToString() + " minutes.";
                MessageBox.Show(message);
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
                MessageBox.Show(message);
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
            waiverDoc.Expires = DateTime.Now + TimeSpan.FromDays(90);
            waiverDoc.FileLocation = "not yet implemented";
            waiverDoc.UserId = LoginId;

            Documents.Add(waiverDoc);
            Logger.Log(LoginId, LogType.Waiver, GetName() + " Signed Waiver.");
            return true;
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
