using IRAPBase.DTO;
using IRAPBase.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using IRAPBase.Serialize;
using System.Data.SqlClient;
namespace IRAPBase
{
    ///<summary>
    ///模块编号：02
    ///作用：IRAP树单实例,明确知道一个叶标识或结点标识时使用
    ///作者：张峰
    ///编写日期：2019-02-18
    ///</summary>
    [Obsolete]
    public class IRAPTree
    {
        protected int _communityID = 0;
        protected int _treeID = 0;
        protected int _leafID = 0;
        protected int _languageID = 30;
        protected bool _isLeaf = false;
        protected IDbContext context = null;
        private TreeNodeEntity node = null;
        private TreeLeafEntity leaf = null;
        private IQueryable<TreeStatus> _statusRepo = null;
        private IQueryable<TreeClassEntity> _classRepo = null;
        private IQueryable<TreeTransient> _transRepo = null;
        private string className = string.Empty;
        Dictionary<int, TreeClassEntity> _classEntity;
        Dictionary<int, TreeTransient> _transientEntity;
        Dictionary<int, TreeStatus> _status;
        NameSpaceEntity _nameSpace;
        //ITreeBase customTree = null;
        public long PK { get { return _communityID * 10000L; } }
        //1.(标识)获取叶子集//叶子
        public TreeLeafEntity LeafEntity { get { return leaf; } }
        //2.(标识/目录)获取节点集合 
        public TreeNodeEntity NodeEntity { get { return node; } }
        //3 检索属性
        public NameSpaceEntity NameSpace { get { return _nameSpace; } }
        //4分类属性
        public Dictionary<int, TreeClassEntity> ClassEntity
        {
            get
            {
                if (_classEntity.Count > 0)
                {
                    return _classEntity;
                }
                foreach (var r in _classRepo)
                {
                    _classEntity.Add(r.Ordinal, r);
                }
                return _classEntity;
            }
        }
        //5.瞬态属性
        public Dictionary<int, TreeTransient> TransientEntity
        {

            get
            {
                if (_transientEntity.Count > 0)
                {
                    return _transientEntity;
                }
                foreach (var r in _transRepo)
                {
                    _transientEntity.Add(r.Ordinal, r);
                }
                return _transientEntity;
            }

        }
        //6.状态属性
        public Dictionary<int, TreeStatus> Status
        {
            get
            {

                if (_status.Count > 0)
                {
                    return _status;
                }
                foreach (var r in _statusRepo)
                {
                    _status.Add(r.Ordinal, r);
                }
                return _status;
            }
        }
        //public IQueryable<BaseRowAttrEntity> GetRowSet(int ordinal)
        //{
        //    if (leaf == null)
        //    {
        //        return null;
        //    }
        //    return customTree.GetRowAttr(ordinal).Where(c => c.PartitioningKey == PK + _treeID && c.EntityID == leaf.EntityID).OrderBy(r => r.Ordinal);
        //}
        //7.一般属性
        //public BaseGenAttrEntity GetGenAttr()
        //{
        //    if (leaf == null)
        //    {
        //        return null;
        //    }
        //    return customTree.GetGenAttr().FirstOrDefault(c => c.PartitioningKey == PK + _treeID && c.EntityID == leaf.EntityID);
        //}
        //8. 行集属性

        /// <summary>
        /// 创建树的实例
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="treeID">树标识</param>
        /// <param name="leafID">结点或叶标识（不要负数）</param>
        /// <param name="isLeaf">是否为叶</param>
        /// <param name="languageID">语言</param>
        public IRAPTree(int communityID, int treeID, int leafID, bool isLeaf = true, int languageID = 30)
        {
            _communityID = communityID;
            _treeID = treeID;
            _leafID = leafID;
            _languageID = languageID;
            className = $"IRAPBase.IRAPTree{_treeID}";
            _isLeaf = isLeaf;
            if (treeID <= 100)
            {
                context = new IRAPSqlDBContext("IRAPContext");
                if (!isLeaf)
                {
                    node = new Repository<ETreeSysDir>(context).Table.FirstOrDefault(r => r.PartitioningKey == PK + treeID && r.NodeID == leafID);
                    if (node == null)
                    {
                        throw new Exception($"node={leafID}不存在");
                    }

                }
                else
                {
                    leaf = new Repository<ETreeSysLeaf>(context).Table.FirstOrDefault(r => r.PartitioningKey == PK + treeID && r.LeafID == leafID);
                    if (leaf == null)
                    {
                        throw new Exception($"LeafID={leafID}不存在");
                    }
                    //状态属性
                    _statusRepo = new Repository<ETreeSysStatus>(context).Table.Where(r => r.PartitioningKey == PK && r.EntityID == leaf.EntityID);
                    //分类属性
                    _classRepo = new Repository<ETreeSysClass>(context).Table.Where(r => r.PartitioningKey == PK + treeID
                                                             && r.LeafID == leafID);
                    //瞬态属性
                    _transRepo = new Repository<ETreeSysTran>(context).Table.Where(r => r.PartitioningKey == PK + treeID && r.EntityID == leaf.EntityID);

                    //检索属性
                    _nameSpace = new Repository<SysNameSpaceEntity>(context).Table.FirstOrDefault(r => r.PartitioningKey == PK && r.NameID == leaf.NameID && r.LanguageID == languageID);
                    //一般属性
                    //行集属性

                }
            }
            else
            {
                context = new IRAPSqlDBContext("IRAPMDMContext");
                if (!isLeaf)
                {
                    node = new Repository<ETreeBizDir>(context).Table.FirstOrDefault(r => r.PartitioningKey == PK + treeID && r.NodeID == leafID);
                    if (node == null)
                    {
                        throw new Exception($"node={leafID}不存在");
                    }
                    //检索属性
                    _nameSpace = new Repository<BizNameSpaceEntity>(context).Table.FirstOrDefault(r => r.PartitioningKey == PK && r.NameID == node.NameID && r.LanguageID == languageID);
                }
                else
                {
                    leaf = new Repository<ETreeBizLeaf>(context).Table.FirstOrDefault(r => r.PartitioningKey == PK + treeID && r.LeafID == -leafID);
                    if (leaf == null)
                    {
                        throw new Exception($"LeafID={leafID}不存在");
                    }
                    //状态属性
                    _statusRepo = new Repository<ETreeBizStatus>(context).Table.Where(r => r.PartitioningKey == PK && r.EntityID == leaf.EntityID);
                    //分类属性
                    _classRepo = new Repository<ETreeBizClass>(context).Table.Where(r => r.PartitioningKey == PK + treeID
                                                             && r.LeafID == leafID);
                    //瞬态属性
                    _transRepo = new Repository<ETreeBizTran>(context).Table.Where(r => r.PartitioningKey == PK + treeID && r.EntityID == leaf.EntityID);

                    //检索属性
                    _nameSpace = new Repository<BizNameSpaceEntity>(context).Table.FirstOrDefault(r => r.PartitioningKey == PK && r.NameID == leaf.NameID && r.LanguageID == languageID);
                    //一般属性
                    //行集属性
                }
            }
            //初始化
            _classEntity = new Dictionary<int, TreeClassEntity>();
            _transientEntity = new Dictionary<int, TreeTransient>();
            _status = new Dictionary<int, TreeStatus>();
            //如果此树有单独定义的动态创建
            // if (System.Type.GetType(className) != null)
            // {
            ///      customTree = (ITreeBase)Activator.CreateInstance(System.Type.GetType(className), context);
            //  }

        }
        /*
        public IRAPError SaveTransient(params TreeTransient[] dict)
        {
            Type e = null;
            if (dict.Length > 0)
            {
                e = dict[0].GetType();
            }
            else
            {
                return new IRAPError(9, "无参数不需要保存！");
            }
            foreach (var r in dict)
            {
                context.GetSet(e).Add(r);
            }
            int res = context.SaveChanges();
            return new IRAPError(0, "保存成功！");
        }
        public IRAPError SaveCassify(params TreeClassEntity[] dict)
        {
            if (leaf == null)
            {
                return new IRAPError(22, "构造IRAPTree参数不正确或结点不支持分类属性！");
            }
            foreach (var item in dict)
            {
                if (item.LeafID != leaf.LeafID)
                {
                    return new IRAPError(22, $"输入参数LeafID不正确 ，应该为{leaf.LeafID}！");
                }
            }
            object repositoryInstance = null;
            if (_treeID <= 100)
            {
                repositoryInstance = new Repository<ETreeSysClass>(context);
            }
            else
            {
                repositoryInstance = new Repository<ETreeBizClass>(context);
            }
            //string entityClassName = "IRAPBase.Entities.ETreeSysClass";
            //if (_treeID > 100)
            //{
            //    entityClassName = "IRAPBase.Entities.ETreeBizClass";
            //}
            //  Type type = Type.GetType(entityClassName, true, true);
            //  var repositoryType = typeof(Repository<BaseEntity>);
            //  Repository<BaseEntity> repositoryInstance = (Repository<BaseEntity>)Activator.CreateInstance(repositoryType.MakeGenericType(type), context);

            foreach (TreeClassEntity item in dict)
            {
                var c = _classRepo.FirstOrDefault(r => r.Ordinal == item.Ordinal);
                if (c == null)
                {
                    //TreeClassEntity newC= (TreeClassEntity)Activator.CreateInstance(type);
                    item.PartitioningKey = PK + _treeID;
                    if (_treeID <= 100)
                    {
                        (repositoryInstance as Repository<ETreeSysClass>).Insert((ETreeSysClass)item);
                        (repositoryInstance as Repository<ETreeSysClass>).SaveChanges();
                    }
                    else
                    {
                        (repositoryInstance as Repository<ETreeBizClass>).Insert((ETreeBizClass)item);
                        (repositoryInstance as Repository<ETreeBizClass>).SaveChanges();
                    }
                    //插入
                }
                else
                {
                    c.A4LeafID = item.A4LeafID;
                    if (_treeID <= 100)
                    {
                        (repositoryInstance as Repository<ETreeSysClass>).Update((ETreeSysClass)c);
                        (repositoryInstance as Repository<ETreeSysClass>).SaveChanges();
                    }
                    else
                    {
                        (repositoryInstance as Repository<ETreeBizClass>).Update((ETreeBizClass)c);
                        (repositoryInstance as Repository<ETreeBizClass>).SaveChanges();
                    }
                }
            }

            return new IRAPError(0, "分类属性保存成功！");

        }
      
        //指定其他分类属性
        public TreeClassEntity GetLinkClassAttr(string dimCode)
        {
            throw new NotImplementedException();
        }
       
        /// <summary>
        /// 保存行集属性
        /// </summary>
        /// <param name="list">行集属性清单</param>
        /// <returns>通用错误</returns>
        public IRAPError SaveRSAttr(List<BaseRowAttrEntity> list)
        {
            if (customTree == null)
            {
                return new IRAPError(99, $"此树:TreeID={_treeID}没有定义个性化类：" + className + "，无法保存属性！");
            }
            var exists = list.FirstOrDefault(c => c.EntityID != leaf.EntityID);
            if (exists != null)
            {
                return new IRAPError(99, "EntityID传入不正确，请检查！");
            }
            int i = 0;
            foreach (BaseRowAttrEntity r in list)
            {
                i++;
                r.Ordinal = i;
                r.PartitioningKey = PK + _treeID;
                r.VersionGE = 0;
                r.VersionLE = (int)(Math.Pow(2, 31) - 1);
            }
            return customTree.SaveRSAttr(list);
        }*/

        /// <summary>
        /// 保存一般属性
        /// </summary>
        /// <param name="e">实体类型</param>
        /// <returns>通用错误</returns>
        //public IRAPError SaveGenAttr(BaseGenAttrEntity e)
        //{
        //    if (customTree == null)
        //    {
        //        return new IRAPError(99, $"此树:TreeID={_treeID}没有定义个性化类：" + className + "，无法保存属性！");
        //    }
        //    if (leaf == null)
        //    {
        //        return new IRAPError(22, "结点不需要保存一般属性！");
        //    }
        //    if (e.EntityID != leaf.EntityID)
        //    {
        //        return new IRAPError(22, $"实体中EntityID不正确，应该为{leaf.EntityID}！");
        //    }
        //    e.PartitioningKey = PK + _treeID;
        //    return customTree.SaveGenAttr(e);
        //}

    }

    /// <summary>
    ///模块编号：02
    ///作用：存取树万能类 
    ///作者：张峰
    ///编写日期：2019-03-1
    /// </summary>
    public class IRAPTreeBase
    {
        #region 变量定义
        /// <summary>
        /// 社区标识
        /// </summary>
        protected int _communityID = 0;
        protected int _leafID = 0;
        protected int _entityID = 0;
        protected int _treeID = 0;
        protected IDbContext _db = null;
        protected IQueryable<TreeNodeEntity> _nodes;
        protected IQueryable<TreeLeafEntity> _leaves;
        protected IQueryable<TreeClassEntity> _treeClass;
        protected IQueryable<TreeTransient> _treeTrans;
        protected IQueryable<TreeStatus> _treeStatus;
        protected TreeLeafEntity _leafEntity = null;
        private long[] _PKDict;
        #endregion

        #region 属性定义
        /// <summary>
        /// 社区号CommunityID*10000+TreID
        /// </summary>
        public long PK { get { return _communityID * 10000L + _treeID; } }

        /// <summary>
        /// 返回的叶实体，有可能为空使用时注意判断
        /// </summary>
        public TreeLeafEntity Leaf
        {
            get { return _leafEntity; }
        }
        /// <summary>
        ///根据社区和树标识查询的叶子集合
        /// </summary>
        public IQueryable<TreeLeafEntity> LeafSet
        {
            get { return _leaves; }
        }

        /// <summary>
        /// 根据社区和树标识查询的结点集合
        /// </summary>
        public IQueryable<TreeNodeEntity> NodeSet
        {
            get { return _nodes; }
        }
        /// <summary>
        /// 此叶结点的分类属性（多个分类属性按行存储）有可能为null注意判断
        /// </summary>
        public IQueryable<TreeClassEntity> ClassifySet
        {

            get { return _treeClass; }
        }

        /// <summary>
        /// 此叶结点瞬态属性（按行存储）有可能为null注意判断
        /// </summary>
        public IQueryable<TreeTransient> TransientSet
        {
            get { return _treeTrans; }
        }
        /// <summary>
        /// 获取瞬态属性的字典，键值为瞬态属性序号
        /// </summary>
        public Dictionary<byte, TransientDTO> TransientDict
        {
            get
            {
                return _treeTrans.Select(r => r)
                    .ToDictionary(s => s.Ordinal, v =>
                     new TransientDTO { AttrValue = v.AttrValue, Scale = v.Scale, UnitOfMeasure = v.UnitOfMeasure });
            }
        }
        /// <summary>
        /// 获取分类属性的字典，键值为分类属性序号
        /// </summary>
        public Dictionary<int, ClassifyDTO> ClassifyDict
        {
            get
            {
                return _treeClass.Select(m => new ClassifyDTO
                {
                    AttrTreeID = m.AttrTreeID,
                    Ordinal = m.Ordinal,
                    A4LeafID = m.A4LeafID,
                    A4Code = m.A4Code,
                    A4AlternateCode = m.A4AlternateCode,
                    A4NodeName = m.A4NodeName
                }).ToDictionary(s => s.Ordinal);
            }
        }

        /// <summary>
        /// 获取分类属性的字典，键值为分类属性的序号
        /// </summary>
        public Dictionary<int, StatusDTO> StatusDict
        {
            get
            {
                return _treeStatus.ToDictionary(s => s.Ordinal, v => new StatusDTO
                {
                    StatusValue = v.StatusValue,
                    ColorRGBValue = v.ColorRGBValue,
                    T5LeafID = v.T5LeafID,
                    TransitCtrlValue = v.TransitCtrlValue
                });
            }
        }

        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="communityID">社区编号</param>
        /// <param name="treeID">树标识</param>
        /// <param name="leafID">结点标识</param>
        public IRAPTreeBase(IDbContext db, int communityID, int treeID, int leafID = 0)
        {
            _db = db;
            _treeID = treeID;
            _communityID = communityID;
            _leafID = Math.Abs(leafID);
            _PKDict = new long[] { PK, _treeID };
            if (_treeID <= 100)
            {
                _nodes = new Repository<ETreeSysDir>(_db).Table.Where(c => _PKDict.Contains(c.PartitioningKey) && c.TreeID == _treeID);
                _leaves = new Repository<ETreeSysLeaf>(_db).Table.Where(c => c.PartitioningKey == PK && c.TreeID == _treeID);
            }
            else
            {
                _nodes = new Repository<ETreeBizDir>(_db).Table.Where(c => _PKDict.Contains(c.PartitioningKey) && c.TreeID == _treeID);
                //  _leaves = new Repository<ETreeBizLeaf>(_db).Table;
                _leaves = _db.Set<ETreeBizLeaf>().Where(c => c.PartitioningKey == PK && c.TreeID == _treeID);
            }

            if (_leafID != 0)
            {
                _leafEntity = _leaves.FirstOrDefault(c => c.LeafID == _leafID);
                if (_leafEntity == null)
                {
                    throw new Exception($"LeafID={_leafID}不存在！");
                }
                _entityID = _leafEntity.EntityID;
                if (_treeID <= 100)
                {
                    //加载分类属性
                    _treeClass = _db.Set<ETreeSysClass>().Where(c => c.PartitioningKey == PK && c.LeafID == _leafID).OrderBy(c => c.Ordinal);
                    //加载瞬态属性
                    _treeTrans = _db.Set<ETreeSysTran>().Where(c => c.PartitioningKey == PK && c.EntityID == _entityID).OrderBy(c => c.Ordinal);
                    //加载状态属性
                    _treeStatus = _db.Set<ETreeSysStatus>().Where(c => c.PartitioningKey == PK && c.EntityID == _entityID).OrderBy(c => c.Ordinal);
                }
                else
                {   //加载分类属性
                    _treeClass = _db.Set<ETreeBizClass>().Where(c => c.PartitioningKey == PK && c.LeafID == _leafID).OrderBy(c => c.Ordinal);
                    //加载瞬态属性
                    _treeTrans = _db.Set<ETreeBizTran>().Where(c => c.PartitioningKey == PK && c.EntityID == _entityID).OrderBy(c => c.Ordinal);
                    //加载状态属性
                    _treeStatus = _db.Set<ETreeBizStatus>().Where(c => c.PartitioningKey == PK && c.EntityID == _entityID).OrderBy(c => c.Ordinal);
                }
            }
        }
        #endregion

        #region 保护的方法
        /// <summary>
        /// 根据实体类型获取存储对象DbSet,以便操作实体（增删改查）
        /// </summary>
        /// <param name="t">实体</param>
        /// <returns>表集合DbSet对象</returns>
        protected DbSet TableSet(BaseEntity t)
        {
            return _db.GetSet(t.GetType());
        }
        #endregion

        #region 新增或修改属性
        /// <summary>
        /// 新增节点或叶子
        /// </summary>
        /// <param name="fatherNode">父结点</param>
        /// <param name="englishName">英文名称</param>
        /// <param name="createBy">创建人</param>
        /// <param name="nodeCode">结点代码</param>
        /// <param name="alterNateCode">替代代码</param>
        /// <param name="nodeName">结点名称</param>
        /// <param name="udfOrdinal">位置序号</param>
        /// <returns></returns>
        private NewTreeNodeDTO NewNode(int fatherNode, string englishName, string createBy,
            string nodeCode, string alterNateCode, string nodeName, float udfOrdinal = 0F)
        {
            NewTreeNodeDTO res = new NewTreeNodeDTO();

            if (nodeName.Trim() == string.Empty)
            {
                res.ErrCode = 22;
                res.ErrText = "结点名称不能为空！";
                return res;
            }
            long[] pkDict = new long[2] { PK, _treeID };
            res.PartitioningKey = PK;
            try
            {
                TreeNodeEntity father = null;
                if (fatherNode == 0)
                {
                    father = _nodes.FirstOrDefault(r => pkDict.Contains(r.PartitioningKey));
                    if (father != null)
                    {
                        res.ErrCode = 93;
                        res.ErrText = "根节点已存在节点不允许创建根节点！FatherNode不能为0";
                        return res;
                    }
                }
                else
                {
                    father = _nodes.FirstOrDefault(r => r.NodeID == fatherNode);
                }
                if (father == null && fatherNode != 0)
                {
                    res.ErrCode = 22;
                    res.ErrText = $"传入的父结点：{fatherNode}不存在！";
                    return res;
                }
                int nodeDepth = father == null ? 0 : father.NodeDepth + 1;

                TreeNodeEntity e = new TreeNodeEntity
                {
                    PartitioningKey = PK,
                    NameID = 0,
                    IconID = 0,
                    NodeDepth = (byte)nodeDepth,
                    NodeID = 0, //申请
                    NodeName = nodeName,
                    NodeStatus = 0,
                    TreeID = (short)_treeID,
                    UDFOrdinal = udfOrdinal,
                    Code = nodeCode,
                    CSTRoot = 0,
                    Father = fatherNode,
                    DescInEnglish = englishName,
                    CreatedBy = createBy,
                    CreatedOn = DateTime.Now,
                    ModifiedBy = "",
                    ModifiedOn = DateTime.Now,
                };
                string seqName = "NextSysNodeID";
                if (_treeID > 100)
                {
                    seqName = "NextUserNodeID";
                }
                SequenceValueDTO dto = IRAPSequence.GetSequence(seqName, 1);
                if (dto.ErrCode != 0)
                {
                    res.ErrCode = dto.ErrCode;
                    res.ErrText = dto.ErrText;
                    return res;
                }
                e.NodeID = (int)dto.SequenceValue;
                res.NewNodeID = e.NodeID;
                res.NewEntityID = 0;
                res.PartitioningKey = PK;
                if (father != null)
                {
                    e.CSTRoot = father.NodeID == 0 ? e.NodeID : father.CSTRoot;
                }
                else
                {
                    e.CSTRoot = e.NodeID;
                }

                TreeNodeEntity c;
                if (_treeID <= 100)
                {
                    c = e.CopyTo<ETreeSysDir>();
                }
                else
                {
                    c = e.CopyTo<ETreeBizDir>();
                }
                TableSet(c).Add(c);
                _db.SaveChanges();
                res.ErrCode = 0;
                res.ErrText = "结点创建成功！ ";
                return res;
            }
            catch (Exception err)
            {
                //_db.RollBack();
                res.ErrCode = 9999;
                res.ErrText = $"创建结点发生错误:{err.Message}";
                return res;
            }
        }

        //新增叶子
        private NewTreeNodeDTO NewLeaf(int fatherNode, string englishName, string createBy,
            string nodeCode, string alterNateCode, string nodeName, float udfOrdinal = 0F)
        {
            NewTreeNodeDTO res = new NewTreeNodeDTO();
            //long[] pkDict = new long[2] { PK, _treeID };
            string seqLeafName = "NextSysLeafID";
            string seqEntityName = "NextSysEntityID";
            try
            {
                TreeNodeEntity father = _nodes.FirstOrDefault(r => r.NodeID == fatherNode);
                if (father == null)
                {
                    res.ErrCode = 22;
                    res.ErrText = $"父结点：{fatherNode}不存在！";
                    return res;
                }

                TreeLeafEntity e = new TreeLeafEntity
                {
                    PartitioningKey = PK,
                    NameID = 0,
                    IconID = 0,
                    AlternateCode = alterNateCode,
                    NodeDepth = (byte)(father.NodeDepth + 1),
                    LeafID = 0, //申请
                    NodeName = nodeName,
                    LeafStatus = 0,
                    TreeID = (short)_treeID,
                    UDFOrdinal = udfOrdinal,
                    Code = nodeCode,
                    CSTRoot = father.CSTRoot,
                    Father = fatherNode,
                    DescInEnglish = englishName,
                    CreatedBy = createBy,
                    CreatedOn = DateTime.Now,
                    ModifiedBy = "",
                    ModifiedOn = DateTime.Now
                };
                if (_treeID > 100)
                {
                    seqLeafName = "NextUserLeafID";
                    seqEntityName = "NextUserEntityID";
                }
                SequenceValueDTO dto = IRAPSequence.GetSequence(seqLeafName, 1);
                if (dto.ErrCode != 0)
                {
                    res.ErrCode = dto.ErrCode;
                    res.ErrText = dto.ErrText;
                    return res;
                }
                e.LeafID = (int)dto.SequenceValue;
                SequenceValueDTO dto2 = IRAPSequence.GetSequence(seqEntityName, 1);
                //为了保证LeafID与EntityID绝对一致性，用同一个序列值
                e.EntityID = e.LeafID; // (int)dto2.SequenceValue;
                TreeLeafEntity c;
                if (_treeID <= 100)
                {
                    c = e.CopyTo<ETreeSysLeaf>();
                }
                else
                {
                    c = e.CopyTo<ETreeBizLeaf>();
                }
                TableSet(c).Add(c);
                _db.SaveChanges();
                res.NewNodeID = e.LeafID;
                res.NewEntityID = e.LeafID;
                res.PartitioningKey = PK;
                res.ErrCode = 0;
                res.ErrText = $"新增叶结点成功！叶结点标识：{e.LeafID} ";
                return res;
            }
            catch (Exception err)
            {
                res.ErrCode = 9999;
                res.ErrText = $"创建叶子时发生错误:{err.Message}";
                return res;
            }
        }

        /// <summary>
        /// 新增结点或叶
        /// </summary>
        /// <param name="nodeType">结点类型：3-结点 4-叶子</param>
        /// <param name="fatherNode">父结点标识</param>
        /// <param name="nodeName">结点(叶)名称</param>
        /// <param name="nodeCode">结点(叶)代码</param>
        /// <param name="createBy">创建人代码</param>
        /// <param name="alterNateCode">替代代码</param>
        /// <param name="englishName">英文名称</param>
        /// <param name="udfOrdinal">插入位置序号</param>
        /// <returns></returns>
        public NewTreeNodeDTO NewTreeNode(int nodeType, int fatherNode,
            string nodeName, string nodeCode, string createBy, string alterNateCode = "", string englishName = "", float udfOrdinal = 0F)
        {
            fatherNode = Math.Abs(fatherNode);
            NewTreeNodeDTO res = new NewTreeNodeDTO();
            if (nodeType == 3)
            {
                return NewNode(fatherNode, englishName, createBy, nodeCode, alterNateCode, nodeName, udfOrdinal);
            }
            else if (nodeType == 4)
            {
                return NewLeaf(fatherNode, englishName, createBy, nodeCode, alterNateCode, nodeName, udfOrdinal);
            }
            else
            {
                res.ErrCode = 91;
                res.ErrText = "您传入的参数NodeType不正确，3=结点 4=叶子，其他类型不支持！";
                res.NewNodeID = 0;
            }
            return res;
        }

        /// <summary>
        /// 修改结点或叶子
        /// </summary>
        /// <param name="nodeType">结点类型：3-结点 4-叶子</param>
        /// <param name="nodeID">结点标识</param>
        /// <param name="nodeName">名称</param>
        /// <param name="nodeCode">代码</param>
        /// <param name="englishName">英文名称</param>
        /// <param name="alternateCode">替代代码</param>
        /// <param name="modifiedBy">修改人代码</param>
        /// <param name="udfOrdinal">序号</param>
        /// <returns></returns>
        public IRAPError ModifyTreeNode(int nodeType, int nodeID, string nodeName, string nodeCode = "", string englishName = "", string alternateCode = "", string modifiedBy = "", float udfOrdinal = 1F)
        {
            IRAPError error = new IRAPError();
            try
            {
                if (nodeName == string.Empty)
                {
                    error.ErrCode = 23;
                    error.ErrText = $"结点名称不能为空！";
                    return error;
                }
                if (nodeID < 0)
                {
                    nodeID = -nodeID;
                }
                if (nodeType == 3)
                {
                    var thisNode = _nodes.FirstOrDefault(r => r.NodeID == nodeID);
                    if (thisNode == null)
                    {
                        error.ErrCode = 22;
                        error.ErrText = $"结点标识：{nodeID}不存在或无权限修改！";
                        return error;
                    }
                    thisNode.Code = nodeCode;
                    thisNode.NodeName = nodeName;
                    thisNode.UDFOrdinal = udfOrdinal;
                    thisNode.DescInEnglish = englishName;
                    thisNode.ModifiedOn = DateTime.Now;
                    thisNode.ModifiedBy = modifiedBy;
                    _db.SaveChanges();
                }
                else if (nodeType == 4)
                {
                    var thisNode = _leaves.FirstOrDefault(r => r.LeafID == nodeID);
                    if (thisNode == null)
                    {
                        error.ErrCode = 22;
                        error.ErrText = $"结点标识：{nodeID}不存在！";
                        return error;
                    }
                    thisNode.Code = nodeCode;
                    thisNode.AlternateCode = alternateCode;
                    thisNode.NodeName = nodeName;
                    thisNode.UDFOrdinal = udfOrdinal;
                    thisNode.DescInEnglish = englishName;
                    thisNode.ModifiedBy = modifiedBy;
                    thisNode.ModifiedOn = DateTime.Now;
                    thisNode.AlternateCode = alternateCode;
                    _db.SaveChanges();
                }
                else
                {
                    error.ErrCode = 91;
                    error.ErrText = "您输入的参数NodeType仅支持3=结点 4=叶子，不支持的其他类型！";
                    return error;
                }
                error.ErrCode = 0;
                error.ErrText = "修改结点成功！";
                return error;
            }
            catch (Exception err)
            {
                error.ErrCode = 9999;
                error.ErrText = $"修改结点失败：{err.Message}";
                return error;
            }
        }
        /// <summary>
        /// 删除结点或叶子
        /// </summary>
        /// <param name="nodeType">结点类型：3-结点 4-叶子</param>
        /// <param name="nodeID">结点标识</param>
        /// <returns></returns>
        public IRAPError DeleteTreeNode(int nodeType, int nodeID)
        {
            IRAPError error = new IRAPError();
            // long[] pkDict = new long[2] { PK, _treeID };
            if (nodeID < 0)
            {
                nodeID = -nodeID;
            }
            try
            {
                if (nodeType == 3)
                {
                    var thisNode = _nodes.FirstOrDefault(r => r.NodeID == nodeID);
                    if (thisNode == null)
                    {
                        error.ErrCode = 22;
                        error.ErrText = $"结点标识：{nodeID}不存在！";
                        return error;
                    }
                    if (thisNode.PartitioningKey == _treeID)
                    {
                        error.ErrCode = 22;
                        error.ErrText = $"结点标识：{nodeID}属于系统结点请从后台删除！";
                        return error;
                    }
                    var tempNode = _nodes.FirstOrDefault(c => c.Father == nodeID);
                    if (tempNode != null)
                    {
                        error.ErrCode = 23;
                        error.ErrText = $"结点“{tempNode.NodeName}”下面已有结点，请先删除子结点！";
                        return error;
                    }
                    TableSet(thisNode).Remove(thisNode);
                    _db.SaveChanges();
                }
                else if (nodeType == 4)
                {
                    var thisNode = _leaves.FirstOrDefault(r => r.LeafID == nodeID);
                    if (thisNode == null)
                    {
                        error.ErrCode = 22;
                        error.ErrText = $"叶结点标识：{nodeID}不存在！";
                        return error;
                    }
                    TableSet(thisNode).Remove(thisNode);
                    _db.SaveChanges();
                }
                else
                {
                    error.ErrCode = 91;
                    error.ErrText = "输入参数NodeType只允许=3 结点 =4 叶子 不支持其他的类型！";
                    return error;
                }
                error.ErrCode = 0;
                error.ErrText = "删除结点成功！";
                return error;
            }
            catch (Exception err)
            {
                error.ErrCode = 9999;
                error.ErrText = $"删除结点失败：{err.Message}";
                return error;
            }
        }
        /// <summary>
        /// 保存或修改分类属性
        /// </summary>
        /// <param name="dict">分类属性字典</param>
        /// <returns>返回错误</returns>
        public IRAPError SaveClassAttr(params TreeClassifyDTO[] dict)
        {
            if (_leafID == 0 || _leafEntity == null)
            {
                 throw  new Exception( "构造IRAPTreeBase参数不正确，检查社区、树标识，LeafID是否正确！");
            }
            var list = _treeClass.ToList();
            foreach (TreeClassifyDTO item in dict)
            {
                //验证分类属性有效性TreeID 和 LeafID 在系统中不存在的情况
                try
                {
                    IRAPTreeBase tree = new IRAPTreeBase(_db, _communityID, item.TreeID, item.DimLeaf);
                }
                catch (Exception err)
                {
                    throw new Exception( $"第{item.Ordinal}分类属性TreeID:{item.TreeID} LeafID:{item.DimLeaf}在系统中不存在！" + err.Message);
                }
                var thisEntity = IRAPTreeSet.GetLeafEntity(item.DimLeaf);
                if (thisEntity == null)
                {
                    throw new Exception("保存的分类属性LeafID不存在！");
                }
                if (list.Exists(c => c.Ordinal == item.Ordinal))
                {

                    var cEntity = list.FirstOrDefault(c => c.Ordinal == item.Ordinal);
                    cEntity.A4LeafID = item.DimLeaf;
                    cEntity.A4Code = thisEntity.Code;
                    cEntity.A4NodeName = thisEntity.NodeName;
                    cEntity.A4AlternateCode = thisEntity.AlternateCode;
                    cEntity.A4DescInEnglish = thisEntity.DescInEnglish;

                }
                else
                {

                    TreeClassEntity c;
                    TreeClassEntity e = new TreeClassEntity
                    {
                        A4LeafID = item.DimLeaf,
                        LeafID = _leafID,
                        Ordinal = item.Ordinal,
                        PartitioningKey = PK,
                        A4Code = thisEntity.Code,
                        A4NodeName = thisEntity.NodeName,
                        A4AlternateCode = thisEntity.AlternateCode,
                        A4DescInEnglish = thisEntity.DescInEnglish
                        //TreeID = (short)_treeID, // (short)item.TreeID //根据模型填写
                    };
                    if (_treeID <= 100)
                    {
                        c = e.CopyTo<ETreeSysClass>();
                    }
                    else
                    {
                        c = e.CopyTo<ETreeBizClass>();
                    }
                    TableSet(c).Add(c);

                }
            }
            _db.SaveChanges();
            return new IRAPError(0, "分类属性保存成功！");
        }
        /// <summary>
        /// 根据参数位置保存或修改分类属性
        /// </summary>
        /// <param name="dict">分类属性值</param>
        /// <returns>通用错误IRAPError</returns>
        public IRAPError SaveClassAttr(params int[] dict)
        {
            if (_leafID == 0 || _leafEntity == null)
            {
                throw new Exception("构造IRAPTreeBase参数不正确，检查社区、树标识，LeafID是否正确！");
            }
            int i = 0;
            var list = _treeClass.ToList();
            foreach (int v in dict)
            {
                var thisEntity = IRAPTreeSet.GetLeafEntity(v);
                if (thisEntity == null)
                {
                    throw new Exception( "保存的分类属性LeafID不存在！");
                }
                i++;
                var obj = list.FirstOrDefault(c => c.Ordinal == i);
                if (obj == null)
                {
                    TreeClassEntity c;
                    TreeClassEntity e = new TreeClassEntity
                    {
                        LeafID = _leafID,
                        Ordinal = i,
                        PartitioningKey = PK,
                        RowChecksum = 0,
                        VersionGE = 0,
                        VersionLE = (int)(Math.Pow(2, 31) - 1),
                        A4LeafID = v,
                        A4Code = thisEntity.Code,
                        A4NodeName = thisEntity.NodeName,
                        A4AlternateCode = thisEntity.AlternateCode,
                        A4DescInEnglish = thisEntity.DescInEnglish
                        //TreeID = (short)_treeID
                    };
                    if (_treeID <= 100)
                    {
                        c = e.CopyTo<ETreeSysClass>();
                    }
                    else
                    {
                        c = e.CopyTo<ETreeBizClass>();
                    }
                    TableSet(c).Add(c);
                }
                else
                {
                    obj.A4LeafID = v;
                }
            }
            _db.SaveChanges();
            return new IRAPError(0, "分类属性保存成功");
        }

        /// <summary>
        /// 保存或修改分类属性
        /// </summary>
        /// <param name="ordinal">分类属性序号</param>
        /// <param name="dimLeaf">分类属性值</param>
        /// <returns></returns>
        public IRAPError SaveClassAttr(int ordinal, int dimLeaf)
        {
            if (ordinal > 254)
            {
                throw new Exception( "分类属性序列超过254，请确认调用的重载方法是否正确！");
            }
            if (_leafID == 0 || _leafEntity == null)
            {
                throw new Exception("构造IRAPTreeBase参数不正确，检查社区、树标识，LeafID是否正确！");
            }
            var thisEntity = IRAPTreeSet.GetLeafEntity(dimLeaf);
            if (thisEntity == null)
            {
                throw new Exception( "保存的分类属性LeafID不存在！");
            }

            TreeClassEntity dimobj = _treeClass.FirstOrDefault(r => r.Ordinal == ordinal);
            if (dimobj == null)
            {

                TreeClassEntity c;
                TreeClassEntity e = new TreeClassEntity
                {
                    LeafID = _leafID,
                    Ordinal = ordinal,
                    PartitioningKey = PK,
                    RowChecksum = 0,
                    VersionGE = 0,
                    VersionLE = (int)(Math.Pow(2, 31) - 1),
                    A4LeafID = dimLeaf,
                    A4Code = thisEntity.Code,
                    A4NodeName = thisEntity.NodeName,
                    A4AlternateCode = thisEntity.AlternateCode,
                    A4DescInEnglish = thisEntity.DescInEnglish

                    //TreeID = (short)_treeID
                };
                if (_treeID <= 100)
                {
                    c = e.CopyTo<ETreeSysClass>();
                }
                else
                {
                    c = e.CopyTo<ETreeBizClass>();
                }
                TableSet(c).Add(c);
            }
            else
            {
                dimobj.A4Code = thisEntity.Code;
                dimobj.A4NodeName = thisEntity.NodeName;
                dimobj.A4AlternateCode = thisEntity.AlternateCode;
                dimobj.A4DescInEnglish = thisEntity.DescInEnglish;
                dimobj.A4LeafID = dimLeaf;
            }
            _db.SaveChanges();
            return new IRAPError(0, "保存指定分类属性成功！");
        }
        /// <summary>
        /// 保存和修改状态属性
        /// </summary>
        /// <param name="statusDict">状态属性值，参数位置代表状态属性序号</param>
        /// <returns>通用错误IRAPError</returns>
        public IRAPError SaveStatusAttr(params byte[] statusDict)
        {
            if (_leafID == 0 || _leafEntity == null)
            {
                throw new Exception( "构造IRAPTreeBase参数不正确，检查社区、树标识，LeafID是否正确！");
            }
            int i = 0;
            var list = _treeStatus.ToList();
            foreach (byte v in statusDict)
            {
                i++;
                var obj = list.FirstOrDefault(c => c.Ordinal == i);
                if (obj == null)
                {
                    TreeStatus c;
                    TreeStatus e = new TreeStatus
                    {
                        EntityID = _entityID,
                        Ordinal = i,
                        PartitioningKey = PK,
                        StatusValue = v,
                        T5LeafID = 0, //从模型里取
                        //TreeID = (short)_treeID   //从模型里取
                    };
                    if (_treeID <= 100)
                    {
                        c = e.CopyTo<ETreeSysStatus>();
                    }
                    else
                    {
                        c = e.CopyTo<ETreeBizStatus>();
                    }
                    TableSet(c).Add(c);
                }
                else
                {
                    obj.StatusValue = v;
                }
            }
            _db.SaveChanges();
            return new IRAPError(0, "状态属性保存成功");
        }

        /// <summary>
        /// 保存和修改指定的状态属性
        /// </summary>
        /// <param name="ordinal">状态属性位置序号</param>
        ///  <param name="statusValue">状态属性值</param>
        /// <returns>通用错误IRAPError</returns>
        public IRAPError SaveStatusAttr(int ordinal, byte statusValue)
        {
            if (_leafID == 0 || _leafEntity == null)
            {
                throw new Exception( "构造IRAPTreeBase参数不正确，检查社区、树标识，LeafID是否正确！");
            }
            var obj = _treeStatus.FirstOrDefault(c => c.Ordinal == ordinal);
            if (obj == null)
            {
                TreeStatus c;
                TreeStatus e = new TreeStatus
                {
                    EntityID = _entityID,
                    Ordinal = ordinal,
                    PartitioningKey = PK,
                    StatusValue = statusValue,
                    T5LeafID = 0, //从模型里取
                    //TreeID = (short)_treeID   //从模型里取
                };
                if (_treeID <= 100)
                {
                    c = e.CopyTo<ETreeSysStatus>();
                }
                else
                {
                    c = e.CopyTo<ETreeBizStatus>();
                }
                TableSet(c).Add(c);
            }
            else
            {
                obj.StatusValue = statusValue;
            }
            _db.SaveChanges();
            return new IRAPError(0, "状态属性保存成功");
        }
        /// <summary>
        /// 保存和修改瞬态属性
        /// </summary>
        /// <param name="dict">瞬态属性值，传参顺序表示瞬态属性序号</param>
        /// <returns>通用错误IRAPError</returns>
        public IRAPError SaveTransAttr(params long[] dict)
        {
            if (_leafID == 0 || _leafEntity == null)
            {
                throw new Exception("构造IRAPTreeBase参数不正确，检查社区、树标识，LeafID是否正确！");
            }
            int i = 0;
            var list = _treeTrans.ToList();
            foreach (long v in dict)
            {
                i++;
                var obj = list.FirstOrDefault(c => c.Ordinal == i);
                if (obj == null)
                {
                    TreeTransient c;
                    TreeTransient e = new TreeTransient
                    {
                        EntityID = _entityID,
                        Ordinal = (byte)i,
                        RowChecksum = 0,
                        VersionGE = 0,
                        VersionLE = (int)Math.Pow(2, 31) - 1,
                        PartitioningKey = PK,
                        AttrValue = v,
                        //TreeID = _treeID  //从模型里取
                    };
                    if (_treeID <= 100)
                    {
                        c = e.CopyTo<ETreeSysTran>();
                    }
                    else
                    {
                        c = e.CopyTo<ETreeBizTran>();
                    }
                    TableSet(c).Add(c);

                }
                else
                {
                    obj.AttrValue = v;
                }
            }
            _db.SaveChanges();
            return new IRAPError(0, "瞬态属性保存成功");
        }

        /// <summary>
        /// 保存和修改瞬态属性
        /// </summary>
        /// <param name="ordinal">瞬态属性序号</param>
        /// <param name="transValue">瞬态属性值</param>
        /// <returns>通用错误IRAPError</returns>
        public IRAPError SaveTransAttr(int ordinal, long transValue)
        {
            if (_leafID == 0 || _leafEntity == null)
            {
                throw new Exception( "构造IRAPTreeBase参数不正确，检查社区、树标识，LeafID是否正确！");
            }
            var obj = _treeTrans.FirstOrDefault(c => c.Ordinal == ordinal);
            if (obj == null)
            {
                TreeTransient c;
                TreeTransient e = new TreeTransient
                {
                    EntityID = _entityID,
                    Ordinal = (byte)ordinal,
                    RowChecksum = 0,
                    VersionGE = 0,
                    VersionLE = (int)Math.Pow(2, 31) - 1,
                    PartitioningKey = PK,
                    AttrValue = transValue,
                    //TreeID = _treeID  //从模型里取
                };
                if (_treeID <= 100)
                {
                    c = e.CopyTo<ETreeSysTran>();
                }
                else
                {
                    c = e.CopyTo<ETreeBizTran>();
                }
                TableSet(c).Add(c);

            }
            else
            {
                obj.AttrValue = transValue;
            }
            _db.SaveChanges();
            return new IRAPError(0, "瞬态属性保存成功！");
        }
        /// <summary>
        /// 保存和修改一般属性
        /// </summary>
        /// <param name="e">一般属性的实体</param>
        /// <returns>通用错误IRAPError</returns>
        public IRAPError SaveGenAttr(BaseGenAttrEntity e)
        {
            if (_leafID == 0 || _leafEntity == null)
            {
                throw new Exception( "构造IRAPTreeBase参数不正确，检查社区、树标识，LeafID是否正确！");
            }
            //验证表对不对与数据字典比较
            e.PartitioningKey = PK;
            e.EntityID = _entityID;
            var genAttr = TableSet(e).Find(e.PartitioningKey, e.EntityID);
            if (genAttr == null)
            {
                TableSet(e).Add(e);
            }
            else
            {
                e.CopyTo((BaseGenAttrEntity)genAttr);
            }
            _db.SaveChanges();
            return new IRAPError(0, $"一般属性{e.GetType().Name}保存成功！");
        }
        /// <summary>
        /// 删除并保存行集属性，处理逻辑是：先删除后插入
        /// </summary>
        /// <param name="type">行集类型（为了list传空值时判断表名）</param>
        /// <param name="list">行集实体</param>
        /// <returns>通用错误IRAPError</returns>
        public IRAPError SaveRSAttr(Type type, List<BaseRowAttrEntity> list)
        {
            if (_leafID == 0 || _leafEntity == null)
            {
                throw new Exception( "构造IRAPTreeBase参数不正确，检查社区、树标识，LeafID是否正确！");
            }
            //IRAPSqlDBContext sql = new IRAPSqlDBContext();

            //if (list == null || list.Count == 0)
            //{
            //    error.ErrCode = 99;
            //    error.ErrText = "输入list没有记录，无法保存行集属性！";
            //    return error;
            //}
            //验证行集序号
            if (list != null && list.Count > 0)
            {
                if (list[0].GetType() != type)
                {
                    throw new Exception("输入list类型与type参数类型不符！");
                }
            }
            //先删除行集再插入
            string tblName = ObjectCopy.GetTBLName(type);
            if (tblName == "")
            {
                return new IRAPError(99, "行集属性表定义时未加入Table属性指定表名。"+ type.ToString());
            }
           
            var list2 = _db.GetSet(type).SqlQuery($"select * from {tblName} where PartitioningKey=@p and EntityID=@e",
                 new SqlParameter("@p", PK), new SqlParameter("@e", _entityID));
            foreach (BaseRowAttrEntity r in list2)
            {
                TableSet(r).Remove(r);
            }

            // Console.WriteLine("表名："+tblName);
            // _db.DataBase.ExecuteSqlCommand($"delete from {tblName} where PartitioningKey=@p and EntityID=@e",
            //    new SqlParameter("@p", PK), new SqlParameter("@e", _entityID));
            int i = 0;
            foreach (BaseRowAttrEntity r in list)
            {
                i++;
                r.Ordinal = i;
                r.PartitioningKey = PK;
                r.EntityID = _entityID;
                r.VersionGE = 0;
                r.VersionLE = (int)(Math.Pow(2, 31) - 1);
                TableSet(r).Add(r);
            }
            _db.SaveChanges();
            return new IRAPError(0, "行集属性保存成功！");
            //throw new NotImplementedException();
        }

        #endregion

        #region 获取其他属性
        /// <summary>
        /// 获取一般属性，所有一般属性必须继承BaseGenAttrEntity 使用者知道具体的子类型是什么
        /// </summary>
        /// <typeparam name="T">一般属性子类型</typeparam>
        /// <returns>一般属性实体可能为空</returns>
        public BaseGenAttrEntity GetGenAttr<T>() where T : BaseGenAttrEntity
        {
            //未来是否根据模型 创建一个子类型返回？
            return _db.Set<T>().FirstOrDefault(c => c.PartitioningKey == PK && c.EntityID == _entityID);
        }

        /// <summary>
        /// 获取行集属性，所有行集属性必须继承BaseRowAttrEntity 使用者知道具体的子类是什么
        /// </summary>
        /// <typeparam name="T">行集属性的子类型</typeparam>
        /// <returns>通用错误IRAPError</returns>
        public IQueryable<BaseRowAttrEntity> GetRSAttr<T>() where T : BaseRowAttrEntity
        {
            return _db.Set<T>().Where(c => c.PartitioningKey == PK && c.EntityID == _entityID).OrderBy(c => c.Ordinal);
        }
        /// <summary>
        /// 根据节点和叶子生成TreeView数据
        /// </summary>
        /// <returns></returns>
        public List<TreeViewDTO> GetTreeView()
        {
            List<TreeViewDTO> list = new List<TreeViewDTO>();
            var nodeSet = _nodes.OrderBy(c => c.NodeDepth).OrderBy(c => c.Father).ToList();
            foreach (TreeNodeEntity r in nodeSet)
            {
                TreeViewDTO item = new TreeViewDTO()
                {
                    Accessibility = 1,
                    AlternateCode = r.Code,
                    CSTRoot = r.CSTRoot,
                    NodeCode = r.Code,
                    NodeDepth = r.NodeDepth,
                    NodeID = r.NodeID,
                    NodeName = r.NodeName,
                    NodeStatus = r.NodeStatus,
                    NodeType = 3,
                    Parent = r.Father,
                    TreeViewType = 2,
                    UDFOrdinal = r.UDFOrdinal
                };
                list.Add(item);
                //找出此节点下面的叶子
                var leaf = _leaves.Where(c => c.Father == r.NodeID);
                foreach (TreeLeafEntity row in leaf)
                {
                    TreeViewDTO dto = new TreeViewDTO()
                    {
                        Accessibility = 1,
                        AlternateCode = row.AlternateCode,
                        CSTRoot = row.CSTRoot,
                        NodeCode = row.Code,
                        NodeDepth = row.NodeDepth,
                        NodeID = -row.LeafID,
                        NodeName = row.NodeName,
                        NodeStatus = (byte)row.LeafStatus,
                        NodeType = 4,
                        Parent = row.Father,
                        TreeViewType = 2,
                        UDFOrdinal = row.UDFOrdinal,
                    };
                    list.Add(dto);
                }
            }
            return list;
        }

        //获取一个叶子的相关所有属性（列式 DTO）

        //获取一个叶子的相关所有属性（行式DTO）

        //保存一个叶子的所有属性（列式 依赖数据字典？）

        //保存一个叶子的所有属性（行式）

        //考虑根据分类属性入口查询树的情况 ？
        /// <summary>
        /// 获取第n级分类属性实体
        /// </summary>
        /// <param name="dimOrdinal">多级分类属性序号</param>
        /// <returns></returns>
        public TreeLeafEntity GetClassifyByOrdinal(params int[] dimOrdinal)
        {
            //必须知道模型才能查出来？
            int leafid = _leafID;
            int treeid = _treeID;
            foreach (int ord in dimOrdinal)
            {
                ClassifyDTO dto = GetClassifyEntity(treeid, leafid, ord);
                if (dto == null)
                {
                    throw new Exception($"树{treeid}的第{ord}分类属性未定义！");
                }
                else
                {
                    leafid = dto.A4LeafID;
                    treeid = dto.AttrTreeID;
                }
            }
            IDbContext db = null;
            if (treeid <= 100)
            {
                if (_db.DataBase.Connection.Database == "IRAPMDM")
                {
                    db = DBContextFactory.Instance.CreateContext("IRAPContext");
                }
                else
                {
                    db = _db;
                }
            }
            else
            {
                db = DBContextFactory.Instance.CreateContext("IRAPMDMContext");
            }
            IRAPTreeBase treebase = new IRAPTreeBase(db, _communityID, treeid, leafid);
            return treebase.Leaf;
        }

        private ClassifyDTO GetClassifyEntity(int treeID, int leafID, int ordinal)
        {
            IDbContext db = null;
            if (treeID <= 100)
            {
                db = DBContextFactory.Instance.CreateContext("IRAPContext");
            }
            else
            {
                db = DBContextFactory.Instance.CreateContext("IRAPMDMContext");
            }

            IRAPTreeBase treeBase = new IRAPTreeBase(db, _communityID, treeID, leafID);
            if (treeBase.ClassifyDict.ContainsKey(ordinal))
            {
                return treeBase.ClassifyDict[ordinal];
            }
            else
            {
                return null;
            }
        }


        #endregion
        /// <summary>
        /// 提交事务 以后不再使用，WebAPI方法中请使用IRAPBizBase中的Commit提交
        /// </summary>
        [Obsolete]
        public void Commit()
        {
            _db.SaveChanges();
        }


    }



}
