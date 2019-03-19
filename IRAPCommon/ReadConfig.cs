using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IRAPCommon
{

    /// <summary>
    /// 读取WebAPI服务的ServiceDlls目录下的IRAPORM.xml作为配置文件
    /// 因为程序集热加载目前还没办法读取自己的配置文件（如果后续发现可以修改）
    /// </summary>
    public class DllReadConfig
    {
        private static Dictionary<string, string> _paramList = new Dictionary<string, string>();
        public  DllReadConfig()
        {
            try
            {
                if (_paramList.Count == 0)
                {
                    XmlDocument _xmlDoc = new XmlDocument();
                    string _assetXMLPath = AppDomain.CurrentDomain.BaseDirectory + @"ServiceDlls\IRAPORM.xml";

                    if (!File.Exists(_assetXMLPath))
                    {
                        foreach (string item in ConfigurationManager.AppSettings.Keys)
                        {
                            _paramList.Add(item, ConfigurationManager.AppSettings[item].ToString());
                        }
                        return;
                    }
                    else
                    {
                        _xmlDoc.Load(_assetXMLPath);
                    }


                    XmlElement xmlContent = _xmlDoc.DocumentElement;

                    XmlNode rowNode = xmlContent.SelectSingleNode("/configuration/appSettings");

                    foreach (XmlNode node in rowNode.ChildNodes)
                    {
                        if (node.NodeType == XmlNodeType.Element)
                        {
                            _paramList.Add(node.Attributes["key"].Value.ToString(), node.Attributes["value"].Value.ToString());
                            // Log.WriteLocalMsg(2, node.Attributes["key"].Value + " " + node.Attributes["value"].Value.ToString());
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        public   Dictionary<string, string> Parameters { get { return _paramList; } }
    }
}
