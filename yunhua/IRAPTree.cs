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
using System.Reflection;
using IRAPBase.Enums;

namespace IRAPBase
{

    #region 废弃的类
    /*
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

    // }*/

    #endregion
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
        protected IRAPTreeModel _treeModel = null;
        private long[] _PKDict;
        protected List<TreeClassifyModelDTO> _treeClassModel = null;
        protected List<TreeTransientModelDTO> _treeTransientModel = null;
        protected List<TreeStatusModelDTO> _treeStatusModel = null;

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
        public IRAPTreeBase(IDbContext db, int communityID, int treeID, int leafID)
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
            else if (_treeID>100 && _treeID <= 1000)
            {
                _nodes = new Repository<ETreeBizDir>(_db).Table.Where(c => _PKDict.Contains(c.PartitioningKey) && c.TreeID == _treeID);
                //  _leaves = new Repository<ETreeBizLeaf>(_db).Table;
                _leaves = _db.Set<ETreeBizLeaf>().Where(c => c.PartitioningKey == PK && c.TreeID == _treeID);
            }
            else
            {
                _nodes = new Repository<ETreeBizDir>(_db).Table.Where(c => _PKDict.Contains(c.PartitioningKey) && c.TreeID == _treeID);
                //  _leaves = new Repository<ETreeBizLeaf>(_db).Table;
                _leaves = _db.Set<ETreeRichLeaf>().Where(c => c.PartitioningKey == PK && c.TreeID == _treeID);
            }
            //加载树模型
            _treeModel = new IRAPTreeModel(_treeID);
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


        #region 删除属性

        #endregion


        /// <summary>
        /// 设置树的叶状态
        /// </summary>
        /// <param name="leafStatus"></param>
        /// <returns></returns>
        public IRAPError SetLeafStatus(int leafStatus)
        {

            if (_leafID == 0 || _leafEntity == null)
            {
                throw new Exception("构造IRAPTreeBase参数不正确，检查社区、树标识，LeafID是否正确！");
            }
            _leafEntity.LeafStatus = (short)leafStatus;
            _db.SaveChanges();
            return new IRAPError(0, "设置成功！");
        }

        #region 获取其他属性
        /// <summary>
        /// 获取一般属性，所有一般属性必须继承BaseGenAttrEntity 使用者知道具体的子类型是什么
        /// </summary>
        /// <typeparam name="T">一般属性子类型</typeparam>
        /// <returns>一般属性实体可能为空</returns>
        public BaseGenAttrEntity GetGenAttr<T>() where T : BaseGenAttrEntity
        {
            //未来是否根据模型 创建一个子类型返回？
            var obj = _db.Set<T>().FirstOrDefault(c => c.PartitioningKey == PK && c.EntityID == _entityID);
            if (obj == null)
            {
                throw new Exception("一般属性没有找到记录！");
            }
            return obj;
        }

        /// <summary>
        /// 获取行集属性，所有行集属性必须继承BaseRowAttrEntity 使用者知道具体的子类是什么
        /// </summary>
        /// <typeparam name="T">行集属性的子类型</typeparam>
        /// <returns>通用错误IRAPError</returns>
        public IQueryable<BaseRowAttrEntity> GetRSAttr<T>() where T : BaseRowAttrEntity
        {
            return _db.Set<T>().Where(c => c.PartitioningKey == PK && c.EntityID == _entityID).AsNoTracking().OrderBy(c => c.Ordinal);
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
                    Accessibility = AccessibilityType.Radio,
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
                        Accessibility = AccessibilityType.Radio,
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

        /// <summary>
        /// 保存一棵树所有属性(除行集属性外)
        /// </summary>
        /// <param name="paramList">属性列表</param>
        /// <returns></returns>
        public IRAPError SaveAllAttr(List<SaveAttrInputDTO> paramList)
        {
            //保存标识属性AttrType=1

            //保存目录属性AttrType=2

            //保存检索属性AttrType=3

            //保存分类属性AttrType=4

            //保存状态属性AttrType=5

            //保存瞬态属性AttrType=6

            //保存一般属性AttrType=7

            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取树所有属性（从模型中获取）
        /// </summary>
        /// <returns></returns>
        public List<IRAPTreeAttrDTO> GetAllAttr(int communityID = 0)
        {
            //如果CommunityID =0 则会从模型中获取所有属性，否则从 173中获取
            //是从173取，还是从模型取？
            throw new NotImplementedException();

        }

        #region 保存分类、状态、瞬态、一般、行集属性
        /// <summary>
        /// 保存或修改分类属性
        /// </summary>
        /// <param name="dict">分类属性字典</param>
        /// <returns>返回错误</returns>
        public IRAPError SaveClassAttr(params TreeClassifyDTO[] dict)
        {
            if (_leafID == 0 || _leafEntity == null)
            {
                throw new Exception("构造IRAPTreeBase参数不正确，检查社区、树标识，LeafID是否正确！");
            }
            var list = _treeClass.ToList();
            if (_treeClassModel == null)
            {
                _treeClassModel = _treeModel.GetClassify();
            }
            int currUnixTime = (int)IRAPCommon.UnixTime.LocalTimeToUnix(DateTime.Now, 8);
            foreach (TreeClassifyDTO item in dict)
            {
                var cModel = _treeClassModel.FirstOrDefault(c => c.AttrIndex == item.Ordinal);
                if (cModel == null)
                {
                    throw new Exception($"第 {_treeID} 棵树的第 {item.Ordinal} 分类属性未定义，请先使用IRAPTreeModel类定义！");
                }
                //验证分类属性有效性TreeID 和 LeafID 在系统中不存在的情况
                TreeLeafEntity thisEntity = null;
                if (item.DimLeaf != 0)
                {
                    thisEntity = IRAPTreeSet.GetLeafEntity(_communityID, cModel.AttrTreeID, item.DimLeaf);
                    if (thisEntity == null)
                    {
                        throw new Exception($"第{item.Ordinal}分类属性TreeID:{cModel.AttrTreeID} LeafID:{item.DimLeaf}在系统中不存在！");
                    }
                }
                TreeClassEntity ch;
                if (list.Exists(c => c.Ordinal == item.Ordinal))
                {
                    var cEntity = list.FirstOrDefault(c => c.Ordinal == item.Ordinal);
                    cEntity.A4LeafID = item.DimLeaf;
                    cEntity.A4Code = thisEntity == null ? "" : thisEntity.Code;
                    cEntity.A4NodeName = thisEntity == null ? "" : thisEntity.NodeName;
                    cEntity.A4AlternateCode = thisEntity == null ? "" : thisEntity.AlternateCode;
                    cEntity.A4DescInEnglish = thisEntity == null ? "" : thisEntity.DescInEnglish;
                    cEntity.AttrTreeID = (short)cModel.AttrTreeID;
                    cEntity.TransactNoLE = 9223372036854775807;
                    cEntity.CSTRoot = thisEntity == null ? 0 : thisEntity.CSTRoot;
                    cEntity.NodeDepth = thisEntity == null ? (byte)0 : thisEntity.NodeDepth;
                    cEntity.MDMLogID = thisEntity == null ? 0L : thisEntity.MDMLogID;
                    cEntity.VersionGE = currUnixTime + 1;
                    if (_treeID <= 100)
                    {
                        ch = cEntity.CopyTo<ETreeSysClass_H>();
                    }
                    else
                    {
                        ch = cEntity.CopyTo<ETreeBizClass_H>();
                    }
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
                        A4Code = thisEntity == null ? "" : thisEntity.Code,
                        A4NodeName = thisEntity == null ? "" : thisEntity.NodeName,
                        A4AlternateCode = thisEntity == null ? "" : thisEntity.AlternateCode,
                        A4DescInEnglish = thisEntity == null ? "" : thisEntity.DescInEnglish,
                        AttrTreeID = (short)cModel.AttrTreeID,
                        VersionLE = (int)(Math.Pow(2, 31) - 1),
                        TransactNoLE = 9223372036854775807,
                        CSTRoot = thisEntity == null ? 0 : thisEntity.CSTRoot,
                        NodeDepth = thisEntity == null ? (byte)0 : thisEntity.NodeDepth,
                        MDMLogID = thisEntity == null ? 0L : thisEntity.MDMLogID
                        //TreeID = (short)_treeID, // (short)item.TreeID //根据模型填写
                    };
                    if (_treeID <= 100)
                    {
                        c = e.CopyTo<ETreeSysClass>();
                        ch = e.CopyTo<ETreeSysClass_H>();
                    }
                    else
                    {
                        c = e.CopyTo<ETreeBizClass>();
                        ch = e.CopyTo<ETreeBizClass_H>();
                    }
                    TableSet(c).Add(c);
                }

                #region  //处理分类属性历史
                TreeClassEntity tempCH;
                // long pk = _communityID * 10000L + cModel.AttrTreeID;
                if (_treeID <= 100)
                {
                    tempCH = _db.Set<ETreeSysClass_H>()
                        .FirstOrDefault(c => c.PartitioningKey == PK && c.LeafID == _leafID && c.VersionLE == 2147483647 && c.Ordinal == item.Ordinal);
                }
                else
                {
                    tempCH = _db.Set<ETreeBizClass_H>()
                        .FirstOrDefault(c => c.PartitioningKey == PK && c.LeafID == _leafID && c.VersionLE == 2147483647 && c.Ordinal == item.Ordinal);
                }
                if (tempCH == null)
                {
                    TableSet(ch).Add(ch);
                }
                else
                {
                    //先删除再插入
                    TreeClassEntity hisCH;
                    if (_treeID <= 100)
                    {
                        hisCH = tempCH.CopyTo<ETreeSysClass_H>();
                    }
                    else
                    {
                        hisCH = tempCH.CopyTo<ETreeBizClass_H>();
                    }
                    hisCH.VersionLE = currUnixTime;
                    //ch.VersionGE = hisCH.VersionLE + 1;
                    TableSet(tempCH).Remove(tempCH);
                    TableSet(hisCH).Add(hisCH);
                    TableSet(ch).Add(ch);
                }
                #endregion
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
            if (_treeClassModel == null)
            {
                _treeClassModel = _treeModel.GetClassify();
            }
            int currUnixTime = (int)IRAPCommon.UnixTime.LocalTimeToUnix(DateTime.Now, 8);
            foreach (int v in dict)
            {
                i++;
                var cModel = _treeClassModel.FirstOrDefault(c => c.AttrIndex == i);
                if (cModel == null)
                {
                    throw new Exception($"第 {_treeID} 棵树的第 {i} 分类属性未定义，请先使用IRAPTreeModel类定义！");
                }
                var thisEntity = IRAPTreeSet.GetLeafEntity(_communityID, cModel.AttrTreeID, v);
                if (thisEntity == null)
                {
                    throw new Exception($"第{i}分类属性TreeID:{cModel.AttrTreeID} LeafID:{v}在系统中不存在！");
                }
                var obj = list.FirstOrDefault(c => c.Ordinal == i);
                TreeClassEntity ch;
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
                        A4DescInEnglish = thisEntity.DescInEnglish,
                        AttrTreeID = (short)cModel.AttrTreeID,
                        TransactNoLE = 9223372036854775807,
                        CSTRoot = thisEntity.CSTRoot,
                        NodeDepth = thisEntity.NodeDepth,
                        MDMLogID = thisEntity.MDMLogID
                    };
                    if (_treeID <= 100)
                    {
                        c = e.CopyTo<ETreeSysClass>();
                        ch = e.CopyTo<ETreeSysClass_H>();
                    }
                    else
                    {
                        c = e.CopyTo<ETreeBizClass>();
                        ch = e.CopyTo<ETreeBizClass_H>();
                    }
                    TableSet(c).Add(c);
                }
                else
                {

                    obj.A4LeafID = v;
                    obj.CSTRoot = thisEntity.CSTRoot;
                    obj.NodeDepth = thisEntity.NodeDepth;
                    obj.MDMLogID = thisEntity.MDMLogID;
                    obj.A4Code = thisEntity.Code;
                    obj.A4NodeName = thisEntity.NodeName;
                    obj.A4AlternateCode = thisEntity.AlternateCode;
                    obj.A4DescInEnglish = thisEntity.DescInEnglish;
                    obj.AttrTreeID = (short)cModel.AttrTreeID;
                    obj.VersionGE = currUnixTime + 1;
                    if (_treeID <= 100)
                    {
                        ch = obj.CopyTo<ETreeSysClass_H>();
                    }
                    else
                    {
                        ch = obj.CopyTo<ETreeBizClass_H>();
                    }
                }
                //处理历史表的问题
                TreeClassEntity tempCH;
                // long pk = _communityID * 10000L + cModel.AttrTreeID;
                if (_treeID <= 100)
                {
                    tempCH = _db.Set<ETreeSysClass_H>()
                        .FirstOrDefault(c => c.PartitioningKey == PK && c.LeafID == _leafID && c.VersionLE == 2147483647 && c.Ordinal == i);
                }
                else
                {
                    tempCH = _db.Set<ETreeBizClass_H>()
                        .FirstOrDefault(c => c.PartitioningKey == PK && c.LeafID == _leafID && c.VersionLE == 2147483647 && c.Ordinal == i);
                }

                if (tempCH == null)
                {
                    TableSet(ch).Add(ch);
                }
                else
                {
                    //先删除再插入
                    TreeClassEntity hisCH;
                    if (_treeID <= 100)
                    {
                        hisCH = tempCH.CopyTo<ETreeSysClass_H>();
                    }
                    else
                    {
                        hisCH = tempCH.CopyTo<ETreeBizClass_H>();
                    }
                    hisCH.VersionLE = currUnixTime;
                    //ch.VersionGE = hisCH.VersionLE + 1;
                    TableSet(tempCH).Remove(tempCH);
                    TableSet(hisCH).Add(hisCH);
                    TableSet(ch).Add(ch);
                }
            }
            _db.SaveChanges();
            return new IRAPError(0, "分类属性保存成功");
        }

        /// <summary>
        /// 保存或修改分类属性(允许设置分类属性为0)
        /// </summary>
        /// <param name="ordinal">分类属性序号</param>
        /// <param name="dimLeaf">分类属性值</param>
        /// <returns></returns>
        public IRAPError SaveClassAttr(int ordinal, int dimLeaf)
        {
            if (ordinal > 254)
            {
                throw new Exception("分类属性序列超过254，请确认调用的重载方法是否正确！");
            }
            if (_leafID == 0 || _leafEntity == null)
            {
                throw new Exception("构造IRAPTreeBase参数不正确，检查社区、树标识，LeafID是否正确！");
            }
            if (_treeClassModel == null)
            {
                _treeClassModel = _treeModel.GetClassify();
            }
            var cModel = _treeClassModel.FirstOrDefault(c => c.AttrIndex == ordinal);
            if (cModel == null)
            {
                throw new Exception($"第 {_treeID} 棵树的第 {ordinal} 分类属性未定义，请先使用IRAPTreeModel类定义！");
            }
            TreeLeafEntity thisEntity = null;
            if (dimLeaf!=0)
            {
                thisEntity = IRAPTreeSet.GetLeafEntity(_communityID, cModel.AttrTreeID, dimLeaf);
                if (thisEntity == null)
                {
                    throw new Exception($"保存的分类属性：TreeID={cModel.AttrTreeID},LeafID={dimLeaf}不存在！");
                }
            }
     
            int currUnixTime = (int)IRAPCommon.UnixTime.LocalTimeToUnix(DateTime.Now, 8);
            TreeClassEntity dimobj = _treeClass.FirstOrDefault(r => r.Ordinal == ordinal);
            TreeClassEntity h;
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
                    A4Code = thisEntity==null?"":  thisEntity.Code,
                    A4NodeName = thisEntity == null ? "" : thisEntity.NodeName,
                    A4AlternateCode = thisEntity == null ? "" : thisEntity.AlternateCode,
                    A4DescInEnglish = thisEntity == null ? "" : thisEntity.DescInEnglish,
                    AttrTreeID = (short)cModel.AttrTreeID,
                    TransactNoLE = 9223372036854775807,
                    CSTRoot = thisEntity == null ? 0 : thisEntity.CSTRoot,
                    NodeDepth = thisEntity == null ? (byte)0 : thisEntity.NodeDepth,
                    MDMLogID = thisEntity == null ? 0L : thisEntity.MDMLogID
                };
                if (_treeID <= 100)
                {
                    c = e.CopyTo<ETreeSysClass>();
                    h = e.CopyTo<ETreeSysClass_H>();
                }
                else
                {
                    c = e.CopyTo<ETreeBizClass>();
                    h = e.CopyTo<ETreeBizClass_H>();
                }
                TableSet(c).Add(c);
            }
            else
            {
                dimobj.A4Code = thisEntity == null ? "" : thisEntity.Code;
                dimobj.A4NodeName = thisEntity == null ? "" : thisEntity.NodeName;
                dimobj.A4AlternateCode = thisEntity == null ? "" : thisEntity.AlternateCode;
                dimobj.A4DescInEnglish = thisEntity == null ? "" : thisEntity.DescInEnglish;
                dimobj.A4LeafID = dimLeaf;
                dimobj.AttrTreeID = (short)cModel.AttrTreeID;
                dimobj.TransactNoLE = 9223372036854775807;
                dimobj.VersionLE = (int)(Math.Pow(2, 31) - 1);
                dimobj.CSTRoot = thisEntity == null ? 0 : thisEntity.CSTRoot;
                dimobj.NodeDepth = thisEntity == null ? (byte)0 : thisEntity.NodeDepth;
                dimobj.MDMLogID = thisEntity == null ? 0L : thisEntity.MDMLogID;
                dimobj.VersionGE = currUnixTime + 1;
                if (_treeID <= 100)
                {
                    h = dimobj.CopyTo<ETreeSysClass_H>();
                }
                else
                {
                    h = dimobj.CopyTo<ETreeBizClass_H>();
                }
            }
            //处理历史表问题
            TreeClassEntity tempCH;
            if (_treeID <= 100)
            {
                tempCH = _db.Set<ETreeSysClass_H>()
                    .FirstOrDefault(c => c.PartitioningKey == PK && c.LeafID == _leafID && c.VersionLE == 2147483647 && c.Ordinal == ordinal);
            }
            else
            {
                tempCH = _db.Set<ETreeBizClass_H>()
                    .FirstOrDefault(c => c.PartitioningKey == PK && c.LeafID == _leafID && c.VersionLE == 2147483647 && c.Ordinal == ordinal);
            }
            if (tempCH == null)
            {
                TableSet(h).Add(h);
            }
            else
            {
                //先删除再插入
                TreeClassEntity hisCH;
                if (_treeID <= 100)
                {
                    hisCH = tempCH.CopyTo<ETreeSysClass_H>();
                }
                else
                {
                    hisCH = tempCH.CopyTo<ETreeBizClass_H>();
                }
                hisCH.VersionLE = currUnixTime;
                // h.VersionGE = hisCH.VersionLE + 1;
                TableSet(tempCH).Remove(tempCH);
                TableSet(hisCH).Add(hisCH);
                TableSet(h).Add(h);
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
                throw new Exception("构造IRAPTreeBase参数不正确，检查社区、树标识，LeafID是否正确！");
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
                throw new Exception("构造IRAPTreeBase参数不正确，检查社区、树标识，LeafID是否正确！");
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
                throw new Exception("构造IRAPTreeBase参数不正确，检查社区、树标识，LeafID是否正确！");
            }
            if (_treeTransientModel == null)
            {
                _treeTransientModel = _treeModel.GetTransient();
            }

            var cMode = _treeTransientModel.FirstOrDefault(c => c.AttrIndex == ordinal);
            if (cMode == null)
            {
                throw new Exception($"第{_treeID}棵树的模型中第 {ordinal} 瞬态属性未定义");
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
                    Scale = cMode.Scale,
                    UnitOfMeasure = cMode.UnitOfMeasure
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
                obj.Scale = cMode.Scale;
                obj.UnitOfMeasure = cMode.UnitOfMeasure;
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
                throw new Exception("构造IRAPTreeBase参数不正确，检查社区、树标识，LeafID是否正确！");
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
                throw new Exception("构造IRAPTreeBase参数不正确，检查社区、树标识，LeafID是否正确！");
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
                return new IRAPError(99, "行集属性表定义时未加入Table属性指定表名。" + type.ToString());
            }

            bool isOutTran = false;
            DbContextTransaction trans = null;
            if (_db.DataBase.CurrentTransaction == null)
            {
                trans = _db.DataBase.BeginTransaction();
            }
            else
            {
                isOutTran = true;
                trans = _db.DataBase.CurrentTransaction;
            }
            // using (var trans = _db.DataBase.BeginTransaction())
            //{
            try
            {
                int backCnt = _db.DataBase.ExecuteSqlCommand($"delete from {tblName} where PartitioningKey=@p and EntityID=@e",
                     new SqlParameter("@p", PK), new SqlParameter("@e", _entityID));
                // Console.WriteLine("表名："+tblName);
                // _db.DataBase.ExecuteSqlCommand($"delete from {tblName} where PartitioningKey=@p and EntityID=@e",
                //    new SqlParameter("@p", PK), new SqlParameter("@e", _entityID));
                int i = 0;
                foreach (BaseRowAttrEntity r in list)
                {
                    BaseRowAttrEntity trueRow = (BaseRowAttrEntity)Activator.CreateInstance(r.GetType());
                    r.CopyTo(trueRow);
                    i++;
                    trueRow.Ordinal = i;
                    trueRow.PartitioningKey = PK;
                    trueRow.EntityID = _entityID;
                    trueRow.VersionGE = 0;
                    trueRow.VersionLE = (int)(Math.Pow(2, 31) - 1);
                    TableSet(trueRow).Add(trueRow);
                }
                _db.SaveChanges();
                if (!isOutTran)
                {
                    trans.Commit();
                }
            }
            catch (Exception err)
            {
                Console.WriteLine($"保存行集属{tblName}性异常：{err.Message}");
                trans.Rollback();
            }
            // }
            return new IRAPError(0, $"行集属性{tblName}保存成功！");
            //throw new NotImplementedException();
        }

        #endregion

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


        //右键菜单定义

       
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
