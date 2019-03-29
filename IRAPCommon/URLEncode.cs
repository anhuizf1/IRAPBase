using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace IRAPCommon
{
    public class URLEncode
    {
        /// <summary>
        /// 对URL进行编码
        /// </summary>
        /// <param name="srcText"></param>
        /// <returns></returns>
        public static string UrlEncode(string srcText)
        {
            return HttpUtility.UrlEncode(srcText);
        }
      
    }
}
