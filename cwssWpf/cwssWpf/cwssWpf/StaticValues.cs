using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cwssWpf
{
    public static class StaticValues
    {
        // Default Admin Account
        public static int DefaultAdminId = 123456;
        public static string DefaultAdminPassword = "abc123";

        // Account Ranges
        public static int NonStudentIdLength = 6;
        public static int StudentIdLength = 7;
        public static int StartNonStudentIdNumber = 100000;
        public static int EndNonStudentIdNumber = 999999;
        public static int StartStudentIdNumber = 1000000;
        public static int EndStudentIdNumber = 9999999;

        // Remote Communication Settings
        public static string RemoteIP = "239.0.0.222";
        public static string RemotePort = "2222";

        // CardReader Options
        public static char CardReaderStartChar = ';';
        public static int CardReaderStartIndexOfID = 11;
    }
}
