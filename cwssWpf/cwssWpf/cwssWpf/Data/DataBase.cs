using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cwssWpf.Data
{
    // Used at program load and close to save entire database
    // -Temp file/s can be written to for storing individual events in case of crash (have a flag)
    // --ex. everytime user checks in, checks out, checks in item, checks out item
    // -In event of crash, at program load, add temp logs to database, then delete temp logs
    // -Large events can resave the entire database, ex. new user created, waiver submitted, event created

    // Possible Expansion
    // -keep multiple database files as they are written as backups

    public static class DataBase
    {
        public static BaseData Data = new BaseData();

        private static string SavePath = @"C:\test";
        private static string passPhrase = "70392DE7-5FB4-4520-A9A6-3CD231E181C0";
        private static string saltValue = "09290C9F-1B71-4A0E-92C9-51E03E6868D3";

        public static bool Load()
        {
            var success = loadFromFile();

            // replace true with tempLog flag
            if (true)
            {
                //Do Add Temp Log Stuff
            }

            return success;
        }

        public static bool Save()
        {
            // replace true with tempLog flag
            if (true)
            {
                //Do Add Temp Log Stuff
            }

            var success = saveToFile();

            return success;
        }

        private static bool loadFromFile()
        {
            // If File Exists
            var data = File.ReadAllText(SavePath + @"\CwssDataBase.json");
            RijndaelEncryptDecrypt.EncryptDecryptUtils.Decrypt(data, passPhrase, saltValue, "SHA1");
            Data = JsonConvert.DeserializeObject<BaseData>(data);

            return true;
        }

        private static bool saveToFile()
        {
            var data = JsonConvert.SerializeObject(Data);
            RijndaelEncryptDecrypt.EncryptDecryptUtils.Encrypt(data, passPhrase, saltValue, "SHA1");
            File.WriteAllText(SavePath + @"\CwssDataBase.json", data);

            return true;
        }
    }
}
