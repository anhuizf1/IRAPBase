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
namespace TestConsole
{
   public class IRAPBizTest:IRAPBizBase
    {
        public void Test()
        {
            var p1 = new SqlParameter("@treeid", 1);
            IEnumerable list =  SqlQuery( typeof(TestEntity), "select NodeName,Code from stb053 where TreeID=@treeid", p1);


         

            IDbContext db = DBContextFactory.Instance.CreateContext("IRAPMDMContext");

           var list2=  IRAPTreeBase.GetLeafSetByCode(db, 57280, 271, "test01");
            
            foreach(var r in list2)
            {
                Console.WriteLine("{0} {1}", r.Code, r.NodeName);
            }
           
           // Console.WriteLine("数据库时间：{0}",now.ToString());
            //foreach (TestEntity r in list)
            //{
            //    Console.WriteLine(r.NodeName +"["+ r.Code+"]");
            //}
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
