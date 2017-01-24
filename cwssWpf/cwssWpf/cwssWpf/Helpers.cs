using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
