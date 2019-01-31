using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IRAPCommon
{
  

    public class IRAPMD5
    {

        public static string MD5(string srcText)
        {

            byte[] result = Encoding.Default.GetBytes(srcText);    //tbPass为输入密码的文本框
            MD5 computMd5 = new MD5CryptoServiceProvider();

            byte[] output = computMd5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "").ToLower();  //tbMd5pass为输出加密文本的文本框
        }
    }
}
