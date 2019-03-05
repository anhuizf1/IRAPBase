using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRAPBase;
using IRAPMES.Entities;

namespace IRAPMES
{
    public class TestUserBiz
    {
        public static Repository<Eutb_Users> Users()
        {
            return   new Repository<Eutb_Users>("IRAPSqlDBContext");

          
        }
    }
}
