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

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //  Console.WriteLine(IRAPUser.Test());
            //IRAPUser.Test2();
           
            IRAPUser irapUser = new IRAPUser("Admin", 57280);
            BackLeafSetDTO list = irapUser.GetAgencyList();
            Console.WriteLine("{0} {1}", list.ErrCode, list.ErrText);
            foreach (LeafDTO item in list.Rows)
            {
                Console.WriteLine("{0} {1} {2}", item.Leaf, item.Code, item.Name);
            }
            
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
           */
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

            TestMES test = new TestMES();
            test.Test();
         
            Console.ReadKey();


        }
    }
}
