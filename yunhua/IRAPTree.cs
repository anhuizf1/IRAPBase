using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    ///<summary>
    ///模块编号：02
    ///作用：IRAP树单实例
    ///作者：张峰
    ///编写日期：2019-02-18
    ///</summary>
    public class IRAPTree
    {
        private int _communityID = 0;
        private int _treeID = 0;
        private int _leafID = 0;
        private int _languageID = 30;
        private IDbContext context = null;
        private TreeNodeEntity node=null;
        private TreeLeafEntity leaf = null;
        TreeClassEntity _classEntity;
        TreeTransient _transientEntity;
        IQueryable<TreeStatus> _status;
        NameSpaceEntity _nameSpace;
        public long PK  { get { return _communityID * 10000L; } }
        //1.(标识)获取叶子集//叶子
        public  TreeLeafEntity LeafEntity { get { return leaf; } }
        //2.(标识/目录)获取节点集合 
        public  TreeNodeEntity NodeEntity { get { return node; } }
        //3 检索属性
        public NameSpaceEntity NameSpace { get { return _nameSpace; } }
        //4分类属性
        public  TreeClassEntity  ClassEntity { get { return _classEntity; } }
        //5.瞬态属性
        public  TreeTransient  TransientEntity { get { return _transientEntity; } }
        //6.状态属性
        public IQueryable<TreeStatus> Status { get { return _status; } }

        //7.一般属性
        public IQueryable<T> GenAttr<T>(T t)  
        {
            return null;
        }
        //8. 行集属性
        public IQueryable<T> RowSet<T>(T t, int ordinal)
        {
            //根据数据字典生成实例
            return null;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public IRAPTree(int communityID , int treeID ,int leafID, int languageID=30 )
        {
            _communityID = communityID;
            _treeID = treeID;
            _leafID = leafID;
            _languageID = languageID;
            if (treeID <= 100)
            {
                context = new IRAPSqlDBContext("IRAPContext");
                if (leafID > 0)
                {
                    node = new Repository<ETreeSysDir>(context).Table.FirstOrDefault(r => r.PartitioningKey == PK+treeID && r.NodeID == leafID);
                }
                else
                {
                    leaf = new Repository<ETreeSysLeaf>(context).Table.FirstOrDefault(r => r.PartitioningKey == PK+treeID && r.LeafID == -leafID);
                    //状态属性
                    _status = new Repository<ETreeSysStatus>(context).Table.Where(r=>r.PartitioningKey==PK&& r.EntityID==leaf.EntityID);
                    //分类属性
                    _classEntity = new Repository<ETreeSysClass>(context).Table.FirstOrDefault(r=>r.PartitioningKey==PK+treeID
                                                             && r.LeafID==-leafID && r.TransactNoLE== 9223372036854775807);
                    //瞬态属性
                    _transientEntity = new Repository<ETreeSysTran>(context).Table.FirstOrDefault(r => r.PartitioningKey == PK + treeID && r.EntityID == leaf.EntityID);

                    //检索属性
                    _nameSpace = new Repository<SysNameSpaceEntity>(context).Table.FirstOrDefault(r => r.PartitioningKey == PK && r.NameID == leaf.NameID && r.LanguageID == languageID);
                    //一般属性
                    //行集属性

                }
            }
            else
            {
                context = new IRAPSqlDBContext("IRAPMDMContext");
                if (leafID > 0)
                {
                    node = new Repository<ETreeBizDir>(context).Table.FirstOrDefault(r => r.PartitioningKey == PK+treeID && r.NodeID == leafID);
                    //检索属性
                    _nameSpace = new Repository<BizNameSpaceEntity>(context).Table.FirstOrDefault(r => r.PartitioningKey == PK && r.NameID == node.NameID && r.LanguageID == languageID);
                }
                else
                {
                    leaf = new Repository<ETreeBizLeaf>(context).Table.FirstOrDefault(r => r.PartitioningKey == PK+treeID && r.LeafID == -leafID);
                    
                    //状态属性
                    _status = new Repository<ETreeBizStatus>(context).Table.Where(r => r.PartitioningKey == PK && r.EntityID == leaf.EntityID); ;
                    //分类属性
                    _classEntity = new Repository<ETreeBizClass>(context).Table.FirstOrDefault(r => r.PartitioningKey == PK + treeID
                                                             && r.LeafID == -leafID && r.TransactNoLE == 9223372036854775807);
                    //瞬态属性
                    _transientEntity = new Repository<ETreeBizTran>(context).Table.FirstOrDefault(r => r.PartitioningKey == PK + treeID && r.EntityID == leaf.EntityID);

                    //检索属性
                    _nameSpace = new Repository<BizNameSpaceEntity>(context).Table.FirstOrDefault(r => r.PartitioningKey == PK && r.NameID == leaf.NameID && r.LanguageID == languageID);
                    //一般属性
                    //行集属性
                }
            }
   
        }
       
        //指定其他分类属性
        public TreeClassEntity GetLinkClassAttr(string dimCode)
        {
            return null;
        }

    }
}
