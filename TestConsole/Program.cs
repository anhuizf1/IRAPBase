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

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            IRAPTree tree = new IRAPTree(60006,102, -64128);
            Console.WriteLine(tree.ClassEntity);
            Console.WriteLine(tree.LeafEntity);
            Console.WriteLine(tree.NameSpace);
            Console.WriteLine(tree.TransientEntity);
            Console.WriteLine(tree.Status);


            string tempfile = "AN3028111S1-Addfddd-NG.xls";
            string barcode;
            string conclusion;
            AnalyzeResult(tempfile, out barcode, out conclusion);
            Console.WriteLine(barcode);
            Console.WriteLine(conclusion);

            IRAPTreeSet treeSet = new IRAPTreeSet(57280,1);
            // NewTreeNodeDTO dto=  treeSet.NewNode(5615111, "Create", "创新研究所", 1.1F);
            NewTreeNodeDTO dto = treeSet.NewLeaf(14263, "test", "alter", "中台开发人员");
            Console.WriteLine(dto.ErrCode);
            Console.WriteLine(dto.ErrText);
            Console.WriteLine(dto.NewNodeID);
            // Repository<IRAPUserEntity> rep = new Repository<IRAPUserEntity>("IRAPContext");
            // var us=  rep.Table.Where(r=>r.UserCode=="Admin").ToList();

            // foreach (var r in us)
            //  {
            //      Console.WriteLine("{0} {1}", r.UserCode,r.UserName);
            //  }
            // Console.ReadKey();
            //  IRAPUser irapUser = new IRAPUser("Admin", 57280);

            // 测试新增用户
            //IRAPError error = irapUser.AddUser("77777", "测试0214", "123", "-1,-4", "-2");

            // Console.WriteLine("{0} {1}", error.ErrCode, error.ErrText);

            //测试获取机构
            // BackLeafSetDTO list = irapUser.GetAgencyList();
            //foreach (LeafDTO item in list.Rows)
            // {
            //     Console.WriteLine("{0} {1} {2}", item.Leaf, item.Code, item.Name);
            // }

            //测试登录
            //  BackLoginInfo backlogin = irapUser.Login("PCWeb", "123", "", "00161701F6BE", "192.168.57.216", 1, 2);
            // Console.WriteLine("{0},{1} {2} {3} {4}", backlogin.ErrCode, backlogin.ErrText, backlogin.SysLogID, backlogin.access_token, backlogin.AgencyName);


            // Console.ReadKey();

            /*
           


            BackLeafSetDTO list2 = irapUser.GetRoleList();
            Console.WriteLine("{0} {1}", list2.ErrCode, list2.ErrText);
            foreach (LeafDTO item in list2.Rows)
            {
                Console.WriteLine("{0} {1} {2}", item.Leaf, item.Code, item.Name);
            }

            //IRAPError error= irapUser.AddUser("Admin2","张峰","123","-1","-2");
            //Console.WriteLine("{0} {1}", error.ErrCode, error.ErrText);

            IRAPError error3 = irapUser.VerifyPWD("123");
            Console.WriteLine("{0} {1}", error3.ErrCode, error3.ErrText);

            //IRAPStation station = new IRAPStation();
            // IRAPError list3 = station.HasStation("00161701F6BE");
            //Console.WriteLine("{0},{1}", list3.ErrCode, list3.ErrText);

            BackLoginInfo backlogin =   irapUser.Login("PCWeb","123", "", "00161701F6BE", "192.168.57.216", 1, 2);
            Console.WriteLine("{0},{1} {2} {3} {4}", backlogin.ErrCode, backlogin.ErrText, backlogin.SysLogID, backlogin.access_token,backlogin.AgencyName);
          
            //IRAPError error2 = irapUser.DeleteUser(9999, "Admin");
            //irapUser.User.EncryptedPWD = IRAPUser.GetBinaryPassword("123");
            // IRAPError error2 = irapUser.ModifyUser(irapUser.User);
            // Console.WriteLine("{0},{1}", error2.ErrCode, error2.ErrText);

            //UnitOfWork work = new UnitOfWork(new IRAPSqlDBContext("name=IRAPMDMContext"));
            //Repository<ETreeBizTran> dd=  work.Repository<ETreeBizTran>();
            //foreach (ETreeBizTran row in dd.Table.ToList())
            //{
            //    Console.WriteLine("{0} {1}", row.Code,row.EntityID);
            //}

            //TestMES test = new TestMES();
            //  test.Test();
            //IRAPGrant grant = new IRAPGrant(57280);
            //List<EGrant> list=    grant.GetGrantListByTree(1, -1, -2);

            //foreach( var r in list)
            //{
            //    PrintProperties(r);
            //}
 */

            //var userlist = TestUserBiz.Users();

            //foreach(var r in userlist.Table)
            //{
            //    Console.WriteLine("{0} {1}", r.UserCode, r.UserName);
            //}
            //var e = userlist.Entities.Find("ning.du");
            //userlist.Delete(e);
            //Console.WriteLine("删除成功！");
            //userlist.SaveChanges();


            // IRAPTreeSet tree = new IRAPTreeSet(57280, 3);
            // IRAPTreeNodes treeNodes = tree.AccessibleTreeView(-1, -2);
            //// IRAPTreeNodes treeNodes = tree.TreeView();

            //int i = 0;
            //Console.WriteLine("打印的树如下：");
            //PrintTree(treeNodes, ref i);
            //Console.WriteLine("总数：{0}", i);

            Console.ReadKey();
            
        }

        static void PrintTree(IRAPTreeNodes rootNode,ref int i)
        {
            
            if (rootNode.Children==null)
            {
                //Console.WriteLine(" -- 叶子：{0}  {1}-{2} Father：{3}  accessible:{4}", rootNode.NodeDepth, rootNode.NodeID, 
                //    rootNode.NodeName, rootNode.Parent, rootNode.Accessibility);
                return;
            }
            if (rootNode.FatherNode==null)
            Console.WriteLine("{0}[{1}]", rootNode.NodeName, rootNode.NodeID);
            foreach (var r in rootNode.Children)
            {
                i++;
                Console.WriteLine(new string('-', r.NodeDepth*2) + (r.NodeID<0?"-":"+" )+"{0}[{1}] ", r.NodeName, r.NodeID);
                PrintTree(r,ref i);
            }

        }

        public static void PrintProperties(Object obj)
        {
            Type type = obj.GetType();
            foreach (PropertyInfo p in type.GetProperties())
            {
                Console.Write("{0}={1} ",  p.Name,  p.GetValue(obj));
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
    }
}
