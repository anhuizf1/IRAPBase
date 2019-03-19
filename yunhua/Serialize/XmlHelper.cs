using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace IRAPBase.Serialize
{
    public static class XmlHelper
    {
        public static string ToXml(this object obj)
        {
            // JavaScriptSerializer serialize = new JavaScriptSerializer();
            //return serialize.Serialize(obj);


            //执行序列化并将序列化结果输出到控制台
            XmlRootAttribute root = new XmlRootAttribute("Result");
            XmlSerializer xml = new XmlSerializer(obj.GetType(), root);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true; settings.IndentChars = " ";
            settings.NewLineChars = "\r\n";
            settings.Encoding = Encoding.UTF8;
            settings.OmitXmlDeclaration = true; // 不生成声明头 
            string str = string.Empty;
            StringBuilder sb = new StringBuilder();
            XmlSerializerNamespaces n = new XmlSerializerNamespaces();
            n.Add(string.Empty, string.Empty);
            using (XmlWriter xmlWriter = XmlWriter.Create(sb, settings))
            {
                //序列化对象  
                xml.Serialize(xmlWriter, obj, n);
                // Stream.Position = 0;
                xmlWriter.Close();
            }
            return sb.ToString();
        }


        public static T GetObjectFromXml<T>(this string obj)
        {

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(obj));
            //执行序列化并将序列化结果输出到控制台
            using (System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8))
            {
                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
                T ret = (T)xs.Deserialize(reader);
                return ret;
            }
        }


        //把XML 返回结果变成类型
       
        public  static Dictionary<string, string> XmlToDict(  string resXml,   out DataTable resTable,
           out int errCode, out string errText)
        {

            Dictionary<string, string> outParamList = new Dictionary<string, string>();
            errCode = 0;
            errText = "转换完成！";
            resTable = new DataTable("ResTable");
            
            try
            {
                

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(resXml);
                //使用xpath表达式选择文档中所有的student子节点
                XmlNodeList nodeList = xmlDoc.SelectSingleNode("Result").ChildNodes;

                if (nodeList != null)
                {
                    foreach (XmlNode childNode in nodeList)
                    {
                        XmlElement childElement = (XmlElement)childNode;
                        if (childElement.Name == "Param")
                        {

                            foreach (XmlAttribute item in childElement.Attributes)
                            {
                                outParamList.Add(item.Name, item.Value);

                            }
                            errCode = int.Parse(childElement.Attributes["ErrCode"].Value.ToString());
                            errText = childElement.Attributes["ErrText"].Value.ToString();
                        }

                        if (childElement.Name == "ParamXML")
                        {
                            XmlNodeList datalist = childElement.ChildNodes;

                            XmlNode node = childElement.FirstChild;
                            //创建列
                            if (node != null)
                            {
                                foreach (XmlAttribute item in node.Attributes)
                                {
                                    DataColumn rowXmlColumn = new DataColumn();
                                    rowXmlColumn.DataType = System.Type.GetType("System.String");
                                    rowXmlColumn.ColumnName = item.Name;
                                    resTable.Columns.Add(rowXmlColumn);
                                }
                            }

                            foreach (XmlNode dataNode in datalist)
                            {
                                XmlElement data = (XmlElement)dataNode;
                                DataRow newRow;
                                newRow = resTable.NewRow();
                                foreach (XmlAttribute item in dataNode.Attributes)
                                {
                                    newRow[item.Name] = item.Value;
                                }
                                resTable.Rows.Add(newRow);
                            }
                        }
                    }

                }
                else
                {
                    errCode = 999999;
                    errText = "输入的XML不合法没有Result根元素！";
                }
            }
            catch (Exception ex)
            {
                errCode = 999999;
                errText = "输入的XML格式错误：" + ex.ToString();
            }

            return outParamList;
        }

        //把Table变成List<T>
        public static List<T> ToList<T>(DataTable dt)
        {
            var list = new List<T>();
            Type t = typeof(T);
            try
            {

                var plist = new List<PropertyInfo>(typeof(T).GetProperties());


                foreach (DataRow item in dt.Rows)
                {
                    T s = System.Activator.CreateInstance<T>();
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        PropertyInfo info = plist.Find(p => p.Name.ToLower() == dt.Columns[i].ColumnName.ToLower());
                        Type tType = info.PropertyType;

                        if (info != null)
                        {
                            if (!Convert.IsDBNull(item[i]))
                            {
                                object obj = Convert.ChangeType(item[i], tType);
                                info.SetValue(s, obj, null);
                            }
                        }
                    }
                    list.Add(s);
                }
            }
            catch (Exception err)
            {
               // IRAPSQLConnection.WriteLocalMsg(t.FullName.ToString() + "转换出错：" + err.Message, MsgType.error);
                throw;
            }
            return list;
        }
    }
}
