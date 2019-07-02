using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using IRAPBase;
using System.Data;
using System.Collections;
using IRAPBase.DTO;
using IRAPBase.Entities;
using TestConsole.Entities;

namespace TestConsole
{
   public class IRAPBizTest:IRAPWorkbench
    {
        public void Test()
        {
            var p1 = new SqlParameter("@treeid", 1);
            IEnumerable list =  SqlQuery( typeof(TestEntity), "select NodeName,Code from stb053 where TreeID=@treeid", p1);
            IDbContext db = DBContextFactory.Instance.CreateContext("IRAPMDMContext");
            IRAPOperBase baseTree = new IRAPOperBase(db, "1B819826-C100-46F4-AE16-019A01941C0D", 794);

         
            //AuxFact_PMO e = new AuxFact_PMO
            //{
            //    Remark = "测试",
            //    WFInstanceID = "good"
            //};
            //IRAPError error = baseTree.SaveAuxFact(e);
            //baseTree.SaveChanges();
            //IRAPOperBase bizBase = new IRAPOperBase(db, "464bcd3b-ec92-42fa-af9b-87bf5123be84", 64);
            //FactEntity f = new FactEntity();
            //f.OpID = 9999;
            //f.TransactNo = 99991;
            //f.BusinessDate = DateTime.Now;
            //IRAPError error = bizBase.SaveTempFact(f);
            //var newTree = new IRAPTreeBase(db, 57280, 133, 2361469);
            //IRAPError error = newTree.SaveTransAttr(2, 20191121);
            //var error = newTree.NewTreeNode(4, 10551, "产线SMD001", "SMD-001", "Admin");
            //  Console.WriteLine(transactno);
            ////每日，每周，每月，每季度，每半年，没N年，指定日期，每两周
            //NewTreeNodeDTO error=  newTree.NewTreeNode(4, 4539741, "每日", "1");
            //NewTreeNodeDTO error2 = newTree.NewTreeNode(4, 4539741, "每周", "2");
            //NewTreeNodeDTO error3 = newTree.NewTreeNode(4, 4539741, "每月", "3");
            //NewTreeNodeDTO error4 = newTree.NewTreeNode(4, 4539741, "每季度", "4");
            //NewTreeNodeDTO error5 = newTree.NewTreeNode(4, 4539741, "每半年", "5");
            //NewTreeNodeDTO error6 = newTree.NewTreeNode(4, 4539741, "每两年", "6");
            //NewTreeNodeDTO error7 = newTree.NewTreeNode(4, 4539741, "指定日期", "7");
            //NewTreeNodeDTO error8 = newTree.NewTreeNode(4, 4539741, "每两周", "8");

            // bizBase.SaveChanges();
           // Console.WriteLine(" {0} {1}  ",error.ErrCode,error.ErrText);
        
            //UsingContext("IRAPMDMContext");
            //Repository< EUTB_Test> utb_test=   GetRepository<EUTB_Test>();

            //utb_test.Insert(new EUTB_Test {  UserCode="anhuizf", UserName = "你好" });
            //utb_test.SaveChanges();
            // Console.WriteLine("保存成功！");
        }
    }

    public class TestEntity
    {
        public int position = 0;
        public object Current { get; private set; }
        public string NodeName { get; set; }
        public string Code { get; set; }
 
    }
}
