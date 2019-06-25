using IRAPBase.Enums;
using Logrila.Logging;
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
        private static Log thisLog = null;

        private static object lockObj = new object();
        /// <summary>
        /// 写本地文件日志
        /// </summary>
        /// <param name="threadID"></param>
        /// <param name="ErrText"></param>
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

        /// <summary>
        /// 静态方法获取实例
        /// </summary>
        /// <returns></returns>
        public static Log InstanceID
        {
            get
            {
                if (thisLog == null)
                {
                    thisLog = new Log();
                }
                return thisLog;
            }

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public Log()
        {
            Logrila.Logging.NLogIntegration.NLogLogger.Use();
        }
        /// <summary>
        /// 通过NLog写本地文件日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <param name="logType"></param>
        public void WriteMsg<T>(LogType logType, string msg) where T : class
        {

            Logrila.Logging.ILog _log = Logger.Get<T>();
            switch (logType)
            {
                case LogType.DEBUG:
                    _log.Debug(msg);
                    break;
                case LogType.ERROR:
                    _log.Error(msg);
                    break;
                case LogType.WARN:
                    _log.Warn(msg);
                    break;
                case LogType.INFO:
                    _log.Info(msg);
                    break;
                case LogType.FATAL:
                    _log.Fatal(msg);
                    break;
                default:
                    _log.Info(msg);
                    break;
            }
        }

        public void WriteMsg<T>(LogType logType, object msg) where T : class
        {

            Logrila.Logging.ILog _log = Logger.Get<T>();
            switch (logType)
            {
                case LogType.DEBUG:
                    _log.Debug(msg);
                    break;
                case LogType.ERROR:
                    _log.Error(msg);
                    break;
                case LogType.WARN:
                    _log.Warn(msg);
                    break;
                case LogType.INFO:
                    _log.Info(msg);
                    break;
                case LogType.FATAL:
                    _log.Fatal(msg);
                    break;
                default:
                    _log.Info(msg);
                    break;
            }
        }

        /// <summary>
        /// 写日志可跟踪异常位置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logType"></param>
        /// <param name="msg"></param>
        /// <param name="err"></param>
        public void WriteMsg<T>(LogType logType, object msg, Exception err) where T : class
        {
            Logrila.Logging.ILog _log = Logger.Get<T>();
            switch (logType)
            {
                case LogType.DEBUG:
                    _log.Debug(msg, err);
                    break;
                case LogType.ERROR:
                    _log.Error(msg, err);
                    break;
                case LogType.WARN:
                    _log.Warn(msg, err);
                    break;
                case LogType.INFO:
                    _log.Info(msg, err);
                    break;
                case LogType.FATAL:
                    _log.Fatal(msg, err);
                    break;
                default:
                    _log.Info(msg, err);
                    break;
            }
        }


    }



}
