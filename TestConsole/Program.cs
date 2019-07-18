using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IRAPBase;
using IRAPBase.DTO;
using IRAPBase.Entities;
using IRAPMES;
using IRAPMES.Entities;
using IRAPBase.Serialize;
using TestConsole.Entities;
using System.IO;
 
using IRAPCommon;
using Logrila.Logging;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Logrila.Logging.NLogIntegration.NLogLogger.Use();
            Logrila.Logging.ILog _log = Logger.Get<Program>();

            _log.Warn("测试!");

            var db= DBContextFactory.Instance.CreateContext("IRAPContext");

           
            // var list= db.Set<ETreeBizClass>().Where(c => c.LeafID == 2361480).ToList();

            //  IRAPTreeModelSet treemodelSet = new IRAPTreeModelSet();
            //  treemodelSet.CreateATree(206, "保养周期目录树", 1000, 5, true, 2, "", "", 1, "", "", true, false, false, false, 1);

            //throw new Exception("Error!");

            //  var list2 = from a in list group a by a.LeafID into g select new { g.Key,items=g };


            //foreach (var r in list)
            //{
            //    Console.WriteLine("{0} {1}", r.TreeID,r.Leaf01);
            //}
            Console.WriteLine("saved.");
            Console.ReadKey();
        }

        static void PrintTree(IRAPTreeNodes rootNode, ref int i)
        {
            if (rootNode.Children == null)
            {
                //Console.WriteLine(" -- 叶子：{0}  {1}-{2} Father：{3}  accessible:{4}", rootNode.NodeDepth, rootNode.NodeID, 
                //    rootNode.NodeName, rootNode.Parent, rootNode.Accessibility);
                return;
            }
            if (rootNode.FatherNode == null)
                Console.WriteLine("{0}[{1}]", rootNode.NodeName, rootNode.NodeID);
            foreach (var r in rootNode.Children)
            {
                i++;
                Console.WriteLine(new string('-', r.NodeDepth * 2) + (r.NodeID < 0 ? "-" : "+") + "{0}[{1}] ", r.NodeName, r.NodeID);
                PrintTree(r, ref i);
            }

        }

        public static void PrintProperties(Object obj)
        {
            Type type = obj.GetType();
            foreach (PropertyInfo p in type.GetProperties())
            {
                Console.Write("{0}={1} ", p.Name, p.GetValue(obj));
            }
            Console.Write("\n");
        }


        static void AnalyzeResult(string fileName, out string barcode, out string conclusion)
        {
            // string tempfile = "AN3028111S1-Addfddd-NG.xls";
            string[] array = fileName.Split('-');
            var barcodeList = new StringBuilder();
            for (int i = 0; i < array.Length - 1; i++)
            {
                barcodeList.Append(array[i] + "-");
            }

            conclusion = array[array.Length - 1].Split('.')[0];
            barcode = barcodeList.ToString().Substring(0, barcodeList.ToString().Length - 1);

        }


        public static void Test()
        {
            IRAPWorkbench biz = new IRAPWorkbench();
            try
            {
                string inParam = File.ReadAllText(@"C:\temp\22.TXT");
                biz.UsingContext("IRAPMDM");
                dynamic dn = biz.GetObjectFromJson(inParam);
                int fatherNode = int.Parse(dn.NodeID.ToString());
                string nodeName = dn.NodeName.ToString();
                int communityID = int.Parse(dn.CommunityID.ToString());


                Console.WriteLine("success.");
                //biz.BackResult.ErrCode = 0;
                //biz.BackResult.ErrText = "添加成功！";
                //return biz.ToJson();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                //biz.BackResult.ErrCode = 9999;
                //biz.BackResult.ErrText = $"出现错误：{err.Message}";
                //return biz.ToJson();
            }

        }
    }


    public class TreeDimDTO
    {
        public byte Index { get; set; }
        public Int16 TreeID { get; set; }
    }
}
