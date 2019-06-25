using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPCommon
{
    /// <summary>
    /// IRAP字符串转换类
    /// </summary>
    public class IRAPStringParser
    {

        
        /// <summary>
        /// 从字符串转List
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>长整型清单</returns>
        public static List<long> ScopeToList(string str)
        {
            var resList = new List<long>();
            string[] array = str.Split(new string[] { "," }, StringSplitOptions.None);
            foreach (var r in array)
            {
                if (r.IndexOf("-", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    string[] subArray = r.Split('-');
                    long startIndex = long.Parse(subArray[0]);
                    long endIndex = long.Parse(subArray[1]);
                    for (long i = startIndex; i<=endIndex; i++) {
                        resList.Add(i);
                    }
                }
                else
                {
                    resList.Add(long.Parse(r));
                }
            }
            return resList;
        }

        /// <summary>
        /// 字符串转清单并去重复
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="delimiter">分隔符</param>
        /// <returns></returns>
        public static  List<string> StringToList( string str, string delimiter)
        {
          var resList = new List<String>();
          string [] array=  str.Split(new string[] { delimiter },StringSplitOptions.None);
            foreach (string r in array)
            {
                if( !resList.Contains(r))
                {
                    resList.Add(r);
                }
            }
            return resList;
        }



    }
}
