using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRAPBase.Entities;
using IRAPBase;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Reflection;
using IRAPMES.Entities;

namespace IRAPMES

{
   
    public class TestMES {

        private IDbContext _db = null;
        public TestMES()
        {
             this._db = new IRAPSqlDBContext("IRAPMDMContext");
        }

        public void Test()
        {
            var  list = new Repository<Estb101>(_db);
            var  list2 = new Repository<ETreeBizDir>(_db);
            foreach (var r in list.Table.ToList())
            {
                Console.WriteLine("{0} {1} {2}", r.TreeCorrID, r.TreeID1, r.TreeID2);
            }
            foreach (var r in list2.Table.Where(r=>r.TreeID==134).ToList())
            {
                Console.WriteLine("{0} {1} {2}", r.NodeName, r.NodeID, r.Code);
            }
             
        }
    }

  




}
