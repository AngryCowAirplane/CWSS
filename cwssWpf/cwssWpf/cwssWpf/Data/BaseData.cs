using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cwssWpf.Data
{
    public static class BaseData
    {
        public static DataObject AppData = new DataObject();
        public static string SavePath = @"C:\test";

        public static bool LoadData()
        {
            return true;
        }

        public static bool SaveData()
        {
            return true;
        }

        public static bool LoadFromBackup()
        {
            var data = File.ReadAllText(SavePath + @"\CwssDataBase.json");
            AppData = JsonConvert.DeserializeObject<DataObject>(data);

            return true;
        }

        public static bool SaveToBackup()
        {
            var data = JsonConvert.SerializeObject(AppData);
            File.WriteAllText(SavePath + @"\CwssDataBase.json", data);

            return true;
        }
    }
}
