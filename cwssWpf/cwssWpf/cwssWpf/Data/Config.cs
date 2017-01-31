﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cwssWpf.Data
{
    public static class Config
    {
        public static ConfigData Data;
        private static string ConfigPath;

        public static void Initialize()
        {
            ConfigPath = System.IO.Path.Combine(Environment.CurrentDirectory, "AppData", @"Config.cwcfg");
            Data = new ConfigData();
            loadConfigFromFile();
        }

        private static void loadConfigFromFile()
        {
            if (File.Exists(ConfigPath))
            {
                var encryptedString = File.ReadAllText(ConfigPath);
                var configString = Helpers.DecryptString(encryptedString);
                Data = JsonConvert.DeserializeObject<ConfigData>(configString);
            }
            else
                Data = new ConfigData();
        }
       
        public static void SaveConfigToFile()
        {
            var configString = JsonConvert.SerializeObject(Data);
            var encryptedConfig = Helpers.EncryptString(configString);
            File.WriteAllText(ConfigPath, encryptedConfig);
            Logger.Log(MainWindow.CurrentUser.GetUserId(), LogType.Settings, MainWindow.CurrentUser.GetName() + " Saved/Changed Settings File.");
        }
    }

    public class ConfigData
    {
        public EmailSettings Email { get; set; }

        public ConfigData()
        {
            Email = new EmailSettings();
        }
    }

    public class EmailSettings
    {
        public EmailClient Client = EmailClient.RemoteClient;
        public string SmtpServer = "smtp.live.com";
        public int SmtpPort = 587;
        public bool UseDefaultCredentials = false;
        public bool EnableSsl = true;
        public string EmailAddress = "";
        public string Password = "";
        public bool StoreCreds = false;
    }

    public enum EmailClient
    {
        LocalClient = 0,
        RemoteClient = 1
    }
}