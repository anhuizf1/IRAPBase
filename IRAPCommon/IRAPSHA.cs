using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IRAPCommon
{
    public class IRAPMD5Calc
    {
        static MD5 md5 = new MD5CryptoServiceProvider();
        public static string GetMD5String(string PlainText, Boolean isBase64 = false)
        {
            byte[] result = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(PlainText));

            if (isBase64)
            {
                return Convert.ToBase64String(result);
            }
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < result.Length; i++)
            {
                sBuilder.Append(result[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
        public static byte[] GetMD5Bytes(string PlainText)
        {
            return System.Text.Encoding.Unicode.GetBytes(GetMD5String(PlainText));
        }

    }

    public class IRAPSHA
    {
        static SHA512 sha512 = new SHA512CryptoServiceProvider();
        public static string GetSHA512String(String plainText, Boolean isBase64)
        {
            if (isBase64)
            {
                string resultSha512 = Convert.ToBase64String(sha512.ComputeHash(Encoding.Default.GetBytes(plainText)));
                return resultSha512;
            }
            else
            {
                byte[] result = sha512.ComputeHash(Encoding.Default.GetBytes(plainText));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < result.Length; i++)
                {
                    sBuilder.Append(result[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }

    }
}
