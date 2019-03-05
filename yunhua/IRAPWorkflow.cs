using IRAPBase.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRAPBase.Entities;
namespace IRAPBase
{
   public  class IRAPWorkflow
    {
        private int _t16LeafID = 0;
        public int T16LeafID { get { return _t16LeafID; } }
        public IRAPWorkflow(int t16LeafID)
        {
            _t16LeafID = t16LeafID;
        }

        public IRAPError Forward()
        {
            return null;
        }
        public IRAPError Backward()
        {
            return null;
        }
    }
}
