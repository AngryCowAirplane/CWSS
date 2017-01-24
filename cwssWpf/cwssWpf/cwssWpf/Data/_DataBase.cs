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

    public static class _DataBase
    {
        public static _BaseDataObject Data = new _BaseDataObject();
        private static string dbPath;

        public static bool Load(string path)
        {
            dbPath = path;
            var success = loadFromFile();

            // TODO:
            // replace true with tempLog flag
            if (true)
            {
                // TODO:
                // Add Temp Log Stuff
            }

            return success;
        }

        public static bool Save(string path = "")
        {
            // TODO:
            // replace true with tempLog flag
            if (true)
            {
                // TODO:
                // Add Temp Log Stuff
            }

            var success = saveToFile(path);

            return success;
        }

        private static bool loadFromFile(string path = "")
        {
            if (string.IsNullOrEmpty(path))
                path = dbPath;
            if(File.Exists(path))
            {
                var data = File.ReadAllText(dbPath);
                var decryptedData = Helpers.DecryptString(data);
                Data = JsonConvert.DeserializeObject<_BaseDataObject>(decryptedData);
            }

            return true;
        }

        private static bool saveToFile(string path = "")
        {
            if (string.IsNullOrEmpty(path))
                path = dbPath;
            var data = JsonConvert.SerializeObject(Data);
            var encryptedData = Helpers.EncryptString(data);
            File.WriteAllText(path, encryptedData);

            return true;
        }
    }
}
