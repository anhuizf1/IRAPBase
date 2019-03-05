using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IRAPCommon
{
    public class ReadConfig
    {
        private static Dictionary<string, string> _paramList = new Dictionary<string, string>();
        public static void Read()
        {
            if (_paramList.Count == 0)
            {
                XmlDocument _xmlDoc = new XmlDocument();
                string _assetXMLPath = AppDomain.CurrentDomain.BaseDirectory + @"ServiceDlls\IRAPORM.xml";

                _xmlDoc.Load(_assetXMLPath);

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

        public static Dictionary<string, string> Parameters { get { return _paramList; } }
    }
}
