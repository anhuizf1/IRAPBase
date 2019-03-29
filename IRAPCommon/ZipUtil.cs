using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Security;
using System.Security.Cryptography;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Runtime.InteropServices;

namespace IRAPCommon
{
    public class ZipUtil
    {
        public static string Zip(string value)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(value);
            byte[] tmpArray;

            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream sw = new GZipStream(ms, CompressionMode.Compress))
                {
                    sw.Write(byteArray, 0, byteArray.Length);
                    sw.Flush();
                }
                tmpArray = ms.ToArray();
            }
            return Convert.ToBase64String(tmpArray);
        }

        public static string UnZip(string value)
        {
            /* .net 4.0 以上版本支持
             * byte[] byteArray = Convert.FromBase64String(value);
             byte[] tmpArray;

             using (MemoryStream msOut = new MemoryStream())
             {
                 using (MemoryStream msIn = new MemoryStream(byteArray))
                 {
                     using (GZipStream swZip = new GZipStream(msIn, CompressionMode.Decompress))
                     {
                         swZip.CopyTo(msOut);
                         tmpArray = msOut.ToArray();
                     }
                 }
             }
             return Encoding.UTF8.GetString(tmpArray);*/

            byte[] byteArray = Convert.FromBase64String(value);
            using (GZipStream stream = new GZipStream(new MemoryStream(byteArray), CompressionMode.Decompress))
            {
                const int size = 512;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }

                    } while (count > 0);

                    byte[] plainData = memory.ToArray();

                    return System.Text.Encoding.UTF8.GetString(plainData);
                };

            }

        }



    }

}
