using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IRAPCommon
{

    public class IRAPUtil
    {
        public static string Post(string HTTPURL, string ExChangeString)
        {
            string fullURL = HTTPURL;
            string resJson = string.Empty;
            try
            {
                System.Net.ServicePointManager.DefaultConnectionLimit = 1024;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullURL);
                request.Method = "POST";
                request.Timeout = 40000;
                request.KeepAlive = false;
                // request.ServicePoint.Expect100Continue = false;
                //是否使用 Nagle 不使用 提高效率   
                // request.ServicePoint.UseNagleAlgorithm = false;
                //最大连接数   
                //  request.ServicePoint.ConnectionLimit = 32767;
                //数据是否缓冲 false 提高效率    
                //  request.AllowWriteStreamBuffering = false;
                //不用代理
                request.Proxy = null;
                request.ContentType = "application/json;charset=UTF-8";
                // request.ContentType = "application/octet-stream;";
                var stream = request.GetRequestStream();
                using (var writer = new StreamWriter(stream))
                {

                    writer.Write(ExChangeString);
                    writer.Flush();
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                // string resJson = getResponseString(response);
                try
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(),
                        System.Text.Encoding.UTF8))
                    {

                        resJson = reader.ReadToEnd();
                    }
                    return resJson;
                }
                finally
                {

                    if (request != null)
                    {
                        request.Abort();
                    }
                    response.Close();
                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }
    }
}
