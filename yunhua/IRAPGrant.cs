using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRAPBase.Entities;

namespace IRAPBase
{
    /// <summary>
    /// 权限管理
    /// </summary>
    public  class IRAPGrant
    {
       
        Repository<EGrant> _queryGrant = null;
        private int _communityID = 0;
        
        private long PK { get { return _communityID * 10000L; } }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="communityID"></param>
        public IRAPGrant( int communityID )
        {
            this._communityID = communityID;
            _queryGrant = new Repository<EGrant>();
        }
        /// <summary>
        /// 获取某棵树的权限清单
        /// </summary>
        /// <param name="treeID"></param>
        /// <param name="agencyNode"></param>
        /// <param name="roleNode"></param>
        /// <returns></returns>
        public List<EGrant> GetGrantListByTree(int treeID,int agencyNode , int roleNode)
        {
            return _queryGrant.Table.Where(c => c.PartitioningKey==PK &&
            c.TreeID == treeID && c.AgencyNode == agencyNode && c.RoleNode == roleNode).ToList();
        }

    }
}
