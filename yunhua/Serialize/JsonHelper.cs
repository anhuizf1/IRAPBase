using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Web.Script.Serialization;

namespace IRAPBase.Serialize
{
    public static class JsonHelper
    {

          /// <summary>
        /// JSON序列化
        /// </summary>
        /// <param name="obj">源对象</param>
        /// <returns>json数据格式</returns>
        public static string ToJson2(this object obj)
        {
            // JavaScriptSerializer serialize = new JavaScriptSerializer();
            //return serialize.Serialize(obj);

            //使用第三方库
             return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 将字符串数组转换为json数据格式:["value1","value2",...]
        /// </summary>
        /// <param name="strs">字符串数组</param>
        /// <returns>json数据格式</returns>
        public static string ToJson(this string[] strs)
        {
            return ToJson2((object)strs);
        }

        /// <summary>
        /// 将DataTable数据源转换为json数据格式:[{"ColumnName":"ColumnValue",...},{"ColumnName":"ColumnValue",...},...]
        /// </summary>
        /// <param name="dt">DataTable数据源</param>
        /// <returns>json数据格式</returns>
        public static string ToJson(this DataTable dt)
        {
           //使用第三方库
           return JsonConvert.SerializeObject(dt, new DataTableConverter());
            /*
            List<object> list = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    dic.Add(dc.ColumnName, dr[dc].ToString());
                }
                list.Add(dic);
            }
            return ToJson(list);*/
        }

        /// <summary>
        /// 将"\/Date(673286400000)\/"Json时间格式替换"yyyy-MM-dd HH:mm:ss"格式的字符串
        /// </summary>
        /// <param name="jsonDateTimeString">"\/Date(673286400000)\/"Json时间格式</param>
        /// <returns></returns>
        public static string ConvertToDateTimeString(this string jsonDateTimeString)
        {
            string result = string.Empty;
            string p = @"\\/Date\((\d+)\)\\/";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
            Regex reg = new Regex(p);
            result = reg.Replace(jsonDateTimeString, matchEvaluator);
            return  result;
        }

        public static string ConvertJsonDateToDateString(Match match)
        {
            string result = string.Empty;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
            dt = dt.ToLocalTime();
            result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }

        //根据简单的Json字符串返回动态类型
        public static dynamic GetSimpleObjectFromJson(this string json)
        {
            dynamic d = new ExpandoObject();
            // 将JSON字符串反序列化
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = int.MaxValue;
            object resobj = s.DeserializeObject(json);
            // 拷贝数据
            IDictionary<string, object> dic = (IDictionary<string, object>)resobj;
            IDictionary<string, object> dicdyn = (IDictionary<string, object>)d;

            foreach (var item in dic)
            {
                dicdyn.Add(item.Key, item.Value);
            }
            return d;
        }

        //把Json字符串反序列化为IList<T>对象
        public static IList<T> GetListFromJson<T> (this string json){

            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483647;
            IList<T> list=  s.Deserialize<IList<T>>(json);
            return list;
        }
        //把Json字符串反序列化为T对象 
        public static T GetObjectFromJson<T>(this string json)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = int.MaxValue;
            T obj = s.Deserialize<T>(json);
            return obj;
        }

        //把Get参数key1=value1&key2=value2&key3=value3用字典的形式返回
        public static Dictionary<string, string> ToDict(this string json)
        {
            Dictionary<string, string> keys = new Dictionary<string, string>();
            try
            {
                string[] inparam = json.Split('&');
                foreach (string item in inparam)
                {
                    string[] keyvalue = item.Split('=');
                    keys.Add(keyvalue[0], keyvalue[1]);
                };
                return keys;
            }
            catch (Exception)
            {
                return keys;
            }
        }
        //把Json Array字符串转换为对象
        public static List<Dictionary<string, object>> ToRows(this string json)
        {

            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            JavaScriptSerializer s = new JavaScriptSerializer();
            object[] resobj = (object[])s.DeserializeObject(json);
            // 拷贝数据


            foreach (Dictionary<string, object> item in resobj)
            {
                rows.Add(item);
            }
            return rows;
        }
    }


}
