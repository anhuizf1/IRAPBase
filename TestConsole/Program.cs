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
            TreeLeafEntity a ;

            a= new ETreeBizLeaf();

            Console.WriteLine(a.GetType().Name);

            //Console.ReadKey();
            string tempfile = "AN3028111S1-Addfddd-NG.xls";
            string barcode;
            string conclusion;
            AnalyzeResult(tempfile, out barcode, out conclusion);
            Console.WriteLine(barcode);
            Console.WriteLine(conclusion);

            //IRAPTreeSet treeSet = new IRAPTreeSet(57280,1);
            //// NewTreeNodeDTO dto=  treeSet.NewNode(5615111, "Create", "创新研究所", 1.1F);
            //NewTreeNodeDTO dto = treeSet.NewLeaf(14263, "test", "alter", "中台开发人员");
            //Console.WriteLine(dto.ErrCode);
            //Console.WriteLine(dto.ErrText);
            //Console.WriteLine(dto.NewNodeID);

            //IRAPSystem system = new IRAPSystem();
         
            //var list= system.GetSystemList();
            //foreach (var r in list)
            //{
            //    Console.WriteLine("{0} - {1}", r.SystemID,r.SystemName);
            //}
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


           IRAPTreeSet tree2 = new IRAPTreeSet(57280, 1);
            // IRAPTreeNodes treeNodes = tree.AccessibleTreeView(-1, -2);
            int errCode;
            string errText;
            //List<TreeViewDTO> treeNodes = tree2.TreeView(out errCode, out errText,5171, false);
            int i=0;
            // PrintTree(treeNodes, ref i);
            //Console.WriteLine("总数：{0}", treeNodes.Count);
           
            List<TreeViewDTO> treeList = tree2.TreeView(out errCode, out errText);

            foreach (TreeViewDTO r in treeList)
            {
                Console.WriteLine(new string('-', r.NodeDepth * 2) + (r.NodeID < 0 ? "-" : "+") + "{2}-{0}[{1}]", r.NodeID, r.NodeCode, r.NodeName);
            }
           // IRAPTree tree3 = new IRAPTree(57280, 1, 1);
           // var list3 = tree3.GetRowSet(1).Cast<RowSet_T1R1>();
 
          
            // list3.AsQueryable<RowSet_T1R1>();
            //var query = from p in list3.AsQueryable<BaseRowAttrEntity>().Where(c=>c.EntityID==1).Select(c=>c.EntityID);
            //foreach (var r in list3)
            //{
            //    Console.WriteLine("{0} {1}",r.EntityID,r.T2LeafID);
            //}
            //Dictionary<int, TreeClassEntity> dict = new Dictionary<int, TreeClassEntity>();
            //dict.Add( 1,new ETreeSysClass { LeafID=1, Ordinal=1, PartitioningKey=0, DimLeaf=199, TreeID=2 } );


            IRAPBizTest testbiz = new IRAPBizTest();


            //IRAPError error= treebase.SaveRSAttr(e);

            // testbiz.Test();
            //IRAPError error=  tree3.SaveRSAttr(list33);

            //var db = new IRAPSqlDBContext("IRAPMDMContext");

            //IRAPTreeBase tree5 = new IRAPTreeBase(db, 57280,271, 2361336);

            //IRAPError error=  tree5.SaveTransAttr(2, 381);
            //Console.WriteLine(error.ErrText);


            //GenAttr_T1 genattr = (GenAttr_T1)tree5.GetGenAttr<GenAttr_T1>();

            //Console.WriteLine("{0} {1}", genattr.EntityID, genattr.BriefDesc);
            //List<TreeViewDTO> list = tree2.TreeView(out errCode, out errText);
            //foreach(TreeViewDTO r in list)
            //{
            //    Console.WriteLine("{0}-{1} {2}", r.NodeID, r.NodeName, r.NodeDepth);
            //}
            //NewTreeNodeDTO err=  tree2.AddNode(4, 4539719, "", "Admin", "", "", "测试工具");
            //Console.WriteLine("{0} {1}", err.NewNodeID, err.ErrText);
            //var list99=  tree2.SubTreeLeaves(5587);
            //foreach (var r in list99)
            //{
            //    Console.WriteLine("{0} {1}",r.NodeName,r.Code);
            //}
            // Test();

       
           testbiz.Test();


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


         public static void Test()
        {
            IRAPBizBase biz = new IRAPBizBase();
            try
            {
                string inParam = File.ReadAllText(@"C:\temp\22.TXT");
                biz.UsingContext("IRAPMDM");
                dynamic dn = biz.GetObjectFromJson(inParam);
                int fatherNode = int.Parse(dn.NodeID.ToString());
                string nodeName = dn.NodeName.ToString();
                int communityID = int.Parse(dn.CommunityID.ToString());

                IRAPTreeSet treeset = new IRAPTreeSet(communityID, 102);
                NewTreeNodeDTO treeDto = treeset.AddNode(4, fatherNode, "", "", "", "", nodeName);
                IRAPTreeBase treebase = biz.GetIRAPTreeBase(communityID, 102, treeDto.NewNodeID);
                EGenAttr_T102 gen = new EGenAttr_T102();
                gen.Unit = dn.Unit.ToString();
                gen.SpecificationType = dn.SpecificationType.ToString();
                gen.Attribute = dn.Attribute.ToString();
                gen.SerialNo = dn.SerialNo.ToString();
                gen.Code = dn.Codeing.ToString();
                gen.ClassType = dn.ClassType.ToString();
                gen.SafetyStock = dn.SafetyStock.ToString();
                gen.Name = dn.Name.ToString();
                treebase.SaveGenAttr(gen);
                treebase.Commit();
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
}
