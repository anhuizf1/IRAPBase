using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase
{
    public class Log
    {
        private static object lockObj = new object();

        public static void WriteLocalMsg(int threadID, string ErrText)
        {
            //Console.WriteLine(DateTime.Now.ToString() + ":" + ErrText);
            string FilePath = AppDomain.CurrentDomain.BaseDirectory + DateTime.Now.ToString("yyyyMM") + "\\";
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }

            lock (lockObj)
            {
                FilePath += DateTime.Now.ToString("yyyy-MM-dd") + "_" + threadID.ToString() + ".log";
                StreamWriter sw = null;
                try
                {
                    if (!File.Exists(FilePath))
                    {
                        sw = File.CreateText(FilePath);
                    }
                    else
                    {
                        sw = File.AppendText(FilePath);
                    }
                    sw.WriteLine(DateTime.Now.ToString() + ":" + ErrText);
                }
                catch
                {; }
                finally
                {
                    if (sw != null)
                    {
                        sw.Close();
                    }
                }
            }
        }
    }
}
