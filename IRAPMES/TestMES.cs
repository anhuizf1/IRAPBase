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

    public class TestMES
    {

        private string dbConnectStr = null;
        public TestMES()
        {
            this.dbConnectStr =  "IRAPMDMContext";
        }

        public void Test()
        {
            var list = new Repository<Estb101>(dbConnectStr);
            var bizDir = new Repository<ETreeBizDir>(dbConnectStr);
            var bizLeaf = new Repository<ETreeBizLeaf>(dbConnectStr);
            foreach (var r in list.Table.Select(p => new { p.TreeID1,p.TreeCorrID }).ToList())
            {
                Console.WriteLine("{0} {1} ", r.TreeCorrID,r.TreeID1);
            }
           var bizdir2= bizDir.Table.Where(r => r.TreeID == 134).OrderBy(r=>r.NodeID).Select(p => new { p.NodeID, p.NodeName });
            // var bizdir3 = from r in bizDir.Table where r.TreeID == 102 orderby r.NodeID descending select new { r.NodeID,r.NodeName };
            var biz3 = from n in bizDir.Table.Where(t=>t.TreeID==134) join l in bizLeaf.Table.Where(t=>t.TreeID==134) 
                       on new { n.TreeID, n.NodeID } equals new { l.TreeID, NodeID= l.Father } select new { n.NodeID, n.NodeName, LNodeName = l.NodeName, l.Father };
            foreach (var r in biz3)
            {
                Console.WriteLine("{0} {1} {2} {3}", r.NodeID, r.NodeName,r.LNodeName,r.Father);
            }
           
            //string[] text = { "Albert was here", "Burke slept late", "Connor is happy" };
            //var tokens1 = text.Select(s => s.Split(' '));
            //var tokens = text.SelectMany(s => s.Split(' '));
            //foreach (string token in tokens)
            // Console.Write("{0}.", token);



        }
    }






}
