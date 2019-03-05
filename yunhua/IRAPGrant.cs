using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRAPBase.Entities;
namespace IRAPBase
{
    public  class IRAPGrant
    {
       
        Repository<EGrant> _queryGrant = null;
        private int _communityID = 0;
        public long PK { get { return _communityID * 10000L; } }
        public IRAPGrant( int communityID )
        {
            this._communityID = communityID;
            _queryGrant = new Repository<EGrant>();
        }

        public List<EGrant> GetGrantListByTree(int treeID,int agencyNode , int roleNode)
        {
            return _queryGrant.Table.Where(c => c.PartitioningKey==PK &&
            c.TreeID == treeID && c.AgencyNode == agencyNode && c.RoleNode == roleNode).ToList();
        }

    }
}
